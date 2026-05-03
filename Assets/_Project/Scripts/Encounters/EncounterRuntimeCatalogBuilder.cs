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

        private static readonly string[] ItemIds =
        {
            "ITEM_FIELD_BANDAGE",
            "ITEM_LANTERN_OIL",
            "ITEM_TORN_CHARM"
        };

        private static readonly string[] AbilityIds =
        {
            "ABILITY_SCOUT",
            "ABILITY_RECALL_ANCHOR"
        };

        private static readonly string[] RewardBundleIds =
        {
            "REWARD_CACHE_MEMORY",
            "REWARD_CACHE_SMALL"
        };

        private static readonly string[] EnemyIds =
        {
            "ENEMY_COLLAPSE_ECHO",
            "ENEMY_EMPTY_ARMOR",
            "ENEMY_FRACTURE_HOUND"
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

            var enemies = new EnemyData[EnemyIds.Length];
            for (var i = 0; i < EnemyIds.Length; i++)
            {
                var path = "Assets/_Project/Data/Enemies/SO_Enemy_" + EnemyIds[i] + ".asset";
                enemies[i] = EnsureAsset<EnemyData>(path, result);
                ApplyEnemy(enemies[i], EnemyIds[i]);
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
            ApplyCatalog(catalog, items, rewardBundles, abilities, enemies, memoryFragments);
            result.SetCatalog(catalog);

            AssetDatabase.SaveAssets();
            TrimTrailingWhitespace(result.AssetPaths);
            AssetDatabase.Refresh();
            return result;
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
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(item);
        }

        private static void ApplyAbility(AbilityData ability, string stableId)
        {
            var serialized = new SerializedObject(ability);
            serialized.FindProperty("id").stringValue = stableId;
            serialized.FindProperty("displayName").stringValue = "PLACEHOLDER_" + stableId + "_NAME";
            serialized.FindProperty("tag").stringValue = string.Empty;
            serialized.FindProperty("description").stringValue = "PLACEHOLDER_" + stableId + "_DESC";
            serialized.FindProperty("numericParams").arraySize = 0;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ability);
        }

        private static void ApplyEnemy(EnemyData enemy, string stableId)
        {
            var serialized = new SerializedObject(enemy);
            serialized.FindProperty("id").stringValue = stableId;
            serialized.FindProperty("hp").intValue = 12;
            serialized.FindProperty("attack").intValue = 3;
            serialized.FindProperty("patternId").stringValue = "PATTERN_PLACEHOLDER";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(enemy);
        }

        private static void ApplyRewardBundle(RewardBundleData rewardBundle, string stableId)
        {
            var serialized = new SerializedObject(rewardBundle);
            serialized.FindProperty("stableId").stringValue = stableId;
            var entries = serialized.FindProperty("entries");
            entries.arraySize = 1;
            var entry = entries.GetArrayElementAtIndex(0);
            entry.FindPropertyRelative("itemRef").stringValue = stableId == "REWARD_CACHE_MEMORY" ? "ITEM_TORN_CHARM" : "ITEM_FIELD_BANDAGE";
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
            EnemyData[] enemies,
            MemoryFragmentData[] memoryFragments)
        {
            var serialized = new SerializedObject(catalog);
            SetObjectArray(serialized.FindProperty("items"), items);
            SetObjectArray(serialized.FindProperty("rewardBundles"), rewardBundles);
            SetObjectArray(serialized.FindProperty("abilities"), abilities);
            SetObjectArray(serialized.FindProperty("enemies"), enemies);
            SetObjectArray(serialized.FindProperty("memoryFragments"), memoryFragments);
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

        private static void EnsureFormalFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "Items");
            EnsureFolder("Assets/_Project/Data", "Rewards");
            EnsureFolder("Assets/_Project/Data", "Abilities");
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
