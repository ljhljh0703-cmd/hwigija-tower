#!/usr/bin/env python3
"""Build a small visual-hull voxel turnaround from front/side PNG silhouettes.

The tool intentionally avoids external dependencies so it can run in a fresh
game repo checkout. It is a validation prepass, not a final-art generator.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import math
import shutil
import struct
import sys
import zlib
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable


PNG_SIGNATURE = b"\x89PNG\r\n\x1a\n"
TOOL_VERSION = "0.1.0"


@dataclass(frozen=True)
class Image:
    width: int
    height: int
    pixels: list[tuple[int, int, int, int]]


@dataclass(frozen=True)
class ViewData:
    mask: list[list[bool]]
    colors: list[list[tuple[int, int, int, int]]]


DIRECTIONS = [
    ("front", 0),
    ("front_right", 45),
    ("right", 90),
    ("back_right", 135),
    ("back", 180),
    ("back_left", 225),
    ("left", 270),
    ("front_left", 315),
]


def paeth(a: int, b: int, c: int) -> int:
    p = a + b - c
    pa = abs(p - a)
    pb = abs(p - b)
    pc = abs(p - c)
    if pa <= pb and pa <= pc:
        return a
    if pb <= pc:
        return b
    return c


def read_png(path: Path) -> Image:
    data = path.read_bytes()
    if not data.startswith(PNG_SIGNATURE):
        raise ValueError(f"{path} is not a PNG file")

    pos = len(PNG_SIGNATURE)
    width = height = bit_depth = color_type = interlace = None
    idat = bytearray()

    while pos < len(data):
        if pos + 8 > len(data):
            raise ValueError(f"{path} has a truncated PNG chunk")
        length = struct.unpack(">I", data[pos : pos + 4])[0]
        chunk_type = data[pos + 4 : pos + 8]
        chunk_data = data[pos + 8 : pos + 8 + length]
        pos += 12 + length

        if chunk_type == b"IHDR":
            width, height, bit_depth, color_type, _compression, _filter, interlace = struct.unpack(
                ">IIBBBBB", chunk_data
            )
        elif chunk_type == b"IDAT":
            idat.extend(chunk_data)
        elif chunk_type == b"IEND":
            break

    if width is None or height is None:
        raise ValueError(f"{path} is missing IHDR")
    if bit_depth != 8:
        raise ValueError(f"{path} uses unsupported bit depth {bit_depth}; expected 8")
    if interlace != 0:
        raise ValueError(f"{path} uses interlacing; only non-interlaced PNG is supported")

    channels_by_type = {0: 1, 2: 3, 4: 2, 6: 4}
    if color_type not in channels_by_type:
        raise ValueError(f"{path} uses unsupported PNG color type {color_type}")
    channels = channels_by_type[color_type]
    stride = width * channels
    raw = zlib.decompress(bytes(idat))
    expected = height * (stride + 1)
    if len(raw) != expected:
        raise ValueError(f"{path} decoded to {len(raw)} bytes; expected {expected}")

    rows: list[bytearray] = []
    prev = bytearray(stride)
    offset = 0
    for _row in range(height):
        filter_type = raw[offset]
        scan = bytearray(raw[offset + 1 : offset + 1 + stride])
        offset += stride + 1

        recon = bytearray(stride)
        for i, value in enumerate(scan):
            left = recon[i - channels] if i >= channels else 0
            up = prev[i]
            up_left = prev[i - channels] if i >= channels else 0
            if filter_type == 0:
                recon[i] = value
            elif filter_type == 1:
                recon[i] = (value + left) & 0xFF
            elif filter_type == 2:
                recon[i] = (value + up) & 0xFF
            elif filter_type == 3:
                recon[i] = (value + ((left + up) // 2)) & 0xFF
            elif filter_type == 4:
                recon[i] = (value + paeth(left, up, up_left)) & 0xFF
            else:
                raise ValueError(f"{path} uses unsupported PNG filter {filter_type}")
        rows.append(recon)
        prev = recon

    pixels: list[tuple[int, int, int, int]] = []
    for row in rows:
        for x in range(width):
            i = x * channels
            if color_type == 0:
                g = row[i]
                pixels.append((g, g, g, 255))
            elif color_type == 2:
                pixels.append((row[i], row[i + 1], row[i + 2], 255))
            elif color_type == 4:
                g = row[i]
                pixels.append((g, g, g, row[i + 1]))
            elif color_type == 6:
                pixels.append((row[i], row[i + 1], row[i + 2], row[i + 3]))

    return Image(width, height, pixels)


def png_chunk(chunk_type: bytes, chunk_data: bytes) -> bytes:
    crc = zlib.crc32(chunk_type)
    crc = zlib.crc32(chunk_data, crc)
    return struct.pack(">I", len(chunk_data)) + chunk_type + chunk_data + struct.pack(">I", crc & 0xFFFFFFFF)


def write_png(path: Path, width: int, height: int, pixels: list[tuple[int, int, int, int]]) -> None:
    if len(pixels) != width * height:
        raise ValueError("pixel count does not match dimensions")
    path.parent.mkdir(parents=True, exist_ok=True)
    ihdr = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    raw = bytearray()
    for y in range(height):
        raw.append(0)
        for x in range(width):
            raw.extend(bytes(pixels[y * width + x]))
    payload = (
        PNG_SIGNATURE
        + png_chunk(b"IHDR", ihdr)
        + png_chunk(b"IDAT", zlib.compress(bytes(raw), 9))
        + png_chunk(b"IEND", b"")
    )
    path.write_bytes(payload)


def sha256_file(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


def clamp_channel(value: float) -> int:
    return max(0, min(255, int(round(value))))


def mix_rgba(a: tuple[int, int, int, int], b: tuple[int, int, int, int], amount_b: float) -> tuple[int, int, int, int]:
    amount_a = 1.0 - amount_b
    return (
        clamp_channel(a[0] * amount_a + b[0] * amount_b),
        clamp_channel(a[1] * amount_a + b[1] * amount_b),
        clamp_channel(a[2] * amount_a + b[2] * amount_b),
        clamp_channel(a[3] * amount_a + b[3] * amount_b),
    )


def darken(color: tuple[int, int, int, int], factor: float) -> tuple[int, int, int, int]:
    return (
        clamp_channel(color[0] * factor),
        clamp_channel(color[1] * factor),
        clamp_channel(color[2] * factor),
        color[3],
    )


def image_to_view(image: Image, resolution: int, alpha_threshold: int = 16) -> ViewData:
    corners = [
        image.pixels[0],
        image.pixels[image.width - 1],
        image.pixels[(image.height - 1) * image.width],
        image.pixels[-1],
    ]
    bg = tuple(sum(c[i] for c in corners) // len(corners) for i in range(3))
    has_alpha = any(pixel[3] < 250 for pixel in image.pixels)

    mask: list[list[bool]] = []
    colors: list[list[tuple[int, int, int, int]]] = []
    for y in range(resolution):
        src_y = min(image.height - 1, int((y + 0.5) * image.height / resolution))
        mask_row: list[bool] = []
        color_row: list[tuple[int, int, int, int]] = []
        for x in range(resolution):
            src_x = min(image.width - 1, int((x + 0.5) * image.width / resolution))
            r, g, b, a = image.pixels[src_y * image.width + src_x]
            luma = (r * 299 + g * 587 + b * 114) / 1000
            bg_dist = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
            if has_alpha:
                occupied = a > alpha_threshold
            else:
                occupied = bg_dist > 30 and luma < 250
            mask_row.append(occupied)
            color_row.append((r, g, b, 255 if occupied else 0))
        mask.append(mask_row)
        colors.append(color_row)

    return ViewData(mask, colors)


def build_voxel_set(front: ViewData, side: ViewData, resolution: int) -> set[tuple[int, int, int]]:
    voxels: set[tuple[int, int, int]] = set()
    for y in range(resolution):
        for x in range(resolution):
            if not front.mask[y][x]:
                continue
            for z in range(resolution):
                if side.mask[y][z]:
                    voxels.add((x, y, z))
    return voxels


def surface_voxels(voxels: set[tuple[int, int, int]]) -> list[tuple[int, int, int]]:
    offsets = [(1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1)]
    surface: list[tuple[int, int, int]] = []
    for voxel in voxels:
        x, y, z = voxel
        if any((x + dx, y + dy, z + dz) not in voxels for dx, dy, dz in offsets):
            surface.append(voxel)
    return surface


def bbox_for(voxels: Iterable[tuple[int, int, int]]) -> dict[str, list[int]]:
    values = list(voxels)
    if not values:
        return {"min": [0, 0, 0], "max": [0, 0, 0]}
    xs = [v[0] for v in values]
    ys = [v[1] for v in values]
    zs = [v[2] for v in values]
    return {"min": [min(xs), min(ys), min(zs)], "max": [max(xs), max(ys), max(zs)]}


def empty_canvas(width: int, height: int) -> list[tuple[int, int, int, int]]:
    return [(0, 0, 0, 0)] * (width * height)


def draw_square(
    pixels: list[tuple[int, int, int, int]],
    width: int,
    height: int,
    cx: int,
    cy: int,
    size: int,
    color: tuple[int, int, int, int],
) -> None:
    half = max(1, size // 2)
    for yy in range(cy - half, cy - half + size):
        if yy < 0 or yy >= height:
            continue
        row = yy * width
        for xx in range(cx - half, cx - half + size):
            if 0 <= xx < width:
                pixels[row + xx] = color


def render_direction(
    voxels: list[tuple[int, int, int]],
    front: ViewData,
    side: ViewData,
    resolution: int,
    angle_degrees: int,
    pixel_size: int,
) -> Image:
    theta = math.radians(angle_degrees)
    sin_t = math.sin(theta)
    cos_t = math.cos(theta)
    margin = pixel_size * 6
    canvas_w = int(math.ceil(resolution * pixel_size * 1.65)) + margin * 2
    canvas_h = resolution * pixel_size + margin * 2
    pixels = empty_canvas(canvas_w, canvas_h)
    center_x = canvas_w // 2
    grid_center = (resolution - 1) / 2

    def depth(voxel: tuple[int, int, int]) -> float:
        x, _y, z = voxel
        return (x - grid_center) * sin_t + (z - grid_center) * cos_t

    for x, y, z in sorted(voxels, key=depth):
        projected_u = (x - grid_center) * cos_t + (z - grid_center) * sin_t
        px = center_x + int(round(projected_u * pixel_size))
        py = margin + y * pixel_size

        fw = abs(cos_t)
        sw = abs(sin_t)
        total = max(0.001, fw + sw)
        side_amount = sw / total
        color = mix_rgba(front.colors[y][x], side.colors[y][z], side_amount)
        if cos_t < -0.1 or sin_t < -0.1:
            color = darken(color, 0.82)
        draw_square(pixels, canvas_w, canvas_h, px, py, pixel_size, color)

    return Image(canvas_w, canvas_h, pixels)


def paste_image(
    target: list[tuple[int, int, int, int]],
    target_width: int,
    target_height: int,
    source: Image,
    left: int,
    top: int,
) -> None:
    for y in range(source.height):
        yy = top + y
        if yy < 0 or yy >= target_height:
            continue
        for x in range(source.width):
            xx = left + x
            if xx < 0 or xx >= target_width:
                continue
            src = source.pixels[y * source.width + x]
            if src[3] > 0:
                target[yy * target_width + xx] = src


def write_contact_sheet(path: Path, images: list[Image], columns: int = 4, gap: int = 8) -> Image:
    if not images:
        raise ValueError("cannot build contact sheet without images")
    cell_w = max(image.width for image in images)
    cell_h = max(image.height for image in images)
    rows = math.ceil(len(images) / columns)
    width = columns * cell_w + (columns - 1) * gap
    height = rows * cell_h + (rows - 1) * gap
    pixels = empty_canvas(width, height)
    for idx, image in enumerate(images):
        col = idx % columns
        row = idx // columns
        left = col * (cell_w + gap) + (cell_w - image.width) // 2
        top = row * (cell_h + gap) + (cell_h - image.height) // 2
        paste_image(pixels, width, height, image, left, top)
    sheet = Image(width, height, pixels)
    write_png(path, width, height, pixels)
    return sheet


def nontransparent_count(image: Image) -> int:
    return sum(1 for pixel in image.pixels if pixel[3] > 0)


def generate_demo_source(out_dir: Path) -> tuple[Path, Path]:
    sources = out_dir / "sources"
    sources.mkdir(parents=True, exist_ok=True)
    front_path = sources / "front.png"
    side_path = sources / "side.png"
    width = height = 64
    transparent = (0, 0, 0, 0)

    def make_image(side_view: bool) -> Image:
        pixels = [transparent] * (width * height)
        for y in range(height):
            if y < 6 or y > 58:
                half = -1
            elif y < 17:
                progress = (y - 6) / 11
                half = int(3 + progress * (7 if not side_view else 4))
            elif y < 48:
                half = 12 if not side_view else 7
                if 22 <= y <= 28:
                    half += 3 if not side_view else 2
            elif y < 58:
                half = 19 if not side_view else 12
            else:
                half = -1

            for x in range(width):
                dx = abs(x - width // 2)
                if half >= 0 and dx <= half:
                    if dx > half - 2 or y in (47, 48):
                        color = (42, 39, 64, 255)
                    elif x < width // 2 - max(1, half // 3):
                        color = (154, 154, 176, 255)
                    elif x > width // 2 + max(1, half // 3):
                        color = (69, 68, 94, 255)
                    else:
                        color = (106, 106, 130, 255)
                    pixels[y * width + x] = color

            if not side_view and 20 <= y <= 42:
                core_half = 2 if y < 35 else 1
                for x in range(width // 2 - core_half, width // 2 + core_half + 1):
                    pixels[y * width + x] = (63, 212, 192, 255)

            if side_view and 25 <= y <= 39:
                for x in range(width // 2 - 1, width // 2 + 2):
                    pixels[y * width + x] = (31, 122, 140, 255)

        return Image(width, height, pixels)

    write_png(front_path, width, height, make_image(side_view=False).pixels)
    write_png(side_path, width, height, make_image(side_view=True).pixels)
    return front_path, side_path


def copy_sources(front_path: Path, side_path: Path, out_dir: Path) -> tuple[Path, Path]:
    sources = out_dir / "sources"
    sources.mkdir(parents=True, exist_ok=True)
    front_copy = sources / "front.png"
    side_copy = sources / "side.png"
    if front_path.resolve() != front_copy.resolve():
        shutil.copyfile(front_path, front_copy)
    if side_path.resolve() != side_copy.resolve():
        shutil.copyfile(side_path, side_copy)
    return front_copy, side_copy


def build_run(args: argparse.Namespace) -> int:
    out_dir = Path(args.out)
    out_dir.mkdir(parents=True, exist_ok=True)
    front_source, side_source = copy_sources(Path(args.front), Path(args.side), out_dir)

    front_image = read_png(front_source)
    side_image = read_png(side_source)
    front = image_to_view(front_image, args.resolution)
    side = image_to_view(side_image, args.resolution)
    voxels = build_voxel_set(front, side, args.resolution)
    surf = surface_voxels(voxels)
    renders_dir = out_dir / "renders"
    renders_dir.mkdir(parents=True, exist_ok=True)

    render_entries = []
    render_images: list[Image] = []
    for idx, (label, angle) in enumerate(DIRECTIONS):
        image = render_direction(surf, front, side, args.resolution, angle, args.pixel_size)
        render_images.append(image)
        rel_path = Path("renders") / f"dir_{idx:02d}_{label}.png"
        render_path = out_dir / rel_path
        write_png(render_path, image.width, image.height, image.pixels)
        render_entries.append(
            {
                "index": idx,
                "label": label,
                "angleDegrees": angle,
                "path": rel_path.as_posix(),
                "width": image.width,
                "height": image.height,
                "nonTransparentPixels": nontransparent_count(image),
                "sha256": sha256_file(render_path),
            }
        )

    sheet_path = out_dir / "turnaround_sheet.png"
    sheet = write_contact_sheet(sheet_path, render_images)
    manifest = {
        "tool": "voxel-turnaround",
        "toolVersion": TOOL_VERSION,
        "assetId": args.asset_id,
        "deterministic": True,
        "inputs": {
            "front": {"path": "sources/front.png", "sha256": sha256_file(front_source)},
            "side": {"path": "sources/side.png", "sha256": sha256_file(side_source)},
        },
        "parameters": {
            "resolution": args.resolution,
            "pixelSize": args.pixel_size,
            "directions": len(DIRECTIONS),
        },
        "voxel": {
            "occupiedVoxels": len(voxels),
            "surfaceVoxels": len(surf),
            "gridVoxels": args.resolution ** 3,
            "fillRatio": round(len(voxels) / (args.resolution ** 3), 6),
            "boundingBox": bbox_for(voxels),
        },
        "contactSheet": {
            "path": "turnaround_sheet.png",
            "width": sheet.width,
            "height": sheet.height,
            "nonTransparentPixels": nontransparent_count(sheet),
            "sha256": sha256_file(sheet_path),
        },
        "renders": render_entries,
        "notes": [
            "visual hull from orthographic front and side silhouettes",
            "output renders are reference sprites, not final production art",
            "final game import should still go through sprite-sheet + manifest pipeline",
        ],
    }
    (out_dir / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Built {args.asset_id}: {len(voxels)} occupied voxels, {len(surf)} surface voxels")
    print(f"Manifest: {out_dir / 'manifest.json'}")
    return 0


def demo_run(args: argparse.Namespace) -> int:
    out_dir = Path(args.out)
    out_dir.mkdir(parents=True, exist_ok=True)
    front_path, side_path = generate_demo_source(out_dir)
    build_args = argparse.Namespace(
        front=front_path,
        side=side_path,
        out=out_dir,
        asset_id=args.asset_id,
        resolution=args.resolution,
        pixel_size=args.pixel_size,
    )
    return build_run(build_args)


def verify_run(args: argparse.Namespace) -> int:
    run_dir = Path(args.run)
    manifest_path = run_dir / "manifest.json"
    failures: list[str] = []
    if not manifest_path.exists():
        print(f"FAIL missing {manifest_path}")
        return 1

    manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    occupied = int(manifest.get("voxel", {}).get("occupiedVoxels", 0))
    surface = int(manifest.get("voxel", {}).get("surfaceVoxels", 0))
    if occupied <= 0:
        failures.append("occupied voxel count is 0")
    if surface <= 0:
        failures.append("surface voxel count is 0")

    renders = manifest.get("renders", [])
    if len(renders) < args.min_directions:
        failures.append(f"render count {len(renders)} < expected {args.min_directions}")

    sheet = manifest.get("contactSheet")
    if not sheet:
        failures.append("missing contactSheet manifest entry")
    else:
        sheet_path = run_dir / sheet.get("path", "")
        if not sheet_path.exists():
            failures.append("missing contact sheet")
        else:
            actual_sha = sha256_file(sheet_path)
            if actual_sha != sheet.get("sha256"):
                failures.append("sha256 mismatch for contact sheet")
            try:
                image = read_png(sheet_path)
                if nontransparent_count(image) <= 0:
                    failures.append("contact sheet has no non-transparent pixels")
            except Exception as exc:  # noqa: BLE001
                failures.append(f"cannot read contact sheet: {exc}")

    for entry in renders:
        rel = entry.get("path")
        if not rel:
            failures.append("render entry has no path")
            continue
        path = run_dir / rel
        if not path.exists():
            failures.append(f"missing render {rel}")
            continue
        actual_sha = sha256_file(path)
        if actual_sha != entry.get("sha256"):
            failures.append(f"sha256 mismatch for {rel}")
        try:
            image = read_png(path)
        except Exception as exc:  # noqa: BLE001 - verifier should report all IO/format failures.
            failures.append(f"cannot read {rel}: {exc}")
            continue
        actual_nontransparent = nontransparent_count(image)
        if actual_nontransparent <= 0:
            failures.append(f"{rel} has no non-transparent pixels")
        if actual_nontransparent != entry.get("nonTransparentPixels"):
            failures.append(f"non-transparent pixel count mismatch for {rel}")

    if failures:
        print("FAIL voxel-turnaround verification")
        for failure in failures:
            print(f"- {failure}")
        return 1

    print("PASS voxel-turnaround verification")
    print(f"- occupied voxels: {occupied}")
    print(f"- surface voxels: {surface}")
    print(f"- renders checked: {len(renders)}")
    return 0


def parse_args(argv: list[str]) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="2D silhouette to voxel turnaround reference renderer")
    sub = parser.add_subparsers(dest="command", required=True)

    demo = sub.add_parser("demo", help="generate demo front/side sources, render 8 directions, and write manifest")
    demo.add_argument("--out", required=True, help="output run directory")
    demo.add_argument("--asset-id", default="demo_obelisk", help="asset id for manifest")
    demo.add_argument("--resolution", type=int, default=48, help="voxel grid resolution")
    demo.add_argument("--pixel-size", type=int, default=3, help="rendered pixels per voxel")
    demo.set_defaults(func=demo_run)

    build = sub.add_parser("build", help="build a voxel turnaround from front/side PNG inputs")
    build.add_argument("--front", required=True, help="front PNG path")
    build.add_argument("--side", required=True, help="side PNG path")
    build.add_argument("--out", required=True, help="output run directory")
    build.add_argument("--asset-id", required=True, help="asset id for manifest")
    build.add_argument("--resolution", type=int, default=64, help="voxel grid resolution")
    build.add_argument("--pixel-size", type=int, default=3, help="rendered pixels per voxel")
    build.set_defaults(func=build_run)

    verify = sub.add_parser("verify", help="verify a generated run directory")
    verify.add_argument("--run", required=True, help="run directory containing manifest.json")
    verify.add_argument("--min-directions", type=int, default=8, help="minimum expected render count")
    verify.set_defaults(func=verify_run)

    args = parser.parse_args(argv)
    if hasattr(args, "resolution") and not (8 <= args.resolution <= 160):
        parser.error("--resolution must be between 8 and 160")
    if hasattr(args, "pixel_size") and not (1 <= args.pixel_size <= 12):
        parser.error("--pixel-size must be between 1 and 12")
    return args


def main(argv: list[str]) -> int:
    args = parse_args(argv)
    return args.func(args)


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
