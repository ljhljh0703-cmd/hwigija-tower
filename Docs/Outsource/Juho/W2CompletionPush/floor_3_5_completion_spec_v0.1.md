# Floor 3-5 Completion Spec v0.1

## Scope

- Baseline: `origin/Proto` `d94697f`.
- Goal: propose a playable Floor 3-5 route using existing baked encounters, existing SOs, and existing stableIds only.
- Unity execution: none.
- Runtime/SO edits: none.
- Final text: not written. All copy remains placeholder key or writer-owned.

## Route Design Rules

| Rule | Requirement |
|---|---|
| StableId rule | Use only stableIds already present in `pack_FULL_25_V003.json` or existing SO assets. |
| Writer rule | Use `PLACEHOLDER_*` keys only; no final NPC, memory, or ending prose. |
| Route rule | Each floor has at least three steps from Shop/Rest, Moral/Memory, Combat, Boss/Gate, Story/Meta support. |
| Boss rule | `BOSS_GATE_01` stays Floor 2 run-ending demo boss. `BOSS_APEX_02` becomes Floor 5 completion boss. |
| Current blocker rule | `ENC_COMBAT_GATE_02` is currently overridden for Floor 2 BossGate. If it is reused for Floor 3 full-scope route, the override must become route-local or a separate existing combat stableId must be selected. |

## Existing StableId Inventory by Floor

| Floor | Existing encounter stableIds |
|---:|---|
| 1 | `ENC_REST_01`, `ENC_REST_02`, `ENC_MEMORY_FRAGMENT_01`, `ENC_COMBAT_GATE_01` |
| 2 | `ENC_REST_03`, `ENC_REST_04`, `ENC_MEMORY_FRAGMENT_02`, `ENC_SHOP_01`, `ENC_REMNANT_01`, `ENC_MORAL_CHOICE_01` |
| 3 | `ENC_REST_05`, `ENC_REST_06`, `ENC_MEMORY_FRAGMENT_03`, `ENC_COMBAT_GATE_02`, `ENC_STORY_01`, `ENC_MORAL_CHOICE_02` |
| 4 | `ENC_REST_07`, `ENC_REST_08`, `ENC_MEMORY_FRAGMENT_04`, `ENC_SHOP_02`, `ENC_MORAL_CHOICE_03` |
| 5 | `ENC_MEMORY_FRAGMENT_05`, `ENC_COMBAT_GATE_03`, `ENC_META_TEXT_01`, `ENC_META_TEXT_02` |

## Proposed Floor 3 Sequence

| Step | Node kind | Encounter stableId | Purpose | Expected gate/result | Implementation note |
|---:|---|---|---|---|---|
| 3-1 | Rest | `ENC_REST_05` | Let player recover after Floor 2 clear/failure pressure and surface S3 shift through placeholder keys. | HP/mental/affinity changes from baked effects. | Public text remains placeholder. |
| 3-2 | Moral/Memory | `ENC_MORAL_CHOICE_02` | Mid-run tradeoff that tests P5 before difficulty rises. | Applies baked moral effects and NPC reaction key. | No final ethical copy. |
| 3-3 | Memory | `ENC_MEMORY_FRAGMENT_03` | Unlock S3 fracture fragment. | Unlocks `MEM_FRAGMENT_03`. | Must use persistent memory unlock policy from restart work. |
| 3-4 | Story support | `ENC_STORY_01` | Non-combat pacing step before gate. | Applies flags/effects from existing SO. | Use only existing placeholder keys. |
| 3-5 | Combat/Gate | `ENC_COMBAT_GATE_02` | Floor 3 gate in full-scope pack. | Combat victory advances to Floor 4. | Blocked unless `d94697f` Floor 2 BossGate override is made route-local or reassigned safely. |

### Floor 3 Acceptance Criteria

| ID | Criteria |
|---|---|
| F3-AC-01 | Floor 3 path can be attached without changing the external pack source file. |
| F3-AC-02 | `ENC_MEMORY_FRAGMENT_03` unlocks exactly once and remains available after restart/regression if memory retention is enabled. |
| F3-AC-03 | `ENC_COMBAT_GATE_02` does not point to `BOSS_GATE_01` when used as Floor 3 full-scope combat. |
| F3-AC-04 | Floor 3 completion saves floor reflection once and unlocks Floor 4. |

## Proposed Floor 4 Sequence

| Step | Node kind | Encounter stableId | Purpose | Expected gate/result | Implementation note |
|---:|---|---|---|---|---|
| 4-1 | Rest | `ENC_REST_07` | Recovery and S4 collapse-state pacing. | Rest/Continue choice effects. | Placeholder only. |
| 4-2 | Shop | `ENC_SHOP_02` | Late shop sells `ITEM_LANTERN_OIL` and `ABILITY_RECALL_ANCHOR` from existing SO refs. | Player can buy or leave. | Requires economy check so Recall Anchor is reachable but not mandatory. |
| 4-3 | Moral/Memory | `ENC_MORAL_CHOICE_03` | Late moral pressure before final floor. | Applies baked moral effects. | Writer-owned text. |
| 4-4 | Memory | `ENC_MEMORY_FRAGMENT_04` | Unlock S4 collapse memory. | Unlocks `MEM_FRAGMENT_04`. | Must not reveal final truth in public copy. |
| 4-5 | Rest alternate | `ENC_REST_08` | Optional final recovery or route compression step. | Rest/Continue choice effects. | Use if Floor 4 lacks combat pressure. |

### Floor 4 Gap Call

| Gap | Severity | Reason | Next dev action |
|---|---|---|---|
| No Floor 4 combat gate in existing pack | P1 | The existing pack has no combat encounter with `floor: 4`; Floor 4 can still be playable but may feel low-pressure. | Either accept Floor 4 as shop/moral/memory attrition floor, or route final pressure into Floor 5 quickly. Do not mint new stableIds in this pass. |
| `ENC_SHOP_02` economy not validated | P1 | Recall Anchor cost and late-shop gold availability need confirmation after Floor 3 rewards are wired. | Add balance test covering buy item, buy Recall Anchor, and leave path. |

### Floor 4 Acceptance Criteria

| ID | Criteria |
|---|---|
| F4-AC-01 | Floor 4 has at least one Rest/Shop step, one moral step, and one memory step. |
| F4-AC-02 | `ENC_SHOP_02` catalog refs resolve to known item/ability assets. |
| F4-AC-03 | Floor 4 can be completed without buying Recall Anchor. |
| F4-AC-04 | Buying Recall Anchor changes later combat/restart outcomes but cannot trigger more than once per run. |

## Proposed Floor 5 Sequence

| Step | Node kind | Encounter stableId | Purpose | Expected gate/result | Implementation note |
|---:|---|---|---|---|---|
| 5-1 | Memory | `ENC_MEMORY_FRAGMENT_05` | Last memory fragment before final gate. | Unlocks `MEM_FRAGMENT_05`. | S5 body prose remains writer-owned. |
| 5-2 | Meta/Story support | `ENC_META_TEXT_01` | Pre-boss final choice/resolve setup using existing placeholder keys. | Sets existing meta flags/effects. | Do not treat as ending choice yet. |
| 5-3 | Boss/Gate | `ENC_COMBAT_GATE_03` + `BOSS_APEX_02` | Final boss combat. | Victory enters `run.clear` then ending choice ready; defeat enters `run.failed` unless Recall Anchor intervenes. | Current SO points to `ENEMY_COLLAPSE_ECHO`; dev must override or bind `BOSS_APEX_02` without changing schema. |
| 5-4 | Ending choice shell | `ENC_META_TEXT_02` | Existing post-boss/meta slot for Rest/Continue choice shell. | `ending.rest` or `ending.continue`. | Placeholder keys only; see ending spec. |

### Floor 5 Acceptance Criteria

| ID | Criteria |
|---|---|
| F5-AC-01 | `BOSS_APEX_02` is the enemy used for final boss combat, not `ENEMY_COLLAPSE_ECHO`. |
| F5-AC-02 | Boss victory grants run clear once and opens ending choice once. |
| F5-AC-03 | Boss defeat reaches `run.failed` unless unused Recall Anchor revives once. |
| F5-AC-04 | `ENC_META_TEXT_02` can host ending choice shell without final prose. |
| F5-AC-05 | `MEM_FRAGMENT_05` unlock does not force a final lore reveal before writer approval. |

## Floor 3-5 Dev Order

| Priority | Work |
|---|---|
| P0 | Decide how `ENC_COMBAT_GATE_02` can be used after Floor 2 BossGate override; avoid breaking current demo clear path. |
| P0 | Attach Floor 3-5 `floorRunPaths` in room data after route-local combat binding is safe. |
| P0 | Bind `BOSS_APEX_02` to final combat and create `run.clear -> ending.choice.ready` transition. |
| P1 | Validate Floor 4 as attrition/non-combat floor or compress it into a faster route. |
| P1 | Balance gold so Floor 4 shop choices matter and final boss can be cleared with/without Recall Anchor. |
| P2 | Add screenshot QA states for Floor 3 memory, Floor 4 shop, Floor 5 boss, and ending choice shell. |
