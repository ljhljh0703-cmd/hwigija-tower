---
session: "2026-09-01 hwigi-runtime-r2"
agent: "codex"
entry_point: "Docs/Portfolio/assets/concept-to-ui/DISPATCH-runtime-r2.md"
based_on:
  - "D-012"
  - "D-029"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: flagged
skill_candidate: false
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 런타임 레인 경화 (휴식 R2)

## 0단 · R1 고정

- R1 commit: `70e46fcf7e81929437e9e1cca4a7a93058b7adfb` (`Implement rest UI runtime static R1`)
- R1 결과 XML·`.codegraph`는 커밋에 넣지 않았다. R2 코드/증거는 이 RETURN과 함께 별도 커밋한다.

## Task C · 기준선 실측

별도 detached worktree `/private/tmp/hwigi-runtime-baseline-e9d5454`에서 `e9d5454`를 같은 전량 명령으로 실행했고, XML을 현재 브랜치에 복사했다.

| 레인 | e9d5454 | R1 | 기존 실패 | 새 실패 | 고쳐진 것 |
|---|---:|---:|---:|---:|---:|
| EditMode | 181/211, fail 30 | 181/211, fail 30 | 30 | 0 | 0 |
| PlayMode | 12/21, fail 8, skip 1 | 12/21, fail 8, skip 1 | 8 | 0 | 0 |

- `baseline-e9d5454-EditMode.xml` SHA-256: `7f792921b68118dc0c8ba5e1e74abfec06025d1ba9ee728b7c135f914dee8ca6`
- `baseline-e9d5454-PlayMode.xml` SHA-256: `4a1eaff6a6de5d2870a81ebcbb4764631bf056308f98f0035f8ff5a935bb12a0`

따라서 R1의 30/8 실패는 추정이 아니라 기준선에도 있던 실패 집합이다.

## Task A · ML-Agents 로그 승격 차단

- 추가: `Assets/_Project/Tests/EditMode/EditModeCatalogRefreshLogGuard.cs`
- 전역 `LogAssert.ignoreFailingMessages`는 사용하지 않았다.
- 코드에 남긴 유일한 pattern: `Asset Packages/com.unity.ml-agents/Samples(?:/3DBall(?:/3DBall.unitypackage)?)? has no meta file, but it's in an immutable folder. The asset will be ignored.`
- `Samples`, `Samples/3DBall`, `Samples/3DBall/3DBall.unitypackage` 중 실제로 `.meta`가 없는 항목 수만큼만 `LogAssert.Expect(LogType.Error, pattern)`을 등록한다.
- `EncounterRuntimeCatalogBuilder.BuildDefaultCatalog` 및 apply bake 호출 직전의 EditMode test 경계에만 적용했다. 패키지는 유지했다.

| 실행 | R1 | R2 |
|---|---:|---:|
| EditMode | 181/211, fail 30 | **203/211, fail 8** |
| ML-Agents meta log 실패 | 22 | **0** |

잔여 8건은 AudioLobby NullReference 및 실제 맵·전투 기대값 실패다. 숨기거나 수정하지 않았다.

## Task B · 테스트 격리 수리

### 가설 판정

「런/저장 상태가 테스트 사이로 샌다」는 가설은 **틀렸다**. `PrototypeRunSaveStore.HasSave()`는 false인데 Continue 버튼은 true였다.

근본 원인은 `LobbyController`가 이전에 비활성화한 legacy Canvas까지 `transform` 전체에서 검색해, 보이는 Continue 버튼이 아니라 **폐기된 동일 이름 버튼**을 바인딩·갱신한 것이었다.

### 변경

- `LobbyController`는 활성 `Lobby Canvas` root 안에서만 runtime UI 존재 여부·버튼·패널을 찾는다.
- partial UI를 재구성할 때 직접 자식 `Lobby Canvas` 전부를 legacy로 retire한다.
- no-save fixture는 target Lobby bootstrap 완료 후 저장을 삭제하고 `ShowContinuePlaceholder()`로 보이는 UI를 재계산한다.
- 원래 `Assert.IsFalse(continueButton.interactable)`는 유지했다. 활성 Continue 버튼은 하나이고 Controller가 바인딩한 인스턴스와 같은지도 추가 검증했다.

| 실행 | R1 | R2 |
|---|---:|---:|
| PlayMode | 12/21, fail 8 | **14/21, fail 6** |
| `Lobby_LoadsV11LayoutAndNoSaveVariant` | fail | **pass** |
| `Lobby_SettingsPanelOpensAndClosesFromTheUtilityRow` | fail | **pass** |

잔여 PlayMode 6건은 `PrototypeRoomSmokeTests`의 맵 노드/선택지 진행 실패이며 기준선에도 있던 집합이다. 수정하지 않았다.

## Task D · runtime actual 대조

정적 덤프의 최상위 `source`는 `contract`, screenshot QA writer의 최상위 `source`는 `runtime`으로 고정했다. runtime writer는 활성 캡처 root의 자식만 우선 선택한다.

| 화면 | runtime actual | 정적 contract 대조 | 판정 |
|---|---|---:|---|
| 로비 | 8 slots, `source: runtime` | 12/12 | **PASS** |
| 전투 | 15/22 slots, `source: runtime` | 2 PASS · 25 FAIL | **발견 보존** |
| 휴식 | 14 slots, `source: runtime` | 20/20 | **PASS** |

### 전투 슬롯 차이 (runtime − spec)

`floorChip`, `sanityChip`, `hpChip`, `goldChip`, `threatReadout`, `playerSanityBar`, `mataiosStateLabel`은 runtime 보고에 없다. 아래는 보고된 슬롯의 `(Δx, Δy, Δw, Δh)`다.

| 슬롯 | runtime visible | Δx | Δy | Δw | Δh |
|---|---:|---:|---:|---:|---:|
| enemyPanel | yes | +.032 | −.068 | −.064 | +.036 |
| enemyTitle | yes | +.044 | −.066 | −.088 | +.019 |
| enemyHpBar | yes | +.004 | −.045 | −.008 | −.002 |
| enemyStatusChips | yes | −.068 | −.049 | +.136 | −.008 |
| enemyImage | yes | −.064 | −.085 | +.129 | +.049 |
| damageEffectOverlay | no | −.062 | −.032 | +.124 | −.035 |
| playerCard | yes | +.073 | +.125 | −.064 | −.018 |
| playerHpBar | yes | +.067 | +.133 | −.052 | −.007 |
| mataiosCard | yes | −.009 | +.125 | −.064 | −.018 |
| mataiosHpBar | yes | −.015 | +.133 | −.052 | −.007 |
| attackButton | yes | +.159 | +.110 | −.161 | −.049 |
| defendButton | yes | +.080 | +.110 | −.161 | −.049 |
| skillButton | no | +.002 | +.110 | −.161 | −.049 |
| combatLog | yes | +.035 | −.357 | +.110 | +.082 |
| itemInspectButton | yes | +.008 | −.064 | −.081 | −.026 |

로비와 휴식은 모든 보고 슬롯이 허용 오차 안에서 일치했다. 처음 기록된 lobby 자동 반려는 inactive legacy 노드를 먼저 고른 writer 결함으로 밝혀졌고, 삭제하지 않고 `_rejections.md`에 정정을 append했다.

### 명시적 QA 증거

- 결과: `TestResults-PortraitUiScreenshotQa-R2.xml` = **1/1 pass**
- 캡처: `/private/tmp/hwigi-portrait-ui-v3-screenshots/01_lobby_no_save.png` (`f37520c7…bc45a`)
- 캡처: `/private/tmp/hwigi-portrait-ui-v3-screenshots/04_rest_mataios.png` (`c1fd8be7…4b6eb`)
- 캡처: `/private/tmp/hwigi-portrait-ui-v3-screenshots/06_normal_combat.png` (`e0260f85…07dd7`)
- runtime JSON: `lobby_actual_layout.json` (`cefdb573…c0208`), `combat_actual_layout.json` (`0313a024…8023e`), `rest_actual_layout.json` (`7d3d569c…6565f`)

## C-08

**SKIP 유지.** `strained`/`broken` variants는 아트 4종 미입고와 `thresholds.*: undecided` 때문에 blocked다. 기본 상태 대비를 대체 측정으로 사용하지 않았다.

## 남은 실패 · 미수정

- EditMode 8: `RoomController_ExplorationContextResetsToNormalBgm`, final boss/route, Floor 1 jar, generic counterplay, item/ability combat loop 계열 실제 단언 실패.
- PlayMode 6: `PrototypeRoomSmokeTests`의 boss gate, jar event, final boss rest ending, rest node, route action, shop route 진행 실패.
- EditMode catalog bake는 실제 data asset을 갱신하므로, 이 검증 중 발생한 범위 밖 asset/settings 변경은 로컬에서 원복했다. 별도의 bake-test asset isolation은 이번 범위 밖으로 남긴다.

## 상태

- ③ Gate: **flagged** — 로비·휴식 runtime 대조는 통과했으나 전투 runtime contract 불일치가 남아 있다.
- `QUEUE.md` 런타임 레인을 실측으로 갱신했고, 4번 층 지도는 미변경이다.
- 휴식 수치·붕괴도 delta·버프·저장 스키마·variant 임계값은 변경하지 않았다.
- Skill candidate: 없음. Sub-brain은 read-only이며 Progress/Memory append 없음.
