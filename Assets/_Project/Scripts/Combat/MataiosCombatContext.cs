using System;
using System.Collections.Generic;

namespace HwigiTower.Combat
{
    public readonly struct MataiosCombatContext
    {
        private static readonly CombatAction[] EmptyRecentActions = Array.Empty<CombatAction>();
        private readonly bool _hasInvalidActorState;

        public MataiosCombatContext(
            bool isMataiosDown,
            CombatAction playerAction,
            IReadOnlyList<CombatAction> recentPlayerActions,
            int playerHp,
            int playerMaxHp,
            int mataiosHp,
            int mataiosMaxHp,
            int enemyHp,
            int enemyMaxHp,
            int mataiosActionPower,
            bool enemyThreatHigh,
            bool incomingTargetsPlayer,
            bool playerSkillReady,
            bool skillContextValuable,
            bool tempoReady)
        {
            IsMataiosDown = isMataiosDown;
            PlayerAction = playerAction;
            RecentPlayerActions = recentPlayerActions ?? EmptyRecentActions;
            PlayerHp = Math.Max(0, playerHp);
            PlayerMaxHp = Math.Max(0, playerMaxHp);
            MataiosHp = Math.Max(0, mataiosHp);
            MataiosMaxHp = Math.Max(0, mataiosMaxHp);
            EnemyHp = Math.Max(0, enemyHp);
            EnemyMaxHp = Math.Max(0, enemyMaxHp);
            MataiosActionPower = Math.Max(0, mataiosActionPower);
            EnemyThreatHigh = enemyThreatHigh;
            IncomingTargetsPlayer = incomingTargetsPlayer;
            PlayerSkillReady = playerSkillReady;
            SkillContextValuable = skillContextValuable;
            TempoReady = tempoReady;
            _hasInvalidActorState = playerMaxHp <= 0 || mataiosMaxHp <= 0 || enemyMaxHp <= 0;
        }

        public bool IsMataiosDown { get; }
        public CombatAction PlayerAction { get; }
        public IReadOnlyList<CombatAction> RecentPlayerActions { get; }
        public int PlayerHp { get; }
        public int PlayerMaxHp { get; }
        public int MataiosHp { get; }
        public int MataiosMaxHp { get; }
        public int EnemyHp { get; }
        public int EnemyMaxHp { get; }
        public int MataiosActionPower { get; }
        public bool EnemyThreatHigh { get; }
        public bool IncomingTargetsPlayer { get; }
        public bool PlayerSkillReady { get; }
        public bool SkillContextValuable { get; }
        public bool TempoReady { get; }

        public bool HasUsableActorState()
        {
            return !_hasInvalidActorState;
        }

        public bool PlayerHpRatioAtOrBelow(float threshold)
        {
            return PlayerMaxHp > 0 && PlayerHp / (float)PlayerMaxHp <= threshold;
        }

        public bool MataiosHpRatioAbove(float threshold)
        {
            return MataiosMaxHp > 0 && MataiosHp / (float)MataiosMaxHp > threshold;
        }

        public bool HasLastTwoPlayerActions(CombatAction action)
        {
            if (RecentPlayerActions == null || RecentPlayerActions.Count < 2)
            {
                return false;
            }

            var last = RecentPlayerActions[RecentPlayerActions.Count - 1];
            var previous = RecentPlayerActions[RecentPlayerActions.Count - 2];
            return last == action && previous == action;
        }
    }
}
