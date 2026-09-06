---
session: "2026-09-01 hwigi-lobby-ui-pilot-round-2"
agent: "codex"
entry_point: "CODEX-DISPATCH-hwigi-lobby-ui-2026-08-31.md §12"
based_on:
  - "D-001"
  - "wiki/projects/hwiglija-tower-AGENTS.md"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: flagged
skill_candidate: false
first_pass: false
retries: 1
usage_receipt: null
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 로비 UI 파일럿 라운드 2

## ② 산출물 (Outputs)

- `Assets/_Project/Scripts/Lobby/LobbyLayout.cs` · v1.1 슬롯·밴드·보호영역·UI 토큰 계약 · settlement: gate_pending
- `Assets/_Project/Scripts/Lobby/LobbyController.cs` · 사양 기반 런타임 UGUI 조립, noSave, 기록/설정/종료 utility row · settlement: gate_pending
- `Assets/_Project/Tests/EditMode/LobbyLayoutSpecTests.cs` · L-01~L-08·토큰·HUD 경계 자동 대조 · settlement: gate_pending
- `Assets/_Project/Tests/PlayMode/LobbySmokeTests.cs` · 런타임 슬롯, noSave/save, 프레임, L-03/L-04/L-08 검증 · settlement: gate_pending
- `Assets/_Project/Tests/PlayMode/PortraitUiScreenshotQaTests.cs` · `01_lobby_no_save.png` / `01_lobby_with_save.png` 캡처 픽스처 · settlement: gate_pending
- `Assets/_Project/Scripts/Lobby/LobbyPresentationData.cs`, `Assets/_Project/Editor/LobbySceneBuilder.cs`, `Assets/_Project/Data/Presentation/SO_LobbyPresentationData.asset` · 화면상 Prototype 문구 제거 · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/lobby_layout_spec.json` · v1.1 입력 SHA-256 `955e452af4d1e8841ad60dc807bdc0d25aa4b22bdaad2a6f64f868da18eb61c2` · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/ui_tokens.json` · 입력 SHA-256 `3f685450e3dc902da82fbfe9411456ff2d1d452530e2a5f1db3e7b8fb67596de` · settlement: gate_pending

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성
- Memory append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성

## ③ 게이트 대조 결과

- 착수 기준 HEAD: `5adc2e174333ef680ce25c258ac57d932743d050` (`git rev-parse HEAD` 명령 출력)
- 작업 기준: main dirty worktree는 보존하고 위 HEAD의 clean detached worktree에서 변경함.
- CodeGraph: required / initialized + indexed + synced. 최종 status = 144 indexed files, 10,441 nodes, 25,647 edges, up to date. `LobbyController` query/context로 save/audio/scene 경계를 확인함.
- 헌법/권한: pass — vault 권위 파일을 수정하지 않음.
- 결정: pass — D-001 제목 「회귀자는 탑을 오른다」를 유지하고 새 게임 규칙·수치를 추가하지 않음.
- 모듈 경계: static pass — `LobbyController.cs`는 `PrototypeHud`를 참조하지 않음. 공유 경계는 `PrototypeRunSaveStore`/`PrototypeRunSaveRequest`, `PrototypeAudioService`/`AudioCueCatalog`, `SceneManager`, `LobbyPresentationData`뿐.

| 제약 | 정적 사양 대조 | Unity 런타임 대조 |
|---|---|---|
| L-01 | pass — interactive 슬롯 전부 y >= 0.66 | N/A — 라이선스 차단 |
| L-02 | pass — primaryAction 1개 | N/A — 라이선스 차단 |
| L-03 | pass — ui/actions/frame 슬롯의 보호영역 겹침 0개, title 면제 | N/A — 라이선스 차단 |
| L-04 | pass — text slot Prototype 0개 | N/A — 라이선스 차단 |
| L-05 | pass — primary/secondary frameStyle 보유 | N/A — 라이선스 차단 |
| L-06 | pass — 최소 interactive 높이 99.84px | N/A — 라이선스 차단 |
| L-07 | pass — resource → judgement → subject → party → action | N/A — 라이선스 차단 |
| L-08 | pass — titleMark/tagline frameStyle·배경 패널 없음 | N/A — 라이선스 차단 |

- 정적 C# 컴파일: pass — Unity API 참조, `LobbyLayoutSpecTests`, `LobbySmokeTests`를 포함한 임시 compile harness가 오류 0개로 빌드됨. 이 검사는 Unity Test Runner의 대체가 아니라 타입/문법 검증이다.
- Unity EditMode: blocked — 두 번의 batch 실행 모두 exit 198, 테스트 발견 전 `No valid Unity Editor license found`로 종료. 결과 XML 없음.
- Unity PlayMode: not run — 동일 라이선스 blocker 때문에 실행하지 않음.
- PNG 캡처: not generated — 예상 경로는 `/private/tmp/hwigi-portrait-ui-v3-screenshots/01_lobby_no_save.png`, `/private/tmp/hwigi-portrait-ui-v3-screenshots/01_lobby_with_save.png`; 실제 Unity 실행 전에는 생성했다고 주장하지 않음.

## 사양과 어긋난 부분

- `ui_tokens.json`의 명조 계열 TMP display font asset이 리포에 없다. 현재 로비는 기존 UGUI `LegacyRuntime.ttf` fallback으로 컴파일되며, title font family·letter spacing의 시각적 일치는 미검증이다. 라이선스가 확인된 명조 폰트 에셋을 제공/지정한 뒤 visual QA에서 확정해야 한다.
- `deepRun.towerArt = tower_deep` 자산은 리포에 없다. 디스패치가 요구한 `noSave`만 구현했고, deepRun art 교체는 임의 대체하지 않았다.
- 회차 누적 횟수 데이터는 현재 save 모델에 없다. runStatus는 저장 유무·최고 도달 층(currentFloor)·기억 조각만 표시하며, 새 저장 스키마를 만들지 않았다.

## 파일럿 관찰

1. **자동 채점**: v1.1에서는 JSON 사각형 대조가 L-01~L-08을 정적으로 판정했다. 사람 눈 확인은 0회였으나, Unity 라이선스 차단 때문에 최종 visible/manual QA 1회는 아직 필요하다.
2. **로비 경계**: 분리는 Round 1 이전에 완료돼 있었다. 로비는 HUD와 상태 필드를 공유하지 않고 save/audio/scene/presentation 참조만 가진다. 다른 화면 코드(`PrototypeHud`, `PrototypeRoomController`, `PresentationLayerTests`)는 변경하지 않았다. 전체 회귀는 Unity Test Runner를 재활성화한 뒤에만 확정 가능하다.

## 위배·STOP 사항

- 완료 조건의 EditMode·PlayMode 전부 통과와 두 PNG 생성은 Unity Editor 라이선스가 없어서 검증되지 않았다. 이 반환은 `gate: flagged`이며, 완료/수락/배포를 주장하지 않는다.
- `_rejections.md`의 Round 1 기록은 읽고 유지했으며, 이번 라운드에서 사양을 사후 변경하지 않았다.

---

> ③ Gate 전 상태: `flagged`. 다음 실행은 Unity 라이선스 활성화 후 EditMode → PlayMode → explicit screenshot fixture 순서로 재검증한다.
