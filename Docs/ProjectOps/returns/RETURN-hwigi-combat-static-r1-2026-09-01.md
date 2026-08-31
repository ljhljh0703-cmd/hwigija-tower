---
session: "2026-09-01 hwigi-combat-static-r1"
agent: "codex"
entry_point: "CODEX-DISPATCH-hwigi-lobby-ui-2026-08-31.md §14-5"
based_on:
  - "D-033"
  - "D-038"
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

# RETURN — 회귀탑 일반 전투 정적 사양 게이트 (라운드 1)

## ② 산출물 (Outputs)

- `Docs/Portfolio/assets/concept-to-ui/combat_layout_spec_v2.json` · 입력 사양 · settlement: gate_pending
- `Assets/_Project/Scripts/UI/CombatLayoutContract.cs` · Unity 비의존 전투 슬롯 계약(22 slots) · settlement: gate_pending
- `Tools/DumpLayout/Program.cs` · `--screen lobby|combat` 순수 덤프 확장 · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/combat_actual_layout.json` · source=`CombatLayoutContract.cs`, Unity 미실행 static actual · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/QUEUE.md` · 전투=`통과(정적)` · settlement: applied

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성
- Memory append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성

## ③ 게이트 대조 결과

- 시작 커밋: `120f37c` (`codex/hwigi-lobby-ui-pilot-r3`)
- CodeGraph: required / fresh sync 후 `PrototypeHud`의 전투 UI 조립부와 `CombatLayoutContract` 경계를 확인함.
- `dotnet run --project Tools/DumpLayout/DumpLayout.csproj -- --screen combat --output Docs/Portfolio/assets/concept-to-ui/combat_actual_layout.json --captured-at "origin/Proto@5adc2e1 static-contract (Unity 미실행)"`: pass
- `python3 Tools/check_layout.py --spec Docs/Portfolio/assets/concept-to-ui/combat_layout_spec_v2.json --actual Docs/Portfolio/assets/concept-to-ui/combat_actual_layout.json --rejections Docs/Portfolio/assets/concept-to-ui/_rejections.md --round 1`: pass
  - C-01~C-07: 7/7 PASS
  - C-08: SKIP — 사양이 요구한 런타임 대비 4.5:1 측정
  - actual visible: 5/5 PASS, slots: 22/22 PASS
- 순수 계약: `CombatLayoutContract.cs`에 `UnityEngine`/`Vector2`/`Color` 참조 없음. 전역 색·타입 스케일은 `UiTokenContract` 하나를 공유한다.
- 정적 C# 컴파일: pass — Unity API 참조를 포함한 임시 compile harness 오류 0개. Unity Test Runner 통과를 뜻하지 않는다.

## 상태 구분

| 층 | 상태 | 근거 |
|---|---|---|
| 사양 자체 | pass | C-01~C-07 |
| 사양↔static actual | pass | visible 5/5 + slot 22/22 |
| `PrototypeHud` 실제 런타임 레이아웃 적용 | Not Applied / Later | Unity runtime gate와 묶어 별도 적용 필요 |
| C-08 대비 | N/A | Unity 캡처 필요 |
| Unity EditMode/PlayMode/PNG | flagged | 현재 license exit 198 |

## 관찰

1. C-08을 억지로 수치화하지 않았다. 사양이 runtime manual check로 남긴 대비 판정은 그대로 보류했다.
2. static contract는 `PrototypeHud`의 현재 전투 상태·행동 로직을 건드리지 않는다. D-033/D-038의 intent/counterplay와 public naming을 바꾸지 않고, 다음 Unity 가능 라운드의 좌표 입력만 고정했다.
3. 큐 3번 휴식 화면부터는 사양이 없다. `QUEUE.md` 규칙에 따라 미작성 화면을 임의로 설계하지 않고 여기서 정지한다.

## 위배·STOP 사항

- 일반 전투의 static gate만 통과했다. 실제 HUD 변경, C-08 대비, Unity tests, screenshot은 수행/주장하지 않았다.
- 다음 화면(휴식)은 사양 미작성 Knowledge Gap이다. 작가가 사양을 제공하거나 작성 게이트를 열기 전에는 진행하지 않는다.

---

> ③ Gate 전 상태: `flagged` (일반 전투 정적 통과). 다음 실행 권한: Unity 라이선스 후 전투 runtime 적용·C-08·tests, 또는 작가의 휴식 화면 사양 승인.
