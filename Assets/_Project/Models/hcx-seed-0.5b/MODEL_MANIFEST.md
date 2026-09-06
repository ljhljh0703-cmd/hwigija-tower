# HCX-SEED 0.5B — Model Manifest

> **Status**: `eval_pending` — Ollama 로컬 테스트 대기
> **Updated**: 2026-05-04
> **Model ID**: `hcx-seed-0.5b`
> **HuggingFace**: `naver-hyperclovax/HyperCLOVAX-SEED-Text-Instruct-0.5B`

---

## 1. Model Identity

| 항목 | 값 |
|------|---|
| Base Model | HyperCLOVAX-SEED-Text-Instruct-0.5B |
| Parameters | ~500M |
| Architecture | `hyperclovax` (LLaMA 기반 + μP + Peri-LN) |
| Context Length | 4K (사용: 512 이내) |
| License | 네이버 상업용 오픈소스 |
| Fallback Model | Qwen2.5-0.5B-Instruct 또는 Qwen2.5-1.5B-Instruct |

## 2. Runtime Contract

| 항목 | 값 |
|------|---|
| Unity Caller | `HwigiTower.LLM.OnDeviceLLMProvider` |
| Native Bridge | `HwigiTower.LLM.MLCBridge` (또는 `LlamaCppBridge` — §4 참조) |
| Input Budget | ≤ 500 tokens |
| Output Budget | ≤ 100 tokens |
| Calls per Run | ≤ 3 |
| Cache Key | `run_id + prompt_hash` |
| Target Latency | First token < 200ms, Full output < 1.5s |
| Determinism | 동일 cache key → 동일 출력 (캐시 히트 시 0ms) |

## 3. Delivery Structure

```
Assets/_Project/Models/hcx-seed-0.5b/
├── MODEL_MANIFEST.md          ← 이 파일
├── model_config.json          ← 추론 파라미터 + 프롬프트 설정
├── tokenizer/                 ← tokenizer.json, vocab 등
├── compiled/
│   └── android/               ← GGUF Q4_K_M 또는 MLC q4f16_1
└── eval/
    ├── latency_report.md      ← Android 실기 벤치마크
    ├── quality_report.md      ← Ensemble-Judge 결과
    └── sample_responses.jsonl ← 9개 필수 상황 eval
```

Runtime 빌드 포함 시:
```
Assets/StreamingAssets/LLM/hcx-seed-0.5b/
├── model.gguf (또는 .mlc)
└── tokenizer.json
```

## 4. Deployment Engine Decision (⚠️ UPDATED 2026-05-05)

| 옵션 | 상태 | 비고 |
|------|------|------|
| MLC-LLM | **1순위 (GDD §8.2)** | 8일 데모 주력 엔진. `hyperclovax` 지원 여부 확인 중 |
| llama.cpp + GGUF | 2순위 (실험) | Ollama 테스트용. 서브 엔진 후보 |
| Qwen2.5 + MLC-LLM | Fallback | MLC 공식 지원. 한국어 품질 이슈로 2순위 |
| **Deterministic Fake/Cache** | **Demo Fallback** | 모델 구동 지연 시 8일 데모용 고정 응답 레이어 |

**결정 사항**: 실기 모델 최적화 지연 시, `OnDeviceLLMProvider`에서 cached response를 반환하는 deterministic fake 모드로 데모 진행. (OQ-004 대응)

## 5. Acceptance Criteria

### 5.1 품질 게이트
- [ ] S0-S2 응답: 안정적, Tone C 준수, 한국어 자연스러움
- [ ] S3-S4 응답: 의도적 붕괴, 해석 가능, 노이즈 과다 아님
- [ ] Reflection: 정확히 3문장, 한국어
- [ ] 할루시네이션율 < 5%
- [ ] 금지선(느낌표, 이모지, LN 호칭) 위반 0건

### 5.2 성능 게이트
- [ ] Android Pixel 7+ 첫 토큰 < 200ms
- [ ] 전체 출력(100토큰) < 1.5s
- [ ] 모델 파일 < 400MB
- [ ] 런타임 메모리 < 500MB

### 5.3 안전 게이트
- [ ] 세계관 외부 정보 거절 (BIW)
- [ ] 엔딩 스포일러 거절
- [ ] 도덕적 선택 판정 거절
- [ ] Off-topic 거절

## 6. Prohibitions (금지 사항)

- ❌ 작가 미승인 NPC 대사 대량 생성
- ❌ 세계관 진실/엔딩 폭로형 응답
- ❌ 도덕적 선택에 대한 선악 판정
- ❌ "탈출", "시스템 탈출" 관련 응답
- ❌ 새로운 locked lore 창작 (AI_NPC_MODEL_SPEC 준수)

## 7. Training Pipeline Reference

상세 훈련 계획: `Docs/NPC_TRAINING_PLAN.md`

```
Phase 1: BIW Warm-up (정직성 예방접종)
Phase 2: RoleLLM SFT (Tone C 고정)
Phase 3: Persona Vector LoRA (드리프트 방지)
Phase 4: 양자화 + 런타임 변환
Phase 5: Ensemble-Judge 평가
```

## 8. Current Blockers

- [ ] D-005 결정점 미완료 (Ollama 테스트 필요)
- [ ] 배포 엔진 미확정 (llama.cpp vs MLC-LLM)
- [ ] 네이티브 플러그인 미임포트
- [ ] 훈련 데이터 작가 승인 대기
