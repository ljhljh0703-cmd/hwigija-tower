using System.Collections.Generic;
using HwigiTower.Encounters;

namespace HwigiTower.Run
{
    public enum PrototypeFloorMapNodeType
    {
        Combat = 0,
        Event = 1,
        Rest = 2,
        Shop = 3,
        Boss = 4
    }

    public readonly struct PrototypeFloorMapNodeView
    {
        public PrototypeFloorMapNodeView(string mapNodeId, PrototypeFloorMapNodeType type, int floor, int layer, int index, bool selectable, bool completed, bool locked)
        {
            MapNodeId = mapNodeId ?? string.Empty;
            Type = type;
            Floor = floor < 1 ? 1 : floor;
            Layer = layer < 1 ? 1 : layer;
            Index = index < 0 ? 0 : index;
            Selectable = selectable;
            Completed = completed;
            Locked = locked;
        }

        public string MapNodeId { get; }
        public PrototypeFloorMapNodeType Type { get; }
        public int Floor { get; }
        public int Layer { get; }
        public int Index { get; }
        public bool Selectable { get; }
        public bool Completed { get; }
        public bool Locked { get; }
    }

    internal sealed class PrototypeFloorMapNode
    {
        public PrototypeFloorMapNode(string mapNodeId, PrototypeFloorMapNodeType type, int floor, int layer, int index, PrototypeDemoRunStep step)
        {
            MapNodeId = mapNodeId ?? string.Empty;
            Type = type;
            Floor = floor < 1 ? 1 : floor;
            Layer = layer < 1 ? 1 : layer;
            Index = index < 0 ? 0 : index;
            Step = step;
        }

        public string MapNodeId { get; }
        public PrototypeFloorMapNodeType Type { get; }
        public int Floor { get; }
        public int Layer { get; }
        public int Index { get; }
        public PrototypeDemoRunStep Step { get; }
        public bool Completed { get; set; }
        public bool Skipped { get; set; }
        public bool IsValid => Step != null && Step.IsValid;
    }

    internal static class PrototypeFloorMapBuilder
    {
        public static void Build(int floor, IReadOnlyList<PrototypeDemoRunStep> steps, List<PrototypeFloorMapNode> destination)
        {
            destination.Clear();
            if (steps == null || steps.Count == 0)
            {
                return;
            }

            var bossIndex = FindBossIndex(steps);
            var shopIndex = FindShopIndex(steps, bossIndex);
            var branchIndex = 0;
            for (var i = 0; i < steps.Count; i++)
            {
                if (i == bossIndex || i == shopIndex || steps[i] == null || !steps[i].IsValid)
                {
                    continue;
                }

                var layer = branchIndex / 2 + 1;
                if (layer > 3)
                {
                    layer = 3;
                }

                destination.Add(new PrototypeFloorMapNode(
                    BuildMapNodeId(floor, layer, branchIndex % 2, steps[i]),
                    Classify(steps[i], false),
                    floor,
                    layer,
                    branchIndex % 2,
                    steps[i]));
                branchIndex++;
            }

            if (shopIndex >= 0)
            {
                destination.Add(new PrototypeFloorMapNode(BuildMapNodeId(floor, 4, 0, steps[shopIndex]), PrototypeFloorMapNodeType.Shop, floor, 4, 0, steps[shopIndex]));
            }

            if (bossIndex >= 0)
            {
                destination.Add(new PrototypeFloorMapNode(BuildMapNodeId(floor, 5, 0, steps[bossIndex]), PrototypeFloorMapNodeType.Boss, floor, 5, 0, steps[bossIndex]));
            }
        }

        private static int FindBossIndex(IReadOnlyList<PrototypeDemoRunStep> steps)
        {
            for (var i = steps.Count - 1; i >= 0; i--)
            {
                var step = steps[i];
                if (step != null && step.IsValid && HasStartCombatEffect(step.Encounter))
                {
                    return i;
                }
            }

            return steps.Count - 1;
        }

        private static int FindShopIndex(IReadOnlyList<PrototypeDemoRunStep> steps, int bossIndex)
        {
            var end = bossIndex < 0 ? steps.Count - 1 : bossIndex - 1;
            for (var i = end; i >= 0; i--)
            {
                var step = steps[i];
                if (step != null && step.IsValid && step.Encounter != null && step.Encounter.Type == EncounterType.Shop)
                {
                    return i;
                }
            }

            return -1;
        }

        private static PrototypeFloorMapNodeType Classify(PrototypeDemoRunStep step, bool boss)
        {
            if (boss)
            {
                return PrototypeFloorMapNodeType.Boss;
            }

            if (step == null || step.Encounter == null)
            {
                return PrototypeFloorMapNodeType.Event;
            }

            return step.Encounter.Type switch
            {
                EncounterType.Battle => PrototypeFloorMapNodeType.Combat,
                EncounterType.Rest => PrototypeFloorMapNodeType.Rest,
                EncounterType.Shop => PrototypeFloorMapNodeType.Shop,
                _ => PrototypeFloorMapNodeType.Event
            };
        }

        private static bool HasStartCombatEffect(EncounterData encounter)
        {
            if (encounter == null || encounter.Choices == null)
            {
                return false;
            }

            for (var i = 0; i < encounter.Choices.Length; i++)
            {
                var choice = encounter.Choices[i];
                if (choice == null || choice.effects == null)
                {
                    continue;
                }

                for (var e = 0; e < choice.effects.Length; e++)
                {
                    var effect = choice.effects[e];
                    if (effect != null && effect.kind == "StartCombat")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string BuildMapNodeId(int floor, int layer, int index, PrototypeDemoRunStep step)
        {
            var encounterId = step == null ? "missing" : step.EncounterId;
            return "floor." + floor + ".layer." + layer + "." + index + "." + encounterId;
        }
    }
}
