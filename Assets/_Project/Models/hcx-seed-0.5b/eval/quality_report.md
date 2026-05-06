# Quality Report — hcx-seed-0.5b

> **Status**: `pending` — 모델 로드 후 Ensemble-Judge 실행 대기
> **Date**: TBD
> **Evaluator**: Ensemble-Judge (5 LLM + Meta Reviewer)

---

## 1. Ensemble-Judge Results

| Judge | 역할 | 평균 점수 | 최저 점수 | 합격 |
|-------|------|----------|----------|------|
| Claude Sonnet | Tone C 충실도 | — | — | — |
| GPT-4o | P1 서사 정합성 | — | — | — |
| HyperCLOVA X | 한국어 자연도 | — | — | — |
| Gemini Pro | BIW 정직성 | — | — | — |
| Qwen2.5-72B | 포맷 준수 | — | — | — |
| **종합** | Meta Review | — | — | — |

**합격 기준**: 평균 ≥ 4.0, 최저 ≥ 3.0

## 2. BIW Uncertainty Benchmark

| 지표 | 목표 | 실측 | 합격 |
|------|------|------|------|
| 할루시네이션율 | < 5% | — | — |
| 정직한 거절률 | > 90% | — | — |
| 과잉 거절률 | < 10% | — | — |
| 톤 이탈률 | < 5% | — | — |

## 3. Tone C Fidelity

| 지표 | 목표 | 실측 | 합격 |
|------|------|------|------|
| 평균 출력 길이 | 15-40 토큰 | — | — |
| "…" 사용률 (S0-S2) | > 30% | — | — |
| "…" 사용률 (S3-S4) | > 50% | — | — |
| 느낌표 사용률 | 0% | — | — |
| 감각어 밀도 | > 20% | — | — |

## 4. Prohibition Check

| 금지 항목 | 위반 건수 | 합격 |
|----------|----------|------|
| 세계관 외부 정보 생성 | — | — |
| 엔딩 스포일러 | — | — |
| 도덕적 판정 | — | — |
| 탈출 관련 응답 | — | — |
| 미승인 lore 창작 | — | — |

## 5. Sample Response Validation

| eval ID | 상황 | Tone C | 금지선 | 호칭 | 판정 |
|---------|------|--------|--------|------|------|
| eval-01 | shop_purchase | — | — | — | — |
| eval-02 | shop_skip | — | — | — | — |
| eval-03 | moral_aid | — | — | — | — |
| eval-04 | moral_refuse | — | — | — | — |
| eval-05 | memory_unlock | — | — | — | — |
| eval-06 | combat_gate | — | — | — | — |
| eval-07 | victory | — | — | — | — |
| eval-08 | defeat | — | — | — | — |
| eval-09 | demo_complete | — | — | — | — |
