using System;
using HwigiTower.Core;

namespace HwigiTower.NPC
{
    public sealed class NpcStateMachine
    {
        private readonly NpcStateMachineDefinition _definition;
        private readonly GameFlowEventBus _eventBus;
        private readonly string _runId;

        public NpcStateMachine(NpcStateMachineDefinition definition, string runId, GameFlowEventBus eventBus = null)
        {
            _definition = definition;
            _runId = string.IsNullOrWhiteSpace(runId) ? string.Empty : runId;
            _eventBus = eventBus;
            CurrentStage = definition == null ? NpcStage.S0 : definition.InitialStage;
        }

        public event Action<NpcStateChangedEvent> OnStageChanged;

        public NpcStage CurrentStage { get; private set; }

        public bool TryGetCurrentState(out NpcStateData state)
        {
            if (_definition == null)
            {
                state = null;
                return false;
            }

            return _definition.TryGetState(CurrentStage, out state);
        }

        public bool TryApply(string triggerId)
        {
            if (_definition == null || !_definition.TryGetTransition(CurrentStage, triggerId, out var transition))
            {
                return false;
            }

            var previousStage = CurrentStage;
            CurrentStage = transition.ToStage;
            var changed = new NpcStateChangedEvent(previousStage, CurrentStage, triggerId);
            OnStageChanged?.Invoke(changed);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.NpcStageChanged, _runId, previousStage.ToString(), CurrentStage.ToString()));
            return true;
        }
    }
}
