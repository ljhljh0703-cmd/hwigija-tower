---
session: "2026-09-02 · R5 이벤트 / 큐 10~12"
agent: "codex"
entry_point: "NEXT.md"
based_on:
  - "D-012"
  - "D-029"
  - "AGENTS.md"
  - "Docs/Portfolio/assets/concept-to-ui/event_layout_spec.json v1.0"
  - "Docs/Portfolio/assets/concept-to-ui/bossgate_layout_spec.json v1.0a"
  - "Docs/Portfolio/assets/concept-to-ui/ending_layout_spec.json v1.0"
gate: pass
skill_candidate: false
first_pass: false
retries: 3
usage_receipt: null
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — R5 이벤트와 큐 10~12 런타임 UI 완료

## ② 산출물 (Outputs)

- `Assets/_Project/Scripts/UI/EventLayoutContract.cs` · `EventLayout.cs` · `EventUiController.cs` · 가변 2~14 선택지 스크롤, Hidden 미표시, 고정 이탈 조작부 · settlement: applied
- `Assets/_Project/Scripts/UI/BossGateLayoutContract.cs` · `BossGateLayout.cs` · `BossGateUiController.cs` · `ENC_COMBAT_GATE_02` 관문의 체력·이성·보유품 점검판과 단일 진입/물러나기 · settlement: applied
- `Assets/_Project/Scripts/UI/EndingLayoutContract.cs` · `EndingLayout.cs` · `EndingUiController.cs` · 기존 authored 두 선택지, 런 요약, 자원 칩 제거 · settlement: applied
- `Assets/_Project/Scripts/UI/PrototypeHud.cs` · 모듈 배선, 보스 전투(`ENC_COMBAT_GATE_03`)와 관문(`ENC_COMBAT_GATE_02`) 분리, 전용 화면의 legacy HUD 크롬 격리 · settlement: applied
- `Assets/_Project/Scripts/Run/PrototypeRoomController.cs` · 이벤트 이탈이 기존 `CancelSelectedMapNode()` 경로로 복귀하도록 배선 · settlement: applied
- `Tools/DumpLayout/` · event/bossgate/ending contract actual 생성 지원 · settlement: applied
- `Docs/Portfolio/assets/concept-to-ui/{event,bossgate,ending}_actual_layout.json` · `source: contract` · settlement: applied
- `Docs/Portfolio/assets/concept-to-ui/{event,bossgate,ending}_runtime_actual_layout.json` · `source: runtime` · settlement: applied
- `Docs/Portfolio/assets/concept-to-ui/runtime-captures/{03_event_jar_room,07_boss_combat,08_ending_choice,12_boss_gate_choices}.png` · Unity QA 실캡처 · settlement: applied
- `TestResults-{EditMode,PlayMode,PresentationLayer,PortraitUiScreenshotQa}-R5.xml` · repo-local 검증 XML · settlement: applied
- `Docs/Portfolio/assets/concept-to-ui/QUEUE.md` · 큐 9~12 런타임 통과 갱신 · settlement: applied

## 작업로그 포인터 (Dual-Track)

- Progress append: 없음 — dispatch가 Sub-brain 쓰기를 허용하지 않았고, Vault는 read-only로 유지했다.
- Memory append: 없음 — 동일 사유.

## ③ 게이트 대조 결과

- 헌법(SSOT D-012): pass — 공개 화면에 `붕괴도`/Glitch 수치·문구를 추가하지 않았다.
- 결정(D-029): pass — 준비도 점수·전투 modifier·저장 스키마를 만들지 않았다. 관문은 기존 체력·이성·보유품만 표시한다.
- 계약(화면 사양): pass — event 8/8 + runtime 14/14, bossgate 8/8 + runtime 15/15, ending 9/9 + runtime 10/10. 보스 전투는 기존 combat 사양 runtime 27/27을 재사용했다.
- 배선: pass — `check_wiring.py`에서 Event 9/9, BossGate 10/10, Ending 6/6; 전 계약 PASS.
- 데이터바인딩: pass — `check_databinding.py` 전체 PASS.
- Unity 표적 검증: `PresentationLayerTests` 46/46 pass, 명시적 `PortraitUiV3_CapturesRequiredQaScreens` 1/1 pass.
- 전량 회귀: EditMode 206/214 pass, 기존 8 fail; PlayMode 15/22 pass, 기존 6 fail + explicit 1 skip. 실패 이름 집합은 R4 기준선과 동일.
- 수치 재현성: Unity 6000.4.3f1, projectPath=`/Users/godju/Downloads/AI Game/hwigi-rest-ui-20260901`, 2026-09-02 실행 XML을 위 경로에 보관했다.

## 위배·STOP 사항

- C-08은 variants blocked 상태라 계속 SKIP이다. 기본 상태로 대체 측정하지 않았다.
- 첫 캡처 실행은 `-nographics`의 `RenderTexture.Create` 환경 오류로 실패했다. 이후 그래픽 컨텍스트 QA에서 보스 전투/관문 ID 분리와 관문 중 자동 지도 복귀를 수정해 통과했다. 통과 캡처의 시각 검토에서 legacy HUD 잔상을 발견해 별도 표현 수정·재검증을 수행했다.
- 기존 범위 밖 실패 14건(EditMode 8, PlayMode 6)은 수정하지 않았다.

## 배운 점

- 정적 contract 대조와 runtime RectTransform 대조가 모두 통과해도, 전용 화면 아래의 legacy HUD 잔상은 PNG 검토에서만 잡힌다. 모듈 표시 시의 기존 크롬 가시성도 별도 확인 대상이다.
- `ENC_COMBAT_GATE_02`는 관문 준비 화면, `ENC_COMBAT_GATE_03`은 보스 전투라는 런타임 카탈로그 구분을 QA fixture에도 그대로 유지해야 한다.

## ⑤ 스킬화 후보 보고

없음. 기존 계약형 Unity UI + runtime actual 절차로 충분했다.

---

Sub-brain status: read-only. 외부 Vault ③ Gate는 이 RETURN을 대상으로 별도 수행 대상이다.
