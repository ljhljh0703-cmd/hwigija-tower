---
session: "2026-09-01 hwigi-lobby-ui-pilot"
agent: "codex"
entry_point: "CODEX-DISPATCH-hwigi-lobby-ui-2026-08-31.md"
based_on:
  - "D-001"
  - "wiki/projects/hwiglija-tower-AGENTS.md"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: flagged
skill_candidate: false
first_pass: false
retries: 0
usage_receipt: null
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 로비 UI 파일럿: 사양 충돌 STOP

## ② 산출물 (Outputs)

- `Docs/ProjectOps/returns/RETURN-hwigi-lobby-ui-pilot-2026-09-01.md` · `origin/Proto@5adc2e174333ef680ce25c258ac57d932743d050` 기준의 STOP 증거 · settlement: repair
- `Docs/Portfolio/assets/concept-to-ui/lobby_layout_spec.json` · 입력 SHA-256 `a2efe65b45a92ed0769116857e63d53da7d2620ddff0a2d62d176aa544d7e4fb` · settlement: gate_pending
- `Docs/Portfolio/assets/concept-to-ui/ui_tokens.json` · 입력 SHA-256 `3f685450e3dc902da82fbfe9411456ff2d1d452530e2a5f1db3e7b8fb67596de` · settlement: gate_pending

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성
- Memory append: 없음 — 디스패치 §1의 vault 쓰기 금지에 따라 미작성

## ③ 게이트 대조 결과

- 착수 기준 HEAD: `5adc2e174333ef680ce25c258ac57d932743d050` (`git rev-parse HEAD` 명령 출력)
- 작업 기준: 기존 dirty main worktree를 보존하고 위 HEAD의 clean detached worktree에서 확인함.
- 헌법/권한: pass — vault 권위 파일을 수정하지 않음.
- 결정: pass — D-001의 제목 「회귀자는 탑을 오른다」와 충돌하는 변경을 하지 않음.
- 모듈 경계: pass (기존 상태) — `Assets/_Project/Scripts/Lobby/LobbyController.cs`가 이미 `PrototypeHud.cs`와 독립되어 있음.
- 사양 자동 대조: flagged — `lobby_layout_spec.json`의 L-03과 `titleMark` 슬롯이 서로 충돌함.
  - 보호 영역: `x=0.35..0.65`, `y=0.10..0.55`
  - `titleMark`: `x=0.10..0.90`, `y=0.46..0.58`
  - 교집합: `x=0.35..0.65`, `y=0.46..0.55`
  - 판정 스크립트 결과: L-01/L-04/L-05/L-06 위반 0건, L-02 primary=`primaryAction` 1건, L-07 band 순서 일치, L-03 위반=`titleMark` 1건.
- CodeGraph: required for a runtime UI/model-boundary change, not run — 사양이 현재 형태로는 완료 조건 L-01~L-07을 만족할 수 없어 코드 변경 전에 STOP함.
- 테스트/캡처: 실행하지 않음 — L-03 불통과 상태를 성공으로 보고하지 않기 위해 보류함.

## 위배·STOP 사항

- 디스패치 §6은 L-03을 모든 UI 슬롯의 무겹침으로 정의하지만, 같은 정본 JSON의 `titleMark`가 그 금지 영역을 침범한다.
- 디스패치 §9 및 `game-ui-from-concept`의 규칙에 따라, 구현으로 우회하거나 JSON을 임의로 변경해 통과시키지 않았다.
- 따라서 로비 코드, PlayMode 캡처 픽스처, Unity 테스트에는 변경이 없다. 현재 worktree의 입력 JSON과 이 RETURN만 새 파일이다.

## 작가 결정 필요

- 권장: L-03의 세로 보호 영역을 `y=0.10..0.46`으로 변경해, 현 `titleMark`의 시작점 `y=0.46`을 경계 접촉으로 허용한다. 다른 슬롯과 위계는 그대로 보존된다.
- 대안: 보호 영역은 유지하고 `titleMark`와 `tagline`의 y/h를 함께 하단으로 옮긴다. 이 경우 제목·태그라인·primary 사이의 여백 값까지 새로 잠가야 한다.

## 배운 점

- 화면 구현 전, 슬롯 사각형과 금지 영역의 교집합을 기계적으로 먼저 검사해야 한다. 이번 파일럿의 사양 대조 채점은 코드 이전 단계에서 결함을 잡아냈다.

---

> ③ Gate 검토 전 상태: `flagged`. 작가가 L-03 해석 또는 수정을 확정하면 같은 기준 HEAD에서 구현을 재개한다.
