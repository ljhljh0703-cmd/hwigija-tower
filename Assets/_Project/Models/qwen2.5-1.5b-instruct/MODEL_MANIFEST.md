# Qwen2.5 1.5B Instruct Model Manifest

Status: fallback candidate intake folder.

Use this only if the primary Korean model fails the D-005 acceptance gate.

## Required Files

- `compiled/android/`: Android MLC output for Vulkan runtime.
- `compiled/ios/`: iOS MLC output for Metal runtime.
- `tokenizer/`: tokenizer files required by the compiled runtime.
- `eval/`: acceptance reports and prompt response samples.

## Acceptance Targets

- Korean NPC response quality is at least shippable for short recall/reflection.
- Input budget: 500 tokens or less.
- Output budget: 100 tokens or less.
- Target latency: 1.5 seconds or less on Android test device.
- Determinism: same `run_id` and prompt hash must produce cached repeat output.
