using UnityEngine;

namespace HwigiTower.Combat
{
    // per GDD D-022 / OQ-014: 적 턴 = 상황 텍스트 1개 + 2택 1, 몬스터별 고유 기믹
    [CreateAssetMenu(menuName = "Hwigi Tower/Data/Enemy Pattern", fileName = "SO_EnemyPattern_Placeholder")]
    public sealed class EnemyPatternData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;

        [SerializeField, TextArea(1, 4)]
        private string situationText = string.Empty;

        [SerializeField] private EnemyChoice choiceA;
        [SerializeField] private EnemyChoice choiceB;

        public string Id => id;
        public string SituationText => situationText;
        public EnemyChoice ChoiceA => choiceA;
        public EnemyChoice ChoiceB => choiceB;
    }

    [System.Serializable]
    public sealed class EnemyChoice
    {
        [SerializeField] private string label = string.Empty;

        // 플레이어가 이 선택지를 고를 때 받는 피해 배율 (1.0 = 전체, 0.5 = 절반, 0 = 무효화)
        // 작가가 패턴 SO에서 직접 수치 설정 — 구체 기믹은 작가 권한 (per AGENTS.md §5.2)
        [SerializeField, Range(0f, 2f)] private float damageMultiplier = 1f;

        // 선택지 결과로 플레이어에게 주는 즉발 피해 (0 = 없음)
        [SerializeField, Min(0)] private int directDamage;

        public string Label => label;
        public float DamageMultiplier => damageMultiplier;
        public int DirectDamage => directDamage;
    }
}
