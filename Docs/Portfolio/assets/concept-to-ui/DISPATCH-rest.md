# DISPATCH — 휴식 화면 재구성 (큐 3번)

> 스킬 `game-ui-from-concept` 3단. 요약하지 말고 그대로 넘긴다.
> **선행**: 로비·전투가 정적 층을 통과했다. 하네스 규칙은 `CODEX-DISPATCH-hwigi-lobby-ui-2026-08-31.md` §13 과 동일하다.

## 대상

- 화면: 휴식 (`rest`)
- 사양: `Docs/Portfolio/assets/concept-to-ui/rest_layout_spec.json` **v1.1**
- 토큰: `Docs/Portfolio/assets/concept-to-ui/ui_tokens.json`
- 브랜치: `codex/hwigi-lobby-ui-pilot-r3`
- 기준 커밋: `e9d5454` (착수 전 `git rev-parse HEAD` 로 재확인)
- 채점기: `Tools/check_layout.py` — **13종 판. `noDuplicateContent`·`minArea`·`textAbsent` 가 있어야 맞는 판이다.** 없으면 구판이니 멈추고 보고

## 해야 할 일

### A. 휴식 화면 (주 작업)

1. `PrototypeHud.cs` 의 휴식 부분을 **독립 모듈로 분리**한다. 로비(`LobbyController.cs`)·전투와 같은 형태.
2. `RestLayoutContract.cs` 를 만든다. **`UnityEngine` 비의존 순수 C#** — 로비·전투 계약과 같은 규격.
3. `Tools/DumpLayout` 이 `rest_actual_layout.json` 을 뱉게 배선한다.
4. 사양 `bands` 비율로 재배치한다. 선택지 3개가 상단에서 `action` 층으로 내려오고, 모닥불 씬이 `subject` 층 주인공이 된다.
5. `restScene` 을 화면의 30% 이상으로 올린다 (R-03). 배경 아트는 리포에 이미 있다.
6. 상단 텍스트 한 줄을 **자원 칩 4개**로 분해한다. 전투와 동일 규격.
7. **라벨 이중 표기를 없앤다** (R-04). 카드 안에만 한 번 적는다. 화면 제목은 「휴식」이 아니라 **「모닥불」**이다 — 아래 선택지에 「휴식」이 이미 있어 중복이었다.
8. `departButton` 을 신설한다. 선택 후 어디로 가는지가 안 보였다.
9. `mataiosStateLabel` — **비수치 authored 문구**다. 게이지·숫자·「붕괴도」 금지 (R-09).

### B. 전투 회귀 1건 (같은 라운드에서 처리)

10. `CombatLayoutContract.cs` 의 `mataiosSanityBar` → **`mataiosStateLabel`** 로 rename 하고 `type: text` 로 바꾼다. `DumpLayout` 을 다시 돌려 `combat_actual_layout.json` 을 갱신한다.
    - 사유: 마타이오스 전용 「이성」 값이 코드에 없다. 사양 v2.1 이 그걸 바로잡았고 대조가 26/27 로 떨어져 있다. **채점기가 맞게 잡은 것이다.**
    - 전투 화면의 다른 슬롯은 건드리지 않는다.

### C. 런타임 검증 — 3화면 묶음 (2026-09-01 라이선스 활성)

**Unity 라이선스가 켜졌다.** 작가가 `Tools/unity_license_probe.sh` 로 확인했고 배치 모드 `exit=0`. `exit 198` 은 해소됐다.
지금까지 「통과」라고 적힌 것은 **전부 정적 층**이다. 이 라운드에서 처음으로 런타임을 본다.

11. EditMode 테스트 전량을 돌린다. **로비·전투·휴식 3화면 묶음.**
12. PlayMode 스크린샷 QA(`PortraitUiScreenshotQaTests`)로 세 화면 캡처를 뜬다.
13. `PortraitUiScreenshotQaTests` 가 캡처와 함께 `<screen>_actual_layout.json` 을 뱉게 배선한다(§13-2). 지금은 `DumpLayout` 이 코드 상수에서 뽑는다 — **런타임이 실제로 배치한 좌표와 같은지가 아직 미검증이다.** 두 산출이 어긋나면 그것이 이 라운드의 수확이다. 고치지 말고 차이를 보고한다.

```
UNITY="/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity"

"$UNITY" -batchmode -projectPath . -runTests -testPlatform EditMode \
  -testResults TestResults-EditMode.xml -logFile editmode.log

"$UNITY" -batchmode -projectPath . -runTests -testPlatform PlayMode \
  -testResults TestResults-PlayMode.xml -logFile playmode.log
```

⚠️ **첫 실행은 오래 걸린다.** `Library/` 가 없어 1.3GB 전량 재임포트가 먼저 돈다. 새 워크트리면 거기서 또 한 번 돈다. **정상이니 끊지 않는다.** 느리다는 이유로 테스트 범위를 줄이지 않는다.

⚠️ **마지막 테스트 결과는 2026-05-10 이다.** 넉 달치 코드 변경이 한 번도 테스트를 안 거쳤다는 뜻이다. **이번 실패는 이번 작업 탓이 아닐 수 있다.** 실패가 나오면 셋으로 갈라 보고한다 — ⓐ 이번 휴식 작업이 낸 것 ⓑ 전투 rename 이 낸 것 ⓒ **그 전부터 깨져 있던 것**. ⓒ 는 고치지 말고 목록만 낸다. 범위 밖이다.

⛔ **C-08 은 이번에도 못 잰다.** 「어떤 variants 에서도 대비 4.5:1」인데 variants 가 `blocked`(아트 4종 미입고 + 임계값 미정)라 붕괴 상태 캡처 자체가 안 나온다. **기본 상태로 대체 측정하지 말고 SKIP 사유를 그대로 적는다.** 없는 것을 쟀다고 적는 것이 이 하네스가 막는 첫 번째 실패다.

## 데이터 바인딩 — 추측 금지

사양 `dataBinding` 절이 정본이다. 요약하면:

| 슬롯 | 출처 | 주의 |
|---|---|---|
| `sanityChip` | `PrototypeRunSnapshot.Mental` | **int −100~+100.** 0~1 이 아니다. 정규화 식은 사양에 적혀 있다 |
| `mataiosStateLabel` | `PrototypeRunSnapshot.GlitchLevel` | int 0~100. **비수치 표시** |
| 나머지 | 사양 `dataBinding` 참조 | |

**없는 것** — 마타이오스 전용 이성 값(전수 `Sanity` 0건) · `NpcStage` 런타임 값(`AttachNpcStateMachine` 호출처가 테스트뿐, 정의 에셋 없음). **이 둘에 붙이려 하지 마라.**

## variants — 이번 라운드에서 구현하지 않는다

`strained` · `broken` 둘 다 `status: blocked` 다. 막은 것이 둘이다.

- 아트 4종 미입고 (`mataios_strained` · `mataios_broken` · `rest_strained` · `rest_broken`)
- 붕괴도 → 표시 단계 **임계값 미정** (`thresholds.*: undecided` · 작가 결정 대기)

**계약만 두고 비활성으로 둔다.** 로비의 `tower_deep` 과 같은 처리다. 임의 아트 대체 금지, 임계값 임의 결정 금지.

## 완료 조건

- [ ] 휴식이 독립 모듈이고, 휴식을 바꿔도 다른 화면 테스트가 안 깨진다
- [ ] `python3 Tools/check_layout.py --spec Docs/Portfolio/assets/concept-to-ui/rest_layout_spec.json` → **9/9 PASS**
- [ ] `--actual Docs/Portfolio/assets/concept-to-ui/rest_actual_layout.json` 대조 **전항 PASS**
- [ ] 전투 재대조 `combat_layout_spec_v2.json` ⟂ `combat_actual_layout.json` → **27/27 PASS 복귀**
- [ ] 리터럴 색값 0개 (토큰 경유)
- [ ] 휴식 로직(회복량·붕괴도 delta·버프)은 한 줄도 안 바뀐다. **표현 층 작업이다**
- [ ] `RestLayoutContract.cs` 가 `UnityEngine` 을 참조하지 않는다
- [ ] **EditMode 테스트 실행됨** — 통과/전체 수와 실패 목록 제출. 실패는 ⓐⓑⓒ 로 갈라서
- [ ] **PlayMode 스크린샷 QA 실행됨** — 3화면 캡처 경로 제출
- [ ] **런타임 `actual` ⟂ `DumpLayout` `actual` 대조** — 두 경로가 같은 좌표를 뱉는지. 어긋나면 수치로 보고

## 기각한 대안

**모닥불 씬을 그대로 두고 선택지만 아래로 내린다** — 기각. 세로 화면에서 총합은 100%다. 아래 3분의 2가 비어 있던 것이 이 화면의 실제 문제이고, 선택지만 옮기면 빈 자리가 그대로 남는다.

**마타이오스 상태를 이성 게이지로 그린다** — 기각. 그 값이 코드에 없고, 붕괴도로 대체하면 D-012 P2(붕괴도 숫자 비노출)에 걸린다. 비수치 문구가 두 제약을 동시에 만족하는 유일한 형태다.

## STOP — 하지 말 것

- 휴식 수치·붕괴도 delta·버프 로직 변경. 표현 층만 만진다.
- `MATAIOS_TRAINING_BUFF_ACTIVE` 경로 변경. **별건이고 D-029 영역이다**
- 붕괴도 임계값을 스스로 정하는 것. `undecided` 로 둔다
- 아트 4종을 다른 이미지로 대체하는 것
- 「붕괴도」 · `Glitch` 문자열을 화면 문구에 넣는 것 (R-09)
- 저장 스키마 변경
- Unity 라이선스 우회 시도. 막히면 `exit 198` 을 그대로 보고한다

## 멈춤 조건 (이 넷만)

사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양 밖 요소 필요 · 자동 재시도 3회 초과.
그 밖에는 작업 브랜치 안에서 **묻지 말고 진행한다.** 과잉 질문도 위반이다.

## 반환 (`templates/RETURN.md` 형식)

- 변경 파일 목록
- 휴식 사양 자체 검사 결과 (R-01 ~ R-09 항목별)
- 휴식 actual 대조 결과
- **전투 재대조 결과** (27/27 복귀 여부)
- `rest_actual_layout.json` 경로
- 사양과 어긋난 부분 — **고치지 말고 보고**
- **EditMode / PlayMode 결과 XML 경로**와 통과/전체 수
- 3화면 캡처 경로
- **넉 달 만의 첫 테스트 실행이다.** 기존 실패(ⓒ)는 별도 목록으로 — 고치지 말 것
- 캡처가 안 나오면 안 나왔다고 적는다. **생성했다고 주장하지 않는다**
- 관찰 2건: 채점기가 판정을 대신했는가 / 무승인 범위가 맞았는가

## 다음

끝나면 `QUEUE.md` 3번을 `통과(정적)` 로, 2번을 `통과(정적)` 로 되돌리고 **4번 층 지도 앞에서 멈춘다.** 사양이 없다.
