# Spine Runtime Export Intake

This folder is reserved for future Spine runtime exports. Spine runtime is not installed yet, so these files are not required for current scene/test pass.

## Cutscene folders

- `CUT_MEMORY_03_FRACTURE/`
- `CUT_FINAL_BOSS_REVEAL/`
- `CUT_ENDING_CHOICE/`
- `CUT_ENDING_REST/`
- `CUT_ENDING_CONTINUE/`

## Naming

Each cutscene export should use the same cutscene id:

- `spine_<cutscene_id>.spine`
- `spine_<cutscene_id>.json`
- `spine_<cutscene_id>.atlas.txt`
- `spine_<cutscene_id>.png`

Example:

- `CUT_FINAL_BOSS_REVEAL/spine_cut_final_boss_reveal.json`
- `CUT_FINAL_BOSS_REVEAL/spine_cut_final_boss_reveal.atlas.txt`
- `CUT_FINAL_BOSS_REVEAL/spine_cut_final_boss_reveal.png`

Do not add Spine runtime package or namespace references until runtime integration is explicitly approved.
