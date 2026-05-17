# EVT_F04_SILENT_ALTAR — 말 없는 제단

## Metadata
- floor: 4
- eventId: EVT_F04_SILENT_ALTAR
- eventType: Event
- tone: 도덕선택
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f04_silent_altar_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
말 없는 제단에 무엇을 바칠지 선택한다.

## Event Body
촛불이 없는 제단 위에 빈 그릇이 놓여 있다. 그릇 안은 어둡고, 가까이 서면 심장 박동만 크게 들린다. 제단은 아무 말도 하지 않는다.

## Cutscene Direction
- 장면: 촛불 없는 검은 제단
- 중심 오브젝트: 빈 그릇
- 인물 포함 여부: 없음
- 색감: 검은 돌, 어두운 적색, 낮은 금빛
- 피해야 할 것: 종교 문장, 직접적인 신 이름

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Black stone altar with an empty bowl, no candles, oppressive silence.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F04_SILENT_ALTAR_BLOOD
- label: 피를 바친다
- preview: HP -8 / 강한 전투 버프
- requirement: None
- effects:
  - ModifyHp(-8)
  - AddRunBuff(BUFF_NEXT_2_COMBATS_DAMAGE_02)
  - ModifyGlitchLevel(+1)
- resultText: 그릇은 아주 조금만 채워졌는데도 무겁게 가라앉았다. 몸은 비었고 무기는 가볍다.
- mataiosIntent: 대가가 큰 선택을 불안하게 지켜본다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F04_SILENT_ALTAR_GOLD
- label: 금을 바친다
- preview: Gold -12 / AddItem
- requirement: StatAtLeast(gold,12)
- effects:
  - ModifyGold(-12)
  - AddItem(RELIC_GENERIC_01)
- resultText: 동전은 소리 없이 사라졌다. 그 자리에 낡은 코트 조각이 남았다.
- mataiosIntent: 물건의 온기를 조심스럽게 확인한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F04_SILENT_ALTAR_BREAK
- label: 제단을 부순다
- preview: StartCombat / AddReward
- requirement: None
- effects:
  - StartCombat(ENEMY_SHADE_03)
  - GrantRewardBundle(REWARD_CACHE_SMALL)
- resultText: 제단의 금이 벌어지자 어둠이 사람 모양으로 일어났다. 남은 조각은 보상처럼 빛났다.
- mataiosIntent: 부순 뒤의 침묵을 경계한다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: StatAtLeast(gold,12)
- effect DTO: ModifyHp(-8), AddRunBuff(BUFF_NEXT_2_COMBATS_DAMAGE_02), ModifyGlitchLevel(+1), ModifyGold(-12), AddItem(RELIC_GENERIC_01), StartCombat(ENEMY_SHADE_03), GrantRewardBundle(REWARD_CACHE_SMALL)
- possible combat handoff: ENEMY_SHADE_03
- possible item reward: RELIC_GENERIC_01
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
