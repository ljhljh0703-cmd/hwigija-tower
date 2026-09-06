using System;

namespace HwigiTower.NPC
{
    [Serializable]
    public readonly struct DialogueRequest
    {
        public DialogueRequest(string runId, NpcStage stage, string promptProfileId, string prompt)
        {
            RunId = string.IsNullOrWhiteSpace(runId) ? string.Empty : runId;
            Stage = stage;
            PromptProfileId = promptProfileId ?? string.Empty;
            Prompt = prompt ?? string.Empty;
        }

        public string RunId { get; }
        public NpcStage Stage { get; }
        public string PromptProfileId { get; }
        public string Prompt { get; }
    }
}
