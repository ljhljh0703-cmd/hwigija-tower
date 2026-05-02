using HwigiTower.Core;

namespace HwigiTower.Combat
{
    public sealed class CombatController
    {
        private readonly DeterministicRandom _random;

        public CombatController(DeterministicRunContext context, string combatId)
        {
            _random = context.CreateRandom($"combat.{combatId}");
        }

        public CombatRoundResult ResolveRound(CombatantState player, CombatantState enemy)
        {
            if (player == null || enemy == null || player.IsDefeated || enemy.IsDefeated)
            {
                return new CombatRoundResult(0, 0, player == null || player.IsDefeated, enemy == null || enemy.IsDefeated);
            }

            var playerDamage = DamageRoll(player.Attack);
            enemy.ApplyDamage(playerDamage);

            var enemyDamage = 0;
            if (!enemy.IsDefeated)
            {
                enemyDamage = DamageRoll(enemy.Attack);
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
