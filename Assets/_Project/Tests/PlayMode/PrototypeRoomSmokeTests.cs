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
        public IEnumerator PrototypeRoom_BranchingMapOpensJarEventWithProbabilityHints()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsTrue(hud.RouteActionButtonVisible);
            StringAssert.Contains("Floor 1", hud.RunStateMessage);
            StringAssert.DoesNotContain("Glitch", hud.RunStateMessage);

            hud.GetRouteActionButton().onClick.Invoke();
            yield return null;

            Assert.GreaterOrEqual(hud.ChoiceButtonCount, 2);
            var eventButton = FindMapChoiceButton(hud, "EVT_F01_JAR_ROOM");
            Assert.IsNotNull(eventButton, DescribeChoiceButtons(hud));
            eventButton.onClick.Invoke();
            yield return null;

            var patternedJar = FindChoiceButton(hud, "CHOICE_EVT_F01_JAR_PATTERNED");
            Assert.IsNotNull(patternedJar, DescribeChoiceButtons(hud));
            var label = ReadButtonText(patternedJar);
            StringAssert.Contains("신기한 문양이 각인된 항아리", label);
            StringAssert.Contains("80%: 골드 획득", label);
            StringAssert.Contains("20%: 엘리트 전투", label);
            StringAssert.DoesNotContain("CHOICE_", label);
            StringAssert.DoesNotContain("EVT_F01_JAR_ROOM", label);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_BranchingMapOpensInteractiveCombatNode()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);

            controller.AutoResolveCombat = false;
            hud.ShowRunState(controller.GetSnapshot());
            yield return ResolveRouteActionChoice(hud, "EVT_F01_JAR_ROOM", "CHOICE_EVT_F01_JAR_PLAIN");
            yield return ResolveRouteActionChoice(hud, "ENC_MORAL_CHOICE_01", "CHOICE_MORAL_01_REFUSE");
            yield return ResolveRouteActionChoice(hud, "ENC_SHOP_01", "CHOICE_SHOP_01_LEAVE");
            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");

            Assert.IsTrue(controller.RunState.IsInCombat);
            Assert.AreEqual("ENEMY_STATUE_01", controller.RunState.LastCombatEnemyId);
            Assert.IsTrue(hud.CombatPanelVisible);
            StringAssert.Contains("전투", hud.CombatMessage);
            StringAssert.Contains("적 HP", hud.CombatMessage);
            StringAssert.DoesNotContain("ENEMY_", hud.CombatMessage);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_RestNodeResolvesHpRecoveryAndHidesGlitch()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            var restNode = FindNode("node.rest");
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsNotNull(restNode);

            controller.AutoResolveCombat = true;
            controller.RunState.ModifyGold(100);
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "EVT_F01_JAR_ROOM", "CHOICE_EVT_F01_JAR_PLAIN");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_MORAL_CHOICE_01", "CHOICE_MORAL_01_REFUSE");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.shop"), "ENC_SHOP_01", "CHOICE_SHOP_01_BUY_ABILITY");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_F02_MORAL_CHOICE_001", "CHOICE_F02_MORAL_LEAVE");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.shop"), "ENC_F02_SHOP_001", "CHOICE_F02_SHOP_BUY_ITEM");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_COMBAT_GATE_02", "CHOICE_COMBAT_02_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_MORAL_CHOICE_02", "CHOICE_MORAL_02_REFUSE");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_MEMORY_FRAGMENT_02", "CHOICE_MEMORY_02_UNLOCK");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.shop"), "ENC_SHOP_02", "CHOICE_SHOP_02_BUY_ABILITY");
            yield return ResolveRouteChoice(controller, hud, FindNode("node.battle"), "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());
            controller.RunState.ModifyPlayerHp(-8);
            var hpBeforeRest = controller.RunState.PlayerHp;
            yield return ResolveRouteActionRest(hud, "ENC_REST_01", "rest.recover", string.Empty);

            Assert.Greater(controller.RunState.PlayerHp, hpBeforeRest);
            Assert.IsFalse(hud.RestInteractionPanelVisible);
            StringAssert.Contains("임시 응답", hud.RestResponseMessage);
            StringAssert.DoesNotContain("Glitch", hud.RunStateMessage);
            StringAssert.DoesNotContain("Glitch", hud.ResultMessage);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_ShopAppearsBeforeBossAndBossClearsFloor()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);

            controller.AutoResolveCombat = true;
            controller.RunState.ModifyGold(100);
            yield return ResolveRouteActionChoice(hud, "EVT_F01_JAR_ROOM", "CHOICE_EVT_F01_JAR_PLAIN");
            yield return ResolveRouteActionChoice(hud, "ENC_MORAL_CHOICE_01", "CHOICE_MORAL_01_REFUSE");

            hud.GetRouteActionButton().onClick.Invoke();
            yield return null;
            Assert.IsNotNull(FindChoiceButton(hud, "CHOICE_SHOP_01_BUY_ITEM"), DescribeChoiceButtons(hud));
            Assert.IsNull(FindChoiceButton(hud, "CHOICE_COMBAT_01_ENGAGE"));
            FindChoiceButton(hud, "CHOICE_SHOP_01_BUY_ITEM").onClick.Invoke();
            yield return null;

            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");

            Assert.IsTrue(controller.RunState.StairUnlocked);
            Assert.IsFalse(controller.RunState.RunClear);
            Assert.IsTrue(GameObject.Find("Next Floor Button").activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_FinalBossRestEndingReachableThroughBranchingMap()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            var shopNode = FindNode("node.shop");
            var battleNode = FindNode("node.battle");
            var restNode = FindNode("node.rest");
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);

            yield return ReachFinalBossClear(controller, hud, shopNode, battleNode, restNode);
            Assert.AreEqual(5, controller.RunState.CurrentFloor);
            Assert.AreEqual("BOSS_APEX_02", controller.RunState.LastCombatEnemyId);
            Assert.IsTrue(hud.EndingRestButtonVisible);
            Assert.IsTrue(hud.EndingContinueButtonVisible);

            GameObject.Find("Ending Button Rest").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.EndingRest);
            Assert.IsFalse(controller.RunState.RestartReady);
            StringAssert.Contains("안식 선택 완료", hud.RouteMessage);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_RouteActionButtonCompletesFullRun()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsNotNull(controller.RunState);

            controller.AutoResolveCombat = true;
            controller.RunState.ModifyGold(100);
            hud.ShowRunState(controller.GetSnapshot());

            yield return ResolveRouteActionChoice(hud, "EVT_F01_JAR_ROOM", "CHOICE_EVT_F01_JAR_PLAIN");
            yield return ResolveRouteActionChoice(hud, "ENC_MORAL_CHOICE_01", "CHOICE_MORAL_01_REFUSE");
            yield return ResolveRouteActionChoice(hud, "ENC_SHOP_01", "CHOICE_SHOP_01_BUY_ABILITY");
            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            yield return ResolveNextFloorButton(hud);

            yield return ResolveRouteActionChoice(hud, "ENC_F02_MORAL_CHOICE_001", "CHOICE_F02_MORAL_LEAVE");
            yield return ResolveRouteActionChoice(hud, "ENC_F02_SHOP_001", "CHOICE_F02_SHOP_BUY_ITEM");
            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_02", "CHOICE_COMBAT_02_ENGAGE");
            yield return ResolveNextFloorButton(hud);

            yield return ResolveRouteActionChoice(hud, "ENC_MORAL_CHOICE_02", "CHOICE_MORAL_02_REFUSE");
            yield return ResolveRouteActionChoice(hud, "ENC_MEMORY_FRAGMENT_02", "CHOICE_MEMORY_02_UNLOCK");
            yield return ResolveRouteActionChoice(hud, "ENC_SHOP_02", "CHOICE_SHOP_02_BUY_ABILITY");
            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            yield return ResolveNextFloorButton(hud);

            yield return ResolveRouteActionRest(hud, "ENC_REST_01", "rest.train", "다음 싸움을 준비하자");
            yield return ResolveRouteActionChoice(hud, "ENC_MEMORY_FRAGMENT_03", "CHOICE_MEMORY_03_UNLOCK");
            yield return ResolveRouteActionChoice(hud, "ENC_SHOP_02", "CHOICE_SHOP_02_LEAVE");
            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            Assert.AreEqual("ENEMY_WRAITH_04", controller.RunState.LastCombatEnemyId);
            yield return ResolveNextFloorButton(hud);

            yield return ResolveRouteActionChoice(hud, "ENC_MEMORY_FRAGMENT_05", "CHOICE_MEMORY_05_UNLOCK");
            yield return ResolveRouteActionRest(hud, "ENC_REST_05", "rest.recover", string.Empty);
            yield return ResolveRouteActionChoice(hud, "ENC_SHOP_02", "CHOICE_SHOP_02_LEAVE");
            yield return ResolveRouteActionChoice(hud, "ENC_COMBAT_GATE_03", "CHOICE_COMBAT_03_ENGAGE");
            hud.ShowRunState(controller.GetSnapshot());

            Assert.AreEqual(5, controller.RunState.CurrentFloor);
            Assert.AreEqual("BOSS_APEX_02", controller.RunState.LastCombatEnemyId);
            Assert.IsTrue(controller.RunState.EndingChoicePending);
            Assert.IsTrue(hud.EndingRestButtonVisible);
            Assert.IsTrue(hud.EndingContinueButtonVisible);
            Assert.IsFalse(hud.RouteActionButtonVisible);

            GameObject.Find("Ending Button Continue").GetComponent<Button>().onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.EndingContinue);
            Assert.IsTrue(controller.RunState.RestartReady);
            Assert.IsFalse(hud.RouteActionButtonVisible);
            StringAssert.Contains("동행 계속 선택", hud.RouteMessage);
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
            Assert.IsTrue(controller.RunState.StairUnlocked);
            Assert.IsFalse(controller.RunState.RunClear);
        }

        private static IEnumerator ReachFinalBossClear(Run.PrototypeRoomController controller, PrototypeHud hud, InteractableNode shopNode, InteractableNode battleNode, InteractableNode restNode)
        {
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsNotNull(shopNode);
            Assert.IsNotNull(battleNode);
            Assert.IsNotNull(restNode);

            controller.AutoResolveCombat = true;
            controller.BeginRun();
            controller.RunState.ModifyGold(100);

            yield return ResolveRouteChoice(controller, hud, battleNode, "EVT_F01_JAR_ROOM", "CHOICE_EVT_F01_JAR_PLAIN");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_MORAL_CHOICE_01", "CHOICE_MORAL_01_REFUSE");
            yield return ResolveRouteChoice(controller, hud, shopNode, "ENC_SHOP_01", "CHOICE_SHOP_01_BUY_ABILITY");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());

            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_F02_MORAL_CHOICE_001", "CHOICE_F02_MORAL_LEAVE");
            yield return ResolveRouteChoice(controller, hud, shopNode, "ENC_F02_SHOP_001", "CHOICE_F02_SHOP_BUY_ITEM");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_COMBAT_GATE_02", "CHOICE_COMBAT_02_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());

            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_MORAL_CHOICE_02", "CHOICE_MORAL_02_REFUSE");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_MEMORY_FRAGMENT_02", "CHOICE_MEMORY_02_UNLOCK");
            yield return ResolveRouteChoice(controller, hud, shopNode, "ENC_SHOP_02", "CHOICE_SHOP_02_BUY_ABILITY");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());

            yield return ResolveRouteChoice(controller, hud, restNode, "ENC_REST_01", "CHOICE_REST_01_REST");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_MEMORY_FRAGMENT_03", "CHOICE_MEMORY_03_UNLOCK");
            yield return ResolveRouteChoice(controller, hud, shopNode, "ENC_SHOP_02", "CHOICE_SHOP_02_LEAVE");
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            controller.ResolveNextFloor();
            hud.ShowRunState(controller.GetSnapshot());

            Assert.AreEqual(5, controller.RunState.CurrentFloor);
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_MEMORY_FRAGMENT_05", "CHOICE_MEMORY_05_UNLOCK");
            yield return ResolveRouteChoice(controller, hud, restNode, "ENC_REST_05", "CHOICE_REST_05_REST");
            yield return ResolveRouteChoice(controller, hud, shopNode, "ENC_SHOP_02", "CHOICE_SHOP_02_LEAVE");

            controller.AutoResolveCombat = false;
            yield return ResolveRouteChoice(controller, hud, battleNode, "ENC_COMBAT_GATE_03", "CHOICE_COMBAT_03_ENGAGE");
            Assert.IsTrue(controller.RunState.IsInCombat);
            Assert.AreEqual("BOSS_APEX_02", controller.RunState.LastCombatEnemyId);

            var skillButton = GameObject.Find("Combat Button Skill").GetComponent<Button>();
            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            Assert.IsNotNull(skillButton);
            Assert.IsNotNull(attackButton);

            var waitForCombatInput = 0;
            while (!attackButton.interactable && waitForCombatInput < 90)
            {
                yield return null;
                waitForCombatInput++;
            }

            Assert.IsTrue(skillButton.interactable);
            StringAssert.Contains("정찰 기술", ReadButtonText(skillButton));
            skillButton.onClick.Invoke();
            yield return null;
            StringAssert.Contains("정찰 기술", hud.CombatMessage);
            StringAssert.Contains("콤보 피해", hud.CombatMessage);

            var guard = 0;
            while (controller.RunState.IsInCombat && guard < 20)
            {
                (skillButton.interactable ? skillButton : attackButton).onClick.Invoke();
                yield return null;
                guard++;
            }

            hud.ShowRunState(controller.GetSnapshot());
            Assert.IsTrue(controller.RunState.RunClear);
            Assert.IsTrue(controller.RunState.EndingChoicePending);
        }

        private static IEnumerator ResolveRouteChoice(Run.PrototypeRoomController controller, PrototypeHud hud, InteractableNode node, string expectedEncounterId, string choiceStableId)
        {
            SelectMapNodeForEncounter(controller.RunState, expectedEncounterId);
            var selection = controller.SelectCurrentRouteEncounter();
            Assert.AreEqual(expectedEncounterId, selection.EncounterId);
            hud.ShowChoices(selection.Encounter, controller.BuildEncounterChoiceViews(selection), selectedChoiceStableId =>
            {
                var resolution = controller.ResolveCurrentRouteChoice(selection, selectedChoiceStableId);
                hud.ShowInteraction(node, selection, resolution);
                hud.ShowRunState(controller.GetSnapshot());
            });

            var button = FindChoiceButton(hud, choiceStableId);
            Assert.IsNotNull(button);
            Assert.IsTrue(button.interactable);
            button.onClick.Invoke();
            yield return null;
        }

        private static IEnumerator ResolveRouteActionChoice(PrototypeHud hud, string expectedEncounterId, string choiceStableId)
        {
            Assert.IsTrue(hud.RouteActionButtonVisible, "Route action should be visible before " + choiceStableId);
            hud.GetRouteActionButton().onClick.Invoke();
            yield return null;

            Assert.IsFalse(hud.RouteActionButtonVisible, "Route action should hide while choices are open.");
            var mapButton = FindMapChoiceButton(hud, expectedEncounterId);
            if (mapButton != null)
            {
                mapButton.onClick.Invoke();
                yield return null;
            }

            var button = FindChoiceButton(hud, choiceStableId);
            Assert.IsNotNull(button, "Missing " + choiceStableId + " among " + DescribeChoiceButtons(hud));
            Assert.IsTrue(button.interactable, "Disabled " + choiceStableId + " among " + DescribeChoiceButtons(hud));
            button.onClick.Invoke();
            yield return null;
        }

        private static IEnumerator ResolveRouteActionRest(PrototypeHud hud, string expectedEncounterId, string actionId, string utterance)
        {
            Assert.IsTrue(hud.RouteActionButtonVisible, "Route action should be visible before " + expectedEncounterId);
            hud.GetRouteActionButton().onClick.Invoke();
            yield return null;

            var mapButton = FindMapChoiceButton(hud, expectedEncounterId);
            if (mapButton != null)
            {
                mapButton.onClick.Invoke();
                yield return null;
            }

            Assert.IsTrue(hud.RestInteractionPanelVisible, "Rest interaction panel should open for " + expectedEncounterId);
            var actionButton = hud.GetRestActionButton(actionId);
            Assert.IsNotNull(actionButton);
            Assert.IsTrue(actionButton.interactable);
            actionButton.onClick.Invoke();
            yield return null;

            var input = hud.GetRestInputField();
            Assert.IsNotNull(input);
            input.text = utterance;
            var submit = hud.GetRestSubmitButton();
            Assert.IsNotNull(submit);
            Assert.IsTrue(submit.interactable);
            submit.onClick.Invoke();
            yield return null;

            StringAssert.Contains("임시 응답", hud.RestResponseMessage);
            var continueButton = hud.GetRestContinueButton();
            Assert.IsNotNull(continueButton);
            Assert.IsTrue(continueButton.gameObject.activeInHierarchy);
            continueButton.onClick.Invoke();
            yield return null;
        }

        private static void SelectMapNodeForEncounter(PrototypeRunState state, string expectedEncounterId)
        {
            if (state == null)
            {
                return;
            }

            var selectable = state.GetSelectableMapNodeViews();
            if (selectable.Length == 0)
            {
                return;
            }

            for (var i = 0; i < selectable.Length; i++)
            {
                if (selectable[i].MapNodeId.EndsWith("." + expectedEncounterId))
                {
                    Assert.IsTrue(state.TrySelectMapNode(selectable[i].MapNodeId, out _));
                    return;
                }
            }

            Assert.Fail("Missing selectable map node for " + expectedEncounterId);
        }

        private static IEnumerator ResolveNextFloorButton(PrototypeHud hud)
        {
            var buttonObject = GameObject.Find("Next Floor Button");
            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var snapshot = controller == null ? default : controller.GetSnapshot();
            Assert.IsNotNull(buttonObject, "Next Floor Button inactive; floor=" + snapshot.CurrentFloor + " status=" + snapshot.RunStatus + " stair=" + snapshot.StairUnlocked + " hp=" + snapshot.PlayerHp + " lastCombat=" + snapshot.LastCombatResultId + "/" + snapshot.LastCombatEnemyId);
            var button = buttonObject.GetComponent<Button>();
            Assert.IsNotNull(button);
            Assert.IsTrue(button.gameObject.activeInHierarchy);
            Assert.IsTrue(button.interactable);
            Assert.IsFalse(hud.RouteActionButtonVisible);
            button.onClick.Invoke();
            yield return null;
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

        private static Button FindMapChoiceButton(PrototypeHud hud, string encounterId)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.name.EndsWith("." + encounterId))
                {
                    return button;
                }
            }

            return null;
        }

        private static string DescribeChoiceButtons(PrototypeHud hud)
        {
            var names = new List<string>();
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null)
                {
                    names.Add(button.name + ":" + ReadButtonText(button));
                }
            }

            return string.Join(" | ", names);
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
