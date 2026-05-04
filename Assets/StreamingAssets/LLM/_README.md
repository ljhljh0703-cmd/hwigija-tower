# LLM StreamingAssets Intake

Place exported on-device model runtime files under a model-specific folder here only after the commit policy for large artifacts is confirmed.

Expected layout for the current demo intake:

```text
Assets/StreamingAssets/LLM/mataios-demo-sft-v0.1/
```

Runtime config should point to paths relative to `Application.streamingAssetsPath`, for example:

```text
LLM/mataios-demo-sft-v0.1/model
LLM/mataios-demo-sft-v0.1/tokenizer
```

Do not commit raw checkpoints, full model weights, or generated native build output without an explicit storage decision.
