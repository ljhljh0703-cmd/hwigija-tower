using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using HwigiTower.Combat;
using HwigiTower.Core;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.EditorTools
{
    public static class TrainingCombatBaselineExporter
    {
        private const int EpisodeCount = 1000;
        private const int PlayerMaxHp = 24;
        private const int PlayerAttack = 6;
        private const int EnemyMaxHp = 20;
        private const int EnemyAttack = 5;
        private const int SkillDamage = 8;
        private const int MaxDecisionSteps = 12;
        private const string OutputDirectory = "Docs/Portfolio/assets";
        private const string CsvPath = OutputDirectory + "/mlagents_exp02_baseline_results.csv";
        private const string JsonPath = OutputDirectory + "/mlagents_exp02_baseline_results.json";
        private const string ComparisonCsvPath = OutputDirectory + "/mlagents_exp02_policy_comparison.csv";

        private static readonly PolicyDefinition[] Policies =
        {
            new PolicyDefinition("RandomPolicy", SelectRandomAction, "Uniform random action choice with deterministic per-episode seed."),
            new PolicyDefinition("AttackSpamPolicy", SelectAttack, "Always Attack."),
            new PolicyDefinition("SkillSpamPolicy", SelectSkill, "Always Skill."),
            new PolicyDefinition("DefendHeavyPolicy", SelectDefendHeavy, "Defend for tempo, Attack every fourth decision, Skill as finisher.")
        };

        [MenuItem("Hwigi Tower/Training/Export Combat Baselines")]
        public static void ExportCombatBaselines()
        {
            Directory.CreateDirectory(OutputDirectory);

            var summaries = new List<PolicySummary>();
            for (var i = 0; i < Policies.Length; i++)
            {
                summaries.Add(EvaluatePolicy(Policies[i]));
            }

            File.WriteAllText(CsvPath, BuildBaselineCsv(summaries));
            File.WriteAllText(JsonPath, JsonUtility.ToJson(new BaselineReport(summaries.ToArray()), true));
            File.WriteAllText(ComparisonCsvPath, BuildComparisonCsv(summaries));

            Debug.Log("Training combat baselines exported: " + CsvPath);
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
            var wins = 0;
            var losses = 0;
            var timeouts = 0;

            for (var episode = 0; episode < EpisodeCount; episode++)
            {
                var context = new DeterministicRunContext("training-combat-baseline", 9101 + episode);
                var combat = new CombatController(context, "baseline." + policy.Name + "." + episode);
                var policyRandom = context.CreateRandom("policy." + policy.Name + "." + episode);
                var player = new CombatantState("baseline.player", PlayerMaxHp, PlayerAttack);
                var enemy = new CombatantState("baseline.enemy", EnemyMaxHp, EnemyAttack);
                var lastAction = CombatAction.Attack;
                var episodeReward = 0f;
                var stepIndex = 0;

                for (; stepIndex < MaxDecisionSteps; stepIndex++)
                {
                    var action = policy.Select(stepIndex, player, enemy, lastAction, policyRandom);
                    CountAction(action, ref attackCount, ref defendCount, ref skillCount);
                    totalActions++;

                    var playerHpBefore = player.Hp;
                    var enemyHpBefore = enemy.Hp;
                    var result = combat.ResolveRound(
                        player,
                        enemy,
                        action,
                        null,
                        action == CombatAction.Skill ? SkillDamage : null);

                    var enemyDamage = Mathf.Max(0, enemyHpBefore - enemy.Hp);
                    var playerDamage = Mathf.Max(0, playerHpBefore - player.Hp);
                    totalEnemyDamage += enemyDamage;
                    totalPlayerDamage += playerDamage;
                    totalDamagePrevented += result.PlayerDamagePrevented;

                    episodeReward += enemyDamage * 0.05f;
                    episodeReward += playerDamage * -0.03f;
                    episodeReward += action == CombatAction.Defend && result.PlayerDamagePrevented > 0 ? 0.05f : 0f;
                    episodeReward += -0.01f;

                    lastAction = action;

                    if (result.EnemyDefeated)
                    {
                        episodeReward += 1f;
                        wins++;
                        stepIndex++;
                        break;
                    }

                    if (result.PlayerDefeated)
                    {
                        episodeReward += -1f;
                        losses++;
                        stepIndex++;
                        break;
                    }
                }

                if (!player.IsDefeated && !enemy.IsDefeated)
                {
                    episodeReward += -0.25f;
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
                enemyDamagePerStep = totalEnemyDamage / Mathf.Max(1, totalActions),
                playerDamagePerStep = totalPlayerDamage / Mathf.Max(1, totalActions),
                damagePreventedPerStep = totalDamagePrevented / Mathf.Max(1, totalActions),
                failureMode = ResolveFailureMode(policy.Name, wins, losses, timeouts, defendCount, skillCount, totalActions)
            };
        }

        private static string BuildBaselineCsv(IReadOnlyList<PolicySummary> summaries)
        {
            var builder = new StringBuilder();
            builder.AppendLine("policy,episodes,cumulative_reward,episode_length,win_proxy,loss_proxy,timeout_proxy,attack_share,defend_share,skill_share,enemy_damage_per_step,player_damage_per_step,damage_prevented_per_step,failure_mode");
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
                    .Append(Format(item.enemyDamagePerStep)).Append(',')
                    .Append(Format(item.playerDamagePerStep)).Append(',')
                    .Append(Format(item.damagePreventedPerStep)).Append(',')
                    .Append(EscapeCsv(item.failureMode))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static string BuildComparisonCsv(IReadOnlyList<PolicySummary> summaries)
        {
            var builder = new StringBuilder();
            builder.AppendLine("policy,source,cumulative_reward,episode_length,win_proxy,attack_share,defend_share,skill_share,policy_failure_mode");
            builder.AppendLine("PPO10k,TensorBoard final scalar,1.605229,2.055046,1.000000,0.266310,0.013761,0.719929,Skill-heavy learned policy; Defend collapsed.");
            for (var i = 0; i < summaries.Count; i++)
            {
                var item = summaries[i];
                builder.Append(item.policy).Append(",baseline evaluator,")
                    .Append(Format(item.cumulativeReward)).Append(',')
                    .Append(Format(item.episodeLength)).Append(',')
                    .Append(Format(item.winProxy)).Append(',')
                    .Append(Format(item.attackShare)).Append(',')
                    .Append(Format(item.defendShare)).Append(',')
                    .Append(Format(item.skillShare)).Append(',')
                    .Append(EscapeCsv(item.failureMode))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static CombatAction SelectRandomAction(int stepIndex, CombatantState player, CombatantState enemy, CombatAction lastAction, DeterministicRandom random)
        {
            return (CombatAction)random.Range(0, 3);
        }

        private static CombatAction SelectAttack(int stepIndex, CombatantState player, CombatantState enemy, CombatAction lastAction, DeterministicRandom random)
        {
            return CombatAction.Attack;
        }

        private static CombatAction SelectSkill(int stepIndex, CombatantState player, CombatantState enemy, CombatAction lastAction, DeterministicRandom random)
        {
            return CombatAction.Skill;
        }

        private static CombatAction SelectDefendHeavy(int stepIndex, CombatantState player, CombatantState enemy, CombatAction lastAction, DeterministicRandom random)
        {
            if (enemy.Hp <= SkillDamage)
            {
                return CombatAction.Skill;
            }

            return stepIndex % 4 == 3 ? CombatAction.Attack : CombatAction.Defend;
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

        private static string ResolveFailureMode(string policyName, int wins, int losses, int timeouts, int defendCount, int skillCount, int totalActions)
        {
            var winRate = wins / (float)EpisodeCount;
            var timeoutRate = timeouts / (float)EpisodeCount;
            var skillShare = skillCount / (float)Mathf.Max(1, totalActions);
            var defendShare = defendCount / (float)Mathf.Max(1, totalActions);

            if (policyName == "SkillSpamPolicy")
            {
                return "Skill spam solves the current short duel quickly, exposing weak Skill cost/context.";
            }

            if (policyName == "AttackSpamPolicy")
            {
                return winRate >= 0.95f
                    ? "Attack spam is already strong, so PPO must beat speed/reward rather than just win rate."
                    : "Attack spam loses enough to justify policy learning.";
            }

            if (policyName == "DefendHeavyPolicy")
            {
                return timeoutRate > 0.25f || defendShare > 0.5f && winRate < 0.95f
                    ? "Defend extends survival but lacks payoff under current reward/enemy pressure."
                    : "Defend-heavy play remains viable but slower than damage-focused policies.";
            }

            return skillShare > 0.45f
                ? "Random baseline benefits when it samples Skill often enough."
                : "Random baseline is noisy and not action-efficient.";
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
            DeterministicRandom random);

        [Serializable]
        private sealed class BaselineReport
        {
            public BaselineReport(PolicySummary[] policies)
            {
                runId = "hwigi_training_combat_exp02_baselines";
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
            public float enemyDamagePerStep;
            public float playerDamagePerStep;
            public float damagePreventedPerStep;
            public string failureMode;
        }
    }
}
