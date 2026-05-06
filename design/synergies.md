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

**기본 효과**: 공격 시 매번 추가 공격 1회 (ATK × 0.5).  
**심화 로직 (Glitch 연동, P1)**: 붕괴할수록 강해지는 역설.

| 조건 | 추가 공격 배율 | 부작용 |
|------|--------------|--------|
| Glitch 0~2 | × 0.5 | 없음 |
| Glitch 3~5 | × 0.75 | 없음 |
| Glitch 6+ | × 1.0 | 전투 시작 시 HP -2 |

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.attack_bonus` | 0 | ✅ 확정 (기본 ATK 보너스 없음, 추가타 로직으로 처리) |
| `synergy.extra_atk_multiplier_low` | 0.5 | 🟡 (임시) — Glitch 0~2 |
| `synergy.extra_atk_multiplier_mid` | 0.75 | 🟡 (임시) — Glitch 3~5 |
| `synergy.extra_atk_multiplier_high` | 1.0 | 🟡 (임시) — Glitch 6+ |
| `glitch_threshold_mid` | 3 | 🟡 (임시) |
| `glitch_threshold_high` | 6 | 🟡 (임시) |
| `player.hp_cost_on_combat_start` | 2 | 🟡 (임시) — Glitch 6+ 조건에서만 |

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

**기본 효과**: 방어 선택 시 받은 피해(GUARD_01 감소 후)의 50% 반사.  
**심화 로직 (Affinity 연동, P1·P5)**: 마타이오스와의 관계가 전투 수치에 직결.

| 조건 | 효과 |
|------|------|
| Affinity ≥ +3 | 반사 피해 +2 + 마타이오스 발화 1줄 트리거 |
| Affinity ≤ -3 | 반사 -1 대신 GUARD_03 쿨타임 리셋 |

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `synergy.reflect_ratio` | 0.5 | 🟡 (임시) — 기본 반사 비율 |
| `synergy.reflect_bonus_high_affinity` | 2 | 🟡 (임시) — Affinity ≥ +3 |
| `synergy.reflect_penalty_low_affinity` | -1 | 🟡 (임시) — Affinity ≤ -3 |
| `affinity_threshold_high` | 3 | 🟡 (임시) |
| `affinity_threshold_low` | -3 | 🟡 (임시) |
| `synergy.resets_guard03_cooldown` | 1 | 🟡 (임시) — 1 = true |

---

## 미결 사항

- 시너지 심화 로직 전체: Glitch/Affinity/방어 횟수/HP 비율을 전투 시스템이 추적해야 함 — Codex 구현 시 키 네이밍 확정 후 본 문서 갱신
- `SynergyDetector.cs` 현재 구조 확인 필요 — 심화 조건 분기가 `SynergyData` NumericParams만으로 처리되는지, 별도 로직 클래스가 필요한지
