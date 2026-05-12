using HwigiTower.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HwigiTower.Lobby
{
    public sealed class LobbyController : MonoBehaviour
    {
        [SerializeField] private string newGameSceneName = "PrototypeRoom";
        [SerializeField] private AudioCueCatalog audioCueCatalog;
        [SerializeField] private LobbyPresentationData presentationData;

        private GameObject _settingsPanel;
        private GameObject _profilePanel;
        private Text _statusText;
        private Button _continueButton;

        public string NewGameSceneName => string.IsNullOrEmpty(newGameSceneName) ? "PrototypeRoom" : newGameSceneName;
        public bool SettingsPanelVisible => _settingsPanel != null && _settingsPanel.activeSelf;
        public bool ProfilePanelVisible => _profilePanel != null && _profilePanel.activeSelf;
        public string ContinueDisabledReason => "저장된 진행 없음";

        private void Awake()
        {
            PrototypeAudioService.GetOrCreate().Configure(audioCueCatalog);
            PrototypeAudioService.GetOrCreate().PlayContext(PrototypeAudioContext.Lobby);
            BuildUi();
        }

        public void StartNewGame()
        {
            SceneManager.LoadScene(NewGameSceneName);
        }

        public void ShowContinuePlaceholder()
        {
            if (_statusText != null)
            {
                _statusText.text = ContinueDisabledReason;
            }
        }

        public void OpenProfile()
        {
            if (_profilePanel != null)
            {
                _profilePanel.SetActive(true);
            }
        }

        public void CloseProfile()
        {
            if (_profilePanel != null)
            {
                _profilePanel.SetActive(false);
            }
        }

        public void OpenSettings()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(true);
            }
        }

        public void CloseSettings()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }
        }

        public void QuitOrShowPlaceholder()
        {
            if (_statusText != null)
            {
                _statusText.text = "종료 준비 중";
            }

            Application.Quit();
        }

        private void BuildUi()
        {
            var canvasObject = new GameObject("Lobby Canvas");
            canvasObject.transform.SetParent(transform, false);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvasObject.AddComponent<GraphicRaycaster>();

            BuildBackground(canvasObject.transform);
            BuildProfileCard(canvasObject.transform);
            BuildHero(canvasObject.transform);

            _statusText = CreateText(canvasObject.transform, "Lobby Status", string.Empty, 30, new Vector2(0.08f, 0.24f), new Vector2(0.56f, 0.29f));
            _statusText.alignment = TextAnchor.MiddleLeft;

            CreateButton(canvasObject.transform, "Lobby New Game Button", "새 게임", new Vector2(0.08f, 0.38f), new Vector2(0.56f, 0.46f), StartNewGame);
            _continueButton = CreateButton(canvasObject.transform, "Lobby Continue Button", "이어 하기", new Vector2(0.08f, 0.29f), new Vector2(0.56f, 0.37f), ShowContinuePlaceholder);
            _continueButton.interactable = false;
            SetButtonDisabledText(_continueButton, ContinueDisabledReason);
            CreateButton(canvasObject.transform, "Lobby Profile Button", "프로필", new Vector2(0.08f, 0.20f), new Vector2(0.56f, 0.28f), OpenProfile);
            CreateButton(canvasObject.transform, "Lobby Settings Button", "설정", new Vector2(0.08f, 0.11f), new Vector2(0.56f, 0.19f), OpenSettings);
            if (ShouldShowQuitButton())
            {
                CreateButton(canvasObject.transform, "Lobby Quit Button", "종료", new Vector2(0.62f, 0.11f), new Vector2(0.92f, 0.19f), QuitOrShowPlaceholder);
            }

            BuildProfilePanel(canvasObject.transform);
            BuildSettingsPanel(canvasObject.transform);
        }

        private void BuildBackground(Transform parent)
        {
            var backgroundObject = new GameObject("Lobby Background");
            backgroundObject.transform.SetParent(parent, false);
            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = backgroundObject.AddComponent<Image>();
            if (presentationData != null && presentationData.BackgroundSprite != null)
            {
                image.sprite = presentationData.BackgroundSprite;
                image.preserveAspect = true;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(0.025f, 0.032f, 0.042f, 1f);
            }

            var overlayObject = new GameObject("Lobby Background Readability Overlay");
            overlayObject.transform.SetParent(parent, false);
            var overlayRect = overlayObject.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            var overlay = overlayObject.AddComponent<Image>();
            overlay.color = new Color(0.0f, 0.0f, 0.0f, presentationData != null && presentationData.BackgroundSprite != null ? 0.36f : 0.0f);
            overlay.raycastTarget = false;
        }

        private void BuildProfileCard(Transform parent)
        {
            var card = CreatePanel(parent, "Lobby Profile Card", new Vector2(0.06f, 0.84f), new Vector2(0.46f, 0.94f), new Color(0.04f, 0.055f, 0.07f, 0.86f));
            var icon = CreatePanel(card.transform, "Lobby Profile Icon Placeholder", new Vector2(0.04f, 0.18f), new Vector2(0.22f, 0.82f), new Color(0.17f, 0.20f, 0.22f, 1f));
            CreateText(icon.transform, "Lobby Profile Icon Text", "P", 36, Vector2.zero, Vector2.one);
            var name = presentationData != null ? presentationData.DefaultProfileName : "Player";
            var profileName = CreateText(card.transform, "Lobby Profile Name", name, 28, new Vector2(0.27f, 0.46f), new Vector2(0.96f, 0.82f));
            profileName.alignment = TextAnchor.MiddleLeft;
            var status = CreateText(card.transform, "Lobby Profile Status", "Prototype", 22, new Vector2(0.27f, 0.16f), new Vector2(0.96f, 0.48f));
            status.alignment = TextAnchor.MiddleLeft;
        }

        private void BuildHero(Transform parent)
        {
            if (presentationData != null && presentationData.LogoSprite != null)
            {
                var logoObject = new GameObject("Lobby Logo");
                logoObject.transform.SetParent(parent, false);
                var logoRect = logoObject.AddComponent<RectTransform>();
                logoRect.anchorMin = new Vector2(0.16f, 0.64f);
                logoRect.anchorMax = new Vector2(0.84f, 0.78f);
                logoRect.offsetMin = Vector2.zero;
                logoRect.offsetMax = Vector2.zero;
                var logo = logoObject.AddComponent<Image>();
                logo.sprite = presentationData.LogoSprite;
                logo.preserveAspect = true;
                logo.raycastTarget = false;
            }

            var titleValue = presentationData != null ? presentationData.TitleText : "회귀자는 탑을 오른다";
            var title = CreateText(parent, "Lobby Title", titleValue, 64, new Vector2(0.08f, 0.66f), new Vector2(0.92f, 0.76f));
            title.color = new Color(0.94f, 0.97f, 0.92f, 1f);
            var subtitleValue = presentationData != null ? presentationData.SubtitleText : "Prototype";
            var subtitle = CreateText(parent, "Lobby Subtitle", subtitleValue, 30, new Vector2(0.14f, 0.61f), new Vector2(0.86f, 0.66f));
            subtitle.color = new Color(0.74f, 0.82f, 0.84f, 1f);
        }

        private void BuildSettingsPanel(Transform parent)
        {
            _settingsPanel = new GameObject("Lobby Settings Panel");
            _settingsPanel.transform.SetParent(parent, false);
            var rect = _settingsPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.08f, 0.24f);
            rect.anchorMax = new Vector2(0.92f, 0.70f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = _settingsPanel.AddComponent<Image>();
            image.color = new Color(0.035f, 0.045f, 0.055f, 0.96f);

            CreateText(_settingsPanel.transform, "Lobby Settings Title", "설정", 42, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.92f));
            CreateSlider(_settingsPanel.transform, "Lobby BGM Volume Slider", "BGM", 0.70f, new Vector2(0.12f, 0.60f), new Vector2(0.88f, 0.70f), value => PrototypeAudioService.GetOrCreate().SetBgmVolume(value));
            CreateSlider(_settingsPanel.transform, "Lobby SFX Volume Slider", "SFX", 0.85f, new Vector2(0.12f, 0.44f), new Vector2(0.88f, 0.54f), value => PrototypeAudioService.GetOrCreate().SetSfxVolume(value));
            CreateSlider(_settingsPanel.transform, "Lobby Text Speed Slider", "Text Speed", 0.60f, new Vector2(0.12f, 0.28f), new Vector2(0.88f, 0.38f), _ => { });
            CreateButton(_settingsPanel.transform, "Lobby Settings Close Button", "닫기", new Vector2(0.30f, 0.08f), new Vector2(0.70f, 0.18f), CloseSettings);
            _settingsPanel.SetActive(false);
        }

        private void BuildProfilePanel(Transform parent)
        {
            _profilePanel = new GameObject("Lobby Profile Panel");
            _profilePanel.transform.SetParent(parent, false);
            var rect = _profilePanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.08f, 0.28f);
            rect.anchorMax = new Vector2(0.92f, 0.68f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = _profilePanel.AddComponent<Image>();
            image.color = new Color(0.035f, 0.045f, 0.055f, 0.96f);

            var name = presentationData != null ? presentationData.DefaultProfileName : "Player";
            CreateText(_profilePanel.transform, "Lobby Profile Panel Title", "프로필", 42, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.92f));
            CreateText(_profilePanel.transform, "Lobby Profile Panel Name", name, 32, new Vector2(0.10f, 0.58f), new Vector2(0.90f, 0.70f));
            CreateText(_profilePanel.transform, "Lobby Profile Runs Cleared", "클리어: 준비 중", 28, new Vector2(0.10f, 0.42f), new Vector2(0.90f, 0.54f));
            CreateText(_profilePanel.transform, "Lobby Profile Memories", "기억: 준비 중", 28, new Vector2(0.10f, 0.28f), new Vector2(0.90f, 0.40f));
            CreateButton(_profilePanel.transform, "Lobby Profile Close Button", "닫기", new Vector2(0.30f, 0.08f), new Vector2(0.70f, 0.20f), CloseProfile);
            _profilePanel.SetActive(false);
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);
            var rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = panelObject.AddComponent<Image>();
            image.color = color;
            return panelObject;
        }

        private static Text CreateText(Transform parent, string name, string value, int fontSize, Vector2 anchorMin, Vector2 anchorMax)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = textObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = fontSize;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(18, fontSize - 10);
            text.resizeTextMaxSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = new Color(0.88f, 0.93f, 0.95f, 1f);
            text.text = value;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.16f, 0.19f, 0.98f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var text = CreateText(buttonObject.transform, name + " Text", label, 34, Vector2.zero, Vector2.one);
            text.raycastTarget = false;
            return button;
        }

        private static void SetButtonDisabledText(Button button, string reason)
        {
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = "이어 하기\n" + reason;
                label.fontSize = 26;
                label.resizeTextMinSize = 18;
                label.resizeTextMaxSize = 26;
            }
        }

        private static void CreateSlider(Transform parent, string name, string label, float value, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction<float> onChanged)
        {
            CreateText(parent, name + " Label", label, 24, new Vector2(anchorMin.x, anchorMax.y), new Vector2(anchorMax.x, anchorMax.y + 0.06f));
            var sliderObject = new GameObject(name);
            sliderObject.transform.SetParent(parent, false);
            var rect = sliderObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var background = sliderObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.10f, 0.12f, 0.96f);
            var slider = sliderObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
            slider.onValueChanged.AddListener(onChanged);
        }

        private static Font ResolveFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private bool ShouldShowQuitButton()
        {
            if (presentationData == null || !presentationData.ShowQuitButtonOnDesktopOnly)
            {
                return true;
            }

            return Application.isEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer;
        }
    }
}
