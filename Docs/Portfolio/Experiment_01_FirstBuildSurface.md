# Experiment 01 — First Build Surface Seed Replay

## Question

Can the First Build Surface be replayed deterministically enough to compare baseline policy behavior before any RL work begins?

## Baseline

Reference commit:

```text
ed30e9f97ee70c6e87dc21364c246edbb1cdd4e0 Implement blood oath overload
```

Current skeleton uses a hand-authored fixture that mirrors the First Build Surface at a coarse level and includes the Track A `SWORD_03` overload rule. It is not yet a source-of-truth export from Unity ScriptableObjects.

## Hypothesis

Scripted policies should expose visible differences in:

- floor reached
- boss result
- turns to kill
- remaining HP
- Gold and pick paths
- sword synergy completion

The result is useful if it shows replayable policy differences. It is not useful yet for exact balance tuning.

## Inputs

Policies:

- `RandomPolicy`
- `GreedyPolicy`
- `SurvivalPolicy`

Initial seed range:

```text
1000..1029
```

First CSV path:

```text
Tools/RoguelikeSim/output/seed_replay.csv
```

## Commands

Generate replay CSV:

```bash
dotnet run --project Tools/RoguelikeSim -- --seed-start 1000 --seed-count 30 --output Tools/RoguelikeSim/output/seed_replay.csv
```

Analyze replay CSV:

```bash
python3 Tools/RoguelikeSimPython/analyze_seed_replay.py --input Tools/RoguelikeSim/output/seed_replay.csv --output-dir Tools/RoguelikeSimPython/output
```

## Metrics

Required first schema:

```text
seed,policy,win_loss,floor_reached,turns_to_kill,remaining_hp,gold,item_picks,ability_picks,synergy_completion,boss_result
```

Primary reads:

- win rate by policy
- average floor reached by policy
- boss win rate by policy
- average turns to kill by policy
- synergy completion rate by policy

Secondary reads:

- average remaining HP
- average Gold
- common pick paths

## Guardrails

- Do not add RL code to Unity runtime.
- Do not add Unity packages.
- Do not start with DQN/PPO.
- Do not treat simulator output as an automatic GDD decision.
- Do not use 붕괴도, Affinity, or NPC state as combat modifiers.
- Do not implement unsupported `ITEM_07` pattern effects before OQ-019 is closed.

## Expected Output

The first run should create:

- `Tools/RoguelikeSim/output/seed_replay.csv`
- `Tools/RoguelikeSimPython/output/policy_summary.csv`
- `Tools/RoguelikeSimPython/output/policy_summary.md`
- `Tools/RoguelikeSimPython/output/floor_reached_histogram.txt`

Optional when local Python has `matplotlib`:

- `Tools/RoguelikeSimPython/output/floor_reached_histogram.png`

## Interpretation Rule

Experiment 01 can say:

```text
Policy A survived deeper than Policy B across these seeds.
```

Experiment 01 cannot say:

```text
The game balance is now locked.
```

Any design consequence must go through review, SSOT update protocol, and a separate Game Track implementation decision.
