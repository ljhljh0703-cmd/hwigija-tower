# Dev Handoff Next Tasks v0.1

## P0 — Demo Execution / Validation Blockers

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Run final manual CombatGate QA pass | no code expected; `PrototypeHud.cs`, `PrototypeRoomController.cs`, `PrototypeRunState.cs` only if issues are found | Default scene manual combat reaches `demo.complete` after final combat result; Attack/Defend buttons are usable; Skill is visibly disabled. | PlayMode manual combat smoke plus one portrait mobile visual pass. | Code path is implemented, but visual overlap/readability still needs a human pass. |
| Run final Mataios portrait placement QA | no code expected; `PrototypeHud.cs` or scene only if issues are found | Portrait is visible and does not cover route, result, memory, choices, or combat panel on portrait viewport. | PlayMode scene load plus screenshot/manual visual check. | Current source is full-body with background; acceptable for W2, but may need crop polish. |
| Keep deterministic AI fallback while model is pending | `LLMRuntimeSettings` asset, `LLMProviderFactory.cs`, model config files | Missing model/tokenizer/compiled artifact never breaks run; same cache key replays same response. | Existing EditMode LLM provider tests plus model intake file existence test. | Replacing fake output too early can destabilize review flow. |
| Resolve Android build host blocker | Unity install / build environment, no runtime code if possible | Android Build Support present; build harness reaches game build stage or fails with actionable project error. | Android build harness. | Local machine setup, not code, is the current known blocker. |

## P1 — First Demo Persuasion

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Wire actual skill selection/effects | `PrototypeHud.cs`, `PrototypeRoomController.cs`, ability runtime layer | Skill button becomes interactable only when at least one usable ability is available and applies ability-specific effect. | EditMode combat branch tests; PlayMode skill smoke. | Current W2 UI intentionally disables Skill; enabling it without ability routing would mislead QA. |
| Tune combat panel visual hierarchy | `PrototypeHud.cs`, UI assets | Last action, damage, enemy HP, player HP, reward deltas, and optional combo damage remain readable on mobile. | PlayMode screenshot/manual check. | Too much text can crowd portrait mobile UI. |
| Apply item passive effects in combat loop | `CombatAbilityModifiers.cs`, `PrototypeRunState.cs`, item SOs | Owned item passives affect attack/defense/first-hit/start/victory behavior per implemented keys. | EditMode item passive combat tests. | Design docs contain more item effects than current loop supports. |
| Replace silent demo with P0 sound placeholders | `Assets/_Project/Audio/**`, UI/combat event hooks | Button, confirm, combat start, memory unlock, and completion cues play or are explicitly muted by policy. | Manual audio smoke; no missing asset warnings. | Sound is currently documentation-only. |

## P2 — Polish

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Convert enemy patterns from placeholder data to reviewed demo patterns | `EnemyPatternData` SOs, enemy SO refs, pattern UI | Pattern text and two response choices appear only when enemy-turn UI exists. | EditMode data existence and PlayMode enemy turn smoke. | Writer-owned choice text must be approved. |
| Decide whether to create a cropped portrait derivative | `Assets/_Project/Art/Characters/**`, scene portrait sprite assignment | If the current full-body source reads poorly in mobile QA, replace scene sprite with a stable cropped derivative. | Manual visual check. | Rename/crop is polish now, not an integration blocker. |
| Improve route/result visual hierarchy | `PrototypeHud.cs`, UI assets | Route, stats, memory, portrait, choices, and result fit 1080x1920 without overlap. | PlayMode screenshot/manual check. | Current text-heavy HUD can feel like debug UI. |
| Create final import map for external art/audio | `Docs/ASSET_INTAKE.md`, optional manifest docs | Every delivered asset has target folder, filename, import settings, owner, and status. | Document review only. | Avoid duplicating Unity meta or overwriting existing assets. |

## P3 — Later Expansion

| goal | files likely touched | acceptance criteria | tests needed | risk |
|---|---|---|---|---|
| Full S3/S4 Mataios visual state support | Art variants, UI state binding, NPC stage state | Stage changes visibly alter portrait without hiding identity. | Stage transition UI smoke. | Needs writer/art approval. |
| Native model runtime smoke | `MLCBridge.cs`, platform plugins, StreamingAssets model folder | On-device model initializes, returns within latency target, and falls back safely. | Android device latency and quality eval. | Native plugin and artifact delivery are not present yet. |
| Full enemy-turn timer / auto Mataios choice | Combat UI, enemy pattern data, seeded timer logic | Glitch shortens timer; timeout triggers Mataios auto choice deterministically. | Timed PlayMode or integration smoke. | Timer must preserve P4 determinism. |
