# DISPATCH M3 — 실패가 나는 그 경로에서 다시 잰다. 아직 고치지 않는다

> **M2 는 맞게 돌았는데 내가 틀린 것을 재게 시켰다.** 계약 결함이다. 고쳐서 다시 낸다.

## M2 잘한 것 셋

① **한계를 스스로 적었다** — *「QA floor-open 경로에서는 재현되지 않았고 … M3 에서 대조해야」*. 초록불을 「해결됐다」로 포장하지 않았다.
② **`MF-03` FAIL 을 숨기지 않고 그대로 커밋**했다. 통과시키려고 사양을 만지지 않았다.
③ **production 맵 코드 변경 0.** 진단 fixture 로만 쟀다.

## 🩸 내 계약 결함 — v1.0 이 2단계뿐이었다

실패 문구를 다시 읽는다.

```
Missing selectable map node **while advancing toward** ENC_REST_01
```

**「진행 중(advancing)」이다.** 층 진입 직후가 아니라 **노드를 몇 개 해결한 뒤**다.
그런데 v1.0 `dumpContract` 는 `beforeSelect` / `afterSelect` 둘만 요구했다. **실패 구간을 볼 수 없는 계약이었다.**

거기에 `OpenQaFloor` 는 층을 **초기화**한다 — `_resolvedDemoSteps.Clear()` · `_selectedMapNodeId=""` · `AttachDemoRunPath` 재부착.
그래서 MF-07·MF-08 이 초록불로 나왔다. **맵 빌더가 멀쩡하다는 것은 맞고, 그게 실패와 무관하다는 것도 맞다.**

## 고친 것

| | |
|---|---|
| `map_flow_spec.json` **v1.1** | `dumpContract` **3단계**(+`afterResolve`) · 층당 반복 순회 요구 · **`MF-11` 신설**(해결하면 다음 노드가 열리는가) · `OpenQaFloor` 금지 명시 |
| `Tools/check_mapflow.py` | **phase 스냅샷이 0개면 `[SKIP]` + exit 1.** `MF-11` 이 실제로 거짓 통과했다 — 아무것도 안 재고 PASS 로 보였다 |

## Task A · 정상 순회로 덤프한다

⛔ **`OpenQaFloor` 를 쓰지 마라.** `BeginRun()` 부터 **정상 순회**로 간다 — 선택 → 해결 → 다음 선택.

층마다 이렇게 찍는다.

```
① beforeSelect   층 진입 직후, 선택 전
② afterSelect    노드 선택 직후
③ afterResolve   인카운터 해결 직후   ← 이번 라운드의 핵심
   ②③을 그 층 노드가 소진될 때까지 반복
```

**진행이 막히면 막힌 그 지점의 스냅샷이 이 라운드의 산출이다.** 막힌 것을 실패로 보지 마라 — 그게 우리가 찾던 것이다.

## Task B · 대조하고 보고한다

```
python3 Tools/check_mapflow.py
```

- **`MF-11`** 이 이 라운드의 표적이다. 해결 후 미완료 노드가 남아 있는데 선택 가능이 0이면 거기가 지점이다
- `[SKIP]` 이 뜨면 그 phase 를 안 찍은 것이다. **통과가 아니다**

## 같이 답할 것 — 각 1줄

- 막힌 지점에서 `_selectedMapNodeId` 값은? (빈 문자열이어야 다음 노드가 열린다)
- 그때 `GetActiveMapLayer()` 반환값과 남은 노드들의 `Layer` 는? (`node.Layer != activeLayer` 가 첫 관문이다)
- 노드 해결 경로가 `_selectedMapNodeId` 를 지우는 6곳 중 **어디를 타는가**, 혹은 **아무 데도 안 타는가**

## ⛔ 하지 말 것

- **고치지 마라.** M4 가 수리 라운드다
- `D-034` 잠긴 값 변경 금지 — 코드가 계약과 다르면 **코드가 틀린 것**
- `map_flow_spec.json` 을 코드에 맞춰 고치기 금지. 어긋나면 **보고**
- 검사기 완화 금지. `[SKIP]` 은 exit 1 이다
- **`MF-03`(cross-lane 4>2) 을 지금 고치지 마라.** 진짜 D-034 위반이지만 14건의 원인이 아니다. 별도 항목으로 남긴다
- 범위 밖 3건(오디오 · `GenericCounterplay` · `ItemAndAbilityEffects`) 무접촉
- UI 12화면·8계약 무접촉 (검사기 4종 PASS 상태)

## 멈춤 조건 (이 넷만)

사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양에 없는 요소가 필요 · 자동 재시도 3회 초과.

## 반환

- 3단계 덤프 경로 + `check_mapflow.py` 출력 전문
- **막힌 지점** — 층·레이어·노드 id·그때의 `_selectedMapNodeId`
- 위 3문 답
- 고친 것 = **0이어야 정상**
