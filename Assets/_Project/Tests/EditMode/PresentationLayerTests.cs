using HwigiTower.UI;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

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
    }
}
