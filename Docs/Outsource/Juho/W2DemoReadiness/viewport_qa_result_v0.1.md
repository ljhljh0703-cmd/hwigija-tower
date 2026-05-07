# PrototypeRoom Portrait + Combat Panel Viewport QA v0.1

## Summary

- Tested commit: `2af1a7c` (`Add PrototypeRoom viewport QA notes`)
- Tested scene: `Assets/_Project/Scenes/PrototypeRoom.unity`
- Target viewport: `1080x1920` portrait
- QA method: Unity Editor PlayMode auto-runner screenshot capture plus pixel review
- Screenshot file paths:
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_01_shop_portrait.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_02_memory_portrait_panel.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_03_combat_portrait_panel.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_04_combat_layering_fixed.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_05_demo_complete_after_combat.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_06_shop_labels_hidden.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_07_memory_labels_hidden.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_08_combat_labels_hidden.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_09_demo_complete_labels_hidden.png`

## Presentation/Cutscene QA Addendum 2026-05-07

- Tested commit: `7092b17` plus local QA fixes for cutscene lifecycle/result summarization
- Target viewport: `1080x1920` portrait
- QA method: PlayMode screenshot capture through a temporary camera-render runner
- Screenshot file paths:
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_10_shop_presentation.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_11_moral_presentation.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_12_memory_cutscene_overlay.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_13_combatgate_cutscene_overlay.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_14_active_combat_presentation.png`
  - `Docs/Outsource/Juho/W2DemoReadiness/screenshots/prototype_room_1080x1920_15_demo_complete_cutscene_overlay.png`

| QA item | Result | Notes |
|---|---:|---|
| Shop presentation state | Pass | Raw encounter/choice IDs hidden; route/HUD readable |
| Moral presentation state | Pass | Choice labels are public demo placeholders, not raw stableIds/textKeys |
| Memory cutscene overlay | Pass | Overlay image displays; placeholder textKey hidden; result summary no longer exposes flag payload |
| CombatGate cutscene overlay | Pass | Combat starts with overlay image; no premature DemoComplete text |
| Active combat | Pass | Cutscene overlay is hidden; combat HP bars/buttons are readable |
| DemoComplete cutscene overlay | Pass | Combat panel closes first; DemoComplete/result state remains readable |

Remaining visual risk: the same full-body Mataios illustration appears both as HUD portrait and cutscene image. This is acceptable for scaffold QA, but a cropped/transparent portrait and separate cutscene image slots are still recommended before public recording.

## Validation

| Check | Result | Evidence |
|---|---:|---|
| `git status` preflight | Pass | clean after discarding `.obsidian/workspace.json` |
| `git diff --check` | Pass | no whitespace errors |
| Fresh EditMode | Pass | `60/60`, `/private/tmp/hwigi-w2-demo-combat-editmode.xml` |
| Fresh PlayMode | Pass | `6/6`, `/private/tmp/hwigi-w2-demo-combat-playmode.xml` |
| Real `1080x1920` screenshot capture | Pass | 9 PNGs captured at `1080 x 1920`; screenshots 06/07/08/09 verify debug labels hidden |

## Pass/Fail Table

| # | QA item | Result | Notes |
|---:|---|---:|---|
| 1 | `PrototypeRoom` scene load | Pass | PlayMode smoke loads scene and finds runtime/controller/player/HUD |
| 2 | Play enter | Pass | Screenshot run entered PlayMode and captured live Game view |
| 3 | Shop -> MoralChoice -> MemoryFragment -> CombatGate route | Pass | Existing route smoke still asserts `[current] Shop` and demo completion |
| 3 | Oversized world/debug node labels hidden | Pass | Screenshots 06/07/08/09 show only small node markers, no large Korean world labels |
| 4 | Mataios portrait displayed | Pass | Visible in all three real screenshots |
| 5 | Portrait does not cover route indicator | Pass | Screenshot 01/02/03: portrait remains lower-left; route indicator remains top-center |
| 5 | Portrait does not cover HUD stats | Pass | Screenshot 01/02/03: top HUD remains readable |
| 5 | Portrait does not cover choice buttons | Pass | Screenshot 02/03: portrait does not block interaction areas |
| 5 | Portrait does not cover result panel | Pass | Screenshot 02/03: result text is dense but not covered by portrait |
| 5 | Portrait does not cover memory panel | Pass | Screenshot 02: memory panel remains readable below portrait band |
| 5 | Portrait does not cover combat panel | Pass | Screenshot 04: combat panel is readable and no longer shares the screen with `demo.complete` overlay/result text |
| 6 | CombatGate shows Attack / Defend / Skill | Pass | PlayMode finds all three combat buttons |
| 7 | Skill disabled | Pass | PlayMode asserts Skill button is not interactable |
| 8 | Attack lowers enemy HP | Pass | PlayMode asserts enemy HP decreases after Attack click |
| 9 | Defend branch result visible | Pass | PlayMode asserts last round result contains `Defend` |
| 10 | Combat final result reaches DemoComplete | Pass | PlayMode asserts `demo.complete` and result message contains `demo.complete` |

## Layout Notes

- The scene `CanvasScaler` is set to `ScaleWithScreenSize`, reference resolution `1080x1920`, match height.
- Mataios portrait is assigned through `PrototypeHud` and safely hidden when the sprite reference is missing.
- Portrait raycast is disabled, so it should not block buttons even if a future layout shifts nearby.
- Combat panel uses the lower-right HUD area and remains separated from the lower-left portrait region.
- Skill is intentionally disabled until a real ability resolver/selection path exists.
- Real screenshots confirm the portrait is safe at `1080x1920`.
- Screenshot 04 confirms combat panel remains visible without `demo.complete` overlay/result text while combat is active.
- Screenshot 05 confirms combat panel closes before `demo.complete` overlay/result text appears.
- Screenshots 06/07/08/09 confirm the recording default hides world/debug node labels while keeping HUD route/status visible.

## Overlap Issues

- Screenshot 01: initial Shop + portrait has no blocking overlap with route/HUD.
- Screenshot 02: MemoryFragment + portrait + memory panel is readable; result text is dense but not portrait-blocked.
- Screenshot 03: CombatGate combat panel + portrait exposes a real overlap issue. `demo.complete` overlay and result text are drawn over the combat panel/stat area, making the combat state visually confusing.
- Screenshot 04: fixed CombatGate active-combat view has no `demo.complete` overlay/result text on top of the combat panel.
- Screenshot 05: fixed post-combat view shows `demo.complete` after the combat panel is closed.
- Screenshots 06/07/08/09: no oversized world/debug node labels visible in Shop, MemoryFragment, active CombatGate, or DemoComplete states.

## Required UI Fixes

- Fixed: `demo.complete` overlay and result text are hidden while `RunState.IsInCombat` is true.
- Fixed: combat panel is active only during active combat, then closes before DemoComplete display.
- Fixed: world/debug node labels are hidden in the default recording view and remain restorable through a development toggle.
- Recommended polish later: reduce result text density or move result panel away from the lower combat panel zone at `1080x1920`.

## Blockers

- No screenshot blocker remains.
- CombatGate demo complete layering blocker is closed by screenshots 04/05.
