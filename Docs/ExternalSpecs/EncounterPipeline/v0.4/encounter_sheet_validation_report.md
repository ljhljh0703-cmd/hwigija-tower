# Encounter Sheet Validation Report v0.4

- physical source rows: 179
- skipped repeated header rows: 8
- normalized content rows: 171
- main events: 33
- choice rows: 138
- duplicate eventStableId: none
- duplicate choiceStableId: none
- missing stableId: none
- unsupported effect/requirement syntax in source sheet: none present; source DTO columns are blank
- temporary runtime effects: assigned after source normalization; see `temporary_effect_assignment_report_v0.4.md`
- temporary item/ability/enemy/reward/memory refs: all resolve against current repo data; see `temporary_effect_homework.csv` for replacement work
- local structural problems: 0

## Unsupported / Author Review Markers
- GAME_OVER_EFFECT_UNSUPPORTED: 1
- NEEDS_AUTHOR_VALUE: 47
- NEEDS_REF: 90

## Category Count
- CombatGate: 1
- GeneralEncounter: 24
- MoralChoice: 3
- NpcRest: 1
- Shop: 4

## Event Index

| floor | eventStableId | title | category | choices | image | author review choices |
|---:|---|---|---|---:|---|---:|
| 1 | EVT_F01_JAR_ROOM | 항아리 방 | GeneralEncounter | 4 | evt_f01_jar_room_bg.png | 4 |
| 1 | EVT_F01_PRIEST | 사제 | MoralChoice | 3 | evt_f01_priest_bg.png | 3 |
| 2 | EVT_F02_MAGIC_ANVIL | 마법 모루 | GeneralEncounter | 3 | evt_f02_magic_anvil_bg.png | 3 |
| 3 | EVT_F03_BALCONY | 발코니 | GeneralEncounter | 10 | evt_f03_balcony_bg.png | 10 |
| 1 | EVT_F01_TREASURE | 보물 | GeneralEncounter | 3 | evt_f01_treasure_bg.png | 3 |
| 3 | EVT_F03_HIGH_AND_LOW | 높은 곳에서, 낮은 곳에서 | GeneralEncounter | 4 | evt_f03_high_and_low_bg.png | 4 |
| 2 | EVT_F02_COURT_PAINTER | 궁중 화가 | GeneralEncounter | 4 | evt_f02_court_painter_bg.png | 4 |
| 1 | EVT_F01_MERCHANT | 상인 (1층) | Shop | 3 | evt_f01_merchant_bg.png | 3 |
| 2 | EVT_F02_MERCHANT | 상인 (2층) | Shop | 3 | evt_f02_merchant_bg.png | 3 |
| 3 | EVT_F03_MERCHANT | 상인 (3층) | Shop | 3 | evt_f03_merchant_bg.png | 3 |
| 4 | EVT_F04_MERCHANT | 상인 (4~5층) | Shop | 4 | evt_f04_merchant_bg.png | 4 |
| 1 | EVT_F01_REST | 휴식 | NpcRest | 4 | evt_f01_rest_bg.png | 4 |
| 1 | EVT_F01_GATEKEEPER | 문지기 | CombatGate | 3 | evt_f01_gatekeeper_bg.png | 3 |
| 1 | EVT_F01_RESISTANCE_LINE | 저항선 | GeneralEncounter | 4 | evt_f01_resistance_line_bg.png | 4 |
| 1 | EVT_F01_LAST_WORDS | 유언 | GeneralEncounter | 2 | evt_f01_last_words_bg.png | 2 |
| 2 | EVT_F02_GARDEN | 정원 | GeneralEncounter | 3 | evt_f02_garden_bg.png | 3 |
| 2 | EVT_F02_MERCENARY_OUTPOST | 용병 전초기지 | GeneralEncounter | 5 | evt_f02_mercenary_outpost_bg.png | 5 |
| 2 | EVT_F02_BANDIT_DEN | 도적소굴 | GeneralEncounter | 9 | evt_f02_bandit_den_bg.png | 9 |
| 3 | EVT_F03_ABANDONED_TOMBSTONE | 버려진 묘비 | GeneralEncounter | 3 | evt_f03_abandoned_tombstone_bg.png | 3 |
| 3 | EVT_F03_LIBRARY | 서재 | GeneralEncounter | 4 | evt_f03_library_bg.png | 4 |
| 3 | EVT_F03_POLTERGEIST | 폴터가이스트 | GeneralEncounter | 3 | evt_f03_poltergeist_bg.png | 3 |
| 4 | EVT_F04_SACRIFICE | 제물 | GeneralEncounter | 3 | evt_f04_sacrifice_bg.png | 3 |
| 4 | EVT_F04_HAUNTED_ARMORY | 귀신들린 무기고 | GeneralEncounter | 6 | evt_f04_haunted_armory_bg.png | 6 |
| 4 | EVT_F04_CULT_LEADER | 사교주 | MoralChoice | 4 | evt_f04_cult_leader_bg.png | 4 |
| 4 | EVT_F04_FORBIDDEN_BOOK | 금서 | GeneralEncounter | 3 | evt_f04_forbidden_book_bg.png | 3 |
| 4 | EVT_F04_RITUAL_ALTAR | 의식제단 | GeneralEncounter | 3 | evt_f04_ritual_altar_bg.png | 3 |
| 4 | EVT_F04_LIFE_CREATION | 생명창조 | GeneralEncounter | 3 | evt_f04_life_creation_bg.png | 3 |
| 5 | EVT_F05_OATH_OF_LOYALTY | 충성의 맹세 | GeneralEncounter | 2 | evt_f05_oath_of_loyalty_bg.png | 2 |
| 5 | EVT_F05_CRADLE_OF_OUTER_GODS | 외신들의 요람 | GeneralEncounter | 4 | evt_f05_cradle_of_outer_gods_bg.png | 4 |
| 5 | EVT_F05_ANCIENT_SEED | 고대의 씨앗 | GeneralEncounter | 6 | evt_f05_ancient_seed_bg.png | 6 |
| 5 | EVT_F05_INQUISITION | 이단 심판 | MoralChoice | 3 | evt_f05_inquisition_bg.png | 3 |
| 5 | EVT_F05_FULL_PREPARATION | 만반의 준비 | GeneralEncounter | 3 | evt_f05_full_preparation_bg.png | 3 |
| 1 | EVT_F01_CULTIST_FUNERAL | 사교도의 장례식 | GeneralEncounter | 14 | evt_f01_cultist_funeral_bg.png | 14 |

## Image Availability
- available expected images: 32
- missing expected images: 1
- Assets/_Project/Art/Encounters/evt_f01_cultist_funeral_bg.png
