# NEXT — 이 레인의 유일한 진입점

> **회귀탑 UI 작업 지시를 받았으면 이 파일부터 읽는다.** 사람이 매 라운드 지시문을 옮기지 않는다.
> 이 파일은 항상 「지금 할 것」 하나만 가리킨다. 라운드가 끝나면 게이트가 갱신한다.

## 지금 할 것

**R3 — 전투 재배선 + 배선 검사기 상시화 + 층 지도**

정본: `Docs/Portfolio/assets/concept-to-ui/DISPATCH-r3-combat-rewire-and-floormap.md`
브랜치: `codex/hwigi-lobby-ui-pilot-r3` @ `7e5a698`
직전 판정: `_rejections.md` 「2026-09-01 · 런타임 R2 · ③Gate — 전투 반려」

## 매 라운드 고정 절차

1. 이 파일 → 정본 DISPATCH → `QUEUE.md` → `_rejections.md` 최근 항목. **요약본을 기다리지 않는다.**
2. 착수 전 두 채점기를 돌려 현재 상태를 확인한다.
   ```
   python3 Tools/check_wiring.py
   python3 Tools/check_layout.py --spec Docs/Portfolio/assets/concept-to-ui/<screen>_layout_spec.json
   ```
3. 작업 브랜치 안에서는 **묻지 말고 진행**한다. 멈춤은 넷뿐 —
   사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양에 없는 요소가 필요 · 자동 재시도 3회 초과.
   **과잉 질문도 위반이다.**
4. 완료 조건은 **런타임 actual 대조**다. 정적(contract) 대조는 완료 조건이 아니다.
5. 산출은 리포 안에 둔다. `/private/tmp` 금지 —
   런타임 `<screen>_runtime_actual_layout.json` (`source: runtime`) / 정적 `<screen>_actual_layout.json` (`source: contract`).
6. 끝나면 `templates/RETURN.md` 형식으로 반환하고 `QUEUE.md` 상태를 갱신한 뒤 **다음 「대기」 항목으로 간다.**
   사양이 없는 화면 앞에서만 멈춘다.

## 사람이 하는 말 — 오독 주의

- **「확인해봐」 / 「다음」 / 「이어서」 = 실행하라는 뜻이다.** 직전 라운드를 다시 검증하라는 뜻이 **아니다**.
  🩸 2026-09-01 실측: 「확인해봐」를 재검증으로 읽어 R2 를 다시 재고 R3 를 안 했다. 라운드 하나가 그대로 날아갔다.
- **재검증은 그 말이 명시적으로 나왔을 때만** 한다 — 「R2 다시 검증해」·「재현해봐」.
- 게이트가 이미 판정한 숫자는 다시 재지 않는다. 그 결과가 다음 라운드의 입력이다.
- 그 외 지시가 없으면 위 절차가 기본값이다.

## 게이트가 판정할 것 (사람 아님)

RETURN 은 자기보고로 수용되지 않는다. 게이트가 채점기를 직접 돌려 재현한다.
지금까지 4라운드 연속 자기보고와 차이 0건이었고, **그 신뢰는 「못 한 것을 못 했다고 적은 것」에서 나왔다.**
캡처가 안 나오면 안 나왔다고 적는다. 숫자를 채우지 않는다.
