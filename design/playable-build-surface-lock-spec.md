# Playable Build Surface Lock Spec

> 기준: GDD D-009, D-022, D-023, D-025(RECONSIDERED), D-029, D-030, D-031, OQ-019, OQ-020 close  
> 목적: Balance Pass 전에 "작동하고, 런에서 노출되고, 비교 가능한" 전투 빌드 표면을 런타임 handoff 기준으로 잠근다.  
> 비범위: exact 수치 확정, UI polish, QA/배포, 붕괴도/Affinity/NPC state 전투 modifier 재도입.

## 0. Runtime Audit Snapshot

- `SO_EncounterRuntimeCatalog`가 런타임에 노출하는 build refs는 능력 `ABILITY_SCOUT`, `ABILITY_RECALL_ANCHOR` 2개와 아이템 `ITEM_FIELD_BANDAGE`, `ITEM_LANTERN_OIL`, `ITEM_TORN_CHARM` 3개다.
- GDD 12 abilities는 formal runtime catalog에 없고, GDD 12 normal items + 6 relics는 `ItemData` 자산이 있으나 catalog pool에 없다.
- 현재 skill 액션은 `ABILITY_SCOUT` 보유 여부로만 열리고, skill 결과도 공격으로 접힌다. 그래서 SCOUT는 현재 runtime skill 표면의 auto-pick 축이다.
- `ITEM_LANTERN_OIL`, `ITEM_TORN_CHARM`은 reachable이나 passive trigger와 numeric params가 비어 있다.
- 현재 `CombatAbilityModifiers`가 읽는 combat build 효과는 능력/시너지의 `player.attack_bonus`, `player.max_hp_bonus`와 아이템 일부 stat/passive keys에 국한된다.
- GDD synergies는 formal runtime assets/pool이 없고, prototype placeholder synergies는 `requiredCount: 2` stat placeholder라 GDD same-tag x3와 다르다.

## A. Target Build Role Matrix

### A1. Abilities 12

| id/name | intended role | effect summary | build axis | dependency/tag | current status | implementation note | GDD impact |
|---|---|---|---|---|---|---|---|
| `ABILITY_SWORD_01` 예리한 감각 | 검 기반 공격 성장 | 공격 선택 ATK 보너스 | direct attack | 검 | unreachable; formal asset absent; base ATK key applied only if asset exists | GDD ability asset/pool 필요 | 없음 |
| `ABILITY_SWORD_02` 연속베기 | 반복 공격 보상 | 공격 주기 추가타 | attack cadence | 검 | unreachable; effect unsupported | 공격 횟수/라운드 트리거 계약 필요 | 없음 |
| `ABILITY_SWORD_03` 피의 서약 | 리스크 강공 | Attack HP 3 비자살 비용, 해당 공격 피해 +5 | HP for tempo | 검; D-031; OQ-020 | unreachable; effect unsupported | OQ-020은 1차 기준값, 붕괴도 조건 폐기 유지 | D-031 반영 |
| `ABILITY_ARTS_01` 화염 인장 | 지속 피해 | 화염 DoT | status damage | 술 | unreachable; effect unsupported | 상태 지속 resolver 필요 | 없음 |
| `ABILITY_ARTS_02` 냉기 장막 | 방어와 약화 | 피해 감소 + 적 ATK 약화 | mitigation/debuff | 술 | unreachable; effect unsupported | 적 debuff duration 계약 필요 | 없음 |
| `ABILITY_ARTS_03` 번개 방출 | 명시 skill damage | skill 직접 피해 + cooldown | skill burst | 술 | unreachable; effect unsupported | SCOUT 독점 깨는 1차 skill 후보 | 없음 |
| `ABILITY_LINE_01` 정밀 조준 | 공격+패턴 옵션 | ATK 보너스 + 적 턴 옵션 해금 | pattern access | 선 | unreachable; ATK portion expressible; option unlock unsupported | 패턴 choice extension 계약 필요 | 없음 |
| `ABILITY_LINE_02` 꿰뚫는 시선 | 패링 보상 | 패링 타이머 + 반격 배율 | pattern counter | 선 | unreachable; effect unsupported | 패링 결과 계약 필요 | 없음 |
| `ABILITY_LINE_03` 마지막 화살 | 위기 공격 | HP threshold attack burst | low-HP finisher | 선 | unreachable; effect unsupported | HP threshold attack resolver 필요 | 없음 |
| `ABILITY_GUARD_01` 철벽의 태세 | 방어 기반 | 방어 피해 감소 | mitigation | 결 | unreachable; ability defense unsupported | item defend reduce만 현재 적용 | 없음 |
| `ABILITY_GUARD_02` 가시 갑옷 | 방어 반격 | 다음 적 공격 반격 | guard counter | 결 | unreachable; effect unsupported | 반격 대기 상태 필요 | 없음 |
| `ABILITY_GUARD_03` 불굴의 의지 | 죽음 보험 | 런당 1회 생존 | death insurance | 결 | unreachable; effect unsupported | Recall Anchor hard-code와 분리 필요 | 없음 |

### A2. Normal Items 12

| id/name | intended role | effect summary | build axis | dependency/tag | current status | implementation note | GDD impact |
|---|---|---|---|---|---|---|---|
| `ITEM_01` 붕대 뭉치 | 생존 기반 | Max HP bonus | max HP | normal | unreachable; effect applied if granted | asset exists, catalog/pool 필요 | 없음 |
| `ITEM_02` 작은 룬석 | 공격 기반 | ATK bonus | flat attack | normal | unreachable; effect applied if granted | asset exists, catalog/pool 필요 | 없음 |
| `ITEM_03` 날카로운 숫돌 | 공격 hit 보조 | 공격 고정 피해 | on-attack payload | normal | unreachable; effect applied if granted | asset exists, catalog/pool 필요 | 없음 |
| `ITEM_04` 낡은 방패 조각 | 방어 보조 | 방어 피해 감소 | mitigation | normal | unreachable; effect applied if granted | asset exists, catalog/pool 필요 | 없음 |
| `ITEM_05` 독침 | 라운드 압박 | 라운드 적 HP 감소 | passive DoT | normal | unreachable; effect applied if granted | asset exists, catalog/pool 필요 | 없음 |
| `ITEM_06` 행운의 동전 | 경제 | Gold drop bonus | economy | normal | unreachable; effect unsupported | item economy passive resolver 필요 | 없음 |
| `ITEM_07` 얼어붙은 심장 | 패턴 대응 예약 | 적 패턴 대응 보조 | pattern response | OQ-019 | unreachable; asset is superseded Glitch placeholder; effect unsupported | 1차 runtime blocker 아님 | D-030 반영 |
| `ITEM_08` 탑의 잔재 | 승리 경제 | combat victory Gold | economy | normal | unreachable; effect unsupported | victory item passive resolver 필요 | 없음 |
| `ITEM_09` 마모된 부적 | 초반 피격 안정 | 첫 피해 감소 | first-hit mitigation | normal | unreachable; effect applied if granted | asset exists, catalog/pool 필요 | 없음 |
| `ITEM_10` 피의 계약서 | stat tradeoff | Max HP penalty + ATK bonus | risk stat trade | normal | unreachable; effect applied if granted | SWORD_03와 HP 리스크 표현 구분 필요 | 없음 |
| `ITEM_11` 녹슨 나침반 | 관계 관리 | encounter Affinity bonus | relationship utility | normal | unreachable; effect unsupported | 전투 modifier 아님; item encounter hook 필요 | 없음 |
| `ITEM_12` 탑의 이슬 | 전투 후 회복 | victory HP restore | sustain | normal | unreachable; intended effect unsupported/mismatched | current `hp_restore` path reads combat-start, trigger mismatch 수정 필요 | 없음 |

### A3. Relics 6

| id/name | intended role | effect summary | build axis | dependency/tag | current status | implementation note | GDD impact |
|---|---|---|---|---|---|---|---|
| `RELIC_SWORD_01` 피묻은 칼날 | 검 snowball | 검 능력 발동 ATK 누적 | tag trigger growth | 검 | unreachable; effect unsupported | ability tag trigger/passive state 필요 | 없음 |
| `RELIC_ARTS_01` 원소 수정 | 술 duration | 상태 지속 연장 | status duration | 술 | unreachable; effect unsupported | status resolver 선행 | 없음 |
| `RELIC_LINE_01` 저격수의 망원경 | 보스 압박 | 현재 HP 비례 추가 피해 | percent damage | 선 | unreachable; effect unsupported | percent damage 적용 계약 필요 | 없음 |
| `RELIC_GUARD_01` 강화 방패 | 결 snowball | 방어 시 방어 누적 | guard growth | 결 | unreachable; effect unsupported | D-030 `반사`와 별도 누적 방어 축 | 없음 |
| `RELIC_GENERIC_01` 회귀자의 낡은 코트 | 범용 sustain | 전투 시작 HP restore | combat-start heal | 범용 | unreachable; effect applied if granted | asset exists, relic pool/slot 필요 | 없음 |
| `RELIC_GENERIC_02` 마타이오스의 메모 | 범용 경제 | Gold drop, relationship economy branch | economy | 범용 | unreachable; effect unsupported | Affinity는 경제 branch만; 전투 modifier 금지 유지 | D-029 점검 |

### A4. Synergies 4

| id/name | intended role | effect summary | build axis | dependency/tag | current status | implementation note | GDD impact |
|---|---|---|---|---|---|---|---|
| `광폭` | 검 완성 보상 | Attack 추가타 ATK×0.5, 직전 Attack이면 ATK×0.75; 방어/스킬 break | attack chain | 검 x3; D-031; OQ-020 | unreachable; placeholder mismatch; effect unsupported | 무한 배율 증폭 금지; chain state 필요 | D-031 반영 |
| `연소` | 술 완성 보상 | 상태 지속/상태 분기 | status synergy | 술 x3 | unreachable; placeholder mismatch; effect unsupported | status system 필요 | 없음 |
| `관통` | 선 완성 보상 | 패링 반격/HP 분기 | pattern counter | 선 x3 | unreachable; placeholder mismatch; effect unsupported | parry/pattern contract 필요 | 없음 |
| `반사` | 결 완성 보상 | Defend 받은 피해 50% 반사 + 다음 Attack 1회 ×1.5 | defend-to-counter | 결 x3; D-031; OQ-020 | unreachable; placeholder mismatch; effect unsupported | 검 다타수와 분리; 1회 소비 counter window 필요 | D-031 반영 |

## B. Runtime Support Matrix

| effect type | data asset 표현 | runtime 적용 | UI feedback | 현재 검증 |
|---|---|---|---|---|
| ATK modifier | yes: NumericParam on ability/item/synergy | partial: ability/synergy `player.attack_bonus`, item `atk_bonus` | partial: combat/state copy reflects attack context | yes for current numeric stat paths |
| Max HP modifier | yes | partial: ability/synergy `player.max_hp_bonus`, item `max_hp_bonus`/`max_hp_penalty` | partial: HP snapshot/card | yes for current numeric stat paths |
| combat-start heal | yes: item `hp_restore` | yes for item modifier path; trigger semantics are broad | partial: combat start/log context | yes for Field Bandage path |
| Gold/economy modifier | yes in ItemData and encounter/enemy data | partial: encounter/enemy Gold works; item `gold_bonus` passives not applied | reward copy exists for encounter/combat rewards | yes only for encounter/enemy rewards |
| defense/mitigation modifier | yes for item keys | partial: defend reduce and first-hit reduce items; ability DEF design unsupported | combat round result has item guard context | yes for item mitigation only |
| skill/action modifier | generic ability data can hold params | no general resolver; Skill gated by `ABILITY_SCOUT` hard-code | yes for current SCOUT skill button/result | only SCOUT hard-coded path |
| revive/death insurance | data can hold placeholder keys | partial hard-code via `ABILITY_RECALL_ANCHOR`; GDD Guard revive unsupported | recall status/result feedback exists | yes for Recall Anchor only |
| affinity/glitch/memory interaction | yes in encounter/enemy/reward data | yes in encounter/run state; build combat modifier forbidden by D-029 | run/encounter result feedback exists | yes outside combat build modifier |
| synergy/relic tag interaction | SynergyData tag/count and relic trigger strings exist | partial: synergy detector/stat keys only; relic tag passive unsupported | activation event path exists, deep effect feedback absent | only placeholder/stat detection paths |

## C. Gap List

### C1. Asset exists but not in runtime pool

- GDD normal item assets `ITEM_01` through `ITEM_12`.
- Relic assets `RELIC_SWORD_01`, `RELIC_ARTS_01`, `RELIC_LINE_01`, `RELIC_GUARD_01`, `RELIC_GENERIC_01`, `RELIC_GENERIC_02`.
- Formal GDD ability assets are absent; prototype placeholder abilities exist but are not the target design surface.

### C2. In pool but effectless or dead reward

- `ITEM_LANTERN_OIL`: reachable catalog item with empty passive/effect data.
- `ITEM_TORN_CHARM`: reachable catalog item with empty passive/effect data.
- `ITEM_FIELD_BANDAGE` is the only current catalog item with a meaningful applied combat passive.

### C3. Runtime unsupported effects

- Ability triggers beyond passive stat aggregation: extra hit cadence, HP-cost attack, status application, cooldown skill damage, enemy choice unlock, parry, low-HP attack threshold, ability-based defense, thorns, revive.
- Synergy combat effects beyond tag detection and stat NumericParams.
- Relic passive triggers beyond item stat keys currently read by `CombatAbilityModifiers`.
- Item economy/victory/encounter hooks for `ITEM_06`, `ITEM_08`, `ITEM_11`, `ITEM_12` intended semantics.
- Enemy pattern result contract needed by `ITEM_07`.

### C4. GDD/runtime mismatch

- Runtime reachable abilities are `ABILITY_SCOUT` and `ABILITY_RECALL_ANCHOR`, not GDD 12 abilities.
- Runtime reachable items are `ITEM_FIELD_BANDAGE`, `ITEM_LANTERN_OIL`, `ITEM_TORN_CHARM`, not GDD 12 normal items/relic shop surface.
- GDD same-tag synergy trigger is x3; prototype placeholder synergies use x2.
- D-029 removed Glitch/Affinity combat modifier directions while superseded ItemData still contains `ITEM_07` `glitch_increased`/`atk_per_glitch`.
- `ITEM_12` design is combat-victory heal but current `hp_restore` path is read as combat-start heal.
- Current Skill surface is SCOUT-specific rather than "owned ability skill choice" from D-022.

### C5. User/PM decision still required

- OQ-019 pattern result contract and concrete effect for `ITEM_07`.
- Balance numbers after a playable surface is actually reachable; do not tune against current 2-ability/3-item surface.

## D. First Implementation Batch Draft

### D1. Batch objective

Balance tuning is not the objective. The batch objective is to make build choices appear in-run, apply deterministically, and compare across at least attack, defense, skill/status, and synergy axes without dead pick pool pollution.

### D2. Implementable now

- Remove dead-pick contamination from any reachable reward/shop pool before expanding it: do not surface `ITEM_LANTERN_OIL`, `ITEM_TORN_CHARM`, superseded `ITEM_07`, or placeholder ability/synergy content as build choices.
- Put a controlled minimum GDD pool in-run using content whose behavior is already supported or whose implementation contract is direct:
  - Normal items with existing applied resolver paths: `ITEM_01`, `ITEM_02`, `ITEM_03`, `ITEM_04`, `ITEM_05`, `ITEM_09`, `ITEM_10`.
  - `RELIC_GENERIC_01` only if relic slot reachability is included in the batch.
- Add formal ability content and resolver support required to break SCOUT skill monopoly with at least:
  - one attack axis ability from 검,
  - one explicit skill axis ability such as `ABILITY_ARTS_03`,
  - one defense axis ability from 결.
- Wire target synergy detection to GDD same-tag x3 and target synergy assets; keep unsupported deep logic out of pool until its effect applies.

### D3. Implement after design/contract

- OQ-019: `ITEM_07` pattern result contract and concrete passive.
- Status/parry/pattern option contracts for arts and line effects that need enemy pattern/system semantics.
- Item economy and victory hooks for `ITEM_06`, `ITEM_08`, `ITEM_11`, `ITEM_12`.
- Relic tag trigger passives and non-stat relic effects.

### D4. Hold until Balance Pass surface exists

- Exact numeric tuning for HP cost/reward, chain thresholds, counter windows, status magnitudes, economy values.
- Pool weights, reroll pressure, shop price tuning beyond existing locked economy decisions.
- Full 12/12/6/4 simultaneous runtime exposure if unsupported entries would reintroduce dead picks.

## Exit Criteria Before Balance Pass

- A deliberate build pool is reachable in run and contains no known effectless pick.
- Skill choice is not SCOUT-exclusive.
- At least one applied attack axis, defense axis, skill/status axis, and synergy completion path can be compared in one run family.
- Runtime support matrix rows used by the exposed pool are verifiable.
- D-029 remains intact: 붕괴도/Affinity/NPC state do not drive combat modifiers.
