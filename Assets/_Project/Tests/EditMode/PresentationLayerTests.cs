using HwigiTower.UI;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.Run;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.Tests.EditMode
{
    public sealed class PresentationLayerTests
    {
        [Test]
        public void DemoPresentationData_ResolvesSlotsByEncounterStableId()
        {
            var data = ScriptableObject.CreateInstance<DemoPresentationData>();
            var serialized = new SerializedObject(data);
            var slots = serialized.FindProperty("slots");
            slots.arraySize = 1;

            var slot = slots.GetArrayElementAtIndex(0);
            slot.FindPropertyRelative("encounterStableId").stringValue = "ENC_COMBAT_GATE_01";
            slot.FindPropertyRelative("displayName").stringValue = "CombatGate";
            serialized.ApplyModifiedPropertiesWithoutUndo();

            Assert.IsTrue(data.TryGetSlot("ENC_COMBAT_GATE_01", out var resolved));
            Assert.AreEqual("CombatGate", resolved.DisplayName);
            Assert.IsFalse(data.TryGetSlot("ENC_UNKNOWN", out _));
        }

        [Test]
        public void DemoPresentationData_BindsLateRouteSprites()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");

            Assert.IsNotNull(data);
            AssertSlotSprites(data, "ENC_COMBAT_GATE_01", null, "enemy_fracture_hound", null, null);
            AssertSlotSprites(data, "ENC_COMBAT_GATE_02", null, "enemy_collapse_echo", null, null);
            AssertSlotSprites(data, "ENC_COMBAT_GATE_03", "enc_combat_gate_03_bg", "enemy_boss_apex_02", "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "EVT_F01_JAR_ROOM", "evt_f01_jar_room_bg", null, "char_mataios_bust_s0_s2", null);
            AssertSlotSprites(data, "ENC_SHOP_02", "enc_shop_02_bg", null, "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "ENC_MEMORY_FRAGMENT_03", "enc_memory_fragment_03_bg", null, "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "ENC_MEMORY_FRAGMENT_05", "enc_memory_fragment_03_bg", null, "char_mataios_bust_s3_s4", "enc_memory_fragment_05_art");
            AssertPublicLabel(data, "ENC_MEMORY_FRAGMENT_01", "기억의 잔향");
            AssertPublicLabel(data, "ENC_MEMORY_FRAGMENT_03", "기억의 잔향");
            AssertPublicLabel(data, "ENC_MEMORY_FRAGMENT_05", "기억의 잔향");
            AssertSlotSprites(data, "ENEMY_EMPTY_ARMOR", null, "enemy_empty_armor", null, null);
            AssertSlotSprites(data, "ENEMY_SHADE_03", null, "enemy_shade_03", null, null);
            AssertSlotSprites(data, "ENEMY_WRAITH_04", null, "enemy_wraith_04", null, null);
            AssertSlotSprites(data, "ENEMY_BANDIT_MELEE_01", null, "enemy_bandit_melee_01", null, null);
            AssertSlotSprites(data, "ENEMY_BANDIT_RANGED_01", null, "enemy_bandit_ranged_01", null, null);
            AssertSlotSprites(data, "ENEMY_SLIME_01", null, "enemy_slime_01", null, null);
            AssertSlotSprites(data, "ENEMY_SKELETON_01", null, "enemy_skeleton_01", null, null);
            AssertSlotSprites(data, "ENEMY_WILD_BEAST_01", null, "enemy_wild_beast_01", null, null);
            AssertSlotSprites(data, "ENEMY_MERCENARY_CAPTAIN_SAGAN_01", null, "boss_mercenary_captain_sagan_01", null, null);
            AssertSlotSprites(data, "ENEMY_HOMUNCULUS_01", null, "enemy_homunculus_01", null, null);
            AssertSlotSprites(data, "ENEMY_FRACTURE_HOUND", null, "enemy_fracture_hound", null, null);
            AssertSlotSprites(data, "ENEMY_LAMPLIGHTER_01", null, "enemy_lamplighter_01", null, null);
            AssertSlotSprites(data, "BOSS_APEX_02", null, "enemy_boss_apex_02", null, null);
            AssertSlotSprites(data, "run.clear", "enc_complete_bg", null, "char_mataios_bust_s3_s4", null);
        }

        [Test]
        public void DemoPresentationData_BindsNodeIcons()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");

            Assert.IsNotNull(data);
            AssertNodeIcon(data, PrototypeFloorMapNodeType.Combat, "node_combat");
            AssertNodeIcon(data, PrototypeFloorMapNodeType.Event, "node_event");
            AssertNodeIcon(data, PrototypeFloorMapNodeType.Rest, "node_rest");
            AssertNodeIcon(data, PrototypeFloorMapNodeType.Shop, "node_shop");
            AssertNodeIcon(data, PrototypeFloorMapNodeType.Boss, "node_boss");
            Assert.IsTrue(data.TryGetCompletedNodeBackground(out var completed));
            Assert.AreEqual("node_back_cleared", completed.name);
            Assert.IsNotNull(data.SelectedNodeRing);
            Assert.AreEqual("map_node_selected_ring", data.SelectedNodeRing.name);
            Assert.IsNotNull(data.LockedNodeOverlay);
            Assert.AreEqual("map_node_locked_overlay", data.LockedNodeOverlay.name);
            Assert.IsNotNull(data.CurrentPositionMarker);
            Assert.AreEqual("map_current_position_marker", data.CurrentPositionMarker.name);

            for (var floor = 1; floor <= 5; floor++)
            {
                Assert.IsTrue(data.TryGetFloorMapBackground(floor, out var background), "Missing map background for floor " + floor);
                Assert.AreEqual("map_floor0" + floor + "_bg", background.name);
            }
        }

        [Test]
        public void DemoPresentationData_BindsCombatActionIconsAndPlayerSlot()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");

            Assert.IsNotNull(data);
            Assert.IsNotNull(data.DefaultPlayerPortrait);
            Assert.AreEqual("char_player_portrait_01", data.DefaultPlayerPortrait.name);
            Assert.IsNotNull(data.CombatMataiosPortrait);
            Assert.AreEqual("char_mataios_portrait_01", data.CombatMataiosPortrait.name);
            Assert.IsNull(data.CombatPortraitFrame);
            AssertCombatActionIcon(data, CombatAction.Attack, "icon_action_attack");
            AssertCombatActionIcon(data, CombatAction.Defend, "icon_action_defend");
            AssertCombatActionIcon(data, CombatAction.Skill, "icon_action_skill_scout");
        }

        [Test]
        public void DemoPresentationData_BindsRestActionIcons()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");

            Assert.IsNotNull(data);
            AssertRestActionIcon(data, "rest.ask_mood", "icon_rest_talk");
            AssertRestActionIcon(data, "rest.train", "icon_rest_train");
            AssertRestActionIcon(data, "rest.recover", "icon_rest_recover");
        }

        [Test]
        public void DemoPresentationData_BindsShopRestUiSupportAndIcons()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");

            Assert.IsNotNull(data);
            Assert.AreEqual("ui_spotlight_gradient", data.SpotlightGradient.name);
            Assert.AreEqual("ui_npc_dialogue_plate", data.NpcDialoguePlate.name);
            Assert.AreEqual("ui_shop_product_card", data.ShopProductCard.name);
            Assert.AreEqual("ui_shop_locked_card", data.ShopLockedCard.name);
            AssertSlotSprites(data, "ENC_REST_01", "enc_rest_01_bg", null, "char_mataios_bust_s0_s2", null);
            AssertSlotSprites(data, "ENC_SHOP_01", "enc_shop_01_bg", null, "char_mataios_bust_s0_s2", null);
            AssertSlotSprites(data, "ENC_SHOP_02", "enc_shop_02_bg", null, "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "ENC_F02_SHOP_001", "enc_shop_02_bg", null, "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "ENC_SHOP_03", "enc_shop_03_bg", null, "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "ENC_SHOP_04", "enc_shop_04_bg", null, "char_mataios_bust_s3_s4", null);
            AssertSlotSprites(data, "ENC_SHOP_05", "enc_shop_05_bg", null, "char_mataios_bust_s3_s4", null);
            AssertIconSlot(data, "resource.gold", "icon_gold");
            AssertIconSlot(data, "resource.memory", "icon_memory");
            AssertIconSlot(data, "resource.affinity", "icon_affinity");
            AssertIconSlot(data, "item.field_bandage", "icon_item_field_bandage");
            AssertIconSlot(data, "item.lantern_oil", "icon_item_lantern_oil");
            AssertIconSlot(data, "item.torn_charm", "icon_item_torn_charm");
            AssertIconSlot(data, "ability.scout", "icon_ability_scout");
            AssertIconSlot(data, "ability.recall_anchor", "icon_ability_recall_anchor");
            AssertIconSlot(data, "status.training", "icon_status_training");
            AssertIconSlot(data, "status.bandage", "icon_status_bandage");
            AssertIconSlot(data, "status.recall_anchor", "icon_status_recall_anchor");
        }

        [Test]
        public void Hud_RestActionCardLabelsStayPublicAndHideGlitch()
        {
            AssertRestActionCardLabel("rest.ask_mood", "대화", "마타이오스와 대화");
            AssertRestActionCardLabel("rest.train", "훈련", "다음 전투 피해 +1");
            AssertRestActionCardLabel("rest.recover", "휴식", "HP 회복");
        }

        [Test]
        public void ProjectSettings_UsePortraitDemoResolution()
        {
            Assert.AreEqual(1080, PlayerSettings.defaultScreenWidth);
            Assert.AreEqual(1920, PlayerSettings.defaultScreenHeight);
            Assert.AreEqual(UIOrientation.Portrait, PlayerSettings.defaultInterfaceOrientation);
            Assert.IsTrue(PlayerSettings.allowedAutorotateToPortrait);
            Assert.IsFalse(PlayerSettings.allowedAutorotateToLandscapeLeft);
            Assert.IsFalse(PlayerSettings.allowedAutorotateToLandscapeRight);
        }

        [Test]
        public void CutsceneData_EmptyStepsIsSafeScaffold()
        {
            var cutscene = ScriptableObject.CreateInstance<CutsceneData>();

            Assert.IsFalse(cutscene.HasSteps);
            Assert.AreEqual(0, cutscene.Steps.Length);
        }

        [Test]
        public void ChoiceViews_BuildPublicConsequenceHints()
        {
            var state = new PrototypeRunState("run-ui", new GameFlowEventBus());
            state.ModifyGold(5);
            var encounter = CreateChoiceEncounter();

            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter);

            Assert.AreEqual(3, views.Length);
            Assert.IsTrue(views[0].Enabled);
            StringAssert.Contains("Gold -5", views[0].HintText);
            StringAssert.Contains("ITEM_FIELD_BANDAGE", views[0].HintText);
            Assert.IsFalse(views[1].Enabled);
            StringAssert.Contains("Unavailable: Gold 부족", views[1].HintText);
            StringAssert.Contains("Combat start", views[2].HintText);
        }

        [Test]
        public void JarRoomChoices_ShowProbabilityHintsInPublicUi()
        {
            var jar = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_EVT_F01_JAR_ROOM.asset");
            Assert.IsNotNull(jar);
            var state = new PrototypeRunState("run-ui-jar", new GameFlowEventBus());
            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, jar);
            var hud = CreateHud(out _);

            hud.ShowChoices(jar, views, _ => { });

            Assert.AreEqual(3, hud.ChoiceButtonCount);
            Assert.IsTrue(hud.EventCutsceneVisible);
            StringAssert.Contains("항아리 방", hud.EventCutsceneMessage);
            StringAssert.DoesNotContain("EVT_F01_JAR_ROOM", hud.EventCutsceneMessage);
            StringAssert.DoesNotContain("Glitch", hud.EventCutsceneMessage);
            var patterned = hud.GetChoiceButton(0).GetComponentInChildren<Text>();
            Assert.IsNotNull(patterned);
            StringAssert.Contains("신기한 문양이 각인된 항아리", patterned.text);
            StringAssert.Contains("80%: 골드 획득", patterned.text);
            StringAssert.Contains("20%: 엘리트 전투", patterned.text);
            StringAssert.DoesNotContain("CHOICE_EVT_F01_JAR_PATTERNED", patterned.text);
            StringAssert.DoesNotContain("EVT_F01_JAR_ROOM", patterned.text);
        }

        [Test]
        public void BossGateChoices_ShowOnlyFightAndReturn()
        {
            var boss = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_02.asset");
            Assert.IsNotNull(boss);
            var state = new PrototypeRunState("run-ui-boss", new GameFlowEventBus());
            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, boss);
            var hud = CreateHud(out _);

            hud.ShowChoices(boss, views, _ => { });

            Assert.AreEqual(2, hud.ChoiceButtonCount);
            var fight = hud.GetChoiceButton(0).GetComponentInChildren<Text>();
            var back = hud.GetChoiceButton(1).GetComponentInChildren<Text>();
            Assert.IsNotNull(fight);
            Assert.IsNotNull(back);
            StringAssert.Contains("전투 시작", fight.text);
            StringAssert.Contains("돌아간다", back.text);
            StringAssert.DoesNotContain("준비", fight.text + back.text);
            StringAssert.DoesNotContain("CHOICE_", fight.text + back.text);
        }

        [Test]
        public void Hud_UtilityButtonsExposeStatusMapAndLoadoutWithoutGlitch()
        {
            var hud = CreateHud(out _);
            var snapshot = new PrototypeRunSnapshot(
                "run-utility",
                18,
                24,
                5,
                6,
                30,
                9,
                3,
                1,
                0,
                2,
                false,
                "demo.active",
                currentFloor: 2,
                itemCount: 1);

            hud.ShowRunState(snapshot);

            StringAssert.Contains("상태", hud.UtilityButtonLabels);
            StringAssert.Contains("지도", hud.UtilityButtonLabels);
            StringAssert.Contains("장비", hud.UtilityButtonLabels);
            hud.GetUtilityButton("status").onClick.Invoke();
            Assert.IsTrue(hud.UtilityPanelVisible);
            StringAssert.Contains("HP 18/24", hud.UtilityPanelMessage);
            StringAssert.Contains("ATK 5", hud.UtilityPanelMessage);
            StringAssert.Contains("스킬", hud.UtilityPanelMessage);
            StringAssert.DoesNotContain("Glitch", hud.UtilityPanelMessage);
            hud.GetUtilityButton("loadout").onClick.Invoke();
            StringAssert.Contains("아이템 1", hud.UtilityPanelMessage);
            StringAssert.DoesNotContain("Glitch", hud.UtilityPanelMessage);
        }

        [Test]
        public void Hud_MapScreenShowsNodeIconsAndHidesRawIds()
        {
            var hud = CreateHud(out var result);
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            hud.SetPresentationData(data);
            var nodes = new[]
            {
                new PrototypeFloorMapNodeView("floor1.layer1.event.EVT_F01_JAR_ROOM", PrototypeFloorMapNodeType.Event, 1, 1, 0, true, false, false),
                new PrototypeFloorMapNodeView("floor1.layer1.combat.ENC_COMBAT_GATE_01", PrototypeFloorMapNodeType.Combat, 1, 1, 1, true, false, false),
                new PrototypeFloorMapNodeView("floor1.layer4.shop.ENC_SHOP_01", PrototypeFloorMapNodeType.Shop, 1, 4, 0, true, false, false),
                new PrototypeFloorMapNodeView("floor1.layer5.boss.ENC_COMBAT_GATE_01", PrototypeFloorMapNodeType.Boss, 1, 5, 0, true, false, false)
            };

            hud.ShowMapChoices(nodes, _ => { });

            Assert.IsTrue(hud.HasScreenLayerPanels);
            StringAssert.Contains("node_event", hud.CurrentMapNodeIconNames);
            StringAssert.Contains("node_combat", hud.CurrentMapNodeIconNames);
            StringAssert.Contains("node_shop", hud.CurrentMapNodeIconNames);
            StringAssert.Contains("node_boss", hud.CurrentMapNodeIconNames);
            Assert.AreEqual(4, hud.ChoiceButtonCount);
            StringAssert.Contains("결과", result.text);
            var first = hud.GetChoiceButton(0).GetComponentInChildren<Text>();
            Assert.IsNotNull(first);
            StringAssert.DoesNotContain("EVT_F01_JAR_ROOM", first.text);
            StringAssert.DoesNotContain("floor1.layer1", first.text);
        }

        [Test]
        public void Hud_PortraitShellUsesMobileBoundsAndHidesGlitch()
        {
            var hud = CreateHud(out _);
            var snapshot = new PrototypeRunSnapshot(
                "run-portrait",
                19,
                24,
                5,
                7,
                12,
                3,
                2,
                1,
                0,
                1,
                false,
                "demo.active",
                "node.battle",
                "ENC_COMBAT_GATE_01",
                4,
                1,
                memoryFragmentCount: 1,
                currentFloor: 2,
                itemCount: 2,
                floorMapNodes: new[]
                {
                    new PrototypeFloorMapNodeView("floor2.layer1.combat.ENC_COMBAT_GATE_01", PrototypeFloorMapNodeType.Combat, 2, 1, 0, true, false, false)
                });

            hud.ShowRunState(snapshot);

            Assert.IsTrue(hud.HasPortraitRoot);
            Assert.AreEqual(new Vector2(1080f, 1920f), hud.PortraitRootSize);
            Assert.IsTrue(hud.HasScreenLayerPanels);
            StringAssert.Contains("Floor 2", hud.RunStateMessage);
            StringAssert.Contains("기억 1", hud.RunStateMessage);
            StringAssert.Contains("아이템 2", hud.RunStateMessage);
            StringAssert.DoesNotContain("Glitch", hud.RunStateMessage);
            StringAssert.Contains("지도", hud.RouteMessage);
        }

        [Test]
        public void Hud_CombatScreenShowsIntentWithoutRawEnemyId()
        {
            var hud = CreateHud(out _);
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            hud.SetPresentationData(data);
            var snapshot = new PrototypeRunSnapshot(
                "run-combat-ui",
                15,
                24,
                6,
                0,
                9,
                1,
                1,
                0,
                0,
                0,
                false,
                "demo.active",
                "node.battle",
                "ENC_COMBAT_GATE_03",
                4,
                2,
                currentFloor: 5,
                isInCombat: true,
                lastCombatEnemyId: "BOSS_APEX_02",
                enemyHp: 22,
                enemyMaxHp: 34,
                combatRound: 3,
                lastCombatRoundResult: "round 3 | action Defend | playerDamage 0 | enemyDamage 2");

            hud.ShowRunState(snapshot);

            Assert.IsTrue(hud.CombatPanelVisible);
            Assert.IsTrue(hud.CombatPartyDockVisible);
            Assert.IsTrue(hud.CombatPlayerPortraitVisible);
            Assert.AreEqual("char_player_portrait_01", hud.CurrentCombatPlayerPortraitSpriteName);
            Assert.IsTrue(hud.CombatMataiosPortraitVisible);
            Assert.AreEqual("char_mataios_portrait_01", hud.CurrentCombatMataiosPortraitSpriteName);
            Assert.IsFalse(hud.CombatPortraitFrameVisible);
            StringAssert.Contains("icon_action_attack", hud.CurrentCombatActionIconNames);
            StringAssert.Contains("icon_action_defend", hud.CurrentCombatActionIconNames);
            StringAssert.Contains("icon_action_skill_scout", hud.CurrentCombatActionIconNames);
            StringAssert.Contains("최종 보스", hud.CombatMessage);
            StringAssert.Contains("적 HP 22/34", hud.CombatMessage);
            StringAssert.Contains("방어: 받은 피해", hud.CombatMessage);
            StringAssert.Contains("플레이어", hud.CombatPartyMessage);
            StringAssert.Contains("마타이오스", hud.CombatPartyMessage);
            StringAssert.DoesNotContain("BOSS_APEX_02", hud.CombatMessage);
            StringAssert.DoesNotContain("Glitch", hud.CombatMessage);
            StringAssert.DoesNotContain("Glitch", hud.CombatPartyMessage);
        }

        [Test]
        public void Hud_ResultSummaryShowsPlayerFacingDeltas()
        {
            var hud = CreateHud(out var result);

            hud.ShowResultMessage("choice applied: CHOICE_SHOP_01_BUY_ITEM; effects=2; ignored=0 | Gold -5, item ITEM_FIELD_BANDAGE +1 | memory unlocked MEM_FRAGMENT_03 | stair unlocked");

            StringAssert.Contains("Gold -5", result.text);
            StringAssert.Contains("아이템 +1", result.text);
            StringAssert.Contains("기억 +1", result.text);
            Assert.AreEqual("Gold|Item|Memory", hud.CurrentResultSummaryLabels);
            Assert.AreEqual("-5|+1|+1", hud.CurrentResultSummaryValues);
            StringAssert.DoesNotContain("기억 파편", result.text);
            Assert.LessOrEqual(result.text.Split('\n').Length, 4);
            StringAssert.DoesNotContain("choice applied", result.text);
            StringAssert.DoesNotContain("ITEM_FIELD_BANDAGE", result.text);
            StringAssert.DoesNotContain("MEM_FRAGMENT_03", result.text);
        }

        [Test]
        public void Hud_ResultSummaryShowsJarOutcomeWithoutDebugIds()
        {
            var hud = CreateHud(out var result);

            hud.ShowResultMessage("choice applied: CHOICE_EVT_F01_JAR_PATTERNED; effects=1; ignored=0 | jar outcome: Gold +8 | Glitch -2");

            StringAssert.Contains("Gold +8", result.text);
            Assert.AreEqual("Gold", hud.CurrentResultSummaryLabels);
            Assert.AreEqual("+8", hud.CurrentResultSummaryValues);
            StringAssert.DoesNotContain("CHOICE_EVT_F01_JAR_PATTERNED", result.text);
            StringAssert.DoesNotContain("jar outcome", result.text);
            StringAssert.DoesNotContain("Glitch", result.text);
        }

        [Test]
        public void Hud_PublicRouteAndCombatFeedbackHideDebugIds()
        {
            var hud = CreateHud(out _);
            var node = CreateNode("node.battle");
            var encounter = CreateEncounter("ENC_COMBAT_GATE_03", EncounterType.Battle);
            hud.ConfigureDemoRoute(new[] { new PrototypeDemoRunStep(node, encounter) });
            var snapshot = new PrototypeRunSnapshot(
                "run-ui",
                17,
                20,
                5,
                0,
                8,
                2,
                1,
                0,
                0,
                1,
                false,
                "demo.active",
                "node.battle",
                "ENC_COMBAT_GATE_03",
                4,
                2,
                currentFloor: 5,
                isInCombat: true,
                enemyHp: 14,
                enemyMaxHp: 30,
                combatRound: 2,
                lastCombatRoundResult: "round 2 | action Attack | playerDamage 6 | enemyDamage 3");

            hud.ShowRunState(snapshot);

            Assert.IsEmpty(hud.RouteMessage);
            StringAssert.Contains("공격: 적 피해 6", hud.CombatMessage);
            StringAssert.Contains("정찰 기술: 추가 공격", hud.CombatMessage);
            StringAssert.DoesNotContain("ENC_COMBAT_GATE_03", hud.RouteMessage);
        }

        [Test]
        public void Hud_CombatVisualsCanUseEnemySpecificPresentationSlot()
        {
            var hud = CreateHud(out _);
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            var node = CreateNode("node.battle");
            var encounter = CreateEncounter("ENC_COMBAT_GATE_01", EncounterType.Battle);
            Assert.IsNotNull(data);

            hud.SetPresentationData(data);
            hud.ConfigureDemoRoute(new[] { new PrototypeDemoRunStep(node, encounter) });
            var snapshot = new PrototypeRunSnapshot(
                "run-ui",
                17,
                20,
                5,
                0,
                8,
                2,
                1,
                0,
                0,
                1,
                false,
                "demo.active",
                "node.battle",
                "ENC_COMBAT_GATE_01",
                4,
                2,
                lastCombatId: "COMBAT_TEST",
                lastCombatEnemyId: "ENEMY_SHADE_03",
                currentFloor: 3,
                isInCombat: true,
                enemyHp: 12,
                enemyMaxHp: 18,
                combatRound: 1,
                lastCombatRoundResult: "round 1 | action Attack | playerDamage 4 | enemyDamage 2");

            hud.ShowRunState(snapshot);

            Assert.AreEqual("enemy_shade_03", hud.CurrentCombatEnemySpriteName);
        }

        private static EncounterData CreateChoiceEncounter()
        {
            var encounter = ScriptableObject.CreateInstance<EncounterData>();
            var serialized = new SerializedObject(encounter);
            serialized.FindProperty("id").stringValue = "ENC_TEST_SHOP";
            serialized.FindProperty("type").enumValueIndex = (int)EncounterType.Shop;

            var choices = serialized.FindProperty("choices");
            choices.arraySize = 3;
            ConfigureChoice(
                choices.GetArrayElementAtIndex(0),
                "CHOICE_SHOP_01_BUY_ITEM",
                "PLACEHOLDER_BUY_ITEM",
                5,
                ("ModifyGold", -5, "ITEM_FIELD_BANDAGE"),
                ("AddItem", 1, "ITEM_FIELD_BANDAGE"));
            ConfigureChoice(
                choices.GetArrayElementAtIndex(1),
                "CHOICE_SHOP_01_BUY_ABILITY",
                "PLACEHOLDER_BUY_ABILITY",
                20,
                ("ModifyGold", -20, "ABILITY_SCOUT"),
                ("AddAbility", 1, "ABILITY_SCOUT"));
            ConfigureChoice(
                choices.GetArrayElementAtIndex(2),
                "CHOICE_COMBAT_01_ENGAGE",
                "PLACEHOLDER_COMBAT",
                0,
                ("StartCombat", 0, string.Empty));

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return encounter;
        }

        private static EncounterData CreateEncounter(string id, EncounterType type)
        {
            var encounter = ScriptableObject.CreateInstance<EncounterData>();
            var serialized = new SerializedObject(encounter);
            serialized.FindProperty("id").stringValue = id;
            serialized.FindProperty("type").enumValueIndex = (int)type;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return encounter;
        }

        private static PrototypeNodeDefinition CreateNode(string nodeId)
        {
            var node = ScriptableObject.CreateInstance<PrototypeNodeDefinition>();
            var serialized = new SerializedObject(node);
            serialized.FindProperty("nodeId").stringValue = nodeId;
            serialized.FindProperty("kind").enumValueIndex = (int)NodeKind.Battle;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return node;
        }

        private static void ConfigureChoice(SerializedProperty choice, string stableId, string textKey, int minGold, params (string kind, int amount, string reference)[] effects)
        {
            choice.FindPropertyRelative("stableId").stringValue = stableId;
            choice.FindPropertyRelative("textKey").stringValue = textKey;
            choice.FindPropertyRelative("requirementMode").stringValue = "All";
            choice.FindPropertyRelative("unavailablePolicyMode").stringValue = "DisabledVisible";
            choice.FindPropertyRelative("unavailableReasonTextKey").stringValue = "PLACEHOLDER_REASON_NOT_ENOUGH_GOLD";

            var requirements = choice.FindPropertyRelative("requirements");
            requirements.arraySize = minGold > 0 ? 1 : 0;
            if (minGold > 0)
            {
                var requirement = requirements.GetArrayElementAtIndex(0);
                requirement.FindPropertyRelative("kind").stringValue = "StatAtLeast";
                requirement.FindPropertyRelative("stat").stringValue = "gold";
                requirement.FindPropertyRelative("value").intValue = minGold;
            }

            var effectArray = choice.FindPropertyRelative("effects");
            effectArray.arraySize = effects.Length;
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effectArray.GetArrayElementAtIndex(i);
                effect.FindPropertyRelative("kind").stringValue = effects[i].kind;
                effect.FindPropertyRelative("amount").intValue = effects[i].amount;
                effect.FindPropertyRelative("count").intValue = 1;
                if (effects[i].kind == "AddItem")
                {
                    effect.FindPropertyRelative("itemRef").stringValue = effects[i].reference;
                }
                else if (effects[i].kind == "AddAbility")
                {
                    effect.FindPropertyRelative("abilityRef").stringValue = effects[i].reference;
                }
            }
        }

        private static PrototypeHud CreateHud(out Text result)
        {
            var root = new GameObject("HUD Test");
            var hud = root.AddComponent<PrototypeHud>();
            var focus = CreateText(root.transform, "Focus");
            var interaction = CreateText(root.transform, "Interaction");
            var run = CreateText(root.transform, "Run");
            result = CreateText(root.transform, "Result");
            hud.Configure(focus, interaction, run, result);
            return hud;
        }

        private static Text CreateText(Transform parent, string name)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            textObject.AddComponent<RectTransform>();
            var text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static void AssertSlotSprites(
            DemoPresentationData data,
            string stableId,
            string background,
            string enemy,
            string portrait,
            string memory)
        {
            Assert.IsTrue(data.TryGetSlot(stableId, out var slot), stableId);
            if (background != null)
            {
                Assert.IsNotNull(slot.BackgroundSprite, stableId);
                Assert.AreEqual(background, slot.BackgroundSprite.name);
            }

            if (enemy != null)
            {
                Assert.IsNotNull(slot.EnemySprite, stableId);
                Assert.AreEqual(enemy, slot.EnemySprite.name);
            }

            if (portrait != null)
            {
                Assert.IsNotNull(slot.MataiosPortrait, stableId);
                Assert.AreEqual(portrait, slot.MataiosPortrait.name);
            }

            if (memory != null)
            {
                Assert.IsNotNull(slot.MemoryFragmentSprite, stableId);
                Assert.AreEqual(memory, slot.MemoryFragmentSprite.name);
            }
        }

        private static void AssertNodeIcon(DemoPresentationData data, PrototypeFloorMapNodeType type, string spriteName)
        {
            Assert.IsTrue(data.TryGetNodeIcon(type, out var icon), type.ToString());
            Assert.IsNotNull(icon, type.ToString());
            Assert.AreEqual(spriteName, icon.name);
        }

        private static void AssertCombatActionIcon(DemoPresentationData data, CombatAction action, string spriteNamePrefix)
        {
            Assert.IsTrue(data.TryGetCombatActionIcon(action, out var icon), action.ToString());
            Assert.IsNotNull(icon, action.ToString());
            StringAssert.StartsWith(spriteNamePrefix, icon.name);
        }

        private static void AssertRestActionIcon(DemoPresentationData data, string actionId, string spriteNamePrefix)
        {
            Assert.IsTrue(data.TryGetRestActionIcon(actionId, out var icon), actionId);
            Assert.IsNotNull(icon, actionId);
            StringAssert.StartsWith(spriteNamePrefix, icon.name);
        }

        private static void AssertIconSlot(DemoPresentationData data, string key, string spriteName)
        {
            Assert.IsTrue(data.TryGetIcon(key, out var icon), key);
            Assert.IsNotNull(icon, key);
            Assert.AreEqual(spriteName, icon.name);
        }

        private static void AssertRestActionCardLabel(string actionId, string title, string preview)
        {
            var method = typeof(PrototypeHud).GetMethod("BuildRestActionCardLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Assert.IsNotNull(method);
            var label = (string)method.Invoke(null, new object[] { actionId });
            StringAssert.Contains(title, label);
            StringAssert.Contains(preview, label);
            StringAssert.DoesNotContain("Glitch", label);
        }

        private static void AssertPublicLabel(DemoPresentationData data, string stableId, string expected)
        {
            Assert.IsTrue(data.TryGetSlot(stableId, out var slot), stableId);
            Assert.AreEqual(expected, slot.DisplayName);
            StringAssert.DoesNotContain("Memory", slot.DisplayName);
            StringAssert.DoesNotContain("기억 파편", slot.DisplayName);
        }
    }
}
