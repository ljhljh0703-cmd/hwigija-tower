# First Build Surface Development Brief

> 기준: GDD D-029, D-030, D-031; `design/playable-build-surface-lock-spec.md`  
> 목적: Balance Pass 전에 첫 작동 빌드 선택 표면을 런에 올린다.  
> 금지: 붕괴도/Affinity/NPC state 전투 modifier 재도입, unsupported dead pick 노출, exact 숫자 임의 확정.

## 1. Batch Objective

현재 runtime의 2 ability / 3 item 표면을 GDD build surface의 첫 비교 가능 subset으로 교체한다.

- 검 x3 완성 경로가 런에서 실제로 닿아야 한다.
- SCOUT가 독점한 skill 축에 명시 skill 능력 1개를 올린다.
- 효과 없는 reward/shop pick은 pool에서 빠져야 한다.
- 이번 배치는 밸런스 튜닝이 아니라 reachability + deterministic effect application 배치다.

## 2. Must Include

### 2.1 Dead Pick Pool Guard

- reachable pool에서 `ITEM_LANTERN_OIL` 제거 또는 build pick 비노출 처리.
- reachable pool에서 `ITEM_TORN_CHARM` 제거 또는 build pick 비노출 처리.
- superseded `ITEM_07`은 OQ-019 전까지 runtime build pick 비노출.
- prototype placeholder ability/synergy를 GDD target pick처럼 노출하지 않음.

### 2.2 Controlled Normal Item Pool

지원 경로가 있는 일반 아이템만 첫 controlled pool에 노출:

| include | reason |
|---|---|
| `ITEM_01` | Max HP modifier path |
| `ITEM_02` | ATK modifier path |
| `ITEM_03` | attack flat damage path |
| `ITEM_04` | defend mitigation path |
| `ITEM_05` | round-start damage path |
| `ITEM_09` | first-hit mitigation path |
| `ITEM_10` | Max HP penalty + ATK trade path |

### 2.3 Sword x3 Surface

첫 검 완성 경로는 아래 target refs를 기준으로 올린다.

| include | contract |
|---|---|
| `ABILITY_SWORD_01` | 검 공격 기반 |
| `ABILITY_SWORD_02` | 검 공격 주기 추가타 |
| `ABILITY_SWORD_03` | D-031/OQ-020: Attack HP 3 비용, 해당 공격 피해 +5, HP 3 이하 전투당 1회 과부하 |
| `광폭` | D-031/OQ-020: 검 x3, Attack 추가타 ATK×0.5, 직전 Attack이면 ATK×0.75, Defend/Skill break |

`SWORD_03`와 `광폭`의 effect contract와 첫 플레이테스트 기준값은 D-031/OQ-020으로 입력됐다. 이 값은 최종 밸런스 잠금이 아니다.

### 2.4 Minimum Skill Axis

- SCOUT skill 독점을 완화할 명시 skill ability 최소 1개 포함.
- 첫 후보는 `ABILITY_ARTS_03`.
- 구현은 "owned explicit skill ability가 Skill 선택 이유를 만든다"는 surface를 우선 검증한다.

## 3. May Include

### Guard Minimum Surface

- `ABILITY_GUARD_01`을 가능하면 같은 배치에 포함한다.
- 첫 배치가 커지면 다음 배치로 분리 가능하다.
- `반사` runtime 구현은 결 x3 surface 배치 시점과 함께 판단한다.

## 4. Explicitly Deferred

| defer | reason |
|---|---|
| `ITEM_07` concrete implementation | OQ-019 pattern result contract open |
| `반사` runtime implementation | first batch is sword x3 surface; guard x3 surface may be separate |
| full 12 abilities / 12 items / 6 relics / 4 synergies exposure | unsupported entries would reintroduce dead picks |
| shop/economy/rest/boss balance tuning | playable surface first |
| UI polish | out of batch |
| release/deploy work | out of batch |

## 5. Non-Negotiable Contracts

- D-031 `SWORD_03`: Attack damage and +5 bonus resolve before HP cost death; HP 3 이하 과부하는 전투당 1회.
- D-031 `광폭`: defense or skill choice breaks attack chain.
- D-031 `광폭`: do not implement as unbounded multiplier ramp.
- D-031 `반사`: reserved direction is one strong counter opportunity on next attack after defend, not sword-like multi-hit.
- D-029: no combat input from 붕괴도/Affinity/NPC state.

## 6. Developer Must Not Decide

- OQ-019 pattern result contract or `ITEM_07` concrete effect.
- Expanding pool exposure to unsupported GDD entries.
- Balance conclusions from current prototype reward/shop pressure.
- Any replacement relation-state combat modifier for removed Glitch/Affinity effects.

## 7. Batch Exit

- Dead-pick catalog contamination listed in 2.1 is removed from first build-choice surface.
- Controlled items listed in 2.2 are reachable and apply their supported effects.
- `SWORD_01` + `SWORD_02` + `SWORD_03` can form the 검 x3 path and activate target `광폭` using the OQ-020 first-playtest baseline, including SWORD_03 overload semantics.
- Skill choice is no longer only a SCOUT-specific axis when `ARTS_03` is owned.
- Deferred scope stays out of the batch.
