using System;
using UnityEngine;

namespace HwigiTower.Core
{
    [Serializable]
    public readonly struct DeterministicRunContext
    {
        public DeterministicRunContext(string runId, int seed)
        {
            RunId = string.IsNullOrWhiteSpace(runId) ? "prototype-run" : runId;
            Seed = seed;
        }

        public string RunId { get; }
        public int Seed { get; }

        public DeterministicRandom CreateRandom()
        {
            return new DeterministicRandom(Seed);
        }
    }

    public sealed class DeterministicRandom
    {
        private readonly System.Random _random;

        public DeterministicRandom(int seed)
        {
            _random = new System.Random(seed);
        }

        public int Range(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxInclusive)
        {
            return Mathf.Lerp(minInclusive, maxInclusive, (float)_random.NextDouble());
        }
    }
}
