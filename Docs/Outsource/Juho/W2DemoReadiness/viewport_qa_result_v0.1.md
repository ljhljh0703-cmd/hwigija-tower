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

## Validation

| Check | Result | Evidence |
|---|---:|---|
| `git status` preflight | Pass | clean after discarding `.obsidian/workspace.json` |
| `git diff --check` | Pass | no whitespace errors |
| Fresh EditMode | Pass | `60/60`, `/private/tmp/hwigi-w2-demo-combat-editmode.xml` |
| Fresh PlayMode | Pass | `6/6`, `/private/tmp/hwigi-w2-demo-combat-playmode.xml` |
| Real `1080x1920` screenshot capture | Pass | 3 PNGs captured at `1080 x 1920` |

## Pass/Fail Table

| # | QA item | Result | Notes |
|---:|---|---:|---|
| 1 | `PrototypeRoom` scene load | Pass | PlayMode smoke loads scene and finds runtime/controller/player/HUD |
| 2 | Play enter | Pass | Screenshot run entered PlayMode and captured live Game view |
| 3 | Shop -> MoralChoice -> MemoryFragment -> CombatGate route | Pass | Existing route smoke still asserts `[current] Shop` and demo completion |
| 4 | Mataios portrait displayed | Pass | Visible in all three real screenshots |
| 5 | Portrait does not cover route indicator | Pass | Screenshot 01/02/03: portrait remains lower-left; route indicator remains top-center |
| 5 | Portrait does not cover HUD stats | Pass | Screenshot 01/02/03: top HUD remains readable |
| 5 | Portrait does not cover choice buttons | Pass | Screenshot 02/03: portrait does not block interaction areas |
| 5 | Portrait does not cover result panel | Pass | Screenshot 02/03: result text is dense but not covered by portrait |
| 5 | Portrait does not cover memory panel | Pass | Screenshot 02: memory panel remains readable below portrait band |
| 5 | Portrait does not cover combat panel | Fail | Screenshot 03: portrait does not cover it, but `demo.complete` overlay/result text overlap the combat panel |
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
- Real screenshots confirm the portrait is safe at `1080x1920`; the remaining visible problem is not portrait placement but combat/demo-complete HUD layering.

## Overlap Issues

- Screenshot 01: initial Shop + portrait has no blocking overlap with route/HUD.
- Screenshot 02: MemoryFragment + portrait + memory panel is readable; result text is dense but not portrait-blocked.
- Screenshot 03: CombatGate combat panel + portrait exposes a real overlap issue. `demo.complete` overlay and result text are drawn over the combat panel/stat area, making the combat state visually confusing.

## Required UI Fixes

- Fix combat/demo-complete layering before external demo recording.
- Recommended minimal fix: suppress or defer `demo.complete` overlay while `RunState.IsInCombat` is true, then show completion after combat panel closes.
- Recommended polish later: reduce result text density or move result panel away from the lower combat panel zone at `1080x1920`.

## Blockers

- No screenshot blocker remains.
- UI blocker: CombatGate screenshot confirms `demo.complete`/result overlay conflict with the combat panel.
