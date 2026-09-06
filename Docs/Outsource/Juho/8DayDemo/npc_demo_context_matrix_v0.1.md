# NPC Demo Context Matrix v0.1

## Scope
- Source pack: `pack_FULL_25_V003.json`
- Demo focus: `Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete`
- Text policy: intent labels only; no final NPC dialogue, memory prose, or moral prose.

## Context Matrix
| flow step | encounter stableId | choice stableId | changed stats and effects | unlocked memory id | combat result context | NPC context payload | expected emotional intent |
|---|---|---|---|---|---|---|---|
| Shop | ENC_SHOP_01 | CHOICE_SHOP_01_BUY_ITEM | gold -5; add ITEM_FIELD_BANDAGE x1; affinity +1 | none | none | choice id; item id; gold cost; affinity delta; reaction key NPC_REACT_CHOICE_SHOP_01_BUY_ITEM | practical trust |
| Shop | ENC_SHOP_01 | CHOICE_SHOP_01_BUY_ABILITY | gold -12; add ABILITY_SCOUT; glitch -1 | none | none | choice id; ability id; gold cost; glitch delta; reaction key NPC_REACT_CHOICE_SHOP_01_BUY_ABILITY | cautious trust |
| Shop | ENC_SHOP_01 | CHOICE_SHOP_01_LEAVE | no immediate stat effect | none | none | choice id; no-effect marker; reaction key NPC_REACT_CHOICE_SHOP_01_LEAVE | neutral observation |
| MoralChoice | ENC_MORAL_CHOICE_01 | CHOICE_MORAL_01_AID | hp -3; affinity +6; glitch -3; set FLAG_MORAL_01_AID | none | none | choice id; hp cost; affinity delta; glitch delta; flag id; reaction key NPC_REACT_CHOICE_MORAL_01_AID | restrained approval |
| MoralChoice | ENC_MORAL_CHOICE_01 | CHOICE_MORAL_01_REFUSE | mental -2; affinity -5; glitch +4; set FLAG_MORAL_01_REFUSE | none | none | choice id; mental delta; affinity delta; glitch delta; flag id; reaction key NPC_REACT_CHOICE_MORAL_01_REFUSE | fracture pressure |
| MoralChoice | ENC_MORAL_CHOICE_01 | CHOICE_MORAL_01_TRADE | gold -2; affinity +1; glitch +1; add ITEM_TORN_CHARM x1 | none | none | choice id; item id; gold cost; affinity delta; glitch delta; reaction key NPC_REACT_CHOICE_MORAL_01_TRADE | unresolved compromise |
| MemoryFragment | ENC_MEMORY_FRAGMENT_01 | CHOICE_MEMORY_01_UNLOCK | glitch +6; affinity +2; set FLAG_MEM_FRAGMENT_01_UNLOCKED | MEM_FRAGMENT_01 | none | choice id; memory id; stage S1_AWARENESS; title/body placeholder keys; reaction key NPC_REACT_CHOICE_MEMORY_01_UNLOCK | recognition uncertainty |
| MemoryFragment | ENC_MEMORY_FRAGMENT_01 | CHOICE_MEMORY_01_WITHDRAW | mental +1; glitch -1 | none | none | choice id; avoided memory marker; mental delta; glitch delta; reaction key NPC_REACT_CHOICE_MEMORY_01_WITHDRAW | containment relief |
| CombatGate | ENC_COMBAT_GATE_01 | CHOICE_COMBAT_01_ENGAGE | start COMBAT_GATE_01 | none | enemy ENEMY_FRACTURE_HOUND; victory: gold +7, glitch -2, affinity +2, set FLAG_COMBAT_GATE_01_VICTORY; defeat: hp -5, glitch +5, affinity -2 | source encounter id; source choice id; combat id; enemy id; handoff reaction key NPC_REACT_COMBAT_GATE_01_START; choice reaction key NPC_REACT_CHOICE_COMBAT_01_ENGAGE | warning tension |
| CombatGate | ENC_COMBAT_GATE_01 | CHOICE_COMBAT_01_PREPARE | mental -1; affinity +1; set FLAG_COMBAT_01_PREPARED | none | none | choice id; prep flag; mental delta; affinity delta; reaction key NPC_REACT_CHOICE_COMBAT_01_PREPARE | steadier preparation |

## Demo Smoke Path Context
| order | selected id | expected state marker | NPC key to watch | QA note |
|---|---|---|---|---|
| 1 | CHOICE_SHOP_01_BUY_ITEM | ITEM_FIELD_BANDAGE x1; gold -5 | NPC_REACT_CHOICE_SHOP_01_BUY_ITEM | Requires gold at least 5. |
| 2 | CHOICE_MORAL_01_REFUSE | affinity -5; glitch +4; FLAG_MORAL_01_REFUSE | NPC_REACT_CHOICE_MORAL_01_REFUSE | Intent should remain non-judgmental. |
| 3 | CHOICE_MEMORY_01_UNLOCK | MEM_FRAGMENT_01; glitch +6; affinity +2 | NPC_REACT_CHOICE_MEMORY_01_UNLOCK | Memory body remains writer-owned. |
| 4 | CHOICE_COMBAT_01_ENGAGE | COMBAT_GATE_01; ENEMY_FRACTURE_HOUND; demo.complete after result | NPC_REACT_CHOICE_COMBAT_01_ENGAGE; NPC_REACT_COMBAT_GATE_01_START | Handoff result may be victory or defeat based on deterministic combat resolution. |

## Context Fields To Preserve For NPC Reflection Demo
| field | source | purpose |
|---|---|---|
| `choiceStableId` | current choice | stable event identity for reflection cache. |
| `sourceEncounterId` | current encounter | lets NPC distinguish Shop, MoralChoice, MemoryFragment, CombatGate. |
| `npcStage` | encounter or handoff | constrains reaction tone. |
| `glitchDelta` | choice effects or combat result | connects instability to player action. |
| `affinityDelta` | choice effects or combat result | connects relationship movement to player action. |
| `memoryFragmentId` | UnlockMemoryFragment effect | lets reflection reference unlocked memory only by stable id until copy is approved. |
| `combatStableId` | StartCombat handoff | anchors post-combat reflection. |
| `enemyRefs` | StartCombat handoff | anchors combat context without final monster text. |
| `resultId` | runtime result | lets demo panel and NPC reflection agree on victory or defeat branch. |
