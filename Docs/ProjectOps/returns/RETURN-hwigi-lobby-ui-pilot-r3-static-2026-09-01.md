---
session: "2026-09-01 hwigi-lobby-ui-pilot-round-3-static"
agent: "codex"
entry_point: "CODEX-DISPATCH-hwigi-lobby-ui-2026-08-31.md §14"
based_on:
  - "D-001"
  - "wiki/projects/hwiglija-tower-AGENTS.md"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: flagged
skill_candidate: false
first_pass: true
retries: 0
usage_receipt: null
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 로비 정적 게이트 및 보존 (라운드 3)

## ② 산출물 (Outputs)

- `Assets/_Project/Scripts/Lobby/LobbyLayoutContract.cs` · Unity 비의존 슬롯·토큰·빌드 기준 계약 · settlement: gate_pending
- `Assets/_Project/Scripts/Lobby/LobbyLayout.cs` · `Vector2`/`Color` 변환만 보유하는 Unity 어댑터 · settlement: gate_pending
- `Tools/DumpLayout/` · 순수 .NET 콘솔. `LobbyLayoutContract.cs`에서 `lobby_actual_layout.json`을 생성 · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/lobby_actual_layout.json` · source=`LobbyLayoutContract.cs`, Unity 미실행 정적 actual · settlement: gate_pending
- `Tools/check_layout.py` · v1.2 spec/actual 대조기 · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/QUEUE.md` · 로비=`통과(정적)`, 전투=`진행` · settlement: applied

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성
- Memory append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성

## ③ 게이트 대조 결과

- 시작 기준 HEAD: `5adc2e174333ef680ce25c258ac57d932743d050`
- 보존 브랜치: `codex/hwigi-lobby-ui-pilot-r3`
- CodeGraph: required / sync 완료. `LobbyLayoutContract`가 pure data, `LobbyLayout`가 Unity adapter, `LobbyController`가 consumer인 경계를 확인함.
- 사양: `lobby_layout_spec.json` v1.2
- `dotnet run --project Tools/DumpLayout/DumpLayout.csproj -- --output Docs/Portfolio/assets/concept-to-ui/lobby_actual_layout.json --captured-at "origin/Proto@5adc2e1 static-contract (Unity 미실행)"`: pass
- `python3 Tools/check_layout.py --spec ...lobby_layout_spec.json --actual ...lobby_actual_layout.json --rejections ..._rejections.md --round 3`: pass
  - 사양 자체 L-01~L-08: 8/8 PASS
  - actual 슬롯 대조: visible 4/4 + slot 8/8 = 12/12 PASS
- 순수 계약 검사: `LobbyLayoutContract.cs`에 `UnityEngine`/`Vector2`/`Color` 참조 없음. Unity 타입 변환은 `LobbyLayout.cs`에만 남김.
- 정적 C# 컴파일: pass — Unity API 참조와 `LobbyLayoutSpecTests`/`LobbySmokeTests`를 포함한 임시 harness가 오류 0개로 빌드됨. Unity Test Runner 결과가 아니다.

## 제약 상태

| 층 | 상태 | 근거 |
|---|---|---|
| 로비 정적 사양 | pass | v1.2 8/8 |
| 사양↔actual 드리프트 | pass | pure DumpLayout + 12/12 |
| Unity EditMode/PlayMode | flagged | 이 Mac의 Unity license exit 198 |
| PNG 캡처 | not generated | Unity 런타임 미실행 |
| 명조 TMP font | N/A | 자산·라이선스 작가 지정 대기 |
| `tower_deep` | blocked | 사양 v1.2 assetFallbacks에 명시 |

## 파일럿 관찰

1. **채점이 판정을 대신했는가**: 예, 정적 층은 `DumpLayout → check_layout`으로 사람 눈 확인 0회에 판정했다. 이 actual은 Unity 실제 좌표가 아니라 pure contract 출력이라는 표기를 보존했다.
2. **무승인 범위가 적절했는가**: 적절했다. §14가 직접 요구한 분리·덤프·드리프트 차단 외에는 새 UI 의도, 폰트, deepRun 대체, save schema를 만들지 않았다.

## 위배·STOP 사항

- Unity runtime gate는 라이선스 활성화 전까지 여전히 `flagged`다. 정적 통과를 런타임 통과나 PNG 생성으로 바꾸어 말하지 않는다.
- 전투 정적 작업은 `QUEUE.md` 2번에서 별도 범위로 이어간다. 로비의 명조 폰트와 deepRun 자산은 전투 착수의 blocker가 아니다.

---

> ③ Gate 전 상태: `flagged` (로비 정적 통과). Unity 라이선스가 활성화되면 기존 EditMode → PlayMode → explicit PNG fixture를 별도 라운드로 재실행한다.
