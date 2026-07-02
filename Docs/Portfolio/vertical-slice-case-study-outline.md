# Hwigi Tower — Vertical Slice Case Study Outline

## 한 줄 포지셔닝

AI-assisted mobile roguelike vertical slice: Android APK, deterministic companion NPC, ML-Agents combat design probe, and evidence-driven multi-agent production pipeline.

## 독자

- 게임 회사 면접관
- AI/ML 엔지니어
- 게임 기획/테크니컬 PM 포지션
- 개인 프로젝트를 실제 산출물로 닫을 수 있는지 보는 사람

## 페이지 구조

### 1. Hero

목표:

- 10초 안에 게임과 포트폴리오 성격을 이해시킨다.

핵심 문구:

- `회귀자는 탑을 오른다`
- `AI-assisted mobile roguelike vertical slice`
- `정식 출시 전 프로토타입 / Android APK 제공`

CTA:

- APK 다운로드
- AI NPC 훈련 케이스 보기
- 제작 파이프라인 보기

증거:

- APK path / Google Drive folder
- package: `com.godju.hwigitower`
- label: `회귀자는 탑을 오른다`

### 2. Game Hook

목표:

- 소비자 관점에서 "무슨 게임인가"를 먼저 납득시킨다.

내용:

- 해골이 된 짐꾼 / 탑을 오르는 회귀자
- 선택형 이벤트, 전투, 보상, 성장
- 동행자 마타이오스가 전투와 선택을 기억하는 구조

주의:

- 아직 상용 출시 완성본처럼 말하지 않는다.
- "플레이 가능한 prototype"을 기준으로 말한다.

### 3. Implemented Vertical Slice

목표:

- 실제 구현이 아이디어 수준이 아님을 보여준다.

섹션:

- Route / map selection
- Event choice
- Two-actor combat
- Rest / shop
- Boss / reward
- Ending choice

Visual:

- 실제 Unity capture 6-8장
- 각 capture에는 `Actual prototype capture` badge

### 4. Companion: Mataios

목표:

- 마타이오스가 UI 장식이 아니라 companion system임을 보여준다.

내용:

- 플레이어와 마타이오스 damage attribution 분리
- protect / support / finish 역할
- deterministic `MataiosCombatBrain`
- event/memory consequence는 전투 modifier가 아니라 consequence/log/ending-route 후보

금지:

- RL로 학습된 runtime NPC처럼 표현 금지

### 5. AI as Design Probe

목표:

- AI/ML을 기능 과시가 아니라 설계 검증 도구로 사용한 점을 설명한다.

내용:

- Exp02: SkillSpam shortcut 발견
- Exp03: Defend metric 회복 but spam baseline 미달
- Exp04: Skill gate + Defend tempo payoff로 PPO가 spam baseline을 넘어섬
- ContextPolicy가 더 안정적이어서 deterministic Mataios policy 후보로 정리

핵심 수치:

- PPO reward: `1.855968`
- AttackSpam: `1.005000`
- SkillSpam: `1.536994`
- SpamPolicyGap: `+0.318974`
- ContextPolicy: `2.101917`
- DefendSuccessRate: `0.841897`

문구:

> I used reinforcement learning not as a black-box gameplay feature, but as a combat design probe.

### 6. AI NPC Training Case Study

목표:

- 마타이오스 대화 모델 실험을 별도 기술 case study로 연결한다.

내용:

- HCX-SEED / Qwen failures
- BPE silent failure
- EXAONE 3.5 2.4B + QLoRA
- FastAPI + Unity integration direction

주의:

- 이 섹션은 dialogue AI training case study다.
- ML-Agents combat probe와 혼동하지 않는다.

### 7. Concept-to-UI Pipeline

목표:

- "AI가 만든 예쁜 이미지"가 아니라, concept image를 실제 Unity UI 작업으로 옮기는 pipeline을 보여준다.

구조:

1. Concept target image
2. Layer breakdown
3. Layout spec
4. Runtime adaptation attempt
5. Remaining gap

Badge:

- `Target visual`
- `Layer breakdown`
- `Runtime adaptation`
- `Actual capture`

금지:

- target visual을 실제 gameplay capture로 표기 금지

### 8. Production Pipeline

목표:

- 바이브코딩이 아니라 검증 가능한 제작 파이프라인으로 보이게 한다.

내용:

- Command Center / System Design / Game Dev / UI / AI / Release 역할 분리
- clean worktree
- CodeGraph preflight
- user-path fixture
- artifact SHA
- Accepted / Candidate / Not Applied
- public portfolio deploy

강조:

- 실패를 숨긴 것이 아니라, 반복되는 실패를 gate와 skill로 바꿨다.

### 9. What I Would Do Next

목표:

- 미완성 약점을 변명하지 않고 다음 개발 계획으로 정리한다.

내용:

- Unity prototype은 portfolio vertical slice로 freeze
- 다음 프로젝트는 libGDX/Rust/Godot 등 더 가벼운 stack 검토
- Validator-first / headless test-first / stable artifact path를 초기부터 적용
- UI layout sandbox와 concept-to-UI pipeline을 별도 tool로 발전

## 면접관 관점 체크리스트

- [ ] 어떤 게임인지 첫 화면에서 이해된다.
- [ ] 실제 APK와 capture evidence가 있다.
- [ ] AI claim이 과장되지 않는다.
- [ ] 실패와 해결 과정이 보인다.
- [ ] 본인의 역할이 PM/tech lead/game developer로 드러난다.
- [ ] 다음 프로젝트에 전이 가능한 pipeline lesson이 있다.

## 소비자 관점 체크리스트

- [ ] 탑을 오르는 이유와 분위기가 보인다.
- [ ] 전투, 이벤트, 보상 루프가 보인다.
- [ ] 마타이오스가 동행자처럼 보인다.
- [ ] APK를 눌러볼 이유가 있다.
- [ ] 프로토타입임을 알고 봐도 흥미가 남는다.
