using System.Collections.Generic;
using HwigiTower.Core;

namespace HwigiTower.Abilities
{
    public sealed class SynergyDetector
    {
        private readonly GameFlowEventBus _eventBus;
        private readonly string _runId;
        private readonly HashSet<string> _raisedActiveSynergies = new HashSet<string>();

        public SynergyDetector(GameFlowEventBus eventBus, string runId)
        {
            _eventBus = eventBus;
            _runId = runId ?? string.Empty;
        }

        public IReadOnlyList<SynergyState> Evaluate(IReadOnlyList<AbilityData> abilities, IReadOnlyList<SynergyData> synergies)
        {
            var result = new List<SynergyState>();
            if (synergies == null)
            {
                return result;
            }

            for (var i = 0; i < synergies.Count; i++)
            {
                var synergy = synergies[i];
                if (synergy == null)
                {
                    continue;
                }

                var active = CountTag(abilities, synergy.Tag) >= synergy.RequiredCount;
                result.Add(new SynergyState(synergy, active));
                if (active && _raisedActiveSynergies.Add(synergy.Tag))
                {
                    _eventBus?.Raise(new GameFlowEvent(GameFlowEventType.SynergyActivated, _runId, synergy.Tag, synergy.RequiredCount.ToString()));
                }
            }

            return result;
        }

        private static int CountTag(IReadOnlyList<AbilityData> abilities, string tag)
        {
            if (abilities == null || string.IsNullOrWhiteSpace(tag))
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < abilities.Count; i++)
            {
                if (abilities[i] != null && abilities[i].Tag == tag)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
