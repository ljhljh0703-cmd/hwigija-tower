# EVT_F01_LOCKED_CHEST — 잠긴 상자

## Metadata
- floor: 1
- eventId: EVT_F01_LOCKED_CHEST
- eventType: Event
- tone: 위험
- riskLevel: low
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f01_locked_chest_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
잠긴 상자를 열 방법을 고르며 낮은 비용의 보상 선택을 익힌다.

## Event Body
복도 끝에 장식이 벗겨진 상자 하나가 놓여 있다. 자물쇠는 낡았지만 아직 단단하다. 안에서는 작은 금속음이 난다.

## Cutscene Direction
- 장면: 좁은 복도 끝의 낡은 보물상자
- 중심 오브젝트: 녹슨 자물쇠
- 인물 포함 여부: 없음
- 색감: 어두운 금속색, 먼지 낀 금빛
- 피해야 할 것: 화려한 보물 폭발, 읽을 수 있는 문장

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Old locked chest at the end of a narrow stone corridor, rusty lock, faint coins visible through a crack.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F01_LOCKED_CHEST_BREAK
- label: 자물쇠를 부순다
- preview: HP -2 / Gold +8
- requirement: None
- effects:
  - ModifyHp(-2)
  - ModifyGold(+8)
- resultText: 자물쇠는 부서졌지만 손등이 찢어졌다. 상자 안의 동전은 차갑다.
- mataiosIntent: 성급함을 걱정하지만 결과는 받아들인다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F01_LOCKED_CHEST_INSPECT
- label: 틈을 조사한다
- preview: 50%: AddItem / 50%: 아무 일 없음
- requirement: None
- effects:
  - AddItem(ITEM_06)
- resultText: 상자 아래의 작은 틈에서 동전 하나가 먼저 굴러나왔다. 잠금쇠는 그대로다.
- mataiosIntent: 신중한 선택을 긍정적으로 본다.
- probabilityHint: 50% item / 50% no reward
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F01_LOCKED_CHEST_LEAVE
- label: 포기한다
- preview: Mental +1
- requirement: None
- effects:
  - ModifyMental(+1)
- resultText: 상자를 그대로 두고 지나쳤다. 손해 본 것은 없다고 스스로 납득했다.
- mataiosIntent: 불필요한 위험을 피한 것을 조용히 기억한다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyHp(-2), ModifyGold(+8), AddItem(ITEM_06), ModifyMental(+1)
- possible combat handoff: None
- possible item reward: ITEM_06
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
