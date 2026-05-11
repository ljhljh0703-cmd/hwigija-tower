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

        private GameObject _settingsPanel;
        private Text _statusText;
        private Button _continueButton;

        public string NewGameSceneName => string.IsNullOrEmpty(newGameSceneName) ? "PrototypeRoom" : newGameSceneName;
        public bool SettingsPanelVisible => _settingsPanel != null && _settingsPanel.activeSelf;

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
                _statusText.text = "Continue: 준비 중";
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

            var title = CreateText(canvasObject.transform, "Lobby Title", "회귀자는 탑을 오른다", 56, new Vector2(0.08f, 0.72f), new Vector2(0.92f, 0.82f));
            title.color = new Color(0.94f, 0.97f, 0.92f, 1f);
            _statusText = CreateText(canvasObject.transform, "Lobby Status", string.Empty, 28, new Vector2(0.12f, 0.24f), new Vector2(0.88f, 0.30f));

            CreateButton(canvasObject.transform, "Lobby New Game Button", "New Game", new Vector2(0.22f, 0.52f), new Vector2(0.78f, 0.60f), StartNewGame);
            _continueButton = CreateButton(canvasObject.transform, "Lobby Continue Button", "Continue", new Vector2(0.22f, 0.42f), new Vector2(0.78f, 0.50f), ShowContinuePlaceholder);
            _continueButton.interactable = false;
            CreateButton(canvasObject.transform, "Lobby Settings Button", "Settings", new Vector2(0.22f, 0.32f), new Vector2(0.78f, 0.40f), OpenSettings);

            BuildSettingsPanel(canvasObject.transform);
        }

        private void BuildSettingsPanel(Transform parent)
        {
            _settingsPanel = new GameObject("Lobby Settings Panel");
            _settingsPanel.transform.SetParent(parent, false);
            var rect = _settingsPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.10f, 0.22f);
            rect.anchorMax = new Vector2(0.90f, 0.66f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = _settingsPanel.AddComponent<Image>();
            image.color = new Color(0.035f, 0.045f, 0.055f, 0.96f);

            CreateText(_settingsPanel.transform, "Lobby Settings Title", "Settings", 40, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.92f));
            CreateSlider(_settingsPanel.transform, "Lobby BGM Volume Slider", "BGM", 0.70f, new Vector2(0.12f, 0.60f), new Vector2(0.88f, 0.70f), value => PrototypeAudioService.GetOrCreate().SetBgmVolume(value));
            CreateSlider(_settingsPanel.transform, "Lobby SFX Volume Slider", "SFX", 0.85f, new Vector2(0.12f, 0.44f), new Vector2(0.88f, 0.54f), value => PrototypeAudioService.GetOrCreate().SetSfxVolume(value));
            CreateSlider(_settingsPanel.transform, "Lobby Text Speed Slider", "Text Speed", 0.60f, new Vector2(0.12f, 0.28f), new Vector2(0.88f, 0.38f), _ => { });
            CreateButton(_settingsPanel.transform, "Lobby Settings Close Button", "Close", new Vector2(0.30f, 0.08f), new Vector2(0.70f, 0.18f), CloseSettings);
            _settingsPanel.SetActive(false);
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
    }
}
