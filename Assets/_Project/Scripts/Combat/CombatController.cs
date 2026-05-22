using HwigiTower.Core;

namespace HwigiTower.Combat
{
    // per GDD D-022: 플레이어 턴 3택 — Attack / Defend / Skill
    public enum CombatAction
    {
        Attack,
        Defend,
        Skill
    }

    public sealed class CombatController
    {
        private readonly DeterministicRandom _random;

        public CombatController(DeterministicRandom random)
        {
            _random = random;
        }

        public CombatController(DeterministicRunContext context, string combatId)
        {
            _random = context.CreateRandom($"combat.{combatId}");
        }

        // per GDD D-022: Skill 효과는 상위 레이어(능력 시스템)에서 처리.
        // per GDD D-023 / TRAIT_OFFENSE_04: secondAction != null 이면 복합행동.
        //   secondAction == Attack 또는 Skill → 추가공격 ATK×0.7
        //   두 번째 행동이 아이템 발동인 경우는 이 레이어 밖 (ItemPassiveResolver 담당)
        public CombatRoundResult ResolveRound(
            CombatantState player,
            CombatantState enemy,
            CombatAction playerAction,
            CombatAction? secondAction = null,
            int? skillDamageOverride = null)
        {
            if (player == null || enemy == null || player.IsDefeated || enemy.IsDefeated)
            {
                return new CombatRoundResult(0, 0, player == null || player.IsDefeated, enemy == null || enemy.IsDefeated);
            }

            var playerDamage = 0;
            var enemyDamage = 0;
            var comboDamage = 0;

            if (playerAction == CombatAction.Attack || playerAction == CombatAction.Skill)
            {
                playerDamage = playerAction == CombatAction.Skill && skillDamageOverride.HasValue
                    ? System.Math.Max(0, skillDamageOverride.Value)
                    : DamageRoll(player.Attack);
                enemy.ApplyDamage(playerDamage);
            }

            // 복합행동 2번째 공격 (per GDD D-023 TRAIT_OFFENSE_04)
            if (!enemy.IsDefeated && secondAction.HasValue &&
                (secondAction.Value == CombatAction.Attack || secondAction.Value == CombatAction.Skill))
            {
                comboDamage = DamageRoll((int)(player.Attack * 0.7f));
                enemy.ApplyDamage(comboDamage);
            }

            if (!enemy.IsDefeated)
            {
                enemyDamage = DamageRoll(enemy.Attack);
                if (playerAction == CombatAction.Defend)
                {
                    enemyDamage /= 2;
                }
                player.ApplyDamage(enemyDamage);
            }

            return new CombatRoundResult(playerDamage, enemyDamage, player.IsDefeated, enemy.IsDefeated, comboDamage);
        }

        private int DamageRoll(int baseDamage)
        {
            if (baseDamage <= 0)
            {
                return 0;
            }

            var variance = _random.Range(0, 3);
            return baseDamage + variance;
        }
    }
}
