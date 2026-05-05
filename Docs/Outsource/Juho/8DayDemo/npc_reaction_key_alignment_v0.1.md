# NPC Reaction Key Alignment v0.1

## Scope
- Source pack: `Docs/ExternalSpecs/EncounterPipeline/v0.3/pack_FULL_25_V003.json`
- Baked data source: `Assets/_Project/Data/Encounters/SO_Encounter_*.asset`
- Scope encounters: `ENC_SHOP_01`, `ENC_MORAL_CHOICE_01`, `ENC_MEMORY_FRAGMENT_01`, `ENC_COMBAT_GATE_01`
- Text policy: placeholder keys only; no final NPC dialogue.

## Alignment Table
| flow step | source encounter | source choice or handoff | baked reaction key | pack reaction text key | alignment status | notes |
|---|---|---|---|---|---|---|
| Shop | ENC_SHOP_01 | CHOICE_SHOP_01_BUY_ITEM | NPC_REACT_CHOICE_SHOP_01_BUY_ITEM | PLACEHOLDER_NPC_REACT_CHOICE_SHOP_01_BUY_ITEM | baked reaction key matches pack; text key needs alignment | SO stores `npcReactionKey`; pack also carries placeholder text key. |
| Shop | ENC_SHOP_01 | CHOICE_SHOP_01_BUY_ABILITY | NPC_REACT_CHOICE_SHOP_01_BUY_ABILITY | PLACEHOLDER_NPC_REACT_CHOICE_SHOP_01_BUY_ABILITY | baked reaction key matches pack; text key needs alignment | Confirm whether NPC reflection demo consumes reaction key, text key, or both. |
| Shop | ENC_SHOP_01 | CHOICE_SHOP_01_LEAVE | NPC_REACT_CHOICE_SHOP_01_LEAVE | PLACEHOLDER_NPC_REACT_CHOICE_SHOP_01_LEAVE | baked reaction key matches pack; text key needs alignment | Reaction-only choice; no effect payload. |
| MoralChoice | ENC_MORAL_CHOICE_01 | CHOICE_MORAL_01_AID | NPC_REACT_CHOICE_MORAL_01_AID | PLACEHOLDER_NPC_REACT_CHOICE_MORAL_01_AID | baked reaction key matches pack; text key needs alignment | Writer-owned tone, intent only. |
| MoralChoice | ENC_MORAL_CHOICE_01 | CHOICE_MORAL_01_REFUSE | NPC_REACT_CHOICE_MORAL_01_REFUSE | PLACEHOLDER_NPC_REACT_CHOICE_MORAL_01_REFUSE | baked reaction key matches pack; text key needs alignment | Demo smoke path uses this choice. |
| MoralChoice | ENC_MORAL_CHOICE_01 | CHOICE_MORAL_01_TRADE | NPC_REACT_CHOICE_MORAL_01_TRADE | PLACEHOLDER_NPC_REACT_CHOICE_MORAL_01_TRADE | baked reaction key matches pack; text key needs alignment | Check item context handoff for `ITEM_TORN_CHARM`. |
| MemoryFragment | ENC_MEMORY_FRAGMENT_01 | CHOICE_MEMORY_01_UNLOCK | NPC_REACT_CHOICE_MEMORY_01_UNLOCK | PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_UNLOCK | baked reaction key matches pack; text key needs alignment | Demo smoke path uses this choice. |
| MemoryFragment | ENC_MEMORY_FRAGMENT_01 | CHOICE_MEMORY_01_WITHDRAW | NPC_REACT_CHOICE_MEMORY_01_WITHDRAW | PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_WITHDRAW | baked reaction key matches pack; text key needs alignment | No memory unlock payload. |
| CombatGate | ENC_COMBAT_GATE_01 | CHOICE_COMBAT_01_ENGAGE | NPC_REACT_CHOICE_COMBAT_01_ENGAGE | PLACEHOLDER_NPC_REACT_CHOICE_COMBAT_01_ENGAGE | baked reaction key matches pack; text key needs alignment | Choice-level reaction before combat handoff. |
| CombatGate | ENC_COMBAT_GATE_01 | COMBAT_GATE_01 start | NPC_REACT_COMBAT_GATE_01_START | PLACEHOLDER_NPC_REACT_COMBAT_GATE_01_START | baked handoff reaction key matches pack; text key needs alignment | Handoff-level NPC start reaction. |
| CombatGate | ENC_COMBAT_GATE_01 | CHOICE_COMBAT_01_PREPARE | NPC_REACT_CHOICE_COMBAT_01_PREPARE | PLACEHOLDER_NPC_REACT_CHOICE_COMBAT_01_PREPARE | baked reaction key matches pack; text key needs alignment | Prep choice does not start combat. |

## Adjacent Placeholder Keys For QA
| encounter | body key | choice keys | result or memory keys | status |
|---|---|---|---|---|
| ENC_SHOP_01 | PLACEHOLDER_ENC_SHOP_01_BODY | PLACEHOLDER_CHOICE_SHOP_01_BUY_ITEM; PLACEHOLDER_CHOICE_SHOP_01_BUY_ABILITY; PLACEHOLDER_CHOICE_SHOP_01_LEAVE | PLACEHOLDER_REASON_NOT_ENOUGH_GOLD | available in baked data; reaction text key binding needs alignment |
| ENC_MORAL_CHOICE_01 | PLACEHOLDER_ENC_MORAL_CHOICE_01_BODY | PLACEHOLDER_CHOICE_MORAL_01_AID; PLACEHOLDER_CHOICE_MORAL_01_REFUSE; PLACEHOLDER_CHOICE_MORAL_01_TRADE | FLAG_MORAL_01_AID; FLAG_MORAL_01_REFUSE | available in baked data; reaction text key binding needs alignment |
| ENC_MEMORY_FRAGMENT_01 | PLACEHOLDER_ENC_MEMORY_FRAGMENT_01_BODY | PLACEHOLDER_CHOICE_MEMORY_01_UNLOCK; PLACEHOLDER_CHOICE_MEMORY_01_WITHDRAW | MEM_FRAGMENT_01; PLACEHOLDER_MEM_FRAGMENT_01; PLACEHOLDER_MEM_FRAGMENT_01_TITLE; PLACEHOLDER_MEM_FRAGMENT_01_BODY | pack effect text key and MemoryFragment SO title/body keys need alignment |
| ENC_COMBAT_GATE_01 | PLACEHOLDER_ENC_COMBAT_GATE_01_BODY | PLACEHOLDER_CHOICE_COMBAT_01_ENGAGE; PLACEHOLDER_CHOICE_COMBAT_01_PREPARE | PLACEHOLDER_POST_COMBAT_GATE_01_VICTORY; PLACEHOLDER_POST_COMBAT_GATE_01_DEFEAT; PLACEHOLDER_POST_COMBAT_GATE_01_DEATH | post-combat text keys are present; final copy remains writer-owned |
