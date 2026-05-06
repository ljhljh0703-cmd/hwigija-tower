# W2 Demo Readiness Audit v0.1

## Scope

- Audit date: 2026-05-06 KST
- Branch checked: `Juho/Codex`
- Current HEAD checked: `d380298 Merge remote-tracking branch 'origin/Proto' into Juho/Codex`
- Requested reference commit: `9300cc1 Integrate Mataios portrait and interactive demo combat UI`
- Note: `9300cc1` is present under the current merge commit, so this refresh treats `d380298` as the working review head.

## Readiness Table

| item | status | evidence | demo impact | required next action | owner |
|---|---|---|---|---|---|
| Shop | ready | `Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_01.asset`; `Assets/_Project/Tests/PlayMode/PrototypeRoomSmokeTests.cs`; `Docs/Outsource/Juho/8DayDemo/demo_review_script_v0.1.md` | high | Keep placeholder labels for review, but confirm review setup grants enough Gold for `CHOICE_SHOP_01_BUY_ITEM` or shows disabled-visible reason. | dev |
| MoralChoice | ready | `SO_Encounter_ENC_MORAL_CHOICE_01.asset`; `npc_demo_context_matrix_v0.1.md`; PlayMode button flow covers `CHOICE_MORAL_01_REFUSE` stat deltas. | high | Writer later replaces placeholder body/choice/reaction copy; no code blocker for demo route. | writer |
| MemoryFragment | partial | `SO_Encounter_ENC_MEMORY_FRAGMENT_01.asset`; `SO_MemoryFragment_MEM_FRAGMENT_01.asset`; `memory_fragment_demo_spec_v0.1.md`; `design/memory-fragments.md` remains writer-review draft. | high | Decide MEM_FRAGMENT_01 final direction or allow placeholder title/body keys in review build. | writer |
| CombatGate | ready-for-review | `SO_Encounter_ENC_COMBAT_GATE_01.asset`; `PrototypeRunState.ResolveCombatHandoff`; `PrototypeRoomController.ResolveCombatAction`; PlayMode `PrototypeRoom_CombatGateShowsInteractiveCombatPanel` covers manual CombatGate. | high | Run final portrait-device QA for CombatGate readability. Auto-resolve remains available for smoke tests, but default scene has `autoResolveCombat: 0`. | dev |
| DemoComplete | ready-for-review | `PrototypeRunState.FinalizeCombat` updates demo progression only after final combat when manual mode is used; `PrototypeHud.UpdateDemoCompletePanel`; PlayMode asserts `demo.complete` after manual combat resolves. | high | Verify completion panel is visually readable on target portrait device. | dev |
| Combat UI | ready-for-review | `PrototypeHud.EnsureCombatPanel`; `CreateCombatButton` creates Attack / Defend / Skill controls; `ResolveCombatAction` calls `CombatAction`; PlayMode confirms Attack lowers enemy HP, Defend branch is visible, Skill button exists but is disabled. | high | Treat Skill as intentionally disabled until ability-specific skill selection is wired; QA Attack/Defend/manual victory path. | dev |
| Item passive | partial | `ItemData.cs`; `CombatAbilityModifiers_UsesItemPassiveNumericParams` test; `design/items.md`; SO item assets exist. | medium | Connect owned item passives to combat loop and snapshot, or mark them out of 1-minute demo scope. | dev |
| Enemy pattern | partial | `EnemyPatternData.cs`; `SO_EnemyPattern_PATTERN_BASIC/GLITCH/ELITE.asset`; pattern choice fields are placeholder and not active timed enemy-turn UI yet. | medium | Keep as content placeholder for W2 demo; implement timed enemy pattern responses after 1-minute route is stable. | writer |
| Mataios portrait | ready-for-review | `Assets/_Project/Art/Characters/마타이오스 전신.png`; `.meta` imports it as Sprite; `PrototypeRoom.unity` assigns `mataiosPortrait`; `PrototypeHud.ApplyPortrait`; PlayMode asserts `hud.PortraitVisible`. | high | Run visual QA for crop/readability. Stable English rename and custom crop are polish, not W2 blockers. | outsource |
| AI NPC fallback | ready | `LLMProviderFactory.cs`; `DeterministicFakeLLMProvider`; `CachedLLMProvider`; `AI_NPC_MODEL_SPEC.md`; `NPC_TRAINING_PLAN.md`; `model_config.example.json`. | high | Keep deterministic fake/cache fallback until model intake passes tokenizer, compiled artifact, checksum, latency, and quality gates. | AI-training |
| Android build | blocked | `Docs/IMPLEMENTATION_READY_CHECKLIST.md`; `AI_NPC_MODEL_SPEC.md` both list Android Build Support missing. | high | Install Android Build Support or move public demo validation to editor/playmode capture until build host is ready. | dev |
| Sound placeholder | missing | `design/sound-brief.md`; `asset_delivery_manifest_v0.1.md`; `Assets/_Project/Audio` contains folder/readme/meta only, no WAV/OGG payload. | medium | Deliver P0 UI/combat/memory sounds or add explicit silent-placeholder policy for review build. | outsource |
| Memory fragment text | blocked | GDD OQ-006 open; `memory_fragment_demo_spec_v0.1.md`; `design/memory-fragments.md` says writer review required. | high | Writer decides whether demo shows placeholder keys, intent-only panel, or approved MEM_FRAGMENT_01 title/body. | writer |

## Key Findings

- The 1-minute scripted route now has an interactive CombatGate review path.
- Attack and Defend are exposed in the combat panel; Skill is present but disabled until ability-specific skill selection is implemented.
- The received Mataios illustration is imported and connected to the demo HUD; the remaining risk is visual crop/readability QA.
- AI NPC runtime should stay deterministic fake/cache for demo unless a model package passes the intake checklist.
- Android build is still blocked by local editor support, not by game code alone.
