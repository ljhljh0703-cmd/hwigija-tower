using UnityEngine;
using HwigiTower.Combat;

namespace HwigiTower.Encounters
{
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
    }
}
