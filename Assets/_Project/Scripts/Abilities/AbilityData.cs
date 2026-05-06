using System.Collections.Generic;
using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Abilities
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Ability", fileName = "SO_Ability_Placeholder")]
    public sealed class AbilityData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private string tag = string.Empty;
        [SerializeField, TextArea(1, 3)] private string description = string.Empty;
        // per GDD OQ-013: 기본 20G, 동일 태그 3번째 30G
        [SerializeField, Min(0)] private int costGold = 20;
        [SerializeField] private NumericParam[] numericParams = new NumericParam[0];

        public string Id => id;
        public string DisplayName => displayName;
        public string Tag => tag;
        public string Description => description;
        public int CostGold => costGold;
        public IReadOnlyList<NumericParam> NumericParams => numericParams;
    }
}
