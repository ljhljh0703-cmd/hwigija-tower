# Map Flow / Route Commitment Lock Spec

Status: D-034 locked 2026-05-27.
Scope: pre-run gate, map route generation, route reveal, route commitment, Rest timing, floor transition. No combat tuning.

## Conclusion

Current runtime already has partial commitment support: a selected map node blocks new selection until it resolves, and completed nodes skip same-layer siblings. The design gap is the route surface around it. `PathCount = 6` over `MapColumnCount = 5`, shared branch nodes, and all branch ends merging into Shop make the map read closer to dense all-to-all routing than a sparse roguelike route.

Locked target: keep the 5-column visual grid, but reinterpret it as a sparse 3-lane route with limited adjacent-lane branches, irreversible node commitment, face-up future node type reveal, and a pre-run placeholder before Floor 1 map entry.

## Pillar Check

| Pillar | Fit |
|--------|-----|
| P1 | Rest is delayed, so recovery is not the obvious early pick. Route choices can create pressure before relief. |
| P2 | No AI/runtime learning is introduced. |
| P3 | 3 branch layers + Shop + Boss keeps the floor compact for 5-7 minute mobile runs. |
| P4 | Seeded map generation and deterministic commitment preserve replayability. |
| P5 | Selecting a node immediately enters the encounter; skipped lanes make the consequence visible. |

## Pre-run Screen Contract

| Topic | Locked Rule |
|-------|-------------|
| New run start | Floor 1 must not start directly on the map or prep-map surface. |
| Pre-run screen | Show a full-screen pre-run placeholder before Floor 1 map generation/entry. |
| Current content | Empty placeholder is allowed. It may have only a neutral start/continue affordance. |
| Reserved slots | Memory inheritance, equipment selection, short story cutscene/image. |
| Forbidden now | No final story/dialogue, no equipment rule invention, no memory inheritance mechanics until separately locked. |
| Transition | Confirming the pre-run placeholder generates/shows Floor 1 map with D-034 route/reveal rules. |

## Route Structure Proposal

| Topic | Recommended Rule | Reason |
|-------|------------------|--------|
| Columns | Keep 5 visual columns. | Existing UI positioning already supports 5 columns; changing this is unnecessary churn. |
| Lanes | Use 3 logical lanes within the 5-column grid. | The player sees a route decision, not a dense web. |
| Branch layers | Keep 3 branch layers before Shop/Boss. | Preserves current floor length and boss approach pacing. |
| Path count | Replace 6 generated paths with 3 primary lane paths. | `PathCount = 6` is the main source of visual density. |
| Cross-lane branches | Allow 0-2 adjacent-lane branch edges per floor. No skip-layer edges. Two-way forks are occasional and should usually point to meaningful Event/Rest tradeoffs. | Keeps route choice alive without all-to-all readability loss. |
| Merge | Allow at most 1 branch merge before Shop; Shop remains the forced merge. | Merge can create interesting route convergence, but too many merges erase lane identity. |
| Boss route length | Require Layer 1 -> Layer 2 -> Layer 3 -> Shop -> Boss. | Keeps a minimum of 3 meaningful commitments before the floor gate. |
| Initial choice count | Start floor with 2-3 selectable Layer 1 nodes. | One node removes choice; five nodes reads like a menu, not a route. |
| Edge count | A non-Shop node has 1 outgoing edge by default, 2 only when it is an intentional fork. | Prevents accidental dense routes. |

## Route Reveal / Visual State Contract

| Node State | Visibility Rule | Interaction Rule |
|------------|-----------------|------------------|
| Cleared current route | Show as cleared/back state. | Not selectable. |
| Previously visited node | May show card back/cleared-back visual. | Not selectable unless a separate history view is added later. |
| Selectable next node | Show face-up node type and lit/active state. | Selectable; click immediately commits to encounter. |
| Future but not selectable node | Show face-up node type but dark/disabled. | Not selectable. |
| Skipped sibling route | Show locked/skipped state. | Not selectable. |

Only already-traversed nodes use the "back/cleared-back" visual language. Future nodes should not hide their type; their disabled state carries the uncertainty/commitment cost.

## Node Type Placement

| Rule | Recommendation |
|------|----------------|
| Layer 1 Rest | Forbidden. |
| Rest minimum timing | Rest can appear only after at least 1 completed node, therefore Layer 2 or Layer 3 only. |
| Rest frequency | First implementation target: 0-1 Rest candidate per floor. Floor 1 has 0 Rest candidates. |
| Rest lane coverage | Rest must not appear on every available lane in the same layer. |
| Boss-adjacent Rest | Do not place Rest immediately before Boss while Shop is fixed before Boss. Rest may appear in Layer 3 before Shop. |
| Shop | Fixed Layer 4, forced merge, one per floor when the floor path has Shop data. |
| Boss | Fixed Layer 5, one per floor. |
| Combat/Event mix | Layer 1 should prefer Combat/Event. Exact ratio remains a tuning parameter, not locked here. |

## Rest Timing Candidates

| Candidate | Rule | Recommendation |
|-----------|------|----------------|
| A. Cap-based delayed Rest | Floor 1 has 0 Rest candidate. Later floors have max 0-1 Rest candidate, placed only in Layer 2/3 by seed and available encounter data. | Recommended for first implementation because it blocks early recovery without pretending the exact probability is balanced. |
| B. Fixed Rest after Floor 1 | Floors 2+ always contain exactly 1 Rest candidate in Layer 2/3. | Not recommended as a default; recovery becomes too reliable and route pressure drops. |
| C. Ratio-only Rest | Keep the old Rest ratio and let it appear wherever generation chooses. | Reject; this is the current early-Rest problem in another form. |

D-034 locks Candidate A for first implementation. Exact weighted probability by floor remains balance tuning; development should implement a cap and placement guard, not a final economy balance.

## Node Commitment Rules

| State | Rule |
|-------|------|
| Floor map visible | Only currently reachable nodes are selectable. Other visible nodes are locked. |
| Node click | Selecting a node immediately commits and opens that encounter. |
| Cancel | No cancel after node click. |
| Map return during encounter | The utility map button must be hidden or disabled while an encounter is active. |
| Sibling routes | On encounter resolution, unchosen nodes in the same layer are marked skipped/locked. |
| Next selectable nodes | After resolution, only selected node outgoing edges become selectable. |
| Single outgoing edge | A forced next step may still require an explicit node click if the map is visible. Auto-opening should not create a hidden route change. |

## Floor Transition Rules

| Moment | Rule |
|--------|------|
| Boss clear | Result state can show floor clear and Next Floor. |
| Next Floor click | Clear previous result/encounter UI state before showing the new floor map. |
| New floor state | Generate/attach the new floor map immediately and show it if at least one node is selectable. |
| Failure state | If no map node is available, disable route progression and show an unavailable state. Do not leave the previous floor result as the active surface. |
| Persistence | Completed/skipped state must be floor-local. New floor starts with no selected/completed/skipped map nodes. |

## D/OQ Impact

| Item | Impact |
|------|--------|
| OQ-012 | Remains closed historically, but its "popup/no map" implementation detail is superseded by D-034. |
| D-034 | Locked decision: pre-run placeholder + sparse lane route + route reveal + irreversible node commitment. |
| OQ-026 | Closed by D-034 for first implementation. Rest exact probability remains balance tuning, not a route-system blocker. |

## Development Handoff: Route Generation Rules

1. Keep `MapColumnCount = 5` as the visual grid.
2. Insert a pre-run placeholder state before Floor 1 map entry.
3. Generate 3 logical primary lanes across branch layers 1-3.
4. Create at most one node per lane per branch layer.
5. Default each node to continue to the same lane on the next layer.
6. Add only adjacent-lane cross edges, max 0-2 total per floor, and prefer forks attached to meaningful Event/Rest tradeoffs.
7. Allow at most 1 pre-Shop branch merge per floor.
8. Connect all live Layer 3 branch ends to Shop.
9. Connect Shop to Boss.
10. Never connect every node in a layer to every node in the next layer.
11. Do not place Rest in Layer 1.
12. Place Rest only in Layer 2 or Layer 3, max 0-1 per floor in the first implementation target.
13. Generate with deterministic seed from run id + floor.
14. Preserve completed/skipped/selected state in `RunState`; route generation should not encode current progress.
15. On floor transition, reset floor-local map progress before showing the new floor map.
16. Future nodes show face-up node type; non-selectable future nodes are dark/disabled, not card-back hidden.

## QA Checklist

| Check | Expected |
|-------|----------|
| New run start | Pre-run placeholder appears before Floor 1 map. |
| New run floor map | After pre-run confirmation, 2-3 Layer 1 nodes selectable, not all nodes. |
| Visual density | Routes form sparse lanes; no all-to-all layer web. |
| Reveal state | Future node types are visible face-up; unavailable nodes are dark/disabled. |
| Back state | Only cleared/visited nodes use back/cleared-back visual. |
| Node click | Encounter opens immediately. |
| During encounter | Map/route utility cannot be used to cancel or switch node. |
| Encounter resolved | Chosen node completed; same-layer siblings skipped/locked. |
| Next route | Only outgoing edges from the completed node are selectable. |
| Rest timing | No Rest selectable at floor start. |
| Rest placement | Rest appears only Layer 2/3 and never immediately before Boss while Shop exists. |
| Determinism | Same run id + floor produces the same map. |
| Boss clear -> next floor | Previous result clears and the new floor map is visible/selectable. |
| Floor-local state | New floor does not inherit completed/skipped nodes from prior floor. |

## Deferred Balance Tuning

- Rest exact weighted probability by floor remains balance tuning.
- Single-outgoing-edge nodes require explicit tap for first implementation. Auto-open may be reconsidered only if map pacing feels too slow after route clarity improves.
