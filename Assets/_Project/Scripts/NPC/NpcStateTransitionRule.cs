using System;
using UnityEngine;

namespace HwigiTower.NPC
{
    [Serializable]
    public sealed class NpcStateTransitionRule
    {
        [SerializeField] private NpcStage fromStage;
        [SerializeField] private string triggerId = string.Empty;
        [SerializeField] private NpcStage toStage;

        public NpcStage FromStage => fromStage;
        public string TriggerId => triggerId;
        public NpcStage ToStage => toStage;

        public bool Matches(NpcStage currentStage, string trigger)
        {
            return fromStage == currentStage && string.Equals(triggerId, trigger, StringComparison.Ordinal);
        }
    }
}
