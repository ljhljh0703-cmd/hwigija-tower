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
        private readonly List<string> _createdAssetPaths = new List<string>();
        private readonly List<string> _updatedAssetPaths = new List<string>();
        private readonly List<string> _reports = new List<string>();

        public IReadOnlyList<string> AssetPaths => _assetPaths;
        public IReadOnlyList<string> CreatedAssetPaths => _createdAssetPaths;
        public IReadOnlyList<string> UpdatedAssetPaths => _updatedAssetPaths;
        public IReadOnlyList<string> Reports => _reports;
        public EncounterValidationResult Validation { get; private set; }
        public bool Success => Validation != null && Validation.IsValid;
        public bool DryRun { get; private set; }

        public void SetValidation(EncounterValidationResult validation)
        {
            Validation = validation;
        }

        public void SetDryRun(bool dryRun)
        {
            DryRun = dryRun;
        }

        public void AddAssetPath(string path)
        {
            _assetPaths.Add(path);
        }

        public void AddCreatedAssetPath(string path)
        {
            _createdAssetPaths.Add(path);
        }

        public void AddUpdatedAssetPath(string path)
        {
            _updatedAssetPaths.Add(path);
        }

        public void AddReport(string code, string stableId, string details)
        {
            _reports.Add((code ?? string.Empty) + "|" + (stableId ?? string.Empty) + "|" + (details ?? string.Empty));
        }

        public string ToSummary()
        {
            return "validation=" + (Validation == null ? "none" : Validation.ToSummary()) +
                "; dryRun=" + DryRun +
                "; assets=" + AssetPaths.Count +
                "; created=" + CreatedAssetPaths.Count +
                "; updated=" + UpdatedAssetPaths.Count +
                "; reports=" + Reports.Count;
        }
    }

    public readonly struct EncounterBakeOptions
    {
        public EncounterBakeOptions(bool dryRun)
        {
            DryRun = dryRun;
        }

        public bool DryRun { get; }

        public static EncounterBakeOptions Apply => new EncounterBakeOptions(false);
        public static EncounterBakeOptions DryRunOnly => new EncounterBakeOptions(true);
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

            Debug.Log("Encounter bake v0.2: " + result.ToSummary());
        }

        public static EncounterBakeResult BakeJsonFile(string path)
        {
            return BakeJsonFile(path, EncounterBakeOptions.Apply);
        }

        public static EncounterBakeResult BakeJsonFile(string path, EncounterBakeOptions options)
        {
            return BakeJson(File.ReadAllText(path), options);
        }

        public static EncounterBakeResult BakeJson(string json)
        {
            return BakeJson(json, EncounterBakeOptions.Apply);
        }

        public static EncounterBakeResult BakeJson(string json, EncounterBakeOptions options)
        {
            var result = new EncounterBakeResult();
            result.SetDryRun(options.DryRun);
            var validation = EncounterPipelineV02Validator.ValidateJson(json);
            result.SetValidation(validation);
            if (!validation.IsValid)
            {
                return result;
            }

            if (!options.DryRun)
            {
                EnsureFormalFolders();
            }
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
                ReportConflicts(result, encounter, assetPath, asset);
                if (asset == null)
                {
                    result.AddCreatedAssetPath(assetPath);
                    if (!options.DryRun)
                    {
                        asset = ScriptableObject.CreateInstance<EncounterData>();
                        AssetDatabase.CreateAsset(asset, assetPath);
                    }
                }
                else
                {
                    result.AddUpdatedAssetPath(assetPath);
                }

                if (!options.DryRun && asset != null)
                {
                    ApplyEncounter(asset, encounter, assetPath);
                    EditorUtility.SetDirty(asset);
                }

                result.AddAssetPath(assetPath);
            }

            if (!options.DryRun)
            {
                EncounterRuntimeCatalogBuilder.ApplyPrototypeRuntimeOverrides();
                AssetDatabase.SaveAssets();
                TrimTrailingWhitespace(result.AssetPaths);
                AssetDatabase.Refresh();
            }

            Debug.Log("Encounter bake v0.2: " + result.ToSummary());
            return result;
        }

        private static void ReportConflicts(EncounterBakeResult result, Dictionary<string, object> encounter, string resolvedAssetPath, EncounterData resolvedAsset)
        {
            var stableId = AsString(encounter, "stableId");
            var declaredAssetPath = AsString(encounter, "assetPath");
            var sourceHash = AsString(encounter, "sourceHash");

            if (!string.IsNullOrEmpty(declaredAssetPath) && !string.IsNullOrEmpty(resolvedAssetPath) && declaredAssetPath != resolvedAssetPath)
            {
                result.AddReport("STABLE_ID_PATH_CONFLICT", stableId, "declared=" + declaredAssetPath + ";existing=" + resolvedAssetPath);
            }

            var declaredAsset = string.IsNullOrEmpty(declaredAssetPath) ? null : AssetDatabase.LoadAssetAtPath<EncounterData>(declaredAssetPath);
            if (declaredAsset != null && declaredAsset.Id != stableId)
            {
                result.AddReport("ASSET_PATH_STABLE_ID_CONFLICT", stableId, "assetPath=" + declaredAssetPath + ";assetId=" + declaredAsset.Id);
            }

            if (resolvedAsset != null && !string.IsNullOrEmpty(resolvedAsset.SourceHash) && resolvedAsset.SourceHash != sourceHash)
            {
                result.AddReport("SOURCE_HASH_CHANGED", stableId, "old=" + resolvedAsset.SourceHash + ";new=" + sourceHash);
            }
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
            SetRuntimeChoices(serialized, encounter);
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

        private static void SetRuntimeChoices(SerializedObject serialized, Dictionary<string, object> encounter)
        {
            var property = serialized.FindProperty("choices");
            if (encounter == null || !encounter.TryGetValue("choices", out var choicesValue) || !(choicesValue is List<object> choices))
            {
                property.arraySize = 0;
                return;
            }

            property.arraySize = choices.Count;
            for (var i = 0; i < choices.Count; i++)
            {
                var choice = choices[i] as Dictionary<string, object>;
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("stableId").stringValue = AsString(choice, "stableId");
                element.FindPropertyRelative("textKey").stringValue = AsString(choice, "textKey");
                element.FindPropertyRelative("requirementMode").stringValue = AsString(choice, "requirementMode");
                var unavailable = choice != null && choice.TryGetValue("unavailablePolicy", out var unavailableValue)
                    ? unavailableValue as Dictionary<string, object>
                    : null;
                element.FindPropertyRelative("unavailablePolicyMode").stringValue = AsString(unavailable, "mode");
                element.FindPropertyRelative("unavailableReasonTextKey").stringValue = AsString(unavailable, "reasonTextKey");
                var reaction = choice != null && choice.TryGetValue("npcImmediateReaction", out var reactionValue)
                    ? reactionValue as Dictionary<string, object>
                    : null;
                element.FindPropertyRelative("npcReactionKey").stringValue = AsString(reaction, "reactionKey");
                SetRuntimeRequirements(element.FindPropertyRelative("requirements"), choice);
                SetRuntimeEffects(element.FindPropertyRelative("effects"), choice);
            }
        }

        private static void SetRuntimeRequirements(SerializedProperty property, Dictionary<string, object> choice)
        {
            if (choice == null || !choice.TryGetValue("requirements", out var requirementsValue) || !(requirementsValue is List<object> requirements))
            {
                property.arraySize = 0;
                return;
            }

            property.arraySize = requirements.Count;
            for (var i = 0; i < requirements.Count; i++)
            {
                var requirement = requirements[i] as Dictionary<string, object>;
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("kind").stringValue = AsString(requirement, "kind");
                element.FindPropertyRelative("stat").stringValue = AsString(requirement, "stat");
                element.FindPropertyRelative("op").stringValue = AsString(requirement, "op");
                element.FindPropertyRelative("value").intValue = AsInt(requirement, "value", 0);
                element.FindPropertyRelative("flag").stringValue = AsString(requirement, "flag");
                element.FindPropertyRelative("expected").boolValue = AsBool(requirement, "expected", false);
                element.FindPropertyRelative("itemRef").stringValue = AsString(requirement, "itemRef");
                element.FindPropertyRelative("minCount").intValue = AsInt(requirement, "minCount", 0);
                element.FindPropertyRelative("abilityRef").stringValue = AsString(requirement, "abilityRef");
                element.FindPropertyRelative("min").intValue = AsInt(requirement, "min", 0);
                element.FindPropertyRelative("max").intValue = AsInt(requirement, "max", 0);
                element.FindPropertyRelative("minFloor").intValue = AsInt(requirement, "minFloor", 0);
                element.FindPropertyRelative("maxFloor").intValue = AsInt(requirement, "maxFloor", 0);
                element.FindPropertyRelative("memoryFragmentId").stringValue = AsString(requirement, "memoryFragmentId");
            }
        }

        private static void SetRuntimeEffects(SerializedProperty property, Dictionary<string, object> choice)
        {
            if (choice == null || !choice.TryGetValue("effects", out var effectsValue) || !(effectsValue is List<object> effects))
            {
                property.arraySize = 0;
                return;
            }

            property.arraySize = effects.Count;
            for (var i = 0; i < effects.Count; i++)
            {
                var effect = effects[i] as Dictionary<string, object>;
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("kind").stringValue = AsString(effect, "kind");
                element.FindPropertyRelative("amount").intValue = AsInt(effect, "amount", 0);
                element.FindPropertyRelative("flag").stringValue = AsString(effect, "flag");
                element.FindPropertyRelative("value").boolValue = AsBool(effect, "value", false);
                element.FindPropertyRelative("itemRef").stringValue = AsString(effect, "itemRef");
                element.FindPropertyRelative("count").intValue = AsInt(effect, "count", 0);
                element.FindPropertyRelative("abilityRef").stringValue = AsString(effect, "abilityRef");
                element.FindPropertyRelative("rewardBundleRef").stringValue = AsString(effect, "rewardBundleRef");
                SetMemoryFragmentEffect(element, effect);
                element.FindPropertyRelative("npcStage").stringValue = AsString(effect, "npcStage");
                element.FindPropertyRelative("targetEncounterId").stringValue = AsString(effect, "targetEncounterId");
                element.FindPropertyRelative("targetNodeId").stringValue = AsString(effect, "targetNodeId");
                SetRuntimeCombatHandoff(element.FindPropertyRelative("combatHandoff"), effect);
            }
        }

        private static void SetRuntimeCombatHandoff(SerializedProperty property, Dictionary<string, object> effect)
        {
            if (effect == null || !effect.TryGetValue("combatHandoff", out var handoffValue) || !(handoffValue is Dictionary<string, object> handoff))
            {
                ClearRuntimeCombatHandoff(property);
                return;
            }

            property.FindPropertyRelative("stableId").stringValue = AsString(handoff, "stableId");
            property.FindPropertyRelative("sourceEncounterId").stringValue = AsString(handoff, "sourceEncounterId");
            property.FindPropertyRelative("sourceChoiceId").stringValue = AsString(handoff, "sourceChoiceId");
            property.FindPropertyRelative("seedKey").stringValue = AsString(handoff, "seedKey");
            property.FindPropertyRelative("sourceHash").stringValue = AsString(handoff, "sourceHash");
            property.FindPropertyRelative("floor").intValue = AsInt(handoff, "floor", 0);
            SetStringArrayProperty(property.FindPropertyRelative("enemyRefs"), GetStringArray(handoff, "enemyRefs"));
            property.FindPropertyRelative("npcStage").stringValue = AsString(handoff, "npcStage");
            SetPostCombatEffects(property.FindPropertyRelative("onVictoryEffects"), handoff, "onVictoryEffects");
            SetPostCombatEffects(property.FindPropertyRelative("onDefeatEffects"), handoff, "onDefeatEffects");

            var returnNode = handoff.TryGetValue("returnNode", out var returnNodeValue) ? returnNodeValue as Dictionary<string, object> : null;
            property.FindPropertyRelative("onVictoryReturnNode").stringValue = AsString(returnNode, "onVictory");
            property.FindPropertyRelative("onDefeatReturnNode").stringValue = AsString(returnNode, "onDefeat");

            var postCombatText = handoff.TryGetValue("postCombatText", out var postCombatValue) ? postCombatValue as Dictionary<string, object> : null;
            property.FindPropertyRelative("victoryTextKey").stringValue = AsString(postCombatText, "victoryTextKey");
            property.FindPropertyRelative("defeatTextKey").stringValue = AsString(postCombatText, "defeatTextKey");
            property.FindPropertyRelative("deathTextKey").stringValue = AsString(postCombatText, "deathTextKey");

            var reaction = handoff.TryGetValue("npcImmediateReaction", out var reactionValue) ? reactionValue as Dictionary<string, object> : null;
            property.FindPropertyRelative("npcReactionKey").stringValue = AsString(reaction, "reactionKey");
            property.FindPropertyRelative("promptHash").stringValue = AsString(handoff, "promptHash");
            property.FindPropertyRelative("cacheKey").stringValue = AsString(handoff, "cacheKey");
        }

        private static void ClearRuntimeCombatHandoff(SerializedProperty property)
        {
            property.FindPropertyRelative("stableId").stringValue = string.Empty;
            property.FindPropertyRelative("sourceEncounterId").stringValue = string.Empty;
            property.FindPropertyRelative("sourceChoiceId").stringValue = string.Empty;
            property.FindPropertyRelative("seedKey").stringValue = string.Empty;
            property.FindPropertyRelative("sourceHash").stringValue = string.Empty;
            property.FindPropertyRelative("floor").intValue = 0;
            SetStringArrayProperty(property.FindPropertyRelative("enemyRefs"), new string[0]);
            property.FindPropertyRelative("npcStage").stringValue = string.Empty;
            property.FindPropertyRelative("onVictoryEffects").arraySize = 0;
            property.FindPropertyRelative("onDefeatEffects").arraySize = 0;
            property.FindPropertyRelative("onVictoryReturnNode").stringValue = string.Empty;
            property.FindPropertyRelative("onDefeatReturnNode").stringValue = string.Empty;
            property.FindPropertyRelative("victoryTextKey").stringValue = string.Empty;
            property.FindPropertyRelative("defeatTextKey").stringValue = string.Empty;
            property.FindPropertyRelative("deathTextKey").stringValue = string.Empty;
            property.FindPropertyRelative("npcReactionKey").stringValue = string.Empty;
            property.FindPropertyRelative("promptHash").stringValue = string.Empty;
            property.FindPropertyRelative("cacheKey").stringValue = string.Empty;
        }

        private static void SetPostCombatEffects(SerializedProperty property, Dictionary<string, object> handoff, string key)
        {
            if (!handoff.TryGetValue(key, out var value) || !(value is List<object> effects))
            {
                property.arraySize = 0;
                return;
            }

            property.arraySize = effects.Count;
            for (var i = 0; i < effects.Count; i++)
            {
                var effect = effects[i] as Dictionary<string, object>;
                var element = property.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("kind").stringValue = AsString(effect, "kind");
                element.FindPropertyRelative("amount").intValue = AsInt(effect, "amount", 0);
                element.FindPropertyRelative("flag").stringValue = AsString(effect, "flag");
                element.FindPropertyRelative("value").boolValue = AsBool(effect, "value", false);
                element.FindPropertyRelative("itemRef").stringValue = AsString(effect, "itemRef");
                element.FindPropertyRelative("count").intValue = AsInt(effect, "count", 0);
                element.FindPropertyRelative("abilityRef").stringValue = AsString(effect, "abilityRef");
                element.FindPropertyRelative("rewardBundleRef").stringValue = AsString(effect, "rewardBundleRef");
                SetMemoryFragmentEffect(element, effect);
                element.FindPropertyRelative("npcStage").stringValue = AsString(effect, "npcStage");
                element.FindPropertyRelative("targetEncounterId").stringValue = AsString(effect, "targetEncounterId");
                element.FindPropertyRelative("targetNodeId").stringValue = AsString(effect, "targetNodeId");
            }
        }

        private static void SetMemoryFragmentEffect(SerializedProperty element, Dictionary<string, object> effect)
        {
            var fragment = effect != null && effect.TryGetValue("memoryFragment", out var fragmentValue)
                ? fragmentValue as Dictionary<string, object>
                : null;
            element.FindPropertyRelative("memoryFragmentId").stringValue = AsString(fragment, "stableId");
            element.FindPropertyRelative("memoryFragmentTextKey").stringValue = AsString(fragment, "textKey");
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

        private static bool AsBool(Dictionary<string, object> map, string key, bool fallback)
        {
            if (map == null || !map.TryGetValue(key, out var value) || value == null)
            {
                return fallback;
            }

            if (value is bool boolValue)
            {
                return boolValue;
            }

            return bool.TryParse(value.ToString(), out var parsed) ? parsed : fallback;
        }

        private static string[] GetStringArray(Dictionary<string, object> map, string key)
        {
            if (map == null || !map.TryGetValue(key, out var value) || !(value is List<object> list))
            {
                return new string[0];
            }

            var result = new string[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                result[i] = list[i] == null ? string.Empty : list[i].ToString();
            }

            return result;
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
            SetStringArrayProperty(property, values);
        }

        private static void SetStringArrayProperty(SerializedProperty property, string[] values)
        {
            property.arraySize = values == null ? 0 : values.Length;
            for (var i = 0; i < property.arraySize; i++)
            {
                property.GetArrayElementAtIndex(i).stringValue = values[i] ?? string.Empty;
            }
        }
    }
}
#endif
