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
        private enum LobbyTextStyle
        {
            Title,
            Primary,
            Body,
            Label,
            Micro
        }

        [SerializeField] private string newGameSceneName = "PrototypeRoom";
        [SerializeField] private AudioCueCatalog audioCueCatalog;
        [SerializeField] private LobbyPresentationData presentationData;

        private const string LobbyCanvasName = "Lobby Canvas";
        private const string LobbyTitleName = "Lobby Title";
        private const string LobbyNewGameButtonName = "Lobby New Game Button";
        private const string LobbyContinueButtonName = "Lobby Continue Button";
        private const string LobbyRecordButtonName = "Lobby Record Button";
        private const string LobbySettingsButtonName = "Lobby Settings Button";
        private const string LobbyQuitButtonName = "Lobby Quit Button";
        private const string LobbyLayoutRevisionName = "Lobby Layout Revision v1.1";

        private GameObject _settingsPanel;
        private GameObject _recordPanel;
        private Text _statusText;
        private Button _continueButton;
        private RectTransform _safeAreaRoot;
        private bool _uiBuilt;
        private bool _audioStarted;

        public string NewGameSceneName => string.IsNullOrEmpty(newGameSceneName) ? "PrototypeRoom" : newGameSceneName;
        public bool SettingsPanelVisible => _settingsPanel != null && _settingsPanel.activeSelf;
        public bool RecordPanelVisible => _recordPanel != null && _recordPanel.activeSelf;
        public bool ProfilePanelVisible => RecordPanelVisible;
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
            _recordPanel = null;
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
            return transform.Find(LobbyCanvasName) != null &&
                GameObject.Find(LobbyTitleName) != null &&
                GameObject.Find(LobbyNewGameButtonName) != null &&
                GameObject.Find(LobbyContinueButtonName) != null &&
                GameObject.Find(LobbyRecordButtonName) != null &&
                GameObject.Find(LobbySettingsButtonName) != null &&
                GameObject.Find(LobbyQuitButtonName) != null &&
                FindChildTransform(transform, LobbyLayoutRevisionName) != null;
        }

        private void HidePartialRuntimeUi()
        {
            var existing = transform.Find(LobbyCanvasName);
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
                existing.name = "Lobby Obsolete Runtime UI";
            }

            _safeAreaRoot = null;
            _settingsPanel = null;
            _recordPanel = null;
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
            ConfigureContinueButton();
        }

        public void OpenRecord()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_recordPanel != null)
            {
                _recordPanel.SetActive(true);
            }
        }

        public void CloseRecord()
        {
            PlayUiSfx(PrototypeAudioContext.UiTap);
            if (_recordPanel != null)
            {
                _recordPanel.SetActive(false);
            }
        }

        public void OpenProfile()
        {
            OpenRecord();
        }

        public void CloseProfile()
        {
            CloseRecord();
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
            scaler.referenceResolution = new Vector2(LobbyLayout.ReferenceWidth, LobbyLayout.ReferenceHeight);
            scaler.matchWidthOrHeight = 1f;
            canvasObject.AddComponent<GraphicRaycaster>();

            var revision = new GameObject(LobbyLayoutRevisionName);
            revision.transform.SetParent(canvasObject.transform, false);

            CreateSafeAreaRoot(canvasObject.transform);
            BuildBackground(canvasObject.transform, _safeAreaRoot.transform);
            BuildRunStatus(_safeAreaRoot.transform);
            BuildHero(_safeAreaRoot.transform);
            BuildActionLayer(_safeAreaRoot.transform);
            BuildBuildStamp(_safeAreaRoot.transform);
            BuildRecordPanel(_safeAreaRoot.transform);
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
            _safeAreaRoot.sizeDelta = new Vector2(LobbyLayout.ReferenceWidth, LobbyLayout.ReferenceHeight);
            _safeAreaRoot.anchoredPosition = Vector2.zero;
        }

        private void BuildBackground(Transform canvasParent, Transform portraitParent)
        {
            var gutterObject = new GameObject("Lobby Landscape Gutter");
            gutterObject.transform.SetParent(canvasParent, false);
            StretchToParent(gutterObject.AddComponent<RectTransform>());
            var gutter = gutterObject.AddComponent<Image>();
            gutter.color = LobbyUiTokens.VoidBg;
            gutter.raycastTarget = false;
            gutterObject.transform.SetAsFirstSibling();

            var backgroundObject = new GameObject("Lobby Background");
            backgroundObject.transform.SetParent(portraitParent, false);
            var backgroundRect = backgroundObject.AddComponent<RectTransform>();
            ApplySlot(backgroundRect, LobbyLayout.TowerArt);
            var background = backgroundObject.AddComponent<Image>();
            background.raycastTarget = false;
            if (presentationData != null && presentationData.BackgroundSprite != null)
            {
                background.sprite = presentationData.BackgroundSprite;
                background.preserveAspect = false;
            }
            else
            {
                background.color = LobbyUiTokens.VoidBg;
            }

            backgroundObject.transform.SetAsFirstSibling();

            var overlayObject = new GameObject("Lobby Background Readability Overlay");
            overlayObject.transform.SetParent(portraitParent, false);
            var overlayRect = overlayObject.AddComponent<RectTransform>();
            ApplySlot(overlayRect, LobbyLayout.TowerArt);
            var overlay = overlayObject.AddComponent<Image>();
            overlay.color = LobbyUiTokens.WithAlpha(LobbyUiTokens.VoidBg, LobbyUiTokens.VignetteStrength);
            overlay.raycastTarget = false;
            overlayObject.transform.SetSiblingIndex(1);
        }

        private void BuildRunStatus(Transform parent)
        {
            _statusText = CreateSlotText(parent, "Lobby Run Status", LobbyLayout.RunStatus, LobbyTextStyle.Label, LobbyUiTokens.InkDim);
            _statusText.alignment = TextAnchor.MiddleCenter;
        }

        private void BuildHero(Transform parent)
        {
            var title = CreateSlotText(parent, LobbyTitleName, LobbyLayout.TitleMark, LobbyTextStyle.Title, LobbyUiTokens.Ink);
            title.fontStyle = FontStyle.Bold;
            AddTextShadow(title, LobbyUiTokens.WithAlpha(LobbyUiTokens.VoidBg, LobbyUiTokens.VignetteStrength));

            var tagline = CreateSlotText(parent, "Lobby Tagline", LobbyLayout.Tagline, LobbyTextStyle.Label, LobbyUiTokens.InkDim);
            AddTextShadow(tagline, LobbyUiTokens.WithAlpha(LobbyUiTokens.VoidBg, LobbyUiTokens.VignetteStrength));
        }

        private void BuildActionLayer(Transform parent)
        {
            CreateFramedButton(parent, LobbyNewGameButtonName, LobbyLayout.PrimaryAction, true, StartNewGame);
            _continueButton = CreateFramedButton(parent, LobbyContinueButtonName, LobbyLayout.SecondaryAction, false, ContinueSavedRun);

            var utilityRow = new GameObject("Lobby Utility Row");
            utilityRow.transform.SetParent(parent, false);
            ApplySlot(utilityRow.AddComponent<RectTransform>(), LobbyLayout.UtilityRow);
            CreateUtilityButton(utilityRow.transform, LobbyRecordButtonName, "기록", 0, OpenRecord);
            CreateUtilityButton(utilityRow.transform, LobbySettingsButtonName, "설정", 1, OpenSettings);
            CreateUtilityButton(utilityRow.transform, LobbyQuitButtonName, "종료", 2, QuitOrShowPlaceholder);
        }

        private void BuildBuildStamp(Transform parent)
        {
            var stamp = CreateSlotText(parent, "Lobby Build Stamp", LobbyLayout.BuildStamp, LobbyTextStyle.Micro, LobbyUiTokens.InkMute);
            stamp.text = Application.version + " · " + LobbyBuildInfo.SourceRevision;
        }

        private void ConfigureContinueButton()
        {
            if (_continueButton == null)
            {
                return;
            }

            if (PrototypeRunSaveStore.TryLoadSummary(out var summary))
            {
                SetButtonState(_continueButton, true, false);
                SetButtonText(_continueButton, LobbyLayout.SecondaryAction.Content, LobbyTextStyle.Body);
                if (_statusText != null)
                {
                    _statusText.text = "저장됨 · 최고 도달 층 " + summary.CurrentFloor + " · 기억 조각 " + summary.MemoryFragmentCount;
                }

                return;
            }

            SetButtonState(_continueButton, false, false);
            SetButtonText(_continueButton, "이어서 오른다 · " + ContinueDisabledReason, LobbyTextStyle.Label);
            if (_statusText != null)
            {
                _statusText.text = "저장 없음 · 최고 도달 층 - · 기억 조각 0";
            }
        }

        private void BuildSettingsPanel(Transform parent)
        {
            _settingsPanel = CreateModalPanel(parent, "Lobby Settings Panel", 0.10f, 0.30f, 0.90f, 0.70f);
            CreateLocalText(_settingsPanel.transform, "Lobby Settings Title", "설정", LobbyTextStyle.Primary, LobbyUiTokens.Ink, 0.08f, 0.78f, 0.92f, 0.92f);
            CreateSlider(_settingsPanel.transform, "Lobby BGM Volume Slider", "BGM", 0.70f, 0.12f, 0.60f, 0.88f, 0.70f, value => PrototypeAudioService.GetOrCreate().SetBgmVolume(value));
            CreateSlider(_settingsPanel.transform, "Lobby SFX Volume Slider", "SFX", 0.85f, 0.12f, 0.44f, 0.88f, 0.54f, value => PrototypeAudioService.GetOrCreate().SetSfxVolume(value));
            CreateSlider(_settingsPanel.transform, "Lobby Text Speed Slider", "Text Speed", 0.60f, 0.12f, 0.28f, 0.88f, 0.38f, _ => { });
            CreatePanelButton(_settingsPanel.transform, "Lobby Settings Close Button", "닫기", 0.30f, 0.08f, 0.70f, 0.18f, CloseSettings);
            _settingsPanel.SetActive(false);
        }

        private void BuildRecordPanel(Transform parent)
        {
            _recordPanel = CreateModalPanel(parent, "Lobby Record Panel", 0.10f, 0.30f, 0.90f, 0.68f);
            var name = presentationData != null ? presentationData.DefaultProfileName : "Player";
            CreateLocalText(_recordPanel.transform, "Lobby Record Panel Title", "기록", LobbyTextStyle.Primary, LobbyUiTokens.Ink, 0.08f, 0.78f, 0.92f, 0.92f);
            CreateLocalText(_recordPanel.transform, "Lobby Record Player", name, LobbyTextStyle.Body, LobbyUiTokens.Ink, 0.10f, 0.58f, 0.90f, 0.70f);
            CreateLocalText(_recordPanel.transform, "Lobby Record Runs Cleared", "클리어 기록: 준비 중", LobbyTextStyle.Label, LobbyUiTokens.InkDim, 0.10f, 0.42f, 0.90f, 0.54f);
            CreateLocalText(_recordPanel.transform, "Lobby Record Memories", "기억 기록: 준비 중", LobbyTextStyle.Label, LobbyUiTokens.InkDim, 0.10f, 0.28f, 0.90f, 0.40f);
            CreatePanelButton(_recordPanel.transform, "Lobby Record Close Button", "닫기", 0.30f, 0.08f, 0.70f, 0.20f, CloseRecord);
            _recordPanel.SetActive(false);
        }

        private static GameObject CreateModalPanel(Transform parent, string name, float left, float bottom, float right, float top)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);
            var rect = panelObject.AddComponent<RectTransform>();
            ApplyAnchors(rect, left, bottom, right, top);
            var image = panelObject.AddComponent<Image>();
            image.color = LobbyUiTokens.PanelBg;
            var outline = panelObject.AddComponent<Outline>();
            outline.effectColor = LobbyUiTokens.FrameHi;
            outline.effectDistance = new Vector2(LobbyUiTokens.FrameBorderWidth, -LobbyUiTokens.FrameBorderWidth);
            return panelObject;
        }

        private static Text CreateSlotText(Transform parent, string name, LobbySlot slot, LobbyTextStyle style, Color color)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            ApplySlot(rect, slot);
            return ConfigureText(textObject.AddComponent<Text>(), slot.Content, style, color);
        }

        private static Text CreateLocalText(Transform parent, string name, string value, LobbyTextStyle style, Color color, float left, float bottom, float right, float top)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            ApplyAnchors(rect, left, bottom, right, top);
            return ConfigureText(textObject.AddComponent<Text>(), value, style, color);
        }

        private static Text ConfigureText(Text text, string value, LobbyTextStyle style, Color color)
        {
            text.font = LobbyUiTokens.ResolveRuntimeFont();
            text.fontSize = ResolveFontSize(style);
            text.lineSpacing = ResolveLineSpacing(style);
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = value;
            return text;
        }

        private static Button CreateFramedButton(Transform parent, string name, LobbySlot slot, bool primary, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = primary ? LobbyUiTokens.PanelBgAlt : LobbyUiTokens.PanelBg;
            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = primary ? LobbyUiTokens.Gold : LobbyUiTokens.FrameHi;
            outline.effectDistance = new Vector2(LobbyUiTokens.FrameBorderWidth, -LobbyUiTokens.FrameBorderWidth);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(action);

            CreateFrameCorner(buttonObject.transform, "Top Left", new Vector2(0f, 1f), new Vector2(0f, 1f), primary ? LobbyUiTokens.Gold : LobbyUiTokens.FrameHi);
            CreateFrameCorner(buttonObject.transform, "Top Right", new Vector2(1f, 1f), new Vector2(1f, 1f), primary ? LobbyUiTokens.Gold : LobbyUiTokens.FrameHi);
            CreateFrameCorner(buttonObject.transform, "Bottom Left", new Vector2(0f, 0f), new Vector2(0f, 0f), primary ? LobbyUiTokens.Gold : LobbyUiTokens.FrameHi);
            CreateFrameCorner(buttonObject.transform, "Bottom Right", new Vector2(1f, 0f), new Vector2(1f, 0f), primary ? LobbyUiTokens.Gold : LobbyUiTokens.FrameHi);

            var text = CreateLocalText(buttonObject.transform, name + " Text", slot.Content, primary ? LobbyTextStyle.Primary : LobbyTextStyle.Body, LobbyUiTokens.Ink, 0f, 0f, 1f, 1f);
            text.raycastTarget = false;
            return button;
        }

        private static void CreateFrameCorner(Transform parent, string name, Vector2 anchor, Vector2 pivot, Color color)
        {
            var cornerObject = new GameObject("Lobby Frame Corner " + name);
            cornerObject.transform.SetParent(parent, false);
            var rect = cornerObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.sizeDelta = new Vector2(LobbyUiTokens.CornerSize, LobbyUiTokens.CornerSize);
            var image = cornerObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
        }

        private static void CreateUtilityButton(Transform parent, string name, string label, int index, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            var width = 1f / 3f;
            rect.anchorMin = new Vector2(index * width, 0f);
            rect.anchorMax = new Vector2((index + 1) * width, 1f);
            rect.offsetMin = new Vector2(index == 0 ? 0f : LobbyUiTokens.SpacingUnit * 0.5f, 0f);
            rect.offsetMax = new Vector2(index == 2 ? 0f : -LobbyUiTokens.SpacingUnit * 0.5f, 0f);
            var image = buttonObject.AddComponent<Image>();
            image.color = LobbyUiTokens.PanelBg;
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(action);
            var text = CreateLocalText(buttonObject.transform, name + " Text", label, LobbyTextStyle.Micro, LobbyUiTokens.InkDim, 0f, 0f, 1f, 1f);
            text.raycastTarget = false;
        }

        private static Button CreatePanelButton(Transform parent, string name, string label, float left, float bottom, float right, float top, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            ApplyAnchors(rect, left, bottom, right, top);
            var image = buttonObject.AddComponent<Image>();
            image.color = LobbyUiTokens.PanelBgAlt;
            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = LobbyUiTokens.Frame;
            outline.effectDistance = new Vector2(LobbyUiTokens.FrameBorderWidth, -LobbyUiTokens.FrameBorderWidth);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(action);
            var text = CreateLocalText(buttonObject.transform, name + " Text", label, LobbyTextStyle.Label, LobbyUiTokens.Ink, 0f, 0f, 1f, 1f);
            text.raycastTarget = false;
            return button;
        }

        private static void CreateSlider(Transform parent, string name, string label, float value, float left, float bottom, float right, float top, UnityEngine.Events.UnityAction<float> onChanged)
        {
            CreateLocalText(parent, name + " Label", label, LobbyTextStyle.Micro, LobbyUiTokens.InkDim, left, top, right, top + 0.06f);
            var sliderObject = new GameObject(name);
            sliderObject.transform.SetParent(parent, false);
            var rect = sliderObject.AddComponent<RectTransform>();
            ApplyAnchors(rect, left, bottom, right, top);
            var background = sliderObject.AddComponent<Image>();
            background.color = LobbyUiTokens.PanelBgAlt;
            var slider = sliderObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
            slider.onValueChanged.AddListener(onChanged);
        }

        private static void SetButtonText(Button button, string value, LobbyTextStyle style)
        {
            var label = button.GetComponentInChildren<Text>();
            if (label == null)
            {
                return;
            }

            label.text = value;
            label.fontSize = ResolveFontSize(style);
            label.lineSpacing = ResolveLineSpacing(style);
        }

        private static void SetButtonState(Button button, bool enabled, bool primary)
        {
            button.interactable = enabled;
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = enabled ? (primary ? LobbyUiTokens.PanelBgAlt : LobbyUiTokens.PanelBg) : LobbyUiTokens.PanelBg;
            }

            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.color = enabled ? LobbyUiTokens.Ink : LobbyUiTokens.InkMute;
            }

            var outline = button.GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectColor = enabled ? (primary ? LobbyUiTokens.Gold : LobbyUiTokens.FrameHi) : LobbyUiTokens.Frame;
            }
        }

        private static void AddTextShadow(Graphic graphic, Color color)
        {
            var shadow = graphic.gameObject.AddComponent<Shadow>();
            shadow.effectColor = color;
            shadow.effectDistance = new Vector2(LobbyUiTokens.FrameBorderWidth, -LobbyUiTokens.FrameBorderWidth);
        }

        private static int ResolveFontSize(LobbyTextStyle style)
        {
            switch (style)
            {
                case LobbyTextStyle.Title:
                    return LobbyUiTokens.TitleFontSize;
                case LobbyTextStyle.Primary:
                    return LobbyUiTokens.PrimaryFontSize;
                case LobbyTextStyle.Body:
                    return LobbyUiTokens.BodyFontSize;
                case LobbyTextStyle.Micro:
                    return LobbyUiTokens.MicroFontSize;
                default:
                    return LobbyUiTokens.LabelFontSize;
            }
        }

        private static float ResolveLineSpacing(LobbyTextStyle style)
        {
            switch (style)
            {
                case LobbyTextStyle.Title:
                    return LobbyUiTokens.TitleLineHeight;
                case LobbyTextStyle.Primary:
                    return LobbyUiTokens.PrimaryLineHeight;
                case LobbyTextStyle.Body:
                    return LobbyUiTokens.BodyLineHeight;
                default:
                    return LobbyUiTokens.LabelLineHeight;
            }
        }

        private static void ApplySlot(RectTransform rect, LobbySlot slot)
        {
            rect.anchorMin = LobbyLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = LobbyLayoutRuntime.AnchorMax(slot);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void ApplyAnchors(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.anchorMin = new Vector2(left, bottom);
            rect.anchorMax = new Vector2(right, top);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void StretchToParent(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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
                scaler.referenceResolution = new Vector2(LobbyLayout.ReferenceWidth, LobbyLayout.ReferenceHeight);
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
            _recordPanel = FindChildGameObject("Lobby Record Panel");
            _statusText = FindChildComponent<Text>("Lobby Run Status");
            _continueButton = FindChildComponent<Button>(LobbyContinueButtonName);

            BindButton(LobbyNewGameButtonName, StartNewGame);
            BindButton(LobbyContinueButtonName, ContinueSavedRun);
            BindButton(LobbyRecordButtonName, OpenRecord);
            BindButton(LobbySettingsButtonName, OpenSettings);
            BindButton(LobbyQuitButtonName, QuitOrShowPlaceholder);
            BindButton("Lobby Record Close Button", CloseRecord);
            BindButton("Lobby Settings Close Button", CloseSettings);

            if (_settingsPanel != null)
            {
                _settingsPanel.SetActive(false);
            }

            if (_recordPanel != null)
            {
                _recordPanel.SetActive(false);
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
    }
}
