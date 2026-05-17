# EVT_F05_MYTH_SHARD — 신화의 파편

## Metadata
- floor: 5
- eventId: EVT_F05_MYTH_SHARD
- eventType: Event
- tone: 기억
- riskLevel: high
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f05_myth_shard_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
신화의 파편을 만지거나 기록하거나 봉인한다.

## Event Body
공중에 떠 있는 얇은 파편이 천천히 돈다. 돌도 유리도 아닌 것이 오래된 이야기처럼 빛난다. 가까이 가면 잊은 이름들이 목 안쪽에서 맴돈다.

## Cutscene Direction
- 장면: 허공에 떠 있는 신화의 파편
- 중심 오브젝트: 유리 같은 파편
- 인물 포함 여부: 없음
- 색감: 흰 금빛, 깊은 남색, 검은 공허
- 피해야 할 것: 세계관 진실 설명, 읽을 수 있는 글자

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Floating shard of myth in a void-like chamber, glass-like fragment glowing pale gold.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F05_MYTH_SHARD_TOUCH
- label: 만진다
- preview: UnlockMemory / Mental -4
- requirement: None
- effects:
  - UnlockMemory(MEM_FRAGMENT_05)
  - ModifyMental(-4)
  - ModifyGlitchLevel(+1)
- resultText: 파편은 차갑지 않았다. 기억의 잔향이 너무 가까운 곳에서 울렸다.
- mataiosIntent: 무언가를 거의 떠올릴 듯하지만 멈춘다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F05_MYTH_SHARD_RECORD
- label: 기록한다
- preview: Affinity +2 / AddItem
- requirement: None
- effects:
  - ModifyAffinity(+2)
  - AddItem(RELIC_GENERIC_02)
- resultText: 본 것을 다 적지는 못했다. 그래도 남은 메모는 이상하게 따뜻했다.
- mataiosIntent: 메모를 알아보는 듯하다가 시선을 돌린다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F05_MYTH_SHARD_SEAL
- label: 봉인한다
- preview: Mental +3 / 내부 불안정 완화
- requirement: None
- effects:
  - ModifyMental(+3)
  - ModifyGlitchLevel(-2)
- resultText: 파편은 천천히 어두워졌다. 방 안의 소리도 함께 낮아졌다.
- mataiosIntent: 봉인된 파편 앞에서 오래 침묵한다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: UnlockMemory(MEM_FRAGMENT_05), ModifyMental(-4), ModifyGlitchLevel(+1), ModifyAffinity(+2), AddItem(RELIC_GENERIC_02), ModifyMental(+3), ModifyGlitchLevel(-2)
- possible combat handoff: None
- possible item reward: RELIC_GENERIC_02
- possible memory unlock: MEM_FRAGMENT_05
- hidden values: ModifyGlitchLevel(+1/-2) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
