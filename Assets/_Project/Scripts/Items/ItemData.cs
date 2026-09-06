using System.Collections.Generic;
using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Items
{
    // per GDD D-023: normal = 일반 아이템 (상점 기본), relic = 유물 (TRAIT_SUPPORT_04 해금 시 슬롯 추가)
    public enum ItemTier { Normal, Relic }

    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Item", fileName = "SO_Item_Placeholder")]
    public sealed class ItemData : ScriptableObject
    {
        [SerializeField] private string stableId = string.Empty;
        [SerializeField] private string displayNameKey = string.Empty;
        [SerializeField] private string descriptionKey = string.Empty;
        [SerializeField, Min(0)] private int maxStack = 99;

        // per GDD D-023: 소모품 없음 — 획득 즉시 패시브 보유 효과 적용
        [SerializeField] private ItemTier tier = ItemTier.Normal;

        // 패시브 발동 트리거 키 (예: "combat_start", "player_attack", "enemy_defeated")
        // 복잡한 relic 트리거 실제 발동은 향후 ItemPassiveResolver — 현재는 데이터 구조만
        [SerializeField] private string passiveTrigger = string.Empty;

        [SerializeField] private NumericParam[] numericParams = new NumericParam[0];

        public string StableId => stableId;
        public string DisplayNameKey => displayNameKey;
        public string DescriptionKey => descriptionKey;
        public int MaxStack => maxStack;
        public ItemTier Tier => tier;
        public string PassiveTrigger => passiveTrigger;
        public IReadOnlyList<NumericParam> NumericParams => numericParams;
    }
}
