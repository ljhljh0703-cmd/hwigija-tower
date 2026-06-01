using HwigiTower.Combat;
using HwigiTower.Encounters;
using HwigiTower.Core;

namespace HwigiTower.Run
{
    public readonly struct PrototypeEncounterChoiceView
    {
        public PrototypeEncounterChoiceView(string choiceStableId, bool visible, bool enabled, string reasonTextKey)
            : this(choiceStableId, string.Empty, visible, enabled, reasonTextKey, string.Empty)
        {
        }

        public PrototypeEncounterChoiceView(string choiceStableId, string textKey, bool visible, bool enabled, string reasonTextKey)
            : this(choiceStableId, textKey, visible, enabled, reasonTextKey, string.Empty)
        {
        }

        public PrototypeEncounterChoiceView(string choiceStableId, string textKey, bool visible, bool enabled, string reasonTextKey, string hintText)
        {
            ChoiceStableId = choiceStableId ?? string.Empty;
            TextKey = textKey ?? string.Empty;
            Visible = visible;
            Enabled = enabled;
            ReasonTextKey = reasonTextKey ?? string.Empty;
            HintText = hintText ?? string.Empty;
        }

        public string ChoiceStableId { get; }
        public string TextKey { get; }
        public bool Visible { get; }
        public bool Enabled { get; }
        public string ReasonTextKey { get; }
        public string HintText { get; }
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
        public static CombatActionPreview BuildCombatActionPreview(PrototypeRunState state, CombatAction action)
        {
            var label = action switch
            {
                CombatAction.Attack => "공격",
                CombatAction.Defend => "방어",
                CombatAction.Skill => "스킬",
                _ => string.Empty
            };

            if (state == null || !state.IsInCombat || state.ActiveCombatPlayer == null || state.ActiveCombatEnemy == null)
            {
                return new CombatActionPreview(action, label, string.Empty, false);
            }

            switch (action)
            {
                case CombatAction.Attack:
                    return BuildAttackPreview(state, label);
                case CombatAction.Defend:
                    return BuildDefendPreview(state, label);
                case CombatAction.Skill:
                    return BuildSkillPreview(state, label);
                default:
                    return new CombatActionPreview(action, label, string.Empty, false);
            }
        }

        private static CombatActionPreview BuildAttackPreview(PrototypeRunState state, string label)
        {
            var attack = state.ActiveCombatPlayer == null ? 0 : state.ActiveCombatPlayer.Attack;
            if (state.ScoutAttackReady)
            {
                attack += state.ScoutNextAttackBonusForPreview;
            }

            if (attack <= 0)
            {
                return new CombatActionPreview(CombatAction.Attack, label, string.Empty, true);
            }

            return new CombatActionPreview(
                CombatAction.Attack,
                label,
                "예상 피해 " + attack + "-" + (attack + 2),
                true);
        }

        private static CombatActionPreview BuildDefendPreview(PrototypeRunState state, string label)
        {
            var enemyAttack = state.ActiveCombatEnemy == null ? 0 : state.ActiveCombatEnemy.Attack;
            if (enemyAttack <= 0)
            {
                return new CombatActionPreview(CombatAction.Defend, label, string.Empty, true);
            }

            return new CombatActionPreview(
                CombatAction.Defend,
                label,
                "피해 감소 " + (System.Math.Max(1, enemyAttack / 2) + (state.ScoutDamageReductionReady ? state.ScoutDamageReductionForPreview : 0)),
                true);
        }

        private static CombatActionPreview BuildSkillPreview(PrototypeRunState state, string label)
        {
            var scoutEquipped = state.IsCommandEquipped(PrototypeRunState.CommandScoutId) && state.HasAbilityRef("ABILITY_SCOUT");
            if (state.Arts03CooldownRounds > 0 && !scoutEquipped)
            {
                return new CombatActionPreview(CombatAction.Skill, label, "CD " + state.Arts03CooldownRounds + "턴", false);
            }

            if (!state.HasAnyPlayableCombatSkill)
            {
                return new CombatActionPreview(CombatAction.Skill, label, "조건 부족", false);
            }

            var cooldownAfterUse = state.HasReadyArts03SkillForPreview
                ? state.EffectiveSkillCooldownRounds()
                : 0;
            if (scoutEquipped && !state.HasReadyArts03SkillForPreview)
            {
                return new CombatActionPreview(CombatAction.Skill, "정찰", "다음 공격 강화 / 다음 피해 감소 1회", true);
            }

            var preview = "사용 가능";
            var skillDamageBonus = state.GetSkillItemDamageBonusForPreview();
            if (skillDamageBonus > 0)
            {
                preview += " / 피해 +" + skillDamageBonus;
            }

            if (state.HasGenericSkillOpeningForPreview())
            {
                preview += " / 빈틈 +" + state.GetGenericSkillOpeningDamageBonusForPreview();
            }

            return new CombatActionPreview(CombatAction.Skill, label, preview + " / 사용 후 CD " + cooldownAfterUse, true);
        }

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
                state.RecordNpcReaction(choice.npcReactionKey);
                return new PrototypeEncounterChoiceResolution(choice.stableId, false, "choice failed: requirements not met");
            }

            if (encounter.Id == "EVT_F01_JAR_ROOM")
            {
                return state.ResolveJarRoomChoice(context, nodeId, choice.stableId);
            }

            if (encounter.Type == EncounterType.Rest && choice.stableId.Contains("_REST"))
            {
                return state.ResolveRestChoice(choice.stableId);
            }

            var visibleChoices = BuildVisibleChoiceSummary(state, encounter);
            var applied = 0;
            var ignored = 0;
            var effectSummary = string.Empty;
            var effects = choice.effects ?? new EncounterEffectRuntimeData[0];
            for (var i = 0; i < effects.Length; i++)
            {
                if (ApplyEffect(state, encounter, effects[i], context, nodeId, out var effectDetail))
                {
                    applied++;
                    AppendSummary(ref effectSummary, effectDetail);
                }
                else
                {
                    ignored++;
                }
            }

            var message = $"choices: {visibleChoices} | choice applied: {choice.stableId}; effects={applied}; ignored={ignored}";
            if (!string.IsNullOrEmpty(effectSummary))
            {
                message += " | " + effectSummary;
            }

            state.RecordNpcReaction(choice.npcReactionKey);
            return new PrototypeEncounterChoiceResolution(choice.stableId, true, message);
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
                    met ? string.Empty : choices[i].unavailableReasonTextKey,
                    BuildChoiceHint(state, encounter, choices[i], met));
            }

            return views;
        }

        private static string BuildChoiceHint(PrototypeRunState state, EncounterData encounter, EncounterChoiceRuntimeData choice, bool requirementsMet)
        {
            if (choice == null)
            {
                return string.Empty;
            }

            if (!requirementsMet)
            {
                var reason = BuildUnavailableReason(choice, state);
                var unavailable = string.IsNullOrEmpty(reason) ? "Unavailable" : "Unavailable: " + reason;
                if (encounter != null && encounter.Type == EncounterType.Shop)
                {
                    var summary = BuildChoiceEffectSummary(choice);
                    return string.IsNullOrEmpty(summary) ? unavailable : summary + "\n" + unavailable;
                }

                return unavailable;
            }

            if (encounter != null && encounter.Id == "EVT_F01_JAR_ROOM")
            {
                return BuildJarRoomChoiceHint(choice.stableId);
            }

            return BuildChoiceEffectSummary(choice);
        }

        private static string BuildChoiceEffectSummary(EncounterChoiceRuntimeData choice)
        {
            var summary = string.Empty;
            var effects = choice.effects ?? new EncounterEffectRuntimeData[0];
            for (var i = 0; i < effects.Length; i++)
            {
                AppendSummary(ref summary, DescribeEffectForChoiceHint(effects[i]));
            }

            return summary;
        }

        private static string BuildJarRoomChoiceHint(string choiceStableId)
        {
            return choiceStableId switch
            {
                "CHOICE_EVT_F01_JAR_ROOM_PATTERNED" => "80%: 골드 획득\n20%: 엘리트 전투",
                "CHOICE_EVT_F01_JAR_ROOM_PATTERNED_ELITE_COMBAT" => "80%: 골드 획득\n20%: 엘리트 전투",
                "CHOICE_EVT_F01_JAR_ROOM_PLAIN" => "HP 회복\n이성 회복",
                "CHOICE_EVT_F01_JAR_ROOM_CRACKED" => "다음 3회 전투 피해 증가",
                _ => string.Empty
            };
        }

        private static string BuildUnavailableReason(EncounterChoiceRuntimeData choice, PrototypeRunState state)
        {
            if (ChoiceAddsOwnedAbility(state, choice))
            {
                return "이미 보유";
            }

            var requirements = choice.requirements ?? new EncounterRequirementRuntimeData[0];
            for (var i = 0; i < requirements.Length; i++)
            {
                var requirement = requirements[i];
                if (requirement == null)
                {
                    continue;
                }

                if (requirement.kind == "StatAtLeast" && requirement.stat == "gold")
                {
                    return "Gold 부족";
                }

                if (requirement.kind == "HasItem")
                {
                    return "Item 필요";
                }

                if (requirement.kind == "HasAbility")
                {
                    return "Ability 필요";
                }
            }

            return string.Empty;
        }

        private static string DescribeEffectForChoiceHint(EncounterEffectRuntimeData effect)
        {
            if (effect == null)
            {
                return string.Empty;
            }

            switch (effect.kind)
            {
                case "ModifyHp":
                    return FormatDelta("HP", effect.amount);
                case "TriggerGameOver":
                    return "실패";
                case "ModifyMental":
                    return FormatDelta("Mental", effect.amount);
                case "ModifyGold":
                    return FormatDelta("Gold", effect.amount);
                case "ModifyGlitchLevel":
                    return FormatDelta("Glitch", effect.amount);
                case "ModifyAffinity":
                    return FormatDelta("Affinity", effect.amount);
                case "AddItem":
                    return string.IsNullOrEmpty(effect.itemRef) ? "Item" : effect.itemRef;
                case "RemoveItem":
                    return string.IsNullOrEmpty(effect.itemRef) ? "Item -" + System.Math.Max(1, effect.count) : effect.itemRef + " -" + System.Math.Max(1, effect.count);
                case "AddAbility":
                    return string.IsNullOrEmpty(effect.abilityRef) ? "Ability" : effect.abilityRef;
                case "GrantRewardBundle":
                    return PrototypeRunState.IsMemoryConsequenceRewardBundleRef(effect.rewardBundleRef)
                        ? PrototypeRunState.MemoryConsequenceFeedback
                        : string.IsNullOrEmpty(effect.rewardBundleRef) ? "Reward" : effect.rewardBundleRef;
                case "UnlockMemoryFragment":
                    return "기억의 잔향";
                case "StartCombat":
                    return "Combat start";
                default:
                    return string.Empty;
            }
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
                return !ChoiceAddsOwnedAbility(state, choice);
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

            return (anyMode ? matched > 0 : true) && !ChoiceAddsOwnedAbility(state, choice);
        }

        private static bool ChoiceAddsOwnedAbility(PrototypeRunState state, EncounterChoiceRuntimeData choice)
        {
            if (state == null || choice == null || choice.effects == null)
            {
                return false;
            }

            for (var i = 0; i < choice.effects.Length; i++)
            {
                var effect = choice.effects[i];
                if (effect != null &&
                    effect.kind == "AddAbility" &&
                    !string.IsNullOrEmpty(effect.abilityRef) &&
                    state.HasAbilityRef(effect.abilityRef))
                {
                    return true;
                }
            }

            return false;
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

        private static bool ApplyEffect(PrototypeRunState state, EncounterData encounter, EncounterEffectRuntimeData effect, DeterministicRunContext context, string nodeId, out string summary)
        {
            summary = string.Empty;
            if (effect == null)
            {
                return false;
            }

            switch (effect.kind)
            {
                case "ModifyHp":
                    state.ModifyPlayerHp(effect.amount);
                    summary = FormatDelta("HP", effect.amount);
                    return true;
                case "TriggerGameOver":
                    state.TriggerGameOver();
                    summary = "run.failed";
                    return true;
                case "ModifyMental":
                    state.ModifyMental(effect.amount);
                    summary = FormatDelta("Mental", effect.amount);
                    return true;
                case "ModifyGold":
                    state.ModifyGold(effect.amount);
                    summary = FormatDelta("Gold", effect.amount);
                    return true;
                case "ModifyGlitchLevel":
                    state.ModifyGlitchLevel(effect.amount);
                    summary = FormatDelta("Glitch", effect.amount);
                    return true;
                case "ModifyAffinity":
                    state.ModifyAffinity(effect.amount);
                    summary = FormatDelta("Affinity", effect.amount);
                    return true;
                case "SetFlag":
                    state.SetFlag(effect.flag, effect.value);
                    summary = "flag " + effect.flag + "=" + effect.value;
                    return true;
                case "AddItem":
                    state.AddItemRef(effect.itemRef, System.Math.Max(1, effect.count));
                    summary = "item " + effect.itemRef + " +" + System.Math.Max(1, effect.count);
                    return true;
                case "RemoveItem":
                    state.AddItemRef(effect.itemRef, -System.Math.Max(1, effect.count));
                    summary = "item " + effect.itemRef + " -" + System.Math.Max(1, effect.count);
                    return true;
                case "AddAbility":
                    var abilityAdded = state.AddAbilityRef(effect.abilityRef);
                    summary = abilityAdded ? "ability " + effect.abilityRef : string.Empty;
                    return abilityAdded;
                case "GrantRewardBundle":
                    var rewardGranted = state.GrantRewardBundleRef(effect.rewardBundleRef);
                    summary = rewardGranted
                        ? PrototypeRunState.IsMemoryConsequenceRewardBundleRef(effect.rewardBundleRef)
                            ? PrototypeRunState.MemoryConsequenceFeedback
                            : "reward " + effect.rewardBundleRef
                        : string.Empty;
                    return rewardGranted;
                case "UnlockMemoryFragment":
                    var unlocked = state.UnlockMemoryFragmentRef(effect.memoryFragmentId);
                    summary = unlocked ? PrototypeRunState.MemoryFragmentPublicFeedback : string.Empty;
                    return unlocked;
                case "StartCombat":
                    var combat = state.ResolveCombatHandoff(context, nodeId, encounter, effect.combatHandoff, ApplyPostCombatEffects);
                    if (combat.Applied)
                    {
                        summary = BuildCombatSummary(combat, effect.combatHandoff);
                    }

                    return combat.Applied;
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
                case "TriggerGameOver":
                    state.TriggerGameOver();
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

        private static string BuildCombatSummary(PrototypeCombatHandoffResolution combat, EncounterCombatHandoffRuntimeData handoff)
        {
            var combatId = string.IsNullOrEmpty(combat.CombatStableId) ? "combat" : combat.CombatStableId;
            var enemyId = string.IsNullOrEmpty(combat.EnemyId) ? "enemy.placeholder" : combat.EnemyId;
            var resultId = string.IsNullOrEmpty(combat.ResultId) ? "unknown" : combat.ResultId;
            var summary = "combat started " + combatId + "; enemy " + enemyId + "; result " + resultId;
            var postEffects = handoff == null
                ? null
                : resultId == "victory"
                    ? handoff.onVictoryEffects
                    : handoff.onDefeatEffects;
            var postSummary = BuildPostCombatEffectSummary(postEffects);
            return string.IsNullOrEmpty(postSummary) ? summary : summary + "; " + postSummary;
        }

        private static string BuildPostCombatEffectSummary(EncounterPostCombatEffectRuntimeData[] effects)
        {
            if (effects == null || effects.Length == 0)
            {
                return string.Empty;
            }

            var summary = string.Empty;
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i];
                if (effect == null)
                {
                    continue;
                }

                AppendSummary(ref summary, DescribePostCombatEffect(effect));
            }

            return summary;
        }

        private static string DescribePostCombatEffect(EncounterPostCombatEffectRuntimeData effect)
        {
            switch (effect.kind)
            {
                case "ModifyHp":
                    return FormatDelta("HP", effect.amount);
                case "TriggerGameOver":
                    return "실패";
                case "ModifyMental":
                    return FormatDelta("Mental", effect.amount);
                case "ModifyGold":
                    return FormatDelta("Gold", effect.amount);
                case "ModifyGlitchLevel":
                    return FormatDelta("Glitch", effect.amount);
                case "ModifyAffinity":
                    return FormatDelta("Affinity", effect.amount);
                case "SetFlag":
                    return "flag " + effect.flag + "=" + effect.value;
                case "AddItem":
                    return "item " + effect.itemRef + " +" + System.Math.Max(1, effect.count);
                case "RemoveItem":
                    return "item " + effect.itemRef + " -" + System.Math.Max(1, effect.count);
                case "AddAbility":
                    return "ability " + effect.abilityRef;
                case "GrantRewardBundle":
                    return PrototypeRunState.IsMemoryConsequenceRewardBundleRef(effect.rewardBundleRef)
                        ? PrototypeRunState.MemoryConsequenceFeedback
                        : "reward " + effect.rewardBundleRef;
                case "UnlockMemoryFragment":
                    return PrototypeRunState.MemoryFragmentPublicFeedback;
                default:
                    return string.Empty;
            }
        }

        private static string FormatDelta(string label, int amount)
        {
            return label + " " + (amount >= 0 ? "+" : string.Empty) + amount;
        }

        private static void AppendSummary(ref string summary, string detail)
        {
            if (string.IsNullOrEmpty(detail))
            {
                return;
            }

            if (!string.IsNullOrEmpty(summary))
            {
                summary += ", ";
            }

            summary += detail;
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
