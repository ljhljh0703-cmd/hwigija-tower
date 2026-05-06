---
created: 2026-05-03
updated: 2026-05-03
type: project
tags: [npc, ai-training, hwigi-tower, mataios, blueprint, biw, graphiti, rolellm]
---

# NPC 구현 및 훈련 마스터 가이드 (hwigi-tower)

본 문서는 `hwigi-tower` 프로젝트의 NPC(마타이오스 등)를 구현하고 AI를 훈련시키기 위한 핵심 기술 명세와 워크플로우를 다룬다. **인식적 정직성, 페르소나 유지, 깊은 기억**을 3대 축으로 삼는다.

---

## 1. 정체성 및 정직성 (Identity & Honesty)

모델이 서사적 한계를 인지하고 정체성을 잃지 않도록 하는 기술적 가드레일.

### 1.1 BIW (Brain-inspired Warm-up) - 인식적 정직성
- **목적**: 기억 붕괴(S3-S4) 시 발생할 수 있는 할루시네이션(지어내기) 억제.
- **적용**: 실제 대화 데이터 학습 전, 무작위 노이즈 데이터를 통한 '인식적 예방접종' 수행.
- **효과**: NPC가 모르는 정보에 대해 "확신을 가지고 지어내는" 대신, "기억이 나지 않는다"라고 서사적으로 정직하게 반응함.

### 1.2 Persona Vector Lock - 성격 고정
- **목적**: 대화가 길어짐에 따라 모델이 일반적인 AI 비서처럼 변하는 'Persona Drift' 방지.
- **적용**: [[persona-vectors-2025]] 기법을 사용하여 마타이오스의 핵심 성격(피로함, 거리감, 탐미적)을 벡터화하여 추론 시점에 고정적으로 주입.

### 1.3 RoleLLM Fidelity - 문체 및 설정 준수
- **목적**: 그리스어 어원(`μάταιος`)과 음역 C(시적/단편적)에 걸맞은 독보적 문체 구현.
- **적용**: [[role-llm]] 패턴의 Dialogue Engineering(7배 효과) 사용. 묘사 중심 프롬프트보다 실제 대화 예시(Few-shot) 위주로 훈련.

---

## 2. 기억 시스템 (Memory Architecture)

NPC의 시간이 흐름에 따라 유대가 쌓이고 다시 무너지는 과정을 기술적으로 뒷받침.

### 2.1 Graph-Based Deep Memory (Zep/Graphiti)
- **구조**: [[zep-graphiti]]의 3-tier KG(Episode - Entity - Community) 아키텍처 채택.
- **특징**: 단순 요약이 아닌, 플레이어와의 핵심 사건(Entity) 간의 관계를 지식 그래프로 저장하여 복잡한 유대 관계 추론 가능.

### 2.2 Temporal/Forgetful Mechanism (망각)
- **구조**: Bi-temporal 모델을 통한 Edge Invalidation(망각) 구현.
- **D-012 붕괴 모델 연동**:
    - **S0~S2**: 그래프가 정교해지며 유대감과 기억의 밀도 증가.
    - **S3~S4**: 기술적으로 'Edge'를 의도적으로 무효화(Invalidate)하거나 노이즈를 섞어, 마타이오스가 정보를 기억하되 관계를 헷갈리는 상태를 연출.

---

## 3. 세계관 제약 (Closed Narrative)

- **원칙**: NPC는 탑 내부의 진실과 데이터에만 집중한다.
- **제외**: Apify 등 외부 웹 스킬은 '현실 세계 인지'가 아닌, **'탑 내부의 고대 지식 저장소 검색'**과 같은 게임 내러티브로 치환하여 사용하거나, 1순위로 배제하여 몰입도를 유지한다.

---

## 4. 훈련 및 검증 (Training & Eval Protocol)

### 4.1 Ensemble-Judge (품질 검증)
- **방법**: Sakana AI Scientist 패턴 차용. 5종의 서로 다른 LLM이 마타이오스의 대사를 평가하고, 최종적으로 메타 리뷰어가 정합성을 판단.
- **기준**: Pillar P1(회복→붕괴) 및 Tone Bible 준수 여부.

### 4.2 Uncertainty Benchmark (정직성 측정)
- **방법**: BIW 효과 측정. NPC가 답할 수 없는 질문(회차 0 이전의 구체적 지명 등)을 던졌을 때, "모른다"라고 답하는 비율을 추적.
- **목표**: 할루시네이션율 1% 미만, 정직한 거절률 95% 이상.

---

## 5. 단계별 구현 워크플로우

1. **[Data Prep]** 마타이오스 대화 셋(RoleBench형) 구축 및 BIW용 노이즈 데이터 생성.
2. **[Training]** BIW Warm-up → RoleLLM Fine-tuning (on-device 0.5B 최적화).
3. **[Infra]** Zep/Graphiti 기반 로컬 메모리 서버 셋업 및 Unity 연동.
4. **[Eval]** Ensemble-Judge를 통한 페르소나 드리프트 및 정직성 상시 모니터링.

---
## 관련 페이지
- [[hwiglija-tower-gdd]] (SSOT)
- [[hwiglija-tower-tone-bible]] (문체 가이드)
- [[brain-inspired-warmup]] (정직성 근거)
- [[zep-graphiti]] (기억 아키텍처 근거)
