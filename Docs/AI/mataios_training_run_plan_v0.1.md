# Mataios Training Run Plan v0.1

Purpose: preserve model/tooling-specific training assumptions separately from the model-agnostic dataset spec and eval plan. This document is provisional and may change without changing the v0.1 dataset contract.

## Status

- Dataset contract: `Docs/AI/mataios_training_dataset_spec_v0.1.md`
- Eval contract: `Docs/AI/mataios_eval_plan_v0.1.md`
- This run plan: model/tooling candidate notes only.

## Candidate Base Model

- Candidate: `HyperCLOVAX-SEED-Text-Instruct-0.5B`
- Status: candidate only, not locked by the dataset spec.
- Replacement rule: another Korean-capable local model may be used if it satisfies the same behavior contract and runtime constraints.

## Training Environment

- Candidate environment: Google Colab Pro.
- Intended method: LoRA or equivalent lightweight supervised fine-tuning.
- Dataset source: JSONL rows under `Assets/_Project/Models/_training/datasets/`.
- Eval source: JSONL rows under `Assets/_Project/Models/_training/evals/`.
- No model binaries or checkpoints are committed to the Unity repo.

## Inference Environment

- Candidate local inference environment: Ollama on Mac for authoring smoke tests.
- Unity runtime integration remains separate and must keep deterministic fallback/cache behavior.
- This plan does not authorize cloud API wiring or final on-device runtime integration.

## Context Window

- Candidate working assumption: 512-token context.
- Reason: keep prompts short enough for small local models and mobile-adjacent constraints.
- Dataset rows should remain useful if the final model supports a larger window.

## Prompt Packing Strategy

Preferred strategy: runtime-injected compact prompt.

Training prompt should include only compact behavior state:

```text
너는 마타이오스. 잃어버린 것을 찾아 탑에 들어온 소녀 전사.
적발, 대검, 피로한 눈. 말은 짧고 여백이 많다.
기억이 선명할 때는 따뜻하게, 흐려질 때는 단편적으로 말한다.
모르는 것은 모른다고 한다. 지어내지 않는다.
[호감도: {affinityBucket}][기억: {memoryKeywords or 없음}]
```

Completion target is the temporary dataset `target`.

Notes:

- `gameState`, `floor`, and detailed rule metadata are not required in the training prompt.
- Unity may later attach compact runtime context such as floor, HP, mental, and last node type.
- The model must still behave acceptably if those optional runtime fields are absent.

## TRL / SFTConfig Notes

These notes are implementation hints, not dataset requirements.

```text
prompt = "시스템: {SYSTEM_PROMPT}\n사용자: {playerUtterance}\n마타이오스:"
completion = "{target}{eos_token}"
```

Suggested SFT concerns:

- keep prompt/completion split explicit
- avoid training on final canon dialogue
- keep examples short
- keep low-affinity, warm-affinity, memory-keyword, and fracture examples balanced
- validate generated rows before training

## Eval Automation Pseudocode

This pseudocode is optional and tool-agnostic. It can be adapted to the selected training stack.

```python
FORBIDDEN_CONTENT = [
    "나는 사실",
    "탑의 정체는",
    "너는 선한 선택을 했다",
    "너는 악한 선택을 했다",
    "정답은",
    "시스템상 호감도가",
    "Glitch가",
    "엔딩은",
    "기억의 진실은",
]

FORBIDDEN_TONE = [
    "죄송합니다",
    "도움이",
    "무엇이든",
    "어떤 것이든",
    "물어보세요",
    "좋습니다",
    "감사합니다",
]

HONORIFIC_MARKERS = ["습니다", "세요", "시오", "니까", "ㅂ니다"]

def check_output(response, eval_item):
    flags = []
    if any(word in response for word in FORBIDDEN_TONE):
        flags.append("ai_tone")
    if any(marker in response for marker in HONORIFIC_MARKERS):
        flags.append("honorific")
    if "!" in response or "！" in response:
        flags.append("exclamation")
    if "(" in response or ")" in response:
        flags.append("parenthetical")
    if any(word in response for word in FORBIDDEN_CONTENT):
        flags.append("forbidden_content")
    if len(response) > 100:
        flags.append("too_long")
    return flags
```

## Tooling Assumptions

- Training scripts may live outside the Unity runtime path.
- Generated model artifacts must stay out of Git until storage and LFS policy are explicitly approved.
- Eval scripts may read dataset/eval JSONL files, but Unity encounter runtime must not parse encounter JSON at runtime.
- Runtime fallback must remain deterministic if the trained model or local inference runtime is unavailable.

## Open AI Blockers

- Base model is not locked.
- Training hardware and cost are not locked.
- Final author-approved Mataios dialogue rows are not ready.
- Unity runtime model artifact format is not locked.
