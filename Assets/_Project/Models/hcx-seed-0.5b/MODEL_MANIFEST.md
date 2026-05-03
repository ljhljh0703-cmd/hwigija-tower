# HCX SEED 0.5B Model Manifest

Status: placeholder intake folder.

## Required Files

- `compiled/android/`: Android MLC output for Vulkan runtime.
- `compiled/ios/`: iOS MLC output for Metal runtime.
- `tokenizer/`: tokenizer files required by the compiled runtime.
- `eval/`: acceptance reports and prompt response samples.

## Acceptance Targets

- Korean NPC response quality is acceptable for Mataios S0-S2 stability.
- S3-S4 degradation is usable as in-game decay, not random nonsense.
- Input budget: 500 tokens or less.
- Output budget: 100 tokens or less.
- Target latency: 1.5 seconds or less on Android test device.
- Determinism: same `run_id` and prompt hash must produce cached repeat output.

## Integration Notes

The Unity bridge currently calls native library `mlc_llm_unity`.
Do not commit raw checkpoints unless the license and file size are approved.
