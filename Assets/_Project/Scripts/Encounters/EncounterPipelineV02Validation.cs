#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HwigiTower.Encounters
{
    public sealed class EncounterValidationIssue
    {
        public EncounterValidationIssue(string code, string path, string message)
        {
            Code = code ?? string.Empty;
            Path = path ?? string.Empty;
            Message = message ?? string.Empty;
        }

        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
    }

    public sealed class EncounterValidationResult
    {
        private readonly List<EncounterValidationIssue> _issues = new List<EncounterValidationIssue>();

        public IReadOnlyList<EncounterValidationIssue> Issues => _issues;
        public bool IsValid => _issues.Count == 0;

        public void Add(string code, string path, string message)
        {
            _issues.Add(new EncounterValidationIssue(code, path, message));
        }

        public bool HasErrorCode(string code)
        {
            for (var i = 0; i < _issues.Count; i++)
            {
                if (_issues[i].Code == code)
                {
                    return true;
                }
            }

            return false;
        }

        public string ToSummary()
        {
            if (IsValid)
            {
                return "valid";
            }

            var builder = new StringBuilder();
            for (var i = 0; i < _issues.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append("; ");
                }

                builder.Append(_issues[i].Code).Append(" @ ").Append(_issues[i].Path);
            }

            return builder.ToString();
        }
    }

    public static class EncounterPipelineV02Validator
    {
        public const string SchemaVersion = "0.2";
        public const string BasePath = "Assets/_Project/Data/Encounters/";
        public const string AssetPathPattern = "Assets/_Project/Data/Encounters/SO_Encounter_<STABLE_ID>.asset";

        private static readonly Regex StableIdRegex = new Regex("^[A-Z][A-Z0-9_:.\\-]{2,127}$", RegexOptions.Compiled);
        private static readonly Regex HashRegex = new Regex("^sha256:[a-f0-9]{64}$", RegexOptions.Compiled);
        private static readonly Regex TextKeyRegex = new Regex("^PLACEHOLDER_[A-Z0-9_:.\\-]+$", RegexOptions.Compiled);
        private static readonly Regex EncounterAssetPathRegex = new Regex("^Assets/_Project/Data/Encounters/SO_Encounter_[A-Z][A-Z0-9_:.\\-]{2,127}\\.asset$", RegexOptions.Compiled);

        public static EncounterValidationResult ValidateFile(string path)
        {
            return ValidateJson(File.ReadAllText(path));
        }

        public static EncounterValidationResult ValidateJson(string json)
        {
            var result = new EncounterValidationResult();
            object root;
            if (!TryParse(json, result, out root))
            {
                return result;
            }

            ValidateTargetObject("Pack", root, "$", result);
            return result;
        }

        public static EncounterValidationResult ValidateTargetJson(string targetSchema, string json, string path = "$")
        {
            var result = new EncounterValidationResult();
            object root;
            if (!TryParse(json, result, out root))
            {
                return result;
            }

            ValidateTargetObject(targetSchema, root, string.IsNullOrEmpty(path) ? "$" : path, result);
            return result;
        }

        internal static EncounterValidationResult ValidateTargetObject(string targetSchema, object value, string path = "$")
        {
            var result = new EncounterValidationResult();
            ValidateTargetObject(targetSchema, value, string.IsNullOrEmpty(path) ? "$" : path, result);
            return result;
        }

        internal static void ValidateTargetObject(string targetSchema, object value, string path, EncounterValidationResult result)
        {
            ValidateForbiddenFields(value, path, result);
            ValidateNpcStages(value, path, result);

            switch (targetSchema)
            {
                case "Pack":
                    ValidatePack(value as Dictionary<string, object>, path, result);
                    break;
                case "Encounter":
                    ValidateEncounter(value as Dictionary<string, object>, path, result);
                    break;
                case "Choice":
                    ValidateChoice(value as Dictionary<string, object>, path, result);
                    break;
                case "CombatHandoff":
                    ValidateCombatHandoff(value as Dictionary<string, object>, path, result);
                    break;
                case "AssetBake":
                    ValidateAssetBake(value as Dictionary<string, object>, path, result);
                    break;
                default:
                    result.Add("TARGET_SCHEMA_UNSUPPORTED", path, targetSchema);
                    break;
            }
        }

        internal static bool TryParse(string json, EncounterValidationResult result, out object value)
        {
            try
            {
                value = EncounterJsonMiniParser.Parse(json);
                return true;
            }
            catch (EncounterJsonParseException exception)
            {
                value = null;
                result.Add("JSON_SYNTAX_INVALID", "$", exception.Message);
                return false;
            }
        }

        private static void ValidatePack(Dictionary<string, object> pack, string path, EncounterValidationResult result)
        {
            if (pack == null)
            {
                result.Add("PACK_OBJECT_INVALID", path, "Pack must be an object.");
                return;
            }

            Require(pack, "schemaVersion", path, result);
            Require(pack, "packStableId", path, result);
            Require(pack, "sourceHash", path, result);
            Require(pack, "assetBake", path, result);
            Require(pack, "packManifest", path, result);
            Require(pack, "encounters", path, result);

            if (pack.ContainsKey("schemaVersion") && AsString(pack, "schemaVersion") != SchemaVersion)
            {
                result.Add("SCHEMA_VERSION_INVALID", path + ".schemaVersion", "schemaVersion must be 0.2.");
            }

            if (pack.ContainsKey("packStableId"))
            {
                ValidateStableId(AsString(pack, "packStableId"), path + ".packStableId", result);
            }

            if (pack.ContainsKey("sourceHash"))
            {
                ValidateHash(AsString(pack, "sourceHash"), path + ".sourceHash", result);
            }

            if (pack.TryGetValue("assetBake", out var assetBake))
            {
                ValidateAssetBake(assetBake as Dictionary<string, object>, path + ".assetBake", result);
            }

            if (pack.TryGetValue("encounters", out var encountersValue) && encountersValue is List<object> encounters)
            {
                for (var i = 0; i < encounters.Count; i++)
                {
                    ValidateEncounter(encounters[i] as Dictionary<string, object>, path + ".encounters[" + i + "]", result);
                }
            }

            ValidateManifestCounts(pack, path, result);
        }

        private static void ValidateAssetBake(Dictionary<string, object> assetBake, string path, EncounterValidationResult result)
        {
            if (assetBake == null)
            {
                result.Add("ASSET_BAKE_INVALID", path, "assetBake must be an object.");
                return;
            }

            if (AsString(assetBake, "mode") != "updateExisting" || !AsBool(assetBake, "prohibitDeleteRecreate", false))
            {
                result.Add("ASSET_BAKE_DELETE_RECREATE_FORBIDDEN", path, "Bake must update existing assets without delete/recreate.");
            }

            if (assetBake.ContainsKey("basePath") && AsString(assetBake, "basePath") != BasePath)
            {
                result.Add("ASSET_BAKE_BASE_PATH_INVALID", path + ".basePath", "basePath must target formal encounter data.");
            }

            if (assetBake.ContainsKey("assetPathPattern") && AsString(assetBake, "assetPathPattern") != AssetPathPattern)
            {
                result.Add("ASSET_BAKE_PATH_PATTERN_INVALID", path + ".assetPathPattern", "assetPathPattern must be the formal v0.2 pattern.");
            }
        }

        private static void ValidateEncounter(Dictionary<string, object> encounter, string path, EncounterValidationResult result)
        {
            if (encounter == null)
            {
                result.Add("ENCOUNTER_OBJECT_INVALID", path, "Encounter must be an object.");
                return;
            }

            if (encounter.ContainsKey("kind") && AsString(encounter, "kind") != "Encounter")
            {
                result.Add("ENCOUNTER_KIND_INVALID", path + ".kind", "Encounter kind must be Encounter.");
            }

            if (encounter.ContainsKey("stableId"))
            {
                ValidateStableId(AsString(encounter, "stableId"), path + ".stableId", result);
            }

            if (encounter.ContainsKey("sourceHash"))
            {
                ValidateHash(AsString(encounter, "sourceHash"), path + ".sourceHash", result);
            }

            if (encounter.ContainsKey("assetPath") && !EncounterAssetPathRegex.IsMatch(AsString(encounter, "assetPath")))
            {
                result.Add("ENCOUNTER_ASSET_PATH_INVALID", path + ".assetPath", "Invalid formal encounter asset path.");
            }

            if (encounter.ContainsKey("bodyTextKey"))
            {
                ValidateTextKey(AsString(encounter, "bodyTextKey"), path + ".bodyTextKey", result);
            }

            if (encounter.TryGetValue("choices", out var choicesValue) && choicesValue is List<object> choices)
            {
                if (choices.Count < 2 || choices.Count > 3)
                {
                    result.Add("ENCOUNTER_CHOICE_COUNT_OUT_OF_RANGE", path + ".choices", "Encounter must have 2-3 choices.");
                }

                for (var i = 0; i < choices.Count; i++)
                {
                    ValidateChoice(choices[i] as Dictionary<string, object>, path + ".choices[" + i + "]", result);
                }
            }

            if (encounter.TryGetValue("nodeRules", out var nodeRulesValue) && nodeRulesValue is Dictionary<string, object> nodeRules)
            {
                var timeLimit = AsInt(nodeRules, "timeLimitSeconds", -1);
                if (timeLimit < 30 || timeLimit > 60)
                {
                    result.Add("ENCOUNTER_TIME_LIMIT_OUT_OF_RANGE", path + ".nodeRules.timeLimitSeconds", "timeLimitSeconds must be 30-60.");
                }
            }

            var encounterType = AsString(encounter, "encounterType");
            if (encounterType == "MoralChoice")
            {
                ValidateMoralChoice(encounter, path, result);
            }
            else if (encounterType == "Shop")
            {
                ValidateShop(encounter, path, result);
            }
        }

        private static void ValidateChoice(Dictionary<string, object> choice, string path, EncounterValidationResult result)
        {
            if (choice == null)
            {
                result.Add("CHOICE_OBJECT_INVALID", path, "Choice must be an object.");
                return;
            }

            if (choice.ContainsKey("kind") && AsString(choice, "kind") != "Choice")
            {
                result.Add("CHOICE_KIND_INVALID", path + ".kind", "Choice kind must be Choice.");
            }

            if (choice.ContainsKey("stableId"))
            {
                ValidateStableId(AsString(choice, "stableId"), path + ".stableId", result);
            }

            if (choice.ContainsKey("textKey"))
            {
                ValidateTextKey(AsString(choice, "textKey"), path + ".textKey", result);
            }

            var hasEffects = choice.TryGetValue("effects", out var effectsValue) && effectsValue is List<object> effects && effects.Count > 0;
            var hasReaction = choice.ContainsKey("npcImmediateReaction");
            if (!hasEffects && !hasReaction)
            {
                result.Add("CHOICE_REQUIRES_EFFECT_OR_REACTION", path, "Every choice requires an effect or immediate NPC reaction.");
            }

            if (choice.TryGetValue("npcImmediateReaction", out var reactionValue) && reactionValue is Dictionary<string, object> reaction)
            {
                if (reaction.ContainsKey("textKey"))
                {
                    ValidateTextKey(AsString(reaction, "textKey"), path + ".npcImmediateReaction.textKey", result);
                }
            }
        }

        private static void ValidateCombatHandoff(Dictionary<string, object> handoff, string path, EncounterValidationResult result)
        {
            if (handoff == null)
            {
                result.Add("COMBAT_HANDOFF_OBJECT_INVALID", path, "CombatHandoff must be an object.");
                return;
            }

            if (handoff.ContainsKey("stableId"))
            {
                ValidateStableId(AsString(handoff, "stableId"), path + ".stableId", result);
            }

            if (handoff.ContainsKey("sourceHash"))
            {
                ValidateHash(AsString(handoff, "sourceHash"), path + ".sourceHash", result);
            }
        }

        private static void ValidateMoralChoice(Dictionary<string, object> encounter, string path, EncounterValidationResult result)
        {
            var found = false;
            ForEachChoiceEffect(encounter, effect =>
            {
                var kind = AsString(effect, "kind");
                if (kind == "ModifyAffinity" || kind == "ModifyGlitchLevel")
                {
                    found = true;
                }
            });

            if (!found)
            {
                result.Add("MORAL_CHOICE_REQUIRES_AFFINITY_OR_GLITCH_EFFECT", path + ".choices[*].effects", "MoralChoice requires ModifyAffinity or ModifyGlitchLevel.");
            }
        }

        private static void ValidateShop(Dictionary<string, object> encounter, string path, EncounterValidationResult result)
        {
            if (!encounter.TryGetValue("choices", out var choicesValue) || !(choicesValue is List<object> choices))
            {
                return;
            }

            var purchaseFound = false;
            for (var i = 0; i < choices.Count; i++)
            {
                if (!(choices[i] is Dictionary<string, object> choice) || !IsShopPurchaseChoice(choice))
                {
                    continue;
                }

                purchaseFound = true;
                var goldGate = TryGetGoldGate(choice, out var gateValue);
                var goldSpend = TryGetGoldSpend(choice, out var spendValue);
                var reward = HasPurchaseReward(choice);
                var choicePath = path + ".choices[" + i + "]";

                if (!goldGate)
                {
                    result.Add("SHOP_PURCHASE_REQUIRES_GOLD_GATE", choicePath + ".requirements", "Shop purchase requires StatAtLeast(gold, price).");
                }

                if (!goldSpend)
                {
                    result.Add("SHOP_PURCHASE_REQUIRES_MODIFY_GOLD", choicePath + ".effects", "Shop purchase requires ModifyGold(-price).");
                }

                if (!reward)
                {
                    result.Add("SHOP_PURCHASE_REQUIRES_PURCHASE_REWARD", choicePath + ".effects", "Shop purchase requires AddItem, AddAbility, or GrantRewardBundle.");
                }

                if (goldGate && goldSpend && gateValue != spendValue)
                {
                    result.Add("SHOP_PURCHASE_PRICE_MISMATCH", choicePath, "Gold gate and spend amount must match.");
                }
            }

            if (!purchaseFound)
            {
                result.Add("SHOP_PURCHASE_REQUIRES_PURCHASE_REWARD", path + ".choices", "Shop requires at least one purchase choice.");
            }
        }

        private static void ValidateManifestCounts(Dictionary<string, object> pack, string path, EncounterValidationResult result)
        {
            if (!pack.TryGetValue("actualFullCounts", out var actualValue) || !(actualValue is Dictionary<string, object> actual))
            {
                return;
            }

            if (!pack.TryGetValue("packManifest", out var manifestValue) || !(manifestValue is Dictionary<string, object> manifest))
            {
                return;
            }

            if (!manifest.TryGetValue("expectedFullCounts", out var expectedValue) || !(expectedValue is Dictionary<string, object> expected))
            {
                return;
            }

            foreach (var pair in expected)
            {
                if (AsInt(actual, pair.Key, int.MinValue) != AsInt(expected, pair.Key, int.MaxValue))
                {
                    result.Add("PACK_CATEGORY_COUNTS_MISMATCH", path + ".encounters", "Full pack category counts do not match manifest.");
                    return;
                }
            }
        }

        private static void ValidateForbiddenFields(object value, string path, EncounterValidationResult result)
        {
            if (value is Dictionary<string, object> map)
            {
                foreach (var pair in map)
                {
                    var childPath = path == "$" ? "$." + pair.Key : path + "." + pair.Key;
                    if (pair.Key == "escapePolicy" || pair.Key == "onEscape" || pair.Key == "escapeTextKey" || pair.Key == "onEscapeEffects")
                    {
                        result.Add("COMBAT_ESCAPE_NOT_SUPPORTED_V02", childPath, "Escape fields are forbidden in v0.2.");
                    }

                    if (pair.Key == "dialogue" || pair.Key == "finalText" || pair.Key == "bodyText" || pair.Key == "choiceText" || pair.Key == "npcText" || pair.Key == "text")
                    {
                        result.Add("DIRECT_TEXT_FIELD_FORBIDDEN", childPath, "Direct text fields are forbidden; use PLACEHOLDER_* keys.");
                    }

                    ValidateForbiddenFields(pair.Value, childPath, result);
                }
            }
            else if (value is List<object> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    ValidateForbiddenFields(list[i], path + "[" + i + "]", result);
                }
            }
        }

        private static void ValidateNpcStages(object value, string path, EncounterValidationResult result)
        {
            if (value is Dictionary<string, object> map)
            {
                foreach (var pair in map)
                {
                    var childPath = path == "$" ? "$." + pair.Key : path + "." + pair.Key;
                    if ((pair.Key == "npcStage" || pair.Key == "stage") && pair.Value is string stage && !EncounterPipelineV02NpcStage.TryMapToRuntime(stage, out _))
                    {
                        result.Add("NPC_STAGE_ENUM_INVALID", childPath, "Invalid v0.2 NPC stage.");
                    }

                    ValidateNpcStages(pair.Value, childPath, result);
                }
            }
            else if (value is List<object> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    ValidateNpcStages(list[i], path + "[" + i + "]", result);
                }
            }
        }

        private static bool IsShopPurchaseChoice(Dictionary<string, object> choice)
        {
            return HasPurchaseReward(choice) || TryGetGoldSpend(choice, out _) || TryGetGoldGate(choice, out _);
        }

        private static bool HasPurchaseReward(Dictionary<string, object> choice)
        {
            var found = false;
            ForEachEffect(choice, effect =>
            {
                var kind = AsString(effect, "kind");
                if (kind == "AddItem" || kind == "AddAbility" || kind == "GrantRewardBundle")
                {
                    found = true;
                }
            });
            return found;
        }

        private static bool TryGetGoldGate(Dictionary<string, object> choice, out int value)
        {
            value = 0;
            if (!choice.TryGetValue("requirements", out var requirementsValue) || !(requirementsValue is List<object> requirements))
            {
                return false;
            }

            for (var i = 0; i < requirements.Count; i++)
            {
                if (requirements[i] is Dictionary<string, object> requirement &&
                    AsString(requirement, "kind") == "StatAtLeast" &&
                    AsString(requirement, "stat") == "gold")
                {
                    value = AsInt(requirement, "value", 0);
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetGoldSpend(Dictionary<string, object> choice, out int value)
        {
            value = 0;
            if (!choice.TryGetValue("effects", out var effectsValue) || !(effectsValue is List<object> effects))
            {
                return false;
            }

            for (var i = 0; i < effects.Count; i++)
            {
                if (effects[i] is Dictionary<string, object> effect &&
                    AsString(effect, "kind") == "ModifyGold" &&
                    AsInt(effect, "amount", 0) < 0)
                {
                    value = -AsInt(effect, "amount", 0);
                    return true;
                }
            }

            return false;
        }

        private static void ForEachChoiceEffect(Dictionary<string, object> encounter, System.Action<Dictionary<string, object>> action)
        {
            if (!encounter.TryGetValue("choices", out var choicesValue) || !(choicesValue is List<object> choices))
            {
                return;
            }

            for (var i = 0; i < choices.Count; i++)
            {
                if (choices[i] is Dictionary<string, object> choice)
                {
                    ForEachEffect(choice, action);
                }
            }
        }

        private static void ForEachEffect(Dictionary<string, object> choice, System.Action<Dictionary<string, object>> action)
        {
            if (!choice.TryGetValue("effects", out var effectsValue) || !(effectsValue is List<object> effects))
            {
                return;
            }

            for (var i = 0; i < effects.Count; i++)
            {
                if (effects[i] is Dictionary<string, object> effect)
                {
                    action(effect);
                    if (effect.TryGetValue("combatHandoff", out var handoffValue) && handoffValue is Dictionary<string, object> handoff)
                    {
                        ForEachEffectArray(handoff, "onVictoryEffects", action);
                        ForEachEffectArray(handoff, "onDefeatEffects", action);
                    }
                }
            }
        }

        private static void ForEachEffectArray(Dictionary<string, object> owner, string key, System.Action<Dictionary<string, object>> action)
        {
            if (!owner.TryGetValue(key, out var value) || !(value is List<object> effects))
            {
                return;
            }

            for (var i = 0; i < effects.Count; i++)
            {
                if (effects[i] is Dictionary<string, object> effect)
                {
                    action(effect);
                }
            }
        }

        private static void ValidateStableId(string value, string path, EncounterValidationResult result)
        {
            if (!StableIdRegex.IsMatch(value ?? string.Empty))
            {
                result.Add("STABLE_ID_INVALID", path, "Invalid stableId.");
            }
        }

        private static void ValidateHash(string value, string path, EncounterValidationResult result)
        {
            if (!HashRegex.IsMatch(value ?? string.Empty))
            {
                result.Add("HASH_INVALID", path, "Invalid hash.");
            }
        }

        private static void ValidateTextKey(string value, string path, EncounterValidationResult result)
        {
            if (!TextKeyRegex.IsMatch(value ?? string.Empty))
            {
                result.Add("TEXT_KEY_INVALID", path, "Text keys must use PLACEHOLDER_*.");
            }
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

        private static void Require(Dictionary<string, object> map, string key, string path, EncounterValidationResult result)
        {
            if (!map.ContainsKey(key))
            {
                result.Add("PACK_REQUIRED_FIELD_MISSING", path + "." + key, key + " is required.");
            }
        }
    }
}
#endif
