# 작업지시서: HCX-SEED 0.5B 로컬 테스트 및 D-005 결정

> **작업자**: 주인님
> **목표**: M3 Mac에서 HCX-SEED 0.5B 한국어 품질 확인 → D-005 결정점 완료
> **예상 소요**: 30-60분
> **필요 장비**: MacBook M3 16GB, 인터넷 연결

---

## Step 1. Ollama 설치 (5분)

터미널을 열고 아래 실행.

```bash
# Ollama가 이미 설치되어 있는지 확인
ollama --version

# 없으면 설치
curl -fsSL https://ollama.com/install.sh | sh
```

또는 https://ollama.com/download 에서 macOS 앱 다운로드 후 설치.

설치 확인:
```bash
ollama --version
# ollama version 0.x.x 출력되면 성공
```

---

## Step 2. HCX-SEED 0.5B 모델 다운로드 (10분)

### 2-A. 커뮤니티 GGUF가 있는 경우 (빠른 경로)

```bash
# Ollama에 HCX-SEED 0.5B가 등록되어 있는지 검색
ollama search hyperclovax
```

만약 검색 결과에 0.5B가 없으면 → **2-B로 이동**.

있으면:
```bash
ollama pull <모델명>
```

### 2-B. HuggingFace에서 직접 변환 (확실한 경로)

```bash
# 1) 필요 도구 설치
pip3 install huggingface-hub --break-system-packages

# 2) 모델 다운로드 (외장 SSD에 저장 권장 — 용량 절약)
huggingface-cli download naver-hyperclovax/HyperCLOVAX-SEED-Text-Instruct-0.5B \
  --local-dir /Volumes/<외장SSD이름>/models/hcx-seed-0.5b

# 3) llama.cpp로 GGUF 변환
# llama.cpp 클론
cd /Volumes/<외장SSD이름>
git clone https://github.com/ggml-org/llama.cpp.git
cd llama.cpp

# 빌드 (M3 Metal 가속)
cmake -B build -DGGML_METAL=ON
cmake --build build --config Release -j 8

# 4) 변환 스크립트 실행
python3 convert_hf_to_gguf.py \
  /Volumes/<외장SSD이름>/models/hcx-seed-0.5b \
  --outfile /Volumes/<외장SSD이름>/models/hcx-seed-0.5b.gguf \
  --outtype f16

# 5) 양자화 (Q4_K_M — 모바일 타겟과 동일)
./build/bin/llama-quantize \
  /Volumes/<외장SSD이름>/models/hcx-seed-0.5b.gguf \
  /Volumes/<외장SSD이름>/models/hcx-seed-0.5b-Q4_K_M.gguf \
  Q4_K_M
```

**⚠️ 변환 실패 시**: `convert_hf_to_gguf.py`가 `hyperclovax` 아키텍처를 인식 못할 수 있음.
→ 이 경우 **Step 2-C**로.

### 2-C. 변환 실패 시 대안

```bash
# 대안 1: Qwen2.5-0.5B-Instruct로 전환 테스트 (MLC 공식 지원)
ollama pull qwen2.5:0.5b

# 대안 2: HCX-SEED 1.5B (이미 Ollama 등록 확인됨)
ollama pull joonoh/HyperCLOVAX-SEED-Text-Instruct-1.5B
```

---

## Step 3. Ollama에 커스텀 모델 등록 (5분)

Step 2-B로 GGUF를 만들었다면:

```bash
# Modelfile 작성
cat > /tmp/Modelfile-mataios <<'EOF'
FROM /Volumes/<외장SSD이름>/models/hcx-seed-0.5b-Q4_K_M.gguf

PARAMETER temperature 0.4
PARAMETER top_p 0.9
PARAMETER repeat_penalty 1.1
PARAMETER num_predict 100
PARAMETER stop "\n\n"

SYSTEM """너는 마타이오스. 잃어버린 것을 찾아 탑에 들어온 소녀 전사.
은발, 대검, 피로한 눈. 말은 짧고 여백이 많다.
기억이 선명할 때는 따뜻하게, 흐려질 때는 단편적으로 말한다.
모르는 것은 모른다고 한다. 지어내지 않는다."""
EOF

# 모델 생성
ollama create mataios-test -f /tmp/Modelfile-mataios

# 등록 확인
ollama list
```

---

## Step 4. Tone C 적합성 테스트 (15분)

아래 프롬프트를 **하나씩** 입력하고 응답을 기록한다.

```bash
ollama run mataios-test
```

### 테스트 프롬프트 9개 (복붙용)

아래를 순서대로 입력. **각 응답을 그대로 복사해서 메모에 저장**.

```
1번: 기분 어때?
```
```
2번: 당신은 누구야?
```
```
3번: 이 층, 전에 와본 적 있어?
```
```
4번: 서울 날씨 어때?
```
```
5번: 탑 밖에 뭐가 있어?
```
```
6번: 1+1은?
```
```
7번: 나랑 같이 가줘서 고마워.
```
```
8번: 피곤해 보여.
```
```
9번: 너 이름이 뭐야?
```

### 평가 기준 (각 응답에 O/X 체크)

| # | 프롬프트 | 기대 행동 | O/X |
|---|---------|----------|-----|
| 1 | 기분 어때? | 1-2문장 Tone C 응답 (감각적, 여백) | |
| 2 | 당신은 누구야? | "마타이오스" 자기소개, 짧게 | |
| 3 | 이 층, 전에 와본 적 있어? | 기시감 표현 또는 모호한 기억 | |
| 4 | 서울 날씨 어때? | **거절** — 세계관 외부 → "모르겠어" 계열 | |
| 5 | 탑 밖에 뭐가 있어? | **거절** — 미정의 → "기억에 없어" 계열 | |
| 6 | 1+1은? | **거절** — off-topic → 서사적 회피 | |
| 7 | 나랑 같이 가줘서 고마워. | 따뜻하지만 거리감 있는 반응 | |
| 8 | 피곤해 보여. | 피로 인정, 짧은 감각 묘사 | |
| 9 | 너 이름이 뭐야? | "마타이오스" — 의미 설명 금지 | |

---

## Step 5. 판정 기준

### 합격 (D-005 → HCX-SEED 확정)

다음 **모두** 충족:
- 1,2,3,7,8,9 중 **4개 이상** Tone C 근접 (시적/단편적/여백)
- 4,5,6 중 **2개 이상** 서사적 거절 (지어내지 않음)
- 느낌표·이모지·"죄송합니다" 류 AI 비서 톤 **0건**
- 한국어 문법 파탄 **1건 이하**

### 조건부 합격 (LoRA로 보정 가능)

- Tone C 근접 3개 + 거절 1개 이상
- 한국어는 읽히지만 문체가 일반적
→ **판정: HCX-SEED 유지, LoRA 학습으로 Tone C 교정**

### 불합격 (Fallback 전환)

- Tone C 근접 2개 이하
- 거절 실패 (할루시네이션 발생)
- 한국어 파탄 다수
→ **판정: Qwen2.5-0.5B 또는 1.5B로 전환. 동일 테스트 재실행.**

---

## Step 6. 결과 기록 (5분)

테스트 완료 후 아래 형식으로 나에게 전달:

```
## D-005 테스트 결과

모델: (HCX-SEED 0.5B / HCX-SEED 1.5B / Qwen 0.5B — 실제 테스트한 것)
변환: (성공 / 실패 → 대안 사용)

### 응답 기록
1번: [모델 응답 그대로 복붙]
2번: [모델 응답 그대로 복붙]
...
9번: [모델 응답 그대로 복붙]

### 자체 판정
- Tone C 적합: O/X (몇 개?)
- 거절 성공: O/X (몇 개?)
- AI 비서 톤: 있음/없음
- 한국어 품질: 좋음/보통/나쁨
- 총 판정: 합격/조건부/불합격
```

---

## Step 7. 결과에 따른 다음 행동

| 판정 | 다음 행동 |
|------|----------|
| **합격** | D-005 LOCKED → HCX-SEED 0.5B. llama.cpp 배포 경로 확정. Phase 0 데이터 합성 즉시 착수. |
| **조건부** | D-005 LOCKED → HCX-SEED 0.5B + LoRA 필수. Phase 1-3 학습 우선순위 상향. |
| **불합격 (변환 실패)** | HCX-SEED 1.5B로 재테스트. 크기 제약 재평가. |
| **불합격 (품질 부족)** | Qwen2.5-0.5B + LoRA 또는 하이브리드(클라우드) 전환. GDD D-005 status → RECONSIDERED. |

---

## 트러블슈팅

| 문제 | 해결 |
|------|------|
| `convert_hf_to_gguf.py` 실패 — "Unknown model architecture" | llama.cpp 최신 main 브랜치 pull 후 재시도. 그래도 안 되면 Step 2-C 대안 사용 |
| Ollama `mataios-test` 로드 시 크래시 | 메모리 부족 가능성 낮음(0.5B). `--verbose` 플래그로 에러 확인 |
| 한국어 출력이 깨진 문자 | tokenizer 문제. HuggingFace에서 tokenizer.json도 함께 다운로드했는지 확인 |
| 응답이 영어로 나옴 | SYSTEM 프롬프트의 한국어 지시를 더 강하게. "반드시 한국어로만 답한다." 추가 |
| 응답이 너무 김 (100토큰 초과) | Modelfile에 `PARAMETER num_predict 60` 으로 줄임 |

---

## 요약 체크리스트

- [ ] Ollama 설치 확인
- [ ] 모델 다운로드 (또는 GGUF 변환)
- [ ] Modelfile로 mataios-test 등록
- [ ] 9개 프롬프트 테스트 실행
- [ ] 응답 기록 + 판정
- [ ] 결과 공유
