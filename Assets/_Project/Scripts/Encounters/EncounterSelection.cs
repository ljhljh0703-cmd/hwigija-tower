namespace HwigiTower.Encounters
{
    public readonly struct EncounterSelection
    {
        public EncounterSelection(PrototypeNodeDefinition node, EncounterData encounter)
        {
            Node = node;
            Encounter = encounter;
        }

        public PrototypeNodeDefinition Node { get; }
        public EncounterData Encounter { get; }
        public bool HasEncounter => Encounter != null;
        public string EncounterId => Encounter == null ? string.Empty : Encounter.Id;
    }
}
