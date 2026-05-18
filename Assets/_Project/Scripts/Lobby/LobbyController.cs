using HwigiTower.Audio;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HwigiTower.Lobby
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-10000)]
    public sealed class LobbyController : MonoBehaviour
    {
        [SerializeField] private string newGameSceneName = "PrototypeRoom";
        [SerializeField] private AudioCueCatalog audioCueCatalog;
        [SerializeField] private LobbyPresentationData presentationData;

        private const string LobbyCanvasName = "Lobby Canvas";
        private const string LobbyTitleName = "Lobby Title";
        private const string LobbyNewGameButtonName = "Lobby New Game Button";
        private const string LobbyContinueButtonName = "Lobby Continue Button";
        private const string LobbySettingsButtonName = "Lobby Settings Button";

        private GameObject _settingsPanel;
        private GameObject _profilePanel;
        private Text _statusText;
        private Button _continueButton;
        private RectTransform _safeAreaRoot;
        private bool _uiBuilt;
        private bool _audioStarted;

        public string NewGameSceneName => string.IsNullOrEmpty(newGameSceneName) ? "PrototypeRoom" : newGameSceneName;
        public bool SettingsPanelVisible => _settingsPanel != null && _settingsPanel.activeSelf;
        public bool ProfilePanelVisible => _profilePanel != null && _profilePanel.activeSelf;
        public string ContinueDisabledReason => "저장된 진행 없음";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterLobbySceneBootstrap()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Lobby")
            {
                EnsureLobbySceneRuntimeUi();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureLobbySceneRuntimeUi()
        {
            if (SceneManager.GetActiveScene().name != "Lobby")
            {
                return;
            }

            var controller = FindFirstObjectByType<LobbyController>();
            if (controller == null)
            {
                controller = new GameObject("Lobby Runtime").AddComponent<LobbyController>();
            }

            controller.EnsureRuntimeUi();
        }

        private void Awake()
        {
            EnsureRuntimeUi();
        }

        private void OnEnable()
        {
            EnsureRuntimeUi();
        }

        private void Start()
        {
            EnsureRuntimeUi();
        }

        private void Update()
        {
            if (!IsRuntimeUiPresent())
            {
                EnsureRuntimeUi();
            }
        }

        private void OnGUI()
        {
            if (SceneManager.GetActiveScene().name == "Lobby" && !IsRuntimeUiPresent())
            {
                EnsureRuntimeUi();
            }
        }

        private void OnDisable()
        {
            _uiBuilt = false;
            _safeAreaRoot = null;
            _settingsPanel = null;
            _profilePanel = null;
            _statusText = null;
            _continueButton = null;
            _audioStarted = false;
        }

        private void EnsureRuntimeUi()
        {
            if (!Application.isPlaying && gameObject.scene.name != "Lobby")
            {
                return;
            }

            EnsureLobbyAudio();
            if (IsRuntimeUiPresent())
            {
                NormalizeRuntimeUi();
                BindRuntimeUi();
                _uiBuilt = true;
                return;
            }

            HidePartialRuntimeUi();
            BuildUi();
            _uiBuilt = true;
        }

        private bool IsRuntimeUiPresent()
        {
            return transform.Find(LobbyCanvasName) != null
                && GameObject.Find(LobbyTitleName) != null
                && GameObject.Find(LobbyNewGameButtonName) != null
                && GameObject.Find(LobbyContinueButtonName) != null
                && GameObject.Find(LobbySettingsButtonName) != null;
        }

        private void HidePartialRuntimeUi()
        {
            var existing = transform.Find(LobbyCanvasName);
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
            }

            _safeAreaRoot = null;
            _settingsPanel = null;
            _profilePanel = null;
            _statusText = null;
            _continueButton = null;
            _uiBuilt = false;
        }

        public void StartNewGame()
        {
            PlayUiSfx(PrototypeAudioContext.UiConfirm);
            PrototypeRunSaveRequest.RequestNewGame();
            SceneManager.LoadScene(NewGameSceneName);
        }

        public void ContinueSavedRun()
        {
            if (!PrototypeRunSaveStore.HasSave())
            {
                ShowContinuePlaceholder();
                return;
            }

            PlayUiSfx(PrototypeAudioContext.UiConfirm);
            PrototypeRunSaveRequest.RequestContinue();
            SceneManager.LoadScene(NewGameSceneName);
        }

        public void ShowContinuePlaceholder()
        {
            PlayUiSfx(PrototypeAudioContext.UiDisabled);
            if (_statusText != null)
            {
                _statusText.text = ContinueDisabledReason;
            }
        }

        public void OpenProfile()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_profilePanel != null)
            {
                _profilePanel.SetActive(true);
            }
        }

        public void CloseProfile()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_profilePanel != null)
            {
                _profilePanel.SetActive(false);
            }
        }

        public void OpenSettings()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(true);
            }
        }

        public void CloseSettings()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }
        }

        public void QuitOrShowPlaceholder()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_statusText != null)
            {
                _statusText.text = "종료 준비 중";
            }

            Application.Quit();
        }

        private void PlayUiSfx(PrototypeAudioContext context)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var service = PrototypeAudioService.GetOrCreate();
            service.Configure(audioCueCatalog);
            service.PlayContext(context);
        }

        private void EnsureLobbyAudio()
        {
            if (!Application.isPlaying || _audioStarted)
            {
                return;
            }

            var service = PrototypeAudioService.GetOrCreate();
            service.Configure(audioCueCatalog);
            service.PlayContext(PrototypeAudioContext.Lobby);
            _audioStarted = true;
        }

        private void BuildUi()
        {
            EnsureEventSystem();

            var canvasObject = new GameObject(LobbyCanvasName);
            canvasObject.transform.SetParent(transform, false);
            canvasObject.transform.localScale = Vector3.one;
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvasObject.AddComponent<GraphicRaycaster>();

            CreateSafeAreaRoot(canvasObject.transform);
            BuildBackground(canvasObject.transform, _safeAreaRoot.transform);
            BuildProfileChip(_safeAreaRoot.transform);
            BuildHero(_safeAreaRoot.transform);
            BuildMenuColumn(_safeAreaRoot.transform);
            BuildProfilePanel(_safeAreaRoot.transform);
            BuildSettingsPanel(_safeAreaRoot.transform);
            BindRuntimeUi();
        }

        private void CreateSafeAreaRoot(Transform parent)
        {
            var safeAreaObject = new GameObject("Lobby Portrait Safe Area");
            safeAreaObject.transform.SetParent(parent, false);
            safeAreaObject.transform.localScale = Vector3.one;
            _safeAreaRoot = safeAreaObject.AddComponent<RectTransform>();
            _safeAreaRoot.anchorMin = new Vector2(0.5f, 0.5f);
            _safeAreaRoot.anchorMax = new Vector2(0.5f, 0.5f);
            _safeAreaRoot.pivot = new Vector2(0.5f, 0.5f);
            _safeAreaRoot.sizeDelta = new Vector2(1080f, 1920f);
            _safeAreaRoot.anchoredPosition = Vector2.zero;
        }

        private void BuildMenuColumn(Transform parent)
        {
            _statusText = CreateText(parent, "Lobby Status", string.Empty, 28, new Vector2(0.14f, 0.52f), new Vector2(0.86f, 0.57f));
            _statusText.alignment = TextAnchor.MiddleCenter;

            CreateButton(parent, "Lobby New Game Button", "새 게임", new Vector2(0.14f, 0.43f), new Vector2(0.86f, 0.50f), StartNewGame);
            _continueButton = CreateButton(parent, "Lobby Continue Button", "이어 하기", new Vector2(0.14f, 0.34f), new Vector2(0.86f, 0.42f), ContinueSavedRun);
            ConfigureContinueButton();
            CreateButton(parent, "Lobby Profile Button", "프로필", new Vector2(0.14f, 0.26f), new Vector2(0.86f, 0.33f), OpenProfile);
            CreateButton(parent, "Lobby Settings Button", "설정", new Vector2(0.14f, 0.18f), new Vector2(0.86f, 0.25f), OpenSettings);
            if (ShouldShowQuitButton())
            {
                CreateButton(parent, "Lobby Quit Button", "종료", new Vector2(0.14f, 0.10f), new Vector2(0.86f, 0.17f), QuitOrShowPlaceholder);
            }
        }

        private void ConfigureContinueButton()
        {
            if (_continueButton == null)
            {
                return;
            }

            if (PrototypeRunSaveStore.TryLoadSummary(out var summary))
            {
                _continueButton.interactable = true;
                SetButtonText(_continueButton, "이어 하기\n" + summary.DisplayText, 24);
                if (_statusText != null)
                {
                    _statusText.text = summary.DisplayText;
                }
                return;
            }

            _continueButton.interactable = false;
            SetButtonText(_continueButton, "이어 하기\n" + ContinueDisabledReason, 24);
        }

        private void BuildBackground(Transform canvasParent, Transform portraitParent)
        {
            var gutterObject = new GameObject("Lobby Landscape Gutter");
            gutterObject.transform.SetParent(canvasParent, false);
            var gutterRect = gutterObject.AddComponent<RectTransform>();
            gutterRect.anchorMin = Vector2.zero;
            gutterRect.anchorMax = Vector2.one;
            gutterRect.offsetMin = Vector2.zero;
            gutterRect.offsetMax = Vector2.zero;
            var gutter = gutterObject.AddComponent<Image>();
            gutter.color = new Color(0.012f, 0.016f, 0.022f, 1f);
            gutter.raycastTarget = false;
            gutterObject.transform.SetAsFirstSibling();

            var backgroundObject = new GameObject("Lobby Background");
            backgroundObject.transform.SetParent(portraitParent, false);
            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = backgroundObject.AddComponent<Image>();
            if (presentationData != null && presentationData.BackgroundSprite != null)
            {
                image.sprite = presentationData.BackgroundSprite;
                image.preserveAspect = false;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(0.025f, 0.032f, 0.042f, 1f);
            }
            backgroundObject.transform.SetAsFirstSibling();

            var overlayObject = new GameObject("Lobby Background Readability Overlay");
            overlayObject.transform.SetParent(portraitParent, false);
            var overlayRect = overlayObject.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            var overlay = overlayObject.AddComponent<Image>();
            overlay.color = new Color(0.0f, 0.0f, 0.0f, presentationData != null && presentationData.BackgroundSprite != null ? 0.36f : 0.0f);
            overlay.raycastTarget = false;
            overlayObject.transform.SetSiblingIndex(1);
        }

        private void BuildProfileChip(Transform parent)
        {
            var card = CreatePanel(parent, "Lobby Profile Chip", new Vector2(0.06f, 0.91f), new Vector2(0.42f, 0.965f), new Color(0.04f, 0.055f, 0.07f, 0.70f));
            var icon = CreatePanel(card.transform, "Lobby Profile Icon Placeholder", new Vector2(0.05f, 0.18f), new Vector2(0.18f, 0.82f), new Color(0.17f, 0.20f, 0.22f, 1f));
            CreateText(icon.transform, "Lobby Profile Icon Text", "P", 24, Vector2.zero, Vector2.one);
            var name = presentationData != null ? presentationData.DefaultProfileName : "Player";
            var profileName = CreateText(card.transform, "Lobby Profile Name", name, 22, new Vector2(0.22f, 0.14f), new Vector2(0.96f, 0.86f));
            profileName.alignment = TextAnchor.MiddleLeft;
        }

        private void BuildHero(Transform parent)
        {
            var hasLogo = presentationData != null && presentationData.LogoSprite != null;
            if (hasLogo)
            {
                var logoObject = new GameObject("Lobby Logo");
                logoObject.transform.SetParent(parent, false);
                var logoRect = logoObject.AddComponent<RectTransform>();
                logoRect.anchorMin = new Vector2(0.20f, 0.69f);
                logoRect.anchorMax = new Vector2(0.80f, 0.82f);
                logoRect.offsetMin = Vector2.zero;
                logoRect.offsetMax = Vector2.zero;
                var logo = logoObject.AddComponent<Image>();
                logo.sprite = presentationData.LogoSprite;
                logo.preserveAspect = true;
                logo.raycastTarget = false;
            }

            var titleValue = presentationData != null ? presentationData.TitleText : "회귀자는 탑을 오른다";
            var title = CreateText(parent, LobbyTitleName, titleValue, hasLogo ? 42 : 66, hasLogo ? new Vector2(0.10f, 0.615f) : new Vector2(0.08f, 0.66f), hasLogo ? new Vector2(0.90f, 0.675f) : new Vector2(0.92f, 0.76f));
            title.color = new Color(0.94f, 0.97f, 0.92f, 1f);
            var subtitleValue = presentationData != null ? presentationData.SubtitleText : "Prototype";
            var subtitle = CreateText(parent, "Lobby Subtitle", subtitleValue, 28, hasLogo ? new Vector2(0.14f, 0.57f) : new Vector2(0.14f, 0.61f), hasLogo ? new Vector2(0.86f, 0.605f) : new Vector2(0.86f, 0.65f));
            subtitle.color = new Color(0.74f, 0.82f, 0.84f, 1f);
        }

        private void BuildSettingsPanel(Transform parent)
        {
            _settingsPanel = new GameObject("Lobby Settings Panel");
            _settingsPanel.transform.SetParent(parent, false);
            var rect = _settingsPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.10f, 0.30f);
            rect.anchorMax = new Vector2(0.90f, 0.70f);
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
            rect.anchorMin = new Vector2(0.10f, 0.30f);
            rect.anchorMax = new Vector2(0.90f, 0.68f);
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

        private static void SetButtonText(Button button, string value, int fontSize)
        {
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = value;
                label.fontSize = fontSize;
                label.resizeTextMinSize = 18;
                label.resizeTextMaxSize = fontSize;
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

        private void NormalizeRuntimeUi()
        {
            var canvasTransform = transform.Find(LobbyCanvasName);
            if (canvasTransform == null)
            {
                return;
            }

            canvasTransform.gameObject.SetActive(true);
            canvasTransform.localScale = Vector3.one;
            if (canvasTransform is RectTransform canvasRect)
            {
                canvasRect.anchorMin = Vector2.zero;
                canvasRect.anchorMax = Vector2.zero;
                canvasRect.anchoredPosition = Vector2.zero;
                canvasRect.sizeDelta = Vector2.zero;
            }

            var canvas = canvasTransform.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = true;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            var scaler = canvasTransform.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080f, 1920f);
                scaler.matchWidthOrHeight = 1f;
            }

            if (canvasTransform.GetComponent<GraphicRaycaster>() == null)
            {
                canvasTransform.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void BindRuntimeUi()
        {
            _safeAreaRoot = FindChildComponent<RectTransform>("Lobby Portrait Safe Area");
            _settingsPanel = FindChildGameObject("Lobby Settings Panel");
            _profilePanel = FindChildGameObject("Lobby Profile Panel");
            _statusText = FindChildComponent<Text>("Lobby Status");
            _continueButton = FindChildComponent<Button>(LobbyContinueButtonName);

            BindButton(LobbyNewGameButtonName, StartNewGame);
            BindButton(LobbyContinueButtonName, ContinueSavedRun);
            BindButton("Lobby Profile Button", OpenProfile);
            BindButton("Lobby Settings Button", OpenSettings);
            BindButton("Lobby Quit Button", QuitOrShowPlaceholder);
            BindButton("Lobby Profile Close Button", CloseProfile);
            BindButton("Lobby Settings Close Button", CloseSettings);

            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }

            if (_profilePanel != null)
            {
                _profilePanel.SetActive(false);
            }

            ConfigureContinueButton();
        }

        private void BindButton(string name, UnityEngine.Events.UnityAction action)
        {
            var button = FindChildComponent<Button>(name);
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        private GameObject FindChildGameObject(string targetName)
        {
            var child = FindChildTransform(transform, targetName);
            return child != null ? child.gameObject : null;
        }

        private T FindChildComponent<T>(string targetName) where T : Component
        {
            var child = FindChildTransform(transform, targetName);
            return child != null ? child.GetComponent<T>() : null;
        }

        private static Transform FindChildTransform(Transform root, string targetName)
        {
            if (root.name == targetName)
            {
                return root;
            }

            for (var i = 0; i < root.childCount; i++)
            {
                var match = FindChildTransform(root.GetChild(i), targetName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static void EnsureEventSystem()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
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
