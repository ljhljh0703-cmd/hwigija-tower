# Qwen2.5-1.5B-Instruct — Model Manifest

> **Status**: `standby` — D-005 Fallback 대기
> **Updated**: 2026-05-04
> **Model ID**: `qwen2.5-1.5b-instruct`
> **HuggingFace**: `Qwen/Qwen2.5-1.5B-Instruct`

---

## 1. Model Identity

| 항목 | 값 |
|------|---|
| Base Model | Qwen2.5-1.5B-Instruct |
| Parameters | ~1.5B |
| Architecture | Qwen2 (MLC-LLM 공식 지원) |
| Context Length | 32K (사용: 512 이내) |
| License | Apache 2.0 |
| 활성화 조건 | HCX-SEED 0.5B D-005 불합격 시 |

## 2. Runtime Contract

| 항목 | 값 |
|------|---|
| Runtime | mlc-llm (네이티브 지원) |
| Platform | android-arm64-vulkan |
| Input Budget | ≤ 450 tokens |
| Output Budget | ≤ 100 tokens |
| Calls per Run | ≤ 2 (reflection은 캐시 전용) |
| Cache Key | `run_id + prompt_hash` |
| Deterministic | required |

## 3. Delivery Structure

```
Assets/_Project/Models/qwen2.5-1.5b-instruct/
├── MODEL_MANIFEST.md
├── model_config.json
├── tokenizer/
├── compiled/android/
└── eval/
```

Runtime:
```
Assets/StreamingAssets/LLM/qwen2.5-1.5b-instruct/
├── compiled/android/    ← q4f16_1 MLC artifact
└── tokenizer/
```

## 4. Known Trade-offs vs HCX-SEED

| 항목 | HCX-SEED 0.5B | Qwen 1.5B |
|------|---------------|-----------|
| 한국어 | ★★★ | ★★ (LoRA 보정 필수) |
| MLC 호환 | 커스텀 변환 필요 | 네이티브 지원 |
| 모델 크기 | ~300MB | ~900MB (분리 다운로드) |
| 호출 횟수 | 3/런 | 2/런 (크기 제약) |

## 5. Current Blockers

- [ ] D-005 HCX-SEED 불합격 확정 대기
- [ ] 활성화 시 동일 BIW+SFT+LoRA 파이프라인 재실행 필요
