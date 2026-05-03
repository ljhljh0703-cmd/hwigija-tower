#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

namespace HwigiTower.Encounters
{
    public sealed class EncounterCatalogReferenceReport
    {
        private readonly List<string> _missingCatalogRefs = new List<string>();
        private readonly List<string> _recognizedMemoryFragmentRefs = new List<string>();
        private readonly List<string> _duplicateStableIds = new List<string>();

        public IReadOnlyList<string> MissingCatalogRefs => _missingCatalogRefs;
        public IReadOnlyList<string> RecognizedMemoryFragmentRefs => _recognizedMemoryFragmentRefs;
        public IReadOnlyList<string> DuplicateStableIds => _duplicateStableIds;
        public int EncounterCount { get; private set; }
        public bool Passed => _missingCatalogRefs.Count == 0 && _duplicateStableIds.Count == 0;

        public void SetEncounterCount(int count)
        {
            EncounterCount = count;
        }

        public void AddMissingCatalogRef(string kind, string stableId, string path)
        {
            _missingCatalogRefs.Add((kind ?? string.Empty) + "|" + (stableId ?? string.Empty) + "|" + (path ?? string.Empty));
        }

        public void AddMemoryFragmentRef(string stableId)
        {
            if (!string.IsNullOrEmpty(stableId) && !_recognizedMemoryFragmentRefs.Contains(stableId))
            {
                _recognizedMemoryFragmentRefs.Add(stableId);
            }
        }

        public void AddDuplicateStableId(string stableId)
        {
            if (!string.IsNullOrEmpty(stableId) && !_duplicateStableIds.Contains(stableId))
            {
                _duplicateStableIds.Add(stableId);
            }
        }
    }

    public static class EncounterRuntimeCatalogReferenceValidator
    {
        public static EncounterCatalogReferenceReport ValidateFile(string jsonPath, EncounterRuntimeCatalogData catalog)
        {
            return ValidateJson(File.ReadAllText(jsonPath), catalog);
        }

        public static EncounterCatalogReferenceReport ValidateJson(string json, EncounterRuntimeCatalogData catalog)
        {
            var report = new EncounterCatalogReferenceReport();
            var validation = new EncounterValidationResult();
            if (!EncounterPipelineV02Validator.TryParse(json, validation, out var root))
            {
                report.AddMissingCatalogRef("Json", validation.ToSummary(), "$");
                return report;
            }

            var pack = root as Dictionary<string, object>;
            if (pack == null || !pack.TryGetValue("encounters", out var encountersValue) || !(encountersValue is List<object> encounters))
            {
                report.AddMissingCatalogRef("Pack", "encounters", "$.encounters");
                return report;
            }

            report.SetEncounterCount(encounters.Count);
            ValidateEncounterDuplicates(encounters, report);
            for (var i = 0; i < encounters.Count; i++)
            {
                ValidateEncounter(encounters[i] as Dictionary<string, object>, catalog, "$.encounters[" + i + "]", report);
            }

            return report;
        }

        private static void ValidateEncounterDuplicates(List<object> encounters, EncounterCatalogReferenceReport report)
        {
            var stableIds = new HashSet<string>();
            for (var i = 0; i < encounters.Count; i++)
            {
                var stableId = AsString(encounters[i] as Dictionary<string, object>, "stableId");
                if (!string.IsNullOrEmpty(stableId) && !stableIds.Add(stableId))
                {
                    report.AddDuplicateStableId(stableId);
                }
            }
        }

        private static void ValidateEncounter(Dictionary<string, object> encounter, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (encounter == null)
            {
                return;
            }

            var memoryFragment = encounter.TryGetValue("memoryFragment", out var memoryValue) ? memoryValue as Dictionary<string, object> : null;
            ValidateMemoryRef(AsString(memoryFragment, "stableId"), catalog, path + ".memoryFragment.stableId", report);

            if (!encounter.TryGetValue("choices", out var choicesValue) || !(choicesValue is List<object> choices))
            {
                return;
            }

            for (var i = 0; i < choices.Count; i++)
            {
                ValidateChoice(choices[i] as Dictionary<string, object>, catalog, path + ".choices[" + i + "]", report);
            }
        }

        private static void ValidateChoice(Dictionary<string, object> choice, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (choice == null)
            {
                return;
            }

            if (choice.TryGetValue("requirements", out var requirementsValue) && requirementsValue is List<object> requirements)
            {
                for (var i = 0; i < requirements.Count; i++)
                {
                    ValidateRequirement(requirements[i] as Dictionary<string, object>, catalog, path + ".requirements[" + i + "]", report);
                }
            }

            if (choice.TryGetValue("effects", out var effectsValue) && effectsValue is List<object> effects)
            {
                for (var i = 0; i < effects.Count; i++)
                {
                    ValidateEffect(effects[i] as Dictionary<string, object>, catalog, path + ".effects[" + i + "]", report);
                }
            }
        }

        private static void ValidateRequirement(Dictionary<string, object> requirement, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            var kind = AsString(requirement, "kind");
            switch (kind)
            {
                case "HasItem":
                    ValidateItemRef(AsString(requirement, "itemRef"), catalog, path + ".itemRef", report);
                    break;
                case "HasAbility":
                    ValidateAbilityRef(AsString(requirement, "abilityRef"), catalog, path + ".abilityRef", report);
                    break;
                case "MemoryFragmentLocked":
                    ValidateMemoryRef(AsString(requirement, "memoryFragmentId"), catalog, path + ".memoryFragmentId", report);
                    break;
            }
        }

        private static void ValidateEffect(Dictionary<string, object> effect, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            var kind = AsString(effect, "kind");
            switch (kind)
            {
                case "AddItem":
                    ValidateItemRef(AsString(effect, "itemRef"), catalog, path + ".itemRef", report);
                    break;
                case "AddAbility":
                    ValidateAbilityRef(AsString(effect, "abilityRef"), catalog, path + ".abilityRef", report);
                    break;
                case "GrantRewardBundle":
                    ValidateRewardRef(AsString(effect, "rewardBundleRef"), catalog, path + ".rewardBundleRef", report);
                    break;
                case "UnlockMemoryFragment":
                    var memoryFragment = effect != null && effect.TryGetValue("memoryFragment", out var memoryValue)
                        ? memoryValue as Dictionary<string, object>
                        : null;
                    ValidateMemoryRef(AsString(memoryFragment, "stableId"), catalog, path + ".memoryFragment.stableId", report);
                    break;
                case "StartCombat":
                    var handoff = effect != null && effect.TryGetValue("combatHandoff", out var handoffValue)
                        ? handoffValue as Dictionary<string, object>
                        : null;
                    ValidateCombatHandoff(handoff, catalog, path + ".combatHandoff", report);
                    break;
            }
        }

        private static void ValidateCombatHandoff(Dictionary<string, object> handoff, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (handoff == null || !handoff.TryGetValue("enemyRefs", out var value) || !(value is List<object> enemyRefs))
            {
                return;
            }

            for (var i = 0; i < enemyRefs.Count; i++)
            {
                var enemyRef = enemyRefs[i] == null ? string.Empty : enemyRefs[i].ToString();
                if (!catalog.TryGetEnemy(enemyRef, out _))
                {
                    report.AddMissingCatalogRef("Enemy", enemyRef, path + ".enemyRefs[" + i + "]");
                }
            }
        }

        private static void ValidateItemRef(string stableId, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (!catalog.TryGetItem(stableId, out _))
            {
                report.AddMissingCatalogRef("Item", stableId, path);
            }
        }

        private static void ValidateAbilityRef(string stableId, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (!catalog.TryGetAbility(stableId, out _))
            {
                report.AddMissingCatalogRef("Ability", stableId, path);
            }
        }

        private static void ValidateRewardRef(string stableId, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (!catalog.TryGetRewardBundle(stableId, out _))
            {
                report.AddMissingCatalogRef("Reward", stableId, path);
            }
        }

        private static void ValidateMemoryRef(string stableId, EncounterRuntimeCatalogData catalog, string path, EncounterCatalogReferenceReport report)
        {
            if (string.IsNullOrEmpty(stableId))
            {
                return;
            }

            if (catalog.TryGetMemoryFragment(stableId, out _))
            {
                report.AddMemoryFragmentRef(stableId);
                return;
            }

            report.AddMissingCatalogRef("MemoryFragment", stableId, path);
        }

        private static string AsString(Dictionary<string, object> map, string key)
        {
            return map != null && map.TryGetValue(key, out var value) && value != null ? value.ToString() : string.Empty;
        }
    }
}
#endif
