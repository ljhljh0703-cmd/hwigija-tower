using System.Collections.Generic;
using HwigiTower.Core;
using UnityEngine;

namespace HwigiTower.Abilities
{
    // per GDD OQ-011: 특성 = 메타 영구 해금 패시브. 승리 횟수 3단계(Lv1:3회/Lv2:6회/Lv3:10회)
    // 태그: Survival / Offense / Support
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Trait", fileName = "SO_Trait_Placeholder")]
    public sealed class TraitData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private string tag = string.Empty;
        [SerializeField, TextArea(1, 3)] private string description = string.Empty;

        // 메타 해금 임계값 — 누적 승리 횟수
        // Lv1=3 / Lv2=6 / Lv3=10 (per GDD OQ-011)
        [SerializeField, Min(1)] private int unlockWinCount = 3;

        [SerializeField] private NumericParam[] numericParams = new NumericParam[0];

        public string Id => id;
        public string DisplayName => displayName;
        public string Tag => tag;
        public string Description => description;
        public int UnlockWinCount => unlockWinCount;
        public IReadOnlyList<NumericParam> NumericParams => numericParams;
    }
}
