# Combat Demo QA Script v0.1

## Scope

- Combat entry: `ENC_COMBAT_GATE_01` through `CHOICE_COMBAT_01_ENGAGE`
- Combat stableId: `COMBAT_GATE_01`
- Enemy: `ENEMY_FRACTURE_HOUND`
- Current default scene behavior: `Assets/_Project/Scenes/PrototypeRoom.unity` sets `autoResolveCombat: 0`, so CombatGate opens the interactive combat panel.
- Legacy smoke behavior: tests can set `controller.AutoResolveCombat = true` to verify the route without manual clicks.

## Pre-Combat State

| check | expected |
|---|---|
| route order | Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete |
| selected smoke path | `CHOICE_SHOP_01_BUY_ITEM`, `CHOICE_MORAL_01_REFUSE`, `CHOICE_MEMORY_01_UNLOCK`, then `CHOICE_COMBAT_01_ENGAGE` |
| memory before combat | `MEM_FRAGMENT_01` unlocked; title/body keys available as placeholders |
| expected combat handoff marker | `combat started COMBAT_GATE_01; enemy ENEMY_FRACTURE_HOUND` |
| current enemy SO values | `SO_Enemy_ENEMY_FRACTURE_HOUND.asset`: hp `12`, attack `3`, pattern `PATTERN_PLACEHOLDER` |
| expected portrait | Mataios portrait is visible before CombatGate and remains separate from choices/result/combat panel. |
| expected state panel | player HP, player ATK, Gold, Glitch, Affinity, combat id, enemy id, combat round, last action, and result |

## Interactive Combat Path

Use this as the primary W2 demo QA path.

| step | action | expected result |
|---|---|---|
| 1 | Reach `ENC_COMBAT_GATE_01`. | HUD route shows CombatGate current. |
| 2 | Press `CHOICE_COMBAT_01_ENGAGE`. | Result contains `choice applied: CHOICE_COMBAT_01_ENGAGE`. |
| 3 | Confirm combat panel. | Combat panel is visible; Attack and Defend buttons are interactable; Skill button exists but is disabled. |
| 4 | Press Attack once. | Enemy HP decreases; combat round increments; last action includes `Attack`. |
| 5 | Press Defend once if combat is still active. | Last action includes `Defend`; incoming damage is reduced compared with a normal hit branch. |
| 6 | Continue Attack until combat resolves. | Result becomes `victory` or `defeat`; post-combat effects apply. |
| 7 | Check completion. | `demo.complete` appears only after combat result; no duplicate memory unlock or duplicate combat reward on revisit. |

## Auto-Resolve Smoke Path

Use only for route regression checks when a test harness sets `AutoResolveCombat = true`.

| step | action | expected result |
|---|---|---|
| 1 | Set `controller.AutoResolveCombat = true` before `BeginRun()`. | CombatGate resolves without manual round clicks. |
| 2 | Press `CHOICE_COMBAT_01_ENGAGE`. | Result contains `combat started COMBAT_GATE_01`, `enemy ENEMY_FRACTURE_HOUND`, and result `victory` or `defeat`. |
| 3 | Check completion. | HUD/result includes `demo.complete`; run state is completed. |

## Button Behavior

| button | expected behavior from current code | QA notes |
|---|---|---|
| Attack | Calls `CombatAction.Attack`; enemy HP decreases by deterministic player damage; if enemy survives, player takes enemy damage. | Damage includes deterministic variance from the combat seed. Do not expect final balance numbers beyond visible HP movement. |
| Defend | Calls `CombatAction.Defend`; player deals no damage in current code; incoming enemy damage is halved. | This verifies defensive branch only. Parry/timer text is not implemented in current UI. |
| Skill | Button is present but disabled in the current HUD. | This is intentional until ability-specific skill selection is wired. Do not report as blocker unless the demo brief promises usable Skill. |

## Combo Damage Display

| condition | expected |
|---|---|
| normal 1-action combat | `ComboDamage = 0`; no combo line required. |
| TRAIT_OFFENSE_04 or explicit second action test path | `CombatRoundResult.ComboDamage > 0`; enemy HP should subtract player damage plus combo damage. |
| current public HUD | Combo damage is displayed when `LastCombatComboDamage > 0`, but there is no public route that triggers a second action yet. |

## Victory / Defeat Checks

| branch | expected state marker | expected post effects |
|---|---|---|
| Victory | `LastCombatResultId = victory`; `BattlesWon` increases. | Gold +7, Glitch -2 clamped at 0, Affinity +2, `FLAG_COMBAT_GATE_01_VICTORY = true`. |
| Defeat | `LastCombatResultId = defeat`; run may complete through defeat if player HP reaches 0. | HP -5, Glitch +5, Affinity -2. |
| DemoComplete | `DemoStatus = demo.complete`; completion panel active. | Should occur after the reviewed combat result. If DemoComplete appears at combat start, report as regression. |

## Failure Report Template

| field | value |
|---|---|
| build/session |  |
| branch and commit |  |
| test mode | auto-resolve / manual combat |
| failed step |  |
| selected stableId |  |
| expected marker |  |
| actual marker |  |
| player HP before/after |  |
| enemy HP before/after |  |
| combat panel visible |  |
| portrait visible / overlap note |  |
| Gold/Glitch/Affinity before/after |  |
| screenshot/video ref |  |
| likely owner | dev / writer / outsource / AI-training |
| notes |  |
