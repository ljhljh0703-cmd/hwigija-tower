# Unityless Demo Review Report v0.1

## Scope

- Review date: 2026-05-06 KST
- Branch reviewed: `Juho/Codex`
- Working head reviewed: `738b157 Merge remote-tracking branch 'origin/Proto' into Juho/Codex`
- Reference commit: `57077ee Fix CombatGate demo complete UI layering`
- Method: file review plus existing 1080x1920 screenshots only
- Unity Editor: not available
- PlayMode: not run in this session
- New screenshot capture: not possible in this session

This review separates what can be verified without Unity from what still needs the Unity owner. It does not modify scene, ScriptableObject, runtime script, NPC dialogue, memory prose, or story truth.

## Could Verify

| item | status | evidence file | confidence | next owner |
|---|---|---|---:|---|
| Document consistency | verified-file-only | `Docs/Project/hwiglija-tower-gdd.md`; `Docs/Project/hwiglija-tower-tone-bible.md`; `Docs/Outsource/Juho/W2DemoReadiness/demo_readiness_audit_v0.1.md`; `Docs/Outsource/Juho/W2DemoReadiness/dev_handoff_next_tasks_v0.1.md` | high | dev |
| Existing screenshot UI risks | verified-from-existing-screenshots | `Docs/Outsource/Juho/W2DemoReadiness/viewport_qa_result_v0.1.md`; `Docs/Outsource/Juho/W2DemoReadiness/screenshots/*.png` | high | dev |
| CombatGate layering fix | verified-from-existing-screenshots | `prototype_room_1080x1920_04_combat_layering_fixed.png`; `prototype_room_1080x1920_05_demo_complete_after_combat.png`; `Assets/_Project/Scripts/UI/PrototypeHud.cs` | high | dev |
| Manual combat HUD state | verified-file-only | `Assets/_Project/Scripts/UI/PrototypeHud.cs`; `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs`; `viewport_qa_result_v0.1.md` | medium | dev |
| stableId/key flow | verified-file-only | `PrototypeHud.BuildChoiceLabel`; `PrototypeHud.BuildMemoryKeyLine`; `PrototypeRoomSmokeTests.cs`; `demo_readiness_audit_v0.1.md` | medium | dev |
| Mataios portrait connection | verified-file-and-screenshot | `Assets/_Project/Art/Characters/마타이오스 전신.png`; `PrototypeHud.ApplyPortrait`; screenshots 01-05 | high | outsource/dev |
| AI/model acceptance criteria | verified-file-only | `Docs/AI_NPC_MODEL_SPEC.md`; `Docs/NPC_TRAINING_PLAN.md`; model manifests under `Assets/_Project/Models/**` | high | AI-training |
| Writer decision blocker | verified-file-only | `Docs/Project/hwiglija-tower-gdd.md` OQ-006; `design/memory-fragments.md`; `demo_readiness_audit_v0.1.md` | high | writer |
| Audio asset state | verified-file-only | `design/sound-brief.md`; `Assets/_Project/Audio/_README.md`; audio folder contents | high | outsource |

## Could Not Verify

| item | status | evidence file | confidence | next owner |
|---|---|---|---:|---|
| Actual PlayMode reproduction | not-verified-this-session | `viewport_qa_result_v0.1.md` records prior PlayMode pass, but this session did not run Unity | high | dev |
| New screenshot capture | not-verified-this-session | existing 5 PNGs only | high | dev |
| Button click feel | not-verified-this-session | PlayMode test covers button events, not physical feel | high | dev |
| Unity import setting runtime reflection | not-verified-this-session | `.meta` and scene references are present, but Editor import refresh was not run here | medium | dev |
| Android build | not-verified-this-session | `AI_NPC_MODEL_SPEC.md` lists Android Build Support as blocker | high | dev |
| Native on-device model load | not-verified-this-session | model manifests and eval templates exist, runtime artifacts are pending | high | AI-training/dev |

## Readiness Judgment

| area | readiness | note |
|---|---|---|
| W2 route logic | ready-for-Unity-owner-recording-check | Existing QA reports Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete route passes. |
| CombatGate visual layering | ready-for-recording-check | Screenshots 04/05 close the active-combat vs demo.complete overlap. |
| Recording visuals | not-ready-for-public-recording | Oversized world/debug node labels dominate the frame and should be hidden by default before capture. |
| Narrative text | blocked-for-polished-recording | Memory fragment final text remains writer-owned; current placeholder keys can appear on HUD. |
| AI/model | pending-model-acceptance | Deterministic fake/cache remains valid for demo, but real model artifacts and eval reports are not complete. |
| Android build | blocked | Build host/runtime artifact blockers remain outside this Unityless review. |

## Pillar Fit

- P1: The packet does not add new narrative truth; it preserves writer ownership for memory and ending material.
- P2: AI/model uncertainty is treated as a visible production gate, not hidden as a solved item.
- P3: The recording runbook keeps the 1-minute route focused and avoids expanding scope.
- P4: Deterministic cache/model gates are called out explicitly.
- P5: Choice/result visibility remains part of the recording path, but debug density needs presentation cleanup.
