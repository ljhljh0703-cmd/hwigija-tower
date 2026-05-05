# Demo Review Script v0.1

## Scope
- Demo path: `Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete`
- Source branch after sync: `Juho/Codex` with `origin/Proto` merged.
- Script policy: placeholder keys and stableIds only; no final NPC dialogue.

## Pre-Run Checks
- Confirm current branch is `Juho/Codex`.
- Confirm first available node resolves to Shop, not an automatic scene requirement.
- Confirm demo route indicator shows Shop before MoralChoice, MemoryFragment, CombatGate.
- Confirm test/demo setup gives at least 5 gold if the reviewed path uses `CHOICE_SHOP_01_BUY_ITEM`.
- If gold is below 5, `CHOICE_SHOP_01_BUY_ITEM` should be disabled with `PLACEHOLDER_REASON_NOT_ENOUGH_GOLD`; record as setup mismatch for this review script.

## 1-Minute Review Sequence
| time | demonstrator action | expected HUD change | expected result message markers | expected NPC reaction placeholder markers | failure check stableIds/keys |
|---|---|---|---|---|---|
| 0-10s | Start run; select the first available Shop node if interaction is required. | Route indicator marks Shop as current; HUD shows hp, mental, gold, glitch, affinity. | Run state visible; no completed marker yet. | none required before first choice. | `ENC_SHOP_01`; `PLACEHOLDER_ENC_SHOP_01_BODY`; route path binding. |
| 10-25s | Press `CHOICE_SHOP_01_BUY_ITEM`. | Gold decreases by 5; item count includes `ITEM_FIELD_BANDAGE`; route step can resolve. | `choice applied: CHOICE_SHOP_01_BUY_ITEM`; `Gold -5`; `item ITEM_FIELD_BANDAGE +1`. | `NPC_REACT_CHOICE_SHOP_01_BUY_ITEM`; `PLACEHOLDER_NPC_REACT_CHOICE_SHOP_01_BUY_ITEM`. | `PLACEHOLDER_CHOICE_SHOP_01_BUY_ITEM`; `PLACEHOLDER_REASON_NOT_ENOUGH_GOLD`; `ITEM_FIELD_BANDAGE`. |
| 25-40s | Move to MoralChoice and press `CHOICE_MORAL_01_REFUSE`. | Affinity decreases by 5; glitch increases by 4. | `choice applied: CHOICE_MORAL_01_REFUSE`; `Affinity -5`; `Glitch +4`. | `NPC_REACT_CHOICE_MORAL_01_REFUSE`; `PLACEHOLDER_NPC_REACT_CHOICE_MORAL_01_REFUSE`. | `ENC_MORAL_CHOICE_01`; `PLACEHOLDER_CHOICE_MORAL_01_REFUSE`; `FLAG_MORAL_01_REFUSE`. |
| 40-50s | Open MemoryFragment and press `CHOICE_MEMORY_01_UNLOCK`. | Memory panel shows `MEM_FRAGMENT_01`; title/body placeholder keys are visible or available. | `choice applied: CHOICE_MEMORY_01_UNLOCK`; `memory unlocked MEM_FRAGMENT_01`; `Glitch +6`; `Affinity +2`. | `NPC_REACT_CHOICE_MEMORY_01_UNLOCK`; `PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_UNLOCK`. | `ENC_MEMORY_FRAGMENT_01`; `MEM_FRAGMENT_01`; `PLACEHOLDER_MEM_FRAGMENT_01_TITLE`; `PLACEHOLDER_MEM_FRAGMENT_01_BODY`; `PLACEHOLDER_MEM_FRAGMENT_01`. |
| 50-60s | Open CombatGate and press `CHOICE_COMBAT_01_ENGAGE`. | Combat context shows `COMBAT_GATE_01`; enemy context shows `ENEMY_FRACTURE_HOUND`; demo complete appears after combat result. | `combat started COMBAT_GATE_01`; `enemy ENEMY_FRACTURE_HOUND`; `demo.complete`. | `NPC_REACT_CHOICE_COMBAT_01_ENGAGE`; `NPC_REACT_COMBAT_GATE_01_START`; `PLACEHOLDER_NPC_REACT_COMBAT_GATE_01_START`. | `ENC_COMBAT_GATE_01`; `CHOICE_COMBAT_01_ENGAGE`; `PLACEHOLDER_POST_COMBAT_GATE_01_VICTORY`; `PLACEHOLDER_POST_COMBAT_GATE_01_DEFEAT`; `PLACEHOLDER_POST_COMBAT_GATE_01_DEATH`. |

## Alternate Checks If Review Path Cannot Buy Item
| condition | allowed review action | expected marker | note |
|---|---|---|---|
| Gold below 5 | Do not press disabled purchase; verify disabled-visible state. | `PLACEHOLDER_REASON_NOT_ENOUGH_GOLD` | Then use `CHOICE_SHOP_01_LEAVE` only for route continuity review. |
| Shop already resolved | Continue to next demo route step. | `already resolved: CHOICE_SHOP_01_*` | Revisit policy should hide resolved choices. |
| Memory already unlocked | Verify no duplicate unlock choice appears. | existing `MEM_FRAGMENT_01` in memory panel | Revisit policy should prevent duplicate fragment grant. |

## StableId And Key Checklist
- Shop: `ENC_SHOP_01`, `CHOICE_SHOP_01_BUY_ITEM`, `ITEM_FIELD_BANDAGE`, `NPC_REACT_CHOICE_SHOP_01_BUY_ITEM`.
- MoralChoice: `ENC_MORAL_CHOICE_01`, `CHOICE_MORAL_01_REFUSE`, `FLAG_MORAL_01_REFUSE`, `NPC_REACT_CHOICE_MORAL_01_REFUSE`.
- MemoryFragment: `ENC_MEMORY_FRAGMENT_01`, `CHOICE_MEMORY_01_UNLOCK`, `MEM_FRAGMENT_01`, `NPC_REACT_CHOICE_MEMORY_01_UNLOCK`.
- CombatGate: `ENC_COMBAT_GATE_01`, `CHOICE_COMBAT_01_ENGAGE`, `COMBAT_GATE_01`, `ENEMY_FRACTURE_HOUND`, `NPC_REACT_COMBAT_GATE_01_START`.
- DemoComplete: `demo.complete`.

## Failure Report Template
| field | value |
|---|---|
| build/session |  |
| branch and commit |  |
| failed step |  |
| selected stableId |  |
| expected HUD marker |  |
| actual HUD marker |  |
| expected reaction key |  |
| actual reaction key |  |
| screenshot/video ref |  |
| notes |  |
