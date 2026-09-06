# Screen-by-Screen UI Spec v0.1

## Scope

- Target: W2 first demo vertical slice only.
- Screens: Shop, Moral, Memory, Combat, DemoComplete.
- Viewport: 1080 x 1920 portrait.
- Unity was not run for this spec. It is based on repo files, existing screenshots, and `PrototypeHud` layout code.
- This spec does not write final NPC dialogue, memory prose, or ethical-choice copy.

## Global Layout Rules

### 1080 x 1920 Frame

| region | pixel range | purpose | notes |
|---|---:|---|---|
| Top safe HUD | x 64-1016, y 40-300 | focus, encounter/status, run stats | Keep compact. No raw stableId in recording mode. |
| Route band | x 64-1016, y 300-520 | 5-step route indicator | Keep, but use short display labels. |
| Main stage | x 0-1080, y 520-1180 | world/background, portrait, cutscene focus, combat panel | Large debug node labels must be hidden. |
| Portrait slot | x 65-324, y 720-1200 current-ish | Mataios portrait | Current code anchors `0.06-0.30`, bottom offset 720, height 360. Crop polish recommended. |
| Combat panel slot | x 367-1015, y 728-1038 current | active combat controls | Current code anchors `0.34-0.94`, bottom offset 728, height 310. |
| Memory/status band | x 86-994, y 1180-1320 current-ish | memory and last combat summary | Must hide raw title/body keys in recording mode. |
| Result band | x 86-994, y 1330-1500 current-ish | one-line result summary | Keep to 1-2 lines for recording. |
| Choice zone | lower safe area | encounter choices | Buttons must not overlap portrait or result. |
| Bottom unsafe margin | bottom 48-96 px | no critical text | Mobile gesture/nav area risk. |

Coordinate note: code uses Unity UI anchors from bottom/center; pixel ranges above are recording targets for 1080 x 1920 review, not serialized coordinates.

### Global Keep/Hide Policy

| category | recording default | debug toggle |
|---|---|---|
| HUD route/status | show | show |
| HP/ATK/Mental/Gold/Glitch/Affinity | show, compact | show full |
| Mataios portrait | show | show |
| Encounter choices | show | show |
| Combat panel | show only while `IsInCombat` | show |
| DemoComplete overlay | show only after combat closes | show |
| World/debug node labels | hide | optional show |
| Raw stableIds | hide | optional show |
| Placeholder memory title/body keys | hide unless writer approves | optional show |
| Result effect internals `effects=`, `ignored=` | hide | optional show |

### Safe Area

- Keep top HUD text below y 40 and above y 300.
- Keep bottom choice buttons above y 1640? No: in Unity screen coordinates the lower region is visually near the bottom; the rule is to keep the lowest button at least 96 px above the bottom edge.
- Do not place critical copy under the portrait or inside the combat panel.
- Any full-screen cutscene overlay should reserve the top HUD and bottom choice zones unless the player is not expected to interact.

## Screen 1: Shop

### Current Evidence

- Encounter stableId: `ENC_SHOP_01`.
- Choices: `CHOICE_SHOP_01_BUY_ITEM`, `CHOICE_SHOP_01_BUY_ABILITY`, `CHOICE_SHOP_01_LEAVE`.
- Existing screenshot: `prototype_room_1080x1920_01_shop_portrait.png`.
- Current issue: world node labels dominate the lower half.

### Wireframe

```text
┌────────────────────────────────────────┐
│ Top HUD: run state, stats              │
│ Route: [current] Shop ...              │
│                                        │
│       dark room / shop background      │
│                                        │
│ Mataios portrait        shop focus     │
│                                        │
│ Memory/combat small status             │
│ Result: one-line last action           │
│                                        │
│ [Buy item]                             │
│ [Buy ability]                          │
│ [Leave]                                │
└────────────────────────────────────────┘
```

### Must Show

- Current route step: Shop.
- Player stats needed for purchase: Gold, HP, ATK.
- Three choice buttons.
- Disabled-visible reason if a purchase is unavailable.
- Mataios portrait lower-left.

### Must Hide

- Large world label `상점`.
- Raw encounter id `ENC_SHOP_01` in recording mode.
- Raw choice stableIds unless debug toggle is on.
- Internal `effects=` and `ignored=` result details.

### Button States

| button | default | disabled state |
|---|---|---|
| Buy item | enabled only if requirements pass | Disabled-visible with short reason, not raw key. |
| Buy ability | enabled only if requirements pass | Disabled-visible with short reason. |
| Leave | always enabled | n/a |

### Result Density

- Recording mode: one short result line, max 70 Korean chars or two compact lines.
- Debug mode: full stableId/effects can remain.
- No reward duplication in repeated/revisit resolution.

## Screen 2: Moral

### Current Evidence

- Encounter stableId: `ENC_MORAL_CHOICE_01`.
- Choices: `CHOICE_MORAL_01_AID`, `CHOICE_MORAL_01_REFUSE`, `CHOICE_MORAL_01_TRADE`.
- Route is part of battle node path in existing PlayMode smoke.

### Wireframe

```text
┌────────────────────────────────────────┐
│ Top HUD: stats with Affinity/Glitch    │
│ Route: Shop complete / Moral current   │
│                                        │
│     moral encounter background/prop    │
│                                        │
│ Mataios portrait   moral focus/cue     │
│                                        │
│ Result: immediate Affinity/Glitch cue  │
│                                        │
│ [Aid]                                  │
│ [Refuse]                               │
│ [Trade]                                │
└────────────────────────────────────────┘
```

### Must Show

- Choice buttons with writer-approved or placeholder-safe labels.
- Immediate result feedback: Affinity and Glitch deltas.
- Route progression to Memory after decision.
- Mataios portrait.

### Must Hide

- Raw moral stableIds.
- Ethical judgment text not approved by writer.
- Any final NPC line or morality explanation.

### Button States

| button | default | note |
|---|---|---|
| Aid | enabled unless requirements block | Should imply cost/benefit through UI, not moral scoring. |
| Refuse | enabled | Current test uses this path. |
| Trade | enabled only if requirements pass | Show short unavailable reason if blocked. |

### Result Density

- Show `Affinity -/+N` and `Glitch +N` as visible deltas.
- Avoid raw `choice applied: CHOICE_MORAL_...` in public recording.
- Keep result text under 2 lines.

## Screen 3: Memory

### Current Evidence

- Encounter stableId: `ENC_MEMORY_FRAGMENT_01`.
- Memory stableId: `MEM_FRAGMENT_01`.
- Choices: `CHOICE_MEMORY_01_UNLOCK`, `CHOICE_MEMORY_01_WITHDRAW`.
- Current memory asset keys: `PLACEHOLDER_MEM_FRAGMENT_01_TITLE`, `PLACEHOLDER_MEM_FRAGMENT_01_BODY`.
- Existing screenshot: `prototype_room_1080x1920_02_memory_portrait_panel.png`.

### Wireframe

```text
┌────────────────────────────────────────┐
│ Top HUD + route                        │
│                                        │
│     memory object / soft focus         │
│                                        │
│ Mataios portrait     memory panel      │
│                    [writer text area]  │
│                                        │
│ Memory status: 1 fragment unlocked     │
│ Result: unlock summary                 │
│                                        │
│ [Unlock]                               │
│ [Withdraw]                             │
└────────────────────────────────────────┘
```

### Must Show

- Memory object/cutscene focus if implemented.
- `Memory 1/5` or `Memory unlocked` style state.
- Route progress to CombatGate after unlock.
- Cutscene trigger if memory unlock cutscene exists.

### Must Hide

- Raw `PLACEHOLDER_MEM_FRAGMENT_01_TITLE/BODY` in recording mode.
- Final prose until writer approves OQ-006.
- Memory truth/ending reveal.

### Button States

| button | default | note |
|---|---|---|
| Unlock | enabled | Triggers `MEM_FRAGMENT_01` unlock and optional cutscene. |
| Withdraw | enabled | Should route safely if selected; not public default. |

### Result Density

- Recording mode: `Memory fragment unlocked` plus subtle stat delta if any.
- Debug mode: can show `MEM_FRAGMENT_01`, title/body keys, flags.
- Never display raw key line unless `showRawKeys` is enabled.

## Screen 4: Combat

### Current Evidence

- Encounter stableId: `ENC_COMBAT_GATE_01`.
- Enemy stableId: `ENEMY_FRACTURE_HOUND`, HP 12, ATK 3.
- Combat panel buttons: Attack, Defend, Skill.
- `Skill` is intentionally disabled.
- Existing screenshots: 03 pre-fix overlap, 04 fixed active combat.

### Wireframe

```text
┌────────────────────────────────────────┐
│ Top HUD: HP/ATK/Mental/Gold/Glitch     │
│ Route: CombatGate current/complete     │
│                                        │
│ Mataios portrait     Combat panel      │
│                    Enemy name + HP bar │
│                    Player HP bar       │
│                    Round result        │
│                    [Attack][Def][Skill]│
│                                        │
│ Memory status compact                  │
│ Result hidden while combat active      │
└────────────────────────────────────────┘
```

### Must Show

- Enemy identity or display label.
- Enemy HP current/max and preferably HP bar.
- Player HP current/max and preferably HP bar.
- Round number.
- Last action/result.
- Attack and Defend enabled while combat is active.
- Skill visible but disabled, with non-raw tooltip/reason if possible.

### Must Hide

- DemoComplete overlay while `IsInCombat`.
- Result text band while `IsInCombat`.
- Raw `COMBAT_GATE_01` and `ENEMY_FRACTURE_HOUND` in recording mode.
- `gold 0 | glitch 0 | affinity 0` if no change; show only nonzero deltas.

### Button States

| button | active combat | post-combat |
|---|---|---|
| Attack | enabled | hidden/disabled with panel closed |
| Defend | enabled | hidden/disabled with panel closed |
| Skill | disabled until ability routing exists | hidden/disabled with panel closed |

### Result Density

- Active combat should use combat panel fields, not bottom result text.
- Last action format: action + damage + HP delta.
- Victory transition should show one compact summary after panel closes.

## Screen 5: DemoComplete

### Current Evidence

- Completion state: `demo.complete`.
- Existing screenshot: `prototype_room_1080x1920_05_demo_complete_after_combat.png`.
- Code shows `Demo Complete Text` only when `DemoStatus == "demo.complete"` and `!IsInCombat`.

### Wireframe

```text
┌────────────────────────────────────────┐
│ Top HUD: final stats compact           │
│ Route: all complete                    │
│                                        │
│              DemoComplete              │
│            [approved textKey]          │
│                                        │
│ Mataios portrait                       │
│                                        │
│ Final result: victory summary          │
│                                        │
│ [Restart/End hidden unless implemented]│
└────────────────────────────────────────┘
```

### Must Show

- Completion overlay.
- Final route complete state.
- Final combat victory summary if compact.
- Mataios portrait unless cutscene full-screen treatment intentionally hides it.

### Must Hide

- Active combat panel.
- Debug node labels.
- Raw reward internals.
- Ending truth or final NPC dialogue.

### Button States

- No new button required for W2 recording.
- If Restart/End appears, it must be clearly out of current scope and not block screenshot capture.

### Result Density

- One compact final line: combat victory + essential deltas.
- Hide repeated `demo.complete` in both overlay and result if it reads redundant.

## Implementation Toggles Needed

| toggle | default | purpose |
|---|---:|---|
| `showDemoNodeDebugLabels` | false | Hide world labels. |
| `showRawStableIds` | false | Hide raw encounter/choice/combat IDs. |
| `showRawMemoryKeys` | false | Hide placeholder title/body keys. |
| `useRecordingResultSummary` | true | Collapse result text to player-facing summary. |
| `showPlaceholderTextKeys` | false | Hide placeholder keys unless writer approves. |

## Acceptance Criteria

- Each screen fits 1080 x 1920 without text overlap.
- Portrait does not cover route, stats, memory, choices, result, or combat panel.
- Combat panel and DemoComplete never appear together.
- Active combat hides bottom result text.
- Raw keys/stableIds are off by default in recording mode.
- Debug toggles restore raw visibility for developer QA.
