using HwigiTower.Abilities;
using HwigiTower.Combat;
using HwigiTower.Core;
using HwigiTower.Items;
using HwigiTower.Rewards;
using UnityEngine;

namespace HwigiTower.Encounters
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Encounter Runtime Catalog", fileName = "SO_EncounterRuntimeCatalog")]
    public sealed class EncounterRuntimeCatalogData : ScriptableObject
    {
        [SerializeField] private ItemData[] items = new ItemData[0];
        [SerializeField] private RewardBundleData[] rewardBundles = new RewardBundleData[0];
        [SerializeField] private AbilityData[] abilities = new AbilityData[0];
        [SerializeField] private SynergyData[] synergies = new SynergyData[0];
        [SerializeField] private EnemyData[] enemies = new EnemyData[0];
        [SerializeField] private FloorEnemyPoolData floorEnemyPools;
        [SerializeField] private MemoryFragmentData[] memoryFragments = new MemoryFragmentData[0];

        public ItemData[] Items => items;
        public RewardBundleData[] RewardBundles => rewardBundles;
        public AbilityData[] Abilities => abilities;
        public SynergyData[] Synergies => synergies;
        public EnemyData[] Enemies => enemies;
        public FloorEnemyPoolData FloorEnemyPools => floorEnemyPools;
        public MemoryFragmentData[] MemoryFragments => memoryFragments;

        public bool TryGetItem(string stableId, out ItemData item)
        {
            item = null;
            if (string.IsNullOrEmpty(stableId))
            {
                return false;
            }

            for (var i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].StableId == stableId)
                {
                    item = items[i];
                    return true;
                }
            }

            return false;
        }

        public bool TryGetRewardBundle(string stableId, out RewardBundleData rewardBundle)
        {
            rewardBundle = null;
            if (string.IsNullOrEmpty(stableId))
            {
                return false;
            }

            for (var i = 0; i < rewardBundles.Length; i++)
            {
                if (rewardBundles[i] != null && rewardBundles[i].StableId == stableId)
                {
                    rewardBundle = rewardBundles[i];
                    return true;
                }
            }

            return false;
        }

        public bool TryGetAbility(string stableId, out AbilityData ability)
        {
            ability = null;
            if (string.IsNullOrEmpty(stableId))
            {
                return false;
            }

            for (var i = 0; i < abilities.Length; i++)
            {
                if (abilities[i] != null && abilities[i].Id == stableId)
                {
                    ability = abilities[i];
                    return true;
                }
            }

            return false;
        }

        public bool TryGetEnemy(string stableId, out EnemyData enemy)
        {
            enemy = null;
            if (string.IsNullOrEmpty(stableId))
            {
                return false;
            }

            for (var i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] != null && enemies[i].Id == stableId)
                {
                    enemy = enemies[i];
                    return true;
                }
            }

            return false;
        }

        public bool TrySelectEnemyForFloor(int floor, EnemyPoolRank rank, DeterministicRunContext context, string seedKey, out EnemyData enemy)
        {
            enemy = null;
            if (floorEnemyPools == null ||
                !floorEnemyPools.TrySelectEnemy(floor, rank, context, seedKey, out var stableId) ||
                !TryGetEnemy(stableId, out enemy))
            {
                return false;
            }

            return enemy != null;
        }

        public bool TryGetMemoryFragment(string stableId, out MemoryFragmentData memoryFragment)
        {
            memoryFragment = null;
            if (string.IsNullOrEmpty(stableId))
            {
                return false;
            }

            for (var i = 0; i < memoryFragments.Length; i++)
            {
                if (memoryFragments[i] != null && memoryFragments[i].StableId == stableId)
                {
                    memoryFragment = memoryFragments[i];
                    return true;
                }
            }

            return false;
        }
    }
}
