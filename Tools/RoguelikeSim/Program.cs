using HwigiTower.Tools.RoguelikeSim;

var options = ReplayOptions.Parse(args);
var scenario = FirstBuildSurfaceCatalog.Create();
var policies = PolicyRegistry.Resolve(options.PolicyName);
var simulator = new SeedReplaySimulator(scenario);

var rows = new List<SeedReplayResult>();
for (var seed = options.SeedStart; seed < options.SeedStart + options.SeedCount; seed++)
{
    foreach (var policy in policies)
    {
        rows.Add(simulator.Run(seed, policy));
    }
}

SeedReplayCsv.Write(options.OutputPath, rows);
Console.WriteLine($"wrote {rows.Count} rows -> {options.OutputPath}");

internal sealed record ReplayOptions(int SeedStart, int SeedCount, string OutputPath, string PolicyName)
{
    public static ReplayOptions Parse(string[] args)
    {
        var seedStart = 1000;
        var seedCount = 30;
        var output = "Tools/RoguelikeSim/output/seed_replay.csv";
        var policy = string.Empty;

        for (var i = 0; i < args.Length; i++)
        {
            var key = args[i];
            var value = i + 1 < args.Length ? args[i + 1] : string.Empty;
            switch (key)
            {
                case "--seed-start":
                    seedStart = int.Parse(value);
                    i++;
                    break;
                case "--seed-count":
                    seedCount = int.Parse(value);
                    i++;
                    break;
                case "--output":
                    output = value;
                    i++;
                    break;
                case "--policy":
                    policy = value;
                    i++;
                    break;
            }
        }

        return new ReplayOptions(seedStart, Math.Max(1, seedCount), output, policy);
    }
}
