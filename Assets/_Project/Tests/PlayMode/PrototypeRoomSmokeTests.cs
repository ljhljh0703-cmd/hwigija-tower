using System.Collections;
using System.Collections.Generic;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.Run;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
    }
}
