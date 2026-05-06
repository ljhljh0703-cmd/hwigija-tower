# Next Dev Order: Debug Node Labels v0.1

## Goal

Hide or shrink `PrototypeRoom` world/debug node labels for demo recording while preserving HUD route/status, interaction choices, result text, combat panel, and Mataios portrait.

## Likely Files

| file | reason |
|---|---|
| `Assets/_Project/Scripts/Run/PrototypeSceneRuntimeBuilder.cs` | Runtime node labels are created as `TextMesh` children named `Label`. |
| `Assets/_Project/Scenes/PrototypeRoom.unity` | Only if serialized scene labels remain visible after runtime builder change. |
| `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs` | Add/adjust coverage for hidden-label default and route still passing. |
| `Docs/Outsource/Juho/W2DemoReadiness/viewport_qa_result_v0.1.md` | Update after screenshot QA, not before implementation. |

## Implementation Constraints

- Do not change encounter data, ScriptableObjects, combat balance, or route order.
- Do not write NPC dialogue or memory prose.
- Do not hide HUD route/status.
- Do not hide encounter choice buttons.
- Do not hide combat panel or Skill disabled state.
- Do not introduce nondeterministic behavior.
- Keep the labels available through a dev toggle.

## Suggested Implementation

1. Add serialized bool `showDemoNodeDebugLabels` to `PrototypeSceneRuntimeBuilder`, default false.
2. In `CreateNode`, create the `TextMesh` label only when the toggle is true, or create it inactive when false.
3. If a serialized scene label exists outside the builder path, disable it during scene startup when the toggle is false.
4. Add a PlayMode assertion that default 1080 x 1920 recording mode has no active world node label text.
5. Keep existing route and combat smoke tests passing.

## Acceptance Criteria

- Default PlayMode view has no oversized world-space Korean node labels.
- HUD route/status remains visible and readable.
- Mataios portrait remains visible.
- Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete still completes.
- Active CombatGate shows Attack/Defend usable and Skill disabled.
- DemoComplete appears only after combat panel closes.
- Enabling the debug toggle restores node labels for development.

## PlayMode Test Expectation

- Add or update a PlayMode test named similar to `PrototypeRoom_HidesDebugNodeLabelsByDefault`.
- The test should load `PrototypeRoom`, wait one frame, and assert active world label objects are absent/inactive in default config.
- Existing `PrototypeRoom_CombatGateShowsInteractiveCombatPanel` should continue to pass.

## Screenshot QA Expectation

After implementation, capture:

- Shop + portrait at 1080 x 1920.
- MemoryFragment + portrait + memory/status.
- Active CombatGate.
- DemoComplete after combat.

Report:

- no oversized world labels,
- no CombatGate/demo.complete overlap,
- portrait not blocking route/status/choices/combat panel,
- raw result/memory density noted separately if still visible.

## Forbidden Items

- No scene/SO route rewrites.
- No final NPC lines.
- No memory fragment prose changes.
- No AI/model runtime changes.
- No Android build setting changes.

## Commit Message

`Hide PrototypeRoom debug node labels for demo recording`

## Report Format

```md
## Changed Files
- ...

## Validation
- PlayMode:
- screenshot QA:
- git diff --check:

## Recording Impact
- hidden:
- kept:
- remaining visual risks:

## Next
- ...
```
