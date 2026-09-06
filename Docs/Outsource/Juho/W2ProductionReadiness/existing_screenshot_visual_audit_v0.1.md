# Existing Screenshot Visual Audit v0.1

## Scope

- Unity was not run in this session.
- Audit source is the existing five 1080 x 1920 PNG screenshots only.
- Screenshots:
  - `prototype_room_1080x1920_01_shop_portrait.png`
  - `prototype_room_1080x1920_02_memory_portrait_panel.png`
  - `prototype_room_1080x1920_03_combat_portrait_panel.png`
  - `prototype_room_1080x1920_04_combat_layering_fixed.png`
  - `prototype_room_1080x1920_05_demo_complete_after_combat.png`

## Findings

| item | severity | evidence screenshot | why it matters | recommended dev fix | must fix before recording |
|---|---:|---|---|---|---|
| Debug node label size | P0 | 01, 02, 03, 04, 05 | The large world-space Korean node labels dominate the lower half of the frame and make the capture read as a debug scene. | Hide world/debug node labels by default for demo recording; keep a dev toggle. | yes |
| Result text density | P1 | 02, 03, 05 | Result lines expose raw stableIds, deltas, and long key strings; viewers will read debug plumbing before they understand the route. | For recording mode, shorten result text to one or two player-facing lines, or hide raw stableId fragments behind a debug toggle. | yes for public recording; no for internal QA |
| Portrait readability | P2 | 01, 02, 03, 04, 05 | Mataios is recognizable, but the full-body source is small in the lower-left and has a light background rectangle that differs from the dark UI. | Keep current sprite for W2 QA; later create a cropped transparent portrait derivative if readability is weak on device. | no |
| Combat panel readability | P1 | 03, 04 | Screenshot 03 shows the old overlap with demo.complete/result text. Screenshot 04 shows the fixed active-combat state and is acceptable for QA. | Keep the 57077ee layering fix; add screenshot regression that active combat never shows demo.complete. | no after 04/05 pass |
| Memory panel readability | P1 | 02 | The memory status is readable but exposes placeholder title/body keys, which are not writer-approved final text. | Hide raw memory keys in recording mode, or replace only after writer OQ-006 approval. | yes for public recording |
| Route indicator readability | P2 | 01, 02, 03, 04, 05 | Route indicator is readable, but raw node/encounter IDs make the upper HUD feel like a dev overlay. | Keep route/status visible, but consider short display labels for recording mode. | no |
| DemoComplete readability | P2 | 05 | `demo.complete` reads clearly after the combat panel closes. It still sits within a debug-heavy frame. | Keep current layering; improve surrounding debug density first. | no |
| Center guide lines and placeholder shapes | P2 | 01-05 | Thin cross lines and simple colored placeholders are acceptable for a prototype, but they reduce presentation polish. | Leave for internal QA; hide or replace only if time remains after debug label cleanup. | no |

## Screenshot-by-Screenshot Notes

| screenshot | note |
|---|---|
| 01 Shop portrait | Portrait does not block HUD or route. The large world labels are the main issue. |
| 02 Memory portrait panel | Memory/result text is dense and placeholder-heavy; portrait still does not block interaction areas. |
| 03 Combat portrait panel | Captures the known pre-fix layering issue: demo.complete/result text appears over the active combat panel. Use as regression evidence only. |
| 04 Combat layering fixed | Preferred active-combat evidence. Combat panel is readable, Skill is disabled, demo.complete is not shown. |
| 05 Demo complete after combat | Preferred completion evidence. Combat panel is closed before demo.complete appears. |

## Recording Recommendation

Do not record a public-facing clip until the world/debug node labels are hidden by default. After that, the 57077ee combat layering state is acceptable for a W2 production-readiness capture, with memory/result raw key exposure called out as either internal-QA-only or writer-approved placeholder policy.
