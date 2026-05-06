# 밸런스 명세 (Balance Spec) — Codex SO 생성 참조

> **GDD 근거**: §7.0, §7.3, §7.5, §7.6 (v0.4.1)  
> **코드 근거**: `EnemyData.cs`, `CombatantState.cs`, `CombatController.cs`  
> **TTK 목표**: 일반 3~4라운드 / 정예 5~6라운드 / 보스 6~8라운드

---

## 노드 맵 구조 — OQ-012 ✅ 확정 2026-05-05

| 항목 | 값 | 상태 |
|------|-----|------|
| 총 층 수 | 5 | ✅ |
| 층당 경로 수 | 3 | ✅ |
| 노드 배치 비율 | 전투 40% / 인카운터 40% / 휴식 20% | ✅ |
| 상인 위치 | 보스 직전 고정 (경로 무관) | ✅ |
| 시각 표현 | 맵 화면 없음, 선택지 팝업 | ✅ |
| 레이아웃 | 시드 랜덤 (`run_id` 기반) | ✅ |

**Codex 구현 참조 키 (임시):**

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `map.floors` | 5 | 🟡 (임시) |
| `map.paths_per_floor` | 3 | 🟡 (임시) |
| `map.combat_ratio` | 0.4 | 🟡 (임시) |
| `map.encounter_ratio` | 0.4 | 🟡 (임시) |
| `map.rest_ratio` | 0.2 | 🟡 (임시) |

---

## NumericParam 키 상태 범례

- ✅ **확정** — `EnemyData.cs` 필드에 직접 매핑
- 🟡 **(임시)** — 설계 확정, 코드 미구현

---

## 플레이어 기초 스탯

| 스탯 | 초기값 | 비고 |
|------|--------|------|
| HP | 20 | `CombatantState` maxHp |
| ATK | 5 | `CombatantState` attack |
| DEF | 0 | 능력·특성으로만 상승 🟡 (임시) |

**1런 기대 성장 (능력 2~3개 기준):**

| 시나리오 | ATK | HP |
|---------|-----|----|
| 최소 (능력 2개) | 9~11 | 20~25 |
| 최대 (능력 3개 + 특성 2개) | 14~16 | 25~30 |

---

## 적 명세

> `EnemyData.cs` 필드: `id` (string), `hp` (int), `attack` (int), `patternId` (string)  
> 보상 필드(`gold`, `xp`, `glitch`, `affinity`)는 🟡 (임시) — EnemyData 확장 또는 별도 RewardData SO 필요

### 일반 적 4종

| SO 파일명 | id | HP | ATK | 층 분포 | Gold | XP | Glitch | Affinity | patternId |
|-----------|----|----|-----|---------|------|----|--------|----------|-----------|
| SO_Enemy_ENEMY_WALKER_01 | ENEMY_WALKER_01 | 12 | 4 | 1~2층 | +8 | +10 | 0 | 0 | pattern_basic 🟡 |
| SO_Enemy_ENEMY_CRAWLER_02 | ENEMY_CRAWLER_02 | 15 | 5 | 2~3층 | +10 | +12 | 0 | 0 | pattern_basic 🟡 |
| SO_Enemy_ENEMY_SHADE_03 | ENEMY_SHADE_03 | 18 | 6 | 3~4층 | +12 | +15 | +1 | -1 | pattern_glitch 🟡 |
| SO_Enemy_ENEMY_WRAITH_04 | ENEMY_WRAITH_04 | 22 | 7 | 4~5층 | +15 | +18 | +1 | -1 | pattern_glitch 🟡 |

### 정예 적 1종

| SO 파일명 | id | HP | ATK | 층 분포 | Gold | XP | Glitch | Affinity | patternId |
|-----------|----|----|-----|---------|------|----|--------|----------|-----------|
| SO_Enemy_ENEMY_HERALD_05 | ENEMY_HERALD_05 | 35 | 9 | 3층 1회 | +25 | +30 | +2 | -2 | pattern_elite 🟡 |

### 보스 2종

| SO 파일명 | id | HP | ATK | 층 | Gold | XP | Glitch | Affinity | 특수 트리거 |
|-----------|----|----|-----|----|------|----|--------|----------|------------|
| SO_Enemy_BOSS_GATE_01 | BOSS_GATE_01 | 50 | 10 | 5층 | +40 | +50 | +3 | +5 | 격파 = NPC 회차 기억 트리거 (D-011) 🟡 |
| SO_Enemy_BOSS_APEX_02 | BOSS_APEX_02 | 80 | 14 | 최종층 | +60 | +100 | 0 | 0 | 격파 = 안식 아이템 드롭 (D-013) 🟡 |

---

## 역산 검증 (P3 — 5분 호흡)

| 전투 | ATK | 적 HP | 예상 라운드 | P3 충족 |
|------|-----|-------|------------|---------|
| 능력 없음 vs WALKER | 5 | 12 | 3라운드 | ✅ |
| 능력 3개 vs BOSS_GATE | ~15 | 50 | 4라운드 | ✅ |
| 능력 3개 vs BOSS_APEX | ~15 | 80 | 방어/스킬 혼용 6~8라운드 | ✅ |

---

## Gold 경제 밸런스

### 파우셋 (1런 기대 수입)

| 수입원 | 기대 Gold |
|-------|-----------|
| 일반 전투 3회 × 평균 11 | 33 |
| 정예 전투 1회 | 25 |
| 도덕적 선택 보상 (선택 시) | 0~20 |
| 인카운터 보상 | 0~10 |
| **합계** | **58~88 (평균 약 70)** |

### 싱크 (지출처)

| 지출 | Gold |
|------|------|
| 일반 능력 구매 | 20 |
| 시너지 완성용 3번째 동일 태그 능력 | 30 |
| 도덕적 선택 기회비용 (보상 포기) | 15~25 실질 손실 |

**1런 Gold 70 기준:**
- 능력 3개 구매: 60G 소모, 잔여 10 (빠듯 — 긴장감)
- 능력 2개 구매: 40G 소모, 잔여 30 (인카운터 대비 여유)

**기회비용 설계 (P1·P5):** "Gold +25" vs "Affinity +3" — Gold를 택하면 마타이오스 호감도 하락 + Glitch 가속.

---

## 상점 Pool — OQ-013 ✅ 확정 2026-05-05

| 항목 | 값 | 상태 |
|------|-----|------|
| 1런당 표시 능력 수 | 3개 | ✅ |
| 기본 능력 가격 | 20G | ✅ |
| 3번째(동일 태그) 능력 가격 | 30G | ✅ |
| 리롤 가격 | 1회 10G → 이후 5G씩 증가 (10→15→20→…) | ✅ |
| 무료 리롤 | TRAIT_SUPPORT_03(흥정꾼) 해금 시 런당 1회 | ✅ |
| 유물 슬롯 | TRAIT_SUPPORT_04(유물상 해금) 해금 시 +1 슬롯 (30G) | ✅ (OQ-016 내용 확정 대기) |

**Codex 구현 참조 키 (임시):**

| NumericParam 키 | 값 | 상태 |
|----------------|-----|------|
| `shop.ability_pool_size` | 3 | 🟡 (임시) |
| `shop.ability_price_base` | 20 | 🟡 (임시) |
| `shop.ability_price_synergy` | 30 | 🟡 (임시) |
| `shop.reroll_price_base` | 10 | 🟡 (임시) |
| `shop.reroll_price_increment` | 5 | 🟡 (임시) |

---

## 적 턴 패턴 구조 — OQ-014 ✅ 확정 2026-05-05

**구조**: 몬스터마다 고유 상황 텍스트 1개 + 선택지 2택 1.

| 항목 | 값 | 상태 |
|------|-----|------|
| 적 턴 텍스트 | 몬스터별 고유 상황 (공격 예고 / 상태 변화 / 선택 강요 / NPC 개입 등) | ✅ |
| 선택지 수 | 2택 1 | ✅ |
| 선택지 내용 | 몬스터마다 상이 — 작가 직접 작성 (작가 권한) | ✅ |
| 타이머 | 10초 (Glitch 연동 축소: 3~5=5초, 6+=3초) | ✅ D-022 |

**Codex 구현 참조:**
- `EnemyData.cs` → `patternId` 필드 → `EnemyPatternData SO` (신규 생성 필요)
- `EnemyPatternData SO` 구조 (임시):
  - `situationText`: string (상황 텍스트)
  - `choiceA`: string + 결과 NumericParam
  - `choiceB`: string + 결과 NumericParam

---

## 기존 SO 자산과 GDD 명세 매핑

> 현재 repo에 이미 생성된 SO 자산 3종. GDD 명세 ID와 다름 — 향후 정리 필요 또는 유지 결정 필요.

| 기존 SO 파일명 | GDD 명세 ID 후보 | 비고 |
|--------------|----------------|------|
| SO_Enemy_ENEMY_COLLAPSE_ECHO | ENEMY_SHADE_03 또는 ENEMY_WRAITH_04 | 확인 필요 🟡 |
| SO_Enemy_ENEMY_EMPTY_ARMOR | ENEMY_WALKER_01 또는 ENEMY_CRAWLER_02 | 확인 필요 🟡 |
| SO_Enemy_ENEMY_FRACTURE_HOUND | ENEMY_HERALD_05 (정예) 후보 | 확인 필요 🟡 |

---

## 미결 사항

- 보상 필드 (`gold`, `xp`, `glitch`, `affinity`): `EnemyData.cs` 확장 vs 별도 `EnemyRewardData SO` 분리 — Codex와 구조 결정 필요 (Phase 2 진입 전)
- 기존 SO 자산 3종 ID 매핑: 작가 확인 후 rename 또는 신규 생성 결정
- `EnemyPatternData SO` 구조: Codex 구현 시 확정 필요
- 유물 아이템 목록·효과: OQ-016 확정 대기
