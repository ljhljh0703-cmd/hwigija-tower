using HwigiTower.UI;
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
        public void Hud_ResultSummaryShowsPlayerFacingDeltas()
        {
            var hud = CreateHud(out var result);

            hud.ShowResultMessage("choice applied: CHOICE_SHOP_01_BUY_ITEM; effects=2; ignored=0 | Gold -5, item ITEM_FIELD_BANDAGE +1 | memory unlocked MEM_FRAGMENT_03 | stair unlocked");

            StringAssert.Contains("Gold -5", result.text);
            StringAssert.Contains("ITEM_FIELD_BANDAGE +1", result.text);
            StringAssert.Contains("MEM_FRAGMENT_03 unlocked", result.text);
            StringAssert.Contains("Next floor ready", result.text);
            StringAssert.DoesNotContain("choice applied", result.text);
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

            StringAssert.Contains("Floor 5", hud.RouteMessage);
            StringAssert.Contains("전투 중", hud.RouteMessage);
            StringAssert.Contains("공격: 피해 6", hud.CombatMessage);
            StringAssert.Contains("Skill ready", hud.CombatMessage);
            StringAssert.DoesNotContain("ENC_COMBAT_GATE_03", hud.RouteMessage);
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
    }
}
