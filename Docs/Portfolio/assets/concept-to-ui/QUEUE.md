# 화면 큐 — 회귀탑 UI 개편

> 코덱스는 **「대기」 맨 위 항목을 스스로 집는다.** 사람이 매번 다음 지시를 주지 않는다.
> 통과하면 상태를 「통과」로 바꾸고 다음 항목으로 간다. 막히면 「보류」로 두고 사유를 적는다.
> 채점 = `python3 Tools/check_layout.py --spec <spec> --actual <actual> --rejections Docs/Portfolio/assets/concept-to-ui/_rejections.md`

## 전역 (화면보다 먼저)

| # | 항목 | 상태 | 라운드 | 비고 |
|---|---|---|---|---|
| G1 | 폰트 교체 (TMP 에셋) | 대기 | 0 | `ui_tokens.json` font 절. **명조 폰트 라이선스 선택 = 작가 몫** |
| G2 | 색·간격 토큰 배선 | 대기 | 0 | 리터럴 색값 제거. 화면 작업의 전제 |

## 화면

| # | 화면 | 사양 | 상태 | 라운드 | 비고 |
|---|---|---|---|---|---|
| 1 | 로비 | `lobby_layout_spec.json` v1.2 | **통과(정적)** | 2 | 정적 8/8 + 12/12. R2 runtime actual **12/12 PASS** |
| 2 | 전투 (일반) | `combat_layout_spec_v2.json` **v2.1** | **통과(정적)** | 2 | 정적 8 PASS·C-08 SKIP·C-09 PASS + 27/27. R2 runtime actual **2 PASS · 25 FAIL** — 좌표/누락 차이는 `_rejections.md` 기록, 수정 대기 |
| 3 | 휴식 (마타이오스) | `rest_layout_spec.json` **v1.1** | **통과(정적)** | 1 | 정적 9/9 + 20/20. R2 runtime actual **20/20 PASS**. variants 는 아트 4종 미입고 + 임계값 미정으로 blocked |
| 4 | 층 지도 | 미작성 | 대기 | 0 | |
| 5 | 상점 | 미작성 | 대기 | 0 | 6·7과 사양 1장 공유 후보 |
| 6 | 상점 3층 | 미작성 | 대기 | 0 | |
| 7 | 상점 4층 | 미작성 | 대기 | 0 | |
| 8 | 상점 5층 | 미작성 | 대기 | 0 | |
| 9 | 이벤트 (항아리 방) | 미작성 | 대기 | 0 | |
| 10 | 보스 전투 | 미작성 | 대기 | 0 | 2번 사양 파생 |
| 11 | 보스 관문 선택 | 미작성 | 대기 | 0 | |
| 12 | 엔딩 선택 | 미작성 | 대기 | 0 | P1 3악장 종착 |

## 런타임 레인 (2026-09-01 개통)

Unity 라이선스 활성 확인(`Tools/unity_license_probe.sh` · 배치 `exit=0`). `exit 198` 해소.
R2 기준선 실측: `e9d5454`와 R1의 실패 이름 집합은 EditMode **30/30**, PlayMode **8/8** 동일(새 실패 0·고쳐진 것 0)이었다.

| 화면 | runtime actual | 정적 contract 대조 | 상태 |
|---|---|---|---|
| 로비 | `source: runtime`, 8 슬롯 | **12/12 PASS** | runtime 검증됨 |
| 전투 | `source: runtime`, 15/22 슬롯 보고 | **2 PASS · 25 FAIL** | 반려 — 좌표/누락 차이를 `_rejections.md`에 기록, 수정 대기 |
| 휴식 | `source: runtime`, 14 슬롯 | **20/20 PASS** | runtime 검증됨 |

EditMode는 좁은 ML-Agents immutable-meta Error 기대 후 **203/211**(잔여 8), PlayMode는 no-save UI 바인딩 격리 수리 후 **14/21**(잔여 맵 진행 6)이다. 명시적 QA는 통과했고 PNG·runtime actual은 `/private/tmp/hwigi-portrait-ui-v3-screenshots/`에 있다.
⚠️ C-08은 아트 4종 미입고 + `thresholds.*: undecided` variants blocked라 여전히 SKIP. 기본 상태로 대체 측정하지 않았다.

## 상태 값

`대기` → `진행` → `통과` / `보류`

- **보류**로 갈 수 있는 사유는 넷뿐 — 사양 자체 모순 · `D-NNN` 충돌 · 사양 밖 요소 필요 · 자동 재시도 3회 초과
- 그 밖의 이유로 멈추지 않는다. 채점기가 FAIL 을 내면 사람을 부르지 말고 다시 돈다

## 사양 미작성 화면

3~12번은 **로비 파일럿이 통과한 뒤** 서식을 고쳐서 쓴다. 지금 미리 쓰면 서식이 바뀔 때 10장을 다시 고쳐야 한다.
