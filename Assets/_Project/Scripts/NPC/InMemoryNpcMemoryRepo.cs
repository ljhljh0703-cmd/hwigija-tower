using System.Collections.Generic;
using HwigiTower.LLM;

namespace HwigiTower.NPC
{
    public sealed class InMemoryNpcMemoryRepo : INPCMemoryRepo
    {
        private readonly Dictionary<string, RunReflection> _reflectionsByRunId = new Dictionary<string, RunReflection>();
        private readonly List<RunReflection> _reflectionsInOrder = new List<RunReflection>();
        private readonly Dictionary<DeterministicCacheKey, LLMResponse> _responsesByCacheKey = new Dictionary<DeterministicCacheKey, LLMResponse>();

        public void SaveReflection(RunReflection reflection)
        {
            if (!reflection.IsValid)
            {
                return;
            }

            if (!_reflectionsByRunId.ContainsKey(reflection.RunId))
            {
                _reflectionsInOrder.Add(reflection);
            }

            _reflectionsByRunId[reflection.RunId] = reflection;
        }

        public bool TryGetReflection(string runId, out RunReflection reflection)
        {
            return _reflectionsByRunId.TryGetValue(runId ?? string.Empty, out reflection);
        }

        public RunReflection[] GetRecentReflections(int count)
        {
            if (count <= 0 || _reflectionsInOrder.Count == 0)
            {
                return new RunReflection[0];
            }

            var length = System.Math.Min(count, _reflectionsInOrder.Count);
            var result = new RunReflection[length];
            var start = _reflectionsInOrder.Count - length;
            for (var i = 0; i < length; i++)
            {
                result[i] = _reflectionsInOrder[start + i];
            }

            return result;
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
            _reflectionsInOrder.Clear();
            _responsesByCacheKey.Clear();
        }
    }
}
