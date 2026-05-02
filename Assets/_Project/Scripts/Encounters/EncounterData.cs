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

        public string Id => id;
        public EncounterType Type => type;
        public int Floor => floor;
        public int Weight => weight;
        public string DeterministicSeedKey => deterministicSeedKey;
        public EnemyData Enemy => enemy;
    }
}
