using System.Collections.Generic;
using HwigiTower.Core;
using HwigiTower.NPC;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.Tests.EditMode
{
    public sealed class CoreArchitectureTests
    {
        [Test]
        public void DeterministicRandom_ReplaysSameSequenceForSameSeed()
        {
            var first = new DeterministicRandom(1234);
            var second = new DeterministicRandom(1234);

            Assert.AreEqual(first.Range(0, 100), second.Range(0, 100));
            Assert.AreEqual(first.Range(-5f, 5f), second.Range(-5f, 5f));
            Assert.AreEqual(first.WeightedIndex(new[] { 0, 3, 7 }), second.WeightedIndex(new[] { 0, 3, 7 }));
        }

        [Test]
        public void DeterministicRunContext_CreatesStableSeedKeyForks()
        {
            var context = new DeterministicRunContext("run-001", 4321);
            var first = context.CreateRandom("encounter.floor.01");
            var second = context.CreateRandom("encounter.floor.01");
            var different = context.CreateRandom("encounter.floor.02");

            Assert.AreEqual(first.Range(0, 1000), second.Range(0, 1000));
            Assert.AreNotEqual(first.Range(0, 1000), different.Range(0, 1000));
        }

        [Test]
        public void NpcStateMachine_AppliesMatchingTriggerOnly()
        {
            var definition = ScriptableObject.CreateInstance<NpcStateMachineDefinition>();
            SetDefinition(definition, NpcStage.S0, NpcStage.S0, "run.completed", NpcStage.S1);
            var stateMachine = new NpcStateMachine(definition, "run-001");

            Assert.IsFalse(stateMachine.TryApply("other.trigger"));
            Assert.AreEqual(NpcStage.S0, stateMachine.CurrentStage);

            Assert.IsTrue(stateMachine.TryApply("run.completed"));
            Assert.AreEqual(NpcStage.S1, stateMachine.CurrentStage);
        }

        [Test]
        public void GameFlowEventBus_RaisesEventsInCallOrder()
        {
            var bus = new GameFlowEventBus();
            var events = new List<GameFlowEventType>();
            using (bus.Subscribe(flowEvent => events.Add(flowEvent.Type)))
            {
                bus.Raise(new GameFlowEvent(GameFlowEventType.RunStarted, "run-001", string.Empty, string.Empty));
                bus.Raise(new GameFlowEvent(GameFlowEventType.RoomEntered, "run-001", "room.prototype", string.Empty));
            }

            bus.Raise(new GameFlowEvent(GameFlowEventType.RunCompleted, "run-001", string.Empty, string.Empty));

            CollectionAssert.AreEqual(new[] { GameFlowEventType.RunStarted, GameFlowEventType.RoomEntered }, events);
        }

        private static void SetDefinition(NpcStateMachineDefinition definition, NpcStage initialStage, NpcStage fromStage, string triggerId, NpcStage toStage)
        {
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("initialStage").enumValueIndex = (int)initialStage;
            var transitionsProperty = serialized.FindProperty("transitions");
            transitionsProperty.arraySize = 1;
            var transition = transitionsProperty.GetArrayElementAtIndex(0);
            transition.FindPropertyRelative("fromStage").enumValueIndex = (int)fromStage;
            transition.FindPropertyRelative("triggerId").stringValue = triggerId;
            transition.FindPropertyRelative("toStage").enumValueIndex = (int)toStage;

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
