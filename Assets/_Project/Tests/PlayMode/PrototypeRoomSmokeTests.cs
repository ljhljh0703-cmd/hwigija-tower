using System.Collections;
using System.Collections.Generic;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.Run;
using HwigiTower.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace HwigiTower.Tests.PlayMode
{
    public sealed class PrototypeRoomSmokeTests
    {
        [UnityTest]
        public IEnumerator PrototypeRoom_LoadsRuntimeAndPlayer()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            Assert.IsNotNull(Object.FindFirstObjectByType<Run.PrototypeRoomController>());
            Assert.IsNotNull(GameObject.Find("Player"));
            Assert.IsNotNull(GameObject.Find("Prototype HUD"));
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_AttachesEncounterRuntimeCatalogToRunState()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.EncounterRuntimeCatalog);

            controller.BeginRun();
            Assert.IsNotNull(controller.RunState);
            Assert.IsNotNull(controller.RunState.EncounterCatalog);

            controller.RunState.ModifyGold(5);
            var encounter = CreateEncounterFromJson(
                "{\"id\":\"encounter.scene.catalog.shop\",\"type\":2,\"floor\":2,\"choices\":[{\"stableId\":\"choice.scene.buy\",\"textKey\":\"PLACEHOLDER_CHOICE_SCENE_BUY\",\"requirementMode\":\"All\",\"unavailablePolicyMode\":\"DisabledVisible\",\"unavailableReasonTextKey\":\"PLACEHOLDER_REASON_NOT_ENOUGH_GOLD\",\"requirements\":[{\"kind\":\"StatAtLeast\",\"stat\":\"gold\",\"value\":5}],\"effects\":[{\"kind\":\"ModifyGold\",\"amount\":-5},{\"kind\":\"AddItem\",\"itemRef\":\"ITEM_FIELD_BANDAGE\",\"count\":1}]}]}");

            var resolution = controller.RunState.ResolveEncounterChoice(
                new DeterministicRunContext("run-scene-catalog", 1001),
                "node.scene.catalog.shop",
                encounter,
                "choice.scene.buy");

            Assert.AreEqual("choice.scene.buy", resolution.PayloadId);
            Assert.AreEqual(1, controller.RunState.GetItemCount("ITEM_FIELD_BANDAGE"));
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_BakedEncounterChoicesUseButtonFlow()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            var shopNode = FindNode("node.shop");
            var battleNode = FindNode("node.battle");
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsNotNull(shopNode);
            Assert.IsNotNull(battleNode);

            controller.BeginRun();

            var shopEncounter = FindEncounter(shopNode, "ENC_SHOP_01");
            var shopSelection = new EncounterSelection(shopNode.Definition, shopEncounter);
            var shopViews = controller.BuildEncounterChoiceViews(shopSelection);
            hud.ShowChoices(shopEncounter, shopViews, choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(shopNode, shopEncounter, choiceStableId);
                hud.ShowInteraction(shopNode, shopSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });

            Assert.AreEqual(3, hud.ChoiceButtonCount);
            AssertChoiceLayout(hud);
            var insufficientButton = FindChoiceButton(hud, "CHOICE_SHOP_01_BUY_ITEM");
            Assert.IsFalse(insufficientButton.interactable);
            StringAssert.Contains("PLACEHOLDER_REASON_NOT_ENOUGH_GOLD", ReadButtonText(insufficientButton));

            controller.RunState.ModifyGold(5);
            hud.ShowChoices(shopEncounter, controller.BuildEncounterChoiceViews(shopSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(shopNode, shopEncounter, choiceStableId);
                hud.ShowInteraction(shopNode, shopSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_SHOP_01_BUY_ITEM").onClick.Invoke();
            Assert.AreEqual(0, controller.RunState.Gold);
            Assert.AreEqual(1, controller.RunState.GetItemCount("ITEM_FIELD_BANDAGE"));
            Assert.AreEqual(0, hud.ChoiceButtonCount);
            StringAssert.Contains("choice applied: CHOICE_SHOP_01_BUY_ITEM", hud.ResultMessage);

            var revisit = controller.ResolveEncounterChoice(shopNode, shopEncounter, "CHOICE_SHOP_01_BUY_ITEM");
            Assert.AreEqual(1, controller.RunState.GetItemCount("ITEM_FIELD_BANDAGE"));
            StringAssert.Contains("already resolved: CHOICE_SHOP_01_BUY_ITEM", revisit.Message);
            hud.ShowChoices(shopEncounter, controller.BuildEncounterChoiceViews(shopSelection), _ => { });
            Assert.AreEqual(0, hud.ChoiceButtonCount);

            var moralEncounter = FindEncounter(battleNode, "ENC_MORAL_CHOICE_01");
            var moralSelection = new EncounterSelection(battleNode.Definition, moralEncounter);
            var affinityBeforeMoral = controller.RunState.Affinity;
            var glitchBeforeMoral = controller.RunState.GlitchLevel;
            hud.ShowChoices(moralEncounter, controller.BuildEncounterChoiceViews(moralSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, moralEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, moralSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_MORAL_01_REFUSE").onClick.Invoke();
            Assert.AreEqual(affinityBeforeMoral - 5, controller.RunState.Affinity);
            Assert.AreEqual(glitchBeforeMoral + 4, controller.RunState.GlitchLevel);

            var memoryEncounter = FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01");
            var memorySelection = new EncounterSelection(battleNode.Definition, memoryEncounter);
            hud.ShowChoices(memoryEncounter, controller.BuildEncounterChoiceViews(memorySelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, memoryEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, memorySelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            Assert.AreEqual(2, hud.ChoiceButtonCount);
            AssertChoiceLayout(hud);
            FindChoiceButton(hud, "CHOICE_MEMORY_01_UNLOCK").onClick.Invoke();
            Assert.IsTrue(controller.RunState.HasMemoryFragmentRef("MEM_FRAGMENT_01"));

            hud.ShowChoices(memoryEncounter, controller.BuildEncounterChoiceViews(memorySelection), _ => { });
            Assert.AreEqual(0, hud.ChoiceButtonCount);

            var events = new List<GameFlowEventType>();
            using (controller.EventBus.Subscribe(flowEvent => events.Add(flowEvent.Type)))
            {
                var combatEncounter = FindEncounter(battleNode, "ENC_COMBAT_GATE_01");
                var combatSelection = new EncounterSelection(battleNode.Definition, combatEncounter);
                hud.ShowChoices(combatEncounter, controller.BuildEncounterChoiceViews(combatSelection), choiceStableId =>
                {
                    var resolution = controller.ResolveEncounterChoice(battleNode, combatEncounter, choiceStableId);
                    hud.ShowInteraction(battleNode, combatSelection, resolution);
                    hud.ShowRunState(controller.GetSnapshot());
                });
                FindChoiceButton(hud, "CHOICE_COMBAT_01_ENGAGE").onClick.Invoke();
            }

            CollectionAssert.Contains(events, GameFlowEventType.CombatStarted);
            CollectionAssert.Contains(events, GameFlowEventType.CombatCompleted);
            Assert.IsTrue(controller.RunState.RunCompleted);
            Assert.AreEqual("demo.complete", controller.RunState.DemoStatus);
            StringAssert.Contains("demo.complete", hud.ResultMessage);
        }

        [UnityTest]
        public IEnumerator BakedEncounterRuntime_ShowsChoicesAndAppliesShopPurchase()
        {
            yield return null;

            var state = new PrototypeRunState("run-playmode-shop", new GameFlowEventBus());
            state.ModifyGold(6);
            var encounter = CreateEncounterFromJson(
                "{\"id\":\"encounter.shop.playmode\",\"type\":2,\"floor\":2,\"choices\":[{\"stableId\":\"choice.buy\",\"textKey\":\"PLACEHOLDER_CHOICE_BUY\",\"requirementMode\":\"All\",\"unavailablePolicyMode\":\"DisabledVisible\",\"unavailableReasonTextKey\":\"PLACEHOLDER_REASON_NOT_ENOUGH_GOLD\",\"requirements\":[{\"kind\":\"StatAtLeast\",\"stat\":\"gold\",\"value\":6}],\"effects\":[{\"kind\":\"ModifyGold\",\"amount\":-6},{\"kind\":\"AddItem\",\"itemRef\":\"ITEM_PLAYMODE_BANDAGE\",\"count\":1}]}]}");

            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter);
            var resolution = state.ResolveEncounterChoice(new DeterministicRunContext("run-playmode-shop", 1001), "node.shop.playmode", encounter, "choice.buy");
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual(1, views.Length);
            Assert.IsTrue(views[0].Visible);
            Assert.IsTrue(views[0].Enabled);
            Assert.AreEqual("choice.buy", resolution.PayloadId);
            Assert.AreEqual(0, snapshot.Gold);
            Assert.AreEqual(1, state.GetItemCount("ITEM_PLAYMODE_BANDAGE"));
        }

        [UnityTest]
        public IEnumerator BakedEncounterRuntime_CombatHandoffStartsCombatFlow()
        {
            yield return null;

            var bus = new GameFlowEventBus();
            var events = new List<GameFlowEventType>();
            using (bus.Subscribe(flowEvent => events.Add(flowEvent.Type)))
            {
                var state = new PrototypeRunState("run-playmode-combat", bus);
                var encounter = CreateEncounterFromJson(
                    "{\"id\":\"encounter.combat.playmode\",\"type\":0,\"floor\":1,\"choices\":[{\"stableId\":\"choice.fight\",\"textKey\":\"PLACEHOLDER_CHOICE_FIGHT\",\"requirementMode\":\"All\",\"unavailablePolicyMode\":\"Hidden\",\"requirements\":[],\"effects\":[{\"kind\":\"StartCombat\",\"combatHandoff\":{\"stableId\":\"COMBAT_PLAYMODE_001\",\"seedKey\":\"COMBAT_PLAYMODE_001\",\"enemyRefs\":[\"ENEMY_PLAYMODE_001\"],\"onVictoryEffects\":[{\"kind\":\"ModifyGold\",\"amount\":8}],\"onDefeatEffects\":[]}}]}]}");

                var resolution = state.ResolveEncounterChoice(new DeterministicRunContext("run-playmode-combat", 1001), "node.combat.playmode", encounter, "choice.fight");
                var snapshot = state.CreateSnapshot();

                Assert.AreEqual("choice.fight", resolution.PayloadId);
                Assert.AreEqual(8, snapshot.Gold);
                CollectionAssert.Contains(events, GameFlowEventType.CombatStarted);
                CollectionAssert.Contains(events, GameFlowEventType.CombatCompleted);
            }
        }

        private static EncounterData CreateEncounterFromJson(string json)
        {
            var encounter = ScriptableObject.CreateInstance<EncounterData>();
            JsonUtility.FromJsonOverwrite(json, encounter);
            return encounter;
        }

        private static InteractableNode FindNode(string nodeId)
        {
            var nodes = Object.FindObjectsByType<InteractableNode>(FindObjectsSortMode.None);
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null && nodes[i].Definition != null && nodes[i].Definition.NodeId == nodeId)
                {
                    return nodes[i];
                }
            }

            return null;
        }

        private static EncounterData FindEncounter(InteractableNode node, string encounterId)
        {
            Assert.IsNotNull(node);
            Assert.IsNotNull(node.Definition);

            var encounters = node.Definition.PossibleEncounters;
            for (var i = 0; i < encounters.Count; i++)
            {
                if (encounters[i] != null && encounters[i].Id == encounterId)
                {
                    return encounters[i];
                }
            }

            Assert.Fail($"Missing encounter {encounterId} on node {node.Definition.NodeId}");
            return null;
        }

        private static Button FindChoiceButton(PrototypeHud hud, string choiceStableId)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.name == $"Choice Button {choiceStableId}")
                {
                    return button;
                }
            }

            return null;
        }

        private static string ReadButtonText(Button button)
        {
            Assert.IsNotNull(button);
            var text = button.GetComponentInChildren<Text>();
            Assert.IsNotNull(text);
            return text.text;
        }

        private static void AssertChoiceLayout(PrototypeHud hud)
        {
            Assert.GreaterOrEqual(hud.ChoiceButtonCount, 2);
            Assert.LessOrEqual(hud.ChoiceButtonCount, 3);

            var previousY = float.PositiveInfinity;
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                Assert.IsNotNull(button);
                var rect = button.GetComponent<RectTransform>();
                Assert.IsNotNull(rect);
                Assert.GreaterOrEqual(rect.sizeDelta.y, 96f);
                Assert.Less(rect.anchoredPosition.y, previousY);
                previousY = rect.anchoredPosition.y;
            }
        }
    }
}
