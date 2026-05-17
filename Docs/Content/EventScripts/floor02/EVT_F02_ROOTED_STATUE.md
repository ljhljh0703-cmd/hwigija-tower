# EVT_F02_ROOTED_STATUE — 뿌리에 감긴 석상

## Metadata
- floor: 2
- eventId: EVT_F02_ROOTED_STATUE
- eventType: Event
- tone: 위험
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f02_rooted_statue_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
뿌리에 감긴 석상을 힘, 관찰, 기도로 다룬다.

## Event Body
정원 깊은 곳에 석상 하나가 반쯤 묻혀 있다. 굵은 뿌리가 팔과 목을 감고 있고 돌 눈은 문 쪽을 향한다. 뿌리 아래에서 약한 맥동이 느껴진다.

## Cutscene Direction
- 장면: 녹음이 우거진 탑 내부 정원
- 중심 오브젝트: 뿌리에 감긴 석상
- 인물 포함 여부: 없음
- 색감: 축축한 녹색, 누런 돌색
- 피해야 할 것: 밝은 정원, 귀여운 식물

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Overgrown indoor garden, stone statue strangled by thick roots, damp floor, faint pulse under roots.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F02_ROOTED_STATUE_PULL
- label: 힘으로 뽑는다
- preview: HP -4 / AddItem
- requirement: None
- effects:
  - ModifyHp(-4)
  - AddItem(ITEM_04)
- resultText: 뿌리가 손목을 파고들었지만 석상 조각 하나가 떨어졌다. 방패처럼 단단하다.
- mataiosIntent: 무리한 돌파를 걱정한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F02_ROOTED_STATUE_INSPECT
- label: 조심히 조사한다
- preview: Mental +1 / Gold +6
- requirement: None
- effects:
  - ModifyMental(+1)
  - ModifyGold(+6)
- resultText: 뿌리 아래에 숨겨진 동전을 찾았다. 석상은 끝까지 문만 바라본다.
- mataiosIntent: 차분한 관찰을 따라 한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F02_ROOTED_STATUE_PRAY
- label: 잠깐 기도한다
- preview: Affinity +2 / 내부 불안정 완화
- requirement: None
- effects:
  - ModifyAffinity(+2)
  - ModifyGlitchLevel(-1)
- resultText: 기도가 끝나자 뿌리의 맥동이 느려졌다. 길은 조금 더 조용해졌다.
- mataiosIntent: 이상하게 익숙한 침묵으로 반응한다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyHp(-4), AddItem(ITEM_04), ModifyMental(+1), ModifyGold(+6), ModifyAffinity(+2), ModifyGlitchLevel(-1)
- possible combat handoff: None
- possible item reward: ITEM_04
- possible memory unlock: None
- hidden values: ModifyGlitchLevel(-1) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
