# DISPATCH R3 — 전투 재배선 + 배선 검사기 상시화 + 층 지도

> 세 덩어리를 한 라운드로 묶는다. 순서가 곧 의존성이다: **A(검사기) → B(전투) → C(층 지도)**.
> 선행 판정 = `_rejections.md` 「2026-09-01 · 런타임 R2 · ③Gate — 전투 반려」. 읽고 시작한다.

## 왜 이 라운드인가

R2 가 이 하네스의 존재 이유를 처음 증명했다. **전투는 「통과(정적) 27/27」이었는데 화면에 배선된 적이 없었다.**
사양과 계약이 서로를 베낀 채 일치했고, 런타임은 구 레이아웃 그대로였다. 좌표 채점기로는 영원히 못 잡는 종류다.

그래서 이번엔 **같은 실수가 기계로 막히게 만든 다음** 전투를 고치고, 그 패턴으로 다음 화면까지 민다.

## 대상

- 브랜치 `codex/hwigi-lobby-ui-pilot-r3` @ `7e5a698` · 워크트리 `/Users/godju/Downloads/AI Game/hwigi-rest-ui-20260901`
- **표준 사례는 휴식이다.** `RestLayoutContract.cs`(순수) → `RestLayout.cs`(Unity 어댑터) → `RestUiController.cs`(모듈) → `PrototypeHud` 에서 `AddComponent`.
  전투를 이 모양의 **두 번째 사례**로 만든다. 새 구조를 발명하지 마라.

---

## Task A · 배선 검사기 상시화 — 제일 먼저

`Tools/check_wiring.py` 를 이미 넣어 뒀다. 지금 돌리면 이렇게 나온다.

```
$ python3 Tools/check_wiring.py
=== CombatLayout (슬롯 22) ===  [FAIL] W-01 배선 — 이 계약을 읽는 런타임 파일이 0개다
=== LobbyLayout  (슬롯 8)  ===  [PASS] W-01 · [PASS] W-02 8/8 = 100%
=== RestLayout   (슬롯 14) ===  [PASS] W-01 · [PASS] W-02 14/14 = 100%
판정: FAIL
```

검사 둘 — **W-01** 계약 타입을 참조하는 비-테스트 런타임 파일이 1개 이상인가 · **W-02** 선언된 슬롯 중 실제 참조 비율이 80% 이상인가(부분 배선 검출).

1. 이 검사기를 **화면 작업의 착수 전·발송 전 필수 절차**로 `QUEUE.md` 채점 줄에 넣는다.
2. Unity 없이 도는 EditMode 테스트로도 한 겹 건다 — 계약이 늘어날 때마다 사람이 스크립트를 기억할 필요가 없게.
3. ⚠️ **`[SKIP] 슬롯 선언을 못 읽었다` 가 뜨면 그건 통과가 아니다.** 검사기가 그 선언 형태를 모르는 것이고 exit 1 로 처리된다. 새 화면이 다른 형태로 슬롯을 선언하면 **검사기를 고쳐서 잡히게 한다.** 선언을 검사기에 맞추지 말고, 검사기를 실물에 맞춘다.
4. ⛔ **검사기를 느슨하게 고쳐 통과시키지 마라.** 임계(`--min-coverage`)를 낮추는 것도 같다. 통과가 안 되면 배선이 안 된 것이다.

**완료 판정**: 계약 3종 전부 W-01·W-02 PASS, `python3 Tools/check_wiring.py` exit 0.

---

## Task B · 전투 재배선 — 이번 라운드의 본체

`combat_layout_spec_v2.json` v2.1 대로 **화면을 실제로 다시 만든다.** 계약 파일은 이미 맞다. 없는 것은 화면이다.

1. `PrototypeHud.cs` 의 전투 부분을 **`CombatUiController.cs` 로 분리**한다. 휴식과 같은 모양.
2. `CombatLayout.cs`(Unity 어댑터)를 만든다. `RestLayout.cs` 를 그대로 본뜬다.
3. `CombatLayoutContract` 의 22 슬롯을 전부 배선한다. **런타임에 존재하지 않던 7종이 핵심이다** —
   `floorChip` · `sanityChip` · `hpChip` · `goldChip` · `threatReadout` · `playerSanityBar` · `mataiosStateLabel`.
   특히 **`threatReadout` 은 전투 개편의 목적 그 자체**다(적의 다음 행동·예상 피해·버틸 턴을 기호와 숫자로. 문장 금지).
4. `combatLog` 는 접힌 상태 기본, 최근 1줄만.
5. `DumpLayout` 의 전투 출력이 새 배선과 계속 맞는지 확인한다.

**참고 — R2 가 잰 런타임 ⟂ 계약 차이**: `combatLog` Δy −0.357, 행동 버튼 3종 Δx +0.002~+0.159 / Δh −0.049, `enemyImage` Δw +0.129.
이건 **구 레이아웃의 좌표**다. 맞춰야 할 대상이 아니라, 지금 화면이 사양과 얼마나 먼지를 보여주는 숫자다.

**⛔ 하지 말 것**
- 전투 수치·밸런스·판정 로직 변경. **표현 층만 만진다.**
- 사양을 코드에 맞춰 고치는 것. 사양이 SSOT 다.
- 「붕괴도」·`Glitch` 를 화면 문구에 넣는 것 (C-09).
- variants(`strained`·`broken`) 구현. 아트 4종 미입고 + `thresholds.*: undecided` 로 여전히 `blocked` 다.

**완료 판정** — 넷 다 충족해야 한다.
- [ ] `python3 Tools/check_wiring.py` 에서 `CombatLayout` W-01 PASS · W-02 100%
- [ ] `check_layout.py --spec combat_layout_spec_v2.json` 8 PASS · C-08 SKIP (변화 없음)
- [ ] **런타임 actual 대조 27/27 PASS** ← 이게 진짜 완료 조건이다. 정적 대조는 완료 조건이 아니다
- [ ] 전투를 바꿔도 로비·휴식 런타임 대조가 안 깨진다 (12/12 · 20/20 유지)

---

## Task C · 층 지도 (큐 4번) — 사양 발행됨

`floormap_layout_spec.json` v1.0 을 냈다. 자체 채점 **8/8 PASS**.

B 와 같은 패턴으로 만든다 — `FloorMapLayoutContract.cs` → `FloorMapLayout.cs` → `FloorMapUiController.cs`.

**이 화면의 요점 하나**: 노드를 **세로 버튼 목록으로 쌓지 않는다.** 지금은 높이 124px·간격 132px 목록이라 갈림길이 길로 안 읽힌다.
데이터에 `NormalizedX` · `NormalizedY` 가 이미 있다. **그 좌표를 그대로 쓴다. 화면이 좌표를 새로 계산하지 않는다.**
간선은 `NextMapNodeIds` 로 그린다.

`dataBinding` 절이 13개 필드를 다 적어 놨다. **없는 것도 적어 놨다** — 층 이름 문자열과 노드 난이도는 데이터에 없으니 사양에서 뺐다. 지어내지 마라.

아이콘 6종 재생성은 아직이다. `assetFallbacks.node_icons_v2` 대로 **기존 A 금색 원형 계열을 재사용**한다. B 회색 선형·C 배너 계열은 쓰지 않는다.

**완료 판정**: 사양 8/8 · `check_wiring.py` W-01·W-02 PASS · **런타임 actual 대조 전항 PASS**.

---

## 공통 · 산출 위치

🔴 **런타임 `actual` JSON 과 캡처를 `/private/tmp` 밖으로 낸다.**
R2 산출이 tmp 에 있어서 내가 슬롯별 수치를 재현하지 못했고, RETURN 표를 인용할 수밖에 없었다. 그건 자기보고 불신 원칙이 깨진 지점이다.

- 런타임 actual → `Docs/Portfolio/assets/concept-to-ui/<screen>_runtime_actual_layout.json` (`"source": "runtime"`)
- 정적 덤프 → `<screen>_actual_layout.json` (`"source": "contract"`) — 파일명이 갈려야 덮어쓰기 사고가 안 난다
- 캡처 → 리포 안 경로. 커밋 여부는 판단에 맡기되 **경로는 리포 안**

## 범위 밖 (고치지 마라)

EditMode 8 · PlayMode 6 잔여 실패는 기준선 `e9d5454` 에도 있던 집합이다. **이번에도 손대지 않는다.** 목록만 유지한다.

## 멈춤 조건 (이 넷만)

사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양에 없는 요소가 필요 · 자동 재시도 3회 초과.
그 밖에는 작업 브랜치 안에서 묻지 말고 진행한다. **과잉 질문도 위반이다.**

## 반환

- Task A: `check_wiring.py` 최종 출력 전문 · EditMode 편입 방식
- Task B: 변경 파일 · 배선 검사 결과 · **런타임 27/27 여부** · 로비·휴식 무회귀 확인
- Task C: 층 지도 사양 8/8 · 배선 · 런타임 대조
- 런타임 actual 3~4종의 리포 내 경로
- 사양과 어긋난 부분 — **고치지 말고 수치로 보고**
- 잔여 실패 목록 (범위 밖 포함)

캡처가 안 나오면 안 나왔다고 적는다. **R1·R2 에서 그 원칙을 지킨 것이 두 번의 판정 근거였다.**

## 다음

C 까지 끝나면 큐 5~8번(상점 4종)이다. **사양 1장을 공유할 후보**라 내가 한 장으로 쓸 수 있는지 보고 있다.
층 지도가 통과하면 그 구조를 그대로 파생시킨다. 4~12번을 한 장씩 새로 쓰지 않는다.
