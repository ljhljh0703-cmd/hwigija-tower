# ML-Agents Combat Experiment 03

## Scope

Experiment 03 follows Experiment 02's baseline comparison. The goal is to test whether a training-only Skill context gate and corrected Defend metric can reduce Skill spam and make defensive play visible in reward/metrics. It remains training/tooling only and does not connect any RL policy to shipped runtime combat.

## Changes From Experiment 02

- Added training-only `CombatTrainingRules` shared by the ML-Agents environment and Exp03 baseline exporter.
- Added a one-turn Skill cooldown through ML-Agents discrete action masking.
- Added a small Skill use penalty: `-0.03`.
- Added Defend prevented-damage measurement in the training rules.
- Added prevented-damage reward: `+0.02` per prevented HP.
- Preserved the 8-scalar observation contract by relying on last-action one-hot to imply a one-turn Skill cooldown.

Reward shaping in Experiment 03:

- enemy damage: `+0.05` per HP
- player damage: `-0.03` per HP
- prevented damage: `+0.02` per HP
- Skill use: `-0.03`
- per decision step: `-0.01`
- enemy defeated: `+1.0`
- player defeated: `-1.0`
- timeout: `-0.25`

## PPO Run

```bash
source "/Users/godju/Downloads/AI Game/Model Training/mlagents-venv030/bin/activate"
mlagents-learn Config/MLAgents/training_combat_exp03_10k.yaml \
  --run-id hwigi_training_combat_exp03_10k \
  --results-dir "/private/tmp/hwigi-mlagents-results" \
  --env "/private/tmp/hwigi-mlagents-build/TrainingCombat.app" \
  --no-graphics \
  --force \
  --torch-device cpu
```

- Run id: `hwigi_training_combat_exp03_10k`
- Max steps: 10,000
- Actual final steps: 10,003
- Result path: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp03_10k`
- TensorBoard event: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp03_10k/TrainingCombat/events.out.tfevents.1779712362.ijuhyeong-ui-MacBookAir.local.71093.0`
- ONNX: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp03_10k/TrainingCombat.onnx`
- Checkpoint: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp03_10k/TrainingCombat/checkpoint.pt`
- Final checkpoint reward: 1.6705983153775208

Curated outputs:

- `Docs/Portfolio/assets/mlagents_exp03_tensorboard_scalars.csv`
- `Docs/Portfolio/assets/mlagents_exp03_baseline_results.csv`
- `Docs/Portfolio/assets/mlagents_exp03_baseline_results.json`
- `Docs/Portfolio/assets/mlagents_exp03_policy_comparison.csv`

## PPO Scalar Summary

| Metric | Exp02 Final | Exp03 Final | Read |
| --- | ---: | ---: | --- |
| Cumulative Reward | 1.605229 | 1.670228 | Reward improved after shaping. |
| Episode Length | 2.055046 | 2.263844 | Episodes stayed short, with a small slowdown. |
| Win Proxy | 1.000000 | 1.000000 | Win rate did not regress. |
| Attack Share | 0.266310 | 0.634528 | PPO shifted away from Skill-heavy play toward Attack. |
| Defend Share | 0.013761 | 0.056189 | Defend usage recovered slightly, but remains low. |
| Skill Share | 0.719929 | 0.309283 | Skill spam was substantially reduced. |
| DamagePreventedPerStep | 0.000000 | 0.245509 | Defend metric is now visible and non-zero. |

## Baseline Comparison

Baselines were evaluated over 1,000 deterministic episodes each with the same Exp03 training rules.

```bash
"/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode \
  -projectPath "/private/tmp/hwigi-mlagents-bootstrap" \
  -executeMethod HwigiTower.EditorTools.TrainingCombatExp03BaselineExporter.ExportCombatExp03Baselines \
  -logFile - \
  -quit
```

| Policy | Reward | Episode Length | Win | Attack | Defend | Skill | Skill Blocked | Damage Prevented | Read |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| PPOExp03_10k | 1.670228 | 2.263844 | 1.000000 | 0.634528 | 0.056189 | 0.309283 | 0.000000 | 0.245509 | Skill share reduced, but reward still trails simple baselines. |
| RandomPolicy | 1.500531 | 4.563000 | 0.935000 | 0.380890 | 0.353715 | 0.265396 | 0.000000 | 1.175323 | Random gains prevention metric but remains slow/noisy. |
| AttackSpamPolicy | 1.687249 | 3.142000 | 1.000000 | 1.000000 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | Attack remains a strong simple baseline. |
| SkillSpamPolicy | 1.703083 | 3.000000 | 1.000000 | 0.333333 | 0.000000 | 0.666667 | 0.333333 | 0.000000 | Cooldown blocks every other Skill, but SkillSpam still has the best reward. |
| DefendHeavyPolicy | -0.834880 | 7.867000 | 0.004000 | 0.236812 | 0.762680 | 0.000508 | 0.000000 | 2.533622 | Defend metric works, but defense lacks offensive tempo. |
| ContextPolicy | 1.678110 | 3.000000 | 1.000000 | 0.666667 | 0.000000 | 0.333333 | 0.000000 | 0.000000 | Simple gated heuristic is slightly above PPO reward. |

## Analysis Questions

- Did PPO become meaningfully better than SkillSpam?
  - No. PPO improved over Experiment 02 and reduced Skill share, but `SkillSpamPolicy` still has higher reward (`1.703083` vs `1.670228`).
- Did Skill share drop?
  - Yes. PPO Skill share dropped from `0.719929` to `0.309283`.
- Did Defend share recover?
  - Partially. PPO Defend share increased from `0.013761` to `0.056189`, but this is still too low for a real defensive tactic.
- Did `DamagePreventedPerStep` leave zero?
  - Yes. PPO reached `0.245509`, Random reached `1.175323`, and DefendHeavy reached `2.533622`.
- Did win rate and episode length break?
  - No. PPO win stayed at `1.0`; episode length increased modestly from `2.055046` to `2.263844`.

## Reward Shaping Read

Experiment 03 fixed the measurement issue: Defend prevention is now visible. It also reduced PPO's Skill-heavy behavior. However, it did not solve the comparison problem because SkillSpam and AttackSpam still beat PPO on scalar reward.

The next change should not be a 50k run. The environment needs a stronger tactical reason to avoid pure damage races:

- Skill needs a harder context gate than one-turn cooldown, such as limited charges, higher cost, or reduced reward when Skill is not a finisher.
- Defend needs offensive tempo, not just survival reward. Options: Defend grants a next-attack bonus, counter window, or enemy telegraph where Defend prevents a high-damage turn.
- Enemy pressure should be added only with a paired Defend payoff. Increasing enemy damage alone will likely make non-damage policies worse.

Portfolio read: Experiment 03 is valuable because it shows a closed ML loop: baseline diagnosis, reward/context intervention, PPO rerun, scalar export, and honest comparison. The result is not "solved RL combat"; it is evidence that the training harness can surface reward-design failures and guide the next iteration.
