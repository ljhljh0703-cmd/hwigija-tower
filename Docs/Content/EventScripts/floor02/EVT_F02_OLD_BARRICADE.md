# EVT_F02_OLD_BARRICADE — 낡은 바리케이드

## Metadata
- floor: 2
- eventId: EVT_F02_OLD_BARRICADE
- eventType: Event
- tone: 전투유도
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f02_old_barricade_bg.png
- writerStatus: draft
- runtimePriority: P1

## Player-Facing Summary
낡은 바리케이드를 부수거나 보강해 다음 전투를 준비한다.

## Event Body
복도 중간을 낡은 목책이 막고 있다. 여기서 누군가 오래 버틴 흔적이 남아 있다. 목책 너머에서는 낮은 숨소리가 들린다.

## Cutscene Direction
- 장면: 피로 낡은 목책과 철책 흔적
- 중심 오브젝트: 부서진 바리케이드
- 인물 포함 여부: 없음
- 색감: 마른 목재색, 녹슨 철색, 낮은 붉은빛
- 피해야 할 것: 과한 시체 묘사

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Old wooden barricade blocking a stone corridor, claw marks, low breathing beyond it.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F02_OLD_BARRICADE_BREAK
- label: 부순다
- preview: StartCombat / Gold +8
- requirement: None
- effects:
  - StartCombat(ENEMY_FRACTURE_HOUND)
  - ModifyGold(+8)
- resultText: 목책이 부서지자 안쪽의 것이 튀어나왔다. 남은 물자는 싸움 뒤에 챙길 수 있다.
- mataiosIntent: 전투 준비를 돕되 긴장한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F02_OLD_BARRICADE_REINFORCE
- label: 보강한다
- preview: HP -3 / 다음 전투 방어 강화
- requirement: None
- effects:
  - ModifyHp(-3)
  - AddRunBuff(BUFF_NEXT_COMBAT_DEFENSE_01)
- resultText: 손바닥에 가시가 박혔지만 틈은 줄었다. 다음 싸움에서 한 번은 버틸 수 있을 것 같다.
- mataiosIntent: 준비하는 선택을 긍정적으로 본다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F02_OLD_BARRICADE_TURN_AROUND
- label: 돌아선다
- preview: Mental +1
- requirement: None
- effects:
  - ModifyMental(+1)
- resultText: 목책을 그대로 두고 다른 길을 잡았다. 뒤쪽의 숨소리는 멀어졌다.
- mataiosIntent: 싸움을 피한 이유를 묻지 않고 따라온다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: StartCombat(ENEMY_FRACTURE_HOUND), ModifyGold(+8), ModifyHp(-3), AddRunBuff(BUFF_NEXT_COMBAT_DEFENSE_01), ModifyMental(+1)
- possible combat handoff: ENEMY_FRACTURE_HOUND
- possible item reward: None
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
