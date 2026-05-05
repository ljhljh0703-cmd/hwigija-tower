using HwigiTower.Core;

namespace HwigiTower.Combat
{
    public enum CombatAction
    {
        Attack,
        Prepare
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

        public CombatRoundResult ResolveRound(CombatantState player, CombatantState enemy, CombatAction playerAction)
        {
            if (player == null || enemy == null || player.IsDefeated || enemy.IsDefeated)
            {
                return new CombatRoundResult(0, 0, player == null || player.IsDefeated, enemy == null || enemy.IsDefeated);
            }

            var playerDamage = 0;
            var enemyDamage = 0;

            if (playerAction == CombatAction.Attack)
            {
                playerDamage = DamageRoll(player.Attack);
                enemy.ApplyDamage(playerDamage);
            }

            if (!enemy.IsDefeated)
            {
                enemyDamage = DamageRoll(enemy.Attack);
                if (playerAction == CombatAction.Prepare)
                {
                    enemyDamage /= 2;
                }
                player.ApplyDamage(enemyDamage);
            }

            return new CombatRoundResult(playerDamage, enemyDamage, player.IsDefeated, enemy.IsDefeated);
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
