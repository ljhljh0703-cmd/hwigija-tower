using System.Text;

namespace HwigiTower.Tools.RoguelikeSim;

public static class SeedReplayCsv
{
    private static readonly string[] Header =
    {
        "seed",
        "policy",
        "win_loss",
        "floor_reached",
        "turns_to_kill",
        "remaining_hp",
        "gold",
        "item_picks",
        "ability_picks",
        "synergy_completion",
        "boss_result"
    };

    public static void Write(string path, IReadOnlyList<SeedReplayResult> rows)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine(string.Join(",", Header));
        foreach (var row in rows)
        {
            writer.WriteLine(string.Join(",", new[]
            {
                row.Seed.ToString(),
                Escape(row.Policy),
                Escape(row.WinLoss),
                row.FloorReached.ToString(),
                row.TurnsToKill.ToString(),
                row.RemainingHp.ToString(),
                row.Gold.ToString(),
                Escape(row.ItemPicks),
                Escape(row.AbilityPicks),
                Escape(row.SynergyCompletion),
                Escape(row.BossResult)
            }));
        }
    }

    private static string Escape(string value)
    {
        value ??= string.Empty;
        return value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\""
            : value;
    }
}
