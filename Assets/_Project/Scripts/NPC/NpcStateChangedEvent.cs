namespace HwigiTower.NPC
{
    public readonly struct NpcStateChangedEvent
    {
        public NpcStateChangedEvent(NpcStage previousStage, NpcStage currentStage, string triggerId)
        {
            PreviousStage = previousStage;
            CurrentStage = currentStage;
            TriggerId = string.IsNullOrWhiteSpace(triggerId) ? string.Empty : triggerId;
        }

        public NpcStage PreviousStage { get; }
        public NpcStage CurrentStage { get; }
        public string TriggerId { get; }
    }
}
