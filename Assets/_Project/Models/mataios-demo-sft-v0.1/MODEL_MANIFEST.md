# mataios-demo-sft-v0.1

External AI NPC model intake scaffold for the demo runtime.

## Purpose

- Connect a later externally trained NPC model without changing encounter or room progression code.
- Keep runtime selection data-driven through `LLMRuntimeSettings` or the LLM-only `model_config.json` handoff.
- Preserve deterministic fake fallback when native plugin files or model artifacts are absent.

## Expected Layout

```text
Assets/_Project/Models/mataios-demo-sft-v0.1/
├── model_config.example.json
├── tokenizer/
├── compiled/
│   └── android/
└── eval/
```

Runtime copy target:

```text
Assets/StreamingAssets/LLM/mataios-demo-sft-v0.1/
```

## Required Before Runtime Smoke

- Compiled MLC-compatible Android runtime artifact.
- Tokenizer files matched to the compiled model.
- Checksums for every copied runtime artifact.
- `LLMRuntimeSettings` asset or LLM-only runtime config pointing to StreamingAssets-relative paths.

## Commit Policy

- Commit this manifest, small config examples, checksums, and evaluation summaries.
- Do not commit raw checkpoints, adapter weights, full model weights, or compiled runtime blobs until storage policy is confirmed.
- Do not add cloud API keys or endpoint configuration here.
