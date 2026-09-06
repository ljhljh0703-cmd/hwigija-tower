#!/usr/bin/env python3
"""맵 흐름 덤프를 D-034 계약과 대조한다.

왜 있는가 — 2026-09-02. 맵 진행 테스트 14건이 기준선부터 실패하고,
M1 격리 실행으로 테스트 오염 가설이 **기각**됐다(14/14 가 새 프로세스·깨끗한 자산에서도 실패).
진짜 회귀인데 「무엇이 맞는 상태인가」가 기계로 판정되지 않으면 고쳐도 고친 줄 모른다.
D-034 는 🔒 LOCKED 이지만 산문이다. 이 검사기가 그 산문을 덤프 대조로 바꾼다.

규율(이 리포의 다른 검사기와 같다):
  · 모르는 입력은 무시하지 말고 터뜨린다
  · SKIP 은 통과가 아니다 — exit 1
  · 검사 못 한 항목은 건수를 찍는다. 조용히 빼지 않는다
"""
import json, sys, argparse, collections
from pathlib import Path

def load(p: Path, what: str):
    if not p.is_file():
        print(f"  [FAIL] {what} 파일이 없다 — {p}")
        return None
    return json.loads(p.read_text())

def check(spec, dump):
    fails = skips = 0
    if dump.get("source") != "runtime":
        print(f"  [FAIL] dump.source = {dump.get('source')!r} — 'runtime' 이어야 한다. "
              f"정적 산출을 런타임 증거로 쓰지 않는다")
        fails += 1
    floors = dump.get("floors") or []
    if not floors:
        print("  [FAIL] floors 가 비어 있다 — 빈 덤프는 통과가 아니다")
        return 1

    have_phases = {fl.get("phase", "beforeSelect") for fl in floors}
    by_id = {c["id"]: c for c in spec["constraints"]}
    for cid in sorted(by_id):
        c = by_id[cid]
        mc = c.get("machineCheck")
        if mc and mc.get("phase") and mc["phase"] not in have_phases:
            print(f"  [SKIP] {cid} — 덤프에 phase={mc['phase']!r} 스냅샷이 0개다. "
                  f"**아무것도 재지 않았다. 통과가 아니다** (있는 phase: {sorted(have_phases)})")
            skips += 1; continue
        if not mc:
            print(f"  [SKIP] {cid} — machineCheck 없음. **통과가 아니다**"); skips += 1; continue
        t = mc["type"]; bad = []

        for fl in floors:
            n = fl.get("nodes") or []
            fno = fl.get("floor")
            phase = fl.get("phase", "beforeSelect")

            if t == "laneCount":
                lanes = {nd.get("lane", nd.get("Index")) for nd in n}
                if len(lanes) != mc["eq"]:
                    bad.append(f"floor{fno} lane {len(lanes)}개 (기대 {mc['eq']})")
            elif t == "outgoingEdges":
                zero = [nd["MapNodeId"] for nd in n
                        if nd.get("Type") not in mc["applyTo"]["typeNot"]
                        and not nd.get("Completed") and not (nd.get("NextMapNodeIds") or [])]
                one = sum(1 for nd in n if len(nd.get("NextMapNodeIds") or []) == 1)
                print(f"         floor{fno} outgoing==1 {one}/{len(n)}")
                if zero: bad.append(f"floor{fno} outgoing 0인 미완료 노드 {len(zero)}개: {zero[:3]}")
            elif t == "crossLaneEdges":
                lane = {nd["MapNodeId"]: nd.get("lane", nd.get("Index")) for nd in n}
                x = sum(1 for nd in n for nxt in (nd.get("NextMapNodeIds") or [])
                        if nxt in lane and lane[nxt] != lane[nd["MapNodeId"]])
                if x > mc["maxPerFloor"]:
                    bad.append(f"floor{fno} cross-lane edge {x} > {mc['maxPerFloor']}")
            elif t == "nodeTypeAbsentInLayer":
                hit = [nd["MapNodeId"] for nd in n
                       if nd.get("Type") == mc["nodeType"] and nd.get("Layer") == mc["layer"]]
                if hit: bad.append(f"floor{fno} layer{mc['layer']} 에 {mc['nodeType']} {hit}")
            elif t == "nodeTypeCountPerFloor":
                k = sum(1 for nd in n if nd.get("Type") == mc["nodeType"])
                if k > mc["max"]: bad.append(f"floor{fno} {mc['nodeType']} {k} > {mc['max']}")
            elif t == "futureNodesLockedWithType":
                for nd in n:
                    if not nd.get("Selectable") and not nd.get("Completed"):
                        if not nd.get("Locked"): bad.append(f"floor{fno} {nd['MapNodeId']} 미래인데 Locked=false")
                        if not nd.get("Type"): bad.append(f"floor{fno} {nd['MapNodeId']} Type 비어 있음")
            elif t == "minSelectable":
                if phase != mc["phase"]: continue
                if mc.get("onlyIfFloorActive"):
                    # 🩸 v1.2: 「미완료가 남았는가」는 틀린 조건이었다. sparse route 는 안 지나간 레인 노드가
                    # 항상 미완료로 남는다. 옳은 조건은 「층이 아직 진행 중인가」이고, 그건 덤프가 실어야 한다.
                    if "floorActive" not in fl:
                        bad.append(f"floor{fno}({phase}) `floorActive` 필드가 없다 — "
                                   f"**완주와 막힘을 구분할 수 없다. 재지 못한 것이다**")
                        continue
                    if not fl.get("floorActive"):
                        continue
                k = sum(1 for nd in n if nd.get("Selectable"))
                if k < mc["min"]:
                    bad.append(f"floor{fno}({phase}) 선택 가능 {k} < {mc['min']} — **진행 불가** "
                               f"(미완료 {sum(1 for x in n if not x.get('Completed'))}개 남음)")
            elif t == "maxSelectable":
                if phase != mc["phase"]: continue
                k = sum(1 for nd in n if nd.get("Selectable"))
                if k > mc["max"]:
                    bad.append(f"floor{fno}({phase}) 선택 가능 {k} > {mc['max']} — commitment 회피 가능")
            elif t == "nextFloorMapPresent":
                if mc.get("onlyIfFloorActive"):
                    if "floorActive" not in fl:
                        bad.append(f"floor{fno}({phase}) `floorActive` 필드가 없다 — 재지 못한 것이다"); continue
                    if not fl.get("floorActive"): continue
                if not n: bad.append(f"floor{fno} 노드 0개")
                elif phase == "beforeSelect" and not any(nd.get("Selectable") for nd in n):
                    bad.append(f"floor{fno} 새 지도에 선택 가능 노드 0")
            elif t == "unvisitedSkippedAtFloorEnd":
                if fl.get("floorActive") is not False: continue
                miss = [nd["MapNodeId"] for nd in n
                        if not nd.get("Completed") and not nd.get("Skipped")]
                if miss:
                    bad.append(f"floor{fno} 층 종료인데 Skipped 아닌 미완료 {len(miss)}개: {miss[:3]}")
            elif t == "deterministicByRunId":
                pass  # 별도 2회 실행 비교. 아래에서 처리
            else:
                print(f"  [FAIL] {cid} — 알 수 없는 검사 유형 {t!r}. 검사기가 이 규칙을 모른다")
                fails += 1; bad = None; break

        if bad is None: continue
        if t == "deterministicByRunId":
            a, b = dump.get("determinismPairA"), dump.get("determinismPairB")
            if a is None or b is None:
                print(f"  [SKIP] {cid} — 같은 runId 2회 덤프가 없다. **통과가 아니다**"); skips += 1; continue
            if a != b: print(f"  [FAIL] {cid} — 같은 runId 두 덤프가 다르다"); fails += 1
            else: print(f"  [PASS] {cid}")
            continue
        if bad:
            fails += 1
            print(f"  [FAIL] {cid} — {c['text'][:44]}")
            for b in bad[:6]: print(f"         {b}")
        else:
            print(f"  [PASS] {cid}")

    if skips:
        print(f"\n  ⚠️ SKIP {skips}건 — **통과가 아니다.** 재지 못한 것은 재지 못했다고 센다")
    print("\n판정:", "PASS" if fails == 0 and skips == 0 else "FAIL")
    return 0 if (fails == 0 and skips == 0) else 1

if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--spec", default="Docs/Portfolio/assets/concept-to-ui/map_flow_spec.json")
    ap.add_argument("--dump", default="Docs/Portfolio/assets/concept-to-ui/mapflow_runtime_dump.json")
    a = ap.parse_args()
    spec = load(Path(a.spec), "사양")
    dump = load(Path(a.dump), "덤프")
    if spec is None or dump is None:
        print("\n판정: FAIL"); sys.exit(1)
    print(f"\n=== 맵 흐름 대조 · D-034 ===")
    sys.exit(check(spec, dump))
