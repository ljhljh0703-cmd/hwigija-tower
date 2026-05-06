using System;
using System.Collections.Generic;
using HwigiTower.Core;
using HwigiTower.Items;

namespace HwigiTower.Abilities
{
    public readonly struct CombatAbilityModifiers
    {
        public CombatAbilityModifiers(
            int playerMaxHpBonus,
            int playerAttackBonus,
            int flatDamageBonus,
            int defendDamageReduce,
            int combatStartHpRestore,
            int poisonDamagePerRound,
            int firstHitDamageReduce)
        {
            PlayerMaxHpBonus = playerMaxHpBonus;
            PlayerAttackBonus = playerAttackBonus;
            FlatDamageBonus = flatDamageBonus;
            DefendDamageReduce = defendDamageReduce;
            CombatStartHpRestore = combatStartHpRestore;
            PoisonDamagePerRound = poisonDamagePerRound;
            FirstHitDamageReduce = firstHitDamageReduce;
        }

        // 스탯 보정 (CombatantState 생성 시 반영)
        public int PlayerMaxHpBonus { get; }
        public int PlayerAttackBonus { get; }

        // 전투 중 즉발 수치
        public int FlatDamageBonus { get; }        // trigger: player_attack — 공격 시 추가 고정 피해
        public int DefendDamageReduce { get; }     // trigger: player_defend — 방어 시 받는 피해 감소
        public int CombatStartHpRestore { get; }   // trigger: combat_start — 전투 시작 시 HP 회복
        public int PoisonDamagePerRound { get; }   // trigger: round_start — 매 라운드 적 HP 감소 (독)
        public int FirstHitDamageReduce { get; }   // trigger: first_hit_per_combat — 첫 피격 1회 감소

        public static CombatAbilityModifiers From(
            IReadOnlyList<AbilityData> abilities,
            IReadOnlyList<SynergyState> synergies,
            IReadOnlyList<ItemData> items = null)
        {
            var hpBonus = 0f;
            var attackBonus = 0f;
            var flatDmg = 0f;
            var defendReduce = 0f;
            var combatStartHp = 0f;
            var poison = 0f;
            var firstHitReduce = 0f;

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

            if (items != null)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item == null)
                    {
                        continue;
                    }

                    hpBonus += NumericParamLookup.Sum(item.NumericParams, "max_hp_bonus");
                    hpBonus += NumericParamLookup.Sum(item.NumericParams, "max_hp_penalty");
                    attackBonus += NumericParamLookup.Sum(item.NumericParams, "atk_bonus");
                    flatDmg += NumericParamLookup.Sum(item.NumericParams, "flat_damage_bonus");
                    combatStartHp += NumericParamLookup.Sum(item.NumericParams, "hp_restore");
                    poison += NumericParamLookup.Sum(item.NumericParams, "poison_damage_per_round");

                    if (item.PassiveTrigger == "player_defend")
                    {
                        defendReduce += NumericParamLookup.Sum(item.NumericParams, "damage_reduce");
                    }
                    else if (item.PassiveTrigger == "first_hit_per_combat")
                    {
                        firstHitReduce += NumericParamLookup.Sum(item.NumericParams, "damage_reduce");
                    }
                }
            }

            return new CombatAbilityModifiers(
                (int)Math.Round(hpBonus),
                (int)Math.Round(attackBonus),
                (int)Math.Round(flatDmg),
                (int)Math.Round(defendReduce),
                (int)Math.Round(combatStartHp),
                (int)Math.Round(poison),
                (int)Math.Round(firstHitReduce));
        }
    }
}
