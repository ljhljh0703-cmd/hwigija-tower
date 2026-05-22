# 아이템 / 유물 명세 (design/items.md)

> **SSOT 참조**: GDD §7.7 (OQ-016 ✅ 확정 2026-05-05, D-023)
> **Codex 용도**: `ItemData ScriptableObject` 생성 소스
> **이름 상태**: LLM 임시명. 작가가 TONE BIBLE 기준으로 최종 확정.
> **수치 상태**: 🟡 임시 수치 포함 — Codex 구현 후 플레이테스트로 확정.

---

## 시스템 구조

- 소모품 없음 — 모든 아이템은 획득 즉시 패시브 보유 효과 적용
- `ItemData SO` 공통 타입, `tier` 필드로 등급 구분
  - `normal`: 일반 아이템 (상점 기본 슬롯)
  - `relic`: 유물 (TRAIT_SUPPORT_04 해금 시 상점 유물 슬롯 추가, 30G)
- 복합 행동(TRAIT_OFFENSE_04) 2번째 행동으로 "아이템 사용" 허용 (per D-023)

---

## 유물 6개 (`tier: relic`)

### RELIC_SWORD_01
- **임시 이름**: 피묻은 칼날
- **연동 태그**: 검
- **효과**: 검 능력 발동 시마다 ATK +1 누적 (상한 없음)
- **NumericParams**:
  - `trigger`: `ability_tag_sword_activated` 🟡
  - `atk_per_trigger`: `1` 🟡
  - `atk_max`: `-1` (무제한) 🟡
- **연출 의도**: 검 빌드 스노우볼. 시너지 광폭과 조합 시 ATK 폭발

---

### RELIC_ARTS_01
- **임시 이름**: 원소 수정
- **연동 태그**: 술
- **효과**: 화염/냉기 상태이상 지속 +1라운드 추가
- **NumericParams**:
  - `trigger`: `status_fire_applied | status_cold_applied` 🟡
  - `duration_bonus`: `1` 🟡
- **연출 의도**: 술 빌드 지속딜 강화. 시너지 연소와 중첩 시 극대화

---

### RELIC_LINE_01
- **임시 이름**: 저격수의 망원경
- **연동 태그**: 선
- **효과**: 공격 시 적 현재 HP의 10% 추가 피해
- **NumericParams**:
  - `trigger`: `player_attack` 🟡
  - `hp_percent_damage`: `0.1` 🟡
- **연출 의도**: 고체력 적(보스)에게 유효. 선 빌드 후반 딜 보조

---

### RELIC_GUARD_01
- **임시 이름**: 강화 방패
- **연동 태그**: 결
- **효과**: 방어 선택 시 받는 피해 -2 영구 누적 (상한 없음)
- **NumericParams**:
  - `trigger`: `player_defend` 🟡
  - `def_per_trigger`: `2` 🟡
  - `def_max`: `-1` (무제한) 🟡
- **연출 의도**: 결 빌드 방어 스노우볼. 시너지 반사와 조합 시 반사 피해도 증폭

---

### RELIC_GENERIC_01
- **임시 이름**: 회귀자의 낡은 코트
- **연동 태그**: 범용
- **효과**: 매 전투 시작 시 HP +3
- **NumericParams**:
  - `trigger`: `combat_start` 🟡
  - `hp_restore`: `3` 🟡
- **연출 의도**: 장기전 생존 보조. 어떤 빌드에도 유효한 범용 픽

---

### RELIC_GENERIC_02
- **임시 이름**: 마타이오스의 메모
- **연동 태그**: 범용
- **효과**: Gold 드롭 +5. Affinity ≥+3이면 +8로 증가
- **NumericParams**:
  - `trigger`: `enemy_defeated` 🟡
  - `gold_bonus_base`: `5` 🟡
  - `gold_bonus_affinity`: `8` 🟡
  - `affinity_threshold`: `3` 🟡
- **연출 의도**: P5 즉각 반응. 마타이오스와의 유대가 경제적 이득으로 직결 (P1)

---

## 일반 아이템 12개 (`tier: normal`)

### ITEM_01
- **임시 이름**: 붕대 뭉치
- **효과**: 최대 HP +4
- **NumericParams**:
  - `max_hp_bonus`: `4` 🟡

---

### ITEM_02
- **임시 이름**: 작은 룬석
- **효과**: ATK +1 영구
- **NumericParams**:
  - `atk_bonus`: `1` 🟡

---

### ITEM_03
- **임시 이름**: 날카로운 숫돌
- **효과**: 공격 시 추가 1 고정 피해
- **NumericParams**:
  - `trigger`: `player_attack` 🟡
  - `flat_damage_bonus`: `1` 🟡

---

### ITEM_04
- **임시 이름**: 낡은 방패 조각
- **효과**: 방어 선택 시 받는 피해 -1
- **NumericParams**:
  - `trigger`: `player_defend` 🟡
  - `damage_reduce`: `1` 🟡

---

### ITEM_05
- **임시 이름**: 독침
- **효과**: 전투 중 매 라운드 시작 시 적 HP -1 (독 상태, 전투 내내)
- **NumericParams**:
  - `trigger`: `round_start` 🟡
  - `poison_damage_per_round`: `1` 🟡

---

### ITEM_06
- **임시 이름**: 행운의 동전
- **효과**: Gold 드롭 +3
- **NumericParams**:
  - `trigger`: `enemy_defeated` 🟡
  - `gold_bonus`: `3` 🟡

---

### ITEM_07
- **임시 이름**: 얼어붙은 심장
- **효과**: 적 패턴 대응 보조 축으로 예약. 기존 Glitch 증가당 ATK 보너스는 D-029로 폐기 유지. 구체 효과와 resolver 입력 계약은 OQ-019.
- **NumericParams**:
  - `trigger`: OQ-019 ❓
  - `pattern_result_params`: OQ-019 ❓
- **서사 의도**: 일반 아이템 표면에서 flat ATK/HP/Gold 대신 적 패턴 대응 보조 축 확보

---

### ITEM_08
- **임시 이름**: 탑의 잔재
- **효과**: 전투 승리 시 Gold +2
- **NumericParams**:
  - `trigger`: `combat_victory` 🟡
  - `gold_bonus`: `2` 🟡

---

### ITEM_09
- **임시 이름**: 마모된 부적
- **효과**: 매 전투 첫 번째 피해 -3 (1회, 매 전투 리셋)
- **NumericParams**:
  - `trigger`: `first_hit_per_combat` 🟡
  - `damage_reduce`: `3` 🟡

---

### ITEM_10
- **임시 이름**: 피의 계약서
- **효과**: 최대 HP -5, ATK +3 영구
- **NumericParams**:
  - `max_hp_penalty`: `-5` 🟡
  - `atk_bonus`: `3` 🟡
- **서사 의도**: 리스크-리워드 교환. P1(얻으려면 잃어야) 직접 구현

---

### ITEM_11
- **임시 이름**: 녹슨 나침반
- **효과**: 인카운터 노드 진입 시 Affinity +1
- **NumericParams**:
  - `trigger`: `encounter_node_entered` 🟡
  - `affinity_bonus`: `1` 🟡
- **서사 의도**: Affinity 관리 수단. P5 즉각 가시화

---

### ITEM_12
- **임시 이름**: 탑의 이슬
- **효과**: 전투 승리 시 HP +1 추가 회복
- **NumericParams**:
  - `trigger`: `combat_victory` 🟡
  - `hp_restore`: `1` 🟡

---

## NumericParam 키 상태 요약

| 키 | 사용처 | 상태 |
|----|-------|------|
| `atk_bonus` | ITEM_02, ITEM_10 | 🟡 임시 (abilities.md 키와 통합 필요) |
| `max_hp_bonus` | ITEM_01 | 🟡 임시 |
| `max_hp_penalty` | ITEM_10 | 🟡 임시 |
| `flat_damage_bonus` | ITEM_03 | 🟡 임시 |
| `damage_reduce` | ITEM_04, ITEM_09 | 🟡 임시 |
| `poison_damage_per_round` | ITEM_05 | 🟡 임시 |
| `gold_bonus` | ITEM_06, ITEM_08 | 🟡 임시 |
| `atk_per_glitch` | ITEM_07 | ⛔ D-029 이후 폐기. OQ-019 패턴 대응 계약으로 대체 필요 |
| `hp_restore` | ITEM_12, RELIC_GENERIC_01 | 🟡 임시 |
| `atk_per_trigger` | RELIC_SWORD_01 | 🟡 임시 |
| `def_per_trigger` | RELIC_GUARD_01 | 🟡 임시 |
| `duration_bonus` | RELIC_ARTS_01 | 🟡 임시 |
| `hp_percent_damage` | RELIC_LINE_01 | 🟡 임시 |
| `gold_bonus_base` / `gold_bonus_affinity` | RELIC_GENERIC_02 | 🟡 임시 |
| `affinity_bonus` | ITEM_11 | 🟡 임시 |

> 모든 trigger 문자열 및 NumericParam 키는 Codex `ItemData.cs` 구현 시 확정.
