# Debug Label Removal Spec v0.1

## Decision

- Demo recording default: hide large world/debug node labels.
- Development debugging: keep the labels behind a toggle.
- HUD route/status: keep visible.
- Encounter choice/result/combat panel: keep visible.
- Mataios portrait: keep visible.

## Hide

| target | reason | likely source |
|---|---|---|
| World-space node label child named `Label` | Oversized labels dominate 1080 x 1920 screenshots. | `Assets/_Project/Scripts/Run/PrototypeSceneRuntimeBuilder.cs` `CreateNode` |
| Large Korean node names in scene screenshots | They are debug navigation aids, not public demo UI. | Runtime-generated `TextMesh` labels and/or serialized scene label objects |
| Any recording-mode duplicate node labels | Route/status already communicates progression. | `PrototypeSceneRuntimeBuilder` or `PrototypeRoom.unity` if serialized labels remain |

## Keep

| target | reason |
|---|---|
| HUD route/status | Needed to verify Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete. |
| HUD stats | Needed to show HP/ATK/Mental/Gold/Glitch/Affinity state changes. |
| Encounter choices | Required for the interactive route. |
| Result text | Required for QA, but may need a later recording-mode short copy pass. |
| Combat panel | Core W2 demo proof. |
| Mataios portrait | Core companion presence proof. |

## Toggle Proposal

| field | default | placement |
|---|---:|---|
| `showDemoNodeDebugLabels` | false | `PrototypeSceneRuntimeBuilder` serialized field |
| Optional method | `SetDemoNodeDebugLabelsVisible(bool visible)` | helper for PlayMode tests or debug console |

Implementation note: if labels are created at runtime, skip creating the `TextMesh` label when the toggle is false. If serialized scene labels already exist, set them inactive on scene startup when recording mode is active.

## Acceptance Criteria

- At 1080 x 1920, no oversized world-space node names are visible in the recording default.
- Colored node markers and colliders still work.
- Player can still identify route progress via HUD route/status.
- Shop, MoralChoice, MemoryFragment, CombatGate, and DemoComplete route still completes.
- CombatGate still shows Attack and Defend as interactable and Skill as disabled.
- DemoComplete does not appear while `RunState.IsInCombat` is true.
- Enabling the debug toggle restores node labels for development.

## PlayMode QA

| test expectation | note |
|---|---|
| Default scene starts with debug labels hidden | Assert no active `TextMesh` child named `Label` under runtime nodes, or assert configured label objects are inactive. |
| Toggle path shows labels | In a dedicated test, enable `showDemoNodeDebugLabels` and assert labels are present. |
| Existing route smoke still passes | Reuse `PrototypeRoom_BakedEncounterChoicesUseButtonFlow`. |
| Manual combat smoke still passes | Reuse `PrototypeRoom_CombatGateShowsInteractiveCombatPanel`. |

## Screenshot QA

Capture 1080 x 1920 after implementation:

1. Initial Shop/portrait state.
2. MemoryFragment after unlock.
3. Active CombatGate panel.
4. DemoComplete after combat.

Pass condition: screenshots should show no large world node labels, while route/status and interaction UI remain readable.

## Suggested Commit Message

`Hide PrototypeRoom debug node labels for demo recording`
