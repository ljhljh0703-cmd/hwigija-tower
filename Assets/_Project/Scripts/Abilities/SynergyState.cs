namespace HwigiTower.Abilities
{
    public readonly struct SynergyState
    {
        public SynergyState(SynergyData synergy, bool active)
        {
            Synergy = synergy;
            Active = active;
        }

        public SynergyData Synergy { get; }
        public bool Active { get; }
    }
}
