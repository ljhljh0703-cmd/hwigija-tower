# D-035 ContextPolicy Handoff

Status: Game Dev implementation handoff.
Accepted runtime candidate: `origin/Proto@16320a293e401d70cc004886c67a5fbaa974c437`.
Scope: deterministic Mataios combat brain only.

## Boundary

This handoff converts the AI Track result into explicit production rules. It does not add PPO inference, ONNX loading, ML-Agents runtime dependencies, remote policy calls, runtime learning, or new combat numbers.

Use the existing OQ-024 payload values for the first implementation:

| payload key | effect contract |
| --- | --- |
| `protect_player` | reduce incoming enemy damage to Player by 2 |
| `finish_attack` | deal 3 enemy damage |
| `pressure_attack` | deal 3 enemy damage |
| `tempo_attack` | deal 3 enemy damage through the existing pressure payload |
| `counter_assist` | deal 2 enemy damage |
| `skill_setup_assist` | deal 2 enemy damage and tag as Skill support |
| `support_attack` | deal 2 enemy damage |
| `none_down` | no effect |

If stronger tempo or Skill-support output is wanted later, open a separate Balance OQ. Do not introduce hidden constants in this batch.

## `MataiosCombatContext`

`MataiosCombatContext` is an immutable one-decision snapshot. It should be a pure C# value type or readonly class; no MonoBehaviour, UI, save, audio, or mutation logic belongs inside it.

Allowed fields:

| field | type | source | note |
| --- | --- | --- | --- |
| `IsMataiosDown` | `bool` | actor state | hard priority stop |
| `PlayerAction` | enum | current player input | Attack / Defend / Skill |
| `RecentPlayerActions` | small fixed buffer or counts | combat history | last two actions are enough for first implementation |
| `PlayerHp` | `int` | actor state | derive ratio in context builder or brain |
| `PlayerMaxHp` | `int` | actor state | must be positive |
| `MataiosHp` | `int` | actor state | protect eligibility |
| `MataiosMaxHp` | `int` | actor state | must be positive |
| `EnemyHp` | `int` | enemy state | finisher and Skill opportunity |
| `EnemyMaxHp` | `int` | enemy state | must be positive |
| `MataiosActionPower` | `int` | OQ-024 config | first value is existing action power 3 |
| `EnemyThreatHigh` | `bool` | enemy intent or deterministic preview | false when unavailable |
| `IncomingTargetsPlayer` | `bool` | deterministic preview if available | do not infer from randomness |
| `PlayerSkillReady` | `bool` | player combat state | Mataios never auto-casts player Skill |
| `SkillContextValuable` | `bool` | deterministic enemy/skill context | conservative false if unavailable |
| `TempoReady` | `bool` | previous round result | true for one round after successful prevention |

Forbidden inputs:

| input | rule |
| --- | --- |
| 붕괴도 value | never read for action choice or payload strength |
| Affinity / relationship state | never read for combat policy |
| NPC state / persona stage | never read for combat policy |
| training-only hidden cadence | not a production threat source |
| `stepIndex % 3` | not allowed in runtime combat |
| random roll | no weighted or stochastic policy selection |
| model output / external API | not part of policy selection |

Down state is allowed because it is actor availability, not a collapse/combat-scaling input.

## `MataiosActionPlan`

Return a value object from the brain. The resolver applies it.

Recommended fields:

| field | type | purpose |
| --- | --- | --- |
| `ActionId` | enum/string | `none_down`, `protect_player`, `finish_attack`, etc. |
| `PayloadKey` | enum/string | maps to OQ-024 payload |
| `Target` | enum/string | Player or Enemy |
| `ReasonKey` | enum/string | QA/debug log token, not normal UI prose |
| `ConsumesTempo` | `bool` | true only for `tempo_attack` |
| `MetricTags` | small string array/flags | `threat`, `skill_context`, `fallback`, etc. |

The plan must not contain final NPC dialogue. Use compact combat log tokens only.

## `MataiosCombatBrain`

Implement as a pure deterministic evaluator:

```csharp
MataiosActionPlan Decide(in MataiosCombatContext context);
```

Ordered decision table:

| priority | condition | action plan | reason |
| ---: | --- | --- | --- |
| 0 | `IsMataiosDown` | `none_down` | down actor cannot act |
| 1 | `EnemyHp <= MataiosActionPower` | `finish_attack` | visible ally cleanup, OQ-024 damage 3 |
| 2 | `TempoReady` | `tempo_attack` | spend prevention into pressure before re-defending |
| 3 | `EnemyThreatHigh && PlayerAction != Defend && MataiosHpRatio > 0.25` | `protect_player` | protect over-aggression |
| 4 | `PlayerHpRatio <= 0.35 && MataiosHpRatio > 0.25` | `protect_player` | OQ-024 emergency protection |
| 5 | `EnemyThreatHigh && PlayerAction == Defend` | `counter_assist` | reward correct defensive read without touching `반사` |
| 6 | `PlayerAction == Skill && PlayerSkillReady && SkillContextValuable` | `skill_setup_assist` | support player-chosen Skill timing |
| 7 | last two player actions are both Attack | `pressure_attack` | support aggression without affecting `광폭` |
| 8 | last two player actions are both Defend | `counter_assist` | avoid pure stall |
| 9 | default | `support_attack` | bounded ally contribution |

Tie-breaking is table order only. No priority weights.

Derived ratios:

```text
PlayerHpRatio = PlayerHp / PlayerMaxHp
MataiosHpRatio = MataiosHp / MataiosMaxHp
```

Clamp invalid HP to safe values before building context. The brain should not throw during combat because of malformed preview data; it should fall back.

## Fallback Rule

Use the OQ-024 fallback table when any required ContextPolicy input is unavailable or invalid:

| priority | condition | action |
| ---: | --- | --- |
| 0 | Mataios down | `none_down` |
| 1 | Player HP ratio <= 35% and Mataios HP ratio >25% | `protect_player` |
| 2 | Enemy HP <= Mataios action power | `finish_attack` |
| 3 | last two player actions include Defend at least twice | `counter_assist` |
| 4 | last two player actions include Attack at least twice | `pressure_attack` |
| 5 | default | `support_attack` |

Fallback reason should be recorded as a QA metric, for example `missing_enemy_threat`, `invalid_hp`, or `no_skill_context`. Do not expose provider/debug text in normal combat UI.

## Integration Notes

- The combat resolver owns HP mutation, damage application, logs, audio, and UI.
- The context builder may read enemy intent or deterministic incoming preview if the accepted runtime candidate exposes it.
- If enemy intent is still absent, set `EnemyThreatHigh = false` and use fallback rather than inventing a cadence.
- `TempoReady` is a one-round flag set by resolver result, then consumed by `tempo_attack`.
- Mataios actions do not maintain, strengthen, or break player `광폭/FRENZY`.
- Mataios actions do not trigger player `SWORD_03` HP cost or bonus.
- Mataios `counter_assist` does not multiply or replace player `반사`.
- Mataios `skill_setup_assist` does not count as player Skill use.

## Implementation Acceptance Criteria

| check | expected result |
| --- | --- |
| deterministic replay | same seed, player inputs, build state, actor state, and enemy state produce the same Mataios action sequence |
| no ML runtime dependency | combat runs without ML-Agents, ONNX files, model artifacts, or policy API |
| forbidden input guard | no brain/context field reads collapse value, Affinity, NPC state, hidden cadence, or `stepIndex % 3` |
| OQ-024 payload reuse | no new damage/heal numbers are introduced |
| high threat protection | high threat + player not defending + healthy Mataios selects `protect_player` |
| tempo spend | successful prevention makes the next eligible action `tempo_attack` once |
| Skill support | player-chosen ready Skill in valuable context selects `skill_setup_assist` and does not count as player Skill |
| low enemy HP | finisher wins before generic pressure/support |
| fallback coverage | missing enemy threat/preview data still returns a valid OQ-024 action |
| build regression | `SWORD_03`, `광폭`, `반사`, and player Skill semantics remain player-owned |
| UI cleanliness | normal UI shows compact action/result, not rule names, provider names, or debug internals |

Suggested tests:

- EditMode tests for each ordered table row.
- EditMode tests for fallback reasons.
- Replay-style test for deterministic action sequence.
- Regression tests around player-owned build surfaces.

## Portfolio Wording

Use:

- "ML-Agents를 combat design probe로 사용했다."
- "Exp04가 spam baseline을 이겼고, ContextPolicy를 deterministic companion policy 후보로 handoff했다."
- "본편 구현은 PPO/ONNX가 아니라 사람이 읽고 테스트할 수 있는 deterministic rule table이다."

Avoid:

- Any wording that says Mataios became an RL-trained runtime character.
- Any wording that says the shipped combat runtime uses RL inference.
- Any wording that presents Exp05/Exp06 as success rather than failure/near-miss evidence.

## Pillar Check

| pillar | fit |
| --- | --- |
| P1 | Mataios down/collapse remains pressure, not a combat scaling input. |
| P2 | ML-Agents evidence becomes character-facing deterministic behavior rather than tech-showcase output. |
| P3 | Ordered rules are readable in a short mobile combat loop. |
| P4 | No random/model inference; same context returns same plan. |
| P5 | Protect, tempo spend, Skill support, and finisher produce immediate combat feedback. |
