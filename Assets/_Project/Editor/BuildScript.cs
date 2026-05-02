using System.Linq;
using UnityEditor;

namespace HwigiTower.EditorTools
{
    public static class BuildScript
    {
        private const string AndroidBuildPath = "Builds/Android/hwigi-tower.apk";

        public static void BuildAndroid()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            BuildPipeline.BuildPlayer(scenes, AndroidBuildPath, BuildTarget.Android, BuildOptions.None);
        }
    }
}
