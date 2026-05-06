# 특성 명세 (Trait Spec) — Codex SO 생성 참조

> **GDD 근거**: §7.1 (v0.4.0), OQ-011 확정 (2026-05-05)
> **코드 근거**: `TraitData.cs` (미구현 — Codex 신규 생성 필요)
> **용도**: Unity SO `.asset` 생성 시 참조. 모든 특성은 패시브 자동 발동.

---

## 시스템 규칙

- **해금 방식**: 메타 영구 해금 — 승리 횟수(전 런 누적) 임계값 도달 시 해금
- **해금 단계**:
  | 단계 | 승리 횟수 임계값 | 해금 특성 |
  |------|----------------|---------|
  | Lv1 | 3회 | 생존 1·2 / 공격 1·2 / 지원 1·2 (6종) |
  | Lv2 | 6회 | 생존 3 / 공격 3 / 지원 3 (3종) |
  | Lv3 | 10회 | 생존 4 / 공격 4 / 지원 4 (3종) |
- **적용 방식**: 해금 후 런 시작 시 자동 적용 (선택·장착 불필요)
- **태그**: 생존(Survival) / 공격(Offense) / 지원(Support)

---

## NumericParam 키 상태 범례

- 🟡 **(임시)** — 전부 미구현. `TraitData.cs` SO 구현 시 키 네이밍 확정 후 갱신 필요

---

## 생존 (Survival) 태그

### TRAIT_SURVIVAL_01 | 생존 | 강건한 체질
**효과**: 전투 시작 시 최대 HP +3 영구 (런 누적).

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.max_hp_bonus` | 3 | 🟡 (임시) — 능력과 동일 키 공유 예정 |
| `trigger` | `passive_run_start` | 🟡 (임시) |
| `unlock_level` | 1 | 🟡 (임시) |

---

### TRAIT_SURVIVAL_02 | 생존 | 철벽 자세
**효과**: 방어 선택 시 피해 -2 추가 감소 (ABILITY_GUARD_01과 중첩 적용).

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.damage_reduction` | 2 | 🟡 (임시) |
| `trigger` | `on_defend` | 🟡 (임시) |
| `unlock_level` | 1 | 🟡 (임시) |

---

### TRAIT_SURVIVAL_03 | 생존 | 재생의 흐름
**효과**: 매 라운드 시작 시 HP +1 회복.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.regen_per_round` | 1 | 🟡 (임시) |
| `trigger` | `on_round_start` | 🟡 (임시) |
| `unlock_level` | 2 | 🟡 (임시) |

---

### TRAIT_SURVIVAL_04 | 생존 | 불사신
**효과**: 런 중 처음으로 HP가 0에 도달하는 순간 HP 1로 부활. 런당 1회.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_hp_threshold` | 🟡 (임시) |
| `hp_threshold` | 0 | 🟡 (임시) |
| `survive_hp` | 1 | 🟡 (임시) |
| `uses_per_run` | 1 | 🟡 (임시) |
| `unlock_level` | 3 | 🟡 (임시) |

> **주의**: ABILITY_GUARD_03(불굴의 의지)와 효과 동일 — 중첩 시 동작 정의 필요 (1회씩 2회 발동 vs 1회만). Codex 구현 시 결정 필요.

---

## 공격 (Offense) 태그

### TRAIT_OFFENSE_01 | 공격 | 예리함
**효과**: 전투 시작 시 ATK +2 영구 (런 누적).

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `player.attack_bonus` | 2 | ✅ 확정 키 공유 (적용 타이밍 로직은 임시) |
| `trigger` | `passive_run_start` | 🟡 (임시) |
| `unlock_level` | 1 | 🟡 (임시) |

---

### TRAIT_OFFENSE_02 | 공격 | 치명적 감각
**효과**: 공격 시 15% 확률로 치명타 (ATK × 1.5). 시드 RNG 필수 (per GDD P4).

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🟡 (임시) |
| `crit_chance` | 0.15 | 🟡 (임시) |
| `crit_multiplier` | 1.5 | 🟡 (임시) |
| `unlock_level` | 1 | 🟡 (임시) |

---

### TRAIT_OFFENSE_03 | 공격 | 연타 본능
**효과**: 공격 선택 시 30% 확률로 ATK × 0.5 추가 타격 1회. 시드 RNG 필수.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_attack` | 🟡 (임시) |
| `extra_hit_chance` | 0.30 | 🟡 (임시) |
| `atk_multiplier` | 0.5 | 🟡 (임시) |
| `unlock_level` | 2 | 🟡 (임시) |

---

### TRAIT_OFFENSE_04 | 공격 | 복합 행동
**효과**: 플레이어 턴에 선택지 2개 순차 선택 가능. 첫 번째 선택의 ATK 계수 × 0.7.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `passive_turn_start` | 🟡 (임시) |
| `extra_action_count` | 1 | 🟡 (임시) |
| `first_action_atk_multiplier` | 0.7 | 🟡 (임시) |
| `unlock_level` | 3 | 🟡 (임시) |

> **주의**: D-022 전투 구조 확장 필요 — OQ-015에서 세부 확정 대기. Codex 구현 전 OQ-015 close 필수.

---

## 지원 (Support) 태그

### TRAIT_SUPPORT_01 | 지원 | 전투 후 숨 고르기
**효과**: 전투 승리 후 HP +2 회복.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_combat_victory` | 🟡 (임시) |
| `player.heal_on_victory` | 2 | 🟡 (임시) |
| `unlock_level` | 1 | 🟡 (임시) |

---

### TRAIT_SUPPORT_02 | 지원 | 탐욕의 손
**효과**: 전투 승리 시 Gold 드롭 +3 고정 추가.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `on_combat_victory` | 🟡 (임시) |
| `gold_bonus_flat` | 3 | 🟡 (임시) |
| `unlock_level` | 1 | 🟡 (임시) |

---

### TRAIT_SUPPORT_03 | 지원 | 흥정꾼
**효과**: 런당 상점 무료 리롤 1회 제공.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `passive_run_start` | 🟡 (임시) |
| `shop_free_reroll_count` | 1 | 🟡 (임시) |
| `unlock_level` | 2 | 🟡 (임시) |

> OQ-013(상점 Pool) 확정 시 리롤 UI 구현과 함께 처리.

---

### TRAIT_SUPPORT_04 | 지원 | 유물상 해금
**효과**: 상점에 유물 슬롯 1개 추가. 유물 = 강력 패시브 아이템, 가격 30G.

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `trigger` | `passive_run_start` | 🟡 (임시) |
| `shop_relic_slot_count` | 1 | 🟡 (임시) |
| `unlock_level` | 3 | 🟡 (임시) |

> **주의**: 유물 목록·효과는 OQ-016에서 별도 설계 필요. 구현 전 OQ-016 close 필수.

---

## TRAIT_GUARD_03 중첩 주의

`TRAIT_SURVIVAL_04`(불사신)와 `ABILITY_GUARD_03`(불굴의 의지) 모두 "HP 0 → HP 1 부활" 효과.  
동시 보유 시 동작 정의 — Codex 구현 시 결정 필요 (OQ 신규 등록 권고).

---

## 미결 사항

| OQ | 내용 |
|----|------|
| OQ-015 | 복합 행동 전투 구조 세부 (2택 순차 선택 UI, ATK 계수 적용 방식) |
| OQ-016 | 유물 아이템 목록 및 효과 (강력 패시브 설계) |
