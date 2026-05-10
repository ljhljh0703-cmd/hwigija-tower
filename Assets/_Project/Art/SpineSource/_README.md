# Spine Source Intake

This folder is for authoring-source files only. Do not place runtime Spine exports here.

## Source size

- Source canvases should be `2000x2000`.
- Keep the character/action centered with enough transparent padding for future camera movement.
- Commit only approved lightweight sources. Large PSD/checkpoint/source files need asset policy approval first.

## Cutscene folders

- `CUT_MEMORY_03_FRACTURE/`
- `CUT_FINAL_BOSS_REVEAL/`
- `CUT_ENDING_CHOICE/`
- `CUT_ENDING_REST/`
- `CUT_ENDING_CONTINUE/`

## Naming

Use lowercase exported filenames inside the matching cutscene folder:

- `spine_<cutscene_id>_source_2000.psd`
- `spine_<cutscene_id>_source_2000.png`

Example:

- `CUT_FINAL_BOSS_REVEAL/spine_cut_final_boss_reveal_source_2000.png`

Large raw source files must follow the project asset policy before commit.
