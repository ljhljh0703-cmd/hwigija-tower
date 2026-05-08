# Next Dev Orders Completion v0.1

## Scope

- Baseline: `origin/Proto` `d94697f`.
- Goal: give the next development session implementation-ready orders for game completion.
- Unity not run in this outsource pass.
- No runtime code or SO assets were changed by this document.

## P0 Orders

### P0-1 Ending Choice State Machine

| Field | Order |
|---|---|
| Goal | Implement `run.clear -> ending.choice.ready -> ending.rest / ending.continue`. |
| Inputs | GDD D-013, `ending_choice_spec_v0.1.md`, existing `run.clear`, `run.failed`, `run.restartReady`. |
| Work | Add ending state ids, branch selection handling, duplicate guard, placeholder key display, and save-once branch record. |
| Acceptance | Ending choice opens only after final boss clear; Rest and Continue produce distinct states; no final prose required. |
| Tests | EditMode for state transitions and duplicate guard; PlayMode for final boss clear to ending choice shell. |
| Forbidden | Do not write final NPC/ending prose; do not change schema unless explicitly approved. |

### P0-2 Android Smoke Candidate

| Field | Order |
|---|---|
| Goal | Verify APK path and first Android device smoke. |
| Inputs | `android_candidate_qa_plan_v0.1.md`, `BuildScript.BuildAndroid`, Android Build Support status. |
| Work | Install Android Build Support if missing, generate APK, run portrait/touch/first-run/shop/combat/restart smoke. |
| Acceptance | APK installs and reaches playable route; portrait safe area and touch controls pass basic smoke. |
| Tests | Android build harness plus device smoke report. |
| Forbidden | Do not treat deterministic fallback as real model acceptance. |

### P0-3 Deterministic Fallback Lock

| Field | Order |
|---|---|
| Goal | Ensure all completion states have safe fallback reaction keys before real model is accepted. |
| Inputs | `ai_fallback_completion_matrix_v0.1.md`, existing LLM fallback/cache tests. |
| Work | Wire fallback prompt profiles for shop, moral, memory, combat, run.failed, run.clear, ending.rest, ending.continue. |
| Acceptance | Missing model/tokenizer/native plugin never breaks route; default UI never exposes provider failure. |
| Tests | EditMode fallback matrix tests with same run/prompt replay and distinct ending state hashes. |
| Forbidden | Do not add cloud secrets/endpoints in this pass; do not generate final dialogue. |

## P1 Orders

### P1-1 Floor 3-5 Route Completion

| Field | Order |
|---|---|
| Goal | Add playable Floor 3-5 `floorRunPaths` using existing stableIds. |
| Inputs | `floor_3_5_completion_spec_v0.1.md`, external pack 25 stableIds, current room asset. |
| Work | Resolve `ENC_COMBAT_GATE_02` Floor 2 override conflict; attach Floor 3-5 paths; keep Floor 2 demo route intact. |
| Acceptance | Route can progress Floor 1 -> 2 -> 3 -> 4 -> 5 -> final boss without route dead ends. |
| Tests | EditMode route order; PlayMode smoke for through-route with deterministic choices. |
| Risk | Floor 4 has no dedicated combat stableId in current pack. |

### P1-2 Final Boss Data Completion

| Field | Order |
|---|---|
| Goal | Bind `BOSS_APEX_02` to final combat and balance it for completion. |
| Inputs | Existing `BOSS_APEX_02`, `ENC_COMBAT_GATE_03`, item/ability effects, Recall Anchor. |
| Work | Override/bind final combat enemy to `BOSS_APEX_02`, verify HP/ATK/reward and defeat/recall/clear outcomes. |
| Acceptance | Final boss can be won in a reasonable build window and lost without Recall Anchor; victory opens ending choice. |
| Tests | EditMode balance tests for no-item, bandage, scout, Recall Anchor, and combined builds. |
| Forbidden | Do not hard-code balance values outside SO/data-driven path. |

### P1-3 Full Content Mapping Lock

| Field | Order |
|---|---|
| Goal | Decide which items/abilities/enemies are public completion pool. |
| Inputs | `completion_gap_audit_v0.1.md`, current item/ability/enemy assets. |
| Work | Mark placeholder-only assets, select 18 target items, decide 7-enemy target set, and map acquisition sources. |
| Acceptance | Catalog refs are all resolvable; no purchase grants unknown refs; no placeholder ability unlocks Skill by accident. |
| Tests | Catalog validation and shop/reward acquisition tests. |

## P2 Orders

### P2-1 UI Polish and Recording States

| Field | Order |
|---|---|
| Goal | Make completion route recordable without debug artifacts. |
| Inputs | Prior W2 presentation docs plus new ending/Android QA plan. |
| Work | Add screenshot QA for Floor 3 memory, Floor 4 shop, Floor 5 boss, ending choice, ending.rest, ending.continue. |
| Acceptance | Public recording mode hides raw keys/debug labels and shows distinct states. |

### P2-2 Asset Replacement Pass

| Field | Order |
|---|---|
| Goal | Replace repeated placeholder art/sound in completion route. |
| Inputs | Existing asset replacement priority docs and Floor 3-5 route spec. |
| Work | Prioritize final boss, ending choice, S4/S5 memory, and late shop/combat backgrounds. |
| Acceptance | Floor 3-5 and ending screens do not look like recycled Floor 1-2 screens. |

### P2-3 Model Acceptance and Recording Lock

| Field | Order |
|---|---|
| Goal | Decide when real model can replace deterministic fallback for public capture/build. |
| Inputs | `ai_fallback_completion_matrix_v0.1.md`, model eval reports, Android QA report. |
| Work | Run latency/quality report; compare fallback vs model outputs; writer reviews forbidden content risks. |
| Acceptance | Real model path passes or deterministic fallback remains the public route. |

## Suggested Implementation Order

1. Finish ending state machine with placeholder keys.
2. Verify deterministic fallback matrix for new run/ending states.
3. Bind final boss and Floor 5 victory to ending choice.
4. Attach Floor 3-5 route paths and resolve `ENC_COMBAT_GATE_02` conflict.
5. Run EditMode/PlayMode route tests.
6. Run Android build/device smoke.
7. Perform recording/polish screenshot QA.

## Definition of Done for Completion Push

| ID | Criteria |
|---|---|
| DONE-01 | A deterministic route can reach `ending.rest`. |
| DONE-02 | A deterministic route can reach `ending.continue`. |
| DONE-03 | Defeat can still reach `run.failed`. |
| DONE-04 | Recall Anchor can revive once and then allow later failure if lethal again. |
| DONE-05 | Restart/regression preserves memory/reflection policy. |
| DONE-06 | Android APK smoke report exists. |
| DONE-07 | AI fallback covers all listed completion states. |
| DONE-08 | No final NPC, memory, or ending prose ships without writer approval. |
