using System.Collections.Generic;
using UnityEngine;

namespace HwigiTower.NPC
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/NPC State", fileName = "SO_NPCState_Placeholder")]
    public sealed class NpcStateData : ScriptableObject
    {
        [SerializeField] private NpcStage stage;
        [SerializeField] private NpcDisplayRule[] displayRules = new NpcDisplayRule[0];
        [SerializeField] private string promptProfileId = string.Empty;

        public NpcStage Stage => stage;
        public IReadOnlyList<NpcDisplayRule> DisplayRules => displayRules;
        public string PromptProfileId => promptProfileId;
    }
}
