#!/usr/bin/env python3
"""계약이 화면에 배선됐는지 검사한다.

왜 있는가 — 2026-09-01 전투 반려.
`combat_layout_spec_v2.json` ⟂ `CombatLayoutContract.cs` 대조가 27/27 PASS 였는데
그 계약을 읽는 런타임 파일이 하나도 없었다. 두 문서가 서로를 베낀 채 일치했고
화면은 구 레이아웃 그대로였다. 좌표 채점기(check_layout.py)로는 영원히 안 잡힌다.

검사 둘:
  W-01 계약 타입을 참조하는 비-테스트 런타임 파일이 1개 이상 있는가
  W-02 선언된 슬롯 중 실제로 참조되는 비율이 임계 이상인가 (부분 배선 검출)
"""
import re, sys, argparse
from pathlib import Path

TESTS = re.compile(r"[/\\]Tests[/\\]")
# 두 선언 형태를 모두 잡는다 — Slot("name",...) 팩토리형(휴식·전투)과 new XSlot("name",...) 생성자형(로비).
# 하나만 잡으면 못 잡은 화면이 "검사 통과"가 아니라 "검사 대상 아님"으로 조용히 빠진다.
SLOT_DECL = re.compile(
    r'public\s+static\s+readonly\s+\w+Slot\s+(\w+)\s*=\s*(?:new\s+\w+Slot\s*\(|Slot\s*\()\s*"([^"]+)"',
    re.S)
TYPE_DECL = re.compile(r'public\s+static\s+class\s+(\w+Layout)\b')

def scan(root: Path, min_coverage: float):
    files = [p for p in root.rglob("*.cs") if "obj" not in p.parts and "bin" not in p.parts]
    runtime = [p for p in files if not TESTS.search(str(p))]
    contracts = {}
    for p in runtime:
        txt = p.read_text(errors="ignore")
        for t in TYPE_DECL.findall(txt):
            contracts[t] = (p, SLOT_DECL.findall(txt))
    if not contracts:
        print("계약 타입을 못 찾았다 — 경로를 확인하라"); return 2

    worst = 0
    worst_holder = []
    for tname, (decl, slots) in sorted(contracts.items()):
        if not slots:
            print(f"\n=== {tname}  (선언 {decl.name}) ===")
            print("  [SKIP] 슬롯 선언을 못 읽었다 — 검사기가 이 선언 형태를 모른다. 통과가 아니다")
            worst_holder.append(1)
            continue
        referrers, test_only = [], []
        for p in files:
            if p == decl: continue
            txt = p.read_text(errors="ignore")
            if re.search(rf"\b{tname}\s*\.", txt):
                (test_only if TESTS.search(str(p)) else referrers).append(p)

        print(f"\n=== {tname}  (선언 {decl.name} · 슬롯 {len(slots)}) ===")
        # W-01
        if referrers:
            print(f"  [PASS] W-01 배선 — 런타임 참조 {len(referrers)}개: " +
                  ", ".join(p.name for p in referrers))
        else:
            worst = 1
            extra = f" (테스트만 {len(test_only)}개 참조: {', '.join(p.name for p in test_only)})" if test_only else ""
            print(f"  [FAIL] W-01 배선 — 이 계약을 읽는 런타임 파일이 0개다{extra}")
            print("         → 사양·계약이 일치해도 화면은 이 계약대로 그려지지 않는다")
            continue
        # W-02
        blob = "\n".join(p.read_text(errors="ignore") for p in referrers)
        used = [(m, s) for m, s in slots if re.search(rf"\b{tname}\s*\.\s*{m}\b", blob)]
        cov = len(used) / len(slots)
        miss = [s for m, s in slots if (m, s) not in used]
        if cov >= min_coverage:
            print(f"  [PASS] W-02 슬롯 배선율 {len(used)}/{len(slots)} = {cov:.0%}")
        else:
            worst = 1
            print(f"  [FAIL] W-02 슬롯 배선율 {len(used)}/{len(slots)} = {cov:.0%} < {min_coverage:.0%}")
            print("         미배선: " + ", ".join(miss[:12]) + (" …" if len(miss) > 12 else ""))
    worst = max([worst] + worst_holder)
    print("\n판정:", "PASS" if worst == 0 else "FAIL")
    return worst

if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--scripts", default="Assets/_Project/Scripts")
    ap.add_argument("--min-coverage", type=float, default=0.8)
    a = ap.parse_args()
    sys.exit(scan(Path(a.scripts), a.min_coverage))
