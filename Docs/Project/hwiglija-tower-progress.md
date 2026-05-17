---
created: 2026-04-27
updated: 2026-04-27
type: project
tags: [progress, hwiglija-tower, flick]
project: 회귀자는 탑을 오른다
---

# hwiglija-tower — Progress Log

> 본 파일은 **append-only** 진행상황 아카이브.
> Codex CLI 가 매 작업 세션 종료 시 한 항목씩 추가한다 (AGENTS.md §9 참조).
> 디자인 결정은 본 파일이 아닌 [[hwiglija-tower-gdd]] §1 CHANGELOG / §2 DECISIONS 에 기록.
> 본 파일은 **무엇을 했는가** (실행 로그) 만 다룬다.

---

## 항목 포맷

```
### YYYY-MM-DD HH:MM — <한 줄 요약>
- **Phase**: W1-1 / W1-2 / ...
- **Done**: 무엇을 했는가 (3-5 bullet)
- **Files**: 변경/추가 파일 N개 (주요 경로)
- **GDD impact**: SSOT 변경 여부 — 있으면 D-NNN / OQ-NNN / CHANGELOG 항목
- **Blockers**: (있으면) 막힌 지점
- **Next**: 다음 권장 작업 1-2줄
- **Agent**: Codex / Claude (Sub brain) / 작가 직접
```

---

## 진행 로그

### 2026-05-17 23:34 — Randomized floor map graph added
- **Phase**: W3-2
- **Done**:
  - Floor 1-5 map background/marker assets imported and bound through `DemoPresentationData`
  - `PrototypeFloorMapBuilder` upgraded to deterministic seeded graph generation with positioned nodes, edges, selectable/completed/skipped/locked state
  - Map UI now draws floor background, route lines, selected/current/locked/completed overlays, and graph-positioned node buttons
  - Save/restore keeps seed-regenerated graph stable while restoring completed/skipped map node ids
  - Tests updated for deterministic graph rules, connected-node selection, save restore, map presentation assets, and branching-map PlayMode traversal
- **Files**: 변경/추가 28개 (주요: `Assets/_Project/Scripts/Run/PrototypeFloorMap.cs`, `Assets/_Project/Scripts/UI/PrototypeHud.cs`, `Assets/_Project/Art/Map/**`)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: 1080x1920 map text sharpness/contrast polish 또는 actual device touch QA
- **Agent**: Codex

### 2026-05-17 22:23 — Final presentation art drop-ins bound
- **Phase**: W3-2
- **Done**:
  - 신규 PNG import/meta 정상화 후 combat action icon 3종과 lobby logo를 presentation data에 바인딩
  - completed node badge slot을 추가하고 `node_back_cleared`를 floor map 완료 상태에 표시
  - `ENEMY_MERCENARY_CAPTAIN_SAGAN_01` presentation/catalog를 `boss_mercenary_captain_sagan_01` 아트로 교체
  - 매칭 stableId가 없는 신규 enemy art 11종은 catalog intake 후보로 문서화
  - 1080x1920 screenshot harness 8종 재생성 및 Lobby/Map/Combat 중심 육안 QA
- **Files**: 변경/추가 44개 (주요: SO_DemoPresentationData, SO_LobbyPresentationData, PrototypeHud, 신규 Art PNG/meta)
- **GDD impact**: 없음
- **Blockers**: player portrait `char_player_bust_01.png`, merchant 전용 sprite, 실제 audio clip 파일은 아직 없음. Unity CLI `-runTests`는 XML 미생성이라 임시 TestRunner wrapper로 검증 후 제거.
- **Next**: player/merchant/audio 실제 에셋 수령 시 동일 슬롯에 바인딩하고, 신규 map art는 별도 map presentation 목표에서 연결
- **Agent**: Codex

### 2026-05-17 19:24 — Memory fragment public terminology rename
- **Phase**: W3-2
- **Done**:
  - Public UI/result/save summary에서 `Memory Fragment`/`기억 파편` 노출을 `기억의 잔향` 또는 `기억`으로 교체
  - `MEM_FRAGMENT_*` stableId와 내부 `MemoryFragment` 타입/저장 정책은 유지
  - DemoPresentationData memory encounter displayName과 asset intake 문구를 memory echo / 기억의 잔향 기준으로 정리
  - EditMode 120/120 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경 9개 (`PrototypeHud.cs`, `PrototypeEncounterRuntimeResolver.cs`, `PrototypeRunSaveData.cs`, presentation asset, tests, docs)
- **GDD impact**: 없음
- **Next**: writer-approved 기억의 잔향 본문/타이틀이 들어오면 placeholder key copyfit 재검수
- **Agent**: Codex

### 2026-05-17 17:34 — Combat action icon drop-in folder
- **Phase**: W3-2
- **Done**:
  - 전용 combat action icon 드롭인 폴더 `Assets/_Project/Art/UI/CombatActions/` 추가
  - Attack/Defend/Skill 권장 파일명과 import 규칙 문서화
  - 기존 `DemoPresentationData` 슬롯과 임시 node icon 바인딩 정책을 art README에 연결
- **Files**: 변경/추가 4개 (`Assets/_Project/Art/UI/CombatActions/**`, `Assets/_Project/Art/_README.md`, progress)
- **GDD impact**: 없음
- **Blockers**: 실제 `icon_action_attack/defend/skill_scout` 에셋은 아직 없음
- **Next**: 전용 아이콘/플레이어 portrait 에셋 수급 후 `SO_DemoPresentationData` 슬롯 교체
- **Agent**: Codex

### 2026-05-17 17:19 — Player portrait slot and combat action icons
- **Phase**: W3-2
- **Done**:
  - `DemoPresentationData`에 player portrait slot과 CombatAction icon slots 추가
  - 전투 하단 player card가 실제 portrait slot 또는 fallback silhouette를 표시하도록 정리
  - Attack/Defend/Skill 버튼을 icon + short label 구조로 바꾸고 임시 node icon을 바인딩
  - EditMode 120/120 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경 7개 (`PrototypeHud.cs`, `DemoPresentationData.cs`, `SO_DemoPresentationData.asset`, tests, QA/progress docs)
- **GDD impact**: 없음
- **Blockers**: 실제 player portrait와 전용 action icon art는 아직 없음
- **Next**: dedicated player portrait/action icon art 수급 후 슬롯 교체, font sharpness polish
- **Agent**: Codex

### 2026-05-17 14:53 — Combat party layout v4 follow-up
- **Phase**: W3-2
- **Done**:
  - 전투 중 objective/route/result/merchant/event 잔상 노출을 차단
  - Combat panel을 top enemy stage / middle combat log / bottom party dock 구조로 확장
  - Player/Mataios card를 하단 dock에 고정하고 action buttons를 compact square 형태로 재배치
  - EditMode 119/119 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경 3개 (`PrototypeHud.cs`, `Portrait_UI_v3_Screenshot_QA_2026-05-16.md`, progress)
- **GDD impact**: 없음
- **Blockers**: font sharpness/player portrait/action icon final art는 후속 polish 필요
- **Next**: player portrait/action icon art binding, font rendering polish, final copyfit pass
- **Agent**: Codex

### 2026-05-17 11:19 — Event cutscene layout reference pass
- **Phase**: W3-2
- **Done**:
  - 이벤트 화면을 top status / central cutscene image / event text / bottom choice buttons 구조로 분리
  - Jar Room 이벤트에서 probability hint 선택지는 유지하면서 route/debug-like text와 raw stableId 노출을 차단
  - 이벤트 진입 중 기존 route/memory/result/portrait 잔상 노출을 정리
  - EditMode 119/119 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경/추가 5개 (`PrototypeHud.cs`, `PresentationLayerTests.cs`, `PrototypeRoomSmokeTests.cs`, `Docs/QA/**`, progress)
- **GDD impact**: 없음
- **Blockers**: event screen font sharpness/final copyfit은 writer-approved text 이후 추가 polish 필요
- **Next**: player portrait/action icon art binding, event/font rendering polish, final copyfit pass
- **Agent**: Codex

### 2026-05-17 09:00 — Combat party layout v4
- **Phase**: W3-2
- **Done**:
  - 전투 화면을 top enemy stage / middle combat log / bottom party dock 구조로 재배치
  - Player card placeholder, HP bar, ATK/buff chips와 Mataios card, affinity/memory/reaction line을 combat dock에 통합
  - Attack/Defend/Skill을 하단 square action row로 고정하고 전투 중 기존 route/debug-like text 노출을 차단
  - EditMode 119/119 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경/추가 5개 (`PrototypeHud.cs`, `PresentationLayerTests.cs`, `PrototypeRoomSmokeTests.cs`, `Docs/QA/**`, progress)
- **GDD impact**: 없음
- **Blockers**: 실제 player portrait asset은 아직 없어 placeholder card 사용
- **Next**: player portrait asset 바인딩, combat action icon art, boss/enemy intent icon polish
- **Agent**: Codex

### 2026-05-17 07:25 — Text density and companion panel polish
- **Phase**: W3-2
- **Done**:
  - `PrototypeHud`의 title/body/button/result/stat/caption 텍스트 크기와 dense line spacing을 상수로 정리
  - Result summary를 핵심 3줄 중심으로 압축하고 Mental/Affinity/Memory/Item 라벨을 player-facing 한국어로 정리
  - Companion strip을 Mataios portrait 옆 고정 영역으로 옮기고 NPC reaction을 1줄 축약 표시로 제한
  - Combat feedback을 enemy/player HP와 최신 action consequence 중심의 2-3줄로 줄이고 action button 위치를 유지
  - EditMode 119/119 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경/추가 4개 (`PrototypeHud.cs`, `PresentationLayerTests.cs`, `Docs/QA/**`, progress)
- **GDD impact**: 없음
- **Blockers**: Unity `-runTests` 직접 XML 출력이 불안정해 임시 TestRunner wrapper로 fresh 검증했고 wrapper는 커밋에서 제거
- **Next**: 최종 폰트 선명도, writer-approved label copyfit, selected/focus animation polish
- **Agent**: Codex

### 2026-05-17 01:27 — Merchant/Rest/Map presentation polish
- **Phase**: W3-2
- **Done**:
  - 기존 `DemoPresentationData` merchant floor slots를 HUD shop 화면에 실제 visual로 연결
  - Shop 구매 카드를 상품명/가격·효과/불가 이유 중심으로 정리하고 보스 전 준비 맥락을 강화
  - Rest interaction을 action 효과 요약, 큰 입력창, response bubble, commit 결과 요약으로 분리
  - Floor Map 선택 가능/완료/잠김 상태의 색 대비와 아이콘/텍스트 크기를 보강
  - EditMode 119/119 pass, PlayMode 19/19 pass, screenshot 8종 1080x1920 확인
- **Files**: 변경/추가 4개 (`PrototypeHud.cs`, `DemoPresentationData.cs`, `Docs/QA/**`, progress)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: 최종 폰트/텍스트 스타일, merchant art variants, selected/focus animation polish
- **Agent**: Codex

### 2026-05-17 00:28 — Portrait UI v3 readability polish
- **Phase**: W3-2
- **Done**:
  - Floor Map을 2열 노드 카드로 재배치하고 선택 가능/완료/잠김 상태 문구를 player-facing하게 정리
  - Shop 화면에 현재 Gold와 보스 전 준비 맥락을 표시하고 구매/부족/지나가기 선택 가독성을 보강
  - Top HUD, Rest input, Combat feedback 텍스트 크기와 영역을 조정해 1080x1920 캡처 기준 판독성을 개선
  - screenshot harness로 8종 PNG를 `/private/tmp/hwigi-portrait-ui-v3-screenshots/`에 재생성하고 1080x1920 dimensions 확인
  - EditMode 119/119 pass, PlayMode 19/19 pass, skipped 0 확인
- **Files**: 변경/추가 3개 (`PrototypeHud.cs`, `Docs/QA/Portrait_UI_v3_Screenshot_QA_2026-05-16.md`, progress)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: merchant 전용 presentation과 최종 폰트/텍스트 스타일 정리로 P1/P2 visual polish 진행
- **Agent**: Codex

### 2026-05-16 16:20 — Portrait UI v3 screenshot harness QA
- **Phase**: W3-2
- **Done**:
  - `PrototypeHud`/`PrototypeRoomController`/`PrototypeRunState`에 `UNITY_EDITOR || UNITY_INCLUDE_TESTS` 범위 QA 진입 hook 추가
  - PlayMode screenshot harness로 Lobby/Floor Map/Event/Rest/Shop/Combat/Boss/Ending 8종을 실제 runtime UI 상태에서 1080x1920 PNG로 생성
  - `/private/tmp/hwigi-portrait-ui-v3-screenshots/`에 필수 screenshot 8개 생성 및 PNG dimensions 1080x1920 확인
  - `Docs/QA/Portrait_UI_v3_Screenshot_QA_2026-05-16.md`에 pass/needs polish 판정과 남은 P1 polish 기록
  - EditMode 119/119 pass, PlayMode 19/19 pass, skipped 0 확인
- **Files**: 변경/추가 7개 (`PrototypeHud.cs`, `PrototypeRoomController.cs`, `PrototypeRunState.cs`, `PortraitUiScreenshotQaTests.cs`, `Docs/QA/**`, progress)
- **GDD impact**: 없음
- **Blockers**: 없음. Unity `-runTests` 직접 XML 출력은 여전히 불안정해 임시 TestRunner wrapper로 fresh PlayMode를 검증했으며 wrapper는 커밋 제외 예정.
- **Next**: harness 커밋/푸시 후 P1로 텍스트 선명도, top HUD 밀도, shop/rest focus polish 진행.
- **Agent**: Codex

### 2026-05-16 11:38 — Portrait gameplay UI v3 shell
- **Phase**: W3-2
- **Done**:
  - `PrototypeHud`에 1080x1920 `PortraitRoot`와 landscape dark gutter를 추가하고 top/objective/visual/companion/result/map/action/ending 레이어를 분리
  - PrototypeRoom 시작 화면을 단일 `진행` 버튼이 아닌 branching node map 중심으로 전환하고 normal HUD에서 Glitch/raw id 노출을 줄임
  - Top HUD/companion strip/combat panel을 portrait 기준으로 재배치하고 enemy visual, HP, action feedback, skill/buff 상태가 크게 읽히도록 정리
  - Lobby scene bootstrap을 `sceneLoaded` 경로까지 보강하고 `Lobby.unity`에 Canvas/EventSystem을 저장해 black Game View를 직접 해소
  - EditMode 119/119 pass, PlayMode 18/18 pass, skipped 0 확인
- **Files**: 변경/추가 6개 (`Lobby.unity`, `PrototypeHud.cs`, `LobbyController.cs`, `PresentationLayerTests.cs`, `PrototypeRoomSmokeTests.cs`, progress)
- **GDD impact**: 없음
- **Blockers**: 전체 8-screen 수동 캡처는 미완료. Lobby visible screenshot만 `/private/tmp/hwigi-portrait-ui-v3-screenshots/`에 저장.
- **Next**: 1080x1920 수동 화면 캡처로 Lobby/Map/Event/Rest/Shop/Combat/Boss/Ending 가독성 확인 후 P1 visual polish
- **Agent**: Codex

### 2026-05-16 10:37 — First boss manual playtest blocked by Lobby/input P0s
- **Phase**: W3-2
- **Done**:
  - Unity `6000.4.3f1` GUI 실행 후 `Lobby.unity` Play Mode 재확인
  - Lobby black Game View 재현: New Game 버튼이 보이지 않아 정식 진입 불가
  - `PrototypeRoom.unity` 직접 우회 실행으로 배경/Mataios/HUD/`진행` 버튼 렌더링 확인
  - `진행` 버튼 클릭, Return, Space 입력을 시도했으나 노드/전투 흐름으로 진행되지 않음
  - 개발용 first-boss playtest QA 리포트 작성
- **Files**: 변경/추가 2개 (`Docs/QA/FirstBoss_Playtest_2026-05-16.md`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (QA/개발 인계 기록만, 신규 디자인 결정 없음)
- **Blockers**: 첫 보스 수동 클리어 실패. `Lobby` black screen + `PrototypeRoom` runtime button input 미동작이 P0.
- **Next**: Lobby actual Play Mode visibility와 EventSystem/Button input path를 먼저 수정한 뒤 첫 보스 클리어 QA 재시도
- **Agent**: Codex

### 2026-05-15 21:18 — Lobby Play Mode black screen fix
- **Phase**: W3-2
- **Done**:
  - Lobby Play Mode black Game View 원인을 stale `_uiBuilt` 상태와 실제 runtime Canvas 누락 불일치로 좁힘
  - `LobbyController`가 `_uiBuilt` 플래그만 믿지 않고 `Lobby Canvas`/title/buttons 존재를 확인해 누락 시 UI를 재생성하도록 수정
  - PlayMode smoke에 Canvas active/render mode/GraphicRaycaster 검증과 Canvas 누락 후 자동 재생성 회귀 테스트 추가
  - 2026-05-15 QA report/handoff 문서를 repo 내부 `Docs/QA/`에 보존
  - `git diff --check` 통과, forbidden search 신규 코드 위반 없음, EditMode 116/116 pass, PlayMode 18/18 pass, skipped 0 확인
- **Files**: 변경/추가 5개 (LobbyController.cs, LobbySmokeTests.cs, Docs/QA/**, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Codex manual Editor 세션은 이전 임시 wrapper compile error 이후 stale 상태가 섞여 육안 판정이 불안정했음. PlayMode 회귀 테스트로 Canvas 누락 재생성을 고정.
- **Next**: 사용자 단일 Editor 인스턴스에서 Lobby 1080x1920 육안 재확인 후 P1 map/combat readability 개선
- **Agent**: Codex

### 2026-05-15 20:48 — Unity retry 성공 + Lobby black screen blocker handoff
- **Phase**: W3-2
- **Done**:
  - Unity `6000.4.3f1` 재실행 성공: LicenseClient 연결, Unity Personal entitlement 확인, access token 갱신 통과
  - 프로젝트 로드, script compilation/domain reload, `Lobby.unity` 열기 성공
  - Play Mode 진입 성공 확인
  - Game View 가 검은 화면으로 유지되어 Lobby UI가 보이지 않는 P0 blocker 확인
  - QA 리포트를 최신 상태로 정정하고 개발 handoff 문서를 추가
- **Files**: 변경/추가 3개 (`Docs/QA/QA_Report_2026-05-15.md`, `Docs/QA/Dev_Handoff_Unity_Retry_2026-05-15.md`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (QA/개발 인계 기록만, 신규 디자인 결정 없음)
- **Blockers**: `Lobby.unity` Play Mode black Game View. 다음 개발 세션은 `LobbyController` UI 생성, Canvas/camera/render mode, presentation data binding 우선 확인 필요
- **Next**: Lobby black screen P0 수정 후 1080x1920 fresh manual QA 재개
- **Agent**: Codex

### 2026-05-15 17:19 — QA playtest report 작성, Unity licensing blocker 기록
- **Phase**: W3-2
- **Done**:
  - `origin/Proto` 최신 상태 확인 (`c4cad87`)
  - SSOT pillars / locked decisions / tone bible 금지선과 최근 PROGRESS 확인
  - Unity `6000.4.3f1` batch/GUI 실행을 시도했으나 LicenseClient 초기화 실패로 fresh manual playtest 차단 확인
  - 기존 PlayMode/Lobby smoke coverage, runtime source, 1080x1920 screenshot artifacts를 근거로 QA 리포트 작성
  - P0 environment blocker, P1 design gaps, dev/content/asset task를 우선순위화
- **Files**: 변경/추가 2개 (`Docs/QA/QA_Report_2026-05-15.md`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음 (QA 제안만, 신규 디자인 결정 없음)
- **Blockers**: Unity LicenseClient timeout으로 실제 Editor 수동 플레이/신규 PlayMode 결과 생성 불가
- **Next**: Unity licensing 복구 후 Lobby부터 1080x1920 fresh manual QA 재실행, combat intent/map preview/Mataios reaction 우선 구현 검토
- **Agent**: Codex

### 2026-05-15 12:31 — Lobby portrait menu layout fix
- **Phase**: W3-2
- **Done**:
  - Lobby UI를 1080x1920 portrait safe-area 루트 아래에 생성하도록 변경해 landscape Game View에서도 중앙 컬럼에 고정
  - 새 게임/이어 하기/프로필/설정/종료 버튼을 동일 폭 세로 컬럼으로 정렬하고 종료 버튼의 far-right 배치를 제거
  - 기본 화면의 큰 profile card를 작은 top-left chip으로 축소하고, 상세 정보는 Profile panel 안에서만 표시되도록 유지
  - Settings slider가 기본 로비 화면에 노출되지 않고 Settings panel 안에서만 생성/표시되는지 PlayMode smoke 검증을 보강
  - scene reload/enter play mode 설정 차이로 Awake가 누락되어도 Start/Update fallback에서 로비 UI를 1회 생성하도록 보강
- **Files**: 변경/추가 3개 (LobbyController.cs, LobbySmokeTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Validation**: `git diff --check` pass, forbidden search 신규 코드 위반 없음, EditMode 116/116 pass, PlayMode 17/17 pass, skipped 0
- **Blockers**: 1080x1920 수동 visual QA는 중복 Unity Editor lock/Hub focus 문제로 캡처 확인까지는 완료하지 못함. PlayMode smoke가 safe-area/button-column/settings/profile visibility를 구조적으로 검증.
- **Next**: 사용자가 Editor에서 단일 인스턴스로 Lobby를 열어 최종 육안 확인. 다음 작업은 실제 배경/로고/저장 슬롯 polish.
- **Agent**: Codex

### 2026-05-13 22:31 — Mataios model-specific training plan split
- **Phase**: W3-1
- **Done**:
  - `d405638 Document Mataios training dataset spec`를 origin/Proto에 push해 모델 비의존 spec 커밋을 보존
  - 외부 편집으로 섞인 HyperCLOVA/Colab/Ollama/512-token/TRL 실행 가정을 새 training run plan 문서로 분리
  - dataset spec v0.1은 schema, behavior contract, task coverage, keyword-only memory, fracture/forbidden rules 중심으로 정리
  - eval plan v0.1은 model/tool 비의존 eval axes, manual rubric, optional automation note 중심으로 정리
- **Files**: 변경/추가 4개 (Docs/AI/**, Docs/Project/hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: base model, training environment, final author-approved rows, Unity model artifact format은 아직 미정
- **Next**: 작가 승인 tone rows 추가 후 선택 모델 기준으로 실제 SFT run script를 별도 repo/tooling에서 준비
- **Agent**: Codex

### 2026-05-13 21:30 — Mataios training dataset spec and seed samples
- **Phase**: W3-1
- **Done**:
  - Mataios SFT dataset schema/spec 문서와 eval plan v0.1 문서를 추가
  - banmal, affinity variation, no direct answer, keyword-only memory, light S3/S4 fracture, no final lore/moral judgment/Glitch exposure 규칙을 명시
  - SFT seed JSONL 13개와 eval seed JSONL 13개를 추가해 rest/combat/event/shop/memory/boss/ending pending task를 커버
  - JSONL line-by-line parse와 required field check를 수행
- **Files**: 변경/추가 7개 (Docs/AI/**, mataios_sft_v0_1.sample.jsonl, mataios_eval_v0_1.sample.jsonl, metas)
- **GDD impact**: 없음
- **Blockers**: base model 미정. 샘플 target은 training-format temporary 예시이며 최종 canon dialogue가 아님.
- **Next**: 실제 작가 승인 tone rows를 추가하고, eval 자동 체크 스크립트/CI 편입 여부 결정
- **Agent**: Codex

### 2026-05-12 22:32 — Prototype run save and Lobby Continue
- **Phase**: W3-1
- **Done**:
  - deterministic prototype run save JSON model/store를 추가하고 missing save safe behavior를 고정
  - PrototypeRoom safe points(run start, node/choice completion, rest commit, combat result, floor/ending transition)에 save write를 연결
  - Lobby Continue가 save 존재 시 활성화되고 Floor/HP/Memory summary를 표시한 뒤 PrototypeRoom continue load로 진입하도록 연결
  - run id/floor/map node completion/memory fragment state restore와 reflection/cache save 제외 정책을 EditMode 테스트로 고정
  - Lobby PlayMode smoke에 no-save disabled, New Game save creation, Continue load path를 추가
- **Files**: 변경/추가 12개 (PrototypeRunSaveData.cs, PrototypeRunSaveStore.cs, PrototypeRunState.cs, PrototypeRoomController.cs, LobbyController.cs, tests)
- **GDD impact**: 없음
- **Blockers**: reflection repo / deterministic LLM cache는 현 구조상 save file에 열거 API가 없어 명시적으로 제외. PlayMode batchmode가 결과 저장 후 종료 신호를 놓쳐 수동 Unity 종료 필요.
- **Next**: save 파일에 reflection/cache를 포함할지 SSOT 결정 후 필요 시 repo snapshot API 추가
- **Agent**: Codex

### 2026-05-12 16:07 — Lobby main menu presentation polish
- **Phase**: W3-1
- **Done**:
  - Lobby 전용 art intake folder/README를 추가하고 `lobby_bg_tower_temp.png`를 Sprite background slot에 바인딩
  - `LobbyPresentationData`와 `SO_LobbyPresentationData`를 추가해 background/logo/title/subtitle/profile/quit visibility를 data-driven으로 분리
  - Lobby UI를 profile card, title/subtitle, large touch buttons, profile/settings panels, desktop quit button이 있는 player-facing main menu로 재구성
  - Continue는 `이어 하기` + `저장된 진행 없음` disabled state로 고정하고 Profile placeholder panel을 추가
  - EditMode/PlayMode lobby smoke coverage를 presentation/profile/settings/new game 흐름에 맞게 확장
- **Files**: 변경/추가 15개 (LobbyController.cs, LobbyPresentationData.cs, LobbySceneBuilder.cs, Lobby.unity, Art/Lobby/**, SO_LobbyPresentationData.asset, tests)
- **GDD impact**: 없음
- **Blockers**: 최종 로고/세이브 시스템 없음. Continue는 placeholder/disabled 유지.
- **Next**: 실제 로비 로고와 음악 cue 수령 후 `SO_LobbyPresentationData` / `SO_AudioCueCatalog` 바인딩
- **Agent**: Codex

### 2026-05-11 22:47 — Lobby scene and audio scaffold
- **Phase**: W2-2
- **Done**:
  - Lobby scene을 추가하고 BuildSettings 첫 scene으로 설정, PrototypeRoom은 두 번째 playable scene으로 유지
  - New Game / Continue placeholder / Settings panel UI를 런타임 생성하는 `LobbyController`를 추가
  - BGM/Ambience/SFX 채널과 volume clamp, context switch API를 가진 `PrototypeAudioService` 및 `SO_AudioCueCatalog`를 추가
  - Music/Ambience/SFX intake folder와 naming/import README를 추가하고 missing clip safe behavior를 테스트로 고정
  - PrototypeRoom runtime에 Lobby/Exploration/Combat/Boss/Rest/Shop/Event/Ending audio context hooks를 연결
- **Files**: 변경/추가 70여 개 (Lobby.unity, Audio/** folders, Audio scripts/data, LobbyController.cs, PrototypeRoomController.cs, tests, EditorBuildSettings.asset)
- **GDD impact**: 없음
- **Blockers**: 실제 음악/SFX asset 없음. Continue는 save system 확장 전 안전 placeholder/disabled 상태.
- **Next**: 로비를 첫 화면으로 수동 확인하고, 실제 audio asset 수령 시 `SO_AudioCueCatalog`에 cue slot 바인딩
- **Agent**: Codex

### 2026-05-11 22:08 — Screen Layer v2 readability polish
- **Phase**: W2-2
- **Done**:
  - Mac Editor visual QA를 시도했으나 Computer Use Unity 접근 timeout/화면 캡처 privacy 제한으로 직접 GameView 조작은 수행하지 못함
  - 코드 리뷰 기준 P0 overlap risk를 줄이기 위해 result panel을 action/rest 영역 밖 우측 상단으로 이동
  - Rest interaction panel, action buttons, input field, response text 크기를 키우고 Rest 입력 중 result panel을 숨기도록 수정
  - 임시 TestRunner entrypoint로 fresh EditMode/PlayMode 결과를 확보한 뒤 임시 파일과 ProjectSettings 부작용을 제거
- **Files**: 변경 2개 (PrototypeHud.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Codex tooling으로 Unity GameView 직접 시각 QA 불가. 실제 1080x1920 손플레이 확인은 사용자/Editor 수동 QA 필요.
- **Next**: 실제 Game View 1080x1920에서 Floor map, Rest input, Combat, Ending 화면을 손으로 확인하고 남은 폰트/터치 P0만 조정
- **Agent**: Codex

### 2026-05-11 18:10 — Screen Layer v2 gameplay layout
- **Phase**: W2-2
- **Done**:
  - `DemoPresentationData`에 node type별 icon slot을 추가하고 Combat/Event/Rest/Shop/Boss node art를 `SO_DemoPresentationData`에 바인딩
  - `PrototypeHud`에 top/objective/visual/map/NPC/action/result/ending screen layer 패널을 분리하고 map node 버튼에 icon/state tint를 표시
  - normal mode combat presentation에 public enemy name, training bonus, recall/bandage feedback을 보강하고 raw `ENEMY_` 노출을 차단
  - EditMode/PlayMode smoke에 node icon binding, map UI icon, rest/combat layer 확인을 추가
  - Unity batchmode 기본 `-runTests` 결과 XML 미생성 문제는 임시 TestRunner entrypoint로 검증 후 제거
- **Files**: 변경/추가 16개 (PrototypeHud.cs, DemoPresentationData.cs, SO_DemoPresentationData.asset, Art/Nodes/**, PresentationLayerTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음
- **Blockers**: Codex가 GameView를 직접 조작하는 수동 시각 QA는 미수행. PlayMode batchmode는 결과 저장 후 종료 신호를 놓치는 현상이 있어 사용자가 Unity 종료/lock 정리 필요.
- **Next**: Mac Editor에서 실제 1080x1920 손플레이로 map/rest/combat/ending layer 가독성 확인 후 폰트 크기/터치 영역 P0만 조정
- **Agent**: Codex

### 2026-05-11 12:54 — Renamed enemy art mappings finalized
- **Phase**: W2-2
- **Done**:
  - `88ebb31` floor enemy catalog commit을 `origin/Proto`에 push하고 원격 HEAD를 확인
  - `ENEMY_FRACTURE_HOUND` stableId는 유지한 채 presentation/art reference를 `enemy_fracture_hound.png`로 교체
  - `ENEMY_LAMPLIGHTER_01` 점등인을 Floor 5 elite EnemyData, floor pool, presentation slot, catalog 문서에 추가
  - `BOSS_APEX_02` final boss art/stableId mapping과 ending path를 유지하는 테스트를 보강
  - `Assets/_Project/Art/Nodes/` intake 파일 목록은 확인만 하고 node icon binding은 다음 작업으로 보류
- **Files**: 변경/추가/삭제 19개 (EnemyCatalog docs, Art README, FloorEnemyCatalogBuilder.cs, EncounterRuntimeCatalogBuilder.cs, SO_FloorEnemyPool_v0_1.asset, SO_DemoPresentationData.asset, EnemyData/Art assets, tests)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: node icon art를 branching map UI에 바인딩하고, Floor 5 elite encounter가 플레이 중 충분히 노출되는지 수동 QA
- **Agent**: Codex

### 2026-05-11 07:55 — Floor enemy catalog and pools
- **Phase**: W2-2
- **Done**:
  - `be0a350` Rest interaction flow를 `origin/Proto`에 push하고 원격 HEAD를 확인
  - `enemy_catalog_v0.1.md`로 floor/rank 기반 enemy catalog 문서를 추가
  - Floor 1-5 normal/elite/boss enemy pool SO와 missing EnemyData placeholder SO를 추가
  - branching map combat handoff가 selected map node 기준으로 floor enemy pool을 사용하도록 연결
  - 사용 가능한 enemy art 20개를 presentation slot에 바인딩하고 catalog/presentation 테스트를 보강
- **Files**: 변경/추가 96개 (EnemyCatalog docs, FloorEnemyPoolData.cs, FloorEnemyCatalogBuilder.cs, SO_FloorEnemyPool_v0_1.asset, EnemyData/Art/Presentation assets, RuntimeShellTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: Mac Editor 수동 QA에서 floor별 enemy pool 체감, elite encounter 진입 빈도, floor boss 난이도와 reward pacing을 확인
- **Agent**: Codex

### 2026-05-10 22:07 — Mataios rest interaction flow
- **Phase**: W2-2
- **Done**:
  - branching map Rest node를 HP 회복 즉시 처리에서 Mataios interaction panel 흐름으로 전환
  - `rest.ask_mood`, `rest.train`, `rest.recover` action을 deterministic LLM/fallback 호출, temporary response, 1회 commit guard에 연결
  - Ask Mood affinity/utterance reflection, Train next-combat damage +1 one-shot buff, Recover HP restore/internal Glitch decrease를 구현
  - normal HUD에서 Glitch/raw provider/cache internals를 숨기고 Rest input/submit/continue UI와 PlayMode smoke를 추가
  - Rest interaction template/content docs 4개를 추가
- **Files**: 변경/추가 10개 (PrototypeRunState.cs, PrototypeHud.cs, PrototypeRoomController.cs, RuntimeShellTests.cs, PrototypeRoomSmokeTests.cs, Docs/Content/RestInteractions/**, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: Mac Editor 수동 QA에서 Rest input UX, mobile keyboard focus, Mataios temporary response replacement points를 확인
- **Agent**: Codex

### 2026-05-10 21:36 — Branching map event and enemy art binding
- **Phase**: W2-2
- **Done**:
  - `EVT_F01_JAR_ROOM` presentation slot에 jar room background를 바인딩
  - `ENEMY_EMPTY_ARMOR`, `ENEMY_SHADE_03`, `ENEMY_WRAITH_04` enemy-specific presentation slots를 추가
  - combat enemy visual이 encounter background를 유지하면서 enemy stableId slot sprite를 우선 사용할 수 있도록 HUD override를 추가
  - 신규 art import meta를 single sprite mode로 정리하고 EditMode asset binding 테스트를 추가
- **Files**: 변경/추가 12개 (SO_DemoPresentationData.asset, PrototypeHud.cs, PresentationLayerTests.cs, Art/Encounters, Art/Enemies)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: Mac Editor에서 branching map event/combat 화면을 직접 확인하고 Floor 3-4 enemy가 실제 route enemy로 쓰일지 디자인 결정 후 combat handoff data를 별도 조정
- **Agent**: Codex

### 2026-05-10 21:26 — Branching map PlayMode smoke coverage restored
- **Phase**: W2-2
- **Done**:
  - `e1d2d2f` / `7d2ba7b`를 `origin/Proto`에 push해 branching map과 event probability 변경을 보존
  - 제거된 선형 route 직접 주입 PlayMode smoke 9개를 삭제하고 branching map route-action 기반 active smoke로 교체
  - jar probability hint, interactive combat node, rest resolution, shop-before-boss, floor boss clear, final boss ending rest/continue 경로를 PlayMode에서 검증
  - PlayMode ignored count를 9에서 0으로 복구
- **Files**: 변경/추가 2개 (PrototypeRoomSmokeTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: 없음
- **Next**: branching map 테스트는 복구됐으므로 다음은 실제 수동 플레이 기준 map UI/선택지 가독성 보강
- **Agent**: Codex

### 2026-05-10 20:54 — Event probability hints for jar room
- **Phase**: W2-2
- **Done**:
  - `EVT_F01_JAR_ROOM` 선택지에 normal mode 확률 힌트를 추가해 patterned jar가 80% gold / 20% elite combat을 선택 전 표시
  - 항아리 선택지 public label을 stableId 대신 플레이어용 이름으로 표시하도록 HUD 정규화 추가
  - 선택 후 jar outcome 결과를 player-facing result summary로 변환하고 Glitch/raw choice id 노출을 차단
  - EditMode 테스트로 확률 힌트, 공개 UI 라벨, 결과 요약을 고정
- **Files**: 변경/추가 5개 (PrototypeEncounterRuntimeResolver.cs, PrototypeHud.cs, PresentationLayerTests.cs, RuntimeShellTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: PlayMode는 6 passed / 9 ignored 상태 유지. ignored 9개는 기존 branching map 전환 후 남은 선형 route smoke 재작성 과제.
- **Next**: branching map 전용 PlayMode smoke를 추가해 ignored 9개를 active pass로 복구하고, Floor 2+ event probability distortion 정책은 별도 설계 결정 후 구현
- **Agent**: Codex

### 2026-05-10 18:28 — Branching floor map and jar event nodes
- **Phase**: W2-2
- **Done**:
  - Floor 1-5 deterministic branching map state를 추가하고 Combat/Event/Rest/Shop/Boss node type을 HUD 선택 흐름에 연결
  - 모든 floor path가 branch layer에서 pre-boss Shop으로 수렴한 뒤 Boss로 이어지도록 `SO_Room_Prototype` route를 갱신
  - `EVT_F01_JAR_ROOM` event script 문서와 runtime SO를 추가하고 patterned/plain/cracked jar deterministic outcome을 구현
  - Rest choice가 HP 회복, 내부 Glitch 감소, NPC fallback reaction을 적용하도록 runtime resolver에 연결하고 normal HUD에서 Glitch 노출을 숨김
  - floor별 merchant presentation hook을 `DemoPresentationData`에 추가하고 EditMode/PlayMode smoke를 branching map 기준으로 갱신
- **Files**: 변경/추가 22개 (PrototypeFloorMap.cs, PrototypeRunState.cs, PrototypeHud.cs, SO_Room_Prototype.asset, EVT_F01_JAR_ROOM, Docs/Content/EventScripts/**)
- **GDD impact**: 없음 (GDD §7.0 OQ-012 결정 범위 내 구현)
- **Blockers**: PlayMode는 6 passed / 9 ignored 상태. ignored 9개는 제거된 선형 route 직접 주입 smoke라 branching map 전용 smoke로 재작성 필요.
- **Next**: PlayMode 선형 smoke를 ignored 상태로 두지 말고 branching map 경로별 shop/rest/combat/failure/ending smoke로 분해해 15/15 active pass로 복구
- **Agent**: Codex

### 2026-05-10 13:35 — Spine-ready cutscene pipeline prep
- **Phase**: W2-2
- **Done**:
  - 2000x2000 cutscene source convention과 Spine export naming을 README에 고정
  - Spine runtime 없이 동작하는 `CutsceneData` placeholder SO 3종을 추가
  - memory fracture, final boss reveal, ending choice presentation slot을 fallback sprite 기반 cutscene으로 바인딩
  - Spine namespace/package 미설치 상태 compile과 EditMode/PlayMode smoke를 검증
- **Files**: 변경/추가 11개 (SpineSource/Data Cutscenes README, SO_CutsceneSpine_*, SO_DemoPresentationData, RuntimeShellTests.cs)
- **GDD impact**: 없음
- **Next**: Spine runtime 설치 승인 전까지는 fallback sprite 컷신만 사용하고, 다음 단계에서 2000x2000 source export 파일 수급 후 import setting 검증
- **Agent**: Codex

### 2026-05-10 13:26 — Combat readability and decision feedback
- **Phase**: W2-2
- **Done**:
  - Attack/Defend/Skill 라운드 결과에 적/플레이어 HP 변화, 선택 행동, 방어/정찰/콤보 결과를 명시
  - Skill을 `ABILITY_SCOUT` 전용 의미 선택지로 고정하고 미보유 시 버튼/피드백에서 정찰 필요를 표시
  - Bandage 전투 시작 회복과 Recall Anchor 1회 개입 가능성을 combat panel에 노출
  - final boss prepared-player 3-6턴 승리 sanity와 Scout skill/Recall smoke를 테스트로 보강
- **Files**: 변경/추가 7개 (PrototypeRunState.cs, PrototypeRoomController.cs, PrototypeHud.cs, combat 관련 테스트, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Next**: Mac Editor 손플레이에서 Attack/Defend/Scout Skill 선택 이유가 실제로 읽히는지 확인하고, 필요하면 전투 패널 레이아웃을 Screen Layer v2로 분리
- **Agent**: Codex

### 2026-05-10 12:25 — Player-facing vertical slice flow polish
- **Phase**: W2-2
- **Done**:
  - normal mode route/result/combat/end-state labels에서 raw stableId/debug-like 문구 노출을 줄이고 debug toggle은 유지
  - Floor 목표 표시를 현재 층, 현재 조우, 다음 행동 이유 중심으로 재작성
  - shop/item/ability/memory/combat/floor/end 결과 요약을 player-facing 시스템 문구로 정리
  - PlayMode smoke 기대값을 normal-mode public UI 기준으로 갱신
- **Files**: 변경/추가 4개 (PrototypeHud.cs, PresentationLayerTests.cs, PrototypeRoomSmokeTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Next**: Mac Editor에서 초회 플레이어 관점으로 Floor 1→5→ending.rest/continue를 다시 손플레이하고, 남은 화면 조악함은 Screen Layer v2에서 레이아웃/타이포 구조로 분리
- **Agent**: Codex

### 2026-05-10 11:30 — Hands-on HUD readability blocker polish
- **Phase**: W2-2
- **Done**:
  - 사용자 손플레이 QA에서 전투/보스 도달/ending.continue는 확인됐지만 ending.rest와 UI 식별성은 미확정 blocker로 기록
  - `PrototypeHud`의 기본 플레이 UI 텍스트, 선택지, 진행/다음층/재시작/엔딩 버튼, 전투 패널 크기와 대비를 상향
  - 공통 HUD text best-fit 하한을 올려 public UI가 지나치게 작은 글씨로 축소되지 않도록 조정
  - fresh EditMode/PlayMode로 Floor 1→5 route 및 ending smoke 회귀 확인
- **Files**: 변경/추가 2개 (PrototypeHud.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: ending.rest는 아직 사용자 손플레이로 미확인. 실제 Mac Editor 화면에서 새 가독성 패스 체감 확인 필요.
- **Next**: Unity Editor에서 ending.rest까지 직접 확인하고, 남은 UI 조악함은 Screen Layer v2로 별도 정리
- **Agent**: Codex

### 2026-05-09 18:30 — Mac Editor hands-on input QA blocker
- **Phase**: W2-2
- **Done**:
  - `4455d95`를 `origin/Proto`에 push하고 원격 HEAD 확인
  - Mac Editor `PrototypeRoom` PlayMode에서 `진행` 버튼 표시까지 확인
  - Computer Use 기반 실제 GameView 클릭/키 입력으로는 Floor 1 choice open이 안정 재현되지 않는 blocker 확인
  - 씬에 직렬화된 HUD Text도 raycast를 차단하도록 보강해 버튼 클릭 가로채기 가능성을 제거
  - fresh EditMode/PlayMode로 route, presentation asset binding, ending smoke 회귀 확인
- **Files**: 변경/추가 2개 (PrototypeHud.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Codex Computer Use 입력이 Unity GameView에 안정 전달되지 않아 사람 손플레이 Floor 1→5→ending.rest/continue 완료는 미확정. 사용자 직접 입력 확인 필요.
- **Next**: 사용자가 Editor에서 `진행` 버튼 또는 Enter/N, 1-4, A/D/S, ending 1/2로 직접 완주 확인
- **Agent**: Codex

### 2026-05-09 17:49 — Late-route presentation asset binding
- **Phase**: W2-2
- **Done**:
  - 후반부 신규 background/portrait/enemy art를 `SO_DemoPresentationData` 슬롯에 연결
  - `ENC_COMBAT_GATE_01/02/03`, `ENC_SHOP_02`, `ENC_MEMORY_FRAGMENT_03/05`, `run.clear` presentation slot 검증 추가
  - run clear/ending choice 상태에서 `ending_choice_bg`를 표시하는 fallback hook 추가
  - PlayMode에서 fracture hound, collapse echo, final boss, ending background 적용을 smoke로 고정
- **Files**: 변경/추가 21개 (Art PNG/meta, SO_DemoPresentationData.asset, PrototypeHud.cs, PresentationLayerTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음
- **Blockers**: 실제 Android 기기 화면에서 신규 후반부 아트 가독성/성능은 아직 미검증
- **Next**: Editor 손플레이로 Floor 1→5 presentation 확인 후 Android 실기 smoke
- **Agent**: Codex

### 2026-05-09 17:22 — Hands-on completion input blocker fix
- **Phase**: W2-2
- **Done**:
  - `a61e86e`을 `origin/Proto`에 push하고 원격 HEAD 확인
  - Mac Editor `PrototypeRoom` Play 진입 후 `진행` 버튼이 표시되지만 실제 클릭/키 입력이 안정적으로 route open으로 이어지지 않는 P0 확인
  - 기존 EventSystem이 있을 때도 `InputSystemUIInputModule`을 보장하고 HUD 비상호작용 텍스트 raycast를 차단
  - fresh EditMode/PlayMode로 기존 Floor 1→5 route와 ending smoke 회귀 확인
- **Files**: 변경/추가 2개 (PrototypeHud.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: 실제 사람 손플레이 Floor 1→5→ending.rest/continue 최종 확인은 사용자 입력으로 남음. Codex UI 자동화는 Unity GameView 클릭 재현성이 낮음.
- **Next**: 사용자가 Editor에서 `진행`/1-4/A-D-S/ending 1-2 키로 직접 완주 확인 후, 동일 경로를 Android 실기 smoke로 진행
- **Agent**: Codex

### 2026-05-09 16:27 — Manual completion route QA fixes
- **Phase**: W2-2
- **Done**:
  - `69d9936`을 `origin/Proto`에 push하고 원격 HEAD 확인
  - Mac Editor `PrototypeRoom`에서 시작 직후 HUD/`진행` 버튼이 보이지 않거나 입력되지 않는 P0 확인
  - 씬 시작 시 run/HUD를 자동 초기화하고 HUD 버튼용 EventSystem을 보장
  - Mac Editor용 키보드 fallback과 route-action full completion PlayMode smoke 추가
- **Files**: 변경/추가 5개 (PlayerMovementController.cs, PrototypeRoomController.cs, PrototypeHud.cs, PrototypeRoomSmokeTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Codex macOS 입력 자동화로 실제 마우스 클릭 완주는 안정 재현하지 못함. 동일 UI onClick route는 PlayMode에서 Floor 1→5→ending.continue까지 검증.
- **Next**: 사람이 직접 Editor에서 마우스/키보드로 완주 확인 후 Android 실기 smoke 진행
- **Agent**: Codex

### 2026-05-09 14:17 — Mac Editor playability route entry pass
- **Phase**: W2-2
- **Done**:
  - `bb19e20`을 `origin/Proto`에 push하고 원격 HEAD 확인
  - Mac Unity Editor에서 `PrototypeRoom`을 열어 PlayMode 수동 진입성 확인
  - 노드 이동/충돌 방식만으로는 선택형 진행 진입점이 약한 P0를 확인하고 HUD `진행` 버튼 추가
  - `진행` 버튼이 현재 route encounter choice를 열고, choices 표시 중에는 숨도록 PlayMode smoke 고정
- **Files**: 변경/추가 4개 (PrototypeRoomController.cs, PrototypeHud.cs, PrototypeRoomSmokeTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Mac Editor에서 자동화 입력으로 Floor 1→ending 전체 수동 완주는 재현하지 못함. 전체 route는 fresh PlayMode로 검증.
- **Next**: Android 실기 전, Editor에서 사람이 직접 마우스/터치로 `진행` 버튼 기반 Floor 1→ending 완주 확인
- **Agent**: Codex

### 2026-05-09 12:00 — Player-facing gameplay loop polish
- **Phase**: W2-2
- **Done**:
  - 기본 HUD/route/result/combat 표시에서 raw stableId/textKey 노출을 줄이고 public play label을 적용
  - 선택 버튼에 구매 비용, 획득, 메모리 해금, 전투 시작, 골드 부족 등 consequence hint를 표시
  - result panel과 combat feedback을 HP/Gold/Mental/Glitch/Affinity/획득/진행 상태 중심으로 압축
  - Floor 1→5, final boss, ending choice smoke가 public label 기준으로 통과하도록 PlayMode 갱신
- **Files**: 변경/추가 5개 (PrototypeEncounterRuntimeResolver.cs, PrototypeHud.cs, PresentationLayerTests.cs, PrototypeRoomSmokeTests.cs, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Android APK build artifact는 존재하나 ADB 연결 기기 없음으로 실기 smoke 미수행
- **Next**: 실제 Android 기기 연결 후 install/run smoke 및 터치/세로 화면 확인
- **Agent**: Codex

### 2026-05-08 18:17 — Floor 1-5 final boss route and Spine intake scaffold
- **Phase**: W2-1
- **Done**:
  - PrototypeRoom deterministic route를 Floor 1→5까지 확장하고 Floor 5 `ENC_COMBAT_GATE_03`에 `BOSS_APEX_02`를 연결
  - Floor 2 BossGate는 Floor 3 unlock으로 유지하고 final boss victory가 기존 ending choice flow로 진입하도록 PlayMode smoke 갱신
  - `BOSS_APEX_02`를 30 HP / 4 ATK / reward 30 gold 기준으로 세로 slice 클리어 가능한 밸런스로 조정
  - run clear / failed / ending fallback reaction key coverage를 보강
  - Spine runtime 미설치 상태에서 안전한 `SpineSource`, exported `Spine`, cutscene data folder와 `CutsceneData` fallback slots를 추가
- **Files**: 변경/추가 51개 (주요: `SO_Room_Prototype.asset`, `SO_Encounter_ENC_COMBAT_GATE_03.asset`, `BOSS_APEX_02`, `CutsceneData.cs`, `PrototypeRoomSmokeTests.cs`, `Assets/_Project/Spine/**`)
- **GDD impact**: 없음
- **Blockers**: Android build smoke는 `Android build target is not installed in this Unity Editor.`로 APK 생성 불가
- **Next**: Android Build Support 설치 후 BuildScript.BuildAndroid 재실행, 이후 실기 smoke와 final boss route 수동 플레이 QA
- **Agent**: Codex

---

### 2026-05-08 10:29 — Ending choice flow implementation blocked on Unity licensing
- **Phase**: W2-1
- **Done**:
  - BossGate victory 이후 `run.clear`에서 엔딩 선택 pending 상태로 멈추도록 런 상태를 확장
  - `PLACEHOLDER_ENDING_REST` / `PLACEHOLDER_ENDING_CONTINUE` HUD 버튼과 1회 commit 정책 추가
  - `ending.rest`는 최종 잠금, `ending.continue`는 restart-ready 루프로 분리
  - restart 후 memory fragment / reflection repo / deterministic LLM cache 유지와 native model 없는 fallback 응답을 테스트 경로에 추가
  - 구현 완료
  - C# compile pass: generated C# csproj 기준 `HwigiTower.EditModeTests.csproj` / `HwigiTower.PlayModeTests.csproj` 컴파일 통과
- **Files**: 변경/추가 7개 (`Assets/_Project/Scripts/Run/**`, `Assets/_Project/Scripts/UI/PrototypeHud.cs`, `Assets/_Project/Tests/**`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음
- **Blockers**: Unity tests blocked by license. Unity batchmode fresh EditMode/PlayMode 및 Android build smoke가 LicenseClient `Licensing initialization failed`로 실행 불가. 외부 Obsidian PROGRESS prepend 대신 repo 내부 PROGRESS에 기록.
- **Next**: Unity Hub/Editor를 열어 라이선스 channel 복구 후 fresh EditMode, PlayMode, Android BuildScript.BuildAndroid 실행 및 통과 시 커밋
- **Agent**: Codex

---

### 2026-05-08 09:38 — Post-BossGate gameplay blocker cleanup
- **Phase**: W2-1
- **Done**:
  - Floor 2 shop refs를 정식 stableId로 정리: `ITEM_FIELD_BANDAGE`, `ABILITY_RECALL_ANCHOR`
  - catalog-backed unknown ability refs가 Skill을 활성화하지 않도록 보강
  - restart 시 memory fragment refs / reflection repo / deterministic LLM cache를 보존하고 floor / hp / gold / combat / route / recall-anchor used flag는 reset하도록 고정
  - failure result에 `Run failed` / `Restart available` 표시 추가
  - 관련 EditMode / PlayMode coverage 추가
- **Files**: 변경/추가 8개 (`Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_SHOP_001.asset`, `Assets/_Project/Scripts/Encounters/EncounterRuntimeCatalogBuilder.cs`, `Assets/_Project/Scripts/Run/**`, `Assets/_Project/Scripts/UI/PrototypeHud.cs`, `Assets/_Project/Tests/**`)
- **GDD impact**: 없음
- **Blockers**: 외부 Obsidian PROGRESS prepend는 승인/사용량 제한으로 미수행. repo 내부 PROGRESS에 기록.
- **Next**: core loop 수동 플레이 체크 후 Android build smoke와 AI fallback/model artifact 연결 점검
- **Agent**: Codex

---

### 2026-05-06 23:20 — W2 vertical slice UI/asset/combat/cutscene spec 작성
- **Phase**: W2-1
- **Done**:
  - Unity 없이 코드/문서/기존 screenshot 기준으로 화면별 UI wireframe spec 작성
  - stableId/usage 기준 Asset Integration Manifest v0.2 작성
  - Attack/Defend/Skill 시연 이해도와 combat feedback 부족분을 acceptance criteria로 정리
  - lightweight cutscene storyboard, writer decision packet v0.2, vertical slice test plan 작성
- **Files**: 변경/추가 7개 (`Docs/Outsource/Juho/W2ProductionReadiness/**`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음
- **Next**: writer v0.2 결정 회수 후 raw key/debug label toggle + combat feedback P0 구현 세션 진행
- **Agent**: Codex

---

### 2026-05-06 22:40 — Unityless W2 production readiness 문서 패킷 작성
- **Phase**: W2-1
- **Done**:
  - `origin/Proto` 기준 커밋 `57077ee`를 `Juho/Codex`에 병합해 viewport screenshot / CombatGate layering fix 맥락 반영
  - Unity 없이 기존 문서와 5개 screenshot만 기반으로 W2 production readiness 검토 범위 정리
  - demo recording runbook, screenshot visual audit, debug label 제거 명세, 다음 개발 지시서 작성
  - 아트/사운드/AI 상태표와 모델 수령 시 Unity 전 평가 packet 작성
- **Files**: 변경/추가 8개 (`Docs/Outsource/Juho/W2ProductionReadiness/**`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음
- **Next**: 큰 world/debug node label 숨김 구현 후 1080x1920 recording screenshot QA 진행
- **Agent**: Codex

---

### 2026-05-06 15:42 — W2 demo readiness docs 최신 전투 UI 기준 refresh
- **Phase**: W2-1
- **Done**:
  - `origin/Proto` 최신 `9300cc1`을 `Juho/Codex`에 병합해 interactive combat UI / Mataios portrait 구현 상태 확인
  - W2 readiness audit에서 CombatGate, DemoComplete, Combat UI, Mataios portrait 상태를 ready-for-review로 갱신
  - 전투 QA 스크립트를 manual Attack/Defend 중심 경로로 갱신하고 auto-resolve를 회귀 smoke 경로로 낮춤
  - 마타이오스 art brief와 dev handoff에서 portrait wiring / combat button TODO를 QA·polish 항목으로 재분류
- **Files**: 변경/추가 5개 (`Docs/Outsource/Juho/W2DemoReadiness/**`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음
- **Next**: portrait viewport screenshot QA와 Android build host blocker 해결
- **Agent**: Codex

---

### 2026-05-06 15:00 — W2 demo readiness audit and handoff packet 작성
- **Phase**: W2-1
- **Done**:
  - 외주 요청 v3 기준 필수 문서/GDD/디자인/AI 모델/데모 QA 맥락 검토
  - W2 데모 준비 상태 audit, 전투 QA 스크립트, 마타이오스 일러스트 적용 기준 작성
  - AI 모델 수령 acceptance checklist, 작가 결정 packet, 개발 handoff TODO 작성
  - 받은 `마타이오스 전신.png`를 확인하고 S0-S2 portrait 후보 기준 정리
- **Files**: 변경/추가 7개 (`Docs/Outsource/Juho/W2DemoReadiness/**`, `Docs/Project/hwiglija-tower-progress.md`)
- **GDD impact**: 없음
- **Next**: 전투 데모 경로를 auto-resolve로 고정할지, Attack/Defend/Skill 수동 UI를 먼저 붙일지 결정 후 P0 개발 진행
- **Agent**: Codex

---

### 2026-05-05 18:45 — GDD v0.3.0 업데이트 및 전투 수직 슬라이스 구현
- **Phase**: W2-1
- **Done**:
  - GDD v0.3.0 승격: 3택 턴제 전투(D-022), 특성 시스템(D-009), 밸런스 시트, 12능력·4시너지·7적 상세 명세 반영
  - `CombatController`: Attack/Prepare 액션 및 데미지 경감 로직 구현
  - `PrototypeRunState`: 인터랙티브 전투 모드(`ResolveCombatRoundInteractive`) 및 `AutoResolveCombat` 테스트 플래그 추가
  - `PrototypeRunSnapshot`: UI용 실시간 전투 상태(Player/Enemy HP, InCombat 여부) 필드 확장
  - OQ-004 해결: 8일 데모용 Deterministic Fake/Cache 모델 인테이크 전략 확정
  - `Juho/Codex` 신규 문서(리뷰 스크립트, 메모리 스펙 등) 통합
- **Files**: 변경/추가 8개 (주요: `hwiglija-tower-gdd.md`, `CombatController.cs`, `PrototypeRunState.cs`, `PrototypeRunSnapshot.cs`, `DECISION_LOG_20260505.md`)
- **GDD impact**: v0.3.0 MAJOR 수준의 시스템 구체화, D-022 추가, D-009 갱신, OQ-011~014 등록, OQ-004 close
- **Next**: `CombatGate` UI에 공격/준비 버튼 연결 및 `ABILITY_SWORD_01` 등 실제 능력 수치 바인딩 시작
- **Agent**: Codex

---

### 2026-05-05 01:12 — external AI model intake 폴더/manifest 보강
- **Phase**: W2-1
- **Done**:
  - `/goals` 재검증 기준으로 `Proto` preflight 수행 (`git fetch`, `git switch Proto`; rebase pull은 unstaged user changes로 중단)
  - `mataios-demo-sft-v0.1` model intake manifest, example config, tokenizer/compiled/eval placeholder 추가
  - `_training/datasets`, `_training/adapters`, `_training/evals` placeholder tracking 보강
  - model intake folder existence EditMode test 추가
  - `git diff --check`, forbidden search, fresh EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 17개 (주요: `Assets/_Project/Models/mataios-demo-sft-v0.1/**`, `Assets/_Project/Models/_training/**`, `Assets/_Project/Tests/EditMode/RuntimeShellTests.cs`)
- **GDD impact**: 없음
- **Blockers**: `git pull --rebase origin Proto` 는 기존 unstaged user changes 때문에 실행 불가. `origin/Proto` 는 local HEAD 대비 behind 0 / ahead 1 상태.
- **Next**: 외부 모델 산출물 수령 후 `model_config.example.json` 값을 실제 checksum/runtime path로 복사해 `LLMRuntimeSettings` SO와 Android plugin smoke 연결
- **Agent**: Codex

---

### 2026-05-04 17:28 — external AI model runtime intake 준비
- **Phase**: W1-2
- **Done**:
  - `LLMRuntimeSettings` / `LLMRuntimeConfig` / provider mode 기반 Fake-OnDevice-CachedOnDevice 선택 구조 추가
  - LLM 전용 `model_config.json` loader 와 safe provider factory 추가
  - MLC bridge에 model/tokenizer path 초기화 지점 추가 및 native plugin 부재 시 deterministic fake fallback 유지
  - `Assets/StreamingAssets/LLM/mataios-demo-sft-v0.1/` intake placeholder 와 대용량 모델 commit 금지 README 추가
  - EditMode model intake tests 추가 및 fresh EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 22개 (주요: `Assets/_Project/Scripts/LLM/**`, `Assets/StreamingAssets/LLM/**`, `Assets/_Project/Tests/EditMode/RuntimeShellTests.cs`)
- **GDD impact**: 없음
- **Next**: 외부 SFT 산출물 도착 후 `LLMRuntimeSettings` SO asset 생성 및 Android native plugin path smoke
- **Agent**: Codex

---

### 2026-05-04 16:04 — demo route UI 바인딩 보강
- **Phase**: W1-2
- **Done**:
  - review/juho-demo-ui 브랜치에서 DemoRunPath route indicator 실패 재현
  - scene 배치 `Prototype HUD`가 `PrototypeSceneRuntimeBuilder` 경로를 타지 않아 route labels 를 받지 못하는 원인 확인
  - `PrototypeRoomController`가 active HUD에 RoomDefinition DemoRunPath 를 주입하도록 단일 바인딩 경로 보강
  - `git diff --check`, forbidden search, fresh EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 1개 (`Assets/_Project/Scripts/Run/PrototypeRoomController.cs`)
- **GDD impact**: 없음
- **Next**: `Juho/Codex` UI polish merge 진행 후 Android build smoke 준비
- **Agent**: Codex

---

### 2026-05-04 10:59 — deterministic demo run progression 고정
- **Phase**: W1-2
- **Done**:
  - `SO_Room_Prototype` 에 Shop → MoralChoice → MemoryFragment → CombatGate demo path를 SO 참조로 고정
  - `PrototypeRunState` 에 demo progression cursor/completion 상태 추가 및 선택지 적용 성공 시에만 진행
  - `PrototypeRoomController` 를 demo encounter selection 단일 진입점으로 정리하고 HUD에 next/demo.complete 상태 표시
  - resolved encounter 재방문 시 effect/progression 중복 없이 `already resolved: <choiceStableId>` 유지
  - EditMode demo progression tests 및 PlayMode 1-run smoke 확장
- **Files**: 변경/추가 12개 (주요: Assets/_Project/Scripts/Run/**, Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset, Assets/_Project/Tests/**)
- **GDD impact**: 없음
- **Blockers**: 없음. Unity Test Runner는 `-quit` 동시 지정 시 테스트 전 종료되어, fresh 검증은 `-runTests` 자체 종료 방식으로 실행.
- **Validation**: `git diff --check` 통과, forbidden search 통과, fresh EditMode 52/52 pass, fresh PlayMode 5/5 pass
- **Next**: Android build 검증 전, demo complete 이후 임시 결과 UX와 combat 후 HP/defeat edge를 한 번 더 얇게 점검
- **Agent**: Codex

---

### 2026-05-04 10:58 — Proto ahead 12 GitHub push
- **Phase**: W2-1
- **Done**:
  - local `Proto...origin/Proto [ahead 12]` 상태 확인
  - dirty working tree/untracked 파일은 그대로 둔 채 커밋된 12개만 push
  - `origin/Proto` 를 `6f34b40..c7af07e` 로 업데이트
- **Files**: repo 파일 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: 남은 unstaged 작업 범위 확인 후 다음 보존 커밋 또는 hygiene 정리
- **Agent**: Codex

---

### 2026-05-04 10:33 — encounter choice UX/revisit 정책 검증 커밋
- **Phase**: W2-1
- **Done**:
  - `71a7c8a` 위 추가 7개 변경 범위를 확인하고 unrelated untracked 파일은 제외
  - `PrototypeHud` result message 영역, 모바일 터치 버튼 레이아웃, disabled best-fit 개선 변경 검증
  - `PrototypeRunState` nodeId+encounterId resolved choice 기록 및 재방문 effect 재적용 차단 검증
  - fresh Unity EditMode 49/49, PlayMode 5/5, `git diff --check`, forbidden search 통과
  - `c7af07e` 커밋으로 UX/result/revisit 정책 변경 보존
- **Files**: 커밋 7개 (PrototypeHud.cs, PrototypeRunState.cs, PrototypeRoomController.cs, NodeInteractionController.cs, PrototypeSceneRuntimeBuilder.cs, RuntimeShellTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음 (최종 텍스트/서사 결정 없음; P4 결정성 유지)
- **Blockers**: unrelated untracked 파일 다수는 hygiene 후보로 남김
- **Next**: D1-D2 데모 플레이 path 고정: shop/moral/memory/combat gate 포함 1-run progression 지시서 작성 및 개발 세션 배분
- **Agent**: Codex

---

### 2026-05-03 23:53 — baked encounter choice button UI 연결
- **Phase**: W1-2
- **Done**:
  - `PrototypeHud` 에 runtime choice button 생성/정리, DisabledVisible reason 표시, InputSystem UI event path 추가
  - `PrototypeRoomController`/`NodeInteractionController` 가 baked choices 를 auto-resolve 하지 않고 버튼 선택 후 `ResolveEncounterChoice` 로 commit 하도록 변경
  - `PrototypeEncounterChoiceView` 에 `textKey` 전달 추가: 버튼에는 placeholder key 또는 stableId 표시
  - Prototype shop/battle node fixture 에 smoke 대상 baked encounter 참조 연결
  - PlayMode smoke 추가: shop insufficient/sufficient, moral affinity, memory unlock hidden policy, combat handoff button flow 검증
- **Files**: 변경 7개 (PrototypeHud.cs, PrototypeRoomController.cs, NodeInteractionController.cs, PrototypeRoomSmokeTests.cs, Prototype Node assets)
- **GDD impact**: 없음 (최종 NPC/서사/도덕 텍스트 미작성, placeholder key 만 표시)
- **Blockers**: 없음
- **Next**: 선택지 버튼 레이아웃/터치 UX 폴리싱 후 encounter 결과 메시지 패널과 node 재방문 정책 정리
- **Agent**: Codex

---

### 2026-05-03 23:44 — v0.3 encounter catalog/bake/bootstrap commit 보존
- **Phase**: W1-2
- **Done**:
  - 요청 scope 파일만 staging 후 `5c0da93` 커밋 생성
  - v0.3 encounter runtime catalog, full 25 encounter bake assets, item/reward/enemy/memory fragment/catalog data 포함
  - encounter catalog builder/reference validator, bootstrap scene/controller path, EditMode/PlayMode smoke tests 포함
  - unrelated local/Obsidian/NPC training/result XML 파일은 stage 제외
  - `git diff --check` 및 staged scope 검토 완료
- **Files**: 커밋 118개 (주요: Assets/_Project/Scripts/Encounters/**, Assets/_Project/Data/{Encounters,Catalogs,Items,Rewards,Enemies,MemoryFragments}/**, Docs/ExternalSpecs/EncounterPipeline/v0.3/**)
- **GDD impact**: 없음
- **Blockers**: 로컬 test result XML 은 stale(33/33, 1/1)로 남아 있어 사용자 제공 최신 검증 결과(48/48, 4/4)를 기준으로 커밋
- **Next**: unrelated untracked 정리 또는 `.gitignore` 반영 승인 후 repo hygiene follow-up
- **Agent**: Codex

---

### 2026-05-03 23:40 — catalog/bootstrap 완료 확인 + 선택지 UI 후속 지시 정리
- **Phase**: W1-2
- **Done**:
  - 개발 세션 보고 확인: EditMode 48/48, PlayMode 4/4, scene/controller catalog attach smoke 통과
  - `SO_EncounterRuntimeCatalog` scene/bootstrap 주입이 완료된 상태로 판단
  - 현재 git 상태 확인: catalog/bake/bootstrap 관련 변경이 아직 working tree 에 남아 있어 보존 커밋 우선 필요
  - 다음 개발 범위를 보존 커밋과 baked encounter 선택지 버튼 UI 연결로 분리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: 현재 변경 묶음을 stage/commit 후, full pack encounter choice UI 버튼 흐름 구현
- **Agent**: Codex

---

### 2026-05-03 23:37 — encounter runtime catalog scene/bootstrap 주입
- **Phase**: W1-2
- **Done**:
  - `PrototypeRoomController` 에 `EncounterRuntimeCatalogData` serialized reference 와 read-only accessor 추가
  - `BeginRun()` 이 새 `PrototypeRunState` 생성 직후 catalog 를 자동 attach 하도록 연결
  - `PrototypeSceneRuntimeBuilder` 의 generated scene path 도 catalog injection 을 전달하도록 확장
  - `PrototypeRoom.unity` 의 static controller 에 `SO_EncounterRuntimeCatalog` 직접 참조 바인딩
  - PlayMode smoke 추가: scene controller path 로 catalog attach 확인 후 `ITEM_FIELD_BANDAGE` purchase effect resolved
- **Files**: 변경 4개 (PrototypeRoomController.cs, PrototypeSceneRuntimeBuilder.cs, PrototypeRoom.unity, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음
- **Blockers**: 이전 stableId catalog integration 변경 묶음이 아직 커밋되지 않은 상태
- **Next**: 현재 catalog/bake/bootstrap 변경 묶음을 stage/commit 한 뒤 full pack node selection UI 버튼화로 진행
- **Agent**: Codex

---

### 2026-05-03 23:32 — 개발 세션 stableId catalog integration 후속 범위 정리
- **Phase**: W1-2
- **Done**:
  - 개발 세션 결과 확인: EditMode 48/48, PlayMode 3/3, full pack dry-run 25 encounters reports=0
  - `EncounterRuntimeCatalogData` 와 full pack catalog/memory integration 이 완료된 상태로 판단
  - 현재 남은 병목을 scene/bootstrap catalog 주입과 full pack runtime selection smoke 로 분리
  - `PrototypeRoomController.BeginRun()` 이 catalog 를 자동 attach 하지 않는 점과 scene/runtime builder 혼재 상태를 확인
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Next**: 개발 세션에 SO_EncounterRuntimeCatalog scene/bootstrap 주입 및 full pack node smoke 확대 지시
- **Agent**: Codex

---

### 2026-05-03 23:29 — v0.3 stableId catalog + memory fragment runtime integration
- **Phase**: W1-2
- **Done**:
  - v0.3 full pack non-memory stableId catalog lookup 유지 및 item/reward/ability/enemy placeholder SO 검증
  - MemoryFragmentData SO와 catalog lookup 추가: MEM_FRAGMENT_01..05 titleKey/bodyKey/stage placeholder 생성
  - baker가 MemoryFragmentLocked/UnlockMemoryFragment stableId를 runtime sub-data로 직렬화하도록 확장
  - RunState/Resolver가 memory fragment unlock을 deterministic stableId set으로 기록하고 Hidden/DisabledVisible 흐름 유지
  - full pack dry-run/bake, missing catalog refs, duplicate stableId, EditMode/PlayMode 검증 통과
- **Files**: 변경/추가 다수 (주요: Assets/_Project/Scripts/Encounters/**, Assets/_Project/Scripts/Run/**, Assets/_Project/Data/**, Docs/ExternalSpecs/EncounterPipeline/v0.3/**)
- **GDD impact**: 없음 (OQ-006 최종 텍스트 미작성, placeholder key만 추가)
- **Blockers**: 없음
- **Next**: scene/bootstrap에서 SO_EncounterRuntimeCatalog 주입 경로 고정 후 full pack encounter selection smoke 확대
- **Agent**: Codex

---

### 2026-05-03 15:40 — memory fragment catalog v0.1 재검수
- **Phase**: W1-2
- **Done**:
  - 외주 `memory_fragment_catalog_v0.1.md` 확인
  - `MEM_FRAGMENT_01..05` 5개가 full pack 참조와 1:1 일치함을 검증
  - title/body/writer note 가 모두 `PLACEHOLDER_*` key 로 유지되고 최종 서사 텍스트가 없음을 확인
  - stage 값은 v0.2 외주 stage 명칭이며 runtime `NpcStage.S1..S5` 매핑 대상으로 판단
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (OQ-006 open 유지; 작가 최종 텍스트 작성 전 placeholder catalog 만 수용)
- **Next**: 개발 세션에서 stableId catalog integration 작업에 memory fragment placeholder tracking 을 포함
- **Agent**: Codex

---

### 2026-05-03 15:10 — encounter runtime resolver + choice/effect flow 연결
- **Phase**: W1-2
- **Done**:
  - rebase 완료 상태 확인: `Proto` 정상 브랜치, `hwigija-tower` gitlink 없음, `hwigija-tower.slnx` 미수정
  - `EncounterData` 에 choices/requirements/effects/unavailablePolicy/combatHandoff runtime sub-data 직렬화 추가
  - `PrototypeEncounterRuntimeResolver` 추가: StatAtLeast/HasAbility, Hidden/DisabledVisible, ModifyGold/Mental/Glitch/Affinity/Hp, AddItem/AddAbility/GrantRewardBundle, StartCombat 처리
  - Items/Rewards ScriptableObject 모델 추가 및 baked sample encounter 3개에 runtime sub-data bake
  - baker dry-run/sourceHash/stableId conflict report 와 trailing whitespace cleanup 추가
  - EditMode 39/39, PlayMode smoke 3/3, diff whitespace/금지 runtime escape/non-determinism 검색 통과
- **Files**: 변경/추가 18개 (주요: EncounterData.cs, EncounterPipelineV02Baker.cs, PrototypeEncounterRuntimeResolver.cs, PrototypeRunState.cs, ItemData.cs, RewardBundleData.cs, RuntimeShellTests.cs, PrototypeRoomSmokeTests.cs)
- **GDD impact**: 없음 (D-016 정합 유지; schema v0.2/escape/NPC 최종 텍스트 변경 없음)
- **Blockers**: 실제 item/reward/ability 카탈로그 lookup 과 선택지 UI 버튼화는 아직 최소 구현 밖
- **Next**: 선택지 UI를 실제 버튼 입력으로 분리하고, stableId catalog lookup 으로 Item/Reward/Ability SO 지급을 완성
- **Agent**: Codex

---

### 2026-05-03 15:07 — 외주 full encounter pack v0.3 검수
- **Phase**: W1-2
- **Done**:
  - `pack_FULL_25_V003.json` JSON 문법 및 count 검증: 25개, GDD 분류 8/5/7/3/2 일치
  - escape/direct text/old NpcStage 금지 토큰 없음 확인
  - choice/effect semantic check 통과: MoralChoice, Shop, StartCombat 기본 규칙 충족
  - pack 참조 stableId 추출: item 3개, ability 2개, reward 2개, enemy 3개, memory fragment 5개
  - 현재 Unity placeholder SO id 와 외주 stableId 불일치 및 item/reward catalog 상세 필드 부족 확인
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (콘텐츠 skeleton 검수; 최종 텍스트/밸런스 결정 아님)
- **Blockers**: item/reward catalog 가 요구 필드(용도, SO 타입 후보, display key)를 충분히 제공하지 않음. Unity SO stableId 매핑 필요.
- **Next**: 외주에는 catalog v0.2 보강 요청, Unity 세션에는 stableId 기반 Items/Rewards/Ability/Enemy lookup 및 full pack dry-run/bake 지시
- **Agent**: Codex

---

### 2026-05-03 14:59 — rebase 완료 + encounter choice/effect runtime resolver 구현
- **Phase**: W1-2
- **Done**:
  - `Proto` rebase 완료: `.gitignore`/`.gitattributes` conflict marker 제거 및 local/origin union 반영
  - add/add 충돌은 로컬 작업 커밋 내용을 보존해 정리하고 `origin/Proto` 위로 8 commits 재적용
  - `EncounterData` 에 runtime choice/requirement/effect payload 추가 및 v0.2 baker 연결
  - `PrototypeEncounterRuntimeResolver` 추가: Stat/Flag/Item/Ability requirement, HP/mental/gold/glitch/affinity/flag/item/ability-ref effects 처리
  - `PrototypeRoomController` 가 formal non-battle encounter choice를 runtime resolver로 연결
- **Files**: 변경/추가 8개 (주요: EncounterData.cs, EncounterPipelineV02Baker.cs, PrototypeEncounterRuntimeResolver.cs, RuntimeShellTests.cs)
- **GDD impact**: 없음 (D-006/P4 정합; 새 디자인 결정 없음)
- **Blockers**: 실제 `AbilityData`/`ItemData` catalog ref 해석은 아직 placeholder ref tracking 수준
- **Next**: Items/Rewards SO 모델 추가 후 `AddItem`/`AddAbility` 를 catalog lookup 기반 실제 보상 지급으로 승격
- **Agent**: Codex

---

### 2026-05-03 14:49 — repo hygiene 상태 점검 및 rebase 충돌 확인
- **Phase**: W1-2
- **Done**:
  - `Proto` rebase 진행 상태 확인: `origin/Proto` onto rebase 중 1/8 커밋 적용 후 중단
  - 원래 `Proto` HEAD `717996a` 를 `backup-proto-before-rebase-20260503-717996a` 브랜치로 백업
  - 충돌 파일 `.gitattributes`, `.gitignore` 와 `.gitattributes` conflict marker 로 인한 attribute parse 오류 확인
  - untracked 파일을 ignore/검토/repo 외부 유지/테스트 산출물 후보로 분류
  - 삭제 및 `.gitignore` 수정은 수행하지 않음
- **Files**: 프로젝트 파일 변경 없음 (git backup branch 추가, PROGRESS 기록만 추가)
- **GDD impact**: 없음
- **Blockers**: rebase 충돌 미해결 상태라 추가 `git pull --rebase origin Proto` 는 실행하지 않음
- **Next**: `.gitattributes`/`.gitignore` 충돌 해결 후 `git rebase --continue`; 별도 승인 시 `.gitignore` 에 Obsidian/macOS/test-result 후보 반영
- **Agent**: Codex

---

### 2026-05-03 14:48 — Proto rebase sync 충돌로 runtime 작업 보류
- **Phase**: W1-2
- **Done**:
  - GDD §0/§0.6/§1/§2/§3 와 최근 PROGRESS 확인
  - 계획에 따라 `git pull --rebase origin Proto` 시도
  - 첫 rebase commit `3c9cd59` 적용 중 `.gitattributes`, `.gitignore` add/add 충돌 확인
  - 계획의 충돌 임의 해결 금지 조건에 따라 runtime resolver 구현 착수 전 중단
- **Files**: 프로젝트 코드 변경 없음 (rebase conflict state: `.gitattributes`, `.gitignore`)
- **GDD impact**: 없음
- **Blockers**: rebase conflict 해결 전략 필요. 현재 repo 는 rebase 진행 중 `HEAD (no branch)` 상태.
- **Next**: 사용자 승인 후 충돌을 union/정책 기반으로 해결하고 rebase continue, 또는 rebase abort 후 별도 sync 전략 결정
- **Agent**: Codex

---

### 2026-05-03 14:38 — encounter pipeline v0.2 validator + baker 기반 구현
- **Phase**: W1-2
- **Done**:
  - v0.2 DTO 구조와 외주 NpcStage enum -> runtime `NpcStage.S0..S5` mapping layer 추가
  - Editor-only JSON syntax/semantic validator 추가: schemaVersion, assetBake, assetPath, choice/time rules, escape/direct text 금지, MoralChoice/Shop custom rules
  - targetSchema/path 기반 validation case runner 추가 및 외주 validation cases 통과
  - minimal editor baker 추가: stableId 우선/updateExisting/no delete-recreate 로 sample pack 3개를 정식 `Assets/_Project/Data/Encounters` SO로 bake
  - EditMode 33/33, PlayMode smoke 1/1 통과
- **Files**: 변경/추가 22개 (주요: Assets/_Project/Scripts/Encounters/EncounterPipelineV02*, Assets/_Project/Tests/EditMode/EncounterPipelineV02Tests.cs, Assets/_Project/Data/Encounters/SO_Encounter_ENC_*.asset)
- **GDD impact**: 없음 (GDD locked 결정/NpcStage enum 변경 없음; v0.2 구현 기반)
- **Blockers**: 실제 런타임 choice/effect 실행 및 reward/item SO 모델은 미구현
- **Next**: Choice/Effect runtime resolver 와 Items/Rewards SO 모델 연결
- **Agent**: Codex

---

### 2026-05-03 13:54 — 외주 encounter schema v0.2 검수
- **Phase**: W1-2
- **Done**:
  - v0.2 schema/catalog/contract/sample/validation 파일 5개 확인
  - JSON 문법 검증 통과 및 `pack_PACK_SAMPLE_3_V002.json` 의 JSON Schema 검증 통과 확인
  - 도주 제거, 상태값 4종, 상점 샘플, GDD 정합 NpcStage, 정식 baked asset path 반영 확인
  - validation cases 의 `targetSchema` 일부 불일치와 custom semantic rule 필요성 확인
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (D-022 후보 구현 계약 검수; 아직 SSOT 잠금 전)
- **Blockers**: validation case 일부는 raw JSON Schema 단독 실행 불가. baker/validator 구현 시 target/path 기반 custom runner 필요.
- **Next**: Unity 개발 세션에 v0.2 schema 기반 DTO/validator/editor baker 구현 지시
- **Agent**: Codex

---

### 2026-05-03 13:30 — RunState economy/status hooks + shop minimum flow
- **Phase**: W1-2
- **Done**:
  - `PrototypeRunState` 에 mental/gold/glitchLevel/affinity 상태값과 clamp modify 메서드 추가
  - HUD snapshot/표시에 HP/ATK/gold/mental/glitchLevel/affinity/ability count 반영
  - Shop node 최소 흐름 구현: gold 부족 실패, gold 충분 시 10 차감 + placeholder ability 지급
  - 정식 데이터 landing zone `Assets/_Project/Data/Encounters`, `Items`, `Rewards` 추가; Prototype/Encounters 는 smoke fixture 로 유지
  - EditMode tests 4개 추가/갱신 및 PlayMode smoke 재검증
- **Files**: 변경/추가 14개 (주요: PrototypeRunState.cs, PrototypeRunSnapshot.cs, PrototypeHud.cs, RuntimeShellTests.cs, Assets/_Project/Data/{Encounters,Items,Rewards})
- **GDD impact**: 없음 (외주 schema v0.2 전 런타임 기반 구현; baker/validator/escapePolicy/NPC 최종 텍스트 미구현)
- **Blockers**: 외주 encounter schema v0.2 미수령. 실제 reward/item schema 확정 대기.
- **Next**: v0.2 schema 수령 후 정식 Encounters/Items/Rewards SO 모델과 baker/validator 설계 검토
- **Agent**: Codex

---

### 2026-05-03 13:16 — encounter schema v0.2 외주 재요청 명세 정리
- **Phase**: W1-2
- **Done**:
  - 도주 정책 제거 결정 반영: escapePolicy 비허용/Disabled 고정
  - mental/gold/glitchLevel/affinity 선택 결과 상태값 전부 구현 목표로 정리
  - 상점 구현 목표 반영: gold/item/ability/reward flow 필수화
  - baked asset 정식 경로 `Assets/_Project/Data/Encounters` 수용 기준 정리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (기존 GDD 방향 내 구현 명세 구체화; D-022 후보)
- **Next**: 외주 AI 에 v0.2 schema/catalog/contract/sample/validation 수정본 요청 후 Codex baker/validator 구현
- **Agent**: Codex

---

### 2026-05-03 13:13 — 외주 encounter schema 산출물 검토
- **Phase**: W1-2
- **Done**:
  - 외주 폴더의 schema/catalog/contract/sample/validation 파일 5개 내용 확인
  - JSON 문법과 sample pack 의 JSON Schema 검증 수행: sample pack schema 통과
  - GDD/Tone Bible 과 대조해 NpcStage 명칭/S5 누락, GDD 콘텐츠 수량 불일치, validation cases 자동화 구조 미흡 확인
  - 설계 AI 에 재요청할 수정 항목과 사용자 결정 필요 항목 정리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (산출물 검토; D-022 잠금 전 수정 필요)
- **Blockers**: NpcStage enum 정합, 25개 콘텐츠 분류, 메타 진행/영구 상태 범위 결정 필요
- **Next**: 외주 산출물 v0.2 재요청 후 baker/validator 구현 착수
- **Agent**: Codex

---

### 2026-05-03 12:43 — 텍스트 RPG 데이터 파이프라인 아키텍처 검토
- **Phase**: W1-2
- **Done**:
  - GDD §0/§0.6/§1/§2/§3 와 최근 PROGRESS 확인
  - 현재 Unity 프로젝트의 Encounter/Run/Combat placeholder 구조 점검
  - JSON schema → Editor bake → ScriptableObject runtime 파이프라인 적합성 검토
  - SerializeReference/SO/AssetDatabase/JsonUtility 리스크와 handoff payload 누락 항목 정리
- **Files**: 프로젝트 변경 없음 (PROGRESS 기록만 추가)
- **GDD impact**: 없음 (새 결정 없음; 제안 단계)
- **Blockers**: 실제 스키마/베이커 구현은 작가 승인 후 진행 필요
- **Next**: Encounter schema v0.1 과 bake validator/EditMode tests 구현 후 설계 AI 산출물을 소량 샘플로 검증
- **Agent**: Codex

---

### 2026-05-03 11:27 — AI NPC model intake + training spec 추가
- **Phase**: W1-2
- **Done**:
  - D-005 1순위/2순위 모델 배치 폴더 추가 (`hcx-seed-0.5b`, `qwen2.5-1.5b-instruct`)
  - 모델별 compiled android/ios, tokenizer, eval, training workspace 폴더와 Unity `.meta` 생성
  - MLC-LLM/SQLite third-party intake 와 Android/iOS plugin landing zone 추가
  - `Docs/AI_NPC_MODEL_SPEC.md` 작성: runtime contract, dataset JSONL schema, training scope, quality gate, conversion outline
  - `Tools/LLM/eval_prompts.jsonl` 로 최소 평가 prompt seed 추가
- **Files**: 변경/추가 42개 (주요: Assets/_Project/Models/**, Assets/ThirdParty/**, Assets/Plugins/**, Docs/AI_NPC_MODEL_SPEC.md)
- **GDD impact**: 없음 (D-005/D-006/§8/§11 정합 구현; 모델 선택 결정 변경 없음)
- **Blockers**: D-005 acceptance gate open. 실제 MLC/SQLite native plugin 미연동. Android Build Support 미설치.
- **Next**: 후보 모델 라이선스/획득 경로 확인 후 eval prompt 로 한국어 품질·지연 측정, 통과 모델만 MLC 변환
- **Agent**: Codex

---

### 2026-05-03 11:23 — asset intake folder structure 추가
- **Phase**: W1-2
- **Done**:
  - 외주/생성 에셋 수용용 `Art`, `Audio`, `Prefabs`, `Animations`, `Materials`, `VFX`, `UI` 폴더 추가
  - Art 하위 Characters/Environments/Nodes/UI/Icons/VFX, Audio 하위 Music/SFX/Ambience/Voice 폴더 추가
  - Prefabs 하위 Characters/Nodes/UI/Runtime 폴더 추가
  - 각 상위 폴더 `_README.md` 와 `Docs/ASSET_INTAKE.md` 로 import 위치/네이밍 규칙 문서화
  - Unity batchmode import 로 folder/file `.meta` 생성 확인
- **Files**: 변경/추가 40개 (주요: Assets/_Project/Art, Assets/_Project/Audio, Assets/_Project/Prefabs, Docs/ASSET_INTAKE.md)
- **GDD impact**: 없음 (GDD §11 repo 구조 정합 보강; 새 디자인 결정 없음)
- **Blockers**: 없음
- **Next**: 일러스트/사운드 파일 입수 시 해당 intake 폴더에 import 후 sprite/audio import settings 검증
- **Agent**: Codex

---

### 2026-05-03 11:16 — placeholder content pipeline + smoke tests
- **Phase**: W1-2
- **Done**:
  - `PrototypeSceneBuilder` 가 placeholder SO 49개(능력 12/시너지 4/적 6/보스 2/인카운터 25)와 MemoryFragments 폴더 생성/연결
  - `PrototypeRunState` persistent HP/ATK/ability/node snapshot 추가, HUD 상태 표시 확장
  - Battle/Rest/Shop/Remnant placeholder flow 를 scene data 에 연결
  - NPC trigger hook scaffold 추가 (`battle.victory`, `battle.defeat`, `rest.recall`, `run.completed`)
  - PlayMode smoke test 및 구현 준비 체크리스트 문서 추가
- **Files**: 변경/추가 127개 (주요: Assets/_Project/Data/Prototype/**, PrototypeSceneBuilder.cs, PrototypeRunState.cs, Docs/IMPLEMENTATION_READY_CHECKLIST.md)
- **GDD impact**: 없음 (D-009 수량에 맞춘 placeholder; 실제 수치/텍스트는 작가 결정 대기)
- **Blockers**: Android Build Support 미설치. MLC/SQLite native plugin 미연동.
- **Next**: Android Build Support 설치 후 APK build 검증, 작가 기획값을 placeholder SO에 치환
- **Agent**: Codex

---

### 2026-05-02 23:53 — hatch-pet Tower Seed base 생성
- **Phase**: W1-2
- **Done**:
  - `hatch-pet` skill 을 재실행해 `Tower Seed` pet run manifest 생성
  - base image prompt 를 생성하고 built-in image generation 으로 기준 sprite 생성
  - 생성 이미지를 `record_imagegen_result.py` 로 run manifest 에 기록해 canonical base/reference 생성
  - row-strip 8개 ready, `running-left` 는 `running-right` 이후 결정 대기 상태 확인
- **Files**: 프로젝트 변경 없음 (run: /private/tmp/hatch-pet-runs/tower-seed, generated image: /Users/godju/.codex/generated_images/019de928-6c23-7ec0-8d5d-23f5b274b810/ig_0acd04ef5560bd5c0169f60f9462088191a53e5a66e86ea93b.png)
- **GDD impact**: 없음
- **Blockers**: row-strip 생성은 hatch-pet 지침상 subagent 필수이나, 현재 상위 규칙상 사용자 명시 요청 전 subagent spawn 불가
- **Next**: 사용자가 subagent 사용을 명시 승인하면 idle/running-right row 부터 생성 후 package finalization 진행
- **Agent**: Codex

---

### 2026-05-02 23:48 — prototype run flow 연결
- **Phase**: W1-2
- **Done**:
  - `PrototypeRunState` 추가: run state, ability inventory, memory repo, cached on-device/fake LLM provider 소유
  - Battle node 선택 시 deterministic combat round 자동 처리 + CombatStarted/Completed, EncounterCompleted 이벤트 발행
  - Rest node recall 로드, Remnant node run completion + reflection 저장 연결
  - Ability/Synergy numeric hook 추가 (`player.max_hp_bonus`, `player.attack_bonus`)
  - MLC native bridge / on-device provider scaffold 추가, native plugin 없으면 fake provider fallback
  - Android build harness 가 Android target 미설치 시 조기 실패하도록 보강
- **Files**: 변경/추가 20개 (주요: Assets/_Project/Scripts/Run/PrototypeRunState.cs, LLM/MLCBridge.cs, Abilities/CombatAbilityModifiers.cs)
- **GDD impact**: 없음 (P4 결정성, D-005/D-006 scaffold 정합; 새 수치/서사 결정 없음)
- **Blockers**: Android Build Support 미설치로 APK 생성 실패. MLC/SQLite 실제 native plugin 미연동.
- **Next**: 작가 결정 전에는 placeholder enemy/ability SO 자동 생성과 HUD 세분화, 이후 능력 12/시너지 4/적 6/보스 2 데이터 입력
- **Agent**: Codex

---

### 2026-05-02 23:48 — hatch-pet Codex skill 설치
- **Phase**: W1-2
- **Done**:
  - AGENTS 규약에 따라 GDD §0/§0.6/§1/§2 와 최근 PROGRESS 확인
  - openai/skills curated `hatch-pet` skill 을 로컬 Codex skills 경로에 설치
  - 설치된 `hatch-pet/SKILL.md` 를 확인해 실제 펫 생성 workflow 와 subagent 요구사항 파악
- **Files**: 프로젝트 변경 없음 (외부 Codex skill: /Users/godju/.codex/skills/hatch-pet)
- **GDD impact**: 없음
- **Blockers**: 실제 펫 row 생성은 사용자의 pet 컨셉/이름 및 subagent 명시 승인 필요
- **Next**: Codex 재시작 후 hatch-pet skill 활성화, 만들 pet 컨셉을 정해 생성 진행
- **Agent**: Codex

---

### 2026-05-02 23:36 — Unity root baseline commit + consistency settings
- **Phase**: W1-2
- **Done**:
  - 루트 승격 변경과 runtime shell 구현을 `Normalize Unity root and runtime shell` 커밋(c165722)으로 고정
  - `.editorconfig` 추가: C#/Unity YAML/JSON indentation, LF, final newline 규칙 고정
  - README 갱신: Unity 프로젝트 루트, 표준 EditMode/Android build 명령, 추적/ignore 규칙 문서화
  - `.gitignore` 에 Unity test/build 로그 및 결과 파일 ignore 추가
  - EditMode tests 재실행: 14/14 pass
- **Files**: 변경/추가 221개 커밋 (주요: .editorconfig, README.md, Assets/_Project/**, Packages/**, ProjectSettings/**)
- **GDD impact**: 없음 (D-016 정합 유지; 새 디자인 결정 없음)
- **Blockers**: 없음
- **Next**: Combat node 를 실제 HP/HUD/승패 흐름에 연결
- **Agent**: Codex

---

### 2026-05-02 23:31 — W1-2 runtime shell + repo root 정상화
- **Phase**: W1-2
- **Done**:
  - D-016 에 맞게 Unity 프로젝트 파일을 repo 루트로 승격하고 nested gitlink 제거
  - `GameFlowEventBus` 를 PrototypeRoom / NodeInteraction 흐름에 연결
  - seed-key 기반 `EncounterSelector`, 전투 stub, Ability/Synergy runtime shell 추가
  - SQLite schema scaffold, deterministic fake/cached LLM provider, reflection/recall pipeline 추가
  - Android build harness (`BuildScript.BuildAndroid`) 추가 및 PrototypeRoom scene 재생성
- **Files**: 변경/추가 217개 (주요: Assets/_Project/Scripts/**, Assets/_Project/Tests/EditMode/RuntimeShellTests.cs, Packages/ProjectSettings 루트 승격)
- **GDD impact**: 없음 (D-016 정합화, P4 결정성 기반 구현; 새 밸런스/서사 결정 없음)
- **Blockers**: SQLite 실제 native/plugin 연동과 on-device MLC bridge 는 아직 scaffold 단계
- **Next**: Combat stub 를 실제 노드 보상/HP HUD 에 연결하고, 작가 결정 후 능력 12/시너지 4 SO 채우기
- **Agent**: Codex

---

### 2026-05-02 — D-020/D-021 잠금 + 재개 준비 (Claude)
- **Phase**: W1-2 진입 직전
- **Done**:
  - 04-30 ~ 05-02 3일 공백 사유 기록: 조사 (작가 가족사)
  - GDD v0.2.2: D-020 (외주 = 동생) + D-021 (스코프 유지, AI 처리량 기반 일정) 잠금
  - OQ-010 등록 (외주 작업 정의, 05-09 마감)
  - W1-2 슬립 인정 + W2-1 흡수 정책
- **Files**: hwiglija-tower-gdd.md, wiki/log.md, 본 파일
- **GDD impact**: D-020 / D-021 / OQ-010 / version 0.2.0 → 0.2.2
- **Next**:
  - 작가 → Gemini 토픽 2 (능력 12 효과·수치) 디벨롭 — Codex W1-2 진입 전제
  - Codex 재개 시 04-29 20:12 의 Next 흡수 (GameFlowEventBus → PrototypeRoom 연결 + Encounter stub) 후 W1-2 진입
- **Agent**: Claude (Sub brain)

---

### 2026-04-29 20:12 — 코어 아키텍처 누락분 보강
- **Phase**: W1-1
- **Done**:
  - 기존 SO 데이터 모델, LLM/memory/dialogue 인터페이스, 기본 seeded RNG 구현 상태 확인
  - `DeterministicSeed` / seed-key fork / weighted index helper 추가
  - NPC S0-S5 FSM runtime (`NpcStateMachine`) 과 SO definition / transition rule 추가
  - 게임 흐름 이벤트 primitives (`GameFlowEvent`, `GameFlowEventBus`, `GameFlowEventType`) 추가
  - RNG/FSM/event ordering EditMode tests 추가
- **Files**: 변경/추가 17개 (주요: `Assets/_Project/Scripts/Core/**`, `Assets/_Project/Scripts/NPC/NpcStateMachine*.cs`, `Assets/_Project/Tests/EditMode/CoreArchitectureTests.cs`)
- **GDD impact**: 없음 (P4 결정성 정합; 새 밸런스/서사 결정 없음)
- **Blockers**: 없음. Unity-Skills compile check 통과, EditMode tests 8/8 pass.
- **Next**: `GameFlowEventBus` 를 prototype room/node interaction 흐름에 연결하고 Encounter 선택 stub 를 seed-key 기반으로 구현
- **Agent**: Codex

---

### 2026-04-29 17:19 — Unity-Skills UPM 설치 확인
- **Phase**: W1-1
- **Done**:
  - Unity 프로젝트 `Packages/manifest.json` 에 `com.besty.unity-skills` 추가 확인
  - `Packages/packages-lock.json` 에 Git package lock 반영 확인
  - localhost `8090-8100` 확인 결과 Unity-Skills REST server 는 아직 미실행 상태 확인
  - Unity 6000.4 API 변경으로 인한 `PlayerSettings.defaultScreenOrientation` compile error 를 `defaultInterfaceOrientation` 로 수정
- **Files**: 변경/추가 3개 (주요: `Packages/manifest.json`, `Packages/packages-lock.json`, `Assets/_Project/Editor/PrototypeSceneBuilder.cs`)
- **GDD impact**: 없음
- **Blockers**: Unity-Skills server 는 Editor 메뉴에서 `Window > UnitySkills > Start Server` 실행 필요
- **Next**: Unity Editor recompile 완료 후 Start Server 실행, Codex 에서 `/health` 재확인
- **Agent**: Codex

---

### 2026-04-29 16:53 — Input System asmdef 참조 오류 수정
- **Phase**: W1-1
- **Done**:
  - `UnityEngine.InputSystem` compile error 원인 확인
  - `com.unity.inputsystem` 패키지는 설치되어 있으나 `_Project` runtime asmdef 참조가 비어 있음을 확인
  - `HwigiTower.Runtime.asmdef` 에 `Unity.InputSystem` 참조 추가
- **Files**: 변경/추가 1개 (`Assets/_Project/Scripts/HwigiTower.Runtime.asmdef`)
- **GDD impact**: 없음
- **Blockers**: Unity batchmode compile 검증은 동일 프로젝트가 Editor 에서 열려 있어 실행 불가
- **Next**: 열린 Unity Editor 에서 자동 recompile 확인 후 남은 compile error 점검
- **Agent**: Codex

---

### 2026-04-29 16:45 — Unity Licensing Client 실행 실패 진단
- **Phase**: W1-1
- **Done**:
  - macOS 디스크/메모리 상태 확인
  - Unity Editor / Licensing Client 로그 확인
  - 실행 중인 Unity Hub / Unity.Licensing.Client 프로세스 확인
  - 직접 원인이 `LocalIPC 1.17.4` Hub licensing client 와 `LocalIPC 1.18.1` Unity 6000.4.3f1 Editor 의 protocol mismatch 임을 확인
- **Files**: 프로젝트 변경/추가 0개 (진단만 수행)
- **GDD impact**: 없음
- **Blockers**: Unity 실행 불가. stale/mismatched licensing client 재시작 또는 Hub/Editor 버전 정렬 필요.
- **Next**: Unity Hub/Editor 완전 종료 후 `Unity.Licensing.Client` 프로세스 정리, Hub 재시작 및 라이선스 재인증
- **Agent**: Codex

---

### 2026-04-29 16:08 — Codex Unity-Skills 로컬 설치
- **Phase**: W1-1
- **Done**:
  - `Besty0728/Unity-Skills` GitHub repo 구조 확인
  - `SkillsForUnity/unity-skills~` 템플릿을 `~/.codex/skills/unity-skills` 로 설치
  - `agent_config.json` 에 Codex agent id 설정
  - `unity_skills.py` helper import 검증 (`1.8.0`)
- **Files**: 프로젝트 변경/추가 0개 (로컬 Codex skill 설정만 변경)
- **GDD impact**: 없음
- **Blockers**: Unity 프로젝트에는 아직 UPM package 미추가. AGENTS 기준 외부 의존성 추가라 실제 Unity 패키지 적용은 별도 확인 필요.
- **Next**: Codex 재시작 후 `unity-skills` 인식 확인, 이후 Unity Editor 에서 `Window > UnitySkills > Start Server` 사용
- **Agent**: Codex

---

### 2026-04-29 13:05 — Codex workspace branch Proto 정렬
- **Phase**: W1-1
- **Done**:
  - 바깥 Codex workspace repo 가 `main` 에 머물러 있던 원인 확인
  - `/Users/godju/Downloads/AI Game/hwigi-tower` 로컬 브랜치를 `Proto` 로 전환
  - 바깥 repo origin 을 GitHub `hwigija-tower` 로 설정
  - 안쪽 Unity repo `/hwigija-tower` 가 기존처럼 `Proto...origin/Proto` 인 상태 확인
- **Files**: 변경/추가 0개 (git branch/remote 설정만 변경)
- **GDD impact**: 없음
- **Blockers**: repo 가 바깥/안쪽 2중 git 구조라 장기적으로 root 정리 필요
- **Next**: 커밋 전 실제 작업 위치를 안쪽 Unity repo 로 고정하거나 repo topology 정리
- **Agent**: Codex

---

### 2026-04-29 12:50 — NPC memory interfaces + deterministic cache tests
- **Phase**: W1-1
- **Done**:
  - `INPCMemoryRepo`, `ILLMProvider`, `IDialogueRouter` 인터페이스 추가
  - `RunReflection`, `DeterministicCacheKey`, `LLMRequest`, `LLMResponse`, `DialogueRequest/Response` 모델 추가
  - `PromptHash` SHA-256 helper 추가: cache key = run_id + prompt_hash
  - `InMemoryNpcMemoryRepo` prototype stub 추가 (reflection + LLM response cache)
  - EditMode deterministic cache tests 4개 추가
- **Files**: 변경/추가 31개 (주요: Assets/_Project/Scripts/LLM/**, NPC/*Memory*.cs, Assets/_Project/Tests/EditMode/**)
- **GDD impact**: 없음 (D-006/P4 정합; real on-device LLM 미통합)
- **Blockers**: Unity EditMode test batchmode 는 프로젝트가 다른 Unity Editor 에서 열려 있어 실행 실패. scoped `git diff --check`, random/time/API 검색 통과.
- **Next**: Unity Editor 종료 후 EditMode tests 실행 또는 SQLite 영속 repo 구현으로 교체
- **Agent**: Codex

---

### 2026-04-29 12:42 — ScriptableObject 데이터 모델 추가
- **Phase**: W1-1
- **Done**:
  - AbilityData / SynergyData SO 모델 추가 (id/tag/description/effect/numeric params)
  - EnemyData SO 모델 추가 (id/hp/attack/pattern id)
  - EncounterData + EncounterType SO 모델 추가 (id/type/floor/weight/deterministic seed key)
  - NpcStateData + NpcStage(S0-S5) + NpcDisplayRule 모델 추가
  - NumericParam 공용 serializable 모델 추가; 최종 수치·서사 텍스트 미작성
- **Files**: 변경/추가 21개 (주요: Assets/_Project/Scripts/Abilities/**, Combat/**, Encounters/EncounterData.cs, NPC/**, Core/NumericParam.cs)
- **GDD impact**: 없음 (D-004 데이터 드리븐 / D-006 결정성 정합; 새 디자인 결정 없음)
- **Blockers**: Unity compile 미실행. 모델 파일 scoped `git diff --check` 및 random/time API 검색 통과.
- **Next**: Unity Editor 에서 컴파일 확인 후 placeholder SO asset 생성 또는 SQLite `INPCMemoryRepo` scaffold 진행
- **Agent**: Codex

---

### 2026-04-29 12:31 — 최소 세로 2D 프로토타입 스캐폴딩
- **Phase**: W1-1
- **Done**:
  - Unity repo 를 `Proto` 브랜치로 전환하고 origin 을 공유 GitHub repo 로 정렬
  - `Assets/_Project` 구조 아래 결정성 run context, keyboard/touch-compatible movement abstraction, player controller 추가
  - Battle/Rest/Shop/Remnant 노드 타입과 ScriptableObject stub, prototype room/runtime settings SO 추가
  - `PrototypeRoom` 테스트 씬 추가: Play 시 portrait room, player, wall, node placeholders, HUD 를 data-driven 으로 생성
  - BuildSettings 를 PrototypeRoom 으로 변경하고 portrait 테스트 해상도/autorotation 제한 적용
- **Files**: 변경/추가 69개 (주요: Assets/_Project/**, ProjectSettings/EditorBuildSettings.asset, ProjectSettings/ProjectSettings.asset)
- **GDD impact**: 없음 (잠긴 결정 변경 없음; D-003/D-004/D-006/D-016 정합 구현)
- **Blockers**: Unity batchmode 검증은 현재 프로젝트가 다른 Unity Editor 에서 열려 있어 실패. 소스/설정 정적 검증만 수행.
- **Next**: Unity Editor 에서 `Assets/_Project/Scenes/PrototypeRoom.unity` 열고 Play 테스트 후, SQLite `INPCMemoryRepo` scaffold 로 진행
- **Agent**: Codex

---

### 2026-04-29 12:05 — Unity Git hygiene 추가 + 프로젝트 점검
- **Phase**: W1-1
- **Done**:
  - AGENTS.md 와 GDD §0/§0.6/§1/§2/§3, PROGRESS 최신 항목 확인
  - Unity 프로젝트 구조 점검: Unity 6000.4.3f1, URP 2D, Input System, SampleScene 상태 확인
  - repo 루트에 Unity .gitignore 추가
  - repo 루트에 Git LFS 포함 .gitattributes 추가
  - D-016 과 실제 nested Unity repo 구조 불일치 확인
- **Files**: 변경/추가 3개 (.gitignore, .gitattributes, hwiglija-tower-progress.md)
- **GDD impact**: 없음
- **Blockers**: Unity 프로젝트가 repo 루트가 아니라 hwigija-tower/ 하위에 있고, 해당 폴더가 별도 git repo 임
- **Next**: D-016 에 맞게 repo topology 정리 여부 확인 후 W1-1 코어 조작 + SQLite scaffold 착수
- **Agent**: Codex

---

### 2026-04-28 — OpenAI Cookbook 카탈로그 → agenda 토픽 9-11
- **Phase**: W1-1
- **Done**:
  - codex-cli-prompting §14: Cookbook 13 카테고리 leverage 매핑 (🔥 즉시 3 / 🟡 후순위 4 / ⚪ 참고 2 / ❌ 미적용 3)
  - design-agenda 토픽 9 (Reflection JSON 스키마, Structured Outputs) / 10 (Eval 설계) / 11 (Fallback Prompt Caching) 신설
  - design-brief §8: 외부 LLM 인용 가능한 자료원 명시
- **Files**: codex-cli-prompting.md, hwiglija-tower-{design-agenda, design-brief, progress}.md, log.md
- **GDD impact**: 없음 (워크플로우 + 학습 archive)
- **Next**: Codex 설치·Unity 프로젝트 생성. 토픽 10/11 은 W1-2 OQ-004 결정점 직전 (~05-04) 다룬다.
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — Codex Prompting Guide 학습 + AGENTS.md v0.4
- **Phase**: W1-1 (Codex 도입 직전)
- **Done**:
  - methods/codex-cli-prompting.md 신규 (v1.0, 13 섹션 압축, 재사용 체크리스트 포함)
  - AGENTS.md v0.4: §11 도구·병렬·Bias to Action·Preamble cadence + §12 Pragmatic + §4 generic 제거
  - repo AGENTS.md 동기화 (246 줄)
- **Files**: codex-cli-prompting.md, hwiglija-tower-AGENTS.md, log.md, index.md, /AI Game/hwigi-tower/AGENTS.md
- **GDD impact**: 없음 (AGENTS.md 위성 변경)
- **Next**: Codex CLI 설치 → Unity 프로젝트 생성 → 첫 호출 (.gitignore + .gitattributes)
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — 외부 LLM 디벨롭 워크플로우 (BRIEF + AGENDA)
- **Phase**: W1-1
- **Done**:
  - design-brief.md 신규 — 외부 LLM 핸드오프 컨텍스트 (Pitch / Pillars / D-001~019 / Tone / OQ / 일정 / 답변 포맷)
  - design-agenda.md 신규 — 8 토픽 우선순위 큐 + 토픽 1-5 상세 프롬프트 + 작가→Claude 확정 공유 포맷
  - 워크플로우: 작가가 brief+토픽 프롬프트 → Gemini → "확정: ..." → Claude 가 SSOT 반영
- **Files**: hwiglija-tower-{design-brief, design-agenda, progress}.md, log.md, index.md
- **GDD impact**: 없음 (워크플로우 문서, GDD 본문 미변경)
- **Next**: 작가가 토픽 1 (NPC 외형) 또는 토픽 2 (능력·시너지) 부터 디벨롭 시작 가능
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — 기획안 디벨롭 인프라 (PILLARS + TONE + JOURNAL)
- **Phase**: W1-1
- **Done**:
  - GDD §0.6 DESIGN PILLARS 잠금 (P1-P5 + Anti-Pillar 3) → v0.2.0 MINOR bump
  - tone-bible.md 신규 (4 음역 / S0-S5 샘플 7개 / 금지선 7개)
  - design-journal.md 신규 (결정의 *왜*·대안·기각 archive — D-019, Pillar 추출 사고 2건)
  - AGENTS.md v0.3 (§2.1 read 단계에 PILLARS·TONE 추가, §2.2 P1-P5 정합성 점검 의무화) + repo 동기화
- **Files**: hwiglija-tower-{gdd, tone-bible, design-journal, AGENTS, progress}.md, log.md, index.md, /AI Game/hwigi-tower/AGENTS.md
- **GDD impact**: §0.6 PILLARS / §14 연결 / v0.2.0
- **Next**: 작가 작업물(Codex 설치 + Unity Hub 프로젝트 생성). 기획 디벨롭 시 design-journal 활용.
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — 코어 내러티브 잠금 (D-019)
- **Phase**: W1-1
- **Done**:
  - D-019 잠금: 잃어버린 것 = 플레이어와의 유대 + 본인의 기억 / 종착 = 재망각의 루프
  - OQ-005 close
  - GDD §4.1 pitch, §5.1 NPC 정체성, §6.1 메타-읽기 (회복→붕괴→망각) 갱신 → v0.1.4
- **Files**: hwiglija-tower-gdd.md, log.md, hwiglija-tower-progress.md
- **GDD impact**: D-019 추가 / OQ-005 close
- **Next**: 작가 작업물(Codex 설치 + Unity Hub 프로젝트 생성) 대기 / OQ-006 메모리 파편 5개는 본 D-019 기반으로 W2-2 에 작성
- **Agent**: Claude (Sub brain)

---

### 2026-04-28 — OQ 4건 일괄 close (작가 결정)
- **Phase**: W1-1
- **Done**:
  - D-017 잠금: NPC 이름 = 마타이오스 (Mataios, μάταιος "무가치함")
  - D-018 잠금: Android APK 1순위, iOS = 영상 데모만 (Apple Dev 미가입)
  - OQ-001 / 007 / 008 / 009 close (07-08 은 외주에 작가 구두 전달)
  - GDD §1 CHANGELOG, §2 DECISIONS, §3 OPEN QUESTIONS, §5.1 NPC, §9 일정, §12 리스크 갱신 → v0.1.3
- **Files**: hwiglija-tower-gdd.md, log.md, hwiglija-tower-progress.md
- **GDD impact**: D-017, D-018 추가 / OQ-001, 007, 008, 009 close
- **Next**: 작가가 Codex CLI 설치 + Unity Hub 로 hwigi-tower 프로젝트 생성 → 첫 Codex 호출 (.gitignore + .gitattributes)
- **Agent**: Claude (Sub brain)

### 2026-04-27 — 프로젝트 부트스트랩
- **Phase**: W1-1 (D-21 시작)
- **Done**:
  - GDD v0.1 / v0.1.1 잠금 ([[hwiglija-tower-gdd]])
  - AGENTS.md 위성 문서 작성 ([[hwiglija-tower-AGENTS]])
  - 진행상황 추적 파일(본 파일) 생성
  - Codex 작업 환경 부트스트랩 절차 정의
- **Files**: wiki/projects/hwiglija-tower-{gdd, AGENTS, progress}.md
- **GDD impact**: D-001~D-015 잠금, OQ-001~OQ-009 등록
- **Blockers**: Codex CLI 미설치, Unity repo 미생성, OQ-007/008/009 작가 결정 대기 (마감 04-30)
- **Next**:
  1. 작가: Codex CLI 설치 (`npm i -g @openai/codex` 또는 공식 가이드)
  2. 작가: Unity 프로젝트 생성 위치 결정 후 repo 부트스트랩
  3. 작가: OQ-007/008 (외주 브리프 톤 레퍼런스) 결정
- **Agent**: Claude (Sub brain)

---

<!-- 이후 신규 항목은 본 줄 위에 prepend (최신이 위로). -->
