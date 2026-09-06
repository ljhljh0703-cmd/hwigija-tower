# DISPATCH M2 — D-034 대조. 아직 고치지 않는다

> M1 이 오염 가설을 **기각**했다(14/14 가 새 프로세스·깨끗한 자산에서도 실패). **진짜 회귀다.**
> 그렇다고 바로 고치지 않는다. **「무엇이 맞는 상태인가」가 기계로 판정되지 않으면 고쳐도 고친 줄 모른다.**

## M1 잘한 것 셋

① 가설을 **기각으로 확정**했다 — 14개를 각각 새 Unity 프로세스로 재는 방식은 내가 지시한 것보다 엄격하다.
② **묶음 실행 중 부수효과를 발견하고 그 결과를 근거로 쓰지 않았다.** 오염이 실재하지만 이번 실패의 원인은 아니라는 두 사실을 섞지 않았다.
③ **코드 변경 0.** 재는 라운드에서 아무것도 안 고친 것이 맞다.

## 이번 라운드에 게이트가 낸 것

| 파일 | 무엇 |
|---|---|
| `Docs/Portfolio/assets/concept-to-ui/map_flow_spec.json` | **D-034 원문을 덤프 대조 가능한 10개 제약으로 옮긴 것** |
| `Tools/check_mapflow.py` | 그 대조를 도는 검사기 |

🔴 **`map_flow_spec.json` 의 값은 전부 D-034 원문에서 옮겼다. 내가 정한 수치는 하나도 없다.**
D-034 에 없는 것은 `undecided` 절에 두고 **검사하지 않는다**(분기 빈도·Rest 확률·lane↔column 매핑).

## Task A · 덤프를 뱉게 배선한다

`map_flow_spec.json` §`dumpContract` 가 형식이다. UI 레인의 `<screen>_runtime_actual_layout.json` 과 같은 자리다.

```
Docs/Portfolio/assets/concept-to-ui/mapflow_runtime_dump.json
{ "source": "runtime", "runId": "...", "floors": [ { "floor": 1, "phase": "beforeSelect",
  "selectedMapNodeId": "", "nodes": [ {13필드} ] }, ... ] }
```

**핵심 = 층마다 두 스냅샷이다.**
- `phase: "beforeSelect"` — 층 진입 직후, 노드 고르기 전
- `phase: "afterSelect"` — 노드 1개 고른 직후

**둘이 다 있어야 「commitment 인가 stuck 인가」가 갈린다.** 하나만 있으면 못 잰다.

⛔ **정적 산출로 대체하지 마라.** `source` 가 `runtime` 이 아니면 검사기가 첫 줄에서 FAIL 낸다.

## Task B · 대조하고 **어긋난 지점만 보고한다**

```
python3 Tools/check_mapflow.py
```

**이 라운드는 여기서 끝이다. 고치지 마라.**

가장 볼 것 둘 —

- **`MF-07` 선택 가능 노드 >= 1 (선택 전)** — 실패 14건의 증상이 정확히 여기다(`선택 가능 노드 0`, `Missing ... among <빈 목록>`)
- **`MF-08` 선택 후 선택 가능 == 0** — MF-07 의 짝

**둘을 같이 봐야 갈린다.**
`IsMapNodeSelectable` 은 `_selectedMapNodeId` 가 차 있으면 **전부 선택 불가**로 만든다. 그 규칙 자체는 D-034 commitment 대로다.
문제는 **그 값이 언제 지워지는가**이고, 지우는 지점이 **6곳**이다(`AttachDemoRunPath` · 층 이동 2곳 · 명시 해제 · fallback 실패 · skip 처리).
→ MF-07 이 빨간불이고 MF-08 이 초록불이면 **stuck**이다. 둘 다 빨간불이면 다른 뿌리다.

## 같이 답할 것 — 각 1줄

- 층 진입 직후 `_selectedMapNodeId` 값은 무엇인가 (빈 문자열이어야 정상)
- `GetActiveMapLayer()` 가 반환하는 층과, 노드들의 `Layer` 분포는 어떤가 (`node.Layer != activeLayer` 가 선택 불가의 첫 관문이다)
- 저장 복원 경로(`IsSavedMapNodeSelectable`)를 타는가 (테스트가 저장을 안 거치면 무관)

## ⛔ 하지 말 것

- **고치지 마라.** M3 에서 고친다. 지금 고치면 무엇이 원인이었는지 또 모른다
- `D-034` 잠긴 값을 바꾸지 마라 — **코드가 계약과 다르면 코드가 틀린 것**이다
- `map_flow_spec.json` 을 코드에 맞춰 고치지 마라. 어긋나면 **보고**한다
- 검사기를 느슨하게 고치지 마라. `[SKIP]` 은 통과가 아니고 exit 1 이다
- 범위 밖 3건(오디오 NullReference · `GenericCounterplay` 문구 · `ItemAndAbilityEffects`) 손대지 마라 — `outOfScope` 절에 적혀 있다
- UI 12화면·8계약 무접촉. 검사기 4종이 PASS 상태다

## 멈춤 조건 (이 넷만)

사양 자체 모순 · `D-NNN` 잠긴 결정 충돌 · 사양에 없는 요소가 필요 · 자동 재시도 3회 초과.

## 반환

- `mapflow_runtime_dump.json` 경로 + `check_mapflow.py` 출력 전문
- MF-01~MF-10 중 **어긋난 것과 그 수치**
- 위 3문 답
- 고친 것 = **0이어야 정상**
