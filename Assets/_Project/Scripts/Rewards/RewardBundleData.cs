using System;
using UnityEngine;

namespace HwigiTower.Rewards
{
    [Serializable]
    public sealed class RewardBundleEntry
    {
        [SerializeField] private string itemRef = string.Empty;
        [SerializeField, Min(0)] private int itemCount;
        [SerializeField] private string abilityRef = string.Empty;

        public string ItemRef => itemRef;
        public int ItemCount => itemCount;
        public string AbilityRef => abilityRef;
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Reward Bundle", fileName = "SO_RewardBundle_Placeholder")]
    public sealed class RewardBundleData : ScriptableObject
    {
        [SerializeField] private string stableId = string.Empty;
        [SerializeField] private RewardBundleEntry[] entries = new RewardBundleEntry[0];

        public string StableId => stableId;
        public RewardBundleEntry[] Entries => entries;
    }
}
