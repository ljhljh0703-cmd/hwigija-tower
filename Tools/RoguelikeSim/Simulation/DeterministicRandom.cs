namespace HwigiTower.Tools.RoguelikeSim;

public sealed class DeterministicRandom
{
    private readonly Random _random;

    public DeterministicRandom(int seed)
    {
        _random = new Random(seed);
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        return _random.Next(minInclusive, maxExclusive);
    }

    public int PickIndex<T>(IReadOnlyList<T> values)
    {
        return values.Count == 0 ? -1 : Range(0, values.Count);
    }
}

public static class DeterministicSeed
{
    public static int Combine(int seed, string key)
    {
        unchecked
        {
            var hash = 2166136261u;
            hash = (hash ^ (uint)seed) * 16777619u;

            foreach (var ch in key ?? string.Empty)
            {
                hash = (hash ^ ch) * 16777619u;
            }

            return (int)(hash & 0x7fffffff);
        }
    }
}
