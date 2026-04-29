using System;

namespace HwigiTower.LLM
{
    [Serializable]
    public readonly struct LLMResponse
    {
        public LLMResponse(DeterministicCacheKey cacheKey, string text)
        {
            CacheKey = cacheKey;
            Text = text ?? string.Empty;
        }

        public DeterministicCacheKey CacheKey { get; }
        public string Text { get; }
    }
}
