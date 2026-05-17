# EVT_F05_LAST_LANTERN — 마지막 등불

## Metadata
- floor: 5
- eventId: EVT_F05_LAST_LANTERN
- eventType: Event
- tone: 휴식
- riskLevel: medium
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f05_last_lantern_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
마지막 등불을 최종 보스 전 준비 자원으로 사용한다.

## Event Body
문 앞에 마지막 등불 하나가 걸려 있다. 불꽃은 작지만 꺼지지 않는다. 이 빛을 어떻게 들고 갈지 정해야 한다.

## Cutscene Direction
- 장면: 최종 문 앞의 작은 등불
- 중심 오브젝트: 마지막 등불
- 인물 포함 여부: 없음
- 색감: 작은 금빛, 차가운 검정, 문 주변의 백색
- 피해야 할 것: 희망적 과장, 엔딩 확정 암시

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Small final lantern hanging before a massive dark door, faint warm light, empty space for UI.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F05_LAST_LANTERN_LIGHT
- label: 더 밝힌다
- preview: Gold -8 / 최종 보스 방어 도움
- requirement: StatAtLeast(gold,8)
- effects:
  - ModifyGold(-8)
  - AddRunBuff(BUFF_FINAL_BOSS_DEFENSE_01)
- resultText: 기름을 더하자 등불이 조금 커졌다. 문 너머의 어둠이 한 걸음 물러난다.
- mataiosIntent: 빛이 커지는 것을 조용히 지켜본다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F05_LAST_LANTERN_EXTINGUISH
- label: 끄고 쉰다
- preview: HP +6 / Mental +2
- requirement: None
- effects:
  - ModifyHp(+6)
  - ModifyMental(+2)
- resultText: 등불을 끄자 방은 완전히 어두워졌다. 그 어둠 속에서 잠깐 숨을 고를 수 있었다.
- mataiosIntent: 어둠 속에서도 곁을 지킨다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F05_LAST_LANTERN_CARRY
- label: 들고 간다
- preview: AddRunBuff / Affinity +2
- requirement: None
- effects:
  - AddRunBuff(BUFF_FINAL_BOSS_CLARITY_01)
  - ModifyAffinity(+2)
- resultText: 등불은 손에 들자 더 작아졌다. 그래도 아직 꺼지지 않았다.
- mataiosIntent: 마지막까지 함께 가겠다는 의도를 받아들인다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: StatAtLeast(gold,8)
- effect DTO: ModifyGold(-8), AddRunBuff(BUFF_FINAL_BOSS_DEFENSE_01), ModifyHp(+6), ModifyMental(+2), AddRunBuff(BUFF_FINAL_BOSS_CLARITY_01), ModifyAffinity(+2)
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
