#!/usr/bin/env python3
"""아트 채점기 — art_tokens.json 기준으로 픽셀 속성을 잰다.

  python3 Tools/check_art.py --tokens Docs/Portfolio/assets/concept-to-ui/art_tokens.json
  옵션 --root Assets/_Project  --json <out.json>  --fail-only

재는 것: 불투명부 명도(중앙값) · 바닥 대비 · 표시 크기에서의 엣지 강도 · 불투명 비율 · 색상 대역
재지 않는 것: 화풍, 구도, 취향. 그건 사람이 본다.
종료 코드 0=PASS 1=FAIL
"""
import argparse, json, glob, os, sys, io, fnmatch, colorsys
import numpy as np
from PIL import Image, ImageFilter

def luma(a):
    return 0.299*a[...,0] + 0.587*a[...,1] + 0.114*a[...,2]

def contrast_ratio(l1, l2):
    a, b = (max(l1,l2)+12.75), (min(l1,l2)+12.75)
    return a/b

def measure(path, display_px):
    im = Image.open(path).convert("RGBA")
    a = np.array(im).astype(float)
    alpha = a[...,3]
    opaque = alpha > 200
    ratio = float(opaque.mean())
    if opaque.sum() == 0:
        return None
    L = luma(a)
    med = float(np.median(L[opaque]))
    rgb = a[...,:3][opaque].mean(axis=0)/255.0
    hue = colorsys.rgb_to_hsv(*rgb)[0]*360
    sat = colorsys.rgb_to_hsv(*rgb)[1]*100
    t = im.convert("RGB").copy()
    t.thumbnail((display_px, display_px))
    edge = float(np.array(t.convert("L").filter(ImageFilter.FIND_EDGES)).mean())
    return {"medianLuma": med, "opaqueRatio": ratio, "hue": hue, "sat": sat,
            "edgeAtDisplay": edge, "size": im.size}

def classify(rel, classes):
    for name, c in classes.items():
        for pat in c["match"]:
            if fnmatch.fnmatch(rel, pat) or fnmatch.fnmatch(rel, "*/"+pat):
                return name
    return None

def main():
    p = argparse.ArgumentParser()
    p.add_argument("--tokens", required=True)
    p.add_argument("--root", default="Assets/_Project")
    p.add_argument("--json")
    p.add_argument("--fail-only", action="store_true")
    ar = p.parse_args()

    T = json.load(io.open(ar.tokens, encoding="utf-8"))
    Mz = T.get("measurable", T)
    ground = Mz["ground"]["panelBg"]["luma"] if isinstance(Mz["ground"]["panelBg"], dict) else 19.0
    classes = Mz["classes"]
    pal = Mz["palette"]

    results, hues = [], []
    for f in sorted(glob.glob(os.path.join(ar.root, "**", "*.png"), recursive=True)):
        rel = f.replace(ar.root.rstrip("/")+"/", "")
        cls = classify(rel, classes)
        if not cls:
            continue
        c = classes[cls]
        m = measure(f, c["displayPx"])
        if m is None:
            results.append((rel, cls, "FAIL", ["불투명 픽셀 없음"], {})); continue
        bad = []
        if "minMedianLuma" in c and m["medianLuma"] < c["minMedianLuma"]:
            bad.append(f"명도 중앙 {m['medianLuma']:.1f} < {c['minMedianLuma']}")
        cr = contrast_ratio(m["medianLuma"], ground)
        if "minContrastVsGround" in c and cr < c["minContrastVsGround"]:
            bad.append(f"바닥 대비 {cr:.2f} < {c['minContrastVsGround']}")
        if "minEdgeAtDisplay" in c and m["edgeAtDisplay"] < c["minEdgeAtDisplay"]:
            bad.append(f"{c['displayPx']}px 엣지 {m['edgeAtDisplay']:.1f} < {c['minEdgeAtDisplay']}")
        if "minOpaqueRatio" in c and m["opaqueRatio"] < c["minOpaqueRatio"]:
            bad.append(f"불투명 비율 {m['opaqueRatio']*100:.1f}% < {c['minOpaqueRatio']*100:.0f}%")
        m["contrastVsGround"] = cr
        if m["sat"] > 5:
            hues.append(m["hue"])
        results.append((rel, cls, "FAIL" if bad else "PASS", bad, m))

    npass = sum(1 for r in results if r[2] == "PASS")
    nfail = len(results) - npass
    by = {}
    for rel, cls, v, bad, m in results:
        by.setdefault(cls, [0,0])
        by[cls][0 if v=="PASS" else 1] += 1

    print("\n=== 클래스별 ===")
    for k in sorted(by):
        pz, fz = by[k]
        print(f"  {k:<12} PASS {pz:>3} · FAIL {fz:>3}")

    print("\n=== FAIL 목록 ===")
    for rel, cls, v, bad, m in results:
        if v != "FAIL":
            continue
        print(f"  [{cls}] {rel}")
        for b in bad:
            print(f"        - {b}")

    if not ar.fail_only:
        lo, hi = pal["primaryHueRange"]
        inr = sum(1 for h in hues if lo <= h <= hi)
        share = inr/len(hues) if hues else 0
        print(f"\n=== 색상 대역 {lo}~{hi}° ===")
        print(f"  {inr}/{len(hues)} = {share*100:.0f}%  (하한 {pal['primaryShareMin']*100:.0f}%)"
              f"  → {'PASS' if share >= pal['primaryShareMin'] else 'FAIL'}")

    print(f"\n판정: {'FAIL' if nfail else 'PASS'}   (PASS {npass} · FAIL {nfail})\n")
    if ar.json:
        json.dump([{"path":r[0],"class":r[1],"verdict":r[2],"violations":r[3],"metrics":r[4]} for r in results],
                  io.open(ar.json,"w",encoding="utf-8"), ensure_ascii=False, indent=2)
        print(f"상세: {ar.json}\n")
    return 1 if nfail else 0

if __name__ == "__main__":
    sys.exit(main())
