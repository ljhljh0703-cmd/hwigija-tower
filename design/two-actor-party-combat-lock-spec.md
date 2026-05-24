# Two-Actor Party Combat Lock Spec

> 기준: GDD D-029, D-030, D-031, D-032, OQ-019, OQ-021 close, OQ-022 temporary, OQ-023 defer, OQ-024 close, First Build Surface docs
> 목적: 다음 Stage 전투 기준선을 `Player + Mataios vs Enemy` 2인 파티 전투로 잠근다.
> 비범위: 구현, Unity asset/runtime 수정, 다중 적, 마타이오스 직접 조작, 본편 runtime 실시간 RL, 마타이오스 대사/스토리 작성.

## 0. Conclusion Summary

- 마타이오스는 UI 보조자가 아니라 전투 actor다.
- 조작은 플레이어 직접 입력이 아니라 deterministic automatic policy다.
- 같은 seed와 같은 플레이어 입력, 같은 빌드/적 상태라면 마타이오스 행동과 전투 결과가 같아야 한다.
- 붕괴도/Affinity/NPC state는 전투 수치 modifier로 재도입하지 않는다.
- 마타이오스 down은 패배 조건이 아니라 관계/회복/런 압박 이벤트다.
- 1차 배치는 적 1체 구조를 유지하고, 다중 적과 RL 학습은 제외한다.
- Down/collapse 값은 최종 lock이 아니라 1차 temporary implementation contract다.
- `광폭/FRENZY`는 이번 2인 전투 배치에서 재설계하지 않고 기존 Player action chain 기준을 유지한다.

## 1. Actor Model

### 1.1 Shared Combat Actor State

| state | Player | Mataios | Enemy | note |
|---|---|---|---|---|
| `actorId` | yes | yes | yes | deterministic log key |
| `role` | `player` | `ally_auto` | `enemy` | branch 없이 읽히는 role enum 권장 |
| `hp` / `maxHp` | yes | yes | yes | Mataios 1차 Max HP 16 |
| `actionPower` / `attack` | yes | yes | yes | Player는 기존 ATK/build modifier, Mataios 1차 action power 3 |
| `guardState` | yes | yes | optional | `none`, `defending`, `protected`, `counterReady` 등 round-local |
| `down/death state` | player death | down | defeated | Mataios down은 combat end가 아님 |
| `actionState` | selected | policy-selected | intent/action | round log와 QA metric source |
| `targetability` | targetable if alive | targetable if up | targetable if alive | Mataios down 중 targetable false 추천 |
| `round-local flags` | yes | yes | yes | first hit, counter window, chain state, downThisRound |

### 1.2 Actor-Specific Responsibilities

| actor | owns | does not own |
|---|---|---|
| Player | 입력 action, 보유 능력/아이템/시너지, 플레이어 HP/ATK, `SWORD_03`, `광폭`, `반사`의 player-facing state | Mataios policy decision, collapse acceleration numeric value |
| Mataios | HP, down state, deterministic policy state, automatic action, contribution log | 직접 조작, Affinity/NPC state 기반 combat modifier, 플레이어 빌드 chain 대리 유지 |
| Enemy | HP/ATK, intent/pattern, target selection, reward/collapse payload | 다중 적 squad behavior, relation-state scaling |

## 2. Turn Order

### 2.1 Recommended Order

1. Round-start effects resolve.
2. Enemy intent/pattern is available if current runtime supports it.
3. Player selects action.
4. Mataios automatic policy selects action from the same pre-resolution snapshot plus selected player action.
5. Party actions resolve in fixed order: Player action, then Mataios action.
6. If enemy is defeated, enemy action is skipped and combat ends.
7. Enemy action resolves against deterministic target selection.
8. Down/collapse events and end-of-round effects resolve.
9. Round summary is emitted.

### 2.2 Why This Order

- Player still leads the turn, so Mataios does not steal agency.
- Mataios can adapt to the player action chosen this round without reading future random results.
- Fixed Player-then-Mataios party resolution avoids ambiguous simultaneous kill/damage ordering.
- Enemy can still punish over-aggression after party actions if not defeated.
- P4 is preserved because all decisions are based on explicit state snapshots and seeded RNG.

### 2.3 Alternative Considered

| alternative | reason rejected |
|---|---|
| Mataios acts before player input | 마타이오스가 turn tempo를 먼저 정해 플레이어 선택 결과가 밀림 |
| Fully simultaneous party resolution | kill/overkill/down ordering ambiguity가 증가 |
| Enemy action between Player and Mataios | 동료가 뒤늦게 행동해 파티 감각이 약하고 round log가 복잡해짐 |

## 3. Mataios Automatic Policy

### 3.1 Policy Inputs

| input | allowed | note |
|---|---|---|
| recent Player action history | yes | Attack/Defend/Skill repetition only |
| current Player HP ratio | yes | exact threshold is implementation/balance constant |
| current Mataios HP ratio | yes | first threshold: >25% for Protect eligibility |
| Enemy HP ratio | yes | finisher support 판단 |
| Enemy intent/pattern | yes if runtime exposes it | OQ-019와 연결 가능하나 ITEM_07 확정 blocker로 만들지 않음 |
| repeated Player action type | yes | adaptive but deterministic |
| Affinity/NPC state | no | D-029 금지선 |
| 붕괴도 value | no for action strength | down event pressure only |

### 3.2 Action Set

| action | role | play feel | guardrail |
|---|---|---|---|
| Basic Attack | contribution damage | 동료가 실제로 때린다는 감각 | Player 공격 빌드를 대체할 만큼 강하면 안 됨 |
| Protect / Mitigate | defensive support | 과공격 플레이어를 짧게 받쳐 줌 | GUARD build보다 강한 기본 방어축 금지 |
| Counter Prep | response setup | 적 공격을 받아낸 뒤 짧은 반격 기회 | `반사`의 player Defend→next Attack 보상과 중복 금지 |
| Weak Stabilize | emergency recovery | down 직전 버팀 | 회복만 강한 런 금지, exact 수치 PM 결정 |
| Finisher Support | cleanup | 적이 낮은 HP일 때 마무리 보조 | 매 턴 최적 해법이 되지 않게 priority 제한 |
| No Action | down/blocked | down 상태 표현 | down 중 자동 회복 금지 |

### 3.3 Recommended Deterministic Priority

Policy는 weighted RNG가 아니라 ordered rule table로 시작한다. 아래 값은 OQ-024로 닫힌 1차 구현값이며, 최종 밸런스 잠금은 아니다.

| priority | condition | action | rationale |
|---|---|---|---|
| 0 | Mataios is down | No Action | down은 전투 actor 부재 상태 |
| 1 | Player HP ratio <= 35% and Mataios HP ratio > 25% | Protect | this round enemy damage to Player -2 |
| 2 | Enemy HP <= Mataios action power | Finisher Attack | enemy damage 3 |
| 3 | last 2 Player actions include Defend at least 2 times | Counter Assist | enemy damage 2 |
| 4 | last 2 Player actions include Attack at least 2 times | Pressure Attack | enemy damage 3 |
| 5 | default | Support Attack | enemy damage 2 |

### 3.4 Anti-Auto-Play Rules

- Mataios는 항상 최적 행동을 고르지 않는다. Policy는 bounded priority table이며, build resolver를 대신하지 않는다.
- Mataios action power는 Player build damage보다 낮은 보조축으로 시작한다. 1차 구현값은 3.
- Mataios는 Player의 `SWORD_03` HP cost, `광폭` chain, `반사` counter window를 대리 발동하지 않는다.
- Runtime RL 학습, on-device policy mutation, external model call은 본편 전투에 넣지 않는다.

## 4. Down / Collapse Event

### 4.1 Down State

| rule | recommended lock |
|---|---|
| trigger | Mataios HP reaches 0 |
| action while down | none |
| targetability while down | false |
| combat continuation | Player solo combat continues |
| defeat condition | Player HP 0 or Enemy defeated; Mataios down alone is not defeat |
| down recovery during combat | no by default |
| repeated down penalty in same combat | temporary: one down penalty per combat |

### 4.2 Collapse Event

- Down immediately emits a collapse event.
- Temporary first implementation value: collapse +5 when Mataios goes down.
- Temporary penalty limit: one down penalty per combat.
- Collapse event does not modify attack, defense, skill damage, enemy ATK, synergy multiplier, or Mataios action power.
- Event presentation must use combat log + non-blocking overlay first.
- Blocking popup is forbidden in the first implementation.
- Collapse value and event route must be replaceable after playtest. Do not hard-code the numeric value into combat formulas, and do not build the UI as a hard-coded modal path.

### 4.3 Temporary Contract Status

OQ-022 is partial-closed as a temporary implementation contract. It is intentionally not final balance/design lock and must remain easy to replace.

## 5. Recovery Rules

| recovery source | recommended rule | status |
|---|---|---|
| combat end, not down | restore 25% of Max HP, minimum 3 | OQ-021 first implementation value |
| combat end, was down | return at HP 4 | OQ-021 first implementation value |
| Rest node | full recover Mataios HP and clear down | OQ-021 first implementation value |
| Shop/item/ability | does not affect Mataios in first batch unless explicitly tagged party recovery | keep controlled item pool player-facing |
| player recovery | separate from Mataios recovery | avoid shared hidden heal side effects |

P1 note: Mataios recovery should create a choice between pushing the run and spending Rest tempo. It must not become free sustain that removes collapse pressure.

## 6. Existing Build Surface Interaction

| surface | interaction rule | reason |
|---|---|---|
| `SWORD_03` 피의 서약/과부하 | Player Attack only. Mataios attacks do not pay HP and do not receive +5. | HP-for-tempo remains player choice |
| `광폭` / `FRENZY` | keep existing Player action chain behavior | Mataios action must not maintain, strengthen, or break chain |
| `ARTS_03` | Player Skill action only. Mataios policy may react after player selects Skill, but does not count as Skill use. | breaks SCOUT monopoly without auto-casting |
| `GUARD_01` | Player Defend modifier only. Mataios Protect is separate ally action. | player defense build remains readable |
| controlled item pool | first batch remains Player-facing unless item explicitly says party/Mataios | avoids hidden support effects |
| combat log/summary | add Mataios action, contribution damage, down event, collapse acceleration flag | QA and player feedback need source attribution |

### 6.1 FRENZY Defer Status

Do not redesign `광폭/FRENZY` in this two-actor combat batch.

| option | effect | recommendation |
|---|---|---|
| Existing Player chain | keep current behavior and regression-test it | first implementation |
| Party chain | Mataios actions affect chain | deferred |
| ally-linked Frenzy redesign | 광폭 and companion behavior interact | deferred until skill expansion or larger combat expansion |

Developer instruction: no `광폭` structural refactor, no new Frenzy numbers, no new Frenzy design proposals in this batch. Only verify that two-actor combat does not break existing Player-chain behavior.

## 7. UI / Feedback Requirements

| UI area | requirement |
|---|---|
| HP display | show Player, Mataios, Enemy HP as three readable combat actors |
| Mataios action | one compact action log line per round |
| Down event | clear down state indicator and immediate collapse event feedback |
| Collapse display | use 붕괴도 naming in normal UI; do not expose `Glitch` |
| Mobile vertical density | combat log should summarize round-level events, not dump every modifier line |
| Build attribution | player build effects and Mataios support should be visually/logically distinct |

No final NPC/story dialogue is specified here.

## 8. AI QA Metric Contract

These metrics are for post-implementation AI QA Track ingestion. They are not implementation scope for this design doc.

| metric | definition |
|---|---|
| `mataios_action_count` | count by action type per combat/run |
| `mataios_contribution_damage` | total damage dealt by Mataios |
| `mataios_down_count` | down events per combat/run |
| `collapse_acceleration_count` | collapse acceleration events caused by down |
| `player_survival_rate` | survival over deterministic seed set |
| `average_ttk` | average rounds to defeat enemy |
| `build_completion_rate` | run share reaching x3 synergy or target build milestones |

## 9. GDD Impact Proposal

### D-032 Locked Decision

Lock `Player + Mataios vs Enemy` as the next combat baseline:

- Mataios is an automatic combat actor.
- Runtime policy is deterministic adaptive rule logic, not live RL.
- Mataios down does not end combat.
- Down emits collapse pressure and post-combat recovery handling.
- First batch keeps single enemy.
- D-029 remains intact: 붕괴도/Affinity/NPC state do not drive combat modifiers.

### New OQ Candidates

| ID | question |
|---|---|
| OQ-021 | closed: Max HP 16, action power 3, post-combat recovery rules, Rest full recovery |
| OQ-022 | partial-close: temporary down/collapse contract, +5, once per combat, non-blocking log/overlay |
| OQ-023 | defer: keep existing Player chain; party chain and ally-linked redesign move to future combat expansion |
| OQ-024 | closed: first deterministic policy rule table |

## 10. Development Session First Implementation Brief

Project tier: small-game prototype moving into a deeper core combat baseline.

Recommended modules:

| module | responsibility |
|---|---|
| Combat actor state | Player/Mataios/Enemy shared runtime state and down/targetability flags |
| Turn resolver | fixed round order, party action resolution, enemy action, end-of-round events |
| Mataios policy | deterministic ordered rule table using allowed inputs |
| Collapse event bridge | down event to collapse/run pressure, with no combat modifier output |
| Combat presentation adapter | HP/action/down/log summary data for UI |
| QA metric emitter | lightweight structured result fields for later AI QA Track |

First implementation scope:

- Add Mataios actor state and HP lifecycle.
- Add deterministic automatic policy with the OQ-024 first rule table.
- Extend round result/log summary with Mataios action, damage, down state, collapse event flag.
- Implement OQ-021 first values as data/config: Max HP 16, action power 3, combat-end recovery, Rest full recovery.
- Implement OQ-022 temporary down/collapse through a replaceable event/config path: collapse +5, one penalty per combat, combat log + non-blocking overlay.
- Keep enemy count at one.
- Keep existing player build surface semantics player-owned.
- Keep `광폭/FRENZY` existing Player-chain behavior; regression-test only.

Skip now:

- Multi-enemy target logic.
- Direct control for Mataios.
- Runtime RL/adaptive learning.
- New NPC dialogue/story content.
- `ITEM_07` concrete pattern contract unless OQ-019 is separately closed.
- `광폭/FRENZY` structural refactor, party chain, ally-linked redesign, or new Frenzy numbers.
- Hard-coded collapse modal or hard-coded collapse constants buried in combat formulas.

## 11. QA Session Checklist

| check | expected |
|---|---|
| deterministic replay | same seed and player inputs produce same Mataios actions/results |
| actor visibility | Player/Mataios/Enemy HP visible in combat |
| down continuation | Mataios down does not end combat |
| down pressure | down emits temporary collapse +5 once per combat without changing combat damage formulas |
| down presentation | combat log + non-blocking overlay; no blocking popup |
| post-combat recovery | not down: Max HP 25%, minimum 3; was down: HP 4 |
| Rest recovery | full Mataios HP recovery and down clear |
| `SWORD_03` | only Player Attack uses HP cost/overload |
| `광폭` | existing Player action chain still works; Mataios action does not affect it |
| `ARTS_03` | Player Skill surface remains player action, not Mataios auto-cast |
| `GUARD_01` | Player Defend modifier remains distinct from Mataios Protect |
| log density | mobile combat log remains scannable |
| metric readiness | result summary exposes fields AI QA Track needs |

## 12. Blockers

- OQ-019 remains open for `ITEM_07`; do not include concrete `ITEM_07` pattern effect.
- OQ-022 is temporary, not final: code must keep collapse amount/presentation/event route replaceable.
- OQ-023 is deferred: do not redesign or refactor `광폭/FRENZY` beyond regression protection.

## 13. Pillar Check

| pillar | fit |
|---|---|
| P1 | down/recovery/collapse makes companionship vulnerable without turning 붕괴도 into damage scaling |
| P2 | 마타이오스 remains a character/system presence, not an RL tech demo |
| P3 | one ally + one enemy keeps 5-7 minute combat readable |
| P4 | deterministic policy and fixed turn order preserve replay trust |
| P5 | Mataios actions, down, and collapse pressure are visible immediately in combat logs/UI |
