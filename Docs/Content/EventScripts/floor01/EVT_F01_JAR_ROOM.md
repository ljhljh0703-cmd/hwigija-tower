# EVT_F01_JAR_ROOM — 항아리 방

## Metadata
- floor: 1
- eventId: EVT_F01_JAR_ROOM
- eventType: Event
- tone: 탐색
- riskLevel: low
- imageSlot: eventCutscene
- recommendedImage: Assets/_Project/Art/Encounters/evt_f01_jar_room_bg.png
- writerStatus: draft
- runtimePriority: P0

## Player-Facing Summary
항아리 하나를 골라 낮은 위험의 보상 또는 전투 위험을 학습한다.

## Event Body
항아리들이 잔뜩 있는 방이다. 항아리마다 신기한 냄새가 난다. 어떤 항아리를 열어볼까?

## Cutscene Direction
- 장면: 단지들이 잔뜩 있는 어두운 창고
- 중심 오브젝트: 문양 항아리와 금 간 항아리
- 인물 포함 여부: 없음
- 색감: 차가운 회색, 낡은 황토색, 약한 금빛
- 피해야 할 것: 읽을 수 있는 문자, 과한 코미디 보물 연출

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
Vertical stone storage room filled with old jars, one patterned jar glowing faintly, one plain clay jar, one cracked jar in the foreground.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId: CHOICE_EVT_F01_JAR_ROOM_PATTERNED
- label: 문양이 새겨진 항아리를 연다
- preview: 80%: Gold +8 / 20%: 강한 적과 조우
- requirement: None
- effects:
  - ModifyGold(+8)
  - StartCombat(ENEMY_SLIME_POOL_01)
- resultText: 항아리 안에서 빛나는 동전이 굴러나왔다. 문양은 잠깐 손끝에 남았다.
- mataiosIntent: 위험을 감수한 선택을 조용히 관찰한다.
- probabilityHint: 80% reward / 20% combat
- followUp: None

### Choice 2
- stableId: CHOICE_EVT_F01_JAR_ROOM_PLAIN
- label: 평범한 항아리를 연다
- preview: HP +5 / Mental +1
- requirement: None
- effects:
  - ModifyHp(+5)
  - ModifyMental(+1)
- resultText: 안에는 아직 먹을 수 있는 음식이 남아 있었다. 짧은 숨을 고를 수 있었다.
- mataiosIntent: 안전한 선택에 안도한다.
- probabilityHint: None
- followUp: None

### Choice 3
- stableId: CHOICE_EVT_F01_JAR_ROOM_CRACKED
- label: 금 간 항아리를 깬다
- preview: 다음 전투 피해 강화 / Mental -1
- requirement: None
- effects:
  - AddRunBuff(BUFF_NEXT_3_COMBATS_DAMAGE_01)
  - ModifyMental(-1)
  - ModifyGlitchLevel(+1)
- resultText: 빈 항아리가 산산이 부서졌다. 아직 풀리지 않은 열이 손에 남았다.
- mataiosIntent: 말리려다 멈추고 선택을 기억해 둔다.
- probabilityHint: None
- followUp: None

## Runtime Conversion Notes
- requirement DTO: None
- effect DTO: ModifyGold(+8), StartCombat(ENEMY_SLIME_POOL_01), ModifyHp(+5), ModifyMental(+1), AddRunBuff(BUFF_NEXT_3_COMBATS_DAMAGE_01), ModifyMental(-1), ModifyGlitchLevel(+1)
- possible combat handoff: ENEMY_SLIME_POOL_01
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
