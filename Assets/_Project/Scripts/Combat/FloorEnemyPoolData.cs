using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Combat
{
    public enum EnemyPoolRank
    {
        Normal = 0,
        Elite = 1,
        Boss = 2
    }

    [System.Serializable]
    public sealed class FloorEnemyPoolEntry
    {
        [SerializeField, Min(1)] private int floor = 1;
        [SerializeField] private string[] normalEnemyRefs = new string[0];
        [SerializeField] private string[] eliteEnemyRefs = new string[0];
        [SerializeField] private string[] bossEnemyRefs = new string[0];

        public int Floor => floor < 1 ? 1 : floor;
        public string[] NormalEnemyRefs => normalEnemyRefs ?? new string[0];
        public string[] EliteEnemyRefs => eliteEnemyRefs ?? new string[0];
        public string[] BossEnemyRefs => bossEnemyRefs ?? new string[0];

        public string[] GetRefs(EnemyPoolRank rank)
        {
            return rank switch
            {
                EnemyPoolRank.Elite => EliteEnemyRefs,
                EnemyPoolRank.Boss => BossEnemyRefs,
                _ => NormalEnemyRefs
            };
        }
    }

    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Floor Enemy Pool", fileName = "SO_FloorEnemyPool")]
    public sealed class FloorEnemyPoolData : ScriptableObject
    {
        [SerializeField] private FloorEnemyPoolEntry[] floors = new FloorEnemyPoolEntry[0];

        public FloorEnemyPoolEntry[] Floors => floors ?? new FloorEnemyPoolEntry[0];

        public bool TryGetPool(int floor, out FloorEnemyPoolEntry pool)
        {
            var source = Floors;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != null && source[i].Floor == floor)
                {
                    pool = source[i];
                    return true;
                }
            }

            pool = null;
            return false;
        }

        public bool TrySelectEnemy(int floor, EnemyPoolRank rank, DeterministicRunContext context, string seedKey, out string enemyStableId)
        {
            enemyStableId = string.Empty;
            if (!TryGetPool(floor, out var pool))
            {
                return false;
            }

            var refs = pool.GetRefs(rank);
            if (refs.Length == 0 && rank == EnemyPoolRank.Elite)
            {
                refs = pool.NormalEnemyRefs;
            }

            if (refs.Length == 0)
            {
                return false;
            }

            var random = context.CreateRandom("floor.enemy." + floor + "." + rank + "." + seedKey);
            enemyStableId = refs[random.Range(0, refs.Length)];
            return !string.IsNullOrEmpty(enemyStableId);
        }
    }
}
