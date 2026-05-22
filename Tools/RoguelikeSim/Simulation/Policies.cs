namespace HwigiTower.Tools.RoguelikeSim;

public interface IReplayPolicy
{
    string Name { get; }
    PickOption? ChoosePick(SimRunState state, IReadOnlyList<PickOption> offers, DeterministicRandom random);
    CombatAction ChooseAction(SimRunState state, CombatView combat, DeterministicRandom random);
}

public sealed class RandomPolicy : IReplayPolicy
{
    public string Name => nameof(RandomPolicy);

    public PickOption? ChoosePick(SimRunState state, IReadOnlyList<PickOption> offers, DeterministicRandom random)
    {
        var affordable = offers.Where(offer => offer.Cost <= state.Gold).ToArray();
        var index = random.PickIndex(affordable);
        return index < 0 ? null : affordable[index];
    }

    public CombatAction ChooseAction(SimRunState state, CombatView combat, DeterministicRandom random)
    {
        if (state.HasAbility("ABILITY_ARTS_03") && combat.EnemyHp >= 10 && random.Range(0, 3) == 0)
        {
            return CombatAction.Skill;
        }

        if (combat.PlayerHp * 100 / Math.Max(1, combat.PlayerMaxHp) <= 35 && random.Range(0, 2) == 0)
        {
            return CombatAction.Defend;
        }

        return CombatAction.Attack;
    }
}

public sealed class GreedyPolicy : IReplayPolicy
{
    public string Name => nameof(GreedyPolicy);

    public PickOption? ChoosePick(SimRunState state, IReadOnlyList<PickOption> offers, DeterministicRandom random)
    {
        return offers
            .Where(offer => offer.Cost <= state.Gold)
            .OrderByDescending(Score)
            .FirstOrDefault();
    }

    public CombatAction ChooseAction(SimRunState state, CombatView combat, DeterministicRandom random)
    {
        return state.HasAbility("ABILITY_ARTS_03") && combat.EnemyHp >= 12
            ? CombatAction.Skill
            : CombatAction.Attack;
    }

    private static int Score(PickOption option)
    {
        return option.Id switch
        {
            "ABILITY_SWORD_01" => 100,
            "ABILITY_SWORD_02" => 95,
            "ABILITY_SWORD_03" => 90,
            "ABILITY_ARTS_03" => 80,
            "ITEM_10" => 70,
            "ITEM_02" => 60,
            "ITEM_03" => 55,
            _ => 10
        };
    }
}

public sealed class SurvivalPolicy : IReplayPolicy
{
    public string Name => nameof(SurvivalPolicy);

    public PickOption? ChoosePick(SimRunState state, IReadOnlyList<PickOption> offers, DeterministicRandom random)
    {
        return offers
            .Where(offer => offer.Cost <= state.Gold)
            .OrderByDescending(Score)
            .FirstOrDefault();
    }

    public CombatAction ChooseAction(SimRunState state, CombatView combat, DeterministicRandom random)
    {
        var hpPercent = combat.PlayerHp * 100 / Math.Max(1, combat.PlayerMaxHp);
        if (hpPercent <= 45 && combat.Round % 3 != 0)
        {
            return CombatAction.Defend;
        }

        return state.HasAbility("ABILITY_ARTS_03") && combat.EnemyHp >= 16
            ? CombatAction.Skill
            : CombatAction.Attack;
    }

    private static int Score(PickOption option)
    {
        return option.Id switch
        {
            "ITEM_01" => 100,
            "ITEM_04" => 90,
            "ITEM_09" => 85,
            "ABILITY_GUARD_01" => 80,
            "ITEM_05" => 50,
            "ABILITY_ARTS_03" => 40,
            _ => 10
        };
    }
}

public static class PolicyRegistry
{
    public static IReadOnlyList<IReplayPolicy> Resolve(string policyName)
    {
        var policies = new IReplayPolicy[]
        {
            new RandomPolicy(),
            new GreedyPolicy(),
            new SurvivalPolicy()
        };

        if (string.IsNullOrWhiteSpace(policyName))
        {
            return policies;
        }

        var selected = policies.FirstOrDefault(policy => string.Equals(policy.Name, policyName, StringComparison.OrdinalIgnoreCase));
        if (selected == null)
        {
            throw new ArgumentException($"Unknown policy: {policyName}");
        }

        return new[] { selected };
    }
}
