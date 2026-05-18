using System.Linq;
using System.Collections.Generic;
using System.IO;
using HwigiTower.Abilities;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.Items;
using HwigiTower.LLM;
using HwigiTower.NPC;
using HwigiTower.Run;
using HwigiTower.UI;
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

            var firstRound = first.ResolveRound(firstPlayer, firstEnemy, CombatAction.Attack);
            var secondRound = second.ResolveRound(secondPlayer, secondEnemy, CombatAction.Attack);

            Assert.AreEqual(firstRound.PlayerDamage, secondRound.PlayerDamage);
            Assert.AreEqual(firstRound.EnemyDamage, secondRound.EnemyDamage);
        }

        [Test]
        public void CombatController_SecondAttackAddsComboDamage()
        {
            var context = new DeterministicRunContext("run-combo", 2002);
            var combat = new CombatController(context, "battle.combo");
            var player = new CombatantState("player", 20, 10);
            var enemy = new CombatantState("enemy", 50, 1);

            var round = combat.ResolveRound(player, enemy, CombatAction.Attack, CombatAction.Attack);

            Assert.Greater(round.ComboDamage, 0);
            Assert.AreEqual(50 - round.PlayerDamage - round.ComboDamage, enemy.Hp);
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
                var state = new PrototypeRunState("run-001", bus) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
        public void CombatAbilityModifiers_UsesItemPassiveNumericParams()
        {
            var attack = CreateItem("item.attack", "player_attack");
            SetNumericParams(attack, ("atk_bonus", 1f), ("flat_damage_bonus", 2f));
            var defend = CreateItem("item.defend", "player_defend");
            SetNumericParams(defend, ("damage_reduce", 3f));
            var firstHit = CreateItem("item.first_hit", "first_hit_per_combat");
            SetNumericParams(firstHit, ("damage_reduce", 4f));
            var start = CreateItem("item.start", "combat_start");
            SetNumericParams(start, ("hp_restore", 5f), ("poison_damage_per_round", 1f));

            var modifiers = CombatAbilityModifiers.From(null, null, new[] { attack, defend, firstHit, start });

            Assert.AreEqual(1, modifiers.PlayerAttackBonus);
            Assert.AreEqual(2, modifiers.FlatDamageBonus);
            Assert.AreEqual(3, modifiers.DefendDamageReduce);
            Assert.AreEqual(4, modifiers.FirstHitDamageReduce);
            Assert.AreEqual(5, modifiers.CombatStartHpRestore);
            Assert.AreEqual(1, modifiers.PoisonDamagePerRound);
        }

        [Test]
        public void CombatLoopContent_RequiredScriptableObjectsExist()
        {
            var requiredPaths = new[]
            {
                "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_BASIC.asset",
                "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_GLITCH.asset",
                "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_ELITE.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_ENEMY_WALKER_01.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_ENEMY_CRAWLER_02.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_ENEMY_SHADE_03.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_ENEMY_WRAITH_04.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_ENEMY_HERALD_05.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_BOSS_GATE_01.asset",
                "Assets/_Project/Data/Enemies/SO_Enemy_BOSS_APEX_02.asset",
                "Assets/_Project/Data/Items/SO_Item_ITEM_01.asset",
                "Assets/_Project/Data/Items/SO_Item_ITEM_12.asset",
                "Assets/_Project/Data/Items/SO_Item_RELIC_SWORD_01.asset",
                "Assets/_Project/Data/Items/SO_Item_RELIC_GENERIC_02.asset"
            };

            foreach (var path in requiredPaths)
            {
                Assert.IsTrue(File.Exists(path), path);
            }
        }

        [Test]
        public void PrototypeRunState_RemnantCompletesRunAndSavesReflection()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };

            var resolution = state.ResolveRemnant("node.remnant");

            Assert.IsTrue(resolution.RunCompleted);
            Assert.IsTrue(state.RunCompleted);
            Assert.IsTrue(state.MemoryRepo.TryGetReflection("run-001", out _));
        }

        [Test]
        public void PrototypeRunState_ClampsCoreRunStateValues()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };

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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };

            state.ModifyGlitchLevel(35);
            state.ModifyAffinity(-20);
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual(35, snapshot.GlitchLevel);
            Assert.AreEqual(-20, snapshot.Affinity);
        }

        [Test]
        public void EncounterRuntimeResolver_AppliesChoiceEffects()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
        public void PrototypeRunState_BlocksResolvedEncounterChoiceRevisit()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.ModifyGold(10);
            var encounter = CreateRuntimeEncounter(
                "encounter.revisit",
                CreateChoice(
                    "choice.once",
                    new EncounterRequirementRuntimeData[0],
                    new[]
                    {
                        CreateEffect("ModifyGold", -3),
                        CreateItemEffect("ITEM_FIELD_BANDAGE", 1)
                    }));

            var first = state.ResolveEncounterChoice("node.revisit", encounter, "choice.once");
            var second = state.ResolveEncounterChoice("node.revisit", encounter, "choice.once");
            var snapshot = state.CreateSnapshot();

            Assert.AreEqual("choice.once", first.PayloadId);
            Assert.AreEqual("choice.once", second.PayloadId);
            Assert.IsTrue(second.Message.Contains("already resolved: choice.once"));
            Assert.IsTrue(state.HasResolvedEncounterChoice("node.revisit", "encounter.revisit"));
            Assert.AreEqual(7, snapshot.Gold);
            Assert.AreEqual(1, state.GetItemCount("ITEM_FIELD_BANDAGE"));
            Assert.AreEqual(1, snapshot.NodesResolved);
        }

        [Test]
        public void EncounterRuntimeResolver_BlocksChoiceWhenRequirementFails()
        {
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            var state = new PrototypeRunState("run-001", new GameFlowEventBus()) { AutoResolveCombat = true };
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
            Assert.IsTrue(second.Message.Contains("already resolved: choice.memory.unlock"));
        }

        [Test]
        public void EncounterRuntimeResolver_StartCombatEntersCombatFlow()
        {
            var bus = new GameFlowEventBus();
            var events = new List<GameFlowEventType>();
            using (bus.Subscribe(flowEvent => events.Add(flowEvent.Type)))
            {
                var state = new PrototypeRunState("run-001", bus) { AutoResolveCombat = true };
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
                var state = new PrototypeRunState("run-001", bus) { AutoResolveCombat = true };
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
        public void DemoProgression_CompletesAfterSelectedMapPathReachesBoss()
        {
            var shop = CreateRuntimeEncounter("ENC_SHOP_DEMO", CreateChoice("CHOICE_SHOP_DEMO", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGold", 5) }));
            var moral = CreateRuntimeEncounter("ENC_MORAL_DEMO", CreateChoice("CHOICE_MORAL_DEMO", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyAffinity", -5), CreateEffect("ModifyGlitchLevel", 3) }));
            var memory = CreateRuntimeEncounter("ENC_MEMORY_DEMO", CreateChoice("CHOICE_MEMORY_DEMO", new EncounterRequirementRuntimeData[0], new[] { CreateMemoryUnlockEffect("MEM_FRAGMENT_01") }));
            var combat = CreateRuntimeEncounter("ENC_COMBAT_DEMO", CreateChoice("CHOICE_COMBAT_DEMO", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_DEMO", "ENEMY_DEMO", new[] { CreatePostCombatEffect("ModifyGold", 1) }) }));
            var shopNode = CreateNode("node.shop.demo", shop);
            var battleNode = CreateNode("node.battle.demo", moral, memory, combat);
            var state = new PrototypeRunState("run-demo", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachDemoRunPath(new[]
            {
                new PrototypeDemoRunStep(shopNode, shop),
                new PrototypeDemoRunStep(battleNode, moral),
                new PrototypeDemoRunStep(battleNode, memory),
                new PrototypeDemoRunStep(battleNode, combat)
            });

            Assert.AreEqual("demo.active", state.DemoStatus);

            var resolved = new List<string>();
            while (!state.RunCompleted)
            {
                var selectable = state.GetSelectableMapNodeViews();
                Assert.Greater(selectable.Length, 0);
                Assert.IsTrue(state.TrySelectMapNode(selectable[0].MapNodeId, out var step));
                resolved.Add(step.EncounterId);
                var choiceId = step.EncounterId switch
                {
                    "ENC_SHOP_DEMO" => "CHOICE_SHOP_DEMO",
                    "ENC_MORAL_DEMO" => "CHOICE_MORAL_DEMO",
                    "ENC_MEMORY_DEMO" => "CHOICE_MEMORY_DEMO",
                    "ENC_COMBAT_DEMO" => "CHOICE_COMBAT_DEMO",
                    _ => step.Encounter.Choices[0].stableId
                };
                state.ResolveEncounterChoice(new DeterministicRunContext("run-demo", 1001), step.NodeId, step.Encounter, choiceId);
            }

            CollectionAssert.Contains(resolved, "ENC_SHOP_DEMO");
            CollectionAssert.Contains(resolved, "ENC_COMBAT_DEMO");
            Assert.IsTrue(state.DemoComplete);
            Assert.AreEqual("run.clear", state.DemoStatus);
        }

        [Test]
        public void DemoProgression_DoesNotAdvanceOrReapplyOnResolvedRevisit()
        {
            var encounter = CreateRuntimeEncounter("ENC_SHOP_DEMO_REVISIT", CreateChoice("CHOICE_SHOP_REVISIT", new EncounterRequirementRuntimeData[0], new[] { CreateItemEffect("ITEM_FIELD_BANDAGE", 1) }));
            var node = CreateNode("node.shop.demo.revisit", encounter);
            var state = new PrototypeRunState("run-demo-revisit", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, encounter) });

            var first = state.ResolveEncounterChoice(new DeterministicRunContext("run-demo-revisit", 1001), node.NodeId, encounter, "CHOICE_SHOP_REVISIT");
            var second = state.ResolveEncounterChoice(new DeterministicRunContext("run-demo-revisit", 1001), node.NodeId, encounter, "CHOICE_SHOP_REVISIT");

            Assert.IsTrue(first.Message.Contains("run.clear"));
            Assert.IsTrue(second.Message.Contains("already resolved: CHOICE_SHOP_REVISIT"));
            Assert.AreEqual(1, state.GetItemCount("ITEM_FIELD_BANDAGE"));
            Assert.AreEqual(1, state.CreateSnapshot().NodesResolved);
        }

        [Test]
        public void FloorProgression_UnlocksStairAndAdvancesToFloorTwo()
        {
            var floorOneEncounter = CreateRuntimeEncounter("ENC_FLOOR_1", CreateChoice("CHOICE_FLOOR_1", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyAffinity", 1) }));
            var floorTwoEncounter = CreateRuntimeEncounter("ENC_FLOOR_2", CreateChoice("CHOICE_FLOOR_2", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGlitchLevel", 1) }));
            var shopNode = CreateNode("node.floor.shop", floorOneEncounter);
            var battleNode = CreateNode("node.floor.battle", floorTwoEncounter);
            var state = new PrototypeRunState("run-floor", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachFloorRunPaths(new[]
            {
                CreateFloorPath(1, new PrototypeDemoRunStep(shopNode, floorOneEncounter)),
                CreateFloorPath(2, new PrototypeDemoRunStep(battleNode, floorTwoEncounter))
            }, null);

            Assert.AreEqual(1, state.CurrentFloor);
            Assert.AreEqual("ENC_FLOOR_1", state.NextDemoEncounterId);

            state.ResolveEncounterChoice(new DeterministicRunContext("run-floor", 1001), shopNode.NodeId, floorOneEncounter, "CHOICE_FLOOR_1");

            Assert.IsTrue(state.StairUnlocked);
            Assert.IsFalse(state.RunCompleted);

            var next = state.ResolveNextFloor();

            Assert.AreEqual("floor.2", next.PayloadId);
            Assert.AreEqual(2, state.CurrentFloor);
            Assert.IsFalse(state.StairUnlocked);
            Assert.AreEqual("ENC_FLOOR_2", state.NextDemoEncounterId);
            Assert.IsTrue(state.MemoryRepo.TryGetReflection("run-floor.floor.1", out _));

            state.ResolveEncounterChoice(new DeterministicRunContext("run-floor", 1001), battleNode.NodeId, floorTwoEncounter, "CHOICE_FLOOR_2");

            Assert.IsTrue(state.RunClear);
            Assert.IsTrue(state.RunCompleted);
        }

        [Test]
        public void FloorTwoBossGate_UnlocksAsFinalCombatStep()
        {
            var floorOneEncounter = CreateRuntimeEncounter("ENC_FLOOR_ONE_CLEAR", CreateChoice("CHOICE_FLOOR_ONE_CLEAR", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyAffinity", 1) }));
            var floorTwoPrep = CreateRuntimeEncounter("ENC_FLOOR_TWO_PREP", CreateChoice("CHOICE_FLOOR_TWO_PREP", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGlitchLevel", 1) }));
            var bossGate = CreateRuntimeEncounter("ENC_FLOOR_TWO_BOSS", CreateChoice("CHOICE_FLOOR_TWO_BOSS", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_FLOOR_TWO_BOSS", "ENEMY_FLOOR_TWO_BOSS", new[] { CreatePostCombatEffect("ModifyGold", 9) }) }));
            var floorOneNode = CreateNode("node.floor.one", floorOneEncounter);
            var floorTwoPrepNode = CreateNode("node.floor.two.prep", floorTwoPrep);
            var bossNode = CreateNode("node.floor.two.boss", bossGate);
            var state = new PrototypeRunState("run-boss-unlock", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachFloorRunPaths(new[]
            {
                CreateFloorPath(1, new PrototypeDemoRunStep(floorOneNode, floorOneEncounter)),
                CreateFloorPath(2, new PrototypeDemoRunStep(floorTwoPrepNode, floorTwoPrep), new PrototypeDemoRunStep(bossNode, bossGate))
            }, null);

            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-unlock", 1001), floorOneNode.NodeId, floorOneEncounter, "CHOICE_FLOOR_ONE_CLEAR");
            state.ResolveNextFloor();
            SelectMapNodeForEncounter(state, "ENC_FLOOR_TWO_PREP");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-unlock", 1001), floorTwoPrepNode.NodeId, floorTwoPrep, "CHOICE_FLOOR_TWO_PREP");

            Assert.AreEqual(2, state.CurrentFloor);
            SelectFirstMapNodeOfType(state, PrototypeFloorMapNodeType.Boss);
            Assert.IsTrue(state.BossGateUnlocked);
            Assert.AreEqual("ENC_FLOOR_TWO_BOSS", state.NextDemoEncounterId);
            Assert.IsTrue(state.CreateSnapshot().BossGateUnlocked);
        }

        [Test]
        public void FloorTwoBossGate_UsesDedicatedBossEnemyFromCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            var boss = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/_Project/Data/Enemies/SO_Enemy_BOSS_GATE_01.asset");
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_02.asset");

            Assert.IsNotNull(catalog);
            Assert.IsNotNull(boss);
            Assert.IsNotNull(encounter);
            Assert.IsTrue(catalog.TryGetEnemy("BOSS_GATE_01", out var catalogBoss));
            Assert.AreSame(boss, catalogBoss);
            Assert.AreEqual("BOSS_GATE_01", encounter.Choices[0].effects[0].combatHandoff.enemyRefs[0]);
            Assert.AreNotEqual("ENEMY_EMPTY_ARMOR", encounter.Choices[0].effects[0].combatHandoff.enemyRefs[0]);
        }

        [Test]
        public void FloorTwoBossGate_BalanceSupportsThreeToSixTurnClear()
        {
            var boss = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/_Project/Data/Enemies/SO_Enemy_BOSS_GATE_01.asset");
            Assert.IsNotNull(boss);
            Assert.AreEqual("BOSS_GATE_01", boss.Id);
            Assert.GreaterOrEqual(boss.Hp, 24);
            Assert.LessOrEqual(boss.Hp, 32);
            Assert.GreaterOrEqual(boss.Attack, 3);
            Assert.LessOrEqual(boss.Attack, 5);
            Assert.GreaterOrEqual(boss.GoldReward, 12);
        }

        [Test]
        public void PrototypeRoomDefinition_FloorOneToFiveRouteOrderIsDeterministic()
        {
            var room = AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            Assert.IsNotNull(room);

            CollectionAssert.AreEqual(new[] { "ENC_SHOP_01", "EVT_F01_JAR_ROOM", "ENC_REST_01", "ENC_MORAL_CHOICE_01", "ENC_MEMORY_FRAGMENT_01", "ENC_COMBAT_GATE_01" }, RouteEncounterIds(room.GetRunPathForFloor(1)));
            CollectionAssert.AreEqual(new[] { "ENC_F02_SHOP_001", "ENC_REST_02", "ENC_F02_MORAL_CHOICE_001", "ENC_COMBAT_GATE_02" }, RouteEncounterIds(room.GetRunPathForFloor(2)));
            CollectionAssert.AreEqual(new[] { "ENC_SHOP_03", "ENC_REST_03", "ENC_MORAL_CHOICE_02", "ENC_MEMORY_FRAGMENT_02", "ENC_COMBAT_GATE_01" }, RouteEncounterIds(room.GetRunPathForFloor(3)));
            CollectionAssert.AreEqual(new[] { "ENC_REST_01", "ENC_MORAL_CHOICE_03", "ENC_SHOP_04", "ENC_MEMORY_FRAGMENT_03", "ENC_COMBAT_GATE_01" }, RouteEncounterIds(room.GetRunPathForFloor(4)));
            CollectionAssert.AreEqual(new[] { "ENC_MEMORY_FRAGMENT_04", "ENC_MEMORY_FRAGMENT_05", "ENC_REST_05", "ENC_SHOP_05", "ENC_COMBAT_GATE_03" }, RouteEncounterIds(room.GetRunPathForFloor(5)));
        }

        [Test]
        public void BranchingFloorMap_IsDeterministicAndConvergesThroughShopToBoss()
        {
            var room = AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            Assert.IsNotNull(room);

            for (var floor = 1; floor <= 5; floor++)
            {
                var first = new PrototypeRunState("run-map-a-" + floor, new GameFlowEventBus()) { AutoResolveCombat = true };
                var second = new PrototypeRunState("run-map-a-" + floor, new GameFlowEventBus()) { AutoResolveCombat = true };
                var different = new PrototypeRunState("run-map-b-" + floor, new GameFlowEventBus()) { AutoResolveCombat = true };
                first.AttachDemoRunPath(room.GetRunPathForFloor(floor));
                second.AttachDemoRunPath(room.GetRunPathForFloor(floor));
                different.AttachDemoRunPath(room.GetRunPathForFloor(floor));

                var nodes = first.CreateSnapshot().FloorMapNodes;
                Assert.IsTrue(first.CreateSnapshot().HasFloorMap);
                Assert.IsTrue(second.CreateSnapshot().HasFloorMap);
                CollectionAssert.AreEqual(
                    BuildMapSignature(nodes),
                    BuildMapSignature(second.CreateSnapshot().FloorMapNodes));
                CollectionAssert.AreNotEqual(
                    BuildMapSignature(nodes),
                    BuildMapSignature(different.CreateSnapshot().FloorMapNodes));
                Assert.GreaterOrEqual(nodes.Count(node => node.Selectable), 1);
                Assert.AreEqual(1, nodes.Count(node => node.Type == PrototypeFloorMapNodeType.Shop));
                Assert.AreEqual(1, nodes.Count(node => node.Type == PrototypeFloorMapNodeType.Boss));
                var shop = nodes.Single(node => node.Type == PrototypeFloorMapNodeType.Shop);
                var boss = nodes.Single(node => node.Type == PrototypeFloorMapNodeType.Boss);
                var lastBranchLayer = nodes.Where(node => node.Layer < shop.Layer).Max(node => node.Layer);
                Assert.AreEqual(4, shop.Layer);
                Assert.AreEqual(5, boss.Layer);
                Assert.Greater(shop.NormalizedY, nodes.Where(node => node.Layer == lastBranchLayer).Max(node => node.NormalizedY));
                Assert.Greater(boss.NormalizedY, shop.NormalizedY);
                Assert.LessOrEqual(nodes.Count(node => node.Type == PrototypeFloorMapNodeType.Rest), 1);
                Assert.IsTrue(nodes.Where(node => node.Layer == lastBranchLayer).All(node => node.NextMapNodeIds.Contains(shop.MapNodeId)));
                CollectionAssert.Contains(shop.NextMapNodeIds, boss.MapNodeId);
                Assert.IsTrue(nodes.Where(node => node.Layer == 1).All(node => node.Selectable));
                Assert.IsTrue(nodes.Where(node => node.Layer > 1).All(node => !node.Selectable));
            }
        }

        [Test]
        public void BranchingFloorMap_OnlyConnectedNodesBecomeSelectable()
        {
            var room = AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            Assert.IsNotNull(room);

            var state = new PrototypeRunState("run-map-connected", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachDemoRunPath(room.GetRunPathForFloor(1));

            var firstSnapshot = state.CreateSnapshot();
            var selected = firstSnapshot.FloorMapNodes.First(node => node.Layer == 1 && node.Selectable);
            Assert.IsTrue(state.TrySelectMapNode(selected.MapNodeId, out var selectedStep));
            Assert.IsNotNull(selectedStep);
            state.ResolveEncounterChoice(new DeterministicRunContext("run-map-connected", 1001), selectedStep.NodeId, selectedStep.Encounter, selectedStep.Encounter.Choices[0].stableId);

            var nextSnapshot = state.CreateSnapshot();
            var selectableIds = nextSnapshot.FloorMapNodes.Where(node => node.Selectable).Select(node => node.MapNodeId).ToArray();
            CollectionAssert.AreEquivalent(selected.NextMapNodeIds, selectableIds);
            Assert.IsTrue(nextSnapshot.FloorMapNodes.Where(node => node.Layer == 1 && node.MapNodeId != selected.MapNodeId).All(node => node.Locked));
        }

        [Test]
        public void RestNode_RestoresHpAndReducesInternalGlitch()
        {
            var rest = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_REST_01.asset");
            var node = AssetDatabase.LoadAssetAtPath<PrototypeNodeDefinition>("Assets/_Project/Data/Prototype/Nodes/SO_Node_Rest.asset");
            var state = new PrototypeRunState("run-rest-map", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.ModifyPlayerHp(-8);
            state.ModifyGlitchLevel(7);

            var hpBefore = state.PlayerHp;
            var glitchBefore = state.GlitchLevel;
            var resolution = state.ResolveEncounterChoice(new DeterministicRunContext("run-rest-map", 1001), node.NodeId, rest, "CHOICE_REST_01_REST");

            Assert.Greater(state.PlayerHp, hpBefore);
            Assert.Less(state.GlitchLevel, glitchBefore);
            Assert.AreEqual("NPC_REACT_REST", state.LastNpcReactionKey);
            StringAssert.Contains("Glitch -3", resolution.Message);
        }

        [Test]
        public void JarRoomEvent_ResolvesDeterministicOutcomesWithoutRuntimeJson()
        {
            var jar = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_EVT_F01_JAR_ROOM.asset");
            var node = AssetDatabase.LoadAssetAtPath<PrototypeNodeDefinition>("Assets/_Project/Data/Prototype/Nodes/SO_Node_Event.asset");
            Assert.IsNotNull(jar);
            Assert.IsNotNull(node);

            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(new PrototypeRunState("run-jar-views", new GameFlowEventBus()), jar);
            Assert.AreEqual(3, views.Length);
            StringAssert.Contains("80%: 골드 획득", views[0].HintText);
            StringAssert.Contains("20%: 엘리트 전투", views[0].HintText);
            StringAssert.Contains("HP 회복", views[1].HintText);
            StringAssert.Contains("다음 3회 전투 피해 증가", views[2].HintText);

            var goldState = new PrototypeRunState("run-jar-gold", new GameFlowEventBus()) { AutoResolveCombat = true };
            goldState.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, jar) });
            var patterned = goldState.ResolveEncounterChoice(new DeterministicRunContext("run-jar-gold", 1001), node.NodeId, jar, "CHOICE_EVT_F01_JAR_PATTERNED");
            Assert.AreEqual("CHOICE_EVT_F01_JAR_PATTERNED", patterned.PayloadId);
            Assert.IsTrue(patterned.Message.Contains("Gold +8") || patterned.Message.Contains("elite combat"));

            var plainState = new PrototypeRunState("run-jar-plain", new GameFlowEventBus()) { AutoResolveCombat = true };
            plainState.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, jar) });
            plainState.ModifyPlayerHp(-8);
            plainState.ModifyMental(-6);
            var hpBefore = plainState.PlayerHp;
            var mentalBefore = plainState.Mental;
            plainState.ResolveEncounterChoice(new DeterministicRunContext("run-jar-plain", 1001), node.NodeId, jar, "CHOICE_EVT_F01_JAR_PLAIN");
            Assert.Greater(plainState.PlayerHp, hpBefore);
            Assert.Greater(plainState.Mental, mentalBefore);

            var buffState = new PrototypeRunState("run-jar-buff", new GameFlowEventBus()) { AutoResolveCombat = true };
            buffState.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, jar) });
            buffState.ResolveEncounterChoice(new DeterministicRunContext("run-jar-buff", 1001), node.NodeId, jar, "CHOICE_EVT_F01_JAR_CRACKED");
            Assert.IsTrue(buffState.HasFlag("FLAG_CRACKED_JAR_DAMAGE_BUFF"));
        }

        [Test]
        public void PresentationData_HasFloorMerchantHooks()
        {
            var presentation = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            Assert.IsNotNull(presentation);
            Assert.AreEqual(5, presentation.MerchantSlots.Count(slot => slot.Floor >= 1 && slot.Floor <= 5));
            Assert.IsTrue(presentation.MerchantSlots.All(slot => !string.IsNullOrEmpty(slot.StateKey)));
            Assert.IsTrue(presentation.TryGetMerchantSprite(1, out var floorOneMerchant));
            Assert.IsTrue(presentation.TryGetMerchantSprite(2, out var floorTwoMerchant));
            Assert.IsTrue(presentation.TryGetMerchantSprite(3, out var floorThreeMerchant));
            Assert.IsTrue(presentation.TryGetMerchantSprite(4, out var floorFourMerchant));
            Assert.IsTrue(presentation.TryGetMerchantSprite(5, out var floorFiveMerchant));
            Assert.AreEqual("merchant_human", floorOneMerchant.name);
            Assert.AreEqual("merchant_human", floorTwoMerchant.name);
            Assert.AreEqual("merchant_human", floorThreeMerchant.name);
            Assert.AreEqual("merchant_otherworld", floorFourMerchant.name);
            Assert.AreEqual("merchant_otherworld", floorFiveMerchant.name);
        }

        [Test]
        public void PrototypeHud_NpcSpotlightUsesPublicModeLabels()
        {
            var hudObject = new GameObject("hud-spotlight-test");
            var texture = new Texture2D(8, 8);
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, 8f, 8f), new Vector2(0.5f, 0.5f));
            sprite.name = "spotlight_test";
            try
            {
                var hud = hudObject.AddComponent<PrototypeHud>();
                var modeType = typeof(PrototypeHud).GetNestedType("NpcSpotlightMode", System.Reflection.BindingFlags.NonPublic);
                var method = typeof(PrototypeHud).GetMethod("ShowNpcSpotlight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                Assert.IsNotNull(modeType);
                Assert.IsNotNull(method);

                method.Invoke(hud, new object[] { sprite, "상인", "임시 안내", System.Enum.Parse(modeType, "Shop") });
                Assert.IsTrue(hud.NpcSpotlightVisible);
                Assert.AreEqual("상점", hud.CurrentNpcSpotlightModeLabel);
                Assert.AreEqual(sprite.name, hud.CurrentNpcSpotlightSpriteName);
                StringAssert.Contains("상인", hud.NpcSpotlightMessage);
            }
            finally
            {
                Object.DestroyImmediate(sprite);
                Object.DestroyImmediate(texture);
                Object.DestroyImmediate(hudObject);
            }
        }

        [Test]
        public void FloorTwoBossGateOverride_DoesNotLeakIntoFloorThreeOrFinalBoss()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var room = AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            var floorTwoBossGate = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_02.asset");
            var finalBossGate = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_03.asset");

            Assert.IsNotNull(catalog);
            Assert.IsNotNull(room);
            Assert.IsNotNull(floorTwoBossGate);
            Assert.IsNotNull(finalBossGate);
            Assert.IsTrue(catalog.TryGetEnemy("BOSS_GATE_01", out _));
            Assert.IsTrue(catalog.TryGetEnemy("BOSS_APEX_02", out _));
            Assert.AreEqual("BOSS_GATE_01", floorTwoBossGate.Choices[0].effects[0].combatHandoff.enemyRefs[0]);
            Assert.AreEqual("BOSS_APEX_02", finalBossGate.Choices[0].effects[0].combatHandoff.enemyRefs[0]);
            Assert.AreNotEqual("BOSS_GATE_01", finalBossGate.Choices[0].effects[0].combatHandoff.enemyRefs[0]);
            Assert.IsFalse(RouteEncounterIds(room.GetRunPathForFloor(3)).Contains("ENC_COMBAT_GATE_02"));
        }

        [Test]
        public void EnemyCatalogV01_ContainsDocumentedStableIdsAndFloorPools()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            Assert.IsNotNull(catalog);
            Assert.IsNotNull(catalog.FloorEnemyPools);

            var expectedIds = new[]
            {
                "ENEMY_BANDIT_MELEE_01",
                "ENEMY_BANDIT_RANGED_01",
                "ENEMY_SLIME_01",
                "ENEMY_SKELETON_01",
                "ENEMY_WILD_BEAST_01",
                "ENEMY_SLIME_POOL_01",
                "ENEMY_WILD_BEAST_PACK_01",
                "ENEMY_STATUE_01",
                "ENEMY_FRACTURE_HOUND",
                "ENEMY_MANEATER_PLANT_01",
                "ENEMY_MERCENARY_MELEE_01",
                "ENEMY_MERCENARY_RANGED_01",
                "ENEMY_IRON_MAIDEN_01",
                "BOSS_GATE_01",
                "ENEMY_MERCENARY_ASSASSIN_01",
                "ENEMY_MERCENARY_MAGE_01",
                "ENEMY_EMPTY_ARMOR",
                "ENEMY_MERCENARY_CAPTAIN_SAGAN_01",
                "ENEMY_COLLAPSE_ECHO",
                "ENEMY_SKELETON_HORDE_01",
                "ENEMY_LIVING_TOMBSTONE_01",
                "ENEMY_LIVING_ARMOR_LIGHT_01",
                "ENEMY_SHADE_03",
                "ENEMY_WRAITH_04",
                "ENEMY_MANEATER_JUNGLE_01",
                "ENEMY_HOMUNCULUS_01",
                "ENEMY_LAMPLIGHTER_01",
                "BOSS_APEX_02"
            };

            foreach (var id in expectedIds)
            {
                Assert.IsTrue(catalog.TryGetEnemy(id, out _), id);
            }

            for (var floor = 1; floor <= 5; floor++)
            {
                Assert.IsTrue(catalog.FloorEnemyPools.TryGetPool(floor, out var pool), "floor " + floor);
                Assert.Greater(pool.NormalEnemyRefs.Length, 0, "normal floor " + floor);
                Assert.Greater(pool.EliteEnemyRefs.Length, 0, "elite floor " + floor);
                Assert.Greater(pool.BossEnemyRefs.Length, 0, "boss floor " + floor);
                AssertPoolRefsResolve(catalog, pool.NormalEnemyRefs);
                AssertPoolRefsResolve(catalog, pool.EliteEnemyRefs);
                AssertPoolRefsResolve(catalog, pool.BossEnemyRefs);
            }

            Assert.IsTrue(catalog.FloorEnemyPools.TryGetPool(5, out var floorFivePool));
            CollectionAssert.Contains(floorFivePool.EliteEnemyRefs, "ENEMY_LAMPLIGHTER_01");
            Assert.IsTrue(catalog.TryGetEnemy("BOSS_APEX_02", out var finalBoss));
            Assert.AreEqual("BOSS_APEX_02", finalBoss.Id);
        }

        [Test]
        public void CombatHandoff_UsesSelectedFloorEnemyPoolByNodeType()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>("Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset");
            var normal = CreateRuntimeEncounter("ENC_POOL_NORMAL", CreateChoice("CHOICE_POOL_NORMAL", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_POOL_NORMAL", "ENEMY_EMPTY_ARMOR", new EncounterPostCombatEffectRuntimeData[0]) }));
            var shop = CreateRuntimeEncounter("ENC_POOL_SHOP", CreateChoice("CHOICE_POOL_SHOP", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGold", 0) }));
            var boss = CreateRuntimeEncounter("ENC_POOL_BOSS", CreateChoice("CHOICE_POOL_BOSS", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_POOL_BOSS", "ENEMY_EMPTY_ARMOR", new EncounterPostCombatEffectRuntimeData[0]) }));
            var battleNode = CreateNode("node.pool.battle", normal);
            var shopNode = CreateNode("node.pool.shop", shop);
            var bossNode = CreateNode("node.pool.boss", boss);
            var state = new PrototypeRunState("run-pool-combat", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.AttachEncounterCatalog(catalog);
            state.AttachFloorRunPaths(new[] { CreateFloorPath(1, new PrototypeDemoRunStep(battleNode, normal), new PrototypeDemoRunStep(shopNode, shop), new PrototypeDemoRunStep(bossNode, boss)) }, null);

            SelectMapNodeForEncounter(state, "ENC_POOL_NORMAL");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-pool-combat", 1001), battleNode.NodeId, normal, "CHOICE_POOL_NORMAL");
            CollectionAssert.Contains(new[] { "ENEMY_BANDIT_MELEE_01", "ENEMY_BANDIT_RANGED_01", "ENEMY_SLIME_01", "ENEMY_SKELETON_01", "ENEMY_WILD_BEAST_01" }, state.LastCombatEnemyId);
        }

        [Test]
        public void FinalBoss_BossApexBalanceSupportsVerticalSliceClear()
        {
            var boss = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/_Project/Data/Enemies/SO_Enemy_BOSS_APEX_02.asset");
            Assert.IsNotNull(boss);
            Assert.AreEqual("BOSS_APEX_02", boss.Id);
            Assert.GreaterOrEqual(boss.Hp, 28);
            Assert.LessOrEqual(boss.Hp, 36);
            Assert.GreaterOrEqual(boss.Attack, 4);
            Assert.LessOrEqual(boss.Attack, 5);
            Assert.GreaterOrEqual(boss.GoldReward, 24);
            Assert.LessOrEqual(boss.GoldReward, 40);
        }

        [Test]
        public void CombatSkill_RequiresScoutAndCreatesReadableCombo()
        {
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_03.asset");
            var node = CreateNode("node.skill.readable", encounter);
            var withoutScout = new PrototypeRunState("run-skill-without-scout", new GameFlowEventBus()) { AutoResolveCombat = false };
            withoutScout.AttachEncounterCatalog(EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog);
            withoutScout.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, encounter) });
            withoutScout.ResolveEncounterChoice(new DeterministicRunContext("run-skill-without-scout", 1001), node.NodeId, encounter, "CHOICE_COMBAT_03_ENGAGE");

            var blocked = withoutScout.ResolveCombatRoundInteractive(CombatAction.Skill);

            Assert.AreEqual(0, blocked.PlayerDamage);
            Assert.AreEqual(0, blocked.ComboDamage);
            StringAssert.Contains("skill unavailable", withoutScout.CreateSnapshot().LastCombatRoundResult);

            var withScout = new PrototypeRunState("run-skill-with-scout", new GameFlowEventBus()) { AutoResolveCombat = false };
            withScout.AttachEncounterCatalog(EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog);
            withScout.AddAbilityRef("ABILITY_SCOUT");
            withScout.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, encounter) });
            withScout.ResolveEncounterChoice(new DeterministicRunContext("run-skill-with-scout", 1001), node.NodeId, encounter, "CHOICE_COMBAT_03_ENGAGE");

            var skill = withScout.ResolveCombatRoundInteractive(CombatAction.Skill);

            Assert.Greater(skill.PlayerDamage, 0);
            Assert.Greater(skill.ComboDamage, 0);
            StringAssert.Contains("scout skill", withScout.CreateSnapshot().LastCombatRoundResult);
        }

        [Test]
        public void FinalBoss_PreparedPlayerWinsInThreeToSixMeaningfulTurns()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_03.asset");
            var node = CreateNode("node.final.boss.balance", encounter);
            var state = new PrototypeRunState("run-final-boss-balance", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.AttachEncounterCatalog(catalog);
            state.AddItemRef("ITEM_FIELD_BANDAGE", 2);
            state.AddAbilityRef("ABILITY_SCOUT");
            state.AddAbilityRef("ABILITY_RECALL_ANCHOR");
            state.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, encounter) });
            state.ModifyPlayerHp(-8);

            state.ResolveEncounterChoice(new DeterministicRunContext("run-final-boss-balance", 1001), node.NodeId, encounter, "CHOICE_COMBAT_03_ENGAGE");
            StringAssert.Contains("bandage", state.CreateSnapshot().LastCombatRoundResult);
            StringAssert.Contains("scout", state.CreateSnapshot().LastCombatRoundResult);
            StringAssert.Contains("recall ready", state.CreateSnapshot().LastCombatRoundResult);

            var turns = 0;
            while (state.IsInCombat && turns < 6)
            {
                state.ResolveCombatRoundInteractive(turns == 0 ? CombatAction.Skill : CombatAction.Attack);
                turns++;
            }

            Assert.IsFalse(state.IsInCombat);
            Assert.IsTrue(state.RunClear);
            Assert.IsTrue(state.EndingChoicePending);
            Assert.GreaterOrEqual(turns, 3);
            Assert.LessOrEqual(turns, 6);
        }

        [Test]
        public void FinalBossVictory_OpensEndingChoiceOnce()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var room = AssetDatabase.LoadAssetAtPath<PrototypeRoomDefinition>("Assets/_Project/Data/Prototype/Rooms/SO_Room_Prototype.asset");
            var state = CreateConfiguredRouteState("run-final-boss-clear", room, catalog);
            state.ModifyGold(100);
            state.AddItemRef("ITEM_FIELD_BANDAGE", 8);
            state.AddAbilityRef("ABILITY_SCOUT");

            ResolveFullRouteToFinalBoss(state);

            Assert.IsTrue(state.RunClear);
            Assert.IsTrue(state.EndingChoicePending);
            Assert.AreEqual("BOSS_APEX_02", state.LastCombatEnemyId);
            Assert.AreEqual("NPC_REACT_RUN_CLEAR", state.LastNpcReactionKey);

            var rest = state.ResolveEndingChoice("PLACEHOLDER_ENDING_REST");
            var duplicate = state.ResolveEndingChoice("PLACEHOLDER_ENDING_CONTINUE");

            Assert.AreEqual("PLACEHOLDER_ENDING_REST", rest.PayloadId);
            Assert.AreEqual("PLACEHOLDER_ENDING_REST", duplicate.PayloadId);
            Assert.AreEqual("ending.rest", state.RunStatus);
            Assert.AreEqual("NPC_REACT_ENDING_REST", state.LastNpcReactionKey);
        }

        [Test]
        public void FinalBossDefeat_WithRecallAnchorRevivesOnceThenCanFail()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var finalBossGate = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_03.asset");
            var bossNode = CreateNode("node.final.boss.recall", finalBossGate);
            var state = new PrototypeRunState("run-final-boss-recall", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.AttachEncounterCatalog(catalog);
            state.AddAbilityRef("ABILITY_RECALL_ANCHOR");
            state.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(bossNode, finalBossGate) });
            state.ModifyPlayerHp(-23);

            state.ResolveEncounterChoice(new DeterministicRunContext("run-final-boss-recall", 1001), bossNode.NodeId, finalBossGate, "CHOICE_COMBAT_03_ENGAGE");
            state.ResolveCombatRoundInteractive(CombatAction.Attack);

            Assert.IsTrue(state.IsInCombat);
            Assert.IsFalse(state.RunCompleted);
            Assert.AreEqual("recall", state.LastCombatResultId);
            Assert.IsTrue(state.HasFlag("FLAG_RECALL_ANCHOR_USED"));
            Assert.AreEqual("NPC_REACT_RECALL_ANCHOR", state.LastNpcReactionKey);

            var guard = 0;
            while (state.IsInCombat && guard < 4)
            {
                state.ResolveCombatRoundInteractive(CombatAction.Attack);
                guard++;
            }

            Assert.IsTrue(state.RunFailed);
            Assert.AreEqual("run.failed", state.RunStatus);
            Assert.AreEqual("NPC_REACT_RUN_FAILED", state.LastNpcReactionKey);
        }

        [Test]
        public void RunAndEndingStates_HaveSafeFallbackReactionKeys()
        {
            var clearState = CreateClearedRun("run-reaction-clear");
            Assert.AreEqual("NPC_REACT_RUN_CLEAR", clearState.LastNpcReactionKey);

            var rest = clearState.ResolveEndingChoice("PLACEHOLDER_ENDING_REST");
            Assert.AreEqual("PLACEHOLDER_ENDING_REST", rest.PayloadId);
            Assert.AreEqual("NPC_REACT_ENDING_REST", clearState.LastNpcReactionKey);

            var continueState = CreateClearedRun("run-reaction-continue");
            continueState.ResolveEndingChoice("PLACEHOLDER_ENDING_CONTINUE");
            Assert.AreEqual("NPC_REACT_ENDING_CONTINUE", continueState.LastNpcReactionKey);

            var failedState = new PrototypeRunState("run-reaction-failed", new GameFlowEventBus()) { AutoResolveCombat = false };
            failedState.ModifyPlayerHp(-23);
            var lethal = CreateRuntimeEncounter(
                "ENC_REACTION_FAILURE",
                CreateChoice("CHOICE_REACTION_FAILURE", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_REACTION_FAILURE", "ENEMY_REACTION_FAILURE", new EncounterPostCombatEffectRuntimeData[0]) }));
            failedState.ResolveEncounterChoice(new DeterministicRunContext("run-reaction-failed", 1001), "node.reaction.failure", lethal, "CHOICE_REACTION_FAILURE");
            failedState.ResolveCombatRoundInteractive(CombatAction.Attack);
            Assert.AreEqual("NPC_REACT_RUN_FAILED", failedState.LastNpcReactionKey);
        }

        [Test]
        public void SpineCutsceneScaffold_FoldersAndReadmesExistWithoutExports()
        {
            var cutsceneIds = new[]
            {
                "CUT_MEMORY_03_FRACTURE",
                "CUT_FINAL_BOSS_REVEAL",
                "CUT_ENDING_CHOICE",
                "CUT_ENDING_REST",
                "CUT_ENDING_CONTINUE"
            };

            Assert.IsTrue(File.Exists("Assets/_Project/Art/SpineSource/_README.md"));
            Assert.IsTrue(File.Exists("Assets/_Project/Spine/_README.md"));
            Assert.IsTrue(File.Exists("Assets/_Project/Data/Cutscenes/Spine/_README.md"));

            var sourceReadme = File.ReadAllText("Assets/_Project/Art/SpineSource/_README.md");
            StringAssert.Contains("2000x2000", sourceReadme);
            StringAssert.Contains("spine_<cutscene_id>_source_2000.png", sourceReadme);

            var exportReadme = File.ReadAllText("Assets/_Project/Spine/_README.md");
            StringAssert.Contains("spine_<cutscene_id>.json", exportReadme);
            StringAssert.Contains("spine_<cutscene_id>.atlas.txt", exportReadme);

            var dataReadme = File.ReadAllText("Assets/_Project/Data/Cutscenes/Spine/_README.md");
            StringAssert.Contains("SO_CutsceneSpine_<CUTSCENE_ID>.asset", dataReadme);
            StringAssert.Contains("fallbackSprite", dataReadme);

            for (var i = 0; i < cutsceneIds.Length; i++)
            {
                Assert.IsTrue(Directory.Exists("Assets/_Project/Art/SpineSource/" + cutsceneIds[i]));
                Assert.IsTrue(Directory.Exists("Assets/_Project/Spine/" + cutsceneIds[i]));
            }
        }

        [Test]
        public void CutsceneData_SpineSlotsAreDependencyFreeAndFallbackSafe()
        {
            var data = ScriptableObject.CreateInstance<CutsceneData>();
            try
            {
                var serialized = new SerializedObject(data);
                Assert.IsNotNull(serialized.FindProperty("animationCutsceneId"));
                Assert.IsNotNull(serialized.FindProperty("animationAssetPath"));
                Assert.IsNotNull(serialized.FindProperty("fallbackSprite"));
                Assert.IsFalse(data.HasPlayableContent);

                var texture = new Texture2D(2, 2);
                var sprite = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), Vector2.one * 0.5f);
                serialized.FindProperty("fallbackSprite").objectReferenceValue = sprite;
                serialized.ApplyModifiedPropertiesWithoutUndo();

                Assert.IsFalse(data.HasSteps);
                Assert.IsTrue(data.HasPlayableContent);
                Assert.AreSame(sprite, data.FallbackSprite);

                var cutsceneDataSource = File.ReadAllText("Assets/_Project/Scripts/UI/CutsceneData.cs");
                var playerSource = File.ReadAllText("Assets/_Project/Scripts/UI/PrototypeCutscenePlayer.cs");
                Assert.IsFalse(cutsceneDataSource.Contains("using Spine"));
                Assert.IsFalse(cutsceneDataSource.Contains("Spine."));
                Assert.IsFalse(playerSource.Contains("using Spine"));
                Assert.IsFalse(playerSource.Contains("Spine."));
            }
            finally
            {
                Object.DestroyImmediate(data);
            }
        }

        [Test]
        public void SpineCutscenePlaceholderBindings_UseFallbackSpritesAndAnimationPaths()
        {
            AssertSpineCutsceneBinding("CUT_MEMORY_03_FRACTURE", "spine_cut_memory_03_fracture.json");
            AssertSpineCutsceneBinding("CUT_FINAL_BOSS_REVEAL", "spine_cut_final_boss_reveal.json");
            AssertSpineCutsceneBinding("CUT_ENDING_CHOICE", "spine_cut_ending_choice.json");

            var presentation = AssetDatabase.LoadAssetAtPath<DemoPresentationData>("Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset");
            Assert.IsNotNull(presentation);
            Assert.IsTrue(presentation.TryGetCutscene("ENC_MEMORY_FRAGMENT_03", PrototypeCutsceneTrigger.MemoryFragmentUnlock, out var memoryCutscene));
            Assert.AreEqual("CUT_MEMORY_03_FRACTURE", memoryCutscene.CutsceneId);
            Assert.IsTrue(presentation.TryGetCutscene("ENC_COMBAT_GATE_03", PrototypeCutsceneTrigger.CombatGateStart, out var finalBossCutscene));
            Assert.AreEqual("CUT_FINAL_BOSS_REVEAL", finalBossCutscene.CutsceneId);
            Assert.IsTrue(presentation.TryGetCutscene("run.clear", PrototypeCutsceneTrigger.DemoComplete, out var endingCutscene));
            Assert.AreEqual("CUT_ENDING_CHOICE", endingCutscene.CutsceneId);
        }

        private static void AssertSpineCutsceneBinding(string cutsceneId, string exportName)
        {
            var path = "Assets/_Project/Data/Cutscenes/Spine/SO_CutsceneSpine_" + cutsceneId + ".asset";
            var data = AssetDatabase.LoadAssetAtPath<CutsceneData>(path);

            Assert.IsNotNull(data, path);
            Assert.AreEqual(cutsceneId, data.CutsceneId);
            Assert.AreEqual(cutsceneId, data.AnimationCutsceneId);
            StringAssert.Contains("Assets/_Project/Spine/" + cutsceneId + "/", data.AnimationAssetPath);
            StringAssert.Contains(exportName, data.AnimationAssetPath);
            Assert.IsNotNull(data.FallbackSprite);
            Assert.IsTrue(data.HasPlayableContent);
            Assert.IsTrue(data.HasSteps);
        }

        [Test]
        public void FloorTwoBossGate_VictoryClearsRunAndSavesReflection()
        {
            var floorOneEncounter = CreateRuntimeEncounter("ENC_FLOOR_ONE_CLEAR_BOSS", CreateChoice("CHOICE_FLOOR_ONE_CLEAR_BOSS", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyAffinity", 1) }));
            var floorTwoPrep = CreateRuntimeEncounter("ENC_FLOOR_TWO_PREP_BOSS", CreateChoice("CHOICE_FLOOR_TWO_PREP_BOSS", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGlitchLevel", 1) }));
            var bossGate = CreateRuntimeEncounter("ENC_FLOOR_TWO_BOSS_CLEAR", CreateChoice("CHOICE_FLOOR_TWO_BOSS_CLEAR", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_FLOOR_TWO_BOSS_CLEAR", "ENEMY_FLOOR_TWO_BOSS_CLEAR", new[] { CreatePostCombatEffect("ModifyGold", 9) }) }));
            var floorOneNode = CreateNode("node.floor.one.clear", floorOneEncounter);
            var floorTwoPrepNode = CreateNode("node.floor.two.prep.clear", floorTwoPrep);
            var bossNode = CreateNode("node.floor.two.boss.clear", bossGate);
            var state = new PrototypeRunState("run-boss-clear", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachFloorRunPaths(new[]
            {
                CreateFloorPath(1, new PrototypeDemoRunStep(floorOneNode, floorOneEncounter)),
                CreateFloorPath(2, new PrototypeDemoRunStep(floorTwoPrepNode, floorTwoPrep), new PrototypeDemoRunStep(bossNode, bossGate))
            }, null);

            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-clear", 1001), floorOneNode.NodeId, floorOneEncounter, "CHOICE_FLOOR_ONE_CLEAR_BOSS");
            state.ResolveNextFloor();
            SelectMapNodeForEncounter(state, "ENC_FLOOR_TWO_PREP_BOSS");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-clear", 1001), floorTwoPrepNode.NodeId, floorTwoPrep, "CHOICE_FLOOR_TWO_PREP_BOSS");
            SelectMapNodeForEncounter(state, "ENC_FLOOR_TWO_BOSS_CLEAR");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-clear", 1001), bossNode.NodeId, bossGate, "CHOICE_FLOOR_TWO_BOSS_CLEAR");

            Assert.IsTrue(state.RunClear);
            Assert.IsTrue(state.RunCompleted);
            Assert.IsFalse(state.RestartReady);
            Assert.IsTrue(state.EndingChoicePending);
            Assert.AreEqual("run.clear", state.RunStatus);
            Assert.IsTrue(state.MemoryRepo.TryGetReflection("run-boss-clear", out _));
        }

        [Test]
        public void EndingChoice_RestLocksFinalStateAndBlocksFurtherInput()
        {
            var state = CreateClearedRun("run-ending-rest");
            Assert.IsTrue(state.EndingChoicePending);

            var ending = state.ResolveEndingChoice("PLACEHOLDER_ENDING_REST");
            var blocked = state.ResolveEndingChoice("PLACEHOLDER_ENDING_CONTINUE");
            var goldBefore = state.Gold;
            var afterEnd = state.ResolveEncounterChoice(
                new DeterministicRunContext("run-ending-rest", 1001),
                "node.after.ending",
                CreateRuntimeEncounter("ENC_AFTER_ENDING", CreateChoice("CHOICE_AFTER_ENDING", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGold", 99) })),
                "CHOICE_AFTER_ENDING");

            Assert.AreEqual("PLACEHOLDER_ENDING_REST", ending.PayloadId);
            Assert.AreEqual("PLACEHOLDER_ENDING_REST", blocked.PayloadId);
            Assert.IsTrue(state.EndingRest);
            Assert.IsFalse(state.EndingContinue);
            Assert.IsFalse(state.RestartReady);
            Assert.AreEqual("ending.rest", state.RunStatus);
            Assert.AreEqual(goldBefore, state.Gold);
            StringAssert.Contains("run already completed", afterEnd.Message);
        }

        [Test]
        public void EndingChoice_ContinueEnablesRestartAndPreservesMemoryReflectionCache()
        {
            var controllerObject = new GameObject("PrototypeRoomController Ending Continue Test");
            try
            {
                var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
                var controller = controllerObject.AddComponent<PrototypeRoomController>();
                controller.Configure(null, catalog);
                controller.BeginRun();
                var firstRunId = controller.RunState.RunId;
                var request = new LLMRequest(firstRunId, "ending continue cache prompt", "ending.policy");
                controller.RunState.MemoryRepo.SaveCachedResponse(new LLMResponse(request.CacheKey, "cached-ending"));
                Assert.IsTrue(controller.RunState.UnlockMemoryFragmentRef("MEM_FRAGMENT_01"));
                var clearEncounter = CreateRuntimeEncounter(
                    "ENC_ENDING_CONTINUE_CLEAR",
                    CreateChoice("CHOICE_ENDING_CONTINUE_CLEAR", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGold", 0) }));
                var clearNode = CreateNode("node.ending.continue.clear", clearEncounter);
                controller.RunState.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(clearNode, clearEncounter) });
                controller.RunState.ResolveEncounterChoice(new DeterministicRunContext(firstRunId, 1001), clearNode.NodeId, clearEncounter, "CHOICE_ENDING_CONTINUE_CLEAR");
                controller.RunState.ResolveEndingChoice("PLACEHOLDER_ENDING_CONTINUE");

                Assert.IsTrue(controller.RunState.EndingContinue);
                Assert.IsTrue(controller.RunState.RestartReady);
                Assert.AreEqual("ending.continue", controller.RunState.RunStatus);

                controller.RestartRun();

                Assert.AreEqual(firstRunId + ".restart.1", controller.RunState.RunId);
                Assert.AreEqual(1, controller.RunState.CurrentFloor);
                Assert.AreEqual(6, controller.RunState.Gold);
                Assert.IsFalse(controller.RunState.RunCompleted);
                Assert.IsTrue(controller.RunState.HasMemoryFragmentRef("MEM_FRAGMENT_01"));
                Assert.IsTrue(controller.RunState.MemoryRepo.TryGetReflection(firstRunId, out _));
                Assert.IsTrue(controller.RunState.MemoryRepo.TryGetCachedResponse(request.CacheKey, out var cached));
                Assert.AreEqual("cached-ending", cached.Text);
                var fallbackRequest = new LLMRequest(controller.RunState.RunId, "ending continue fallback prompt", "ending.policy");
                Assert.IsTrue(controller.RunState.LLMProvider.TryComplete(fallbackRequest, out var fallbackResponse));
                Assert.AreEqual(fallbackRequest.CacheKey, fallbackResponse.CacheKey);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
            }
        }

        [Test]
        public void FloorTwoBossGate_ClearRewardDoesNotApplyOnRevisit()
        {
            var bossGate = CreateRuntimeEncounter("ENC_FLOOR_TWO_BOSS_REVISIT", CreateChoice("CHOICE_FLOOR_TWO_BOSS_REVISIT", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_FLOOR_TWO_BOSS_REVISIT", "BOSS_GATE_01", new[] { CreatePostCombatEffect("ModifyGold", 16) }) }));
            var bossNode = CreateNode("node.floor.two.boss.revisit", bossGate);
            var state = new PrototypeRunState("run-boss-revisit", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(bossNode, bossGate) });

            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-revisit", 1001), bossNode.NodeId, bossGate, "CHOICE_FLOOR_TWO_BOSS_REVISIT");
            var goldAfterClear = state.Gold;
            var revisit = state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-revisit", 1001), bossNode.NodeId, bossGate, "CHOICE_FLOOR_TWO_BOSS_REVISIT");

            Assert.IsTrue(state.RunClear);
            Assert.AreEqual(16, goldAfterClear);
            Assert.AreEqual(goldAfterClear, state.Gold);
            StringAssert.Contains("already resolved: CHOICE_FLOOR_TWO_BOSS_REVISIT", revisit.Message);
        }

        [Test]
        public void FloorTwoShop_RefsResolveThroughCatalogAndGrantExpectedPurchases()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var encounter = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_SHOP_001.asset");
            Assert.IsNotNull(catalog);
            Assert.IsNotNull(encounter);
            Assert.IsTrue(catalog.TryGetItem("ITEM_FIELD_BANDAGE", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_RECALL_ANCHOR", out _));
            Assert.AreEqual("ITEM_FIELD_BANDAGE", encounter.Choices[0].effects[1].itemRef);
            Assert.AreEqual("ABILITY_RECALL_ANCHOR", encounter.Choices[1].effects[1].abilityRef);

            var itemState = new PrototypeRunState("run-f2-shop-item", new GameFlowEventBus());
            itemState.AttachEncounterCatalog(catalog);
            itemState.ModifyGold(6);
            var itemResult = itemState.ResolveEncounterChoice(new DeterministicRunContext("run-f2-shop-item", 1001), "node.f2.shop.item", encounter, "CHOICE_F02_SHOP_BUY_ITEM");
            Assert.AreEqual("CHOICE_F02_SHOP_BUY_ITEM", itemResult.PayloadId);
            Assert.AreEqual(0, itemState.Gold);
            Assert.AreEqual(1, itemState.GetItemCount("ITEM_FIELD_BANDAGE"));

            var abilityState = new PrototypeRunState("run-f2-shop-ability", new GameFlowEventBus());
            abilityState.AttachEncounterCatalog(catalog);
            abilityState.ModifyGold(12);
            var abilityResult = abilityState.ResolveEncounterChoice(new DeterministicRunContext("run-f2-shop-ability", 1001), "node.f2.shop.ability", encounter, "CHOICE_F02_SHOP_BUY_ABILITY");
            Assert.AreEqual("CHOICE_F02_SHOP_BUY_ABILITY", abilityResult.PayloadId);
            Assert.AreEqual(0, abilityState.Gold);
            Assert.IsTrue(abilityState.HasAbilityRef("ABILITY_RECALL_ANCHOR"));
        }

        [Test]
        public void CatalogBackedAbilityGrant_RejectsUnknownRefs()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var state = new PrototypeRunState("run-unknown-ability", new GameFlowEventBus());
            state.AttachEncounterCatalog(catalog);

            Assert.IsFalse(state.AddAbilityRef("ABILITY_PLACEHOLDER_SCOUT"));
            Assert.IsFalse(state.HasAbilityRef("ABILITY_PLACEHOLDER_SCOUT"));
            Assert.AreEqual(0, state.CreateSnapshot().AbilityCount);
        }

        [Test]
        public void CombatDefeat_WithoutRecallFailsRun()
        {
            var state = new PrototypeRunState("run-defeat", new GameFlowEventBus()) { AutoResolveCombat = false };
            var encounter = CreateRuntimeEncounter("ENC_DEFEAT", CreateChoice("CHOICE_DEFEAT", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_DEFEAT", "ENEMY_DEFEAT", new EncounterPostCombatEffectRuntimeData[0], new[] { CreatePostCombatEffect("ModifyGlitchLevel", 5) }) }));

            state.ModifyPlayerHp(-23);
            state.ResolveEncounterChoice(new DeterministicRunContext("run-defeat", 1001), "node.defeat", encounter, "CHOICE_DEFEAT");
            state.ResolveCombatRoundInteractive(CombatAction.Attack);

            Assert.IsTrue(state.RunFailed);
            Assert.IsTrue(state.RunCompleted);
            Assert.IsTrue(state.RestartReady);
            Assert.AreEqual("run.failed", state.RunStatus);
            Assert.IsTrue(state.MemoryRepo.TryGetReflection("run-defeat", out _));
        }

        [Test]
        public void CombatDefeat_WithRecallRevivesOnceAndContinues()
        {
            var state = new PrototypeRunState("run-recall-defeat", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.AddAbilityRef("ABILITY_RECALL_ANCHOR");
            var encounter = CreateRuntimeEncounter("ENC_RECALL_DEFEAT", CreateChoice("CHOICE_RECALL_DEFEAT", new EncounterRequirementRuntimeData[0], new[] { CreateCombatEffect("COMBAT_RECALL_DEFEAT", "ENEMY_RECALL_DEFEAT", new EncounterPostCombatEffectRuntimeData[0]) }));

            state.ModifyPlayerHp(-23);
            state.ResolveEncounterChoice(new DeterministicRunContext("run-recall-defeat", 1001), "node.recall.defeat", encounter, "CHOICE_RECALL_DEFEAT");
            state.ResolveCombatRoundInteractive(CombatAction.Attack);

            Assert.IsFalse(state.RunFailed);
            Assert.IsFalse(state.RunCompleted);
            Assert.IsTrue(state.IsInCombat);
            Assert.Greater(state.PlayerHp, 0);
            Assert.AreEqual("recall", state.LastCombatResultId);
            Assert.IsTrue(state.HasFlag("FLAG_RECALL_ANCHOR_USED"));
        }

        [Test]
        public void RestartRun_CreatesCleanStateAndPreservesReflectionRepo()
        {
            var controllerObject = new GameObject("PrototypeRoomController Test");
            try
            {
                var controller = controllerObject.AddComponent<PrototypeRoomController>();
                var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
                controller.Configure(null, catalog);
                controller.BeginRun();
                var firstRunId = controller.RunState.RunId;
                var request = new LLMRequest(firstRunId, "restart cache prompt", "restart.policy");
                controller.RunState.MemoryRepo.SaveCachedResponse(new LLMResponse(request.CacheKey, "cached"));
                Assert.IsTrue(controller.RunState.UnlockMemoryFragmentRef("MEM_FRAGMENT_01"));
                controller.RunState.SetFlag("FLAG_RECALL_ANCHOR_USED", true);
                controller.RunState.ModifyGold(20);
                controller.RunState.ResolveRemnant("node.remnant.end");

                Assert.IsTrue(controller.RunState.RestartReady);

                var restart = controller.RestartRun();

                Assert.AreEqual("run.restart", restart.NodeId);
                Assert.AreNotEqual(firstRunId, controller.RunState.RunId);
                Assert.AreEqual(firstRunId + ".restart.1", controller.RunState.RunId);
                Assert.AreEqual(1, controller.RunState.CurrentFloor);
                Assert.AreEqual(6, controller.RunState.Gold);
                Assert.IsFalse(controller.RunState.RunCompleted);
                Assert.IsFalse(controller.RunState.HasFlag("FLAG_RECALL_ANCHOR_USED"));
                Assert.IsTrue(controller.RunState.HasMemoryFragmentRef("MEM_FRAGMENT_01"));
                Assert.AreEqual(1, controller.GetSnapshot().MemoryFragmentCount);
                Assert.IsTrue(controller.RunState.MemoryRepo.TryGetReflection(firstRunId, out _));
                Assert.IsTrue(controller.RunState.MemoryRepo.TryGetCachedResponse(request.CacheKey, out var cached));
                Assert.AreEqual("cached", cached.Text);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
            }
        }

        [Test]
        public void RunFailed_BlocksFurtherNodeRewardsAndEffects()
        {
            var state = new PrototypeRunState("run-failed-blocks", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.ModifyPlayerHp(-23);
            var lethal = CreateRuntimeEncounter(
                "ENC_LETHAL_FAILURE",
                CreateChoice(
                    "CHOICE_LETHAL_FAILURE",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateCombatEffect("COMBAT_LETHAL_FAILURE", "ENEMY_LETHAL_FAILURE", new EncounterPostCombatEffectRuntimeData[0]) }));
            state.ResolveEncounterChoice(new DeterministicRunContext("run-failed-blocks", 1001), "node.lethal", lethal, "CHOICE_LETHAL_FAILURE");
            state.ResolveCombatRoundInteractive(CombatAction.Attack);
            Assert.IsTrue(state.RunFailed);

            var goldBefore = state.Gold;
            var blocked = state.ResolveEncounterChoice(
                new DeterministicRunContext("run-failed-blocks", 1001),
                "node.after.failure",
                CreateRuntimeEncounter("ENC_AFTER_FAILURE", CreateChoice("CHOICE_AFTER_FAILURE", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGold", 99) })),
                "CHOICE_AFTER_FAILURE");

            Assert.IsTrue(blocked.RunCompleted);
            Assert.AreEqual(goldBefore, state.Gold);
            StringAssert.Contains("run already completed", blocked.Message);
        }

        [Test]
        public void ItemAndAbilityEffects_ApplyToCombatLoop()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var state = new PrototypeRunState("run-effects", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.AttachEncounterCatalog(catalog);
            state.AddItemRef("ITEM_FIELD_BANDAGE", 1);
            state.AddAbilityRef("ABILITY_SCOUT");
            var encounter = CreateRuntimeEncounter(
                "ENC_EFFECT_COMBAT",
                CreateChoice(
                    "CHOICE_EFFECT_COMBAT",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateCombatEffect("COMBAT_EFFECT", "ENEMY_EMPTY_ARMOR", new[] { CreatePostCombatEffect("ModifyGold", 4) }) }));

            state.ModifyPlayerHp(-10);
            var before = state.CreateSnapshot();
            state.ResolveEncounterChoice(new DeterministicRunContext("run-effects", 1001), "node.effects.combat", encounter, "CHOICE_EFFECT_COMBAT");
            var started = state.CreateSnapshot();
            var round = state.ResolveCombatRoundInteractive(CombatAction.Skill);

            Assert.Greater(before.ItemCount, 0);
            Assert.Greater(started.PlayerMaxHp, 24);
            Assert.Greater(started.PlayerHp, before.PlayerHp);
            Assert.Greater(round.ComboDamage, 0);
        }

        [Test]
        public void CombatReward_DoesNotApplyTwiceOnResolvedRevisit()
        {
            var state = new PrototypeRunState("run-combat-revisit", new GameFlowEventBus()) { AutoResolveCombat = true };
            var encounter = CreateRuntimeEncounter(
                "ENC_COMBAT_REVISIT",
                CreateChoice(
                    "CHOICE_COMBAT_REVISIT",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateCombatEffect("COMBAT_REVISIT", "ENEMY_REVISIT", new[] { CreatePostCombatEffect("ModifyGold", 8) }) }));

            state.ResolveEncounterChoice(new DeterministicRunContext("run-combat-revisit", 1001), "node.combat.revisit", encounter, "CHOICE_COMBAT_REVISIT");
            var firstGold = state.Gold;
            var second = state.ResolveEncounterChoice(new DeterministicRunContext("run-combat-revisit", 1001), "node.combat.revisit", encounter, "CHOICE_COMBAT_REVISIT");

            Assert.AreEqual(8, firstGold);
            Assert.AreEqual(firstGold, state.Gold);
            StringAssert.Contains("already resolved: CHOICE_COMBAT_REVISIT", second.Message);
        }

        [Test]
        public void NpcFallbackReaction_IsDeterministicForSameChoice()
        {
            var first = new PrototypeRunState("run-npc", new GameFlowEventBus()) { AutoResolveCombat = true };
            var second = new PrototypeRunState("run-npc", new GameFlowEventBus()) { AutoResolveCombat = true };
            var encounter = CreateRuntimeEncounter(
                "ENC_NPC_REACTION",
                CreateChoice(
                    "CHOICE_NPC_REACTION",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateEffect("ModifyAffinity", 1) },
                    "Hidden",
                    string.Empty,
                    "NPC_REACT_TEST_CHOICE"));

            first.ResolveEncounterChoice(new DeterministicRunContext("run-npc", 1001), "node.npc", encounter, "CHOICE_NPC_REACTION");
            second.ResolveEncounterChoice(new DeterministicRunContext("run-npc", 1001), "node.npc", encounter, "CHOICE_NPC_REACTION");

            Assert.AreEqual(first.LastNpcReactionKey, second.LastNpcReactionKey);
            Assert.AreEqual("NPC_REACT_TEST_CHOICE", first.LastNpcReactionKey);
        }

        [Test]
        public void RestInteraction_AskMoodRequiresInputAndCommitsOnce()
        {
            var state = new PrototypeRunState("run-rest-ask", new GameFlowEventBus());

            var rejected = state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.ask_mood", string.Empty);
            Assert.AreEqual(0, state.Affinity);
            StringAssert.Contains("input required", rejected.Message);

            state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.ask_mood", "괜찮아?");
            Assert.AreEqual(2, state.Affinity);
            Assert.IsTrue(state.HasFlag("MATAIOS_HINT_S1_01"));
            Assert.AreEqual("괜찮아?", state.LastRestUtterance);
            Assert.IsFalse(string.IsNullOrEmpty(state.LastMataiosResponse));
            Assert.IsTrue(state.MemoryRepo.GetRecentReflections(4).Any(reflection => reflection.Summary.Contains("괜찮아?")));

            var duplicate = state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.ask_mood", "다시");
            Assert.AreEqual(2, state.Affinity);
            StringAssert.Contains("already resolved", duplicate.Message);
        }

        [Test]
        public void RestInteraction_TrainBuffAppliesOnceAndDoesNotStack()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var state = new PrototypeRunState("run-rest-train", new GameFlowEventBus()) { AutoResolveCombat = false };
            state.AttachEncounterCatalog(catalog);

            var rejected = state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.train", string.Empty);
            StringAssert.Contains("input required", rejected.Message);

            state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.train", "검을 맞춰 보자");
            Assert.IsTrue(state.TrainingBuffActive);
            state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.train", "한 번 더");
            Assert.IsTrue(state.TrainingBuffActive);

            var encounter = CreateRuntimeEncounter(
                "ENC_TRAIN_COMBAT",
                CreateChoice(
                    "CHOICE_TRAIN_COMBAT",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateCombatEffect("COMBAT_TRAIN", "ENEMY_EMPTY_ARMOR", new EncounterPostCombatEffectRuntimeData[0]) }));

            state.ResolveEncounterChoice(new DeterministicRunContext("run-rest-train", 1001), "node.combat", encounter, "CHOICE_TRAIN_COMBAT");
            state.ResolveCombatRoundInteractive(CombatAction.Attack);

            Assert.IsFalse(state.TrainingBuffActive);
            StringAssert.Contains("training +1", state.CreateSnapshot().LastCombatRoundResult);

            var restarted = new PrototypeRunState("run-rest-train.restart", new GameFlowEventBus(), null, state.MemoryRepo);
            Assert.IsFalse(restarted.TrainingBuffActive);
        }

        [Test]
        public void RestInteraction_RecoverAllowsEmptyInputAndHidesGlitchFromNormalSummary()
        {
            var state = new PrototypeRunState("run-rest-recover", new GameFlowEventBus());
            state.ModifyPlayerHp(-10);
            state.ModifyGlitchLevel(8);
            var hpBefore = state.PlayerHp;

            var applied = state.ResolveRestInteraction("node.rest", "ENC_REST_01", "rest.recover", string.Empty);
            Assert.Greater(state.PlayerHp, hpBefore);
            Assert.AreEqual(5, state.GlitchLevel);
            StringAssert.Contains("Glitch -3", applied.Message);

            var hudObject = new GameObject("HUD");
            try
            {
                var hud = hudObject.AddComponent<PrototypeHud>();
                hud.SetRawDebugTextVisible(false);
                hud.ShowResult(applied);
                StringAssert.DoesNotContain("Glitch", hud.ResultMessage);
            }
            finally
            {
                Object.DestroyImmediate(hudObject);
            }
        }

        [Test]
        public void DemoProgression_OrderIsReproducibleForSameSeed()
        {
            var shop = CreateRuntimeEncounter("ENC_SHOP_DEMO_ORDER", CreateChoice("CHOICE_SHOP_ORDER", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyGold", 1) }));
            var moral = CreateRuntimeEncounter("ENC_MORAL_DEMO_ORDER", CreateChoice("CHOICE_MORAL_ORDER", new EncounterRequirementRuntimeData[0], new[] { CreateEffect("ModifyAffinity", 1) }));
            var shopNode = CreateNode("node.shop.demo.order", shop);
            var battleNode = CreateNode("node.battle.demo.order", moral);

            var first = CreateDemoOrderState(shopNode, shop, battleNode, moral);
            var second = CreateDemoOrderState(shopNode, shop, battleNode, moral);

            Assert.AreEqual(first.NextDemoEncounterId, second.NextDemoEncounterId);
            first.ResolveEncounterChoice(new DeterministicRunContext("run-demo-order", 1001), shopNode.NodeId, shop, "CHOICE_SHOP_ORDER");
            second.ResolveEncounterChoice(new DeterministicRunContext("run-demo-order", 1001), shopNode.NodeId, shop, "CHOICE_SHOP_ORDER");
            Assert.AreEqual(first.NextDemoEncounterId, second.NextDemoEncounterId);
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
        public void LLMProviderFactory_UsesFakeProviderWhenConfigIsEmpty()
        {
            var repo = new InMemoryNpcMemoryRepo();
            var provider = LLMProviderFactory.Create((LLMRuntimeConfig)null, repo);
            var request = new LLMRequest("run-empty-model", "prompt", "profile");

            Assert.IsTrue(provider.TryComplete(request, out var response));
            Assert.AreEqual(request.CacheKey, response.CacheKey);
            Assert.IsTrue(response.Text.Contains(request.CacheKey.PromptHash));
        }

        [Test]
        public void LLMProviderFactory_CachesByRunIdAndPromptHash()
        {
            var repo = new InMemoryNpcMemoryRepo();
            var provider = LLMProviderFactory.Create(LLMRuntimeConfig.FakeDefault(), repo);
            var request = new LLMRequest("run-cache-model", "same prompt", "profile");

            Assert.IsTrue(provider.TryComplete(request, out var first));
            Assert.IsTrue(repo.TryGetCachedResponse(request.CacheKey, out var cached));
            Assert.IsTrue(provider.TryComplete(request, out var second));

            Assert.AreEqual(first.CacheKey, cached.CacheKey);
            Assert.AreEqual(cached.Text, second.Text);
        }

        [Test]
        public void LLMModelManifestLoader_InvalidModelPathFallsBackWithoutBreakingFlow()
        {
            var configPath = Path.Combine(Application.temporaryCachePath, "hwigi-llm-invalid-model-config.json");
            File.WriteAllText(configPath,
                "{\"providerMode\":1,\"modelId\":\"missing-model\",\"streamingAssetsRelativePath\":\"LLM/missing-model/model\",\"tokenizerRelativePath\":\"LLM/missing-model/tokenizer\",\"maxInputTokens\":128,\"maxOutputTokens\":16,\"deterministicCacheEnabled\":true,\"fallbackEnabled\":true}");

            Assert.IsTrue(LLMModelManifestLoader.TryLoadConfigFile(configPath, out var config));
            var provider = LLMProviderFactory.Create(config, new InMemoryNpcMemoryRepo());
            var request = new LLMRequest("run-invalid-model", "prompt", "profile");

            Assert.IsTrue(provider.TryComplete(request, out var response));
            Assert.AreEqual(request.CacheKey, response.CacheKey);
        }

        [Test]
        public void LLMModelManifestLoader_EmptyModelIdReturnsFakeConfig()
        {
            Assert.IsFalse(LLMModelManifestLoader.TryLoadProjectModelConfig(string.Empty, out var config));
            var provider = LLMProviderFactory.Create(config, new InMemoryNpcMemoryRepo());
            var request = new LLMRequest("run-empty-manifest", "prompt", "profile");

            Assert.IsTrue(provider.TryComplete(request, out var response));
            Assert.AreEqual(request.CacheKey, response.CacheKey);
        }

        [Test]
        public void LLMModelIntakeFiles_ExistInExpectedFolders()
        {
            Assert.IsTrue(Directory.Exists("Assets/_Project/Models/_training/datasets"));
            Assert.IsTrue(Directory.Exists("Assets/_Project/Models/_training/adapters"));
            Assert.IsTrue(Directory.Exists("Assets/_Project/Models/_training/evals"));
            Assert.IsTrue(Directory.Exists("Assets/_Project/Models/mataios-demo-sft-v0.1/tokenizer"));
            Assert.IsTrue(Directory.Exists("Assets/_Project/Models/mataios-demo-sft-v0.1/compiled/android"));
            Assert.IsTrue(Directory.Exists("Assets/_Project/Models/mataios-demo-sft-v0.1/eval"));
            Assert.IsTrue(Directory.Exists("Assets/StreamingAssets/LLM/mataios-demo-sft-v0.1"));
            Assert.IsTrue(File.Exists("Assets/_Project/Models/mataios-demo-sft-v0.1/MODEL_MANIFEST.md"));
            Assert.IsTrue(File.Exists("Assets/_Project/Models/mataios-demo-sft-v0.1/model_config.example.json"));
            Assert.IsTrue(File.Exists("Assets/StreamingAssets/LLM/_README.md"));
        }

        [Test]
        public void PrototypeRunState_ForwardsRunEventsToAttachedNpcStateMachine()
        {
            var definition = ScriptableObject.CreateInstance<NpcStateMachineDefinition>();
            SetNpcTransition(definition, NpcStage.S0, "run.completed", NpcStage.S1);
            var bus = new GameFlowEventBus();
            var state = new PrototypeRunState("run-001", bus) { AutoResolveCombat = true };
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
                choice.FindPropertyRelative("npcReactionKey").stringValue = choices[i].npcReactionKey;
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
            string unavailableReasonTextKey = "",
            string npcReactionKey = "")
        {
            return new EncounterChoiceRuntimeData
            {
                stableId = stableId,
                requirementMode = "All",
                requirements = requirements,
                effects = effects,
                unavailablePolicyMode = unavailableMode,
                unavailableReasonTextKey = unavailableReasonTextKey,
                npcReactionKey = npcReactionKey
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
            return CreateCombatEffect(combatId, enemyRef, onVictoryEffects, new EncounterPostCombatEffectRuntimeData[0]);
        }

        private static EncounterEffectRuntimeData CreateCombatEffect(
            string combatId,
            string enemyRef,
            EncounterPostCombatEffectRuntimeData[] onVictoryEffects,
            EncounterPostCombatEffectRuntimeData[] onDefeatEffects)
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
                    onDefeatEffects = onDefeatEffects
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

        private static PrototypeRunState CreateDemoOrderState(
            PrototypeNodeDefinition firstNode,
            EncounterData firstEncounter,
            PrototypeNodeDefinition secondNode,
            EncounterData secondEncounter)
        {
            var state = new PrototypeRunState("run-demo-order", new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachDemoRunPath(new[]
            {
                new PrototypeDemoRunStep(firstNode, firstEncounter),
                new PrototypeDemoRunStep(secondNode, secondEncounter)
            });
            return state;
        }

        private static PrototypeRunState CreateClearedRun(string runId)
        {
            var encounter = CreateRuntimeEncounter(
                "ENC_CLEAR_FOR_ENDING",
                CreateChoice(
                    "CHOICE_CLEAR_FOR_ENDING",
                    new EncounterRequirementRuntimeData[0],
                    new[] { CreateEffect("ModifyAffinity", 1) }));
            var node = CreateNode("node.clear.for.ending", encounter);
            var state = new PrototypeRunState(runId, new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachDemoRunPath(new[] { new PrototypeDemoRunStep(node, encounter) });
            state.ResolveEncounterChoice(new DeterministicRunContext(runId, 1001), node.NodeId, encounter, "CHOICE_CLEAR_FOR_ENDING");
            return state;
        }

        private static PrototypeRunState CreateConfiguredRouteState(string runId, PrototypeRoomDefinition room, EncounterRuntimeCatalogData catalog)
        {
            Assert.IsNotNull(room);
            Assert.IsNotNull(catalog);
            var state = new PrototypeRunState(runId, new GameFlowEventBus()) { AutoResolveCombat = true };
            state.AttachEncounterCatalog(catalog);
            state.AttachFloorRunPaths(room.FloorRunPaths, room.DemoRunPath);
            return state;
        }

        private static void ResolveFullRouteToFinalBoss(PrototypeRunState state)
        {
            var guard = 0;
            while (!state.EndingChoicePending && guard++ < 64)
            {
                if (state.StairUnlocked)
                {
                    state.ResolveNextFloor();
                    continue;
                }

                var selectable = state.GetSelectableMapNodeViews();
                if (selectable.Length == 0 && state.GetFloorMapNodeViews().Length == 0 && state.TryGetNextDemoStep(out var pendingStep))
                {
                    ResolveRouteStep(state, pendingStep);
                    continue;
                }

                Assert.Greater(selectable.Length, 0, "Expected selectable map nodes on floor " + state.CurrentFloor);
                var selected = selectable[0];
                Assert.IsTrue(state.TrySelectMapNode(selected.MapNodeId, out var step), "Expected selectable map node " + selected.MapNodeId);
                ResolveRouteStep(state, step);
            }

            Assert.Less(guard, 64);
            Assert.AreEqual(5, state.CurrentFloor);
        }

        private static void ResolveRouteChoice(PrototypeRunState state, string expectedEncounterId, string choiceStableId)
        {
            SelectMapNodeForEncounter(state, expectedEncounterId);
            Assert.IsTrue(state.TryGetNextDemoStep(out var step), "Expected a route step for " + expectedEncounterId);
            Assert.AreEqual(expectedEncounterId, step.EncounterId);
            var resolution = state.ResolveEncounterChoice(new DeterministicRunContext(state.RunId, 1001), step.NodeId, step.Encounter, choiceStableId);
            Assert.AreEqual(choiceStableId, resolution.PayloadId);
        }

        private static string ResolvePreferredChoiceId(PrototypeRunState state, PrototypeDemoRunStep step)
        {
            Assert.IsNotNull(step);
            Assert.IsNotNull(step.Encounter);
            string preferredChoiceId;
            switch (step.EncounterId)
            {
                case "EVT_F01_JAR_ROOM":
                    preferredChoiceId = "CHOICE_EVT_F01_JAR_PLAIN";
                    break;
                case "ENC_MORAL_CHOICE_01":
                    preferredChoiceId = "CHOICE_MORAL_01_REFUSE";
                    break;
                case "ENC_MORAL_CHOICE_02":
                    preferredChoiceId = "CHOICE_MORAL_02_REFUSE";
                    break;
                case "ENC_MORAL_CHOICE_03":
                    preferredChoiceId = "CHOICE_MORAL_03_AID";
                    break;
                case "ENC_F02_MORAL_CHOICE_001":
                    preferredChoiceId = "CHOICE_F02_MORAL_LEAVE";
                    break;
                case "ENC_SHOP_01":
                    preferredChoiceId = "CHOICE_SHOP_01_LEAVE";
                    break;
                case "ENC_SHOP_02":
                    preferredChoiceId = "CHOICE_SHOP_02_LEAVE";
                    break;
                case "ENC_SHOP_03":
                    preferredChoiceId = "CHOICE_SHOP_03_LEAVE";
                    break;
                case "ENC_SHOP_04":
                    preferredChoiceId = "CHOICE_SHOP_04_LEAVE";
                    break;
                case "ENC_SHOP_05":
                    preferredChoiceId = "CHOICE_SHOP_05_LEAVE";
                    break;
                case "ENC_F02_SHOP_001":
                    preferredChoiceId = "CHOICE_F02_SHOP_BUY_ITEM";
                    break;
                case "ENC_COMBAT_GATE_01":
                    preferredChoiceId = "CHOICE_COMBAT_01_ENGAGE";
                    break;
                case "ENC_COMBAT_GATE_02":
                    preferredChoiceId = "CHOICE_COMBAT_02_ENGAGE";
                    break;
                case "ENC_COMBAT_GATE_03":
                    preferredChoiceId = "CHOICE_COMBAT_03_ENGAGE";
                    break;
                case "ENC_MEMORY_FRAGMENT_01":
                    preferredChoiceId = "CHOICE_MEMORY_01_UNLOCK";
                    break;
                case "ENC_MEMORY_FRAGMENT_02":
                    preferredChoiceId = "CHOICE_MEMORY_02_UNLOCK";
                    break;
                case "ENC_MEMORY_FRAGMENT_03":
                    preferredChoiceId = "CHOICE_MEMORY_03_UNLOCK";
                    break;
                case "ENC_MEMORY_FRAGMENT_04":
                    preferredChoiceId = "CHOICE_MEMORY_04_UNLOCK";
                    break;
                case "ENC_MEMORY_FRAGMENT_05":
                    preferredChoiceId = "CHOICE_MEMORY_05_UNLOCK";
                    break;
                case "ENC_REST_01":
                    preferredChoiceId = "CHOICE_REST_01_REST";
                    break;
                case "ENC_REST_02":
                    preferredChoiceId = "CHOICE_REST_02_REST";
                    break;
                case "ENC_REST_03":
                    preferredChoiceId = "CHOICE_REST_03_REST";
                    break;
                case "ENC_REST_05":
                    preferredChoiceId = "CHOICE_REST_05_REST";
                    break;
                default:
                    preferredChoiceId = string.Empty;
                    break;
            }

            return FindAvailableChoiceOrFirst(state, step, preferredChoiceId);
        }

        private static void ResolveRouteStep(PrototypeRunState state, PrototypeDemoRunStep step)
        {
            Assert.IsNotNull(step);
            var choiceStableId = ResolvePreferredChoiceId(state, step);
            var resolution = state.ResolveEncounterChoice(new DeterministicRunContext(state.RunId, 1001), step.NodeId, step.Encounter, choiceStableId);
            Assert.AreEqual(choiceStableId, resolution.PayloadId);
        }

        private static string FindAvailableChoiceOrFirst(PrototypeRunState state, PrototypeDemoRunStep step, string preferredChoiceId)
        {
            var views = PrototypeEncounterRuntimeResolver.BuildChoiceViews(state, step.Encounter);
            if (!string.IsNullOrEmpty(preferredChoiceId))
            {
                for (var i = 0; i < views.Length; i++)
                {
                    if (views[i].ChoiceStableId == preferredChoiceId && views[i].Enabled)
                    {
                        return preferredChoiceId;
                    }
                }
            }

            for (var i = 0; i < views.Length; i++)
            {
                if (views[i].Visible && views[i].Enabled)
                {
                    return views[i].ChoiceStableId;
                }
            }

            return FindChoiceOrFirst(step, preferredChoiceId);
        }

        private static string FindChoiceOrFirst(PrototypeDemoRunStep step, string preferredChoiceId)
        {
            Assert.IsNotNull(step.Encounter.Choices);
            Assert.Greater(step.Encounter.Choices.Length, 0);
            for (var i = 0; i < step.Encounter.Choices.Length; i++)
            {
                if (step.Encounter.Choices[i].stableId == preferredChoiceId)
                {
                    return preferredChoiceId;
                }
            }

            return step.Encounter.Choices[0].stableId;
        }

        private static void SelectMapNodeForEncounter(PrototypeRunState state, string expectedEncounterId)
        {
            var selectable = state.GetSelectableMapNodeViews();
            if (selectable.Length == 0)
            {
                return;
            }

            var match = selectable.FirstOrDefault(node => node.MapNodeId.EndsWith("." + expectedEncounterId));
            Assert.IsFalse(string.IsNullOrEmpty(match.MapNodeId), "Expected selectable map node for " + expectedEncounterId);
            Assert.IsTrue(state.TrySelectMapNode(match.MapNodeId, out _), "Expected map selection for " + expectedEncounterId);
        }

        private static void SelectFirstMapNodeOfType(PrototypeRunState state, PrototypeFloorMapNodeType type)
        {
            var match = state.GetSelectableMapNodeViews().FirstOrDefault(node => node.Type == type);
            Assert.IsFalse(string.IsNullOrEmpty(match.MapNodeId), "Expected selectable map node type " + type);
            Assert.IsTrue(state.TrySelectMapNode(match.MapNodeId, out _), "Expected selectable map node type " + type);
        }

        private static void AssertPoolRefsResolve(EncounterRuntimeCatalogData catalog, string[] refs)
        {
            foreach (var enemyRef in refs)
            {
                Assert.IsTrue(catalog.TryGetEnemy(enemyRef, out _), enemyRef);
            }
        }

        private static string[] RouteEncounterIds(IReadOnlyList<PrototypeDemoRunStep> steps)
        {
            Assert.IsNotNull(steps);
            return steps.Select(step => step == null ? string.Empty : step.EncounterId).ToArray();
        }

        private static string[] BuildMapSignature(PrototypeFloorMapNodeView[] nodes)
        {
            return nodes
                .OrderBy(node => node.Layer)
                .ThenBy(node => node.Index)
                .Select(node => string.Join("|",
                    node.MapNodeId,
                    node.Type.ToString(),
                    node.Layer.ToString(),
                    node.Index.ToString(),
                    node.NormalizedX.ToString("0.000"),
                    node.NormalizedY.ToString("0.000"),
                    string.Join(",", node.NextMapNodeIds)))
                .ToArray();
        }

        private static PrototypeFloorRunPath CreateFloorPath(int floor, params PrototypeDemoRunStep[] steps)
        {
            return new PrototypeFloorRunPath(floor, steps);
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

        private static ItemData CreateItem(string id, string passiveTrigger)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            var serialized = new SerializedObject(item);
            serialized.FindProperty("stableId").stringValue = id;
            serialized.FindProperty("passiveTrigger").stringValue = passiveTrigger;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return item;
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
