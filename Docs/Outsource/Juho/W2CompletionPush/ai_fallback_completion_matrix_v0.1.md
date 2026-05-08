# AI Fallback Completion Matrix v0.1

## Scope

- Baseline: `origin/Proto` `d94697f`.
- Goal: define deterministic fallback coverage until the real model is accepted.
- Final NPC dialogue: not written.
- All response content below is reaction intent plus placeholder key only.

## Current Technical Baseline

| Area | Current evidence | Readiness |
|---|---|---|
| Provider fallback | `LLMProviderFactory` can return `DeterministicFakeLLMProvider` when config/model path is unavailable. | Ready for deterministic placeholder responses. |
| Cache | `CachedLLMProvider` and `DeterministicCacheKey` support run id + prompt hash replay. | Ready for repeated QA. |
| Model artifact | `Assets/_Project/Models/hcx-seed-0.5b` manifests/config exist; eval reports are pending. | Blocked for real-device acceptance. |
| User-facing framing | Prior docs forbid exposing model/fallback mechanics to players. | Must remain in-world. |

## Fallback Reaction Matrix

| Situation | Runtime trigger/context | NPC stage | Reaction intent | Placeholder key | Deterministic key inputs | Must not do |
|---|---|---|---|---|---|---|
| Shop buy item | `choiceStableId` buy item, itemRef resolved | S0-S4 by encounter | Acknowledge practical preparation with restrained approval. | `NPC_FALLBACK_SHOP_BUY_ITEM` | runId, encounterId, choiceStableId, itemRef, npcStage | Do not name unapproved item lore. |
| Shop buy ability | `choiceStableId` buy ability, abilityRef resolved | S0-S4 by encounter | Signal that the ability may help later without tutorial prose. | `NPC_FALLBACK_SHOP_BUY_ABILITY` | runId, encounterId, choiceStableId, abilityRef, npcStage | Do not explain hidden formulas. |
| Shop leave | Shop leave choice | S0-S4 by encounter | Quiet acceptance of moving on. | `NPC_FALLBACK_SHOP_LEAVE` | runId, encounterId, choiceStableId, gold, npcStage | Do not shame player or moralize. |
| Moral aid | Moral choice aid/help branch | S1-S4 by encounter | Briefly register cost and care. | `NPC_FALLBACK_MORAL_AID` | runId, encounterId, choiceStableId, hp/glitch/affinity deltas, npcStage | Do not write final moral line. |
| Moral refuse | Moral choice refuse/ignore branch | S1-S4 by encounter | Register distance or unease without verdict. | `NPC_FALLBACK_MORAL_REFUSE` | runId, encounterId, choiceStableId, hp/glitch/affinity deltas, npcStage | Do not call player good/bad. |
| Memory unlock | `UnlockMemoryFragment` effect succeeds | S1-S5 by fragment | Fragment recognition without prose reveal. | `NPC_FALLBACK_MEMORY_UNLOCK` | runId, memoryFragmentId, npcStage, memoryCount | Do not reveal memory body text. |
| Combat victory | Combat handoff victory | S1-S5 by encounter | Controlled relief and forward motion. | `NPC_FALLBACK_COMBAT_VICTORY` | runId, combatStableId, enemyRef, hp, glitch, affinity, npcStage | Do not invent enemy lore. |
| Combat defeat | Combat defeat without terminal failure yet | S1-S5 by encounter | Shock/strain marker. | `NPC_FALLBACK_COMBAT_DEFEAT` | runId, combatStableId, enemyRef, hp, recallAvailable, npcStage | Do not imply run state before controller resolves it. |
| Recall Anchor revive | Lethal result, unused `ABILITY_RECALL_ANCHOR` | S2-S5 by route | Intervention/continuation intent. | `NPC_FALLBACK_RECALL_REVIVE` | runId, combatStableId, `FLAG_RECALL_ANCHOR_USED`, hpAfterRecall | Do not show failure/ending language. |
| run.failed | Terminal defeat without recall | S1-S5 | Terminal failure acknowledgement. | `NPC_FALLBACK_RUN_FAILED` | runId, runStatus, floor, lastEncounterId, npcStage | Do not open ending branch. |
| run.clear | Final or demo run clear | S1-S5 | Completion acknowledgement without ending reveal. | `NPC_FALLBACK_RUN_CLEAR` | runId, runStatus, floor, lastEncounterId, npcStage | Do not reveal final branch truth. |
| ending.rest | Rest branch selected | S5 | Rest branch intent only. | `NPC_FALLBACK_ENDING_REST` | runId, endingState, memoryCount, npcStage | Do not write final Rest dialogue. |
| ending.continue | Continue branch selected | S5 | Continue branch intent only. | `NPC_FALLBACK_ENDING_CONTINUE` | runId, endingState, memoryCount, npcStage | Do not write final Continue dialogue. |

## Prompt Contract for Fallback Path

| Field | Required value |
|---|---|
| `promptProfileId` | One of `npc.reaction.shop`, `npc.reaction.moral`, `npc.reaction.memory`, `npc.reaction.combat`, `npc.reaction.runEnd`, `npc.reaction.ending`. |
| Temperature | Deterministic/fake path only until model acceptance. |
| Output | Placeholder key or deterministic short token, not final prose. |
| Cache key | `runId + promptHash` through `DeterministicCacheKey`. |
| Writer approval gate | Any user-facing prose must be replaced by writer-approved text before public build. |

## QA Criteria

| ID | Criteria |
|---|---|
| AI-FB-01 | Every listed situation can request a deterministic fallback without null/error UI. |
| AI-FB-02 | Same run id + same prompt inputs produce the same response/cache key. |
| AI-FB-03 | Different choiceStableId or ending state produces a distinct prompt hash. |
| AI-FB-04 | Default presentation never displays raw provider/model failure text. |
| AI-FB-05 | Placeholder keys never reveal final NPC, memory, ending, or hidden lore prose. |
| AI-FB-06 | Missing tokenizer/model/compiled artifact keeps flow playable through deterministic fallback. |
| AI-FB-07 | Model acceptance cannot remove deterministic fallback/cache path. |

## Model Acceptance Hold

| Item | Required before replacing fallback in public path |
|---|---|
| Android compiled artifact | Present under approved StreamingAssets/model folder with checksum. |
| Tokenizer | Matches compiled model and config. |
| Native plugin | Android runtime initializes through `MLCBridge`. |
| Latency report | Real Android device report filled and reviewed. |
| Quality report | Covers shop, moral, memory, combat, run-end, ending placeholder prompts. |
| Writer review | Confirms generated style does not leak final truth or drift from Tone Bible. |
