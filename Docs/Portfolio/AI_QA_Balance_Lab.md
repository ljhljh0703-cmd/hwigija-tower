# AI QA Balance Lab

## Purpose

`AI QA Balance Lab` is a separate portfolio and tooling track for deterministic roguelike balance experiments.

It supports the game track by producing replayable evidence from the First Build Surface, but it does not place RL, AI QA, or experiment-only logic inside Unity runtime.

Baseline commit:

```text
ed30e9f97ee70c6e87dc21364c246edbb1cdd4e0 Implement blood oath overload
```

## Track Boundary

In scope:

- read or mirror the First Build Surface into an offline simulator fixture
- replay deterministic seeds across scripted policies
- export CSV metrics for balance review
- summarize policy behavior with Python analysis
- use results as QA evidence and portfolio material

Out of scope:

- Unity runtime RL code
- Unity package additions
- DQN/PPO as the first step
- direct GDD balance locks from simulator output
- new combat, item, ability, or encounter rules

## First Skeleton

Created folders:

- `Tools/RoguelikeSim/`
- `Tools/RoguelikeSimPython/`
- `Docs/Portfolio/AI_QA_Balance_Lab.md`
- `Docs/Portfolio/Experiment_01_FirstBuildSurface.md`

## Policy Baselines

`RandomPolicy`

- randomly chooses affordable build picks
- mixes Attack, Defend, and Skill based on seeded randomness
- exists as the noise floor baseline

`GreedyPolicy`

- prioritizes attack and sword completion
- uses Skill only when it is an immediate damage advantage
- exists as the damage-race baseline

`SurvivalPolicy`

- prioritizes HP, mitigation, and defensive picks
- defends under HP pressure
- exists as the sustain baseline

## CSV Metric Schema

The first replay CSV uses:

```text
seed,policy,win_loss,floor_reached,turns_to_kill,remaining_hp,gold,item_picks,ability_picks,synergy_completion,boss_result
```

Interpretation:

- `seed`: deterministic replay seed
- `policy`: policy class name
- `win_loss`: run outcome
- `floor_reached`: deepest reached floor
- `turns_to_kill`: accumulated combat rounds
- `remaining_hp`: final player HP
- `gold`: final Gold
- `item_picks`: pipe-delimited item ids
- `ability_picks`: pipe-delimited ability ids
- `synergy_completion`: pipe-delimited completed synergy ids
- `boss_result`: `win`, `loss`, or `not_reached`

## P1-P5 Alignment

P1: The lab observes tradeoffs but does not erase loss. Survival and greedy baselines are compared by cost, not converted into a single optimal answer.

P2: LLM limitations are not hidden by this track. The lab does not generate NPC dialogue or narrative text.

P3: Experiments focus on short replayable runs and 12-run scale evidence, matching the target session rhythm.

P4: Every replay is keyed by explicit seed and policy. Results must be reproducible before they can be discussed.

P5: Results can trigger design review, but they cannot directly lock GDD choices without human decision and SSOT update protocol.

## Game Track Alignment

Track A baseline includes First Build Surface plus the `SWORD_03` overload update:

- `ABILITY_SWORD_03` is modeled as a once-per-combat overload when HP is low.
- The simulator applies the overload bonus as an attack payoff, not as a relationship, Affinity, or 붕괴도 modifier.
- `ITEM_07` remains excluded because OQ-019 is still open.

## Next Steps

1. Add a Unity Editor exporter that writes the stable First Build Surface to neutral JSON.
2. Replace hand-authored simulator fixture values with exported JSON.
3. Run `RandomPolicy`, `GreedyPolicy`, and `SurvivalPolicy` across at least 100 seeds.
4. Write experiment notes as evidence, not final balance decisions.
