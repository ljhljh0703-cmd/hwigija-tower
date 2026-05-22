#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using HwigiTower.Abilities;
using HwigiTower.Combat;
using HwigiTower.Items;
using HwigiTower.Rewards;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.Encounters
{
    public sealed class EncounterRuntimeCatalogBuildResult
    {
        private readonly List<string> _assetPaths = new List<string>();

        public IReadOnlyList<string> AssetPaths => _assetPaths;
        public EncounterRuntimeCatalogData Catalog { get; private set; }

        public void AddAssetPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _assetPaths.Add(path);
            }
        }

        public void SetCatalog(EncounterRuntimeCatalogData catalog)
        {
            Catalog = catalog;
        }
    }

    public static class EncounterRuntimeCatalogBuilder
    {
        public const string CatalogPath = "Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset";
        public const string PrototypeBossGateEncounterPath = "Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_02.asset";
        public const string PrototypeFinalBossEncounterPath = "Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_03.asset";
        public const string PrototypeFloorTwoShopEncounterPath = "Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_SHOP_001.asset";
        private const string FloorEnemyPoolPath = "Assets/_Project/Data/Enemies/SO_FloorEnemyPool_v0_1.asset";

        private static readonly string[] ItemIds =
        {
            "ITEM_01",
            "ITEM_02",
            "ITEM_03",
            "ITEM_04",
            "ITEM_05",
            "ITEM_09",
            "ITEM_10",
            "ITEM_FIELD_BANDAGE",
            "ITEM_LANTERN_OIL",
            "ITEM_TORN_CHARM"
        };

        private static readonly string[] AbilityIds =
        {
            "ABILITY_SWORD_01",
            "ABILITY_SWORD_02",
            "ABILITY_SWORD_03",
            "ABILITY_ARTS_03",
            "ABILITY_GUARD_01",
            "ABILITY_SCOUT",
            "ABILITY_RECALL_ANCHOR"
        };

        private static readonly string[] SynergyIds =
        {
            "FRENZY"
        };

        private static readonly string[] RewardBundleIds =
        {
            "REWARD_CACHE_MEMORY",
            "REWARD_CACHE_SMALL"
        };

        private static readonly string[] EnemyIds =
        {
            "ENEMY_BANDIT_MELEE_01",
            "ENEMY_BANDIT_RANGED_01",
            "ENEMY_COLLAPSE_ECHO",
            "ENEMY_EMPTY_ARMOR",
            "ENEMY_FRACTURE_HOUND",
            "ENEMY_HOMUNCULUS_01",
            "ENEMY_IRON_MAIDEN_01",
            "ENEMY_LAMPLIGHTER_01",
            "ENEMY_LIVING_ARMOR_LIGHT_01",
            "ENEMY_LIVING_TOMBSTONE_01",
            "ENEMY_MANEATER_JUNGLE_01",
            "ENEMY_MANEATER_PLANT_01",
            "ENEMY_MERCENARY_ASSASSIN_01",
            "ENEMY_MERCENARY_CAPTAIN_SAGAN_01",
            "ENEMY_MERCENARY_MAGE_01",
            "ENEMY_MERCENARY_MELEE_01",
            "ENEMY_MERCENARY_RANGED_01",
            "ENEMY_SHADE_03",
            "ENEMY_SKELETON_01",
            "ENEMY_SKELETON_HORDE_01",
            "ENEMY_SLIME_01",
            "ENEMY_SLIME_POOL_01",
            "ENEMY_STATUE_01",
            "ENEMY_WILD_BEAST_01",
            "ENEMY_WILD_BEAST_PACK_01",
            "ENEMY_WRAITH_04",
            "BOSS_GATE_01",
            "BOSS_APEX_02",
            "ENEMY_WALKER_01",
            "ENEMY_CRAWLER_02",
            "ENEMY_HERALD_05"
        };

        private static readonly string[] MemoryFragmentIds =
        {
            "MEM_FRAGMENT_01",
            "MEM_FRAGMENT_02",
            "MEM_FRAGMENT_03",
            "MEM_FRAGMENT_04",
            "MEM_FRAGMENT_05"
        };

        private static readonly string[] MemoryFragmentStages =
        {
            "S1_AWARENESS",
            "S2_COMPANION",
            "S3_FRACTURE",
            "S4_COLLAPSE",
            "S5_REST_OR_CONTINUE"
        };

        private static readonly string[] ShopEncounterPaths =
        {
            "Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_01.asset",
            PrototypeFloorTwoShopEncounterPath,
            "Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_02.asset",
            "Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_03.asset",
            "Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_04.asset",
            "Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_05.asset"
        };

        [MenuItem("Hwigi Tower/Encounter Pipeline v0.2/Build Runtime StableId Catalog")]
        public static void BuildDefaultCatalogMenu()
        {
            var result = BuildDefaultCatalog();
            Debug.Log("Encounter runtime catalog built: assets=" + result.AssetPaths.Count + "; catalog=" + CatalogPath);
        }

        public static EncounterRuntimeCatalogBuildResult BuildDefaultCatalog()
        {
            EnsureFormalFolders();
            var result = new EncounterRuntimeCatalogBuildResult();

            var items = new ItemData[ItemIds.Length];
            for (var i = 0; i < ItemIds.Length; i++)
            {
                var path = "Assets/_Project/Data/Items/SO_Item_" + ItemIds[i] + ".asset";
                items[i] = EnsureAsset<ItemData>(path, result);
                ApplyItem(items[i], ItemIds[i]);
            }

            var abilities = new AbilityData[AbilityIds.Length];
            for (var i = 0; i < AbilityIds.Length; i++)
            {
                var path = "Assets/_Project/Data/Abilities/SO_Ability_" + AbilityIds[i] + ".asset";
                abilities[i] = EnsureAsset<AbilityData>(path, result);
                ApplyAbility(abilities[i], AbilityIds[i]);
            }

            var synergies = new SynergyData[SynergyIds.Length];
            for (var i = 0; i < SynergyIds.Length; i++)
            {
                var path = "Assets/_Project/Data/Synergies/SO_Synergy_" + SynergyIds[i] + ".asset";
                synergies[i] = EnsureAsset<SynergyData>(path, result);
                ApplySynergy(synergies[i], SynergyIds[i]);
            }

            var enemies = new EnemyData[EnemyIds.Length];
            for (var i = 0; i < EnemyIds.Length; i++)
            {
                var path = "Assets/_Project/Data/Enemies/SO_Enemy_" + EnemyIds[i] + ".asset";
                enemies[i] = EnsureAsset<EnemyData>(path, result);
                if (string.IsNullOrEmpty(enemies[i].Id))
                {
                    ApplyEnemy(enemies[i], EnemyIds[i]);
                }
            }

            var rewardBundles = new RewardBundleData[RewardBundleIds.Length];
            for (var i = 0; i < RewardBundleIds.Length; i++)
            {
                var path = "Assets/_Project/Data/Rewards/SO_Reward_" + RewardBundleIds[i] + ".asset";
                rewardBundles[i] = EnsureAsset<RewardBundleData>(path, result);
                ApplyRewardBundle(rewardBundles[i], RewardBundleIds[i]);
            }

            var memoryFragments = new MemoryFragmentData[MemoryFragmentIds.Length];
            for (var i = 0; i < MemoryFragmentIds.Length; i++)
            {
                var path = "Assets/_Project/Data/MemoryFragments/SO_MemoryFragment_" + MemoryFragmentIds[i] + ".asset";
                memoryFragments[i] = EnsureAsset<MemoryFragmentData>(path, result);
                ApplyMemoryFragment(memoryFragments[i], MemoryFragmentIds[i], MemoryFragmentStages[i]);
            }

            var catalog = EnsureAsset<EncounterRuntimeCatalogData>(CatalogPath, result);
            ApplyCatalog(catalog, items, rewardBundles, abilities, synergies, enemies, memoryFragments);
            result.SetCatalog(catalog);
            ApplyPrototypeRuntimeOverrides(result);

            AssetDatabase.SaveAssets();
            TrimTrailingWhitespace(result.AssetPaths);
            AssetDatabase.Refresh();
            return result;
        }

        public static void ApplyPrototypeRuntimeOverrides(EncounterRuntimeCatalogBuildResult result = null)
        {
            var bossGate = AssetDatabase.LoadAssetAtPath<EncounterData>(PrototypeBossGateEncounterPath);
            if (bossGate != null)
            {
                ApplyPrototypeBossGateOverride(bossGate);
                EditorUtility.SetDirty(bossGate);
                result?.AddAssetPath(PrototypeBossGateEncounterPath);
            }

            for (var i = 0; i < ShopEncounterPaths.Length; i++)
            {
                var shop = AssetDatabase.LoadAssetAtPath<EncounterData>(ShopEncounterPaths[i]);
                if (shop == null)
                {
                    continue;
                }

                ApplyPrototypeShopBuildSurfaceOverride(shop);
                EditorUtility.SetDirty(shop);
                result?.AddAssetPath(ShopEncounterPaths[i]);
            }

            var finalBoss = AssetDatabase.LoadAssetAtPath<EncounterData>(PrototypeFinalBossEncounterPath);
            if (finalBoss != null)
            {
                ApplyPrototypeFinalBossOverride(finalBoss);
                EditorUtility.SetDirty(finalBoss);
                result?.AddAssetPath(PrototypeFinalBossEncounterPath);
            }
        }

        private static T EnsureAsset<T>(string path, EncounterRuntimeCatalogBuildResult result)
            where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }

            result.AddAssetPath(path);
            return asset;
        }

        private static void ApplyItem(ItemData item, string stableId)
        {
            var serialized = new SerializedObject(item);
            serialized.FindProperty("stableId").stringValue = stableId;
            serialized.FindProperty("displayNameKey").stringValue = "PLACEHOLDER_" + stableId + "_NAME";
            serialized.FindProperty("descriptionKey").stringValue = "PLACEHOLDER_" + stableId + "_DESC";
            serialized.FindProperty("maxStack").intValue = 99;
            serialized.FindProperty("passiveTrigger").stringValue = ResolveItemTrigger(stableId);
            SetNumericParams(serialized.FindProperty("numericParams"), ResolveItemParams(stableId));
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(item);
        }

        private static void ApplyAbility(AbilityData ability, string stableId)
        {
            var serialized = new SerializedObject(ability);
            serialized.FindProperty("id").stringValue = stableId;
            serialized.FindProperty("displayName").stringValue = "PLACEHOLDER_" + stableId + "_NAME";
            serialized.FindProperty("tag").stringValue = ResolveAbilityTag(stableId);
            serialized.FindProperty("description").stringValue = "PLACEHOLDER_" + stableId + "_DESC";
            SetNumericParams(serialized.FindProperty("numericParams"), ResolveAbilityParams(stableId));
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ability);
        }

        private static string ResolveItemTrigger(string stableId)
        {
            switch (stableId)
            {
                case "ITEM_03":
                    return "player_attack";
                case "ITEM_04":
                    return "player_defend";
                case "ITEM_05":
                    return "round_start";
                case "ITEM_09":
                    return "first_hit_per_combat";
                case "ITEM_FIELD_BANDAGE":
                    return "combat_start";
                default:
                    return string.Empty;
            }
        }

        private static NumericParamSpec[] ResolveItemParams(string stableId)
        {
            switch (stableId)
            {
                case "ITEM_01":
                    return new[] { new NumericParamSpec("max_hp_bonus", 4f) };
                case "ITEM_02":
                    return new[] { new NumericParamSpec("atk_bonus", 1f) };
                case "ITEM_03":
                    return new[] { new NumericParamSpec("flat_damage_bonus", 1f) };
                case "ITEM_04":
                    return new[] { new NumericParamSpec("damage_reduce", 1f) };
                case "ITEM_05":
                    return new[] { new NumericParamSpec("poison_damage_per_round", 1f) };
                case "ITEM_09":
                    return new[] { new NumericParamSpec("damage_reduce", 3f) };
                case "ITEM_10":
                    return new[]
                    {
                        new NumericParamSpec("max_hp_penalty", -5f),
                        new NumericParamSpec("atk_bonus", 3f)
                    };
                case "ITEM_FIELD_BANDAGE":
                    return new[]
                    {
                        new NumericParamSpec("max_hp_bonus", 2f),
                        new NumericParamSpec("hp_restore", 4f)
                    };
                default:
                    return new NumericParamSpec[0];
            }
        }

        private static string ResolveAbilityTag(string stableId)
        {
            switch (stableId)
            {
                case "ABILITY_SWORD_01":
                case "ABILITY_SWORD_02":
                case "ABILITY_SWORD_03":
                    return "검";
                case "ABILITY_ARTS_03":
                    return "술";
                case "ABILITY_GUARD_01":
                    return "결";
                default:
                    return string.Empty;
            }
        }

        private static NumericParamSpec[] ResolveAbilityParams(string stableId)
        {
            switch (stableId)
            {
                case "ABILITY_SWORD_01":
                    return new[] { new NumericParamSpec("player.attack_bonus", 3f) };
                case "ABILITY_SWORD_02":
                    return new[]
                    {
                        new NumericParamSpec("attack.extra_atk_multiplier", 0.5f),
                        new NumericParamSpec("attack.rounds_interval", 3f)
                    };
                case "ABILITY_SWORD_03":
                    return new[]
                    {
                        new NumericParamSpec("attack_strike_bonus", 5f),
                        new NumericParamSpec("player.hp_cost_nonlethal", 3f)
                    };
                case "ABILITY_ARTS_03":
                    return new[]
                    {
                        new NumericParamSpec("skill.direct_damage", 8f),
                        new NumericParamSpec("skill.cooldown_rounds", 4f)
                    };
                case "ABILITY_GUARD_01":
                    return new[] { new NumericParamSpec("player.damage_reduction", 3f) };
                case "ABILITY_SCOUT":
                    return new[] { new NumericParamSpec("player.attack_bonus", 1f) };
                case "ABILITY_RECALL_ANCHOR":
                    return new[] { new NumericParamSpec("recall_anchor_restore", 6f) };
                default:
                    return new NumericParamSpec[0];
            }
        }

        private static void ApplySynergy(SynergyData synergy, string stableId)
        {
            var serialized = new SerializedObject(synergy);
            serialized.FindProperty("tag").stringValue = stableId == "FRENZY" ? "검" : string.Empty;
            serialized.FindProperty("requiredCount").intValue = stableId == "FRENZY" ? 3 : 0;
            serialized.FindProperty("effectDescription").stringValue = stableId == "FRENZY"
                ? "검 x3: Attack extra hit; chain bonus after Attack."
                : string.Empty;
            SetNumericParams(serialized.FindProperty("numericParams"),
                stableId == "FRENZY"
                    ? new[]
                    {
                        new NumericParamSpec("synergy.extra_atk_multiplier", 0.5f),
                        new NumericParamSpec("synergy.attack_chain_extra_atk_multiplier", 0.75f)
                    }
                    : new NumericParamSpec[0]);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(synergy);
        }

        private static void ApplyEnemy(EnemyData enemy, string stableId)
        {
            var serialized = new SerializedObject(enemy);
            serialized.FindProperty("id").stringValue = stableId;
            serialized.FindProperty("hp").intValue = ResolveEnemyHp(stableId);
            serialized.FindProperty("attack").intValue = ResolveEnemyAttack(stableId);
            serialized.FindProperty("patternId").stringValue = IsEliteOrBoss(stableId) ? "PATTERN_ELITE" : "PATTERN_BASIC";
            serialized.FindProperty("pattern").objectReferenceValue = AssetDatabase.LoadAssetAtPath<EnemyPatternData>(
                IsEliteOrBoss(stableId)
                    ? "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_ELITE.asset"
                    : "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_BASIC.asset");
            serialized.FindProperty("goldReward").intValue = ResolveEnemyGold(stableId);
            serialized.FindProperty("xpReward").intValue = ResolveEnemyXp(stableId);
            serialized.FindProperty("glitchDelta").intValue = IsBoss(stableId) ? -3 : 0;
            serialized.FindProperty("affinityDelta").intValue = IsBoss(stableId) ? 2 : 0;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(enemy);
        }

        private static int ResolveEnemyHp(string stableId)
        {
            switch (stableId)
            {
                case "BOSS_APEX_02":
                    return 30;
                case "BOSS_GATE_01":
                    return 28;
                case "ENEMY_WRAITH_04":
                    return 12;
                case "ENEMY_COLLAPSE_ECHO":
                case "ENEMY_LAMPLIGHTER_01":
                    return 26;
                case "ENEMY_STATUE_01":
                    return 20;
                case "ENEMY_MERCENARY_CAPTAIN_SAGAN_01":
                    return 24;
                case "ENEMY_SHADE_03":
                    return 22;
                case "ENEMY_IRON_MAIDEN_01":
                    return 20;
                case "ENEMY_WILD_BEAST_PACK_01":
                    return 18;
                case "ENEMY_SLIME_POOL_01":
                    return 16;
                default:
                    return IsFloorFiveNormal(stableId) ? 18 : IsFloorFourNormal(stableId) ? 16 : IsFloorThreeNormal(stableId) ? 14 : 12;
            }
        }

        private static int ResolveEnemyAttack(string stableId)
        {
            if (stableId == "ENEMY_WRAITH_04")
            {
                return 2;
            }

            if (stableId == "ENEMY_MERCENARY_CAPTAIN_SAGAN_01" ||
                stableId == "ENEMY_SHADE_03" ||
                stableId == "ENEMY_HOMUNCULUS_01")
            {
                return 5;
            }

            return IsEliteOrBoss(stableId) || IsFloorThreeNormal(stableId) || IsFloorFourNormal(stableId) || IsFloorFiveNormal(stableId) ? 4 : 3;
        }

        private static int ResolveEnemyGold(string stableId)
        {
            switch (stableId)
            {
                case "BOSS_APEX_02":
                    return 30;
                case "BOSS_GATE_01":
                    return 16;
                case "ENEMY_WRAITH_04":
                    return 18;
                case "ENEMY_COLLAPSE_ECHO":
                    return 14;
                case "ENEMY_STATUE_01":
                    return 10;
                case "ENEMY_MERCENARY_CAPTAIN_SAGAN_01":
                case "ENEMY_SHADE_03":
                    return 10;
                case "ENEMY_IRON_MAIDEN_01":
                    return 8;
                case "ENEMY_SLIME_POOL_01":
                case "ENEMY_WILD_BEAST_PACK_01":
                    return 6;
                default:
                    return IsFloorFiveNormal(stableId) ? 6 : IsFloorThreeNormal(stableId) || IsFloorFourNormal(stableId) ? 5 : 4;
            }
        }

        private static int ResolveEnemyXp(string stableId)
        {
            var gold = ResolveEnemyGold(stableId);
            return IsBoss(stableId) ? gold * 3 : gold;
        }

        private static bool IsEliteOrBoss(string stableId)
        {
            return IsBoss(stableId) ||
                   stableId == "ENEMY_IRON_MAIDEN_01" ||
                   stableId == "ENEMY_MERCENARY_CAPTAIN_SAGAN_01" ||
                stableId == "ENEMY_SHADE_03" ||
                stableId == "ENEMY_LAMPLIGHTER_01" ||
                stableId == "ENEMY_SLIME_POOL_01" ||
                   stableId == "ENEMY_WILD_BEAST_PACK_01";
        }

        private static bool IsBoss(string stableId)
        {
            return stableId == "BOSS_APEX_02" ||
                   stableId == "BOSS_GATE_01" ||
                   stableId == "ENEMY_COLLAPSE_ECHO" ||
                   stableId == "ENEMY_STATUE_01" ||
                   stableId == "ENEMY_WRAITH_04";
        }

        private static bool IsFloorThreeNormal(string stableId)
        {
            return stableId == "ENEMY_MERCENARY_ASSASSIN_01" ||
                   stableId == "ENEMY_MERCENARY_MAGE_01" ||
                   stableId == "ENEMY_EMPTY_ARMOR";
        }

        private static bool IsFloorFourNormal(string stableId)
        {
            return stableId == "ENEMY_SKELETON_HORDE_01" ||
                   stableId == "ENEMY_LIVING_TOMBSTONE_01" ||
                   stableId == "ENEMY_LIVING_ARMOR_LIGHT_01";
        }

        private static bool IsFloorFiveNormal(string stableId)
        {
            return stableId == "ENEMY_MANEATER_JUNGLE_01" ||
                   stableId == "ENEMY_HOMUNCULUS_01";
        }

        private static void ApplyPrototypeBossGateOverride(EncounterData encounter)
        {
            var serialized = new SerializedObject(encounter);
            serialized.FindProperty("floor").intValue = 2;

            var choices = serialized.FindProperty("choices");
            if (choices.arraySize == 0)
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            var effects = choices.GetArrayElementAtIndex(0).FindPropertyRelative("effects");
            if (effects.arraySize == 0)
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            var handoff = effects.GetArrayElementAtIndex(0).FindPropertyRelative("combatHandoff");
            handoff.FindPropertyRelative("floor").intValue = 2;
            SetStringArray(handoff.FindPropertyRelative("enemyRefs"), new[] { "BOSS_GATE_01" });
            SetBossGatePostCombatEffects(handoff.FindPropertyRelative("onVictoryEffects"));
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplyPrototypeFinalBossOverride(EncounterData encounter)
        {
            var serialized = new SerializedObject(encounter);
            serialized.FindProperty("floor").intValue = 5;

            var choices = serialized.FindProperty("choices");
            if (choices.arraySize == 0)
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            var effects = choices.GetArrayElementAtIndex(0).FindPropertyRelative("effects");
            if (effects.arraySize == 0)
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            var handoff = effects.GetArrayElementAtIndex(0).FindPropertyRelative("combatHandoff");
            handoff.FindPropertyRelative("floor").intValue = 5;
            SetStringArray(handoff.FindPropertyRelative("enemyRefs"), new[] { "BOSS_APEX_02" });
            SetFinalBossPostCombatEffects(handoff.FindPropertyRelative("onVictoryEffects"));
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplyPrototypeShopBuildSurfaceOverride(EncounterData encounter)
        {
            var serialized = new SerializedObject(encounter);
            var choices = serialized.FindProperty("choices");
            for (var i = 0; i < choices.arraySize; i++)
            {
                var choice = choices.GetArrayElementAtIndex(i);
                var stableId = choice.FindPropertyRelative("stableId").stringValue;
                var effects = choice.FindPropertyRelative("effects");
                ApplyShopChoiceRef(encounter.Id, stableId, effects);
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ApplyShopChoiceRef(string encounterId, string stableId, SerializedProperty effects)
        {
            if (stableId.EndsWith("_BUY_ITEM"))
            {
                SetEffectRef(effects, "AddItem", "itemRef", ResolveShopItemRef(encounterId, 0));
            }
            else if (stableId.EndsWith("_BUY_OIL"))
            {
                SetEffectRef(effects, "AddItem", "itemRef", ResolveShopItemRef(encounterId, 1));
            }
            else if (stableId.EndsWith("_BUY_CHARM"))
            {
                SetEffectRef(effects, "AddItem", "itemRef", ResolveShopItemRef(encounterId, 2));
            }
            else if (stableId.EndsWith("_BUY_ABILITY"))
            {
                SetEffectRef(effects, "AddAbility", "abilityRef", ResolveShopAbilityRef(encounterId, 0));
            }
            else if (stableId.EndsWith("_BUY_RECALL"))
            {
                SetEffectRef(effects, "AddAbility", "abilityRef", ResolveShopAbilityRef(encounterId, 1));
            }
        }

        private static string ResolveShopItemRef(string encounterId, int slot)
        {
            var refs = encounterId switch
            {
                "ENC_SHOP_01" => new[] { "ITEM_01", "ITEM_02", "ITEM_03" },
                "ENC_F02_SHOP_001" => new[] { "ITEM_04", "ITEM_05", "ITEM_09" },
                "ENC_SHOP_02" => new[] { "ITEM_04", "ITEM_05", "ITEM_09" },
                "ENC_SHOP_03" => new[] { "ITEM_10", "ITEM_01", "ITEM_02" },
                "ENC_SHOP_04" => new[] { "ITEM_03", "ITEM_04", "ITEM_05" },
                _ => new[] { "ITEM_09", "ITEM_10", "ITEM_01" }
            };
            return refs[slot];
        }

        private static string ResolveShopAbilityRef(string encounterId, int slot)
        {
            var refs = encounterId switch
            {
                "ENC_SHOP_01" => new[] { "ABILITY_SWORD_01", "ABILITY_ARTS_03" },
                "ENC_F02_SHOP_001" => new[] { "ABILITY_SWORD_02", "ABILITY_GUARD_01" },
                "ENC_SHOP_02" => new[] { "ABILITY_SWORD_02", "ABILITY_GUARD_01" },
                "ENC_SHOP_03" => new[] { "ABILITY_SWORD_03", "ABILITY_ARTS_03" },
                "ENC_SHOP_04" => new[] { "ABILITY_SWORD_03", "ABILITY_GUARD_01" },
                _ => new[] { "ABILITY_ARTS_03", "ABILITY_SWORD_01" }
            };
            return refs[slot];
        }

        private static void SetEffectRef(SerializedProperty effects, string kind, string propertyName, string stableId)
        {
            for (var i = 0; i < effects.arraySize; i++)
            {
                var effect = effects.GetArrayElementAtIndex(i);
                if (effect.FindPropertyRelative("kind").stringValue == kind)
                {
                    effect.FindPropertyRelative(propertyName).stringValue = stableId;
                    return;
                }
            }
        }

        private static void SetBossGatePostCombatEffects(SerializedProperty property)
        {
            if (property.arraySize < 3)
            {
                property.arraySize = 3;
            }

            SetPostCombatEffect(property.GetArrayElementAtIndex(0), "ModifyGold", 16);
            SetPostCombatEffect(property.GetArrayElementAtIndex(1), "ModifyGlitchLevel", -4);
            SetPostCombatEffect(property.GetArrayElementAtIndex(2), "ModifyAffinity", 4);
        }

        private static void SetFinalBossPostCombatEffects(SerializedProperty property)
        {
            if (property.arraySize < 4)
            {
                property.arraySize = 4;
            }

            SetPostCombatEffect(property.GetArrayElementAtIndex(0), "ModifyGold", 30);
            SetPostCombatEffect(property.GetArrayElementAtIndex(1), "ModifyGlitchLevel", -8);
            SetPostCombatEffect(property.GetArrayElementAtIndex(2), "ModifyAffinity", 6);
            SetPostCombatFlag(property.GetArrayElementAtIndex(3), "FLAG_FINAL_BOSS_VICTORY");
        }

        private static void SetPostCombatEffect(SerializedProperty element, string kind, int amount)
        {
            element.FindPropertyRelative("kind").stringValue = kind;
            element.FindPropertyRelative("amount").intValue = amount;
            element.FindPropertyRelative("flag").stringValue = string.Empty;
            element.FindPropertyRelative("value").boolValue = false;
            element.FindPropertyRelative("itemRef").stringValue = string.Empty;
            element.FindPropertyRelative("count").intValue = 0;
            element.FindPropertyRelative("abilityRef").stringValue = string.Empty;
            element.FindPropertyRelative("rewardBundleRef").stringValue = string.Empty;
            element.FindPropertyRelative("memoryFragmentId").stringValue = string.Empty;
            element.FindPropertyRelative("memoryFragmentTextKey").stringValue = string.Empty;
            element.FindPropertyRelative("npcStage").stringValue = string.Empty;
            element.FindPropertyRelative("targetEncounterId").stringValue = string.Empty;
            element.FindPropertyRelative("targetNodeId").stringValue = string.Empty;
        }

        private static void SetPostCombatFlag(SerializedProperty element, string flag)
        {
            SetPostCombatEffect(element, "SetFlag", 0);
            element.FindPropertyRelative("flag").stringValue = flag;
            element.FindPropertyRelative("value").boolValue = true;
        }

        private static void ApplyRewardBundle(RewardBundleData rewardBundle, string stableId)
        {
            var serialized = new SerializedObject(rewardBundle);
            serialized.FindProperty("stableId").stringValue = stableId;
            var entries = serialized.FindProperty("entries");
            entries.arraySize = 1;
            var entry = entries.GetArrayElementAtIndex(0);
            entry.FindPropertyRelative("itemRef").stringValue = stableId == "REWARD_CACHE_MEMORY" ? "ITEM_09" : "ITEM_01";
            entry.FindPropertyRelative("itemCount").intValue = 1;
            entry.FindPropertyRelative("abilityRef").stringValue = string.Empty;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(rewardBundle);
        }

        private static void ApplyMemoryFragment(MemoryFragmentData memoryFragment, string stableId, string stage)
        {
            var serialized = new SerializedObject(memoryFragment);
            serialized.FindProperty("stableId").stringValue = stableId;
            serialized.FindProperty("titleKey").stringValue = "PLACEHOLDER_" + stableId + "_TITLE";
            serialized.FindProperty("bodyKey").stringValue = "PLACEHOLDER_" + stableId + "_BODY";
            serialized.FindProperty("stage").stringValue = stage;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(memoryFragment);
        }

        private static void ApplyCatalog(
            EncounterRuntimeCatalogData catalog,
            ItemData[] items,
            RewardBundleData[] rewardBundles,
            AbilityData[] abilities,
            SynergyData[] synergies,
            EnemyData[] enemies,
            MemoryFragmentData[] memoryFragments)
        {
            var serialized = new SerializedObject(catalog);
            SetObjectArray(serialized.FindProperty("items"), items);
            SetObjectArray(serialized.FindProperty("rewardBundles"), rewardBundles);
            SetObjectArray(serialized.FindProperty("abilities"), abilities);
            SetObjectArray(serialized.FindProperty("synergies"), synergies);
            SetObjectArray(serialized.FindProperty("enemies"), enemies);
            SetObjectArray(serialized.FindProperty("memoryFragments"), memoryFragments);
            serialized.FindProperty("floorEnemyPools").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<FloorEnemyPoolData>(FloorEnemyPoolPath);
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void SetObjectArray<T>(SerializedProperty property, T[] values)
            where T : Object
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }

        private static void SetStringArray(SerializedProperty property, string[] values)
        {
            property.arraySize = values == null ? 0 : values.Length;
            for (var i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i] ?? string.Empty;
            }
        }

        private readonly struct NumericParamSpec
        {
            public NumericParamSpec(string key, float value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; }
            public float Value { get; }
        }

        private static void SetNumericParams(SerializedProperty property, NumericParamSpec[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("key").stringValue = values[i].Key;
                element.FindPropertyRelative("value").floatValue = values[i].Value;
            }
        }

        private static void EnsureFormalFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "Items");
            EnsureFolder("Assets/_Project/Data", "Rewards");
            EnsureFolder("Assets/_Project/Data", "Abilities");
            EnsureFolder("Assets/_Project/Data", "Synergies");
            EnsureFolder("Assets/_Project/Data", "Enemies");
            EnsureFolder("Assets/_Project/Data", "Catalogs");
            EnsureFolder("Assets/_Project/Data", "MemoryFragments");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void TrimTrailingWhitespace(IReadOnlyList<string> assetPaths)
        {
            for (var i = 0; i < assetPaths.Count; i++)
            {
                var path = assetPaths[i];
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    continue;
                }

                var lines = File.ReadAllLines(path);
                var changed = false;
                for (var line = 0; line < lines.Length; line++)
                {
                    var trimmed = lines[line].TrimEnd(' ', '\t');
                    if (trimmed != lines[line])
                    {
                        lines[line] = trimmed;
                        changed = true;
                    }
                }

                if (changed)
                {
                    File.WriteAllLines(path, lines);
                }
            }
        }
    }
}
#endif
