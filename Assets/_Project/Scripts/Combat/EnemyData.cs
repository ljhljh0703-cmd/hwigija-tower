using UnityEngine;

namespace HwigiTower.Combat
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Enemy", fileName = "SO_Enemy_Placeholder")]
    public sealed class EnemyData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField, Min(0)] private int hp;
        [SerializeField, Min(0)] private int attack;
        // per GDD D-022 / OQ-014: patternId → EnemyPatternData SO 직접 참조
        [SerializeField] private string patternId = string.Empty;
        [SerializeField] private EnemyPatternData pattern;

        // per design/balance.md: 전투 승리 보상
        [SerializeField, Min(0)] private int goldReward;
        [SerializeField, Min(0)] private int xpReward;
        [SerializeField] private int glitchDelta;    // 양수 = Glitch 증가, 음수 = 감소
        [SerializeField] private int affinityDelta;  // 양수 = Affinity 증가, 음수 = 감소

        public string Id => id;
        public int Hp => hp;
        public int Attack => attack;
        public string PatternId => patternId;
        public EnemyPatternData Pattern => pattern;
        public int GoldReward => goldReward;
        public int XpReward => xpReward;
        public int GlitchDelta => glitchDelta;
        public int AffinityDelta => affinityDelta;
    }
}
