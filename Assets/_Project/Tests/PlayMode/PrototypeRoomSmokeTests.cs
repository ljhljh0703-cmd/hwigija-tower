using System.Collections;
using System.Collections.Generic;
using HwigiTower.Combat;
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
        public IEnumerator PrototypeRoom_HidesDebugNodeLabelsByDefaultAndRestoresWithToggle()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            Assert.IsNotNull(controller);
            Assert.AreEqual(0, CountActiveWorldNodeLabels());

            controller.SetDemoNodeDebugLabelsVisible(true);
            Assert.GreaterOrEqual(CountActiveWorldNodeLabels(), 4);

            controller.SetDemoNodeDebugLabelsVisible(false);
            Assert.AreEqual(0, CountActiveWorldNodeLabels());
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

            controller.AutoResolveCombat = true;
            controller.BeginRun();
            hud.SetRawDebugTextVisible(true);
            hud.ShowRunState(controller.GetSnapshot());
            StringAssert.Contains("[current] Shop", hud.RouteMessage);

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
            var insufficientButton = FindChoiceButton(hud, "CHOICE_SHOP_01_BUY_ABILITY");
            Assert.IsFalse(insufficientButton.interactable);
            StringAssert.Contains("PLACEHOLDER_REASON_NOT_ENOUGH_GOLD", ReadButtonText(insufficientButton));

            hud.ShowChoices(shopEncounter, controller.BuildEncounterChoiceViews(shopSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(shopNode, shopEncounter, choiceStableId);
                hud.ShowInteraction(shopNode, shopSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_SHOP_01_BUY_ITEM").onClick.Invoke();
            Assert.AreEqual(1, controller.RunState.Gold);
            Assert.AreEqual(1, controller.RunState.GetItemCount("ITEM_FIELD_BANDAGE"));
            Assert.AreEqual(0, hud.ChoiceButtonCount);
            StringAssert.Contains("choice applied: CHOICE_SHOP_01_BUY_ITEM", hud.ResultMessage);
            StringAssert.Contains("Gold -5", hud.ResultMessage);
            StringAssert.Contains("item ITEM_FIELD_BANDAGE +1", hud.ResultMessage);

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
            StringAssert.Contains("Affinity -5", hud.ResultMessage);
            StringAssert.Contains("Glitch +4", hud.ResultMessage);
            StringAssert.Contains("NPC:", hud.MemoryMessage);

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
            StringAssert.Contains("memory unlocked MEM_FRAGMENT_01", hud.ResultMessage);
            StringAssert.Contains("MEM_FRAGMENT_01", hud.MemoryMessage);

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
            Assert.IsFalse(controller.RunState.RunCompleted);
            Assert.IsTrue(controller.RunState.StairUnlocked);
            Assert.AreEqual("stair.unlocked", controller.RunState.DemoStatus);
            StringAssert.Contains("combat started COMBAT_GATE_01", hud.ResultMessage);
            StringAssert.Contains("enemy ENEMY_FRACTURE_HOUND", hud.ResultMessage);
            StringAssert.Contains("stair unlocked", hud.ResultMessage);
            StringAssert.Contains("COMBAT_GATE_01", hud.MemoryMessage);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_CombatGateShowsInteractiveCombatPanel()
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

            controller.AutoResolveCombat = false;
            controller.BeginRun();
            hud.ShowRunState(controller.GetSnapshot());
            Assert.IsTrue(hud.PortraitVisible);

            var shopEncounter = FindEncounter(shopNode, "ENC_SHOP_01");
            controller.ResolveEncounterChoice(shopNode, shopEncounter, "CHOICE_SHOP_01_BUY_ITEM");
            var moralEncounter = FindEncounter(battleNode, "ENC_MORAL_CHOICE_01");
            controller.ResolveEncounterChoice(battleNode, moralEncounter, "CHOICE_MORAL_01_REFUSE");
            var memoryEncounter = FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01");
            controller.ResolveEncounterChoice(battleNode, memoryEncounter, "CHOICE_MEMORY_01_UNLOCK");

            var combatEncounter = FindEncounter(battleNode, "ENC_COMBAT_GATE_01");
            var combatSelection = new EncounterSelection(battleNode.Definition, combatEncounter);
            hud.ShowChoices(combatEncounter, controller.BuildEncounterChoiceViews(combatSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, combatEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, combatSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_COMBAT_01_ENGAGE").onClick.Invoke();

            Assert.IsTrue(controller.RunState.IsInCombat);
            Assert.IsTrue(hud.CombatPanelVisible);
            StringAssert.Contains("Combat", hud.CombatMessage);
            StringAssert.Contains("Enemy HP", hud.CombatMessage);
            StringAssert.DoesNotContain("ENEMY_", hud.CombatMessage);
            Assert.IsFalse(GameObject.Find("Demo Complete Text") != null && GameObject.Find("Demo Complete Text").activeInHierarchy);
            var beforeEnemyHp = controller.GetSnapshot().EnemyHp;
            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            var defendButton = GameObject.Find("Combat Button Defend").GetComponent<Button>();
            var skillButton = GameObject.Find("Combat Button Skill").GetComponent<Button>();
            Assert.IsNotNull(attackButton);
            Assert.IsNotNull(defendButton);
            Assert.IsNotNull(skillButton);
            Assert.IsFalse(skillButton.interactable);

            attackButton.onClick.Invoke();
            yield return null;
            Assert.Less(controller.GetSnapshot().EnemyHp, beforeEnemyHp);

            var hpBeforeDefend = controller.GetSnapshot().PlayerHp;
            defendButton.onClick.Invoke();
            yield return null;
            StringAssert.Contains("Defend", controller.GetSnapshot().LastCombatRoundResult);
            Assert.LessOrEqual(hpBeforeDefend - controller.GetSnapshot().PlayerHp, 3);

            var guard = 0;
            while (controller.RunState.IsInCombat && guard < 12)
            {
                attackButton.onClick.Invoke();
                yield return null;
                guard++;
            }

            Assert.IsFalse(controller.RunState.IsInCombat);
            Assert.AreEqual("stair.unlocked", controller.RunState.DemoStatus);
            Assert.IsTrue(controller.RunState.StairUnlocked);
            StringAssert.Contains("Stair unlocked", hud.ResultMessage);
            Assert.IsFalse(hud.CombatPanelVisible);
            Assert.IsFalse(GameObject.Find("Demo Complete Text") != null && GameObject.Find("Demo Complete Text").activeInHierarchy);
            var nextFloorButton = GameObject.Find("Next Floor Button").GetComponent<Button>();
            Assert.IsTrue(nextFloorButton.gameObject.activeInHierarchy);
            nextFloorButton.onClick.Invoke();
            yield return null;
            Assert.AreEqual(2, controller.RunState.CurrentFloor);
            StringAssert.Contains("Floor 2", hud.RouteMessage);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_FloorTwoBossGateClearsRunAndRestarts()
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

            controller.AutoResolveCombat = true;
            controller.BeginRun();

            controller.ResolveEncounterChoice(shopNode, FindEncounter(shopNode, "ENC_SHOP_01"), "CHOICE_SHOP_01_BUY_ITEM");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MORAL_CHOICE_01"), "CHOICE_MORAL_01_REFUSE");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01"), "CHOICE_MEMORY_01_UNLOCK");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_COMBAT_GATE_01"), "CHOICE_COMBAT_01_ENGAGE");

            Assert.IsTrue(controller.RunState.StairUnlocked);
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());

            var floorTwoShop = controller.SelectEncounter(shopNode);
            Assert.AreEqual("ENC_F02_SHOP_001", floorTwoShop.EncounterId);
            controller.RunState.ModifyGold(12);
            var bandageBeforeFloorTwoShop = controller.RunState.GetItemCount("ITEM_FIELD_BANDAGE");
            controller.ResolveEncounterChoice(shopNode, floorTwoShop.Encounter, "CHOICE_F02_SHOP_BUY_ITEM");
            Assert.AreEqual(bandageBeforeFloorTwoShop + 1, controller.RunState.GetItemCount("ITEM_FIELD_BANDAGE"));
            var floorTwoMoral = controller.SelectEncounter(battleNode);
            Assert.AreEqual("ENC_F02_MORAL_CHOICE_001", floorTwoMoral.EncounterId);
            controller.ResolveEncounterChoice(battleNode, floorTwoMoral.Encounter, "CHOICE_F02_MORAL_LEAVE");
            hud.ShowRunState(controller.GetSnapshot());

            Assert.IsTrue(controller.RunState.BossGateUnlocked);
            StringAssert.Contains("Boss Gate", hud.RouteMessage);

            controller.AutoResolveCombat = false;
            var bossSelectionFromRoute = controller.SelectEncounter(battleNode);
            Assert.AreEqual("ENC_COMBAT_GATE_02", bossSelectionFromRoute.EncounterId);
            var bossEncounter = bossSelectionFromRoute.Encounter;
            var bossSelection = new EncounterSelection(battleNode.Definition, bossEncounter);
            hud.ShowChoices(bossEncounter, controller.BuildEncounterChoiceViews(bossSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, bossEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, bossSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_COMBAT_02_ENGAGE").onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.IsInCombat);
            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            Assert.IsNotNull(attackButton);

            var guard = 0;
            while (controller.RunState.IsInCombat && guard < 12)
            {
                attackButton.onClick.Invoke();
                yield return null;
                guard++;
            }

            Assert.IsTrue(controller.RunState.RunClear);
            Assert.IsFalse(controller.RunState.RestartReady);
            Assert.IsTrue(controller.RunState.EndingChoicePending);
            Assert.IsTrue(hud.EndingRestButtonVisible);
            Assert.IsTrue(hud.EndingContinueButtonVisible);
            StringAssert.Contains("Choose ending", hud.RouteMessage);
            StringAssert.Contains("Run Clear", hud.RunStateMessage);
            Assert.AreEqual(0, hud.ChoiceButtonCount);

            GameObject.Find("Ending Button Continue").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.EndingContinue);
            Assert.IsTrue(controller.RunState.RestartReady);
            Assert.IsFalse(hud.EndingRestButtonVisible);
            Assert.IsFalse(hud.EndingContinueButtonVisible);
            StringAssert.Contains("Ending continue", hud.RouteMessage);
            var restartButton = GameObject.Find("Restart Run Button").GetComponent<Button>();
            Assert.IsTrue(restartButton.gameObject.activeInHierarchy);
            var firstRunId = controller.RunContext.RunId;
            restartButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(firstRunId + ".restart.1", controller.RunState.RunId);
            Assert.AreEqual(1, controller.RunState.CurrentFloor);
            Assert.AreEqual(6, controller.RunState.Gold);
            Assert.IsTrue(controller.RunState.HasMemoryFragmentRef("MEM_FRAGMENT_01"));
            Assert.AreEqual(1, controller.GetSnapshot().MemoryFragmentCount);
            Assert.IsFalse(controller.RunState.RunCompleted);
            StringAssert.Contains("Shop", hud.RouteMessage);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_BossGateClearRestEndingLocksFinalState()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            var shopNode = FindNode("node.shop");
            var battleNode = FindNode("node.battle");
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);

            yield return ReachBossGateClear(controller, hud, shopNode, battleNode);
            Assert.IsTrue(hud.EndingRestButtonVisible);
            GameObject.Find("Ending Button Rest").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.EndingRest);
            Assert.IsFalse(controller.RunState.RestartReady);
            Assert.IsFalse(hud.EndingRestButtonVisible);
            Assert.IsFalse(hud.EndingContinueButtonVisible);
            var restartButton = GameObject.Find("Restart Run Button");
            Assert.IsTrue(restartButton == null || !restartButton.activeInHierarchy);
            StringAssert.Contains("Ending rest", hud.RouteMessage);

            var blocked = controller.ResolveEncounterChoice(shopNode, controller.SelectEncounter(shopNode).Encounter, "CHOICE_SHOP_01_BUY_ITEM");
            StringAssert.Contains("run already completed", blocked.Message);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_FloorTwoBossGateFailureShowsRestartState()
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

            controller.AutoResolveCombat = true;
            controller.BeginRun();
            controller.ResolveEncounterChoice(shopNode, FindEncounter(shopNode, "ENC_SHOP_01"), "CHOICE_SHOP_01_LEAVE");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MORAL_CHOICE_01"), "CHOICE_MORAL_01_REFUSE");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01"), "CHOICE_MEMORY_01_UNLOCK");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_COMBAT_GATE_01"), "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();

            var floorTwoShop = controller.SelectEncounter(shopNode);
            controller.ResolveEncounterChoice(shopNode, floorTwoShop.Encounter, "CHOICE_F02_SHOP_LEAVE");
            var floorTwoMoral = controller.SelectEncounter(battleNode);
            controller.ResolveEncounterChoice(battleNode, floorTwoMoral.Encounter, "CHOICE_F02_MORAL_LEAVE");
            controller.RunState.ModifyPlayerHp(-999);
            controller.RunState.ModifyPlayerHp(1);

            controller.AutoResolveCombat = false;
            var bossSelectionFromRoute = controller.SelectEncounter(battleNode);
            Assert.AreEqual("ENC_COMBAT_GATE_02", bossSelectionFromRoute.EncounterId);
            var bossEncounter = bossSelectionFromRoute.Encounter;
            var bossSelection = new EncounterSelection(battleNode.Definition, bossEncounter);
            hud.ShowChoices(bossEncounter, controller.BuildEncounterChoiceViews(bossSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, bossEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, bossSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_COMBAT_02_ENGAGE").onClick.Invoke();
            yield return null;

            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            Assert.IsNotNull(attackButton);
            attackButton.onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.RunFailed);
            Assert.IsTrue(controller.RunState.RestartReady);
            Assert.IsFalse(hud.CombatPanelVisible);
            Assert.AreEqual(0, hud.ChoiceButtonCount);
            StringAssert.Contains("Run failed", hud.RouteMessage);
            StringAssert.Contains("Run Failed", hud.RunStateMessage);
            StringAssert.Contains("Run failed", hud.ResultMessage);
            StringAssert.Contains("Restart available", hud.ResultMessage);

            var restartButton = GameObject.Find("Restart Run Button").GetComponent<Button>();
            Assert.IsTrue(restartButton.gameObject.activeInHierarchy);
            restartButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(1, controller.RunState.CurrentFloor);
            Assert.AreEqual(6, controller.RunState.Gold);
            Assert.AreEqual(controller.RunState.PlayerMaxHp, controller.RunState.PlayerHp);
            Assert.IsTrue(controller.RunState.HasMemoryFragmentRef("MEM_FRAGMENT_01"));
            Assert.AreEqual(1, controller.GetSnapshot().MemoryFragmentCount);
            Assert.IsFalse(controller.RunState.RunCompleted);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_FloorTwoBossGateRecallFallbackDelaysFailure()
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

            controller.AutoResolveCombat = true;
            controller.BeginRun();
            controller.ResolveEncounterChoice(shopNode, FindEncounter(shopNode, "ENC_SHOP_01"), "CHOICE_SHOP_01_LEAVE");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MORAL_CHOICE_01"), "CHOICE_MORAL_01_REFUSE");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01"), "CHOICE_MEMORY_01_UNLOCK");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_COMBAT_GATE_01"), "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            controller.ResolveEncounterChoice(shopNode, controller.SelectEncounter(shopNode).Encounter, "CHOICE_F02_SHOP_LEAVE");
            controller.ResolveEncounterChoice(battleNode, controller.SelectEncounter(battleNode).Encounter, "CHOICE_F02_MORAL_LEAVE");
            controller.RunState.AddAbilityRef("ABILITY_RECALL_ANCHOR");
            controller.RunState.ModifyPlayerHp(-999);
            controller.RunState.ModifyPlayerHp(1);

            controller.AutoResolveCombat = false;
            var bossSelectionFromRoute = controller.SelectEncounter(battleNode);
            var bossEncounter = bossSelectionFromRoute.Encounter;
            var bossSelection = new EncounterSelection(battleNode.Definition, bossEncounter);
            hud.ShowChoices(bossEncounter, controller.BuildEncounterChoiceViews(bossSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, bossEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, bossSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_COMBAT_02_ENGAGE").onClick.Invoke();
            yield return null;

            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            attackButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.RunState.RunFailed);
            Assert.IsFalse(controller.RunState.RunCompleted);
            Assert.IsTrue(controller.RunState.IsInCombat);
            Assert.AreEqual("recall", controller.RunState.LastCombatResultId);
            Assert.IsTrue(controller.RunState.HasFlag("FLAG_RECALL_ANCHOR_USED"));
            Assert.IsTrue(hud.CombatPanelVisible);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_CutsceneScaffoldDoesNotBlockRouteOrDuplicateTriggers()
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

            controller.AutoResolveCombat = false;
            controller.BeginRun();
            controller.ResolveEncounterChoice(shopNode, FindEncounter(shopNode, "ENC_SHOP_01"), "CHOICE_SHOP_01_BUY_ITEM");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MORAL_CHOICE_01"), "CHOICE_MORAL_01_REFUSE");

            var memoryEncounter = FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01");
            var memorySelection = new EncounterSelection(battleNode.Definition, memoryEncounter);
            hud.ShowInteraction(
                battleNode,
                memorySelection,
                controller.ResolveEncounterChoice(battleNode, memoryEncounter, "CHOICE_MEMORY_01_UNLOCK"));
            hud.ShowRunState(controller.GetSnapshot());
            hud.ShowRunState(controller.GetSnapshot());
            Assert.AreEqual(1, Object.FindObjectsByType<PrototypeCutscenePlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length);
            StringAssert.Contains("Combat", hud.RouteMessage);
            StringAssert.DoesNotContain("[current]", hud.RouteMessage);

            var combatEncounter = FindEncounter(battleNode, "ENC_COMBAT_GATE_01");
            var combatSelection = new EncounterSelection(battleNode.Definition, combatEncounter);
            hud.ShowInteraction(
                battleNode,
                combatSelection,
                controller.ResolveEncounterChoice(battleNode, combatEncounter, "CHOICE_COMBAT_01_ENGAGE"));
            hud.ShowRunState(controller.GetSnapshot());
            hud.ShowRunState(controller.GetSnapshot());
            Assert.AreEqual(1, Object.FindObjectsByType<PrototypeCutscenePlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length);
            Assert.IsTrue(controller.RunState.IsInCombat);
            Assert.IsTrue(hud.CombatPanelVisible);

            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            var guard = 0;
            while (controller.RunState.IsInCombat && guard < 12)
            {
                attackButton.onClick.Invoke();
                yield return null;
                guard++;
            }

            Assert.IsFalse(controller.RunState.IsInCombat);
            Assert.AreEqual("stair.unlocked", controller.RunState.DemoStatus);
            Assert.IsFalse(hud.CombatPanelVisible);
            StringAssert.Contains("Stair", hud.RouteMessage);
            Assert.AreEqual(1, Object.FindObjectsByType<PrototypeCutscenePlayer>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length);
            Assert.IsTrue(Object.FindFirstObjectByType<PrototypeCutscenePlayer>(FindObjectsInactive.Include).IsPlaying);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_DefaultPresentationHidesRawRouteAndChoiceKeys()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            var shopNode = FindNode("node.shop");
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsNotNull(shopNode);
            Assert.IsTrue(hud.HasPresentationData);

            controller.BeginRun();
            hud.ShowRunState(controller.GetSnapshot());
            StringAssert.Contains("Shop", hud.RouteMessage);
            StringAssert.DoesNotContain("[current]", hud.RouteMessage);
            StringAssert.DoesNotContain("MoralChoice", hud.RouteMessage);
            StringAssert.DoesNotContain("MemoryFragment", hud.RouteMessage);
            StringAssert.DoesNotContain("CombatGate", hud.RouteMessage);
            StringAssert.DoesNotContain("ENC_SHOP_01", hud.RouteMessage);
            StringAssert.DoesNotContain("node.shop", hud.RouteMessage);

            var shopEncounter = FindEncounter(shopNode, "ENC_SHOP_01");
            var shopSelection = new EncounterSelection(shopNode.Definition, shopEncounter);
            hud.ShowChoices(shopEncounter, controller.BuildEncounterChoiceViews(shopSelection), _ => { });
            var button = hud.GetChoiceButton(0);
            Assert.IsNotNull(button);
            var label = ReadButtonText(button);
            StringAssert.DoesNotContain("Choice 1", label);
            StringAssert.DoesNotContain("CHOICE_", label);
            StringAssert.DoesNotContain("PLACEHOLDER_", label);
        }

        [UnityTest]
        public IEnumerator BakedEncounterRuntime_ShowsChoicesAndAppliesShopPurchase()
        {
            yield return null;

            var state = new PrototypeRunState("run-playmode-shop", new GameFlowEventBus()) { AutoResolveCombat = true };
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
                var state = new PrototypeRunState("run-playmode-combat", bus) { AutoResolveCombat = true };
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

        private static IEnumerator ReachBossGateClear(Run.PrototypeRoomController controller, PrototypeHud hud, InteractableNode shopNode, InteractableNode battleNode)
        {
            controller.AutoResolveCombat = true;
            controller.BeginRun();
            controller.ResolveEncounterChoice(shopNode, FindEncounter(shopNode, "ENC_SHOP_01"), "CHOICE_SHOP_01_BUY_ITEM");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MORAL_CHOICE_01"), "CHOICE_MORAL_01_REFUSE");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_MEMORY_FRAGMENT_01"), "CHOICE_MEMORY_01_UNLOCK");
            controller.ResolveEncounterChoice(battleNode, FindEncounter(battleNode, "ENC_COMBAT_GATE_01"), "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            controller.RunState.ModifyGold(12);
            controller.ResolveEncounterChoice(shopNode, controller.SelectEncounter(shopNode).Encounter, "CHOICE_F02_SHOP_BUY_ITEM");
            controller.ResolveEncounterChoice(battleNode, controller.SelectEncounter(battleNode).Encounter, "CHOICE_F02_MORAL_LEAVE");
            hud.ShowRunState(controller.GetSnapshot());
            Assert.IsTrue(controller.RunState.BossGateUnlocked);

            controller.AutoResolveCombat = false;
            var bossSelectionFromRoute = controller.SelectEncounter(battleNode);
            Assert.AreEqual("ENC_COMBAT_GATE_02", bossSelectionFromRoute.EncounterId);
            var bossEncounter = bossSelectionFromRoute.Encounter;
            var bossSelection = new EncounterSelection(battleNode.Definition, bossEncounter);
            hud.ShowChoices(bossEncounter, controller.BuildEncounterChoiceViews(bossSelection), choiceStableId =>
            {
                var resolution = controller.ResolveEncounterChoice(battleNode, bossEncounter, choiceStableId);
                hud.ShowInteraction(battleNode, bossSelection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });
            FindChoiceButton(hud, "CHOICE_COMBAT_02_ENGAGE").onClick.Invoke();
            yield return null;

            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            var guard = 0;
            while (controller.RunState.IsInCombat && guard < 12)
            {
                attackButton.onClick.Invoke();
                yield return null;
                guard++;
            }

            hud.ShowRunState(controller.GetSnapshot());
            Assert.IsTrue(controller.RunState.RunClear);
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

        private static int CountActiveWorldNodeLabels()
        {
            var count = 0;
            var labels = Object.FindObjectsByType<TextMesh>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (label == null || label.gameObject.name != "Label" || !label.gameObject.activeInHierarchy || label.transform.parent == null)
                {
                    continue;
                }

                if (label.transform.parent.GetComponent<InteractableNode>() != null)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
