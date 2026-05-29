# Enemy Intent Mini Batch

Status: design-only mini batch for playable completeness.
Scope: 1-2 example intents and action counterplay mapping only. This does not implement OQ-025, enemy decks, payload tables, runtime ML/RL/ONNX, balance retuning, or final story/dialogue.

## Source Alignment

- D-033 locks the high-level intent/counterplay direction.
- D-036 requires combat preview and feedback to come from resolver-owned deterministic data.
- OQ-025 remains open for full enemy intent decks and exact enemy-specific payloads.

## Pillar Check

| Pillar | Fit |
|--------|-----|
| P1 | Intent preview makes pressure readable before collapse/recovery decisions. |
| P2 | No runtime learning, final NPC prose, or external model binding is introduced. |
| P3 | One visible intent per round keeps mobile combat scan-friendly. |
| P4 | Future implementation must derive intent from seeded combat state/resolver data, not wall clock or random globals. |
| P5 | The player sees immediate consequences through action matchups before choosing Attack/Defend/Skill. |

## Counterplay Mapping

| Enemy intent family | Attack | Defend | Skill |
|---------------------|--------|--------|-------|
| Attack intent | Race damage if lethal or safe. | Primary answer when incoming damage matters. | Punish only when preview says the skill is usable and valuable. |
| Defend intent | Lower priority unless the preview still shows meaningful damage. | Stabilize if player HP is unsafe, but it may lose tempo. | Primary answer when the skill preview shows value into guard/brace state. |
| Skill intent | Race damage if enemy setup can be ended now. | Primary answer when the skill is a clear incoming threat. | Counter-skill only when existing player skill rules support it. |

This table is qualitative. It must not create hidden damage numbers, cooldowns, status effects, or enemy defense fields by itself.

## Example 1: Pressure Strike

| Field | Draft |
|-------|-------|
| Example enemy | `ENEMY_BANDIT_MELEE_01` |
| Intent family | Attack |
| Player read | Enemy is preparing direct pressure with existing attack value. |
| Best response | Defend if incoming damage matters; Attack if the preview shows lethal tempo; Skill if current skill preview shows a stronger punish. |
| Implementation guard | Use existing enemy attack/resolver preview only. Do not add new damage values. |

## Example 2: Brace And Watch

| Field | Draft |
|-------|-------|
| Example enemy | `ENEMY_EMPTY_ARMOR` |
| Intent family | Defend |
| Player read | Enemy is bracing, so the turn is about tempo rather than a new damage spike. |
| Best response | Skill if current skill preview shows value; otherwise Attack for progress or Defend only when player HP requires safety. |
| Implementation guard | Do not invent an armor/defense stat unless a resolver-owned defense field is separately approved. |

## Non-goals

- No final prose. Any Korean UI line remains placeholder system copy until writing pass.
- No full OQ-025 enemy deck.
- No per-enemy payload table.
- No new combat balance numbers.
- No item behavior, `ITEM_07`, runtime RL, ONNX, or ML-Agents binding.

## Future Implementation Checklist

1. Add a resolver-owned, non-mutating intent preview DTO only after OQ-025 is narrowed.
2. Choose intent deterministically from combat state and run seed.
3. Keep UI text short and system-register, not NPC/story voice.
4. Add tests that the same run/combat state returns the same intent and that unsupported values are hidden.
