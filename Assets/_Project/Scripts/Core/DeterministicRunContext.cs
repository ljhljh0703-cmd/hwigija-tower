using System;
using System.Collections.Generic;
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

        public DeterministicRandom CreateRandom(string seedKey)
        {
            return new DeterministicRandom(DeterministicSeed.Combine(Seed, seedKey));
        }
    }

    public static class DeterministicSeed
    {
        public static int Combine(int seed, string seedKey)
        {
            unchecked
            {
                var hash = 2166136261u;
                hash = (hash ^ (uint)seed) * 16777619u;

                if (!string.IsNullOrEmpty(seedKey))
                {
                    for (var i = 0; i < seedKey.Length; i++)
                    {
                        hash = (hash ^ seedKey[i]) * 16777619u;
                    }
                }

                return (int)(hash & 0x7fffffff);
            }
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

        public float Next01()
        {
            return (float)_random.NextDouble();
        }

        public bool Chance(float probability)
        {
            return Next01() < Mathf.Clamp01(probability);
        }

        public int WeightedIndex(IReadOnlyList<int> weights)
        {
            if (weights == null || weights.Count == 0)
            {
                return -1;
            }

            var total = 0;
            for (var i = 0; i < weights.Count; i++)
            {
                total += Mathf.Max(0, weights[i]);
            }

            if (total <= 0)
            {
                return -1;
            }

            var roll = Range(0, total);
            var cumulative = 0;
            for (var i = 0; i < weights.Count; i++)
            {
                cumulative += Mathf.Max(0, weights[i]);
                if (roll < cumulative)
                {
                    return i;
                }
            }

            return weights.Count - 1;
        }
    }
}
