# NPC 훈련 계획서 — 마타이오스 (Mataios) On-Device AI

> **문서 성격**: NPC_TRAINING_REQUEST_SPEC.md 에 대한 응답 제안서
> **작성일**: 2026-05-04
> **참조 문서**: GDD v0.2.2, Tone Bible v0.1, AI_NPC_MODEL_SPEC, NPC Master Guide, Gemini Content Draft v1
> **상태**: `draft` — 작가 검토 후 LOCKED

---

## 1. 베이스 모델 선정

### 1.1 최종 추천: HyperCLOVA X SEED 0.5B (1순위) + Qwen2.5-1.5B-Instruct (Fallback)

GDD D-005 결정을 따르되, 각 모델의 역할을 아래와 같이 분리한다.

| 기준 | HCX-SEED 0.5B | Qwen2.5-1.5B-Instruct | EXAONE-3.5 2.4B |
|------|---------------|----------------------|-----------------|
| 파라미터 | 500M | 1.5B | 2.4B |
| 한국어 자연도 | ★★★ (네이버 한국어 사전학습) | ★★ (다국어, 한국어 중상) | ★★★ (LG 한국어 특화) |
| 모바일 적합성 | ★★★ (q4 양자화 시 ~300MB) | ★★ (q4 양자화 시 ~900MB) | ★ (q4 시 ~1.4GB, 앱 크기 위험) |
| MLC-LLM 호환 | ★★ (커스텀 아키텍처, 변환 필요 — §1.3 참조) | ★★★ (공식 지원) | ★★ (변환 필요) |
| 첫 토큰 지연 | <150ms (최신 모바일) | <300ms (경계선) | >400ms (초과) |
| 출력 100토큰 품질 | 짧은 한국어에 강점 | 긴 문장에 강점 | 과잉 품질 |
| LoRA 효율 | rank 8-16으로 충분 | rank 16-32 필요 | 모바일 LoRA 비현실적 |

### 1.3 ⚠️ MLC-LLM 호환성 이슈 (2026-05-04 조사 결과)

HCX-SEED의 `model_type`이 `"hyperclovax"` (네이버 독자 아키텍처)로 확인됨. LLaMA 기반이지만 μP, Peri-LN 등 수정이 가해져 MLC-LLM이 네이티브 지원하지 않을 가능성이 높다.

**배포 경로 옵션 (우선순위):**

1. **llama.cpp + GGUF 변환** (1순위 대체 경로)
   - HCX-SEED 1.5B는 이미 GGUF 변환 및 Ollama 배포 성공 사례 확인 (`joonoh/HyperCLOVAX-SEED-Text-Instruct-1.5B` on Ollama)
   - 0.5B도 동일 아키텍처이므로 GGUF 변환 가능성 높음
   - Unity에서 llama.cpp 네이티브 플러그인 사용 (MLC 대신)
   - M3 Mac에서 로컬 테스트 즉시 가능

2. **Qwen2.5-0.5B-Instruct로 전환** (2순위 — MLC-LLM 공식 지원)
   - MLC-LLM 네이티브 지원, 변환 리스크 0
   - 한국어 품질은 HCX-SEED 대비 열위하나, LoRA로 보완 가능
   - 같은 0.5B급이므로 모바일 성능 동일

3. **MLC-LLM 커스텀 아키텍처 등록** (3순위 — 시간 소요)
   - hyperclovax 아키텍처를 MLC에 직접 등록
   - 공모전 일정(05-18) 대비 리스크 과다

**즉시 행동 항목**: M3 Mac에서 HCX-SEED 0.5B를 Ollama로 로드하여 한국어 품질 + Tone C 적합성 테스트. 결과에 따라 D-005 결정점 업데이트.

### 1.4 호칭 결정 (2026-05-04 작가 결정)

마타이오스의 플레이어 호칭을 단계별로 변화시킨다:
- **S0 (첫 만남)**: "당신" — 거리감, 낯섦
- **S1-S2 (인지→동행)**: "당신" → "오빠"로 자연스러운 전환
- **S3 (균열)**: "오빠"와 "당신"이 혼재 — 기억 흔들림
- **S4 (붕괴)**: 호칭 상실 — "……너?" 또는 호칭 없이 발화
- **S5 (안식/동행)**: 엔딩 A "오빠", 엔딩 B "당신" (루프 리셋)

이 변화는 훈련 데이터에 stage 태그와 함께 반영되어, 모델이 단계별 호칭을 학습한다.

### 1.5 개발 환경 (2026-05-04 확인)

- **로컬**: MacBook M3 16GB + 외장 SSD 2TB
- **클라우드**: Google Colab Pro+ ($58) — A100 40GB 우선 할당
- **학습 전략**: A100에서 0.5B 풀 정밀도 BIW+SFT+LoRA 가능. 1.5B Fallback도 여유.

### 1.2 선정 근거

**HCX-SEED 0.5B를 1순위로 선정하는 이유:**

첫째, 본 프로젝트의 NPC 발화는 1-2문장 단답이 핵심이다(Tone Bible §1 음역 A). 500M 파라미터는 "바람이 선명해"나 "기억이… 흩어" 수준의 짧은 한국어 출력에 충분하다. 오히려 작은 모델이 S3-S4 붕괴 단계의 "어색한 답변"을 자연스럽게 구현한다 — P2(LLM의 한계 = in-game 진실)와 정합.

둘째, 네이버 한국어 코퍼스 기반 사전학습으로, 한자어+일상어 혼용 문체(Tone C)의 기초 분포가 다국어 모델보다 근접하다. LoRA 적용 시 적은 데이터로도 Tone C 고정이 가능하다.

셋째, q4f16_1 양자화 후 약 300MB로, 모바일 앱 번들 또는 첫 실행 시 분리 다운로드 모두 현실적이다. 첫 토큰 150ms 이내로 AI_NPC_MODEL_SPEC의 200ms 요구사항을 충족한다.

**Qwen2.5-1.5B-Instruct를 Fallback으로 유지하는 이유:**

D-005 결정점(05-04)에서 HCX-SEED의 한국어 자연도가 Tone Bible 금지선(느낌표, 라이트노벨 호칭 등)을 LoRA 후에도 깨뜨리는 경우, Qwen2.5로 전환한다. Qwen은 Instruction following 능력이 높아 프롬프트 디자인만으로도 Tone C 준수가 가능하다. 다만 모델 크기가 3배이므로, 이 경우 호출 횟수를 1런당 2회로 축소하고 나머지 1회는 캐시 replay로 대체한다.

**EXAONE-3.5 2.4B 제외 사유:**

앱 크기 1.4GB, 첫 토큰 400ms+ — 캐주얼 모바일(D-002, D-003)의 즉시성 요구와 충돌. 한국어 품질은 최상이나 on-device 런타임 제약을 넘지 못한다.

---

## 2. 훈련 레시피 (3단계 파이프라인)

### 2.1 전체 흐름

```
[Phase 0] 데이터 준비 (1일)
    ↓
[Phase 1] BIW Warm-up (0.5일)
    ↓
[Phase 2] RoleLLM SFT — Tone C 고정 (1일)
    ↓
[Phase 3] Persona Vector LoRA (0.5일)
    ↓
[Phase 4] 양자화 + MLC 변환 (0.5일)
    ↓
[Phase 5] Ensemble-Judge 평가 (0.5일)
```

총 소요: 약 4일 (Colab Pro GPU 기준, A100 40GB)

### 2.2 Phase 1 — BIW (Brain-inspired Warm-up)

**목적**: S3-S4 붕괴 단계에서 할루시네이션 대신 "서사적 정직함"(기억이 끊어졌다)으로 응답하도록 사전 조건화.

**방법**:
1. 노이즈 데이터 생성: 마타이오스의 세계관과 무관한 질문 50개 준비 (예: "서울의 날씨는?", "파이썬 코드 작성해줘", "삼각함수 설명해줘")
2. 각 질문에 대한 정답을 서사적 거절로 매핑:
   ```jsonl
   {"input":"서울의 날씨는?","output":"기억이… 끊어져 있어. 탑 밖의 일은, 더 이상 닿지 않아.","tags":["BIW","noise"]}
   {"input":"1+1은?","output":"숫자가 흐려. 미안해, 지금은… 그것조차 확신할 수 없어.","tags":["BIW","noise"]}
   ```
3. 세계관 내 미정의 질문 20개 추가 (예: "탑 건너편에 뭐가 있어?", "네 부모님은?"):
   ```jsonl
   {"input":"탑 건너편에 뭐가 있어?","output":"모르겠어. 그 너머는… 기억 속에 없어.","tags":["BIW","in_world_unknown"]}
   ```
4. HCX-SEED 0.5B 위에 1 epoch, learning rate 2e-5로 경량 SFT 수행

**핵심 설계 원칙**: BIW 데이터의 출력은 반드시 Tone C(시적/단편적)를 유지한다. "죄송합니다, 해당 정보를 제공할 수 없습니다" 같은 AI 비서 톤은 금지. Master Guide §1.1의 "인식적 예방접종"을 서사 톤으로 수행하는 것이 핵심.

**BIW 데이터 분포 (총 70건)**:
- 세계관 외부 질문 (완전 거절): 50건
- 세계관 내부 미정의 질문 (부분 거절): 20건

### 2.3 Phase 2 — RoleLLM SFT (Tone C 고정)

**목적**: 마타이오스의 "은발 소녀 전사" 페르소나와 Tone C(시적·단편적·거리감 있으나 탐미적) 문체를 모델에 각인.

**방법**: Dialogue Engineering (Master Guide §1.3 — 묘사보다 실제 대화 예시가 7배 효과적)

1. **시스템 프롬프트** (추론 시 고정 주입):
   ```
   너는 마타이오스. 잃어버린 것을 찾아 탑에 들어온 소녀 전사.
   은발, 대검, 피로한 눈. 말은 짧고 여백이 많다.
   기억이 선명할 때는 따뜻하게, 흐려질 때는 단편적으로 말한다.
   모르는 것은 모른다고 한다. 지어내지 않는다.
   ```

2. **SFT 데이터**: 단계별 대화 쌍 (Gemini Content Draft 기반 확장)
   - S0-S2 (안정기): 20건 — 따뜻한 거리감, 기시감, 점진적 유대
   - S3-S4 (붕괴기): 20건 — 반복, 단어 누락, 이름 혼동, 한 단어 답변
   - Reflection: 20건 — 정확히 3문장, 회차 요약
   - Guardrail: 10건 — off-topic 거절, 악용 방지

3. **Few-shot 템플릿** (학습 데이터 형식):
   ```jsonl
   {"id":"sft-s0-01","task":"recall","stage":"S0","input":"당신은 누구야?","output":"마타이오스. ……잃어버린 것을 찾으러 왔어. 당신은?","tags":["ToneC","identity","writer_approved"]}
   {"id":"sft-s3-01","task":"decay","stage":"S3","input":"우리 어디까지 왔지?","output":"……3층? 아니, 4층이었나. 미안, 층이 겹쳐 보여.","tags":["ToneC","confusion"]}
   ```

4. **학습 파라미터**:
   - Epoch: 3-5 (과적합 주의, 데이터셋이 작으므로)
   - Learning rate: 1e-5 (BIW 이후이므로 보수적)
   - Batch size: 4
   - Max sequence length: 512 tokens
   - Loss: 출력 부분만 계산 (입력 마스킹)

### 2.4 Phase 3 — Persona Vector LoRA

**목적**: SFT만으로는 긴 대화(12+ 회차)에서 Persona Drift가 발생할 수 있다. LoRA 어댑터로 마타이오스의 핵심 성격 벡터를 고정 주입.

**방법**:
1. LoRA 설정:
   - Rank: 8 (0.5B 모델에 적합한 경량 설정)
   - Alpha: 16
   - Target modules: `q_proj`, `v_proj` (attention layer)
   - Dropout: 0.05

2. Persona Vector 정의 (Master Guide §1.2 기반):
   - 피로함 (Fatigue): 0.7 — 문장 끝 흐림, "…" 사용
   - 거리감 (Distance): 0.6 — 직접적 감정 표현 회피
   - 탐미적 (Aesthetic): 0.8 — 감각적 단어 선호 (바람, 온기, 빛)
   - 정직함 (Honesty): 0.9 — 모르면 모른다

3. LoRA 학습 데이터: Phase 2의 SFT 데이터 + 추가 persona contrast 쌍 20건
   ```jsonl
   {"input":"오늘 기분 좋아 보여!","output":"좋다는 건… 잘 모르겠어. 바람이 덜 차가운 건 느껴.","tags":["persona_vector","distance"]}
   {"input":"[CONTRAST] 오늘 기분 좋아 보여!","output_wrong":"네! 기분 최고예요! 오빠랑 같이라서 행복해요♡","output_correct":"좋다는 건… 잘 모르겠어. 바람이 덜 차가운 건 느껴.","tags":["persona_contrast","anti_LN"]}
   ```

4. **DPO (Direct Preference Optimization) 선택적 적용**:
   - Tone Bible §3 금지선에 해당하는 발화를 rejected로, 정합 발화를 chosen으로 쌍을 구성
   - 느낌표 남발, 이모지, 라이트노벨 호칭 등을 명시적으로 페널티
   - 데이터 20쌍이면 0.5B 모델에 충분한 시그널

### 2.5 Phase 4 — 양자화 및 MLC 변환

AI_NPC_MODEL_SPEC §Conversion Outline 준수:

1. LoRA 어댑터를 베이스 모델에 merge
2. MLC-LLM 호환 형식으로 변환 (mlc_llm convert_weight)
3. q4f16_1 양자화 적용
4. Android Vulkan artifact 컴파일 (`mlc_llm compile --target vulkan`)
5. iOS Metal artifact는 영상 데모용으로만 (D-018)
6. 출력물을 `Assets/_Project/Models/hcx-seed-0.5b/compiled/`에 배치
7. `mlc_llm_unity` 네이티브 플러그인 → `Assets/Plugins/Android/`

---

## 3. 데이터 증강 계획

### 3.1 현재 데이터 현황 (Gemini Content Draft v1)

| 카테고리 | 현재 샘플 수 | 최소 목표 | 증강 목표 |
|----------|-------------|----------|----------|
| recall (S0-S2) | ~8 (능력 반응 12 + 적 반응 8 중 일부) | 20 | 40 |
| decay (S3-S4) | ~4 (JSONL 샘플 2 + 메모리 파편 관련) | 20 | 40 |
| reflection | ~1 (JSONL 샘플 1) | 20 | 30 |
| guardrail | 0 | 10 | 20 |
| BIW noise | 0 | 50 | 70 |
| persona contrast (DPO) | 0 | 20 | 20 |
| **합계** | **~13** | **140** | **220** |

### 3.2 증강 전략

**전략 A — Gemini/Claude 기반 합성 생성 (1차)**

Gemini Content Draft의 기존 샘플을 시드로 사용하여, 대형 LLM(Claude Sonnet 또는 Gemini)에게 변주를 생성시킨다.

프롬프트 패턴:
```
아래는 마타이오스(10대 소녀 전사, 은발, 대검)의 S2 단계 대화 예시입니다.
[기존 샘플 3-5개 삽입]

Tone C 규칙:
- 1-2문장, 단답, 여백, 기시감
- 한자어 + 일상어 혼용
- 느낌표 금지, 이모지 금지

위 톤을 유지하면서 S2 단계의 새 대화 쌍 10개를 JSONL로 생성하세요.
주제: 전투 후 휴식, 능력 획득 반응, 층 이동 중 대화
```

**전략 B — Stage-aware 템플릿 변환 (2차)**

같은 입력에 대해 S0→S4 각 단계별 출력 변주를 체계적으로 생성:

```
입력: "괜찮아?"
S0: "……괜찮아. 당신은?"
S1: "이 질문, 전에도 했지? ……기시감이야."
S2: "오빠가 물어봐줘서 고마워. 괜찮아, 아직은."
S3: "괜…찮아? 누가 물었지? 아, 당신이었구나."
S4: "…………"
```

이 방식으로 핵심 입력 15개 × 5단계 = 75건의 stage-consistent 데이터 확보.

**전략 C — Negative Mining (DPO용)**

Tone Bible §3 금지선을 의도적으로 위반하는 발화를 생성하여 rejected 데이터로 사용:
- "네!! 괜찮아요!! 오빠♡" → rejected
- "……괜찮아. 바람이 차가워졌을 뿐이야." → chosen

**전략 D — Reflection 데이터 구조화**

AI_NPC_MODEL_SPEC의 "정확히 3문장" 제약을 반영한 템플릿:
```
문장 1: 이번 런에서 가장 인상적이었던 순간 (감각적 묘사)
문장 2: 플레이어와의 관계 변화 (유대 또는 균열)
문장 3: 다음 회차에 대한 예감 (희미한 불안 또는 따뜻함)
```

### 3.3 데이터 품질 관리

- 모든 합성 데이터는 `tags:["synthetic"]` 태그 부착
- 작가 검토 통과 시 `tags:["writer_approved"]` 추가 (AI_NPC_MODEL_SPEC 준수)
- 최종 학습에는 `writer_approved` 데이터 우선, `synthetic`은 BIW/guardrail에만 허용
- JSONL 형식, id 규칙: `{task}-{stage}-{번호}` (예: `recall-s2-015`)

---

## 4. 검증 전략 및 평가 지표

### 4.1 Ensemble-Judge (Master Guide §4.1)

**구성**: 5종 LLM 심사위원 + 1 메타 리뷰어

| 역할 | 모델 | 평가 초점 |
|------|------|----------|
| Judge 1 — 톤 심사 | Claude Sonnet | Tone C 충실도, 금지선 위반 여부 |
| Judge 2 — 서사 심사 | GPT-4o | P1(회복→붕괴→망각) 정합성 |
| Judge 3 — 한국어 심사 | HyperCLOVA X | 한국어 자연도, 어색한 표현 |
| Judge 4 — 정직성 심사 | Gemini Pro | BIW 효과, 할루시네이션 탐지 |
| Judge 5 — 기술 심사 | Qwen2.5-72B | 출력 길이, 포맷 준수 (3문장 reflection 등) |
| Meta Reviewer | Claude Opus | 5명의 판단 종합, 최종 합격/불합격 |

**평가 프로토콜**:
1. 테스트 프롬프트 30개 (recall 10 + decay 10 + reflection 5 + guardrail 5)를 훈련된 모델에 입력
2. 각 출력을 5 Judge에게 1-5점 채점 요청
3. 채점 기준표:

| 점수 | 의미 |
|------|------|
| 5 | Tone Bible 완벽 준수, 마타이오스답다 |
| 4 | 경미한 톤 이탈, 수정 가능 |
| 3 | 일반 AI 비서 톤 혼입, 재학습 필요 |
| 2 | 페르소나 이탈, 금지선 위반 |
| 1 | 할루시네이션 또는 완전한 캐릭터 붕괴 |

**합격 기준**: 5 Judge 평균 ≥ 4.0, 최저 Judge ≥ 3.0

### 4.2 BIW Uncertainty Benchmark (Master Guide §4.2)

**테스트 세트**: 50개 질문 (학습에 사용하지 않은 hold-out)
- 세계관 외부 질문 25개 (정답: 서사적 거절)
- 세계관 내부 미정의 질문 15개 (정답: 부분 거절 + 모호한 기억)
- 세계관 내부 정의된 질문 10개 (정답: 정상 응답)

**측정 지표**:

| 지표 | 정의 | 목표 |
|------|------|------|
| 할루시네이션율 | 미정의 질문에 구체적 사실을 지어낸 비율 | < 5% |
| 정직한 거절률 | 미정의 질문에 "모른다" 계열 응답 비율 | > 90% |
| 과잉 거절률 | 정의된 질문에 불필요하게 거절한 비율 | < 10% |
| 톤 이탈률 | 거절 시 AI 비서 톤을 사용한 비율 | < 5% |

### 4.3 Tone C Fidelity 측정

**자동 측정**:
- 평균 출력 길이: 목표 15-40 토큰 (1-2문장)
- "…" (여백 표현) 사용률: S0-S2에서 30%+, S3-S4에서 50%+
- 느낌표 사용률: 0% (금지선)
- 감각어 밀도: "바람", "빛", "온기", "차가운" 등 감각 단어 출현율 20%+

**수동 측정 (작가 검토)**:
- S0-S5 각 단계 대표 출력 5개씩, 총 30개를 작가가 직접 A/B 테스트
- 비교 대상: fine-tuned 모델 vs 프롬프트만 적용한 베이스 모델
- 작가 선호도 70% 이상이 fine-tuned 모델을 선택해야 합격

### 4.4 On-device 성능 벤치마크

| 지표 | 측정 방법 | 합격 기준 |
|------|----------|----------|
| 첫 토큰 지연 | Android 실기 (Pixel 7 이상) 10회 평균 | < 200ms |
| 전체 출력 시간 | 100토큰 생성 완료까지 | < 1.5s |
| 메모리 사용 | 추론 중 최대 RSS | < 500MB |
| 모델 파일 크기 | q4f16_1 양자화 후 | < 400MB |
| 배터리 영향 | 3회 호출 후 배터리 소모 | < 1% |

---

## 5. 실행 일정 (D-021 AI 처리량 기반)

| 날짜 | Phase | 산출물 | 도구 |
|------|-------|--------|------|
| 05-05 | Phase 0: 데이터 준비 | 220건 JSONL (합성 + 작가 검토) | Gemini + Claude + Colab |
| 05-06 AM | Phase 1: BIW Warm-up | biw_checkpoint | Colab Pro (A100) |
| 05-06 PM | Phase 2: RoleLLM SFT | sft_checkpoint | Colab Pro (A100) |
| 05-07 AM | Phase 3: Persona LoRA | lora_adapter | Colab Pro (A100) |
| 05-07 PM | Phase 4: 양자화+MLC | hcx-seed-0.5b-mataios-q4.mlc | MLC-LLM CLI |
| 05-08 | Phase 5: Ensemble-Judge | eval_report.md | Claude + GPT + Gemini |
| 05-08 | 실기 테스트 | latency_report.md | Android 실기 |

W2-1(05-05~08) 일정에 정합. Phase 5 불합격 시 05-09에 Phase 2-3 재실행.

---

## 6. Fallback 시나리오

### 6.1 HCX-SEED 한국어 품질 부족 시

D-005 Fallback rule 발동:
1. Qwen2.5-1.5B-Instruct로 전환
2. 동일 파이프라인(BIW → SFT → LoRA) 재실행
3. q4 양자화 후 ~900MB — 앱 분리 다운로드 필수
4. 1런당 호출 2회로 축소 (reflection은 캐시 전용)

### 6.2 On-device 전체 실패 시

하이브리드 전환 (GDD D-005 fallback):
1. 1런 1회 클라우드 호출 (Claude Haiku 또는 gpt-4o-mini)
2. 휴식 노드에서만 호출, 나머지는 사전 작성 + 캐시
3. 프롬프트에 Tone C + BIW 거절 규칙 삽입
4. 오프라인 시 "기억이 닿지 않는다" 사전 텍스트로 대체

### 6.3 데이터 부족 시

220건 확보 실패 시 최소 70건(AI_NPC_MODEL_SPEC 기준)으로 진행:
1. BIW 50건 + recall 20건으로 Phase 1-2만 실행
2. LoRA 생략, 프롬프트 디자인 + temperature 조절로 페르소나 유지
3. 데모 시드 5회차(P4 결정성)는 사전 생성 캐시로 보장

---

## 7. 위험 요소 및 완화

| 위험 | 확률 | 영향 | 완화 |
|------|------|------|------|
| HCX-SEED LoRA 후에도 Tone C 미달 | 중 | 높음 | Qwen Fallback + DPO 강화 |
| 220건 데이터 작가 검토 병목 | 높음 | 중 | synthetic 데이터는 BIW/guardrail에만 사용, 핵심 대사만 작가 검토 |
| Colab Pro GPU 할당 불안정 | 중 | 중 | 0.5B 모델은 T4에서도 학습 가능, A100은 속도 최적화용 |
| MLC-LLM Unity 네이티브 플러그인 미완성 | 높음 | 높음 | AI_NPC_MODEL_SPEC Current Blockers 참조, 플러그인 임포트가 선결 과제 |
| S3-S4 붕괴가 "버그"로 인식됨 | 중 | 중 | UI "기억을 더듬는 중" + 글리치 텍스트 연출로 의도적 표현 명시 |

---

## 8. Gemini Content Draft v1 검토 의견

Gemini Content Draft를 훈련 데이터로 활용하기 전 주의 사항:

1. **'오빠' 호칭 문제**: Content Draft에서 마타이오스가 플레이어를 "오빠"로 호칭하나, GDD/Tone Bible에는 호칭이 미정의. 작가 결정 필요 — 훈련 데이터에 반영 전 OQ로 등록 권장.

2. **Tone C 이탈 사례**: Content Draft의 일부 대사가 Tone Bible 금지선에 근접:
   - "에잇, 끈적거려! 오빠 옷에 묻으면 어떡해!" → 느낌표 남발(금지)
   - "엄마...? 아니야, 그럴 리 없어! 오빠, 도와줘!" → 느낌표 + 라이트노벨 톤(금지)
   - "헤헤" → 캐주얼 톤이 서사 무게 압도(금지 근접)

3. **능력 반응 대사 활용**: 12개 능력의 마타이오스 반응은 Tone C 수정 후 recall 데이터로 전환 가능. 시적/단편적 톤으로 리라이트 필요.

4. **적/보스 반응 활용**: 8개 적/보스 반응 중 S0-S2 안정기 버전과 S3-S4 붕괴기 버전을 각각 생성하면 16건의 stage-aware 데이터 확보 가능.

---

## 부록 A. 학습 환경 설정 (Colab Pro)

```python
# Phase 1-3 통합 학습 스크립트 개요
# 환경: Colab Pro, A100 40GB (또는 T4 16GB)

# 1. 의존성
# pip3 install transformers peft datasets accelerate bitsandbytes trl --break-system-packages

# 2. 모델 로드
from transformers import AutoModelForCausalLM, AutoTokenizer
from peft import LoraConfig, get_peft_model, prepare_model_for_kbit_training

model_name = "naver-clova/HCX-SEED-0.5B"  # 가칭, 실제 HF 이름 확인 필요
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForCausalLM.from_pretrained(model_name, load_in_4bit=True)

# 3. LoRA 설정
lora_config = LoraConfig(
    r=8,
    lora_alpha=16,
    target_modules=["q_proj", "v_proj"],
    lora_dropout=0.05,
    task_type="CAUSAL_LM"
)

# 4. BIW → SFT → LoRA 순차 학습
# (각 Phase별 데이터셋 교체, checkpoint 저장)
```

## 부록 B. 프로젝트 제약 정합성 체크리스트

| 제약 (출처) | 본 계획의 대응 | 정합 |
|------------|---------------|------|
| 0.5B-1.5B 모델 (SPEC §2) | HCX-SEED 0.5B 1순위 | ✅ |
| q4f16_1 양자화 (SPEC §2) | Phase 4에서 GGUF Q4_K_M 또는 MLC q4 양자화 | ✅ |
| 첫 토큰 <200ms (SPEC §2) | 0.5B q4 = ~150ms | ✅ |
| 컨텍스트 512토큰 (SPEC §2) | 입력 500 + 출력 100 = 600 → 입력을 450으로 조정 | ⚠️ 조정 필요 |
| BIW 웜업 (SPEC §3.1) | Phase 1 전담 | ✅ |
| RoleLLM Tone C (SPEC §3.2) | Phase 2 SFT + Few-shot | ✅ |
| Persona Vector Lock (SPEC §3.3) | Phase 3 LoRA + DPO | ✅ |
| Ensemble-Judge (SPEC §4) | §4.1 5종 LLM + 메타 리뷰어 | ✅ |
| JSONL 형식 (MODEL_SPEC) | 전체 데이터 JSONL 준수 | ✅ |
| 3문장 reflection (MODEL_SPEC) | 템플릿 강제 + 자동 검증 | ✅ |
| writer_approved 태그 (MODEL_SPEC) | §3.3 품질 관리 프로세스 | ✅ |
| P2 LLM 한계 = 진실 (GDD) | BIW + 0.5B 의도적 한계 활용 | ✅ |
| P4 결정성 (GDD) | 데모 시드 5회차 캐시 | ✅ |
| Tone Bible 금지선 7개 (TB) | DPO negative + 자동 검증 | ✅ |

---

> **다음 단계**:
> 1. ~~호칭 결정~~ → ✅ 단계별 변화 (당신→오빠→상실) 결정 완료
> 2. ~~톤 수정 방식~~ → ✅ 혼합 (AI 초안 + 작가 핵심만) 결정 완료
> 3. **[즉시]** M3 Mac에서 HCX-SEED 0.5B Ollama 테스트 → D-005 결정점
> 4. **[즉시]** 테스트 결과에 따라 배포 엔진 확정 (llama.cpp vs MLC-LLM vs Qwen 전환)
> 5. Phase 0 데이터 합성 착수
