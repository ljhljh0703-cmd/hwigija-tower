using System.IO;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.Run;
using NUnit.Framework;
using UnityEditor;

namespace HwigiTower.Tests.EditMode
{
    public sealed class PrototypeRunSaveTests
    {
        private string _savePath;

        [SetUp]
        public void SetUp()
        {
            _savePath = Path.Combine(Path.GetTempPath(), "hwigi-prototype-save-test.json");
            PrototypeRunSaveStore.Delete(_savePath);
        }

        [TearDown]
        public void TearDown()
        {
            PrototypeRunSaveStore.Delete(_savePath);
        }

        [Test]
        public void SaveStore_MissingSaveIsSafe()
        {
            Assert.IsFalse(PrototypeRunSaveStore.HasSave(_savePath));
            Assert.IsFalse(PrototypeRunSaveStore.TryLoad(out _, _savePath));
            Assert.IsFalse(PrototypeRunSaveStore.TryLoadSummary(out _, _savePath));
        }

        [Test]
        public void SaveStore_RoundTripsDeterministicRunData()
        {
            var data = new PrototypeRunSaveData
            {
                runId = "run-save-roundtrip",
                currentFloor = 3,
                playerHp = 14,
                playerMaxHp = 24,
                gold = 11,
                memoryFragmentRefs = new[] { "MEM_FRAGMENT_01", "MEM_FRAGMENT_03" },
                resolvedDemoStepKeys = new[] { "node/encounter" }
            };

            PrototypeRunSaveStore.Save(data, _savePath);

            Assert.IsTrue(PrototypeRunSaveStore.TryLoad(out var loaded, _savePath));
            Assert.AreEqual("run-save-roundtrip", loaded.runId);
            Assert.AreEqual(3, loaded.currentFloor);
            Assert.AreEqual(14, loaded.playerHp);
            Assert.AreEqual(2, loaded.memoryFragmentRefs.Length);
            Assert.IsTrue(PrototypeRunSaveStore.TryLoadSummary(out var summary, _savePath));
            Assert.AreEqual("Floor 3 | HP 14/24 | 기억 2", summary.DisplayText);
        }

        [Test]
        public void RunState_SaveRestoreKeepsRunFloorNodeAndMemoryState()
        {
            var room = LoadRoom();
            var catalog = LoadCatalog();
            var state = new PrototypeRunState("run-save-state", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachEncounterCatalog(catalog);
            state.AttachFloorRunPaths(room.FloorRunPaths, room.DemoRunPath);
            state.ModifyGold(12);
            Assert.IsTrue(state.UnlockMemoryFragmentRef("MEM_FRAGMENT_01"));

            var selectable = state.GetSelectableMapNodeViews();
            Assert.Greater(selectable.Length, 0);
            Assert.IsTrue(state.TrySelectMapNode(selectable[0].MapNodeId, out var step));
            var choice = FirstChoiceId(step.Encounter);
            state.ResolveEncounterChoice(new DeterministicRunContext(state.RunId, 1001), step.NodeId, step.Encounter, choice);
            var saved = state.CreateSaveData();

            var restored = new PrototypeRunState(saved.runId, new GameFlowEventBus()) { AutoResolveCombat = true };
            restored.AttachEncounterCatalog(catalog);
            restored.RestoreFromSaveData(saved, room.FloorRunPaths, room.DemoRunPath);

            Assert.AreEqual("run-save-state", restored.RunId);
            Assert.AreEqual(state.CurrentFloor, restored.CurrentFloor);
            Assert.AreEqual(state.Gold, restored.Gold);
            Assert.IsTrue(restored.HasMemoryFragmentRef("MEM_FRAGMENT_01"));
            Assert.AreEqual(state.NodesResolved, restored.NodesResolved);
            var restoredSnapshot = restored.CreateSnapshot();
            Assert.IsTrue(HasCompletedMapNode(restoredSnapshot, selectable[0].MapNodeId));
            Assert.IsTrue(TryGetMapNode(state.CreateSnapshot(), selectable[0].MapNodeId, out var originalNode));
            Assert.IsTrue(TryGetMapNode(restoredSnapshot, selectable[0].MapNodeId, out var restoredNode));
            Assert.AreEqual(originalNode.NormalizedX, restoredNode.NormalizedX);
            Assert.AreEqual(originalNode.NormalizedY, restoredNode.NormalizedY);
            CollectionAssert.AreEqual(originalNode.NextMapNodeIds, restoredNode.NextMapNodeIds);
        }

        [Test]
        public void RoomDefinition_FloorThreeFiveShopRoutesUseDedicatedShopEncounters()
        {
            var room = LoadRoom();

            AssertFloorShop(room, 3, "ENC_SHOP_03");
            AssertFloorShop(room, 4, "ENC_SHOP_04");
            AssertFloorShop(room, 5, "ENC_SHOP_05");
        }

        [Test]
        public void SaveData_ExplicitlyExcludesReflectionAndLlmCache()
        {
            var state = new PrototypeRunState("run-save-policy", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.MemoryRepo.SaveReflection(new HwigiTower.NPC.RunReflection("run-save-policy", "hash", "summary"));

            var data = state.CreateSaveData();

            Assert.AreEqual("run-save-policy", data.runId);
            Assert.IsNull(data.GetType().GetField("reflections"));
            Assert.IsNull(data.GetType().GetField("llmCache"));
        }

        private static PrototypeRoomDefinition LoadRoom()
        {
            var room = AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            Assert.IsNotNull(room);
            return room;
        }

        private static EncounterRuntimeCatalogData LoadCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            Assert.IsNotNull(catalog);
            return catalog;
        }

        private static string FirstChoiceId(EncounterData encounter)
        {
            Assert.IsNotNull(encounter);
            Assert.IsNotNull(encounter.Choices);
            Assert.Greater(encounter.Choices.Length, 0);
            return encounter.Choices[0].stableId;
        }

        private static void AssertFloorShop(PrototypeRoomDefinition room, int floor, string expectedEncounterId)
        {
            var steps = room.GetRunPathForFloor(floor);
            Assert.IsNotNull(steps, "Missing floor path " + floor);

            var shopCount = 0;
            EncounterData shop = null;
            for (var i = 0; i < steps.Count; i++)
            {
                var encounter = steps[i] == null ? null : steps[i].Encounter;
                if (encounter != null && encounter.Type == EncounterType.Shop)
                {
                    shopCount++;
                    shop = encounter;
                }
            }

            Assert.AreEqual(1, shopCount, "Expected exactly one shop on floor " + floor);
            Assert.IsNotNull(shop);
            Assert.AreEqual(expectedEncounterId, shop.Id);
            Assert.AreEqual(floor, shop.Floor);
            Assert.IsNotNull(shop.Choices);
            Assert.GreaterOrEqual(shop.Choices.Length, 3);
        }

        private static bool HasCompletedMapNode(PrototypeRunSnapshot snapshot, string mapNodeId)
        {
            for (var i = 0; i < snapshot.FloorMapNodes.Length; i++)
            {
                var node = snapshot.FloorMapNodes[i];
                if (node.MapNodeId == mapNodeId && node.Completed)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetMapNode(PrototypeRunSnapshot snapshot, string mapNodeId, out PrototypeFloorMapNodeView node)
        {
            for (var i = 0; i < snapshot.FloorMapNodes.Length; i++)
            {
                if (snapshot.FloorMapNodes[i].MapNodeId == mapNodeId)
                {
                    node = snapshot.FloorMapNodes[i];
                    return true;
                }
            }

            node = default;
            return false;
        }
    }
}
