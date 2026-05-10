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
            string nextDemoEncounterId = "",
            int demoStepCount = 0,
            int demoResolvedStepCount = 0,
            int memoryFragmentCount = 0,
            string lastMemoryFragmentId = "",
            string lastMemoryFragmentTitleKey = "",
            string lastMemoryFragmentBodyKey = "",
            string lastCombatId = "",
            string lastCombatEnemyId = "",
            string lastCombatResultId = "",
            string lastCombatRoundResult = "",
            bool isInCombat = false,
            int enemyHp = 0,
            int enemyMaxHp = 0,
            int combatRound = 0,
            int lastCombatGoldReward = 0,
            int lastCombatGlitchDelta = 0,
            int lastCombatAffinityDelta = 0,
            bool lastCombatEnemyDefeated = false,
            int lastCombatComboDamage = 0,
            int currentFloor = 1,
            bool stairUnlocked = false,
            bool runClear = false,
            bool runFailed = false,
            bool restartReady = false,
            bool bossGateUnlocked = false,
            string runStatus = "",
            string lastNpcReactionKey = "",
            int itemCount = 0,
            bool endingChoicePending = false,
            bool endingRest = false,
            bool endingContinue = false,
            string endingChoiceId = "",
            PrototypeFloorMapNodeView[] floorMapNodes = null,
            string selectedMapNodeId = "")
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
            DemoStepCount = demoStepCount;
            DemoResolvedStepCount = demoResolvedStepCount;
            MemoryFragmentCount = memoryFragmentCount;
            LastMemoryFragmentId = lastMemoryFragmentId ?? string.Empty;
            LastMemoryFragmentTitleKey = lastMemoryFragmentTitleKey ?? string.Empty;
            LastMemoryFragmentBodyKey = lastMemoryFragmentBodyKey ?? string.Empty;
            LastCombatId = lastCombatId ?? string.Empty;
            LastCombatEnemyId = lastCombatEnemyId ?? string.Empty;
            LastCombatResultId = lastCombatResultId ?? string.Empty;
            LastCombatRoundResult = lastCombatRoundResult ?? string.Empty;
            IsInCombat = isInCombat;
            EnemyHp = enemyHp;
            EnemyMaxHp = enemyMaxHp;
            CombatRound = combatRound;
            LastCombatGoldReward = lastCombatGoldReward;
            LastCombatGlitchDelta = lastCombatGlitchDelta;
            LastCombatAffinityDelta = lastCombatAffinityDelta;
            LastCombatEnemyDefeated = lastCombatEnemyDefeated;
            LastCombatComboDamage = lastCombatComboDamage;
            CurrentFloor = currentFloor < 1 ? 1 : currentFloor;
            StairUnlocked = stairUnlocked;
            RunClear = runClear;
            RunFailed = runFailed;
            RestartReady = restartReady;
            BossGateUnlocked = bossGateUnlocked;
            RunStatus = string.IsNullOrEmpty(runStatus) ? runClear ? "run.clear" : runFailed ? "run.failed" : "run.active" : runStatus;
            LastNpcReactionKey = lastNpcReactionKey ?? string.Empty;
            ItemCount = itemCount;
            EndingChoicePending = endingChoicePending;
            EndingRest = endingRest;
            EndingContinue = endingContinue;
            EndingChoiceId = endingChoiceId ?? string.Empty;
            FloorMapNodes = floorMapNodes ?? new PrototypeFloorMapNodeView[0];
            SelectedMapNodeId = selectedMapNodeId ?? string.Empty;
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
        public int DemoStepCount { get; }
        public int DemoResolvedStepCount { get; }
        public int MemoryFragmentCount { get; }
        public string LastMemoryFragmentId { get; }
        public string LastMemoryFragmentTitleKey { get; }
        public string LastMemoryFragmentBodyKey { get; }
        public string LastCombatId { get; }
        public string LastCombatEnemyId { get; }
        public string LastCombatResultId { get; }
        public string LastCombatRoundResult { get; }
        public bool IsInCombat { get; }
        public int EnemyHp { get; }
        public int EnemyMaxHp { get; }
        public int CombatRound { get; }
        public int LastCombatGoldReward { get; }
        public int LastCombatGlitchDelta { get; }
        public int LastCombatAffinityDelta { get; }
        public bool LastCombatEnemyDefeated { get; }
        public int LastCombatComboDamage { get; }
        public int CurrentFloor { get; }
        public bool StairUnlocked { get; }
        public bool RunClear { get; }
        public bool RunFailed { get; }
        public bool RestartReady { get; }
        public bool BossGateUnlocked { get; }
        public string RunStatus { get; }
        public string LastNpcReactionKey { get; }
        public int ItemCount { get; }
        public bool EndingChoicePending { get; }
        public bool EndingRest { get; }
        public bool EndingContinue { get; }
        public string EndingChoiceId { get; }
        public PrototypeFloorMapNodeView[] FloorMapNodes { get; }
        public string SelectedMapNodeId { get; }
        public bool HasFloorMap => FloorMapNodes.Length > 0;
        public bool HasSelectedMapNode => !string.IsNullOrEmpty(SelectedMapNodeId);
    }
}
