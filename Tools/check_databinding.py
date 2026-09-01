#!/usr/bin/env python3
"""사양의 dataBinding.source 가 실제 C# 멤버를 가리키는지 검사한다.

왜 있는가 — 2026-09-02 상점 사양이 같은 유형으로 두 번 막았다.
  ① 개수를 안 열어보고 3으로 씀 (실물 5)
  ② `PrototypeRunSnapshot.OwnedItemCount` — 그런 멤버는 없다. 실제는 `ItemCount`
둘 다 사양이 자체 채점을 9/9 로 통과한 상태에서 Unity 컴파일·착수 단계까지 갔다.
좌표 채점기는 문자열이 실재하는 심볼인지 모른다. 이 검사기가 그 자리를 맡는다.

검사:
  B-01 dataBinding 절이 있는가 (없으면 2단 미완)
  B-02 `Type.Member` 가 그 타입의 public 멤버로 실재하는가
  B-03 컬렉션·목록 바인딩에 `count` 가 적혀 있는가 (개수는 실물을 열어야 적을 수 있다)
"""
import json, re, sys, argparse
from pathlib import Path

MEMBER = re.compile(r'\bpublic\s+(?:static\s+|readonly\s+|sealed\s+|override\s+|virtual\s+)*[\w\[\]<>,\.\?]+\s+(\w+)\s*(?:\{|=>|\()')
SYMBOL = re.compile(r'\b([A-Z]\w+)\s*\.\s*([A-Z]\w+)')
COLLECTION_HINT = re.compile(r'\[\]|List<|IReadOnlyList|View\[\]')

def members_of(root: Path):
    idx = {}
    for p in root.rglob("*.cs"):
        if any(x in p.parts for x in ("obj", "bin")): continue
        txt = p.read_text(errors="ignore")
        for m in re.finditer(r'public\s+(?:sealed\s+|static\s+|readonly\s+|partial\s+)*(?:class|struct|enum|interface)\s+(\w+)', txt):
            tname = m.group(1)
            body = txt[m.end():]
            idx.setdefault(tname, set()).update(MEMBER.findall(body[:20000]))
    return idx

def check(spec_path: Path, idx):
    d = json.loads(spec_path.read_text())
    name = spec_path.name
    db = d.get("dataBinding")
    print(f"\n=== {name} ===")
    if not db:
        print("  [FAIL] B-01 dataBinding 절이 없다 — 2단 미완. 슬롯이 무엇을 읽는지 사양이 말하지 않는다")
        return 1
    bad = 0
    for slot, v in db.items():
        if slot.startswith("_") or not isinstance(v, dict): continue
        src = str(v.get("source", ""))
        pairs = SYMBOL.findall(src)
        if not pairs:
            print(f"  [SKIP] {slot} — source 에 Type.Member 형태가 없다: {src[:60]}")
            continue
        for t, mem in pairs:
            if t not in idx:
                print(f"  [SKIP] {slot} — 타입 {t} 를 소스에서 못 찾음(외부/제네릭일 수 있다)"); continue
            if mem in idx[t]:
                print(f"  [PASS] {slot} — {t}.{mem}")
            else:
                bad = 1
                cand = [c for c in idx[t] if mem.lower().endswith(c.lower()) or c.lower().endswith(mem.lower())]
                hint = f" · 비슷한 것: {', '.join(sorted(cand)[:4])}" if cand else ""
                print(f"  [FAIL] {slot} — **{t}.{mem} 는 없다**{hint}")
        if COLLECTION_HINT.search(src) and not v.get("count"):
            bad = 1
            print(f"  [FAIL] {slot} — 컬렉션인데 `count` 가 비어 있다. **개수는 실물을 열어야 적을 수 있다**(B-03)")
    return bad

if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--scripts", default="Assets/_Project/Scripts")
    ap.add_argument("--specs", default="Docs/Portfolio/assets/concept-to-ui")
    a = ap.parse_args()
    idx = members_of(Path(a.scripts))
    worst = 0
    for sp in sorted(Path(a.specs).glob("*_layout_spec*.json")):
        worst = max(worst, check(sp, idx))
    print("\n판정:", "PASS" if worst == 0 else "FAIL")
    sys.exit(worst)
