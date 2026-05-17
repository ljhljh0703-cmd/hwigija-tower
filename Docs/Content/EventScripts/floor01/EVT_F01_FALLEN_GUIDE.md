# EVT_F01_FALLEN_GUIDE — 쓰러진 안내자 흔적

## Metadata
- floor: 1
- eventId: EVT_F01_FALLEN_GUIDE
- eventType: Event
- tone: 도덕선택
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f01_fallen_guide_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
쓰러진 안내자의 흔적을 어떻게 다룰지 선택한다.

## Event Body
계단 아래에 낡은 지도와 부러진 지팡이가 떨어져 있다. 주인은 보이지 않지만 먼지는 아직 완전히 내려앉지 않았다. 누군가 이곳에서 길을 잃었다.

## Cutscene Direction
- 장면: 탑 입구 계단 아래의 지도와 부러진 지팡이
- 중심 오브젝트: 찢어진 지도와 손때 묻은 지팡이
- 인물 포함 여부: 없음
- 색감: 희미한 녹색, 차가운 돌색
- 피해야 할 것: 시체 노출, 명확한 사망 설명

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Stair landing with a torn map and broken wooden staff, no body, dust and cold wind.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F01_FALLEN_GUIDE_HELP
- label: 흔적을 정리한다
- preview: Affinity +2 / Mental +1
- requirement: None
- effects:
  - ModifyAffinity(+2)
  - ModifyMental(+1)
- resultText: 지도와 지팡이를 벽가에 가지런히 두었다. 방 안의 소리가 조금 낮아졌다.
- mataiosIntent: 작은 배려를 오래 기억할 듯 침묵한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F01_FALLEN_GUIDE_LOOT
- label: 쓸 만한 것을 챙긴다
- preview: AddItem / Affinity -1
- requirement: None
- effects:
  - AddItem(ITEM_11)
  - ModifyAffinity(-1)
- resultText: 지도 조각 사이에서 녹슨 나침반을 찾았다. 바늘은 느리게 흔들린다.
- mataiosIntent: 선택을 막지는 않지만 거리를 둔다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F01_FALLEN_GUIDE_PASS
- label: 지나간다
- preview: 아무 일 없음
- requirement: None
- effects:
  - ModifyMental(+0)
- resultText: 흔적은 그대로 남았다. 계단 위의 바람이 조금 더 차갑다.
- mataiosIntent: 판단을 보류하고 주변을 살핀다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyAffinity(+2), ModifyMental(+1), AddItem(ITEM_11), ModifyAffinity(-1), ModifyMental(+0)
- possible combat handoff: None
- possible item reward: ITEM_11
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
