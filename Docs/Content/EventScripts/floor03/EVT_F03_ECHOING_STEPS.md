# EVT_F03_ECHOING_STEPS — 뒤따라오는 발소리

## Metadata
- floor: 3
- eventId: EVT_F03_ECHOING_STEPS
- eventType: Event
- tone: 기억
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f03_echoing_steps_bg.png
- writerStatus: draft
- runtimePriority: P1

## Player-Facing Summary
뒤따르는 발소리에 반응해 전투, 정신 회복, 관계 변화를 만든다.

## Event Body
발소리가 하나 더 있다. 멈추면 멈추고, 걸으면 같은 박자로 따라온다. 바로 뒤를 돌아봐도 복도는 비어 있다.

## Cutscene Direction
- 장면: 끝이 긴 복도와 반복되는 그림자
- 중심 오브젝트: 젖은 발자국
- 인물 포함 여부: 없음
- 색감: 먹색, 희미한 흰 안개
- 피해야 할 것: 점프스케어, 괴물 실루엣 과노출

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Long empty corridor with wet footprints appearing behind the player, soft fog, no visible creature.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F03_ECHOING_STEPS_WAIT
- label: 기다린다
- preview: Mental +2 / Affinity +1
- requirement: None
- effects:
  - ModifyMental(+2)
  - ModifyAffinity(+1)
- resultText: 한참 뒤에 발소리도 멎었다. 그 침묵이 이상하게 위로처럼 남았다.
- mataiosIntent: 같이 멈춰 선 시간을 기억한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F03_ECHOING_STEPS_RUN
- label: 빠르게 걷는다
- preview: HP -4 / 다음 전투 피해 강화
- requirement: None
- effects:
  - ModifyHp(-4)
  - AddRunBuff(BUFF_NEXT_COMBAT_DAMAGE_01)
- resultText: 숨이 턱까지 차올랐지만 발소리는 멀어졌다. 무기는 더 단단히 쥐어져 있다.
- mataiosIntent: 서두르는 이유를 알 것 같다는 듯 따라온다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F03_ECHOING_STEPS_SPEAK
- label: 말을 건다
- preview: 50%: Affinity +2 / 50%: Mental -2
- requirement: None
- effects:
  - ModifyAffinity(+2)
  - ModifyMental(-2)
- resultText: 대답은 없었다. 대신 발소리가 잠깐 둘이 아니라 셋처럼 들렸다.
- mataiosIntent: 대화의 시도를 조심스럽게 받아들인다.
- probabilityHint: 50% calm / 50% unease
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyMental(+2), ModifyAffinity(+1), ModifyHp(-4), AddRunBuff(BUFF_NEXT_COMBAT_DAMAGE_01), ModifyAffinity(+2), ModifyMental(-2)
- possible combat handoff: None
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
