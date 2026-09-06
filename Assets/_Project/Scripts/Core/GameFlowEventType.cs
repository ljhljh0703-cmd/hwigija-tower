namespace HwigiTower.Core
{
    public enum GameFlowEventType
    {
        RunStarted = 0,
        RoomEntered = 1,
        NodeEntered = 2,
        NodeExited = 3,
        NodeResolved = 4,
        EncounterSelected = 5,
        EncounterCompleted = 6,
        CombatStarted = 7,
        CombatCompleted = 8,
        AbilityAdded = 9,
        SynergyActivated = 10,
        ReflectionSaved = 11,
        RecallLoaded = 12,
        NpcStageChanged = 13,
        RunCompleted = 14,
        // per GDD D-022 / OQ-011
        CombatRoundResolved = 15,
        TraitUnlocked = 16,
        ShopRerolled = 17,
        PlayerDefeated = 18,
        EnemyDefeated = 19
    }
}
