---
state: provisional
gate: flagged
scope: Nectorial 2026 portfolio repository preparation
updated: 2026-09-06
---

# Nectorial 2026 — 회귀자 타워 증거 패킷

이 문서는 공개 문구의 상한을 정하는 증거 원장이다. 결과물이 아니라, README와 최종 포트폴리오가 넘어서는 안 되는 사실·검증·한계를 기록한다.

## 기여 경계

| 대상 | 작성·소유 | 확인 방법 |
|---|---|---|
| 게임 코드 | 코딩 에이전트 | 커밋 이력 |
| 화면 사양·판정 기준 | 작가 | `Docs/Portfolio/assets/concept-to-ui/` |
| 검사기 | 작가 | `Tools/check_*.py`, `Tools/DumpLayout/` |
| 병합·반려 판정 | 작가 | 검사 결과와 반려 기록 |

## 주장 원장

| ID | 주장 | 근거 포인터 | 증거 상태 | 검증 | 경계 |
|---|---|---|---|---|---|
| HWI-01 | 빈 기본 브랜치에 있던 증거 브랜치 179커밋을 로컬 준비 브랜치에 충돌 없이 병합했다. | merge commit `f5d29fb`, 부모 `908d586` + `cbf7307` | source-backed | current | 원격 push·공개 설정 변경은 하지 않았다. |
| HWI-02 | 화면 사양 대조, 배선, 데이터 연결, 색 토큰 검사기가 저장소에 있다. | `Tools/check_layout.py`, `check_wiring.py`, `check_databinding.py`, `check_color_tokens.py` | source-backed | current | Unity 런타임 성공을 뜻하지 않는다. |
| HWI-03 | 8개 화면의 정적 사양/실물 대조가 이번 준비 실행에서 통과했다. | `Docs/Portfolio/assets/concept-to-ui/*_layout_spec*.json`, `*_actual_layout.json` | measured | current | 전투 사양의 사람 검토 항목 C-08은 자동 검사를 건너뛴다. |
| HWI-04 | 배선·데이터 연결·신규 UI 범위 색 토큰 검사는 통과했다. | `Tools/check_wiring.py`, `check_databinding.py`, `check_color_tokens.py` | measured | current | `PrototypeHud.cs` 등 범위 밖 2파일의 리터럴 색 154건은 이월 상태다. |
| HWI-05 | 맵 흐름 검사는 실패를 보고한다. | `Tools/check_mapflow.py`, `map_flow_spec.json`, `mapflow_runtime_dump.json` | measured | current | MF-03과 `floorActive` 측정 누락 관련 항목은 해결하지 않았다. 맵 흐름 완성을 주장하지 않는다. |
| HWI-06 | 아트 검사는 129개 통과·2개 실패를 냈고, 대조 시트는 사람 판정을 위한 보조물이다. | `Tools/check_art.py`, `art_audit.json`, `art_sheet_*.png` | measured | current | `icon_rest_recover`, `icon_rest_talk` 명도 실패는 그대로 남는다. 화풍·구도 판정은 자동화하지 않는다. |
| HWI-07 | 비로그인 GitHub 공개 요청은 HTTP 404를 반환했다. | `https://github.com/ljhljh0703-cmd/hwigija-tower` | measured | current | 저장소 공개 상태 또는 외부 접근 가능성을 작가가 확인한 뒤에만 지원서 링크로 쓴다. |

## 최종 포트폴리오에 허용되는 문장

- 사양을 먼저 쓰고, 그 사양을 읽어 배선·데이터·화면 상태를 검사하는 구조를 만들었다.
- 자동 검사가 잡지 못하는 맵 흐름과 아트 판단의 한계를 실패 결과로 남기고, 사람 판정 또는 추가 계측으로 분리했다.
- 게임 코드 작성과 사양·검사·병합 판정의 책임을 구분했다.

## 금지되는 확장

- Unity 런타임, 실제 기기, 전체 맵 흐름이 모두 통과했다고 쓰지 않는다.
- 두 아이콘의 아트 실패를 통과로 바꾸지 않는다.
- 이 문서의 로컬 병합을 원격 공개 또는 배포 완료로 쓰지 않는다.
- 회귀자 타워의 현재 비로그인 HTTP 404 응답을 공개 저장소로 쓰지 않는다.
