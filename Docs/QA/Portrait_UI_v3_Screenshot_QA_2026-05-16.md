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
| Floor Map | `/private/tmp/hwigi-portrait-ui-v3-screenshots/02_floor_map.png` | minor polish | Node choices now render in a two-column map with selectable/completed/locked states visible. Floor/objective/boss distance copy is more player-facing; overall typography still needs final art-direction polish. |
| Event / Jar Room | `/private/tmp/hwigi-portrait-ui-v3-screenshots/03_event_jar_room.png` | pass | Probability hints are visible before selection. No raw stableId or Glitch visible in normal UI. |
| Rest / Mataios | `/private/tmp/hwigi-portrait-ui-v3-screenshots/04_rest_mataios.png` | pass | Mataios portrait, three rest actions, input field, and submit button are visible. Input field needs stronger focus styling later. |
| Shop | `/private/tmp/hwigi-portrait-ui-v3-screenshots/05_shop.png` | minor polish | Shop context, current Gold, boss-prep purpose, purchase effects, insufficient-gold state, and leave option are visible. Merchant presentation remains placeholder-like until dedicated merchant art/state is added. |
| Normal Combat | `/private/tmp/hwigi-portrait-ui-v3-screenshots/06_normal_combat.png` | pass | Enemy visual, enemy HP, player HP, round/status text, and Attack/Defend/Skill buttons are visible. Combat log text can be sharpened later. |
| Boss Combat | `/private/tmp/hwigi-portrait-ui-v3-screenshots/07_boss_combat.png` | pass | Boss visual and boss HP are distinguishable from normal combat. Buttons remain reachable. |
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

## Remaining UI Polish
- P1: improve font sharpness/readability in captured Game View output.
- P1: replace placeholder-like shop merchant presentation with dedicated merchant visual/state.
- P1: keep reducing companion/result text density as final content labels arrive.
- P2: add stronger selected/focused visual states for rest input and map nodes.
