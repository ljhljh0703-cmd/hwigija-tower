namespace HwigiTower.Combat
{
    public readonly struct CombatRoundResult
    {
        public CombatRoundResult(
            int playerDamage,
            int enemyDamage,
            bool playerDefeated,
            bool enemyDefeated,
            int comboDamage = 0,
            int allyDamage = 0,
            int playerDamagePrevented = 0)
        {
            PlayerDamage = playerDamage;
            EnemyDamage = enemyDamage;
            PlayerDefeated = playerDefeated;
            EnemyDefeated = enemyDefeated;
            ComboDamage = comboDamage;
            AllyDamage = allyDamage;
            PlayerDamagePrevented = playerDamagePrevented;
        }

        public int PlayerDamage { get; }
        public int EnemyDamage { get; }
        public bool PlayerDefeated { get; }
        public bool EnemyDefeated { get; }
        // per GDD D-023 / TRAIT_OFFENSE_04: 복합행동 2번째 공격 피해 (0 = 콤보 없음)
        public int ComboDamage { get; }
        public int AllyDamage { get; }
        public int PlayerDamagePrevented { get; }
        public bool IsComplete => PlayerDefeated || EnemyDefeated;
    }
}
