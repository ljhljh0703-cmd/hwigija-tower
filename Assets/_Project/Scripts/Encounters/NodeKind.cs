namespace HwigiTower.Encounters
{
    // per GDD §7.0 OQ-012: 노드 종류 — 전투/인카운터/휴식/상인(보스 직전 고정)/보스
    public enum NodeKind
    {
        Battle = 0,
        Encounter = 1,
        Rest = 2,
        Shop = 3,      // 보스 직전 고정 (경로 무관)
        Boss = 4,
        Remnant = 5    // 레거시 유지 (기존 SO 자산 호환)
    }
}
