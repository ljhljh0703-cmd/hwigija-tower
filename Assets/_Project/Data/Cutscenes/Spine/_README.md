# Spine Cutscene Data Slots

This folder is reserved for future ScriptableObject data that maps cutscene ids to Spine exports.

Current runtime behavior:

- `CutsceneData` remains sprite based.
- `animationCutsceneId` and `animationAssetPath` are string identifiers only.
- `fallbackSprite` is used when Spine runtime is unavailable.
- No `Spine.*` namespace or runtime package dependency is allowed at this stage.

## Placeholder binding assets

Use `SO_CutsceneSpine_<CUTSCENE_ID>.asset` for early data binding. Required fields:

- `cutsceneId`: stable event id, for example `CUT_FINAL_BOSS_REVEAL`
- `animationCutsceneId`: same stable id until the Spine runtime integration defines a loader contract
- `animationAssetPath`: future export path under `Assets/_Project/Spine/<CUTSCENE_ID>/`
- `fallbackSprite`: static sprite used by the current cutscene player

These assets may be referenced from presentation slots before the matching Spine export exists.

Expected ids:

- `CUT_MEMORY_03_FRACTURE`
- `CUT_FINAL_BOSS_REVEAL`
- `CUT_ENDING_CHOICE`
- `CUT_ENDING_REST`
- `CUT_ENDING_CONTINUE`
