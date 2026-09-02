---
session: "2026-09-02 · M1 맵 진행 실패 뿌리 판별"
agent: "codex"
entry_point: "NEXT.md"
based_on:
  - "Docs/Portfolio/assets/concept-to-ui/DISPATCH-map-flow-m1.md"
  - "D-034"
  - "AGENTS.md"
gate: flagged
skill_candidate: false
first_pass: false
retries: 1
usage_receipt: null
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — M1 맵 진행 실패 격리 측정

## ② 산출물 (Outputs)

- `TestResults-MapFlow-M1-01-audio.xml` ~ `TestResults-MapFlow-M1-14-shop-boss.xml` · 실패 14건을 각각 새 Unity 프로세스에서 실행한 원본 XML · settlement: applied
- `Docs/ProjectOps/returns/RETURN-hwigi-m1-map-flow-isolation-2026-09-02.md` · 격리/전량 대조와 M2 분기 근거 · settlement: applied

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — dispatch가 Sub-brain 쓰기를 허용하지 않았고, Vault는 read-only로 유지했다.
- Memory append: 없음 — 동일 사유.

## ③ 게이트 대조 결과

### 착수 게이트

- 8개 runtime layout actual: 로비 12 · 전투 27 · 휴식 20 · 층지도 14 · 상점 20 · 이벤트 14 · 관문 15 · 엔딩 10, 모두 PASS. 전투 C-08은 기존 blocked SKIP 유지.
- `check_wiring.py`, `check_databinding.py`, `check_color_tokens.py` 모두 PASS.
- 실행 전·후 인카운터/아이템/카탈로그 git 상태는 clean이다.

### 격리 대조

| 테스트 | 격리(개별 새 프로세스) | 전량 R5 |
|---|---:|---:|
| `RoomController_ExplorationContextResetsToNormalBgm` | FAIL | FAIL |
| `FinalBoss_PreparedPlayerWinsInSevenToTenMeaningfulTurns` | FAIL | FAIL |
| `FinalBossVictory_OpensEndingChoiceOnce` | FAIL | FAIL |
| `Floor1_EventChoice_CompletesNodeAndReturnsToSelectableMap` | FAIL | FAIL |
| `Floor1_FirstLayer_DoesNotClassifyJarEventAsCombat` | FAIL | FAIL |
| `GenericCounterplay_HeavyPressureAndSkillOpeningChangeChoices` | FAIL | FAIL |
| `ItemAndAbilityEffects_ApplyToCombatLoop` | FAIL | FAIL |
| `JarRoomEvent_ResolvesDeterministicOutcomesWithoutRuntimeJson` | FAIL | FAIL |
| `PrototypeRoom_BossGateShowsFightOnlyAfterMapCommit` | FAIL | FAIL |
| `PrototypeRoom_BranchingMapOpensJarEventWithProbabilityHints` | FAIL | FAIL |
| `PrototypeRoom_FinalBossRestEndingReachableThroughBranchingMap` | FAIL | FAIL |
| `PrototypeRoom_RestNodeResolvesHpRecoveryAndHidesGlitch` | FAIL | FAIL |
| `PrototypeRoom_RouteActionButtonCompletesFullRun` | FAIL | FAIL |
| `PrototypeRoom_ShopAppearsBeforeBossAndBossClearsFloor` | FAIL | FAIL |

- 실패 이름 집합 비교(`diff -u`)는 EditMode/PlayMode 모두 exit 0이다.
- 첫 묶음 격리에서 선택된 `BuildDefaultCatalog()` 계열이 공유 상점·유물 에셋 8개를 바꾸는 부수효과를 관찰했다. 따라서 그 묶음 결과는 오염 판정 근거로 쓰지 않고, 14개를 각각 새 프로세스로 재측정했다. 각 측정 뒤 알려진 11개 test-artifact 에셋을 원복했고 최종 상태는 clean이다.

### 판정

- **테스트 오염 가설 기각.** 14/14가 깨끗한 독립 실행에서도 실패한다.
- 따라서 M2는 bake 격리 수리가 아니라 **D-034 대조표**가 필요하다. 이번 M1에서 코드·단언·설계 값은 변경하지 않았다.

### 같이 답한 세 문장

- 언제부터: 마지막 정상 실행은 불명이다. Audio test 최종 수정은 2026-05-29, RuntimeShell test는 2026-09-01, PlayMode smoke test는 2026-09-02, map state는 2026-07-01, builder는 2026-06-02다. 2026-05-10 이후 장기 미실행 구간이 있어 git mtime만으로 최초 파손 시점은 특정할 수 없다.
- 결정성: **예.** `PrototypeRunState`가 `PrototypeFloorMapBuilder.Build(_currentFloor, _demoRunPath, RunId, ...)`로 넘기고, builder는 `DeterministicSeed.Combine(0, "floor-map." + runId + "." + floor)`를 사용한다.
- 맵 진행과 무관: `RoomController_ExplorationContextResetsToNormalBgm`은 NullReference, `GenericCounterplay_HeavyPressureAndSkillOpeningChangeChoices`는 기대 문구 불일치, `ItemAndAbilityEffects_ApplyToCombatLoop`는 combat effect 0이다.

## 위배·STOP 사항

- D-034 값·맵 코드·테스트 단언·UI 12화면을 수정하지 않았다.
- 이 RETURN은 진단 결과만 제공한다. M2 설계/수리 범위는 Gate가 이 결과로 별도 발행해야 한다.

## 배운 점

- “bake fixture를 빼고 묶어 실행”만으로는 격리가 아니다. 선택된 테스트가 공유 에셋을 바꾸는지 확인하고, 변조하면 테스트별 새 프로세스+자산 원복으로 재측정해야 한다.

## ⑤ 스킬화 후보 보고

없음. 이번 절차는 dispatch가 충분히 구체적으로 지정했다.

---

Sub-brain status: read-only. M2 dispatch 전에는 코드 수정 금지 상태다.
