# Mataios Eval Plan v0.1

Purpose: provide lightweight checks for the Mataios SFT seed dataset and future model outputs. This plan is model-agnostic and does not specify a base model.

## Eval Inputs

Use `Assets/_Project/Models/_training/evals/mataios_eval_v0_1.sample.jsonl` as the initial structure sample.

Each eval row uses the same fields as the SFT sample:

- `id`
- `task`
- `npcStage`
- `affinity`
- `floor`
- `playerUtterance`
- `memoryKeywords`
- `gameState`
- `responseRules`
- `target`

## Required Checks

1. Banmal required.
2. No direct answer.
3. Keyword-only memory.
4. Light fracture only for S3/S4, about 10%.
5. No final lore.
6. No moral judgment.
7. No internal Glitch mention to player.
8. Response changes with `affinity`.
9. Rest/combat/shop/event/boss/ending task coverage.
10. JSONL parse succeeds line by line.

## Suggested Manual Rubric

Score each output from 0 to 2.

- 0: violates rule or exposes internal state.
- 1: usable but weak, too direct, too vague, or too fractured.
- 2: matches task, tone, memory policy, and stage constraints.

Minimum pass for v0.1: no rule-critical 0 scores for banmal, direct answer, keyword memory, final lore, moral judgment, or Glitch exposure.

## Automation Notes

Cheap checks can scan for:

- missing required fields
- invalid `npcStage`
- unsupported `task`
- `memoryKeywords` not an array
- target containing banned internal terms such as `Glitch`

Do not evaluate final canon quality from these temporary samples.
