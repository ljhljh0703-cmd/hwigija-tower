# Demo QA Checklist v0.1

## Unity PlayMode QA

- [ ] PlayMode starts without console errors.
- [ ] Current branch/build contains commit `0357d75 Add deterministic demo run progression`.
- [ ] HUD shows HP, Mental, Gold, Glitch, and Affinity.
- [ ] Vertical mobile layout does not clip choice labels.
- [ ] Placeholder keys are either intentionally visible for debug or mapped to review copy.

## 1-Run Path QA

- [ ] First available node is ENC_SHOP_01.
- [ ] The path proceeds Shop -> MoralChoice -> MemoryFragment -> CombatGate -> DemoComplete.
- [ ] No unrelated random node interrupts the demo route.
- [ ] The route can be completed in about 60 seconds.
- [ ] DemoComplete appears after CombatGate result.

## Choice UI QA

- [ ] Each encounter shows 2-3 choices.
- [ ] Choice buttons cannot be double-submitted.
- [ ] Disabled choices remain readable.
- [ ] Gold-gated Shop choices show `PLACEHOLDER_REASON_NOT_ENOUGH_GOLD` when unavailable.
- [ ] Choice result appears quickly enough for a public demo.

## Result Panel QA

- [ ] Shop result panel shows Gold and item or ability changes.
- [ ] Moral result panel shows HP/Mental, Affinity, and Glitch changes.
- [ ] Memory result panel shows MEM_FRAGMENT_01 unlock or withdraw result.
- [ ] Combat result panel shows Victory or Defeat result values.
- [ ] Result panel close advances to the next demo node.

## Revisit Policy QA

- [ ] Shop purchase effects do not apply twice after returning to the node.
- [ ] MemoryFragment unlock does not duplicate MEM_FRAGMENT_01.
- [ ] Result panel reopen does not reapply effects.
- [ ] DemoComplete does not restart the route unexpectedly.
- [ ] Previously resolved choices are visually or logically stable.

## Memory Unlock QA

- [ ] `CHOICE_MEMORY_01_UNLOCK` unlocks `MEM_FRAGMENT_01`.
- [ ] Unlock applies Glitch +6 and Affinity +2.
- [ ] Withdraw applies Mental +1 and Glitch -1.
- [ ] Memory title/body are placeholder or writer-review copy only.
- [ ] No final memory body is presented as approved text.

## Combat Handoff QA

- [ ] `CHOICE_COMBAT_01_ENGAGE` starts `COMBAT_GATE_01`.
- [ ] `COMBAT_GATE_01` uses `ENEMY_FRACTURE_HOUND`.
- [ ] Combat handoff uses sourceEncounterId `ENC_COMBAT_GATE_01`.
- [ ] Combat handoff uses sourceChoiceId `CHOICE_COMBAT_01_ENGAGE`.
- [ ] Victory applies Gold +7, Glitch -2, Affinity +2.
- [ ] Defeat applies HP -5, Glitch +5, Affinity -2.
- [ ] Death/Regression, if encountered, is treated as optional hidden risk and not the default demo route.

## Android Build Candidate QA

- [ ] Android support is installed on the test machine.
- [ ] App launches in portrait orientation.
- [ ] Touch targets feel usable on a phone screen.
- [ ] Audio levels are not harsh on phone speakers.
- [ ] First run load time is acceptable for a live demo.
- [ ] No debug-only editor UI blocks the demo.

## Known Issues to Track

- [ ] Writer approval is still required for NPC reaction lines.
- [ ] OQ-006 memory final text is still open.
- [ ] Placeholder-to-baked-key alignment may need final pass.
- [ ] Result panel implementation may still be under validation.
- [ ] Revisit policy needs regression coverage.
- [ ] On-device AI may use cached or fallback response for demo.

## Bug Report Template

```md
## Bug Report

- Branch:
- Commit:
- Platform:
- Scene/build:
- Encounter stableId:
- Choice stableId:
- Steps to reproduce:
- Expected result:
- Actual result:
- Screenshot/video:
- Console/log excerpt:
- Severity: Blocker / High / Medium / Low
- Demo impact:
- Suggested owner: Dev / Writer / Art / QA
```
