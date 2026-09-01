using System.IO;
using System.Text.RegularExpressions;
using HwigiTower.Encounters;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace HwigiTower.Tests.EditMode
{
    internal static class EditModeCatalogRefreshLogGuard
    {
        // Only this immutable-package meta pattern is expected; global LogAssert suppression remains forbidden.
        public const string MlAgentsImmutableSamplesMetaErrorPattern = "^Asset Packages/com\\.unity\\.ml-agents/Samples(?:/3DBall(?:/3DBall\\.unitypackage)?)? has no meta file, but it's in an immutable folder\\. The asset will be ignored\\.$";

        public const string CatalogPath = HwigiTower.Encounters.EncounterRuntimeCatalogBuilder.CatalogPath;
        public const string PrototypeBossGateEncounterPath = HwigiTower.Encounters.EncounterRuntimeCatalogBuilder.PrototypeBossGateEncounterPath;
        public const string PrototypeFloorTwoShopEncounterPath = HwigiTower.Encounters.EncounterRuntimeCatalogBuilder.PrototypeFloorTwoShopEncounterPath;

        public static EncounterRuntimeCatalogBuildResult BuildDefaultCatalog()
        {
            ExpectKnownMlAgentsImmutableSamplesMetaError();
            return HwigiTower.Encounters.EncounterRuntimeCatalogBuilder.BuildDefaultCatalog();
        }

        internal static void ExpectKnownMlAgentsImmutableSamplesMetaError()
        {
            var package = PackageInfo.FindForAssetPath("Packages/com.unity.ml-agents");
            if (package == null)
            {
                return;
            }

            var samplesDirectory = Path.Combine(package.resolvedPath, "Samples");
            var immutableTargets = new[]
            {
                samplesDirectory,
                Path.Combine(samplesDirectory, "3DBall"),
                Path.Combine(samplesDirectory, "3DBall", "3DBall.unitypackage")
            };
            var expectedLogCount = 0;
            for (var i = 0; i < immutableTargets.Length; i++)
            {
                if (File.Exists(immutableTargets[i]) || Directory.Exists(immutableTargets[i]))
                {
                    if (!File.Exists(immutableTargets[i] + ".meta"))
                    {
                        expectedLogCount++;
                    }
                }
            }

            var pattern = new Regex(MlAgentsImmutableSamplesMetaErrorPattern);
            for (var i = 0; i < expectedLogCount; i++)
            {
                LogAssert.Expect(LogType.Error, pattern);
            }
        }
    }

    internal static class EditModeEncounterBakerLogGuard
    {
        public static EncounterBakeResult BakeJsonFile(string path)
        {
            EditModeCatalogRefreshLogGuard.ExpectKnownMlAgentsImmutableSamplesMetaError();
            return HwigiTower.Encounters.EncounterPipelineV02Baker.BakeJsonFile(path);
        }

        public static EncounterBakeResult BakeJsonFile(string path, EncounterBakeOptions options)
        {
            if (!options.DryRun)
            {
                EditModeCatalogRefreshLogGuard.ExpectKnownMlAgentsImmutableSamplesMetaError();
            }

            return HwigiTower.Encounters.EncounterPipelineV02Baker.BakeJsonFile(path, options);
        }

        public static EncounterBakeResult BakeJson(string json, EncounterBakeOptions options)
        {
            return HwigiTower.Encounters.EncounterPipelineV02Baker.BakeJson(json, options);
        }
    }
}
