using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.Training;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.EditorTools
{
    public static class TrainingCombatExp03BaselineExporter
    {
        private const int EpisodeCount = 1000;
        private const string OutputDirectory = "Docs/Portfolio/assets";
        private const string CsvPath = OutputDirectory + "/mlagents_exp03_baseline_results.csv";
        private const string JsonPath = OutputDirectory + "/mlagents_exp03_baseline_results.json";

        private static readonly PolicyDefinition[] Policies =
        {
            new PolicyDefinition("RandomPolicy", SelectRandomAction, "Uniform valid action choice with deterministic per-episode seed."),
            new PolicyDefinition("AttackSpamPolicy", SelectAttack, "Always Attack."),
            new PolicyDefinition("SkillSpamPolicy", SelectSkill, "Always requests Skill; cooldown turns fall back to Attack."),
            new PolicyDefinition("DefendHeavyPolicy", SelectDefendHeavy, "Defend for tempo, Attack every fourth decision, Skill as finisher."),
            new PolicyDefinition("ContextPolicy", SelectContextAction, "Use Skill only as a ready finisher, Defend at low HP, otherwise Attack.")
        };

        [MenuItem("Hwigi Tower/Training/Export Combat Exp03 Baselines")]
        public static void ExportCombatExp03Baselines()
        {
            Directory.CreateDirectory(OutputDirectory);

            var summaries = new List<PolicySummary>();
            for (var i = 0; i < Policies.Length; i++)
            {
                summaries.Add(EvaluatePolicy(Policies[i]));
            }

            File.WriteAllText(CsvPath, BuildBaselineCsv(summaries));
            File.WriteAllText(JsonPath, JsonUtility.ToJson(new BaselineReport(summaries.ToArray()), true));

            Debug.Log("Training combat Exp03 baselines exported: " + CsvPath);
        }

        private static PolicySummary EvaluatePolicy(PolicyDefinition policy)
        {
            var totalReward = 0f;
            var totalEpisodeLength = 0f;
            var totalEnemyDamage = 0f;
            var totalPlayerDamage = 0f;
            var totalDamagePrevented = 0f;
            var totalActions = 0;
            var attackCount = 0;
            var defendCount = 0;
            var skillCount = 0;
            var skillBlockedCount = 0;
            var wins = 0;
            var losses = 0;
            var timeouts = 0;

            for (var episode = 0; episode < EpisodeCount; episode++)
            {
                var random = new DeterministicRandom(
                    DeterministicSeed.Combine(9401 + episode, "exp03.baseline." + policy.Name + "." + episode));
                var policyRandom = new DeterministicRandom(
                    DeterministicSeed.Combine(9501 + episode, "exp03.policy." + policy.Name + "." + episode));
                var player = new CombatantState(
                    "baseline.player",
                    CombatTrainingRules.PlayerMaxHp,
                    CombatTrainingRules.PlayerAttack);
                var enemy = new CombatantState(
                    "baseline.enemy",
                    CombatTrainingRules.EnemyMaxHp,
                    CombatTrainingRules.EnemyAttack);
                var lastAction = CombatAction.Attack;
                var skillCooldownRemaining = 0;
                var episodeReward = 0f;
                var stepIndex = 0;

                for (; stepIndex < CombatTrainingRules.MaxDecisionSteps; stepIndex++)
                {
                    var requestedAction = policy.Select(
                        stepIndex,
                        player,
                        enemy,
                        lastAction,
                        skillCooldownRemaining,
                        policyRandom);
                    var result = CombatTrainingRules.ResolveRound(
                        player,
                        enemy,
                        requestedAction,
                        skillCooldownRemaining,
                        random);

                    CountAction(result.Action, ref attackCount, ref defendCount, ref skillCount);
                    skillBlockedCount += result.SkillBlocked ? 1 : 0;
                    totalActions++;
                    totalEnemyDamage += result.EnemyDamage;
                    totalPlayerDamage += result.PlayerDamage;
                    totalDamagePrevented += result.DamagePrevented;
                    episodeReward += CombatTrainingRules.CalculateStepReward(result);

                    lastAction = result.Action;
                    skillCooldownRemaining = CombatTrainingRules.NextSkillCooldown(skillCooldownRemaining, result.Action);

                    if (result.EnemyDefeated)
                    {
                        episodeReward += CombatTrainingRules.WinReward;
                        wins++;
                        stepIndex++;
                        break;
                    }

                    if (result.PlayerDefeated)
                    {
                        episodeReward += CombatTrainingRules.LossPenalty;
                        losses++;
                        stepIndex++;
                        break;
                    }
                }

                if (!player.IsDefeated && !enemy.IsDefeated)
                {
                    episodeReward += CombatTrainingRules.TimeoutPenalty;
                    timeouts++;
                }

                totalReward += episodeReward;
                totalEpisodeLength += stepIndex;
            }

            return new PolicySummary
            {
                policy = policy.Name,
                definition = policy.Definition,
                episodes = EpisodeCount,
                cumulativeReward = totalReward / EpisodeCount,
                episodeLength = totalEpisodeLength / EpisodeCount,
                winProxy = wins / (float)EpisodeCount,
                lossProxy = losses / (float)EpisodeCount,
                timeoutProxy = timeouts / (float)EpisodeCount,
                attackShare = attackCount / (float)Mathf.Max(1, totalActions),
                defendShare = defendCount / (float)Mathf.Max(1, totalActions),
                skillShare = skillCount / (float)Mathf.Max(1, totalActions),
                skillBlockedShare = skillBlockedCount / (float)Mathf.Max(1, totalActions),
                enemyDamagePerStep = totalEnemyDamage / Mathf.Max(1, totalActions),
                playerDamagePerStep = totalPlayerDamage / Mathf.Max(1, totalActions),
                damagePreventedPerStep = totalDamagePrevented / Mathf.Max(1, totalActions),
                failureMode = ResolveFailureMode(policy.Name, wins, losses, timeouts, skillBlockedCount, totalActions)
            };
        }

        private static string BuildBaselineCsv(IReadOnlyList<PolicySummary> summaries)
        {
            var builder = new StringBuilder();
            builder.AppendLine("policy,episodes,cumulative_reward,episode_length,win_proxy,loss_proxy,timeout_proxy,attack_share,defend_share,skill_share,skill_blocked_share,enemy_damage_per_step,player_damage_per_step,damage_prevented_per_step,failure_mode");
            for (var i = 0; i < summaries.Count; i++)
            {
                var item = summaries[i];
                builder.Append(item.policy).Append(',')
                    .Append(item.episodes.ToString(CultureInfo.InvariantCulture)).Append(',')
                    .Append(Format(item.cumulativeReward)).Append(',')
                    .Append(Format(item.episodeLength)).Append(',')
                    .Append(Format(item.winProxy)).Append(',')
                    .Append(Format(item.lossProxy)).Append(',')
                    .Append(Format(item.timeoutProxy)).Append(',')
                    .Append(Format(item.attackShare)).Append(',')
                    .Append(Format(item.defendShare)).Append(',')
                    .Append(Format(item.skillShare)).Append(',')
                    .Append(Format(item.skillBlockedShare)).Append(',')
                    .Append(Format(item.enemyDamagePerStep)).Append(',')
                    .Append(Format(item.playerDamagePerStep)).Append(',')
                    .Append(Format(item.damagePreventedPerStep)).Append(',')
                    .Append(EscapeCsv(item.failureMode))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static CombatAction SelectRandomAction(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            DeterministicRandom random)
        {
            return CombatTrainingRules.CanUseSkill(skillCooldownRemaining)
                ? CombatTrainingRules.ResolveAction(random.Range(0, 3))
                : CombatTrainingRules.ResolveAction(random.Range(0, 2));
        }

        private static CombatAction SelectAttack(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            DeterministicRandom random)
        {
            return CombatAction.Attack;
        }

        private static CombatAction SelectSkill(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            DeterministicRandom random)
        {
            return CombatAction.Skill;
        }

        private static CombatAction SelectDefendHeavy(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            DeterministicRandom random)
        {
            if (enemy.Hp <= CombatTrainingRules.SkillDamage &&
                CombatTrainingRules.CanUseSkill(skillCooldownRemaining))
            {
                return CombatAction.Skill;
            }

            return stepIndex % 4 == 3 ? CombatAction.Attack : CombatAction.Defend;
        }

        private static CombatAction SelectContextAction(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            DeterministicRandom random)
        {
            if (enemy.Hp <= CombatTrainingRules.SkillDamage &&
                CombatTrainingRules.CanUseSkill(skillCooldownRemaining))
            {
                return CombatAction.Skill;
            }

            return player.Hp <= 8 ? CombatAction.Defend : CombatAction.Attack;
        }

        private static void CountAction(CombatAction action, ref int attackCount, ref int defendCount, ref int skillCount)
        {
            switch (action)
            {
                case CombatAction.Defend:
                    defendCount++;
                    break;
                case CombatAction.Skill:
                    skillCount++;
                    break;
                default:
                    attackCount++;
                    break;
            }
        }

        private static string ResolveFailureMode(
            string policyName,
            int wins,
            int losses,
            int timeouts,
            int skillBlockedCount,
            int totalActions)
        {
            var winRate = wins / (float)EpisodeCount;
            var skillBlockedShare = skillBlockedCount / (float)Mathf.Max(1, totalActions);

            if (policyName == "SkillSpamPolicy")
            {
                return skillBlockedShare > 0.25f
                    ? "Cooldown blocks repeated Skill, forcing fallback attacks between Skill turns."
                    : "Skill spam still bypasses the context gate.";
            }

            if (policyName == "DefendHeavyPolicy")
            {
                return winRate < 0.5f
                    ? "Defend now produces prevention metric but still lacks enough offensive tempo."
                    : "Defend-heavy survives while preserving enough damage tempo.";
            }

            if (policyName == "ContextPolicy")
            {
                return winRate >= 0.95f
                    ? "Simple gated heuristic is a useful non-learning target."
                    : "Context heuristic is too conservative for the current duel.";
            }

            if (policyName == "AttackSpamPolicy")
            {
                return winRate >= 0.95f
                    ? "Attack remains a strong simple baseline even after Skill gate."
                    : "Attack no longer solves the reshaped environment.";
            }

            return losses > timeouts ? "Random loses through poor action timing." : "Random is slow and noisy.";
        }

        private static string Format(float value)
        {
            return value.ToString("0.000000", CultureInfo.InvariantCulture);
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private sealed class PolicyDefinition
        {
            public PolicyDefinition(string name, PolicySelector select, string definition)
            {
                Name = name;
                Select = select;
                Definition = definition;
            }

            public string Name { get; }
            public PolicySelector Select { get; }
            public string Definition { get; }
        }

        private delegate CombatAction PolicySelector(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            DeterministicRandom random);

        [Serializable]
        private sealed class BaselineReport
        {
            public BaselineReport(PolicySummary[] policies)
            {
                runId = "hwigi_training_combat_exp03_baselines";
                episodesPerPolicy = EpisodeCount;
                policiesEvaluated = policies;
            }

            public string runId;
            public int episodesPerPolicy;
            public PolicySummary[] policiesEvaluated;
        }

        [Serializable]
        private sealed class PolicySummary
        {
            public string policy;
            public string definition;
            public int episodes;
            public float cumulativeReward;
            public float episodeLength;
            public float winProxy;
            public float lossProxy;
            public float timeoutProxy;
            public float attackShare;
            public float defendShare;
            public float skillShare;
            public float skillBlockedShare;
            public float enemyDamagePerStep;
            public float playerDamagePerStep;
            public float damagePreventedPerStep;
            public string failureMode;
        }
    }
}
