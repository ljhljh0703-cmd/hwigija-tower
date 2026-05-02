using HwigiTower.LLM;

namespace HwigiTower.NPC
{
    public interface INPCMemoryRepo
    {
        void SaveReflection(RunReflection reflection);
        bool TryGetReflection(string runId, out RunReflection reflection);
        RunReflection[] GetRecentReflections(int count);
        void SaveCachedResponse(LLMResponse response);
        bool TryGetCachedResponse(DeterministicCacheKey cacheKey, out LLMResponse response);
        void Clear();
    }
}
