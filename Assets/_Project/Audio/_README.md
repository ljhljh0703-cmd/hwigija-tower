# Audio Intake Rules

This folder is the drop zone for prototype audio assets. Keep runtime bindings data-driven through `SO_AudioCueCatalog`.

## Folders
- `Music/Lobby`, `Music/Exploration`, `Music/Combat`, `Music/Boss`, `Music/Rest`, `Music/Shop`, `Music/Ending`
- `Ambience/Floor01` through `Ambience/Floor05`
- `SFX/UI`, `SFX/Combat`, `SFX/Event`, `SFX/Shop`, `SFX/Rest`, `SFX/Memory`, `SFX/Cutscene`

## Naming
- Music: `bgm_<context>_<variant>.wav`
- Ambience: `amb_floor##_ <variant>.wav`
- SFX: `sfx_<category>_<action>.wav`

Use lowercase ASCII filenames. Avoid spaces.

## Import
- Music/Ambience: streaming or compressed-in-memory depending final size.
- SFX: decompress on load for short clips.
- Loop metadata lives in `AudioCueData`, not filename suffixes.

No final music or sound design is implied by placeholder folders.
