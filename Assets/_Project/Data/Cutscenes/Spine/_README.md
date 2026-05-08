# Spine Cutscene Data Slots

This folder is reserved for future ScriptableObject data that maps cutscene ids to Spine exports.

Current runtime behavior:

- `CutsceneData` remains sprite based.
- `animationCutsceneId` and `animationAssetPath` are string identifiers only.
- `fallbackSprite` is used when Spine runtime is unavailable.
- No `Spine.*` namespace or runtime package dependency is allowed at this stage.

Expected ids:

- `CUT_MEMORY_03_FRACTURE`
- `CUT_FINAL_BOSS_REVEAL`
- `CUT_ENDING_CHOICE`
- `CUT_ENDING_REST`
- `CUT_ENDING_CONTINUE`
