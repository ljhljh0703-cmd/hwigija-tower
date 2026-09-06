# Dispatch — Portfolio Capture Polish RC1

이 문서는 각 세션에 그대로 전달할 수 있는 지시서다.

## 공통 기준

- Base: latest `origin/Proto` after harness status update. Current command-center baseline: `origin/Proto@9f59b51b430f022a17eb6c599a79bccbc675f813`
- Clean worktree 필수.
- Main dirty worktree 사용 금지.
- 목표: 포트폴리오/데모 영상에 찍힐 화면 품질 개선.
- 신규 시스템 추가 금지.
- 본편 runtime RL/ONNX 연결 금지.
- raw ML output, checkpoint, ONNX, TensorBoard event, secret 포함 금지.

참고:

- `Docs/ProjectOps/portfolio-capture-polish-harness.md`
- `Docs/Portfolio/hwigi-tower-steam-portfolio.html`
- `Docs/Portfolio/portfolio_ai_npc.html`
- `Docs/Portfolio/assets/hwigi_target_*.jpg`
- `Docs/Portfolio/assets/hwigi_impl_remaster_*.jpg`
- Public portfolio: `https://ljhljh0703-cmd.github.io/hwigi-tower-portfolio/`

## UI/Asset 세션 지시

```markdown
# UI/Asset 지시 — Portfolio Capture Polish RC1 Triage

## 목표
데모 영상에 들어갈 화면을 기준으로 UI polish target과 구현 가능한 P0 change list를 만든다.

## 범위
- Combat
- Map
- Event
- Reward/Growth
- Rest
- Shop

## 산출
각 화면별로 다음을 작성:
- before-risk: 지금 영상에 찍히면 부끄러운 점
- target direction: 포트폴리오 target visual과 맞출 방향
- concrete UI changes: Game Dev가 구현 가능한 수정 목록
- priority: P0/P1/P2

## P0 우선순위
1. Combat readability / hit feel / Mataios presence
2. Map/Event first impression / choice/result clarity
3. Reward/Growth popup clarity

## 금지
- 새 스킬/아이템/전투 규칙 제안 금지
- 최종 스토리/대사 작성 금지
- screenshot harness 요구 금지
- "더 예쁘게" 같은 추상 보고 금지

## 보고
[Done]
[Files]
[Gate] Gate 1 pass/fail
[P0 list]
[P1/P2 deferred]
[Next Game Dev instruction]
```

## Game Dev 세션 지시

```markdown
# Game Dev 지시 — Portfolio Capture Polish RC1 P0 Implementation

## 기준
- Base: latest origin/Proto after command-center Gate 1 status
- clean worktree 필수
- C# runtime 변경 전 CodeGraph fresh preflight
- main dirty worktree 사용 금지
- 먼저 `Docs/ProjectOps/portfolio-capture-polish-harness.md`와 `Docs/ProjectOps/portfolio-capture-polish-status.md`를 읽을 것

## 구현 범위
Gate 1에서 확정된 P0만 구현. 이번 pass는 `PrototypeHud.cs` 중심 presentation polish가 원칙이다.

P0:
1. Combat HUD
   - enemy stage + bottom player/Mataios party dock 구조로 정리
   - enemy HP / player HP / Mataios HP / 최근 combat log 2~3줄 중심
   - `플레이어: N 피해`, `마타이오스: N 지원 피해/보호/마무리` 분리
   - Attack/Defend/Skill 버튼은 크게 유지하고 disabled Skill은 이유를 표시
   - 피해 숫자, shake/pulse, 처치 feedback은 유지하거나 표시 계층만 강화

2. Map/Event
   - route/debug/internal label 숨김: `[current]`, `[locked]`, `MoralChoice`, `CHOICE_`, `ENC_` 등 노출 금지
   - 기존 node/map/button sprite를 써서 sparse route, current glow, future dark, locked state를 플레이어용 label로 표시
   - event choice는 Korean label + 즉시 효과 preview + 선택 후 compact result strip으로 정리

3. Reward/Growth/Shop
   - 보상/성장 팝업 중앙성, 수치, 아이콘, CTA 정리
   - plain result text 대신 중앙 popup과 단일 CTA 사용
   - shop을 데모 루트에 넣을 경우 2~3개 item card, name/effect/price/current Gold/disabled reason/leave CTA가 보여야 함

## 금지
- 새 시스템 추가 금지
- 전투/경제 밸런스 임의 확장 금지
- `CombatController`, `PrototypeRunState`, `PrototypeFloorMapBuilder`, `MataiosCombatBrain` 변경 금지. 단, UI 수정만으로 불가능한 실제 버그가 테스트로 증명되면 보고 후 최소 변경
- runtime RL/ONNX 금지
- ITEM_07 / OQ-025 full deck 금지
- raw ML output commit 금지

## 검증
- CodeGraph preflight 보고
- git diff --check
- forbidden diff search
- targeted EditMode: `PresentationLayerTests` 우선. 가능하면 combat/map/event/reward/shop 표시 test 추가 또는 갱신
- Android APK build

## 보고
[Done]
[Files]
[Gate] Gate 2 pass/fail
[Commit]
[Validation]
[APK if built]
[Risks]
[Next Deploy instruction]
```

## Deploy / Project Ops 세션 지시

```markdown
# Deploy 지시 — Portfolio Capture Polish RC1 APK

## 목표
데모 영상 촬영용 APK 후보를 repo-local Builds/Android 아래 산출한다.

## 기준
- Base: latest origin/Proto after Game Dev RC1 implementation
- clean build worktree
- main dirty worktree 사용 금지

## 산출
- APK path: `/Users/godju/Downloads/AI Game/hwigi-tower/Builds/Android/...apk`
- SHA-256
- package
- launcher label
- ABI
- apksigner v2 result

## device smoke
- 기기가 없으면 `N/A`, blocker 아님.
- 기기가 있으면 install/launch/capture route smoke.

## 보고
PLAYTEST_STATUS:
COMMIT:
PATH:
SHA-256:
package/label/ABI/signature:
device smoke:
remaining risk:
```

## Portfolio / HTML 세션 지시

```markdown
# Portfolio 지시 — Capture Sync

## 조건
Deploy APK와 실제 캡처/영상 후보가 나온 뒤에만 실행.

## 목표
public portfolio의 screenshot/GIF/copy를 실제 데모 캡처 기준으로 맞춘다.

## 기준
- Source edit first: origin/Proto
- Public deploy repo is copy-only: `hwigi-tower-portfolio`

## 금지
- raw ML CSV/JSON 복사 금지
- checkpoint/ONNX/event log 복사 금지
- secret/local path 노출 금지
- final release 과장 금지

## 검증
- claim boundary check
- relative asset check
- public URL HTTP 200
```

## Command Center 운영 루프

1. UI/Asset 보고 수신.
2. P0만 Game Dev에 전달.
3. Game Dev 보고에서 gate 실패 시 Deploy 금지.
4. APK 후보가 나오면 사용자 smoke 또는 capture 준비.
5. 캡처 후보가 나오면 Portfolio sync.
6. RC1 accepted 여부는 사용자 확인 후만 표기.
