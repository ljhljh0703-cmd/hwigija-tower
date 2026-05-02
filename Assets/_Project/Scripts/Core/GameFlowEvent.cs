using System;

namespace HwigiTower.Core
{
    public readonly struct GameFlowEvent
    {
        public GameFlowEvent(GameFlowEventType type, string runId, string subjectId, string payloadId)
        {
            Type = type;
            RunId = string.IsNullOrWhiteSpace(runId) ? string.Empty : runId;
            SubjectId = string.IsNullOrWhiteSpace(subjectId) ? string.Empty : subjectId;
            PayloadId = string.IsNullOrWhiteSpace(payloadId) ? string.Empty : payloadId;
        }

        public GameFlowEventType Type { get; }
        public string RunId { get; }
        public string SubjectId { get; }
        public string PayloadId { get; }

        public bool Matches(GameFlowEventType type)
        {
            return Type == type;
        }
    }
}
