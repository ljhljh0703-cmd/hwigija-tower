using System.Collections;
using System.IO;
using HwigiTower.Run;
using HwigiTower.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace HwigiTower.Tests.PlayMode
{
    public sealed class PortraitUiScreenshotQaTests
    {
        private const int ScreenshotWidth = 1080;
        private const int ScreenshotHeight = 1920;
        private const string ScreenshotDirectory = "/private/tmp/hwigi-portrait-ui-v3-screenshots";

        [SetUp]
        public void SetUp()
        {
            ResetRunStateIsolation();
        }

        [TearDown]
        public void TearDown()
        {
            ResetRunStateIsolation();
        }

        [UnityTest]
        [Explicit("Screenshot QA is manual-only and excluded from standard PlayMode validation.")]
        public IEnumerator PortraitUiV3_CapturesRequiredQaScreens()
        {
            Directory.CreateDirectory(ScreenshotDirectory);
            DeleteRequiredScreenshots();
            Screen.SetResolution(ScreenshotWidth, ScreenshotHeight, false);
            yield return WaitForFrames(8);

            yield return CaptureLobby();
            yield return CapturePrototypeRoomScreen("02_floor_map.png", (controller, hud) =>
            {
                controller.ConfirmPreRunPlaceholder();
                hud.ShowRunState(controller.GetSnapshot());
                var mapNodes = controller.GetFloorMapNodes();
                Assert.Greater(mapNodes.Length, 0, "Expected current floor route map nodes.");
                Assert.IsTrue(ContainsSelectableMapNode(mapNodes), "Expected current route map to expose an active route node.");
                Assert.IsTrue(ContainsLockedMapNode(mapNodes), "Expected current route map to expose locked future route nodes.");
                hud.ShowMapChoices(mapNodes, mapNodeId =>
                {
                    var selection = controller.SelectMapNode(mapNodeId);
                    hud.OpenQaRouteStep(selection);
                });
                Assert.IsFalse(hud.PreRunPlaceholderVisible, "Map capture must not be blocked by the pre-run placeholder.");
                Assert.IsTrue(hud.NodeMapVisible, "Expected current route map to be visible.");
            });
            yield return CaptureEncounterScreen("03_event_jar_room.png", "EVT_F01_JAR_ROOM");
            yield return CaptureRestScreen();
            yield return CaptureShopScreen();
            yield return CaptureFloorShopScreen("09_shop_floor3.png", 3, "ENC_SHOP_03", "enc_shop_03_bg", "merchant_human");
            yield return CaptureFloorShopScreen("10_shop_floor4.png", 4, "ENC_SHOP_04", "enc_shop_04_bg", "merchant_otherworld");
            yield return CaptureFloorShopScreen("11_shop_floor5.png", 5, "ENC_SHOP_05", "enc_shop_05_bg", "merchant_otherworld");
            yield return CaptureCombatScreen("06_normal_combat.png", "ENC_COMBAT_GATE_01");
            yield return CaptureCombatScreen("07_boss_combat.png", "ENC_COMBAT_GATE_03");
            yield return CaptureBossGateChoices();
            yield return CaptureEndingChoice();

            AssertRequiredScreenshots();
        }

        private static IEnumerator CaptureLobby()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return WaitForFrames(10);

            Assert.IsNotNull(GameObject.Find("Lobby Canvas"));
            Assert.IsNotNull(GameObject.Find("Lobby Title"));
            Assert.IsNotNull(GameObject.Find("Lobby New Game Button"));
            yield return CaptureAndAssert("01_lobby.png");
        }

        private static IEnumerator CaptureEncounterScreen(string fileName, string encounterId)
        {
            yield return CapturePrototypeRoomScreen(fileName, (controller, hud) =>
            {
                var selection = controller.CreateQaEncounterSelection(encounterId);
                Assert.IsTrue(selection.HasEncounter, "Missing QA encounter selection for " + encounterId);
                hud.OpenQaRouteStep(selection);
            });
        }

        private static IEnumerator CaptureShopScreen()
        {
            yield return CapturePrototypeRoomScreen("05_shop.png", (controller, hud) =>
            {
                var selection = controller.CreateQaEncounterSelection("ENC_SHOP_01");
                Assert.IsTrue(selection.HasEncounter, "Missing QA shop selection");
                controller.RunState.ModifyGold(100);
                hud.OpenQaRouteStep(selection);

                var merchantVisual = GameObject.Find("Merchant Visual");
                Assert.IsNotNull(merchantVisual, "Missing merchant spotlight visual");
                Assert.IsTrue(merchantVisual.activeInHierarchy, "Merchant spotlight should be visible for shop");
                var image = merchantVisual.GetComponent<Image>();
                Assert.IsNotNull(image);
                Assert.IsNotNull(image.sprite);
                Assert.AreEqual("merchant_human", image.sprite.name);
                Assert.IsTrue(hud.NpcSpotlightVisible);
                Assert.AreEqual("상점", hud.CurrentNpcSpotlightModeLabel);
                StringAssert.Contains("상인", hud.NpcSpotlightMessage);
                StringAssert.Contains("ui_spotlight_gradient", hud.CurrentNpcSupportSpriteNames);
                StringAssert.Contains("ui_npc_dialogue_plate", hud.CurrentNpcSupportSpriteNames);
                StringAssert.Contains("ui_shop_product_card", hud.CurrentShopChoiceCardSpriteNames);
                StringAssert.Contains("icon_item_field_bandage", hud.CurrentShopChoiceIconNames);
                StringAssert.Contains("icon_ability_scout", hud.CurrentShopChoiceIconNames);
                Assert.GreaterOrEqual(hud.ChoiceButtonCount, 3, "Expected opening shop products plus leave.");
                Assert.IsNotNull(FindChoiceButtonTextContaining(hud, "보유 Gold"), "Expected current shop product cards to include player Gold context.");
            });
        }

        private static IEnumerator CaptureFloorShopScreen(string fileName, int floor, string encounterId, string expectedBackground, string expectedMerchant)
        {
            yield return CapturePrototypeRoomScreen(fileName, (controller, hud) =>
            {
                Assert.IsTrue(controller.OpenQaFloor(floor), "Missing QA floor path " + floor);
                controller.RunState.ModifyGold(100);
                var selection = controller.CreateQaEncounterSelection(encounterId);
                Assert.IsTrue(selection.HasEncounter, "Missing QA shop selection " + encounterId);
                hud.OpenQaRouteStep(selection);

                var merchantVisual = GameObject.Find("Merchant Visual");
                Assert.IsNotNull(merchantVisual, "Missing merchant spotlight visual for floor " + floor);
                var image = merchantVisual.GetComponent<Image>();
                Assert.IsNotNull(image);
                Assert.IsNotNull(image.sprite);
                Assert.AreEqual(expectedMerchant, image.sprite.name);
                Assert.AreEqual(expectedBackground, hud.CurrentBackgroundSpriteName);
                Assert.IsTrue(hud.NpcSpotlightVisible);
                Assert.AreEqual("상점", hud.CurrentNpcSpotlightModeLabel);
                StringAssert.Contains("ui_spotlight_gradient", hud.CurrentNpcSupportSpriteNames);
                StringAssert.Contains("ui_shop_product_card", hud.CurrentShopChoiceCardSpriteNames);
            });
        }

        private static IEnumerator CaptureRestScreen()
        {
            yield return CapturePrototypeRoomScreen("04_rest_mataios.png", (controller, hud) =>
            {
                var selection = controller.CreateQaEncounterSelection("ENC_REST_01");
                Assert.IsTrue(selection.HasEncounter, "Missing QA rest selection");
                hud.OpenQaRouteStep(selection);
                Assert.IsTrue(hud.RestInteractionPanelVisible);
                Assert.IsFalse(hud.NpcSpotlightVisible);
                Assert.AreEqual("enc_rest_01_bg", hud.CurrentBackgroundSpriteName);
                StringAssert.Contains("icon_rest_talk", hud.CurrentRestActionIconNames);
                StringAssert.Contains("icon_rest_train", hud.CurrentRestActionIconNames);
                StringAssert.Contains("icon_rest_recover", hud.CurrentRestActionIconNames);
            });
        }

        private static IEnumerator CaptureCombatScreen(string fileName, string encounterId)
        {
            yield return CapturePrototypeRoomScreen(fileName, (controller, hud) =>
            {
                controller.AutoResolveCombat = false;
                controller.RunState.ModifyGold(100);
                var selection = controller.CreateQaEncounterSelection(encounterId);
                Assert.IsTrue(selection.HasEncounter, "Missing QA combat selection for " + encounterId);
                hud.OpenQaRouteStep(selection);
                EnsureCombatStarted(controller, hud, encounterId);
                Assert.IsTrue(controller.RunState.IsInCombat, "Expected active combat for " + encounterId);
                hud.ShowRunState(controller.GetSnapshot());
                StringAssert.Contains("icon_gold", hud.CurrentTopHudIconNames);
                Assert.AreEqual("char_player_portrait_01", hud.CurrentCombatPlayerPortraitSpriteName);
                Assert.AreEqual("char_mataios_portrait_01", hud.CurrentCombatMataiosPortraitSpriteName);
                Assert.IsFalse(hud.CombatPortraitFrameVisible);
            });
        }

        private static IEnumerator CaptureEndingChoice()
        {
            yield return CapturePrototypeRoomScreen("08_ending_choice.png", (controller, hud) =>
            {
                controller.ConfirmPreRunPlaceholder();
                controller.OpenQaEndingChoice();
                hud.ShowRunState(controller.GetSnapshot());
                var snapshot = controller.GetSnapshot();
                Assert.IsTrue(snapshot.RunCompleted, "Expected ending capture to use a completed run state.");
                Assert.IsFalse(hud.PreRunPlaceholderVisible, "Ending capture must not show pre-run placeholder.");
                if (snapshot.EndingChoicePending)
                {
                    Assert.IsTrue(
                        hud.EndingRestButtonVisible || hud.EndingContinueButtonVisible,
                        "Expected current ending choice UI to expose an ending action.");
                }
            });
        }

        private static IEnumerator CaptureBossGateChoices()
        {
            yield return CapturePrototypeRoomScreen("12_boss_gate_choices.png", (controller, hud) =>
            {
                var selection = controller.CreateQaEncounterSelection("ENC_COMBAT_GATE_03");
                Assert.IsTrue(selection.HasEncounter, "Missing QA boss gate selection");
                hud.OpenQaRouteStep(selection);
                Assert.IsTrue(
                    controller.RunState.IsInCombat || hud.ChoiceButtonCount > 0,
                    "Expected boss gate to either auto-start combat or expose current gate choices.");
            });
        }

        private static IEnumerator CapturePrototypeRoomScreen(string fileName, System.Action<PrototypeRoomController, PrototypeHud> arrange)
        {
            yield return SceneManager.LoadSceneAsync("PrototypeRoom", LoadSceneMode.Single);
            yield return WaitForFrames(8);

            var controller = Object.FindFirstObjectByType<PrototypeRoomController>();
            var hud = Object.FindFirstObjectByType<PrototypeHud>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(hud);
            Assert.IsTrue(hud.HasPortraitRoot);
            Assert.AreEqual(new Vector2(ScreenshotWidth, ScreenshotHeight), hud.PortraitRootSize);

            arrange(controller, hud);
            yield return WaitForStablePortfolioFrame(hud);
            yield return CaptureAndAssert(fileName);
        }

        private static IEnumerator CaptureAndAssert(string fileName)
        {
            var path = Path.Combine(ScreenshotDirectory, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            yield return WaitForFrames(2);
            CaptureVisibleCanvases(path);
            Assert.IsTrue(File.Exists(path), "Screenshot was not created: " + path);
            Assert.Greater(new FileInfo(path).Length, 0, "Screenshot was empty: " + path);
            AssertPngSize(path);
        }

        private static void CaptureVisibleCanvases(string path)
        {
            var camera = Camera.main;
            var createdCamera = false;
            if (camera == null)
            {
                var cameraObject = new GameObject("QA Screenshot Camera");
                camera = cameraObject.AddComponent<Camera>();
                createdCamera = true;
            }

            var previousClearFlags = camera.clearFlags;
            var previousBackground = camera.backgroundColor;
            var previousOrthographic = camera.orthographic;
            var previousOrthographicSize = camera.orthographicSize;
            var previousPosition = camera.transform.position;
            var previousRotation = camera.transform.rotation;
            var previousTargetTexture = camera.targetTexture;
            var previousActive = RenderTexture.active;

            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude);
            var renderModes = new RenderMode[canvases.Length];
            var worldCameras = new Camera[canvases.Length];
            var planeDistances = new float[canvases.Length];

            var renderTexture = new RenderTexture(ScreenshotWidth, ScreenshotHeight, 24, RenderTextureFormat.ARGB32);
            var texture = new Texture2D(ScreenshotWidth, ScreenshotHeight, TextureFormat.RGB24, false);

            try
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.orthographic = true;
                camera.orthographicSize = 10f;
                camera.transform.position = new Vector3(0f, 0f, -10f);
                camera.transform.rotation = Quaternion.identity;
                camera.targetTexture = renderTexture;

                for (var i = 0; i < canvases.Length; i++)
                {
                    var canvas = canvases[i];
                    renderModes[i] = canvas.renderMode;
                    worldCameras[i] = canvas.worldCamera;
                    planeDistances[i] = canvas.planeDistance;

                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = camera;
                    canvas.planeDistance = 1f;
                }

                Canvas.ForceUpdateCanvases();
                camera.Render();
                RenderTexture.active = renderTexture;
                texture.ReadPixels(new Rect(0f, 0f, ScreenshotWidth, ScreenshotHeight), 0, 0);
                texture.Apply();
                File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            finally
            {
                for (var i = 0; i < canvases.Length; i++)
                {
                    if (canvases[i] == null)
                    {
                        continue;
                    }

                    canvases[i].renderMode = renderModes[i];
                    canvases[i].worldCamera = worldCameras[i];
                    canvases[i].planeDistance = planeDistances[i];
                }

                camera.clearFlags = previousClearFlags;
                camera.backgroundColor = previousBackground;
                camera.orthographic = previousOrthographic;
                camera.orthographicSize = previousOrthographicSize;
                camera.transform.position = previousPosition;
                camera.transform.rotation = previousRotation;
                camera.targetTexture = previousTargetTexture;
                RenderTexture.active = previousActive;

                Object.Destroy(texture);
                renderTexture.Release();
                Object.Destroy(renderTexture);
                if (createdCamera)
                {
                    Object.Destroy(camera.gameObject);
                }
            }
        }

        private static void AssertPngSize(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            Assert.IsTrue(ImageConversion.LoadImage(texture, bytes), "Failed to load screenshot PNG: " + path);
            Assert.AreEqual(ScreenshotWidth, texture.width, "Unexpected screenshot width: " + path);
            Assert.AreEqual(ScreenshotHeight, texture.height, "Unexpected screenshot height: " + path);
            Object.Destroy(texture);
        }

        private static void ResetRunStateIsolation()
        {
            PrototypeRunSaveStore.Delete();
            PrototypeRunSaveRequest.RequestNewGame();
            Time.timeScale = 1f;
        }

        private static void EnsureCombatStarted(PrototypeRoomController controller, PrototypeHud hud, string encounterId)
        {
            if (controller.RunState.IsInCombat)
            {
                return;
            }

            var button = FindFirstInteractableChoiceButton(hud);
            Assert.IsNotNull(button, "Expected combat to auto-start or expose an enabled start choice for " + encounterId);
            button.onClick.Invoke();
        }

        private static Button FindFirstInteractableChoiceButton(PrototypeHud hud)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button != null && button.gameObject.activeInHierarchy && button.interactable)
                {
                    return button;
                }
            }

            return null;
        }

        private static Text FindChoiceButtonTextContaining(PrototypeHud hud, string value)
        {
            for (var i = 0; i < hud.ChoiceButtonCount; i++)
            {
                var button = hud.GetChoiceButton(i);
                if (button == null)
                {
                    continue;
                }

                var text = button.GetComponentInChildren<Text>();
                if (text != null && text.text.Contains(value))
                {
                    return text;
                }
            }

            return null;
        }

        private static bool ContainsSelectableMapNode(PrototypeFloorMapNodeView[] mapNodes)
        {
            for (var i = 0; i < mapNodes.Length; i++)
            {
                if (mapNodes[i].Selectable)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsLockedMapNode(PrototypeFloorMapNodeView[] mapNodes)
        {
            for (var i = 0; i < mapNodes.Length; i++)
            {
                if (mapNodes[i].Locked)
                {
                    return true;
                }
            }

            return false;
        }

        private static void DeleteRequiredScreenshots()
        {
            for (var i = 0; i < RequiredScreenshots.Length; i++)
            {
                var path = Path.Combine(ScreenshotDirectory, RequiredScreenshots[i]);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static void AssertRequiredScreenshots()
        {
            for (var i = 0; i < RequiredScreenshots.Length; i++)
            {
                var path = Path.Combine(ScreenshotDirectory, RequiredScreenshots[i]);
                Assert.IsTrue(File.Exists(path), "Missing required screenshot: " + path);
                AssertPngSize(path);
            }
        }

        private static IEnumerator WaitForFrames(int frameCount)
        {
            for (var i = 0; i < frameCount; i++)
            {
                yield return null;
            }
        }

        private static IEnumerator WaitForStablePortfolioFrame(PrototypeHud hud)
        {
            for (var i = 0; i < 45; i++)
            {
                Canvas.ForceUpdateCanvases();
                if (hud == null || (!hud.CombatIntroOverlayVisible && !hud.PreRunPlaceholderVisible))
                {
                    break;
                }

                yield return null;
            }

            yield return WaitForFrames(4);
        }

        private static readonly string[] RequiredScreenshots =
        {
            "01_lobby.png",
            "02_floor_map.png",
            "03_event_jar_room.png",
            "04_rest_mataios.png",
            "05_shop.png",
            "09_shop_floor3.png",
            "10_shop_floor4.png",
            "11_shop_floor5.png",
            "06_normal_combat.png",
            "07_boss_combat.png",
            "12_boss_gate_choices.png",
            "08_ending_choice.png"
        };
    }
}
