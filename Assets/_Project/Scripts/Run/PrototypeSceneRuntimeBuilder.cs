using System.Collections.Generic;
using HwigiTower.Core;
using HwigiTower.Encounters;
using HwigiTower.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.Run
{
    public sealed class PrototypeSceneRuntimeBuilder : MonoBehaviour
    {
        [SerializeField] private PrototypeRuntimeSettings runtimeSettings;
        [SerializeField] private PlayerMovementProfile movementProfile;
        [SerializeField] private PrototypeRoomDefinition roomDefinition;
        [SerializeField] private EncounterRuntimeCatalogData encounterRuntimeCatalog;

        private static Sprite _placeholderSprite;

        private void Awake()
        {
            ApplyPortraitRuntimeSettings();

            var camera = CreateCamera();
            var hud = CreateHud();
            var roomController = CreateRoomController();
            CreateBounds();
            var player = CreatePlayer(camera, hud, roomController);
            camera.GetComponent<CameraFollow2D>().SetTarget(player.transform);
            CreateNodes(roomDefinition == null ? null : roomDefinition.AvailableNodes);
            roomController.BeginRun();
        }

        private void ApplyPortraitRuntimeSettings()
        {
            Application.targetFrameRate = 60;

            if (runtimeSettings != null && !Application.isMobilePlatform)
            {
                Screen.SetResolution(runtimeSettings.TargetWidth, runtimeSettings.TargetHeight, runtimeSettings.Fullscreen);
            }
        }

        private Camera CreateCamera()
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

        private PrototypeHud CreateHud()
        {
            var canvasObject = new GameObject("Prototype HUD");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;

            canvasObject.AddComponent<GraphicRaycaster>();

            var focus = CreateText(canvasObject.transform, "Focus Text", new Vector2(0f, -40f));
            var interaction = CreateText(canvasObject.transform, "Interaction Text", new Vector2(0f, -96f));
            var runState = CreateText(canvasObject.transform, "Run State Text", new Vector2(0f, -152f));
            var hud = canvasObject.AddComponent<PrototypeHud>();
            hud.Configure(focus, interaction, runState);
            return hud;
        }

        private Text CreateText(Transform parent, string name, Vector2 anchoredPosition)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 52f);
            rect.anchoredPosition = anchoredPosition;

            var text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 30;
            text.alignment = TextAnchor.UpperCenter;
            text.color = new Color(0.84f, 0.88f, 0.90f, 1f);
            return text;
        }

        private PrototypeRoomController CreateRoomController()
        {
            var controllerObject = new GameObject("Prototype Room");
            var controller = controllerObject.AddComponent<PrototypeRoomController>();
            controller.Configure(roomDefinition, encounterRuntimeCatalog);
            return controller;
        }

        private GameObject CreatePlayer(Camera inputCamera, PrototypeHud hud, PrototypeRoomController roomController)
        {
            var player = CreateSpriteObject("Player", Vector2.zero, new Vector2(0.6f, 0.6f), new Color(0.78f, 0.82f, 0.86f, 1f));

            var body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.34f;

            var movement = player.AddComponent<PlayerMovementController>();
            movement.Configure(movementProfile, inputCamera);

            var interaction = player.AddComponent<NodeInteractionController>();
            interaction.Configure(hud, roomController);
            return player;
        }

        private void CreateBounds()
        {
            CreateWall("Wall Top", new Vector2(0f, 5.2f), new Vector2(6.4f, 0.25f));
            CreateWall("Wall Bottom", new Vector2(0f, -5.2f), new Vector2(6.4f, 0.25f));
            CreateWall("Wall Left", new Vector2(-3.2f, 0f), new Vector2(0.25f, 10.4f));
            CreateWall("Wall Right", new Vector2(3.2f, 0f), new Vector2(0.25f, 10.4f));
        }

        private void CreateWall(string name, Vector2 position, Vector2 size)
        {
            var wall = CreateSpriteObject(name, position, size, new Color(0.16f, 0.18f, 0.21f, 1f));
            wall.AddComponent<BoxCollider2D>();
        }

        private void CreateNodes(IReadOnlyList<PrototypeNodeDefinition> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return;
            }

            var positions = new[]
            {
                new Vector2(-1.9f, 2.6f),
                new Vector2(1.9f, 2.6f),
                new Vector2(-1.9f, -2.6f),
                new Vector2(1.9f, -2.6f)
            };

            var colors = new[]
            {
                new Color(0.35f, 0.32f, 0.34f, 1f),
                new Color(0.30f, 0.36f, 0.34f, 1f),
                new Color(0.35f, 0.34f, 0.29f, 1f),
                new Color(0.28f, 0.32f, 0.38f, 1f)
            };

            for (var i = 0; i < nodes.Count && i < positions.Length; i++)
            {
                CreateNode(nodes[i], positions[i], colors[i]);
            }
        }

        private void CreateNode(PrototypeNodeDefinition definition, Vector2 position, Color color)
        {
            if (definition == null)
            {
                return;
            }

            var node = CreateSpriteObject($"{definition.DisplayName} Node", position, new Vector2(1.1f, 1.1f), color);

            var trigger = node.AddComponent<CircleCollider2D>();
            trigger.radius = 0.75f;
            trigger.isTrigger = true;

            var interactable = node.AddComponent<InteractableNode>();
            interactable.Configure(definition, node.GetComponent<SpriteRenderer>());

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

        private GameObject CreateSpriteObject(string name, Vector2 position, Vector2 scale, Color color)
        {
            var spriteObject = new GameObject(name);
            spriteObject.transform.position = position;
            spriteObject.transform.localScale = new Vector3(scale.x, scale.y, 1f);

            var renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = GetPlaceholderSprite();
            renderer.color = color;
            return spriteObject;
        }

        private static Sprite GetPlaceholderSprite()
        {
            if (_placeholderSprite != null)
            {
                return _placeholderSprite;
            }

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            _placeholderSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            _placeholderSprite.name = "Runtime Placeholder Sprite";
            return _placeholderSprite;
        }
    }
}
