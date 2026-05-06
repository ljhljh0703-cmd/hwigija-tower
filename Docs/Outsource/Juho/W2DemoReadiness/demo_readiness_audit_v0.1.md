# W2 Demo Readiness Audit v0.1

## Scope

- Audit date: 2026-05-06 KST
- Branch checked: `Juho/Codex`
- Current HEAD checked: `ad82eef Merge remote-tracking branch 'origin/Proto' into Juho/Codex`
- Requested reference commit: `27c43da Implement combat loop features and add new SO assets`
- Note: `27c43da` is present under the current merge commit, so this audit treats `ad82eef` as the working review head.

## Readiness Table

| item | status | evidence | demo impact | required next action | owner |
|---|---|---|---|---|---|
| Shop | ready | `Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_01.asset`; `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs`; `Docs/Outsource/Juho/8DayDemo/demo_review_script_v0.1.md` | high | Keep placeholder labels for review, but confirm review setup grants enough Gold for `CHOICE_SHOP_01_BUY_ITEM` or shows disabled-visible reason. | dev |
| MoralChoice | ready | `SO_Encounter_ENC_MORAL_CHOICE_01.asset`; `npc_demo_context_matrix_v0.1.md`; PlayMode button flow covers `CHOICE_MORAL_01_REFUSE` stat deltas. | high | Writer later replaces placeholder body/choice/reaction copy; no code blocker for demo route. | writer |
| MemoryFragment | partial | `SO_Encounter_ENC_MEMORY_FRAGMENT_01.asset`; `SO_MemoryFragment_MEM_FRAGMENT_01.asset`; `memory_fragment_demo_spec_v0.1.md`; `design/memory-fragments.md` remains writer-review draft. | high | Decide MEM_FRAGMENT_01 final direction or allow placeholder title/body keys in review build. | writer |
| CombatGate | partial | `SO_Encounter_ENC_COMBAT_GATE_01.asset`; `PrototypeRunState.ResolveCombatHandoff`; PlayMode smoke reaches `demo.complete` through `AutoResolveCombat`. | high | For public 1-minute demo, keep auto-resolve or wire interactive combat UI and delay DemoComplete until final combat result. | dev |
| DemoComplete | ready | `PrototypeRunState.DemoComplete`; `PrototypeHud.UpdateDemoCompletePanel`; PlayMode smoke asserts `demo.complete`. | high | Verify completion panel is visually readable on target portrait device after art/UI pass. | dev |
| Combat UI | partial | `CombatAction.Attack/Defend/Skill` exists; `ResolveCombatRoundInteractive(CombatAction)` exists; HUD only creates encounter choice buttons, not Attack/Defend/Skill controls. | high | Add combat action buttons or intentionally keep auto-resolve for first external review. If interactive mode is enabled, expose HP, enemy HP, last action, result, and combo damage. | dev |
| Item passive | partial | `ItemData.cs`; `CombatAbilityModifiers_UsesItemPassiveNumericParams` test; `design/items.md`; SO item assets exist. | medium | Connect owned item passives to combat loop and snapshot, or mark them out of 1-minute demo scope. | dev |
| Enemy pattern | partial | `EnemyPatternData.cs`; `SO_EnemyPattern_PATTERN_BASIC/GLITCH/ELITE.asset`; pattern choice fields are placeholder and not active timed enemy-turn UI yet. | medium | Keep as content placeholder for W2 demo; implement timed enemy pattern responses after 1-minute route is stable. | writer |
| Mataios portrait | partial | `Assets/_Project/Art/Characters/마타이오스 전신.png`; image reviewed locally; no rename/import brief applied yet. | high | Produce cropped S0-S2 portrait source and wire it to HUD/rest or encounter panel. Do not edit current image in this pass. | outsource |
| AI NPC fallback | partial | `LLMProviderFactory.cs`; `DeterministicFakeLLMProvider`; `CachedLLMProvider`; `AI_NPC_MODEL_SPEC.md`; `NPC_TRAINING_PLAN.md`; `model_config.example.json`. | high | Keep deterministic fake/cache fallback until model intake passes tokenizer, compiled artifact, checksum, latency, and quality gates. | AI-training |
| Android build | blocked | `Docs/IMPLEMENTATION_READY_CHECKLIST.md`; `AI_NPC_MODEL_SPEC.md` both list Android Build Support missing. | high | Install Android Build Support or move public demo validation to editor/playmode capture until build host is ready. | dev |
| Sound placeholder | missing | `design/sound-brief.md`; `asset_delivery_manifest_v0.1.md`; `Assets/_Project/Audio` contains folder/readme/meta only, no WAV/OGG payload. | medium | Deliver P0 UI/combat/memory sounds or add explicit silent-placeholder policy for review build. | outsource |
| Memory fragment text | blocked | GDD OQ-006 open; `memory_fragment_demo_spec_v0.1.md`; `design/memory-fragments.md` says writer review required. | high | Writer decides whether demo shows placeholder keys, intent-only panel, or approved MEM_FRAGMENT_01 title/body. | writer |

## Key Findings

- The 1-minute scripted route is functionally close when combat remains auto-resolved.
- Interactive combat has core code, but public UI buttons and round-by-round display are not yet a safe demo path.
- The received Mataios illustration is useful for S0-S2 direction, but needs a transparent/cropped UI-ready derivative before integration.
- AI NPC runtime should stay deterministic fake/cache for demo unless a model package passes the intake checklist.
- Android build is still blocked by local editor support, not by game code alone.
