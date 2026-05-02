using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.Run;
using HwigiTower.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HwigiTower.EditorTools
{
    public static class PrototypeSceneBuilder
    {
        private const string DataRoot = "Assets/_Project/Data/Prototype";
        private const string ScenePath = "Assets/_Project/Scenes/PrototypeRoom.unity";

        [MenuItem("Hwigi Tower/Build Prototype Scene")]
        public static void Build()
        {
            EnsureFolders();

            var runtimeSettings = CreateRuntimeSettings();
            var movementProfile = CreateMovementProfile();
            var battle = CreateNodeDefinition("SO_Node_Battle", "node.battle", NodeKind.Battle, "전투");
            var rest = CreateNodeDefinition("SO_Node_Rest", "node.rest", NodeKind.Rest, "휴식");
            var shop = CreateNodeDefinition("SO_Node_Shop", "node.shop", NodeKind.Shop, "상점");
            var remnant = CreateNodeDefinition("SO_Node_Remnant", "node.remnant", NodeKind.Remnant, "잔재");
            var room = CreateRoomDefinition(new[] { battle, rest, shop, remnant });

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "PrototypeRoom";

            var mainCamera = CreateCamera();
            var hud = CreateHud();
            var roomController = CreateRuntime(room, runtimeSettings);
            CreateBounds();
            var player = CreatePlayer(movementProfile, mainCamera, hud, roomController);
            mainCamera.GetComponent<CameraFollow2D>().SetTarget(player.transform);
            CreateNode(battle, new Vector2(-1.9f, 2.6f), new Color(0.35f, 0.32f, 0.34f, 1f));
            CreateNode(rest, new Vector2(1.9f, 2.6f), new Color(0.30f, 0.36f, 0.34f, 1f));
            CreateNode(shop, new Vector2(-1.9f, -2.6f), new Color(0.35f, 0.34f, 0.29f, 1f));
            CreateNode(remnant, new Vector2(1.9f, -2.6f), new Color(0.28f, 0.32f, 0.38f, 1f));

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            PlayerSettings.defaultScreenWidth = 1080;
            PlayerSettings.defaultScreenHeight = 1920;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "Data");
            EnsureFolder("Assets/_Project/Data", "Prototype");
            EnsureFolder(DataRoot, "Nodes");
            EnsureFolder(DataRoot, "Rooms");
            EnsureFolder(DataRoot, "Settings");
            EnsureFolder("Assets/_Project", "Scenes");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static PrototypeRuntimeSettings CreateRuntimeSettings()
        {
            var settings = LoadOrCreate<PrototypeRuntimeSettings>($"{DataRoot}/Settings/SO_Prototype_RuntimeSettings.asset");
            SetInt(settings, "targetWidth", 1080);
            SetInt(settings, "targetHeight", 1920);
            SetBool(settings, "fullscreen", false);
            return settings;
        }

        private static PlayerMovementProfile CreateMovementProfile()
        {
            var profile = LoadOrCreate<PlayerMovementProfile>($"{DataRoot}/Settings/SO_Prototype_PlayerMovement.asset");
            SetFloat(profile, "moveSpeed", 4f);
            SetFloat(profile, "touchDeadZoneWorldUnits", 0.35f);
            return profile;
        }

        private static PrototypeNodeDefinition CreateNodeDefinition(string fileName, string nodeId, NodeKind kind, string displayName)
        {
            var definition = LoadOrCreate<PrototypeNodeDefinition>($"{DataRoot}/Nodes/{fileName}.asset");
            SetString(definition, "nodeId", nodeId);
            SetEnum(definition, "kind", (int)kind);
            SetString(definition, "displayName", displayName);
            SetString(definition, "placeholderOutcome", displayName);
            return definition;
        }

        private static PrototypeRoomDefinition CreateRoomDefinition(PrototypeNodeDefinition[] nodes)
        {
            var room = LoadOrCreate<PrototypeRoomDefinition>($"{DataRoot}/Rooms/SO_Room_Prototype.asset");
            SetString(room, "roomId", "room.prototype");
            SetInt(room, "deterministicSeed", 1001);
            SetVector2(room, "roomSize", new Vector2(6f, 10f));
            SetObjectArray(room, "availableNodes", nodes);
            return room;
        }

        private static Camera CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.09f, 0.10f, 0.12f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5.5f;

            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<CameraFollow2D>();
            return camera;
        }

        private static PrototypeHud CreateHud()
        {
            var canvasObject = new GameObject("Prototype HUD");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            var focus = CreateText(canvasObject.transform, "Focus Text", new Vector2(0f, -32f), TextAnchor.UpperCenter);
            var interaction = CreateText(canvasObject.transform, "Interaction Text", new Vector2(0f, -80f), TextAnchor.UpperCenter);
            interaction.text = "방";

            var hud = canvasObject.AddComponent<PrototypeHud>();
            SetObject(hud, "focusText", focus);
            SetObject(hud, "interactionText", interaction);
            return hud;
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 48f);
            rect.anchoredPosition = anchoredPosition;

            var text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 28;
            text.alignment = alignment;
            text.color = new Color(0.84f, 0.88f, 0.90f, 1f);
            text.text = string.Empty;
            return text;
        }

        private static PrototypeRoomController CreateRuntime(PrototypeRoomDefinition room, PrototypeRuntimeSettings runtimeSettings)
        {
            var runtime = new GameObject("Prototype Runtime");
            var bootstrap = runtime.AddComponent<PrototypeRuntimeBootstrap>();
            var roomController = runtime.AddComponent<PrototypeRoomController>();
            SetObject(bootstrap, "runtimeSettings", runtimeSettings);
            SetObject(roomController, "roomDefinition", room);
            return roomController;
        }

        private static GameObject CreatePlayer(PlayerMovementProfile movementProfile, Camera inputCamera, PrototypeHud hud, PrototypeRoomController roomController)
        {
            var player = CreateSpriteObject("Player", Vector2.zero, new Vector2(0.6f, 0.6f), new Color(0.78f, 0.82f, 0.86f, 1f));
            var body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.34f;

            var movement = player.AddComponent<PlayerMovementController>();
            var interaction = player.AddComponent<NodeInteractionController>();
            SetObject(movement, "movementProfile", movementProfile);
            SetObject(movement, "inputCamera", inputCamera);
            SetObject(interaction, "hud", hud);
            SetObject(interaction, "roomController", roomController);
            return player;
        }

        private static void CreateBounds()
        {
            CreateWall("Wall Top", new Vector2(0f, 5.2f), new Vector2(6.4f, 0.25f));
            CreateWall("Wall Bottom", new Vector2(0f, -5.2f), new Vector2(6.4f, 0.25f));
            CreateWall("Wall Left", new Vector2(-3.2f, 0f), new Vector2(0.25f, 10.4f));
            CreateWall("Wall Right", new Vector2(3.2f, 0f), new Vector2(0.25f, 10.4f));
        }

        private static void CreateWall(string name, Vector2 position, Vector2 size)
        {
            var wall = CreateSpriteObject(name, position, size, new Color(0.16f, 0.18f, 0.21f, 1f));
            wall.AddComponent<BoxCollider2D>();
        }

        private static void CreateNode(PrototypeNodeDefinition definition, Vector2 position, Color color)
        {
            var node = CreateSpriteObject($"{definition.DisplayName} Node", position, new Vector2(1.1f, 1.1f), color);
            var trigger = node.AddComponent<CircleCollider2D>();
            trigger.radius = 0.75f;
            trigger.isTrigger = true;

            var interactable = node.AddComponent<InteractableNode>();
            SetObject(interactable, "definition", definition);
            SetObject(interactable, "visual", node.GetComponent<SpriteRenderer>());

            var label = new GameObject("Label");
            label.transform.SetParent(node.transform, false);
            label.transform.localPosition = new Vector3(0f, -0.85f, 0f);

            var text = label.AddComponent<TextMesh>();
            text.text = definition.DisplayName;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = 0.18f;
            text.fontSize = 64;
            text.color = new Color(0.84f, 0.88f, 0.90f, 1f);
        }

        private static GameObject CreateSpriteObject(string name, Vector2 position, Vector2 scale, Color color)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.position = position;
            gameObject.transform.localScale = new Vector3(scale.x, scale.y, 1f);

            var renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = GetBuiltinSprite();
            renderer.color = color;
            return gameObject;
        }

        private static Sprite GetBuiltinSprite()
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd")
                ?? AssetDatabase.GetBuiltinExtraResource<Sprite>("Sprites/Square.psd");
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
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray(Object target, string propertyName, Object[] values)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);
            property.arraySize = values.Length;
            for (var i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(Object target, string propertyName, string value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(Object target, string propertyName, int value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(Object target, string propertyName, float value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string propertyName, bool value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum(Object target, string propertyName, int value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).enumValueIndex = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector2(Object target, string propertyName, Vector2 value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).vector2Value = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
