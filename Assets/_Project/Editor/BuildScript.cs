using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.EditorTools
{
    public static class BuildScript
    {
        private const string AndroidBuildPath = "Builds/Android/hwigi-tower.apk";

        public static void BuildAndroid()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                Debug.LogError("Android build target is not installed in this Unity Editor.");
                EditorApplication.Exit(1);
                return;
            }

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            Directory.CreateDirectory(Path.GetDirectoryName(AndroidBuildPath));
            BuildPipeline.BuildPlayer(scenes, AndroidBuildPath, BuildTarget.Android, BuildOptions.None);
        }
    }
}
