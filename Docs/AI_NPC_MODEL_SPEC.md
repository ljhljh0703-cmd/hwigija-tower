# AI NPC Model Specification

This document defines what to prepare before training, converting, and integrating
the on-device NPC model. It does not change GDD D-005.

## Goal

The model supports Mataios recall and run reflection while preserving:

- P2: model limitations become visible during S3-S4 decay.
- P4: repeat prompts are deterministic through cache keys.
- D-005: HyperCLOVA X SEED 0.5B first, Qwen2.5 1.5B fallback.
- D-006: SQLite stores one reflection per run.

## Runtime Contract

- Unity caller: `HwigiTower.LLM.OnDeviceLLMProvider`
- Native bridge: `HwigiTower.LLM.MLCBridge`
- Native library name: `mlc_llm_unity`
- Native entry point: `hwigi_mlc_complete`
- Input budget: 500 tokens or less.
- Output budget: 100 tokens or less.
- Call frequency: 3 calls or less per run.
- Cache key: `run_id + prompt_hash`

## Folder Contract

- Primary model: `Assets/_Project/Models/hcx-seed-0.5b/`
- Fallback model: `Assets/_Project/Models/qwen2.5-1.5b-instruct/`
- Training workspace: `Assets/_Project/Models/_training/`
- MLC intake: `Assets/ThirdParty/MLC-LLM/`
- Platform plugins: `Assets/Plugins/Android/`, `Assets/Plugins/iOS/`

## Dataset Format

Use JSONL. One object per line.

```json
{"id":"sample-001","task":"recall","stage":"S2","input":"...","output":"...","tags":["trust","short"]}
```

Required fields:

- `id`: stable unique id.
- `task`: `recall`, `reflection`, or `decay`.
- `stage`: `S0`, `S1`, `S2`, `S3`, `S4`, `S5`, or `reflection`.
- `input`: prompt body.
- `output`: approved target output.
- `tags`: short labels for filtering.

Do not put unapproved final NPC canon lines into a generated dataset. Writer-approved
lines should be marked with `tags:["writer_approved"]`.

## Training Scope

Preferred first pass:

- Prompt tuning or LoRA only.
- No full fine-tune unless D-005 quality fails and licensing allows it.
- Keep outputs short and structured for recall/reflection.

Minimum dataset targets:

- 20 recall examples for S0-S2.
- 20 decay examples for S3-S4.
- 20 reflection examples with exactly 3 sentence summaries.
- 10 refusal/guardrail examples for off-topic or malformed prompts.

## Quality Gate

Accept a candidate only if:

- Korean is readable without breaking the intended tone.
- S0-S2 answers are stable and coherent.
- S3-S4 answers can degrade but remain interpretable.
- Reflection outputs are exactly 3 short Korean sentences.
- No output invents new locked lore.
- Same prompt can be cached and replayed by `DeterministicCacheKey`.

## Conversion Outline

1. Export or merge the approved adapter.
2. Convert weights to the chosen MLC-compatible format.
3. Quantize for mobile, starting with q4-level quantization.
4. Compile Android Vulkan artifact.
5. Compile iOS Metal artifact only if needed for video/demo work.
6. Place outputs in the matching `compiled/` folder.
7. Import the native plugin under `Assets/Plugins`.
8. Run EditMode and PlayMode tests.
9. Run Android device latency and quality eval.

## Current Blockers

- D-005 acceptance gate is still open.
- Actual MLC native plugin is not imported.
- Actual SQLite native plugin is not imported.
- Android Build Support is not installed in the current Unity Editor.
