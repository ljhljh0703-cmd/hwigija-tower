# Demo Recording Runbook v0.1

## Scope

- This runbook was written without running Unity.
- It is based on existing QA notes and screenshots from `Docs/Outsource/Juho/W2DemoReadiness/`.
- Unity owner must execute and update the result after recording.
- Target reference: `57077ee Fix CombatGate demo complete UI layering` or later.

## Preflight

| check | expected |
|---|---|
| Branch | `Juho/Codex` containing `57077ee` or later |
| Scene | `Assets/_Project/Scenes/PrototypeRoom.unity` |
| Target Game View | 1080 x 1920 portrait |
| `PrototypeRoomController.autoResolveCombat` | false for the manual recording path |
| Skill button | visible but disabled |
| Known visual blocker | large world/debug node labels must be hidden or reduced before public capture |

## Unity Setup

1. Open Unity with the project root.
2. Open `Assets/_Project/Scenes/PrototypeRoom.unity`.
3. Set Game View to a portrait 1080 x 1920 resolution.
4. Confirm the Canvas Scaler uses the existing 1080 x 1920 portrait reference setup.
5. Confirm the Mataios portrait sprite is assigned on the HUD path.
6. Confirm debug node label default is hidden after the next dev task. If not implemented yet, treat this as an internal capture only.

## Recording Path

| step | action | expected screen |
|---:|---|---|
| 1 | Enter Play Mode | Dark prototype room, HUD route/status visible, Mataios portrait lower-left. |
| 2 | Move to the Shop node and interact | Shop route becomes current; choice/result UI appears without blocking portrait. |
| 3 | Resolve the Shop step | Route advances toward MoralChoice. If buy is disabled due Gold, use the approved review setup or choose the route-safe option used by the current demo scene. |
| 4 | Move to the battle node for MoralChoice and interact | Choice buttons appear; result text updates after selection. |
| 5 | Resolve MoralChoice | Route advances to MemoryFragment; Affinity/Glitch deltas appear in debug/result text. |
| 6 | Resolve MemoryFragment by selecting unlock | Memory count becomes 1; current placeholder title/body keys may appear until writer text is approved. |
| 7 | Resolve CombatGate by selecting Engage | Combat panel opens; Attack and Defend are enabled; Skill is disabled. |
| 8 | Press Attack once | Enemy HP decreases; combat text updates round/action state. |
| 9 | Press Defend once | Last action/result reflects Defend; player damage is reduced. |
| 10 | Press Attack until combat ends | Combat panel closes before demo.complete appears. |
| 11 | Hold on DemoComplete | `demo.complete` and `Demo Route Complete` are readable; no active combat panel remains. |

## Expected Capture Beats

| time | beat |
|---:|---|
| 0-5s | Portrait + route introduction. |
| 5-12s | Shop choice/result, showing the route is interactive. |
| 12-18s | MoralChoice and MemoryFragment update stats/memory. |
| 18-27s | CombatGate opens with Attack/Defend/Skill. |
| 27-35s | Attack/Defend actions update HP and combat result. |
| 35-40s | Combat panel closes, then DemoComplete appears. |

## Failure Checks

| failure | check files/state |
|---|---|
| Combat panel and demo.complete appear together | `Assets/_Project/Scripts/UI/PrototypeHud.cs` `UpdateDemoCompletePanel`, `UpdateResultVisibility`, `UpdateCombatPanel`; compare screenshots 04/05. |
| Skill is clickable too early | `PrototypeHud.UpdateCombatPanel`; PlayMode test `PrototypeRoom_CombatGateShowsInteractiveCombatPanel`. |
| Portrait covers buttons/result | `PrototypeHud.EnsurePortraitImage`; screenshots 01-05; capture at 1080 x 1920. |
| Route does not advance | `PrototypeRoomController.ResolveEncounterChoice`; `PrototypeRunState.UpdateDemoProgression`; PlayMode smoke route. |
| Shop buy blocks route | Confirm whether the recording route uses review gold setup, skip path, or another available route-safe option. |
| Memory text looks final but is placeholder | `design/memory-fragments.md`; GDD OQ-006; writer owner decision required. |

## 30-Second Explanation Script

This is the W2 vertical slice for `회귀자는 탑을 오른다`. The route shows a one-minute prototype path: shop, moral choice, memory fragment, and CombatGate. Mataios is already visible as a companion portrait, while the NPC model remains behind a deterministic fallback until the on-device package passes evaluation. The combat panel demonstrates manual Attack and Defend, with Skill intentionally disabled until real ability routing is connected. The important recording check is that combat ends first, then `demo.complete` appears cleanly.

## Hide During Public Recording

- Oversized world/debug node labels such as the large Korean node names.
- Raw stableId-heavy debug lines if the goal is a polished public clip.
- Placeholder memory title/body keys unless the writer approves them for the recording.
- Any model/eval placeholder folders or file explorer windows.
- Any disabled Skill behavior explanation unless it is framed as W2 scope.
