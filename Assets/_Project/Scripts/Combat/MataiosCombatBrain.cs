namespace HwigiTower.Combat
{
    public static class MataiosCombatBrain
    {
        public const string ActionNoneDown = "none_down";
        public const string ActionProtectPlayer = "protect_player";
        public const string ActionFinishAttack = "finish_attack";
        public const string ActionPressureAttack = "pressure_attack";
        public const string ActionTempoAttack = "tempo_attack";
        public const string ActionCounterAssist = "counter_assist";
        public const string ActionSkillSetupAssist = "skill_setup_assist";
        public const string ActionSupportAttack = "support_attack";

        private const float PlayerLowHpThreshold = 0.35f;
        private const float MataiosCanProtectThreshold = 0.25f;

        public static MataiosActionPlan Decide(MataiosCombatContext context)
        {
            if (context.IsMataiosDown)
            {
                return Plan(ActionNoneDown, ActionNoneDown, MataiosActionTarget.None, "mataios_down", false, "down");
            }

            if (!context.HasUsableActorState())
            {
                return DecideFallback(context, "invalid_context");
            }

            if (context.EnemyHp <= context.MataiosActionPower)
            {
                return Plan(ActionFinishAttack, ActionFinishAttack, MataiosActionTarget.Enemy, "enemy_can_be_finished", false, "finish");
            }

            if (context.TempoReady)
            {
                return Plan(ActionTempoAttack, ActionTempoAttack, MataiosActionTarget.Enemy, "tempo_ready", true, "tempo");
            }

            if (context.EnemyThreatHigh &&
                context.PlayerAction != CombatAction.Defend &&
                context.MataiosHpRatioAbove(MataiosCanProtectThreshold))
            {
                return Plan(ActionProtectPlayer, ActionProtectPlayer, MataiosActionTarget.Player, "high_threat_player_open", false, "protect");
            }

            if (context.PlayerHpRatioAtOrBelow(PlayerLowHpThreshold) &&
                context.MataiosHpRatioAbove(MataiosCanProtectThreshold))
            {
                return Plan(ActionProtectPlayer, ActionProtectPlayer, MataiosActionTarget.Player, "player_low_hp", false, "protect");
            }

            if (context.EnemyThreatHigh && context.PlayerAction == CombatAction.Defend)
            {
                return Plan(ActionCounterAssist, ActionCounterAssist, MataiosActionTarget.Enemy, "defend_into_high_threat", false, "counter");
            }

            if (context.PlayerAction == CombatAction.Skill && context.PlayerSkillReady && context.SkillContextValuable)
            {
                return Plan(ActionSkillSetupAssist, ActionSkillSetupAssist, MataiosActionTarget.Enemy, "skill_opportunity", false, "skill_support");
            }

            if (context.HasLastTwoPlayerActions(CombatAction.Attack))
            {
                return Plan(ActionPressureAttack, ActionPressureAttack, MataiosActionTarget.Enemy, "attack_pressure", false, "pressure");
            }

            if (context.HasLastTwoPlayerActions(CombatAction.Defend))
            {
                return Plan(ActionCounterAssist, ActionCounterAssist, MataiosActionTarget.Enemy, "defense_pattern", false, "counter");
            }

            return Plan(ActionSupportAttack, ActionSupportAttack, MataiosActionTarget.Enemy, "default_support", false, "support");
        }

        private static MataiosActionPlan DecideFallback(MataiosCombatContext context, string reasonKey)
        {
            if (context.PlayerHpRatioAtOrBelow(PlayerLowHpThreshold) &&
                context.MataiosHpRatioAbove(MataiosCanProtectThreshold))
            {
                return Plan(ActionProtectPlayer, ActionProtectPlayer, MataiosActionTarget.Player, reasonKey, false, "fallback", "protect");
            }

            if (context.EnemyHp <= context.MataiosActionPower)
            {
                return Plan(ActionFinishAttack, ActionFinishAttack, MataiosActionTarget.Enemy, reasonKey, false, "fallback", "finish");
            }

            if (context.HasLastTwoPlayerActions(CombatAction.Defend))
            {
                return Plan(ActionCounterAssist, ActionCounterAssist, MataiosActionTarget.Enemy, reasonKey, false, "fallback", "counter");
            }

            if (context.HasLastTwoPlayerActions(CombatAction.Attack))
            {
                return Plan(ActionPressureAttack, ActionPressureAttack, MataiosActionTarget.Enemy, reasonKey, false, "fallback", "pressure");
            }

            return Plan(ActionSupportAttack, ActionSupportAttack, MataiosActionTarget.Enemy, reasonKey, false, "fallback", "support");
        }

        private static MataiosActionPlan Plan(
            string actionId,
            string payloadKey,
            MataiosActionTarget target,
            string reasonKey,
            bool consumesTempo,
            params string[] metricTags)
        {
            return new MataiosActionPlan(actionId, payloadKey, target, reasonKey, consumesTempo, metricTags);
        }
    }
}
