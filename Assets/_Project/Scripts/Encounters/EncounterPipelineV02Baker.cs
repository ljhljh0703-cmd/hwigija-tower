#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.Encounters
{
    public sealed class EncounterBakeResult
    {
        private readonly List<string> _assetPaths = new List<string>();

        public IReadOnlyList<string> AssetPaths => _assetPaths;
        public EncounterValidationResult Validation { get; private set; }
        public bool Success => Validation != null && Validation.IsValid;

        public void SetValidation(EncounterValidationResult validation)
        {
            Validation = validation;
        }

        public void AddAssetPath(string path)
        {
            _assetPaths.Add(path);
        }
    }

    public static class EncounterPipelineV02Baker
    {
        public const string ExternalSamplePackPath = "/Users/godju/Downloads/외주 폴더/pack_PACK_SAMPLE_3_V002.json";

        [MenuItem("Hwigi Tower/Encounter Pipeline v0.2/Bake External Sample Pack")]
        public static void BakeExternalSamplePack()
        {
            var result = BakeJsonFile(ExternalSamplePackPath);
            if (!result.Success)
            {
                Debug.LogError(result.Validation == null ? "Encounter bake failed." : result.Validation.ToSummary());
                return;
            }

            Debug.Log("Baked encounter assets: " + result.AssetPaths.Count);
        }

        public static EncounterBakeResult BakeJsonFile(string path)
        {
            return BakeJson(File.ReadAllText(path));
        }

        public static EncounterBakeResult BakeJson(string json)
        {
            var result = new EncounterBakeResult();
            var validation = EncounterPipelineV02Validator.ValidateJson(json);
            result.SetValidation(validation);
            if (!validation.IsValid)
            {
                return result;
            }

            EnsureFormalFolders();
            object root;
            var parseResult = new EncounterValidationResult();
            if (!EncounterPipelineV02Validator.TryParse(json, parseResult, out root))
            {
                result.SetValidation(parseResult);
                return result;
            }

            var pack = root as Dictionary<string, object>;
            if (pack == null || !pack.TryGetValue("encounters", out var encountersValue) || !(encountersValue is List<object> encounters))
            {
                result.SetValidation(Failed("PACK_DESERIALIZE_FAILED", "$.encounters", "Pack encounters could not deserialize."));
                return result;
            }

            for (var i = 0; i < encounters.Count; i++)
            {
                var encounter = encounters[i] as Dictionary<string, object>;
                var stableId = AsString(encounter, "stableId");
                if (string.IsNullOrEmpty(stableId))
                {
                    continue;
                }

                var assetPath = ResolveAssetPath(encounter);
                var asset = AssetDatabase.LoadAssetAtPath<EncounterData>(assetPath);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<EncounterData>();
                    AssetDatabase.CreateAsset(asset, assetPath);
                }

                ApplyEncounter(asset, encounter, assetPath);
                EditorUtility.SetDirty(asset);
                result.AddAssetPath(assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return result;
        }

        private static EncounterValidationResult Failed(string code, string path, string message)
        {
            var result = new EncounterValidationResult();
            result.Add(code, path, message);
            return result;
        }

        private static void EnsureFormalFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "Encounters");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static string ResolveAssetPath(Dictionary<string, object> encounter)
        {
            var stableId = AsString(encounter, "stableId");
            var existing = FindExistingByStableId(stableId);
            if (!string.IsNullOrEmpty(existing))
            {
                return existing;
            }

            var assetPath = AsString(encounter, "assetPath");
            if (!string.IsNullOrEmpty(assetPath))
            {
                return assetPath;
            }

            return EncounterPipelineV02Validator.BasePath + "SO_Encounter_" + stableId + ".asset";
        }

        private static string FindExistingByStableId(string stableId)
        {
            var guids = AssetDatabase.FindAssets("t:EncounterData", new[] { EncounterPipelineV02Validator.BasePath.TrimEnd('/') });
            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<EncounterData>(path);
                if (asset != null && asset.Id == stableId)
                {
                    return path;
                }
            }

            return string.Empty;
        }

        private static void ApplyEncounter(EncounterData asset, Dictionary<string, object> encounter, string assetPath)
        {
            var serialized = new SerializedObject(asset);
            SetString(serialized, "id", AsString(encounter, "stableId"));
            SetEnum(serialized, "type", (int)MapEncounterType(AsString(encounter, "encounterType")));
            SetInt(serialized, "floor", AsInt(encounter, "floor", 0));
            SetInt(serialized, "weight", 1);
            SetString(serialized, "deterministicSeedKey", AsString(encounter, "seedKey"));
            SetString(serialized, "assetPath", assetPath);
            SetString(serialized, "sourceHash", AsString(encounter, "sourceHash"));
            SetString(serialized, "contentCategory", AsString(encounter, "contentCategory"));
            SetString(serialized, "npcStage", AsString(encounter, "npcStage"));
            SetString(serialized, "bodyTextKey", AsString(encounter, "bodyTextKey"));
            var writerStatus = AsString(encounter, "writerStatus");
            SetString(serialized, "writerStatus", string.IsNullOrEmpty(writerStatus) ? "placeholder" : writerStatus);
            SetInt(serialized, "glitchLevel", AsInt(encounter, "glitchLevel", 0));
            var nodeRules = encounter.TryGetValue("nodeRules", out var nodeRulesValue) ? nodeRulesValue as Dictionary<string, object> : null;
            SetInt(serialized, "timeLimitSeconds", AsInt(nodeRules, "timeLimitSeconds", 0));
            SetStringArray(serialized, "choiceStableIds", GetChoiceStableIds(encounter));
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static EncounterType MapEncounterType(string type)
        {
            switch (type)
            {
                case "CombatGate":
                    return EncounterType.Battle;
                case "Rest":
                    return EncounterType.Rest;
                case "Shop":
                    return EncounterType.Shop;
                case "Remnant":
                    return EncounterType.Remnant;
                case "MoralChoice":
                    return EncounterType.MoralChoice;
                case "MemoryFragment":
                    return EncounterType.MemoryFragment;
                case "Story":
                    return EncounterType.Story;
                case "MetaText":
                    return EncounterType.MetaText;
                default:
                    return EncounterType.Story;
            }
        }

        private static string[] GetChoiceStableIds(Dictionary<string, object> encounter)
        {
            if (encounter == null || !encounter.TryGetValue("choices", out var choicesValue) || !(choicesValue is List<object> choices))
            {
                return new string[0];
            }

            var ids = new string[choices.Count];
            for (var i = 0; i < choices.Count; i++)
            {
                ids[i] = choices[i] is Dictionary<string, object> choice ? AsString(choice, "stableId") : string.Empty;
            }

            return ids;
        }

        private static string AsString(Dictionary<string, object> map, string key)
        {
            return map != null && map.TryGetValue(key, out var value) && value != null ? value.ToString() : string.Empty;
        }

        private static int AsInt(Dictionary<string, object> map, string key, int fallback)
        {
            if (map == null || !map.TryGetValue(key, out var value) || value == null)
            {
                return fallback;
            }

            if (value is int intValue)
            {
                return intValue;
            }

            if (value is long longValue)
            {
                return (int)longValue;
            }

            if (value is double doubleValue)
            {
                return (int)doubleValue;
            }

            return int.TryParse(value.ToString(), out var parsed) ? parsed : fallback;
        }

        private static void SetString(SerializedObject serialized, string propertyName, string value)
        {
            serialized.FindProperty(propertyName).stringValue = value ?? string.Empty;
        }

        private static void SetInt(SerializedObject serialized, string propertyName, int value)
        {
            serialized.FindProperty(propertyName).intValue = value;
        }

        private static void SetEnum(SerializedObject serialized, string propertyName, int value)
        {
            serialized.FindProperty(propertyName).enumValueIndex = value;
        }

        private static void SetStringArray(SerializedObject serialized, string propertyName, string[] values)
        {
            var property = serialized.FindProperty(propertyName);
            property.arraySize = values == null ? 0 : values.Length;
            for (var i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i] ?? string.Empty;
            }
        }
    }
}
#endif
