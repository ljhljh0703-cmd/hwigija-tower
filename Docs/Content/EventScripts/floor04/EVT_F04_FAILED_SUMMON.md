# EVT_F04_FAILED_SUMMON — 실패한 소환진

## Metadata
- floor: 4
- eventId: EVT_F04_FAILED_SUMMON
- eventType: Event
- tone: 전투유도
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f04_failed_summon_bg.png
- writerStatus: draft
- runtimePriority: P1

## Player-Facing Summary
실패한 소환진을 정리하거나 이용하거나 조용히 물러선다.

## Event Body
바닥의 소환진은 반쯤 지워져 있다. 중심에는 타다 만 초와 검은 재가 둥글게 남았다. 선 밖으로 무언가 기어 나온 흔적이 있다.

## Cutscene Direction
- 장면: 반쯤 지워진 소환진
- 중심 오브젝트: 검은 재와 꺼진 초
- 인물 포함 여부: 없음
- 색감: 검은 재, 어두운 붉은 선, 차가운 청색
- 피해야 할 것: 완전한 악마 소환 묘사, 읽을 수 있는 주문

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Failed summoning circle half erased on stone floor, black ash, extinguished candles.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F04_FAILED_SUMMON_CLEAN
- label: 선을 정리한다
- preview: Mental +2 / 다음 보스 방어 도움
- requirement: None
- effects:
  - ModifyMental(+2)
  - AddRunBuff(BUFF_NEXT_BOSS_DEFENSE_01)
- resultText: 흐트러진 선을 지우자 방이 평평해졌다. 다음 큰 싸움에서 흔들림이 줄 것 같다.
- mataiosIntent: 정리된 바닥을 보고 숨을 고른다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F04_FAILED_SUMMON_USE
- label: 남은 힘을 이용한다
- preview: HP -8 / 강한 공격 버프
- requirement: None
- effects:
  - ModifyHp(-8)
  - AddRunBuff(BUFF_NEXT_BOSS_DAMAGE_02)
  - ModifyGlitchLevel(+2)
- resultText: 꺼진 초가 잠깐 다시 타올랐다. 몸은 무거워졌지만 공격의 감각은 선명하다.
- mataiosIntent: 대가가 과한 선택인지 걱정한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F04_FAILED_SUMMON_STEP_BACK
- label: 물러선다
- preview: Affinity +1
- requirement: None
- effects:
  - ModifyAffinity(+1)
- resultText: 소환진은 그대로 두었다. 문을 닫자 안쪽에서 재가 흩어지는 소리만 났다.
- mataiosIntent: 함부로 건드리지 않은 선택을 받아들인다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyMental(+2), AddRunBuff(BUFF_NEXT_BOSS_DEFENSE_01), ModifyHp(-8), AddRunBuff(BUFF_NEXT_BOSS_DAMAGE_02), ModifyGlitchLevel(+2), ModifyAffinity(+1)
- possible combat handoff: None
- possible item reward: None
- possible memory unlock: None
- hidden values: ModifyGlitchLevel(+2) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
