namespace HwigiTower.Tools.RoguelikeSim;

public static class FirstBuildSurfaceCatalog
{
    public static FirstBuildSurfaceScenario Create()
    {
        var route = new[]
        {
            new EnemySpec("ENEMY_SLIME_01", 1, 16, 3, 8, false),
            new EnemySpec("ENEMY_CRAWLER_02", 2, 24, 4, 10, false),
            new EnemySpec("ENEMY_SHADE_03", 3, 30, 5, 12, false),
            new EnemySpec("ENEMY_WRAITH_04", 4, 38, 6, 16, false),
            new EnemySpec("BOSS_GATE_01", 5, 50, 8, 40, true)
        };

        var offers = new IReadOnlyList<PickOption>[]
        {
            new[]
            {
                new PickOption("ABILITY_SWORD_01", PickKind.Ability, 10, "sword"),
                new PickOption("ITEM_01", PickKind.Item, 10),
                new PickOption("ITEM_02", PickKind.Item, 10)
            },
            new[]
            {
                new PickOption("ABILITY_SWORD_02", PickKind.Ability, 10, "sword"),
                new PickOption("ITEM_03", PickKind.Item, 10),
                new PickOption("ITEM_04", PickKind.Item, 10)
            },
            new[]
            {
                new PickOption("ABILITY_ARTS_03", PickKind.Ability, 10, "arts"),
                new PickOption("ITEM_05", PickKind.Item, 10),
                new PickOption("ITEM_09", PickKind.Item, 10)
            },
            new[]
            {
                new PickOption("ABILITY_SWORD_03", PickKind.Ability, 10, "sword"),
                new PickOption("ABILITY_GUARD_01", PickKind.Ability, 10, "guard"),
                new PickOption("ITEM_10", PickKind.Item, 10)
            }
        };

        return new FirstBuildSurfaceScenario(route, offers);
    }
}
