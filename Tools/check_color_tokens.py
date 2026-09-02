#!/usr/bin/env python3
"""UI/Lobby 색 토큰 소비를 검사한다.

G2가 필요한 이유: 레이아웃 계약은 UiTokenContract를 참조해도, 기존 런타임
표현 코드가 new Color/Color.white/직접 hex를 계속 쓰면 화면은 토큰 체계 밖에
남는다. 이 검사는 그 우회를 FAIL로 다룬다.
"""
import argparse
import json
import re
import sys
from pathlib import Path

HEX_VALUE = re.compile(r"^#[0-9A-Fa-f]{6}(?:[0-9A-Fa-f]{2})?$")
CONTRACT_HEX = re.compile(r'public\s+const\s+string\s+(\w+Hex)\s*=\s*"(#[0-9A-Fa-f]{6}(?:[0-9A-Fa-f]{2})?)"')
CONTRACT_ALPHA = re.compile(r"public\s+const\s+float\s+(\w+Alpha)\s*=\s*([0-9]+(?:\.[0-9]+)?)f?;")
NEW_COLOR = re.compile(r"\bnew\s+Color(?:32)?\s*\(")
BUILTIN_COLOR = re.compile(r"\bColor\.(?:red|green|blue|white|black|gray|grey|clear|yellow|cyan|magenta)\b")
HEX_STRING = re.compile(r'"(#[0-9A-Fa-f]{6}(?:[0-9A-Fa-f]{2})?)"')


def pascal_case(name: str) -> str:
    return "".join(part[:1].upper() + part[1:] for part in name.split("_"))


def line_of(text: str, index: int) -> int:
    return text.count("\n", 0, index) + 1


def report_matches(path: Path, text: str, pattern: re.Pattern, label: str) -> int:
    failures = 0
    for match in pattern.finditer(text):
        print(f"  [FAIL] {label} — {path}:{line_of(text, match.start())}: {match.group(0)}")
        failures += 1
    return failures


def load_token_data(path: Path) -> tuple[dict, dict]:
    try:
        payload = json.loads(path.read_text())
    except (OSError, json.JSONDecodeError) as error:
        raise ValueError(f"토큰 JSON을 읽지 못했다: {error}") from error

    colors = payload.get("color")
    if not isinstance(colors, dict) or not colors:
        raise ValueError("토큰 JSON의 color 객체가 없거나 비어 있다")

    for key, value in colors.items():
        if not isinstance(value, str) or not HEX_VALUE.fullmatch(value):
            raise ValueError(f"알 수 없는 color 입력: {key}={value!r}")
    opacities = payload.get("opacity", {})
    if not isinstance(opacities, dict):
        raise ValueError("토큰 JSON의 opacity 객체가 올바르지 않다")
    for key, value in opacities.items():
        if key.startswith("_"):
            continue
        if not isinstance(value, (int, float)) or isinstance(value, bool) or value < 0 or value > 1:
            raise ValueError(f"알 수 없는 opacity 입력: {key}={value!r}")
    return colors, {key: value for key, value in opacities.items() if not key.startswith("_")}


def load_contract_tokens(path: Path) -> tuple[dict, dict]:
    try:
        text = path.read_text()
    except OSError as error:
        raise ValueError(f"UiTokenContract를 읽지 못했다: {error}") from error

    colors = dict(CONTRACT_HEX.findall(text))
    alphas = {name: float(value) for name, value in CONTRACT_ALPHA.findall(text)}
    if not colors:
        raise ValueError("UiTokenContract hex 선언을 못 읽었다 — 검사 대상 누락은 통과가 아니다")
    return colors, alphas


DEFERRED_NOTE = (
    "이월(deferred) = 검사 범위 밖이지 통과가 아니다. 건수를 매번 찍어 눈에 남긴다.\n"
    "  2026-09-02 작가 판정: G2 범위를 신규 UI 모듈로 좁히고 모놀리스 잔여는 별건 백로그로 분리."
)


def load_deferred(path: Path) -> list[str]:
    if not path or not path.is_file():
        return []
    out = []
    for line in path.read_text(errors="ignore").splitlines():
        line = line.split("#", 1)[0].strip()
        if line:
            out.append(line)
    return out


def scan(roots: list[Path], tokens_path: Path, contract_path: Path, deferred_path: Path = None) -> int:
    missing_roots = [root for root in roots if not root.is_dir()]
    if missing_roots:
        for root in missing_roots:
            print(f"  [FAIL] 경로 없음 — {root}")
        return 1

    try:
        token_colors, token_alphas = load_token_data(tokens_path)
        contract_colors, contract_alphas = load_contract_tokens(contract_path)
    except ValueError as error:
        print(f"  [FAIL] 입력 — {error}")
        return 1

    failures = 0
    print("\n=== 색 토큰 검사 · " + ", ".join(str(root) for root in roots) + " ===")
    for token_name, value in sorted(token_colors.items()):
        constant = pascal_case(token_name) + "Hex"
        actual = contract_colors.get(constant)
        if actual == value:
            print(f"  [PASS] token:{token_name} — UiTokenContract.{constant}")
            continue
        failures += 1
        if actual is None:
            print(f"  [FAIL] token:{token_name} — UiTokenContract.{constant} 선언이 없다")
        else:
            print(f"  [FAIL] token:{token_name} — {actual} != {value}")

    for token_name, value in sorted(token_alphas.items()):
        constant = pascal_case(token_name) + "Alpha"
        actual = contract_alphas.get(constant)
        if actual is not None and abs(actual - float(value)) < 0.000001:
            print(f"  [PASS] opacity:{token_name} — UiTokenContract.{constant}")
            continue
        failures += 1
        if actual is None:
            print(f"  [FAIL] opacity:{token_name} — UiTokenContract.{constant} 선언이 없다")
        else:
            print(f"  [FAIL] opacity:{token_name} — {actual} != {value}")

    contract_resolved = contract_path.resolve()
    files = sorted(path for root in roots for path in root.rglob("*.cs"))
    if not files:
        print("  [FAIL] C# 파일이 없다 — 빈 검사 대상은 통과가 아니다")
        return 1

    deferred_names = load_deferred(deferred_path)
    deferred_files = [p for p in files if p.name in deferred_names]
    scoped_files = [p for p in files if p.name not in deferred_names]
    unknown = [n for n in deferred_names if not any(p.name == n for p in files)]
    if unknown:
        # 존재하지 않는 이름을 이월 목록에 두면 오타로 검사 범위가 조용히 넓어진다
        print(f"  [FAIL] deferred 목록에 없는 파일 {unknown} — 오타이거나 파일이 사라졌다")
        failures += 1

    literal_count = 0
    for path in scoped_files:
        try:
            text = path.read_text()
        except OSError as error:
            print(f"  [FAIL] 파일 읽기 — {path}: {error}")
            failures += 1
            continue

        literal_count += report_matches(path, text, NEW_COLOR, "literal:new-color")
        literal_count += report_matches(path, text, BUILTIN_COLOR, "literal:builtin-color")
        if path.resolve() != contract_resolved:
            literal_count += report_matches(path, text, HEX_STRING, "literal:hex")

    failures += literal_count
    if literal_count == 0:
        print(f"  [PASS] literal-color — 검사 범위 {len(scoped_files)}파일에서 0건")
    else:
        print(f"  [FAIL] literal-color — {literal_count}건")

    if deferred_files:
        deferred_count = 0
        for path in deferred_files:
            text = path.read_text(errors="ignore")
            deferred_count += (len(NEW_COLOR.findall(text)) + len(BUILTIN_COLOR.findall(text))
                               + len(HEX_STRING.findall(text)))
        print(f"\n  [DEFERRED] 범위 밖 {len(deferred_files)}파일 · 리터럴 {deferred_count}건 — "
              + ", ".join(p.name for p in deferred_files))
        for line in DEFERRED_NOTE.splitlines():
            print("             " + line)

    print("\n판정:", "PASS" if failures == 0 else "FAIL")
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--roots",
        nargs="+",
        default=["Assets/_Project/Scripts/UI", "Assets/_Project/Scripts/Lobby"],
    )
    parser.add_argument("--tokens", default="Docs/Portfolio/assets/concept-to-ui/ui_tokens.json")
    parser.add_argument("--contract", default="Assets/_Project/Scripts/Lobby/LobbyLayoutContract.cs")
    parser.add_argument("--deferred", default="Tools/color-token-deferred.txt")
    args = parser.parse_args()
    sys.exit(scan([Path(root) for root in args.roots], Path(args.tokens), Path(args.contract), Path(args.deferred)))
