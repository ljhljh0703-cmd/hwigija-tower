# EVT_F04_CRYSTAL_SERVANT — 결정화된 하인

## Metadata
- floor: 4
- eventId: EVT_F04_CRYSTAL_SERVANT
- eventType: Event
- tone: 도덕선택
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f04_crystal_servant_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
결정화된 하인을 구할지 채굴할지 피할지 고른다.

## Event Body
노란 결정에 반쯤 파묻힌 하인이 벽에 기대어 있다. 손은 아직 쟁반을 든 모양으로 굳어 있다. 결정 안쪽에서 아주 작은 호흡이 보이는 듯하다.

## Cutscene Direction
- 장면: 결정에 잠긴 하인이 있는 복도
- 중심 오브젝트: 노란 결정과 굳은 쟁반
- 인물 포함 여부: 하인 형상 포함
- 색감: 누런 금빛, 창백한 피부색, 차가운 그림자
- 피해야 할 것: 고어, 직접적인 고통 묘사

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Servant half encased in yellow crystal, frozen posture holding a tray, gothic corridor.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F04_CRYSTAL_SERVANT_SAVE
- label: 꺼내려 한다
- preview: HP -7 / Affinity +3
- requirement: None
- effects:
  - ModifyHp(-7)
  - ModifyAffinity(+3)
  - ModifyGlitchLevel(-1)
- resultText: 결정은 손을 베며 조금씩 떨어졌다. 하인의 손가락이 잠깐 움직인 듯했다.
- mataiosIntent: 구하려는 선택을 오래 기억한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F04_CRYSTAL_SERVANT_MINE
- label: 결정을 떼어낸다
- preview: Gold +16 / Affinity -3
- requirement: None
- effects:
  - ModifyGold(+16)
  - ModifyAffinity(-3)
  - ModifyGlitchLevel(+1)
- resultText: 떼어낸 결정은 무겁고 값나가 보인다. 벽에 남은 손 모양은 더 작아졌다.
- mataiosIntent: 선택을 비난하지 않지만 말수가 줄어든다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F04_CRYSTAL_SERVANT_AVOID
- label: 건드리지 않는다
- preview: Mental +1
- requirement: None
- effects:
  - ModifyMental(+1)
- resultText: 그대로 지나쳤다. 쟁반 위의 먼지만 아주 천천히 내려앉았다.
- mataiosIntent: 지나치는 이유를 묻지 않는다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyHp(-7), ModifyAffinity(+3), ModifyGlitchLevel(-1), ModifyGold(+16), ModifyAffinity(-3), ModifyGlitchLevel(+1), ModifyMental(+1)
- possible combat handoff: None
- possible item reward: None
- possible memory unlock: None
- hidden values: ModifyGlitchLevel(+/-1) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
