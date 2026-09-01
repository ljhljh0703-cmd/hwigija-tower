using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using HwigiTower.Lobby;
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
        private static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static string ScreenshotDirectory => Path.Combine(ProjectRoot, "Docs", "Portfolio", "assets", "concept-to-ui", "runtime-captures");
        private static string RuntimeActualDirectory => Path.Combine(ProjectRoot, "Docs", "Portfolio", "assets", "concept-to-ui");

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
            Directory.CreateDirectory(RuntimeActualDirectory);
            DeleteRequiredScreenshots();
            Screen.SetResolution(ScreenshotWidth, ScreenshotHeight, false);
            yield return WaitForFrames(8);

            yield return CaptureLobbyNoSave();
            yield return CaptureLobbyWithSave();
            ResetRunStateIsolation();
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
            }, "floormap");
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

        private static IEnumerator CaptureLobbyNoSave()
        {
            PrototypeRunSaveStore.Delete();
            PrototypeRunSaveRequest.RequestNewGame();
            yield return CaptureLobby("01_lobby_no_save.png", false);
        }

        private static IEnumerator CaptureLobbyWithSave()
        {
            PrototypeRunSaveStore.Save(new PrototypeRunSaveData
            {
                runId = "run-portrait-lobby-save",
                currentFloor = 4,
                playerHp = 19,
                playerMaxHp = 24,
                memoryFragmentRefs = new[] { "MEM_FRAGMENT_01", "MEM_FRAGMENT_03" }
            });
            yield return CaptureLobby("01_lobby_with_save.png", true);
        }

        private static IEnumerator CaptureLobby(string fileName, bool expectsSave)
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return WaitForFrames(10);

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            if (!expectsSave)
            {
                PrototypeRunSaveStore.Delete();
                PrototypeRunSaveRequest.RequestNewGame();
                Assert.IsFalse(PrototypeRunSaveStore.HasSave(), "No-save screenshot fixture observed a persisted run at " + PrototypeRunSaveStore.DefaultPath);
                controller.ShowContinuePlaceholder();
                yield return null;
            }

            Assert.IsNotNull(GameObject.Find("Lobby Canvas"));
            Assert.IsNotNull(GameObject.Find("Lobby Title"));
            Assert.IsNotNull(GameObject.Find("Lobby New Game Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Utility Row"));
            var continueButton = GameObject.Find("Lobby Continue Button").GetComponent<Button>();
            Assert.AreEqual(expectsSave, continueButton.interactable);
            if (expectsSave)
            {
                StringAssert.Contains("최고 도달 층 4", GameObject.Find("Lobby Run Status").GetComponent<Text>().text);
            }
            else
            {
                StringAssert.Contains("저장된 진행 없음", continueButton.GetComponentInChildren<Text>().text);
            }

            WriteLobbyRuntimeActual();
            yield return CaptureAndAssert(fileName);
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

                Assert.IsTrue(hud.ShopUiVisible);
                Assert.IsFalse(hud.NpcSpotlightVisible);
                Assert.AreEqual("enc_shop_01_bg", hud.ShopSceneSpriteName);
                Assert.AreEqual(6, hud.ChoiceButtonCount, "Expected five offer rows plus leave.");
                StringAssert.Contains("붕대", hud.ShopOfferText);
                Assert.IsEmpty(hud.CurrentShopChoiceIconNames);
            }, "shop");
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

                Assert.AreEqual(expectedBackground, hud.CurrentBackgroundSpriteName);
                Assert.IsTrue(hud.ShopUiVisible);
                Assert.IsFalse(hud.NpcSpotlightVisible);
                Assert.AreEqual(expectedBackground, hud.ShopSceneSpriteName);
                Assert.AreEqual(6, hud.ChoiceButtonCount);
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
            }, "rest");
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
            }, encounterId == "ENC_COMBAT_GATE_01" ? "combat" : null);
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

        private static IEnumerator CapturePrototypeRoomScreen(string fileName, System.Action<PrototypeRoomController, PrototypeHud> arrange, string runtimeLayoutScreen = null)
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
            if (!string.IsNullOrEmpty(runtimeLayoutScreen))
            {
                WritePrototypeRuntimeActual(runtimeLayoutScreen);
            }
            yield return CaptureAndAssert(fileName);
        }

        private static void WriteLobbyRuntimeActual()
        {
            WriteRuntimeActual("lobby", RequireRectTransform("Lobby Portrait Safe Area"), new[]
            {
                new RuntimeSlotRequest("runStatus", "Lobby Run Status"),
                new RuntimeSlotRequest("towerArt", "Lobby Background"),
                new RuntimeSlotRequest("titleMark", "Lobby Title"),
                new RuntimeSlotRequest("tagline", "Lobby Tagline"),
                new RuntimeSlotRequest("primaryAction", "Lobby New Game Button"),
                new RuntimeSlotRequest("secondaryAction", "Lobby Continue Button"),
                new RuntimeSlotRequest("utilityRow", "Lobby Utility Row"),
                new RuntimeSlotRequest("buildStamp", "Lobby Build Stamp")
            });
        }

        private static void WritePrototypeRuntimeActual(string screen)
        {
            var root = RequireRectTransform("PortraitRoot");
            if (screen == "rest")
            {
                WriteRuntimeActual(screen, root, new[]
                {
                    new RuntimeSlotRequest("floorChip", "Rest floorChip"),
                    new RuntimeSlotRequest("sanityChip", "Rest sanityChip"),
                    new RuntimeSlotRequest("hpChip", "Rest hpChip"),
                    new RuntimeSlotRequest("goldChip", "Rest goldChip"),
                    new RuntimeSlotRequest("screenTitle", "Rest Screen Title"),
                    new RuntimeSlotRequest("restScene", "Rest Scene"),
                    new RuntimeSlotRequest("mataiosState", "Rest Mataios State"),
                    new RuntimeSlotRequest("mataiosStateLabel", "Rest Mataios State Label"),
                    new RuntimeSlotRequest("playerState", "Rest Player State"),
                    new RuntimeSlotRequest("playerHpBar", "Rest Player HP Bar"),
                    new RuntimeSlotRequest("choiceTalk", "Rest Choice Talk"),
                    new RuntimeSlotRequest("choiceTrain", "Rest Choice Train"),
                    new RuntimeSlotRequest("choiceSleep", "Rest Choice Sleep"),
                    new RuntimeSlotRequest("departButton", "Rest Depart Button")
                });
                return;
            }

            if (screen == "combat")
            {
                WriteRuntimeActual(screen, root, new[]
                {
                    new RuntimeSlotRequest("floorChip", "Combat floorChip"),
                    new RuntimeSlotRequest("sanityChip", "Combat sanityChip"),
                    new RuntimeSlotRequest("hpChip", "Combat hpChip"),
                    new RuntimeSlotRequest("goldChip", "Combat goldChip"),
                    new RuntimeSlotRequest("threatReadout", "Combat Threat Readout"),
                    new RuntimeSlotRequest("enemyPanel", "Combat Enemy Stage"),
                    new RuntimeSlotRequest("enemyTitle", "Combat Enemy Title"),
                    new RuntimeSlotRequest("enemyHpBar", "Enemy HP Bar Frame"),
                    new RuntimeSlotRequest("enemyStatusChips", "Combat Enemy Status Chips"),
                    new RuntimeSlotRequest("enemyImage", "Combat Enemy Image"),
                    new RuntimeSlotRequest("damageEffectOverlay", "Combat Enemy Damage Number"),
                    new RuntimeSlotRequest("playerCard", "Combat Player Card"),
                    new RuntimeSlotRequest("playerHpBar", "Player HP Bar Frame"),
                    new RuntimeSlotRequest("playerSanityBar", "Player Sanity Bar Frame"),
                    new RuntimeSlotRequest("mataiosCard", "Combat Mataios Card"),
                    new RuntimeSlotRequest("mataiosHpBar", "Mataios HP Bar Frame"),
                    new RuntimeSlotRequest("mataiosStateLabel", "Combat Mataios State Label"),
                    new RuntimeSlotRequest("attackButton", "Combat Button Attack"),
                    new RuntimeSlotRequest("defendButton", "Combat Button Defend"),
                    new RuntimeSlotRequest("skillButton", "Combat Button Skill"),
                    new RuntimeSlotRequest("combatLog", "Combat Log Panel"),
                    new RuntimeSlotRequest("itemInspectButton", "Combat Item Inspect Button")
                });
                return;
            }

            if (screen == "floormap")
            {
                WriteRuntimeActual(screen, root, new[]
                {
                    new RuntimeSlotRequest("floorChip", "FloorMap floorChip"),
                    new RuntimeSlotRequest("sanityChip", "FloorMap sanityChip"),
                    new RuntimeSlotRequest("hpChip", "FloorMap hpChip"),
                    new RuntimeSlotRequest("goldChip", "FloorMap goldChip"),
                    new RuntimeSlotRequest("screenTitle", "FloorMap Screen Title"),
                    new RuntimeSlotRequest("nodeGraph", "FloorMap Node Graph"),
                    new RuntimeSlotRequest("nodeLegend", "FloorMap Node Legend"),
                    new RuntimeSlotRequest("selectedNodeCard", "FloorMap Selected Card"),
                    new RuntimeSlotRequest("departButton", "FloorMap Depart Button"),
                    new RuntimeSlotRequest("backButton", "FloorMap Back Button")
                });
                return;
            }

            if (screen == "shop")
            {
                WriteRuntimeActual(screen, root, new[]
                {
                    new RuntimeSlotRequest("floorChip", "Shop floorChip"),
                    new RuntimeSlotRequest("sanityChip", "Shop sanityChip"),
                    new RuntimeSlotRequest("hpChip", "Shop hpChip"),
                    new RuntimeSlotRequest("goldChip", "Shop goldChip"),
                    new RuntimeSlotRequest("screenTitle", "Shop Screen Title"),
                    new RuntimeSlotRequest("shopScene", "Shop Scene"),
                    new RuntimeSlotRequest("ownedStrip", "Shop Owned Strip"),
                    new RuntimeSlotRequest("offerRow1", "Shop Offer Row 1"),
                    new RuntimeSlotRequest("offerRow2", "Shop Offer Row 2"),
                    new RuntimeSlotRequest("offerRow3", "Shop Offer Row 3"),
                    new RuntimeSlotRequest("offerRow4", "Shop Offer Row 4"),
                    new RuntimeSlotRequest("offerRow5", "Shop Offer Row 5"),
                    new RuntimeSlotRequest("leaveButton", "Shop Leave Button")
                });
                return;
            }

            Assert.Fail("Unsupported runtime layout screen: " + screen);
        }

        private static void WriteRuntimeActual(string screen, RectTransform reference, RuntimeSlotRequest[] requests)
        {
            Canvas.ForceUpdateCanvases();
            var referenceCorners = new Vector3[4];
            reference.GetWorldCorners(referenceCorners);
            var referenceWidth = referenceCorners[2].x - referenceCorners[0].x;
            var referenceHeight = referenceCorners[1].y - referenceCorners[0].y;
            Assert.Greater(referenceWidth, 0f, "Runtime layout reference has no width: " + reference.name);
            Assert.Greater(referenceHeight, 0f, "Runtime layout reference has no height: " + reference.name);

            var builder = new StringBuilder();
            builder.Append("{\n");
            builder.Append("  \"screen\": \"").Append(JsonEscape(screen)).Append("\",\n");
            builder.Append("  \"capturedAt\": \"").Append(System.DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)).Append(" runtime RectTransform capture\",\n");
            builder.Append("  \"source\": \"runtime\",\n");
            builder.Append("  \"slots\": {");
            var wroteSlot = false;
            for (var i = 0; i < requests.Length; i++)
            {
                var request = requests[i];
                var rect = FindRuntimeRectTransform(request.ObjectName, reference);
                if (rect == null)
                {
                    continue;
                }

                var corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                var x = (corners[0].x - referenceCorners[0].x) / referenceWidth;
                var y = (referenceCorners[1].y - corners[1].y) / referenceHeight;
                var w = (corners[2].x - corners[0].x) / referenceWidth;
                var h = (corners[1].y - corners[0].y) / referenceHeight;
                var text = rect.GetComponent<Text>() ?? rect.GetComponentInChildren<Text>(true);

                if (wroteSlot)
                {
                    builder.Append(',');
                }

                builder.Append("\n    \"").Append(JsonEscape(request.Key)).Append("\": {");
                builder.Append("\"x\": ").Append(FormatFloat(x));
                builder.Append(", \"y\": ").Append(FormatFloat(y));
                builder.Append(", \"w\": ").Append(FormatFloat(w));
                builder.Append(", \"h\": ").Append(FormatFloat(h));
                if (text != null)
                {
                    builder.Append(", \"fontSize\": ").Append(text.fontSize.ToString(CultureInfo.InvariantCulture));
                }
                builder.Append(", \"visible\": ").Append(rect.gameObject.activeInHierarchy ? "true" : "false");
                builder.Append('}');
                wroteSlot = true;
            }

            if (wroteSlot)
            {
                builder.Append('\n');
            }
            builder.Append("  }\n}");
            File.WriteAllText(Path.Combine(RuntimeActualDirectory, screen + "_runtime_actual_layout.json"), builder.ToString());
        }

        private static RectTransform RequireRectTransform(string objectName)
        {
            var target = GameObject.Find(objectName);
            Assert.IsNotNull(target, "Missing runtime layout target: " + objectName);
            var rect = target.GetComponent<RectTransform>();
            Assert.IsNotNull(rect, "Runtime layout target has no RectTransform: " + objectName);
            return rect;
        }

        private static RectTransform FindRuntimeRectTransform(string objectName, RectTransform reference)
        {
            var transforms = Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            RectTransform inactiveMatch = null;
            for (var i = 0; i < transforms.Length; i++)
            {
                var rect = transforms[i];
                if (rect == null || rect.name != objectName || !rect.IsChildOf(reference))
                {
                    continue;
                }

                if (rect.gameObject.activeInHierarchy)
                {
                    return rect;
                }

                inactiveMatch ??= rect;
            }

            return inactiveMatch;
        }

        private static string FormatFloat(float value)
        {
            return value.ToString("0.######", CultureInfo.InvariantCulture);
        }

        private static string JsonEscape(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private sealed class RuntimeSlotRequest
        {
            public RuntimeSlotRequest(string key, string objectName)
            {
                Key = key;
                ObjectName = objectName;
            }

            public string Key { get; }
            public string ObjectName { get; }
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
            "01_lobby_no_save.png",
            "01_lobby_with_save.png",
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
