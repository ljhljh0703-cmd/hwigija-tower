using System.Collections.Generic;
using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Abilities
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Synergy", fileName = "SO_Synergy_Placeholder")]
    public sealed class SynergyData : ScriptableObject
    {
        [SerializeField] private string tag = string.Empty;
        [SerializeField, Min(0)] private int requiredCount;
        [SerializeField, TextArea(1, 3)] private string effectDescription = string.Empty;
        [SerializeField] private NumericParam[] numericParams = new NumericParam[0];

        public string Tag => tag;
        public int RequiredCount => requiredCount;
        public string EffectDescription => effectDescription;
        public IReadOnlyList<NumericParam> NumericParams => numericParams;
    }
}
