using System;
using UnityEngine;

namespace HwigiTower.NPC
{
    [Serializable]
    public sealed class NpcDisplayRule
    {
        [SerializeField] private string ruleId = string.Empty;
        [SerializeField] private string value = string.Empty;

        public string RuleId => ruleId;
        public string Value => value;
    }
}
