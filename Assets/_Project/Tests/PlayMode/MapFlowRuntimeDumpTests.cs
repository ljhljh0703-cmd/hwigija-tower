using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HwigiTower.Encounters;
using HwigiTower.Run;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HwigiTower.Tests.PlayMode
{
    public sealed class MapFlowRuntimeDumpTests
    {
        private static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static string DumpPath => Path.Combine(ProjectRoot, "Docs", "Portfolio", "assets", "concept-to-ui", "mapflow_runtime_dump.json");
        private static readonly FieldInfo FloorMapNodesField = typeof(PrototypeRunState).GetField("_floorMapNodes", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo ActiveMapLayerMethod = typeof(PrototypeRunState).GetMethod("GetActiveMapLayer", BindingFlags.Instance | BindingFlags.NonPublic);

        [SetUp]
        public void SetUp()
        {
            ResetRunStateIsolation();
        }

        [TearDown]
        public void TearDown()
        {
            ResetRunStateIsolation();
        }

        [UnityTest]
        [Explicit("M3 diagnostic dump: records normal traversal without changing map logic.")]
        public IEnumerator MapFlowM3_WritesNormalTraversalRuntimeDump()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = UnityEngine.Object.FindFirstObjectByType<PrototypeRoomController>();
            Assert.IsNotNull(controller);
            PrepareNormalRun(controller);

            var document = new MapFlowDumpDocument
            {
                source = "runtime",
                runId = controller.RunState == null ? string.Empty : controller.RunState.RunId
            };

            document.determinismPairA = CaptureFloor(controller, "beforeSelect", false);
            PrepareNormalRun(controller);
            document.determinismPairB = CaptureFloor(controller, "beforeSelect", false);
            PrepareNormalRun(controller);

            var guard = 0;
            while (controller.RunState != null && !controller.RunState.RunCompleted && guard++ < 80)
            {
                if (controller.RunState.StairUnlocked)
                {
                    controller.ResolveNextFloor();
                    continue;
                }

                var before = CaptureFloor(controller, "beforeSelect", false);
                document.floors.Add(before);

                var selectable = controller.GetSelectableMapNodes();
                if (selectable.Length == 0)
                {
                    document.traversalStopReason = "no selectable map node";
                    break;
                }

                var selection = controller.SelectMapNode(selectable[0].MapNodeId);
                document.floors.Add(CaptureFloor(controller, "afterSelect", selection.HasEncounter));
                if (!selection.HasEncounter)
                {
                    document.traversalStopReason = "selected node has no encounter";
                    break;
                }

                ResolveSelection(controller, selection);
                document.floors.Add(CaptureFloor(controller, "afterResolve", true));
            }

            if (string.IsNullOrEmpty(document.traversalStopReason))
            {
                document.traversalStopReason = controller.RunState != null && controller.RunState.RunCompleted
                    ? "run completed"
                    : guard >= 80 ? "traversal guard exceeded" : "traversal ended";
            }

            Directory.CreateDirectory(Path.GetDirectoryName(DumpPath));
            File.WriteAllText(DumpPath, JsonUtility.ToJson(document, true));
            Assert.IsTrue(File.Exists(DumpPath));
        }

        private static void PrepareNormalRun(PrototypeRoomController controller)
        {
            controller.BeginRun();
            controller.AutoResolveCombat = true;
            if (controller.RunState != null)
            {
                controller.RunState.AutoResolveCombat = true;
            }

            controller.ConfirmPreRunPlaceholder();
        }

        private static void ResolveSelection(PrototypeRoomController controller, EncounterSelection selection)
        {
            if (selection.Encounter.Type == EncounterType.Rest)
            {
                controller.ResolveCurrentRouteRestInteraction(selection, "rest.recover", string.Empty);
                return;
            }

            var choiceStableId = SelectChoiceStableId(controller.BuildEncounterChoiceViews(selection), selection.Encounter.Type);
            if (string.IsNullOrEmpty(choiceStableId))
            {
                controller.ResolveCurrentRouteNode(selection);
                return;
            }

            controller.ResolveCurrentRouteChoice(selection, choiceStableId);
        }

        private static string SelectChoiceStableId(PrototypeEncounterChoiceView[] views, EncounterType type)
        {
            var first = string.Empty;
            for (var i = 0; i < views.Length; i++)
            {
                var view = views[i];
                if (!view.Visible || !view.Enabled)
                {
                    continue;
                }

                if (type == EncounterType.Shop && view.ChoiceStableId.IndexOf("_LEAVE", StringComparison.Ordinal) >= 0)
                {
                    return view.ChoiceStableId;
                }

                if (string.IsNullOrEmpty(first))
                {
                    first = view.ChoiceStableId;
                }
            }

            return first;
        }

        private static MapFlowFloorDump CaptureFloor(PrototypeRoomController controller, string phase, bool selectionApplied)
        {
            var snapshot = controller.GetSnapshot();
            var runState = controller.RunState;
            Assert.IsNotNull(runState);
            var skippedNodeIds = GetSkippedMapNodeIds(runState);
            var floorComplete = runState.FloorComplete;
            var floor = new MapFlowFloorDump
            {
                floor = snapshot.CurrentFloor,
                phase = phase,
                selectedMapNodeId = snapshot.SelectedMapNodeId,
                selectionApplied = selectionApplied,
                activeMapLayer = GetActiveMapLayer(runState),
                floorActive = !floorComplete && !runState.RunCompleted,
                floorComplete = floorComplete,
                stairUnlocked = runState.StairUnlocked,
                runCompleted = runState.RunCompleted,
                runFailed = runState.RunFailed,
                runClear = runState.RunClear
            };

            var nodes = controller.GetQaFloorMapNodes();
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                floor.nodes.Add(new MapFlowNodeDump
                {
                    MapNodeId = node.MapNodeId,
                    Type = node.Type.ToString(),
                    Floor = node.Floor,
                    Layer = node.Layer,
                    Index = node.Index,
                    Selectable = node.Selectable,
                    Completed = node.Completed,
                    Skipped = skippedNodeIds.Contains(node.MapNodeId),
                    Locked = node.Locked,
                    NormalizedX = node.NormalizedX,
                    NormalizedY = node.NormalizedY,
                    Current = node.Current,
                    NextMapNodeIds = node.NextMapNodeIds,
                    PreviousMapNodeIds = node.PreviousMapNodeIds
                });
            }

            return floor;
        }

        private static int GetActiveMapLayer(PrototypeRunState runState)
        {
            Assert.IsNotNull(ActiveMapLayerMethod, "M3 diagnostic requires the runtime active-map-layer result.");
            return (int)ActiveMapLayerMethod.Invoke(runState, null);
        }

        private static HashSet<string> GetSkippedMapNodeIds(PrototypeRunState runState)
        {
            Assert.IsNotNull(FloorMapNodesField, "M3 diagnostic requires runtime skipped-node state.");
            var nodes = FloorMapNodesField.GetValue(runState) as System.Collections.IEnumerable;
            Assert.IsNotNull(nodes);

            var skippedNodeIds = new HashSet<string>();
            foreach (var node in nodes)
            {
                if (node == null)
                {
                    continue;
                }

                var nodeType = node.GetType();
                var mapNodeIdProperty = nodeType.GetProperty("MapNodeId");
                var skippedProperty = nodeType.GetProperty("Skipped");
                Assert.IsNotNull(mapNodeIdProperty);
                Assert.IsNotNull(skippedProperty);
                if ((bool)skippedProperty.GetValue(node))
                {
                    skippedNodeIds.Add((string)mapNodeIdProperty.GetValue(node));
                }
            }

            return skippedNodeIds;
        }

        private static void ResetRunStateIsolation()
        {
            PrototypeRunSaveStore.Delete();
            PrototypeRunSaveRequest.RequestNewGame();
            Time.timeScale = 1f;
        }

        [Serializable]
        private sealed class MapFlowDumpDocument
        {
            public string source;
            public string runId;
            public string traversalStopReason;
            public List<MapFlowFloorDump> floors = new List<MapFlowFloorDump>();
            public MapFlowFloorDump determinismPairA;
            public MapFlowFloorDump determinismPairB;
        }

        [Serializable]
        private sealed class MapFlowFloorDump
        {
            public int floor;
            public string phase;
            public string selectedMapNodeId;
            public bool selectionApplied;
            public int activeMapLayer;
            public bool floorActive;
            public bool floorComplete;
            public bool stairUnlocked;
            public bool runCompleted;
            public bool runFailed;
            public bool runClear;
            public List<MapFlowNodeDump> nodes = new List<MapFlowNodeDump>();
        }

        [Serializable]
        private sealed class MapFlowNodeDump
        {
            public string MapNodeId;
            public string Type;
            public int Floor;
            public int Layer;
            public int Index;
            public bool Selectable;
            public bool Completed;
            public bool Skipped;
            public bool Locked;
            public float NormalizedX;
            public float NormalizedY;
            public bool Current;
            public string[] NextMapNodeIds;
            public string[] PreviousMapNodeIds;
        }
    }
}
