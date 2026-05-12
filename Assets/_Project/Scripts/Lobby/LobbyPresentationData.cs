using UnityEngine;

namespace HwigiTower.Lobby
{
    [CreateAssetMenu(menuName = "Hwigi Tower/Lobby/Presentation Data", fileName = "SO_LobbyPresentationData")]
    public sealed class LobbyPresentationData : ScriptableObject
    {
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite logoSprite;
        [SerializeField] private string titleText = "회귀자는 탑을 오른다";
        [SerializeField] private string subtitleText = "Prototype";
        [SerializeField] private string defaultProfileName = "Player";
        [SerializeField] private bool showQuitButtonOnDesktopOnly = true;

        public Sprite BackgroundSprite => backgroundSprite;
        public Sprite LogoSprite => logoSprite;
        public string TitleText => string.IsNullOrWhiteSpace(titleText) ? "회귀자는 탑을 오른다" : titleText;
        public string SubtitleText => string.IsNullOrWhiteSpace(subtitleText) ? "Prototype" : subtitleText;
        public string DefaultProfileName => string.IsNullOrWhiteSpace(defaultProfileName) ? "Player" : defaultProfileName;
        public bool ShowQuitButtonOnDesktopOnly => showQuitButtonOnDesktopOnly;
    }
}
