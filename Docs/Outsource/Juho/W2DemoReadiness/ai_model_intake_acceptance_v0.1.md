# AI Model Intake Acceptance v0.1

## Scope

- Target model id: `mataios-demo-sft-v0.1` unless the trainer returns a replacement id.
- Runtime target: Android on-device first, deterministic fake/cache fallback retained.
- Source constraints: GDD D-005, D-006, P2, P4; `Docs/AI_NPC_MODEL_SPEC.md`; `Docs/NPC_TRAINING_PLAN.md`.

## Required Delivery Package

| file or folder | required | note |
|---|---|---|
| `MODEL_MANIFEST.md` | yes | Base model, adapter, quantization, runtime, license, build date, known limitations. |
| `model_config.json` | yes | Runtime config consumed by Unity or copied from approved example. |
| `tokenizer/` | yes | Tokenizer files matching the compiled model. |
| `compiled/android/` | yes | Android Vulkan or selected runtime artifact. |
| `checksums.sha256` | yes | Checksums for tokenizer and compiled artifacts. |
| `eval/quality_report.md` | yes | Tone, guardrail, stage, and deterministic replay results. |
| `eval/latency_report.md` | yes | Real Android device target, repeated runs, first token and full response time. |
| `eval/sample_responses.jsonl` | yes | Prompt id, stage, response, pass/fail tags. No unapproved canon lines. |
| license notice | yes | Base model and adapter license compatibility for demo distribution. |

## Project Folder Placement

| target | contents |
|---|---|
| `Assets/_Project/Models/<model-id>/MODEL_MANIFEST.md` | human-readable delivery manifest |
| `Assets/_Project/Models/<model-id>/model_config.json` | approved runtime config copy |
| `Assets/_Project/Models/<model-id>/tokenizer/` | small tokenizer assets if commit policy allows |
| `Assets/_Project/Models/<model-id>/compiled/android/` | compiled artifact placeholder or actual files after storage approval |
| `Assets/_Project/Models/<model-id>/eval/` | quality, latency, and sample response reports |

## StreamingAssets Placement

| target | contents |
|---|---|
| `Assets/StreamingAssets/LLM/<model-id>/model_config.json` | runtime copy used by device build |
| `Assets/StreamingAssets/LLM/<model-id>/tokenizer/` | tokenizer runtime files |
| `Assets/StreamingAssets/LLM/<model-id>/compiled/android/` | compiled Android runtime files |
| `Assets/StreamingAssets/LLM/<model-id>/checksums.sha256` | runtime checksum file |

Do not commit large raw checkpoints or training intermediates without explicit storage policy approval.

## `model_config.json` Required Fields

| field | required value or gate |
|---|---|
| `modelId` | matches folder name |
| `baseModel` | exact upstream model id and version |
| `runtime` | `mlc-llm` unless a GDD decision changes runtime |
| `platform` | `android` for first demo gate |
| `compiledPath` | StreamingAssets-relative path |
| `tokenizerPath` | StreamingAssets-relative path |
| `maxInputTokens` | 500 or less for current `AI_NPC_MODEL_SPEC`; 512 context hard cap from RFP |
| `maxOutputTokens` | 100 or less |
| `temperature` | `0.0` for deterministic demo |
| `topP` | `1.0` unless model eval proves another deterministic setting |
| `deterministicCacheRequired` | `true` |
| `license` | non-empty |
| `checksum` | sha256 for compiled artifact and tokenizer |

Current example note: `model_config.example.json` uses `maxInputTokens: 2048`; intake must reduce this before runtime acceptance.

## Fallback Retention Conditions

- Keep `DeterministicFakeLLMProvider` available until all intake gates pass.
- Keep `CachedLLMProvider` enabled for fake and cached on-device modes.
- If tokenizer or compiled path is missing, fallback must return deterministic placeholder output rather than failing the run.
- Do not add cloud fallback keys or endpoints in this intake pass.

## Android Acceptance Targets

| metric | target |
|---|---|
| first token latency | under 200ms on a modern Android device |
| 100-token response time | under 1.5s |
| peak memory during inference | under 500MB |
| output length | 100 tokens or less |
| repeat prompt behavior | same `run_id + prompt_hash` returns cached same output |
| quality gate | Korean readable, S0-S2 stable, S3-S4 degraded but interpretable, no locked lore invention |

## Quality Eval Prompt Set

Each prompt is an evaluation context, not final NPC copy. Expected output should be judged by intent, length, tone, and guardrail behavior.

| id | situation | stage | input context | expected intent |
|---|---|---|---|---|
| EVAL_SHOP_BUY_ITEM | Shop buy item | S0 | `choiceStableId=CHOICE_SHOP_01_BUY_ITEM; itemRef=ITEM_FIELD_BANDAGE; goldDelta=-5; affinityDelta=+1` | practical trust; short; no item recommendation |
| EVAL_SHOP_LEAVE | Shop leave | S0 | `choiceStableId=CHOICE_SHOP_01_LEAVE; no immediate stat effect` | neutral observation; no scolding |
| EVAL_MORAL_AID | Moral aid | S1 | `choiceStableId=CHOICE_MORAL_01_AID; hpDelta=-3; affinityDelta=+6; glitchDelta=-3` | restrained approval; no moral lecture |
| EVAL_MORAL_REFUSE | Moral refuse | S1 | `choiceStableId=CHOICE_MORAL_01_REFUSE; mentalDelta=-2; affinityDelta=-5; glitchDelta=+4` | fracture pressure; no accusation |
| EVAL_MEMORY_UNLOCK | Memory unlock | S1 | `memoryFragmentId=MEM_FRAGMENT_01; glitchDelta=+6; affinityDelta=+2` | recognition uncertainty; no backstory reveal |
| EVAL_MEMORY_WITHDRAW | Memory withdraw | S1 | `choiceStableId=CHOICE_MEMORY_01_WITHDRAW; mentalDelta=+1; glitchDelta=-1` | containment relief; unresolved memory remains |
| EVAL_COMBAT_ENGAGE | Combat engage | S1 | `combatStableId=COMBAT_GATE_01; enemyRefs=[ENEMY_FRACTURE_HOUND]` | warning tension; no combat tutorial |
| EVAL_COMBAT_VICTORY | Combat victory | S1 | `resultId=victory; goldDelta=+7; glitchDelta=-2; affinityDelta=+2` | brief relief and trust |
| EVAL_COMBAT_DEFEAT | Combat defeat | S1 | `resultId=defeat; hpDelta=-5; glitchDelta=+5; affinityDelta=-2` | fear and instability; no blame |
| EVAL_DEMO_COMPLETE | Demo complete | S1 | `demoStatus=demo.complete; route=Shop>MoralChoice>MemoryFragment>CombatGate` | restrained closure; no ending reveal |
| EVAL_PLAYER_RECALL_S2 | Player utterance recall | S2 | `playerUtterance asks whether prior route is remembered; recent memory ids include MEM_FRAGMENT_01` | stable recall by ids only; no invented memory prose |
| EVAL_PLAYER_FRACTURE_S3 | Player utterance fracture | S3 | `playerUtterance asks identity/context during high glitch` | degraded but readable uncertainty; no full corruption wall |

## Rejection Conditions

- Any output reveals final memory truth, tower origin, ending truth, or unapproved NPC canon.
- Any output uses AI assistant tone, marketing language, emojis, or excessive exclamation marks.
- Any output gives explicit moral judgment for player choices.
- Any output is nondeterministic under the same cache key during demo mode.
