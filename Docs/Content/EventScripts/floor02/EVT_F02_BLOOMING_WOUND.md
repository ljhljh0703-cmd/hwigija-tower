# EVT_F02_BLOOMING_WOUND — 피어난 상처

## Metadata
- floor: 2
- eventId: EVT_F02_BLOOMING_WOUND
- eventType: Event
- tone: 위험
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f02_blooming_wound_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
피처럼 핀 꽃을 건드릴지 태울지 선택한다.

## Event Body
벽의 갈라진 틈에서 붉은 꽃이 피어 있다. 꽃잎은 상처처럼 젖어 있고, 가까이 가면 살갗이 따끔거린다. 꽃은 아주 느리게 벌어진다.

## Cutscene Direction
- 장면: 갈라진 벽과 붉은 꽃
- 중심 오브젝트: 상처처럼 젖은 꽃
- 인물 포함 여부: 없음
- 색감: 붉은 꽃잎, 습한 검정, 병든 녹색
- 피해야 할 것: 노골적인 신체 훼손

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Cracked stone wall with a wet red flower blooming like a wound, dim magical light.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F02_BLOOMING_WOUND_TOUCH
- label: 꽃을 만진다
- preview: HP -4 / AddAbility
- requirement: None
- effects:
  - ModifyHp(-4)
  - AddAbility(ABILITY_ARTS_01)
- resultText: 손끝이 타는 듯 아팠다. 대신 뜨거운 문양이 잠시 손등에 떠올랐다.
- mataiosIntent: 아픔을 대가로 힘을 얻는 선택을 불안하게 본다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F02_BLOOMING_WOUND_BURN
- label: 꽃을 태운다
- preview: 다음 전투 피해 강화 / Mental -1
- requirement: None
- effects:
  - AddRunBuff(BUFF_NEXT_COMBAT_DAMAGE_01)
  - ModifyMental(-1)
- resultText: 꽃은 타면서 짧은 비명을 닮은 소리를 냈다. 그 재가 무기에 얇게 묻었다.
- mataiosIntent: 필요한 잔혹함인지 판단하지 못한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F02_BLOOMING_WOUND_STEP_BACK
- label: 물러선다
- preview: Mental +1
- requirement: None
- effects:
  - ModifyMental(+1)
- resultText: 꽃은 다시 닫혔다. 아무것도 얻지 못했지만 피부의 따끔거림도 사라졌다.
- mataiosIntent: 위험을 피한 선택을 침착하게 받아들인다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyHp(-4), AddAbility(ABILITY_ARTS_01), AddRunBuff(BUFF_NEXT_COMBAT_DAMAGE_01), ModifyMental(-1), ModifyMental(+1)
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
