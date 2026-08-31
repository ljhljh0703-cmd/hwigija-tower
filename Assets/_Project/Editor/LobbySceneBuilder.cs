using HwigiTower.Audio;
using HwigiTower.Lobby;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HwigiTower.EditorTools
{
    public static class LobbySceneBuilder
    {
        private const string LobbyScenePath = "Assets/_Project/Scenes/Lobby.unity";
        private const string PrototypeScenePath = "Assets/_Project/Scenes/PrototypeRoom.unity";
        private const string AudioCatalogPath = "Assets/_Project/Data/Audio/SO_AudioCueCatalog.asset";
        private const string LobbyPresentationPath = "Assets/_Project/Data/Presentation/SO_LobbyPresentationData.asset";
        private const string LobbyBackgroundPath = "Assets/_Project/Art/Lobby/lobby_bg_tower_temp.png";

        [MenuItem("Hwigi Tower/Build Lobby Scene")]
        public static void Build()
        {
            EnsureFolders();
            var catalog = LoadOrCreate<AudioCueCatalog>(AudioCatalogPath);
            var presentation = LoadOrCreate<LobbyPresentationData>(LobbyPresentationPath);
            ConfigureLobbyPresentation(presentation);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Lobby";
            CreateCamera();
            CreateAudioService(catalog);
            CreateLobbyRuntime(catalog, presentation);

            EditorSceneManager.SaveScene(scene, LobbyScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(LobbyScenePath, true),
                new EditorBuildSettingsScene(PrototypeScenePath, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "Audio");
            EnsureFolder("Assets/_Project/Data", "Presentation");
            EnsureFolder("Assets/_Project", "Scenes");
            EnsureFolder("Assets/_Project", "Art");
            EnsureFolder("Assets/_Project/Art", "Lobby");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.025f, 0.03f, 0.04f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5.5f;
            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreateAudioService(AudioCueCatalog catalog)
        {
            var service = new GameObject("Prototype Audio Service").AddComponent<PrototypeAudioService>();
            SetObject(service, "cueCatalog", catalog);
        }

        private static void CreateLobbyRuntime(AudioCueCatalog catalog, LobbyPresentationData presentation)
        {
            var controller = new GameObject("Lobby Runtime").AddComponent<LobbyController>();
            SetString(controller, "newGameSceneName", "PrototypeRoom");
            SetObject(controller, "audioCueCatalog", catalog);
            SetObject(controller, "presentationData", presentation);
        }

        private static void ConfigureLobbyPresentation(LobbyPresentationData presentation)
        {
            var background = LoadLobbyBackgroundSprite();
            var serialized = new SerializedObject(presentation);
            serialized.FindProperty("backgroundSprite").objectReferenceValue = background;
            serialized.FindProperty("logoSprite").objectReferenceValue = null;
            serialized.FindProperty("titleText").stringValue = "회귀자는 탑을 오른다";
            serialized.FindProperty("subtitleText").stringValue = "되찾는 것은 잃기 위해서다";
            serialized.FindProperty("defaultProfileName").stringValue = "Player";
            serialized.FindProperty("showQuitButtonOnDesktopOnly").boolValue = true;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(presentation);
        }

        private static Sprite LoadLobbyBackgroundSprite()
        {
            if (!System.IO.File.Exists(LobbyBackgroundPath))
            {
                return null;
            }

            if (AssetImporter.GetAtPath(LobbyBackgroundPath) is TextureImporter importer)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 2048;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(LobbyBackgroundPath);
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(Object target, string propertyName, string value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
