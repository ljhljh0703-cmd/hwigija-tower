using System.Collections.Generic;
using HwigiTower.Core;

namespace HwigiTower.Abilities
{
    public sealed class AbilityInventory
    {
        private readonly List<AbilityData> _abilities = new List<AbilityData>();
        private readonly GameFlowEventBus _eventBus;
        private readonly string _runId;

        public AbilityInventory(GameFlowEventBus eventBus, string runId)
        {
            _eventBus = eventBus;
            _runId = runId ?? string.Empty;
        }

        public IReadOnlyList<AbilityData> Abilities => _abilities;

        public bool Add(AbilityData ability)
        {
            if (ability == null)
            {
                return false;
            }

            _abilities.Add(ability);
            _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.AbilityAdded, _runId, ability.Id, ability.Tag));
            return true;
        }
    }
}
