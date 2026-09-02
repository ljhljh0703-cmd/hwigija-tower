using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        [Explicit("M2 diagnostic dump: records runtime map state without changing map logic.")]
        public IEnumerator MapFlowM2_WritesBeforeAndAfterRuntimeDump()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = UnityEngine.Object.FindFirstObjectByType<PrototypeRoomController>();
            Assert.IsNotNull(controller);
            controller.ConfirmPreRunPlaceholder();

            var document = new MapFlowDumpDocument
            {
                source = "runtime",
                runId = controller.RunState == null ? string.Empty : controller.RunState.RunId
            };

            for (var floor = 1; floor <= 5; floor++)
            {
                Assert.IsTrue(controller.OpenQaFloor(floor), "Missing runtime floor path " + floor);
                var before = CaptureFloor(controller, "beforeSelect", false);
                document.floors.Add(before);

                var selectable = controller.GetSelectableMapNodes();
                var selectionApplied = false;
                if (selectable.Length > 0)
                {
                    var selection = controller.SelectMapNode(selectable[0].MapNodeId);
                    selectionApplied = selection.HasEncounter;
                }

                document.floors.Add(CaptureFloor(controller, "afterSelect", selectionApplied));
            }

            Assert.IsTrue(controller.OpenQaFloor(1));
            document.determinismPairA = CaptureFloor(controller, "beforeSelect", false);
            Assert.IsTrue(controller.OpenQaFloor(1));
            document.determinismPairB = CaptureFloor(controller, "beforeSelect", false);

            Directory.CreateDirectory(Path.GetDirectoryName(DumpPath));
            File.WriteAllText(DumpPath, JsonUtility.ToJson(document, true));
            Assert.IsTrue(File.Exists(DumpPath));
        }

        private static MapFlowFloorDump CaptureFloor(PrototypeRoomController controller, string phase, bool selectionApplied)
        {
            var snapshot = controller.GetSnapshot();
            var floor = new MapFlowFloorDump
            {
                floor = snapshot.CurrentFloor,
                phase = phase,
                selectedMapNodeId = snapshot.SelectedMapNodeId,
                selectionApplied = selectionApplied
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
            public bool Locked;
            public float NormalizedX;
            public float NormalizedY;
            public bool Current;
            public string[] NextMapNodeIds;
            public string[] PreviousMapNodeIds;
        }
    }
}
