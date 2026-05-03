using UnityEngine;
using HwigiTower.Combat;

namespace HwigiTower.Encounters
{
    [System.Serializable]
    public sealed class EncounterChoiceRuntimeData
    {
        public string stableId = string.Empty;
        public string textKey = string.Empty;
        public string requirementMode = "All";
        public EncounterRequirementRuntimeData[] requirements = new EncounterRequirementRuntimeData[0];
        public EncounterEffectRuntimeData[] effects = new EncounterEffectRuntimeData[0];
        public string unavailablePolicyMode = "Hidden";
        public string unavailableReasonTextKey = string.Empty;
        public string npcReactionKey = string.Empty;
    }

    [System.Serializable]
    public sealed class EncounterRequirementRuntimeData
    {
        public string kind = string.Empty;
        public string stat = string.Empty;
        public string op = string.Empty;
        public int value;
        public string flag = string.Empty;
        public bool expected;
        public string itemRef = string.Empty;
        public int minCount;
        public string abilityRef = string.Empty;
        public int min;
        public int max;
        public int minFloor;
        public int maxFloor;
    }

    [System.Serializable]
    public class EncounterEffectRuntimeData
    {
        public string kind = string.Empty;
        public int amount;
        public string flag = string.Empty;
        public bool value;
        public string itemRef = string.Empty;
        public int count;
        public string abilityRef = string.Empty;
        public string rewardBundleRef = string.Empty;
        public string targetEncounterId = string.Empty;
        public string targetNodeId = string.Empty;
        public EncounterCombatHandoffRuntimeData combatHandoff;
    }

    [System.Serializable]
    public sealed class EncounterPostCombatEffectRuntimeData
    {
        public string kind = string.Empty;
        public int amount;
        public string flag = string.Empty;
        public bool value;
        public string itemRef = string.Empty;
        public int count;
        public string abilityRef = string.Empty;
        public string rewardBundleRef = string.Empty;
        public string targetEncounterId = string.Empty;
        public string targetNodeId = string.Empty;
    }

    [System.Serializable]
    public sealed class EncounterCombatHandoffRuntimeData
    {
        public string stableId = string.Empty;
        public string sourceEncounterId = string.Empty;
        public string sourceChoiceId = string.Empty;
        public string seedKey = string.Empty;
        public string sourceHash = string.Empty;
        public int floor;
        public string[] enemyRefs = new string[0];
        public string npcStage = string.Empty;
        public EncounterPostCombatEffectRuntimeData[] onVictoryEffects = new EncounterPostCombatEffectRuntimeData[0];
        public EncounterPostCombatEffectRuntimeData[] onDefeatEffects = new EncounterPostCombatEffectRuntimeData[0];
        public string onVictoryReturnNode = string.Empty;
        public string onDefeatReturnNode = string.Empty;
        public string victoryTextKey = string.Empty;
        public string defeatTextKey = string.Empty;
        public string deathTextKey = string.Empty;
        public string npcReactionKey = string.Empty;
        public string promptHash = string.Empty;
        public string cacheKey = string.Empty;
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Encounter", fileName = "SO_Encounter_Placeholder")]
    public sealed class EncounterData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private EncounterType type;
        [SerializeField, Min(0)] private int floor;
        [SerializeField, Min(0)] private int weight;
        [SerializeField] private string deterministicSeedKey = string.Empty;
        [SerializeField] private EnemyData enemy;
        [SerializeField] private string assetPath = string.Empty;
        [SerializeField] private string sourceHash = string.Empty;
        [SerializeField] private string contentCategory = string.Empty;
        [SerializeField] private string npcStage = string.Empty;
        [SerializeField] private string bodyTextKey = string.Empty;
        [SerializeField] private string writerStatus = string.Empty;
        [SerializeField] private int glitchLevel;
        [SerializeField] private int timeLimitSeconds;
        [SerializeField] private string[] choiceStableIds = new string[0];
        [SerializeField] private EncounterChoiceRuntimeData[] choices = new EncounterChoiceRuntimeData[0];

        public string Id => id;
        public EncounterType Type => type;
        public int Floor => floor;
        public int Weight => weight;
        public string DeterministicSeedKey => deterministicSeedKey;
        public EnemyData Enemy => enemy;
        public string AssetPath => assetPath;
        public string SourceHash => sourceHash;
        public string ContentCategory => contentCategory;
        public string NpcStage => npcStage;
        public string BodyTextKey => bodyTextKey;
        public string WriterStatus => writerStatus;
        public int GlitchLevel => glitchLevel;
        public int TimeLimitSeconds => timeLimitSeconds;
        public string[] ChoiceStableIds => choiceStableIds;
        public EncounterChoiceRuntimeData[] Choices => choices;
    }
}
