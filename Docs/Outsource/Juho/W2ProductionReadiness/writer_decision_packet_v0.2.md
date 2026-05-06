# Writer Decision Packet v0.2

## Scope

- Only decisions needed today for the W2 vertical slice.
- No final NPC dialogue is drafted here.
- No final memory prose is drafted here.
- Writer decisions should unblock UI/cutscene implementation without forcing final canon too early.

## Decisions Needed Today

| decision | recommended option | why today | implementation impact if approved |
|---|---|---|---|
| MEM_FRAGMENT_01 public visibility | Approve intent-only public visibility, but do not show raw title/body placeholder keys. | Memory unlock is in the default route; public recording needs either approved text or hidden placeholder handling. | Dev implements `showRawMemoryKeys = false` and optional approved display key slot. |
| S3 fracture exposure | Do not expose S3 fracture in the default 1-minute W2 route. Keep only optional audio/visual risk accent if writer approves. | S3 too early can read as model bug before attachment is established. | Dev keeps S3 cutscene/audio out of default route; AI eval still tests S3 separately. |
| Combat defeat exposure | Hide defeat from public recording. Keep defeat for QA/regression only. | The W2 slice should prove route coherence and companion/combat loop first. | Dev does not add defeat cutscene to public runbook; test plan can include hidden path later. |
| Mataios portrait crop approval | Approve current full-body art as temporary S0-S2, request cropped transparent portrait derivative for recording polish. | Current portrait is integrated but small and background-boxed in screenshots. | Outsource creates `Character_Mataios_S0S2_Portrait_Crop.png`; dev swaps HUD slot after QA. |
| Demo textKey policy | Hide placeholder/raw keys by default; use writer-approved keys only when supplied. | Current screenshots expose `PLACEHOLDER_*` and stableIds; public clip needs cleaner surface. | Dev adds raw key display toggle default off; writer can later fill approved display strings. |

## Decision Form

| item | approve? | notes |
|---|---|---|
| MEM_FRAGMENT_01 may appear in public W2 recording as intent-only / approved display, not raw key |  |  |
| S3 fracture stays out of default W2 recording |  |  |
| Combat defeat stays out of default W2 recording |  |  |
| Current Mataios full-body art is accepted as temporary S0-S2 direction |  |  |
| Cropped transparent portrait derivative should be made for recording |  |  |
| Placeholder/raw textKeys hidden by default |  |  |

## What This Does Not Decide

- Final MEM_FRAGMENT_01 title/body prose.
- Final NPC reaction lines.
- Ending wording.
- S3/S4 final collapse dialogue.
- Whether future full game exposes defeat as narrative beat.

## P1-P5 Check

- P1: Keeps memory meaningful without prematurely locking final loss/reveal text.
- P2: Keeps fracture as a deliberate later signal, not accidental W2 noise.
- P3: Protects the short demo route from overload.
- P4: Uses stableId/textKey toggles for deterministic presentation.
- P5: Keeps immediate result visibility while hiding raw implementation details.
