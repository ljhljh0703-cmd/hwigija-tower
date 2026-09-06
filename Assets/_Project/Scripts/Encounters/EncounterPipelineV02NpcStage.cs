using HwigiTower.NPC;

namespace HwigiTower.Encounters
{
    public enum EncounterNpcStageV02
    {
        S0_FIRST_MEETING = 0,
        S1_AWARENESS = 1,
        S2_COMPANION = 2,
        S3_FRACTURE = 3,
        S4_COLLAPSE = 4,
        S5_REST_OR_CONTINUE = 5
    }

    public static class EncounterPipelineV02NpcStage
    {
        public static bool TryMapToRuntime(string value, out NpcStage stage)
        {
            switch (value)
            {
                case "S0_FIRST_MEETING":
                    stage = NpcStage.S0;
                    return true;
                case "S1_AWARENESS":
                    stage = NpcStage.S1;
                    return true;
                case "S2_COMPANION":
                    stage = NpcStage.S2;
                    return true;
                case "S3_FRACTURE":
                    stage = NpcStage.S3;
                    return true;
                case "S4_COLLAPSE":
                    stage = NpcStage.S4;
                    return true;
                case "S5_REST_OR_CONTINUE":
                    stage = NpcStage.S5;
                    return true;
                default:
                    stage = default;
                    return false;
            }
        }
    }
}
