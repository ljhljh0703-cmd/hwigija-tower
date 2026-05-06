# Dev Handoff Next Tasks v0.1

## P0 — Demo Execution / Validation Blockers

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Decide combat presentation path for the 1-minute demo | `PrototypeRoomController.cs`, `PrototypeRunState.cs`, `PrototypeHud.cs` | Either auto-resolve remains explicitly enabled for review, or manual combat buttons work and DemoComplete waits for final combat result. | PlayMode route smoke for CombatGate and DemoComplete. | Manual mode may currently mark demo progression at combat start instead of final result. |
| Wire Mataios portrait into the demo HUD or encounter panel | `PrototypeHud.cs`, scene/prefab setup, imported sprite asset | S0-S2 portrait appears on Shop/Moral/Memory/CombatGate surfaces without covering choices or result text. | PlayMode scene load plus screenshot/manual visual check. | Current source has a background and full-body framing; portrait derivative is needed. |
| Keep deterministic AI fallback while model is pending | `LLMRuntimeSettings` asset, `LLMProviderFactory.cs`, model config files | Missing model/tokenizer/compiled artifact never breaks run; same cache key replays same response. | Existing EditMode LLM provider tests plus model intake file existence test. | Replacing fake output too early can destabilize review flow. |
| Resolve Android build host blocker | Unity install / build environment, no runtime code if possible | Android Build Support present; build harness reaches game build stage or fails with actionable project error. | Android build harness. | Local machine setup, not code, is the current known blocker. |

## P1 — First Demo Persuasion

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Add combat action UI for Attack / Defend / Skill | `PrototypeHud.cs`, `PrototypeRoomController.cs`, `PrototypeRunState.cs` | Buttons appear only during combat; each calls the correct `CombatAction`; HP/enemy HP update after each round. | EditMode combat branch tests; PlayMode manual combat smoke. | Skill currently behaves like Attack unless ability layer is connected. |
| Display combat round result and combo damage | `PrototypeHud.cs`, `PrototypeRunSnapshot.cs`, `CombatRoundResult.cs` if snapshot expands | Last action, damage, enemy HP, player HP, and optional combo damage are visible and compact. | PlayMode UI assertions for visible markers. | Too much text can crowd portrait mobile UI. |
| Apply item passive effects in combat loop | `CombatAbilityModifiers.cs`, `PrototypeRunState.cs`, item SOs | Owned item passives affect attack/defense/first-hit/start/victory behavior per implemented keys. | EditMode item passive combat tests. | Design docs contain more item effects than current loop supports. |
| Replace silent demo with P0 sound placeholders | `Assets/_Project/Audio/**`, UI/combat event hooks | Button, confirm, combat start, memory unlock, and completion cues play or are explicitly muted by policy. | Manual audio smoke; no missing asset warnings. | Sound is currently documentation-only. |

## P2 — Polish

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Convert enemy patterns from placeholder data to reviewed demo patterns | `EnemyPatternData` SOs, enemy SO refs, pattern UI | Pattern text and two response choices appear only when enemy-turn UI exists. | EditMode data existence and PlayMode enemy turn smoke. | Writer-owned choice text must be approved. |
| Improve route/result visual hierarchy | `PrototypeHud.cs`, UI assets | Route, stats, memory, portrait, choices, and result fit 1080x1920 without overlap. | PlayMode screenshot/manual check. | Current text-heavy HUD can feel like debug UI. |
| Create final import map for external art/audio | `Docs/ASSET_INTAKE.md`, optional manifest docs | Every delivered asset has target folder, filename, import settings, owner, and status. | Document review only. | Avoid duplicating Unity meta or overwriting existing assets. |

## P3 — Later Expansion

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Full S3/S4 Mataios visual state support | Art variants, UI state binding, NPC stage state | Stage changes visibly alter portrait without hiding identity. | Stage transition UI smoke. | Needs writer/art approval. |
| Native model runtime smoke | `MLCBridge.cs`, platform plugins, StreamingAssets model folder | On-device model initializes, returns within latency target, and falls back safely. | Android device latency and quality eval. | Native plugin and artifact delivery are not present yet. |
| Full enemy-turn timer / auto Mataios choice | Combat UI, enemy pattern data, seeded timer logic | Glitch shortens timer; timeout triggers Mataios auto choice deterministically. | Timed PlayMode or integration smoke. | Timer must preserve P4 determinism. |
