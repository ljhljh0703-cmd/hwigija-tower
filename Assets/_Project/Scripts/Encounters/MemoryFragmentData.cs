using UnityEngine;

namespace HwigiTower.Encounters
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Memory Fragment", fileName = "SO_MemoryFragment_Placeholder")]
    public sealed class MemoryFragmentData : ScriptableObject
    {
        [SerializeField] private string stableId = string.Empty;
        [SerializeField] private string titleKey = string.Empty;
        [SerializeField] private string bodyKey = string.Empty;
        [SerializeField] private string stage = string.Empty;

        public string StableId => stableId;
        public string TitleKey => titleKey;
        public string BodyKey => bodyKey;
        public string Stage => stage;
    }
}
