# EVT_F05_KINGS_ECHO — 왕의 잔향

## Metadata
- floor: 5
- eventId: EVT_F05_KINGS_ECHO
- eventType: Event
- tone: 기억
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f05_kings_echo_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
왕의 잔향을 듣거나 반박하거나 침묵한다.

## Event Body
빈 홀에 낡은 깃발이 걸려 있다. 왕좌는 없지만 명령을 기다리는 듯한 공기가 남아 있다. 낮은 목소리가 벽을 타고 되돌아온다.

## Cutscene Direction
- 장면: 왕좌가 사라진 빈 홀
- 중심 오브젝트: 낡은 왕국 깃발
- 인물 포함 여부: 없음
- 색감: 빛바랜 금색, 검은 대리석, 차가운 백색
- 피해야 할 것: 왕의 정체 확정, 엔딩 진실 노출

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Empty royal hall without a throne, old banner, echoing invisible voice.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F05_KINGS_ECHO_LISTEN
- label: 듣는다
- preview: Mental -3 / AddRunBuff
- requirement: None
- effects:
  - ModifyMental(-3)
  - AddRunBuff(BUFF_FINAL_BOSS_INSIGHT_01)
- resultText: 목소리는 끝까지 명령처럼 들렸다. 하지만 어디를 노리는지 조금은 알 것 같다.
- mataiosIntent: 그 목소리를 싫어하는지 그리워하는지 판단하지 못한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F05_KINGS_ECHO_DENY
- label: 반박한다
- preview: HP -5 / Affinity +2
- requirement: None
- effects:
  - ModifyHp(-5)
  - ModifyAffinity(+2)
- resultText: 말이 끝나자 홀의 압력이 몸을 눌렀다. 그래도 깃발은 잠깐 흔들렸다.
- mataiosIntent: 반박한 용기를 조용히 받아들인다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F05_KINGS_ECHO_SILENCE
- label: 침묵한다
- preview: Mental +2
- requirement: None
- effects:
  - ModifyMental(+2)
- resultText: 아무 말도 하지 않았다. 목소리는 대답을 얻지 못하고 벽 속으로 스며들었다.
- mataiosIntent: 침묵을 함께 지킨다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyMental(-3), AddRunBuff(BUFF_FINAL_BOSS_INSIGHT_01), ModifyHp(-5), ModifyAffinity(+2), ModifyMental(+2)
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
