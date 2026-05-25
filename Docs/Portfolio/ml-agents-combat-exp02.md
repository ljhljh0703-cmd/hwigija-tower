# ML-Agents Combat Experiment 02

## Scope

Experiment 02 is the first meaningful PPO run after the 515-step bootstrap smoke. It remains training-only and does not connect an RL policy to shipped runtime combat.

## Baseline

- Base commit: `origin/Proto@271f0c65f96050f8de5712868eb4baaf1ff8d616`
- Training scene: `Assets/_Project/Scenes/TrainingCombat.unity`
- Agent: `Assets/_Project/Scripts/Training/CombatTrainingAgent.cs`
- Behavior: `TrainingCombat`
- Observation: 8 scalar values
- Actions: Attack / Defend / Skill

## Prior Milestone: 512-Step Smoke

The `hwigi_training_combat_smoke` run is an integration milestone only. It proves that the Unity executable and `mlagents-learn` trainer can connect, step the `TrainingCombat` behavior, write TensorBoard events, save checkpoints, and export ONNX. It should not be described as learning-quality evidence.

Smoke artifacts:

```text
/private/tmp/hwigi-mlagents-results/hwigi_training_combat_smoke/
|-- TrainingCombat.onnx
|-- TrainingCombat/TrainingCombat-515.onnx
|-- TrainingCombat/TrainingCombat-515.pt
|-- TrainingCombat/checkpoint.pt
`-- TrainingCombat/events.out.tfevents.1779706949.ijuhyeong-ui-MacBookAir.local.44033.0
```

## Run

```bash
source "/Users/godju/Downloads/AI Game/Model Training/mlagents-venv030/bin/activate"
mlagents-learn Config/MLAgents/training_combat_exp02_10k.yaml \
  --run-id hwigi_training_combat_exp02_10k \
  --results-dir "/private/tmp/hwigi-mlagents-results" \
  --env "/private/tmp/hwigi-mlagents-build/TrainingCombat.app" \
  --no-graphics \
  --force \
  --torch-device cpu
```

Trainer config excerpt:

```yaml
behaviors:
  TrainingCombat:
    trainer_type: ppo
    hyperparameters:
      batch_size: 64
      buffer_size: 1024
      learning_rate: 3.0e-4
    network_settings:
      hidden_units: 64
      num_layers: 2
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
    max_steps: 10000
    time_horizon: 32
    summary_freq: 1000
```

## Observation / Action / Reward Schema

Observation is eight scalar values: player HP ratio, enemy HP ratio, normalized player attack, normalized enemy attack, last-action one-hot values for Attack/Defend/Skill, and episode step ratio.

The action space is one discrete branch with three choices: `0` Attack, `1` Defend, `2` Skill.

Reward shaping in Experiment 02:

- enemy damage: `+0.05` per HP
- player damage: `-0.03` per HP
- useful Defend prevention: `+0.05`
- per decision step: `-0.01`
- enemy defeated: `+1.0`
- player defeated: `-1.0`
- timeout: `-0.25`

## Metrics To Read

Core trainer metrics:

- Environment/Cumulative Reward
- Environment/Episode Length
- Policy/Entropy
- Losses/Policy Loss
- Losses/Value Loss

Training-only custom metrics:

- HwigiTraining/Win
- HwigiTraining/Loss
- HwigiTraining/Timeout
- HwigiTraining/EpisodeLength
- HwigiTraining/ActionAttack
- HwigiTraining/ActionDefend
- HwigiTraining/ActionSkill
- HwigiTraining/EpisodeAttackShare
- HwigiTraining/EpisodeDefendShare
- HwigiTraining/EpisodeSkillShare
- HwigiTraining/EnemyDamagePerStep
- HwigiTraining/PlayerDamagePerStep
- HwigiTraining/DamagePreventedPerStep

Curated scalar export:

- `Docs/Portfolio/assets/mlagents_exp02_tensorboard_scalars.csv`
- `Docs/Portfolio/assets/mlagents_exp02_baseline_results.csv`
- `Docs/Portfolio/assets/mlagents_exp02_baseline_results.json`
- `Docs/Portfolio/assets/mlagents_exp02_policy_comparison.csv`

## Interpretation Rules

- The previous 515-step smoke proves integration only, not learning quality.
- A successful 10k run should produce TensorBoard events, checkpoint, ONNX, and readable reward/episode/action metrics.
- Attack-only convergence is a risk if `ActionAttack` or `EpisodeAttackShare` approaches 1.0 while defend/skill usage collapses.
- This run is comparable to Random/AttackSpam baselines by tracking win proxy, episode length, reward, and action share.

## Baseline Comparison

Baselines were evaluated by `TrainingCombatBaselineExporter` over 1,000 deterministic episodes per policy. This is not a new ML-Agents training run and does not touch shipped runtime combat. The evaluator reuses the same `CombatController` and Experiment 02 reward formula, then writes compact CSV/JSON evidence.

Baseline policy definitions:

- `RandomPolicy`: uniform random choice among Attack, Defend, and Skill with deterministic per-episode seed.
- `AttackSpamPolicy`: always Attack.
- `SkillSpamPolicy`: always Skill.
- `DefendHeavyPolicy`: mostly Defend, Attack every fourth decision, Skill only as a finisher.

Baseline command:

```bash
"/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode \
  -projectPath "/private/tmp/hwigi-mlagents-bootstrap" \
  -executeMethod HwigiTower.EditorTools.TrainingCombatBaselineExporter.ExportCombatBaselines \
  -logFile - \
  -quit
```

Comparison table:

| Policy | Source | Reward | Episode Length | Win Proxy | Attack | Defend | Skill | Read |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| PPO10k | TensorBoard final scalar | 1.605229 | 2.055046 | 1.000000 | 0.266310 | 0.013761 | 0.719929 | Learned policy is Skill-heavy and Defend collapsed. |
| RandomPolicy | baseline evaluator | 1.325538 | 4.429000 | 0.933000 | 0.335064 | 0.334161 | 0.330774 | Noisy but often wins because the duel is forgiving. |
| AttackSpamPolicy | baseline evaluator | 1.581029 | 3.149000 | 1.000000 | 1.000000 | 0.000000 | 0.000000 | Attack spam is already strong; PPO must beat it on speed/reward, not win rate. |
| SkillSpamPolicy | baseline evaluator | 1.609308 | 3.000000 | 1.000000 | 0.000000 | 0.000000 | 1.000000 | Skill spam slightly beats PPO reward, exposing weak Skill cost/context. |
| DefendHeavyPolicy | baseline evaluator | -1.139359 | 7.870000 | 0.003000 | 0.237230 | 0.762389 | 0.000381 | Defend extends time but lacks a scoring/survival payoff. |

Judgment:

- PPO is clearly better than Random and slightly better than AttackSpam by reward and episode length.
- PPO is not clearly distinguished from SkillSpam yet. Its final reward is slightly below SkillSpam, and its action distribution moved toward Skill rather than learning a more balanced tactical policy.
- Defend is not just underused by PPO; a Defend-heavy baseline also fails badly. The current environment does not pressure the player in a way that makes defense necessary, and the current reward formula records `DamagePreventedPerStep` as 0 because basic Defend halving is not surfaced as `PlayerDamagePrevented`.

Reward shaping proposal:

- Add a real Skill cost or context gate in the training environment before longer PPO runs. Options: cooldown observation, limited charges, HP/collapse cost, or reduced reward for repeated Skill when Attack would finish the enemy.
- Make Defend measurable before rewarding it. Either expose basic Defend damage reduction as prevented damage in the training metric path, or compute prevented damage inside the training wrapper from expected unblocked enemy damage.
- Increase enemy pressure only after Defend is measurable. Stronger enemy damage without a clean Defend reward may simply punish non-Skill policies harder.
- Next experiment should run the same four baselines after any reward/environment change, then run PPO only if SkillSpam no longer dominates the scalar target.

## Result Summary

- Run id: `hwigi_training_combat_exp02_10k`
- Run date: 2026-05-25 KST
- Max steps: 10,000
- Actual final steps: 10,001
- Result path: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp02_10k`
- TensorBoard event: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp02_10k/TrainingCombat/events.out.tfevents.1779709830.ijuhyeong-ui-MacBookAir.local.55070.0`
- ONNX: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp02_10k/TrainingCombat.onnx`
- Checkpoint: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp02_10k/TrainingCombat/checkpoint.pt`
- Final checkpoint reward: 1.6044000611305236

Experiment 02 artifacts:

```text
/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp02_10k/
|-- TrainingCombat.onnx
|-- TrainingCombat/TrainingCombat-10001.onnx
|-- TrainingCombat/TrainingCombat-10001.pt
|-- TrainingCombat/checkpoint.pt
|-- TrainingCombat/events.out.tfevents.1779709830.ijuhyeong-ui-MacBookAir.local.55070.0
|-- configuration.yaml
`-- run_logs/training_status.json
```

| Metric | Step 1000 | Step 10000 | Read |
| --- | ---: | ---: | --- |
| Environment/Cumulative Reward | 1.413884 | 1.605229 | Reward increased from the smoke-scale run, but this is still an early PPO experiment. |
| Environment/Episode Length | 3.462222 | 2.055046 | Episodes became shorter as wins became more direct. |
| Policy/Entropy | 1.098530 | 0.694268 | Policy became less random but did not collapse to a single action. |
| Losses/Policy Loss | 0.110101 at step 2000 | 0.085797 | Stable enough for a portfolio smoke-plus run. |
| Losses/Value Loss | 0.770925 at step 2000 | 0.002871 | Value estimate settled during this short run. |
| HwigiTraining/Win | 0.973333 | 1.000000 | Win proxy reached 1.0 by the final summary bucket. |
| HwigiTraining/EpisodeAttackShare | 0.361594 | 0.266310 | Attack-only convergence did not occur. |
| HwigiTraining/EpisodeDefendShare | 0.278049 | 0.013761 | Defend collapsed and needs reward shaping if defense should remain meaningful. |
| HwigiTraining/EpisodeSkillShare | 0.360356 | 0.719929 | Policy leaned heavily toward Skill by the final summary bucket. |

Action share trend:

| Step | Attack | Defend | Skill |
| ---: | ---: | ---: | ---: |
| 1000 | 0.361594 | 0.278049 | 0.360356 |
| 3000 | 0.388693 | 0.141487 | 0.469821 |
| 5000 | 0.332497 | 0.078094 | 0.589409 |
| 7000 | 0.323998 | 0.038608 | 0.637395 |
| 10000 | 0.266310 | 0.013761 | 0.719929 |

## Portfolio Read

Experiment 02 is usable as the first meaningful PPO evidence: it exports ONNX, produces TensorBoard metrics over 10k steps, and records action distribution metrics that can be compared against Random and AttackSpam heuristics. It should not be presented as solved combat AI. The main follow-up is reward shaping around Defend/Skill so the learned policy does not simply replace attack spam with skill spam.

Experiment 02 showed the training loop was functional and surfaced a reward-design issue: the learned policy avoided pure attack spam but shifted heavily toward Skill usage, while Defend collapsed. This makes reward shaping and baseline comparison the next design step.

The likely cause is that the current reward model gives direct value to fast enemy damage and only a small bonus for useful Defend. In this environment, Skill shortens episodes and increases damage throughput, while Defend is rarely necessary enough to survive or win. The next experiment should compare Random and AttackSpam baselines against this PPO run, then adjust reward shaping so Skill usage has cost/context and Defend has a clearer survival or tempo payoff.
