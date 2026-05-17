using System.Collections.Generic;
using HwigiTower.Core;
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
            : this(mapNodeId, type, floor, layer, index, selectable, completed, locked, 0.5f, 0.5f, false, null, null)
        {
        }

        public PrototypeFloorMapNodeView(
            string mapNodeId,
            PrototypeFloorMapNodeType type,
            int floor,
            int layer,
            int index,
            bool selectable,
            bool completed,
            bool locked,
            float normalizedX,
            float normalizedY,
            bool current,
            string[] nextMapNodeIds,
            string[] previousMapNodeIds)
        {
            MapNodeId = mapNodeId ?? string.Empty;
            Type = type;
            Floor = floor < 1 ? 1 : floor;
            Layer = layer < 1 ? 1 : layer;
            Index = index < 0 ? 0 : index;
            Selectable = selectable;
            Completed = completed;
            Locked = locked;
            NormalizedX = Clamp01(normalizedX);
            NormalizedY = Clamp01(normalizedY);
            Current = current;
            NextMapNodeIds = nextMapNodeIds ?? System.Array.Empty<string>();
            PreviousMapNodeIds = previousMapNodeIds ?? System.Array.Empty<string>();
        }

        public string MapNodeId { get; }
        public PrototypeFloorMapNodeType Type { get; }
        public int Floor { get; }
        public int Layer { get; }
        public int Index { get; }
        public bool Selectable { get; }
        public bool Completed { get; }
        public bool Locked { get; }
        public float NormalizedX { get; }
        public float NormalizedY { get; }
        public bool Current { get; }
        public string[] NextMapNodeIds { get; }
        public string[] PreviousMapNodeIds { get; }

        private static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            return value > 1f ? 1f : value;
        }
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
        public float NormalizedX { get; set; }
        public float NormalizedY { get; set; }
        public List<string> NextMapNodeIds { get; } = new List<string>();
        public List<string> PreviousMapNodeIds { get; } = new List<string>();
        public bool Completed { get; set; }
        public bool Skipped { get; set; }
        public bool IsValid => Step != null && Step.IsValid;
    }

    internal static class PrototypeFloorMapBuilder
    {
        public static void Build(int floor, IReadOnlyList<PrototypeDemoRunStep> steps, List<PrototypeFloorMapNode> destination)
        {
            Build(floor, steps, "prototype-run", destination);
        }

        public static void Build(int floor, IReadOnlyList<PrototypeDemoRunStep> steps, string runId, List<PrototypeFloorMapNode> destination)
        {
            destination.Clear();
            if (steps == null || steps.Count == 0)
            {
                return;
            }

            var bossIndex = FindBossIndex(steps);
            var shopIndex = FindShopIndex(steps, bossIndex);
            var branchSteps = new List<PrototypeDemoRunStep>();
            for (var i = 0; i < steps.Count; i++)
            {
                if (i == bossIndex || i == shopIndex || steps[i] == null || !steps[i].IsValid)
                {
                    continue;
                }

                var type = Classify(steps[i], false);
                if (type == PrototypeFloorMapNodeType.Combat || type == PrototypeFloorMapNodeType.Event || type == PrototypeFloorMapNodeType.Rest)
                {
                    branchSteps.Add(steps[i]);
                }
            }

            if (branchSteps.Count == 0)
            {
                for (var i = 0; i < steps.Count; i++)
                {
                    if (i != bossIndex && i != shopIndex && steps[i] != null && steps[i].IsValid)
                    {
                        branchSteps.Add(steps[i]);
                    }
                }
            }

            var random = new DeterministicRandom(DeterministicSeed.Combine(0, "floor-map." + (runId ?? "prototype-run") + "." + floor));
            if (shopIndex < 0)
            {
                BuildFallbackBranchMap(floor, branchSteps, bossIndex >= 0 ? steps[bossIndex] : null, random, destination);
                return;
            }

            for (var layer = 1; layer <= 3; layer++)
            {
                var count = random.Range(2, 4);
                for (var index = 0; index < count; index++)
                {
                    var step = branchSteps.Count == 0
                        ? null
                        : index == 0 && branchSteps.Count >= layer
                            ? branchSteps[layer - 1]
                            : branchSteps[random.Range(0, branchSteps.Count)];
                    if (step == null || !step.IsValid)
                    {
                        continue;
                    }

                    var node = new PrototypeFloorMapNode(
                        BuildMapNodeId(floor, layer, index, step),
                        Classify(step, false),
                        floor,
                        layer,
                        index,
                        step);
                    AssignPosition(node, count, random);
                    destination.Add(node);
                }
            }

            PrototypeFloorMapNode shopNode = null;
            if (shopIndex >= 0)
            {
                shopNode = new PrototypeFloorMapNode(BuildMapNodeId(floor, 4, 0, steps[shopIndex]), PrototypeFloorMapNodeType.Shop, floor, 4, 0, steps[shopIndex])
                {
                    NormalizedX = 0.5f,
                    NormalizedY = 0.24f
                };
                destination.Add(shopNode);
            }

            PrototypeFloorMapNode bossNode = null;
            if (bossIndex >= 0)
            {
                bossNode = new PrototypeFloorMapNode(BuildMapNodeId(floor, 5, 0, steps[bossIndex]), PrototypeFloorMapNodeType.Boss, floor, 5, 0, steps[bossIndex])
                {
                    NormalizedX = 0.5f,
                    NormalizedY = 0.08f
                };
                destination.Add(bossNode);
            }

            ConnectLayers(destination);
            if (shopNode != null && bossNode != null)
            {
                Connect(shopNode, bossNode);
            }
        }

        private static void BuildFallbackBranchMap(
            int floor,
            List<PrototypeDemoRunStep> branchSteps,
            PrototypeDemoRunStep bossStep,
            DeterministicRandom random,
            List<PrototypeFloorMapNode> destination)
        {
            var branchCount = branchSteps.Count == 0 ? 0 : branchSteps.Count;
            for (var index = 0; index < branchCount; index++)
            {
                var step = branchSteps[index];
                var node = new PrototypeFloorMapNode(
                    BuildMapNodeId(floor, 1, index, step),
                    Classify(step, false),
                    floor,
                    1,
                    index,
                    step);
                AssignPosition(node, branchCount, random);
                destination.Add(node);
            }

            if (bossStep == null || !bossStep.IsValid)
            {
                return;
            }

            var bossNode = new PrototypeFloorMapNode(BuildMapNodeId(floor, 5, 0, bossStep), PrototypeFloorMapNodeType.Boss, floor, 5, 0, bossStep)
            {
                NormalizedX = 0.5f,
                NormalizedY = 0.08f
            };
            destination.Add(bossNode);

            var branchNodes = CollectLayer(destination, 1);
            if (branchNodes.Count == 0)
            {
                bossNode.NormalizedY = 0.5f;
                return;
            }

            for (var i = 0; i < branchNodes.Count; i++)
            {
                Connect(branchNodes[i], bossNode);
            }
        }

        private static void AssignPosition(PrototypeFloorMapNode node, int layerCount, DeterministicRandom random)
        {
            var baseX = layerCount <= 1 ? 0.5f : (node.Index + 1f) / (layerCount + 1f);
            var jitter = random.Range(-0.045f, 0.045f);
            node.NormalizedX = Clamp01(baseX + jitter);
            node.NormalizedY = node.Layer switch
            {
                1 => random.Range(0.73f, 0.81f),
                2 => random.Range(0.55f, 0.64f),
                3 => random.Range(0.37f, 0.46f),
                _ => 0.5f
            };
        }

        private static void ConnectLayers(List<PrototypeFloorMapNode> nodes)
        {
            for (var layer = 1; layer <= 3; layer++)
            {
                var current = CollectLayer(nodes, layer);
                var next = CollectLayer(nodes, layer + 1);
                if (layer == 3)
                {
                    next = CollectLayer(nodes, 4);
                }

                for (var i = 0; i < current.Count; i++)
                {
                    for (var n = 0; n < next.Count; n++)
                    {
                        Connect(current[i], next[n]);
                    }
                }
            }
        }

        private static List<PrototypeFloorMapNode> CollectLayer(List<PrototypeFloorMapNode> nodes, int layer)
        {
            var results = new List<PrototypeFloorMapNode>();
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null && nodes[i].Layer == layer)
                {
                    results.Add(nodes[i]);
                }
            }

            return results;
        }

        private static void Connect(PrototypeFloorMapNode from, PrototypeFloorMapNode to)
        {
            if (from == null || to == null)
            {
                return;
            }

            if (!from.NextMapNodeIds.Contains(to.MapNodeId))
            {
                from.NextMapNodeIds.Add(to.MapNodeId);
            }

            if (!to.PreviousMapNodeIds.Contains(from.MapNodeId))
            {
                to.PreviousMapNodeIds.Add(from.MapNodeId);
            }
        }

        private static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            return value > 1f ? 1f : value;
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
