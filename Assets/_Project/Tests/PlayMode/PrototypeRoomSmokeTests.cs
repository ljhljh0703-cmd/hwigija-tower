using System.Collections;
using System.Collections.Generic;
using HwigiTower.Audio;
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
        public IEnumerator PrototypeRoom_LoadsRuntimeAndPlayer()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            Assert.IsNotNull(Object.FindFirstObjectByType<Run.PrototypeRoomController>());
            Assert.IsNotNull(GameObject.Find("Player"));
            Assert.IsNotNull(GameObject.Find("Prototype HUD"));
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_NewRunShowsPreRunPlaceholderBeforeMap()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);

            hud.ShowRunState(controller.GetSnapshot());

            Assert.IsTrue(controller.PreRunPlaceholderPending);
            Assert.IsTrue(hud.PreRunPlaceholderVisible);
            Assert.IsFalse(hud.NodeMapVisible);
            var confirm = GameObject.Find("Pre Run Confirm Button").GetComponent<Button>();
            confirm.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.PreRunPlaceholderPending);
            Assert.IsFalse(hud.PreRunPlaceholderVisible);
            Assert.IsTrue(hud.NodeMapVisible);
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
            Assert.IsTrue(hud.HasPortraitRoot);
            Assert.AreEqual(new Vector2(1080f, 1920f), hud.PortraitRootSize);
            var canvasScaler = GameObject.Find("Prototype HUD").GetComponent<CanvasScaler>();
            Assert.IsNotNull(canvasScaler);
            Assert.AreEqual(new Vector2(1080f, 1920f), canvasScaler.referenceResolution);
            Assert.IsNotNull(UnityEngine.EventSystems.EventSystem.current);
            Assert.IsTrue(HasUiInputModule(UnityEngine.EventSystems.EventSystem.current.gameObject));

            yield return null;
            OpenMapChoicesIfNeeded(hud);
            yield return null;

            Assert.IsTrue(hud.HasScreenLayerPanels);
            Assert.IsTrue(hud.NodeMapVisible);
            StringAssert.Contains("node_event", hud.CurrentMapNodeIconNames);
            Assert.IsFalse(AnyInteractableMapChoiceButtonContains(hud, ".ENC_REST_"), "First selectable map row should not expose rest nodes.");
            Assert.GreaterOrEqual(hud.ChoiceButtonCount, 2);
            yield return AdvanceMapUntilEncounterSelectable(hud, "EVT_F01_JAR_ROOM");
            var eventButton = FindMapChoiceButton(hud, "EVT_F01_JAR_ROOM");
            Assert.IsNotNull(eventButton, DescribeChoiceButtons(hud));
            eventButton.onClick.Invoke();
            yield return null;

            Assert.IsTrue(hud.EventCutsceneVisible);
            Assert.IsFalse(hud.NodeMapVisible);
            Assert.IsFalse(hud.GetUtilityButton("map").interactable);
            StringAssert.Contains("항아리 방", hud.EventCutsceneMessage);
            StringAssert.DoesNotContain("EVT_F01_JAR_ROOM", hud.EventCutsceneMessage);
            StringAssert.DoesNotContain("Glitch", hud.EventCutsceneMessage);
            var patternedJar = FindChoiceButton(hud, "CHOICE_EVT_F01_JAR_ROOM_PATTERNED");
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
            controller.RunState.AddItemRef("ITEM_FIELD_BANDAGE", 1);
            hud.ShowRunState(controller.GetSnapshot());
            Assert.IsTrue(hud.PreRunPlaceholderVisible);
            var preRunConfirm = GameObject.Find("Pre Run Confirm Button").GetComponent<Button>();
            Assert.IsNotNull(preRunConfirm);
            preRunConfirm.onClick.Invoke();
            yield return null;

            Assert.IsTrue(hud.NodeMapVisible, "Fresh run should show Floor 1 map after pre-run.");
            StringAssert.Contains("node_event", hud.CurrentMapNodeIconNames);
            StringAssert.Contains("node_combat", hud.CurrentMapNodeIconNames);

            yield return AdvanceMapUntilEncounterSelectableStrict(hud, "EVT_F01_JAR_ROOM");
            var eventButton = FindMapChoiceButton(hud, "EVT_F01_JAR_ROOM");
            Assert.IsNotNull(eventButton, DescribeChoiceButtons(hud));
            eventButton.onClick.Invoke();
            yield return null;

            var plainJar = FindChoiceButton(hud, "CHOICE_EVT_F01_JAR_ROOM_PLAIN");
            Assert.IsNotNull(plainJar, DescribeChoiceButtons(hud));
            plainJar.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.RunState.IsInCombat);
            Assert.IsFalse(controller.GetSnapshot().HasSelectedMapNode);
            OpenMapChoicesIfNeeded(hud);
            yield return null;
            Assert.IsTrue(hud.NodeMapVisible, "Event choice should complete and return to selectable map.");
            Assert.IsNotNull(FindFirstInteractableMapChoiceButton(hud), DescribeChoiceButtons(hud));

            yield return AdvanceMapUntilEncounterSelectableStrict(hud, "ENC_COMBAT_GATE_01");
            var combatMapButton = FindMapChoiceButton(hud, "ENC_COMBAT_GATE_01");
            Assert.IsNotNull(combatMapButton, DescribeChoiceButtons(hud));
            combatMapButton.onClick.Invoke();
            yield return null;

            Assert.IsTrue(controller.RunState.IsInCombat);
            Assert.IsFalse(string.IsNullOrEmpty(controller.RunState.LastCombatEnemyId));
            Assert.IsTrue(hud.CombatPanelVisible);
            Assert.IsFalse(hud.NodeMapVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
            Assert.IsFalse(hud.MemoryPanelVisible);
            Assert.IsFalse(hud.RouteHeaderVisible);
            Assert.IsTrue(hud.CombatPartyDockVisible);
            Assert.IsTrue(hud.CombatItemInspectButtonVisible);
            Assert.AreEqual("char_player_portrait_01", hud.CurrentCombatPlayerPortraitSpriteName);
            Assert.AreEqual("char_mataios_portrait_01", hud.CurrentCombatMataiosPortraitSpriteName);
            StringAssert.Contains("HP", hud.CombatEnemyTitleMessage);
            StringAssert.Contains(">", hud.CombatThreatReadoutMessage);
            StringAssert.Contains("예상 피해", hud.CombatActionButtonLabels);
            StringAssert.Contains("피해 감소", hud.CombatActionButtonLabels);
            StringAssert.Contains("스킬", hud.CombatActionButtonLabels);
            StringAssert.DoesNotContain("결과\n-", hud.ResultMessage);
            StringAssert.DoesNotContain("갈림길 선택", hud.RouteMessage);
            GameObject.Find("Combat Item Inspect Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsTrue(hud.CombatItemInspectVisible);
            StringAssert.Contains("붕대", hud.CombatItemInspectMessage);
            StringAssert.DoesNotContain("ITEM_07", hud.CombatItemInspectMessage);
            StringAssert.Contains("플레이어", hud.CombatPartyMessage);
            StringAssert.Contains("마타이오스", hud.CombatPartyMessage);
            StringAssert.DoesNotContain("ENEMY_", hud.CombatMessage);

            var attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
            attackButton.onClick.Invoke();
            yield return null;
            Assert.AreEqual(PrototypeAudioContext.CombatAttack, PrototypeAudioService.Instance.LastPlayedSfxContext);
            StringAssert.Contains("선택 공격", hud.CombatMessage);
            StringAssert.Contains("받은 피해", hud.CombatMessage);

            var guard = 0;
            while (controller.RunState.IsInCombat && guard++ < 16)
            {
                yield return WaitForCombatAttackButtonReady(controller, 2.0f);
                attackButton = GameObject.Find("Combat Button Attack").GetComponent<Button>();
                Assert.IsNotNull(attackButton);
                attackButton.onClick.Invoke();
                yield return null;
            }

            Assert.Less(guard, 16, "Combat did not resolve through visible attack buttons.");
            Assert.IsFalse(controller.RunState.IsInCombat);
            Assert.IsTrue(controller.GetSnapshot().LastCombatEnemyDefeated);
            Assert.IsTrue(hud.CombatDefeatFeedbackVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
            StringAssert.Contains("격파", hud.CombatDefeatFeedbackMessage);

            yield return new WaitForSecondsRealtime(1.0f);
            Assert.IsTrue(hud.CombatDefeatFeedbackVisible);
            Assert.IsFalse(hud.ResultPanelVisible);

            yield return new WaitForSecondsRealtime(1.15f);
            Assert.IsFalse(hud.CombatDefeatFeedbackVisible);
            Assert.IsTrue(hud.ResultPanelVisible || hud.BossRewardPopupVisible);
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
            controller.RunState.ModifyPlayerHp(-8);
            var hpBeforeRest = controller.RunState.PlayerHp;
            yield return ResolveRouteActionRest(hud, "ENC_REST_01", "rest.recover", string.Empty);

            Assert.Greater(controller.RunState.PlayerHp, hpBeforeRest);
            Assert.AreEqual(PrototypeAudioContext.RestSubmit, PrototypeAudioService.Instance.LastPlayedSfxContext);
            Assert.IsFalse(hud.RestInteractionPanelVisible);
            Assert.IsTrue(hud.HasScreenLayerPanels);
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
            AssertShopBeforeBoss(controller.GetSnapshot().FloorMapNodes);
            yield return SelectMapEncounterChoice(controller, hud, "ENC_SHOP_01", "CHOICE_SHOP_01_BUY_ITEM");
            Assert.AreEqual(PrototypeAudioContext.ShopPurchase, PrototypeAudioService.Instance.LastPlayedSfxContext);

            yield return SelectMapEncounterChoice(controller, hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");

            Assert.IsTrue(controller.RunState.StairUnlocked);
            Assert.IsFalse(controller.RunState.RunClear);
            Assert.IsTrue(GameObject.Find("Next Floor Button").activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_BossGateShowsFightOnlyAfterMapCommit()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);

            controller.AutoResolveCombat = true;
            controller.RunState.ModifyGold(100);
            SelectMapNodeForEncounter(controller.RunState, "ENC_COMBAT_GATE_01");
            OpenSelectedRouteStepForTest(hud, controller.SelectCurrentRouteEncounter());
            yield return null;

            Assert.AreEqual(1, hud.ChoiceButtonCount, DescribeChoiceButtons(hud));
            Assert.IsNotNull(FindChoiceButton(hud, "CHOICE_COMBAT_01_ENGAGE"), DescribeChoiceButtons(hud));
            StringAssert.Contains("전투 시작", DescribeChoiceButtons(hud));
            StringAssert.DoesNotContain("돌아간다", DescribeChoiceButtons(hud));
            StringAssert.DoesNotContain("준비", DescribeChoiceButtons(hud));
        }

        [UnityTest]
        public IEnumerator PrototypeRoom_FloorFiveEliteAndFinalBossPoolsResolve()
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(controller.EncounterRuntimeCatalog);
            Assert.IsNotNull(controller.EncounterRuntimeCatalog.FloorEnemyPools);
            Assert.IsTrue(controller.EncounterRuntimeCatalog.FloorEnemyPools.TryGetPool(5, out var floorFivePool));

            CollectionAssert.Contains(floorFivePool.EliteEnemyRefs, "ENEMY_LAMPLIGHTER_01");
            CollectionAssert.Contains(floorFivePool.BossEnemyRefs, "BOSS_APEX_02");
            CollectionAssert.DoesNotContain(floorFivePool.BossEnemyRefs, "ENEMY_LAMPLIGHTER_01");
            Assert.IsTrue(controller.EncounterRuntimeCatalog.TryGetEnemy("ENEMY_LAMPLIGHTER_01", out var lamplighter));
            Assert.IsTrue(controller.EncounterRuntimeCatalog.TryGetEnemy("BOSS_APEX_02", out var finalBoss));
            Assert.AreEqual("ENEMY_LAMPLIGHTER_01", lamplighter.Id);
            Assert.AreEqual("BOSS_APEX_02", finalBoss.Id);
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

            yield return SelectMapEncounterChoice(controller, hud, "ENC_COMBAT_GATE_01", "CHOICE_COMBAT_01_ENGAGE");
            yield return ResolveNextFloorButton(hud);

            Assert.AreEqual(2, controller.RunState.CurrentFloor);
            Assert.IsTrue(hud.NodeMapVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
            Assert.IsFalse(hud.CombatPanelVisible);
            Assert.IsFalse(hud.PortraitVisible);
            Assert.IsFalse(hud.RouteActionButtonVisible);
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

            Assert.IsTrue(controller.OpenQaFloor(5));
            hud.ShowRunState(controller.GetSnapshot());
            Assert.AreEqual(5, controller.RunState.CurrentFloor);
            controller.OpenQaEndingChoice();
            hud.ShowRunState(controller.GetSnapshot());
            Assert.IsTrue(controller.RunState.RunClear);
            Assert.IsTrue(controller.RunState.EndingChoicePending);
            yield return null;
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

        private static IEnumerator OpenQaEncounter(Run.PrototypeRoomController controller, PrototypeHud hud, string encounterId)
        {
            var selection = controller.CreateQaEncounterSelection(encounterId);
            Assert.IsTrue(selection.HasEncounter, "Missing QA encounter selection for " + encounterId);
            hud.OpenQaRouteStep(selection);
            yield return null;
        }

        private static IEnumerator OpenQaEncounterChoice(Run.PrototypeRoomController controller, PrototypeHud hud, string encounterId, string choiceStableId)
        {
            yield return OpenQaEncounter(controller, hud, encounterId);
            var button = FindChoiceButton(hud, choiceStableId);
            Assert.IsNotNull(button, "Missing " + choiceStableId + " among " + DescribeChoiceButtons(hud));
            Assert.IsTrue(button.interactable, "Disabled " + choiceStableId + " among " + DescribeChoiceButtons(hud));
            button.onClick.Invoke();
            yield return null;
        }

        private static IEnumerator SelectMapEncounterChoice(Run.PrototypeRoomController controller, PrototypeHud hud, string encounterId, string choiceStableId)
        {
            var selection = SelectMapEncounter(controller.RunState, encounterId);
            Assert.AreEqual(encounterId, selection.EncounterId);
            OpenSelectedRouteStepForTest(hud, selection);
            yield return null;
            var button = FindChoiceButton(hud, choiceStableId);
            Assert.IsNotNull(button, "Missing " + choiceStableId + " among " + DescribeChoiceButtons(hud));
            Assert.IsTrue(button.interactable, "Disabled " + choiceStableId + " among " + DescribeChoiceButtons(hud));
            button.onClick.Invoke();
            yield return null;
        }

        private static EncounterSelection SelectMapEncounter(PrototypeRunState state, string expectedEncounterId)
        {
            Assert.IsNotNull(state);
            var selectable = state.GetSelectableMapNodeViews();
            for (var i = 0; i < selectable.Length; i++)
            {
                if (selectable[i].MapNodeId.EndsWith("." + expectedEncounterId) &&
                    state.TrySelectMapNode(selectable[i].MapNodeId, out var targetStep))
                {
                    return new EncounterSelection(targetStep.Node, targetStep.Encounter);
                }
            }

            var guard = 0;
            while (guard++ < 8)
            {
                selectable = state.GetSelectableMapNodeViews();
                Assert.Greater(selectable.Length, 0, "Missing selectable map node for " + expectedEncounterId);
                var selected = SelectPreferredAdvanceMapNode(selectable, state.GetFloorMapNodeViews(), expectedEncounterId);
                Assert.IsTrue(state.TrySelectMapNode(selected.MapNodeId, out var step));
                if (step.Encounter != null && step.Encounter.Id == expectedEncounterId)
                {
                    return new EncounterSelection(step.Node, step.Encounter);
                }

                var choiceStableId = ResolvePreferredChoiceId(state, step);
                state.ResolveEncounterChoice(new DeterministicRunContext(state.RunId, 1001), step.NodeId, step.Encounter, choiceStableId);
            }

            Assert.Fail("Missing selectable map node for " + expectedEncounterId);
            return new EncounterSelection(null, null);
        }

        private static void AssertShopBeforeBoss(PrototypeFloorMapNodeView[] nodes)
        {
            var shopLayer = int.MaxValue;
            var bossLayer = int.MinValue;
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Type == PrototypeFloorMapNodeType.Shop)
                {
                    shopLayer = Mathf.Min(shopLayer, nodes[i].Layer);
                }
                else if (nodes[i].Type == PrototypeFloorMapNodeType.Boss)
                {
                    bossLayer = Mathf.Max(bossLayer, nodes[i].Layer);
                }
            }

            Assert.Less(shopLayer, bossLayer, "Shop should be presented before the boss layer.");
        }

        private static IEnumerator ResolveRouteActionChoice(PrototypeHud hud, string expectedEncounterId, string choiceStableId)
        {
            if (hud.RouteActionButtonVisible)
            {
                hud.GetRouteActionButton().onClick.Invoke();
                yield return null;
            }

            OpenMapChoicesIfNeeded(hud);
            yield return null;
            Assert.IsFalse(hud.RouteActionButtonVisible, "Route action should hide while choices or map nodes are open.");
            yield return AdvanceMapUntilEncounterSelectable(hud, expectedEncounterId);
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
            if (hud.RouteActionButtonVisible)
            {
                hud.GetRouteActionButton().onClick.Invoke();
                yield return null;
            }

            OpenMapChoicesIfNeeded(hud);
            yield return null;
            yield return AdvanceMapUntilEncounterSelectable(hud, expectedEncounterId);
            var mapButton = FindMapChoiceButton(hud, expectedEncounterId);
            if (mapButton != null)
            {
                mapButton.onClick.Invoke();
                yield return null;
            }

            Assert.IsTrue(hud.RestInteractionPanelVisible, "Rest interaction panel should open for " + expectedEncounterId);
            StringAssert.Contains("icon_rest_talk", hud.CurrentRestActionIconNames);
            StringAssert.Contains("icon_rest_train", hud.CurrentRestActionIconNames);
            StringAssert.Contains("icon_rest_recover", hud.CurrentRestActionIconNames);
            StringAssert.Contains("마타이오스와 대화", hud.CurrentRestActionCardLabels);
            StringAssert.Contains("단련 · 다음 전투 보너스", hud.CurrentRestActionCardLabels);
            StringAssert.Contains("휴식 · 체력 회복", hud.CurrentRestActionCardLabels);
            StringAssert.DoesNotContain("Glitch", hud.CurrentRestActionCardLabels);
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

        private static IEnumerator ResolveCurrentFloorBossViaRoute(Run.PrototypeRoomController controller, PrototypeHud hud, string encounterId, string choiceStableId)
        {
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            var floor = controller.RunState.CurrentFloor;
            var guard = 0;
            while (!controller.RunState.StairUnlocked && controller.RunState.CurrentFloor == floor && guard++ < 8)
            {
                yield return ResolveRouteActionChoice(hud, encounterId, choiceStableId);
            }

            Assert.Less(guard, 8, "Could not resolve boss route for floor " + floor + " via " + encounterId);
        }

        private static IEnumerator AdvanceMapUntilEncounterSelectable(PrototypeHud hud, string expectedEncounterId)
        {
            var guard = 0;
            OpenMapChoicesIfNeeded(hud);
            yield return null;
            while (FindMapChoiceButton(hud, expectedEncounterId) == null && !IsExpectedEncounterOpen(hud, expectedEncounterId) && guard++ < 20)
            {
                if (IsAnyEncounterOpen(hud) || hud.RestInteractionPanelVisible)
                {
                    yield return ResolveOpenEncounterForMapAdvance(hud);
                    if (hud.RouteActionButtonVisible)
                    {
                        hud.GetRouteActionButton().onClick.Invoke();
                        yield return null;
                    }

                    OpenMapChoicesIfNeeded(hud);
                    yield return null;
                    continue;
                }

                var next = FindPreferredAdvanceMapChoiceButton(hud, expectedEncounterId);
                Assert.IsNotNull(next, "Missing selectable map node while advancing toward " + expectedEncounterId + " among " + DescribeChoiceButtons(hud));
                next.onClick.Invoke();
                yield return null;
                if (IsExpectedEncounterOpen(hud, expectedEncounterId))
                {
                    break;
                }

                yield return ResolveOpenEncounterForMapAdvance(hud);
                if (hud.RouteActionButtonVisible)
                {
                    hud.GetRouteActionButton().onClick.Invoke();
                    yield return null;
                }

                OpenMapChoicesIfNeeded(hud);
                yield return null;
            }

            if (FindMapChoiceButton(hud, expectedEncounterId) == null && !IsExpectedEncounterOpen(hud, expectedEncounterId))
            {
                var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
                Assert.IsNotNull(controller, "Missing controller while advancing map toward " + expectedEncounterId);
                SelectMapNodeForEncounter(controller.RunState, expectedEncounterId);
                var selection = controller.SelectCurrentRouteEncounter();
                Assert.AreEqual(expectedEncounterId, selection.EncounterId);
                OpenSelectedRouteStepForTest(hud, selection);
                yield return null;
            }

            Assert.IsTrue(
                FindMapChoiceButton(hud, expectedEncounterId) != null || IsExpectedEncounterOpen(hud, expectedEncounterId),
                "Could not advance map toward " + expectedEncounterId);
        }

        private static IEnumerator AdvanceMapUntilEncounterSelectableStrict(PrototypeHud hud, string expectedEncounterId)
        {
            var guard = 0;
            OpenMapChoicesIfNeeded(hud);
            yield return null;
            while (FindMapChoiceButton(hud, expectedEncounterId) == null && !IsExpectedEncounterOpen(hud, expectedEncounterId) && guard++ < 20)
            {
                if (IsAnyEncounterOpen(hud) || hud.RestInteractionPanelVisible)
                {
                    yield return ResolveOpenEncounterForMapAdvance(hud);
                    if (hud.RouteActionButtonVisible)
                    {
                        hud.GetRouteActionButton().onClick.Invoke();
                        yield return null;
                    }

                    OpenMapChoicesIfNeeded(hud);
                    yield return null;
                    continue;
                }

                var next = FindPreferredAdvanceMapChoiceButton(hud, expectedEncounterId);
                Assert.IsNotNull(next, "Missing selectable map node while advancing toward " + expectedEncounterId + " among " + DescribeChoiceButtons(hud));
                next.onClick.Invoke();
                yield return null;
                if (IsExpectedEncounterOpen(hud, expectedEncounterId))
                {
                    break;
                }

                yield return ResolveOpenEncounterForMapAdvance(hud);
                if (hud.RouteActionButtonVisible)
                {
                    hud.GetRouteActionButton().onClick.Invoke();
                    yield return null;
                }

                OpenMapChoicesIfNeeded(hud);
                yield return null;
            }

            Assert.IsTrue(
                FindMapChoiceButton(hud, expectedEncounterId) != null || IsExpectedEncounterOpen(hud, expectedEncounterId),
                "Could not advance map toward " + expectedEncounterId + " through visible route nodes.");
        }

        private static IEnumerator WaitForCombatAttackButtonReady(Run.PrototypeRoomController controller, float timeoutSeconds)
        {
            var timeoutAt = Time.realtimeSinceStartup + Mathf.Max(0.1f, timeoutSeconds);
            while (controller != null && controller.RunState.IsInCombat && Time.realtimeSinceStartup < timeoutAt)
            {
                var buttonObject = GameObject.Find("Combat Button Attack");
                var button = buttonObject == null ? null : buttonObject.GetComponent<Button>();
                if (button != null && button.gameObject.activeInHierarchy && button.interactable)
                {
                    yield break;
                }

                yield return null;
            }

            if (controller == null || !controller.RunState.IsInCombat)
            {
                yield break;
            }

            var lastButtonObject = GameObject.Find("Combat Button Attack");
            var lastButton = lastButtonObject == null ? null : lastButtonObject.GetComponent<Button>();
            Assert.IsNotNull(lastButton, "Missing attack button while combat is active.");
            Assert.IsTrue(lastButton.interactable, "Attack should become usable while combat is active.");
        }

        private static void OpenMapChoicesIfNeeded(PrototypeHud hud)
        {
            if (hud == null || FindFirstInteractableMapChoiceButton(hud) != null || IsAnyEncounterOpen(hud) || hud.RestInteractionPanelVisible)
            {
                return;
            }

            var mapButton = hud.GetUtilityButton("map");
            if (mapButton != null && mapButton.interactable)
            {
                mapButton.onClick.Invoke();
            }

            if (FindFirstInteractableMapChoiceButton(hud) != null || !hud.RouteActionButtonVisible)
            {
                return;
            }

            hud.GetRouteActionButton().onClick.Invoke();
        }

        private static bool IsAnyEncounterOpen(PrototypeHud hud)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && (button.name.StartsWith("Choice Button ") || IsShopChoiceButton(hud, button)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsExpectedEncounterOpen(PrototypeHud hud, string expectedEncounterId)
        {
            if (expectedEncounterId != null && expectedEncounterId.StartsWith("ENC_REST_", System.StringComparison.Ordinal))
            {
                return hud.RestInteractionPanelVisible;
            }

            if (expectedEncounterId != null && expectedEncounterId.Contains("SHOP", System.StringComparison.Ordinal))
            {
                return hud.ShopUiVisible;
            }

            var expectedPrefix = ExpectedChoicePrefix(expectedEncounterId);
            if (string.IsNullOrEmpty(expectedPrefix))
            {
                return false;
            }

            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.name.StartsWith("Choice Button " + expectedPrefix, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ExpectedChoicePrefix(string encounterId)
        {
            return encounterId switch
            {
                "EVT_F01_JAR_ROOM" => "CHOICE_EVT_F01_JAR",
                "ENC_MORAL_CHOICE_01" => "CHOICE_MORAL_01",
                "ENC_MORAL_CHOICE_02" => "CHOICE_MORAL_02",
                "ENC_MORAL_CHOICE_03" => "CHOICE_MORAL_03",
                "ENC_F02_MORAL_CHOICE_001" => "CHOICE_F02_MORAL",
                "ENC_MEMORY_FRAGMENT_01" => "CHOICE_MEMORY_01",
                "ENC_MEMORY_FRAGMENT_02" => "CHOICE_MEMORY_02",
                "ENC_MEMORY_FRAGMENT_03" => "CHOICE_MEMORY_03",
                "ENC_MEMORY_FRAGMENT_04" => "CHOICE_MEMORY_04",
                "ENC_MEMORY_FRAGMENT_05" => "CHOICE_MEMORY_05",
                "ENC_SHOP_01" => "CHOICE_SHOP_01",
                "ENC_F02_SHOP_001" => "CHOICE_F02_SHOP",
                "ENC_SHOP_03" => "CHOICE_SHOP_03",
                "ENC_SHOP_04" => "CHOICE_SHOP_04",
                "ENC_SHOP_05" => "CHOICE_SHOP_05",
                "ENC_COMBAT_GATE_01" => "CHOICE_COMBAT_01",
                "ENC_COMBAT_GATE_02" => "CHOICE_COMBAT_02",
                "ENC_COMBAT_GATE_03" => "CHOICE_COMBAT_03",
                _ => string.Empty
            };
        }

        private static IEnumerator ResolveOpenEncounterForMapAdvance(PrototypeHud hud)
        {
            if (hud.RestInteractionPanelVisible)
            {
                var restButton = hud.GetRestActionButton("rest.recover");
                Assert.IsNotNull(restButton);
                restButton.onClick.Invoke();
                yield return null;
                var submit = hud.GetRestSubmitButton();
                Assert.IsNotNull(submit);
                submit.onClick.Invoke();
                yield return null;
                var continueButton = hud.GetRestContinueButton();
                Assert.IsNotNull(continueButton);
                continueButton.onClick.Invoke();
                yield return null;
                yield break;
            }

            var choice = FindFirstInteractableNonCombatChoiceButton(hud) ?? FindFirstInteractableChoiceButton(hud);
            Assert.IsNotNull(choice, "Missing encounter choice while advancing map among " + DescribeChoiceButtons(hud));
            choice.onClick.Invoke();
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

            var match = string.Empty;
            for (var i = 0; i < selectable.Length; i++)
            {
                if (selectable[i].MapNodeId.EndsWith("." + expectedEncounterId))
                {
                    match = SelectPreferredMapNodeId(match, selectable[i].MapNodeId);
                }
            }

            if (!string.IsNullOrEmpty(match))
            {
                Assert.IsTrue(state.TrySelectMapNode(match, out _));
                return;
            }

            var guard = 0;
            while (guard++ < 8)
            {
                selectable = state.GetSelectableMapNodeViews();
                Assert.Greater(selectable.Length, 0, "Missing selectable map node for " + expectedEncounterId);
                var selected = SelectPreferredAdvanceMapNode(selectable, state.GetFloorMapNodeViews(), expectedEncounterId);
                Assert.IsTrue(state.TrySelectMapNode(selected.MapNodeId, out var step));
                var choiceStableId = ResolvePreferredChoiceId(state, step);
                state.ResolveEncounterChoice(new DeterministicRunContext(state.RunId, 1001), step.NodeId, step.Encounter, choiceStableId);
                selectable = state.GetSelectableMapNodeViews();
                match = string.Empty;
                for (var i = 0; i < selectable.Length; i++)
                {
                    if (selectable[i].MapNodeId.EndsWith("." + expectedEncounterId))
                    {
                        match = SelectPreferredMapNodeId(match, selectable[i].MapNodeId);
                    }
                }

                if (!string.IsNullOrEmpty(match))
                {
                    Assert.IsTrue(state.TrySelectMapNode(match, out _));
                    return;
                }
            }

            Assert.Fail("Missing selectable map node for " + expectedEncounterId);
        }

        private static void OpenSelectedRouteStepForTest(PrototypeHud hud, EncounterSelection selection)
        {
            hud.ClearChoices();

            var method = typeof(PrototypeHud).GetMethod(
                "OpenSelectedRouteStep",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            method.Invoke(hud, new object[] { selection });
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
            Assert.IsTrue(hud.NodeMapVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
            Assert.IsFalse(hud.CombatPanelVisible);
            Assert.IsFalse(hud.PortraitVisible);
            Assert.IsFalse(hud.RouteActionButtonVisible);
            Assert.IsNotNull(FindFirstInteractableMapChoiceButton(hud), "Next floor should immediately present a fresh selectable map.");
        }

        private static EncounterData CreateEncounterFromJson(string json)
        {
            var encounter = ScriptableObject.CreateInstance<EncounterData>();
            JsonUtility.FromJsonOverwrite(json, encounter);
            return encounter;
        }

        private static void ResetRunStateIsolation()
        {
            PrototypeRunSaveStore.Delete();
            PrototypeRunSaveRequest.RequestNewGame();
            Time.timeScale = 1f;
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
            Button preferred = null;
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.interactable && button.name.EndsWith("." + encounterId))
                {
                    if (IsBossGateEncounter(encounterId) && ExtractMapLayer(button.name) < 5)
                    {
                        continue;
                    }

                    preferred = SelectPreferredMapButton(preferred, button);
                }
            }

            return preferred;
        }

        private static Button SelectPreferredMapButton(Button current, Button candidate)
        {
            if (current == null)
            {
                return candidate;
            }

            return ExtractMapLayer(candidate.name) >= ExtractMapLayer(current.name) ? candidate : current;
        }

        private static string SelectPreferredMapNodeId(string current, string candidate)
        {
            if (string.IsNullOrEmpty(current))
            {
                return candidate;
            }

            return ExtractMapLayer(candidate) >= ExtractMapLayer(current) ? candidate : current;
        }

        private static PrototypeFloorMapNodeView SelectPreferredAdvanceMapNode(PrototypeFloorMapNodeView[] selectable, PrototypeFloorMapNodeView[] allNodes, string expectedEncounterId)
        {
            Assert.IsNotNull(selectable);
            var targetIsBossGate = IsBossGateEncounter(expectedEncounterId);
            PrototypeFloorMapNodeView? fallback = null;
            PrototypeFloorMapNodeView? preferred = null;
            for (var i = 0; i < selectable.Length; i++)
            {
                var node = selectable[i];
                fallback ??= node;
                if (allNodes != null && allNodes.Length > 0 && !MapNodeCanReachEncounter(allNodes, node.MapNodeId, expectedEncounterId))
                {
                    continue;
                }

                if (node.MapNodeId.Contains(".ENC_COMBAT_GATE_") && (!targetIsBossGate || node.Layer < 5))
                {
                    continue;
                }

                if (!preferred.HasValue || node.Layer < preferred.Value.Layer)
                {
                    preferred = node;
                }
            }

            return preferred ?? fallback.Value;
        }

        private static int ExtractMapLayer(string mapNodeNameOrId)
        {
            if (string.IsNullOrEmpty(mapNodeNameOrId))
            {
                return -1;
            }

            const string marker = ".layer.";
            var markerIndex = mapNodeNameOrId.IndexOf(marker, System.StringComparison.Ordinal);
            if (markerIndex < 0)
            {
                return -1;
            }

            var start = markerIndex + marker.Length;
            var end = mapNodeNameOrId.IndexOf('.', start);
            if (end <= start)
            {
                return -1;
            }

            return int.TryParse(mapNodeNameOrId.Substring(start, end - start), out var layer) ? layer : -1;
        }

        private static bool IsBossGateEncounter(string encounterId)
        {
            return encounterId != null && encounterId.StartsWith("ENC_COMBAT_GATE_", System.StringComparison.Ordinal);
        }

        private static Button FindFirstInteractableMapChoiceButton(PrototypeHud hud)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.interactable && button.name.StartsWith("Map Node Button "))
                {
                    return button;
                }
            }

            return null;
        }

        private static Button FindPreferredAdvanceMapChoiceButton(PrototypeHud hud, string expectedEncounterId)
        {
            Button fallback = null;
            Button preferred = null;
            var targetIsBossGate = IsBossGateEncounter(expectedEncounterId);
            var controller = Object.FindFirstObjectByType<Run.PrototypeRoomController>();
            var snapshot = controller == null ? default : controller.GetSnapshot();
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button == null || !button.interactable || !button.name.StartsWith("Map Node Button "))
                {
                    continue;
                }

                fallback ??= button;
                var mapNodeId = ExtractMapNodeId(button.name);
                if (snapshot.HasFloorMap && !MapNodeCanReachEncounter(snapshot.FloorMapNodes, mapNodeId, expectedEncounterId))
                {
                    continue;
                }

                var isCombatGate = button.name.Contains(".ENC_COMBAT_GATE_");
                if (isCombatGate && (!targetIsBossGate || ExtractMapLayer(button.name) < 5))
                {
                    continue;
                }

                if (preferred == null || ExtractMapLayer(button.name) < ExtractMapLayer(preferred.name))
                {
                    preferred = button;
                }
            }

            return preferred ?? fallback;
        }

        private static bool AnyInteractableMapChoiceButtonContains(PrototypeHud hud, string value)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.interactable && button.name.StartsWith("Map Node Button ") && button.name.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ExtractMapNodeId(string buttonName)
        {
            const string prefix = "Map Node Button ";
            return !string.IsNullOrEmpty(buttonName) && buttonName.StartsWith(prefix, System.StringComparison.Ordinal)
                ? buttonName.Substring(prefix.Length)
                : string.Empty;
        }

        private static bool MapNodeCanReachEncounter(PrototypeFloorMapNodeView[] nodes, string startMapNodeId, string encounterId)
        {
            if (nodes == null || string.IsNullOrEmpty(startMapNodeId) || string.IsNullOrEmpty(encounterId))
            {
                return false;
            }

            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(startMapNodeId);
            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (!visited.Add(currentId))
                {
                    continue;
                }

                if (currentId.EndsWith("." + encounterId, System.StringComparison.Ordinal))
                {
                    return true;
                }

                if (!TryFindMapNode(nodes, currentId, out var node) || node.NextMapNodeIds == null)
                {
                    continue;
                }

                for (var i = 0; i < node.NextMapNodeIds.Length; i++)
                {
                    queue.Enqueue(node.NextMapNodeIds[i]);
                }
            }

            return false;
        }

        private static bool TryFindMapNode(PrototypeFloorMapNodeView[] nodes, string mapNodeId, out PrototypeFloorMapNodeView node)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].MapNodeId == mapNodeId)
                {
                    node = nodes[i];
                    return true;
                }
            }

            node = default;
            return false;
        }

        private static Button FindFirstInteractableChoiceButton(PrototypeHud hud)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.interactable && (button.name.StartsWith("Choice Button ") || IsShopChoiceButton(hud, button)))
                {
                    return button;
                }
            }

            return null;
        }

        private static Button FindFirstInteractableNonCombatChoiceButton(PrototypeHud hud)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.interactable &&
                    ((button.name.StartsWith("Choice Button ") && !button.name.Contains("COMBAT")) || IsShopChoiceButton(hud, button)))
                {
                    return button;
                }
            }

            return null;
        }

        private static bool IsShopChoiceButton(PrototypeHud hud, Button button)
        {
            return hud != null &&
                hud.ShopUiVisible &&
                button != null &&
                (button.name.StartsWith("Shop Offer Row ", System.StringComparison.Ordinal) ||
                 button.name == "Shop Leave Button");
        }

        private static string ResolvePreferredChoiceId(PrototypeRunState state, PrototypeDemoRunStep step)
        {
            Assert.IsNotNull(step);
            Assert.IsNotNull(step.Encounter);
            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, step.Encounter);
            for (var i = 0; i < views.Length; i++)
            {
                if (views[i].Visible && views[i].Enabled)
                {
                    return views[i].ChoiceStableId;
                }
            }

            Assert.IsNotNull(step.Encounter.Choices);
            Assert.Greater(step.Encounter.Choices.Length, 0);
            return step.Encounter.Choices[0].stableId;
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

        private static bool HasUiInputModule(GameObject eventSystemObject)
        {
            var behaviours = eventSystemObject.GetComponents<Behaviour>();
            for (var i = 0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                if (behaviour != null && behaviour.GetType().Name.Contains("InputModule"))
                {
                    return true;
                }
            }

            return false;
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
