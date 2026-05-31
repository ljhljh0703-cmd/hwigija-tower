# Temporary Effect Assignment Report v0.4

- scope: 33 EncounterData assets / 138 choices
- purpose: make sheet encounters temporarily resolvable in runtime without inventing final design
- rule: no internal instability stat effect; no new C# runtime code; existing refs only; all assignments require later writer/designer review

## Temporary Reference Defaults
- generic relic: `RELIC_GENERIC_01`
- sword reward: `RELIC_SWORD_01` / `ABILITY_SWORD_02`
- guard reward: `RELIC_GUARD_01` / `ABILITY_GUARD_01`
- arts reward: `RELIC_ARTS_01` / `ABILITY_ARTS_03`
- memory by floor: `MEM_FRAGMENT_01`...`MEM_FRAGMENT_05`
- fallback reward bundle: `REWARD_CACHE_SMALL`

## Assignments

| row | event | choice | source label | source hint | temporary effects |
|---:|---|---|---|---|---|
| 3 | EVT_F01_JAR_ROOM | CHOICE_EVT_F01_JAR_ROOM_PATTERNED | 신기한 문양이 각인 된 항아리 | 80% 확률 골드 획득 | ModifyGold(10) |
| 4 | EVT_F01_JAR_ROOM | CHOICE_EVT_F01_JAR_ROOM_PATTERNED_ELITE_COMBAT | 신기한 문양이 각인 된 항아리 | 20% 확률 엘리트 전투 | StartCombat(ENEMY_SLIME_01) |
| 5 | EVT_F01_JAR_ROOM | CHOICE_EVT_F01_JAR_ROOM_PLAIN | 평범한 항아리 | 체력 // 스테미나 회복 | ModifyHp(8); ModifyMental(2) |
| 6 | EVT_F01_JAR_ROOM | CHOICE_EVT_F01_JAR_ROOM_CRACKED | 금 간 항아리 | 3번의 전투 데미지 버프 | AddAbility(ABILITY_ARTS_03); ModifyAffinity(-1) |
| 8 | EVT_F01_PRIEST | CHOICE_EVT_F01_PRIEST_PRAY | 함께 기도한다. | 체력 완전 회복 | ModifyHp(20); ModifyMental(3); ModifyMental(2); ModifyAffinity(1) |
| 9 | EVT_F01_PRIEST | CHOICE_EVT_F01_PRIEST_ASK_TEACHING | 가르침을 구한다 | 다음 전투 (코스트 관련) 버프  + 강화 | AddAbility(ABILITY_SCOUT); ModifyAffinity(1) |
| 10 | EVT_F01_PRIEST | CHOICE_EVT_F01_PRIEST_ATTACK | 공격한다 | 디버프 + 유물 | StartCombat(ENEMY_SLIME_01); ModifyHp(-5); ModifyMental(-2); AddItem(RELIC_SWORD_01); AddAbility(ABILITY_SWORD_02); ModifyAffinity(-1) |
| 12 | EVT_F02_MAGIC_ANVIL | CHOICE_EVT_F02_MAGIC_ANVIL_PLACE_WEAPON | 모루 위에 무기를 올린다. | 무기 강화 (or 영구 데미지 up) | ModifyHp(-5); AddAbility(ABILITY_SCOUT) |
| 13 | EVT_F02_MAGIC_ANVIL | CHOICE_EVT_F02_MAGIC_ANVIL_PLACE_ARMOR | 모루 위에 방어구를 올린다. | 방어구 강화 (or 영구 방어도 up) | ModifyHp(-5); AddAbility(ABILITY_GUARD_01) |
| 14 | EVT_F02_MAGIC_ANVIL | CHOICE_EVT_F02_MAGIC_ANVIL_CLIMB_ON | 모루 위에 올라간다. | 체력 감소 + 디버프 + 강화 | ModifyHp(-5); ModifyMental(-2); AddAbility(ABILITY_SCOUT) |
| 16 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_NORMAL_VIEW | 평범한 풍경 | 20%확률 소량의 체력, 스태미나 회복 | ModifyHp(5); ModifyMental(2) |
| 17 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_MURKY_VIEW | 혼탁한 풍경 |  20%확률 강화 | AddAbility(ABILITY_SWORD_02) |
| 18 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_OMINOUS_VIEW | 불길한 풍경 | 20%확률 저주 | ModifyHp(-5); ModifyMental(-2) |
| 19 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_SUNNY_VIEW | 화창한 풍경 | 20%확률 엔딩 분기점 | ModifyHp(-5) |
| 20 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_JUMP_INTO_SUNNY_VIEW |  화창한 풍경 속으로 뛰어든다. | 게임 오버 | ModifyHp(-999) |
| 21 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_BASK | 볕을 쬔다. | 버프, 체력, 스태미나 회복 | ModifyHp(8); ModifyMental(2); AddAbility(ABILITY_SWORD_01) |
| 22 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_APPROACHING_VIEW | 다가오는 풍경 | 20%확률 유물 획득, 엘리트 전투 기회 | StartCombat(ENEMY_SKELETON_01); ModifyHp(-5); AddItem(RELIC_LINE_01) |
| 23 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_FACE_IT | 마주한다. | 해당 층 엘리트 전투, 전투 후 유물 획득 | StartCombat(ENEMY_SKELETON_01); AddItem(RELIC_GENERIC_01) |
| 24 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_WITHDRAW | 도망친다. | 체력 감소 | ModifyHp(-5) |
| 25 | EVT_F03_BALCONY | CHOICE_EVT_F03_BALCONY_RETURN | 돌아간다. | 아무 일 도 일어나지 않았다. | ModifyMental(1) |
| 27 | EVT_F01_TREASURE | CHOICE_EVT_F01_TREASURE_OPEN | 열어본다. | 95% 유물 | ModifyMental(2); AddItem(RELIC_GENERIC_01) |
| 28 | EVT_F01_TREASURE | CHOICE_EVT_F01_TREASURE_OPEN_GOLD | 열어본다. | 4% 대량의 골드 | ModifyGold(20) |
| 29 | EVT_F01_TREASURE | CHOICE_EVT_F01_TREASURE_OPEN_HP | 열어본다. | 1% 체력 감소 | ModifyHp(-5) |
| 31 | EVT_F03_HIGH_AND_LOW | CHOICE_EVT_F03_HIGH_AND_LOW_HIGH_VOICE | 높은 곳에서의 탄성 | 49.5% 확률 외신의 부름. 고대 신 진영 몬스터 대항용 유물 획득 | ModifyMental(2); ModifyMental(-2); AddItem(RELIC_GENERIC_01) |
| 32 | EVT_F03_HIGH_AND_LOW | CHOICE_EVT_F03_HIGH_AND_LOW_ACTION_32 | 낮은 곳에서의 괴성 | 49.5% 확률 고대신의 부름. 외신 진영 몬스터 대항용 유물 획득 | ModifyMental(2); ModifyMental(-2); AddItem(RELIC_GUARD_01) |
| 33 | EVT_F03_HIGH_AND_LOW | CHOICE_EVT_F03_HIGH_AND_LOW_ACTION_33 | 내면의 울림 | 1% 확률 주신의 부름. 특별한, 강력한 효과 유물 획득 | ModifyMental(-2); AddItem(RELIC_GENERIC_01) |
| 34 | EVT_F03_HIGH_AND_LOW | CHOICE_EVT_F03_HIGH_AND_LOW_ACTION_34 | 무시한다. | 스태미나? 감소, 디버프, 체력 잃음, 특별한 유물 획득 | ModifyHp(-5); ModifyMental(2); ModifyMental(-2); AddItem(RELIC_GENERIC_01); AddAbility(ABILITY_RECALL_ANCHOR) |
| 36 | EVT_F02_COURT_PAINTER | CHOICE_EVT_F02_COURT_PAINTER_PORTRAIT | 초상화를 부탁한다 | 유물 획득 | AddItem(RELIC_ARTS_01) |
| 37 | EVT_F02_COURT_PAINTER | CHOICE_EVT_F02_COURT_PAINTER_STILL_LIFE | 정물화를 부탁한다 | 체력 소량 회복 | ModifyHp(5) |
| 38 | EVT_F02_COURT_PAINTER | CHOICE_EVT_F02_COURT_PAINTER_LANDSCAPE | 풍경화를 부탁한다 | 스탯 강화 | AddAbility(ABILITY_ARTS_03) |
| 39 | EVT_F02_COURT_PAINTER | CHOICE_EVT_F02_COURT_PAINTER_ACTION_39 | 자화상을 부탁한다(히든) | 특별한 유물 획득 | AddItem(RELIC_ARTS_01) |
| 42 | EVT_F01_MERCHANT | CHOICE_EVT_F01_MERCHANT_USE_SHOP | 상점을 이용한다. | 상점 이용 | ModifyMental(-1) |
| 43 | EVT_F01_MERCHANT | CHOICE_EVT_F01_MERCHANT_ACTION_43 | 상점을 떠난다. | 상점 퇴장 | ModifyMental(-1) |
| 44 | EVT_F01_MERCHANT | CHOICE_EVT_F01_MERCHANT_LEAVE_SHOP | 상점을 이용하지 않는다. | 상점 퇴장 | ModifyMental(1) |
| 46 | EVT_F02_MERCHANT | CHOICE_EVT_F02_MERCHANT_USE_SHOP | 상점을 이용한다. | 상점 이용 | ModifyMental(-1) |
| 47 | EVT_F02_MERCHANT | CHOICE_EVT_F02_MERCHANT_ACTION_47 | 상점을 떠난다. | 상점 퇴장 | ModifyMental(-1) |
| 48 | EVT_F02_MERCHANT | CHOICE_EVT_F02_MERCHANT_LEAVE_SHOP | 상점을 이용하지 않는다. | 상점 퇴장 | ModifyMental(1) |
| 50 | EVT_F03_MERCHANT | CHOICE_EVT_F03_MERCHANT_USE_SHOP | 상점을 이용한다. | 상점 이용 | ModifyMental(-1) |
| 51 | EVT_F03_MERCHANT | CHOICE_EVT_F03_MERCHANT_ACTION_51 | 상점을 떠난다. | 상점 퇴장 | ModifyMental(-1) |
| 52 | EVT_F03_MERCHANT | CHOICE_EVT_F03_MERCHANT_LEAVE_SHOP | 상점을 이용하지 않는다. | 상점 퇴장 | ModifyMental(-1) |
| 54 | EVT_F04_MERCHANT | CHOICE_EVT_F04_MERCHANT_USE_SHOP | 상점을 이용한다. | 상점 이용 | ModifyMental(-1) |
| 55 | EVT_F04_MERCHANT | CHOICE_EVT_F04_MERCHANT_ACTION_55 | 상점을 떠난다. | 상점 퇴장 | ModifyMental(-1) |
| 56 | EVT_F04_MERCHANT | CHOICE_EVT_F04_MERCHANT_ACTION_56 | 공격한다. | 상점 퇴장 ( 해당 회차에서 등장x ) | StartCombat(ENEMY_STATUE_01); ModifyAffinity(-1) |
| 57 | EVT_F04_MERCHANT | CHOICE_EVT_F04_MERCHANT_LEAVE_SHOP | 상점을 이용하지 않는다. | 상점 퇴장 | ModifyMental(-1) |
| 60 | EVT_F01_REST | CHOICE_EVT_F01_REST_ACTION_60 | 휴식을 취한다 | 체력 회복 | ModifyGold(10); ModifyHp(8) |
| 61 | EVT_F01_REST | CHOICE_EVT_F01_REST_ACTION_61 | 훈련을 진행한다 | 능력치 강화 | ModifyHp(8); AddAbility(ABILITY_SWORD_02) |
| 62 | EVT_F01_REST | CHOICE_EVT_F01_REST_ACTION_62 | 유물과 공명한다 | 유물 강화 | ModifyHp(8); AddItem(RELIC_GENERIC_01); AddAbility(ABILITY_SWORD_02) |
| 63 | EVT_F01_REST | CHOICE_EVT_F01_REST_ACTION_63 | 기분을 묻는다 | 마타이오스 호감도 등락 | ModifyHp(8) |
| 66 | EVT_F01_GATEKEEPER | CHOICE_EVT_F01_GATEKEEPER_ACTION_66 | 억지로 지나간다 | 석상 전투 승리 후 유물 획득 /  50% 보물 이벤트 | StartCombat(ENEMY_FRACTURE_HOUND); AddItem(RELIC_GENERIC_01) |
| 67 | EVT_F01_GATEKEEPER | CHOICE_EVT_F01_GATEKEEPER_PASS | 지나친다 | 아무 일도 일어나지 않았다. | ModifyMental(1) |
| 68 | EVT_F01_GATEKEEPER | CHOICE_EVT_F01_GATEKEEPER_ACTION_68 | 설득한다 (AI 대화?) | 50% 특별한 유물 획득 / 50% 보물 이벤트 | AddItem(RELIC_GENERIC_01) |
| 70 | EVT_F01_RESISTANCE_LINE | CHOICE_EVT_F01_RESISTANCE_LINE_ACTION_70 | 철책을 조사한다 | 체력 소량 감소, 소량의 골드 획득 | ModifyGold(6) |
| 71 | EVT_F01_RESISTANCE_LINE | CHOICE_EVT_F01_RESISTANCE_LINE_ACTION_71 | 돌아간다 | 이벤트 퇴장 | ModifyMental(1) |
| 72 | EVT_F01_RESISTANCE_LINE | CHOICE_EVT_F01_RESISTANCE_LINE_ACTION_72 | 철책 안쪽을 살펴본다 | 30% 유물 발견 / 70%체력 소량 감소, 재도전 or 돌아간다 선택지 | ModifyHp(-5); ModifyMental(2); AddItem(RELIC_LINE_01) |
| 73 | EVT_F01_RESISTANCE_LINE | CHOICE_EVT_F01_RESISTANCE_LINE_PASS | 지나친다 | 아무 일도 일어나지 않았다. | ModifyMental(1) |
| 75 | EVT_F01_LAST_WORDS | CHOICE_EVT_F01_LAST_WORDS_BEHEAD | 목을 친다 | 스태미나 (정신력 관련) 감소 & 유물 획득 | ModifyMental(2); AddItem(RELIC_GENERIC_01); UnlockMemoryFragment(MEM_FRAGMENT_01) |
| 76 | EVT_F01_LAST_WORDS | CHOICE_EVT_F01_LAST_WORDS_ACTION_76 | 창을 뽑는다 | 스태미나 (정신력 관련) 감소 & 골드 획득 & 저주 획득 | ModifyGold(6); ModifyHp(-5); ModifyMental(2); ModifyMental(-2); UnlockMemoryFragment(MEM_FRAGMENT_01) |
| 79 | EVT_F02_GARDEN | CHOICE_EVT_F02_GARDEN_PICK_FLOWER | 꽃을 딴다 | 최대 체력 감소, 강화 | ModifyHp(-5); AddAbility(ABILITY_SWORD_02) |
| 80 | EVT_F02_GARDEN | CHOICE_EVT_F02_GARDEN_PULL_WEEDS | 잡초를 뽑는다 | 체력 소량 감소. 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 81 | EVT_F02_GARDEN | CHOICE_EVT_F02_GARDEN_WALK | 잠시 걷는다 | 체력, 정신력 회복 | ModifyHp(8); ModifyMental(2) |
| 83 | EVT_F02_MERCENARY_OUTPOST | CHOICE_EVT_F02_MERCENARY_OUTPOST_ASK_LEADER | 당신들의 대장에 대하여 | 물러난다. 로 진행 | ModifyMental(1) |
| 84 | EVT_F02_MERCENARY_OUTPOST | CHOICE_EVT_F02_MERCENARY_OUTPOST_ASK_MERCENARIES | 용병대에 대하여 | 물러난다. 로 진행 | ModifyMental(1) |
| 85 | EVT_F02_MERCENARY_OUTPOST | CHOICE_EVT_F02_MERCENARY_OUTPOST_ASK_ADVICE | 조언을 구한다 | 물러난다. 로 진행 | ModifyAffinity(1) |
| 86 | EVT_F02_MERCENARY_OUTPOST | CHOICE_EVT_F02_MERCENARY_OUTPOST_SMALL_TALK | 잡담 | 물러난다. 로 진행 | ModifyMental(1) |
| 87 | EVT_F02_MERCENARY_OUTPOST | CHOICE_EVT_F02_MERCENARY_OUTPOST_ACTION_87 | 물러난다. | 소량의 골드 획득 | ModifyGold(6) |
| 89 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_89 |  화술로 무력화 한다. | 화술 성공시 : 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 90 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_90 | 골드로 매수한다 |  | ModifyGold(6) |
| 91 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_91 | 더 많은 골드를 준다 | 유물 획득 | ModifyGold(20); AddItem(RELIC_GENERIC_01) |
| 92 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_92 | 조금의 골드를 준다 | 50%확률 돈을 더 쥐어준다 선택지 // 50% 확률 더 많은 골드를 준다 선택지 | ModifyGold(20) |
| 93 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_93 | 돈을 더 쥐어준다 | 50%확률 돈을 더 쥐어준다 선택지 // 50% 확률 더 많은 골드를 준다 선택지 | ModifyGold(20) |
| 94 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ATTACK | 공격한다 | 무작위 도적과 전투 // 전투 승리시 골드 반환 | StartCombat(ENEMY_BANDIT_MELEE_01); ModifyGold(10); ModifyAffinity(-1) |
| 95 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_95 | 돌아간다 | 골드 손실 | ModifyGold(10) |
| 96 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_96 | 물리적 으로 제압한다. | 전투 승리시 : 소량의 골드 획득 | StartCombat(ENEMY_BANDIT_MELEE_01); ModifyGold(6) |
| 97 | EVT_F02_BANDIT_DEN | CHOICE_EVT_F02_BANDIT_DEN_ACTION_97 | 물러난다. | 아무 일도 일어나지 않았다. | ModifyMental(1) |
| 99 | EVT_F03_ABANDONED_TOMBSTONE | CHOICE_EVT_F03_ABANDONED_TOMBSTONE_ACTION_99 | 파묘한다 | 저주 & 골드 획득 | ModifyGold(10); ModifyHp(-5); ModifyMental(-2) |
| 100 | EVT_F03_ABANDONED_TOMBSTONE | CHOICE_EVT_F03_ABANDONED_TOMBSTONE_ACTION_100 | 묘비를 닦는다 | 강화 | AddAbility(ABILITY_SCOUT) |
| 101 | EVT_F03_ABANDONED_TOMBSTONE | CHOICE_EVT_F03_ABANDONED_TOMBSTONE_ACTION_101 | 헌화 한다 (히든) | 다음단계 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 104 | EVT_F03_LIBRARY | CHOICE_EVT_F03_LIBRARY_ACTION_104 | 돌아 나간다 | 성공시 : 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 105 | EVT_F03_LIBRARY | CHOICE_EVT_F03_LIBRARY_ACTION_105 | 책을 반납한다 | 성공시 : 유물 획득 | ModifyMental(2); AddItem(RELIC_GENERIC_01) |
| 106 | EVT_F03_LIBRARY | CHOICE_EVT_F03_LIBRARY_ACTION_106 | 책을 책장에 꽂는다 | 성공시 : 유물 획득 | ModifyMental(2); AddItem(RELIC_GENERIC_01) |
| 107 | EVT_F03_LIBRARY | CHOICE_EVT_F03_LIBRARY_ACTION_107 | 오답 | 실패 시, 스태미나,정신력 등 감소 | ModifyMental(2) |
| 109 | EVT_F03_POLTERGEIST | CHOICE_EVT_F03_POLTERGEIST_ACTION_109 | 액자를 조사한다 | 강화 | AddAbility(ABILITY_SCOUT) |
| 110 | EVT_F03_POLTERGEIST | CHOICE_EVT_F03_POLTERGEIST_ACTION_110 | 책상을 조사한다 | 버프 | AddAbility(ABILITY_SCOUT) |
| 111 | EVT_F03_POLTERGEIST | CHOICE_EVT_F03_POLTERGEIST_ACTION_111 | 화병을 조사한다 | 골드 획득 | ModifyGold(10) |
| 113 | EVT_F04_SACRIFICE | CHOICE_EVT_F04_SACRIFICE_ACTION_113 | 사로잡는다 | 특별한 디버프 획득 (노예) | ModifyHp(-8); ModifyMental(-2); AddAbility(ABILITY_RECALL_ANCHOR) |
| 114 | EVT_F04_SACRIFICE | CHOICE_EVT_F04_SACRIFICE_ACTION_114 | 처치한다 | 강화 & 작은 디버프 | ModifyHp(-8); ModifyMental(-2); AddAbility(ABILITY_SWORD_02) |
| 115 | EVT_F04_SACRIFICE | CHOICE_EVT_F04_SACRIFICE_ACTION_115 | 풀어준다 | 아무 일도 일어나지 않았다. | ModifyMental(-1) |
| 117 | EVT_F04_HAUNTED_ARMORY | CHOICE_EVT_F04_HAUNTED_ARMORY_ACTION_117 | 냉병기를 찾아본다 | 강화 위주 | AddAbility(ABILITY_SWORD_02) |
| 118 | EVT_F04_HAUNTED_ARMORY | CHOICE_EVT_F04_HAUNTED_ARMORY_ACTION_118 | 검 | 강화 | AddAbility(ABILITY_SWORD_02) |
| 119 | EVT_F04_HAUNTED_ARMORY | CHOICE_EVT_F04_HAUNTED_ARMORY_ACTION_119 | 철퇴 | 유물 강화 | AddItem(RELIC_SWORD_01); AddAbility(ABILITY_SWORD_02) |
| 120 | EVT_F04_HAUNTED_ARMORY | CHOICE_EVT_F04_HAUNTED_ARMORY_ACTION_120 | 열병기를 찾아본다 | 버프 위주 | AddAbility(ABILITY_SWORD_02) |
| 121 | EVT_F04_HAUNTED_ARMORY | CHOICE_EVT_F04_HAUNTED_ARMORY_ACTION_121 | 화승총 | 데미지 강화 버프 | AddAbility(ABILITY_SWORD_02) |
| 122 | EVT_F04_HAUNTED_ARMORY | CHOICE_EVT_F04_HAUNTED_ARMORY_ACTION_122 | 화약단지 | 체력 강화 버프 | AddAbility(ABILITY_SWORD_02) |
| 125 | EVT_F04_CULT_LEADER | CHOICE_EVT_F04_CULT_LEADER_ACTION_125 | 무엇을 믿는지 묻는다 | 보스 "불완전한 부름" 약화 (만약 보스라면) | ModifyMental(-1) |
| 126 | EVT_F04_CULT_LEADER | CHOICE_EVT_F04_CULT_LEADER_ACTION_126 | 계시를 듣는다 | 강화 | AddAbility(ABILITY_SWORD_02) |
| 127 | EVT_F04_CULT_LEADER | CHOICE_EVT_F04_CULT_LEADER_ACTION_127 | 모독한다 | 즉시 보스전 "불완전한 부름" 실시. 승리 시 강력한 유물. | AddItem(ITEM_TORN_CHARM) |
| 128 | EVT_F04_CULT_LEADER | CHOICE_EVT_F04_CULT_LEADER_ACTION_128 | 나는 아무것도 믿지 않는다 | 보스 "불완전한 부름" 강화 (만약 보스라면) & 유물 획득 | AddItem(ITEM_TORN_CHARM); AddAbility(ABILITY_SWORD_02) |
| 130 | EVT_F04_FORBIDDEN_BOOK | CHOICE_EVT_F04_FORBIDDEN_BOOK_ACTION_130 | 책을 펼쳐본다 | 50% 축복 책 | ModifyMental(-1) |
| 131 | EVT_F04_FORBIDDEN_BOOK | CHOICE_EVT_F04_FORBIDDEN_BOOK_ACTION_131 | 책을 펼쳐본다 | 50% 저주 책  | ModifyHp(-8); ModifyMental(-2) |
| 132 | EVT_F04_FORBIDDEN_BOOK | CHOICE_EVT_F04_FORBIDDEN_BOOK_IGNORE | 무시한다 | 아무일도 일어나지 않았다. | ModifyMental(1) |
| 134 | EVT_F04_RITUAL_ALTAR | CHOICE_EVT_F04_RITUAL_ALTAR_ACTION_134 | 바친다 | 조력자의 체력, 호감도 대폭 감소, 강력한 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 135 | EVT_F04_RITUAL_ALTAR | CHOICE_EVT_F04_RITUAL_ALTAR_ACTION_135 | 바쳐진다 | 체력 대폭 감소, 강력한 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 136 | EVT_F04_RITUAL_ALTAR | CHOICE_EVT_F04_RITUAL_ALTAR_ACTION_136 | 제물 올리기 (히든) | 강력한 유물 획득 + 특별한 디버프 (노예) 제거 | ModifyHp(-8); ModifyMental(-2); AddItem(RELIC_GENERIC_01); AddAbility(ABILITY_RECALL_ANCHOR) |
| 138 | EVT_F04_LIFE_CREATION | CHOICE_EVT_F04_LIFE_CREATION_ACTION_138 | 시험관을 파괴한다 | 최대 체력 감소 & 버프 | ModifyHp(-8); AddAbility(ABILITY_SWORD_01) |
| 139 | EVT_F04_LIFE_CREATION | CHOICE_EVT_F04_LIFE_CREATION_ACTION_139 | 동력을 차단한다 | 보스전 시작 시, 보스에게 특수 디버프 | ModifyHp(-8); ModifyMental(-2); AddAbility(ABILITY_SWORD_01) |
| 140 | EVT_F04_LIFE_CREATION | CHOICE_EVT_F04_LIFE_CREATION_ACTION_140 | 통로를 파괴한다 | 해당 층 몹 디버프 | ModifyHp(-8); ModifyMental(-2); AddAbility(ABILITY_SWORD_01) |
| 143 | EVT_F05_OATH_OF_LOYALTY | CHOICE_EVT_F05_OATH_OF_LOYALTY_KING | 왕 | 특별한 유물 획득, 저주 획득 | ModifyHp(-8); ModifyMental(-2); AddItem(RELIC_GENERIC_01) |
| 144 | EVT_F05_OATH_OF_LOYALTY | CHOICE_EVT_F05_OATH_OF_LOYALTY_KINGDOM | 왕국 | 특별한 유물 획득, 저주 획득 | ModifyHp(-8); ModifyMental(-2); AddItem(RELIC_GENERIC_01) |
| 146 | EVT_F05_CRADLE_OF_OUTER_GODS | CHOICE_EVT_F05_CRADLE_OF_OUTER_GODS_PASS | 지나친다 | 아무 일도 일어나지 않았다 | ModifyMental(1) |
| 147 | EVT_F05_CRADLE_OF_OUTER_GODS | CHOICE_EVT_F05_CRADLE_OF_OUTER_GODS_WATCH_STARS | 별을 바라본다 |  | ModifyMental(-1) |
| 148 | EVT_F05_CRADLE_OF_OUTER_GODS | CHOICE_EVT_F05_CRADLE_OF_OUTER_GODS_BEAUTIFUL | 아름답다 | 체력 회복, 강화, 버프, 저주 | ModifyHp(8); ModifyMental(-2); AddAbility(ABILITY_SWORD_02) |
| 149 | EVT_F05_CRADLE_OF_OUTER_GODS | CHOICE_EVT_F05_CRADLE_OF_OUTER_GODS_UGLY | 추하다 | 체력 감소, 유물 획득 | ModifyHp(-8); AddItem(RELIC_GENERIC_01) |
| 151 | EVT_F05_ANCIENT_SEED | CHOICE_EVT_F05_ANCIENT_SEED_EAT | 먹는다 | 최대 체력 감소 , 체력 최대 회복 | ModifyHp(8); ModifyHp(-8) |
| 152 | EVT_F05_ANCIENT_SEED | CHOICE_EVT_F05_ANCIENT_SEED_DISCARD | 버린다 | 스태미나 감소 | ModifyAffinity(-1) |
| 153 | EVT_F05_ANCIENT_SEED | CHOICE_EVT_F05_ANCIENT_SEED_PLANT | 심는다 |  | ModifyMental(-1) |
| 154 | EVT_F05_ANCIENT_SEED | CHOICE_EVT_F05_ANCIENT_SEED_HEAD | 머리에 | 최대 체력 감소, 강화 | ModifyHp(-8); AddAbility(ABILITY_SWORD_02) |
| 155 | EVT_F05_ANCIENT_SEED | CHOICE_EVT_F05_ANCIENT_SEED_HEART | 심장에 | 최대 체력 감소, 유물 획득 | ModifyHp(-8); AddItem(RELIC_GENERIC_01) |
| 156 | EVT_F05_ANCIENT_SEED | CHOICE_EVT_F05_ANCIENT_SEED_LEG | 다리에 | 최대 체력 감소, 버프 | ModifyHp(-8); AddAbility(ABILITY_SWORD_01) |
| 158 | EVT_F05_INQUISITION | CHOICE_EVT_F05_INQUISITION_SILENCE | 침묵한다 | 아무 일도 일어나지 않았다. | ModifyMental(1) |
| 159 | EVT_F05_INQUISITION | CHOICE_EVT_F05_INQUISITION_WITHDRAW | 도망간다 | 최대 체력 감소, 스태미나 감소, 노드 변경 | ModifyHp(-8) |
| 160 | EVT_F05_INQUISITION | CHOICE_EVT_F05_INQUISITION_PERSUADE | 설득 | 성공시 유물 획득 // 실패시 체력 감소 | ModifyHp(-8); AddItem(RELIC_GENERIC_01) |
| 162 | EVT_F05_FULL_PREPARATION | CHOICE_EVT_F05_FULL_PREPARATION_COMPOSE_MIND | 정신을 갈무리한다 | 강화 | ModifyMental(2); AddAbility(ABILITY_SWORD_02) |
| 163 | EVT_F05_FULL_PREPARATION | CHOICE_EVT_F05_FULL_PREPARATION_INVESTIGATE | 주변을 조사해본다 | 유물 획득 | AddItem(RELIC_GENERIC_01) |
| 164 | EVT_F05_FULL_PREPARATION | CHOICE_EVT_F05_FULL_PREPARATION_ASK_SHORT_ADVICE |  짧은 조언을 구한다 | 마타이오스 버프 & 호감도 상승 | AddAbility(ABILITY_SWORD_01); ModifyAffinity(1) |
| 167 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_DONATE | 부조금을 전한다 | 골드 소량 감소 | ModifyGold(6) |
| 168 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_GREET_POLITELY | 안녕인사를 공손히 전한다 |  | ModifyAffinity(1) |
| 169 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_BURN_INCENSE | 분향 한다 |  | ModifyMental(1) |
| 170 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_OFFER_FLOWERS | 헌화 한다 |  | ModifyMental(1) |
| 171 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_BOW | 절 한다 |  | ModifyAffinity(1) |
| 172 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_ONE | 1번 |  | ModifyMental(-1) |
| 173 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_TWO | 2번 |  | ModifyMental(-1) |
| 174 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_THREE | 3번 |  | ModifyMental(-1) |
| 175 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_SHAKE_HANDS | 상주와 악수한다 |  | ModifyAffinity(-1) |
| 176 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_BOW_TO_MOURNER | 상주에게 절 한다 |  | ModifyAffinity(1) |
| 177 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_EMBRACE_MOURNER | 상주를 껴안는다 |  | ModifyAffinity(1); ModifyAffinity(-1) |
| 178 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_ONE_R178 | 1번 |  | ModifyMental(-1) |
| 179 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_TWO_R179 | 2번 |  | ModifyMental(-1) |
| 180 | EVT_F01_CULTIST_FUNERAL | CHOICE_EVT_F01_CULTIST_FUNERAL_THREE_R180 | 3번 |  | ModifyMental(-1) |
