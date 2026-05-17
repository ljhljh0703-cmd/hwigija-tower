# EVT_F03_FALSE_REST — 가짜 휴식처

## Metadata
- floor: 3
- eventId: EVT_F03_FALSE_REST
- eventType: Event
- tone: 위험
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f03_false_rest_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
안전해 보이는 휴식처가 진짜인지 의심한다.

## Event Body
작은 모닥불과 정돈된 담요가 보인다. 너무 완벽해서 오히려 낯설다. 불빛은 따뜻하지만 그림자는 반대로 흔들린다.

## Cutscene Direction
- 장면: 완벽하게 정돈된 가짜 휴식처
- 중심 오브젝트: 모닥불과 반대로 흔들리는 그림자
- 인물 포함 여부: 없음
- 색감: 따뜻한 주황과 차가운 보라 대비
- 피해야 할 것: 진짜 Rest UI처럼 보이는 화면

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
False rest camp with neat blanket and warm fire, shadows moving in the wrong direction.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F03_FALSE_REST_REST
- label: 쉰다
- preview: HP +8 / Mental -3
- requirement: None
- effects:
  - ModifyHp(+8)
  - ModifyMental(-3)
  - ModifyGlitchLevel(+1)
- resultText: 몸은 가벼워졌지만 꿈에서 본 길이 계속 따라온다. 불빛은 어느새 꺼져 있다.
- mataiosIntent: 회복된 몸과 흔들린 표정을 함께 살핀다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F03_FALSE_REST_DOUBT
- label: 의심한다
- preview: Mental +2 / Gold +8
- requirement: None
- effects:
  - ModifyMental(+2)
  - ModifyGold(+8)
- resultText: 담요를 걷자 아래에 숨겨진 동전이 드러났다. 모닥불은 처음부터 차가웠다.
- mataiosIntent: 의심이 맞았다는 사실에 작게 긴장한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F03_FALSE_REST_LIGHT
- label: 다른 불을 밝힌다
- preview: StartCombat / AddItem
- requirement: None
- effects:
  - StartCombat(ENEMY_MERCENARY_MAGE_01)
  - AddItem(ITEM_LANTERN_OIL)
- resultText: 새 불이 켜지자 방의 그림자가 들켰다는 듯 일어났다. 싸움 뒤에는 기름병 하나가 남았다.
- mataiosIntent: 불빛 옆에 서서 전투를 준비한다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyHp(+8), ModifyMental(-3), ModifyGlitchLevel(+1), ModifyMental(+2), ModifyGold(+8), StartCombat(ENEMY_MERCENARY_MAGE_01), AddItem(ITEM_LANTERN_OIL)
- possible combat handoff: ENEMY_MERCENARY_MAGE_01
- possible item reward: ITEM_LANTERN_OIL
- possible memory unlock: None
- hidden values: ModifyGlitchLevel(+1) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
