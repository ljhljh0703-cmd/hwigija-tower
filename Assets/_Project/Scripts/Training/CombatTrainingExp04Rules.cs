using HwigiTower.Combat;
using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Training
{
    public static class CombatTrainingExp04Rules
    {
        public const int PlayerMaxHp = 24;
        public const int PlayerAttack = 6;
        public const int EnemyMaxHp = 22;
        public const int EnemyAttack = 5;
        public const int SkillDamage = 10;
        public const int MaxDecisionSteps = 12;
        public const int SkillCooldownTurns = 2;
        public const int ThreatDamageBonus = 5;
        public const int TempoAttackBonusDamage = 4;
        public const int SkillWasteHpThreshold = 8;
        public const float EnemyDamageReward = 0.05f;
        public const float PlayerDamagePenalty = -0.035f;
        public const float DamagePreventedReward = 0.025f;
        public const float DefendTempoReward = 0.08f;
        public const float SkillUsePenalty = -0.04f;
        public const float SkillWastePenalty = -0.25f;
        public const float StepPenalty = -0.01f;
        public const float WinReward = 1f;
        public const float LossPenalty = -1f;
        public const float TimeoutPenalty = -0.25f;

        public static CombatTrainingExp04RoundResult ResolveRound(
            CombatantState player,
            CombatantState enemy,
            CombatAction requestedAction,
            int skillCooldownRemaining,
            bool tempoAttackReady,
            int stepIndex,
            DeterministicRandom random)
        {
            if (player == null || enemy == null || player.IsDefeated || enemy.IsDefeated)
            {
                return new CombatTrainingExp04RoundResult(
                    CombatAction.Attack,
                    requestedAction,
                    false,
                    false,
                    false,
                    false,
                    0,
                    0,
                    0,
                    player == null || player.IsDefeated,
                    enemy == null || enemy.IsDefeated);
            }

            var skillBlocked = requestedAction == CombatAction.Skill && skillCooldownRemaining > 0;
            var action = skillBlocked ? CombatAction.Attack : requestedAction;
            var enemyDamage = 0;
            var playerDamage = 0;
            var damagePrevented = 0;
            var skillWasted = action == CombatAction.Skill && enemy.Hp <= SkillWasteHpThreshold;
            var tempoBonusTriggered = tempoAttackReady && action == CombatAction.Attack;

            if (action == CombatAction.Attack || action == CombatAction.Skill)
            {
                enemyDamage = action == CombatAction.Skill ? SkillDamage : DamageRoll(PlayerAttack, random);
                enemyDamage += tempoBonusTriggered ? TempoAttackBonusDamage : 0;
                enemy.ApplyDamage(enemyDamage);
            }

            var threatHigh = IsEnemyThreatHigh(stepIndex);
            var defendSucceeded = false;
            if (!enemy.IsDefeated && !player.IsDefeated)
            {
                var rawEnemyDamage = DamageRoll(EnemyAttack, random) + (threatHigh ? ThreatDamageBonus : 0);
                if (action == CombatAction.Defend)
                {
                    var reductionDivisor = threatHigh ? 4 : 2;
                    playerDamage = rawEnemyDamage / reductionDivisor;
                    damagePrevented = rawEnemyDamage - playerDamage;
                    defendSucceeded = damagePrevented > 0;
                }
                else
                {
                    playerDamage = rawEnemyDamage;
                }

                player.ApplyDamage(playerDamage);
            }

            return new CombatTrainingExp04RoundResult(
                action,
                requestedAction,
                skillBlocked,
                skillWasted,
                defendSucceeded,
                tempoBonusTriggered,
                enemyDamage,
                playerDamage,
                damagePrevented,
                player.IsDefeated,
                enemy.IsDefeated);
        }

        public static float CalculateStepReward(CombatTrainingExp04RoundResult result)
        {
            var reward = result.EnemyDamage * EnemyDamageReward;
            reward += result.PlayerDamage * PlayerDamagePenalty;
            reward += result.DamagePrevented * DamagePreventedReward;
            reward += result.DefendSucceeded ? DefendTempoReward : 0f;
            reward += result.Action == CombatAction.Skill ? SkillUsePenalty : 0f;
            reward += result.SkillWasted ? SkillWastePenalty : 0f;
            reward += StepPenalty;
            return reward;
        }

        public static int NextSkillCooldown(int currentCooldown, CombatAction action)
        {
            return action == CombatAction.Skill
                ? SkillCooldownTurns
                : Mathf.Max(0, currentCooldown - 1);
        }

        public static bool NextTempoAttackReady(bool currentReady, CombatTrainingExp04RoundResult result)
        {
            if (result.TempoBonusTriggered)
            {
                return false;
            }

            return result.DefendSucceeded || currentReady;
        }

        public static bool CanUseSkill(int skillCooldownRemaining)
        {
            return skillCooldownRemaining <= 0;
        }

        public static bool IsEnemyThreatHigh(int stepIndex)
        {
            return stepIndex % 3 == 1;
        }

        public static CombatAction ResolveAction(int action)
        {
            return action switch
            {
                1 => CombatAction.Defend,
                2 => CombatAction.Skill,
                _ => CombatAction.Attack
            };
        }

        public static float NormalizeHp(int hp, int maxHp)
        {
            return maxHp <= 0 ? 0f : Mathf.Clamp01(hp / (float)maxHp);
        }

        private static int DamageRoll(int baseDamage, DeterministicRandom random)
        {
            return baseDamage <= 0 ? 0 : baseDamage + random.Range(0, 3);
        }
    }

    public readonly struct CombatTrainingExp04RoundResult
    {
        public CombatTrainingExp04RoundResult(
            CombatAction action,
            CombatAction requestedAction,
            bool skillBlocked,
            bool skillWasted,
            bool defendSucceeded,
            bool tempoBonusTriggered,
            int enemyDamage,
            int playerDamage,
            int damagePrevented,
            bool playerDefeated,
            bool enemyDefeated)
        {
            Action = action;
            RequestedAction = requestedAction;
            SkillBlocked = skillBlocked;
            SkillWasted = skillWasted;
            DefendSucceeded = defendSucceeded;
            TempoBonusTriggered = tempoBonusTriggered;
            EnemyDamage = enemyDamage;
            PlayerDamage = playerDamage;
            DamagePrevented = damagePrevented;
            PlayerDefeated = playerDefeated;
            EnemyDefeated = enemyDefeated;
        }

        public CombatAction Action { get; }
        public CombatAction RequestedAction { get; }
        public bool SkillBlocked { get; }
        public bool SkillWasted { get; }
        public bool DefendSucceeded { get; }
        public bool TempoBonusTriggered { get; }
        public int EnemyDamage { get; }
        public int PlayerDamage { get; }
        public int DamagePrevented { get; }
        public bool PlayerDefeated { get; }
        public bool EnemyDefeated { get; }
    }
}
