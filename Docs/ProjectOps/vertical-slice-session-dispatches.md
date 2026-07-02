# Dispatches — Vertical Slice Case Study Closure

이 문서는 `Portfolio Vertical Slice Case Study v1`을 닫기 위한 세션별 지시서다.

공통 원칙:

- Base: latest `origin/Proto`
- Main dirty worktree 사용 금지
- 새 gameplay 구현 금지. 포트폴리오 증거에 직접 필요한 최소 작업만 허용
- runtime RL/ONNX 연결 금지
- raw ML output, checkpoint, ONNX, TensorBoard event, secret 복사 금지
- `Accepted / Candidate / Not Applied`를 명확히 구분
- Device가 없으면 `N/A`, blocker 아님

## Command Center 지시

```markdown
# Command Center 지시 — Vertical Slice Case Study Closure

## 목표
Hwigi Tower를 더 완성하려고 끌지 말고, 포트폴리오 case study로 닫는다.

## 읽을 문서
- `Docs/ProjectOps/vertical-slice-case-study-harness.md`
- `Docs/ProjectOps/portfolio-capture-final-apk-report.md`
- `Docs/Portfolio/vertical-slice-case-study-outline.md`

## 해야 할 일
1. 현재 보고를 Accepted / Candidate / Not Applied로 분류한다.
2. 새 gameplay 요청이 들어오면 portfolio proof에 필요한지 먼저 판단한다.
3. 각 세션에 아래 dispatch 중 필요한 것만 보낸다.
4. 최종 public URL / APK / SHA / screenshot bundle / AI claim boundary를 하나의 release note로 묶는다.

## 금지
- user smoke 전 `Accepted` 표기 금지
- `/private/tmp` artifact를 최종 배포 위치로 보고 금지
- "게임 완성" 목표로 scope 확장 금지

## 보고
[Status]
- Accepted:
- Candidate:
- Not Applied:

[Risk]
- P0:
- P1:
- N/A:

[Next Orders]
- Portfolio:
- UI/Asset:
- AI:
- Release/Ops:
- WIKI:
```

## Portfolio / HTML 세션 지시

```markdown
# Portfolio 지시 — Vertical Slice Case Study HTML Finalization

## 목표
Steam-style 메인 포트폴리오와 AI NPC 독립 페이지를 `Vertical Slice Case Study`로 완성한다.

## 기준
- Source repo first: `/Users/godju/Downloads/AI Game/hwigi-tower`
- Public repo is copy-only deploy target: `hwigi-tower-portfolio`
- Base: latest `origin/Proto`

## 읽을 문서
- `Docs/ProjectOps/vertical-slice-case-study-harness.md`
- `Docs/Portfolio/vertical-slice-case-study-outline.md`
- `Docs/Portfolio/HANDOFF_portfolio_ai_npc.md`
- `Docs/ProjectOps/portfolio-capture-final-apk-report.md`

## 수정 대상
- `Docs/Portfolio/hwigi-tower-steam-portfolio.html`
- `Docs/Portfolio/portfolio_ai_npc.html`
- 필요한 경우 `Docs/Portfolio/assets/*`

## 구현
1. Hero를 "플레이 가능 APK + AI-assisted vertical slice"로 정리한다.
2. 게임 소개, 핵심 시스템, 마타이오스, AI/ML 설계 검증, 제작 파이프라인을 한 흐름으로 연결한다.
3. 실제 Unity capture와 remaster/concept target을 구분해서 배치한다.
4. APK 다운로드 링크는 Google Drive folder를 사용한다.
5. `정식 출시 전 프로토타입`, `Vertical Slice`, `Case Study` 경계를 명확히 한다.

## Claim Boundary
허용:
- ML-Agents as combat design probe
- deterministic Mataios combat brain
- EXAONE/QLoRA/FastAPI/Unity AI NPC training case study

금지:
- shipped runtime RL
- PPO/ONNX runtime 연결
- 상용 출시 완성 claim
- 실제 device smoke 완료 claim

## 검증
- `git diff --check`
- local file render 확인
- broken relative asset path check
- forbidden string check: `runtime RL`, `PPO runtime`, `.onnx`, `checkpoint`, `OPENAI_API_KEY`, `ANTHROPIC_API_KEY`
- public deploy 시 HTTP 200 확인

## 보고
[Done]
[Files]
[Public URL]
[Claim check]
[Remaining risk]
[Next]
```

## UI / Asset 세션 지시

```markdown
# UI/Asset 지시 — Visual Evidence Package

## 목표
포트폴리오에 넣을 visual evidence를 "실제 구현 캡처"와 "target/remaster 방향"으로 분리해 패키징한다.

## 읽을 문서
- `Docs/ProjectOps/vertical-slice-case-study-harness.md`
- `Docs/ProjectOps/portfolio-capture-polish-harness.md`
- `Docs/Portfolio/remaster_capture_notes.md`가 있으면 읽기

## 입력 후보
- `Builds/PortfolioScreenshots/hwigi-tower-0739f53-20260702/`
- `Builds/PortfolioScreenshots/hwigi-tower-185f794-20260702/`
- `Docs/Portfolio/assets/hwigi_target_*.jpg`
- `Docs/Portfolio/assets/hwigi_impl_remaster_*.jpg`
- local candidate: `Docs/Portfolio/assets/concept-to-ui/*`가 있으면 source promotion 필요 여부 보고

## 해야 할 일
1. 포트폴리오에 쓸 6-8개 visual을 선별한다.
2. 각 visual에 붙일 caption을 작성한다.
3. 실제 캡처 / remaster target / concept breakdown을 badge로 구분한다.
4. contact sheet 1장을 만들 수 있으면 생성한다.
5. 원본 캡처를 덮어쓰지 않는다.

## 금지
- AI target image를 actual gameplay capture로 표기 금지
- 새 gameplay 구현 요구 금지
- screenshot harness를 blocker로 만들기 금지

## 보고
[Done]
[Files]
[Selected visuals]
[Badge classification]
[Needs source promotion]
[Next Portfolio instruction]
```

## AI 세션 지시

```markdown
# AI 지시 — Claim Freeze for Portfolio

## 목표
포트폴리오에 들어갈 AI/ML claim을 증거 기반으로 정리하고 과장 리스크를 제거한다.

## 읽을 문서
- `Docs/ProjectOps/vertical-slice-case-study-harness.md`
- `Docs/Portfolio/ai-assisted-combat-design-lab.md`
- `Docs/Portfolio/ml-agents-combat-exp02.md`
- `Docs/Portfolio/ml-agents-combat-exp03.md`
- `Docs/Portfolio/ml-agents-combat-exp04.md`
- `Docs/Portfolio/HANDOFF_portfolio_ai_npc.md`
- `Docs/Portfolio/portfolio_ai_npc.html`

## 해야 할 일
1. 포트폴리오용 AI claim table을 만든다.
2. 각 claim에 evidence file을 연결한다.
3. forbidden / risky phrase를 별도 표로 뽑는다.
4. Exp04 / ContextPolicy / deterministic Mataios brain 연결을 짧게 설명한다.
5. AI NPC dialogue training case study와 ML-Agents combat probe를 혼동하지 않게 분리한다.

## 금지
- "마타이오스가 RL로 학습됐다" 표현 금지
- "ONNX runtime 연결" 표현 금지
- raw TensorBoard event, `.pt`, `.onnx`, checkpoint 포함 금지

## 보고
[Done]
[Files]
[Allowed claims]
[Forbidden claims]
[Evidence map]
[Next Portfolio instruction]
```

## Release / Ops 세션 지시

```markdown
# Release/Ops 지시 — Portfolio Artifact Manifest

## 목표
포트폴리오에 연결할 APK, screenshot bundle, public URL 상태를 하나의 manifest로 고정한다.

## 기준
- Source repo: `/Users/godju/Downloads/AI Game/hwigi-tower`
- main dirty worktree는 build input으로 쓰지 않음
- `Builds/Android`와 `Builds/PortfolioScreenshots`의 현재 산출물 확인

## 해야 할 일
1. 최신 portfolio APK path와 SHA-256 확인.
2. APK package/label/ABI/signature 기록.
3. screenshot bundle `SHA256SUMS.txt` 검증.
4. public portfolio URLs HTTP 200 확인.
5. 결과를 `Docs/ProjectOps/vertical-slice-artifact-manifest.md`로 작성.

## 검증
- `shasum -a 256`
- `shasum -a 256 -c SHA256SUMS.txt`
- `apksigner verify --verbose` 가능하면 실행
- `aapt dump badging` 가능하면 실행
- `curl -I` 또는 equivalent HTTP 200

## 보고
PLAYTEST_STATUS:
APK:
SHA-256:
screenshots:
public URLs:
device smoke:
remaining risk:
```

## WIKI / Sub-brain 세션 지시

```markdown
# WIKI 지시 — Hwigi Tower Vertical Slice Lessons Absorption

## 목표
Hwigi Tower에서 얻은 시행착오를 다음 게임 개발 pipeline에 재사용 가능한 지식으로 흡수한다.

## 읽을 repo 문서
- `Docs/ProjectOps/vertical-slice-case-study-harness.md`
- `Docs/ProjectOps/portfolio-capture-polish-harness.md`
- `Docs/ProjectOps/codex-pipeline-incident-ledger.md`가 있으면 읽기
- `Docs/Portfolio/vertical-slice-case-study-outline.md`

## 정리할 지식
1. Dirty worktree와 `/private/tmp` artifact 혼선 방지.
2. Accepted / Candidate / Not Applied 상태어.
3. smoke proof와 visible proof 분리.
4. user screenshot 기반 fixture 생성.
5. concept-to-UI는 구조 분해 / layout spec / runtime adaptation 세 단계로 분리.
6. ML experiment는 runtime claim과 분리.
7. portfolio closure는 "게임 완성"과 다른 목표로 관리.

## 금지
- vault authority 직접 rewrite 금지.
- 확정 결정처럼 쓰지 말고 proposed method / lesson으로 정리.

## 산출
- WIKI 학습 asset 또는 proposed method
- Codex skill candidate가 필요하면 제안
- repo로 되돌릴 10-line note
```

## Game Dev 세션 사용 조건

Game Dev는 기본적으로 멈춘다.

다음 조건 중 하나일 때만 호출한다.

1. 포트폴리오 HTML에 넣을 실제 캡처가 명백히 깨져 있고, runtime 최소 수정이 필요하다.
2. APK 실행 루프가 user path에서 막힌다.
3. claim boundary를 지키기 위해 구현 상태를 확인해야 한다.
4. UI/Asset의 visual package가 "source promotion"이 아니라 실제 Unity layer 적용을 요구한다.

그 외에는 Game Dev를 부르지 않는다. 현재 목표는 game completion이 아니라 case study closure다.
