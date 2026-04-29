using System;
using HwigiTower.LLM;

namespace HwigiTower.NPC
{
    [Serializable]
    public readonly struct DialogueResponse
    {
        public DialogueResponse(DeterministicCacheKey cacheKey, string text, bool fromCache)
        {
            CacheKey = cacheKey;
            Text = text ?? string.Empty;
            FromCache = fromCache;
        }

        public DeterministicCacheKey CacheKey { get; }
        public string Text { get; }
        public bool FromCache { get; }
    }
}
