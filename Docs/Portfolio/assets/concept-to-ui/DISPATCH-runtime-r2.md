# DISPATCH — 런타임 레인 경화 (휴식 R2)

> 스킬 `game-ui-from-concept` 4단 반려 후속. 화면 작업이 아니다. **런타임 검증이 돌게 만드는 라운드**다.
> 선행 판정 = `_rejections.md` 「2026-09-01 · 휴식 R1 · ③Gate 독립 재현」. 읽고 시작한다.

## 왜 이 라운드인가

R1 은 정적 층을 통과했지만 **런타임을 한 번도 못 봤다.** 캡처가 로비 첫 화면에서 멈춰 PNG 0장·런타임 `actual` 0건이다.
지금 `20/20`·`27/27` 은 **사양 ⟂ C# 상수** 대조이고, 그 상수대로 런타임이 배치하는지는 미검증이다. 이 라운드의 목적은 **그 미검증을 없애는 것 하나**다.

화면 사양을 고치지 않는다. 슬롯 좌표를 건드리지 않는다.

## 대상

- 브랜치 `codex/hwigi-lobby-ui-pilot-r3` · 워크트리 `/Users/godju/Downloads/AI Game/hwigi-rest-ui-20260901`
- 기준 커밋 `c72b191` · R1 산출물은 아직 **미커밋 상태로 워크트리에 있다**

## 0단 · R1 을 먼저 커밋한다

§13-3 무승인 범위에 「작업 브랜치에 커밋」이 있다. R1 산출을 커밋하지 않으면 아래 기준선 대조가 뭘 재는지 흐려진다.
**A·B 착수 전에 R1 을 한 커밋으로 고정하고, 그 해시를 반환에 적는다.**

---

## Task C · 기준선 실측 — 제일 먼저, 아무것도 고치기 전에

**왜 먼저인가**: R1 실패 39건을 「기존 파손(ⓒ)」으로 판정했지만 **그건 근본 원인 판독과 mtime 에 근거한 것이지 실측이 아니다.**
브랜치의 옛 XML 은 52·57 케이스뿐이고 이번 실행은 211 이라 범위가 달라 비교가 성립하지 않았다. 숫자로 닫는다.

1. 별도 워크트리를 만들어 **`e9d5454`**(R1 직전)를 체크아웃한다. 현재 작업 워크트리를 건드리지 말 것.
2. **R1 과 똑같은 명령·똑같은 범위**로 EditMode·PlayMode 를 돌린다. 필터를 걸지 않는다.
3. 결과 XML 을 `Docs/Portfolio/assets/concept-to-ui/baseline-e9d5454-{EditMode,PlayMode}.xml` 로 남긴다.
4. R1 결과와 **실패 케이스 이름 집합**을 비교해 표로 낸다 — 기존 실패 / 새 실패 / 고쳐진 것.

⛔ 기준선이 안 돌면 **A·B 를 진행하되 C 는 「미실측」으로 적는다.** 추정으로 채우지 않는다.

---

## Task A · ml-agents 로그 승격 차단 (22건)

EditMode 실패 30건 중 **22건이 단일 원인**이다 — `com.unity.ml-agents/Samples has no meta file, but it's in an immutable folder` 라는 **로그**가 TestRunner 에서 error 로 승격돼 실패 처리된다. 단언 실패가 아니다.

**⛔ 절대 하지 말 것 — 전역 억제.**
`LogAssert.ignoreFailingMessages = true` 를 전체에 거는 것은 **채점기를 끄는 것**이다. 진짜 error 도 같이 묻힌다. 이 하네스가 막는 종류의 우회다.

**허용되는 처리 — 좁을수록 좋다. 아래 순서로 시도한다.**
1. 해당 **메시지 패턴 하나만** 무시하도록 좁힌 규칙(정규식/문자열 일치). 어떤 문자열을 무시하는지 코드에 그대로 남긴다.
2. 1이 안 되면 패키지 쪽 meta 누락을 정공법으로 해소.
3. **패키지 제거는 금지.** `Assets/_Project/Scripts/Training/CombatTrainingAgent.cs` 와 `Editor/TrainingCombatSceneBuilder.cs` 가 실제로 쓰고 있다.

**완료 판정**: 무시 대상이 그 메시지 하나로 한정돼 있고, 그 사실이 코드에 문자열로 보인다. EditMode 실패가 30 → 8 근처로 내려간다.
숫자가 예상과 다르면 **고치지 말고 그대로 보고한다.** 예상이 틀린 것이 발견이다.

---

## Task B · 테스트 격리 수리 — 이 라운드의 목적지

로비 no-save 검증이 실패하고(`Expected: False But was: True`) 거기서 캡처가 멈춘다. 그래서 런타임 `actual` 이 한 번도 안 나왔다.

**관측 사실만 적는다.**
- `PrototypeRunSaveStore` 는 `Application.persistentDataPath` 의 파일이다. `Delete()` 는 파일이 있으면 지운다.
- `LobbySmokeTests` 는 `[SetUp]` 에서 `Delete()` + `RequestNewGame()` 을 부르는데도 Continue 버튼이 활성이었다.
- `CaptureLobbyNoSave` 도 같은 순서로 같은 지점에서 실패한다.
- PlayMode 맵 진행 실패 6건은 `ENC_REST_01` · `CHOICE_COMBAT_1_ENGAGE` · `CHOICE_SHOP_1_BUY_ITEM` 을 못 찾는다.

**🧪 미검증 가설(내 것이다. 사실로 취급하지 마라)**: 위 둘이 **런/저장 상태가 테스트 사이로 새는 한 뿌리**일 수 있다. 파일을 지워도 남는 정적 캐시가 있는지부터 본다.
**가설이 틀리면 틀렸다고 보고한다.** 가설에 맞추려고 코드를 비틀지 않는다.

**⛔ 하지 말 것**
- 단언을 약하게 고쳐 통과시키는 것. `Assert.IsFalse(continueButton.interactable)` 는 **맞는 단언이다** — 저장이 없으면 이어하기는 꺼져 있어야 한다. 이 줄을 고치면 그 순간 이 테스트는 죽은 것이다.
- 로비의 기대 동작을 누수에 맞춰 바꾸는 것.
- 테스트를 `Ignore`/`Explicit` 으로 돌려 실패를 없애는 것.
- 저장 스키마 변경. 여전히 별건이다.

**완료 판정**: 로비 no-save 테스트가 **단언을 그대로 둔 채** 통과하고, `PortraitUiScreenshotQaTests` 가 끝까지 돌아 **PNG 와 런타임 `actual` JSON 이 실제로 생성된다.**

---

## Task D · 런타임 actual 대조 (B 가 열려야 가능)

B 로 런타임 `actual` 이 나오면, **이 라운드의 진짜 수확**이 여기 있다.

```
python3 Tools/check_layout.py --spec <spec> --actual <런타임 actual> \
  --rejections Docs/Portfolio/assets/concept-to-ui/_rejections.md --round 2
```

로비·전투·휴식 3화면 전부. 그리고 **런타임 `actual` ⟂ `DumpLayout` 정적 `actual` 을 서로 대조**한다.
**어긋나면 그것이 발견이다. 고치지 말고 슬롯별 수치 차이로 보고한다.** 어느 쪽이 옳은지는 내가 판정한다.

🆕 **산출 구분 표기**: 이번부터 `*_actual_layout.json` 최상위에 `"source": "runtime"` 또는 `"source": "contract"` 를 넣는다.
R1 에서 정적 산출과 런타임 산출이 같은 파일명을 쓸 뻔했다. 파일이 스스로 출처를 말해야 한다.

---

## C-08

**여전히 못 잰다.** variants 가 `blocked`(아트 4종 미입고 + `thresholds.*: undecided`)다. 붕괴 상태 캡처가 없다.
**기본 상태로 대체 측정하지 말고 SKIP 사유를 그대로 적는다.** R1 에서 맞게 처리했다. 이번에도 같다.

## 멈춤 조건 (이 넷만)

사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양에 없는 요소가 필요 · 자동 재시도 3회 초과.
그 밖에는 작업 브랜치 안에서 묻지 말고 진행한다. **과잉 질문도 위반이다.**

## 반환

- R1 커밋 해시 · 이 라운드 커밋 해시
- **Task C 표**: 기존 실패 / 새 실패 / 고쳐진 것 (이름 집합 비교). 못 돌렸으면 「미실측」
- **Task A**: 무시한 문자열 원문 · EditMode 실패 30 → ?
- **Task B**: 근본 원인 (내 가설이 맞았는지 틀렸는지 명시) · 고친 파일 · **단언을 약화한 곳이 있으면 전부 신고**
- **Task D**: 3화면 런타임 actual 대조 결과 · 런타임 ⟂ 정적 차이 슬롯별 수치 · PNG 경로
- 남은 실패 목록 (고치지 않은 것 포함)

캡처가 또 안 나오면 안 나왔다고 적는다. **생성했다고 주장하지 않는다.** R1 에서 그 원칙을 지킨 것이 이번 판정의 근거였다.

## 다음

끝나면 `QUEUE.md` 의 런타임 레인 절을 실측으로 갱신하고 **4번 층 지도 앞에서 멈춘다.** 사양은 내가 쓰고 있다.
