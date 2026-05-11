using HwigiTower.Audio;
using HwigiTower.Lobby;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HwigiTower.Tests.EditMode
{
    public sealed class AudioLobbyTests
    {
        [Test]
        public void AudioCueCatalog_MissingClipsAndMissingCuesAreSafe()
        {
            var catalog = ScriptableObject.CreateInstance<AudioCueCatalog>();
            var serviceObject = new GameObject("Audio Service Test");
            var service = serviceObject.AddComponent<PrototypeAudioService>();

            service.Configure(catalog);
            service.PlayContext(PrototypeAudioContext.Lobby);

            Assert.AreEqual(PrototypeAudioContext.Lobby, service.LastContext);
            Object.DestroyImmediate(serviceObject);
            Object.DestroyImmediate(catalog);
        }

        [Test]
        public void AudioService_ClampsVolumeSettings()
        {
            var serviceObject = new GameObject("Audio Service Clamp Test");
            var service = serviceObject.AddComponent<PrototypeAudioService>();

            service.SetBgmVolume(3f);
            service.SetSfxVolume(-2f);
            service.SetAmbienceVolume(0.4f);

            Assert.AreEqual(1f, service.BgmVolume);
            Assert.AreEqual(0f, service.SfxVolume);
            Assert.AreEqual(0.4f, service.AmbienceVolume);
            Object.DestroyImmediate(serviceObject);
        }

        [Test]
        public void Lobby_NewGameTargetSceneIsConfigured()
        {
            var controller = new GameObject("Lobby Controller Test").AddComponent<LobbyController>();

            Assert.AreEqual("PrototypeRoom", controller.NewGameSceneName);
            Assert.IsTrue(System.IO.File.Exists("Assets/_Project/Scenes/Lobby.unity"));
            Assert.IsTrue(System.IO.File.Exists("Assets/_Project/Data/Audio/SO_AudioCueCatalog.asset"));
            Assert.AreEqual("Assets/_Project/Scenes/Lobby.unity", EditorBuildSettings.scenes[0].path);
            Assert.AreEqual("Assets/_Project/Scenes/PrototypeRoom.unity", EditorBuildSettings.scenes[1].path);
            Object.DestroyImmediate(controller.gameObject);
        }
    }
}
