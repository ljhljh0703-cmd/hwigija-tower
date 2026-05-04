# NPC Reaction Intent v0.1

## Scope

- Primary demo stages: S0_FIRST_MEETING, S1_AWARENESS, S2_COMPANION.
- S3_FRACTURE is optional and should only appear as a brief risk accent if used.
- Do not write final NPC dialogue from this document.

## Global Guardrails

- Do not make Mataios preach at the player.
- Do not tell the player which choice is correct.
- Do not reveal final truths about memory, regression, tower origin, or relationship history.
- Do not explain mechanics in NPC voice.
- Do not use wording that implies a removed flight/retreat system.
- Keep reactions short enough for mobile UI.

## ENC_SHOP_01

| item | intent |
|---|---|
| before choice | Mataios is cautious and observing. He is not fully attached yet. |
| after buy item | Small relief. The player is preparing to survive. |
| after buy ability | Trust nudges upward because the player plans ahead. |
| after leave | Neutral acceptance. He should not scold resource saving. |
| glitch/affinity link | Practical preparation may slightly increase Affinity. Ability purchase may support later low-risk route comprehension. |
| forbidden tone | No tutorial voice, no item recommendation, no "you should buy this" framing. |
| writer one-line intent | Mataios reads the Shop choice as a sign of how the player thinks about the next risk. |

## ENC_MORAL_CHOICE_01

| item | intent |
|---|---|
| before choice | S1_AWARENESS. Mataios starts comparing survival logic with empathy. |
| after aid | Trust rises, but the reaction should stay restrained. |
| after refuse | Fracture begins, but not accusation. |
| after trade | He sees compromise as ambiguous rather than clean or evil. |
| glitch/affinity link | Aid: Affinity up and Glitch down. Refuse: Affinity down and Glitch up. Trade: small mixed movement. |
| forbidden tone | No moral lecture, no final judgement, no absolute good/bad phrasing. |
| writer one-line intent | Mataios does not judge the player aloud, but the choice changes what he can bear to remember. |

## ENC_MEMORY_FRAGMENT_01

| item | intent |
|---|---|
| before choice | S1_AWARENESS. Recognition is possible but uncertain. |
| after unlock | Connection deepens while instability rises. |
| after withdraw | Temporary relief; the unresolved memory remains open. |
| glitch/affinity link | Unlock raises both Glitch and Affinity, showing that closeness is costly. Withdraw lowers immediate pressure. |
| forbidden tone | No confirmed memory truth, no backstory exposition, no final body text. |
| writer one-line intent | The fragment is not an answer; it is a pressure point that makes Mataios more connected and less stable. |

## ENC_COMBAT_GATE_01

| item | intent |
|---|---|
| before choice | S0/S1 edge. Mataios is tense but present. |
| after engage | He recognizes direct confrontation without glamorizing it. |
| after prepare | He stabilizes because the player pauses before danger. |
| after victory | Trust and relief, still brief. |
| after defeat | Fear and instability, but not blame. |
| glitch/affinity link | Victory lowers Glitch and raises Affinity. Defeat raises Glitch and lowers Affinity. |
| forbidden tone | No combat strategy explanation, no enemy lore reveal, no heroic praise. |
| writer one-line intent | The first combat proves whether the player can remain present with Mataios under pressure. |

## Optional S3_FRACTURE Use

| item | guidance |
|---|---|
| placement | Only after MemoryFragment unlock or after Defeat result. |
| duration | One short reaction or result-panel accent. |
| style | 80-90% readable sentence, 10-20% fractured rhythm or hesitation. |
| avoid | No heavy corruption wall, no final reveal, no loss of all readability. |
| writer one-line intent | S3 should feel like the relationship is overloading the context, not like a horror jump scare. |
