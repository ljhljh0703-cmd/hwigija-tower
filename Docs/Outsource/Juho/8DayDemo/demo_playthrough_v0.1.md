# Demo Playthrough v0.1

## Scope

- Demo target date: 2026-05-12 KST.
- Branch context: Juho/Codex.
- Demo route: Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete.
- Implementation note: the first available node should be Shop. This document does not add a new entry requirement.
- Text note: all draft labels are writer-review candidates, not final NPC dialogue or final story prose.

## 1-Minute Route

| time | player action | screen state | expected result | demo value |
|---|---|---|---|---|
| 0-10s | Start run and select the first available node if needed | Vertical HUD shows HP, Mental, Gold, Glitch, Affinity. Route preview or current node shows Shop as the first available node. | Player reaches ENC_SHOP_01 without random detour. | Shows that the run has deterministic demo progression and NPC-related state is visible beside survival state. |
| 10-25s | Choose one Shop option | Shop node, Gold amount, two purchase options, one leave option. Insufficient Gold choices use disabled visible state. | Buy item: Gold -5 and ITEM_FIELD_BANDAGE. Buy ability: Gold -12 and ABILITY_SCOUT. Leave: only reaction. | Shows that choices can affect inventory, ability access, and later combat prep. |
| 25-40s | Choose one MoralChoice option | Moral node with 2-3 choices. HP or Gold gated choices may be disabled. | Aid: HP -3, Affinity +6, Glitch -3. Refuse: Mental -2, Affinity -5, Glitch +4. Trade: Gold -2, ITEM_TORN_CHARM, Affinity/Glitch change. | Shows moral pressure as state change rather than a final judgement. |
| 40-50s | Unlock or avoid the first memory fragment | Memory card or fragment panel. Title/body remain placeholder or writer-review copy. | Unlock: MEM_FRAGMENT_01, Glitch +6, Affinity +2. Withdraw: Mental +1, Glitch -1. | Shows memory as a game system that can strengthen connection while increasing instability. |
| 50-60s | Enter CombatGate, resolve demo combat, show completion panel | Combat gate screen with ENEMY_FRACTURE_HOUND visual or placeholder. Result panel appears after result. | Victory: Gold +7, Glitch -2, Affinity +2. Defeat: HP -5, Glitch +5, Affinity -2. DemoComplete appears after result. | Shows handoff from encounter choices to combat and back into NPC state. |

## Segment Detail

### 0-10s: Start and HUD

- UI: title or prototype start state, run HUD, first available node indicator.
- Draft UI copy:
  - `PLACEHOLDER_DEMO_ROUTE_START`: `검토용: 첫 번째 노드가 열렸습니다.`
  - `PLACEHOLDER_DEMO_ROUTE_HINT`: `검토용: Shop -> Moral -> Memory -> Combat`
- NPC emotion intent: cautious awareness, not full trust yet.
- System result message: route seed resolves first available node as ENC_SHOP_01.
- Differentiator: survival stats and NPC-state stats share the same decision surface.

### 10-25s: Shop

- UI: ENC_SHOP_01 body, three choices, Gold display, disabled visible purchase if Gold is low.
- Draft choice labels:
  - `CHOICE_SHOP_01_BUY_ITEM`: `검토용: 응급 붕대를 산다`
  - `CHOICE_SHOP_01_BUY_ABILITY`: `검토용: 길을 살핀다`
  - `CHOICE_SHOP_01_LEAVE`: `검토용: 지나간다`
- NPC emotion intent: watches whether the player prepares for the next floor.
- System result messages:
  - `Gold -5 / ITEM_FIELD_BANDAGE`
  - `Gold -12 / ABILITY_SCOUT`
  - `구매 조건 미충족: PLACEHOLDER_REASON_NOT_ENOUGH_GOLD`
- Differentiator: shop choice is connected to later requirement and NPC reaction, not just economy.

### 25-40s: MoralChoice

- UI: ENC_MORAL_CHOICE_01 body, 3 options, short result panel after selection.
- Draft choice labels:
  - `CHOICE_MORAL_01_AID`: `검토용: 상처를 감수한다`
  - `CHOICE_MORAL_01_REFUSE`: `검토용: 지나친다`
  - `CHOICE_MORAL_01_TRADE`: `검토용: 대가를 치른다`
- NPC emotion intent: interprets player priorities without giving a sermon.
- System result messages:
  - Aid: `HP -3 / Affinity +6 / Glitch -3`
  - Refuse: `Mental -2 / Affinity -5 / Glitch +4`
  - Trade: `Gold -2 / ITEM_TORN_CHARM / Affinity +1 / Glitch +1`
- Differentiator: moral pressure becomes relationship and context pressure.

### 40-50s: MemoryFragment

- UI: ENC_MEMORY_FRAGMENT_01, MEM_FRAGMENT_01 card, unlock or withdraw choice.
- Draft choice labels:
  - `CHOICE_MEMORY_01_UNLOCK`: `검토용: 파편을 확인한다`
  - `CHOICE_MEMORY_01_WITHDRAW`: `검토용: 물러난다`
- NPC emotion intent: uncertain recognition, no confirmed truth reveal.
- System result messages:
  - Unlock: `MEM_FRAGMENT_01 unlocked / Glitch +6 / Affinity +2`
  - Withdraw: `Mental +1 / Glitch -1`
- Differentiator: memory unlock improves connection while stressing the NPC context.

### 50-60s: CombatGate and DemoComplete

- UI: ENC_COMBAT_GATE_01, enemy placeholder or ENEMY_FRACTURE_HOUND, combat start transition, result panel, DemoComplete.
- Draft choice labels:
  - `CHOICE_COMBAT_01_ENGAGE`: `검토용: 정면으로 맞선다`
  - `CHOICE_COMBAT_01_PREPARE`: `검토용: 숨을 고른다`
- NPC emotion intent: tense but present. Use "이번엔 마주 보는구나" style if writer approves.
- System result messages:
  - Handoff: `COMBAT_GATE_01`
  - Victory: `Gold +7 / Glitch -2 / Affinity +2`
  - Defeat: `HP -5 / Glitch +5 / Affinity -2`
  - DemoComplete: `Demo route complete`
- Differentiator: combat result feeds back into NPC state and memory continuity.

## Optional Hidden Risk

- Death/Regression is not the default demo path.
- If triggered during testing, show only a short hidden-risk result panel.
- Do not require the public 1-minute demo to demonstrate death or regression.
