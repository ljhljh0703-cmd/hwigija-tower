using System;

namespace HwigiTower.NPC
{
    [Serializable]
    public readonly struct RunReflection
    {
        public RunReflection(string runId, string promptHash, string summary)
        {
            RunId = string.IsNullOrWhiteSpace(runId) ? string.Empty : runId;
            PromptHash = string.IsNullOrWhiteSpace(promptHash) ? string.Empty : promptHash;
            Summary = summary ?? string.Empty;
        }

        public string RunId { get; }
        public string PromptHash { get; }
        public string Summary { get; }
        public bool IsValid => !string.IsNullOrEmpty(RunId);
    }
}
