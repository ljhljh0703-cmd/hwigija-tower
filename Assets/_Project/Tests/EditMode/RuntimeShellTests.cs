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
        public void DemoProgression_CompletesOnlyAfterRequiredFourEncounters()
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
            Assert.AreEqual("ENC_SHOP_DEMO", state.NextDemoEncounterId);

            state.ResolveEncounterChoice(new DeterministicRunContext("run-demo", 1001), shopNode.NodeId, shop, "CHOICE_SHOP_DEMO");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-demo", 1001), battleNode.NodeId, moral, "CHOICE_MORAL_DEMO");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-demo", 1001), battleNode.NodeId, memory, "CHOICE_MEMORY_DEMO");
            Assert.IsFalse(state.RunCompleted);
            Assert.AreEqual("ENC_COMBAT_DEMO", state.NextDemoEncounterId);

            var completion = state.ResolveEncounterChoice(new DeterministicRunContext("run-demo", 1001), battleNode.NodeId, combat, "CHOICE_COMBAT_DEMO");

            Assert.IsTrue(state.RunCompleted);
            Assert.IsTrue(state.DemoComplete);
            Assert.AreEqual("run.clear", state.DemoStatus);
            StringAssert.Contains("run.clear", completion.Message);
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
            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-unlock", 1001), floorTwoPrepNode.NodeId, floorTwoPrep, "CHOICE_FLOOR_TWO_PREP");

            Assert.AreEqual(2, state.CurrentFloor);
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
            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-clear", 1001), floorTwoPrepNode.NodeId, floorTwoPrep, "CHOICE_FLOOR_TWO_PREP_BOSS");
            state.ResolveEncounterChoice(new DeterministicRunContext("run-boss-clear", 1001), bossNode.NodeId, bossGate, "CHOICE_FLOOR_TWO_BOSS_CLEAR");

            Assert.IsTrue(state.RunClear);
            Assert.IsTrue(state.RunCompleted);
            Assert.IsTrue(state.RestartReady);
            Assert.AreEqual("run.clear", state.RunStatus);
            Assert.IsTrue(state.MemoryRepo.TryGetReflection("run-boss-clear", out _));
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
                controller.Configure(null, null);
                controller.BeginRun();
                var firstRunId = controller.RunState.RunId;
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
                Assert.IsTrue(controller.RunState.MemoryRepo.TryGetReflection(firstRunId, out _));
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
            }
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
