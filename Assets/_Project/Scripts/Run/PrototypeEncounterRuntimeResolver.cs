using HwigiTower.Encounters;
using HwigiTower.Core;

namespace HwigiTower.Run
{
    public readonly struct PrototypeEncounterChoiceView
    {
        public PrototypeEncounterChoiceView(string choiceStableId, bool visible, bool enabled, string reasonTextKey)
            : this(choiceStableId, string.Empty, visible, enabled, reasonTextKey)
        {
        }

        public PrototypeEncounterChoiceView(string choiceStableId, string textKey, bool visible, bool enabled, string reasonTextKey)
        {
            ChoiceStableId = choiceStableId ?? string.Empty;
            TextKey = textKey ?? string.Empty;
            Visible = visible;
            Enabled = enabled;
            ReasonTextKey = reasonTextKey ?? string.Empty;
        }

        public string ChoiceStableId { get; }
        public string TextKey { get; }
        public bool Visible { get; }
        public bool Enabled { get; }
        public string ReasonTextKey { get; }
    }

    public readonly struct PrototypeEncounterChoiceResolution
    {
        public PrototypeEncounterChoiceResolution(string choiceStableId, bool applied, string message)
        {
            ChoiceStableId = choiceStableId ?? string.Empty;
            Applied = applied;
            Message = message ?? string.Empty;
        }

        public string ChoiceStableId { get; }
        public bool Applied { get; }
        public string Message { get; }
    }

    public static class PrototypeEncounterRuntimeResolver
    {
        public static PrototypeEncounterChoiceResolution Resolve(PrototypeRunState state, EncounterData encounter, string choiceStableId)
        {
            return Resolve(state, encounter, choiceStableId, default, string.Empty);
        }

        public static PrototypeEncounterChoiceResolution Resolve(PrototypeRunState state, EncounterData encounter, string choiceStableId, DeterministicRunContext context, string nodeId)
        {
            if (state == null || encounter == null)
            {
                return new PrototypeEncounterChoiceResolution(string.Empty, false, "choice failed: missing state or encounter");
            }

            var choice = SelectChoice(state, encounter, choiceStableId);
            if (choice == null)
            {
                return new PrototypeEncounterChoiceResolution(string.Empty, false, "choice failed: no available choice");
            }

            if (!RequirementsMet(state, encounter, choice))
            {
                return new PrototypeEncounterChoiceResolution(choice.stableId, false, "choice failed: requirements not met");
            }

            var visibleChoices = BuildVisibleChoiceSummary(state, encounter);
            var applied = 0;
            var ignored = 0;
            var effects = choice.effects ?? new EncounterEffectRuntimeData[0];
            for (var i = 0; i < effects.Length; i++)
            {
                if (ApplyEffect(state, encounter, effects[i], context, nodeId))
                {
                    applied++;
                }
                else
                {
                    ignored++;
                }
            }

            return new PrototypeEncounterChoiceResolution(choice.stableId, true, $"choices: {visibleChoices} | choice applied: {choice.stableId}; effects={applied}; ignored={ignored}");
        }

        public static PrototypeEncounterChoiceView[] BuildChoiceViews(PrototypeRunState state, EncounterData encounter)
        {
            if (state == null || encounter == null || encounter.Choices == null)
            {
                return new PrototypeEncounterChoiceView[0];
            }

            var choices = encounter.Choices;
            var views = new PrototypeEncounterChoiceView[choices.Length];
            for (var i = 0; i < choices.Length; i++)
            {
                var met = RequirementsMet(state, encounter, choices[i]);
                var hidden = !met && choices[i].unavailablePolicyMode == "Hidden";
                views[i] = new PrototypeEncounterChoiceView(
                    choices[i].stableId,
                    choices[i].textKey,
                    !hidden,
                    met,
                    met ? string.Empty : choices[i].unavailableReasonTextKey);
            }

            return views;
        }

        private static EncounterChoiceRuntimeData SelectChoice(PrototypeRunState state, EncounterData encounter, string choiceStableId)
        {
            var choices = encounter.Choices ?? new EncounterChoiceRuntimeData[0];
            for (var i = 0; i < choices.Length; i++)
            {
                if (!string.IsNullOrEmpty(choiceStableId) && choices[i].stableId == choiceStableId)
                {
                    return choices[i];
                }
            }

            if (!string.IsNullOrEmpty(choiceStableId))
            {
                return null;
            }

            for (var i = 0; i < choices.Length; i++)
            {
                if (RequirementsMet(state, encounter, choices[i]))
                {
                    return choices[i];
                }
            }

            return null;
        }

        private static string BuildVisibleChoiceSummary(PrototypeRunState state, EncounterData encounter)
        {
            var views = BuildChoiceViews(state, encounter);
            var text = string.Empty;
            for (var i = 0; i < views.Length; i++)
            {
                if (!views[i].Visible)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    text += ",";
                }

                text += views[i].ChoiceStableId + (views[i].Enabled ? "[enabled]" : "[disabled]");
            }

            return string.IsNullOrEmpty(text) ? "none" : text;
        }

        private static bool RequirementsMet(PrototypeRunState state, EncounterData encounter, EncounterChoiceRuntimeData choice)
        {
            var requirements = choice.requirements ?? new EncounterRequirementRuntimeData[0];
            if (requirements.Length == 0)
            {
                return true;
            }

            var anyMode = choice.requirementMode == "Any";
            var matched = 0;
            for (var i = 0; i < requirements.Length; i++)
            {
                var met = RequirementMet(state, encounter, requirements[i]);
                if (met && anyMode)
                {
                    return true;
                }

                if (!met && !anyMode)
                {
                    return false;
                }

                if (met)
                {
                    matched++;
                }
            }

            return anyMode ? matched > 0 : true;
        }

        private static bool RequirementMet(PrototypeRunState state, EncounterData encounter, EncounterRequirementRuntimeData requirement)
        {
            if (requirement == null)
            {
                return true;
            }

            switch (requirement.kind)
            {
                case "StatAtLeast":
                    return ReadStat(state, encounter, requirement.stat) >= requirement.value;
                case "StatAtMost":
                    return ReadStat(state, encounter, requirement.stat) <= requirement.value;
                case "FloorInRange":
                    return encounter != null &&
                        encounter.Floor >= requirement.minFloor &&
                        (requirement.maxFloor <= 0 || encounter.Floor <= requirement.maxFloor);
                case "FlagEquals":
                    return state.HasFlag(requirement.flag) == requirement.expected;
                case "HasItem":
                    return state.GetItemCount(requirement.itemRef) >= System.Math.Max(1, requirement.minCount);
                case "HasAbility":
                    return state.HasAbilityRef(requirement.abilityRef);
                case "MemoryFragmentLocked":
                    return !state.HasMemoryFragmentRef(requirement.memoryFragmentId);
                default:
                    return true;
            }
        }

        private static bool ApplyEffect(PrototypeRunState state, EncounterData encounter, EncounterEffectRuntimeData effect, DeterministicRunContext context, string nodeId)
        {
            if (effect == null)
            {
                return false;
            }

            switch (effect.kind)
            {
                case "ModifyHp":
                    state.ModifyPlayerHp(effect.amount);
                    return true;
                case "ModifyMental":
                    state.ModifyMental(effect.amount);
                    return true;
                case "ModifyGold":
                    state.ModifyGold(effect.amount);
                    return true;
                case "ModifyGlitchLevel":
                    state.ModifyGlitchLevel(effect.amount);
                    return true;
                case "ModifyAffinity":
                    state.ModifyAffinity(effect.amount);
                    return true;
                case "SetFlag":
                    state.SetFlag(effect.flag, effect.value);
                    return true;
                case "AddItem":
                    state.AddItemRef(effect.itemRef, System.Math.Max(1, effect.count));
                    return true;
                case "RemoveItem":
                    state.AddItemRef(effect.itemRef, -System.Math.Max(1, effect.count));
                    return true;
                case "AddAbility":
                    return state.AddAbilityRef(effect.abilityRef);
                case "GrantRewardBundle":
                    return state.GrantRewardBundleRef(effect.rewardBundleRef);
                case "UnlockMemoryFragment":
                    return state.UnlockMemoryFragmentRef(effect.memoryFragmentId);
                case "StartCombat":
                    return state.ResolveCombatHandoff(context, nodeId, encounter, effect.combatHandoff, ApplyPostCombatEffects).Applied;
                default:
                    return false;
            }
        }

        private static void ApplyPostCombatEffects(PrototypeRunState state, EncounterPostCombatEffectRuntimeData[] effects)
        {
            if (effects == null)
            {
                return;
            }

            for (var i = 0; i < effects.Length; i++)
            {
                ApplyPostCombatEffect(state, effects[i]);
            }
        }

        private static bool ApplyPostCombatEffect(PrototypeRunState state, EncounterPostCombatEffectRuntimeData effect)
        {
            if (effect == null)
            {
                return false;
            }

            switch (effect.kind)
            {
                case "ModifyHp":
                    state.ModifyPlayerHp(effect.amount);
                    return true;
                case "ModifyMental":
                    state.ModifyMental(effect.amount);
                    return true;
                case "ModifyGold":
                    state.ModifyGold(effect.amount);
                    return true;
                case "ModifyGlitchLevel":
                    state.ModifyGlitchLevel(effect.amount);
                    return true;
                case "ModifyAffinity":
                    state.ModifyAffinity(effect.amount);
                    return true;
                case "SetFlag":
                    state.SetFlag(effect.flag, effect.value);
                    return true;
                case "AddItem":
                    state.AddItemRef(effect.itemRef, System.Math.Max(1, effect.count));
                    return true;
                case "RemoveItem":
                    state.AddItemRef(effect.itemRef, -System.Math.Max(1, effect.count));
                    return true;
                case "AddAbility":
                    return state.AddAbilityRef(effect.abilityRef);
                case "GrantRewardBundle":
                    return state.GrantRewardBundleRef(effect.rewardBundleRef);
                case "UnlockMemoryFragment":
                    return state.UnlockMemoryFragmentRef(effect.memoryFragmentId);
                default:
                    return false;
            }
        }

        private static int ReadStat(PrototypeRunState state, EncounterData encounter, string stat)
        {
            switch (stat)
            {
                case "hp":
                    return state.PlayerHp;
                case "maxHp":
                    return state.PlayerMaxHp;
                case "mental":
                    return state.Mental;
                case "gold":
                    return state.Gold;
                case "glitchLevel":
                    return state.GlitchLevel;
                case "affinity":
                    return state.Affinity;
                case "floor":
                    return encounter == null ? 0 : encounter.Floor;
                default:
                    return 0;
            }
        }
    }
}
