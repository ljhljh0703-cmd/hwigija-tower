using System.IO;
using System.Linq;
using HwigiTower.Encounters;
using NUnit.Framework;
using UnityEditor;

namespace HwigiTower.Tests.EditMode
{
    public sealed class EncounterPipelineV02Tests
    {
        private const string SamplePackPath = "/Users/godju/Downloads/외주 폴더/pack_PACK_SAMPLE_3_V002.json";
        private const string FullPackPath = "/Users/godju/Downloads/외주 폴더/pack_FULL_25_V003.json";
        private const string ValidationCasesPath = "/Users/godju/Downloads/외주 폴더/encounter_validation_cases_v0.2.json";

        [Test]
        public void SamplePack_PassesV02Validation()
        {
            var result = EncounterPipelineV02Validator.ValidateFile(SamplePackPath);

            Assert.IsTrue(result.IsValid, result.ToSummary());
        }

        [Test]
        public void FullPack_PassesV02Validation()
        {
            var result = EncounterPipelineV02Validator.ValidateFile(FullPackPath);

            Assert.IsTrue(result.IsValid, result.ToSummary());
        }

        [Test]
        public void RuntimeCatalog_BuildsRequiredStableIdLookups()
        {
            var result = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog();
            var catalog = result.Catalog;

            Assert.NotNull(catalog);
            Assert.IsTrue(catalog.TryGetItem("ITEM_FIELD_BANDAGE", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_LANTERN_OIL", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_TORN_CHARM", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_01", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_02", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_03", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_04", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_05", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_09", out _));
            Assert.IsTrue(catalog.TryGetItem("ITEM_10", out _));
            Assert.IsTrue(catalog.TryGetRewardBundle("REWARD_CACHE_MEMORY", out _));
            Assert.IsTrue(catalog.TryGetRewardBundle("REWARD_CACHE_SMALL", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_SCOUT", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_RECALL_ANCHOR", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_SWORD_01", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_SWORD_02", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_SWORD_03", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_ARTS_03", out _));
            Assert.IsTrue(catalog.TryGetAbility("ABILITY_GUARD_01", out _));
            Assert.IsTrue(catalog.TryGetEnemy("ENEMY_COLLAPSE_ECHO", out _));
            Assert.IsTrue(catalog.TryGetEnemy("ENEMY_EMPTY_ARMOR", out _));
            Assert.IsTrue(catalog.TryGetEnemy("ENEMY_FRACTURE_HOUND", out _));
            Assert.IsTrue(catalog.TryGetMemoryFragment("MEM_FRAGMENT_01", out _));
            Assert.IsTrue(catalog.TryGetMemoryFragment("MEM_FRAGMENT_02", out _));
            Assert.IsTrue(catalog.TryGetMemoryFragment("MEM_FRAGMENT_03", out _));
            Assert.IsTrue(catalog.TryGetMemoryFragment("MEM_FRAGMENT_04", out _));
            Assert.IsTrue(catalog.TryGetMemoryFragment("MEM_FRAGMENT_05", out _));
        }

        [Test]
        public void RuntimeCatalogReferenceValidator_FullPackHasNoMissingRefsAndRecognizesMemoryFragments()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;

            var report = EncounterRuntimeCatalogReferenceValidator.ValidateFile(FullPackPath, catalog);

            Assert.AreEqual(25, report.EncounterCount);
            Assert.AreEqual(0, report.MissingCatalogRefs.Count, string.Join("; ", report.MissingCatalogRefs));
            Assert.AreEqual(0, report.DuplicateStableIds.Count, string.Join("; ", report.DuplicateStableIds));
            Assert.AreEqual(5, report.RecognizedMemoryFragmentRefs.Count);
            CollectionAssert.Contains(report.RecognizedMemoryFragmentRefs, "MEM_FRAGMENT_01");
            CollectionAssert.Contains(report.RecognizedMemoryFragmentRefs, "MEM_FRAGMENT_05");
        }

        [Test]
        public void RuntimeCatalogReferenceValidator_ReportsDuplicateEncounterStableIds()
        {
            var catalog = EncounterRuntimeCatalogBuilder.BuildDefaultCatalog().Catalog;
            var json = File.ReadAllText(FullPackPath).Replace(
                "\"stableId\": \"ENC_REST_02\"",
                "\"stableId\": \"ENC_REST_01\"");

            var report = EncounterRuntimeCatalogReferenceValidator.ValidateJson(json, catalog);

            CollectionAssert.Contains(report.DuplicateStableIds, "ENC_REST_01");
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
            var json = "{\"escape" + "Policy\":{\"mode\":\"SkillCheck\"}}";

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
            var moral = AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_MORAL_CHOICE_001.asset");
            Assert.NotNull(moral);
            Assert.AreEqual(3, moral.Choices.Length);
            Assert.AreEqual("CHOICE_F02_MORAL_HELP", moral.Choices[0].stableId);
            Assert.AreEqual("ModifyHp", moral.Choices[0].effects[0].kind);
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_SHOP_001.asset"));
        }

        [Test]
        public void Baker_DryRunThenBakesFullPackIntoFormalEncounterAssets()
        {
            EncounterRuntimeCatalogBuilder.BuildDefaultCatalog();

            var dryRun = EncounterPipelineV02Baker.BakeJsonFile(FullPackPath, EncounterBakeOptions.DryRunOnly);

            Assert.IsTrue(dryRun.Success, dryRun.Validation.ToSummary());
            Assert.IsTrue(dryRun.DryRun);
            Assert.AreEqual(25, dryRun.AssetPaths.Count);
            Assert.AreEqual(0, dryRun.Reports.Count, string.Join("; ", dryRun.Reports));

            var bake = EncounterPipelineV02Baker.BakeJsonFile(FullPackPath, EncounterBakeOptions.Apply);

            Assert.IsTrue(bake.Success, bake.Validation.ToSummary());
            Assert.AreEqual(25, bake.AssetPaths.Count);
            Assert.AreEqual(25, bake.AssetPaths.Count(path => AssetDatabase.LoadAssetAtPath<EncounterData>(path) != null));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_SHOP_01.asset"));
            Assert.NotNull(AssetDatabase.LoadAssetAtPath<EncounterData>("Assets/_Project/Data/Encounters/SO_Encounter_ENC_COMBAT_GATE_03.asset"));
            var bossGate = AssetDatabase.LoadAssetAtPath<EncounterData>(EncounterRuntimeCatalogBuilder.PrototypeBossGateEncounterPath);
            Assert.AreEqual("BOSS_GATE_01", bossGate.Choices[0].effects[0].combatHandoff.enemyRefs[0]);
            var floorTwoShop = AssetDatabase.LoadAssetAtPath<EncounterData>(EncounterRuntimeCatalogBuilder.PrototypeFloorTwoShopEncounterPath);
            var itemChoice = floorTwoShop.Choices.First(choice => choice.stableId == "CHOICE_F02_SHOP_BUY_ITEM");
            var abilityChoice = floorTwoShop.Choices.First(choice => choice.stableId == "CHOICE_F02_SHOP_BUY_ABILITY");
            Assert.AreEqual("ITEM_04", itemChoice.effects[1].itemRef);
            Assert.AreEqual("ABILITY_SWORD_02", abilityChoice.effects[1].abilityRef);
        }

        [Test]
        public void Baker_DryRunReportsSourceHashAndStableIdConflicts()
        {
            var bake = EncounterPipelineV02Baker.BakeJsonFile(SamplePackPath);
            Assert.IsTrue(bake.Success, bake.ToSummary());

            var changedHashJson = File.ReadAllText(SamplePackPath)
                .Replace(
                    "\"sourceHash\": \"sha256:1111111111111111111111111111111111111111111111111111111111111111\"",
                    "\"sourceHash\": \"sha256:bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb\"");

            var hashDryRun = EncounterPipelineV02Baker.BakeJson(changedHashJson, EncounterBakeOptions.DryRunOnly);

            Assert.IsTrue(hashDryRun.Success, hashDryRun.ToSummary());
            Assert.IsTrue(hashDryRun.DryRun);
            Assert.IsTrue(hashDryRun.Reports.Any(report => report.Contains("SOURCE_HASH_CHANGED")), hashDryRun.ToSummary());

            var pathConflictJson = File.ReadAllText(SamplePackPath)
                .Replace(
                    "Assets/_Project/Data/Encounters/SO_Encounter_ENC_F01_COMBAT_GATE_001.asset",
                    "Assets/_Project/Data/Encounters/SO_Encounter_ENC_F02_SHOP_001.asset");

            var conflictDryRun = EncounterPipelineV02Baker.BakeJson(pathConflictJson, EncounterBakeOptions.DryRunOnly);

            Assert.IsTrue(conflictDryRun.Success, conflictDryRun.ToSummary());
            Assert.IsTrue(conflictDryRun.Reports.Any(report => report.Contains("STABLE_ID_PATH_CONFLICT")), conflictDryRun.ToSummary());
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
