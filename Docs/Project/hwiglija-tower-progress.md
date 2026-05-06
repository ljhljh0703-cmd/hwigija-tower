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
