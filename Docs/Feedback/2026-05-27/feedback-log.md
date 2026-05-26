# 2026-05-27 Feedback Log

Status: ready for aggregation. User said "피드백 마쳤어" on 2026-05-27.

## 1. Floor 1 Start / Pre-Run Screen

Image: `Docs/Feedback/2026-05-27/assets/feedback-01-start-screen.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_JfJHrx/스크린샷 2026-05-27 오전 12.00.12.png`

User feedback:
- Floor 1 should not start directly on the current preparation/map-like screen.
- Add a separate pre-run screen before Floor 1.
- The pre-run screen will later support:
  - memory inheritance
  - equipment selection
  - a short story cutscene
- It is acceptable for the implementation to leave this area blank for now, as long as it is ready for user-provided images/assets to be inserted later.

Triage status: pending aggregation.
Implementation status: not started.

## 2. Top HUD Simplification / Alignment

Image: `Docs/Feedback/2026-05-27/assets/feedback-02-top-hud.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_8KotH6/스크린샷 2026-05-27 오전 12.01.43.png`

User feedback:
- The top HUD should show only four values:
  - Floor
  - HP
  - Gold
  - 이성
- Remove these from the top HUD:
  - 정신
  - 기억
  - 능력
  - 아이템
- Increase the top portrait size so it fits the top UI better.
- Gold/item/etc. icons are placed inconsistently and should be aligned.

Triage status: pending aggregation.
Implementation status: not started.

## 3. Map Header Copy Simplification

Image: `Docs/Feedback/2026-05-27/assets/feedback-03-map-copy.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_rfVmc3/스크린샷 2026-05-27 오전 12.02.56.png`

User feedback:
- Remove the current verbose map header text.
- Leave only:
  - `갈림길 선택`
- Remove/collapse copy such as:
  - `지도`
  - `Floor 1`
  - `보스까지 5번`
  - `밝은 노드를 선택하세요`
  - `완료 0개`

Triage status: pending aggregation.
Implementation status: not started.

## 4. Remove Mataios Status Block From This Screen

Image: `Docs/Feedback/2026-05-27/assets/feedback-04-mataios-status-block.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_t4WIuk/스크린샷 2026-05-27 오전 12.03.43.png`

User feedback:
- Remove the highlighted/screenshoted UI block from this screen.
- The Mataios image and status text are not useful here.
- Do not place this Mataios image/status block unless the user later gives a concrete instruction for where it should appear.

Triage status: pending aggregation.
Implementation status: not started.

## 5. Prevent Map Header / Node Overlap

Image: `Docs/Feedback/2026-05-27/assets/feedback-05-map-header-overlap.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_5vXVYj/스크린샷 2026-05-27 오전 12.04.42.png`

User feedback:
- Map nodes and text UI must not overlap.
- Move this text/header UI upward.
- This overlaps with feedback item 3, but adds a layout requirement: the remaining map header/copy must sit above the node field, not on top of nodes.

Triage status: pending aggregation.
Implementation status: not started.

## 6. Map Branching / Node Reveal Rules

Image: `Docs/Feedback/2026-05-27/assets/feedback-06-map-branching-reveal.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_FxQI8k/스크린샷 2026-05-27 오전 12.05.10.png`

User feedback:
- Current branches still do not feel meaningful.
- Each path should mostly continue as a single forward route.
- Split into 2 outgoing paths only occasionally, such as when routing toward an event or rest node.
- Only previously visited nodes should show the card back/cleared-back style.
- Most future nodes should be face-up, so the player can read what kind of node they are planning toward.
- Selectable nodes should be highlighted/lit.
- Nodes that are not selectable yet should still show their face/type, but appear dark/disabled.

Triage status: pending aggregation.
Implementation status: not started.

## 7. Combat Enemy HP Bar / Hit Feedback / Stat UI

Image: `Docs/Feedback/2026-05-27/assets/feedback-07-combat-enemy-hp-feedback.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_0tGZ6w/스크린샷 2026-05-27 오전 12.06.54.png`

User feedback:
- In combat, HP bars must shrink proportionally to current HP.
- When the monster attacks or gets hit, the monster image should shake.
- Combat UI needs a way to inspect/show:
  - attack power
  - defense
  - buffs
  - debuffs
  - other relevant status effects

Triage status: pending aggregation.
Implementation status: not started.

## 8. Combat Log Density / Party HP Bar / Inventory Access

Image: `Docs/Feedback/2026-05-27/assets/feedback-08-combat-log-party-ui.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_Yi8x8d/스크린샷 2026-05-27 오전 12.07.39.png`

User feedback:
- Original user numbering repeated `7`; logged as item 8 to keep this file sequential.
- The combat log contains too much unnecessary information, including:
  - enemy name
  - remaining enemy HP
  - player HP
  - selected attack text
  - skill unavailable reason
- Character HP bars are not reflecting reduced HP.
- Combat needs a UI path to inspect/check owned items.

Triage status: pending aggregation.
Implementation status: not started.

## 9. Combat Action Preview / Skill Cooldown Display

Image: `Docs/Feedback/2026-05-27/assets/feedback-09-combat-action-preview.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_4gARzU/스크린샷 2026-05-27 오전 12.08.05.png`

User feedback:
- Original user numbering used `8`; logged as item 9 to keep this file sequential.
- The Attack action should show how much practical damage it will deal.
- The Defend action should show how much defense/damage prevention it will provide this turn.
- The Skill action should show:
  - whether it is currently usable
  - its cooldown after use
  - remaining cooldown if unavailable

Triage status: pending aggregation.
Implementation status: not started.

## 10. Rest UI Position

Image: `Docs/Feedback/2026-05-27/assets/feedback-10-rest-ui-position.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_bHA12E/스크린샷 2026-05-27 오전 12.08.29.png`

User feedback:
- Original user numbering used `9`; logged as item 10 to keep this file sequential.
- Move the Rest UI upward.
- The Rest UI should not cover the faces of the background characters in the middle/lower area.

Triage status: pending aggregation.
Implementation status: not started.

## 11. Rest Background Missing After Choice Trigger

Image: `Docs/Feedback/2026-05-27/assets/feedback-11-rest-background-missing.png`

Source image:
`/var/folders/55/61sb0c3j4lg4ywxxs37_tlq00000gn/T/TemporaryItems/NSIRD_screencaptureui_A6RuUX/스크린샷 2026-05-27 오전 12.09.04.png`

User feedback:
- Original user numbering used `10`; logged as item 11 to keep this file sequential.
- After using a Rest choice, some trigger causes later Rest screens to appear without the background.
- The Rest background should remain visible/persistent after Rest choices unless explicitly changed by design.

Triage status: pending aggregation.
Implementation status: not started.

## 12. Map BGM Reset / Low HP Danger Feedback

User feedback:
- Original user numbering used `11`; logged as item 12 to keep this file sequential.
- When transitioning back to the map, background music should reset to the normal/map BGM.
- It feels wrong when boss BGM keeps playing until the next node choice after defeating a boss.
- Consider adding a red flash/low-HP danger visual when player health is low.
- The goal is to make the character's dangerous condition immediately readable.

Triage status: pending aggregation.
Implementation status: not started.
