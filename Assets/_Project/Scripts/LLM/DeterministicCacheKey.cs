using System;

namespace HwigiTower.LLM
{
    [Serializable]
    public readonly struct DeterministicCacheKey : IEquatable<DeterministicCacheKey>
    {
        public DeterministicCacheKey(string runId, string promptHash)
        {
            RunId = string.IsNullOrWhiteSpace(runId) ? string.Empty : runId;
            PromptHash = string.IsNullOrWhiteSpace(promptHash) ? string.Empty : promptHash;
        }

        public string RunId { get; }
        public string PromptHash { get; }
        public bool IsValid => !string.IsNullOrEmpty(RunId) && !string.IsNullOrEmpty(PromptHash);

        public bool Equals(DeterministicCacheKey other)
        {
            return string.Equals(RunId, other.RunId, StringComparison.Ordinal)
                && string.Equals(PromptHash, other.PromptHash, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is DeterministicCacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((RunId != null ? StringComparer.Ordinal.GetHashCode(RunId) : 0) * 397)
                    ^ (PromptHash != null ? StringComparer.Ordinal.GetHashCode(PromptHash) : 0);
            }
        }

        public override string ToString()
        {
            return $"{RunId}:{PromptHash}";
        }

        public static bool operator ==(DeterministicCacheKey left, DeterministicCacheKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DeterministicCacheKey left, DeterministicCacheKey right)
        {
            return !left.Equals(right);
        }
    }
}
