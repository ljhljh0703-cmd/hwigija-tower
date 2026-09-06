# Event Pack Floor 01-05 Index v0.1

| floor | eventId | title | riskLevel | choices count | possible rewards | possible costs | combat handoff | memory unlock | image needed | implementation priority |
|---:|---|---|---|---:|---|---|---|---|---|---|
| 1 | EVT_F01_ABANDONED_CAMP | 버려진 야영지 | low | 3 | ModifyHp(+4)<br>ModifyAffinity(+1)<br>ModifyGold(+6)<br>ModifyMental(+1)<br>AddRunBuff(BUFF_NEXT_COMBAT_DEFENSE_01) | ModifyAffinity(-1) | None | None | Assets/_Project/Art/Encounters/evt_f01_abandoned_camp_bg.png | P0 |
| 1 | EVT_F01_FALLEN_GUIDE | 쓰러진 안내자 흔적 | medium | 3 | ModifyAffinity(+2)<br>ModifyMental(+1)<br>AddItem(ITEM_11)<br>ModifyMental(+0) | ModifyAffinity(-1) | None | None | Assets/_Project/Art/Encounters/evt_f01_fallen_guide_bg.png | P0 |
| 1 | EVT_F01_JAR_ROOM | 항아리 방 | low | 3 | ModifyGold(+8)<br>ModifyHp(+5)<br>ModifyMental(+1)<br>AddRunBuff(BUFF_NEXT_3_COMBATS_DAMAGE_01) | ModifyMental(-1) | ENEMY_SLIME_POOL_01 | None | Assets/_Project/Art/Encounters/evt_f01_jar_room_bg.png | P0 |
| 1 | EVT_F01_LOCKED_CHEST | 잠긴 상자 | low | 3 | ModifyGold(+8)<br>AddItem(ITEM_06)<br>ModifyMental(+1) | ModifyHp(-2) | None | None | Assets/_Project/Art/Encounters/evt_f01_locked_chest_bg.png | P0 |
| 2 | EVT_F02_BLOOMING_WOUND | 피어난 상처 | medium | 3 | AddAbility(ABILITY_ARTS_01)<br>AddRunBuff(BUFF_NEXT_COMBAT_DAMAGE_01)<br>ModifyMental(+1) | ModifyHp(-4)<br>ModifyMental(-1) | None | None | Assets/_Project/Art/Encounters/evt_f02_blooming_wound_bg.png | P0 |
| 2 | EVT_F02_MERCENARY_CACHE | 용병 보급품 | medium | 3 | ModifyGold(+12)<br>AddItem(ITEM_FIELD_BANDAGE)<br>ModifyAffinity(+1) | ModifyMental(-1) | ENEMY_MERCENARY_MELEE_01 | None | Assets/_Project/Art/Encounters/evt_f02_mercenary_cache_bg.png | P0 |
| 2 | EVT_F02_OLD_BARRICADE | 낡은 바리케이드 | medium | 3 | ModifyGold(+8)<br>AddRunBuff(BUFF_NEXT_COMBAT_DEFENSE_01)<br>ModifyMental(+1) | ModifyHp(-3) | ENEMY_FRACTURE_HOUND | None | Assets/_Project/Art/Encounters/evt_f02_old_barricade_bg.png | P1 |
| 2 | EVT_F02_ROOTED_STATUE | 뿌리에 감긴 석상 | medium | 3 | AddItem(ITEM_04)<br>ModifyMental(+1)<br>ModifyGold(+6)<br>ModifyAffinity(+2) | ModifyHp(-4) | None | None | Assets/_Project/Art/Encounters/evt_f02_rooted_statue_bg.png | P0 |
| 3 | EVT_F03_BROKEN_MIRROR | 깨진 거울 | medium | 3 | UnlockMemory(MEM_FRAGMENT_03)<br>AddItem(ITEM_03)<br>ModifyMental(+2)<br>ModifyAffinity(+1) | ModifyMental(-2)<br>ModifyHp(-4) | None | MEM_FRAGMENT_03 | Assets/_Project/Art/Encounters/evt_f03_broken_mirror_bg.png | P0 |
| 3 | EVT_F03_ECHOING_STEPS | 뒤따라오는 발소리 | medium | 3 | ModifyMental(+2)<br>ModifyAffinity(+1)<br>AddRunBuff(BUFF_NEXT_COMBAT_DAMAGE_01)<br>ModifyAffinity(+2) | ModifyHp(-4)<br>ModifyMental(-2) | None | None | Assets/_Project/Art/Encounters/evt_f03_echoing_steps_bg.png | P1 |
| 3 | EVT_F03_FALSE_REST | 가짜 휴식처 | medium | 3 | ModifyHp(+8)<br>ModifyMental(+2)<br>ModifyGold(+8)<br>AddItem(ITEM_LANTERN_OIL) | ModifyMental(-3) | ENEMY_MERCENARY_MAGE_01 | None | Assets/_Project/Art/Encounters/evt_f03_false_rest_bg.png | P0 |
| 3 | EVT_F03_NAMELESS_DOOR | 이름 없는 문 | medium | 3 | ModifyGold(+10)<br>ModifyAffinity(+2)<br>UnlockMemory(MEM_FRAGMENT_02)<br>ModifyMental(+1) | None | ENEMY_EMPTY_ARMOR | MEM_FRAGMENT_02 | Assets/_Project/Art/Encounters/evt_f03_nameless_door_bg.png | P1 |
| 4 | EVT_F04_CRYSTAL_SERVANT | 결정화된 하인 | high | 3 | ModifyAffinity(+3)<br>ModifyGold(+16)<br>ModifyMental(+1) | ModifyHp(-7)<br>ModifyAffinity(-3) | None | None | Assets/_Project/Art/Encounters/evt_f04_crystal_servant_bg.png | P0 |
| 4 | EVT_F04_FAILED_SUMMON | 실패한 소환진 | high | 3 | ModifyMental(+2)<br>AddRunBuff(BUFF_NEXT_BOSS_DEFENSE_01)<br>AddRunBuff(BUFF_NEXT_BOSS_DAMAGE_02)<br>ModifyAffinity(+1) | ModifyHp(-8) | None | None | Assets/_Project/Art/Encounters/evt_f04_failed_summon_bg.png | P1 |
| 4 | EVT_F04_FORBIDDEN_PAGE | 금기의 페이지 | high | 3 | AddAbility(ABILITY_ARTS_03)<br>AddItem(ITEM_10) | ModifyMental(-4)<br>ModifyHp(-3)<br>ModifyMental(-2) | None | None | Assets/_Project/Art/Encounters/evt_f04_forbidden_page_bg.png | P0 |
| 4 | EVT_F04_SILENT_ALTAR | 말 없는 제단 | high | 3 | AddRunBuff(BUFF_NEXT_2_COMBATS_DAMAGE_02)<br>AddItem(RELIC_GENERIC_01)<br>GrantRewardBundle(REWARD_CACHE_SMALL) | ModifyHp(-8)<br>ModifyGold(-12) | ENEMY_SHADE_03 | None | Assets/_Project/Art/Encounters/evt_f04_silent_altar_bg.png | P0 |
| 5 | EVT_F05_BURNING_MARK | 불타는 낙인 | high | 3 | AddRunBuff(BUFF_FINAL_BOSS_DAMAGE_02)<br>ModifyMental(+2)<br>ModifyHp(+4)<br>AddRunBuff(BUFF_FINAL_BOSS_DEFENSE_02) | ModifyHp(-7) | None | None | Assets/_Project/Art/Encounters/evt_f05_burning_mark_bg.png | P0 |
| 5 | EVT_F05_KINGS_ECHO | 왕의 잔향 | high | 3 | AddRunBuff(BUFF_FINAL_BOSS_INSIGHT_01)<br>ModifyAffinity(+2)<br>ModifyMental(+2) | ModifyMental(-3)<br>ModifyHp(-5) | None | None | Assets/_Project/Art/Encounters/evt_f05_kings_echo_bg.png | P0 |
| 5 | EVT_F05_LAST_LANTERN | 마지막 등불 | medium | 3 | AddRunBuff(BUFF_FINAL_BOSS_DEFENSE_01)<br>ModifyHp(+6)<br>ModifyMental(+2)<br>AddRunBuff(BUFF_FINAL_BOSS_CLARITY_01)<br>ModifyAffinity(+2) | ModifyGold(-8) | None | None | Assets/_Project/Art/Encounters/evt_f05_last_lantern_bg.png | P0 |
| 5 | EVT_F05_MYTH_SHARD | 신화의 파편 | high | 3 | UnlockMemory(MEM_FRAGMENT_05)<br>ModifyAffinity(+2)<br>AddItem(RELIC_GENERIC_02)<br>ModifyMental(+3) | ModifyMental(-4) | None | MEM_FRAGMENT_05 | Assets/_Project/Art/Encounters/evt_f05_myth_shard_bg.png | P0 |

## Count
- Floor 1: 4
- Floor 2: 4
- Floor 3: 4
- Floor 4: 4
- Floor 5: 4
- Total: 20

## Notes
- Source priority: Google Sheet event concepts and this session directive; prose remains writer-review draft.
- Shop and Rest node content are intentionally excluded from this Event pack because they use separate node flows.
