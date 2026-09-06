# Vertical Slice Case Study Harness — Hwigi Tower

## 목적

이 harness의 최종 목표는 `회귀자는 탑을 오른다`를 상용 출시 직전 게임으로 포장하는 것이 아니라, 대외 포트폴리오용 **Vertical Slice Case Study**로 닫는 것이다.

이제 성공 기준은 "게임을 계속 완성한다"가 아니다.

성공 기준은 다음이다.

- Android APK로 실제 플레이 가능한 prototype을 제시한다.
- 구현된 핵심 루프와 화면을 증거로 보여준다.
- 마타이오스 / AI NPC / ML-Agents 실험을 과장 없이 설명한다.
- 기획, 구현, 검증, 배포, 멀티 세션 운영을 하나의 제작 역량 사례로 묶는다.
- 부족한 부분은 다음 개발 과제로 정직하게 분리한다.

## 기준선

- Source repo: `/Users/godju/Downloads/AI Game/hwigi-tower`
- Official source base: `origin/Proto@0739f53bfc86c19866b88feb0ab466e2f7bcb2ae`
- Current worktree for this harness: `/private/tmp/hwigi-vertical-slice-case-study-harness`
- Main dirty worktree is not a baseline.
- CodeGraph: optional for this docs-only harness.
- Public portfolio: `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/`
- APK download folder: `https://drive.google.com/drive/folders/1GSTXDCrkHHI-TVfRA-hYQs7hh3PtnTZG?usp=drive_link`

## 최종 목표

`Portfolio Vertical Slice Case Study v1`

포트폴리오에서 한 문장으로 말할 수 있어야 한다.

> 모바일 세로 로그라이크 프로토타입을 직접 구현하고, AI NPC / 전투 설계 실험 / 멀티 에이전트 제작 파이프라인까지 증거 기반으로 정리한 vertical slice case study.

## 범위

### In Scope

- Steam 상세 페이지처럼 읽히는 메인 포트폴리오 HTML
- AI NPC training 독립 case study HTML
- Android APK 후보와 SHA-256
- 주요 화면 캡처 6-8장 또는 contact sheet
- AI/ML claim boundary 검증
- concept-to-UI / remaster target visual 설명
- multi-agent / clean worktree / gate 운영 사례
- 개발 블로그 초안 또는 요약 섹션

### Out of Scope

- Hwigi Tower 상용 출시 수준 완성
- 전투 시스템 추가 재설계
- full story/dialogue complete
- on-device LLM shipped integration
- runtime PPO / ONNX 연결
- 50k ML-Agents 추가 학습
- Unity 전체 테스트 suite 녹색화
- Android device smoke를 hard blocker로 취급

## 상태 분류

### Accepted Evidence

아래는 source repo 또는 repo-local artifact로 증거가 남아 있는 항목이다.

| 항목 | 상태 | 증거 |
|---|---:|---|
| Portfolio source HTML | Accepted source | `Docs/Portfolio/hwigi-tower-steam-portfolio.html`, `Docs/Portfolio/portfolio_ai_npc.html` |
| AI/ML evidence freeze Exp02-04 | Accepted source | `Docs/Portfolio/ml-agents-combat-exp02.md`, `ml-agents-combat-exp03.md`, `ml-agents-combat-exp04.md`, `assets/mlagents_exp04_*` |
| AI NPC handoff | Accepted source | `Docs/Portfolio/HANDOFF_portfolio_ai_npc.md` |
| Portfolio capture harness | Accepted source | `Docs/ProjectOps/portfolio-capture-polish-harness.md` |
| Final capture APK record | Accepted source | `Docs/ProjectOps/portfolio-capture-final-apk-report.md` |
| Capture APK artifact | Local release artifact | `Builds/Android/hwigi-tower-185f794-portfolio-capture-final-20260702.apk`, SHA-256 `836850eafd2c8bffae7b04b71cfd0562a3de53ca0131c72d404743e5aca57041` |
| Screenshot bundles | Local release artifact | `Builds/PortfolioScreenshots/hwigi-tower-0739f53-20260702/`, `Builds/PortfolioScreenshots/hwigi-tower-185f794-20260702/`, SHA manifests verified locally |

### Candidate Evidence

아래는 포트폴리오에 쓸 수 있지만, 사용 전 source/publish 위치를 확정해야 한다.

| 항목 | 상태 | 처리 |
|---|---:|---|
| Remaster target images | Source candidate | `Docs/Portfolio/assets/hwigi_target_*.jpg`, `Docs/Portfolio/assets/hwigi_impl_remaster_*.jpg`는 이미 source에 포함됨 |
| Concept-to-UI breakdown | Local candidate | main dirty worktree의 `Docs/Portfolio/assets/concept-to-ui/`에 존재. 사용 시 별도 promotion 필요 |
| UI runtime layout attempt | Candidate / unaccepted | `/private/tmp/hwigi-combat-layout-spec` 변경은 commit/push 전. case study에는 "attempt / design handoff"로만 사용 |
| Public portfolio deploy | Candidate | 기존 Pages URL은 있음. 최종 publish 후 HTTP 200과 새 copy 확인 필요 |

### Not Applied / Later

- 실제 Unity 화면 전체 리마스터 완료
- UI Text 선명도 전면 개선
- 완성형 성장/인벤토리/스킬 트리
- 보스 보상 풀 확장
- 상용 수준 밸런스 패스
- 온디바이스 대화 모델 실제 게임 runtime 통합

## Claim Gate

포트폴리오 문구는 아래 경계를 넘지 않는다.

### 허용

- "Android playable prototype APK를 만들었다."
- "마타이오스는 deterministic companion combat brain으로 구현했다."
- "ML-Agents를 전투 설계 디버거로 사용했다."
- "AI NPC 대화 모델 훈련/실험을 별도 case study로 정리했다."
- "concept target을 UI/Asset 세션에서 Unity layer로 분해해 반영하는 pipeline을 설계했다."
- "여러 Codex 세션을 역할별로 나눠 clean worktree, gate, artifact SHA 기반으로 운영했다."

### 금지

- "마타이오스가 본편에서 RL로 학습되어 행동한다."
- "PPO/ONNX가 본편 runtime에 연결되어 있다."
- "상용 출시 수준으로 완성됐다."
- "모든 Unity 테스트가 통과한다."
- "실제 기기 visual smoke가 Codex에 의해 완료됐다."
- "concept image가 그대로 구현 완료됐다."

## Case Study Gate

### Gate 0 — Evidence Freeze

Pass:

- 공식 source base와 artifact base를 분리해 기록한다.
- APK SHA-256과 screenshot SHA manifest를 확인한다.
- main dirty worktree는 evidence source로만 분류한다.

Fail:

- `/private/tmp/...`만 최종 경로로 제시한다.
- untracked evidence를 accepted로 표기한다.

### Gate 1 — Narrative / Claim Alignment

Pass:

- 모든 headline claim이 증거 파일 하나 이상과 연결된다.
- AI/ML claim boundary가 통과한다.
- "프로토타입"과 "상용 출시" 경계가 명확하다.

Fail:

- 구현되지 않은 concept를 실제 플레이 화면처럼 말한다.
- AI runtime claim이 혼재한다.

### Gate 2 — Visual Package

Pass:

- 실제 Unity capture 6-8장 또는 contact sheet를 선별한다.
- remaster target visual은 "target / direction / pipeline"으로 구분한다.
- concept-to-UI 산출물은 "layer breakdown / layout spec / runtime adaptation" 흐름으로 설명한다.

Fail:

- 조잡한 원본 화면만 나열한다.
- AI가 만든 target image를 실제 gameplay capture로 속인다.

### Gate 3 — HTML Publication

Pass:

- source repo HTML 먼저 갱신한다.
- public repo는 정적 배포 복사본으로만 취급한다.
- main page / AI NPC page HTTP 200과 대표 copy 확인.
- raw ML output, checkpoint, ONNX, secret 복사 없음.

Fail:

- public repo만 고치고 source repo를 잃어버린다.
- private/local path가 public HTML에 노출된다.

### Gate 4 — External Review

Pass:

- 소비자 관점: 게임이 무엇인지 10초 안에 이해된다.
- 면접관 관점: 역할, 문제, 구현, 검증, 실패 대응이 보인다.
- 약점은 "next iteration"으로 정리되고 변명처럼 보이지 않는다.

Fail:

- 내부 작업 로그처럼 보인다.
- AI를 많이 썼다는 말만 있고 결과물 품질/검증이 약하다.

## 최종 산출 패키지

### 필수

1. `Docs/Portfolio/hwigi-tower-steam-portfolio.html`
2. `Docs/Portfolio/portfolio_ai_npc.html`
3. `Docs/Portfolio/vertical-slice-case-study-outline.md`
4. APK path + SHA-256
5. screenshot bundle path + SHA manifest
6. public portfolio URL
7. claim boundary checklist

### 권장

- 60-90초 demo video
- blog post 1: "AI를 게임 기능이 아니라 전투 설계 디버거로 쓴 이유"
- blog post 2: "Codex multi-session으로 Unity prototype을 vertical slice까지 끌고 간 방식"
- concept-to-UI breakdown contact sheet

## 최종 중단선

다음 조건을 만족하면 Hwigi Tower 개발은 더 늘리지 않고 포트폴리오 closure로 전환한다.

- APK 후보와 public portfolio URL이 같이 제시된다.
- 메인 페이지가 게임의 hook, implemented systems, AI/ML pipeline, downloadable build를 설명한다.
- AI NPC 페이지가 별도 기술 case study로 연결된다.
- visual package가 "현재 구현"과 "리마스터 방향"을 정직하게 나눈다.
- 다음 단계가 새 gameplay 구현이 아니라, `libGDX Rogue OS` 또는 새 엔진 프로젝트로 넘어가는 학습 자산으로 정리된다.

## 10-line Handoff

1. Final target is `Portfolio Vertical Slice Case Study v1`.
2. Do not keep polishing Hwigi Tower as a full game unless it supplies missing portfolio proof.
3. Official source base is `origin/Proto@0739f53`.
4. Main dirty worktree is not a baseline.
5. APK artifact exists at `Builds/Android/hwigi-tower-185f794-portfolio-capture-final-20260702.apk`.
6. APK SHA-256 is `836850eafd2c8bffae7b04b71cfd0562a3de53ca0131c72d404743e5aca57041`.
7. Screenshot bundles under `Builds/PortfolioScreenshots` have SHA manifests verified locally.
8. ML-Agents can be claimed only as combat design probe, not shipped runtime RL.
9. Concept/remaster images are target/pipeline evidence, not actual gameplay capture unless re-imported and recaptured.
10. Next work should update portfolio pages, package visuals, verify public URLs, and then stop Hwigi Tower active development.
