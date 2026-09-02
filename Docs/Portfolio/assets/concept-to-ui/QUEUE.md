# 화면 큐 — 회귀탑 UI 개편

> 🏁 **큐 12화면 전건 런타임 통과 (2026-09-02 · `d8c64b4`).** 배선 8계약 100% · 자기보고 차이 0건 7라운드 연속.
> 남은 것 = 전역 G1(폰트 라이선스=작가) · **G2 보류(색 토큰 결정 필요)** · 아트 아이콘 6종 · push.
> ▶ **진입점은 `NEXT.md` 다.** 지시를 받았으면 거기부터 읽는다. 사람이 지시문을 옮기지 않는다.

> 코덱스는 **「대기」 맨 위 항목을 스스로 집는다.** 사람이 매번 다음 지시를 주지 않는다.
> 통과하면 상태를 「통과」로 바꾸고 다음 항목으로 간다. 막히면 「보류」로 두고 사유를 적는다.
> 채점 = **두 개다. 둘 다 돌린다.**
>   ① 좌표 `python3 Tools/check_layout.py --spec <spec> --actual <actual> --rejections Docs/Portfolio/assets/concept-to-ui/_rejections.md`
>   ② **배선 `python3 Tools/check_wiring.py`** — 계약이 화면에 연결됐는지. 🔒 2026-09-01 신설, 전투 반려가 낳았다.
>   ⚠️ ①만 통과하고 ②가 FAIL 이면 **사양대로 그려지지 않는다.** 「27/27 통과」가 그렇게 나왔다.
>   ⛔ 완료 조건은 **런타임 actual 대조**다. 정적(contract) 대조는 완료 조건이 아니다.

## 전역 (화면보다 먼저)

| # | 항목 | 상태 | 라운드 | 비고 |
|---|---|---|---|---|
| G1 | 폰트 교체 (TMP 에셋) | 대기 | 0 | `ui_tokens.json` font 절. **명조 폰트 라이선스 선택 = 작가 몫** |
| G2 | 색·간격 토큰 배선 | **보류** | 1 | `UiTokenContract` 16/16 정합 완료. `check_color_tokens.py`가 리터럴 **162건**을 검출했고, `Color.white`·임의 RGBA·상태색의 대응 토큰이 없다. 새 색 선택은 작가 결정 필요 |

## 화면

| # | 화면 | 사양 | 상태 | 라운드 | 비고 |
|---|---|---|---|---|---|
| 1 | 로비 | `lobby_layout_spec.json` v1.2 | ✅ **통과(런타임)** | 3 | 정적 12/12 · **런타임 12/12** · 최초 5 FAIL 은 writer 계측 오류로 판명(정정 보존) |
| 2 | 전투 (일반) | `combat_layout_spec_v2.json` v2.1 | ✅ **통과(런타임)** | 3 | 반려 해소. **런타임 27/27** · 배선 22/22 · 구 빌더 경로 안전 제거(−149줄) |
| 3 | 휴식 (마타이오스) | `rest_layout_spec.json` v1.1 | ✅ **통과(런타임)** | 1 | 사양 9/9 · 정적 20/20 · **런타임 20/20** · 캡처 생성됨. variants 는 아트 4종 + 임계값 미정으로 blocked |
| 4 | 층 지도 | `floormap_layout_spec.json` v1.0 | ✅ **통과(런타임)** | 1 | 사양 8/8 · **런타임 14/14** · 배선 10/10 |
| 5 | 상점 | `shop_layout_spec.json` v1.2 (공유) | ✅ **통과(런타임)** | 1 | 5행+나가기 · 배선 13/13 · 런타임 PASS |
| 6 | 상점 3층 | `shop_layout_spec.json` v1.2 (공유) | ✅ **통과(런타임)** | 1 | 5행+나가기 · 배선 13/13 · 런타임 PASS |
| 7 | 상점 4층 | `shop_layout_spec.json` v1.2 (공유) | ✅ **통과(런타임)** | 1 | 5행+나가기 · 배선 13/13 · 런타임 PASS |
| 8 | 상점 5층 | `shop_layout_spec.json` v1.2 (공유) | ✅ **통과(런타임)** | 1 | 5행+나가기 · 배선 13/13 · 런타임 PASS |
| 9 | 이벤트 (항아리 방) | `event_layout_spec.json` v1.0 | ✅ **통과(런타임)** | 1 | 런타임 14/14 · 배선 9/9 |
| 10 | 보스 전투 | `combat_layout_spec_v2.json` 재사용 | ✅ **통과(런타임)** | 1 | 런타임 27/27 · 새 계약 0 |
| 11 | 보스 관문 선택 | `bossgate_layout_spec.json` v1.0a | ✅ **통과(런타임)** | 1 | 런타임 15/15 · 배선 10/10 |
| 12 | 엔딩 선택 | `ending_layout_spec.json` v1.0 | ✅ **통과(런타임)** | 1 | 런타임 10/10 · 배선 6/6 · **두 선택지 크기 동일 실증** |

## 런타임 레인 (2026-09-01 개통)

Unity 라이선스 활성 확인(`Tools/unity_license_probe.sh` · 배치 `exit=0`). `exit 198` 해소.
R2 기준선 실측: `e9d5454`와 R1의 실패 이름 집합은 EditMode **30/30**, PlayMode **8/8** 동일(새 실패 0·고쳐진 것 0)이었다.

| 화면 | runtime actual | 정적 contract 대조 | 상태 |
|---|---|---|---|
| 로비 | `source: runtime`, 8 슬롯 | **12/12 PASS** | R3 재캡처 runtime 검증됨 |
| 전투 | `source: runtime`, 22 슬롯 | **27/27 PASS** | R3 재배선 runtime 검증됨 |
| 휴식 | `source: runtime`, 14 슬롯 | **20/20 PASS** | R3 재캡처 runtime 검증됨 |
| 층 지도 | `source: runtime`, 10 슬롯 | **14/14 PASS** | R3 신규 runtime 검증됨 |
| 상점 (5~8 공유) | `source: runtime`, 13 슬롯 | **20/20 PASS** | R4 1·3·4·5층 캡처 runtime 검증됨 |
| 이벤트 | `source: runtime`, 9 슬롯 | **14/14 PASS** | R5 스크롤 목록·고정 이탈 조작부 runtime 검증됨 |
| 보스 관문 | `source: runtime`, 10 슬롯 | **15/15 PASS** | R5 상태 점검판·단일 진입 runtime 검증됨 |
| 엔딩 | `source: runtime`, 6 슬롯 | **10/10 PASS** | R5 두 선택지 동일 크기·자원 칩 없음 runtime 검증됨 |

R5 전량 검증은 EditMode **206/214**(잔여 8), PlayMode **15/22**(잔여 맵 진행 6 + explicit 1 SKIP)로 기준선 실패 이름 집합을 유지했다. 명시적 QA **1/1 pass**; PNG는 `Docs/Portfolio/assets/concept-to-ui/runtime-captures/`, runtime actual은 같은 상위 디렉터리에 있다.
⚠️ C-08은 아트 4종 미입고 + `thresholds.*: undecided` variants blocked라 여전히 SKIP. 기본 상태로 대체 측정하지 않았다.

## 상태 값

`대기` → `진행` → `통과` / `보류`

- **보류**로 갈 수 있는 사유는 넷뿐 — 사양 자체 모순 · `D-NNN` 충돌 · 사양 밖 요소 필요 · 자동 재시도 3회 초과
- 그 밖의 이유로 멈추지 않는다. 채점기가 FAIL 을 내면 사람을 부르지 말고 다시 돈다

## 사양 미작성 화면

없음. 12화면 사양이 모두 발행됐고, 화면 1~12의 런타임 대조까지 완료됐다.

## 표준 배선 패턴 (휴식이 기준 사례)

`<S>LayoutContract.cs`(순수 C#) → `<S>Layout.cs`(Unity 어댑터) → `<S>UiController.cs`(모듈) → `PrototypeHud` 에서 `AddComponent`.
**새 구조를 발명하지 않는다.** 전투가 이 모양의 두 번째 사례이고, 층 지도가 세 번째다.
