using System.Linq;
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
        public void DemoPresentationData_AllEventEncountersHavePresentationSlots()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            var guids = AssetDatabase.FindAssets("t:EncounterData", new[] { "Assets/_Project/Data/Encounters" });
            var eventCount = 0;
            var missingSlots = 0;

            Assert.IsNotNull(data);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!System.IO.Path.GetFileName(path).StartsWith("SO_Encounter_EVT_", System.StringComparison.Ordinal))
                {
                    continue;
                }

                eventCount++;
                var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>(path);
                Assert.IsNotNull(encounter, path);
                if (!data.TryGetSlot(encounter.Id, out var slot))
                {
                    missingSlots++;
                    continue;
                }

                Assert.IsNotNull(slot.BackgroundSprite, encounter.Id);
                var expectedBackgroundPath = "Assets/_Project/Art/Encounters/" + encounter.Id.ToLowerInvariant() + "_bg.png";
                var expectedBackground = AssetDatabase.LoadAssetAtPath<Sprite>(expectedBackgroundPath);
                if (expectedBackground != null)
                {
                    Assert.AreEqual(expectedBackground.name, slot.BackgroundSprite.name, encounter.Id);
                }
            }

            Assert.AreEqual(33, eventCount);
            Assert.AreEqual(0, missingSlots);
        }

        [Test]
        public void RepresentativeEventEditorSmoke_UsesPublicPresentationSurface()
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            var eventIds = new[]
            {
                "EVT_F01_JAR_ROOM",
                "EVT_F01_CULTIST_FUNERAL",
                "EVT_F02_GARDEN",
                "EVT_F03_BALCONY",
                "EVT_F04_RITUAL_ALTAR",
                "EVT_F05_CRADLE_OF_OUTER_GODS",
                "EVT_F05_OATH_OF_LOYALTY"
            };

            Assert.IsNotNull(data);
            Assert.IsNotNull(catalog);
            for (var i = 0; i < eventIds.Length; i++)
            {
                var eventId = eventIds[i];
                var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_" + eventId + ".asset");
                Assert.IsNotNull(encounter, eventId);
                Assert.IsTrue(data.TryGetSlot(eventId, out var slot), eventId);
                Assert.IsNotNull(slot.BackgroundSprite, eventId);

                var state = new PrototypeRunState("run-event-ui-" + i, new GameFlowEventBus());
                state.AttachEncounterCatalog(catalog);
                state.ModifyGold(999);
                var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter);
                var hud = CreateHud(out _);
                hud.SetPresentationData(data);

                hud.ShowChoices(encounter, views, _ => { });

                Assert.IsTrue(hud.EventCutsceneVisible, eventId);
                Assert.IsNotEmpty(hud.CurrentBackgroundSpriteName, eventId);
                AssertPublicSurface(hud.EventCutsceneMessage, eventId);
                for (var choiceIndex = 0; choiceIndex < hud.ChoiceButtonCount; choiceIndex++)
                {
                    var label = hud.GetChoiceButton(choiceIndex).GetComponentInChildren<Text>();
                    Assert.IsNotNull(label, eventId + " choice " + choiceIndex);
                    AssertPublicSurface(label.text, eventId + " choice " + choiceIndex);
                }
            }
        }

        [Test]
        public void ProjectSettings_UseFinalAndroidIdentity()
        {
            Assert.AreEqual("com.godju.hwigitower", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android));
            Assert.AreEqual("회귀자는 탑을 오른다", PlayerSettings.productName);
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
            AssertRestActionCardLabel("rest.train", "단련", "다음 전투 보너스");
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
            StringAssert.Contains("Gold -20", views[1].HintText);
            StringAssert.Contains("ABILITY_SCOUT", views[1].HintText);
            StringAssert.Contains("Unavailable: Gold 부족", views[1].HintText);
            StringAssert.Contains("Combat start", views[2].HintText);
        }

        [Test]
        public void Hud_ShopDisabledCardsKeepProductComparisonDetails()
        {
            var state = new PrototypeRunState("run-ui-shop-disabled", new GameFlowEventBus());
            state.ModifyGold(5);
            var encounter = CreateChoiceEncounter();
            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter);
            var hud = CreateHud(out _);

            hud.ShowChoices(encounter, views, _ => { });

            var disabledLabel = hud.GetChoiceButton(1).GetComponentInChildren<Text>();
            Assert.IsNotNull(disabledLabel);
            StringAssert.Contains("정찰", disabledLabel.text);
            StringAssert.Contains("Gold -20", disabledLabel.text);
            StringAssert.Contains("Gold 부족", disabledLabel.text);
            StringAssert.DoesNotContain("ABILITY_SCOUT", disabledLabel.text);
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

            Assert.AreEqual(4, hud.ChoiceButtonCount);
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
            var eliteCombat = hud.GetChoiceButton(1).GetComponentInChildren<Text>();
            Assert.IsNotNull(eliteCombat);
            StringAssert.Contains("신기한 문양이 각인된 항아리", eliteCombat.text);
            StringAssert.Contains("20%: 엘리트 전투", eliteCombat.text);
            StringAssert.DoesNotContain("CHOICE_EVT_F01_JAR_ROOM_PATTERNED_ELITE_COMBAT", eliteCombat.text);
        }

        [Test]
        public void BossGateChoices_DoNotShowManualFightButtonAfterMapCommit()
        {
            var boss = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_02.asset");
            Assert.IsNotNull(boss);
            var state = new PrototypeRunState("run-ui-boss", new GameFlowEventBus());
            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, boss);
            var hud = CreateHud(out _);

            hud.ShowChoices(boss, views, _ => { });

            Assert.AreEqual(0, hud.ChoiceButtonCount);
            StringAssert.DoesNotContain("전투 시작", hud.ResultMessage);
            StringAssert.DoesNotContain("CHOICE_", hud.ResultMessage);
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
        public void Hud_LevelRewardPopupShowsGrowthChoices()
        {
            var hud = CreateHud(out _);
            var snapshot = new PrototypeRunSnapshot(
                "run-level-popup",
                18,
                24,
                5,
                0,
                10,
                0,
                0,
                1,
                1,
                0,
                false,
                combatLevel: 2,
                levelUpRewardPending: true,
                pendingLevelRewardChoices: 1,
                lastGrowthMessage: "레벨 2 보상 선택 가능");

            hud.ShowRunState(snapshot);

            Assert.IsTrue(hud.LevelRewardPopupVisible);
            StringAssert.Contains("레벨 상승", hud.LevelRewardPopupMessage);
            StringAssert.Contains("공격력 +1", hud.LevelRewardPopupMessage);
            StringAssert.Contains("최대 HP +4", hud.LevelRewardPopupMessage);
            StringAssert.Contains("마타이오스 HP +3", hud.LevelRewardPopupMessage);
            StringAssert.Contains("Skill CD -1", hud.LevelRewardPopupMessage);
        }

        [Test]
        public void Hud_FloorClearResultHidesMapStatusButtonsAndKeepsObjective()
        {
            var hud = CreateHud(out _);
            var floorNodes = new[]
            {
                new PrototypeFloorMapNodeView("floor.1.layer.5.0.ENC_BOSS", PrototypeFloorMapNodeType.Boss, 1, 5, 0, false, true, false)
            };
            var active = new PrototypeRunSnapshot(
                "run-floor-clear-ui",
                18,
                24,
                5,
                0,
                10,
                0,
                0,
                1,
                1,
                0,
                false,
                currentFloor: 1,
                floorMapNodes: floorNodes);
            hud.ShowRunState(active);
            hud.GetUtilityButton("status").onClick.Invoke();
            Assert.IsTrue(hud.UtilityPanelVisible);

            var cleared = new PrototypeRunSnapshot(
                "run-floor-clear-ui",
                18,
                24,
                5,
                0,
                10,
                0,
                0,
                1,
                1,
                0,
                false,
                currentFloor: 1,
                stairUnlocked: true,
                floorMapNodes: floorNodes);
            hud.ShowRunState(cleared);

            Assert.IsFalse(hud.GetUtilityButton("map").gameObject.activeSelf);
            Assert.IsFalse(hud.GetUtilityButton("status").gameObject.activeSelf);
            Assert.IsFalse(hud.UtilityPanelVisible);
            StringAssert.Contains("Floor 1 완료", hud.RouteMessage);
            StringAssert.Contains("다음 층으로 올라가세요", hud.RouteMessage);
        }

        [Test]
        public void Hud_BossClearRewardUsesProminentPopupWithNextFloorCta()
        {
            var hud = CreateHud(out _);
            hud.ShowResultMessage("combat victory | enemyDefeated True | gold reward 9 | affinity +1 | stair unlocked");
            hud.ShowRunState(new PrototypeRunSnapshot(
                "run-boss-popup",
                18,
                24,
                5,
                0,
                19,
                0,
                1,
                3,
                1,
                0,
                false,
                lastCombatResultId: "victory",
                lastCombatGoldReward: 9,
                lastCombatAffinityDelta: 1,
                lastCombatEnemyDefeated: true,
                currentFloor: 1,
                stairUnlocked: true));

            Assert.IsTrue(hud.BossRewardPopupVisible);
            StringAssert.Contains("보스 격파", hud.BossRewardPopupMessage);
            StringAssert.Contains("Gold +9", hud.BossRewardPopupMessage);
            StringAssert.Contains("신뢰 +1", hud.BossRewardPopupMessage);
            Assert.IsTrue(hud.BossRewardNextFloorButtonVisible);
            Assert.IsFalse(hud.RouteActionButtonVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
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
            Assert.IsFalse(hud.ResultPanelVisible);
            StringAssert.DoesNotContain("결과\n-", result.text);
            var first = hud.GetChoiceButton(0).GetComponentInChildren<Text>();
            Assert.IsNotNull(first);
            StringAssert.DoesNotContain("EVT_F01_JAR_ROOM", first.text);
            StringAssert.DoesNotContain("floor1.layer1", first.text);
        }

        [Test]
        public void FreshRun_Floor1MapShowsSelectableNodes()
        {
            var hud = CreateHud(out _);
            var controller = CreateConfiguredRoomController(hud);
            try
            {
                controller.BeginRun();
                controller.ConfirmPreRunPlaceholder();

                hud.ShowRunState(controller.GetSnapshot());

                Assert.IsTrue(hud.NodeMapVisible);
                Assert.Greater(hud.ChoiceButtonCount, 0);
                Assert.IsNotNull(FindFirstInteractableMapChoiceButton(hud));
                Assert.IsFalse(hud.ResultPanelVisible);
                Assert.IsFalse(hud.EventCutsceneVisible);
            }
            finally
            {
                Object.DestroyImmediate(controller.gameObject);
                Object.DestroyImmediate(hud.gameObject);
            }
        }

        [Test]
        public void MapState_DoesNotShowEventResultPanel()
        {
            var hud = CreateHud(out var result);
            var nodes = CreateFloorOneMapNodes();

            hud.ShowResultMessage(string.Empty);
            hud.ShowMapChoices(nodes, _ => { });

            Assert.IsTrue(hud.NodeMapVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
            StringAssert.DoesNotContain("결과\n-", result.text);
        }

        [Test]
        public void MapState_DoesNotUseEventPresentationBackground()
        {
            var hud = CreateHud(out _);
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_EVT_F01_JAR_ROOM.asset");
            Assert.IsNotNull(data);
            Assert.IsNotNull(catalog);
            Assert.IsNotNull(encounter);
            hud.SetPresentationData(data);
            var state = new PrototypeRunState("run-map-bg", new GameFlowEventBus());
            state.AttachEncounterCatalog(catalog);

            hud.ShowChoices(encounter, PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter), _ => { });
            Assert.IsTrue(hud.EventCutsceneVisible);
            Assert.AreEqual("evt_f01_jar_room_bg", hud.CurrentBackgroundSpriteName);

            hud.ShowMapChoices(CreateFloorOneMapNodes(), _ => { });

            Assert.IsTrue(hud.NodeMapVisible);
            Assert.IsFalse(hud.EventCutsceneVisible);
            Assert.AreEqual(string.Empty, hud.CurrentBackgroundSpriteName);
        }

        [Test]
        public void CombatNodeTap_EntersCombatState()
        {
            var hud = CreateHud(out _);
            var room = CreateCombatFirstRoomDefinition();
            var controller = CreateConfiguredRoomController(hud, room);
            try
            {
                controller.AutoResolveCombat = false;
                controller.BeginRun();
                controller.ConfirmPreRunPlaceholder();
                hud.ShowRunState(controller.GetSnapshot());

                var combatNode = FindFirstInteractableMapChoiceButton(hud, "ENC_COMBAT");
                Assert.IsNotNull(combatNode, "Expected a selectable Floor 1 combat map node.");

                combatNode.onClick.Invoke();

                Assert.IsTrue(controller.RunState.IsInCombat);
                Assert.IsTrue(hud.CombatPanelVisible);
                Assert.IsFalse(hud.NodeMapVisible);
                StringAssert.Contains("공격", hud.CombatActionButtonLabels);
                StringAssert.Contains("방어", hud.CombatActionButtonLabels);
            }
            finally
            {
                Object.DestroyImmediate(controller.gameObject);
                Object.DestroyImmediate(hud.gameObject);
                Object.DestroyImmediate(room);
            }
        }

        [Test]
        public void FreshRun_FirstCombatNodeTap_AutoStartsCombat()
        {
            var hud = CreateHud(out _);
            var controller = CreateConfiguredRoomController(hud);
            try
            {
                controller.AutoResolveCombat = false;
                controller.BeginRun();
                controller.ConfirmPreRunPlaceholder();
                AdvanceRunStateUntilEncounterSelectable(controller.RunState, "ENC_COMBAT_GATE_01");
                hud.ShowRunState(controller.GetSnapshot());

                var combatNode = FindFirstInteractableMapChoiceButton(hud, "ENC_COMBAT_GATE_01");
                Assert.IsNotNull(combatNode, "Expected a selectable combat map node.");
                combatNode.onClick.Invoke();

                Assert.IsTrue(controller.GetSnapshot().IsInCombat);
                Assert.IsTrue(controller.RunState.IsInCombat);
            }
            finally
            {
                Object.DestroyImmediate(controller.gameObject);
                Object.DestroyImmediate(hud.gameObject);
            }
        }

        [Test]
        public void CombatAutoStart_HidesMapResultAndMemorySurfaces()
        {
            var hud = CreateHud(out _);
            var room = CreateCombatFirstRoomDefinition();
            var controller = CreateConfiguredRoomController(hud, room);
            try
            {
                controller.AutoResolveCombat = false;
                controller.BeginRun();
                controller.ConfirmPreRunPlaceholder();
                hud.ShowRunState(controller.GetSnapshot());

                FindFirstInteractableMapChoiceButton(hud, "ENC_COMBAT").onClick.Invoke();

                Assert.IsFalse(hud.NodeMapVisible);
                Assert.IsFalse(hud.ResultPanelVisible);
                Assert.IsFalse(hud.EventCutsceneVisible);
                Assert.IsFalse(hud.MemoryPanelVisible);
                Assert.IsFalse(hud.RouteHeaderVisible);
                StringAssert.DoesNotContain("결과\n-", hud.ResultMessage);
                StringAssert.DoesNotContain("갈림길 선택", hud.RouteMessage);
            }
            finally
            {
                Object.DestroyImmediate(controller.gameObject);
                Object.DestroyImmediate(hud.gameObject);
                Object.DestroyImmediate(room);
            }
        }

        [Test]
        public void CombatAutoStart_ShowsEnemyAndActionButtons()
        {
            var hud = CreateHud(out _);
            var room = CreateCombatFirstRoomDefinition();
            var controller = CreateConfiguredRoomController(hud, room);
            try
            {
                controller.AutoResolveCombat = false;
                controller.BeginRun();
                controller.ConfirmPreRunPlaceholder();
                hud.ShowRunState(controller.GetSnapshot());

                FindFirstInteractableMapChoiceButton(hud, "ENC_COMBAT").onClick.Invoke();

                Assert.IsTrue(hud.CombatPanelVisible);
                Assert.IsTrue(hud.CombatEnemyVisible);
                StringAssert.Contains("적 HP", hud.CombatMessage);
                StringAssert.Contains("공격", hud.CombatActionButtonLabels);
                StringAssert.Contains("방어", hud.CombatActionButtonLabels);
                StringAssert.Contains("스킬", hud.CombatActionButtonLabels);
            }
            finally
            {
                Object.DestroyImmediate(controller.gameObject);
                Object.DestroyImmediate(hud.gameObject);
                Object.DestroyImmediate(room);
            }
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
            StringAssert.Contains("HP 19/24", hud.RunStateMessage);
            StringAssert.Contains("Gold 12", hud.RunStateMessage);
            StringAssert.Contains("이성 7", hud.RunStateMessage);
            StringAssert.DoesNotContain("기억", hud.RunStateMessage);
            StringAssert.DoesNotContain("능력", hud.RunStateMessage);
            StringAssert.DoesNotContain("아이템", hud.RunStateMessage);
            StringAssert.DoesNotContain("정신", hud.RunStateMessage);
            StringAssert.DoesNotContain("Glitch", hud.RunStateMessage);
            Assert.AreEqual("갈림길 선택", hud.RouteMessage);
            Assert.LessOrEqual(hud.NodeMapLayerAnchorMax.y, hud.RouteHeaderAnchorMin.y);
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
                enemyAttack: 7,
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
            StringAssert.Contains("ATK 7", hud.CombatEnemyStatusMessage);
            StringAssert.Contains("최종 보스", hud.CombatMessage);
            StringAssert.Contains("적 HP 22/34", hud.CombatMessage);
            StringAssert.Contains("선택 방어 | 받은 피해", hud.CombatMessage);
            StringAssert.Contains("절반 감소", hud.CombatMessage);
            StringAssert.Contains("플레이어", hud.CombatPartyMessage);
            StringAssert.Contains("마타이오스", hud.CombatPartyMessage);
            StringAssert.DoesNotContain("BOSS_APEX_02", hud.CombatMessage);
            StringAssert.DoesNotContain("Glitch", hud.CombatMessage);
            StringAssert.DoesNotContain("Glitch", hud.CombatPartyMessage);
            Assert.AreEqual(22f / 34f, hud.CurrentEnemyHpFillAmount, 0.001f);
            Assert.AreEqual(15f / 24f, hud.CurrentPlayerHpFillAmount, 0.001f);
            Assert.AreEqual(1f, hud.CurrentMataiosHpFillAmount, 0.001f);
        }

        [Test]
        public void Hud_CombatSummaryShowsBuildAndMataiosAssist()
        {
            var hud = CreateHud(out _);
            var snapshot = new PrototypeRunSnapshot(
                "run-combat-build-ui",
                15,
                26,
                7,
                0,
                9,
                1,
                1,
                0,
                0,
                2,
                false,
                isInCombat: true,
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                enemyHp: 8,
                enemyMaxHp: 12,
                enemyAttack: 3,
                combatRound: 2,
                lastCombatRoundResult: "round 2 | action Defend | playerDamage 0 | enemyDamage 1 | mataios protect protect -2",
                lastMataiosProtectReduction: 2,
                levelAttackBonus: 1,
                levelMaxHpBonus: 4,
                skillCooldownReduction: 1,
                combatBuildSummary: "성장 Lv 2 XP 0/20 | ATK +1 | Max HP +4 | Skill CD -1\n정찰: 다음 공격 강화 / 피해 감소 1회 | 특성 없음");

            hud.ShowRunState(snapshot);

            StringAssert.Contains("마타이오스 보호", hud.CombatMessage);
            StringAssert.Contains("성장", hud.CombatMessage);
            StringAssert.Contains("상세 접힘", hud.CombatMessage);
            StringAssert.DoesNotContain("Skill CD -1", hud.CombatMessage);
            StringAssert.Contains("성장 ATK +1", hud.CombatPartyMessage);
            StringAssert.Contains("성장 HP +4", hud.CombatPartyMessage);
        }

        [Test]
        public void Hud_CombatIntroBlocksActionsWithoutManualStartButton()
        {
            var hud = CreateHud(out _);
            var snapshot = new PrototypeRunSnapshot(
                "run-combat-intro",
                24,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_TEST",
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                lastCombatResultId: "started",
                lastCombatRoundResult: "round 0 | ready",
                isInCombat: true,
                enemyHp: 12,
                enemyMaxHp: 12,
                enemyAttack: 3);

            hud.ShowRunState(snapshot);

            Assert.IsTrue(hud.CombatPanelVisible);
            Assert.IsTrue(hud.CombatIntroOverlayVisible);
            Assert.AreEqual(0, hud.ChoiceButtonCount);
            StringAssert.DoesNotContain("전투 시작", hud.ResultMessage);
        }

        [Test]
        public void CombatPresentation_StillSeparatesPlayerAndMataiosDamage()
        {
            var hud = CreateHud(out _);
            var before = new PrototypeRunSnapshot(
                "run-combat-damage",
                20,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_TEST",
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                lastCombatResultId: "started",
                lastCombatRoundResult: "round 0 | ready",
                isInCombat: true,
                enemyHp: 12,
                enemyMaxHp: 12,
                enemyAttack: 3,
                combatRound: 0);
            var after = new PrototypeRunSnapshot(
                "run-combat-damage",
                18,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_TEST",
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                lastCombatResultId: "started",
                lastCombatRoundResult: "round 1 | action Attack | playerDamage 7 | enemyDamage 2 | enemyHp 12->3 | playerHp 20->18 | mataios support 2",
                isInCombat: true,
                enemyHp: 3,
                enemyMaxHp: 12,
                enemyAttack: 3,
                combatRound: 1,
                lastMataiosCombatDamage: 2);

            hud.ShowRunState(before);
            hud.ShowRunState(after);

            StringAssert.Contains("-7", hud.CombatDamageNumberMessage);
            StringAssert.Contains("마타이오스 -2", hud.CombatDamageNumberMessage);
            StringAssert.Contains("플레이어: 7 피해", hud.CombatMessage);
            StringAssert.Contains("HP 20->18", hud.CombatMessage);
            StringAssert.Contains("마타이오스: 2 지원 피해", hud.CombatMessage);
        }

        [Test]
        public void Hud_PlayerHitFeedbackShowsLocalDamageState()
        {
            var hud = CreateHud(out _);
            var before = new PrototypeRunSnapshot(
                "run-player-hit",
                20,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_TEST",
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                lastCombatResultId: "started",
                lastCombatRoundResult: "round 0 | ready",
                isInCombat: true,
                enemyHp: 12,
                enemyMaxHp: 12,
                enemyAttack: 3,
                combatRound: 0);
            var after = new PrototypeRunSnapshot(
                "run-player-hit",
                17,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_TEST",
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                lastCombatResultId: "started",
                lastCombatRoundResult: "round 1 | action Defend | playerDamage 0 | enemyDamage 3 | enemyHp 12->12 | playerHp 20->17",
                isInCombat: true,
                enemyHp: 12,
                enemyMaxHp: 12,
                enemyAttack: 3,
                combatRound: 1);

            hud.ShowRunState(before);
            hud.ShowRunState(after);

            StringAssert.Contains("-3", hud.CombatDamageNumberMessage);
            Assert.IsTrue(hud.CombatPlayerHitFeedbackActive);
        }

        [Test]
        public void Hud_EnemyDefeatFeedbackAppearsBeforeResultSurface()
        {
            var hud = CreateHud(out _);
            var defeated = new PrototypeRunSnapshot(
                "run-defeat-feedback",
                18,
                24,
                5,
                0,
                8,
                0,
                1,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_BOSS",
                lastCombatEnemyId: "BOSS_GATE_01",
                lastCombatResultId: "victory",
                lastCombatRoundResult: "round 3 | action Attack | playerDamage 8 | enemyDamage 0 | enemyHp 8->0 | playerHp 18->18",
                isInCombat: false,
                enemyHp: 0,
                enemyMaxHp: 48,
                enemyAttack: 4,
                combatRound: 3,
                lastCombatEnemyDefeated: true);

            hud.ShowRunState(defeated);

            Assert.IsTrue(hud.CombatPanelVisible);
            Assert.IsTrue(hud.CombatDefeatFeedbackVisible);
            Assert.IsFalse(hud.ResultPanelVisible);
            StringAssert.Contains("보스 격파", hud.CombatDefeatFeedbackMessage);
        }

        [Test]
        public void CombatVictory_ResultPanelDelayedUntilDefeatFeedbackCompletes()
        {
            var hud = CreateHud(out _);
            var defeated = new PrototypeRunSnapshot(
                "run-result-delay",
                18,
                24,
                5,
                0,
                8,
                0,
                1,
                0,
                0,
                0,
                false,
                lastCombatId: "COMBAT_DELAY",
                lastCombatEnemyId: "ENEMY_EMPTY_ARMOR",
                lastCombatResultId: "victory",
                lastCombatRoundResult: "round 2 | action Attack | playerDamage 12 | enemyDamage 0 | enemyHp 12->0 | playerHp 18->18",
                isInCombat: false,
                enemyHp: 0,
                enemyMaxHp: 12,
                enemyAttack: 3,
                combatRound: 2,
                lastCombatEnemyDefeated: true);

            hud.ShowResultMessage("combat victory | enemyDefeated True | gold reward 4");
            hud.ShowRunState(defeated);

            Assert.IsTrue(hud.CombatDefeatFeedbackVisible);
            Assert.IsFalse(hud.ResultPanelVisible);

            AdvanceCombatDefeatFeedback(hud, 1.99f);
            Assert.IsTrue(hud.CombatDefeatFeedbackVisible);
            Assert.IsFalse(hud.ResultPanelVisible);

            AdvanceCombatDefeatFeedback(hud, 0.02f);
            Assert.IsFalse(hud.CombatDefeatFeedbackVisible);
            Assert.IsTrue(hud.ResultPanelVisible);
            StringAssert.Contains("Gold", hud.ResultMessage);
        }

        [Test]
        public void Hud_LowHpWarningUsesConfiguredThresholdAndClearsOnHeal()
        {
            var hud = CreateHud(out _);
            hud.ShowRunState(new PrototypeRunSnapshot(
                "run-low-hp",
                6,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false));

            Assert.IsTrue(hud.LowHpWarningVisible);

            hud.ShowRunState(new PrototypeRunSnapshot(
                "run-low-hp",
                12,
                24,
                5,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                false));

            Assert.IsFalse(hud.LowHpWarningVisible);
        }

        [Test]
        public void Hud_RestInteractionKeepsBackgroundAndMovesChoiceUiUp()
        {
            var hud = CreateHud(out _);
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            hud.SetPresentationData(data);
            var restNode = CreateNode("node.rest");
            var restEncounter = CreateEncounter("ENC_REST_UNKNOWN", EncounterType.Rest);
            var selection = new EncounterSelection(restNode, restEncounter);
            var method = typeof(PrototypeHud).GetMethod("ShowRestInteraction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(method);
            method.Invoke(hud, new object[] { selection });

            Assert.IsTrue(hud.RestInteractionPanelVisible);
            Assert.AreEqual("enc_rest_01_bg", hud.CurrentBackgroundSpriteName);
            Assert.GreaterOrEqual(hud.RestInteractionAnchorMin.y, 0.45f);
        }

        [Test]
        public void Hud_ResultSummaryShowsPlayerFacingDeltas()
        {
            var hud = CreateHud(out var result);

            hud.ShowResultMessage("choice applied: CHOICE_SHOP_01_BUY_ITEM; effects=2; ignored=0 | Gold -5, item ITEM_FIELD_BANDAGE +1 | memory unlocked MEM_FRAGMENT_03 | stair unlocked");

            StringAssert.Contains("Gold -5", result.text);
            StringAssert.Contains("아이템 +1", result.text);
            StringAssert.Contains("기억의 잔향을 얻었습니다.", result.text);
            Assert.AreEqual("Gold|Item|기억의 잔향", hud.CurrentResultSummaryLabels);
            Assert.AreEqual("-5|+1|획득", hud.CurrentResultSummaryValues);
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
                lastCombatRoundResult: "round 2 | action Attack | playerDamage 6 | enemyDamage 3",
                combatBuildSummary: "정찰: 다음 공격 강화 / 피해 감소 1회");

            hud.ShowRunState(snapshot);

            Assert.IsEmpty(hud.RouteMessage);
            StringAssert.Contains("선택 공격 | 적 피해 6", hud.CombatMessage);
            StringAssert.Contains("받은 피해 3", hud.CombatMessage);
            StringAssert.Contains("정찰: 다음 공격 강화", hud.CombatMessage);
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

        private static PrototypeRoomController CreateConfiguredRoomController(PrototypeHud hud, PrototypeRoomDefinition roomOverride = null)
        {
            var room = roomOverride != null
                ? roomOverride
                : AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            var presentation = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            Assert.IsNotNull(room);
            Assert.IsNotNull(catalog);
            Assert.IsNotNull(presentation);

            hud.SetPresentationData(presentation);
            var controllerObject = new GameObject("PrototypeRoomController Map Flow Test");
            var controller = controllerObject.AddComponent<PrototypeRoomController>();
            controller.Configure(room, catalog);
            controller.SetDemoPresentationData(presentation);
            var serialized = new SerializedObject(controller);
            serialized.FindProperty("hud").objectReferenceValue = hud;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            hud.BindRoomController(controller);
            return controller;
        }

        private static PrototypeRoomDefinition CreateCombatFirstRoomDefinition()
        {
            var room = ScriptableObject.CreateInstance<PrototypeRoomDefinition>();
            var node = AssetDatabase.LoadAssetAtPath<PrototypeNodeDefinition>("Assets/_Project/Data/Prototype/Nodes/SO_Node_Battle.asset");
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_01.asset");
            Assert.IsNotNull(node);
            Assert.IsNotNull(encounter);

            var serialized = new SerializedObject(room);
            serialized.FindProperty("roomId").stringValue = "room.test.combat-first";
            serialized.FindProperty("deterministicSeed").intValue = 1001;
            var availableNodes = serialized.FindProperty("availableNodes");
            availableNodes.arraySize = 1;
            availableNodes.GetArrayElementAtIndex(0).objectReferenceValue = node;

            var demoRunPath = serialized.FindProperty("demoRunPath");
            demoRunPath.arraySize = 1;
            AssignRunStep(demoRunPath.GetArrayElementAtIndex(0), node, encounter);

            var floorRunPaths = serialized.FindProperty("floorRunPaths");
            floorRunPaths.arraySize = 1;
            var floorRunPath = floorRunPaths.GetArrayElementAtIndex(0);
            floorRunPath.FindPropertyRelative("floor").intValue = 1;
            var steps = floorRunPath.FindPropertyRelative("steps");
            steps.arraySize = 1;
            AssignRunStep(steps.GetArrayElementAtIndex(0), node, encounter);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return room;
        }

        private static void AssignRunStep(SerializedProperty step, PrototypeNodeDefinition node, EncounterData encounter)
        {
            step.FindPropertyRelative("node").objectReferenceValue = node;
            step.FindPropertyRelative("encounter").objectReferenceValue = encounter;
        }

        private static PrototypeFloorMapNodeView[] CreateFloorOneMapNodes()
        {
            return new[]
            {
                new PrototypeFloorMapNodeView("floor1.layer1.event.EVT_F01_JAR_ROOM", PrototypeFloorMapNodeType.Event, 1, 1, 0, true, false, false),
                new PrototypeFloorMapNodeView("floor1.layer1.combat.ENC_COMBAT_GATE_01", PrototypeFloorMapNodeType.Combat, 1, 1, 1, true, false, false),
                new PrototypeFloorMapNodeView("floor1.layer4.shop.ENC_SHOP_01", PrototypeFloorMapNodeType.Shop, 1, 4, 0, false, false, true),
                new PrototypeFloorMapNodeView("floor1.layer5.boss.ENC_COMBAT_GATE_01", PrototypeFloorMapNodeType.Boss, 1, 5, 0, false, false, true)
            };
        }

        private static Button FindFirstInteractableMapChoiceButton(PrototypeHud hud, string requiredNamePart = null)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button == null ||
                    !button.interactable ||
                    !button.name.StartsWith("Map Node Button ", System.StringComparison.Ordinal))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(requiredNamePart) || button.name.Contains(requiredNamePart))
                {
                    return button;
                }
            }

            return null;
        }

        private static void AdvanceCombatDefeatFeedback(PrototypeHud hud, float seconds)
        {
            var method = typeof(PrototypeHud).GetMethod(
                "UpdateCombatDefeatFeedback",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            method.Invoke(hud, new object[] { seconds });
        }

        private static void AdvanceRunStateUntilEncounterSelectable(PrototypeRunState state, string expectedEncounterId)
        {
            var guard = 0;
            while (guard++ < 16)
            {
                var selectable = state.GetSelectableMapNodeViews();
                var match = selectable.FirstOrDefault(node => node.MapNodeId.Contains("." + expectedEncounterId));
                if (!string.IsNullOrEmpty(match.MapNodeId))
                {
                    return;
                }

                Assert.Greater(selectable.Length, 0, "Expected selectable map nodes while advancing toward " + expectedEncounterId);
                var selected = selectable.FirstOrDefault(node => !node.MapNodeId.Contains(".ENC_COMBAT_GATE_"));
                if (string.IsNullOrEmpty(selected.MapNodeId))
                {
                    selected = selectable[0];
                }

                Assert.IsTrue(state.TrySelectMapNode(selected.MapNodeId, out var step), "Expected selectable map node " + selected.MapNodeId);
                Assert.IsFalse(step.EncounterId == expectedEncounterId, "Expected target node to remain selectable for UI tap.");
                var choiceStableId = ResolvePreferredRouteChoiceId(step);
                var resolution = state.ResolveEncounterChoice(new DeterministicRunContext(state.RunId, 1001), step.NodeId, step.Encounter, choiceStableId);
                Assert.AreEqual(choiceStableId, resolution.PayloadId);
                Assert.IsFalse(state.IsInCombat, "Route setup should not start combat before tapping " + expectedEncounterId);
            }

            Assert.Fail("Could not make encounter selectable: " + expectedEncounterId);
        }

        private static string ResolvePreferredRouteChoiceId(PrototypeDemoRunStep step)
        {
            switch (step.EncounterId)
            {
                case "EVT_F01_JAR_ROOM":
                    return "CHOICE_EVT_F01_JAR_ROOM_PLAIN";
                case "ENC_SHOP_01":
                    return "CHOICE_SHOP_01_LEAVE";
                case "ENC_REST_01":
                    return "CHOICE_REST_01_REST";
                case "ENC_MORAL_CHOICE_01":
                    return "CHOICE_MORAL_01_REFUSE";
                case "ENC_MEMORY_FRAGMENT_01":
                    return "CHOICE_MEMORY_01_UNLOCK";
                default:
                    Assert.IsNotNull(step.Encounter);
                    Assert.IsNotNull(step.Encounter.Choices);
                    Assert.Greater(step.Encounter.Choices.Length, 0, "Missing route choice for " + step.EncounterId);
                    return step.Encounter.Choices[0].stableId;
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

        private static void AssertPublicSurface(string text, string context)
        {
            Assert.IsNotNull(text, context);
            StringAssert.DoesNotContain("REWARD_CACHE_MEMORY", text, context);
            StringAssert.DoesNotContain("MoralChoice", text, context);
            StringAssert.DoesNotContain("도덕 선택", text, context);
            StringAssert.DoesNotContain("MEM_FRAGMENT_", text, context);
            StringAssert.DoesNotContain("TriggerGameOver", text, context);
            StringAssert.DoesNotContain("PLACEHOLDER_EVT_", text, context);
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
