# RoguelikeSim

Offline seed replay skeleton for the AI QA / balance lab track.

This project is intentionally outside Unity runtime:

- no Unity package changes
- no `Assets/_Project` runtime dependency
- no RL agent code in the game client
- no balance conclusion is treated as a GDD decision

Baseline reference: `ed30e9f97ee70c6e87dc21364c246edbb1cdd4e0 Implement blood oath overload`.

## Run

```bash
dotnet run --project Tools/RoguelikeSim -- --seed-start 1000 --seed-count 30 --output Tools/RoguelikeSim/output/seed_replay.csv
```

Optional single policy:

```bash
dotnet run --project Tools/RoguelikeSim -- --policy GreedyPolicy --seed-start 1000 --seed-count 10 --output Tools/RoguelikeSim/output/greedy.csv
```

## CSV Schema

Columns:

- `seed`
- `policy`
- `win_loss`
- `floor_reached`
- `turns_to_kill`
- `remaining_hp`
- `gold`
- `item_picks`
- `ability_picks`
- `synergy_completion`
- `boss_result`

The simulator currently uses a hand-authored First Build Surface snapshot aligned to the SWORD_03 overload baseline. A later pass can replace that with an exporter that reads Unity ScriptableObject data into a neutral JSON fixture.
