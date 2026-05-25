# AI-Assisted Combat Design Lab

## Purpose

This document freezes the portfolio evidence from ML-Agents Combat Experiments 02-04 and records the handoff from PPO research to a deterministic `ContextPolicy` candidate.

The important result is not that the shipped NPC is trained. The important result is that the training harness found a combat-design shortcut, measured it against deterministic baselines, and guided a rule redesign that made spam policies weaker.

Runtime boundary:

- No shipped combat runtime RL integration.
- No `CombatController` or `PrototypeRunState` changes.
- No Mataios runtime policy replacement.
- No ONNX connection to the game.
- ONNX/checkpoint/event raw artifacts remain outside the repo until PM approval.

## Experiment Narrative

| Experiment | Question | Result | Portfolio read |
| --- | --- | --- | --- |
| Exp02 | Does the ML-Agents loop produce useful 10k evidence? | Yes, but PPO shifted into Skill-heavy behavior. | Training loop works and exposed a SkillSpam shortcut. |
| Exp03 | Can Skill share be reduced and Defend measured? | Partially. Skill dropped and Defend prevention became visible, but spam baselines still beat PPO. | Reward/metric repair worked, combat rule design still weak. |
| Exp04 | Can rule/reward/context redesign break spam baselines? | Yes. PPO beat AttackSpam/SkillSpam, while ContextPolicy beat PPO. | The best immediate handoff is deterministic ContextPolicy, not ONNX/PPO runtime. |

## Exp04 Key Metrics

| Policy | Reward | Win | Attack | Defend | Skill | DamagePrevented/Step | SkillWasted/Ep | Tempo/Ep |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| PPOExp04_10k | 1.855968 | 0.988142 | 0.436213 | 0.269650 | 0.294137 | 1.720721 | 0.280632 | 0.924901 |
| AttackSpamPolicy | 1.005000 | 0.785000 | 1.000000 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.000000 |
| SkillSpamPolicy | 1.536994 | 1.000000 | 0.666667 | 0.000000 | 0.333333 | 0.000000 | 0.000000 | 0.000000 |
| ContextPolicy | 2.101917 | 1.000000 | 0.438940 | 0.296159 | 0.264901 | 2.397086 | 0.000000 | 1.118000 |

Exp04 changed the comparison result:

- PPO reward exceeded the strongest spam baseline by `+0.318974`.
- PPO used all three actions and raised Defend share to `0.269650`.
- ContextPolicy outperformed PPO by using the redesigned context more cleanly.
- SkillWasted remained a PPO tuning issue, while ContextPolicy recorded `0.0`.

## Why ContextPolicy Beats PPO

ContextPolicy has explicit access to the same rule intent that Exp04 was designed to test:

- Defend on high-threat turns.
- Spend the next attack after a successful Defend to convert prevention into tempo.
- Use Skill only when enemy HP is high enough to avoid waste.
- Fall back to Attack when Skill is unavailable or inefficient.

PPO learned the broad shape but still wastes some Skill usage and carries a small loss rate. With only 10k steps, PPO is useful evidence that the redesigned reward surface is learnable, but it is not the best production candidate. The safer handoff is a deterministic policy derived from ContextPolicy, with later PPO runs used as a design probe.

## ContextPolicy Handoff Candidate

Candidate decision order for a future Mataios deterministic combat policy:

```text
if tempo_attack_ready:
    Attack
else if enemy_threat_high or mataios_low_hp:
    Defend
else if skill_ready and enemy_hp >= skill_waste_threshold + margin:
    Skill
else:
    Attack
```

Training constants used by Exp04:

- `SkillCooldownTurns = 2`
- `SkillWasteHpThreshold = 8`
- `TempoAttackBonusDamage = 4`
- high-threat cadence: `stepIndex % 3 == 1`

Production caveat: these constants are experiment evidence, not final combat design. Moving them into shipped combat requires PM/design confirmation and should be implemented as deterministic rule logic, not by connecting the Exp04 ONNX.

## Evidence Freeze File Classification

The pre-freeze worktree had 31 changed files: 3 tracked modifications and 28 untracked files.

### Exp04 Core

- `Assets/_Project/Scripts/Training/CombatTrainingAgent.cs`
- `Assets/_Project/Scripts/Training/CombatTrainingExp04Rules.cs`
- `Assets/_Project/Scripts/Training/CombatTrainingExp04Rules.cs.meta`
- `Assets/_Project/Editor/TrainingCombatExp04BaselineExporter.cs`
- `Assets/_Project/Editor/TrainingCombatExp04BaselineExporter.cs.meta`
- `Assets/_Project/Editor/TrainingCombatSceneBuilder.cs`
- `Assets/_Project/Scenes/TrainingCombat.unity`
- `Config/MLAgents/training_combat_exp04_10k.yaml`
- `Docs/Portfolio/ml-agents-combat-exp04.md`
- `Docs/Portfolio/assets/mlagents_exp04_baseline_results.csv`
- `Docs/Portfolio/assets/mlagents_exp04_baseline_results.json`
- `Docs/Portfolio/assets/mlagents_exp04_policy_comparison.csv`
- `Docs/Portfolio/assets/mlagents_exp04_tensorboard_scalars.csv`

### Exp02/03 Document And Asset Backfill

- `Assets/_Project/Scripts/Training/CombatTrainingRules.cs`
- `Assets/_Project/Scripts/Training/CombatTrainingRules.cs.meta`
- `Assets/_Project/Editor/TrainingCombatBaselineExporter.cs`
- `Assets/_Project/Editor/TrainingCombatBaselineExporter.cs.meta`
- `Assets/_Project/Editor/TrainingCombatExp03BaselineExporter.cs`
- `Assets/_Project/Editor/TrainingCombatExp03BaselineExporter.cs.meta`
- `Config/MLAgents/training_combat_exp02_10k.yaml`
- `Config/MLAgents/training_combat_exp03_10k.yaml`
- `Docs/Portfolio/ml-agents-combat-exp02.md`
- `Docs/Portfolio/ml-agents-combat-exp03.md`
- `Docs/Portfolio/assets/mlagents_exp02_baseline_results.csv`
- `Docs/Portfolio/assets/mlagents_exp02_baseline_results.json`
- `Docs/Portfolio/assets/mlagents_exp02_policy_comparison.csv`
- `Docs/Portfolio/assets/mlagents_exp02_tensorboard_scalars.csv`
- `Docs/Portfolio/assets/mlagents_exp03_baseline_results.csv`
- `Docs/Portfolio/assets/mlagents_exp03_baseline_results.json`
- `Docs/Portfolio/assets/mlagents_exp03_policy_comparison.csv`
- `Docs/Portfolio/assets/mlagents_exp03_tensorboard_scalars.csv`

### Generated Summary Outputs Suitable For Commit

These are small curated summaries, not raw trainer artifacts:

- `Docs/Portfolio/assets/mlagents_exp02_*.csv`
- `Docs/Portfolio/assets/mlagents_exp02_*.json`
- `Docs/Portfolio/assets/mlagents_exp03_*.csv`
- `Docs/Portfolio/assets/mlagents_exp03_*.json`
- `Docs/Portfolio/assets/mlagents_exp04_*.csv`
- `Docs/Portfolio/assets/mlagents_exp04_*.json`

### Unity/Package/Cache Drift

No Unity/package/cache drift is intended for commit.

Observed but not repo-scoped:

- Unity rewrote `.vscode`, ProjectSettings, URP settings, and encounter assets during test/import cycles; those were restored.
- ML-Agents package cache sample `.meta` files were repaired under `Library/PackageCache` only to unblock local EditMode validation. `Library/` is not a commit candidate.

## Commit Candidate Scope

Recommended commit contents:

- Training source/config:
  - `Assets/_Project/Scripts/Training/CombatTrainingAgent.cs`
  - `Assets/_Project/Scripts/Training/CombatTrainingRules.cs`
  - `Assets/_Project/Scripts/Training/CombatTrainingExp04Rules.cs`
  - `Assets/_Project/Editor/TrainingCombatSceneBuilder.cs`
  - `Assets/_Project/Editor/TrainingCombatBaselineExporter.cs`
  - `Assets/_Project/Editor/TrainingCombatExp03BaselineExporter.cs`
  - `Assets/_Project/Editor/TrainingCombatExp04BaselineExporter.cs`
  - `Config/MLAgents/training_combat_exp02_10k.yaml`
  - `Config/MLAgents/training_combat_exp03_10k.yaml`
  - `Config/MLAgents/training_combat_exp04_10k.yaml`
- Training scene contract:
  - `Assets/_Project/Scenes/TrainingCombat.unity`
- Portfolio docs:
  - `Docs/Portfolio/ml-agents-combat-exp02.md`
  - `Docs/Portfolio/ml-agents-combat-exp03.md`
  - `Docs/Portfolio/ml-agents-combat-exp04.md`
  - `Docs/Portfolio/ai-assisted-combat-design-lab.md`
- Curated evidence:
  - `Docs/Portfolio/assets/mlagents_exp02_*`
  - `Docs/Portfolio/assets/mlagents_exp03_*`
  - `Docs/Portfolio/assets/mlagents_exp04_*`

## Excluded Generated Raw Outputs

Do not commit:

- `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat/events.out.tfevents.*`
- `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat/checkpoint.pt`
- `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat/TrainingCombat-10002.pt`
- `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat.onnx`
- `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/TrainingCombat/TrainingCombat-10002.onnx`
- `/private/tmp/hwigi-mlagents-results/hwigi_training_combat_exp04_10k/run_logs/`
- `/private/tmp/hwigi-mlagents-build/TrainingCombat.app`
- `Library/PackageCache/**`

ONNX is intentionally held outside the repo until PM confirms whether a small model export should be treated as portfolio evidence or generated build output.

## Next Experiment Recommendation

Do not run 50k yet.

Recommended next step:

- Slightly tune Skill opportunity/waste criteria.
- Keep Exp04 schema.
- Rerun 10k PPO and all deterministic baselines.
- Compare specifically against ContextPolicy, not only AttackSpam/SkillSpam.

Only consider 50k after a 10k rerun shows the reward surface is stable and PPO closes part of the gap to ContextPolicy without increasing SkillWasted.
