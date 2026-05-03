using UnityEngine;

namespace HwigiTower.Items
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Item", fileName = "SO_Item_Placeholder")]
    public sealed class ItemData : ScriptableObject
    {
        [SerializeField] private string stableId = string.Empty;
        [SerializeField] private string displayNameKey = string.Empty;
        [SerializeField] private string descriptionKey = string.Empty;
        [SerializeField, Min(0)] private int maxStack = 99;

        public string StableId => stableId;
        public string DisplayNameKey => displayNameKey;
        public string DescriptionKey => descriptionKey;
        public int MaxStack => maxStack;
    }
}
