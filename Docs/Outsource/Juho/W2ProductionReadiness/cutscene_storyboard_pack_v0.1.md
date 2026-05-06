# Cutscene Storyboard Pack v0.1

## Scope

- No final NPC dialogue is written here.
- No final memory prose is written here.
- Story truth and ending reveal are intentionally absent.
- Design target: SO-based lightweight cutscene player, not Timeline full production.
- Each shot defines visual intent, `textKey`, duration, SFX cue, and transition only.

## Suggested Lightweight Cutscene Data

| field | purpose |
|---|---|
| `cutsceneStableId` | Unique cutscene id. |
| `triggerStableId` | Encounter/choice/result stableId that starts the cutscene. |
| `blockingMode` | `BlocksRouteUntilComplete` for W2. |
| `shots[]` | Ordered shot list. |
| `fallbackPolicy` | If assets/text missing, skip visual but continue route deterministically. |

Shot fields:

| field | allowed value |
|---|---|
| `visualIntent` | Direction only; no final art dependency. |
| `textKey` | Placeholder key. Writer-owned text marked `작성자 입력 필요`. |
| `durationSeconds` | Fixed deterministic duration. |
| `sfxCue` | Manifest cue id or `none`. |
| `transition` | `fade`, `hold`, `cut`, `panel-slide`, `flash`. |

## Cutscene 1: Memory Unlock

| metadata | value |
|---|---|
| `cutsceneStableId` | `CUTSCENE_MEM_FRAGMENT_01_UNLOCK` |
| `triggerStableId` | `CHOICE_MEMORY_01_UNLOCK` |
| related stableId | `MEM_FRAGMENT_01` |
| blocking | yes, before route advances visually to CombatGate |

| shot | visual intent | textKey | duration | sfx cue | transition |
|---:|---|---|---:|---|---|
| 1 | Background darkens; memory object silhouette enters center focus. | `CUTSCENE_MEM_FRAGMENT_01_SHOT_01_TEXT` - 작성자 입력 필요 | 0.8s | `SFX_UI_MemoryUnlock_01` | fade in |
| 2 | Mataios portrait remains lower-left; memory object glow pulses once. | `CUTSCENE_MEM_FRAGMENT_01_SHOT_02_TEXT` - 작성자 입력 필요 | 1.2s | `SFX_NPC_RecallStart_01` | hold |
| 3 | Memory status increments to 1; route next marker becomes CombatGate. | `CUTSCENE_MEM_FRAGMENT_01_SHOT_03_TEXT` - 작성자 입력 필요 | 0.8s | `SFX_UI_ChoiceConfirm_01` | panel-slide |

Acceptance:

- If text is missing, show no raw textKey in recording mode.
- The memory fragment unlock should not be applied twice if the cutscene is replayed.
- Route continues only after cutscene completion or deterministic skip.

## Cutscene 2: Combat Start

| metadata | value |
|---|---|
| `cutsceneStableId` | `CUTSCENE_COMBAT_GATE_01_START` |
| `triggerStableId` | `CHOICE_COMBAT_01_ENGAGE` |
| related stableId | `ENC_COMBAT_GATE_01`, `ENEMY_FRACTURE_HOUND` |
| blocking | yes, before combat panel buttons become interactable |

| shot | visual intent | textKey | duration | sfx cue | transition |
|---:|---|---|---:|---|---|
| 1 | CombatGate background shifts colder; enemy silhouette appears on right. | `CUTSCENE_COMBAT_GATE_01_SHOT_01_TEXT` - 작성자 입력 필요 | 0.7s | `SFX_Combat_Start_01` | cut |
| 2 | Enemy focus: `ENEMY_FRACTURE_HOUND` slot flashes; HP bar initializes. | `CUTSCENE_COMBAT_GATE_01_SHOT_02_TEXT` - 작성자 입력 필요 | 0.8s | `SFX_Enemy_FractureHound_Attack_01` low volume | flash |
| 3 | Combat panel slides in; Attack/Defend enabled, Skill disabled. | `CUTSCENE_COMBAT_GATE_01_SHOT_03_TEXT` - 작성자 입력 필요 | 0.5s | `SFX_UI_ChoiceConfirm_01` | panel-slide |

Acceptance:

- Combat buttons are not clickable until the cutscene ends.
- DemoComplete and bottom result text remain hidden during active combat.
- Missing enemy art falls back to placeholder silhouette without route failure.

## Cutscene 3: Demo Complete

| metadata | value |
|---|---|
| `cutsceneStableId` | `CUTSCENE_DEMO_COMPLETE` |
| `triggerStableId` | `DEMO_COMPLETE` or combat `victory` when `DemoStatus == demo.complete` |
| related stableId | `DEMO_COMPLETE` |
| blocking | yes, after combat panel closes |

| shot | visual intent | textKey | duration | sfx cue | transition |
|---:|---|---|---:|---|---|
| 1 | Combat panel fades out; final route markers settle to complete. | `CUTSCENE_DEMO_COMPLETE_SHOT_01_TEXT` - 작성자 입력 필요 | 0.7s | `SFX_Combat_Victory_01` | fade |
| 2 | Center overlay appears with restrained completion title. | `CUTSCENE_DEMO_COMPLETE_SHOT_02_TEXT` - 작성자 입력 필요 | 1.2s | `SFX_UI_DemoComplete_01` | fade in |
| 3 | Mataios portrait remains visible; no final lore reveal. | `CUTSCENE_DEMO_COMPLETE_SHOT_03_TEXT` - 작성자 입력 필요 | 1.0s | `none` | hold |

Acceptance:

- Trigger occurs once after combat victory and after `IsInCombat` is false.
- No ending truth, NPC final line, or memory prose is introduced.
- If writer text is missing, overlay can show approved system key or no text, never raw placeholder.

## Trigger Rules

| trigger | cutscene | stableId source |
|---|---|---|
| `CHOICE_MEMORY_01_UNLOCK` applied | Memory Unlock | encounter choice stableId |
| `CHOICE_COMBAT_01_ENGAGE` applied | Combat Start | encounter choice stableId |
| `DemoStatus == demo.complete` after combat victory | Demo Complete | run state + combat result |

## Determinism Rules

- Shot durations are fixed.
- Cutscene trigger keys are stableIds, not display text.
- Replaying an already resolved encounter should not replay blocking cutscene unless explicit debug replay is requested.
- Missing asset/text fallback must be deterministic and nonfatal.
