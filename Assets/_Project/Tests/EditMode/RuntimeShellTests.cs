using System.Linq;
using HwigiTower.Abilities;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.LLM;
using HwigiTower.NPC;
using HwigiTower.Run;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.Tests.EditMode
{
    public sealed class RuntimeShellTests
    {
        [Test]
        public void EncounterSelector_ReplaysSameChoiceForSameRunAndNode()
        {
            var firstEncounter = CreateEncounter("encounter.a", 1);
            var secondEncounter = CreateEncounter("encounter.b", 9);
            var node = CreateNode("node.battle", firstEncounter, secondEncounter);
            var selector = new EncounterSelector();
            var context = new DeterministicRunContext("run-001", 1001);

            var first = selector.Select(context, node);
            var second = selector.Select(context, node);

            Assert.AreEqual(first.EncounterId, second.EncounterId);
            Assert.IsNotEmpty(first.EncounterId);
        }

        [Test]
        public void CombatController_ResolvesDeterministicRounds()
        {
            var context = new DeterministicRunContext("run-001", 1001);
            var first = new CombatController(context, "battle.01");
            var second = new CombatController(context, "battle.01");
            var firstPlayer = new CombatantState("player", 20, 4);
            var firstEnemy = new CombatantState("enemy", 12, 3);
            var secondPlayer = new CombatantState("player", 20, 4);
            var secondEnemy = new CombatantState("enemy", 12, 3);

            var firstRound = first.ResolveRound(firstPlayer, firstEnemy);
            var secondRound = second.ResolveRound(secondPlayer, secondEnemy);

            Assert.AreEqual(firstRound.PlayerDamage, secondRound.PlayerDamage);
            Assert.AreEqual(firstRound.EnemyDamage, secondRound.EnemyDamage);
        }

        [Test]
        public void SynergyDetector_ActivatesWhenRequiredTagCountIsMet()
        {
            var bus = new GameFlowEventBus();
            var inventory = new AbilityInventory(bus, "run-001");
            inventory.Add(CreateAbility("ability.1", "flame"));
            inventory.Add(CreateAbility("ability.2", "flame"));
            var detector = new SynergyDetector(bus, "run-001");
            var synergy = CreateSynergy("flame", 2);

            var states = detector.Evaluate(inventory.Abilities, new[] { synergy });

            Assert.AreEqual(1, states.Count);
            Assert.IsTrue(states[0].Active);
        }

        [Test]
        public void CachedLLMProvider_ReusesStoredResponse()
        {
            var repo = new InMemoryNpcMemoryRepo();
            var provider = new CachedLLMProvider(repo, new DeterministicFakeLLMProvider());
            var request = new LLMRequest("run-001", "same prompt", "reflection.v1");

            Assert.IsTrue(provider.TryComplete(request, out var first));
            Assert.IsTrue(provider.TryComplete(request, out var second));

            Assert.AreEqual(first.CacheKey, second.CacheKey);
            Assert.AreEqual(first.Text, second.Text);
        }

        [Test]
        public void ReflectionPipeline_SavesReflectionAndBuildsRecentRecallPrompt()
        {
            var repo = new InMemoryNpcMemoryRepo();
            var pipeline = new ReflectionPipeline(repo, new DeterministicFakeLLMProvider(), new GameFlowEventBus());

            Assert.IsTrue(pipeline.TrySaveReflection("run-001", "cleared floor 1", out var reflection));
            var recall = pipeline.LoadRecallPrompt("run-002", 3);

            Assert.IsTrue(reflection.IsValid);
            Assert.IsTrue(recall.Contains("run-001"));
        }

        [Test]
        public void PrototypeRunState_BattleEmitsCombatAndCompletionEvents()
        {
            var bus = new GameFlowEventBus();
            var events = new System.Collections.Generic.List<GameFlowEventType>();
            using (bus.Subscribe(flowEvent => events.Add(flowEvent.Type)))
            {
                var state = new PrototypeRunState("run-001", bus);
                var context = new DeterministicRunContext("run-001", 1001);

                var resolution = state.ResolveBattle(context, "node.battle", "encounter.battle", null, null);

                Assert.IsFalse(string.IsNullOrEmpty(resolution.Message));
                CollectionAssert.Contains(events, GameFlowEventType.CombatStarted);
                CollectionAssert.Contains(events, GameFlowEventType.CombatCompleted);
                CollectionAssert.Contains(events, GameFlowEventType.EncounterCompleted);
            }
        }

        [Test]
        public void PrototypeRunState_RemnantCompletesRunAndSavesReflection()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());

            var resolution = state.ResolveRemnant("node.remnant");

            Assert.IsTrue(resolution.RunCompleted);
            Assert.IsTrue(state.RunCompleted);
            Assert.IsTrue(state.MemoryRepo.TryGetReflection("run-001", out _));
        }

        [Test]
        public void OnDeviceLLMProvider_FallsBackWhenNativePluginIsMissing()
        {
            var provider = new OnDeviceLLMProvider(100, new DeterministicFakeLLMProvider());
            var request = new LLMRequest("run-001", "prompt", "profile");

            Assert.IsTrue(provider.TryComplete(request, out var response));
            Assert.AreEqual(request.CacheKey, response.CacheKey);
        }

        [Test]
        public void SqliteRepo_ExposesSchemaWithoutExternalPlugin()
        {
            var repo = new SqliteNpcMemoryRepo("memory.sqlite");

            Assert.AreEqual(3, repo.SchemaStatements.Length);
            Assert.IsTrue(repo.SchemaStatements.Any(statement => statement.Contains("llm_cache")));
        }

        private static EncounterData CreateEncounter(string id, int weight)
        {
            var encounter = ScriptableObject.CreateInstance<EncounterData>();
            var serialized = new SerializedObject(encounter);
            serialized.FindProperty("id").stringValue = id;
            serialized.FindProperty("weight").intValue = weight;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return encounter;
        }

        private static PrototypeNodeDefinition CreateNode(string nodeId, params EncounterData[] encounters)
        {
            var node = ScriptableObject.CreateInstance<PrototypeNodeDefinition>();
            var serialized = new SerializedObject(node);
            serialized.FindProperty("nodeId").stringValue = nodeId;
            var property = serialized.FindProperty("possibleEncounters");
            property.arraySize = encounters.Length;
            for (var i = 0; i < encounters.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = encounters[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return node;
        }

        private static AbilityData CreateAbility(string id, string tag)
        {
            var ability = ScriptableObject.CreateInstance<AbilityData>();
            var serialized = new SerializedObject(ability);
            serialized.FindProperty("id").stringValue = id;
            serialized.FindProperty("tag").stringValue = tag;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return ability;
        }

        private static SynergyData CreateSynergy(string tag, int requiredCount)
        {
            var synergy = ScriptableObject.CreateInstance<SynergyData>();
            var serialized = new SerializedObject(synergy);
            serialized.FindProperty("tag").stringValue = tag;
            serialized.FindProperty("requiredCount").intValue = requiredCount;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return synergy;
        }
    }
}
