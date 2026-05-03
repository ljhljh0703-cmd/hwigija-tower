using System.Linq;
using System.Collections.Generic;
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
        public void PrototypeRunState_RestRestoresPersistentPlayerHp()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            var context = new DeterministicRunContext("run-001", 1001);

            state.ResolveBattle(context, "node.battle", "encounter.battle", null, null);
            var afterBattle = state.CreateSnapshot();
            state.ResolveRest("node.rest");
            var afterRest = state.CreateSnapshot();

            Assert.LessOrEqual(afterBattle.PlayerHp, afterBattle.PlayerMaxHp);
            Assert.AreEqual(afterRest.PlayerMaxHp, afterRest.PlayerHp);
        }

        [Test]
        public void CombatAbilityModifiers_UsesAbilityNumericParams()
        {
            var ability = CreateAbility("ability.1", "tag");
            SetNumericParams(ability, ("player.attack_bonus", 2f), ("player.max_hp_bonus", 4f));

            var modifiers = CombatAbilityModifiers.From(new[] { ability }, null);

            Assert.AreEqual(2, modifiers.PlayerAttackBonus);
            Assert.AreEqual(4, modifiers.PlayerMaxHpBonus);
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
        public void PrototypeRunState_ClampsCoreRunStateValues()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());

            state.ModifyMental(150);
            state.ModifyGold(1200);
            state.ModifyGlitchLevel(120);
            state.ModifyAffinity(-150);
            var upper = state.CreateSnapshot();

            Assert.AreEqual(100, upper.Mental);
            Assert.AreEqual(999, upper.Gold);
            Assert.AreEqual(100, upper.GlitchLevel);
            Assert.AreEqual(-100, upper.Affinity);

            state.ModifyMental(-250);
            state.ModifyGold(-1200);
            state.ModifyGlitchLevel(-150);
            state.ModifyAffinity(250);
            var lower = state.CreateSnapshot();

            Assert.AreEqual(-100, lower.Mental);
            Assert.AreEqual(0, lower.Gold);
            Assert.AreEqual(0, lower.GlitchLevel);
            Assert.AreEqual(100, lower.Affinity);
        }

        [Test]
        public void PrototypeRunState_ShopFailsWhenGoldIsInsufficient()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            var ability = CreateAbility("ability.shop", "shop");

            var resolution = state.ResolveShop("node.shop", ability, null);
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual(0, snapshot.Gold);
            Assert.AreEqual(0, snapshot.AbilityCount);
            Assert.AreEqual(string.Empty, resolution.PayloadId);
            Assert.IsTrue(resolution.Message.Contains("purchase failed"));
        }

        [Test]
        public void PrototypeRunState_ShopSpendsGoldAndGrantsAbility()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            var ability = CreateAbility("ability.shop", "shop");
            state.ModifyGold(10);

            var resolution = state.ResolveShop("node.shop", ability, null);
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual(0, snapshot.Gold);
            Assert.AreEqual(1, snapshot.AbilityCount);
            Assert.AreEqual("ability.shop", resolution.PayloadId);
            Assert.IsTrue(resolution.Message.Contains("purchase success"));
        }

        [Test]
        public void PrototypeRunState_ModifiesGlitchAndAffinity()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());

            state.ModifyGlitchLevel(35);
            state.ModifyAffinity(-20);
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual(35, snapshot.GlitchLevel);
            Assert.AreEqual(-20, snapshot.Affinity);
        }

        [Test]
        public void EncounterRuntimeResolver_AppliesChoiceEffects()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            state.ModifyGold(10);
            var encounter = CreateRuntimeEncounter(
                "encounter.moral",
                CreateChoice(
                    "choice.help",
                    new[] { CreateRequirement("StatAtLeast", "gold", 3) },
                    new[]
                    {
                        CreateEffect("ModifyGold", -3),
                        CreateEffect("ModifyAffinity", 8),
                        CreateEffect("ModifyGlitchLevel", -4),
                        CreateFlagEffect("FLAG_HELPED", true)
                    }));

            var resolution = state.ResolveEncounterChoice("node.moral", encounter, "choice.help");
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual("choice.help", resolution.PayloadId);
            Assert.AreEqual(7, snapshot.Gold);
            Assert.AreEqual(8, snapshot.Affinity);
            Assert.AreEqual(0, snapshot.GlitchLevel);
            Assert.IsTrue(state.HasFlag("FLAG_HELPED"));
        }

        [Test]
        public void EncounterRuntimeResolver_BlocksChoiceWhenRequirementFails()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            var encounter = CreateRuntimeEncounter(
                "encounter.shop",
                CreateChoice(
                    "choice.buy",
                    new[] { CreateRequirement("StatAtLeast", "gold", 6) },
                    new[]
                    {
                        CreateEffect("ModifyGold", -6),
                        CreateItemEffect("ITEM_FIELD_BANDAGE", 1)
                    }));

            var resolution = state.ResolveEncounterChoice("node.shop", encounter, "choice.buy");
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual("choice.buy", resolution.PayloadId);
            Assert.AreEqual(0, snapshot.Gold);
            Assert.AreEqual(0, state.GetItemCount("ITEM_FIELD_BANDAGE"));
            Assert.IsTrue(resolution.Message.Contains("requirements not met"));
        }

        [Test]
        public void EncounterRuntimeResolver_HandlesHiddenAndDisabledVisibleChoices()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            var encounter = CreateRuntimeEncounter(
                "encounter.policy",
                CreateChoice(
                    "choice.hidden",
                    new[] { CreateRequirement("StatAtLeast", "gold", 6) },
                    new[] { CreateEffect("ModifyMental", 1) },
                    "Hidden"),
                CreateChoice(
                    "choice.disabled",
                    new[] { CreateRequirement("StatAtLeast", "gold", 6) },
                    new[] { CreateEffect("ModifyMental", 2) },
                    "DisabledVisible",
                    "PLACEHOLDER_REASON_NOT_ENOUGH_GOLD"),
                CreateChoice(
                    "choice.free",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateEffect("ModifyMental", 3) }));

            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter);
            var resolution = state.ResolveEncounterChoice("node.policy", encounter, string.Empty);
            var snapshot = state.CreateSnapshot();

            Assert.IsFalse(views[0].Visible);
            Assert.IsTrue(views[1].Visible);
            Assert.IsFalse(views[1].Enabled);
            Assert.AreEqual("PLACEHOLDER_REASON_NOT_ENOUGH_GOLD", views[1].ReasonTextKey);
            Assert.AreEqual("choice.free", resolution.PayloadId);
            Assert.AreEqual(3, snapshot.Mental);
        }

        [Test]
        public void EncounterRuntimeResolver_ShopSpendsGoldAndAddsItem()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            state.ModifyGold(6);
            var encounter = CreateRuntimeEncounter(
                "encounter.shop.runtime",
                CreateChoice(
                    "choice.buy.item",
                    new[] { CreateRequirement("StatAtLeast", "gold", 6) },
                    new[]
                    {
                        CreateEffect("ModifyGold", -6),
                        CreateItemEffect("ITEM_FIELD_BANDAGE", 1)
                    },
                    "DisabledVisible",
                    "PLACEHOLDER_REASON_NOT_ENOUGH_GOLD"));

            var resolution = state.ResolveEncounterChoice("node.shop.runtime", encounter, "choice.buy.item");
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual("choice.buy.item", resolution.PayloadId);
            Assert.AreEqual(0, snapshot.Gold);
            Assert.AreEqual(1, state.GetItemCount("ITEM_FIELD_BANDAGE"));
        }

        [Test]
        public void EncounterRuntimeResolver_UsesCatalogForShopItemAndAbility()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            state.AttachEncounterCatalog(catalog);
            state.ModifyGold(16);
            var encounter = CreateRuntimeEncounter(
                "encounter.shop.catalog",
                CreateChoice(
                    "choice.buy.bundle",
                    new[] { CreateRequirement("StatAtLeast", "gold", 16) },
                    new[]
                    {
                        CreateEffect("ModifyGold", -16),
                        CreateItemEffect("ITEM_FIELD_BANDAGE", 1),
                        CreateAbilityEffect("ABILITY_SCOUT")
                    },
                    "DisabledVisible",
                    "PLACEHOLDER_REASON_NOT_ENOUGH_GOLD"));

            var resolution = state.ResolveEncounterChoice("node.shop.catalog", encounter, "choice.buy.bundle");
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual("choice.buy.bundle", resolution.PayloadId);
            Assert.AreEqual(0, snapshot.Gold);
            Assert.AreEqual(1, state.GetItemCount("ITEM_FIELD_BANDAGE"));
            Assert.IsTrue(state.HasAbilityRef("ABILITY_SCOUT"));
            Assert.AreEqual(1, snapshot.AbilityCount);
        }

        [Test]
        public void EncounterRuntimeResolver_UsesCatalogRewardBundleEntries()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            state.AttachEncounterCatalog(catalog);
            var encounter = CreateRuntimeEncounter(
                "encounter.reward.catalog",
                CreateChoice(
                    "choice.reward",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateRewardBundleEffect("REWARD_CACHE_SMALL") }));

            var resolution = state.ResolveEncounterChoice("node.reward.catalog", encounter, "choice.reward");

            Assert.AreEqual("choice.reward", resolution.PayloadId);
            Assert.IsTrue(state.HasRewardBundleRef("REWARD_CACHE_SMALL"));
            Assert.AreEqual(1, state.GetItemCount("ITEM_FIELD_BANDAGE"));
        }

        [Test]
        public void EncounterRuntimeResolver_MemoryUnlockUpdatesRunStateDeterministically()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var state = new PrototypeRunState("run-001", new GameFlowEventBus());
            state.AttachEncounterCatalog(catalog);
            var encounter = CreateRuntimeEncounter(
                "encounter.memory.catalog",
                CreateChoice(
                    "choice.memory.unlock",
                    new[] { CreateMemoryLockedRequirement("MEM_FRAGMENT_01") },
                    new[] { CreateMemoryUnlockEffect("MEM_FRAGMENT_01") }));

            var first = state.ResolveEncounterChoice("node.memory.catalog", encounter, "choice.memory.unlock");
            var viewsAfterUnlock = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, encounter);
            var second = state.ResolveEncounterChoice("node.memory.catalog", encounter, "choice.memory.unlock");

            Assert.AreEqual("choice.memory.unlock", first.PayloadId);
            Assert.IsTrue(state.HasMemoryFragmentRef("MEM_FRAGMENT_01"));
            Assert.IsFalse(viewsAfterUnlock[0].Visible);
            Assert.IsTrue(second.Message.Contains("requirements not met"));
        }

        [Test]
        public void EncounterRuntimeResolver_StartCombatEntersCombatFlow()
        {
            var bus = new GameFlowEventBus();
            var events = new List<GameFlowEventType>();
            using (bus.Subscribe(flowEvent => events.Add(flowEvent.Type)))
            {
                var state = new PrototypeRunState("run-001", bus);
                var encounter = CreateRuntimeEncounter(
                    "encounter.combat.runtime",
                    CreateChoice(
                        "choice.fight",
                        new EncounterRequirementRuntimeData[0],
                        new[] { CreateCombatEffect("COMBAT_RUNTIME_001", "ENEMY_RUNTIME_001", new[] { CreatePostCombatEffect("ModifyGold", 8) }) }));

                var resolution = state.ResolveEncounterChoice(new DeterministicRunContext("run-001", 1001), "node.combat.runtime", encounter, "choice.fight");
                var snapshot = state.CreateSnapshot();

                Assert.AreEqual("choice.fight", resolution.PayloadId);
                Assert.AreEqual(8, snapshot.Gold);
                Assert.AreEqual(1, snapshot.BattlesWon);
                CollectionAssert.Contains(events, GameFlowEventType.CombatStarted);
                CollectionAssert.Contains(events, GameFlowEventType.CombatCompleted);
            }
        }

        [Test]
        public void EncounterRuntimeResolver_UsesCatalogEnemyForCombatHandoff()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var bus = new GameFlowEventBus();
            var combatPayloads = new List<string>();
            using (bus.Subscribe(flowEvent =>
            {
                if (flowEvent.Type == GameFlowEventType.CombatStarted)
                {
                    combatPayloads.Add(flowEvent.PayloadId);
                }
            }))
            {
                var state = new PrototypeRunState("run-001", bus);
                state.AttachEncounterCatalog(catalog);
                var encounter = CreateRuntimeEncounter(
                    "encounter.combat.catalog",
                    CreateChoice(
                        "choice.fight.catalog",
                        new EncounterRequirementRuntimeData[0],
                        new[] { CreateCombatEffect("COMBAT_CATALOG_001", "ENEMY_EMPTY_ARMOR", new[] { CreatePostCombatEffect("ModifyGold", 4) }) }));

                var resolution = state.ResolveEncounterChoice(new DeterministicRunContext("run-001", 1001), "node.combat.catalog", encounter, "choice.fight.catalog");
                var snapshot = state.CreateSnapshot();

                Assert.AreEqual("choice.fight.catalog", resolution.PayloadId);
                CollectionAssert.Contains(combatPayloads, "ENEMY_EMPTY_ARMOR");
                Assert.AreEqual(4, snapshot.Gold);
            }
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
        public void PrototypeRunState_ForwardsRunEventsToAttachedNpcStateMachine()
        {
            var definition = ScriptableObject.CreateInstance<NpcStateMachineDefinition>();
            SetNpcTransition(definition, NpcStage.S0, "run.completed", NpcStage.S1);
            var bus = new GameFlowEventBus();
            var state = new PrototypeRunState("run-001", bus);
            var machine = new NpcStateMachine(definition, "run-001", bus);
            state.AttachNpcStateMachine(machine);

            state.ResolveRemnant("node.remnant");

            Assert.AreEqual(NpcStage.S1, machine.CurrentStage);
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

        private static EncounterData CreateRuntimeEncounter(string id, params EncounterChoiceRuntimeData[] choices)
        {
            var encounter = ScriptableObject.CreateInstance<EncounterData>();
            var serialized = new SerializedObject(encounter);
            serialized.FindProperty("id").stringValue = id;
            var property = serialized.FindProperty("choices");
            property.arraySize = choices.Length;
            for (var i = 0; i < choices.Length; i++)
            {
                var choice = property.GetArrayElementAtIndex(i);
                choice.FindPropertyRelative("stableId").stringValue = choices[i].stableId;
                choice.FindPropertyRelative("textKey").stringValue = choices[i].textKey;
                choice.FindPropertyRelative("requirementMode").stringValue = choices[i].requirementMode;
                choice.FindPropertyRelative("unavailablePolicyMode").stringValue = choices[i].unavailablePolicyMode;
                choice.FindPropertyRelative("unavailableReasonTextKey").stringValue = choices[i].unavailableReasonTextKey;
                var requirements = choice.FindPropertyRelative("requirements");
                requirements.arraySize = choices[i].requirements.Length;
                for (var r = 0; r < choices[i].requirements.Length; r++)
                {
                    var source = choices[i].requirements[r];
                    var target = requirements.GetArrayElementAtIndex(r);
                    target.FindPropertyRelative("kind").stringValue = source.kind;
                    target.FindPropertyRelative("stat").stringValue = source.stat;
                    target.FindPropertyRelative("value").intValue = source.value;
                    target.FindPropertyRelative("abilityRef").stringValue = source.abilityRef;
                    target.FindPropertyRelative("itemRef").stringValue = source.itemRef;
                    target.FindPropertyRelative("minCount").intValue = source.minCount;
                    target.FindPropertyRelative("memoryFragmentId").stringValue = source.memoryFragmentId;
                    target.FindPropertyRelative("minFloor").intValue = source.minFloor;
                    target.FindPropertyRelative("maxFloor").intValue = source.maxFloor;
                }

                var effects = choice.FindPropertyRelative("effects");
                effects.arraySize = choices[i].effects.Length;
                for (var e = 0; e < choices[i].effects.Length; e++)
                {
                    var source = choices[i].effects[e];
                    var target = effects.GetArrayElementAtIndex(e);
                    target.FindPropertyRelative("kind").stringValue = source.kind;
                    target.FindPropertyRelative("amount").intValue = source.amount;
                    target.FindPropertyRelative("flag").stringValue = source.flag;
                    target.FindPropertyRelative("value").boolValue = source.value;
                    target.FindPropertyRelative("itemRef").stringValue = source.itemRef;
                    target.FindPropertyRelative("count").intValue = source.count;
                    target.FindPropertyRelative("abilityRef").stringValue = source.abilityRef;
                    target.FindPropertyRelative("rewardBundleRef").stringValue = source.rewardBundleRef;
                    target.FindPropertyRelative("memoryFragmentId").stringValue = source.memoryFragmentId;
                    target.FindPropertyRelative("memoryFragmentTextKey").stringValue = source.memoryFragmentTextKey;
                    target.FindPropertyRelative("npcStage").stringValue = source.npcStage;
                    SetCombatHandoff(target.FindPropertyRelative("combatHandoff"), source.combatHandoff);
                }
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            return encounter;
        }

        private static void SetCombatHandoff(SerializedProperty property, EncounterCombatHandoffRuntimeData handoff)
        {
            if (handoff == null)
            {
                property.FindPropertyRelative("stableId").stringValue = string.Empty;
                property.FindPropertyRelative("enemyRefs").arraySize = 0;
                property.FindPropertyRelative("onVictoryEffects").arraySize = 0;
                property.FindPropertyRelative("onDefeatEffects").arraySize = 0;
                return;
            }

            property.FindPropertyRelative("stableId").stringValue = handoff.stableId;
            property.FindPropertyRelative("seedKey").stringValue = handoff.seedKey;
            var enemyRefs = property.FindPropertyRelative("enemyRefs");
            enemyRefs.arraySize = handoff.enemyRefs.Length;
            for (var i = 0; i < handoff.enemyRefs.Length; i++)
            {
                enemyRefs.GetArrayElementAtIndex(i).stringValue = handoff.enemyRefs[i];
            }

            SetPostCombatEffects(property.FindPropertyRelative("onVictoryEffects"), handoff.onVictoryEffects);
            SetPostCombatEffects(property.FindPropertyRelative("onDefeatEffects"), handoff.onDefeatEffects);
        }

        private static void SetPostCombatEffects(SerializedProperty property, EncounterPostCombatEffectRuntimeData[] effects)
        {
            property.arraySize = effects.Length;
            for (var i = 0; i < effects.Length; i++)
            {
                var source = effects[i];
                var target = property.GetArrayElementAtIndex(i);
                target.FindPropertyRelative("kind").stringValue = source.kind;
                target.FindPropertyRelative("amount").intValue = source.amount;
                target.FindPropertyRelative("flag").stringValue = source.flag;
                target.FindPropertyRelative("value").boolValue = source.value;
                target.FindPropertyRelative("itemRef").stringValue = source.itemRef;
                target.FindPropertyRelative("count").intValue = source.count;
                target.FindPropertyRelative("abilityRef").stringValue = source.abilityRef;
                target.FindPropertyRelative("rewardBundleRef").stringValue = source.rewardBundleRef;
            }
        }

        private static EncounterChoiceRuntimeData CreateChoice(
            string stableId,
            EncounterRequirementRuntimeData[] requirements,
            EncounterEffectRuntimeData[] effects,
            string unavailableMode = "Hidden",
            string unavailableReasonTextKey = "")
        {
            return new EncounterChoiceRuntimeData
            {
                stableId = stableId,
                requirementMode = "All",
                requirements = requirements,
                effects = effects,
                unavailablePolicyMode = unavailableMode,
                unavailableReasonTextKey = unavailableReasonTextKey
            };
        }

        private static EncounterRequirementRuntimeData CreateRequirement(string kind, string stat, int value)
        {
            return new EncounterRequirementRuntimeData
            {
                kind = kind,
                stat = stat,
                value = value
            };
        }

        private static EncounterRequirementRuntimeData CreateMemoryLockedRequirement(string memoryFragmentId)
        {
            return new EncounterRequirementRuntimeData
            {
                kind = "MemoryFragmentLocked",
                memoryFragmentId = memoryFragmentId
            };
        }

        private static EncounterEffectRuntimeData CreateEffect(string kind, int amount)
        {
            return new EncounterEffectRuntimeData
            {
                kind = kind,
                amount = amount
            };
        }

        private static EncounterEffectRuntimeData CreateFlagEffect(string flag, bool value)
        {
            return new EncounterEffectRuntimeData
            {
                kind = "SetFlag",
                flag = flag,
                value = value
            };
        }

        private static EncounterEffectRuntimeData CreateItemEffect(string itemRef, int count)
        {
            return new EncounterEffectRuntimeData
            {
                kind = "AddItem",
                itemRef = itemRef,
                count = count
            };
        }

        private static EncounterEffectRuntimeData CreateAbilityEffect(string abilityRef)
        {
            return new EncounterEffectRuntimeData
            {
                kind = "AddAbility",
                abilityRef = abilityRef
            };
        }

        private static EncounterEffectRuntimeData CreateRewardBundleEffect(string rewardBundleRef)
        {
            return new EncounterEffectRuntimeData
            {
                kind = "GrantRewardBundle",
                rewardBundleRef = rewardBundleRef
            };
        }

        private static EncounterEffectRuntimeData CreateMemoryUnlockEffect(string memoryFragmentId)
        {
            return new EncounterEffectRuntimeData
            {
                kind = "UnlockMemoryFragment",
                memoryFragmentId = memoryFragmentId,
                memoryFragmentTextKey = "PLACEHOLDER_" + memoryFragmentId,
                npcStage = "S1_AWARENESS"
            };
        }

        private static EncounterEffectRuntimeData CreateCombatEffect(string combatId, string enemyRef, EncounterPostCombatEffectRuntimeData[] onVictoryEffects)
        {
            return new EncounterEffectRuntimeData
            {
                kind = "StartCombat",
                combatHandoff = new EncounterCombatHandoffRuntimeData
                {
                    stableId = combatId,
                    seedKey = combatId,
                    enemyRefs = new[] { enemyRef },
                    onVictoryEffects = onVictoryEffects,
                    onDefeatEffects = new EncounterPostCombatEffectRuntimeData[0]
                }
            };
        }

        private static EncounterPostCombatEffectRuntimeData CreatePostCombatEffect(string kind, int amount)
        {
            return new EncounterPostCombatEffectRuntimeData
            {
                kind = kind,
                amount = amount
            };
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

        private static void SetNumericParams(ScriptableObject target, params (string key, float value)[] values)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty("numericParams");
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("key").stringValue = values[i].key;
                element.FindPropertyRelative("value").floatValue = values[i].value;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetNpcTransition(NpcStateMachineDefinition definition, NpcStage fromStage, string triggerId, NpcStage toStage)
        {
            var serialized = new SerializedObject(definition);
            serialized.FindProperty("initialStage").enumValueIndex = (int)fromStage;
            var transitions = serialized.FindProperty("transitions");
            transitions.arraySize = 1;
            var transition = transitions.GetArrayElementAtIndex(0);
            transition.FindPropertyRelative("fromStage").enumValueIndex = (int)fromStage;
            transition.FindPropertyRelative("triggerId").stringValue = triggerId;
            transition.FindPropertyRelative("toStage").enumValueIndex = (int)toStage;
            serialized.ApplyModifiedPropertiesWithoutUndo();
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
