---
session: "2026-09-02 · M2 D-034 map-flow contract dump"
agent: "codex"
entry_point: "NEXT.md"
based_on:
  - "Docs/Portfolio/assets/concept-to-ui/DISPATCH-map-flow-m2.md"
  - "Docs/Portfolio/assets/concept-to-ui/map_flow_spec.json"
  - "D-034"
  - "RETURN-hwigi-m1-map-flow-isolation-2026-09-02.md"
gate: flagged
skill_candidate: false
first_pass: true
retries: 0
usage_receipt: null
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — M2 D-034 런타임 덤프 대조

## ② 산출물 (Outputs)

- `Assets/_Project/Tests/PlayMode/MapFlowRuntimeDumpTests.cs` · production 맵 로직을 수정하지 않고 5층 beforeSelect/afterSelect + 동일 runId determinism pair를 runtime에서 기록하는 명시적 diagnostic fixture · settlement: applied
- `Docs/Portfolio/assets/concept-to-ui/mapflow_runtime_dump.json` · `source: runtime`, `runId: room.prototype`, 10 floor-phase snapshot과 pair · settlement: applied
- `TestResults-MapFlow-M2.xml` · `MapFlowM2_WritesBeforeAndAfterRuntimeDump` 1/1 pass · settlement: applied
- `Docs/Portfolio/assets/concept-to-ui/map_flow_spec.json` · D-034 10제약 입력 · settlement: applied
- `Tools/check_mapflow.py` · 덤프 대조기 · settlement: applied

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — dispatch가 Sub-brain 쓰기를 허용하지 않았고, Vault는 read-only로 유지했다.
- Memory append: 없음 — 동일 사유.

## ③ 게이트 대조 결과

### `python3 Tools/check_mapflow.py` 전문 요약

- PASS: MF-01, MF-02, MF-04, MF-05, MF-06, MF-07, MF-08, MF-09, MF-10.
- **FAIL: MF-03만.** floor 1~5 모두 cross-lane edge **4 > 2**. beforeSelect/afterSelect 두 phase에서 같은 구조를 재므로 checker 출력은 층마다 두 번 나온다.
- SKIP 0. `source: runtime`이며 determinism pair A/B는 동일하다.

### runtime state 3문

- 층 진입 직후: floor 1~5 모두 `selectedMapNodeId=""`, Layer 1 node 3개가 Selectable=true다. layer 분포는 각각 `1:3, 2:3, 3:3, 4:1, 5:1`.
- 선택 직후: 각 floor에서 node 하나가 실제로 선택되어 `selectedMapNodeId`가 채워지고 Selectable=true는 **0개**다. MF-08 commitment는 정상이다.
- 저장 복원: 이 fixture는 `BeginRun → OpenQaFloor` 경로이며 `StartRunFromSave/RestoreFromSaveData`를 타지 않는다. 따라서 `IsSavedMapNodeSelectable`은 이번 덤프 원인이 아니다.

### M1과의 관계

- M1의 14/14 독립 실패는 사실로 유지된다.
- 이번 **QA floor-open runtime**에서는 MF-07이 통과한다. 따라서 빈 selectable 목록은 단순 builder 초기 상태가 아니라, M3에서 normal route/test traversal과 QA floor-open 경로의 상태 전이를 대조해야 한다.
- 이번 라운드는 M3 수리를 위한 측정만 수행했으며, D-034 값·production map code·기존 단언은 수정하지 않았다.

## 위배·STOP 사항

- D-034 MF-03 cross-lane 상한 위반이 runtime evidence로 확인됐다. 고치지 않았다.
- M3는 Gate가 별도 발행해야 하며, normal route/test traversal의 empty selectable 상태와 MF-03을 같은 원인으로 가정해서는 안 된다.

## 배운 점

- before/after 쌍이 있어야 `_selectedMapNodeId`에 의한 정상 commitment와 selectable 0 stuck을 분리할 수 있다.
- QA floor-open snapshot이 정상이라고 해서 M1의 normal traversal failure를 지우지 않는다. 두 runtime entry path를 별도 상태 전이로 남겨야 한다.

## ⑤ 스킬화 후보 보고

없음. M2 dispatch와 map-flow checker가 이 진단에 충분히 구체적이다.

---

Sub-brain status: read-only. M3 dispatch 전 production map fix 금지.
