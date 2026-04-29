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
        [SerializeField] private NumericParam[] numericParams = new NumericParam[0];

        public string Id => id;
        public string DisplayName => displayName;
        public string Tag => tag;
        public string Description => description;
        public IReadOnlyList<NumericParam> NumericParams => numericParams;
    }
}
