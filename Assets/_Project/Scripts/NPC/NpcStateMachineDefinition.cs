using System.Collections.Generic;
using UnityEngine;

namespace HwigiTower.NPC
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/NPC State Machine", fileName = "SO_NPCStateMachine_Placeholder")]
    public sealed class NpcStateMachineDefinition : ScriptableObject
    {
        [SerializeField] private NpcStage initialStage;
        [SerializeField] private NpcStateData[] states = new NpcStateData[0];
        [SerializeField] private NpcStateTransitionRule[] transitions = new NpcStateTransitionRule[0];

        public NpcStage InitialStage => initialStage;
        public IReadOnlyList<NpcStateData> States => states;
        public IReadOnlyList<NpcStateTransitionRule> Transitions => transitions;

        public bool TryGetState(NpcStage stage, out NpcStateData state)
        {
            for (var i = 0; i < states.Length; i++)
            {
                if (states[i] != null && states[i].Stage == stage)
                {
                    state = states[i];
                    return true;
                }
            }

            state = null;
            return false;
        }

        public bool TryGetTransition(NpcStage stage, string triggerId, out NpcStateTransitionRule transition)
        {
            for (var i = 0; i < transitions.Length; i++)
            {
                if (transitions[i] != null && transitions[i].Matches(stage, triggerId))
                {
                    transition = transitions[i];
                    return true;
                }
            }

            transition = null;
            return false;
        }
    }
}
