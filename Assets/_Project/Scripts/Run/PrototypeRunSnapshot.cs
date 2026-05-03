namespace HwigiTower.Run
{
    public readonly struct PrototypeRunSnapshot
    {
        public PrototypeRunSnapshot(
            string runId,
            int playerHp,
            int playerMaxHp,
            int playerAttack,
            int mental,
            int gold,
            int glitchLevel,
            int affinity,
            int nodesResolved,
            int battlesWon,
            int abilityCount,
            bool runCompleted)
        {
            RunId = runId ?? string.Empty;
            PlayerHp = playerHp;
            PlayerMaxHp = playerMaxHp;
            PlayerAttack = playerAttack;
            Mental = mental;
            Gold = gold;
            GlitchLevel = glitchLevel;
            Affinity = affinity;
            NodesResolved = nodesResolved;
            BattlesWon = battlesWon;
            AbilityCount = abilityCount;
            RunCompleted = runCompleted;
        }

        public string RunId { get; }
        public int PlayerHp { get; }
        public int PlayerMaxHp { get; }
        public int PlayerAttack { get; }
        public int Mental { get; }
        public int Gold { get; }
        public int GlitchLevel { get; }
        public int Affinity { get; }
        public int NodesResolved { get; }
        public int BattlesWon { get; }
        public int AbilityCount { get; }
        public bool RunCompleted { get; }
    }
}
