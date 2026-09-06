# 시너지 명세 (Synergy Spec) — Codex SO 생성 참조

> **GDD 근거**: §7.1 (v0.3.0)  
> **코드 근거**: `SynergyData.cs`, `SynergyDetector.cs`, `SynergyState.cs`, `CombatAbilityModifiers.cs`  
> **발동 조건**: 동일 태그 능력 3개 보유 시 자동 발동 (`requiredCount: 3`)

---

## NumericParam 키 상태 범례

- ✅ **확정** — `CombatAbilityModifiers.cs`에서 실제로 읽는 키
- 🟡 **(임시)** — 설계 확정, 코드 미구현. Codex 시너지 심화 로직 구현 시 확정

---

## SO 공통 필드 (`SynergyData.cs`)

| 필드 | 값 | 비고 |
|------|----|------|
| `tag` | 해당 태그 문자열 | 검 / 술 / 선 / 결 |
| `requiredCount` | 3 | 전 시너지 동일 |

---

## 광폭 (Frenzy) — 검×3

**기본 효과**: Attack 시 추가타 ATK×0.5.  
**심화 로직**: 직전 플레이어 액션도 Attack이면 추가타 ATK×0.75. Defend/Skill 선택 시 공격 연쇄 초기화. 기존 Glitch 배율 분기와 HP 부작용은 D-029로 폐기 유지하며, 단순 무한 배율 증폭은 금지. 수치는 OQ-020 1차 플레이테스트 기준값.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.attack_bonus` | 0 | ✅ 확정 (기본 ATK 보너스 없음, 추가타 로직으로 처리) |
| `synergy.extra_atk_multiplier` | 0.5 | 🟡 OQ-020 1차 기준값 |
| `synergy.attack_chain_break_actions` | `defend`, `skill` | 🔒 D-031 effect contract |
| `synergy.attack_chain_extra_atk_multiplier` | 0.75 | 🟡 OQ-020 1차 기준값 |

---

## 연소 (Combustion) — 술×3

**기본 효과**: 화염 지속 +1라운드, 냉기 ATK 감소 -3 → -5 강화.  
**심화 로직 (플레이스타일 연동)**: 이번 런 방어 선택 횟수 기반 분기.

| 조건 | 효과 |
|------|------|
| 방어 횟수 ≤ 3 | 화염 DoT 피해 2 → 3 |
| 방어 횟수 ≥ 7 | 냉기 중첩 가능 (최대 2스택) |

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `synergy.dot_duration_bonus` | 1 | 🟡 (임시) — 화염 지속 +1라운드 |
| `synergy.cold_atk_debuff` | 5 | 🟡 (임시) — 냉기 ATK 감소 강화값 |
| `synergy.dot_damage_bonus` | 1 | 🟡 (임시) — 방어 횟수 ≤3 조건 (2→3) |
| `defend_count_threshold_low` | 3 | 🟡 (임시) |
| `defend_count_threshold_high` | 7 | 🟡 (임시) |
| `synergy.cold_max_stacks` | 2 | 🟡 (임시) — 방어 횟수 ≥7 조건 |

---

## 관통 (Penetration) — 선×3

**기본 효과**: 패링 성공 시 반격 ATK × 2.5, 패링 타이머 총 +2초.  
**심화 로직 (HP 비율 연동)**: 위기 상황이 정밀함을 끌어낸다.

| 조건 | 효과 |
|------|------|
| HP ≥ 50% | 공격 ATK +2 |
| HP < 50% | ABILITY_LINE_03 발동 조건 'HP ≤ 5' → 'HP < 50%' 완화 |

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `synergy.parry_atk_multiplier` | 2.5 | 🟡 (임시) |
| `synergy.parry_timer_bonus` | 2 | 🟡 (임시) — 초 단위 |
| `synergy.hp_high_attack_bonus` | 2 | 🟡 (임시) — HP ≥ 50% 조건 |
| `hp_ratio_threshold` | 0.5 | 🟡 (임시) |

---

## 반사 (Reflection) — 결×3

**기본 효과**: Defend 시 받은 피해의 50% 반사.  
**심화 로직**: 방어 후 다음 Attack 1회 피해×1.5. 반격 창은 1회 소비한다. 반격 보상은 검 다타수와 겹치지 않는 강한 반격 1회 방향이다. 기존 Affinity 전투 연동과 방어 누적 요새화안은 채택하지 않는다. 수치는 OQ-020 1차 플레이테스트 기준값.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `synergy.reflect_ratio` | 0.5 | 🟡 OQ-020 1차 기준값 |
| `synergy.counter_window_source` | `defend` | 🔒 D-031 effect contract |
| `synergy.counter_window_consumes_on` | `next_attack_once` | 🔒 D-031 effect contract |
| `synergy.counter_attack_multiplier` | 1.5 | 🟡 OQ-020 1차 기준값 |

---

## 미결 사항

- 시너지 심화 로직 전체: 붕괴도/Affinity는 D-029/D-030 이후 전투 조건으로 추적하지 않음. `광폭` 공격 연쇄와 `반사` 반격 창 effect contract는 D-031, 1차 기준값은 OQ-020
- `SynergyDetector.cs` 현재 구조 확인 필요 — 심화 조건 분기가 `SynergyData` NumericParams만으로 처리되는지, 별도 로직 클래스가 필요한지
