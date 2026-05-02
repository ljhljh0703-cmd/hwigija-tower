using System.Collections.Generic;
using HwigiTower.Core;

namespace HwigiTower.Encounters
{
    public sealed class EncounterSelector
    {
        public EncounterSelection Select(DeterministicRunContext context, PrototypeNodeDefinition node)
        {
            if (node == null)
            {
                return new EncounterSelection(null, null);
            }

            var encounters = node.PossibleEncounters;
            if (encounters == null || encounters.Count == 0)
            {
                return new EncounterSelection(node, null);
            }

            var weights = new List<int>(encounters.Count);
            for (var i = 0; i < encounters.Count; i++)
            {
                var encounter = encounters[i];
                weights.Add(encounter == null ? 0 : encounter.Weight);
            }

            var seedKey = $"encounter.{node.NodeId}";
            var random = context.CreateRandom(seedKey);
            var index = random.WeightedIndex(weights);
            return index < 0 ? new EncounterSelection(node, null) : new EncounterSelection(node, encounters[index]);
        }
    }
}
