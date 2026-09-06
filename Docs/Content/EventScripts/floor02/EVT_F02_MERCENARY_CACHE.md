# EVT_F02_MERCENARY_CACHE — 용병 보급품

## Metadata
- floor: 2
- eventId: EVT_F02_MERCENARY_CACHE
- eventType: Event
- tone: 거래
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f02_mercenary_cache_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
용병의 보급품을 가져가거나 함정을 해제한다.

## Event Body
부서진 보급 상자가 바리케이드 뒤에 숨겨져 있다. 상자 위에는 급히 묶은 끈과 조잡한 경보기들이 얽혀 있다. 누군가 돌아올 것처럼 정리되어 있다.

## Cutscene Direction
- 장면: 임시 용병 전초기지 잔해
- 중심 오브젝트: 보급 상자와 경보기 끈
- 인물 포함 여부: 없음
- 색감: 녹슨 철색, 탁한 갈색
- 피해야 할 것: 현대 군장, 선명한 부대 문장

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Broken mercenary supply crate behind a crude barricade, tied alarm strings and old bandages.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F02_MERCENARY_CACHE_TAKE
- label: 그대로 가져간다
- preview: Gold +12 / 30% 전투
- requirement: None
- effects:
  - ModifyGold(+12)
  - StartCombat(ENEMY_MERCENARY_MELEE_01)
- resultText: 상자를 열자 동전과 붕대가 보였다. 동시에 복도 끝에서 발소리가 들렸다.
- mataiosIntent: 위험한 욕심으로 판단하고 경계한다.
- probabilityHint: 70% reward only / 30% combat
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F02_MERCENARY_CACHE_DISARM
- label: 함정을 해제한다
- preview: StatAtLeast(mental,1) / AddItem
- requirement: StatAtLeast(mental,1)
- effects:
  - ModifyMental(-1)
  - AddItem(ITEM_FIELD_BANDAGE)
- resultText: 끈을 하나씩 풀자 경보기는 조용히 멎었다. 상자 안에는 깨끗한 붕대가 남아 있다.
- mataiosIntent: 침착한 손놀림에 안도한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F02_MERCENARY_CACHE_LEAVE
- label: 건드리지 않는다
- preview: Affinity +1
- requirement: None
- effects:
  - ModifyAffinity(+1)
- resultText: 보급품을 그대로 두었다. 누가 돌아오든 빈손은 아닐 것이다.
- mataiosIntent: 그 선택을 조용히 받아들인다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: StatAtLeast(mental,1)
- effect DTO: ModifyGold(+12), StartCombat(ENEMY_MERCENARY_MELEE_01), ModifyMental(-1), AddItem(ITEM_FIELD_BANDAGE), ModifyAffinity(+1)
- possible combat handoff: ENEMY_MERCENARY_MELEE_01
- possible item reward: ITEM_FIELD_BANDAGE
- possible memory unlock: None
- hidden values: None
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
