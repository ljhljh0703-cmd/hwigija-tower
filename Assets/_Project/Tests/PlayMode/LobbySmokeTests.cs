using System.Collections;
using System.Reflection;
using HwigiTower.Audio;
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
        public IEnumerator Lobby_LoadsV11LayoutAndNoSaveVariant()
        {
            yield return LoadLobby();

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            Assert.IsNotNull(GameObject.Find("Lobby Canvas"));
            Assert.IsNotNull(GameObject.Find("Lobby Layout Revision v1.1"));
            Assert.IsNotNull(GameObject.Find("Lobby Run Status"));
            Assert.IsNull(GameObject.Find("Lobby Profile Chip"));
            Assert.IsNotNull(GameObject.Find("Lobby Record Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Settings Button"));
            Assert.IsNotNull(GameObject.Find("Lobby Quit Button"));

            AssertSlot("Lobby Run Status", LobbyLayout.RunStatus);
            AssertSlot("Lobby Background", LobbyLayout.TowerArt);
            AssertSlot("Lobby Title", LobbyLayout.TitleMark);
            AssertSlot("Lobby Tagline", LobbyLayout.Tagline);
            AssertSlot("Lobby New Game Button", LobbyLayout.PrimaryAction);
            AssertSlot("Lobby Continue Button", LobbyLayout.SecondaryAction);
            AssertSlot("Lobby Utility Row", LobbyLayout.UtilityRow);
            AssertSlot("Lobby Build Stamp", LobbyLayout.BuildStamp);

            var continueButton = GameObject.Find("Lobby Continue Button").GetComponent<Button>();
            Assert.IsFalse(continueButton.interactable);
            Assert.AreEqual("이어서 오른다 · " + controller.ContinueDisabledReason, continueButton.GetComponentInChildren<Text>().text);
            var noSaveStatus = GameObject.Find("Lobby Run Status").GetComponent<Text>().text;
            StringAssert.Contains("저장 없음", noSaveStatus);
            StringAssert.DoesNotContain("HP", noSaveStatus);
            AssertFramedButton("Lobby New Game Button");
            AssertFramedButton("Lobby Continue Button");
            AssertL01AndL03AtRuntime();
            AssertL04AndL08AtRuntime();

            var canvas = GameObject.Find("Lobby Canvas");
            Assert.AreEqual(RenderMode.ScreenSpaceOverlay, canvas.GetComponent<Canvas>().renderMode);
            Assert.IsNotNull(canvas.GetComponent<GraphicRaycaster>());
            var scaler = canvas.GetComponent<CanvasScaler>();
            Assert.AreEqual(CanvasScaler.ScaleMode.ScaleWithScreenSize, scaler.uiScaleMode);
            Assert.AreEqual(new Vector2(LobbyLayout.ReferenceWidth, LobbyLayout.ReferenceHeight), scaler.referenceResolution);
            Assert.AreEqual(1f, scaler.matchWidthOrHeight);
            Assert.IsNotNull(PrototypeAudioService.Instance);
            Assert.AreEqual(PrototypeAudioContext.Lobby, PrototypeAudioService.Instance.LastPlayedBgmContext);
        }

        [UnityTest]
        public IEnumerator Lobby_RebuildsTheV11RuntimeUiIfCanvasIsMissing()
        {
            yield return LoadLobby();

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            var canvas = GameObject.Find("Lobby Canvas");
            Assert.IsNotNull(canvas);

            Object.DestroyImmediate(canvas);
            ForceUiBuiltFlag(controller, true);
            yield return null;
            yield return null;

            Assert.IsNotNull(GameObject.Find("Lobby Canvas"));
            Assert.IsNotNull(GameObject.Find("Lobby Layout Revision v1.1"));
            AssertSlot("Lobby Title", LobbyLayout.TitleMark);
            AssertSlot("Lobby New Game Button", LobbyLayout.PrimaryAction);
            AssertSlot("Lobby Continue Button", LobbyLayout.SecondaryAction);
        }

        [UnityTest]
        public IEnumerator Lobby_SettingsPanelOpensAndClosesFromTheUtilityRow()
        {
            yield return LoadLobby();

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsFalse(controller.SettingsPanelVisible);
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
        public IEnumerator Lobby_RecordPanelOpensAndClosesFromTheUtilityRow()
        {
            yield return LoadLobby();

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsFalse(controller.RecordPanelVisible);
            GameObject.Find("Lobby Record Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsTrue(controller.RecordPanelVisible);
            Assert.IsNotNull(GameObject.Find("Lobby Record Runs Cleared"));
            Assert.IsNotNull(GameObject.Find("Lobby Record Memories"));

            GameObject.Find("Lobby Record Close Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            Assert.IsFalse(controller.RecordPanelVisible);
        }

        [UnityTest]
        public IEnumerator Lobby_NewGameLoadsPrototypeRoom()
        {
            yield return LoadLobby();

            GameObject.Find("Lobby New Game Button").GetComponent<Button>().onClick.Invoke();
            yield return null;
            yield return null;

            Assert.AreEqual("PrototypeRoom", SceneManager.GetActiveScene().name);
            Assert.IsNotNull(Object.FindFirstObjectByType<PrototypeRoomController>());
            Assert.IsNotNull(PrototypeAudioService.Instance);
            Assert.IsTrue(PrototypeRunSaveStore.HasSave());
        }

        [UnityTest]
        public IEnumerator Lobby_ContinueUsesSavedRunAndKeepsHpOutOfLobbyStatus()
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

            yield return LoadLobby();

            var continueButton = GameObject.Find("Lobby Continue Button").GetComponent<Button>();
            Assert.IsTrue(continueButton.interactable);
            Assert.AreEqual("이어서 오른다", continueButton.GetComponentInChildren<Text>().text);
            var status = GameObject.Find("Lobby Run Status").GetComponent<Text>().text;
            StringAssert.Contains("저장됨", status);
            StringAssert.Contains("최고 도달 층 2", status);
            StringAssert.Contains("기억 조각 1", status);
            StringAssert.DoesNotContain("HP", status);

            continueButton.onClick.Invoke();
            yield return null;
            yield return null;

            Assert.AreEqual("PrototypeRoom", SceneManager.GetActiveScene().name);
            var room = Object.FindFirstObjectByType<PrototypeRoomController>();
            Assert.IsNotNull(room);
            Assert.AreEqual("run-playmode-continue", room.RunState.RunId);
            Assert.AreEqual(2, room.RunState.CurrentFloor);
        }

        private static IEnumerator LoadLobby()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;
            yield return null;
            Canvas.ForceUpdateCanvases();
        }

        private static void AssertSlot(string name, LobbySlot expected)
        {
            var target = GameObject.Find(name);
            Assert.IsNotNull(target, "Missing lobby slot: " + name);
            var rect = target.GetComponent<RectTransform>();
            Assert.IsNotNull(rect, "Missing RectTransform: " + name);
            var anchorMin = LobbyLayoutRuntime.AnchorMin(expected);
            var anchorMax = LobbyLayoutRuntime.AnchorMax(expected);
            Assert.AreEqual(anchorMin.x, rect.anchorMin.x, 0.0001f, name + " anchorMin.x");
            Assert.AreEqual(anchorMin.y, rect.anchorMin.y, 0.0001f, name + " anchorMin.y");
            Assert.AreEqual(anchorMax.x, rect.anchorMax.x, 0.0001f, name + " anchorMax.x");
            Assert.AreEqual(anchorMax.y, rect.anchorMax.y, 0.0001f, name + " anchorMax.y");
        }

        private static void AssertFramedButton(string name)
        {
            var button = GameObject.Find(name);
            Assert.IsNotNull(button.GetComponent<Button>(), name + " is not interactive");
            Assert.IsNotNull(button.GetComponent<Outline>(), name + " is missing its frame outline");
            Assert.GreaterOrEqual(button.GetComponentsInChildren<Image>(true).Length, 5, name + " is missing frame corners");
        }

        private static void AssertL01AndL03AtRuntime()
        {
            AssertActionSlotDoesNotCoverTower("Lobby New Game Button");
            AssertActionSlotDoesNotCoverTower("Lobby Continue Button");
            AssertActionSlotDoesNotCoverTower("Lobby Utility Row");

            var actionNames = new[] { "Lobby New Game Button", "Lobby Continue Button", "Lobby Utility Row" };
            for (var i = 0; i < actionNames.Length; i++)
            {
                var rect = GameObject.Find(actionNames[i]).GetComponent<RectTransform>();
                var topOriginY = 1f - rect.anchorMax.y;
                Assert.GreaterOrEqual(topOriginY, 0.66f, actionNames[i] + " violates L-01");
            }
        }

        private static void AssertActionSlotDoesNotCoverTower(string name)
        {
            var rect = GameObject.Find(name).GetComponent<RectTransform>();
            var xMin = rect.anchorMin.x;
            var xMax = rect.anchorMax.x;
            var yMin = 1f - rect.anchorMax.y;
            var yMax = 1f - rect.anchorMin.y;
            var overlaps = xMin < LobbyLayout.TowerProtectedXMax &&
                xMax > LobbyLayout.TowerProtectedXMin &&
                yMin < LobbyLayout.TowerProtectedYMax &&
                yMax > LobbyLayout.TowerProtectedYMin;
            Assert.IsFalse(overlaps, name + " violates L-03");
        }

        private static void AssertL04AndL08AtRuntime()
        {
            var canvas = GameObject.Find("Lobby Canvas");
            var texts = canvas.GetComponentsInChildren<Text>(true);
            for (var i = 0; i < texts.Length; i++)
            {
                StringAssert.DoesNotContain("Prototype", texts[i].text, "L-04: " + texts[i].name);
            }

            Assert.IsNull(GameObject.Find("Lobby Title").GetComponent<Image>(), "L-08: title must not have a panel image");
            Assert.IsNull(GameObject.Find("Lobby Tagline").GetComponent<Image>(), "L-08: tagline must not have a panel image");
        }

        private static void ForceUiBuiltFlag(LobbyController controller, bool value)
        {
            var field = typeof(LobbyController).GetField("_uiBuilt", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(controller, value);
        }
    }
}
