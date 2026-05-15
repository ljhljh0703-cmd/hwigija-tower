using System.Collections;
using System.Reflection;
using HwigiTower.Lobby;
using HwigiTower.Run;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace HwigiTower.Tests.PlayMode
{
    public sealed class LobbySmokeTests
    {
        [SetUp]
        public void SetUp()
        {
            PrototypeRunSaveStore.Delete();
            PrototypeRunSaveRequest.RequestNewGame();
        }

        [TearDown]
        public void TearDown()
        {
            PrototypeRunSaveStore.Delete();
            PrototypeRunSaveRequest.RequestNewGame();
        }

        [UnityTest]
        public IEnumerator Lobby_LoadsAndShowsSafeButtons()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(GameObject.Find("Lobby New Game Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Profile Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Title"));
            Assert.IsNotNull(GameObject.Find("Lobby Subtitle"));
            Assert.IsNotNull(GameObject.Find("Lobby Portrait Safe Area"));
            Assert.IsNotNull(GameObject.Find("Lobby Profile Chip"));
            Assert.IsNull(GameObject.Find("Lobby Profile Card"));
            var canvas = GameObject.Find("Lobby Canvas");
            Assert.IsNotNull(canvas);
            Assert.IsTrue(canvas.activeInHierarchy);
            Assert.AreEqual(RenderMode.ScreenSpaceOverlay, canvas.GetComponent<Canvas>().renderMode);
            Assert.IsNotNull(canvas.GetComponent<GraphicRaycaster>());
            var continueButton = GameObject.Find("Lobby Continue Button").GetComponent<Button>();
            Assert.IsNotNull(continueButton);
            Assert.IsFalse(continueButton.interactable);
            Assert.IsTrue(continueButton.GetComponentInChildren<Text>().text.Contains(controller.ContinueDisabledReason));
            AssertMainMenuButtonsSharePortraitColumn();

            var canvasScaler = canvas.GetComponent<CanvasScaler>();
            Assert.AreEqual(CanvasScaler.ScaleMode.ScaleWithScreenSize, canvasScaler.uiScaleMode);
            Assert.AreEqual(new Vector2(1080f, 1920f), canvasScaler.referenceResolution);
            Assert.AreEqual(1f, canvasScaler.matchWidthOrHeight);
        }

        [UnityTest]
        public IEnumerator Lobby_RebuildsRuntimeUiIfCanvasIsMissing()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            var canvas = GameObject.Find("Lobby Canvas");
            Assert.IsNotNull(canvas);

            Object.DestroyImmediate(canvas);
            ForceUiBuiltFlag(controller, true);
            yield return null;

            var rebuiltCanvas = GameObject.Find("Lobby Canvas");
            Assert.IsNotNull(rebuiltCanvas);
            Assert.IsTrue(rebuiltCanvas.activeInHierarchy);
            Assert.IsNotNull(GameObject.Find("Lobby Title"));
            Assert.IsNotNull(GameObject.Find("Lobby New Game Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Continue Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Settings Button"));
        }

        [UnityTest]
        public IEnumerator Lobby_SettingsPanelOpensAndCloses()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            Assert.IsFalse(controller.SettingsPanelVisible);
            Assert.IsNull(GameObject.Find("Lobby BGM Volume Slider"));
            Assert.IsNull(GameObject.Find("Lobby SFX Volume Slider"));
            Assert.IsNull(GameObject.Find("Lobby Text Speed Slider"));

            GameObject.Find("Lobby Settings Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsTrue(controller.SettingsPanelVisible);
            Assert.IsNotNull(GameObject.Find("Lobby BGM Volume Slider").GetComponent<Slider>());
            Assert.IsNotNull(GameObject.Find("Lobby SFX Volume Slider").GetComponent<Slider>());
            Assert.IsNotNull(GameObject.Find("Lobby Text Speed Slider").GetComponent<Slider>());

            GameObject.Find("Lobby Settings Close Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsFalse(controller.SettingsPanelVisible);
        }

        [UnityTest]
        public IEnumerator Lobby_ProfilePanelOpensAndCloses()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            Assert.IsFalse(controller.ProfilePanelVisible);
            Assert.IsNull(GameObject.Find("Lobby Profile Runs Cleared"));
            Assert.IsNull(GameObject.Find("Lobby Profile Memories"));

            GameObject.Find("Lobby Profile Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsTrue(controller.ProfilePanelVisible);
            Assert.IsNotNull(GameObject.Find("Lobby Profile Runs Cleared"));
            Assert.IsNotNull(GameObject.Find("Lobby Profile Memories"));

            GameObject.Find("Lobby Profile Close Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsFalse(controller.ProfilePanelVisible);
        }

        [UnityTest]
        public IEnumerator Lobby_NewGameLoadsPrototypeRoom()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            GameObject.Find("Lobby New Game Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            yield return null;

            Assert.AreEqual("PrototypeRoom", SceneManager.GetActiveScene().name);
            var controller = Object.FindFirstObjectByType<PrototypeRoomController>();
            Assert.IsNotNull(controller);
            Assert.IsTrue(PrototypeRunSaveStore.HasSave());
        }

        [UnityTest]
        public IEnumerator Lobby_ContinueLoadsSavedPrototypeRoom()
        {
            PrototypeRunSaveStore.Save(new PrototypeRunSaveData
            {
                runId = "run-playmode-continue",
                currentFloor = 2,
                playerHp = 19,
                playerMaxHp = 24,
                gold = 7,
                memoryFragmentRefs = new[] { "MEM_FRAGMENT_01" }
            });

            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var continueButton = GameObject.Find("Lobby Continue Button").GetComponent<Button>();
            Assert.IsTrue(continueButton.interactable);
            StringAssert.Contains("Floor 2", continueButton.GetComponentInChildren<Text>().text);

            continueButton.onClick.Invoke();
            yield return null;
            yield return null;

            Assert.AreEqual("PrototypeRoom", SceneManager.GetActiveScene().name);
            var room = Object.FindFirstObjectByType<PrototypeRoomController>();
            Assert.IsNotNull(room);
            Assert.AreEqual("run-playmode-continue", room.RunState.RunId);
            Assert.AreEqual(2, room.RunState.CurrentFloor);
        }

        private static void AssertMainMenuButtonsSharePortraitColumn()
        {
            var names = new[]
            {
                "Lobby New Game Button",
                "Lobby Continue Button",
                "Lobby Profile Button",
                "Lobby Settings Button",
                "Lobby Quit Button"
            };

            foreach (var name in names)
            {
                var button = GameObject.Find(name);
                if (button == null)
                {
                    continue;
                }

                var rect = button.GetComponent<RectTransform>();
                Assert.AreEqual(0.14f, rect.anchorMin.x, 0.001f, name);
                Assert.AreEqual(0.86f, rect.anchorMax.x, 0.001f, name);
            }
        }

        private static void ForceUiBuiltFlag(LobbyController controller, bool value)
        {
            var field = typeof(LobbyController).GetField("_uiBuilt", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(controller, value);
        }
    }
}
