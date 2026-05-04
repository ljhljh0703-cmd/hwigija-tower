# Model Intake

This folder stores local model artifacts for the AI NPC pipeline.

Demo runtime intake target:

- `mataios-demo-sft-v0.1/`: external SFT model drop-in scaffold for the first AI NPC runtime intake.

Primary target:

- `hcx-seed-0.5b/`: HyperCLOVA X SEED 0.5B candidate, per GDD D-005.

Fallback target:

- `qwen2.5-1.5b-instruct/`: Qwen2.5 1.5B Instruct candidate, per GDD §8.1.

Training workspace:

- `_training/datasets/`: local JSONL datasets and eval prompts.
- `_training/adapters/`: LoRA or adapter outputs before merge/quantization.
- `_training/evals/`: quality and latency reports.

Runtime integration expects an MLC-compatible native plugin named `mlc_llm_unity`
with entry points `hwigi_mlc_initialize` and `hwigi_mlc_complete`, matching
`HwigiTower.LLM.MLCBridge`.

Do not commit raw checkpoints, full model weights, merged adapter weights, or compiled runtime blobs until storage policy is explicitly confirmed. Keep handoff specs, small manifests, and evaluation summaries in git.
