namespace HwigiTower.Combat
{
    public readonly struct CombatRoundResult
    {
        public CombatRoundResult(int playerDamage, int enemyDamage, bool playerDefeated, bool enemyDefeated)
        {
            PlayerDamage = playerDamage;
            EnemyDamage = enemyDamage;
            PlayerDefeated = playerDefeated;
            EnemyDefeated = enemyDefeated;
        }

        public int PlayerDamage { get; }
        public int EnemyDamage { get; }
        public bool PlayerDefeated { get; }
        public bool EnemyDefeated { get; }
        public bool IsComplete => PlayerDefeated || EnemyDefeated;
    }
}
