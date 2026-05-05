# Memory Fragment Demo Spec v0.1

## Scope
- Memory stableId: `MEM_FRAGMENT_01`
- Source encounter: `ENC_MEMORY_FRAGMENT_01`
- Unlock choice: `CHOICE_MEMORY_01_UNLOCK`
- Current rule: OQ-006 memory fragment final text is writer-owned and open.
- Text policy: placeholder keys only; no final memory prose.

## MEM_FRAGMENT_01
| field | value | alignment status | notes |
|---|---|---|---|
| stableId | MEM_FRAGMENT_01 | aligned | Appears in v0.3 pack and baked MemoryFragment SO. |
| referenced by encounter/choice | ENC_MEMORY_FRAGMENT_01 / CHOICE_MEMORY_01_UNLOCK | aligned | Unlock effect and requirement use this stable id. |
| suggested Unity SO type | MemoryFragmentData | aligned | Existing asset: `SO_MemoryFragment_MEM_FRAGMENT_01`. |
| stage | S1_AWARENESS | aligned | Matches pack fragment stage and baked MemoryFragment SO stage. |
| encounter body key | PLACEHOLDER_ENC_MEMORY_FRAGMENT_01_BODY | aligned | Encounter-level placeholder only. |
| choice unlock key | PLACEHOLDER_CHOICE_MEMORY_01_UNLOCK | aligned | Choice label placeholder only. |
| choice withdraw key | PLACEHOLDER_CHOICE_MEMORY_01_WITHDRAW | aligned | Choice label placeholder only. |
| pack unlock fragment text key | PLACEHOLDER_MEM_FRAGMENT_01 | needs alignment | Pack effect carries a single fragment text key. |
| baked title key | PLACEHOLDER_MEM_FRAGMENT_01_TITLE | aligned | MemoryFragment SO title key. |
| baked body key | PLACEHOLDER_MEM_FRAGMENT_01_BODY | aligned | MemoryFragment SO body key. |
| unlock reaction key | NPC_REACT_CHOICE_MEMORY_01_UNLOCK | aligned; text key needs alignment | Pack text key: PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_UNLOCK. |
| withdraw reaction key | NPC_REACT_CHOICE_MEMORY_01_WITHDRAW | aligned; text key needs alignment | Pack text key: PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_WITHDRAW. |

## Intent Contract
| item | intent label | implementation note |
|---|---|---|
| unlock intent | recognition uncertainty | NPC reflection should receive `MEM_FRAGMENT_01`, `S1_AWARENESS`, glitch +6, affinity +2. |
| withdraw intent | containment relief | NPC reflection should receive no unlocked memory id, mental +1, glitch -1. |
| UI display intent | fragment acquired state | Display stable id and placeholder title/body keys until writer approval. |
| system risk intent | instability tradeoff | Use numeric deltas, not prose, for QA review. |

## Writer Approval Required
| approval item | current placeholder key | blocker if unresolved |
|---|---|---|
| memory title direction | PLACEHOLDER_MEM_FRAGMENT_01_TITLE | Demo may show placeholder title key. |
| memory body direction | PLACEHOLDER_MEM_FRAGMENT_01_BODY | Demo may show placeholder body key. |
| pack single text key policy | PLACEHOLDER_MEM_FRAGMENT_01 | Decide whether this remains legacy pack key or maps to body key. |
| unlock NPC reaction | PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_UNLOCK | Reflection output must remain placeholder or intent-only. |
| withdraw NPC reaction | PLACEHOLDER_NPC_REACT_CHOICE_MEMORY_01_WITHDRAW | Reflection output must remain placeholder or intent-only. |

## QA Assertions
- `MEM_FRAGMENT_01` appears after `CHOICE_MEMORY_01_UNLOCK`.
- `PLACEHOLDER_MEM_FRAGMENT_01_TITLE` and `PLACEHOLDER_MEM_FRAGMENT_01_BODY` are available from baked MemoryFragment SO.
- `PLACEHOLDER_MEM_FRAGMENT_01` remains documented as pack effect text key until key alignment is decided.
- No final memory title or body text is introduced by this spec.
