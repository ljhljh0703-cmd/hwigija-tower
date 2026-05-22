namespace HwigiTower.Tools.RoguelikeSim;

public enum CombatAction
{
    Attack,
    Defend,
    Skill
}

public enum PickKind
{
    Ability,
    Item
}

public sealed record PickOption(string Id, PickKind Kind, int Cost, string Tag = "");

public sealed record EnemySpec(string Id, int Floor, int MaxHp, int Attack, int GoldReward, bool IsBoss);

public sealed record FirstBuildSurfaceScenario(
    IReadOnlyList<EnemySpec> Route,
    IReadOnlyList<IReadOnlyList<PickOption>> PickOffers);

public sealed class SimRunState
{
    public int Seed { get; init; }
    public int FloorReached { get; set; } = 1;
    public int PlayerMaxHp { get; set; } = 24;
    public int PlayerHp { get; set; } = 24;
    public int PlayerAttack { get; set; } = 5;
    public int Gold { get; set; } = 20;
    public int TurnsToKill { get; set; }
    public string BossResult { get; set; } = "not_reached";
    public HashSet<string> Abilities { get; } = new(StringComparer.Ordinal);
    public HashSet<string> Items { get; } = new(StringComparer.Ordinal);
    public HashSet<string> Synergies { get; } = new(StringComparer.Ordinal);

    public bool HasAbility(string id)
    {
        return Abilities.Contains(id);
    }

    public bool HasItem(string id)
    {
        return Items.Contains(id);
    }

    public bool HasSynergy(string id)
    {
        return Synergies.Contains(id);
    }
}

public sealed record CombatView(
    int Floor,
    string EnemyId,
    int EnemyHp,
    int EnemyMaxHp,
    int EnemyAttack,
    int PlayerHp,
    int PlayerMaxHp,
    int PlayerAttack,
    int Round);

public sealed record SeedReplayResult(
    int Seed,
    string Policy,
    string WinLoss,
    int FloorReached,
    int TurnsToKill,
    int RemainingHp,
    int Gold,
    string ItemPicks,
    string AbilityPicks,
    string SynergyCompletion,
    string BossResult);
