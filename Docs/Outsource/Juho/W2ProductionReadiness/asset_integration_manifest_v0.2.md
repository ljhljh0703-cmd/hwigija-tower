# Asset Integration Manifest v0.2

## Scope

- Unity was not run.
- Manifest is organized by actual W2 stableId/usage targets.
- This replaces the broader status table with Unity slot-oriented intake rows.
- It does not import or modify assets.

## Current Known Assets

| asset | status | evidence |
|---|---|---|
| Mataios full-body art | imported and scene-bound | `Assets/_Project/Art/Characters/마타이오스 전신.png`; `PrototypeRoom.unity` `mataiosPortrait` reference |
| Audio payloads | missing | `Assets/_Project/Audio/**` contains folders/readme/meta only |
| Environment/node/UI art | missing | `Assets/_Project/Art/Environments`, `Nodes`, `UI`, `Icons`, `VFX` contain no payload assets |

## Import Setting Defaults

| asset type | recommended Unity import |
|---|---|
| Character/portrait PNG | Texture Type Sprite (2D and UI), Sprite Mode Single, sRGB on, Alpha Is Transparency on, mipmaps off, max 2048 or 4096 for source, compression Normal/High Quality after review |
| Background PNG | Texture Type Sprite or Default UI texture, sRGB on, mipmaps off for UI, max 2048, compression Normal |
| UI icon PNG | Sprite Single, transparent background, max 512 or 1024, no mipmaps |
| VFX sprite sheet | Sprite Multiple, transparent background, max 2048, no mipmaps, pivot documented |
| SFX WAV source | Mono unless spatialized, 44.1/48 kHz, short clips PCM/ADPCM, normalize peak around -3 dB |
| BGM/ambience | OGG/Vorbis or WAV source with streaming/compressed import, seamless loop metadata documented |

## StableId / Usage Manifest

| stableId / usage | Unity slot | file name | folder | resolution / length | transparent bg | import recommendation | status / owner |
|---|---|---|---|---|---|---|---|
| Global Mataios portrait S0-S2 | `PrototypeHud.mataiosPortrait` / future `PresentationData.portraitSprite` | `Character_Mataios_S0S2_Portrait_Crop.png` | `Assets/_Project/Art/Characters/` | 768x1024 or 1024x1024 crop | yes preferred | Sprite Single, alpha on, max 2048, no mipmaps | optional crop, outsource + dev |
| Global demo room background | future `PresentationData.backgroundSprite` fallback | `BG_PrototypeRoom_DarkTower_1080x1920.png` | `Assets/_Project/Art/Environments/` | 1080x1920 | no | Sprite/Default, sRGB, max 2048, no mipmaps | missing, outsource |
| `ENC_SHOP_01` | encounter background | `BG_Encounter_Shop_01.png` | `Assets/_Project/Art/Environments/` | 1080x1920 or 1080x960 safe crop | no | Sprite/Default, max 2048 | missing, outsource |
| `ENC_SHOP_01` | node/icon accent | `Icon_Node_Shop_01.png` | `Assets/_Project/Art/Nodes/` | 256x256 | yes | Sprite Single, max 512 | missing, outsource |
| `ENC_SHOP_01` | choice confirm / buy cue | `SFX_UI_Purchase_01.wav` | `Assets/_Project/Audio/SFX/` | < 1.0s | n/a | Mono, short clip compressed/decompressed on load | missing, outsource |
| `ENC_MORAL_CHOICE_01` | encounter background | `BG_Encounter_MoralChoice_01.png` | `Assets/_Project/Art/Environments/` | 1080x1920 | no | Sprite/Default, max 2048 | missing, outsource |
| `ENC_MORAL_CHOICE_01` | moral prop/icon | `Icon_Encounter_MoralChoice_01.png` | `Assets/_Project/Art/Icons/` | 512x512 | yes | Sprite Single, alpha on | missing, outsource |
| `ENC_MORAL_CHOICE_01` | choice confirm cue | `SFX_UI_ChoiceConfirm_01.wav` | `Assets/_Project/Audio/SFX/` | < 0.8s | n/a | Mono, short clip | missing, outsource |
| `ENC_MORAL_CHOICE_01` | glitch/affinity delta cue | `SFX_UI_GlitchDelta_01.wav` | `Assets/_Project/Audio/SFX/` | < 1.0s | n/a | Mono, short clip | missing, outsource |
| `ENC_MEMORY_FRAGMENT_01` | encounter background | `BG_Encounter_MemoryFragment_01.png` | `Assets/_Project/Art/Environments/` | 1080x1920 | no | Sprite/Default, max 2048 | missing, outsource |
| `ENC_MEMORY_FRAGMENT_01` | memory object prop | `Prop_MemoryFragment_01.png` | `Assets/_Project/Art/Icons/` or `Art/Environments/Props/` | 512x512 or 1024x1024 | yes | Sprite Single, alpha on | missing, outsource |
| `ENC_MEMORY_FRAGMENT_01` | memory unlock cue | `SFX_UI_MemoryUnlock_01.wav` | `Assets/_Project/Audio/SFX/` | 1.0-2.0s | n/a | Mono/stereo, short clip | missing, outsource |
| `MEM_FRAGMENT_01` | memory fragment icon | `Icon_MemoryFragment_01.png` | `Assets/_Project/Art/Icons/` | 512x512 | yes | Sprite Single, alpha on | missing, outsource |
| `MEM_FRAGMENT_01` | cutscene focus art | `Cutscene_MemoryFragment_01_Focus.png` | `Assets/_Project/Art/VFX/` or `Art/Environments/` | 1080x1080 safe center | yes preferred | Sprite Single, alpha on | missing, outsource |
| `ENC_COMBAT_GATE_01` | combat start background | `BG_Encounter_CombatGate_01.png` | `Assets/_Project/Art/Environments/` | 1080x1920 | no | Sprite/Default, max 2048 | missing, outsource |
| `ENC_COMBAT_GATE_01` | combat start stinger | `SFX_Combat_Start_01.wav` | `Assets/_Project/Audio/SFX/` | 0.8-1.5s | n/a | Mono/stereo short clip | missing, outsource |
| `ENEMY_FRACTURE_HOUND` | enemy sprite | `Character_Enemy_FractureHound_01.png` | `Assets/_Project/Art/Characters/` | 768x768 or 1024x1024 | yes | Sprite Single, alpha on, max 2048 | missing, outsource |
| `ENEMY_FRACTURE_HOUND` | enemy portrait/icon | `Icon_Enemy_FractureHound_01.png` | `Assets/_Project/Art/Icons/` | 256x256 | yes | Sprite Single, max 512 | missing, outsource |
| `ENEMY_FRACTURE_HOUND` | attack SFX | `SFX_Enemy_FractureHound_Attack_01.wav` | `Assets/_Project/Audio/SFX/` | < 0.8s | n/a | Mono short clip | missing, outsource |
| `ENEMY_FRACTURE_HOUND` | defeat SFX | `SFX_Enemy_FractureHound_Defeat_01.wav` | `Assets/_Project/Audio/SFX/` | 0.8-1.5s | n/a | Mono/stereo short clip | missing, outsource |
| Combat player attack | Attack button feedback | `SFX_Combat_PlayerAttack_01.wav` | `Assets/_Project/Audio/SFX/` | < 0.5s | n/a | Mono short clip | missing, outsource |
| Combat defend | Defend button feedback | `SFX_Combat_PlayerDefend_01.wav` | `Assets/_Project/Audio/SFX/` | < 0.5s | n/a | Mono short clip | missing, outsource |
| Combat damage number | UI/VFX feedback | `VFX_DamageNumber_Default.prefab` or `VFX_DamageNumber_Default.png` | `Assets/_Project/Art/VFX/` | prefab or 512 atlas | yes | Sprite atlas or prefab | missing, dev/outsource |
| Combat HP bar | UI art | `UI_Combat_HPBar_Frame.png`, `UI_Combat_HPBar_Fill.png` | `Assets/_Project/Art/UI/` | 512x64 each | yes | Sprite Single, alpha on | missing, outsource |
| `DEMO_COMPLETE` | completion overlay | `UI_DemoComplete_Overlay.png` | `Assets/_Project/Art/UI/` | 1080x420 or scalable panel | yes | Sprite Single, alpha on | missing, outsource |
| `DEMO_COMPLETE` | completion sting | `SFX_UI_DemoComplete_01.wav` | `Assets/_Project/Audio/SFX/` | 1.0-2.5s | n/a | Stereo short clip | missing, outsource |

## Slot Mapping Needed in Code

| slot | suggested data home | fallback |
|---|---|---|
| encounter background | `PresentationData` keyed by encounter stableId | dark runtime background |
| encounter portrait override | `PresentationData` keyed by NPC stage | `PrototypeHud.mataiosPortrait` |
| encounter SFX | lightweight presentation lookup keyed by stableId/event | silent |
| cutscene storyboard | SO list keyed by trigger stableId | skip cutscene and continue route |
| raw key display | HUD recording/debug config | default off |

## Acceptance Criteria

- Every row has an imported file or an explicit `missing` status before W2 recording QA.
- Required recording assets can fall back silently only if the runbook marks the clip as internal QA.
- Asset filenames use role-first English names, not source chat filenames.
- Imported portrait/cutscene/UI assets do not create new scene references without a matching manifest row.
