# Proto Merge Notes v0.1

## Commits to Review for Proto

| commit | message | merge note |
|---|---|---|
| `a18277e` | Draft 8-day demo outsource package | Docs-only package from the previous outsource pass. Safe to review separately. |
| this pass | Polish demo UI flow and QA notes | UI polish plus QA/docs from this pass. Review after Unity PlayMode confirms layout. |

## Files with Higher Conflict Risk

| file | reason |
|---|---|
| `Assets/_Project/Scripts/UI/PrototypeHud.cs` | Main portrait HUD layout and display logic changed. |
| `Assets/_Project/Scripts/Run/PrototypeRunState.cs` | Adds read-only display state for memory/combat. |
| `Assets/_Project/Scripts/Run/PrototypeEncounterRuntimeResolver.cs` | Result summary format changed. |
| `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs` | Smoke test expectations expanded. |

## Tests to Run Before Proto Merge

- [ ] Fresh EditMode test run.
- [ ] Fresh PlayMode test run.
- [ ] Manual `PrototypeRoom` vertical layout check.
- [ ] One-run route check: Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete.
- [ ] Android portrait build candidate check if Android support is installed.

## Changes Not to Merge Blindly

- Any experimental scene edits outside the committed file list.
- Any generated or stale test XML.
- Any unrelated untracked files.
- Any prototype data deletion or bulk regeneration.
- Any writer-facing final prose not approved by the writer.

## Merge Guidance

- Safe to merge after Unity tests pass and portrait layout is visually checked.
- If Proto has concurrent HUD work, manually compare `PrototypeHud.cs` first.
- If Proto has combat result changes, manually compare `PrototypeEncounterRuntimeResolver.cs`.
- Keep schema, baker, and catalog stableId structure out of this merge unless separately reviewed.
