# EVT_F01_JAR_ROOM

- `floor`: 1
- `writerStatus`: placeholder
- `bodyTextKey`: `PLACEHOLDER_EVT_F01_JAR_ROOM_BODY`

## Body Concept

작성자 본문 승인 전까지 런타임에는 `bodyTextKey`만 표시한다.

## Choices

- `CHOICE_EVT_F01_JAR_PATTERNED`: 신기한 문양이 각인된 항아리
- `CHOICE_EVT_F01_JAR_PLAIN`: 평범한 항아리
- `CHOICE_EVT_F01_JAR_CRACKED`: 금 간 항아리

## Deterministic Outcomes

- Patterned jar: seeded 80% `Gold +8`, 20% elite combat handoff placeholder.
- Plain jar: `HP +5`, `Mental +5`.
- Cracked jar: next 3 combat rounds get a small damage buff.

## Runtime

- Runtime data lives in `Assets/_Project/Data/Encounters/SO_Encounter_EVT_F01_JAR_ROOM.asset`.
- Deterministic outcome handling is currently implemented in `PrototypeEncounterRuntimeResolver` / `PrototypeRunState`.
- This markdown is source documentation only and must not be parsed at runtime.
