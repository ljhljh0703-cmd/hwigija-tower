# EVT_F01_ABANDONED_CAMP — 버려진 야영지

## Metadata
- floor: 1
- eventId: EVT_F01_ABANDONED_CAMP
- eventType: Event
- tone: 휴식
- riskLevel: low
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f01_abandoned_camp_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
꺼진 모닥불에서 쉬거나 남은 물자를 챙긴다.

## Event Body
꺼진 모닥불 옆에 젖은 담요와 작은 가방이 놓여 있다. 누군가 급히 떠난 흔적이지만 발자국은 문 앞에서 끊긴다. 아직 쓸 만한 것이 남아 있을지도 모른다.

## Cutscene Direction
- 장면: 석실 한쪽의 꺼진 모닥불과 방치된 가방
- 중심 오브젝트: 젖은 담요와 작은 가방
- 인물 포함 여부: 없음
- 색감: 차가운 청회색, 약한 주황 불씨
- 피해야 할 것: 현대 캠핑 장비, 시체 직접 노출

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Abandoned camp inside a stone room, extinguished campfire, wet blanket, small leather bag, footprints ending at a doorway.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F01_ABANDONED_CAMP_REST_FIRE
- label: 모닥불을 다시 피운다
- preview: HP +4 / Affinity +1
- requirement: None
- effects:
  - ModifyHp(+4)
  - ModifyAffinity(+1)
- resultText: 불씨가 아주 늦게 살아났다. 차가운 방이 잠깐 사람 사는 곳처럼 보인다.
- mataiosIntent: 불빛을 보고 짧게 숨을 고른다.
- probabilityHint: None
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F01_ABANDONED_CAMP_SEARCH_BAG
- label: 가방을 뒤진다
- preview: Gold +6 / Affinity -1
- requirement: None
- effects:
  - ModifyGold(+6)
  - ModifyAffinity(-1)
- resultText: 가방 안쪽 주머니에서 동전 몇 닢이 나왔다. 담요 밑의 빈자리는 그대로 남아 있다.
- mataiosIntent: 비난하지 않지만 한동안 가방을 본다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F01_ABANDONED_CAMP_LEAVE_MARK
- label: 표시를 남긴다
- preview: Mental +1 / 다음 전투 방어 강화
- requirement: None
- effects:
  - ModifyMental(+1)
  - AddRunBuff(BUFF_NEXT_COMBAT_DEFENSE_01)
- resultText: 벽에 짧은 표시를 새겼다. 돌아올 일은 없겠지만 손은 덜 떨린다.
- mataiosIntent: 이 표시를 본 적 있는 듯 잠깐 망설인다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyHp(+4), ModifyAffinity(+1), ModifyGold(+6), ModifyAffinity(-1), ModifyMental(+1), AddRunBuff(BUFF_NEXT_COMBAT_DEFENSE_01)
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
