using System;

namespace HwigiTower.LLM
{
    [Serializable]
    public readonly struct LLMRequest
    {
        public LLMRequest(string runId, string prompt, string promptProfileId)
        {
            RunId = string.IsNullOrWhiteSpace(runId) ? string.Empty : runId;
            Prompt = prompt ?? string.Empty;
            PromptProfileId = promptProfileId ?? string.Empty;
            CacheKey = new DeterministicCacheKey(RunId, PromptHash.FromPrompt(Prompt));
        }

        public string RunId { get; }
        public string Prompt { get; }
        public string PromptProfileId { get; }
        public DeterministicCacheKey CacheKey { get; }
    }
}
