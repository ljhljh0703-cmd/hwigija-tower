# Combat Demo QA Script v0.1

## Scope

- Combat entry: `ENC_COMBAT_GATE_01` through `CHOICE_COMBAT_01_ENGAGE`
- Combat stableId: `COMBAT_GATE_01`
- Enemy: `ENEMY_FRACTURE_HOUND`
- Current demo behavior: `PrototypeRoomController.BeginRun()` sets `AutoResolveCombat = true`, so the scene smoke path resolves combat automatically after CombatGate.
- Manual combat QA applies after Attack / Defend / Skill controls are exposed to the player.

## Pre-Combat State

| check | expected |
|---|---|
| route order | Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete |
| selected smoke path | `CHOICE_SHOP_01_BUY_ITEM`, `CHOICE_MORAL_01_REFUSE`, `CHOICE_MEMORY_01_UNLOCK`, then `CHOICE_COMBAT_01_ENGAGE` |
| memory before combat | `MEM_FRAGMENT_01` unlocked; title/body keys available as placeholders |
| expected combat handoff marker | `combat started COMBAT_GATE_01; enemy ENEMY_FRACTURE_HOUND` |
| current enemy SO values | `SO_Enemy_ENEMY_FRACTURE_HOUND.asset`: hp `12`, attack `3`, pattern `PATTERN_PLACEHOLDER` |
| expected state panel | player HP, player ATK, Gold, Glitch, Affinity, combat id, enemy id, combat result |

## Auto-Resolve Smoke Path

Use this for the public 1-minute route until manual combat controls are wired.

| step | action | expected result |
|---|---|---|
| 1 | Reach `ENC_COMBAT_GATE_01`. | HUD route shows CombatGate current. |
| 2 | Press `CHOICE_COMBAT_01_ENGAGE`. | Result contains `choice applied: CHOICE_COMBAT_01_ENGAGE`. |
| 3 | Wait for auto-resolve. | Result contains `combat started COMBAT_GATE_01`, `enemy ENEMY_FRACTURE_HOUND`, and result `victory` or `defeat`. |
| 4 | Check post-combat state. | Victory applies Gold +7, Glitch -2, Affinity +2, `FLAG_COMBAT_GATE_01_VICTORY`; defeat applies HP -5, Glitch +5, Affinity -2. |
| 5 | Check completion. | HUD/result includes `demo.complete`; run state is completed; no duplicate memory unlock or duplicate combat reward on revisit. |

## Manual Combat QA

Only run this section after `AutoResolveCombat` is off for the reviewed build and UI buttons call `ResolveCombatRoundInteractive`.

| button | expected behavior from current code | QA notes |
|---|---|---|
| Attack | Calls `CombatAction.Attack`; enemy HP decreases by deterministic player damage; if enemy survives, player takes enemy damage. | Damage includes deterministic variance from the combat seed. Do not expect final balance numbers beyond visible HP movement. |
| Defend | Calls `CombatAction.Defend`; player deals no damage in current code; incoming enemy damage is halved. | This verifies defensive branch only. Parry/timer text is not implemented in current UI. |
| Skill | Calls `CombatAction.Skill`; current combat layer treats it like Attack unless an upper ability layer handles extra skill effects. | Mark as partial if no skill picker or ability-specific effect is visible. |

## Combo Damage Display

| condition | expected |
|---|---|
| normal 1-action combat | `ComboDamage = 0`; no combo line required. |
| TRAIT_OFFENSE_04 or explicit second action test path | `CombatRoundResult.ComboDamage > 0`; enemy HP should subtract player damage plus combo damage. |
| current public HUD | Combo damage is not displayed separately. Record as P1 UI gap, not a failed current smoke path. |

## Victory / Defeat Checks

| branch | expected state marker | expected post effects |
|---|---|---|
| Victory | `LastCombatResultId = victory`; `BattlesWon` increases. | Gold +7, Glitch -2 clamped at 0, Affinity +2, `FLAG_COMBAT_GATE_01_VICTORY = true`. |
| Defeat | `LastCombatResultId = defeat`; run may complete through defeat if player HP reaches 0. | HP -5, Glitch +5, Affinity -2. |
| DemoComplete | `DemoStatus = demo.complete`; completion panel active. | Should occur after the reviewed combat result. If manual combat is used and DemoComplete appears before final result, report as blocker. |

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
| Gold/Glitch/Affinity before/after |  |
| screenshot/video ref |  |
| likely owner | dev / writer / outsource / AI-training |
| notes |  |
