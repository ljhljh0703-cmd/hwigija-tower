using System;
using System.Collections.Generic;
using HwigiTower.Core;

namespace HwigiTower.Abilities
{
    public readonly struct CombatAbilityModifiers
    {
        public CombatAbilityModifiers(int playerMaxHpBonus, int playerAttackBonus)
        {
            PlayerMaxHpBonus = playerMaxHpBonus;
            PlayerAttackBonus = playerAttackBonus;
        }

        public int PlayerMaxHpBonus { get; }
        public int PlayerAttackBonus { get; }

        public static CombatAbilityModifiers From(IReadOnlyList<AbilityData> abilities, IReadOnlyList<SynergyState> synergies)
        {
            var hpBonus = 0f;
            var attackBonus = 0f;

            if (abilities != null)
            {
                for (var i = 0; i < abilities.Count; i++)
                {
                    var ability = abilities[i];
                    if (ability == null)
                    {
                        continue;
                    }

                    hpBonus += NumericParamLookup.Sum(ability.NumericParams, "player.max_hp_bonus");
                    attackBonus += NumericParamLookup.Sum(ability.NumericParams, "player.attack_bonus");
                }
            }

            if (synergies != null)
            {
                for (var i = 0; i < synergies.Count; i++)
                {
                    var state = synergies[i];
                    if (!state.Active || state.Synergy == null)
                    {
                        continue;
                    }

                    hpBonus += NumericParamLookup.Sum(state.Synergy.NumericParams, "player.max_hp_bonus");
                    attackBonus += NumericParamLookup.Sum(state.Synergy.NumericParams, "player.attack_bonus");
                }
            }

            return new CombatAbilityModifiers((int)Math.Round(hpBonus), (int)Math.Round(attackBonus));
        }
    }
}
