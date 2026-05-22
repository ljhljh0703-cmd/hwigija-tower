# 능력 명세 (Ability Spec) — Codex SO 생성 참조

> **GDD 근거**: §7.1 (v0.5.0)  
> **코드 근거**: `AbilityData.cs`, `CombatAbilityModifiers.cs`, `NumericParamLookup.cs`  
> **용도**: Unity SO `.asset` 생성 시 NumericParams 키/값 참조. 트리거·조건부 로직은 **(임시)** 표기 — 코드 구현 전 Codex와 키 네이밍 확정 필요.

---

## NumericParam 키 상태 범례

- ✅ **확정** — `CombatAbilityModifiers.cs`에서 실제로 `NumericParamLookup.Sum()` 호출 중
- 🟡 **(임시)** — 설계 확정, 코드 미구현. Codex가 시스템 구현 시 키 네이밍 확정 후 본 문서 갱신 필요

---

## 공통 필드

| 필드 | 설명 | 상태 |
|------|------|------|
| `cost_gold` | 상점 구매 가격 — 기본 20G, 동일 태그 3번째 30G | ✅ OQ-013 확정 |
| `trigger` | 발동 조건 (`on_attack` / `on_defend` / `on_skill` / `on_parry` / `on_hp_threshold` / `passive`) | 🟡 (임시) |

---

## 검 (Sword) 태그

### ABILITY_SWORD_01 | 검 | 예리한 감각

**효과**: 공격 선택 시 ATK +3 영구. 장착 즉시 HUD ATK 갱신.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.attack_bonus` | 3 | ✅ 확정 |
| `trigger` | `on_attack` | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_SWORD_02 | 검 | 연속베기

**효과**: 공격 선택 시, N라운드마다 ATK×0.5 추가 공격 1회. 카운터는 전투 시작 시 리셋.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🟡 (임시) |
| `atk_multiplier` | 0.5 | 🟡 (임시) |
| `rounds_interval` | 3 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_SWORD_03 | 검 | 피의 서약

**효과**: Attack 선택 시 HP 3 비용을 지불하고 해당 공격 피해 +5. HP 비용은 그 비용만으로 플레이어가 자살하지 않게 적용한다. 기존 "Glitch ≥ 3에서 ATK +5 + HP -3" 전투 조건은 D-029로 폐기 유지. 수치는 OQ-020 1차 플레이테스트 기준값이며 최종 밸런스 잠금이 아니다.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🔒 D-031 effect contract |
| `attack_strike_bonus` | 5 | 🟡 OQ-020 1차 기준값 |
| `player.hp_cost_nonlethal` | 3 | 🟡 OQ-020 1차 기준값 |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

## 술 (Arts) 태그

### ABILITY_ARTS_01 | 술 | 화염 인장

**효과**: 공격 선택 시 화염 상태 부여. 화염: 이후 2라운드, 라운드 시작 시 적 HP -2 고정.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🟡 (임시) |
| `enemy.dot_damage` | 2 | 🟡 (임시) |
| `enemy.dot_duration` | 2 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_ARTS_02 | 술 | 냉기 장막

**효과**: 방어 선택 시 다음 적 공격 피해 -4. 냉기 상태: 적 다음 턴 ATK -2 (1라운드).

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_defend` | 🟡 (임시) |
| `player.damage_reduction` | 4 | 🟡 (임시) |
| `enemy.attack_debuff` | 2 | 🟡 (임시) |
| `enemy.debuff_duration` | 1 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_ARTS_03 | 술 | 번개 방출

**효과**: 스킬 슬롯 사용 시 적 HP -8 직접 피해. 쿨타임 4라운드.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_skill` | 🟡 (임시) |
| `skill.direct_damage` | 8 | 🟡 (임시) |
| `skill.cooldown_rounds` | 4 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

## 선 (Line) 태그

### ABILITY_LINE_01 | 선 | 정밀 조준

**효과**: 공격 선택 시 ATK +2 영구. 추가: 적 턴 선택지에 '회피' 4번째 옵션 해금.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🟡 (임시) |
| `player.attack_bonus` | 2 | ✅ 확정 |
| `unlocks_evasion_option` | 1 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_LINE_02 | 선 | 꿰뚫는 시선

**효과**: 패링 성공 시 타이머 +1초, 반격 피해 ATK × 1.5.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_parry` | 🟡 (임시) |
| `atk_multiplier` | 1.5 | 🟡 (임시) |
| `timer_bonus_seconds` | 1 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_LINE_03 | 선 | 마지막 화살

**효과**: HP ≤ 5 상태에서 공격 선택 시 ATK × 2 (해당 공격만). 조건 유지 시 매 공격 발동.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🟡 (임시) |
| `hp_threshold` | 5 | 🟡 (임시) |
| `atk_multiplier` | 2.0 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

## 결 (Guard) 태그

### ABILITY_GUARD_01 | 결 | 철벽의 태세

**효과**: 방어 선택 시 받는 피해 -3 영구. 장착 즉시 HUD DEF 수치 표시 시작.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_defend` | 🟡 (임시) |
| `player.damage_reduction` | 3 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_GUARD_02 | 결 | 가시 갑옷

**효과**: 방어 선택 시, 적이 다음 턴 공격 시 적 HP -3 반격. HUD '반격 대기 중' 아이콘 표시, 1라운드 유지 후 소멸.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_defend` | 🟡 (임시) |
| `player.thorns_damage` | 3 | 🟡 (임시) |
| `thorns_duration` | 1 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

### ABILITY_GUARD_03 | 결 | 불굴의 의지

**효과**: HP가 처음으로 0에 도달하는 순간 (런당 1회) HP를 1로 고정. 이후 HP 1 상태로 전투 지속.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_hp_threshold` | 🟡 (임시) |
| `hp_threshold` | 0 | 🟡 (임시) |
| `survive_hp` | 1 | 🟡 (임시) |
| `uses_per_run` | 1 | 🟡 (임시) |
| `cost_gold` | 20 | ✅ OQ-013 확정 |

---

## 미결 사항 (OQ 연동)

- `cost_gold` 가격: 기본 20G / 동일 태그 3번째 30G ✅ OQ-013 확정 (이전 임시 표기 해소)
- `unlocks_evasion_option` 키: 적 턴 선택지 4번째 옵션 해금 — 코드 구조 확정 필요 (OQ-014 연동)
- `trigger` 키 전체: Codex가 조건부 전투 시스템 구현 시 키 네이밍 확정 및 본 문서 갱신
