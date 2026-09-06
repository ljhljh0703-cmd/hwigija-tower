# EVT_F05_BURNING_MARK — 불타는 낙인

## Metadata
- floor: 5
- eventId: EVT_F05_BURNING_MARK
- eventType: Event
- tone: 위험
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f05_burning_mark_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
몸에 떠오른 낙인을 견디거나 식히거나 옮긴다.

## Event Body
손등에 오래된 낙인이 떠오른다. 뜨겁지만 빛은 차갑다. 낙인은 맥박에 맞춰 조금씩 넓어진다.

## Cutscene Direction
- 장면: 최상층 복도의 손등 위 낙인 클로즈업
- 중심 오브젝트: 불타는 낙인
- 인물 포함 여부: 손 일부만
- 색감: 차가운 백색 불빛, 검은 피부 그림자
- 피해야 할 것: 문자처럼 읽히는 낙인, 노골적 화상

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Close-up of a hand with a cold burning mark in a final floor corridor.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F05_BURNING_MARK_ENDURE
- label: 견딘다
- preview: HP -7 / 최종 보스 피해 강화
- requirement: None
- effects:
  - ModifyHp(-7)
  - AddRunBuff(BUFF_FINAL_BOSS_DAMAGE_02)
- resultText: 통증은 오래 갔지만 낙인은 더 이상 번지지 않았다. 대신 무기가 뜨거워졌다.
- mataiosIntent: 아픔을 견디는 모습을 불안하게 지켜본다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F05_BURNING_MARK_COOL
- label: 식힌다
- preview: Mental +2 / HP +4
- requirement: None
- effects:
  - ModifyMental(+2)
  - ModifyHp(+4)
- resultText: 찬 돌에 손을 대자 빛이 가라앉았다. 숨이 조금 돌아왔다.
- mataiosIntent: 회복을 확인하고 안도한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F05_BURNING_MARK_TRANSFER
- label: 부적에 옮긴다
- preview: HasItem(ITEM_09) / AddRunBuff
- requirement: HasItem(ITEM_09)
- effects:
  - AddRunBuff(BUFF_FINAL_BOSS_DEFENSE_02)
  - ModifyGlitchLevel(+1)
- resultText: 부적이 낙인의 열을 대신 머금었다. 더 오래 버틸 수 있을 것 같다.
- mataiosIntent: 부적의 변화를 조심스럽게 살핀다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: HasItem(ITEM_09)
- effect DTO: ModifyHp(-7), AddRunBuff(BUFF_FINAL_BOSS_DAMAGE_02), ModifyMental(+2), ModifyHp(+4), AddRunBuff(BUFF_FINAL_BOSS_DEFENSE_02), ModifyGlitchLevel(+1)
- possible combat handoff: None
- possible item reward: None
- possible memory unlock: None
- hidden values: ModifyGlitchLevel(+1) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
