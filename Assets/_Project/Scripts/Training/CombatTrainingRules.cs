using HwigiTower.Combat;
using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Training
{
    public static class CombatTrainingRules
    {
        public const int PlayerMaxHp = 24;
        public const int PlayerAttack = 6;
        public const int EnemyMaxHp = 20;
        public const int EnemyAttack = 5;
        public const int SkillDamage = 8;
        public const int MaxDecisionSteps = 12;
        public const int SkillCooldownTurns = 1;
        public const float EnemyDamageReward = 0.05f;
        public const float PlayerDamagePenalty = -0.03f;
        public const float DamagePreventedReward = 0.02f;
        public const float SkillUsePenalty = -0.03f;
        public const float StepPenalty = -0.01f;
        public const float WinReward = 1f;
        public const float LossPenalty = -1f;
        public const float TimeoutPenalty = -0.25f;

        public static CombatTrainingRoundResult ResolveRound(
            CombatantState player,
            CombatantState enemy,
            CombatAction requestedAction,
            int skillCooldownRemaining,
            DeterministicRandom random)
        {
            if (player == null || enemy == null || player.IsDefeated || enemy.IsDefeated)
            {
                return new CombatTrainingRoundResult(
                    CombatAction.Attack,
                    requestedAction,
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

            if (action == CombatAction.Attack || action == CombatAction.Skill)
            {
                enemyDamage = action == CombatAction.Skill ? SkillDamage : DamageRoll(PlayerAttack, random);
                enemy.ApplyDamage(enemyDamage);
            }

            if (!enemy.IsDefeated && !player.IsDefeated)
            {
                var rawEnemyDamage = DamageRoll(EnemyAttack, random);
                if (action == CombatAction.Defend)
                {
                    playerDamage = rawEnemyDamage / 2;
                    damagePrevented = rawEnemyDamage - playerDamage;
                }
                else
                {
                    playerDamage = rawEnemyDamage;
                }

                player.ApplyDamage(playerDamage);
            }

            return new CombatTrainingRoundResult(
                action,
                requestedAction,
                skillBlocked,
                enemyDamage,
                playerDamage,
                damagePrevented,
                player.IsDefeated,
                enemy.IsDefeated);
        }

        public static float CalculateStepReward(CombatTrainingRoundResult result)
        {
            var reward = result.EnemyDamage * EnemyDamageReward;
            reward += result.PlayerDamage * PlayerDamagePenalty;
            reward += result.DamagePrevented * DamagePreventedReward;
            reward += result.Action == CombatAction.Skill ? SkillUsePenalty : 0f;
            reward += StepPenalty;
            return reward;
        }

        public static int NextSkillCooldown(int currentCooldown, CombatAction action)
        {
            return action == CombatAction.Skill
                ? SkillCooldownTurns
                : Mathf.Max(0, currentCooldown - 1);
        }

        public static bool CanUseSkill(int skillCooldownRemaining)
        {
            return skillCooldownRemaining <= 0;
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

    public readonly struct CombatTrainingRoundResult
    {
        public CombatTrainingRoundResult(
            CombatAction action,
            CombatAction requestedAction,
            bool skillBlocked,
            int enemyDamage,
            int playerDamage,
            int damagePrevented,
            bool playerDefeated,
            bool enemyDefeated)
        {
            Action = action;
            RequestedAction = requestedAction;
            SkillBlocked = skillBlocked;
            EnemyDamage = enemyDamage;
            PlayerDamage = playerDamage;
            DamagePrevented = damagePrevented;
            PlayerDefeated = playerDefeated;
            EnemyDefeated = enemyDefeated;
        }

        public CombatAction Action { get; }
        public CombatAction RequestedAction { get; }
        public bool SkillBlocked { get; }
        public int EnemyDamage { get; }
        public int PlayerDamage { get; }
        public int DamagePrevented { get; }
        public bool PlayerDefeated { get; }
        public bool EnemyDefeated { get; }
    }
}
