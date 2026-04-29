using System.Collections.Generic;
using HwigiTower.LLM;

namespace HwigiTower.NPC
{
    public sealed class InMemoryNpcMemoryRepo : INPCMemoryRepo
    {
        private readonly Dictionary<string, RunReflection> _reflectionsByRunId = new Dictionary<string, RunReflection>();
        private readonly Dictionary<DeterministicCacheKey, LLMResponse> _responsesByCacheKey = new Dictionary<DeterministicCacheKey, LLMResponse>();

        public void SaveReflection(RunReflection reflection)
        {
            if (!reflection.IsValid)
            {
                return;
            }

            _reflectionsByRunId[reflection.RunId] = reflection;
        }

        public bool TryGetReflection(string runId, out RunReflection reflection)
        {
            return _reflectionsByRunId.TryGetValue(runId ?? string.Empty, out reflection);
        }

        public void SaveCachedResponse(LLMResponse response)
        {
            if (!response.CacheKey.IsValid)
            {
                return;
            }

            _responsesByCacheKey[response.CacheKey] = response;
        }

        public bool TryGetCachedResponse(DeterministicCacheKey cacheKey, out LLMResponse response)
        {
            return _responsesByCacheKey.TryGetValue(cacheKey, out response);
        }

        public void Clear()
        {
            _reflectionsByRunId.Clear();
            _responsesByCacheKey.Clear();
        }
    }
}
