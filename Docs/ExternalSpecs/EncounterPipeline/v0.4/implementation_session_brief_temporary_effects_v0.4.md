# Implementation Session Brief — Temporary Event Effects v0.4

## Goal
Replace the temporary effect assignments generated from the Google Sheet with author-approved runtime DTOs when the sheet has final values and refs.

## Current State
- 33 `SO_Encounter_EVT_*` assets exist.
- 138 choices now have temporary runtime effects.
- No C# runtime code was changed for these temporary effects.
- the internal instability stat effect was intentionally not used because current choice hint code can expose that word.

## Do Not Treat As Final
- Any `TEMP_*` combat handoff stableId.
- Any generic item/relic/ability/reward mapping in `temporary_effect_assignment_report_v0.4.md`.
- The fatal `ModifyHp(-999)` handling for the balcony game-over choice.

## Replacement Tasks
1. Open `temporary_effect_homework.csv`.
2. For each row, replace temp effects with sheet-approved `effectDto`, `requirementDto`, and refs.
3. Replace generic refs like `RELIC_GENERIC_01`, `ABILITY_SWORD_02`, and `REWARD_CACHE_SMALL` with final design refs.
4. Replace `TEMP_COMBAT_*` handoffs with final combat handoff IDs and enemy refs.
5. Decide final handling for game-over style choices. Prefer explicit runtime support or approved fatal damage convention.
6. Re-run encounter validation and player-facing text scan.

## Validation Required
- duplicate stableId: none
- missing stableId: none
- all choices have stableId/textKey/effects
- all item/ability/enemy/reward/memory refs resolve
- no player-facing `Glitch`
- no raw event/choice stableId in player-facing text
- `git diff --check`
