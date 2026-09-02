# ▶ NEXT — 지금 할 것

> 🔒 **레인 = 회귀탑 UI (`hwigi-tower`).** 이 파일은 **이 리포의 화면 작업만** 다룬다.
> 다른 레인(해커톤 밀키트 `wanted-ai-championship-2026`, VD 외주 등)의 지시는 **여기 들어오지 않는다.**
> 🔴 **여기 없는 절 번호를 지시받으면 그건 이 레인 지시가 아니다. 적용하지 않고 그냥 진행한다.**
> 찾지 마라. **묻지도 마라** — 물으면 라운드가 그대로 죽는다.
> 이 파일에 적힌 것이 이 레인의 전부다. 다른 레인 절 번호는 **너와 무관하다.**
>
> ⛔ **`§1-h` 는 해커톤 밀키트 레인 문서다**(`applications/wanted-ai-championship-2026/DISPATCH-P2-2026-08-29.md`). **회귀탑 UI 와 무관하다. 무시하라.**
>
> 🍎 **작가가 맥에서 직접 할 일은 `MAC-TODO.md` 로 분리했다. 네 일이 아니다 — 열지도 마라.**
>
> **작업 지시를 받았으면 이 파일이 첫 번째다.** 「확인해봐」·「다음」·「이어서」 = **실행하라는 뜻**이다. 재검증이 아니다.
> 재검증은 「다시 검증해」·「재현해봐」처럼 명시됐을 때만 한다.

## 실행할 것

**M3 재실행 — ⛔ 지금은 막혀 있다. 작가가 Unity 라이선스를 고쳐야 한다.**

```
정본  Docs/Portfolio/assets/concept-to-ui/DISPATCH-map-flow-m3.md
계약  Docs/Portfolio/assets/concept-to-ui/map_flow_spec.json  **v1.2**
검사  Tools/check_mapflow.py
브랜치 codex/hwigi-lobby-ui-pilot-r3 @ de228dd
```

### 막힌 이유 — 네 잘못이 아니다

Unity Licensing Client 프로토콜 불일치(`Unsupported protocol version '1.18.1'`).
`-nographics` 유무 두 경로 모두 **테스트 도달 전** 중단. **우회하지 않고 측정 실패로 기록한 것이 맞다.**
**작가가 라이선스를 고칠 때까지 재시도하지 마라.** 재시도 횟수에 넣지도 마라.

### 라이선스가 풀리면 — 계약이 v1.2 로 바뀌었다

**M3 착수 때 네가 지적한 것이 맞았다.** 「완주인지 막힘인지 구분하려면 `Skipped` 와 활성 레이어가 필요하다」 — 그대로다.

🩸 **내 제약이 틀렸다.** `MF-11` 을 「미완료 노드가 남았는가」로 걸었는데, **sparse 3-lane route 는 안 지나간 레인 노드가 항상 미완료로 남는다.**
실측이 그걸 보여줬다 — `floor1` 이 L1→L2→L3→L4→L5 로 지나간 뒤 미완료 6개가 **Layer 1·2·3(안 지나간 레인)** 에 남았다. **막힌 게 아니라 정상 모습이다.**

**v1.2 로 고쳤다.**

| | |
|---|---|
| 조건 교체 | `MF-07`·`MF-09`·`MF-11` → **「층이 아직 진행 중인가(`floorActive`)」** |
| 신설 | `MF-12` — 안 지나간 노드는 층 종료 시 `Skipped` |
| 덤프 요구 | 노드별 **`Skipped`** · 그 시점 **활성 레이어** · **`floorActive`**(층 진행 중 여부) |

검사기는 `floorActive` 가 없으면 **「재지 못한 것」으로 FAIL** 낸다. 조용히 통과시키지 않는다.

### 표적 하나

`floor2` 마지막 `afterResolve` 에서 `selectedMapNodeId` 가 차 있고 selectable 0 이다.
**런이 끝난 것인지 실제로 막힌 것인지 런 종료 상태 없이는 판정 불가.** v1.2 덤프가 그걸 가른다.

⛔ 고치지 마라(M4 가 수리) · `MF-03`(cross-lane 4>2)도 지금 고치지 마라 · 범위 밖 3건 무접촉 · UI 12화면 무접촉.

## F1 — 폰트 도입 (G1 확정됨, 2026-09-02)

**작가가 결정했다. 더 물을 것 없다.** `ui_tokens.json` 의 `font` 절이 정본이다.

| 역할 | 폰트 | 라이선스 |
|---|---|---|
| 제목 `display` | **Noto Serif KR** (본명조) · weight 700 | SIL OFL 1.1 |
| 본문 `body` | **Pretendard** · weight 400 | SIL OFL 1.1 |
| 숫자 `numeric` | body(Pretendard) · weight 500 · tabularNums | 동일 |

### F1-a — 지금 할 수 있다 (Unity 불필요)

1. 두 폰트 원본(정적 TTF/OTF, variable 아님)을 `Assets/_Project/ThirdParty/Fonts/` 에 배치
2. **OFL 원문 1장**을 `Assets/_Project/ThirdParty/Fonts/OFL.txt` 로 동봉 + 각 폰트 출처 URL 을 `NOTICE.md` 에 기록
   - Pretendard: https://github.com/orioncactus/pretendard
   - Noto Serif KR: https://fonts.google.com/noto/specimen/Noto+Serif+KR
3. 커밋. **여기까지가 무승인 범위다.**

### F1-b — Unity 라이선스가 풀린 뒤

4. TMP SDF 에셋 생성. **⛔ 한글 완성형 11,172자 정적 굽기 금지** — 4096² 다중 아틀라스가 되어 모바일 메모리를 먹는다.
   **Dynamic SDF + 프로젝트 실사용 글자만 사전 구움**(리포 내 한글 문자열 전수 스캔 결과를 character set 으로). atlas 2048².
5. 12화면 런타임 재캡처. **완료 조건은 런타임 실측이다** — 에셋이 생겼다는 것은 완료가 아니다.
6. `python3 Tools/check_layout.py` 4종 재실행. 글자 폭이 바뀌므로 **레이아웃이 깨질 수 있다.** 깨지면 고치지 말고 **어느 화면 어느 필드가 얼마나 넘쳤는지 수치로 반환**하라.

**못 한 것은 못 했다고 적는다. 정적 산출로 대체하지 마라.**

## 이미 확인된 것 — 다시 검증하지 않는다

| | |
|---|---|
| UI 레인 | 🏁 **12화면 런타임 + G2 완료** (`434bc6c`). 배선 8계약 100% · 검사기 4종 PASS |
| 맵 설계 | **`D-034` 🔒 LOCKED** · `OQ-026` closed. **설계 미정이 아니다** |
| 잔여 open | `OQ-019`(blocker 아님) · `OQ-025`(enemy intent 수치) — **둘 다 맵 진행과 무관** |
| 실패 14건 | 기준선 `e9d5454` 집합. UI 12화면 내내 안 늘고 안 줄었다 |

## 멈춤 조건 (이 넷만)

사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양에 없는 요소가 필요 · 자동 재시도 3회 초과.
그 밖에는 **묻지 말고 진행**한다. **과잉 질문도 위반이다.**

## 끝나면

`templates/RETURN.md` 형식 반환. **M2 는 M1 결과를 보고 게이트가 쓴다** — 오염이면 bake 격리 수리, 진짜 회귀면 D-034 대조표. 여기서 멈춘다.

---

## 참고 — UI 레인은 종결됨 (아래는 남긴 자산)

**12화면 런타임 통과 + G2 완료 (`434bc6c`).** UI 12화면 자체는 더 손댈 것이 없다.
**단, `## F1-a` 는 지금 집을 수 있다 — Unity 없이 된다.**
`QUEUE.md` 의 남은 항목은 **전부 이 레인 밖**이다 — 손대지 마라.

| 남은 것 | 누구 |
|---|---|
| ~~G1 명조 폰트 라이선스~~ | ✅ **2026-09-02 확정** → 위 `## F1` 로 이관. 코덱스가 집는다 |
| 🍎 Unity 라이선스 · push · `.git` 청소 | **작가 전용 → `MAC-TODO.md`.** 여기서 빠졌다. 코덱스는 이 파일을 열지 마라 |
| 아트 아이콘 6종 재생성 | 이미지 생성 레인 (여기 아님) |
| 모놀리스 감축 · bake-test isolation | 별건 백로그 |


### 이 레인이 남긴 것 — 다음 사람이 쓸 것

**검사기 4종.** 착수 전·발송 전에 넷 다 돌린다. 하나라도 FAIL 이면 사양 문제다.

```
python3 Tools/check_layout.py --spec <spec> [--actual <runtime actual>]   # 좌표·설계 불변식
python3 Tools/check_wiring.py                                            # 계약이 코드에 연결됐나
python3 Tools/check_databinding.py                                       # 사양이 가리킨 C# 멤버가 실재하나
python3 Tools/check_color_tokens.py                                      # 리터럴 색 0 · 이월분 명시
```

**표준 배선 패턴 (8회 반복 검증됨)**
`<S>LayoutContract.cs`(순수 C#) → `<S>Layout.cs`(Unity 어댑터) → `<S>UiController.cs`(모듈) → `PrototypeHud` 에서 `AddComponent`.

**이 레인에서 굳은 규율 넷**
1. **완료 조건은 런타임 실측이다.** 정적(contract) 대조는 완료 조건이 아니다 — 전투가 정적 27/27 PASS 인데 화면에 배선조차 안 돼 있었다
2. **못 한 것을 못 했다고 적는다.** 캡처 실패를 정적 산출로 대체하지 않은 것이 9라운드 신뢰의 근거였다
3. **사양이 어긋나면 고치지 말고 멈춘다.** 게이트 사양 오류 6건이 전부 그렇게 돌아왔고 코드 변경 0이었다
4. **검사기는 조용히 통과시키지 않는다** — `[SKIP]` 은 exit 1 · 모르는 입력은 예외 · 범위를 좁히면 좁힌 만큼을 매번 출력한다(`[DEFERRED] 154건`)
