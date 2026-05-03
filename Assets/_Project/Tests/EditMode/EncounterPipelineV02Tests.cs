using HwigiTower.Encounters;
using NUnit.Framework;
using UnityEditor;

namespace HwigiTower.Tests.EditMode
{
    public sealed class EncounterPipelineV02Tests
    {
        private const string SamplePackPath = "/Users/godju/Downloads/외주 폴더/pack_PACK_SAMPLE_3_V002.json";
        private const string ValidationCasesPath = "/Users/godju/Downloads/외주 폴더/encounter_validation_cases_v0.2.json";

        [Test]
        public void SamplePack_PassesV02Validation()
        {
            var result = EncounterPipelineV02Validator.ValidateFile(SamplePackPath);

            Assert.IsTrue(result.IsValid, result.ToSummary());
        }

        [Test]
        public void ValidationCases_RunnerMatchesExpectedResults()
        {
            var summary = EncounterPipelineV02ValidationCaseRunner.RunFile(ValidationCasesPath);

            Assert.IsTrue(summary.Passed, BuildCaseFailureSummary(summary));
        }

        [Test]
        public void Validator_FailsWhenEscapePolicyIsPresent()
        {
            var json = "{\"escapePolicy\":{\"mode\":\"SkillCheck\"}}";

            var result = EncounterPipelineV02Validator.ValidateTargetJson("CombatHandoff", json);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.HasErrorCode("COMBAT_ESCAPE_NOT_SUPPORTED_V02"), result.ToSummary());
        }

        [Test]
        public void Validator_FailsOldNpcStageEnum()
        {
            var json = "{\"npcStage\":\"S0_RECOVERY\"}";

            var result = EncounterPipelineV02Validator.ValidateTargetJson("Encounter", json);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.HasErrorCode("NPC_STAGE_ENUM_INVALID"), result.ToSummary());
        }

        [Test]
        public void Validator_FailsInvalidAssetPath()
        {
            var json = "{\"assetPath\":\"Assets/_Project/Data/Prototype/Encounters/SO_Encounter_INVALID.asset\"}";

            var result = EncounterPipelineV02Validator.ValidateTargetJson("Encounter", json);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.HasErrorCode("ENCOUNTER_ASSET_PATH_INVALID"), result.ToSummary());
        }

        [Test]
        public void Validator_FailsSilentChoice()
        {
            var json = "{\"kind\":\"Choice\",\"stableId\":\"CHOICE_INVALID_SILENT\",\"textKey\":\"PLACEHOLDER_CHOICE_INVALID_SILENT\",\"requirementMode\":\"All\",\"requirements\":[],\"effects\":[],\"unavailablePolicy\":{\"mode\":\"Hidden\"}}";

            var result = EncounterPipelineV02Validator.ValidateTargetJson("Choice", json);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.HasErrorCode("CHOICE_REQUIRES_EFFECT_OR_REACTION"), result.ToSummary());
        }

        [Test]
        public void Validator_FailsShopMissingGoldRequirement()
        {
            var json = "{\"encounterType\":\"Shop\",\"choices\":[{\"kind\":\"Choice\",\"stableId\":\"CHOICE_INVALID_SHOP\",\"textKey\":\"PLACEHOLDER_CHOICE_INVALID_SHOP\",\"requirementMode\":\"All\",\"requirements\":[],\"effects\":[{\"kind\":\"ModifyGold\",\"amount\":-5},{\"kind\":\"AddItem\",\"itemRef\":\"ITEM_VALID\",\"count\":1}],\"unavailablePolicy\":{\"mode\":\"DisabledVisible\",\"reasonTextKey\":\"PLACEHOLDER_REASON_NOT_ENOUGH_GOLD\"},\"npcImmediateReaction\":{\"reactionKey\":\"NPC_REACT_SHOP\",\"stage\":\"S2_COMPANION\",\"textKey\":\"PLACEHOLDER_NPC_REACT_SHOP\"}}]}";

            var result = EncounterPipelineV02Validator.ValidateTargetJson("Encounter", json);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.HasErrorCode("SHOP_PURCHASE_REQUIRES_GOLD_GATE"), result.ToSummary());
        }

        [Test]
        public void Validator_FailsMoralChoiceWithoutAffinityOrGlitch()
        {
            var json = "{\"encounterType\":\"MoralChoice\",\"choices\":[{\"kind\":\"Choice\",\"stableId\":\"CHOICE_INVALID_MORAL\",\"textKey\":\"PLACEHOLDER_CHOICE_INVALID_MORAL\",\"requirementMode\":\"All\",\"requirements\":[],\"effects\":[{\"kind\":\"ModifyMental\",\"amount\":-1}],\"unavailablePolicy\":{\"mode\":\"Hidden\"},\"npcImmediateReaction\":{\"reactionKey\":\"NPC_REACT_MORAL\",\"stage\":\"S1_AWARENESS\",\"textKey\":\"PLACEHOLDER_NPC_REACT_MORAL\"}}]}";

            var result = EncounterPipelineV02Validator.ValidateTargetJson("Encounter", json);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.HasErrorCode("MORAL_CHOICE_REQUIRES_AFFINITY_OR_GLITCH_EFFECT"), result.ToSummary());
        }

        [Test]
        public void Baker_BakesSamplePackIntoFormalEncounterAssets()
        {
            var result = EncounterPipelineV02Baker.BakeJsonFile(SamplePackPath);

            Assert.IsTrue(result.Success, result.Validation.ToSummary());
            Assert.AreEqual(3, result.AssetPaths.Count);
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_F01_COMBAT_GATE_001.asset"));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_MORAL_CHOICE_001.asset"));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_SHOP_001.asset"));
        }

        private static string BuildCaseFailureSummary(EncounterValidationCaseRunSummary summary)
        {
            var text = string.Empty;
            for (var i = 0; i < summary.Runs.Count; i++)
            {
                if (summary.Runs[i].Passed)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    text += "; ";
                }

                text += summary.Runs[i].Id + " expected " + summary.Runs[i].ExpectedErrorCode + " got " + summary.Runs[i].Validation.ToSummary();
            }

            return text;
        }
    }
}
