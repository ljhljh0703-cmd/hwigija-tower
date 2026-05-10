# REST_ASK_MOOD_TEMP

- action id: `rest.ask_mood`
- player intent: player asks how Mataios is feeling.
- input policy: required.
- prompt profile: `rest.ask_mood`
- temporary fallback response: `임시 응답: 상태를 확인했다.`
- deterministic effect: Affinity +2, store player utterance/reflection, set `MATAIOS_HINT_S1_01`.
- writer replacement note: replace only after final Mataios voice and lore approval.
