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
    public static class TrainingCombatExp04BaselineExporter
    {
        private const int EpisodeCount = 1000;
        private const string OutputDirectory = "Docs/Portfolio/assets";
        private const string CsvPath = OutputDirectory + "/mlagents_exp04_baseline_results.csv";
        private const string JsonPath = OutputDirectory + "/mlagents_exp04_baseline_results.json";

        private static readonly PolicyDefinition[] Policies =
        {
            new PolicyDefinition("RandomPolicy", SelectRandomAction, "Uniform valid action choice with deterministic per-episode seed."),
            new PolicyDefinition("AttackSpamPolicy", SelectAttack, "Always Attack."),
            new PolicyDefinition("SkillSpamPolicy", SelectSkill, "Always requests Skill; cooldown turns fall back to Attack."),
            new PolicyDefinition("DefendHeavyPolicy", SelectDefendHeavy, "Defend on threat and filler turns, Attack every third non-threat turn."),
            new PolicyDefinition("ContextPolicy", SelectContextAction, "Defend high threat, Skill high HP when ready, spend tempo Attack bonus, otherwise Attack.")
        };

        [MenuItem("Hwigi Tower/Training/Export Combat Exp04 Baselines")]
        public static void ExportCombatExp04Baselines()
        {
            Directory.CreateDirectory(OutputDirectory);

            var summaries = new List<PolicySummary>();
            for (var i = 0; i < Policies.Length; i++)
            {
                summaries.Add(EvaluatePolicy(Policies[i]));
            }

            File.WriteAllText(CsvPath, BuildBaselineCsv(summaries));
            File.WriteAllText(JsonPath, JsonUtility.ToJson(new BaselineReport(summaries.ToArray()), true));

            Debug.Log("Training combat Exp04 baselines exported: " + CsvPath);
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
            var skillWastedCount = 0;
            var defendAttemptCount = 0;
            var defendSuccessCount = 0;
            var tempoBonusTriggeredCount = 0;
            var diversityTotal = 0f;
            var wins = 0;
            var losses = 0;
            var timeouts = 0;

            for (var episode = 0; episode < EpisodeCount; episode++)
            {
                var random = new DeterministicRandom(
                    DeterministicSeed.Combine(10401 + episode, "exp04.baseline." + policy.Name + "." + episode));
                var policyRandom = new DeterministicRandom(
                    DeterministicSeed.Combine(10501 + episode, "exp04.policy." + policy.Name + "." + episode));
                var player = new CombatantState(
                    "baseline.player",
                    CombatTrainingExp04Rules.PlayerMaxHp,
                    CombatTrainingExp04Rules.PlayerAttack);
                var enemy = new CombatantState(
                    "baseline.enemy",
                    CombatTrainingExp04Rules.EnemyMaxHp,
                    CombatTrainingExp04Rules.EnemyAttack);
                var lastAction = CombatAction.Attack;
                var skillCooldownRemaining = 0;
                var tempoAttackReady = false;
                var episodeReward = 0f;
                var episodeAttackCount = 0;
                var episodeDefendCount = 0;
                var episodeSkillCount = 0;
                var stepIndex = 0;

                for (; stepIndex < CombatTrainingExp04Rules.MaxDecisionSteps; stepIndex++)
                {
                    var requestedAction = policy.Select(
                        stepIndex,
                        player,
                        enemy,
                        lastAction,
                        skillCooldownRemaining,
                        tempoAttackReady,
                        policyRandom);
                    var result = CombatTrainingExp04Rules.ResolveRound(
                        player,
                        enemy,
                        requestedAction,
                        skillCooldownRemaining,
                        tempoAttackReady,
                        stepIndex,
                        random);

                    CountAction(result.Action, ref attackCount, ref defendCount, ref skillCount);
                    CountAction(result.Action, ref episodeAttackCount, ref episodeDefendCount, ref episodeSkillCount);
                    skillBlockedCount += result.SkillBlocked ? 1 : 0;
                    skillWastedCount += result.SkillWasted ? 1 : 0;
                    defendAttemptCount += result.Action == CombatAction.Defend ? 1 : 0;
                    defendSuccessCount += result.DefendSucceeded ? 1 : 0;
                    tempoBonusTriggeredCount += result.TempoBonusTriggered ? 1 : 0;
                    totalActions++;
                    totalEnemyDamage += result.EnemyDamage;
                    totalPlayerDamage += result.PlayerDamage;
                    totalDamagePrevented += result.DamagePrevented;
                    episodeReward += CombatTrainingExp04Rules.CalculateStepReward(result);

                    lastAction = result.Action;
                    skillCooldownRemaining = CombatTrainingExp04Rules.NextSkillCooldown(skillCooldownRemaining, result.Action);
                    tempoAttackReady = CombatTrainingExp04Rules.NextTempoAttackReady(tempoAttackReady, result);

                    if (result.EnemyDefeated)
                    {
                        episodeReward += CombatTrainingExp04Rules.WinReward;
                        wins++;
                        stepIndex++;
                        break;
                    }

                    if (result.PlayerDefeated)
                    {
                        episodeReward += CombatTrainingExp04Rules.LossPenalty;
                        losses++;
                        stepIndex++;
                        break;
                    }
                }

                if (!player.IsDefeated && !enemy.IsDefeated)
                {
                    episodeReward += CombatTrainingExp04Rules.TimeoutPenalty;
                    timeouts++;
                }

                var actionTypesUsed = (episodeAttackCount > 0 ? 1 : 0) +
                    (episodeDefendCount > 0 ? 1 : 0) +
                    (episodeSkillCount > 0 ? 1 : 0);
                diversityTotal += actionTypesUsed / 3f;
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
                skillWastedPerEpisode = skillWastedCount / (float)EpisodeCount,
                defendSuccessRate = defendSuccessCount / (float)Mathf.Max(1, defendAttemptCount),
                tempoBonusTriggeredPerEpisode = tempoBonusTriggeredCount / (float)EpisodeCount,
                actionDiversity = diversityTotal / EpisodeCount,
                enemyDamagePerStep = totalEnemyDamage / Mathf.Max(1, totalActions),
                playerDamagePerStep = totalPlayerDamage / Mathf.Max(1, totalActions),
                damagePreventedPerStep = totalDamagePrevented / Mathf.Max(1, totalActions),
                failureMode = ResolveFailureMode(policy.Name, wins, skillWastedCount, tempoBonusTriggeredCount)
            };
        }

        private static string BuildBaselineCsv(IReadOnlyList<PolicySummary> summaries)
        {
            var builder = new StringBuilder();
            builder.AppendLine("policy,episodes,cumulative_reward,episode_length,win_proxy,loss_proxy,timeout_proxy,attack_share,defend_share,skill_share,skill_blocked_share,skill_wasted_per_episode,defend_success_rate,tempo_bonus_triggered_per_episode,action_diversity,enemy_damage_per_step,player_damage_per_step,damage_prevented_per_step,failure_mode");
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
                    .Append(Format(item.skillWastedPerEpisode)).Append(',')
                    .Append(Format(item.defendSuccessRate)).Append(',')
                    .Append(Format(item.tempoBonusTriggeredPerEpisode)).Append(',')
                    .Append(Format(item.actionDiversity)).Append(',')
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
            bool tempoAttackReady,
            DeterministicRandom random)
        {
            return CombatTrainingExp04Rules.CanUseSkill(skillCooldownRemaining)
                ? CombatTrainingExp04Rules.ResolveAction(random.Range(0, 3))
                : CombatTrainingExp04Rules.ResolveAction(random.Range(0, 2));
        }

        private static CombatAction SelectAttack(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            bool tempoAttackReady,
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
            bool tempoAttackReady,
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
            bool tempoAttackReady,
            DeterministicRandom random)
        {
            if (tempoAttackReady)
            {
                return CombatAction.Attack;
            }

            return CombatTrainingExp04Rules.IsEnemyThreatHigh(stepIndex) || stepIndex % 3 != 2
                ? CombatAction.Defend
                : CombatAction.Attack;
        }

        private static CombatAction SelectContextAction(
            int stepIndex,
            CombatantState player,
            CombatantState enemy,
            CombatAction lastAction,
            int skillCooldownRemaining,
            bool tempoAttackReady,
            DeterministicRandom random)
        {
            if (tempoAttackReady)
            {
                return CombatAction.Attack;
            }

            if (CombatTrainingExp04Rules.IsEnemyThreatHigh(stepIndex) || player.Hp <= 8)
            {
                return CombatAction.Defend;
            }

            return enemy.Hp >= CombatTrainingExp04Rules.SkillWasteHpThreshold + 4 &&
                CombatTrainingExp04Rules.CanUseSkill(skillCooldownRemaining)
                ? CombatAction.Skill
                : CombatAction.Attack;
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

        private static string ResolveFailureMode(string policyName, int wins, int skillWastedCount, int tempoBonusTriggeredCount)
        {
            if (policyName == "SkillSpamPolicy")
            {
                return skillWastedCount > 0
                    ? "Skill spam is punished by cooldown and low-HP waste penalty."
                    : "Skill spam still avoids the waste penalty.";
            }

            if (policyName == "AttackSpamPolicy")
            {
                return "Attack spam ignores high-threat defend tempo and pays extra damage penalty.";
            }

            if (policyName == "DefendHeavyPolicy")
            {
                return wins < EpisodeCount / 2
                    ? "Defend creates tempo but lacks enough attack conversion."
                    : "Defend-heavy converts prevention into tempo often enough.";
            }

            if (policyName == "ContextPolicy")
            {
                return tempoBonusTriggeredCount > 0
                    ? "Context policy uses threat defense and tempo attacks as intended."
                    : "Context policy did not trigger tempo often enough.";
            }

            return "Random policy has high diversity but poor timing.";
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
            bool tempoAttackReady,
            DeterministicRandom random);

        [Serializable]
        private sealed class BaselineReport
        {
            public BaselineReport(PolicySummary[] policies)
            {
                runId = "hwigi_training_combat_exp04_baselines";
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
            public float skillWastedPerEpisode;
            public float defendSuccessRate;
            public float tempoBonusTriggeredPerEpisode;
            public float actionDiversity;
            public float enemyDamagePerStep;
            public float playerDamagePerStep;
            public float damagePreventedPerStep;
            public string failureMode;
        }
    }
}
