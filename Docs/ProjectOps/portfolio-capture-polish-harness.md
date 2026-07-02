# Portfolio Capture Polish Harness — Hwigi Tower

## 목적

포트폴리오와 데모 영상에 들어갈 게임 화면이 조잡해 보이지 않도록, 여러 세션을 하나의 우선순위 아래에서 운영한다.

이 harness의 목표는 새 게임 시스템을 늘리는 것이 아니라, 이미 구현된 화면을 영상/포트폴리오 기준으로 정리하는 것이다.

## 기준선

- Source repo: `/Users/godju/Downloads/AI Game/hwigi-tower`
- Work base: `origin/Proto`
- Current accepted planning commit at harness creation: `55c873695857d5a13fb2e3d0a89e1a39c3d0ac59`
- Main dirty worktree is not a baseline.
- Development must use a clean worktree.
- Public portfolio URL: `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/`

## RC 목표

`Portfolio Capture Polish RC1`

데모 영상에서 반드시 보여도 되는 화면 품질을 만든다.

우선순위:

1. Combat: 전투 화면의 가독성, 타격감, 동행자 존재감.
2. Map/Event: 첫인상 화면의 정렬, 선택지/결과 카드, CTA.
3. Reward/Growth: 보상과 성장의 즉시 이해.
4. Rest/Shop: 지나가는 화면의 조잡함 제거.
5. Capture/Deploy: APK, 데모 루트, public portfolio 갱신 준비.

## 총괄 원칙

Command Center는 세션을 나누되, 목표를 하나로 고정한다.

- UI/Asset은 목표 화면과 규격을 정한다.
- Game Dev는 P0 규격만 구현한다.
- Deploy/Project Ops는 APK와 검증 산출물을 고정한다.
- Portfolio/HTML은 영상 캡처가 나온 뒤 public page와 맞춘다.
- System Design은 새 규칙/밸런스/경제/전투 구조 변경이 필요할 때만 호출한다.

## 세션 역할

### Command Center

권한:

- 우선순위 결정.
- 세션 dispatch 발행.
- 보고의 acceptance 여부 결정.
- scope creep 차단.

금지:

- dirty main worktree를 기준선으로 삼기.
- 사용자 smoke 전 accepted라고 표기.
- 신규 기능을 polish로 위장.

### UI/Asset

목표:

- 포트폴리오용 화면 기준을 만든다.
- 실제 구현 가능한 UI change list를 뽑는다.

산출:

- 화면별 `before risk / target direction / concrete change`.
- P0/P1/P2 분류.
- 전투 화면 우선 개선안.
- 필요 에셋 gap list.

금지:

- 새 전투 규칙 제안.
- 최종 스토리/대사 작성.
- screenshot harness 의존.
- 추상적인 "더 예쁘게" 보고.

### Game Dev

목표:

- UI/Asset P0을 실제 Unity runtime에 반영한다.

필수:

- clean worktree.
- C# runtime 변경 전 CodeGraph fresh preflight.
- `git diff --check`.
- forbidden search.
- targeted EditMode 가능한 범위.
- Android APK build.

금지:

- runtime RL/ONNX 연결.
- ITEM_07 / OQ-025 full intent deck.
- 새 밸런스/경제 수치 임의 확장.
- main dirty worktree 사용.

### Deploy / Project Ops

목표:

- 영상 촬영용 APK 산출물을 안정적으로 만든다.

필수 보고:

- `PLAYTEST_STATUS`
- `COMMIT`
- repo-local `Builds/Android/*.apk` path
- SHA-256
- package/label/signature/ABI
- device smoke status. Device 없으면 `N/A`, blocker 아님.

금지:

- `/private/tmp/...`만 최종 배포 path로 보고.
- APK build success를 user accepted로 표현.
- raw ML/training artifact 포함.

### Portfolio / HTML

목표:

- 데모 영상/캡처가 나오면 public portfolio page와 맞춘다.

필수:

- source repo `Proto` 먼저 수정.
- public repo `hwigi-tower-portfolio`는 배포 복사본으로만 취급.
- raw ML CSV/JSON, ONNX, checkpoint, event log, secret 복사 금지.
- HTTP 200 확인.

## 우선순위 게이트

### Gate 0 — Baseline

Pass 조건:

- `origin/Proto` clean worktree 사용.
- 현재 APK/portfolio source commit 명시.
- dirty main worktree 미사용.

Fail 시:

- 작업 중단 후 기준선 보고.

### Gate 1 — Capture Risk Triage

Pass 조건:

- Combat / Map / Event / Reward / Rest / Shop 화면별 P0 risk가 분리됨.
- P0가 "영상에서 부끄러운가" 기준으로 정렬됨.

Fail 시:

- 구현 세션으로 넘기지 않음.

### Gate 2 — P0 Implementation

Pass 조건:

- Combat P0와 Map/Event P0가 적용됨.
- 신규 시스템 추가 없이 기존 UI/feedback 조정.
- targeted tests 또는 명시적 미실행 사유.
- Android APK build pass.

Fail 시:

- Deploy 세션으로 넘기지 않음.

### Gate 3 — Capture APK

Pass 조건:

- repo-local APK path.
- SHA-256.
- package/label/signature/ABI.
- capture route smoke checklist.

Fail 시:

- 포트폴리오 캡처 금지.

### Gate 4 — Public Portfolio Sync

Pass 조건:

- 데모 캡처/영상 기준으로 public page 업데이트.
- main page / AI NPC page HTTP 200.
- claim boundary check pass.

Fail 시:

- 공개 링크 재공유 금지.

## P0 정의

P0는 다음 중 하나다.

- 데모 영상에서 즉시 조잡해 보이는 UI.
- 전투/이벤트 진행이 막히거나, 막힌 것처럼 보이는 화면.
- 마타이오스가 동행자인지 알 수 없는 전투 표시.
- 보상/성장/결과가 이해되지 않는 화면.
- 포트폴리오 공개 문구와 실제 게임 화면이 심하게 충돌하는 상태.

P0가 아닌 것:

- 새 스킬/아이템/경제 시스템.
- 장기 밸런스.
- 엔딩/스토리 완성.
- 온디바이스 LLM 실통합.
- 50k ML training.

## 보고 템플릿

각 세션은 아래 형식으로 보고한다.

```text
[Done] <한 줄>
[Files] <변경 파일 수와 주요 파일>
[GDD impact] 없음 / 필요 시 D-OQ 명시
[Gate] Gate N pass/fail
[Commit] <sha or no>
[APK] <path/SHA or N/A>
[Validation] <실행한 검증과 결과>
[Risks] <남은 실제 리스크>
[Next] <다음 세션 지시 1줄>
```

## Acceptance 용어

- `Accepted`: 사용자가 실제로 확인했거나, PM이 명시 승인한 상태.
- `Candidate`: push/build/검증은 됐지만 사용자 확인 전.
- `Blocked`: 작업 진행에 필요한 외부 조건이 없어 중단.
- `N/A`: 기기 없음, 해당 검증 범위 아님 등. blocker와 구분.

## 금지 검색 기준

구현/배포 세션은 diff 또는 staged 범위에서 아래를 확인한다.

```text
ONNX
MLAgents runtime
PPO runtime
runtime RL
ITEM_07
OQ-025 full deck
DateTime.Now
new Random()
OPENAI_API_KEY
ANTHROPIC_API_KEY
events.out.tfevents
.pt
.onnx
checkpoint
```

기존 문서/훈련 전용 파일의 known hit는 분리 보고한다.

## 이번 RC1 완료 조건

RC1은 아래가 모두 충족되면 완료 후보가 된다.

- Combat screen capture가 첫눈에 읽힌다.
- 마타이오스 지원/보호/행동이 플레이어에게 보인다.
- Event/Map 화면에서 선택지와 CTA가 명확하다.
- Reward/Growth 화면에서 획득/성장 효과가 즉시 이해된다.
- 영상 촬영용 APK가 repo-local path에 있다.
- public portfolio와 실제 캡처 방향이 충돌하지 않는다.
