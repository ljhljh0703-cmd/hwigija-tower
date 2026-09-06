# Latency Report — hcx-seed-0.5b

> **Status**: `pending` — Android 실기 테스트 대기
> **Date**: TBD
> **Device**: TBD (Pixel 7 이상 권장)

---

## 1. Test Environment

| 항목 | 값 |
|------|---|
| Device | — |
| OS | Android — |
| RAM | — |
| GPU | — |
| Engine | llama.cpp / MLC-LLM (TBD) |
| Quantization | Q4_K_M / q4f16_1 (TBD) |
| Model Size | — MB |

## 2. Latency Measurements (10회 평균)

| 지표 | 목표 | 실측 | 합격 |
|------|------|------|------|
| First Token Latency | < 200ms | — | — |
| Full Output (100 tokens) | < 1500ms | — | — |
| Tokens/sec | > 66 t/s | — | — |
| Cache Hit Response | < 5ms | — | — |

## 3. Resource Usage

| 지표 | 목표 | 실측 | 합격 |
|------|------|------|------|
| Model File Size | < 400MB | — | — |
| Peak RSS Memory | < 500MB | — | — |
| Battery per 3 calls | < 1% | — | — |

## 4. M3 Mac Local Benchmark (참고용)

| 지표 | 실측 |
|------|------|
| Ollama Load Time | — |
| First Token | — |
| Tokens/sec | — |
| Memory | — |

## 5. Stress Test

| 시나리오 | 결과 |
|----------|------|
| 연속 3회 호출 (1런 최대) | — |
| 장시간 대기 후 재호출 | — |
| 메모리 부족 환경 (2GB free) | — |
