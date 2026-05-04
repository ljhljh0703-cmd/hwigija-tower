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
            bool runCompleted,
            string demoStatus = "",
            string nextDemoNodeId = "",
            string nextDemoEncounterId = "")
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
            DemoStatus = demoStatus ?? string.Empty;
            NextDemoNodeId = nextDemoNodeId ?? string.Empty;
            NextDemoEncounterId = nextDemoEncounterId ?? string.Empty;
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
        public string DemoStatus { get; }
        public string NextDemoNodeId { get; }
        public string NextDemoEncounterId { get; }
    }
}
