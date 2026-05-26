# 2026-05-27 Feedback Implementation Brief

Status: ready for session handoff.
Source log: `Docs/Feedback/2026-05-27/feedback-log.md`
Image assets: `Docs/Feedback/2026-05-27/assets/`

## Operating Rules

- Use latest accepted `origin/Proto` in a clean worktree. Do not use the dirty main worktree as a playtest or implementation baseline.
- Do not run screenshot harness. Use the user-provided feedback images in `Docs/Feedback/2026-05-27/assets/`.
- Report every item as `Accepted`, `In Progress`, or `Not Applied`.
- Implementation completion requires pushed commit + explicit playtest path.
- CodeGraph is required before touching core C# runtime paths (`PrototypeHud`, `PrototypeRunState`, `PrototypeFloorMap`, `PrototypeRoomController`, combat UI/state).
- Do not introduce RL/ONNX into runtime. This batch is Game/UI feedback, not AI training.

## Feedback Triage

### P0

1. Combat HP bars do not shrink proportionally to current HP.
   - Source: feedback items 7, 8.
   - Required: enemy, player, and Mataios HP bars must reflect current/max HP immediately after damage/heal.

2. Rest background disappears after some rest choice trigger.
   - Source: item 11.
   - Required: rest background remains visible/persistent unless an explicit later design says otherwise.

### P1

3. Add a pre-run screen before Floor 1.
   - Source: item 1.
   - Required: blank/placeholder-ready screen for memory inheritance, equipment selection, and short story cutscene.
   - Do not write final story/NPC copy.

4. Simplify top HUD.
   - Source: item 2.
   - Required top HUD values only: `Floor`, `HP`, `Gold`, `이성`.
   - Remove from top HUD: `정신`, `기억`, `능력`, `아이템`.
   - Enlarge top portrait to fit HUD scale.
   - Align icons and values consistently.

5. Simplify map header.
   - Source: item 3.
   - Required: only `갈림길 선택`.
   - Remove `지도`, `Floor N`, `보스까지 N번`, `밝은 노드`, `완료 N개` from the header surface.

6. Remove Mataios status block from map/preparation screen.
   - Source: item 4.
   - Required: no Mataios image/status text block on that screen unless the user later gives a concrete placement instruction.

7. Prevent map header/node overlap.
   - Source: item 5.
   - Required: map copy/header sits above node field and does not overlap nodes.

8. Improve map branching and node reveal rules.
   - Source: item 6.
   - Required:
     - Most nodes have one forward outgoing path.
     - Only occasional event/rest routing may split into two paths.
     - Previously visited nodes use the back/cleared-back visual.
     - Future nodes are mostly face-up so the player can plan.
     - Selectable nodes are lit/highlighted.
     - Future but not selectable nodes are face-up but dark/disabled.

9. Combat feedback and stat readability.
   - Source: items 7, 8, 9.
   - Required:
     - Enemy image shakes when enemy attacks or is hit.
     - Show/inspect enemy attack, defense, buffs, debuffs, and relevant status.
     - Reduce combat log clutter.
     - Provide inventory/owned item access in combat.
     - Attack button shows practical expected damage.
     - Defend button shows expected defense/prevented damage this turn.
     - Skill button shows usability, cooldown after use, and remaining cooldown when unavailable.

10. Rest UI position.
    - Source: item 10.
    - Required: move rest choice UI upward so it does not cover background character faces.

11. Map BGM reset.
    - Source: item 12.
    - Required: when returning to map, reset to normal/map BGM. Boss BGM must not continue into the next node-choice state.

### P2

12. Low HP danger feedback.
    - Source: item 12.
    - Required: consider red flash/low-HP danger visual. Keep this optional unless the critical P0/P1 batch is stable.

## Recommended Implementation Batches

### Batch 1: Critical State/Feedback Fixes

Goal: fix the issues that make playtest feedback unreliable.

Scope:
- HP bars proportional for enemy/player/Mataios.
- Rest background persistence after rest choices.
- Map BGM reset on map entry.
- Map header reduced to `갈림길 선택` and moved out of node field.
- Remove Mataios status block from map/preparation screen.

Validation:
- EditMode / PlayMode green.
- No screenshot harness.
- Manual QA should confirm using the saved feedback images and playtest path after push.

### Batch 2: HUD + Map Readability

Goal: make the main navigation surface readable.

Scope:
- Top HUD simplification and alignment.
- Portrait size adjustment.
- Map node reveal state: visited back, future face-up dark, selectable lit.
- Route generation adjustment: mostly single forward path, occasional two-way split only for event/rest routes.
- Rest first-row policy should remain consistent with previous map feedback.

Design note:
- Item 6 changes route/reveal behavior and may require D-034/map-flow spec update or confirmation before implementation.

### Batch 3: Combat Readability / Decision UI

Goal: make combat choices understandable and responsive.

Scope:
- Combat log density reduction.
- Enemy stat/status surface.
- Inventory access during combat.
- Attack/Defend/Skill preview labels.
- Skill cooldown/usability display.
- Enemy image shake on attack/hit.

Design note:
- Do not implement OQ-025 enemy intent deck in this batch.
- If defense/buff/debuff data does not exist yet, expose only current available stats and add placeholders carefully.

### Batch 4: Pre-Run Screen

Goal: prevent Floor 1 from starting directly on the current preparation/map-like screen.

Scope:
- Add a separate pre-run placeholder screen before Floor 1.
- It should be ready to receive user-provided images/assets later.
- It may contain placeholder slots for memory inheritance, equipment selection, and story cutscene.

Design note:
- Do not write final story text.
- Do not invent final equipment selection rules without PM/design confirmation.

## UI Session Handoff

Ask UI session to produce:
- Top HUD layout spec for `Floor / HP / Gold / 이성`.
- Map header placement spec with only `갈림길 선택`.
- Rest screen layout adjustment that avoids character faces.
- Combat action button preview layout.
- Enemy stat/status UI compact layout.
- Confirm whether pre-run screen is full-screen, modal, or stepper.

UI session must not implement code.

## System Design Handoff

Ask system/design session to decide or update:
- D-034 route/reveal adjustment:
  - Future nodes face-up but disabled.
  - Visited nodes use back/cleared-back visual.
  - Most nodes have one outgoing edge.
  - Two outgoing edges only occasionally for event/rest route choices.
- Pre-run screen contract:
  - memory inheritance slot
  - equipment slot
  - cutscene/art slot
  - placeholder allowed now
- Combat preview contract:
  - what damage/defense values can be previewed without lying to the player
  - skill cooldown display rules
  - item access scope in combat

System/design session must not introduce ITEM_07, OQ-025 enemy intent, final story text, or runtime RL.

## Development Handoff

Use this only after UI/system handoffs are reviewed.

Required:
- Clean worktree from latest `origin/Proto`.
- CodeGraph preflight for C# runtime/UI touched paths.
- No main dirty worktree.
- No screenshot harness.
- Use saved user images as visual references only.

Validation:
- `git diff --check`
- forbidden added-line search:
  - no runtime RL/ONNX
  - no DateTime.Now or seedless Random
  - no Glitch normal UI exposure
  - no new AI/Remote/training code in Game/UI batch
- GUI EditMode
- GUI PlayMode
- Report playtest path after push.

Completion report must include:
- local HEAD
- origin/Proto HEAD
- pushed yes/no
- accepted commit if pushed
- playtest path
- implemented feedback item numbers
- not-yet-applied item numbers
- changed files
- validation results
- dirty/untracked summary
- blockers

## QA Handoff

QA should verify by behavior, not screenshot harness:
- HP bars match current/max HP for enemy/player/Mataios.
- Rest background persists after rest choices.
- Map BGM resets after boss/combat/rest back to map.
- Top HUD only shows Floor/HP/Gold/이성.
- Map header only says `갈림길 선택`.
- Header does not overlap nodes.
- Mataios block is absent from the map/preparation screen.
- Future nodes are face-up dark; selectable nodes are lit; visited nodes are back/cleared.
- Combat buttons preview expected effect.
- Skill cooldown/usability is visible.
- Combat inventory access exists.
