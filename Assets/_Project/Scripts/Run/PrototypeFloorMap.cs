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

            var bossStep = bossIndex >= 0 ? steps[bossIndex] : null;
            var branchLayers = BuildFixedBranchLayers(BuildFixedBranchSteps(branchSteps, bossStep, random));
            for (var layer = 1; layer <= branchLayers.Length; layer++)
            {
                var layerSteps = branchLayers[layer - 1];
                var count = layerSteps.Count;
                for (var index = 0; index < count; index++)
                {
                    var step = layerSteps[index];
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
                    NormalizedY = 0.75f
                };
                destination.Add(shopNode);
            }

            PrototypeFloorMapNode bossNode = null;
            if (bossIndex >= 0)
            {
                bossNode = new PrototypeFloorMapNode(BuildMapNodeId(floor, 5, 0, steps[bossIndex]), PrototypeFloorMapNodeType.Boss, floor, 5, 0, steps[bossIndex])
                {
                    NormalizedX = 0.5f,
                    NormalizedY = 0.91f
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
                NormalizedY = 0.91f
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
            var baseX = ResolveLaneX(node.Index, layerCount);
            var jitter = random.Range(-0.018f, 0.018f);
            node.NormalizedX = Clamp01(baseX + jitter);
            node.NormalizedY = node.Layer switch
            {
                1 => random.Range(0.14f, 0.17f),
                2 => random.Range(0.36f, 0.39f),
                3 => random.Range(0.58f, 0.61f),
                _ => 0.5f
            };
        }

        private static float ResolveLaneX(int index, int layerCount)
        {
            if (layerCount <= 1)
            {
                return 0.5f;
            }

            if (layerCount == 2)
            {
                return index <= 0 ? 0.34f : 0.66f;
            }

            if (layerCount == 3)
            {
                return index <= 0 ? 0.22f : index == 1 ? 0.5f : 0.78f;
            }

            return index switch
            {
                0 => 0.17f,
                1 => 0.39f,
                2 => 0.61f,
                _ => 0.83f
            };
        }

        private static List<PrototypeDemoRunStep>[] BuildBranchLayers(List<PrototypeDemoRunStep> branchSteps)
        {
            var layers = new[]
            {
                new List<PrototypeDemoRunStep>(),
                new List<PrototypeDemoRunStep>(),
                new List<PrototypeDemoRunStep>()
            };

            if (branchSteps == null || branchSteps.Count == 0)
            {
                return layers;
            }

            for (var i = 0; i < branchSteps.Count; i++)
            {
                var step = branchSteps[i];
                if (step == null || !step.IsValid)
                {
                    continue;
                }

                var layer = ResolveBranchLayer(i, branchSteps.Count);
                layers[layer].Add(step);
            }

            return layers;
        }

        private static List<PrototypeDemoRunStep> BuildFixedBranchSteps(List<PrototypeDemoRunStep> branchSteps, PrototypeDemoRunStep bossStep, DeterministicRandom random)
        {
            var combat = CollectStepsOfType(branchSteps, PrototypeFloorMapNodeType.Combat);
            var events = CollectStepsOfType(branchSteps, PrototypeFloorMapNodeType.Event);
            var rests = CollectStepsOfType(branchSteps, PrototypeFloorMapNodeType.Rest);
            if (combat.Count == 0 && bossStep != null && bossStep.IsValid)
            {
                combat.Add(bossStep);
            }

            var fallback = branchSteps ?? new List<PrototypeDemoRunStep>();
            var results = new List<PrototypeDemoRunStep>(10);
            AddRepeated(results, events, fallback, 4);
            AddRepeated(results, combat, fallback, 4);
            AddRepeated(results, rests, fallback, 2);
            Shuffle(results, random);
            return results;
        }

        private static List<PrototypeDemoRunStep>[] BuildFixedBranchLayers(List<PrototypeDemoRunStep> branchSteps)
        {
            var layers = new[]
            {
                new List<PrototypeDemoRunStep>(),
                new List<PrototypeDemoRunStep>(),
                new List<PrototypeDemoRunStep>()
            };

            if (branchSteps == null || branchSteps.Count == 0)
            {
                return layers;
            }

            MoveNonRestStepsToFront(branchSteps, 3);
            var layerSizes = new[] { 3, 4, 3 };
            var cursor = 0;
            for (var layer = 0; layer < layers.Length; layer++)
            {
                var count = layerSizes[layer];
                for (var i = 0; i < count && cursor < branchSteps.Count; i++, cursor++)
                {
                    layers[layer].Add(branchSteps[cursor]);
                }
            }

            return layers;
        }

        private static void MoveNonRestStepsToFront(List<PrototypeDemoRunStep> steps, int requiredCount)
        {
            if (steps == null || steps.Count == 0 || requiredCount <= 0)
            {
                return;
            }

            var firstLayer = new List<PrototypeDemoRunStep>(requiredCount);
            for (var i = 0; i < steps.Count && firstLayer.Count < requiredCount; i++)
            {
                var step = steps[i];
                if (step != null && step.IsValid && Classify(step, false) != PrototypeFloorMapNodeType.Rest)
                {
                    firstLayer.Add(step);
                    steps.RemoveAt(i);
                    i--;
                }
            }

            for (var i = firstLayer.Count - 1; i >= 0; i--)
            {
                steps.Insert(0, firstLayer[i]);
            }
        }

        private static List<PrototypeDemoRunStep> CollectStepsOfType(List<PrototypeDemoRunStep> branchSteps, PrototypeFloorMapNodeType type)
        {
            var results = new List<PrototypeDemoRunStep>();
            if (branchSteps == null)
            {
                return results;
            }

            for (var i = 0; i < branchSteps.Count; i++)
            {
                var step = branchSteps[i];
                if (step != null && step.IsValid && Classify(step, false) == type)
                {
                    results.Add(step);
                }
            }

            return results;
        }

        private static void AddRepeated(List<PrototypeDemoRunStep> destination, List<PrototypeDemoRunStep> preferred, List<PrototypeDemoRunStep> fallback, int count)
        {
            var source = preferred != null && preferred.Count > 0 ? preferred : fallback;
            if (destination == null || source == null || source.Count == 0)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                destination.Add(source[i % source.Count]);
            }
        }

        private static void Shuffle(List<PrototypeDemoRunStep> steps, DeterministicRandom random)
        {
            if (steps == null || random == null)
            {
                return;
            }

            for (var i = steps.Count - 1; i > 0; i--)
            {
                var swap = random.Range(0, i + 1);
                var temp = steps[i];
                steps[i] = steps[swap];
                steps[swap] = temp;
            }
        }

        private static int ResolveBranchLayer(int index, int count)
        {
            if (count <= 1)
            {
                return 0;
            }

            if (count == 2)
            {
                return index == 0 ? 0 : 1;
            }

            if (count == 3)
            {
                return index;
            }

            if (index == 0)
            {
                return 0;
            }

            if (index == count - 1)
            {
                return 2;
            }

            return 1;
        }

        private static void ConnectLayers(List<PrototypeFloorMapNode> nodes)
        {
            var nonEmptyLayers = new List<List<PrototypeFloorMapNode>>();
            for (var layer = 1; layer <= 4; layer++)
            {
                var layerNodes = CollectLayer(nodes, layer);
                if (layerNodes.Count > 0)
                {
                    nonEmptyLayers.Add(layerNodes);
                }
            }

            for (var layer = 0; layer < nonEmptyLayers.Count - 1; layer++)
            {
                var current = nonEmptyLayers[layer];
                var next = nonEmptyLayers[layer + 1];
                SortNodesByPosition(current);
                SortNodesByPosition(next);
                for (var i = 0; i < current.Count; i++)
                {
                    var primaryIndex = ResolveNearestLaneIndex(next, current[i].NormalizedX);
                    Connect(current[i], next[primaryIndex]);

                    var crossIndex = ResolveAdjacentLaneIndex(i, current.Count, primaryIndex, next.Count);
                    if (crossIndex >= 0 && crossIndex != primaryIndex)
                    {
                        Connect(current[i], next[crossIndex]);
                    }
                }
            }
        }

        private static void SortNodesByPosition(List<PrototypeFloorMapNode> nodes)
        {
            nodes.Sort((left, right) =>
            {
                var compare = left.Layer.CompareTo(right.Layer);
                if (compare != 0)
                {
                    return compare;
                }

                compare = left.NormalizedX.CompareTo(right.NormalizedX);
                return compare != 0 ? compare : left.Index.CompareTo(right.Index);
            });
        }

        private static int ResolveNearestLaneIndex(List<PrototypeFloorMapNode> nodes, float normalizedX)
        {
            var bestIndex = 0;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < nodes.Count; i++)
            {
                var distance = nodes[i] == null ? float.MaxValue : System.Math.Abs(nodes[i].NormalizedX - normalizedX);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static int ResolveAdjacentLaneIndex(int currentIndex, int currentCount, int primaryIndex, int nextCount)
        {
            if (nextCount <= 1)
            {
                return -1;
            }

            var preferRight = (currentIndex + currentCount) % 2 == 0;
            var right = primaryIndex + 1;
            var left = primaryIndex - 1;
            if (preferRight && right < nextCount)
            {
                return right;
            }

            if (!preferRight && left >= 0)
            {
                return left;
            }

            if (right < nextCount)
            {
                return right;
            }

            return left >= 0 ? left : -1;
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

            if (HasStartCombatEffect(step.Encounter))
            {
                return PrototypeFloorMapNodeType.Combat;
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
