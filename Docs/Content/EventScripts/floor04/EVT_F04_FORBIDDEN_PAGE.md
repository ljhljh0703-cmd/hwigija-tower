# EVT_F04_FORBIDDEN_PAGE — 금기의 페이지

## Metadata
- floor: 4
- eventId: EVT_F04_FORBIDDEN_PAGE
- eventType: Event
- tone: 위험
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f04_forbidden_page_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
사슬에 묶인 페이지를 읽거나 태우거나 보관한다.

## Event Body
사슬에 묶인 책장이 허공에 떠 있다. 한 장만 찢겨 나와 천천히 펄럭인다. 글자는 읽기 직전마다 다른 모양으로 바뀐다.

## Cutscene Direction
- 장면: 사슬에 묶인 책장과 떠 있는 페이지
- 중심 오브젝트: 금기의 페이지
- 인물 포함 여부: 없음
- 색감: 잉크 검정, 탁한 보라, 낡은 양피지색
- 피해야 할 것: 읽을 수 있는 실제 문장

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Forbidden page chained in midair, fluttering parchment, shifting unreadable ink.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F04_FORBIDDEN_PAGE_READ
- label: 읽는다
- preview: Mental -4 / AddAbility
- requirement: None
- effects:
  - ModifyMental(-4)
  - AddAbility(ABILITY_ARTS_03)
  - ModifyGlitchLevel(+1)
- resultText: 뜻을 이해하기 전에 머리가 먼저 아파졌다. 손끝에는 번개 같은 떨림이 남았다.
- mataiosIntent: 읽은 내용을 묻지 않고 상태를 살핀다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F04_FORBIDDEN_PAGE_BURN
- label: 태운다
- preview: HP -3 / 내부 불안정 완화
- requirement: None
- effects:
  - ModifyHp(-3)
  - ModifyGlitchLevel(-2)
- resultText: 페이지는 잘 타지 않았다. 재가 된 뒤에야 방 안의 압력이 내려갔다.
- mataiosIntent: 태우는 선택을 조용히 지지한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F04_FORBIDDEN_PAGE_KEEP
- label: 접어 보관한다
- preview: AddItem / Mental -2
- requirement: None
- effects:
  - AddItem(ITEM_10)
  - ModifyMental(-2)
- resultText: 페이지는 접히지 않으려 했지만 결국 품 안에 들어왔다. 몸 안쪽이 조금 차가워졌다.
- mataiosIntent: 보관한 물건을 불안하게 바라본다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyMental(-4), AddAbility(ABILITY_ARTS_03), ModifyGlitchLevel(+1), ModifyHp(-3), ModifyGlitchLevel(-2), AddItem(ITEM_10), ModifyMental(-2)
- possible combat handoff: None
- possible item reward: ITEM_10
- possible memory unlock: None
- hidden values: ModifyGlitchLevel(+1/-2) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
