# Model Intake

This folder stores local model artifacts for the AI NPC pipeline.

Primary target:

- `hcx-seed-0.5b/`: HyperCLOVA X SEED 0.5B candidate, per GDD D-005.

Fallback target:

- `qwen2.5-1.5b-instruct/`: Qwen2.5 1.5B Instruct candidate, per GDD §8.1.

Training workspace:

- `_training/datasets/`: local JSONL datasets and eval prompts.
- `_training/adapters/`: LoRA or adapter outputs before merge/quantization.
- `_training/evals/`: quality and latency reports.

Runtime integration expects an MLC-compatible native plugin named `mlc_llm_unity`
with entry point `hwigi_mlc_complete`, matching `HwigiTower.LLM.MLCBridge`.
