using System;

namespace HwigiTower.Run
{
    [Serializable]
    public sealed class PrototypeRunSaveData
    {
        public int version = 1;
        public string runId = string.Empty;
        public int currentFloor = 1;
        public int playerHp = 24;
        public int playerMaxHp = 24;
        public int mataiosHp = 16;
        public bool mataiosDown;
        public int mental;
        public int gold;
        public int glitchLevel;
        public int affinity;
        public int nodesResolved;
        public int battlesWon;
        public int combatXp;
        public int combatLevel = 1;
        public int pendingLevelRewardChoices;
        public int levelAttackBonus;
        public int levelMaxHpBonus;
        public int mataiosActionPowerBonus;
        public int mataiosMaxHpBonus;
        public int skillCooldownReduction;
        public string lastGrowthMessage = string.Empty;
        public bool scoutAttackReady;
        public bool scoutDamageReductionReady;
        public bool runCompleted;
        public bool runClear;
        public bool runFailed;
        public bool restartReady;
        public bool endingRest;
        public bool endingContinue;
        public string endingChoiceId = string.Empty;
        public bool stairUnlocked;
        public bool trainingBuffActive;
        public string selectedMapNodeId = string.Empty;
        public string[] flags = Array.Empty<string>();
        public PrototypeRunSaveItemEntry[] items = Array.Empty<PrototypeRunSaveItemEntry>();
        public string[] abilityRefs = Array.Empty<string>();
        public string[] ownedCommandIds = Array.Empty<string>();
        public string[] equippedCommandIds = Array.Empty<string>();
        public string pendingCommandEquipId = string.Empty;
        public string[] rewardBundleRefs = Array.Empty<string>();
        public string[] memoryFragmentRefs = Array.Empty<string>();
        public string[] memoryConsequenceKeys = Array.Empty<string>();
        public PrototypeRunSaveResolvedChoice[] resolvedChoices = Array.Empty<PrototypeRunSaveResolvedChoice>();
        public string[] resolvedDemoStepKeys = Array.Empty<string>();
        public string[] completedMapNodeIds = Array.Empty<string>();
        public string[] skippedMapNodeIds = Array.Empty<string>();
    }

    [Serializable]
    public sealed class PrototypeRunSaveItemEntry
    {
        public string itemRef = string.Empty;
        public int count;
    }

    [Serializable]
    public sealed class PrototypeRunSaveResolvedChoice
    {
        public string key = string.Empty;
        public string choiceStableId = string.Empty;
    }

    public readonly struct PrototypeRunSaveSummary
    {
        public PrototypeRunSaveSummary(string runId, int currentFloor, int playerHp, int playerMaxHp, int memoryFragmentCount, string runStatus)
        {
            RunId = runId ?? string.Empty;
            CurrentFloor = currentFloor < 1 ? 1 : currentFloor;
            PlayerHp = playerHp;
            PlayerMaxHp = playerMaxHp <= 0 ? 1 : playerMaxHp;
            MemoryFragmentCount = memoryFragmentCount < 0 ? 0 : memoryFragmentCount;
            RunStatus = string.IsNullOrEmpty(runStatus) ? "run.active" : runStatus;
        }

        public string RunId { get; }
        public int CurrentFloor { get; }
        public int PlayerHp { get; }
        public int PlayerMaxHp { get; }
        public int MemoryFragmentCount { get; }
        public string RunStatus { get; }
        public bool IsValid => !string.IsNullOrEmpty(RunId);
        public string DisplayText => "Floor " + CurrentFloor + " | HP " + PlayerHp + "/" + PlayerMaxHp + " | 기억 " + MemoryFragmentCount;
    }
}
