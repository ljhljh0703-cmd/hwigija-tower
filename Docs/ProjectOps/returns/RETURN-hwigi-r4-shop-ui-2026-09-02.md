---
session: "2026-09-02 hwigi-r4-shop-ui"
agent: "codex"
entry_point: "NEXT.md · R4"
based_on:
  - "D-012"
  - "D-029"
  - "skills/game-ui-from-concept/SKILL.md v0.1.0"
gate: ready_for_review
skill_candidate: false
type: return-manifest
origin: agent_generated
state: provisional
---

# RETURN — 회귀탑 UI R4 상점 5~8 공통 화면

## 기준

- 시작 HEAD: `1e653d36ae78f2b2ccaf0e5bdbfe94170e7d4792` (`Implement runtime UI R3 combat and floor map`)
- 정본: `Docs/Portfolio/assets/concept-to-ui/shop_layout_spec.json` v1.2.
- D-012/D-029에 따라 `붕괴도`·`Glitch` 표시, 전투/상점 수치 로직, 저장 스키마는 변경하지 않았다.

## 착수 전 사양 검증

v1.0의 구매 3장과 v1.1의 `OwnedItemCount`가 각각 실제 데이터와 C# 공개 멤버에 맞지 않아 STOP했다. 두 초안은 코드 변경 없이 되돌렸다.

- v1.2는 구매 5행 + 나가기 1을 명시한다.
- `ENC_SHOP_01·03·04·05`와 `ENC_F02_SHOP_001` 모두 `BUY_ITEM·BUY_OIL·BUY_CHARM·BUY_ABILITY·BUY_RECALL + LEAVE` 구조임을 확인했다.
- `ownedCount.source`는 실제 공개 멤버 `PrototypeRunSnapshot.ItemCount`로 정정됐다.
- 착수 전 3검사기: 좌표 9/9 · 배선 4계약 PASS · dataBinding PASS.

## 구현

- `Assets/_Project/Scripts/UI/ShopLayoutContract.cs` — Unity 비의존 13슬롯 계약.
- `Assets/_Project/Scripts/UI/ShopLayout.cs` — Unity anchor/token 어댑터.
- `Assets/_Project/Scripts/UI/ShopUiController.cs` — 구매 5행, 행별 비활성 사유, 나가기 버튼을 생성하는 전용 모듈.
- `Assets/_Project/Scripts/UI/PrototypeHud.cs` — `Shop UI Module`을 `AddComponent`하여 shop 선택지를 모듈에 배선. 기존 상인 spotlight/card/icon 경로는 상점 모듈이 보일 때 사용하지 않는다.
- `Tools/DumpLayout/` — `--screen shop` contract dump 지원.
- `PortraitUiScreenshotQaTests` — shop runtime actual writer와 1·3·4·5층 캡처 검증.
- `PresentationLayerTests`/`PrototypeRoomSmokeTests` — 새 5행 모듈의 실제 버튼·행별 사유를 검증하도록 이관.

### 데이터 경계

- 가격은 `TextKey`/`HintText` 문구를 그대로 표시하며, 숫자를 파싱하지 않는다.
- 선택지별 아이콘·가격 전용 필드·상인과 선택지의 연결은 만들지 않았다.
- `Enabled=false` 구매 행은 숨기지 않고 비활성화하며, `ReasonTextKey`의 public 표현을 같은 행 안에 표시한다.

## 검증

| 층 | 캡처 | 구조 |
|---|---|---|
| 1 | `05_shop.png` | `ENC_SHOP_01` |
| 3 | `09_shop_floor3.png` | `ENC_SHOP_03` |
| 4 | `10_shop_floor4.png` | `ENC_SHOP_04` |
| 5 | `11_shop_floor5.png` | `ENC_SHOP_05` |

- `check_layout.py --spec shop_layout_spec.json`: 사양 **S-01~S-09 9/9 PASS**.
- `shop_actual_layout.json` contract 대조: **20/20 PASS**.
- `shop_runtime_actual_layout.json` runtime 대조: **20/20 PASS**.
- `check_wiring.py`: `ShopLayout` W-01 PASS · W-02 **13/13 = 100%**; 전체 5계약 PASS.
- `check_databinding.py`: shop 포함 전체 PASS.
- `LayoutWiringTests`: **1/1 pass**.
- `PresentationLayerTests`: **44/44 pass**.
- 명시적 screenshot QA: **1/1 pass**.
- 전량 EditMode: **204/212 pass, 기존 8 fail**.
- 전량 PlayMode: **14/21 pass, 기존 6 fail, explicit 1 SKIP**.
- CodeGraph: final sync 완료 (`Added 4, Modified 5`, 1,057 nodes).
- EditMode catalog bake가 바꾼 11개 encounter/item data asset은 PlayMode 전 정확히 원복했다. R4 gameplay data 변경 없음.

## 런타임 actual · 캡처

- 정적: `Docs/Portfolio/assets/concept-to-ui/shop_actual_layout.json` (`source: contract`)
  - SHA-256: `76d687a0982417796afc7cfe9137077ad2b0b7fabcd49a27240c43d68ddb3f54`
- 런타임: `Docs/Portfolio/assets/concept-to-ui/shop_runtime_actual_layout.json` (`source: runtime`)
  - SHA-256: `085d52f26b0f08795508da9f7516ae2fc81b76fa21f1f67c3baef05862d39985`
- PNG (모두 `Docs/Portfolio/assets/concept-to-ui/runtime-captures/`):
  - `05_shop.png` `43ee9a0978d399e70ceba5b602a1bd13cc56cf48655bb131dad64f40f847a1ac`
  - `09_shop_floor3.png` `5fb20ace5d3dc60dc32438349d83dc0aab639b302c9f7461f87fd8051ef698dd`
  - `10_shop_floor4.png` `71f6f43a5a23c0c08b9ea8a7045b738b6a431d02a8f36fe0a9c8a2ddd36461fb`
  - `11_shop_floor5.png` `3732c2bf15a187080b8cee34d6a526cb0297d3d81fd07f03c5539b49c36e1611`

## 제약 상태

- C-08: variants blocked이므로 SKIP 유지. 기본 상태를 대체 측정으로 쓰지 않았다.
- 사양과 런타임 actual 사이의 최종 슬롯 차이: 없음.
- `QUEUE.md` 5~8을 runtime 통과로 갱신했다.
- 다음 화면 9 이벤트는 사양 미작성이라 착수하지 않았다.

## 남은 실패 · 범위 밖 · 미수정

- EditMode 8: `RoomController_ExplorationContextResetsToNormalBgm`, final boss 2건, Floor 1 jar 2건, generic counterplay, item/ability combat loop, jar deterministic outcome.
- PlayMode 6: boss gate, jar event, final boss rest ending, rest node, route action, shop-before-boss progression.

Skill candidate: 없음. Sub-brain: read-only, Progress/Memory append 없음.
