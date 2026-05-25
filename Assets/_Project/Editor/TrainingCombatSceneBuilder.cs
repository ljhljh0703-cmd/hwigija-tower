using HwigiTower.Training;
using Unity.MLAgents.Policies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HwigiTower.EditorTools
{
    public static class TrainingCombatSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/TrainingCombat.unity";
        private const string BuildPath = "/private/tmp/hwigi-mlagents-build/TrainingCombat.app";

        [MenuItem("Hwigi Tower/Training/Rebuild Training Combat Scene")]
        public static void RebuildTrainingCombatScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TrainingCombat";

            var agentObject = new GameObject("CombatTrainingAgent");
            agentObject.transform.position = Vector3.zero;

            var behavior = agentObject.AddComponent<BehaviorParameters>();
            behavior.BehaviorName = "TrainingCombat";
            behavior.BehaviorType = BehaviorType.Default;
            behavior.TeamId = 0;
            behavior.BrainParameters.VectorObservationSize = 8;
            behavior.BrainParameters.NumStackedVectorObservations = 1;
            behavior.BrainParameters.ActionSpec = Unity.MLAgents.Actuators.ActionSpec.MakeDiscrete(3);

            agentObject.AddComponent<CombatTrainingAgent>();

            var cameraObject = new GameObject("Training Camera");
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            cameraObject.AddComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
        }

        public static void BuildTrainingCombatMac()
        {
            RebuildTrainingCombatScene();
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(BuildPath));

            var report = BuildPipeline.BuildPlayer(
                new[] { ScenePath },
                BuildPath,
                BuildTarget.StandaloneOSX,
                BuildOptions.None);

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("TrainingCombat build failed: " + report.summary.result);
                EditorApplication.Exit(1);
                return;
            }

            Debug.Log("TrainingCombat build completed: " + BuildPath);
        }
    }
}
