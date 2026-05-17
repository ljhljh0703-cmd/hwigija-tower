# <EVENT_ID> — <표시용 제목>

## Metadata
- floor:
- eventId:
- eventType: Event
- tone: 탐색 / 위험 / 휴식 / 기억 / 거래 / 전투유도 / 도덕선택
- riskLevel: low / medium / high
- imageSlot:
- recommendedImage:
- writerStatus: draft
- runtimePriority: P0 / P1 / P2

## Player-Facing Summary
플레이어가 한눈에 이해할 수 있는 1문장 요약.

## Event Body
이벤트 본문.
2~5문장.
최종 서사 확정처럼 쓰지 말고, 임시 플레이 가능 문장으로 작성.

## Cutscene Direction
이미지 제작용 설명.
- 장면:
- 중심 오브젝트:
- 인물 포함 여부:
- 색감:
- 피해야 할 것:

### Image Prompt
Vertical dark fantasy mobile game event cutscene, 1080x1920.
<장면 설명>.
No readable text, no UI.
Cinematic lighting, painterly gothic fantasy style.
Central focus object, empty space at top and bottom for mobile UI.

## Choices

### Choice 1
- stableId:
- label:
- preview:
- requirement:
- effects:
- resultText:
- mataiosIntent:
- probabilityHint:
- followUp:

### Choice 2
- stableId:
- label:
- preview:
- requirement:
- effects:
- resultText:
- mataiosIntent:
- probabilityHint:
- followUp:

### Choice 3
- stableId:
- label:
- preview:
- requirement:
- effects:
- resultText:
- mataiosIntent:
- probabilityHint:
- followUp:

## Runtime Conversion Notes
- requirement DTO:
- effect DTO:
- possible combat handoff:
- possible item reward:
- possible memory unlock:
- hidden values:
- UI notes:

## QA Checklist
- [ ] 선택지는 2~3개다
- [ ] 선택 전 위험/보상 힌트가 있다
- [ ] 결과가 수치/상태로 연결된다
- [ ] Glitch 수치를 직접 말하지 않는다
- [ ] raw stableId가 노출되지 않는다
- [ ] 최종 NPC 대사처럼 보이지 않는다
