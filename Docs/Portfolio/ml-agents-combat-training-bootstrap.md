# ML-Agents Combat Training Bootstrap

## Scope

This training track is isolated from the shipped runtime. It adds a small `TrainingCombat` environment for portfolio experiments and does not insert an RL policy into prototype combat, Mataios behavior, enemy intent, economy, or build-surface systems.

## Unity Environment

- Scene: `Assets/_Project/Scenes/TrainingCombat.unity`
- Agent script: `Assets/_Project/Scripts/Training/CombatTrainingAgent.cs`
- Behavior name: `TrainingCombat`
- Unity package: `com.unity.ml-agents` from `release_20`

## Python Environment

Expected local venv:

```bash
source "/Users/godju/Downloads/AI Game/Model Training/mlagents-venv030/bin/activate"
python -m pip show mlagents mlagents-envs torch protobuf
```

Expected package line:

```text
mlagents==0.30.0
mlagents-envs==0.30.0
torch==1.11.0
protobuf==3.20.3
```

## Observation / Action / Reward

Observations are eight scalar values:

- player HP ratio
- enemy HP ratio
- player attack normalized
- enemy attack normalized
- last action one-hot: Attack
- last action one-hot: Defend
- last action one-hot: Skill
- step ratio

Discrete actions:

- `0`: Attack
- `1`: Defend
- `2`: Skill

Rewards:

- enemy damage: `+0.05` per HP
- player damage: `-0.03` per HP
- useful Defend prevention: `+0.05`
- per decision step: `-0.01`
- enemy defeat: `+1.0`
- player defeat: `-1.0`
- timeout: `-0.25`

## Smoke Command

```bash
source "/Users/godju/Downloads/AI Game/Model Training/mlagents-venv030/bin/activate"
mlagents-learn Config/MLAgents/training_combat_ppo.yaml \
  --run-id hwigi_training_combat_smoke \
  --results-dir "/private/tmp/hwigi-mlagents-results" \
  --env "/private/tmp/hwigi-mlagents-build/TrainingCombat.app" \
  --no-graphics \
  --force
```

## Notes

- The scene is intentionally minimal and deterministic per episode seed.
- Runtime combat still uses the existing deterministic prototype controllers.
- This is not OQ-025 enemy intent work and does not implement `ITEM_07`.
