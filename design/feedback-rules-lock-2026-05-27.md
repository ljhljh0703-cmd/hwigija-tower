# 2026-05-27 Feedback Rules Lock Spec

Status: D-034 addendum + D-036 locked 2026-05-27.
Scope: system rules required by the 2026-05-27 feedback pass. This is a design/development contract only; no runtime, Unity asset, item, intent deck, RL, ONNX, or story/dialogue implementation is included.

## Source Inputs

- `Docs/Feedback/2026-05-27/feedback-log.md`
- `Docs/Feedback/2026-05-27/feedback-implementation-brief.md`
- D-034 Map Flow / Route Commitment
- D-035 Mataios ContextPolicy Combat Brain
- OQ-019 and OQ-025 remain open

## Decision Summary

| Area | Locked Rule | D/OQ Impact |
|------|-------------|-------------|
| Pre-run screen | A pre-run placeholder must appear before Floor 1 map. Empty placeholder is valid now; memory inheritance, equipment selection, and short cutscene/image are reserved slots. | Folded into D-034. |
| Map route/reveal | Most nodes have one outgoing edge. Two-way forks are occasional and meaningful, preferably Event/Rest tradeoffs. Future node types are face-up but disabled until reachable. Cleared/visited nodes use back/cleared-back. | D-034 corrected and OQ-026 closed for first implementation. |
| Combat preview | Preview must come from resolver-owned deterministic preview data, not duplicated UI formulas. Attack shows expected resolver damage or range. Defend shows mitigation/reduction for the current round when known. Skill shows usability and cooldown state. | New D-036. |
| Enemy stat surface | Show only fields currently represented by data/runtime state: attack first, defense only if supported, buffs/debuffs/status only if state exists. Missing values are hidden or neutral placeholder, not newly invented mechanics. | New D-036. |
| Combat item access | Owned items may be inspectable during combat as read-only information. No in-combat item activation, no new item effect, and no `ITEM_07` implementation is added. | New D-036. |
| Audio/feedback | Map entry resets to normal/map BGM. Boss BGM cannot persist outside a boss encounter. Low HP red flash is P1 feedback, not P2 polish, because it communicates immediate survival risk. | New D-036. |

## Pillar Check

| Pillar | Fit |
|--------|-----|
| P1 | Pre-run and map route rules increase commitment pressure without adding free recovery. Low HP feedback clarifies risk; it does not add healing. |
| P2 | No runtime RL/ONNX, NPC state combat scaling, or final NPC/story text is introduced. |
| P3 | Sparse route, face-up disabled nodes, and compact previews keep 5-7 minute mobile decisions readable. |
| P4 | Route generation, preview data, and audio state transitions must be deterministic from run/combat state. |
| P5 | Lit/disabled node state, numeric combat previews, BGM reset, and danger feedback make consequences immediately legible. |

## A. Pre-run Screen Contract

| Topic | Contract |
|-------|----------|
| Entry point | Starting a new run enters a pre-run screen before Floor 1 map. |
| Current implementation content | Placeholder is allowed. It should not imply final feature content. |
| Reserved slots | Memory inheritance, equipment selection, short story cutscene/image. |
| Prohibited content | Final story/dialogue, equipment mechanics, memory inheritance effects, or new meta progression rules. |
| Exit | Player confirms start; then Floor 1 map is generated/visible under D-034 route rules. |

## B. Map Route / Reveal Contract

| Topic | Contract |
|-------|----------|
| Structure | Keep 5 visual columns and 3 logical sparse lanes. |
| Default outgoing edge | Most non-Shop/non-Boss nodes have exactly one outgoing edge. |
| Forks | Two outgoing edges appear only occasionally and should represent a meaningful branch such as Event vs Combat or Rest vs non-Rest. |
| All-to-all graph | Forbidden. Do not connect every node in one layer to every node in the next. |
| Visited state | Only previously visited/cleared nodes use card-back or cleared-back visuals. |
| Future node reveal | Future nodes reveal their type face-up. |
| Disabled future node | Future but not currently reachable nodes are dark/disabled and not clickable. |
| Active node | Currently reachable nodes are lit/active and clickable. |
| Commitment | Selecting a node immediately enters the encounter. No cancel or map return for alternate route selection during an active encounter. |

## C. Combat Preview Contract

| Preview | Contract |
|---------|----------|
| Source of truth | UI reads a resolver-owned non-mutating preview result such as `CombatActionPreview`. UI must not recalculate combat formulas independently. |
| Attack | Show expected damage from the same resolver path that will resolve the action. If variance exists, show a range. If only a deterministic baseline is available, label it as expected/base damage. |
| Defend | Show expected damage reduction/guard value for this turn when incoming damage/intent is known. If intent is unavailable, show the action's mitigation value only and avoid promising exact prevented damage. |
| Skill usable | Show whether the selected skill can be used now. |
| Skill cooldown after use | Show cooldown that will be applied after use. |
| Skill remaining cooldown | If unavailable, show remaining cooldown. |
| False precision guard | No preview number may be invented in UI. If resolver cannot produce a trustworthy value, the UI must show unavailable/unknown state instead of a guessed number. |

OQ-025 remains open. This contract does not implement enemy intent decks or enemy-specific payload numbers.

## D. Enemy Stat Surface

| Field | Contract |
|-------|----------|
| Attack | Show when enemy attack exists in data/runtime. |
| Defense | Show only if a defense field/effect exists and resolver uses it. |
| Buffs/debuffs/status | Show only active runtime states already represented by the combat system. |
| Missing field | Hide or show neutral placeholder. Do not create a new stat to fill the panel. |
| Inspect/readability | Enemy surface may be compact, but it must not claim unsupported stats. |

## E. Combat Item Inspect Scope

| Topic | Contract |
|-------|----------|
| Owned item access | The player may inspect currently owned items during combat if UI space allows. |
| Interaction | Read-only for first implementation. Do not add in-combat item use or activation. |
| Unsupported effects | Do not expose `ITEM_07` as implemented; OQ-019 remains open. |

## F. Audio / Feedback Contract

| Trigger | Contract |
|---------|----------|
| Map entry | Reset/play normal map BGM when entering map or node choice state. |
| Boss encounter | Boss BGM is scoped to boss encounter active state. |
| Boss clear/exit | Stop or replace boss BGM before showing route/map/next-node choice. |
| Low HP danger | Classify as P1 feedback. First implementation should use a data/config threshold; do not hard-code a hidden magic number in UI. |
| Hit/impact feedback | Combat impact feedback may be added as feedback triggers, but this document does not create new combat numbers. |

## Development Contract

| Work Item | Implement Now | Notes |
|-----------|---------------|-------|
| Pre-run placeholder | Yes | Blank/neutral placeholder only. No final story/dialogue. |
| D-034 route/reveal | Yes | Sparse lane route, occasional meaningful forks, disabled future node reveal, irreversible commitment. |
| Combat preview DTO/source | Yes | Resolver-owned non-mutating preview before UI display work. |
| Enemy stat display | Yes | Use existing attack/status data only. Hide unsupported fields. |
| Combat item inspect | Yes | Read-only owned item view only; no in-combat activation. |
| BGM state reset | Yes | Boss BGM must not leak back to map. |
| Low HP red flash | Yes, as P1 feedback | Threshold must be config/data-owned. |
| ITEM_07 | No | OQ-019 remains open. |
| Enemy intent deck payloads | No | OQ-025 remains open. |
| Runtime RL/ONNX | No | D-035 forbids it for main runtime. |

Before touching core C# runtime paths for combat or map, the development session must perform CodeGraph fresh status/sync/query/context per D-035/project process.

## QA Checklist

| Check | Expected |
|-------|----------|
| New run | Pre-run placeholder appears before Floor 1 map. |
| Placeholder content | No final story/dialogue, no equipment/memory mechanics. |
| Map density | Route reads as sparse lanes, not all-to-all. |
| Fork frequency | Most nodes have one outgoing edge; 2-way forks are occasional and meaningful. |
| Node reveal | Future node type is visible face-up. |
| Node disabled state | Unreachable future nodes are dark/disabled and cannot be clicked. |
| Node active state | Reachable nodes are lit/active and enter encounter immediately on click. |
| Commitment | During active encounter, map route switching/cancel is unavailable. |
| Attack preview | Displayed damage matches resolver result or approved preview range. |
| Defend preview | Displayed mitigation matches resolver preview; unknown incoming damage is not shown as exact. |
| Skill preview | Usability, cooldown after use, and remaining cooldown match runtime state. |
| Enemy stats | Unsupported defense/status fields are hidden/placeholder, not invented. |
| Map BGM | Map/node-choice state plays normal/map BGM. |
| Boss BGM | Boss BGM stops or is replaced after boss encounter exit. |
| Low HP feedback | Low HP visual appears at configured danger threshold and does not block input. |

## Blockers / Non-goals

- OQ-019 remains open for `ITEM_07`; no implementation instruction is included.
- OQ-025 remains open for enemy intent decks; no deck payload is included.
- This spec does not add final story/dialogue, runtime RL, ONNX inference, multi-enemy combat, or new combat balance numbers.
