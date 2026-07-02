# Portfolio Capture Polish RC1 Status

## Current Gate

- Gate 0 Baseline: PASS
- Gate 1 Capture Risk Triage: PASS for triage
- Demo-video readiness: FAIL until P0 implementation is complete
- Gate 1 input baseline: `origin/Proto@9f59b51b430f022a17eb6c599a79bccbc675f813`
- Main dirty worktree: not used as evidence

## Gate 1 Findings

### P0

1. Combat
   - Replace raw centered HUD/log with a battle-stage layout and bottom party dock.
   - Keep enemy HP, player HP, Mataios HP, and last 2-3 combat rows readable.
   - Attribute player and Mataios actions separately: `플레이어: N 피해`, `마타이오스: N 지원 피해`, `보호`, `마무리`.
   - Keep hit numbers, shake/pulse, and defeat feedback visible.
   - Disabled Skill must show a player-facing reason.

2. Map
   - Hide debug/internal labels such as `[current]`, `[locked]`, `MoralChoice`, `CHOICE_`, `ENC_`.
   - Use existing map/node sprites for sparse route, current glow, future dark nodes, and locked skipped nodes.
   - Keep labels Korean and player-facing.

3. Event
   - Remove raw labels such as `MoralChoice | choices`, `Choice 1`, `Choice 2`, and `Unavailable`.
   - Use event title/body panel plus 2-3 large choice buttons.
   - Each choice should show a concise effect preview; after selection, show a compact result strip.

4. Reward/Growth
   - Replace plain result text with a centered reward/growth popup.
   - Show reward rows and growth choice rows clearly.
   - Use one clear CTA.

5. Shop
   - If shop appears in the capture route, it must show 2-3 visible item cards.
   - Cards need name, effect, price, current Gold, affordability/disabled reason, and leave CTA.

### P1/P2 Deferred

- Rest layout polish and Mataios reaction chip after rest/shop/event outcomes.
- Dedicated final icon pass for combat/map/shop.
- Merchant framing and purchase SFX/VFX.
- Full growth/inventory screen parity with target visual.
- Final Korean copy/story/dialogue pass.

## Implementation Boundary

The next Game Dev pass should stay presentation-first.

Allowed default scope:

- `Assets/_Project/Scripts/UI/PrototypeHud.cs`
- `Assets/_Project/Tests/EditMode/PresentationLayerTests.cs`
- narrowly related UI tests if needed

Avoid unless a failing user-path test proves it is required:

- `Assets/_Project/Scripts/Combat/CombatController.cs`
- `Assets/_Project/Scripts/Run/PrototypeRunState.cs`
- `Assets/_Project/Scripts/Run/PrototypeFloorMap.cs`
- `Assets/_Project/Scripts/Run/PrototypeEncounterRuntimeResolver.cs`
- `Assets/_Project/Scripts/Combat/MataiosCombatBrain.cs`

## Code Anchors

- Combat UI: `PrototypeHud.UpdateCombatPanel`, `BuildCombatPresentation`, `BuildCombatLogDetailLine`, `BuildCombatDecisionLine`, `BuildCombatPlayerCardText`, `BuildCombatMataiosCardText`, `UpdatePartyDock`
- Combat feedback: `UpdateCombatIntro`, `UpdateCombatDefeatFeedback`, `UpdateCombatEnemyFeedbackAnimation`, `UpdateCombatPlayerFeedbackAnimation`, `UpdateCombatDamageNumberAnimation`
- Map UI: `ShowMapChoices`, `CreateMapNodeButton`, `BuildMapNodeHint`, `ResolveMapNodeLabel`, `ApplyFloorMapBackground`
- Event UI: `ShowChoices`, `ConfigureChoiceContainerForEvent`, `ShowEventCutsceneLayout`, `ResolveEventHeader`, `ResolveEventBody`, `ResolvePublicChoiceLabel`, `ReplacePublicRefs`
- Reward UI: `EnsureLevelRewardPanel`, `UpdateLevelRewardPopup`, `EnsureBossRewardPanel`, `UpdateBossRewardPopup`
- Rest UI: `ShowRestInteraction`, `EnsureRestInteractionPanel`, `BuildRestActionCardLabel`, `ResolveRestActionPreview`
- Shop UI: `ConfigureChoiceContainerForShop`, `ApplyShopChoiceCard`, `ResolvePurchaseChoiceTitle`, `ResolveShopCardSprite`, `ApplyMerchantPresentation`

## Next Game Dev Dispatch

```markdown
# Game Dev 지시 — Portfolio Capture Polish RC1 P0 Implementation

## 기준
- Base: latest origin/Proto after command-center Gate 1 status
- clean worktree 필수
- main dirty worktree 사용 금지
- C# runtime 변경 전 CodeGraph fresh preflight
- 먼저 `Docs/ProjectOps/portfolio-capture-polish-harness.md`와 `Docs/ProjectOps/portfolio-capture-polish-status.md`를 읽을 것

## 구현 범위
Gate 1 P0만 구현한다. 기본 write scope는 `PrototypeHud.cs` + `PresentationLayerTests.cs`다.

1. Combat
- enemy stage + bottom player/Mataios party dock로 정리
- enemy HP / player HP / Mataios HP / combat log 2~3줄 중심
- player damage와 Mataios support/protect/finish를 분리 표시
- Attack/Defend/Skill 버튼을 읽기 좋게 유지하고 disabled Skill 이유 표시

2. Map/Event
- `[current]`, `[locked]`, `MoralChoice`, `CHOICE_`, `ENC_` 같은 raw/internal label 노출 제거
- sparse route/current/future/locked state를 기존 sprite와 Korean label로 표시
- event choice는 Korean label + effect preview + compact result strip으로 정리

3. Reward/Growth/Shop
- result/reward/growth는 중앙 popup + 명확한 reward row + 단일 CTA
- shop capture가 가능하도록 item card 2~3개, name/effect/price/current Gold/disabled reason/leave CTA 표시

## 금지
- 새 시스템, 새 밸런스, 새 경제 수치 추가 금지
- `CombatController`, `PrototypeRunState`, `PrototypeFloorMapBuilder`, `MataiosCombatBrain` 변경 금지. 실제 버그가 테스트로 증명되면 보고 후 최소 변경
- runtime RL/ONNX 금지
- ITEM_07 / OQ-025 full deck 금지
- raw ML output commit 금지

## 검증
- CodeGraph preflight
- git diff --check
- forbidden diff search
- targeted EditMode: `PresentationLayerTests`
- Android APK build

## 보고
[Done]
[Files]
[Gate] Gate 2 pass/fail
[Commit]
[Validation]
[APK]
[Risks]
[Next Deploy instruction]
```

## Gate Decision

Proceed to Gate 2 P0 Implementation. Do not proceed to Deploy until Gate 2 produces a buildable APK candidate.
