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


def load_token_colors(path: Path) -> dict:
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
    return colors


def load_contract_colors(path: Path) -> dict:
    try:
        text = path.read_text()
    except OSError as error:
        raise ValueError(f"UiTokenContract를 읽지 못했다: {error}") from error

    colors = dict(CONTRACT_HEX.findall(text))
    if not colors:
        raise ValueError("UiTokenContract hex 선언을 못 읽었다 — 검사 대상 누락은 통과가 아니다")
    return colors


def scan(roots: list[Path], tokens_path: Path, contract_path: Path) -> int:
    missing_roots = [root for root in roots if not root.is_dir()]
    if missing_roots:
        for root in missing_roots:
            print(f"  [FAIL] 경로 없음 — {root}")
        return 1

    try:
        token_colors = load_token_colors(tokens_path)
        contract_colors = load_contract_colors(contract_path)
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

    contract_resolved = contract_path.resolve()
    files = sorted(path for root in roots for path in root.rglob("*.cs"))
    if not files:
        print("  [FAIL] C# 파일이 없다 — 빈 검사 대상은 통과가 아니다")
        return 1

    literal_count = 0
    for path in files:
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
        print("  [PASS] literal-color — 0건")
    else:
        print(f"  [FAIL] literal-color — {literal_count}건")

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
    args = parser.parse_args()
    sys.exit(scan([Path(root) for root in args.roots], Path(args.tokens), Path(args.contract)))
