# EVT_F03_NAMELESS_DOOR — 이름 없는 문

## Metadata
- floor: 3
- eventId: EVT_F03_NAMELESS_DOOR
- eventType: Event
- tone: 기억
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f03_nameless_door_bg.png
- writerStatus: draft
- runtimePriority: P1

## Player-Facing Summary
이름 없는 문 앞에서 열기, 표시 남기기, 지나가기를 선택한다.

## Event Body
문패가 있어야 할 자리가 비어 있다. 손잡이는 여러 번 잡힌 듯 반들반들하다. 문 너머에서는 아무 소리도 나지 않는다.

## Cutscene Direction
- 장면: 문패 없는 낡은 문
- 중심 오브젝트: 반들거리는 손잡이
- 인물 포함 여부: 없음
- 색감: 낡은 목재색, 어두운 금속색
- 피해야 할 것: 읽을 수 있는 이름표

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Nameless old wooden door with polished handle, empty nameplate space, dim corridor.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F03_NAMELESS_DOOR_OPEN
- label: 문을 연다
- preview: StartCombat / Gold +10
- requirement: None
- effects:
  - StartCombat(ENEMY_EMPTY_ARMOR)
  - ModifyGold(+10)
- resultText: 문 안쪽에는 빈 갑옷이 서 있었다. 바닥에는 이미 버려진 금화가 흩어져 있다.
- mataiosIntent: 문 너머의 침묵을 경계한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F03_NAMELESS_DOOR_MARK
- label: 표시를 남긴다
- preview: Affinity +2 / UnlockMemory
- requirement: None
- effects:
  - ModifyAffinity(+2)
  - UnlockMemory(MEM_FRAGMENT_02)
- resultText: 문 옆에 짧은 획을 남겼다. 기억의 잔향이 획 사이에 걸린다.
- mataiosIntent: 그 획을 오래 바라본다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F03_NAMELESS_DOOR_PASS
- label: 지나간다
- preview: Mental +1
- requirement: None
- effects:
  - ModifyMental(+1)
- resultText: 문은 끝까지 열리지 않았다. 등 뒤에서 손잡이가 아주 작게 움직인 것 같았다.
- mataiosIntent: 뒤돌아보지 않고 걸음을 맞춘다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: StartCombat(ENEMY_EMPTY_ARMOR), ModifyGold(+10), ModifyAffinity(+2), UnlockMemory(MEM_FRAGMENT_02), ModifyMental(+1)
- possible combat handoff: ENEMY_EMPTY_ARMOR
- possible item reward: None
- possible memory unlock: MEM_FRAGMENT_02
- hidden values: None
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
