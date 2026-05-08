# Completion Gap Audit v0.1

## Scope

- Baseline: `origin/Proto` `d94697f Fix post bossgate gameplay blockers`.
- Method: Unityless document/code/data review only.
- Runtime code changes: none in this outsource pass.
- SO asset changes: none in this outsource pass.
- Final NPC, memory, ending prose: not written here.

## Pillar Fit

| Pillar | Fit check |
|---|---|
| P1 | Completion plan keeps memory/endings as loss-oriented systems, not power fantasy resolution. |
| P2 | AI fallback is treated as deterministic runtime coverage until the model is accepted. |
| P3 | Floor 3-5 proposals preserve a short mobile run cadence with route steps, not a sprawling map. |
| P4 | Route, fallback, Android, and ending acceptance criteria require deterministic state ids and replayable QA. |
| P5 | Ending choice is specified as immediate state transition, not delayed moral flavor. |

## Full Scope vs Current Implementation

| Scope item | Target | Status | Evidence at `d94697f` | Next dev action | Owner |
|---|---:|---|---|---|---|
| 5 floors | Floors 1-5 playable | Partial | GDD D-009 locks `floor 5`; `SO_Room_Prototype.asset` has `floorRunPaths` only for floor 1 and floor 2. External pack contains floor 1-5 encounter coverage. | Add `floorRunPaths` for floors 3-5 using existing encounter SO/stableIds; keep Floor 2 BossGate clear path intact. | dev/design |
| 25 encounters | 25 encounter pack usable | Partial | `pack_FULL_25_V003.json` has `encounterCount=25`; `Assets/_Project/Data/Encounters` has baked pack assets plus prototype-specific SOs. Current route uses only the recording/Floor 1-2 subset. | Promote the 25-pack from baked inventory to playable route coverage; validate all referenced item/ability/enemy/memory ids through catalog. | dev |
| 12 abilities | 12 playable active abilities | Partial | Formal ability data has `ABILITY_SCOUT` and `ABILITY_RECALL_ANCHOR`; `Assets/_Project/Data/Prototype/Abilities` has 12 placeholder assets with placeholder ids. | Decide which 12 ability stableIds are final enough to expose; replace placeholder ids or map them to approved stableIds without final prose. | design/dev |
| 18 items | 18 item/relic pool | Partial | `Assets/_Project/Data/Items` has 21 assets: `ITEM_01`-`ITEM_12`, named demo items, and relics. Many use placeholder display/description keys but numeric effects exist. | Select the 18-item target subset, mark extras as dev-only or relic variants, and map acquisition sources. | design/dev |
| 7 enemies | 7 non-boss enemies | Partial | Enemy assets include `ENEMY_WALKER_01`, `ENEMY_CRAWLER_02`, `ENEMY_SHADE_03`, `ENEMY_WRAITH_04`, `ENEMY_HERALD_05`, `ENEMY_FRACTURE_HOUND`, `ENEMY_EMPTY_ARMOR`, `ENEMY_COLLAPSE_ECHO`; count exceeds the order's 7-enemy audit target. | Decide whether `ENEMY_EMPTY_ARMOR` or `ENEMY_COLLAPSE_ECHO` is demo-only/supporting, then bind each combat gate to an approved enemy progression. | design/dev |
| 2 bosses | mini boss + final boss | Partial | `BOSS_GATE_01` is integrated into Floor 2 BossGate and covered by tests. `BOSS_APEX_02` exists at HP 80 / ATK 14 / Gold 60 but is not wired to route/endings. | Bind `BOSS_APEX_02` to the Floor 5 final combat gate and connect victory to ending choice. | dev/design |
| memory 5 | 5 unlockable memory fragments | Partial | `MEM_FRAGMENT_01`-`MEM_FRAGMENT_05` assets exist with placeholder title/body keys; memory unlock route currently proven for early demo and restart retention was improved upstream. | Route fragments across floors 1-5; preserve unlocks across restart/regression; keep body prose writer-owned. | dev/writer |
| endings 2 | Rest / Continue endings | Missing | GDD D-013 locks final boss -> rest item -> Rest/Continue choice. No runtime `ending.rest` or `ending.continue` state was found. | Add ending choice state machine and QA gates; use placeholder keys only until writer approval. | dev/writer |
| Android | APK candidate | Blocked | `BuildScript.BuildAndroid` exists; docs and model spec still note Android Build Support missing. ProjectSettings has Android fields, but no Unity run was performed here. | Install Android Build Support, produce APK, run smoke checklist on portrait device. | dev/build |
| AI fallback/model | deterministic fallback plus real model path | Partial | `LLMProviderFactory` uses `DeterministicFakeLLMProvider` fallback/cache; model manifests and config folders exist; eval reports show pending model/device validation. | Lock fallback reaction matrix for all completion states, then accept model artifact only after Android latency/quality reports pass. | AI/dev/writer |

## Completion Readiness Calls

| Area | Ready call | Reason |
|---|---|---|
| Current Floor 1-2 demo loop | ready for code-based QA, Unity not rerun here | `d94697f` adds blocker cleanup and tests around Floor 2 shop refs, Recall Anchor, restart retention, run clear/failure. |
| Full game route | partial | Encounter inventory exists, route only covers Floor 1-2 in current room data. |
| Full content text | blocked by writer ownership | All final NPC, memory, ending prose remains placeholder/writer-owned. |
| Completion build | blocked | Android Build Support and model runtime artifact/eval are not accepted. |
| Ending choice | missing | Design locked by D-013, runtime state not implemented. |

## Evidence Index

| Evidence | Path |
|---|---|
| GDD full scope, D-009, D-013, D-018 | `Docs/Project/hwiglija-tower-gdd.md` |
| Current prototype floor path | `Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset` |
| Full 25 encounter source pack | `Docs/ExternalSpecs/EncounterPipeline/v0.3/pack_FULL_25_V003.json` |
| Boss assets | `Assets/_Project/Data/Enemies/SO_Enemy_BOSS_GATE_01.asset`, `Assets/_Project/Data/Enemies/SO_Enemy_BOSS_APEX_02.asset` |
| Android build harness | `Assets/_Project/Editor/BuildScript.cs` |
| Fallback/cache implementation | `Assets/_Project/Scripts/LLM/*`, `Assets/_Project/Scripts/NPC/*` |
