# UI Implementation Report v0.1

## Implemented Screens

| area | implemented behavior |
|---|---|
| Portrait HUD | Multi-line mobile HUD now separates run status, HP, ATK, Mental, Gold, Glitch, Affinity, ability count, and demo step count. |
| Choice panel | Existing 2-3 button flow is preserved; hidden choices remain hidden and disabled choices keep their reason key. |
| Result panel | Choice results now include readable state deltas and refs such as Gold, Affinity, Glitch, item refs, ability refs, memory refs, and combat refs. |
| Route indicator | HUD builds a route list from the configured demo run path and marks current, complete, and locked steps. |
| Memory fragment | HUD shows memory unlock count, last memory stableId, and title/body key placeholders when available from the catalog. |
| Combat gate | Combat handoff result now surfaces combat stableId, enemy stableId, result id, and post-combat state deltas. |
| Demo complete | Demo completion is shown as a non-ending completion state and does not restart the route. |

## Modified Files

| file | summary |
|---|---|
| `Assets/_Project/Scripts/UI/PrototypeHud.cs` | Added portrait layout normalization, route indicator, memory/combat panel, demo complete panel, and safer placeholder/stableId display. |
| `Assets/_Project/Scripts/Run/PrototypeSceneRuntimeBuilder.cs` | Passes the configured demo run path into the HUD and enables wrapping/best-fit on runtime HUD text. |
| `Assets/_Project/Scripts/Run/PrototypeRunSnapshot.cs` | Adds read-only snapshot fields for demo progress, memory display, and last combat display. |
| `Assets/_Project/Scripts/Run/PrototypeRunState.cs` | Tracks last unlocked memory key data and last combat handoff display data without changing progression rules. |
| `Assets/_Project/Scripts/Run/PrototypeEncounterRuntimeResolver.cs` | Adds result summaries for state deltas, item/ability/reward refs, memory unlocks, and combat handoff. |
| `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs` | Extends smoke coverage for route indicator, result deltas, memory display, and combat display. |

## Still Placeholder

| item | current state |
|---|---|
| Encounter copy | Kept as `PLACEHOLDER_*` keys or stableId-oriented debug labels. |
| NPC reaction text | Not finalized. Writer approval still required. |
| Memory body | Not written. Only stableId/titleKey/bodyKey are surfaced. |
| Demo complete copy | Uses demo completion placeholder-style status, not an ending line. |
| Art/SFX | Not changed in this implementation pass. |

## Writer Decisions Needed

- Approve or replace any human-readable temporary label before public demo.
- Confirm whether memory title/body keys should be shown to players or only debug QA.
- Confirm S0-S2 Mataios reaction tone before replacing placeholder keys.
- Confirm whether the public demo should avoid the hidden death/regression risk path.

## Merge Cautions

- This is a UI polish pass, not a schema or baker update.
- The route indicator reads from `PrototypeRoomDefinition.DemoRunPath`; keep that source aligned with demo progression.
- Memory title/body display depends on `EncounterRuntimeCatalogData.MemoryFragments`.
- The result panel intentionally shows stable IDs and placeholder-style refs until writer-approved text exists.
