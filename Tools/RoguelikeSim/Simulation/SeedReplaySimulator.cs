namespace HwigiTower.Tools.RoguelikeSim;

public sealed class SeedReplaySimulator
{
    private readonly FirstBuildSurfaceScenario _scenario;

    public SeedReplaySimulator(FirstBuildSurfaceScenario scenario)
    {
        _scenario = scenario;
    }

    public SeedReplayResult Run(int seed, IReplayPolicy policy)
    {
        var state = new SimRunState { Seed = seed };
        var pickRandom = new DeterministicRandom(DeterministicSeed.Combine(seed, policy.Name + ".picks"));

        for (var floorIndex = 0; floorIndex < _scenario.Route.Count; floorIndex++)
        {
            state.FloorReached = floorIndex + 1;
            if (floorIndex < _scenario.PickOffers.Count)
            {
                ApplyPick(state, policy.ChoosePick(state, _scenario.PickOffers[floorIndex], pickRandom));
            }

            var enemy = _scenario.Route[floorIndex];
            var won = ResolveCombat(state, enemy, policy, seed);
            if (!won)
            {
                if (enemy.IsBoss)
                {
                    state.BossResult = "loss";
                }

                return ToResult(state, policy.Name, "loss");
            }

            state.Gold += enemy.GoldReward;
            if (enemy.IsBoss)
            {
                state.BossResult = "win";
                return ToResult(state, policy.Name, "win");
            }
        }

        return ToResult(state, policy.Name, "win");
    }

    private static void ApplyPick(SimRunState state, PickOption? pick)
    {
        if (pick == null || pick.Cost > state.Gold)
        {
            return;
        }

        state.Gold -= pick.Cost;
        if (pick.Kind == PickKind.Ability)
        {
            state.Abilities.Add(pick.Id);
        }
        else
        {
            state.Items.Add(pick.Id);
        }

        RecalculateStats(state);
        EvaluateSynergies(state);
    }

    private static void RecalculateStats(SimRunState state)
    {
        var previousMaxHp = state.PlayerMaxHp;
        var maxHp = 24;
        var attack = 5;

        if (state.HasAbility("ABILITY_SWORD_01"))
        {
            attack += 1;
        }

        if (state.HasItem("ITEM_01"))
        {
            maxHp += 4;
        }

        if (state.HasItem("ITEM_02"))
        {
            attack += 1;
        }

        if (state.HasItem("ITEM_10"))
        {
            maxHp -= 3;
            attack += 2;
        }

        state.PlayerMaxHp = Math.Max(1, maxHp);
        state.PlayerAttack = Math.Max(0, attack);
        if (state.PlayerMaxHp > previousMaxHp)
        {
            state.PlayerHp += state.PlayerMaxHp - previousMaxHp;
        }

        state.PlayerHp = Math.Clamp(state.PlayerHp, 0, state.PlayerMaxHp);
    }

    private static void EvaluateSynergies(SimRunState state)
    {
        var swordCount = 0;
        if (state.HasAbility("ABILITY_SWORD_01")) swordCount++;
        if (state.HasAbility("ABILITY_SWORD_02")) swordCount++;
        if (state.HasAbility("ABILITY_SWORD_03")) swordCount++;

        if (swordCount >= 3)
        {
            state.Synergies.Add("FRENZY");
        }
    }

    private static bool ResolveCombat(SimRunState state, EnemySpec enemy, IReplayPolicy policy, int seed)
    {
        var random = new DeterministicRandom(DeterministicSeed.Combine(seed, policy.Name + ".combat." + enemy.Id));
        var enemyHp = enemy.MaxHp;
        var round = 0;
        var consecutiveAttacks = 0;
        var artsUsed = false;
        var firstHitBlocked = false;
        var swordOverloadUsed = false;

        while (state.PlayerHp > 0 && enemyHp > 0 && round < 30)
        {
            round++;
            if (state.HasItem("ITEM_05"))
            {
                enemyHp -= 1;
            }

            if (enemyHp <= 0)
            {
                break;
            }

            var action = policy.ChooseAction(
                state,
                new CombatView(enemy.Floor, enemy.Id, enemyHp, enemy.MaxHp, enemy.Attack, state.PlayerHp, state.PlayerMaxHp, state.PlayerAttack, round),
                random);

            var effectiveAction = action == CombatAction.Skill && (!state.HasAbility("ABILITY_ARTS_03") || artsUsed)
                ? CombatAction.Attack
                : action;

            var playerDamage = 0;
            if (effectiveAction == CombatAction.Skill && state.HasAbility("ABILITY_ARTS_03") && !artsUsed)
            {
                playerDamage = 8 + random.Range(0, 3);
                artsUsed = true;
                consecutiveAttacks = 0;
            }
            else if (effectiveAction == CombatAction.Attack)
            {
                playerDamage = state.PlayerAttack + random.Range(0, 3);
                if (state.HasItem("ITEM_03"))
                {
                    playerDamage += 2;
                }

                if (state.HasAbility("ABILITY_SWORD_02") && consecutiveAttacks >= 1)
                {
                    playerDamage += Math.Max(1, state.PlayerAttack / 2);
                }

                if (state.HasAbility("ABILITY_SWORD_03") && state.PlayerHp <= 3 && !swordOverloadUsed)
                {
                    playerDamage += 5;
                    swordOverloadUsed = true;
                }

                if (state.HasSynergy("FRENZY"))
                {
                    playerDamage += Math.Max(1, consecutiveAttacks >= 1 ? (int)Math.Round(state.PlayerAttack * 0.75f) : state.PlayerAttack / 2);
                }

                consecutiveAttacks++;
            }
            else
            {
                consecutiveAttacks = 0;
            }

            enemyHp -= playerDamage;
            if (enemyHp <= 0)
            {
                break;
            }

            var enemyDamage = enemy.Attack + random.Range(0, 3);
            if (effectiveAction == CombatAction.Defend)
            {
                enemyDamage /= 2;
                if (state.HasItem("ITEM_04"))
                {
                    enemyDamage = Math.Max(0, enemyDamage - 2);
                }

                if (state.HasAbility("ABILITY_GUARD_01"))
                {
                    enemyDamage = Math.Max(0, enemyDamage - 2);
                }
            }

            if (state.HasItem("ITEM_09") && !firstHitBlocked)
            {
                enemyDamage = Math.Max(0, enemyDamage - 3);
                firstHitBlocked = true;
            }

            state.PlayerHp = Math.Max(0, state.PlayerHp - enemyDamage);
        }

        state.TurnsToKill += round;
        return enemyHp <= 0 && state.PlayerHp > 0;
    }

    private static SeedReplayResult ToResult(SimRunState state, string policyName, string winLoss)
    {
        return new SeedReplayResult(
            state.Seed,
            policyName,
            winLoss,
            state.FloorReached,
            state.TurnsToKill,
            state.PlayerHp,
            state.Gold,
            string.Join("|", state.Items.OrderBy(id => id, StringComparer.Ordinal)),
            string.Join("|", state.Abilities.OrderBy(id => id, StringComparer.Ordinal)),
            string.Join("|", state.Synergies.OrderBy(id => id, StringComparer.Ordinal)),
            state.BossResult);
    }
}
