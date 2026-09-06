# Combat Core Rebuild Spec

> 기준: GDD D-029, D-030, D-031, D-032, OQ-019 open, OQ-021 close, OQ-022 temporary, OQ-023 defer, OQ-024 close
> 목적: attack spam damage race를 `Player + Mataios vs Enemy` 기반의 전략적 턴 전투로 교체한다.
> 비범위: 구현, Unity asset/runtime 수정, `광폭/FRENZY` 재설계, `ITEM_07` 구현, 다중 적, 본편 runtime RL, 배포/APK.

## 0. Conclusion

Combat Core Rebuild는 D-032 확장만으로는 부족하다. D-032는 2인 actor baseline이고, 이 spec은 그 위에 enemy intent, counterplay, feedback, balance target을 올리는 상위 전투 코어 잠금안이다.

Recommendation: create D-033 as "Combat Core Rebuild: Intent + Counterplay + Feedback" rather than expanding D-032 indefinitely.

## 1. Problem Statement

| current problem | rebuild answer |
|---|---|
| Attack spam으로 대부분 클리어 | visible enemy intent + counterplay table makes Attack sometimes wrong |
| Defend 선택 이유 부족 | Heavy/Charge/Special intent에서 damage prevention and counter window value |
| Skill 선택 이유 부족 | Guard/Brace, Charge, Weak timing에 burst/interrupt/setup role 부여 |
| 적 패턴/위협 부재 | every enemy turn exposes an intent and predictable pressure |
| 마타이오스 actor 부재 | D-032 two-actor baseline with automatic deterministic policy |
| SFX/VFX 미사용 | event-to-cue trigger map in Batch 3 |
| 전투 로그/결과 약함 | round summary must explain intent, response, Mataios action, damage, down/collapse |
| 긴장감 없는 damage race | HP pressure + down pressure + charge timers + cooldown timing + build payoff |

## 2. Non-Negotiables

- D-029 stays intact: 붕괴도/Affinity/NPC state are not combat modifiers.
- Mataios is a combat actor, not a relationship buff.
- Same seed/input/build/enemy state must produce the same outcome.
- First rebuild keeps one enemy.
- `광폭/FRENZY` redesign is deferred. Preserve existing Player action chain only.
- `ITEM_07` is still blocked by OQ-019.
- No real-time RL in the shipped runtime.

## 3. Target Combat Loop

1. Round-start effects resolve.
2. Enemy intent is revealed for the current round.
3. Player chooses Attack / Defend / Skill.
4. Mataios deterministic policy chooses an automatic action.
5. Player action resolves.
6. Mataios action resolves.
7. Enemy intent resolves.
8. Down/collapse, cooldown, charge, and round-local flags update.
9. Combat log emits one compact summary.

This keeps D-032 turn order but inserts intent reveal before player choice and intent-specific resolution after party actions.

## 4. Enemy Intent System

### 4.1 Intent Types

| intent | enemy behavior | player question | preferred responses | poor response |
|---|---|---|---|---|
| `normal_attack` | baseline damage this round | can I race or should I preserve HP? | Attack if stable, Defend if low HP, Mataios pressure | blind Skill if cooldown value is wasted |
| `heavy_attack` | high incoming damage, often single target | can I absorb this hit? | Defend, Mataios Protect, Guard build payoff | Attack spam at low HP |
| `guard_brace` | enemy reduces direct Attack damage or prepares armor | should I bypass/setup instead of hitting shield? | Skill, status/setup, defend/cooldown recovery | raw Attack into guard |
| `charge` | enemy spends a turn charging a future threat | can I burst or interrupt before payoff? | Attack burst, Skill burst/interrupt, Mataios pressure | passive Defend if charge continues unchecked |
| `weak_opening` | enemy exposes a punish window or lower damage | can I cash in damage now? | Attack, `광폭` Player chain, finisher, Skill if lethal | over-defending |
| `special_boss` | boss-specific rule or staged threat | what pattern rule matters now? | pattern-specific counterplay | ignoring telegraph |

### 4.2 Intent Contract

Each intent should expose:

| field | purpose |
|---|---|
| `intentId` | deterministic resolver key |
| `intentType` | one of the intent types above |
| `telegraphLabel` | short UI label, not story dialogue |
| `baseThreat` | low / medium / high / lethal-warning tier |
| `counterTags` | Attack / Defend / Skill / Mataios response tags |
| `resolvePayload` | damage, guard, charge stack, opening flag, or special result |
| `sfxVfxHint` | cue group for Batch 3 feedback |

This is a gameplay contract, not final writer-facing enemy text.

## 5. Action Counterplay Matrix

| enemy intent | Attack | Defend | Skill | Mataios automatic action |
|---|---|---|---|---|
| `normal_attack` | acceptable race if HP stable | valid HP preservation | use only if cooldown/kill justifies | Support Attack or Protect by policy |
| `heavy_attack` | risky unless lethal | primary answer; should visibly reduce damage | emergency burst only if lethal/interrupt-capable | Protect becomes high-value if Player HP low |
| `guard_brace` | reduced value; should feel inefficient | safe setup, not a win button | primary answer if skill bypasses guard or applies status | Pressure chip or Counter Assist |
| `charge` | good if it advances interrupt/lethal | buys HP but may not stop future threat | primary interrupt/burst candidate | Pressure Attack if recent Attack pattern, Finish if possible |
| `weak_opening` | primary punish window; `광폭` payoff | usually low value unless HP critical | burst if cooldown timing fits | Finish Assist / Pressure Attack |
| `special_boss` | depends on telegraph rule | depends on telegraph rule | depends on telegraph rule | policy stays bounded; no boss auto-solver |

## 6. Player Action Role Redefinition

| action | new role | should feel good when | should feel bad when |
|---|---|---|---|
| Attack | punish, finish, Player-chain payoff | enemy is weak/open, charge can be raced, lethal is near | enemy is braced or heavy attack threatens survival |
| Defend | survival, heavy counterplay, Guard build payoff | heavy/special threat is incoming, Mataios needs time, HP is low | enemy is weak/open and damage window is being wasted |
| Skill | cooldown-limited burst, interrupt, bypass/setup | enemy guards, charges, or needs burst timing | used blindly into low-value normal attack |

`광폭/FRENZY` note: Attack chain remains Player-action only. Mataios action never maintains, strengthens, or breaks chain.

## 7. Mataios Combat Role

Mataios is not just extra damage. The first policy from OQ-024 creates five strategic roles:

| role | trigger/use | player-facing value |
|---|---|---|
| protection | Player HP <=35% and Mataios HP >25% | heavy/normal attack no longer forces pure damage race |
| finish assist | enemy HP <= action power | reduces dead turns and makes ally visible |
| pressure | repeated Player Attack | supports aggressive plan without preserving `광폭` chain |
| stabilize | default support and down recovery loop | keeps party presence steady without becoming heal engine |
| down/collapse risk | Mataios HP reaches 0 | turns careless fights into run-pressure events |

Guardrails:

- No Affinity/NPC state/붕괴도 policy input.
- No random action selection.
- No player build chain proxy.
- Policy result must appear as one combat log line.

## 8. Down / Collapse Pressure

Use the OQ-022 temporary contract:

- Mataios HP 0 -> down.
- Combat continues.
- Down emits collapse event.
- Temporary collapse delta: +5.
- One down penalty per combat.
- Blocking popup forbidden.
- Use combat log + non-blocking overlay.
- The numeric delta and event route must be config/event driven, not buried as hard-coded combat formula.

This creates pressure without making collapse a combat buff/debuff.

## 9. SFX / VFX Trigger Map

Current usable assets:

- SFX: `sfx_combat_attack`, `sfx_combat_defend`, `sfx_combat_enemy_hit`, `sfx_combat_victory`
- VFX: `vfx_combat_hit_slash`, `vfx_combat_defend_shield`, `vfx_memory_unlock_pulse`
- UI icons: `icon_action_attack`, `icon_action_defend`, `icon_action_skill_scout`

| combat event | SFX cue | VFX/UI cue | note |
|---|---|---|---|
| Player Attack hit | `sfx_combat_attack` | `vfx_combat_hit_slash` | baseline hit feedback |
| Defend success | `sfx_combat_defend` | `vfx_combat_defend_shield` | must make mitigation visible |
| Enemy heavy intent reveal | no dedicated current SFX | intent badge pulse / enemy warning tint | do not misuse hit SFX before impact |
| Enemy hit impact | `sfx_combat_enemy_hit` | hit flash on party target | player/Mataios target attribution needed |
| Skill hit | temporary reuse `sfx_combat_attack` only if no skill cue exists | `vfx_combat_hit_slash` with distinct timing/tint | new skill SFX can be later asset task |
| Mataios attack/pressure/finish | `sfx_combat_attack` | compact ally action flash | no dialogue/voice line |
| Mataios Protect | `sfx_combat_defend` | shield flash on Player/Mataios line | show source as Mataios |
| Mataios down | `sfx_combat_enemy_hit` low-priority variant if supported | non-blocking collapse overlay; no modal | no story dialogue |
| `광폭` trigger | `sfx_combat_attack` | second slash flash | no new Frenzy design |
| Victory | `sfx_combat_victory` | victory result panel pulse | after final damage summary |

Batch 3 may add cue IDs in `SO_AudioCueCatalog`, but should not require new audio files before mapping existing assets.

## 10. Tension Design

Every 1-2 turns, the player should answer at least one concrete question:

| pressure | source | decision it creates |
|---|---|---|
| HP pressure | normal/heavy attack and low HP thresholds | attack race or defend/protect |
| down/collapse pressure | Mataios target damage and down state | preserve ally or accept collapse acceleration |
| charge timer | charge intent | burst/interrupt now or prepare for payoff |
| skill cooldown | ARTS_03 and future skills | spend burst now or save for guard/charge/opening |
| build payoff | Sword/Guard/Skill item paths | use the action your build asks for, not default Attack |
| result feedback | log/SFX/VFX | understand why the turn worked or failed |

Attack should remain useful, but no longer universally correct.

## 11. First Balance Targets

These are first rebuild targets, not final Balance Pass locks.

| target | first goal |
|---|---|
| normal combat length | 3-5 rounds |
| elite combat length | 5-7 rounds |
| boss combat length | 6-9 rounds |
| non-Attack relevance | at least one high-value Defend or Skill window in most non-trivial combats |
| Attack spam failure mode | repeated Attack into heavy/guard/charge should cost HP, Mataios HP, tempo, or collapse risk |
| Mataios damage contribution | meaningful but secondary; should not solve combat alone |
| Mataios down | possible under poor play, not expected every normal combat |
| Skill use | owned skill should have visible timing value, especially guard/charge/opening |

## 12. Implementation Batch Plan

### Batch 1: Two-Actor Baseline

Goal: make D-032 real without changing enemy strategy yet.

Include:

- Player/Mataios/Enemy actor state.
- Mataios HP 16, action power 3, OQ-024 deterministic policy.
- OQ-022 temporary down/collapse event path.
- Combat log fields for Mataios action, damage, down, collapse event.
- Post-combat and Rest recovery from OQ-021.
- Existing `광폭/FRENZY` Player chain regression protection.

Exclude:

- Enemy intent counterplay.
- New SFX/VFX binding.
- `ITEM_07`.
- `광폭` redesign.

### Batch 2: Enemy Intent + Counterplay

Goal: break Attack spam by making enemy turns readable and answerable.

Include:

- Intent enum/data contract: normal, heavy, guard, charge, weak, special.
- Deterministic intent selection per enemy/pattern.
- Intent reveal before player action.
- Intent-specific action outcomes for Attack/Defend/Skill.
- Basic charge/opening state.
- Combat log summary: intent -> player response -> result.

Exclude:

- Multi-enemy encounters.
- Boss-specific complex scripts beyond one special intent placeholder.
- `ITEM_07` unless OQ-019 is closed.

### Batch 3: Feedback / SFX / VFX Trigger

Goal: make tactical decisions legible.

Include:

- Trigger map from section 9.
- Defend mitigation feedback.
- Heavy/charge/weak intent visual telegraph.
- Mataios action and down feedback.
- Victory feedback.
- Mobile log density pass for round summaries.

Exclude:

- Final UI polish.
- New final audio asset requests unless current cues cannot cover the event.
- NPC dialogue/story lines.

### Batch 4: Balance + AI QA Metric

Goal: tune against observed behavior instead of static guesses.

Include:

- Seed replay metric export/readiness.
- Attack spam policy replay.
- Random/greedy/survival policy comparison if AI QA Track consumes it.
- TTK, action mix, down count, collapse event count, skill usage, build completion rate.
- First balance target report against section 11.

Exclude:

- Runtime RL in the shipped game.
- Large economy/shop/rest rebalance until combat loop proves readable.

## 13. Development Session Batch 1 Brief

Project tier: small-game prototype, core combat rebuild.

Recommended modules:

| module | responsibility |
|---|---|
| Actor state | shared combat state for Player/Mataios/Enemy |
| Party resolver | fixed Player -> Mataios -> Enemy resolution |
| Mataios policy | OQ-024 deterministic rule table |
| Collapse event bridge | temporary OQ-022 event output with replaceable config |
| Combat summary | structured log/result fields |

Batch 1 acceptance:

- Same seed/input gives same Mataios actions.
- Mataios can act, take damage, down, and recover.
- Down emits collapse event once per combat.
- No blocking popup.
- `광폭/FRENZY` existing Player chain still works and ignores Mataios actions.
- No enemy intent work yet.

## 14. Development Session Batch 2 Brief

Project tier: small-game prototype, tactical layer.

Recommended modules:

| module | responsibility |
|---|---|
| Intent data | authored intent type, label, threat, payload |
| Pattern selector | deterministic per-enemy intent sequence |
| Counterplay resolver | action-vs-intent outcome rules |
| Intent UI adapter | pre-action telegraph data |
| Round summary | compact reason/result message |

Batch 2 acceptance:

- Every enemy round exposes one intent before action choice.
- Heavy attack makes Defend/Protect valuable.
- Guard/Brace makes raw Attack inefficient and Skill/setup valuable.
- Charge creates a burst/interrupt decision.
- Weak/opening creates an Attack punish window.
- Attack spam has measurable downside in at least heavy/guard/charge cases.

## 15. QA Checklist

| check | expected |
|---|---|
| deterministic replay | same seed/input/build/enemy pattern -> same result |
| attack spam test | repeated Attack should be worse than counterplay in heavy/guard/charge combats |
| Defend value | Defend visibly reduces heavy damage |
| Skill value | Skill has timing value against guard/charge/opening |
| Mataios actor | action count, damage, protection, down, recovery visible |
| collapse pressure | down causes temporary +5 once per combat, no combat modifier |
| Frenzy regression | Player chain preserved; Mataios actions do not affect chain |
| ITEM_07 guardrail | no concrete ITEM_07 behavior added |
| feedback | attack/defend/skill/Mataios/down/victory trigger mapped feedback |
| mobile log | one compact round summary, not raw debug spam |

## 16. AI QA Metric Update Note

Add or preserve these metric columns for post-implementation AI QA Track:

| metric | why |
|---|---|
| `enemy_intent_count_by_type` | verifies pattern variety |
| `player_action_by_intent` | measures counterplay adoption |
| `damage_taken_by_intent` | identifies unfair/heavy spikes |
| `defend_value_prevented_damage` | proves Defend is useful |
| `skill_uses_by_intent` | proves Skill has timing value |
| `attack_spam_win_rate` | direct regression target |
| `mataios_action_count_by_type` | validates actor presence |
| `mataios_contribution_damage` | detects auto-solver risk |
| `mataios_down_count` | collapse pressure frequency |
| `collapse_event_count` | run pressure from down |
| `average_ttk_by_enemy_tier` | pacing target |
| `build_completion_rate` | combat reward/build loop health |

## 17. GDD Impact

Recommended: D-033 candidate.

### D-033 Candidate

Lock Combat Core Rebuild as a strategic turn combat layer over D-032:

- enemy intent is visible before player action,
- Attack/Defend/Skill have intent-specific roles,
- Mataios is strategic support actor, not relationship buff,
- down/collapse pressure remains non-combat-modifier pressure,
- SFX/VFX/log feedback are part of combat readability,
- rebuild is split into four development batches.

### Follow-Up OQ

| ID | question |
|---|---|
| OQ-025 | Per-enemy first intent decks and exact payload numbers for normal/heavy/guard/charge/weak/special intents |

## 18. Pillar Check

| pillar | fit |
|---|---|
| P1 | down/collapse pressure creates loss risk without turning collapse into combat scaling |
| P2 | Mataios is a bounded character actor, not RL tech demo |
| P3 | one enemy, visible intent, compact logs preserve mobile 5-7 minute pacing |
| P4 | deterministic pattern and policy keep replay trust |
| P5 | intent/counterplay/feedback makes action consequences immediate |
