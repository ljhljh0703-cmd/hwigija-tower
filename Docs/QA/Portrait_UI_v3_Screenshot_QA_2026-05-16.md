# Portrait UI v3 Screenshot QA — 2026-05-16

## Scope
- Scene/screens: Lobby, PrototypeRoom floor map, event, rest, shop, combat, boss combat, ending choice
- Viewport: 1080x1920 portrait
- Screenshot folder: `/private/tmp/hwigi-portrait-ui-v3-screenshots/`
- Harness: PlayMode QA screenshot test using real `LobbyController`, `PrototypeRoomController`, and `PrototypeHud` screens

## Results

| Screen | Screenshot | Status | Notes |
| --- | --- | --- | --- |
| Lobby | `/private/tmp/hwigi-portrait-ui-v3-screenshots/01_lobby.png` | pass | Menu renders in portrait, background visible, buttons centered. Text contrast is acceptable; typography still needs polish. |
| Floor Map | `/private/tmp/hwigi-portrait-ui-v3-screenshots/02_floor_map.png` | pass | Selectable nodes are brighter with larger icons/text, completed/locked nodes are dimmed, and the route hint shows boss distance and completed-node count without raw ids. |
| Event / Jar Room | `/private/tmp/hwigi-portrait-ui-v3-screenshots/03_event_jar_room.png` | pass | Event screen now uses a top status bar, central cutscene image panel, event text area, and large bottom choice buttons with probability hints. No raw stableId or Glitch visible in normal UI. |
| Rest / Mataios | `/private/tmp/hwigi-portrait-ui-v3-screenshots/04_rest_mataios.png` | pass | Mataios portrait, three rest actions with effect summaries, larger input field, response bubble, and submit button are visible. |
| Shop | `/private/tmp/hwigi-portrait-ui-v3-screenshots/05_shop.png` | pass | Merchant visual slot is visible, current Gold and boss-prep context are shown, and purchase cards expose item name, price/effect, insufficient-gold state, and leave option. |
| Normal Combat | `/private/tmp/hwigi-portrait-ui-v3-screenshots/06_normal_combat.png` | pass | Combat reads as top enemy stage, middle log, and bottom party dock. Player fallback portrait and Mataios portrait are both visible, HP bars are readable, and Attack/Defend/Skill now show icon + short label controls. |
| Boss Combat | `/private/tmp/hwigi-portrait-ui-v3-screenshots/07_boss_combat.png` | pass | Boss visual and boss HP stay in the top stage with no map/event/shop residue. Player fallback portrait, Mataios portrait, and icon action buttons remain anchored in the bottom party dock. |
| Ending Choice | `/private/tmp/hwigi-portrait-ui-v3-screenshots/08_ending_choice.png` | pass | Ending choice screen renders with Rest/Continue buttons and no extra node/combat controls. |

## Checks
- Portrait root stays inside 1080x1920 capture bounds.
- No landscape gutter covers the UI in generated captures.
- Main buttons are large enough for mobile touch.
- Main combat screens show enemy HP, player HP, and action buttons in one view.
- Glitch is not visible in normal mode screenshots.
- Raw stableIds are not visible in normal mode screenshots.
- Result panels do not block the active action buttons in the captured states.
- 2026-05-17 readability pass refreshed Floor Map, Shop, Top HUD, Rest input, and Combat text density using the same screenshot harness.
- 2026-05-17 merchant/rest/map pass connected floor merchant slots, strengthened map focus contrast, and separated rest input/response/effect areas.
- 2026-05-17 text density pass compressed result summaries to core lines, moved companion status beside the portrait, and limited combat feedback to enemy/player HP plus the latest action consequence.
- 2026-05-17 combat party layout pass split combat into top enemy stage, middle combat log, and bottom player/Mataios party dock with square action buttons.
- 2026-05-17 event cutscene layout pass split event presentation into top status, central cutscene art, event text, and bottom probability choices.
- 2026-05-17 combat party layout follow-up removed combat objective residue, expanded the combat panel into a clearer top/middle/bottom stack, and kept action controls as compact square buttons.
- 2026-05-17 combat icon binding pass added DemoPresentationData player portrait/action icon slots, uses a visible player fallback portrait when no portrait asset exists, and binds temporary Attack/Defend/Skill icons from existing node icon art.
- 2026-05-17 terminology pass replaced public memory-fragment wording with `기억의 잔향` while preserving internal `MEM_FRAGMENT_*` stableIds.

## Remaining UI Polish
- P1: improve font sharpness/readability in captured Game View output.
- P1: final content labels may need one more copyfit pass after writer-approved text lands.
- P1: event screen still needs final font/rendering polish after text asset approval.
- P1: replace temporary node-icon combat action bindings with dedicated Attack/Defend/Skill icons.
- P2: add final player portrait, merchant art variants, and stronger selected/focused animation states for combat/rest/map controls.
