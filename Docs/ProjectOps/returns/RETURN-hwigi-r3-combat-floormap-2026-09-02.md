---
session: "2026-09-02 hwigi-r3-combat-floormap"
agent: "codex"
entry_point: "Docs/Portfolio/assets/concept-to-ui/DISPATCH-r3-combat-rewire-and-floormap.md"
based_on:
  - "D-012"
  - "D-029"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: ready_for_review
skill_candidate: false
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 UI R3 전투 재배선 + 층 지도

## 기준

- 브랜치: `codex/hwigi-lobby-ui-pilot-r3`
- 시작 HEAD: `7e5a6985cd9ad5f264814521d53269dbf4820f31` (`Harden runtime validation R2`)
- 순서 준수: A(배선 검사기) → B(전투) → C(층 지도)
- `D-012`/`D-029`에 따라 화면 문구에 `붕괴도`·`Glitch`를 추가하지 않았고, 수치·전투 판정·저장 스키마는 변경하지 않았다.

## Task A · 배선 검사기 상시화

### 변경

- `Assets/_Project/Tests/EditMode/LayoutWiringTests.cs`
  - 모든 non-test `*Layout` 계약을 발견해 W-01(런타임 참조)과 W-02(선언 슬롯 100% 참조)를 EditMode에서 확인한다.
- `Docs/Portfolio/assets/concept-to-ui/QUEUE.md`
  - 좌표 대조와 `Tools/check_wiring.py`를 착수 전·발송 전의 두 필수 채점으로 명시했다.

### 최종 `python3 Tools/check_wiring.py` 출력

```text
=== CombatLayout  (선언 CombatLayoutContract.cs · 슬롯 22) ===
  [PASS] W-01 배선 — 런타임 참조 1개: CombatUiController.cs
  [PASS] W-02 슬롯 배선율 22/22 = 100%

=== FloorMapLayout  (선언 FloorMapLayoutContract.cs · 슬롯 10) ===
  [PASS] W-01 배선 — 런타임 참조 1개: FloorMapUiController.cs
  [PASS] W-02 슬롯 배선율 10/10 = 100%

=== LobbyLayout  (선언 LobbyLayoutContract.cs · 슬롯 8) ===
  [PASS] W-01 배선 — 런타임 참조 1개: LobbyController.cs
  [PASS] W-02 슬롯 배선율 8/8 = 100%

=== RestLayout  (선언 RestLayoutContract.cs · 슬롯 14) ===
  [PASS] W-01 배선 — 런타임 참조 1개: RestUiController.cs
  [PASS] W-02 슬롯 배선율 14/14 = 100%

판정: PASS
```

- 좁은 EditMode: `LayoutWiringTests.RuntimeLayoutContractsHaveAFullRuntimeWiringPath` = **1/1 pass**.

## Task B · 전투 재배선

### 변경

- `Assets/_Project/Scripts/UI/CombatLayout.cs` — Unity `RectTransform`/토큰 어댑터.
- `Assets/_Project/Scripts/UI/CombatUiController.cs` — 22개 `CombatLayout` 슬롯을 실물 UI로 생성하는 전용 모듈.
- `Assets/_Project/Scripts/UI/PrototypeHud.cs` — `Combat UI Module`을 `AddComponent`하고 기존 HUD 표현 필드를 모듈 표면에 배선. 이전 전투 빌더의 도달 불가 경로는 제거했다.
- `Assets/_Project/Tests/EditMode/PresentationLayerTests.cs`, `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs` — 기존 3줄 로그 단언을 새 계약의 적 제목·기호 판단줄·단일 기록줄 단언으로 이관했다. Ignore/삭제/단언 완화는 하지 않았다.

### 결과

- `CombatLayout`: W-01 PASS, W-02 **22/22 = 100%**.
- 런타임 actual: **27/27 PASS**.
- `threatReadout`은 문장 대신 `> 피해 · 적 HP/최대 HP · 생존 턴`의 기호+숫자를 표시한다. 기본 폰트에서 렌더되지 않는 칼 기호에도 ASCII `>`가 남도록 했다.
- `combatLog`는 최근 기록 1줄만 보이고, 판단 정보는 판단줄로 분리했다.
- v2의 7 신규 슬롯(`floorChip`, `sanityChip`, `hpChip`, `goldChip`, `threatReadout`, `playerSanityBar`, `mataiosStateLabel`)이 모두 런타임 actual에 존재한다.
- 같은 명시적 실행에서 로비 **12/12**, 휴식 **20/20**도 유지했다.

## Task C · 층 지도

### 변경

- `Assets/_Project/Scripts/UI/FloorMapLayoutContract.cs` — Unity 비의존 10슬롯 계약.
- `Assets/_Project/Scripts/UI/FloorMapLayout.cs` — Unity 어댑터/토큰.
- `Assets/_Project/Scripts/UI/FloorMapUiController.cs` — `NormalizedX/Y`를 그대로 쓰고 `NextMapNodeIds`로 간선을 그리는 전용 모듈.
- `Tools/DumpLayout/DumpLayout.csproj`, `Tools/DumpLayout/Program.cs` — `--screen floormap` contract dump 지원.
- `Assets/_Project/Tests/PlayMode/PortraitUiScreenshotQaTests.cs` — `floormap_runtime_actual_layout.json` writer 및 리포 내부 캡처 경로 추가.

### 결과

- 자체 사양 M-01~M-08: **8/8 PASS**.
- `FloorMapLayout`: W-01 PASS, W-02 **10/10 = 100%**.
- 런타임 actual: **14/14 PASS**.
- 5개 노드 타입은 기존 A 금색 원형 아이콘 계열을 사용했고, 층 이름·난이도는 지어내지 않았다.
- 선택 전 `departButton`은 비활성 상태로도 슬롯을 유지한다. 선택 가능한 노드만 실제로 상호작용 가능하다.

## 런타임 actual · 캡처 (리포 내부)

| 화면 | runtime actual | source | 대조 |
|---|---|---|---:|
| 로비 | `Docs/Portfolio/assets/concept-to-ui/lobby_runtime_actual_layout.json` | `runtime` | 12/12 |
| 전투 | `Docs/Portfolio/assets/concept-to-ui/combat_runtime_actual_layout.json` | `runtime` | 27/27 |
| 휴식 | `Docs/Portfolio/assets/concept-to-ui/rest_runtime_actual_layout.json` | `runtime` | 20/20 |
| 층 지도 | `Docs/Portfolio/assets/concept-to-ui/floormap_runtime_actual_layout.json` | `runtime` | 14/14 |

- 신규 정적 dump: `Docs/Portfolio/assets/concept-to-ui/floormap_actual_layout.json` (`source: contract`).
- 명시적 QA PNG: `Docs/Portfolio/assets/concept-to-ui/runtime-captures/`.
  - 층 지도: `02_floor_map.png` SHA-256 `88779147b5a2fa7f6fabfb8928b31594e827479e836727ad9978b34782da47d1`
  - 전투: `06_normal_combat.png` SHA-256 `9249281d6297db09a136dde80820d7be7224b3f1830e2ca92dc7b8a4dfa56bf2`
- runtime actual SHA-256:
  - lobby `4a7020e7cdfe2c67fd8a3e8e811b06c9695711c6bba714ceeee6d98dc29bb36e`
  - combat `e46ee6691ed031180e18041bed63c70006b5aac8fcb8aa73d3d8cbba65bd0a04`
  - rest `77b08d2aa0cac63d855396928e99055bfade61206bc59c8ddc64ce13c66b1c44`
  - floormap `e4ccf839edd4dcc77c55586d0b6f19e7e7482740a446c911aefa801bf335b082`

## 검증

- `PortraitUiV3_CapturesRequiredQaScreens`: **1/1 pass**.
- 전량 EditMode: **204/212 pass, 8 fail** — R2 기준선 잔여와 같은 이름 집합. 새 `LayoutWiringTests` 1건이 추가됐으므로 total만 211→212.
- 전량 PlayMode: **14/21 pass, 6 fail, explicit 1 SKIP** — R2 기준선 잔여와 같은 이름 집합.
- `git diff --check` (R3 소유 파일): pass.
- CodeGraph: 죽은 전투 빌더 정리 뒤 최종 sync 완료 (667 nodes).
- 테스트가 실제 encounter/item asset을 갱신한 11개 부수 변경은 검증 후 정확히 원복했다. R3에 gameplay data asset 변경은 없다.

## C-08

**SKIP 유지.** `strained`/`broken` variants는 아트 4종 미입고와 `thresholds.*: undecided` 때문에 blocked다. 기본 상태를 대체 측정으로 사용하지 않았다.

## 남은 실패 · 범위 밖 · 미수정

- EditMode 8:
  - `RoomController_ExplorationContextResetsToNormalBgm`
  - `FinalBoss_PreparedPlayerWinsInSevenToTenMeaningfulTurns`
  - `FinalBossVictory_OpensEndingChoiceOnce`
  - `Floor1_EventChoice_CompletesNodeAndReturnsToSelectableMap`
  - `Floor1_FirstLayer_DoesNotClassifyJarEventAsCombat`
  - `GenericCounterplay_HeavyPressureAndSkillOpeningChangeChoices`
  - `ItemAndAbilityEffects_ApplyToCombatLoop`
  - `JarRoomEvent_ResolvesDeterministicOutcomesWithoutRuntimeJson`
- PlayMode 6:
  - `PrototypeRoom_BossGateShowsFightOnlyAfterMapCommit`
  - `PrototypeRoom_BranchingMapOpensJarEventWithProbabilityHints`
  - `PrototypeRoom_FinalBossRestEndingReachableThroughBranchingMap`
  - `PrototypeRoom_RestNodeResolvesHpRecoveryAndHidesGlitch`
  - `PrototypeRoom_RouteActionButtonCompletesFullRun`
  - `PrototypeRoom_ShopAppearsBeforeBossAndBossClearsFloor`

## 상태

- `QUEUE.md` 2번(전투)과 4번(층 지도)을 runtime 통과로 갱신했다.
- 다음 큐 5~8은 공유 사양 발행 전이므로 착수하지 않았다.
- Skill candidate: 없음.
- Sub-brain: read-only. Progress/Memory append 없음.
