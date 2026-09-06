# Combat Feel Audit v0.1

## Scope

- Unity was not run.
- Audit is based on `PrototypeHud`, `PrototypeRoomController`, `PrototypeRunState`, PlayMode tests, and existing screenshots.
- Current demo target: explain Attack / Defend / Skill clearly enough for W2 recording.

## Current Combat Implementation

| area | current state | evidence |
|---|---|---|
| Combat starts | `CHOICE_COMBAT_01_ENGAGE` starts `COMBAT_GATE_01` against `ENEMY_FRACTURE_HOUND`. | `SO_Encounter_ENC_COMBAT_GATE_01.asset`; `PrototypeRunState.ResolveCombatHandoff` |
| Manual mode | Default scene has `autoResolveCombat: 0`. | `PrototypeRoom.unity` |
| Attack | Enabled during active combat; lowers enemy HP in PlayMode. | `PrototypeHud.UpdateCombatPanel`; `PrototypeRoom_CombatGateShowsInteractiveCombatPanel` |
| Defend | Enabled during active combat; result includes `Defend`; damage reduced. | PlayMode test |
| Skill | Visible but always disabled. | `PrototypeHud.UpdateCombatPanel` |
| Combat panel | Shows text fields for combat id, enemy HP, player HP, round, last result, reward deltas, combo if >0. | `PrototypeHud.UpdateCombatPanel` |
| Result text | Hidden during active combat, returns after combat. | `UpdateResultVisibility`; screenshots 04/05 |
| DemoComplete | Appears after combat closes. | `UpdateDemoCompletePanel`; screenshot 05 |

## Is Each Action Understandable?

| action | understandable today | issue | recommendation |
|---|---:|---|---|
| Attack | partial | Enemy HP changes in text, but no bar, damage number, hit flash, or SFX. | Add enemy HP bar delta, floating damage number, and hit SFX. |
| Defend | partial | Text says `Defend`, but the reduced damage is not visually contrasted. | Add shield flash, "blocked/reduced" marker, and player HP delta comparison. |
| Skill | understandable as unavailable, not as a game system | Button is visible/disabled, but no reason is shown. | Add disabled reason label/tooltip: locked until ability route, or hide if recording copy should avoid future promise. |

## Missing Feedback Audit

| feedback | current state | demo risk | implementation-ready acceptance criteria |
|---|---|---|---|
| HP bar change | Text-only HP values. | Viewer may not notice Attack impact. | Enemy and player HP bars appear in combat panel; after Attack, enemy fill decreases within 0.2s; after damage, player fill decreases; text and bar values match snapshot. |
| Damage number | Missing. | Attack/Defend feel static. | After each round, spawn one enemy damage number near enemy/combat panel and one player damage number only if player damage > 0; numbers clear within 1s and do not shift layout. |
| Round result | Dense `last:` text. | Hard to parse what just happened. | Combat panel has a dedicated `Round Result` line: action, enemy damage, player damage, optional block/combo; max 1 line in recording mode. |
| Victory/defeat transition | Victory summary appears as dense result after combat. Defeat is not public route. | Completion can feel abrupt. | On enemy defeat, disable combat buttons immediately, play victory flash/sting, close combat panel, then show DemoComplete cutscene/overlay. Defeat remains hidden unless QA flag path. |
| Combo marker | Text only when combo > 0. Current route may not trigger it. | Feature invisible; text-only marker would be missed. | If combo > 0, show a small `Combo +N` marker near round result for 0.8s; if no combo, do not reserve space. |
| Button press response | Default Unity button only. | Weak tactile feel in recording. | Attack/Defend buttons have pressed color/scale state and SFX; Skill disabled uses dim style consistently. |
| Enemy identity | Raw id in text. | Looks like debug data. | Recording mode maps `ENEMY_FRACTURE_HOUND` to approved display key; raw id only in debug. |
| Reward deltas | Text line always includes gold/glitch/affinity. | Zero deltas add noise. | Recording mode shows only nonzero deltas after victory; zero values hidden. |

## Suggested Dev Work Order

| priority | task | likely files | acceptance |
|---|---|---|---|
| P0 | Add recording-mode combat summary formatter | `PrototypeHud.cs` or new formatter | Active combat shows clean fields; debug mode can still show raw ids. |
| P0 | Add HP bars for enemy/player | `PrototypeHud.cs`, optional UI sprites | Bars update from `PrototypeRunSnapshot.EnemyHp/EnemyMaxHp` and `PlayerHp/PlayerMaxHp`. |
| P0 | Add round result line | `PrototypeHud.cs`, `PrototypeRoomController.ResolveCombatAction` if summary needed | Attack/Defend effects are visible in one line after each click. |
| P1 | Add damage number / block marker | UI/VFX helper | Damage markers appear and expire without layout shift. |
| P1 | Add SFX hooks | presentation lookup / audio source | Attack, Defend, victory use manifest SFX or silent fallback. |
| P1 | Add Skill disabled reason | `PrototypeHud.cs` | Skill state is visibly disabled with short reason or hidden for public recording. |

## Test Expectations

- Attack lowers enemy HP bar and text.
- Defend records `Defend`, reduces/blocks player damage, and shows block marker.
- Skill is disabled by default and cannot invoke combat action.
- Victory closes combat panel before DemoComplete.
- No reward duplication occurs if result summary is regenerated.
