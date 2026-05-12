using System.Collections;
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
            var continueButton = GameObject.Find("Lobby Continue Button").GetComponent<Button>();
            Assert.IsNotNull(continueButton);
            Assert.IsFalse(continueButton.interactable);
            Assert.IsTrue(continueButton.GetComponentInChildren<Text>().text.Contains(controller.ContinueDisabledReason));
        }

        [UnityTest]
        public IEnumerator Lobby_SettingsPanelOpensAndCloses()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
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
        public IEnumerator Lobby_ProfilePanelOpensAndCloses()
        {
            yield return SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
            yield return null;

            var controller = Object.FindFirstObjectByType<LobbyController>();
            Assert.IsNotNull(controller);
            Assert.IsFalse(controller.ProfilePanelVisible);

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
            Assert.IsNotNull(Object.FindFirstObjectByType<PrototypeRoomController>());
        }
    }
}
