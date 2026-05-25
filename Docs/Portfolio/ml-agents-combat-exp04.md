# ML-Agents Combat Experiment 04

## Scope

Experiment 04 is a combat-rule redesign probe after Experiment 03. It is not a 50k optimization run and does not connect an RL policy, ONNX model, or Mataios runtime policy to the shipped game. The goal is to test whether training-only combat rules can break simple spam policies and expose a better deterministic policy candidate.

Portfolio framing:

- Experiment 02 showed the ML training loop worked, but PPO discovered a Skill-heavy shortcut.
- Experiment 03 reduced Skill share and restored Defend prevention metrics, but SkillSpam and AttackSpam still beat PPO on scalar reward.
- Experiment 04 redesigns the training-only combat rules to make Skill context-sensitive and make Defend create offensive tempo.

The conclusion is not "AI finished the NPC." The useful result is an AI-assisted combat design loop: training exposed a combat-design shortcut, baselines confirmed it, and rule/reward changes produced a measurable comparison target.

## Rule Redesign

Training-only Exp04 rules are implemented in `Assets/_Project/Scripts/Training/CombatTrainingExp04Rules.cs`.

Key changes:

- Skill cooldown increased to 2 turns.
- Skill has a small use cost: `-0.04`.
- Skill against low enemy HP is treated as inefficient and receives `-0.25`.
- Enemy high-threat turns occur on a deterministic cadence.
- Defend reduces incoming damage and records prevented damage.
- Successful Defend grants a next-Attack tempo bonus.
- Successful Defend reward includes prevented damage plus a tempo reward.

Reward shaping:

- enemy damage: `+0.05` per HP
- player damage: `-0.035` per HP
- prevented damage: `+0.025` per HP
- successful Defend tempo: `+0.08`
- Skill use: `-0.04`
- wasted Skill: `-0.25`
- per decision step: `-0.01`
- enemy defeated: `+1.0`
- player defeated: `-1.0`
- timeout: `-0.25`

Observation changed from 8 scalar values to 11 scalar values for the training scene: player HP ratio, enemy HP ratio, normalized player attack, normalized enemy attack, last-action one-hot values, step ratio, Skill cooldown ratio, enemy high-threat flag, and tempo-attack-ready flag.

## PPO Run

```bash
source "/Users/godju/Downloads/AI Game/Model Training/mlagents-venv030/bin/activate"
mlagents-learn Config/MLAgents/training_combat_exp04_10k.yaml \
  --run-id hwigi_training_combat_exp04_10k \
  --results-dir "/private/tmp/hwigi-mlagents-results" \
  --env "/private/tmp/hwigi-mlagents-build/TrainingCombat.app" \
  --no-graphics \
  --force \
  --torch-device cpu
```

- Run id: `hwigi_training_combat_exp04_10k`
- Max steps: 10,000
- Actual final steps: 10,002
- Result path: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k`
- TensorBoard event: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat/events.out.tfevents.1779713294.ijuhyeong-ui-MacBookAir.local.83459.0`
- ONNX: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat.onnx`
- Checkpoint: `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat/checkpoint.pt`
- Final checkpoint reward: 1.8632564422411797

Curated outputs:

- `Docs/Portfolio/assets/mlagents_exp04_tensorboard_scalars.csv`
- `Docs/Portfolio/assets/mlagents_exp04_baseline_results.csv`
- `Docs/Portfolio/assets/mlagents_exp04_baseline_results.json`
- `Docs/Portfolio/assets/mlagents_exp04_policy_comparison.csv`

## PPO Scalar Summary

| Metric | Exp03 Final | Exp04 Final | Read |
| --- | ---: | ---: | --- |
| Cumulative Reward | 1.670228 | 1.855968 | Reward improved after rule redesign. |
| Episode Length | 2.263844 | 2.948617 | Episodes became slightly longer, which is expected because tempo matters now. |
| Win Proxy | 1.000000 | 0.988142 | Win rate remains high with a small loss tradeoff. |
| Attack Share | 0.634528 | 0.436213 | PPO moved away from attack-heavy play. |
| Defend Share | 0.056189 | 0.269650 | Defend became a meaningful part of the policy. |
| Skill Share | 0.309283 | 0.294137 | Skill stayed below the 30-50% target range by a small margin. |
| DamagePreventedPerStep | 0.245509 | 1.720721 | Defend prevention is now strongly visible. |
| SkillWastedPerEpisode | n/a | 0.280632 | PPO still wastes some Skill usage; this remains a tuning target. |
| DefendSuccessRate | n/a | 0.841897 | Most Defends are happening on useful turns. |
| TempoBonusTriggeredPerEpisode | n/a | 0.924901 | The Defend-to-Attack payoff is active. |
| ActionDiversity | n/a | 0.919631 | The learned policy is not a single-action policy. |

## Baseline Comparison

Baselines were evaluated over 1,000 deterministic episodes each with the same Exp04 training rules.

```bash
"/Applications/Unity/Hub/Editor/6000.4.3f1/Unity.app/Contents/MacOS/Unity" \
  -batchmode \
  -projectPath "/private/tmp/hwigi-mlagents-bootstrap" \
  -executeMethod HwigiTower.EditorTools.TrainingCombatExp04BaselineExporter.ExportCombatExp04Baselines \
  -logFile - \
  -quit
```

| Policy | Reward | Episode Length | Win | Attack | Defend | Skill | Prevented/Step | Skill Wasted | Tempo/Ep | Spam Gap | Read |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| PPOExp04_10k | 1.855968 | 2.948617 | 0.988142 | 0.436213 | 0.269650 | 0.294137 | 1.720721 | 0.280632 | 0.924901 | 0.318974 | PPO beats AttackSpam and SkillSpam, but trails the hand-authored context policy. |
| RandomPolicy | 1.406340 | 4.388000 | 0.807000 | 0.378077 | 0.387420 | 0.234503 | 1.988377 | 0.199000 | 0.728000 | n/a | High diversity, poor timing. |
| AttackSpamPolicy | 1.005000 | 3.408000 | 0.785000 | 1.000000 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | n/a | Punished by high-threat turns because it never Defends. |
| SkillSpamPolicy | 1.536994 | 3.000000 | 1.000000 | 0.666667 | 0.000000 | 0.333333 | 0.000000 | 0.000000 | 0.000000 | n/a | Still strong, but no longer dominates. |
| DefendHeavyPolicy | 1.373620 | 4.776000 | 0.776000 | 0.581240 | 0.418761 | 0.000000 | 1.403894 | 0.000000 | 1.776000 | n/a | Shows tempo works, but defense alone is too slow. |
| ContextPolicy | 2.101917 | 3.775000 | 1.000000 | 0.438940 | 0.296159 | 0.264901 | 2.397086 | 0.000000 | 1.118000 | n/a | Best current policy; good candidate for deterministic NPC combat logic. |

## Success Criteria Read

- PPO or ContextPolicy is clearly above SkillSpam/AttackSpam: yes. PPO reward is `1.855968` vs max spam reward `1.536994`; ContextPolicy reaches `2.101917`.
- Skill share is 30-50%: partial. PPO Skill share is `0.294137`, just below the lower bound. ContextPolicy Skill share is `0.264901`.
- Defend share meaningfully increased: yes. PPO Defend share is `0.269650`, up from Exp03 `0.056189`.
- `DamagePreventedPerStep` stays above zero: yes. PPO reaches `1.720721`, ContextPolicy reaches `2.397086`.
- SkillWasted decreases: not fully proven. Exp04 adds the metric, and PPO still records `0.280632` wasted Skills per episode. ContextPolicy records `0.0`.
- SpamPolicyGap turns positive: yes. PPO gap is `+0.318974`.

## Interpretation

Experiment 04 changed the comparison result. In Exp02 and Exp03, simple spam policies either matched or beat PPO. In Exp04, the same 10k PPO budget beats AttackSpam and SkillSpam, while the strengthened ContextPolicy beats all measured policies. That suggests the new rule shape creates a tactical target: defend on threat, convert prevention into tempo, and reserve Skill for context.

The most useful output for the game is not the ONNX file. The useful output is the deterministic ContextPolicy candidate and the evidence that the combat rules now punish pure spam. The next iteration should tune Skill waste detection and compare PPO against ContextPolicy after adding a slightly clearer Skill opportunity signal, rather than running a long 50k job immediately.

Runtime status: no shipped combat runtime, Mataios runtime policy, or ONNX runtime integration was changed for this experiment.
