# PrototypeRoom Portrait + Combat Panel Viewport QA v0.1

## Summary

- Tested commit: `2912ea0` (`Refresh W2 demo readiness docs after combat UI integration`)
- Tested scene: `Assets/_Project/Scenes/PrototypeRoom.unity`
- Target viewport: `1080x1920` portrait
- QA method: Unity fresh PlayMode smoke run plus static scene/HUD layout review
- Screenshot file path: not captured in this pass

## Validation

| Check | Result | Evidence |
|---|---:|---|
| `git status` preflight | Pass | clean after discarding `.obsidian/workspace.json` |
| `git diff --check` | Pass | no whitespace errors |
| Fresh EditMode | Pass | `60/60`, `/private/tmp/hwigi-w2-demo-combat-editmode.xml` |
| Fresh PlayMode | Pass | `6/6`, `/private/tmp/hwigi-w2-demo-combat-playmode.xml` |

## Pass/Fail Table

| # | QA item | Result | Notes |
|---:|---|---:|---|
| 1 | `PrototypeRoom` scene load | Pass | PlayMode smoke loads scene and finds runtime/controller/player/HUD |
| 2 | Play enter | Pass | Fresh PlayMode run completed |
| 3 | Shop -> MoralChoice -> MemoryFragment -> CombatGate route | Pass | Existing route smoke still asserts `[current] Shop` and demo completion |
| 4 | Mataios portrait displayed | Pass | `PrototypeHud.PortraitVisible` passes after scene load and run begin |
| 5 | Portrait does not cover route indicator | Pass | Portrait anchors bottom-left, route text anchors top center |
| 5 | Portrait does not cover HUD stats | Pass | Portrait vertical band starts below top HUD bands |
| 5 | Portrait does not cover choice buttons | Pass | Choice buttons anchor lower center; portrait is left side and non-raycast |
| 5 | Portrait does not cover result panel | Pass | Result panel remains centered above choice area; portrait is offset left |
| 5 | Portrait does not cover memory panel | Risk | Portrait and memory/combat lower HUD share nearby vertical space; no automated pixel screenshot captured |
| 5 | Portrait does not cover combat panel | Pass | Combat panel anchors right side, portrait anchors left side |
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

## Overlap Issues

- No blocking overlap was found from the current serialized anchors and PlayMode smoke coverage.
- Residual visual risk: memory panel and portrait are both lower HUD elements. A human screenshot pass should still confirm exact readability on device or Game view because this pass did not capture pixels.

## Required UI Fixes

- None required before the next demo smoke pass.
- Recommended polish later: capture a real Game view screenshot at `1080x1920` and adjust memory/combat panel vertical spacing if the portrait visually competes with memory text.

## Blockers

- Screenshot automation is not currently wired into the project; this pass records QA notes and automated PlayMode evidence only.
