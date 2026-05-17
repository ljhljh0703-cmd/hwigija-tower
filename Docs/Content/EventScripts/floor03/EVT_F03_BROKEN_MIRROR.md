# EVT_F03_BROKEN_MIRROR — 깨진 거울

## Metadata
- floor: 3
- eventId: EVT_F03_BROKEN_MIRROR
- eventType: Event
- tone: 기억
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f03_broken_mirror_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
깨진 거울을 통해 기억의 잔향을 얻거나 혼란을 피한다.

## Event Body
복도 벽에 금 간 거울이 걸려 있다. 거울 안쪽의 방은 지금 서 있는 곳과 조금 다르다. 비친 얼굴도 한 박자 늦게 움직인다.

## Cutscene Direction
- 장면: 금 간 거울이 걸린 어두운 복도
- 중심 오브젝트: 깨진 전신 거울
- 인물 포함 여부: 흐릿한 반사만
- 색감: 은색 파편, 차가운 남색
- 피해야 할 것: 선명한 얼굴, 최종 정체 암시

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Cracked full-length mirror in a dark corridor, delayed reflection, cloth nearby.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F03_BROKEN_MIRROR_LOOK
- label: 들여다본다
- preview: UnlockMemory / Mental -2
- requirement: None
- effects:
  - UnlockMemory(MEM_FRAGMENT_03)
  - ModifyMental(-2)
  - ModifyGlitchLevel(+1)
- resultText: 거울 안에서 낯선 길 조각이 겹쳐 보였다. 기억의 잔향 하나가 손끝에 남았다.
- mataiosIntent: 무언가 떠올리려 하지만 확신하지 못한다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F03_BROKEN_MIRROR_BREAK
- label: 깨뜨린다
- preview: HP -4 / AddItem
- requirement: None
- effects:
  - ModifyHp(-4)
  - AddItem(ITEM_03)
- resultText: 거울이 부서지고 날카로운 조각이 남았다. 피가 조금 났지만 쓸 만하다.
- mataiosIntent: 급한 결단에 놀라며 주변을 살핀다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F03_BROKEN_MIRROR_COVER
- label: 천으로 덮는다
- preview: Mental +2 / Affinity +1
- requirement: None
- effects:
  - ModifyMental(+2)
  - ModifyAffinity(+1)
- resultText: 거울을 덮자 늦게 움직이던 그림자도 사라졌다. 복도가 제 속도를 되찾는다.
- mataiosIntent: 덮인 거울에서 눈을 떼지 못한다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: UnlockMemory(MEM_FRAGMENT_03), ModifyMental(-2), ModifyGlitchLevel(+1), ModifyHp(-4), AddItem(ITEM_03), ModifyMental(+2), ModifyAffinity(+1)
- possible combat handoff: None
- possible item reward: ITEM_03
- possible memory unlock: MEM_FRAGMENT_03
- hidden values: ModifyGlitchLevel(+1) internal only
- UI notes: Keep event body above bottom choice buttons; result text should replace body or appear in compact result panel.

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
