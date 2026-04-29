using UnityEngine;

namespace HwigiTower.Combat
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Enemy", fileName = "SO_Enemy_Placeholder")]
    public sealed class EnemyData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField, Min(0)] private int hp;
        [SerializeField, Min(0)] private int attack;
        [SerializeField] private string patternId = string.Empty;

        public string Id => id;
        public int Hp => hp;
        public int Attack => attack;
        public string PatternId => patternId;
    }
}
