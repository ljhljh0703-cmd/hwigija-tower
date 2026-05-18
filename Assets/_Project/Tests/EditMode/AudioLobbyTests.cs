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
            Assert.AreEqual("저장된 진행 없음", controller.ContinueDisabledReason);
            Assert.IsTrue(System.IO.File.Exists("Assets/_Project/Scenes/Lobby.unity"));
            Assert.IsTrue(System.IO.File.Exists("Assets/_Project/Data/Audio/SO_AudioCueCatalog.asset"));
            Assert.IsTrue(System.IO.File.Exists("Assets/_Project/Data/Presentation/SO_LobbyPresentationData.asset"));
            Assert.AreEqual("Assets/_Project/Scenes/Lobby.unity", EditorBuildSettings.scenes[0].path);
            Assert.AreEqual("Assets/_Project/Scenes/PrototypeRoom.unity", EditorBuildSettings.scenes[1].path);
            Object.DestroyImmediate(controller.gameObject);
        }

        [Test]
        public void AudioCueCatalog_BindsCoreBgmAndSfxClips()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<AudioCueCatalog>("Assets/_Project/Data/Audio/SO_AudioCueCatalog.asset");

            Assert.IsNotNull(catalog);
            AssertCue(catalog, PrototypeAudioContext.Lobby, PrototypeAudioChannel.Bgm, "bgm_lobby_main_loop");
            AssertCue(catalog, PrototypeAudioContext.Combat, PrototypeAudioChannel.Bgm, "bgm_combat_normal_loop");
            AssertCue(catalog, PrototypeAudioContext.Rest, PrototypeAudioChannel.Bgm, "bgm_rest_companion_loop");
            AssertCue(catalog, PrototypeAudioContext.UiTap, PrototypeAudioChannel.Sfx, "sfx_ui_tap");
            AssertCue(catalog, PrototypeAudioContext.UiConfirm, PrototypeAudioChannel.Sfx, "sfx_ui_confirm");
            AssertCue(catalog, PrototypeAudioContext.CombatAttack, PrototypeAudioChannel.Sfx, "sfx_combat_attack");
            AssertCue(catalog, PrototypeAudioContext.CombatDefend, PrototypeAudioChannel.Sfx, "sfx_combat_defend");
            AssertCue(catalog, PrototypeAudioContext.CombatHit, PrototypeAudioChannel.Sfx, "sfx_combat_enemy_hit");
            AssertCue(catalog, PrototypeAudioContext.CombatVictory, PrototypeAudioChannel.Sfx, "sfx_combat_victory");
            AssertCue(catalog, PrototypeAudioContext.ShopPurchase, PrototypeAudioChannel.Sfx, "sfx_shop_purchase");
            AssertCue(catalog, PrototypeAudioContext.RestSubmit, PrototypeAudioChannel.Sfx, "sfx_rest_submit");
        }

        [Test]
        public void PlayerAndAudioAssets_UseExpectedImportSettings()
        {
            AssertSpriteImportSettings("Assets/_Project/Art/Characters/char_player_portrait_01.png");
            AssertSpriteImportSettings("Assets/_Project/Art/Characters/char_player_bust_01.png");
            AssertSpriteImportSettings("Assets/_Project/Art/Characters/char_mataios_portrait_01.png");

            AssertAudioLoadType("Assets/_Project/Audio/Music/Lobby/bgm_lobby_main_loop.ogg", AudioClipLoadType.Streaming);
            AssertAudioLoadType("Assets/_Project/Audio/Music/Combat/bgm_combat_normal_loop.ogg", AudioClipLoadType.Streaming);
            AssertAudioLoadType("Assets/_Project/Audio/Music/Rest/bgm_rest_companion_loop.ogg", AudioClipLoadType.Streaming);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/UI/sfx_ui_tap.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/UI/sfx_ui_confirm.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/Combat/sfx_combat_attack.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/Combat/sfx_combat_defend.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/Combat/sfx_combat_enemy_hit.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/Combat/sfx_combat_victory.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/Shop/sfx_shop_purchase.wav", AudioClipLoadType.DecompressOnLoad);
            AssertAudioLoadType("Assets/_Project/Audio/SFX/Rest/sfx_rest_submit.wav", AudioClipLoadType.DecompressOnLoad);
        }

        [Test]
        public void LobbyPresentationData_ExistsAndHandlesMissingBackground()
        {
            var data = AssetDatabase.LoadAssetAtPath<LobbyPresentationData>("Assets/_Project/Data/Presentation/SO_LobbyPresentationData.asset");

            Assert.IsNotNull(data);
            Assert.IsFalse(string.IsNullOrWhiteSpace(data.TitleText));
            Assert.IsFalse(string.IsNullOrWhiteSpace(data.SubtitleText));
            Assert.IsFalse(string.IsNullOrWhiteSpace(data.DefaultProfileName));
            Assert.IsNotNull(data.LogoSprite);
            Assert.AreEqual("lobby_logo_temp", data.LogoSprite.name);
        }

        private static void AssertCue(AudioCueCatalog catalog, PrototypeAudioContext context, PrototypeAudioChannel channel, string clipName)
        {
            Assert.IsTrue(catalog.TryGetCue(context, out var cue), "Missing cue for " + context);
            Assert.IsNotNull(cue);
            Assert.AreEqual(channel, cue.Channel);
            Assert.IsTrue(cue.HasClip, "Missing clip for " + context);
            Assert.AreEqual(clipName, cue.Clip.name);
        }

        private static void AssertAudioLoadType(string path, AudioClipLoadType loadType)
        {
            var importer = AssetImporter.GetAtPath(path) as AudioImporter;
            Assert.IsNotNull(importer, path);
            Assert.AreEqual(loadType, importer.defaultSampleSettings.loadType, path);
        }

        private static void AssertSpriteImportSettings(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            Assert.IsNotNull(importer, path);
            Assert.AreEqual(TextureImporterType.Sprite, importer.textureType, path);
            Assert.AreEqual(SpriteImportMode.Single, importer.spriteImportMode, path);
            Assert.IsTrue(importer.alphaIsTransparency, path);
            Assert.IsFalse(importer.mipmapEnabled, path);
        }
    }
}
