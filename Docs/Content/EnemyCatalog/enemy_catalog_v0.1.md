# Enemy Catalog v0.1

Runtime source of truth is ScriptableObject data under `Assets/_Project/Data/Enemies/` plus `SO_FloorEnemyPool_v0_1`.
This document is a handoff index only. It does not define final lore or final encounter prose.

| floor | enemyStableId | koreanName | rank | concept | assetFilename | implementationStatus | notes |
|---:|---|---|---|---|---|---|---|
| 1 | ENEMY_BANDIT_MELEE_01 | 도적 검수 | normal | 근거리 인간형 적 | enemy_bandit_melee_01.png | ready | 신규 placeholder EnemyData |
| 1 | ENEMY_BANDIT_RANGED_01 | 도적 궁수 | normal | 원거리 인간형 적 | enemy_bandit_ranged_01.png | ready | 신규 placeholder EnemyData |
| 1 | ENEMY_SLIME_01 | 슬라임 | normal | 기초 생물형 적 | enemy_slime_01.png | ready | 신규 placeholder EnemyData |
| 1 | ENEMY_SKELETON_01 | 스켈레톤 | normal | 기초 언데드 적 | enemy_skeleton_01.png | ready | 신규 placeholder EnemyData |
| 1 | ENEMY_WILD_BEAST_01 | 야생 짐승 | normal | 기초 야수형 적 | enemy_wild_beast_01.png | ready | 신규 placeholder EnemyData |
| 1 | ENEMY_SLIME_POOL_01 | 슬라임 무리 | elite | 층 1 엘리트 생물형 적 | enemy_slime_pool_01.png | ready | jar elite fallback에도 사용 가능 |
| 1 | ENEMY_WILD_BEAST_PACK_01 | 야생 짐승 무리 | elite | 층 1 엘리트 야수형 적 | enemy_wild_beast_pack_01.png | ready | jar elite fallback에도 사용 가능 |
| 1 | ENEMY_STATUE_01 | 움직이는 석상 | boss | Floor 1 boss placeholder | enemy_statue_01.png | ready | floor boss slot |
| 2 | ENEMY_FRACTURE_HOUND | 변이된 들짐승 | normal | 균열에 오염된 야수 | enemy_fracture_hound_demo.png | ready | existing mapping |
| 2 | ENEMY_MANEATER_PLANT_01 | 식인 식물 | normal | 층 2 식물형 적 | enemy_maneater_plant_01.png | ready | 신규 placeholder EnemyData |
| 2 | ENEMY_MERCENARY_MELEE_01 | 용병 검수 | normal | 근거리 용병 | enemy_mercenary_melee_01.png | ready | 신규 placeholder EnemyData |
| 2 | ENEMY_MERCENARY_RANGED_01 | 용병 사수 | normal | 원거리 용병 | enemy_mercenary_ranged_01.png | ready | 신규 placeholder EnemyData |
| 2 | ENEMY_IRON_MAIDEN_01 | 아이언 메이든 | elite | 층 2 엘리트 장치형 적 | enemy_iron_maiden_01.png | ready | 신규 placeholder EnemyData |
| 2 | BOSS_GATE_01 | 문지기 | boss | Floor 2 boss | enemy_empty_armor.png | ready | existing dedicated boss data preserved |
| 3 | ENEMY_MERCENARY_ASSASSIN_01 | 용병 암살자 | normal | 속공형 용병 | enemy_mercenary_assassin_01.png | ready | 신규 placeholder EnemyData |
| 3 | ENEMY_MERCENARY_MAGE_01 | 용병 마도사 | normal | 마법형 용병 | enemy_mercenary_mage_01.png | ready | 신규 placeholder EnemyData |
| 3 | ENEMY_EMPTY_ARMOR | 리빙아머 중무장 | normal | 중무장 갑옷형 적 | enemy_empty_armor.png | ready | existing mapping |
| 3 | ENEMY_MERCENARY_CAPTAIN_SAGAN_01 | 용병대장 사간 | elite | 층 3 엘리트 지휘관 | enemy_mercenary_captain_sagan_01.png | ready | 신규 placeholder EnemyData |
| 3 | ENEMY_COLLAPSE_ECHO | 스펙터 | boss | collapse echo | enemy_collapse_echo.png | ready | existing mapping |
| 4 | ENEMY_SKELETON_HORDE_01 | 스켈레톤 군집 | normal | 언데드 무리 | enemy_skeleton_horde_01.png | ready | 신규 placeholder EnemyData |
| 4 | ENEMY_LIVING_TOMBSTONE_01 | 살아있는 묘비 | normal | 묘비형 적 | enemy_living_tombstone_01.png | ready | 신규 placeholder EnemyData |
| 4 | ENEMY_LIVING_ARMOR_LIGHT_01 | 리빙아머 경장 | normal | 경장 갑옷형 적 | enemy_living_armor_light_01.png | ready | 신규 placeholder EnemyData |
| 4 | ENEMY_SHADE_03 | 금기를 엿본 자 | elite | 그림자형 엘리트 | enemy_shade_03.png | ready | existing mapping |
| 4 | ENEMY_WRAITH_04 | 진리를 엿본 자 | boss | Floor 4 boss placeholder | enemy_wraith_04.png | ready | existing mapping |
| 5 | ENEMY_MANEATER_JUNGLE_01 | 밀림 식인수 | normal | 고층 식물형 적 | enemy_maneater_jungle_01.png | ready | 신규 placeholder EnemyData |
| 5 | ENEMY_HOMUNCULUS_01 | 호문쿨루스 | normal | 조형 생명체 | enemy_homunculus_01.png | ready | 신규 placeholder EnemyData |
| 5 | ENEMY_COLLAPSE_ECHO | 스펙터 | elite | 고층 스펙터 elite reuse | enemy_collapse_echo.png | ready | existing mapping reused |
| 5 | BOSS_APEX_02 | 최종 보스 | boss | final boss | enemy_boss_apex_02.png | ready | existing final boss data preserved |

## Missing Art

- No listed art is missing at the time of this catalog.
