using System.Collections.Generic;
using System.IO;
using HwigiTower.Combat;
using HwigiTower.Encounters;
using HwigiTower.UI;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.EditorTools
{
    public static class FloorEnemyCatalogBuilder
    {
        private const string EnemyFolder = "Assets/_Project/Data/Enemies";
        private const string ArtFolder = "Assets/_Project/Art/Enemies";
        private const string PoolPath = "Assets/_Project/Data/Enemies/SO_FloorEnemyPool_v0_1.asset";
        private const string CatalogPath = "Assets/_Project/Data/Catalogs/SO_EncounterRuntimeCatalog.asset";
        private const string PresentationPath = "Assets/_Project/Data/Presentation/SO_DemoPresentationData.asset";

        private sealed class EnemySpec
        {
            public EnemySpec(int floor, string id, string rank, string koreanName, string concept, string assetFilename, int hp, int attack, int goldReward)
            {
                Floor = floor;
                Id = id;
                Rank = rank;
                KoreanName = koreanName;
                Concept = concept;
                AssetFilename = assetFilename;
                Hp = hp;
                Attack = attack;
                GoldReward = goldReward;
            }

            public int Floor { get; }
            public string Id { get; }
            public string Rank { get; }
            public string KoreanName { get; }
            public string Concept { get; }
            public string AssetFilename { get; }
            public int Hp { get; }
            public int Attack { get; }
            public int GoldReward { get; }
        }

        public static void BuildV01()
        {
            Directory.CreateDirectory(EnemyFolder);
            var specs = BuildSpecs();
            AssetDatabase.Refresh();
            ImportSprites(specs);
            var enemies = EnsureEnemies(specs);
            var pool = EnsureFloorPool(specs);
            UpdateCatalog(enemies, pool);
            UpdatePresentation(specs);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Floor enemy catalog v0.1 built: " + specs.Length + " specs");
        }

        private static EnemySpec[] BuildSpecs()
        {
            return new[]
            {
                new EnemySpec(1, "ENEMY_BANDIT_MELEE_01", "normal", "도적 검수", "근거리 인간형 적", "enemy_bandit_melee_01.png", 10, 3, 3),
                new EnemySpec(1, "ENEMY_BANDIT_RANGED_01", "normal", "도적 궁수", "원거리 인간형 적", "enemy_bandit_ranged_01.png", 9, 3, 3),
                new EnemySpec(1, "ENEMY_SLIME_01", "normal", "슬라임", "기초 생물형 적", "enemy_slime_01.png", 8, 2, 2),
                new EnemySpec(1, "ENEMY_SKELETON_01", "normal", "스켈레톤", "기초 언데드 적", "enemy_skeleton_01.png", 10, 3, 3),
                new EnemySpec(1, "ENEMY_WILD_BEAST_01", "normal", "야생 짐승", "기초 야수형 적", "enemy_wild_beast_01.png", 11, 3, 3),
                new EnemySpec(1, "ENEMY_SLIME_POOL_01", "elite", "슬라임 무리", "층 1 엘리트 생물형 적", "enemy_slime_pool_01.png", 16, 4, 6),
                new EnemySpec(1, "ENEMY_WILD_BEAST_PACK_01", "elite", "야생 짐승 무리", "층 1 엘리트 야수형 적", "enemy_wild_beast_pack_01.png", 18, 4, 6),
                new EnemySpec(1, "ENEMY_STATUE_01", "boss", "움직이는 석상", "층 1 보스 placeholder", "enemy_statue_01.png", 20, 4, 10),
                new EnemySpec(2, "ENEMY_FRACTURE_HOUND", "normal", "변이된 들짐승", "균열에 오염된 야수", "enemy_fracture_hound_demo.png", 12, 3, 4),
                new EnemySpec(2, "ENEMY_MANEATER_PLANT_01", "normal", "식인 식물", "층 2 식물형 적", "enemy_maneater_plant_01.png", 13, 3, 4),
                new EnemySpec(2, "ENEMY_MERCENARY_MELEE_01", "normal", "용병 검수", "근거리 용병", "enemy_mercenary_melee_01.png", 13, 3, 4),
                new EnemySpec(2, "ENEMY_MERCENARY_RANGED_01", "normal", "용병 사수", "원거리 용병", "enemy_mercenary_ranged_01.png", 12, 3, 4),
                new EnemySpec(2, "ENEMY_IRON_MAIDEN_01", "elite", "아이언 메이든", "층 2 엘리트 장치형 적", "enemy_iron_maiden_01.png", 20, 4, 8),
                new EnemySpec(2, "BOSS_GATE_01", "boss", "문지기", "층 2 보스", "enemy_empty_armor.png", 28, 4, 16),
                new EnemySpec(3, "ENEMY_MERCENARY_ASSASSIN_01", "normal", "용병 암살자", "층 3 속공형 용병", "enemy_mercenary_assassin_01.png", 14, 4, 5),
                new EnemySpec(3, "ENEMY_MERCENARY_MAGE_01", "normal", "용병 마도사", "층 3 마법형 용병", "enemy_mercenary_mage_01.png", 13, 4, 5),
                new EnemySpec(3, "ENEMY_EMPTY_ARMOR", "normal", "리빙아머 중무장", "중무장 갑옷형 적", "enemy_empty_armor.png", 16, 4, 5),
                new EnemySpec(3, "ENEMY_MERCENARY_CAPTAIN_SAGAN_01", "elite", "용병대장 사간", "층 3 엘리트 지휘관", "enemy_mercenary_captain_sagan_01.png", 24, 5, 10),
                new EnemySpec(3, "ENEMY_COLLAPSE_ECHO", "boss", "스펙터", "collapse echo", "enemy_collapse_echo.png", 26, 4, 14),
                new EnemySpec(4, "ENEMY_SKELETON_HORDE_01", "normal", "스켈레톤 군집", "층 4 언데드 무리", "enemy_skeleton_horde_01.png", 16, 4, 5),
                new EnemySpec(4, "ENEMY_LIVING_TOMBSTONE_01", "normal", "살아있는 묘비", "층 4 묘비형 적", "enemy_living_tombstone_01.png", 17, 4, 5),
                new EnemySpec(4, "ENEMY_LIVING_ARMOR_LIGHT_01", "normal", "리빙아머 경장", "층 4 경장 갑옷형 적", "enemy_living_armor_light_01.png", 15, 4, 5),
                new EnemySpec(4, "ENEMY_SHADE_03", "elite", "금기를 엿본 자", "층 4 엘리트 그림자", "enemy_shade_03.png", 22, 5, 10),
                new EnemySpec(4, "ENEMY_WRAITH_04", "boss", "진리를 엿본 자", "층 4 보스 placeholder", "enemy_wraith_04.png", 12, 2, 18),
                new EnemySpec(5, "ENEMY_MANEATER_JUNGLE_01", "normal", "밀림 식인수", "층 5 식물형 적", "enemy_maneater_jungle_01.png", 18, 4, 6),
                new EnemySpec(5, "ENEMY_HOMUNCULUS_01", "normal", "호문쿨루스", "층 5 조형 생명체", "enemy_homunculus_01.png", 18, 5, 6),
                new EnemySpec(5, "ENEMY_COLLAPSE_ECHO", "elite", "스펙터", "층 5 엘리트 collapse echo", "enemy_collapse_echo.png", 26, 4, 14),
                new EnemySpec(5, "BOSS_APEX_02", "boss", "최종 보스", "final boss", "enemy_boss_apex_02.png", 30, 4, 30)
            };
        }

        private static void ImportSprites(IEnumerable<EnemySpec> specs)
        {
            foreach (var spec in specs)
            {
                var path = GetArtPath(spec);
                if (!File.Exists(path))
                {
                    continue;
                }

                AssetDatabase.ImportAsset(path);
                if (AssetImporter.GetAtPath(path) is TextureImporter importer)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaIsTransparency = true;
                    importer.mipmapEnabled = false;
                    importer.SaveAndReimport();
                }
            }
        }

        private static EnemyData[] EnsureEnemies(IReadOnlyList<EnemySpec> specs)
        {
            var enemies = new List<EnemyData>();
            var seen = new HashSet<string>();
            foreach (var spec in specs)
            {
                if (!seen.Add(spec.Id))
                {
                    continue;
                }

                var path = GetEnemyAssetPath(spec.Id);
                var enemy = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
                if (enemy == null)
                {
                    enemy = ScriptableObject.CreateInstance<EnemyData>();
                    AssetDatabase.CreateAsset(enemy, path);
                    ApplyEnemyDefaults(enemy, spec);
                }

                enemies.Add(enemy);
            }

            var guids = AssetDatabase.FindAssets("t:EnemyData", new[] { EnemyFolder });
            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var enemy = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
                if (enemy != null && seen.Add(enemy.Id))
                {
                    enemies.Add(enemy);
                }
            }

            return enemies.ToArray();
        }

        private static void ApplyEnemyDefaults(EnemyData enemy, EnemySpec spec)
        {
            var serialized = new SerializedObject(enemy);
            serialized.FindProperty("id").stringValue = spec.Id;
            serialized.FindProperty("hp").intValue = spec.Hp;
            serialized.FindProperty("attack").intValue = spec.Attack;
            serialized.FindProperty("patternId").stringValue = spec.Rank == "normal" ? "PATTERN_BASIC" : "PATTERN_ELITE";
            serialized.FindProperty("pattern").objectReferenceValue = AssetDatabase.LoadAssetAtPath<EnemyPatternData>(
                spec.Rank == "normal"
                    ? "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_BASIC.asset"
                    : "Assets/_Project/Data/EnemyPatterns/SO_EnemyPattern_PATTERN_ELITE.asset");
            serialized.FindProperty("goldReward").intValue = spec.GoldReward;
            serialized.FindProperty("xpReward").intValue = spec.Rank == "boss" ? spec.GoldReward * 3 : spec.GoldReward;
            serialized.FindProperty("glitchDelta").intValue = spec.Rank == "boss" ? -3 : 0;
            serialized.FindProperty("affinityDelta").intValue = spec.Rank == "boss" ? 2 : 0;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(enemy);
        }

        private static FloorEnemyPoolData EnsureFloorPool(IReadOnlyList<EnemySpec> specs)
        {
            var pool = AssetDatabase.LoadAssetAtPath<FloorEnemyPoolData>(PoolPath);
            if (pool == null)
            {
                pool = ScriptableObject.CreateInstance<FloorEnemyPoolData>();
                AssetDatabase.CreateAsset(pool, PoolPath);
            }

            var serialized = new SerializedObject(pool);
            var floors = serialized.FindProperty("floors");
            floors.arraySize = 5;
            for (var floor = 1; floor <= 5; floor++)
            {
                var property = floors.GetArrayElementAtIndex(floor - 1);
                property.FindPropertyRelative("floor").intValue = floor;
                SetStringArray(property.FindPropertyRelative("normalEnemyRefs"), EnemyRefs(specs, floor, "normal"));
                SetStringArray(property.FindPropertyRelative("eliteEnemyRefs"), EnemyRefs(specs, floor, "elite"));
                SetStringArray(property.FindPropertyRelative("bossEnemyRefs"), EnemyRefs(specs, floor, "boss"));
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pool);
            return pool;
        }

        private static void UpdateCatalog(EnemyData[] enemies, FloorEnemyPoolData pool)
        {
            var catalog = AssetDatabase.LoadAssetAtPath<EncounterRuntimeCatalogData>(CatalogPath);
            var serialized = new SerializedObject(catalog);
            var property = serialized.FindProperty("enemies");
            property.arraySize = enemies.Length;
            for (var i = 0; i < enemies.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = enemies[i];
            }

            serialized.FindProperty("floorEnemyPools").objectReferenceValue = pool;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
        }

        private static void UpdatePresentation(IReadOnlyList<EnemySpec> specs)
        {
            var data = AssetDatabase.LoadAssetAtPath<DemoPresentationData>(PresentationPath);
            var serialized = new SerializedObject(data);
            var slots = serialized.FindProperty("slots");
            var byId = new Dictionary<string, int>();
            for (var i = 0; i < slots.arraySize; i++)
            {
                var slot = slots.GetArrayElementAtIndex(i);
                byId[slot.FindPropertyRelative("encounterStableId").stringValue] = i;
            }

            foreach (var spec in specs)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(GetArtPath(spec));
                if (sprite == null)
                {
                    continue;
                }

                if (!byId.TryGetValue(spec.Id, out var index))
                {
                    index = slots.arraySize;
                    slots.arraySize++;
                    byId[spec.Id] = index;
                }

                var slot = slots.GetArrayElementAtIndex(index);
                slot.FindPropertyRelative("encounterStableId").stringValue = spec.Id;
                slot.FindPropertyRelative("displayName").stringValue = spec.KoreanName;
                slot.FindPropertyRelative("backgroundSprite").objectReferenceValue = null;
                slot.FindPropertyRelative("titleTextKey").stringValue = string.Empty;
                slot.FindPropertyRelative("bodyTextKey").stringValue = string.Empty;
                slot.FindPropertyRelative("enemySprite").objectReferenceValue = sprite;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(data);
        }

        private static string[] EnemyRefs(IEnumerable<EnemySpec> specs, int floor, string rank)
        {
            var refs = new List<string>();
            var seen = new HashSet<string>();
            foreach (var spec in specs)
            {
                if (spec.Floor == floor && spec.Rank == rank && seen.Add(spec.Id))
                {
                    refs.Add(spec.Id);
                }
            }

            return refs.ToArray();
        }

        private static void SetStringArray(SerializedProperty property, string[] values)
        {
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i];
            }
        }

        private static string GetEnemyAssetPath(string stableId)
        {
            return EnemyFolder + "/SO_Enemy_" + stableId + ".asset";
        }

        private static string GetArtPath(EnemySpec spec)
        {
            return ArtFolder + "/" + spec.AssetFilename;
        }
    }
}
