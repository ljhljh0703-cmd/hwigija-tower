---
session: "2026-09-01 hwigi-rest-ui-r1"
agent: "codex"
entry_point: "Docs/Portfolio/assets/concept-to-ui/DISPATCH-rest.md"
based_on:
  - "D-012"
  - "D-029"
  - "wiki/projects/hwiglija-tower-AGENTS.md"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: flagged
skill_candidate: false
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 UI 큐 3번 휴식

## ② 산출물 (Outputs)

- `Assets/_Project/Scripts/UI/RestLayoutContract.cs` · `UnityEngine` 비의존 순수 C# 휴식 슬롯 계약
- `Assets/_Project/Scripts/UI/RestLayout.cs` · 계약 좌표와 UI 토큰의 Unity 어댑터
- `Assets/_Project/Scripts/UI/RestUiController.cs` · 휴식 표현·선택·입력 컨트롤 모듈
- `Assets/_Project/Scripts/UI/PrototypeHud.cs` · 기존 휴식 진입/해결 호출을 유지한 모듈 배선
- `Tools/DumpLayout/` · `--screen rest` 정적 actual 출력 배선
- `Docs/Portfolio/assets/concept-to-ui/rest_actual_layout.json` · source=`RestLayoutContract.cs`, **Unity 미실행 정적 actual**
- `Assets/_Project/Scripts/UI/CombatLayoutContract.cs` + `combat_actual_layout.json` · `mataiosSanityBar`를 `mataiosStateLabel` text 슬롯으로 교정
- `Assets/_Project/Tests/PlayMode/PortraitUiScreenshotQaTests.cs` · 로비·전투·휴식 런타임 RectTransform actual 기록 배선
- `Docs/Portfolio/assets/concept-to-ui/QUEUE.md` · 2번·3번을 `통과(정적)`으로 갱신. 4번 층 지도는 미변경

## ③ 게이트 대조 결과

- 시작 HEAD: `c72b19128aa395b7fe41ca7c7d2285f38cc8aab6`
- 브랜치: `codex/hwigi-lobby-ui-pilot-r3`
- 휴식 사양 자체: R-01~R-09 **9/9 PASS**
- 휴식 정적 actual 대조: required visible 6/6 + slot 14/14 = **20/20 PASS**
- 전투 사양 자체: C-01~C-07·C-09 **8 PASS**, C-08 **SKIP**
- 전투 정적 actual 대조: required visible 5/5 + slot 22/22 = **27/27 PASS**
- 순수 계약: `RestLayoutContract.cs`에 `UnityEngine`/`Vector2`/Unity `Color` 의존 없음. 색상 리터럴도 휴식 모듈에 없음.
- 휴식 로직 보존: `ResolveCurrentRouteRestInteraction`, `ModifyGlitchLevel`, `MATAIOS_TRAINING_BUFF_ACTIVE`, 저장 호출은 변경하지 않음.

## 런타임 레인

Unity는 `exit 198` 없이 배치 실행·컴파일·테스트까지 진입했다.

| 실행 | 결과 | 판정 |
|---|---:|---|
| `TestResults-EditMode.xml` | 181/211 pass, 30 fail | flagged |
| `TestResults-PlayMode.xml` | 12/21 pass, 8 fail, explicit 1 skip | flagged |
| `TestResults-PortraitUiScreenshotQa.xml` | 0/1 pass | flagged |

명시적 QA는 첫 `01_lobby_no_save`에서 Continue 버튼이 비활성이어야 한다는 검증이 실패해 멈췄다 (`CaptureLobby` line 106, Expected `False`, actual `True`). 따라서 아래 런타임 증거는 **생성되지 않았다**.

- PNG: `/private/tmp/hwigi-portrait-ui-v3-screenshots/` 에 생성된 파일 없음
- 런타임 actual: `lobby_actual_layout.json`, `combat_actual_layout.json`, `rest_actual_layout.json` 모두 미생성
- 정적 `rest_actual_layout.json`을 런타임 actual의 대체물로 사용하지 않음

## 실패 분리

| 구분 | 결과 | 근거 / 처리 |
|---|---|---|
| ⓐ 이번 휴식 작업 | 0건 확인 | `Hud_RestInteractionUsesThePortraitRestLayoutContract`, `RestLayout_ChoiceCardLabelsStayPublicAndHideGlitch`, 기존 AskMood/Recover 휴식 테스트 통과. 명시적 QA는 휴식 진입 전 로비에서 중단됨. |
| ⓑ 전투 rename | 0건 확인 | rename은 정적 계약 슬롯 1개와 DumpLayout 출력만 변경했고, 전투 관련 실패 stack trace에 이 변경이 없음. |
| ⓒ 기존/범위 밖 | 38건의 실행 실패·중단 | EditMode 22건은 `com.unity.ml-agents/Samples` immutable folder의 meta 누락 로그가 TestRunner error로 승격됨. 나머지는 AudioLobby NullReference와 맵·카탈로그·전투 흐름 기대값 실패. PlayMode 8건은 no-save 격리, Lobby settings NullReference, 맵 진행/선택지 누락이며 휴식 모듈 전 단계에서 발생. 수정하지 않음. |

## C-08

**SKIP 유지.** `strained`/`broken` variants는 `mataios_strained`, `mataios_broken`, `rest_strained`, `rest_broken` 아트 4종 미입고와 `thresholds.*: undecided` 때문에 blocked다. 기본 상태 대비로 대체 측정하지 않았다.

## 관찰

1. 채점기는 정적 계약층에서 판정을 대신했다. 런타임 writer도 별도 경로로 배선했으나, 캡처가 실패한 사실을 보존했다.
2. 무승인 범위는 유지했다. 휴식 수치·붕괴도 delta·버프·저장 스키마·variant 임계값을 바꾸지 않았다.

## 상태

- ③ Gate: **flagged** — 정적 통과, 런타임 3화면 캡처/actual 대조 미완료
- Skill candidate: 없음 — 기존 `game-ui-from-concept` 절차가 이 작업을 포괄함
- Sub-brain: read-only. Progress/Memory append 및 vault 권위 문서 수정 없음

