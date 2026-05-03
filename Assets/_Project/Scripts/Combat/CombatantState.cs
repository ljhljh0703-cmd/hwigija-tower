using System;

namespace HwigiTower.Combat
{
    [Serializable]
    public sealed class CombatantState
    {
        public CombatantState(string id, int maxHp, int attack)
            : this(id, maxHp, attack, maxHp)
        {
        }

        public CombatantState(string id, int maxHp, int attack, int currentHp)
        {
            Id = string.IsNullOrWhiteSpace(id) ? "combatant.placeholder" : id;
            MaxHp = Math.Max(1, maxHp);
            Hp = Math.Max(0, Math.Min(MaxHp, currentHp));
            Attack = Math.Max(0, attack);
        }

        public string Id { get; }
        public int MaxHp { get; }
        public int Hp { get; private set; }
        public int Attack { get; }
        public bool IsDefeated => Hp <= 0;

        public void ApplyDamage(int amount)
        {
            Hp = Math.Max(0, Hp - Math.Max(0, amount));
        }
    }
}
