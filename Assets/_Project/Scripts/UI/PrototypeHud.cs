using System;
using System.Collections.Generic;
using HwigiTower.Combat;
using HwigiTower.Encounters;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        [SerializeField] private Text focusText;
        [SerializeField] private Text interactionText;
        [SerializeField] private Text runStateText;
        [SerializeField] private Text resultText;
        [SerializeField] private Text routeText;
        [SerializeField] private Text memoryText;
        [SerializeField] private Text demoCompleteText;
        [SerializeField] private RectTransform choiceContainer;
        [SerializeField] private Sprite mataiosPortrait;
        [SerializeField] private DemoPresentationData presentationData;
        [SerializeField] private PrototypeCutscenePlayer cutscenePlayer;
        [SerializeField] private bool showRawDebugText;
        [SerializeField] private Image encounterBackgroundImage;
        [SerializeField] private Image npcPortraitImage;
        [SerializeField] private RectTransform combatPanel;
        [SerializeField] private Text combatText;
        [SerializeField] private Image combatEnemyImage;
        [SerializeField] private RectTransform combatEnemyStage;
        [SerializeField] private RectTransform combatLogPanel;
        [SerializeField] private RectTransform combatPartyDock;
        [SerializeField] private RectTransform combatPlayerCard;
        [SerializeField] private RectTransform combatMataiosCard;
        [SerializeField] private RectTransform eventCutscenePanel;
        [SerializeField] private Image eventCutsceneImage;
        [SerializeField] private Text eventHeaderText;
        [SerializeField] private Text eventBodyText;
        [SerializeField] private Text eventUtilityText;
        [SerializeField] private Text combatEnemyTitleText;
        [SerializeField] private Text combatPlayerCardText;
        [SerializeField] private Text combatMataiosCardText;
        [SerializeField] private Image combatPlayerPortraitImage;
        [SerializeField] private Image combatMataiosPortraitImage;
        [SerializeField] private Image combatPlayerPortraitFrameImage;
        [SerializeField] private Image combatMataiosPortraitFrameImage;
        [SerializeField] private Text combatPlayerPortraitFallbackText;
        [SerializeField] private Image attackActionIconImage;
        [SerializeField] private Image defendActionIconImage;
        [SerializeField] private Image skillActionIconImage;
        [SerializeField] private Image combatTrainingStatusIconImage;
        [SerializeField] private Image combatBandageStatusIconImage;
        [SerializeField] private Image combatRecallStatusIconImage;
        [SerializeField] private Image merchantVisualImage;
        [SerializeField] private RectTransform npcSpotlightLayer;
        [SerializeField] private Image npcSpotlightBackdropImage;
        [SerializeField] private Image npcSpotlightGlowImage;
        [SerializeField] private Image npcSpotlightShadowImage;
        [SerializeField] private Image npcDialoguePlateImage;
        [SerializeField] private Text npcSpotlightNameText;
        [SerializeField] private Text npcSpotlightDialogueText;
        [SerializeField] private Image topGoldIconImage;
        [SerializeField] private Image topMemoryIconImage;
        [SerializeField] private Image topAffinityIconImage;
        [SerializeField] private Image enemyHpFill;
        [SerializeField] private Image playerHpFill;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button skillButton;
        [SerializeField] private Button routeActionButton;
        [SerializeField] private Button nextFloorButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button endingRestButton;
        [SerializeField] private Button endingContinueButton;
        [SerializeField] private RectTransform restInteractionPanel;
        [SerializeField] private Text restResponseText;
        [SerializeField] private InputField restInputField;
        [SerializeField] private Button restAskMoodButton;
        [SerializeField] private Button restTrainButton;
        [SerializeField] private Button restRecoverButton;
        [SerializeField] private Image restAskMoodIconImage;
        [SerializeField] private Image restTrainIconImage;
        [SerializeField] private Image restRecoverIconImage;
        [SerializeField] private Button restSubmitButton;
        [SerializeField] private Button restContinueButton;
        [SerializeField] private RectTransform restResponsePanel;
        [SerializeField] private RectTransform topStatusLayer;
        [SerializeField] private RectTransform objectiveLayer;
        [SerializeField] private RectTransform visualLayer;
        [SerializeField] private RectTransform nodeMapLayer;
        [SerializeField] private RectTransform npcReactionLayer;
        [SerializeField] private RectTransform actionLayer;
        [SerializeField] private RectTransform resultLayer;
        [SerializeField] private RectTransform endingLayer;
        [SerializeField] private RectTransform portraitRoot;
        [SerializeField] private Image floorMapBackgroundImage;

        private readonly List<Button> _choiceButtons = new List<Button>();
        private readonly List<Image> _mapNodeIconImages = new List<Image>();
        private readonly List<Image> _shopChoiceCardImages = new List<Image>();
        private readonly List<Image> _shopChoiceIconImages = new List<Image>();
        private readonly List<GameObject> _mapDecorations = new List<GameObject>();
        private readonly List<string> _demoRouteLabels = new List<string>();
        private readonly List<string> _demoRouteEncounterIds = new List<string>();
        private PrototypeRoomController _roomController;
        private EncounterSelection _pendingRestSelection;
        private string _pendingRestActionId = string.Empty;
        private string _lastMemoryCutsceneKey = string.Empty;
        private string _lastCombatCutsceneKey = string.Empty;
        private string _lastDemoCompleteCutsceneKey = string.Empty;
        private string _npcSpotlightModeLabel = string.Empty;
        private string _activePresentationEncounterId = string.Empty;
        private bool _cutsceneFinishedSubscribed;
        private bool _shopPresentationActive;
        private bool _eventPresentationActive;

        private enum NpcSpotlightMode
        {
            Shop,
            Rest,
            Event
        }

        private const float ChoiceButtonHeight = 118f;
        private const float ChoiceButtonSpacing = 130f;
        private const float EventChoiceButtonHeight = 104f;
        private const float EventChoiceButtonSpacing = 116f;
        private const float MapNodeButtonHeight = 132f;
        private const float MapNodeButtonSpacing = 132f;
        private const float MapNodeIconSize = 90f;
        private const int TitleFontSize = 34;
        private const int SubtitleFontSize = 30;
        private const int BodyFontSize = 28;
        private const int ButtonFontSize = 30;
        private const int ResultFontSize = 28;
        private const int StatFontSize = 26;
        private const int CaptionFontSize = 23;
        private const int CombatBodyFontSize = 31;
        private const float CombatActionButtonSize = 118f;
        private const int ChoiceFontSize = ButtonFontSize;
        private const int MapNodeFontSize = 29;
        private const int RestBodyFontSize = 30;
        private const int RestInputFontSize = 34;
        private const float RestActionCardWidth = 286f;
        private const float RestActionCardHeight = 190f;
        private const int RestActionCardFontSize = 24;
        private const int ResultLineLimit = 3;
        private const float DenseLineSpacing = 0.92f;
        private static readonly Color PrimaryTextColor = new Color(0.90f, 0.95f, 0.96f, 1f);
        private static readonly Color ResultTextColor = new Color(0.88f, 0.93f, 0.95f, 1f);
        private static readonly Color PanelColor = new Color(0.035f, 0.045f, 0.055f, 0.88f);

        public int ChoiceButtonCount => _choiceButtons.Count;
        public string ResultMessage => resultText == null ? string.Empty : resultText.text;
        public string RunStateMessage => runStateText == null ? string.Empty : runStateText.text;
        public string RouteMessage => routeText == null ? string.Empty : routeText.text;
        public string MemoryMessage => memoryText == null ? string.Empty : memoryText.text;
        public string CombatMessage => combatText == null ? string.Empty : combatText.text;
        public bool CombatPanelVisible => combatPanel != null && combatPanel.gameObject.activeSelf;
        public bool PortraitVisible => npcPortraitImage != null && npcPortraitImage.gameObject.activeSelf;
        public bool RouteActionButtonVisible => routeActionButton != null && routeActionButton.gameObject.activeSelf;
        public bool EndingRestButtonVisible => endingRestButton != null && endingRestButton.gameObject.activeSelf;
        public bool EndingContinueButtonVisible => endingContinueButton != null && endingContinueButton.gameObject.activeSelf;
        public bool RestInteractionPanelVisible => restInteractionPanel != null && restInteractionPanel.gameObject.activeSelf;
        public string RestResponseMessage => restResponseText == null ? string.Empty : restResponseText.text;
        public bool RawDebugTextVisible => showRawDebugText;
        public bool HasPresentationData => presentationData != null;
        public bool HasPortraitRoot => portraitRoot != null && portraitRoot.gameObject.activeInHierarchy;
        public Vector2 PortraitRootSize => portraitRoot == null ? Vector2.zero : portraitRoot.sizeDelta;
        public string CurrentBackgroundSpriteName => encounterBackgroundImage != null && encounterBackgroundImage.sprite != null ? encounterBackgroundImage.sprite.name : string.Empty;
        public string CurrentCombatEnemySpriteName => combatEnemyImage != null && combatEnemyImage.sprite != null ? combatEnemyImage.sprite.name : string.Empty;
        public string CurrentCombatPlayerPortraitSpriteName => combatPlayerPortraitImage != null && combatPlayerPortraitImage.sprite != null ? combatPlayerPortraitImage.sprite.name : string.Empty;
        public string CurrentCombatMataiosPortraitSpriteName => combatMataiosPortraitImage != null && combatMataiosPortraitImage.sprite != null ? combatMataiosPortraitImage.sprite.name : string.Empty;
        public string CurrentCombatPortraitFrameSpriteName => combatPlayerPortraitFrameImage != null && combatPlayerPortraitFrameImage.sprite != null ? combatPlayerPortraitFrameImage.sprite.name : string.Empty;
        public bool CombatPlayerPortraitVisible => combatPlayerPortraitImage != null && combatPlayerPortraitImage.gameObject.activeInHierarchy;
        public bool CombatMataiosPortraitVisible => combatMataiosPortraitImage != null && combatMataiosPortraitImage.gameObject.activeInHierarchy;
        public bool CombatPortraitFrameVisible => combatPlayerPortraitFrameImage != null && combatPlayerPortraitFrameImage.gameObject.activeInHierarchy;
        public bool CombatPartyDockVisible => combatPartyDock != null && combatPartyDock.gameObject.activeInHierarchy;
        public string CombatPartyMessage => ((combatPlayerCardText == null ? string.Empty : combatPlayerCardText.text) + "\n" + (combatMataiosCardText == null ? string.Empty : combatMataiosCardText.text)).Trim();
        public string CurrentCombatActionIconNames => string.Join("|", new[]
        {
            attackActionIconImage != null && attackActionIconImage.sprite != null ? attackActionIconImage.sprite.name : string.Empty,
            defendActionIconImage != null && defendActionIconImage.sprite != null ? defendActionIconImage.sprite.name : string.Empty,
            skillActionIconImage != null && skillActionIconImage.sprite != null ? skillActionIconImage.sprite.name : string.Empty
        });
        public string CurrentCombatStatusIconNames => string.Join("|", new[]
        {
            combatTrainingStatusIconImage != null && combatTrainingStatusIconImage.sprite != null ? combatTrainingStatusIconImage.sprite.name : string.Empty,
            combatBandageStatusIconImage != null && combatBandageStatusIconImage.sprite != null ? combatBandageStatusIconImage.sprite.name : string.Empty,
            combatRecallStatusIconImage != null && combatRecallStatusIconImage.sprite != null ? combatRecallStatusIconImage.sprite.name : string.Empty
        });
        public string CurrentTopHudIconNames => string.Join("|", new[]
        {
            topGoldIconImage != null && topGoldIconImage.sprite != null ? topGoldIconImage.sprite.name : string.Empty,
            topMemoryIconImage != null && topMemoryIconImage.sprite != null ? topMemoryIconImage.sprite.name : string.Empty,
            topAffinityIconImage != null && topAffinityIconImage.sprite != null ? topAffinityIconImage.sprite.name : string.Empty
        });
        public string CurrentShopChoiceCardSpriteNames => JoinImageSpriteNames(_shopChoiceCardImages);
        public string CurrentShopChoiceIconNames => JoinImageSpriteNames(_shopChoiceIconImages);
        public string CurrentRestActionIconNames => string.Join("|", new[]
        {
            restAskMoodIconImage != null && restAskMoodIconImage.sprite != null ? restAskMoodIconImage.sprite.name : string.Empty,
            restTrainIconImage != null && restTrainIconImage.sprite != null ? restTrainIconImage.sprite.name : string.Empty,
            restRecoverIconImage != null && restRecoverIconImage.sprite != null ? restRecoverIconImage.sprite.name : string.Empty
        });
        public string CurrentRestActionCardLabels => string.Join("|", new[]
        {
            ResolveButtonLabel(restAskMoodButton),
            ResolveButtonLabel(restTrainButton),
            ResolveButtonLabel(restRecoverButton)
        });
        public bool EventCutsceneVisible => eventCutscenePanel != null && eventCutscenePanel.gameObject.activeInHierarchy;
        public string EventCutsceneMessage => ((eventHeaderText == null ? string.Empty : eventHeaderText.text) + "\n" + (eventBodyText == null ? string.Empty : eventBodyText.text)).Trim();
        public string CurrentPortraitSpriteName => npcPortraitImage != null && npcPortraitImage.sprite != null ? npcPortraitImage.sprite.name : string.Empty;
        public bool NpcSpotlightVisible => npcSpotlightLayer != null && npcSpotlightLayer.gameObject.activeInHierarchy;
        public string CurrentNpcSpotlightSpriteName => merchantVisualImage != null && merchantVisualImage.sprite != null ? merchantVisualImage.sprite.name : string.Empty;
        public string CurrentNpcSupportSpriteNames => string.Join("|", new[]
        {
            npcSpotlightGlowImage != null && npcSpotlightGlowImage.sprite != null ? npcSpotlightGlowImage.sprite.name : string.Empty,
            npcSpotlightShadowImage != null && npcSpotlightShadowImage.sprite != null ? npcSpotlightShadowImage.sprite.name : string.Empty,
            npcDialoguePlateImage != null && npcDialoguePlateImage.sprite != null ? npcDialoguePlateImage.sprite.name : string.Empty
        });
        public string NpcSpotlightMessage => ((npcSpotlightNameText == null ? string.Empty : npcSpotlightNameText.text) + "\n" + (npcSpotlightDialogueText == null ? string.Empty : npcSpotlightDialogueText.text)).Trim();
        public string CurrentNpcSpotlightModeLabel => _npcSpotlightModeLabel;
        public bool HasScreenLayerPanels => topStatusLayer != null && objectiveLayer != null && visualLayer != null && nodeMapLayer != null && npcReactionLayer != null && actionLayer != null && resultLayer != null && endingLayer != null;
        public string CurrentMapNodeIconNames
        {
            get
            {
                var names = new List<string>();
                for (var i = 0; i < _mapNodeIconImages.Count; i++)
                {
                    var icon = _mapNodeIconImages[i];
                    if (icon != null && icon.sprite != null)
                    {
                        names.Add(icon.sprite.name);
                    }
                }

                return string.Join("|", names);
            }
        }

        private static string JoinImageSpriteNames(List<Image> images)
        {
            var names = new List<string>();
            for (var i = 0; i < images.Count; i++)
            {
                var image = images[i];
                if (image != null && image.sprite != null)
                {
                    names.Add(image.sprite.name);
                }
            }

            return string.Join("|", names);
        }

        public void BindRoomController(PrototypeRoomController controller)
        {
            _roomController = controller;
        }

        public void SetPresentationData(DemoPresentationData data)
        {
            presentationData = data;
            if (presentationData != null && presentationData.DefaultMataiosPortrait != null)
            {
                mataiosPortrait = presentationData.DefaultMataiosPortrait;
            }

            ApplyPortrait();
            ApplyRestActionIcons();
            ApplyStaticUiAssetSprites();
        }

        public void SetRawDebugTextVisible(bool visible)
        {
            showRawDebugText = visible;
        }

        private void ApplyStaticUiAssetSprites()
        {
            if (npcSpotlightGlowImage != null)
            {
                npcSpotlightGlowImage.sprite = presentationData == null ? null : presentationData.SpotlightGradient;
                npcSpotlightGlowImage.preserveAspect = true;
                npcSpotlightGlowImage.color = npcSpotlightGlowImage.sprite == null
                    ? new Color(0.52f, 0.72f, 0.62f, 0.22f)
                    : new Color(0.86f, 0.96f, 0.86f, 0.72f);
            }

            if (npcSpotlightShadowImage != null)
            {
                npcSpotlightShadowImage.sprite = presentationData == null ? null : presentationData.SpotlightGradient;
                npcSpotlightShadowImage.preserveAspect = true;
                npcSpotlightShadowImage.color = npcSpotlightShadowImage.sprite == null
                    ? new Color(0f, 0f, 0f, 0.42f)
                    : new Color(0f, 0f, 0f, 0.36f);
            }

            if (npcDialoguePlateImage != null)
            {
                npcDialoguePlateImage.sprite = presentationData == null ? null : presentationData.NpcDialoguePlate;
                npcDialoguePlateImage.preserveAspect = true;
                npcDialoguePlateImage.color = npcDialoguePlateImage.sprite == null
                    ? new Color(0.03f, 0.045f, 0.050f, 0.78f)
                    : Color.white;
            }

            ApplyTopHudIconSprites();
            ApplyCombatStatusIconSprites();
        }

        public void SetNpcPortrait(Sprite portrait)
        {
            mataiosPortrait = portrait;
            ApplyPortrait();
        }

        private void Awake()
        {
            EnsureScreenLayers();
            NormalizeLayout();
            ShowFocus(null);
            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "room" : "준비";
            }

            ShowRunState(default);
            if (resultText != null)
            {
                resultText.text = showRawDebugText ? "result: -" : "결과\n-";
            }

            ApplyPortrait();
            ClearChoices();
        }

        private void Update()
        {
            HandleManualShortcuts();
        }

        public void Configure(Text focus, Text interaction, Text runState = null, Text result = null)
        {
            focusText = focus;
            interactionText = interaction;
            runStateText = runState;
            resultText = result;
            NormalizeLayout();
            ShowFocus(null);
            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "room" : "준비";
            }

            ShowRunState(default);
            if (resultText != null)
            {
                resultText.text = showRawDebugText ? "result: -" : "결과\n-";
            }

            ApplyPortrait();
            ClearChoices();
        }

        private void HandleManualShortcuts()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (restInteractionPanel != null && restInteractionPanel.gameObject.activeSelf)
            {
                if (restContinueButton != null && restContinueButton.gameObject.activeSelf && restContinueButton.interactable &&
                    (keyboard.enterKey.wasPressedThisFrame || keyboard.nKey.wasPressedThisFrame))
                {
                    ContinueAfterRestInteraction();
                    return;
                }

                if (restSubmitButton != null && restSubmitButton.gameObject.activeSelf && restSubmitButton.interactable &&
                    keyboard.enterKey.wasPressedThisFrame)
                {
                    SubmitRestInteraction();
                    return;
                }
            }

            if (routeActionButton != null && routeActionButton.gameObject.activeSelf && routeActionButton.interactable &&
                (keyboard.enterKey.wasPressedThisFrame || keyboard.nKey.wasPressedThisFrame))
            {
                OpenCurrentRouteStep();
                return;
            }

            if (nextFloorButton != null && nextFloorButton.gameObject.activeSelf && nextFloorButton.interactable &&
                (keyboard.enterKey.wasPressedThisFrame || keyboard.nKey.wasPressedThisFrame))
            {
                ResolveNextFloor();
                return;
            }

            if (endingRestButton != null && endingRestButton.gameObject.activeSelf && endingRestButton.interactable &&
                keyboard.digit1Key.wasPressedThisFrame)
            {
                ResolveEndingRest();
                return;
            }

            if (endingContinueButton != null && endingContinueButton.gameObject.activeSelf && endingContinueButton.interactable &&
                keyboard.digit2Key.wasPressedThisFrame)
            {
                ResolveEndingContinue();
                return;
            }

            if (_choiceButtons.Count > 0)
            {
                TryInvokeChoiceShortcut(keyboard);
                return;
            }

            if (combatPanel != null && combatPanel.gameObject.activeSelf)
            {
                if (keyboard.aKey.wasPressedThisFrame && attackButton != null && attackButton.interactable)
                {
                    ResolveCombatAction(CombatAction.Attack);
                    return;
                }

                if (keyboard.dKey.wasPressedThisFrame && defendButton != null && defendButton.interactable)
                {
                    ResolveCombatAction(CombatAction.Defend);
                    return;
                }

                if (keyboard.sKey.wasPressedThisFrame && skillButton != null && skillButton.interactable)
                {
                    ResolveCombatAction(CombatAction.Skill);
                }
            }
        }

        private void TryInvokeChoiceShortcut(Keyboard keyboard)
        {
            var index =
                keyboard.digit1Key.wasPressedThisFrame ? 0 :
                keyboard.digit2Key.wasPressedThisFrame ? 1 :
                keyboard.digit3Key.wasPressedThisFrame ? 2 :
                keyboard.digit4Key.wasPressedThisFrame ? 3 :
                -1;
            if (index < 0 || index >= _choiceButtons.Count)
            {
                return;
            }

            var button = _choiceButtons[index];
            if (button != null && button.gameObject.activeSelf && button.interactable)
            {
                button.onClick.Invoke();
            }
        }

        public void ConfigureDemoRoute(IReadOnlyList<PrototypeDemoRunStep> demoRunPath)
        {
            _demoRouteLabels.Clear();
            _demoRouteEncounterIds.Clear();

            if (demoRunPath != null)
            {
                for (var i = 0; i < demoRunPath.Count; i++)
                {
                    var step = demoRunPath[i];
                    if (step == null || !step.IsValid)
                    {
                        continue;
                    }

                    _demoRouteLabels.Add(BuildRouteLabel(step, false));
                    _demoRouteEncounterIds.Add(step.EncounterId);
                }
            }

            if (_demoRouteLabels.Count > 0)
            {
                _demoRouteLabels.Add("클리어");
                _demoRouteEncounterIds.Add("demo.complete");
            }
        }

        public Button GetChoiceButton(int index)
        {
            return index >= 0 && index < _choiceButtons.Count ? _choiceButtons[index] : null;
        }

        public Button GetRouteActionButton()
        {
            return routeActionButton;
        }

        public Button GetRestActionButton(string actionId)
        {
            switch (actionId)
            {
                case "rest.ask_mood":
                    return restAskMoodButton;
                case "rest.train":
                    return restTrainButton;
                case "rest.recover":
                    return restRecoverButton;
                default:
                    return null;
            }
        }

        public InputField GetRestInputField()
        {
            return restInputField;
        }

        public Button GetRestSubmitButton()
        {
            return restSubmitButton;
        }

        public Button GetRestContinueButton()
        {
            return restContinueButton;
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public void OpenQaRouteStep(EncounterSelection selection)
        {
            OpenSelectedRouteStep(selection);
        }
#endif

        public void ShowChoices(EncounterData encounter, PrototypeEncounterChoiceView[] choiceViews, Action<string> onChoiceSelected)
        {
            ClearChoices();
            HideRestInteractionPanel();
            EnsureScreenLayers();
            HideEventCutsceneLayout();
            SetLayerVisible(actionLayer, true);
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(objectiveLayer, true);
            SetLayerVisible(visualLayer, true);
            SetLayerVisible(npcReactionLayer, true);
            SetLayerVisible(resultLayer, false);
            EnsureEventSystem();
            EnsureChoiceContainer();

            if (choiceContainer == null || choiceViews == null)
            {
                return;
            }

            for (var i = 0; i < choiceViews.Length; i++)
            {
                var view = choiceViews[i];
                if (!view.Visible)
                {
                    continue;
                }

                var button = CreateChoiceButton(view, onChoiceSelected);
                _choiceButtons.Add(button);
            }

            if (interactionText != null && encounter != null)
            {
                ApplyPresentationSlot(encounter.Id);
                if (!showRawDebugText && IsEventCutsceneEncounter(encounter))
                {
                    _shopPresentationActive = false;
                    HideMerchantPresentation();
                    ShowEventCutsceneLayout(encounter);
                    ConfigureChoiceContainerForEvent();
                }
                else if (encounter.Type == EncounterType.Shop)
                {
                    _eventPresentationActive = false;
                    _shopPresentationActive = true;
                    SetLayerVisible(objectiveLayer, false);
                    SetLayerVisible(npcReactionLayer, false);
                    SetLayerVisible(resultLayer, false);
                    SetResultVisible(false);
                    HideLegacyEncounterVisuals(hideBackground: false);
                    ApplyMerchantPresentation(_roomController == null ? 1 : _roomController.GetSnapshot().CurrentFloor);
                    ConfigureChoiceContainerDefault();
                }
                else
                {
                    _eventPresentationActive = false;
                    _shopPresentationActive = false;
                    HideMerchantPresentation();
                    ConfigureChoiceContainerDefault();
                }

                interactionText.text = showRawDebugText
                    ? $"node: {ResolveEncounterDisplayName(encounter)} | encounter: {encounter.Id} | choices pending"
                    : _eventPresentationActive ? string.Empty : ResolvePresentationDisplayName(encounter);
                if (!showRawDebugText)
                {
                    interactionText.gameObject.SetActive(encounter.Type != EncounterType.Shop && !_eventPresentationActive);
                }
            }

            ShowResultMessage(string.Empty);
            if (!showRawDebugText && encounter != null && encounter.Type == EncounterType.Shop && resultText != null)
            {
                resultText.text = string.Empty;
                SetResultVisible(false);
            }
        }

        public void ClearChoices()
        {
            for (var i = 0; i < _choiceButtons.Count; i++)
            {
                if (_choiceButtons[i] != null)
                {
                    Destroy(_choiceButtons[i].gameObject);
                }
            }

            _choiceButtons.Clear();
            _mapNodeIconImages.Clear();
            _shopChoiceCardImages.Clear();
            _shopChoiceIconImages.Clear();
            ClearMapDecorations();
            _shopPresentationActive = false;
            HideMerchantPresentation();
        }

        public void ShowMapChoices(PrototypeFloorMapNodeView[] nodes, Action<string> onNodeSelected)
        {
            ClearChoices();
            HideRestInteractionPanel();
            HideEventCutsceneLayout();
            EnsureScreenLayers();
            SetLayerVisible(nodeMapLayer, true);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            HideMerchantPresentation();
            EnsureEventSystem();
            EnsureChoiceContainer();
            if (choiceContainer == null || nodes == null)
            {
                return;
            }

            ConfigureChoiceContainerForMap();
            ApplyFloorMapBackground(nodes);
            DrawMapRouteLines(nodes);
            DrawMapStartMarker(nodes);

            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var button = CreateMapNodeButton(node, onNodeSelected);
                _choiceButtons.Add(button);
            }

            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "map node selection" : "지도";
                interactionText.gameObject.SetActive(showRawDebugText);
            }

            ShowResultMessage(showRawDebugText ? "select map node" : "선택 가능한 길이 밝게 표시됩니다");
            SetResultVisible(false);
            HideLegacyEncounterVisuals(hideBackground: true);
        }

        private void ClearMapDecorations()
        {
            for (var i = 0; i < _mapDecorations.Count; i++)
            {
                if (_mapDecorations[i] != null)
                {
                    Destroy(_mapDecorations[i]);
                }
            }

            _mapDecorations.Clear();
        }

        private void ApplyFloorMapBackground(PrototypeFloorMapNodeView[] nodes)
        {
            EnsureFloorMapBackgroundImage();
            if (floorMapBackgroundImage == null)
            {
                return;
            }

            var floor = nodes != null && nodes.Length > 0 ? nodes[0].Floor : 1;
            floorMapBackgroundImage.sprite = presentationData != null && presentationData.TryGetFloorMapBackground(floor, out var sprite) ? sprite : null;
            floorMapBackgroundImage.color = floorMapBackgroundImage.sprite == null
                ? new Color(0.024f, 0.033f, 0.043f, 0.92f)
                : new Color(1f, 1f, 1f, 0.62f);
            floorMapBackgroundImage.gameObject.SetActive(true);
        }

        private void EnsureFloorMapBackgroundImage()
        {
            if (floorMapBackgroundImage != null || nodeMapLayer == null)
            {
                return;
            }

            var backgroundObject = new GameObject("Floor Map Background");
            backgroundObject.transform.SetParent(nodeMapLayer, false);
            backgroundObject.transform.SetAsFirstSibling();
            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            floorMapBackgroundImage = backgroundObject.AddComponent<Image>();
            floorMapBackgroundImage.preserveAspect = true;
            floorMapBackgroundImage.raycastTarget = false;
        }

        private void DrawMapRouteLines(PrototypeFloorMapNodeView[] nodes)
        {
            if (nodes == null || nodeMapLayer == null)
            {
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                var from = nodes[i];
                if (from.NextMapNodeIds == null)
                {
                    continue;
                }

                for (var n = 0; n < from.NextMapNodeIds.Length; n++)
                {
                    if (TryFindMapNode(nodes, from.NextMapNodeIds[n], out var to))
                    {
                        DrawMapRouteLine(from, to);
                    }
                }
            }
        }

        private void DrawMapStartMarker(PrototypeFloorMapNodeView[] nodes)
        {
            if (nodes == null || nodes.Length == 0 || presentationData == null || presentationData.CurrentPositionMarker == null)
            {
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Current)
                {
                    return;
                }
            }

            var markerObject = new GameObject("Map Start Marker");
            markerObject.transform.SetParent(nodeMapLayer, false);
            var rect = markerObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.04f);
            rect.anchorMax = new Vector2(0.5f, 0.04f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(MapNodeIconSize * 0.78f, MapNodeIconSize * 0.78f);
            var image = markerObject.AddComponent<Image>();
            image.sprite = presentationData.CurrentPositionMarker;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0.95f);
            _mapDecorations.Add(markerObject);
        }

        private static bool TryFindMapNode(PrototypeFloorMapNodeView[] nodes, string mapNodeId, out PrototypeFloorMapNodeView node)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].MapNodeId == mapNodeId)
                {
                    node = nodes[i];
                    return true;
                }
            }

            node = default;
            return false;
        }

        private void DrawMapRouteLine(PrototypeFloorMapNodeView from, PrototypeFloorMapNodeView to)
        {
            var lineObject = new GameObject("Map Route Line");
            lineObject.transform.SetParent(nodeMapLayer, false);
            lineObject.transform.SetAsFirstSibling();
            var rect = lineObject.AddComponent<RectTransform>();
            var start = ResolveMapNodePosition(from);
            var end = ResolveMapNodePosition(to);
            var center = (start + end) * 0.5f;
            rect.anchorMin = center;
            rect.anchorMax = center;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            var parentRect = nodeMapLayer.rect;
            var parentWidth = parentRect.width > 1f ? parentRect.width : 960f;
            var parentHeight = parentRect.height > 1f ? parentRect.height : 500f;
            var delta = new Vector2((end.x - start.x) * parentWidth, (end.y - start.y) * parentHeight);
            rect.sizeDelta = new Vector2(Mathf.Max(12f, delta.magnitude), 5f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);

            var image = lineObject.AddComponent<Image>();
            image.color = from.Completed && !to.Locked
                ? new Color(0.72f, 0.94f, 0.70f, 0.82f)
                : new Color(0.42f, 0.48f, 0.54f, 0.48f);
            image.raycastTarget = false;
            _mapDecorations.Add(lineObject);
        }

        public void ShowFocus(InteractableNode node)
        {
            if (focusText == null)
            {
                return;
            }

            focusText.text = node == null
                ? (showRawDebugText ? "-" : string.Empty)
                : (showRawDebugText ? node.DisplayName : string.Empty);
        }

        public void ShowInteraction(InteractableNode node, EncounterSelection selection, PrototypeNodeResolution resolution)
        {
            if (interactionText == null || node == null)
            {
                return;
            }

            if (selection.HasEncounter)
            {
                interactionText.text = showRawDebugText
                    ? (string.IsNullOrEmpty(resolution.PayloadId)
                        ? $"encounter: {selection.EncounterId}"
                        : $"encounter: {selection.EncounterId} | payload: {resolution.PayloadId}")
                    : ResolvePresentationDisplayName(selection.Encounter);
                ShowResult(resolution);
                TryPlayInteractionCutscene(selection.EncounterId, resolution.Message);
                return;
            }

            interactionText.text = node.Definition == null ? "진입" : node.Definition.PlaceholderOutcome;
            ShowResult(resolution);
        }

        public void ShowResult(PrototypeNodeResolution resolution)
        {
            ShowResultMessage(resolution.Message);
        }

        public void ShowResultMessage(string message)
        {
            EnsureResultText();
            if (resultText == null)
            {
                return;
            }

            resultText.text = string.IsNullOrEmpty(message)
                ? "결과\n-"
                : BuildResultSummary(message);
        }

        public void ShowRunState(PrototypeRunSnapshot snapshot)
        {
            if (runStateText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(snapshot.RunId))
            {
                runStateText.text = "run: -";
                return;
            }

            if (showRawDebugText)
            {
                var state = snapshot.RunCompleted ? "complete" : "active";
                var demo = string.IsNullOrEmpty(snapshot.NextDemoEncounterId)
                    ? snapshot.DemoStatus
                    : $"{snapshot.DemoStatus} next {snapshot.NextDemoNodeId}/{snapshot.NextDemoEncounterId}";
                runStateText.text =
                    $"run {state} | {demo}\n" +
                    $"HP {snapshot.PlayerHp}/{snapshot.PlayerMaxHp} | ATK {snapshot.PlayerAttack} | Mental {snapshot.Mental}\n" +
                    $"Gold {snapshot.Gold} | Glitch {snapshot.GlitchLevel} | Affinity {snapshot.Affinity}\n" +
                    $"Abilities {snapshot.AbilityCount} | Step {snapshot.DemoResolvedStepCount}/{snapshot.DemoStepCount} | {snapshot.RunStatus}";
            }
            else
            {
                var status = snapshot.EndingRest ? " | 안식" :
                    snapshot.EndingContinue ? " | 동행 계속" :
                    snapshot.RunClear ? " | 클리어" :
                    snapshot.RunFailed ? " | 실패" :
                    string.Empty;
                runStateText.text =
                    $"Floor {snapshot.CurrentFloor}{status}  HP {snapshot.PlayerHp}/{snapshot.PlayerMaxHp}  Gold {snapshot.Gold}  정신 {snapshot.Mental}\n" +
                    $"기억 {snapshot.MemoryFragmentCount}  능력 {snapshot.AbilityCount}  아이템 {snapshot.ItemCount}";
            }

            if (snapshot.RunCompleted)
            {
                ClearChoices();
            }

            EnsureEventSystem();
            AutoShowMapIfNeeded(snapshot);
            UpdateScreenLayers(snapshot);
            UpdateNextFloorButton(snapshot);
            UpdateRouteActionButton(snapshot);
            UpdateRestartButton(snapshot);
            UpdateEndingButtons(snapshot);
            UpdatePresentationState(snapshot);
            UpdateRouteIndicator(snapshot);
            UpdatePrimaryHeaderVisibility(snapshot);
            UpdateTopHudIcons(snapshot);
            UpdateMemoryAndCombatPanel(snapshot);
            UpdateResultVisibility(snapshot);
            UpdateDemoCompletePanel(snapshot);
            UpdateCutsceneTriggers(snapshot);
        }

        private void UpdateScreenLayers(PrototypeRunSnapshot snapshot)
        {
            EnsureScreenLayers();
            var restVisible = RestInteractionPanelVisible;
            var mapVisible = IsMapSelectionVisible(snapshot);
            var shopVisible = !snapshot.IsInCombat && !restVisible && _shopPresentationActive;
            var eventVisible = _eventPresentationActive;
            SetLayerVisible(topStatusLayer, true);
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, !snapshot.IsInCombat && !eventVisible && !mapVisible);
            SetLayerVisible(nodeMapLayer, mapVisible);
            SetLayerVisible(npcReactionLayer, showRawDebugText && !snapshot.IsInCombat && !snapshot.EndingChoicePending && !eventVisible && !mapVisible && !shopVisible && !restVisible);
            SetLayerVisible(actionLayer, !snapshot.IsInCombat && !snapshot.RunCompleted && !mapVisible);
            SetLayerVisible(resultLayer, !snapshot.IsInCombat && !eventVisible && !mapVisible && !shopVisible && !restVisible && resultText != null && resultText.gameObject.activeSelf);
            SetLayerVisible(endingLayer, snapshot.EndingChoicePending && !snapshot.IsInCombat);
        }

        private bool IsMapSelectionVisible(PrototypeRunSnapshot snapshot)
        {
            return snapshot.HasFloorMap &&
                !snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                choiceContainer != null &&
                nodeMapLayer != null &&
                choiceContainer.parent == nodeMapLayer &&
                _choiceButtons.Count > 0;
        }

        private void AutoShowMapIfNeeded(PrototypeRunSnapshot snapshot)
        {
            if (_roomController == null ||
                !snapshot.HasFloorMap ||
                snapshot.HasSelectedMapNode ||
                snapshot.IsInCombat ||
                snapshot.RunCompleted ||
                snapshot.StairUnlocked ||
                RestInteractionPanelVisible ||
                _choiceButtons.Count > 0)
            {
                return;
            }

            var mapNodes = _roomController.GetFloorMapNodes();
            if (mapNodes.Length == 0)
            {
                return;
            }

            ShowMapChoices(mapNodes, mapNodeId =>
            {
                var selected = _roomController.SelectMapNode(mapNodeId);
                OpenSelectedRouteStep(selected);
            });
        }

        private Transform HudParent
        {
            get
            {
                EnsurePortraitRoot();
                return portraitRoot == null ? transform : portraitRoot;
            }
        }

        private void EnsurePortraitRoot()
        {
            if (portraitRoot != null)
            {
                return;
            }

            var gutterObject = new GameObject("Portrait Dark Gutter");
            gutterObject.transform.SetParent(transform, false);
            gutterObject.transform.SetAsFirstSibling();
            var gutterRect = gutterObject.AddComponent<RectTransform>();
            gutterRect.anchorMin = Vector2.zero;
            gutterRect.anchorMax = Vector2.one;
            gutterRect.offsetMin = Vector2.zero;
            gutterRect.offsetMax = Vector2.zero;
            var gutter = gutterObject.AddComponent<Image>();
            gutter.color = new Color(0.006f, 0.008f, 0.012f, 1f);
            gutter.raycastTarget = false;

            var rootObject = new GameObject("PortraitRoot");
            rootObject.transform.SetParent(transform, false);
            var rect = rootObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(1080f, 1920f);
            rect.anchoredPosition = Vector2.zero;

            var image = rootObject.AddComponent<Image>();
            image.color = new Color(0.018f, 0.023f, 0.030f, 1f);
            image.raycastTarget = false;
            portraitRoot = rect;
        }

        private Button CreateChoiceButton(PrototypeEncounterChoiceView view, Action<string> onChoiceSelected)
        {
            var buttonObject = new GameObject($"Choice Button {view.ChoiceStableId}");
            buttonObject.transform.SetParent(choiceContainer, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, ChoiceButtonHeight);
            rect.anchoredPosition = new Vector2(0f, -_choiceButtons.Count * ChoiceButtonSpacing);

            var image = buttonObject.AddComponent<Image>();
            image.color = view.Enabled
                ? new Color(0.12f, 0.16f, 0.19f, 0.98f)
                : new Color(0.06f, 0.07f, 0.08f, 0.84f);

            var button = buttonObject.AddComponent<Button>();
            button.interactable = view.Enabled;
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.92f, 0.96f, 1f, 1f);
            colors.pressedColor = new Color(0.74f, 0.82f, 0.90f, 1f);
            colors.disabledColor = new Color(0.56f, 0.58f, 0.62f, 0.78f);
            button.colors = colors;

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);

            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(20f, 8f);
            labelRect.offsetMax = new Vector2(-20f, -8f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = ChoiceFontSize;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 22;
            label.resizeTextMaxSize = ChoiceFontSize;
            label.supportRichText = false;
            label.lineSpacing = 1f;
            label.raycastTarget = false;
            label.color = view.Enabled
                ? new Color(0.88f, 0.92f, 0.94f, 1f)
                : new Color(0.58f, 0.62f, 0.66f, 1f);
            label.text = BuildChoiceLabel(view, _choiceButtons.Count);
            ApplyShopChoiceCard(view, image, label);

            var stableId = view.ChoiceStableId;
            button.onClick.AddListener(() =>
            {
                ClearChoices();
                onChoiceSelected?.Invoke(stableId);
            });

            return button;
        }

        private void ApplyShopChoiceCard(PrototypeEncounterChoiceView view, Image background, Text label)
        {
            if (showRawDebugText || background == null || label == null || !IsPurchaseChoice(view.ChoiceStableId))
            {
                return;
            }

            var cardSprite = ResolveShopCardSprite(view.Enabled);
            if (cardSprite != null)
            {
                background.sprite = cardSprite;
                background.type = Image.Type.Simple;
                background.preserveAspect = false;
                background.color = view.Enabled
                    ? new Color(1f, 1f, 1f, 0.92f)
                    : new Color(0.68f, 0.72f, 0.76f, 0.88f);
                _shopChoiceCardImages.Add(background);
            }

            var labelRect = label.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                labelRect.offsetMin = new Vector2(130f, 10f);
                labelRect.offsetMax = new Vector2(-18f, -10f);
            }

            label.alignment = TextAnchor.MiddleLeft;
            label.fontSize = 27;
            label.resizeTextMinSize = 20;
            label.resizeTextMaxSize = 27;
            label.color = view.Enabled
                ? new Color(0.95f, 0.97f, 0.94f, 1f)
                : new Color(0.78f, 0.82f, 0.82f, 0.96f);

            var iconSprite = ResolvePurchaseChoiceIcon(view);
            if (iconSprite == null)
            {
                return;
            }

            var iconObject = new GameObject("Shop Choice Icon");
            iconObject.transform.SetParent(background.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.04f, 0.18f);
            iconRect.anchorMax = new Vector2(0.18f, 0.82f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            var icon = iconObject.AddComponent<Image>();
            icon.sprite = iconSprite;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.color = view.Enabled ? Color.white : new Color(0.72f, 0.76f, 0.78f, 0.88f);
            _shopChoiceIconImages.Add(icon);
        }

        private Button CreateMapNodeButton(PrototypeFloorMapNodeView node, Action<string> onNodeSelected)
        {
            var index = _choiceButtons.Count;
            var view = new PrototypeEncounterChoiceView(
                node.MapNodeId,
                ResolveMapNodeLabel(node.Type),
                true,
                node.Selectable,
                string.Empty,
                BuildMapNodeHint(node));
            var button = CreateChoiceButton(view, onNodeSelected);
            button.name = "Map Node Button " + node.MapNodeId;

            var rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                var position = ResolveMapNodePosition(node);
                rect.anchorMin = position;
                rect.anchorMax = position;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(MapNodeButtonHeight, MapNodeButtonHeight);
                rect.anchoredPosition = Vector2.zero;
            }

            var image = button.targetGraphic as Image;
            if (image != null)
            {
                image.color = ResolveMapNodeTint(node);
            }

            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                var labelRect = label.GetComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0f, 0f);
                labelRect.anchorMax = new Vector2(1f, 0.28f);
                labelRect.offsetMin = new Vector2(4f, 2f);
                labelRect.offsetMax = new Vector2(-4f, -2f);
                label.text = ResolveMapNodeLabel(node.Type);
                label.fontSize = node.Selectable ? 22 : 19;
                label.resizeTextMinSize = 16;
                label.resizeTextMaxSize = node.Selectable ? 22 : 19;
                label.alignment = TextAnchor.MiddleCenter;
                label.color = node.Selectable
                    ? new Color(0.96f, 0.99f, 1f, 1f)
                    : node.Completed
                        ? new Color(0.74f, 0.84f, 0.78f, 0.92f)
                        : new Color(0.50f, 0.54f, 0.58f, 0.82f);
            }

            var iconObject = new GameObject("Node Icon");
            iconObject.transform.SetParent(button.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.58f);
            iconRect.anchorMax = new Vector2(0.5f, 0.58f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(MapNodeIconSize * 0.92f, MapNodeIconSize * 0.92f);

            var icon = iconObject.AddComponent<Image>();
            icon.sprite = ResolveNodeIcon(node.Type);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.color = node.Completed
                ? new Color(0.56f, 0.78f, 0.66f, 0.84f)
                : node.Locked
                    ? new Color(0.32f, 0.34f, 0.38f, 0.52f)
                    : Color.white;
            _mapNodeIconImages.Add(icon);

            if (node.Selectable && presentationData != null && presentationData.SelectedNodeRing != null)
            {
                AddMapNodeOverlay(button.transform, "Node Selectable Ring", presentationData.SelectedNodeRing, new Color(1f, 0.94f, 0.62f, 0.96f), 1.10f);
            }

            if (node.Locked && presentationData != null && presentationData.LockedNodeOverlay != null)
            {
                AddMapNodeOverlay(button.transform, "Node Locked Overlay", presentationData.LockedNodeOverlay, new Color(1f, 1f, 1f, 0.72f), 1.02f);
            }

            if (node.Completed && ResolveCompletedNodeBackground() != null)
            {
                AddMapNodeOverlay(button.transform, "Node Completed Mark", ResolveCompletedNodeBackground(), new Color(1f, 1f, 1f, 0.92f), 0.86f);
            }

            if (node.Current && presentationData != null && presentationData.CurrentPositionMarker != null)
            {
                AddMapNodeOverlay(button.transform, "Node Current Marker", presentationData.CurrentPositionMarker, new Color(1f, 1f, 1f, 0.98f), 1.24f);
            }

            return button;
        }

        private static Vector2 ResolveMapNodePosition(PrototypeFloorMapNodeView node)
        {
            if (node.NormalizedX > 0f || node.NormalizedY > 0f)
            {
                return new Vector2(node.NormalizedX, node.NormalizedY);
            }

            var fallbackX = node.Index <= 0 ? 0.34f : node.Index == 1 ? 0.66f : 0.50f;
            var fallbackY = node.Layer switch
            {
                1 => 0.16f,
                2 => 0.37f,
                3 => 0.58f,
                4 => 0.75f,
                5 => 0.91f,
                _ => 0.5f
            };
            return new Vector2(fallbackX, fallbackY);
        }

        private void AddMapNodeOverlay(Transform parent, string name, Sprite sprite, Color color, float scale)
        {
            if (sprite == null)
            {
                return;
            }

            var overlayObject = new GameObject(name);
            overlayObject.transform.SetParent(parent, false);
            overlayObject.transform.SetAsFirstSibling();
            var rect = overlayObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(MapNodeButtonHeight * scale, MapNodeButtonHeight * scale);

            var image = overlayObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = color;
        }

        private Sprite ResolveNodeIcon(PrototypeFloorMapNodeType type)
        {
            return presentationData != null && presentationData.TryGetNodeIcon(type, out var icon) ? icon : null;
        }

        private Sprite ResolveCompletedNodeBackground()
        {
            return presentationData != null && presentationData.TryGetCompletedNodeBackground(out var sprite) ? sprite : null;
        }

        private static Color ResolveMapNodeTint(PrototypeFloorMapNodeView node)
        {
            if (node.Completed)
            {
                return new Color(0.07f, 0.20f, 0.14f, 0.90f);
            }

            if (node.Locked || !node.Selectable)
            {
                return new Color(0.035f, 0.040f, 0.050f, 0.64f);
            }

            return node.Type == PrototypeFloorMapNodeType.Boss
                ? new Color(0.36f, 0.08f, 0.10f, 0.99f)
                : node.Type == PrototypeFloorMapNodeType.Shop
                    ? new Color(0.27f, 0.20f, 0.08f, 0.99f)
                    : new Color(0.11f, 0.22f, 0.28f, 0.99f);
        }

        private static string BuildMapNodeHint(PrototypeFloorMapNodeView node)
        {
            var state = node.Completed ? "완료" : node.Locked ? "잠김" : node.Selectable ? "선택 가능" : "대기";
            return state + " · " + ResolveMapNodeHint(node.Type);
        }

        private void EnsureScreenLayers()
        {
            EnsurePortraitRoot();
            topStatusLayer = EnsureLayerPanel(topStatusLayer, "Screen Layer Top Status", new Vector2(0f, 0.92f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, new Color(0.025f, 0.032f, 0.04f, 0.88f), false);
            objectiveLayer = EnsureLayerPanel(objectiveLayer, "Screen Layer Objective", new Vector2(0.04f, 0.84f), new Vector2(0.96f, 0.915f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, new Color(0.05f, 0.07f, 0.09f, 0.82f), false);
            visualLayer = EnsureLayerPanel(visualLayer, "Screen Layer Visual", new Vector2(0.04f, 0.49f), new Vector2(0.96f, 0.835f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.03f, 0.04f, 0.05f, 0.52f), false);
            npcReactionLayer = EnsureLayerPanel(npcReactionLayer, "Screen Layer Companion Status", new Vector2(0.04f, 0.375f), new Vector2(0.96f, 0.485f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, PanelColor, false);
            resultLayer = EnsureLayerPanel(resultLayer, "Screen Layer Result", new Vector2(0.06f, 0.305f), new Vector2(0.94f, 0.405f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.035f, 0.045f, 0.055f, 0.90f), false);
            nodeMapLayer = EnsureLayerPanel(nodeMapLayer, "Screen Layer Node Map", new Vector2(0.06f, 0.09f), new Vector2(0.94f, 0.73f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.030f, 0.040f, 0.050f, 0.90f), false);
            actionLayer = EnsureLayerPanel(actionLayer, "Screen Layer Action", new Vector2(0.06f, 0.045f), new Vector2(0.94f, 0.265f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.02f, 0.025f, 0.03f, 0.50f), false);
            endingLayer = EnsureLayerPanel(endingLayer, "Screen Layer Ending", new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.30f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.06f, 0.055f, 0.04f, 0.90f), false);
        }

        private void EnsureTopHudIcons()
        {
            EnsureScreenLayers();
            topGoldIconImage = EnsureHudIcon(topGoldIconImage, "Top Gold Icon", topStatusLayer, new Vector2(0.56f, 0.58f), 34f);
            topMemoryIconImage = EnsureHudIcon(topMemoryIconImage, "Top Memory Icon", topStatusLayer, new Vector2(0.28f, 0.24f), 32f);
            topAffinityIconImage = EnsureHudIcon(topAffinityIconImage, "Top Affinity Icon", topStatusLayer, new Vector2(0.48f, 0.24f), 32f);
        }

        private static Image EnsureHudIcon(Image current, string name, Transform parent, Vector2 anchor, float size)
        {
            if (current != null || parent == null)
            {
                return current;
            }

            var iconObject = new GameObject(name);
            iconObject.transform.SetParent(parent, false);
            var rect = iconObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = Vector2.zero;
            var image = iconObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private RectTransform EnsureLayerPanel(RectTransform layer, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color color, bool active)
        {
            if (layer != null)
            {
                return layer;
            }

            var layerObject = new GameObject(name);
            layerObject.transform.SetParent(HudParent, false);
            layerObject.transform.SetAsFirstSibling();

            var rect = layerObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var image = layerObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            layerObject.SetActive(active);
            return rect;
        }

        private static void SetLayerVisible(RectTransform layer, bool visible)
        {
            if (layer != null)
            {
                layer.gameObject.SetActive(visible);
            }
        }

        private void SetResultVisible(bool visible)
        {
            if (resultText != null)
            {
                resultText.gameObject.SetActive(visible);
            }

            SetLayerVisible(resultLayer, visible);
        }

        private void NormalizeLayout()
        {
            EnsureScreenLayers();
            MoveTextUnderPortraitRoot(focusText);
            MoveTextUnderPortraitRoot(interactionText);
            MoveTextUnderPortraitRoot(runStateText);
            MoveTextUnderPortraitRoot(resultText);
            ApplyTextRect(focusText, new Vector2(0.06f, 0.965f), new Vector2(0.94f, 0.995f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, SubtitleFontSize, TextAnchor.UpperCenter);
            ApplyTextRect(interactionText, new Vector2(0.06f, 0.89f), new Vector2(0.94f, 0.925f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, TitleFontSize, TextAnchor.MiddleCenter);
            ApplyTextRect(runStateText, new Vector2(0.06f, 0.925f), new Vector2(0.94f, 0.965f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, BodyFontSize, TextAnchor.MiddleCenter);
            ApplyTextRect(resultText, new Vector2(0.08f, 0.315f), new Vector2(0.92f, 0.395f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, ResultFontSize, TextAnchor.MiddleCenter);
            if (resultText != null)
            {
                resultText.lineSpacing = DenseLineSpacing;
            }
        }

        private void MoveTextUnderPortraitRoot(Text text)
        {
            if (text != null && portraitRoot != null && text.transform.parent != portraitRoot)
            {
                text.transform.SetParent(portraitRoot, false);
            }
        }

        private static void ApplyTextRect(Text text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)
        {
            if (text == null)
            {
                return;
            }

            var rect = text.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = anchorMin;
                rect.anchorMax = anchorMax;
                rect.pivot = pivot;
                rect.anchoredPosition = position;
                rect.sizeDelta = size;
            }

            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(18, fontSize - 8);
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
            text.raycastTarget = false;
        }

        private void EnsureChoiceContainer()
        {
            if (choiceContainer != null)
            {
                return;
            }

            var containerObject = new GameObject("Choice Buttons");
            containerObject.transform.SetParent(HudParent, false);

            choiceContainer = containerObject.AddComponent<RectTransform>();
            choiceContainer.anchorMin = new Vector2(0.08f, 0f);
            choiceContainer.anchorMax = new Vector2(0.92f, 0f);
            choiceContainer.pivot = new Vector2(0.5f, 0f);
            choiceContainer.sizeDelta = new Vector2(0f, 560f);
            choiceContainer.anchoredPosition = new Vector2(0f, 52f);
        }

        private void ConfigureChoiceContainerDefault()
        {
            EnsureChoiceContainer();
            if (choiceContainer == null)
            {
                return;
            }

            choiceContainer.SetParent(HudParent, false);
            choiceContainer.anchorMin = new Vector2(0.08f, 0f);
            choiceContainer.anchorMax = new Vector2(0.92f, 0f);
            choiceContainer.pivot = new Vector2(0.5f, 0f);
            choiceContainer.sizeDelta = new Vector2(0f, 560f);
            choiceContainer.anchoredPosition = new Vector2(0f, 52f);
        }

        private void ConfigureChoiceContainerForEvent()
        {
            EnsureChoiceContainer();
            if (choiceContainer == null)
            {
                return;
            }

            choiceContainer.SetParent(HudParent, false);
            choiceContainer.anchorMin = new Vector2(0.08f, 0f);
            choiceContainer.anchorMax = new Vector2(0.92f, 0f);
            choiceContainer.pivot = new Vector2(0.5f, 0f);
            choiceContainer.sizeDelta = new Vector2(0f, 360f);
            choiceContainer.anchoredPosition = new Vector2(0f, 74f);

            for (var i = 0; i < _choiceButtons.Count; i++)
            {
                var button = _choiceButtons[i];
                if (button == null)
                {
                    continue;
                }

                var rect = button.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.pivot = new Vector2(0.5f, 1f);
                    rect.sizeDelta = new Vector2(0f, EventChoiceButtonHeight);
                    rect.anchoredPosition = new Vector2(0f, -i * EventChoiceButtonSpacing);
                }
            }
        }

        private void ConfigureChoiceContainerForMap()
        {
            EnsureChoiceContainer();
            if (choiceContainer == null || nodeMapLayer == null)
            {
                return;
            }

            choiceContainer.SetParent(nodeMapLayer, false);
            choiceContainer.anchorMin = Vector2.zero;
            choiceContainer.anchorMax = Vector2.one;
            choiceContainer.pivot = new Vector2(0.5f, 0.5f);
            choiceContainer.sizeDelta = Vector2.zero;
            choiceContainer.anchoredPosition = Vector2.zero;
        }

        private void EnsureEventCutscenePanel()
        {
            if (eventCutscenePanel != null)
            {
                return;
            }

            EnsureScreenLayers();
            var panelObject = new GameObject("Event Cutscene Panel");
            panelObject.transform.SetParent(HudParent, false);

            eventCutscenePanel = panelObject.AddComponent<RectTransform>();
            eventCutscenePanel.anchorMin = new Vector2(0.06f, 0.255f);
            eventCutscenePanel.anchorMax = new Vector2(0.94f, 0.835f);
            eventCutscenePanel.offsetMin = Vector2.zero;
            eventCutscenePanel.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.015f, 0.018f, 0.022f, 0.86f);
            image.raycastTarget = false;

            eventHeaderText = CreateCombatChildText(eventCutscenePanel, "Event Header Text", new Vector2(0.04f, 0.88f), new Vector2(0.96f, 0.99f), 30, TextAnchor.MiddleCenter);
            eventHeaderText.color = new Color(0.92f, 0.95f, 0.94f, 1f);

            var imageFrame = CreateCombatPanelRect(eventCutscenePanel, "Event Image Frame", new Vector2(0.07f, 0.36f), new Vector2(0.93f, 0.86f), new Color(0.035f, 0.040f, 0.046f, 0.95f));
            eventCutsceneImage = CreateCombatImage(imageFrame, "Event Cutscene Image", new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.98f));
            eventCutsceneImage.color = Color.white;

            eventBodyText = CreateCombatChildText(eventCutscenePanel, "Event Body Text", new Vector2(0.06f, 0.10f), new Vector2(0.94f, 0.33f), 29, TextAnchor.MiddleLeft);
            eventBodyText.lineSpacing = 0.94f;

            eventUtilityText = CreateCombatChildText(eventCutscenePanel, "Event Utility Text", new Vector2(0.08f, 0.01f), new Vector2(0.92f, 0.07f), 24, TextAnchor.MiddleCenter);
            eventUtilityText.color = new Color(0.76f, 0.82f, 0.84f, 1f);
            eventUtilityText.text = "기록    정보    설정";
            eventCutscenePanel.gameObject.SetActive(false);
        }

        private void ShowEventCutsceneLayout(EncounterData encounter)
        {
            EnsureEventCutscenePanel();
            _eventPresentationActive = true;
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            if (eventCutscenePanel != null)
            {
                eventCutscenePanel.gameObject.SetActive(true);
            }

            if (routeText != null)
            {
                routeText.gameObject.SetActive(false);
            }

            if (memoryText != null)
            {
                memoryText.text = string.Empty;
                memoryText.gameObject.SetActive(false);
            }

            if (resultText != null)
            {
                resultText.text = string.Empty;
                resultText.gameObject.SetActive(false);
            }

            if (npcPortraitImage != null)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            var slot = ResolvePresentationSlot(encounter == null ? string.Empty : encounter.Id);
            if (eventCutsceneImage != null)
            {
                eventCutsceneImage.sprite = ResolveEventSprite(slot);
                eventCutsceneImage.gameObject.SetActive(eventCutsceneImage.sprite != null);
            }

            if (eventHeaderText != null)
            {
                eventHeaderText.text = ResolveEventHeader(encounter);
            }

            if (eventBodyText != null)
            {
                eventBodyText.text = ResolveEventBody(encounter, slot);
            }

            if (eventUtilityText != null)
            {
                eventUtilityText.gameObject.SetActive(true);
            }
        }

        private void HideEventCutsceneLayout(bool restoreRouteText = true)
        {
            _eventPresentationActive = false;
            if (eventCutscenePanel != null)
            {
                eventCutscenePanel.gameObject.SetActive(false);
            }

            if (restoreRouteText && routeText != null)
            {
                routeText.gameObject.SetActive(true);
            }
        }

        private DemoPresentationSlot ResolvePresentationSlot(string encounterStableId)
        {
            return presentationData != null && presentationData.TryGetSlot(encounterStableId, out var slot) ? slot : null;
        }

        private static Sprite ResolveEventSprite(DemoPresentationSlot slot)
        {
            if (slot == null)
            {
                return null;
            }

            return slot.MemoryFragmentSprite != null ? slot.MemoryFragmentSprite : slot.BackgroundSprite;
        }

        private static bool IsEventCutsceneEncounter(EncounterData encounter)
        {
            if (encounter == null)
            {
                return false;
            }

            return encounter.Type == EncounterType.Story ||
                encounter.Type == EncounterType.MoralChoice ||
                encounter.Type == EncounterType.MemoryFragment ||
                encounter.Type == EncounterType.Remnant;
        }

        private static string ResolveEventHeader(EncounterData encounter)
        {
            if (encounter != null && encounter.Id == "EVT_F01_JAR_ROOM")
            {
                return "당신은 이상한 냄새가 나는 항아리 방에 들어섰다.";
            }

            return encounter == null ? "이벤트" : ResolvePublicEncounterLabel(string.Empty, encounter);
        }

        private static string ResolveEventBody(EncounterData encounter, DemoPresentationSlot slot)
        {
            if (encounter != null && encounter.Id == "EVT_F01_JAR_ROOM")
            {
                return "방 한가운데 놓인 항아리들이 낮게 울린다.\n무엇을 건드릴지 고르면 결과가 결정된다.";
            }

            if (slot != null && !string.IsNullOrEmpty(slot.BodyTextKey))
            {
                return "임시 이벤트 텍스트\n" + slot.BodyTextKey;
            }

            return "임시 이벤트 텍스트\n선택 전 결과와 위험을 확인하세요.";
        }

        private void ShowRestInteraction(EncounterSelection selection)
        {
            ClearChoices();
            EnsureRestInteractionPanel();
            HideEventCutsceneLayout();
            EnsureScreenLayers();
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(visualLayer, true);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            ApplyPresentationSlot(selection.EncounterId);
            ShowNpcSpotlight(mataiosPortrait, "마타이오스", "잠시 숨을 고른다. 무엇을 건넬지 정하세요.", NpcSpotlightMode.Rest);
            _pendingRestSelection = selection;
            _pendingRestActionId = string.Empty;
            SetRestActionCardsVisible(true);
            UpdateRestActionCardStates();
            if (restInputField != null)
            {
                restInputField.text = string.Empty;
                restInputField.interactable = true;
            }

            if (restResponseText != null)
            {
                restResponseText.text = string.Empty;
            }

            SetRestActionButtonsInteractable(true);
            SetRestInputPhaseVisible(false, false);

            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "rest interaction: " + selection.EncounterId : "휴식";
                if (!showRawDebugText)
                {
                    interactionText.gameObject.SetActive(false);
                }
            }

            if (restInteractionPanel != null)
            {
                restInteractionPanel.gameObject.SetActive(true);
            }

            ShowResultMessage(string.Empty);
            SetResultVisible(false);
        }

        private void EnsureRestInteractionPanel()
        {
            if (restInteractionPanel != null)
            {
                return;
            }

            EnsureEventSystem();
            var panelObject = new GameObject("Rest Interaction Panel");
            panelObject.transform.SetParent(HudParent, false);

            restInteractionPanel = panelObject.AddComponent<RectTransform>();
            restInteractionPanel.anchorMin = new Vector2(0.08f, 0f);
            restInteractionPanel.anchorMax = new Vector2(0.92f, 0f);
            restInteractionPanel.pivot = new Vector2(0.5f, 0f);
            restInteractionPanel.anchoredPosition = new Vector2(0f, 54f);
            restInteractionPanel.sizeDelta = new Vector2(0f, 730f);

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.045f, 0.055f, 0.070f, 0.97f);

            var title = CreateHudText("Rest Interaction Title", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), new Vector2(-36f, 56f), TitleFontSize, TextAnchor.MiddleCenter, new Color(0.92f, 0.96f, 0.94f, 1f));
            title.transform.SetParent(panelObject.transform, false);
            title.text = "휴식";

            restAskMoodButton = CreateRestActionButton(panelObject.transform, "Rest Button Ask Mood", new Vector2(0.17f, 0.81f), "rest.ask_mood");
            restTrainButton = CreateRestActionButton(panelObject.transform, "Rest Button Train", new Vector2(0.50f, 0.81f), "rest.train");
            restRecoverButton = CreateRestActionButton(panelObject.transform, "Rest Button Recover", new Vector2(0.83f, 0.81f), "rest.recover");

            restInputField = CreateRestInputField(panelObject.transform);
            restSubmitButton = CreateRestButton(panelObject.transform, "Rest Submit Button", "전달", new Vector2(0.16f, 0.13f), new Vector2(0.46f, 0.26f));
            restSubmitButton.onClick.AddListener(SubmitRestInteraction);
            restContinueButton = CreateRestButton(panelObject.transform, "Rest Continue Button", "계속", new Vector2(0.54f, 0.13f), new Vector2(0.84f, 0.26f));
            restContinueButton.onClick.AddListener(ContinueAfterRestInteraction);
            restContinueButton.gameObject.SetActive(false);

            restResponsePanel = CreatePanel("Rest Response Bubble", panelObject.transform, new Vector2(0.06f, 0.30f), new Vector2(0.94f, 0.50f), new Color(0.10f, 0.13f, 0.15f, 0.96f));
            restResponseText = CreateHudText("Rest Response Text", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(20f, 10f), new Vector2(-20f, -10f), RestBodyFontSize, TextAnchor.MiddleLeft, new Color(0.88f, 0.93f, 0.92f, 1f));
            restResponseText.horizontalOverflow = HorizontalWrapMode.Wrap;
            restResponseText.transform.SetParent(panelObject.transform, false);
            restResponseText.transform.SetParent(restResponsePanel, false);
            restResponseText.text = string.Empty;
            restResponsePanel.gameObject.SetActive(false);
            restInteractionPanel.gameObject.SetActive(false);
            ApplyRestActionIcons();
        }

        private Button CreateRestActionButton(Transform parent, string name, Vector2 center, string actionId)
        {
            var button = CreateRestButton(parent, name, BuildRestActionCardLabel(actionId), center, center);
            var rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = center;
                rect.anchorMax = center;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(RestActionCardWidth, RestActionCardHeight);
            }

            var labelText = button.GetComponentInChildren<Text>();
            if (labelText != null)
            {
                var labelRect = labelText.GetComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0f, 0f);
                labelRect.anchorMax = new Vector2(1f, 0.37f);
                labelRect.offsetMin = new Vector2(10f, 8f);
                labelRect.offsetMax = new Vector2(-10f, -8f);
                labelText.fontSize = RestActionCardFontSize;
                labelText.resizeTextMinSize = 18;
                labelText.resizeTextMaxSize = RestActionCardFontSize;
                labelText.alignment = TextAnchor.MiddleCenter;
                labelText.lineSpacing = 0.92f;
            }

            var labelScrim = CreatePanel(name + " Label Scrim", button.transform, new Vector2(0f, 0f), new Vector2(1f, 0.38f), new Color(0.015f, 0.020f, 0.025f, 0.72f));
            labelScrim.SetSiblingIndex(0);
            if (labelText != null)
            {
                labelText.transform.SetAsLastSibling();
            }

            var cardImage = button.targetGraphic as Image;
            if (actionId == "rest.ask_mood")
            {
                restAskMoodIconImage = cardImage;
            }
            else if (actionId == "rest.train")
            {
                restTrainIconImage = cardImage;
            }
            else if (actionId == "rest.recover")
            {
                restRecoverIconImage = cardImage;
            }

            button.onClick.AddListener(() => SelectRestAction(actionId));
            return button;
        }

        private void ApplyRestActionIcons()
        {
            ApplyRestActionIcon(restAskMoodIconImage, "rest.ask_mood");
            ApplyRestActionIcon(restTrainIconImage, "rest.train");
            ApplyRestActionIcon(restRecoverIconImage, "rest.recover");
        }

        private void ApplyRestActionIcon(Image image, string actionId)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = ResolveRestActionIcon(actionId);
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            ApplyRestActionCardState(image, actionId);
        }

        private void UpdateRestActionCardStates()
        {
            ApplyRestActionCardState(restAskMoodIconImage, "rest.ask_mood");
            ApplyRestActionCardState(restTrainIconImage, "rest.train");
            ApplyRestActionCardState(restRecoverIconImage, "rest.recover");
        }

        private void ApplyRestActionCardState(Image image, string actionId)
        {
            if (image == null)
            {
                return;
            }

            if (image.sprite == null)
            {
                image.color = new Color(0.12f, 0.16f, 0.19f, 0.98f);
                return;
            }

            var hasSelection = !string.IsNullOrEmpty(_pendingRestActionId);
            var selected = hasSelection && _pendingRestActionId == actionId;
            image.color = !hasSelection || selected ? Color.white : new Color(0.70f, 0.76f, 0.80f, 0.88f);
        }

        private Sprite ResolveRestActionIcon(string actionId)
        {
            if (presentationData != null && presentationData.TryGetRestActionIcon(actionId, out var icon))
            {
                return icon;
            }

            return null;
        }

        private static string BuildRestActionCardLabel(string actionId)
        {
            return ResolveRestActionTitle(actionId) + "\n" + ResolveRestActionPreview(actionId);
        }

        private static string ResolveRestActionTitle(string actionId)
        {
            return actionId switch
            {
                "rest.ask_mood" => "대화",
                "rest.train" => "훈련",
                "rest.recover" => "휴식",
                _ => "행동"
            };
        }

        private static string ResolveRestActionPreview(string actionId)
        {
            return actionId switch
            {
                "rest.ask_mood" => "마타이오스와 대화",
                "rest.train" => "다음 전투 피해 +1",
                "rest.recover" => "HP 회복",
                _ => "선택"
            };
        }

        private static string ResolveButtonLabel(Button button)
        {
            var label = button == null ? null : button.GetComponentInChildren<Text>();
            return label == null ? string.Empty : label.text;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
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
            image.raycastTarget = false;
            return rect;
        }

        private Button CreateRestButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax)
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

            var text = CreateHudText(name + " Text", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, BodyFontSize, TextAnchor.MiddleCenter, PrimaryTextColor);
            text.lineSpacing = DenseLineSpacing;
            text.transform.SetParent(buttonObject.transform, false);
            text.text = label;
            return button;
        }

        private InputField CreateRestInputField(Transform parent)
        {
            var inputObject = new GameObject("Rest Utterance Input");
            inputObject.transform.SetParent(parent, false);
            var rect = inputObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.06f, 0.535f);
            rect.anchorMax = new Vector2(0.94f, 0.705f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = inputObject.AddComponent<Image>();
            image.color = new Color(0.96f, 0.985f, 0.98f, 1f);
            var input = inputObject.AddComponent<InputField>();
            input.targetGraphic = image;

            var text = CreateHudText("Rest Input Text", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, RestInputFontSize, TextAnchor.MiddleLeft, new Color(0.05f, 0.07f, 0.08f, 1f));
            text.transform.SetParent(inputObject.transform, false);
            text.GetComponent<RectTransform>().offsetMin = new Vector2(18f, 0f);
            text.GetComponent<RectTransform>().offsetMax = new Vector2(-18f, 0f);
            input.textComponent = text;

            var placeholder = CreateHudText("Rest Input Placeholder", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, RestBodyFontSize, TextAnchor.MiddleLeft, new Color(0.16f, 0.22f, 0.24f, 1f));
            placeholder.transform.SetParent(inputObject.transform, false);
            placeholder.GetComponent<RectTransform>().offsetMin = new Vector2(18f, 0f);
            placeholder.GetComponent<RectTransform>().offsetMax = new Vector2(-18f, 0f);
            placeholder.text = "마타이오스에게 전할 말";
            input.placeholder = placeholder;
            return input;
        }

        private void SelectRestAction(string actionId)
        {
            _pendingRestActionId = actionId;
            UpdateRestActionCardStates();
            SetRestActionCardsVisible(false);
            SetRestInputPhaseVisible(true, false);

            if (restResponseText != null)
            {
                restResponseText.text = string.Empty;
            }
        }

        private void SubmitRestInteraction()
        {
            if (_roomController == null || string.IsNullOrEmpty(_pendingRestActionId))
            {
                if (restResponseText != null)
                {
                    restResponseText.text = "행동을 먼저 선택하세요";
                }
                SetRestResponseVisible(true);
                return;
            }

            var utterance = restInputField == null ? string.Empty : restInputField.text;
            var resolution = _roomController.ResolveCurrentRouteRestInteraction(_pendingRestSelection, _pendingRestActionId, utterance);
            if (resolution.Message.Contains("input required", StringComparison.Ordinal))
            {
                if (restResponseText != null)
                {
                    restResponseText.text = "말을 입력해야 합니다";
                }
                SetRestResponseVisible(true);
                ShowResult(resolution);
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            SetRestActionButtonsInteractable(false);
            SetRestActionCardsVisible(false);
            if (restInputField != null)
            {
                restInputField.interactable = false;
                restInputField.gameObject.SetActive(false);
            }

            if (restSubmitButton != null)
            {
                restSubmitButton.interactable = false;
                restSubmitButton.gameObject.SetActive(false);
            }

            if (restContinueButton != null)
            {
                restContinueButton.gameObject.SetActive(true);
                restContinueButton.interactable = true;
            }

            if (restResponseText != null)
            {
                var response = _roomController.RunState == null ? string.Empty : _roomController.RunState.LastMataiosResponse;
                restResponseText.text = BuildRestCommittedMessage(_pendingRestActionId, response);
            }

            SetRestResponseVisible(true);
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private static string BuildRestCommittedMessage(string actionId, string response)
        {
            var effect = actionId switch
            {
                "rest.ask_mood" => "결과: 신뢰 +2",
                "rest.train" => "결과: 다음 전투 피해 +1",
                "rest.recover" => "결과: HP 회복",
                _ => "결과: 완료"
            };
            var line = string.IsNullOrEmpty(response) ? "마타이오스 응답 준비 완료" : response;
            return line + "\n" + effect;
        }

        private void ContinueAfterRestInteraction()
        {
            HideRestInteractionPanel();
            if (_roomController != null)
            {
                ShowRunState(_roomController.GetSnapshot());
            }
        }

        private void HideRestInteractionPanel()
        {
            if (restInteractionPanel != null)
            {
                restInteractionPanel.gameObject.SetActive(false);
            }

            SetRestActionCardsVisible(true);
            SetRestInputPhaseVisible(false, false);
            HideNpcSpotlight();
        }

        private void SetRestActionButtonsInteractable(bool interactable)
        {
            if (restAskMoodButton != null)
            {
                restAskMoodButton.interactable = interactable;
            }

            if (restTrainButton != null)
            {
                restTrainButton.interactable = interactable;
            }

            if (restRecoverButton != null)
            {
                restRecoverButton.interactable = interactable;
            }
        }

        private void SetRestActionCardsVisible(bool visible)
        {
            SetButtonVisible(restAskMoodButton, visible);
            SetButtonVisible(restTrainButton, visible);
            SetButtonVisible(restRecoverButton, visible);
        }

        private void SetRestInputPhaseVisible(bool inputVisible, bool committed)
        {
            if (restInputField != null)
            {
                restInputField.gameObject.SetActive(inputVisible && !committed);
                restInputField.interactable = inputVisible && !committed;
            }

            if (restSubmitButton != null)
            {
                restSubmitButton.gameObject.SetActive(inputVisible && !committed);
                restSubmitButton.interactable = inputVisible && !committed;
            }

            if (restContinueButton != null)
            {
                restContinueButton.gameObject.SetActive(committed);
                restContinueButton.interactable = committed;
            }

            SetRestResponseVisible(committed);
        }

        private void SetRestResponseVisible(bool visible)
        {
            if (restResponsePanel != null)
            {
                restResponsePanel.gameObject.SetActive(visible);
                return;
            }

            if (restResponseText != null && restResponseText.transform.parent != null)
            {
                restResponseText.transform.parent.gameObject.SetActive(visible);
            }
        }

        private static void SetButtonVisible(Button button, bool visible)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
            }
        }

        private void EnsureResultText()
        {
            if (resultText != null)
            {
                return;
            }

            var resultObject = new GameObject("Encounter Result Text");
            resultObject.transform.SetParent(HudParent, false);

            var rect = resultObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.34f, 0f);
            rect.anchorMax = new Vector2(0.94f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, 170f);
            rect.anchoredPosition = new Vector2(0f, 650f);

            resultText = resultObject.AddComponent<Text>();
            resultText.font = ResolveFont();
            resultText.fontSize = ResultFontSize;
            resultText.alignment = TextAnchor.MiddleCenter;
            resultText.horizontalOverflow = HorizontalWrapMode.Wrap;
            resultText.verticalOverflow = VerticalWrapMode.Truncate;
            resultText.resizeTextForBestFit = true;
            resultText.resizeTextMinSize = CaptionFontSize;
            resultText.resizeTextMaxSize = ResultFontSize;
            resultText.supportRichText = false;
            resultText.color = ResultTextColor;
            resultText.lineSpacing = DenseLineSpacing;
            NormalizeLayout();
        }

        private void EnsureRouteText()
        {
            if (routeText != null)
            {
                return;
            }

            routeText = CreateHudText("Demo Route Text", new Vector2(0.06f, 1f), new Vector2(0.94f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -330f), new Vector2(0f, 112f), 28, TextAnchor.UpperCenter, new Color(0.88f, 0.94f, 0.98f, 1f));
        }

        private void EnsureMemoryText()
        {
            if (memoryText != null)
            {
                return;
            }

            memoryText = CreateHudText("Memory Combat Text", new Vector2(0.31f, 0.382f), new Vector2(0.92f, 0.478f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, CaptionFontSize, TextAnchor.MiddleLeft, new Color(0.82f, 0.89f, 0.92f, 1f));
            memoryText.lineSpacing = 0.88f;
        }

        private void EnsureDemoCompleteText()
        {
            if (demoCompleteText != null)
            {
                return;
            }

            demoCompleteText = CreateHudText("Demo Complete Text", new Vector2(0.12f, 0.5f), new Vector2(0.88f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0f, 150f), 42, TextAnchor.MiddleCenter, new Color(0.94f, 0.98f, 0.82f, 1f));
        }

        private void ApplyPortrait()
        {
            EnsurePortraitImage();
            if (npcPortraitImage == null)
            {
                return;
            }

            npcPortraitImage.sprite = mataiosPortrait;
            npcPortraitImage.preserveAspect = true;
            npcPortraitImage.gameObject.SetActive(mataiosPortrait != null);
        }

        private void EnsurePortraitImage()
        {
            if (npcPortraitImage != null)
            {
                return;
            }

            var portraitObject = new GameObject("Mataios Portrait");
            portraitObject.transform.SetParent(HudParent, false);

            var rect = portraitObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.06f, 0f);
            rect.anchorMax = new Vector2(0.30f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 720f);
            rect.sizeDelta = new Vector2(0f, 360f);

            npcPortraitImage = portraitObject.AddComponent<Image>();
            npcPortraitImage.color = Color.white;
            npcPortraitImage.raycastTarget = false;
        }

        private void EnsureEncounterBackgroundImage()
        {
            if (encounterBackgroundImage != null)
            {
                return;
            }

            var backgroundObject = new GameObject("Encounter Background");
            backgroundObject.transform.SetParent(HudParent, false);
            backgroundObject.transform.SetAsFirstSibling();

            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            encounterBackgroundImage = backgroundObject.AddComponent<Image>();
            encounterBackgroundImage.color = new Color(1f, 1f, 1f, 0.72f);
            encounterBackgroundImage.preserveAspect = false;
            encounterBackgroundImage.raycastTarget = false;
            encounterBackgroundImage.gameObject.SetActive(false);
        }

        private void EnsureNpcSpotlightLayer()
        {
            if (npcSpotlightLayer != null)
            {
                return;
            }

            EnsureScreenLayers();
            var layerObject = new GameObject("NPC Spotlight Layer");
            layerObject.transform.SetParent(visualLayer == null ? HudParent : visualLayer, false);
            npcSpotlightLayer = layerObject.AddComponent<RectTransform>();
            npcSpotlightLayer.anchorMin = Vector2.zero;
            npcSpotlightLayer.anchorMax = Vector2.one;
            npcSpotlightLayer.offsetMin = Vector2.zero;
            npcSpotlightLayer.offsetMax = Vector2.zero;

            npcSpotlightBackdropImage = CreateCombatImage(npcSpotlightLayer, "NPC Spotlight Backdrop", Vector2.zero, Vector2.one);
            npcSpotlightBackdropImage.sprite = null;
            npcSpotlightBackdropImage.color = new Color(0f, 0f, 0f, 0.34f);
            npcSpotlightBackdropImage.gameObject.SetActive(true);

            npcSpotlightGlowImage = CreateCombatImage(npcSpotlightLayer, "NPC Spotlight Glow", new Vector2(0.01f, 0.08f), new Vector2(0.52f, 0.98f));
            npcSpotlightGlowImage.sprite = presentationData == null ? null : presentationData.SpotlightGradient;
            npcSpotlightGlowImage.preserveAspect = true;
            npcSpotlightGlowImage.color = npcSpotlightGlowImage.sprite == null ? new Color(0.52f, 0.72f, 0.62f, 0.22f) : new Color(0.86f, 0.96f, 0.86f, 0.72f);
            npcSpotlightGlowImage.gameObject.SetActive(true);

            npcSpotlightShadowImage = CreateCombatImage(npcSpotlightLayer, "NPC Spotlight Shadow", new Vector2(0.04f, 0.05f), new Vector2(0.48f, 0.88f));
            npcSpotlightShadowImage.sprite = presentationData == null ? null : presentationData.SpotlightGradient;
            npcSpotlightShadowImage.preserveAspect = true;
            npcSpotlightShadowImage.color = npcSpotlightShadowImage.sprite == null ? new Color(0f, 0f, 0f, 0.42f) : new Color(0f, 0f, 0f, 0.36f);
            npcSpotlightShadowImage.gameObject.SetActive(true);

            var merchantObject = new GameObject("Merchant Visual");
            merchantObject.transform.SetParent(npcSpotlightLayer, false);

            var rect = merchantObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, -0.05f);
            rect.anchorMax = new Vector2(0.48f, 0.96f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            merchantVisualImage = merchantObject.AddComponent<Image>();
            merchantVisualImage.color = new Color(1f, 1f, 1f, 0.98f);
            merchantVisualImage.preserveAspect = true;
            merchantVisualImage.raycastTarget = false;
            merchantVisualImage.gameObject.SetActive(false);

            var plate = CreatePanel("NPC Dialogue Plate", npcSpotlightLayer, new Vector2(0.04f, 0.05f), new Vector2(0.49f, 0.28f), new Color(0.03f, 0.045f, 0.050f, 0.78f));
            npcDialoguePlateImage = plate.GetComponent<Image>();
            if (npcDialoguePlateImage != null && presentationData != null && presentationData.NpcDialoguePlate != null)
            {
                npcDialoguePlateImage.sprite = presentationData.NpcDialoguePlate;
                npcDialoguePlateImage.preserveAspect = true;
                npcDialoguePlateImage.color = Color.white;
            }

            npcSpotlightNameText = CreateCombatChildText(plate, "NPC Spotlight Name", new Vector2(0.06f, 0.60f), new Vector2(0.94f, 0.92f), 25, TextAnchor.MiddleLeft);
            npcSpotlightNameText.color = new Color(0.91f, 0.98f, 0.91f, 1f);
            npcSpotlightDialogueText = CreateCombatChildText(plate, "NPC Spotlight Dialogue", new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.58f), 23, TextAnchor.MiddleLeft);
            npcSpotlightDialogueText.color = new Color(0.84f, 0.91f, 0.89f, 1f);
            npcSpotlightLayer.gameObject.SetActive(false);
        }

        private void EnsureMerchantVisualImage()
        {
            EnsureNpcSpotlightLayer();
        }

        private void ApplyMerchantPresentation(int floor)
        {
            EnsureMerchantVisualImage();
            if (merchantVisualImage == null)
            {
                return;
            }

            var sprite = presentationData != null && presentationData.TryGetMerchantSprite(floor, out var merchantSprite) ? merchantSprite : mataiosPortrait;
            ShowNpcSpotlight(sprite, "상인", ResolveMerchantSpotlightLine(floor), NpcSpotlightMode.Shop);
        }

        private void HideMerchantPresentation()
        {
            HideNpcSpotlight();
        }

        private void ShowNpcSpotlight(Sprite sprite, string displayName, string dialogue, NpcSpotlightMode mode)
        {
            EnsureNpcSpotlightLayer();
            SetLayerVisible(visualLayer, true);
            _npcSpotlightModeLabel = ResolveNpcSpotlightModeLabel(mode);
            if (npcSpotlightLayer != null)
            {
                npcSpotlightLayer.gameObject.SetActive(sprite != null);
            }

            if (merchantVisualImage != null)
            {
                merchantVisualImage.sprite = sprite;
                merchantVisualImage.gameObject.SetActive(sprite != null);
            }

            ApplyNpcSpotlightModeLayout(mode);
            if (npcSpotlightNameText != null)
            {
                npcSpotlightNameText.text = displayName;
            }

            if (npcSpotlightDialogueText != null)
            {
                npcSpotlightDialogueText.text = dialogue;
            }
        }

        private void HideNpcSpotlight()
        {
            _npcSpotlightModeLabel = string.Empty;
            if (npcSpotlightLayer != null)
            {
                npcSpotlightLayer.gameObject.SetActive(false);
            }
        }

        private void HideLegacyEncounterVisuals(bool hideBackground)
        {
            if (npcPortraitImage != null)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            if (hideBackground && encounterBackgroundImage != null)
            {
                encounterBackgroundImage.gameObject.SetActive(false);
            }
        }

        private void ApplyNpcSpotlightModeLayout(NpcSpotlightMode mode)
        {
            if (npcSpotlightLayer == null || merchantVisualImage == null)
            {
                return;
            }

            var characterRect = merchantVisualImage.GetComponent<RectTransform>();
            var plateRect = npcSpotlightNameText == null ? null : npcSpotlightNameText.transform.parent.GetComponent<RectTransform>();
            if (mode == NpcSpotlightMode.Shop)
            {
                ApplyRect(characterRect, new Vector2(0.02f, -0.08f), new Vector2(0.43f, 0.99f));
                ApplyRect(plateRect, new Vector2(0.04f, 0.05f), new Vector2(0.44f, 0.29f));
                ApplyRect(npcSpotlightGlowImage == null ? null : npcSpotlightGlowImage.GetComponent<RectTransform>(), new Vector2(0.00f, 0.05f), new Vector2(0.47f, 0.98f));
                ApplyRect(npcSpotlightShadowImage == null ? null : npcSpotlightShadowImage.GetComponent<RectTransform>(), new Vector2(0.03f, 0.00f), new Vector2(0.45f, 0.88f));
                return;
            }

            if (mode == NpcSpotlightMode.Rest)
            {
                ApplyRect(characterRect, new Vector2(0.04f, -0.12f), new Vector2(0.42f, 0.96f));
                ApplyRect(plateRect, new Vector2(0.08f, 0.06f), new Vector2(0.52f, 0.28f));
                ApplyRect(npcSpotlightGlowImage == null ? null : npcSpotlightGlowImage.GetComponent<RectTransform>(), new Vector2(0.02f, 0.04f), new Vector2(0.48f, 0.98f));
                ApplyRect(npcSpotlightShadowImage == null ? null : npcSpotlightShadowImage.GetComponent<RectTransform>(), new Vector2(0.05f, 0.00f), new Vector2(0.44f, 0.88f));
                return;
            }

            ApplyRect(characterRect, new Vector2(0.04f, -0.06f), new Vector2(0.42f, 0.94f));
            ApplyRect(plateRect, new Vector2(0.07f, 0.06f), new Vector2(0.50f, 0.27f));
        }

        private static void ApplyRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static string ResolveNpcSpotlightModeLabel(NpcSpotlightMode mode)
        {
            return mode switch
            {
                NpcSpotlightMode.Shop => "상점",
                NpcSpotlightMode.Rest => "휴식",
                NpcSpotlightMode.Event => "이벤트",
                _ => string.Empty
            };
        }

        private static string ResolveMerchantSpotlightLine(int floor)
        {
            return floor >= 4
                ? "필요한 걸 골라. 오래 머무를 곳은 아니니까."
                : "보스 전에 필요한 준비를 끝내세요.";
        }

        private void EnsureCombatPanel()
        {
            if (combatPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Combat Panel");
            panelObject.transform.SetParent(HudParent, false);

            combatPanel = panelObject.AddComponent<RectTransform>();
            combatPanel.anchorMin = new Vector2(0.06f, 0.035f);
            combatPanel.anchorMax = new Vector2(0.94f, 0.895f);
            combatPanel.pivot = new Vector2(0.5f, 0.5f);
            combatPanel.anchoredPosition = Vector2.zero;
            combatPanel.sizeDelta = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.025f, 0.032f, 0.040f, 0.92f);

            combatEnemyStage = CreateCombatPanelRect(panelObject.transform, "Combat Enemy Stage", new Vector2(0.04f, 0.515f), new Vector2(0.96f, 0.985f), new Color(0.02f, 0.028f, 0.035f, 0.78f));
            combatEnemyImage = CreateCombatImage(combatEnemyStage, "Combat Enemy Image", new Vector2(0.05f, 0.03f), new Vector2(0.95f, 0.76f));
            combatEnemyTitleText = CreateCombatChildText(combatEnemyStage, "Combat Enemy Title", new Vector2(0.06f, 0.83f), new Vector2(0.94f, 0.97f), 32, TextAnchor.MiddleCenter);
            enemyHpFill = CreateHpBar(combatEnemyStage, "Enemy HP Bar", new Vector2(0.07f, 0.765f), new Vector2(0.93f, 0.815f), new Color(0.84f, 0.24f, 0.24f, 1f));

            combatLogPanel = CreateCombatPanelRect(panelObject.transform, "Combat Log Panel", new Vector2(0.04f, 0.345f), new Vector2(0.96f, 0.505f), new Color(0.02f, 0.025f, 0.030f, 0.84f));
            combatText = CreateCombatChildText(combatLogPanel, "Combat Status Text", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.92f), CombatBodyFontSize, TextAnchor.MiddleLeft);
            combatText.lineSpacing = DenseLineSpacing;

            combatPartyDock = CreateCombatPanelRect(panelObject.transform, "Combat Party Dock", new Vector2(0.04f, 0.025f), new Vector2(0.96f, 0.33f), new Color(0.018f, 0.022f, 0.028f, 0.94f));
            combatPlayerCard = CreateCombatPanelRect(combatPartyDock, "Combat Player Card", new Vector2(0.04f, 0.43f), new Vector2(0.48f, 0.94f), new Color(0.055f, 0.072f, 0.085f, 0.96f));
            combatMataiosCard = CreateCombatPanelRect(combatPartyDock, "Combat Mataios Card", new Vector2(0.52f, 0.43f), new Vector2(0.96f, 0.94f), new Color(0.055f, 0.064f, 0.083f, 0.96f));
            combatPlayerPortraitImage = CreateCombatPortraitBox(combatPlayerCard, "Combat Player Portrait", new Vector2(0.04f, 0.22f), new Vector2(0.30f, 0.88f), new Color(0.15f, 0.19f, 0.22f, 1f), "P", out combatPlayerPortraitFallbackText);
            combatMataiosPortraitImage = CreateCombatPortraitBox(combatMataiosCard, "Combat Mataios Portrait", new Vector2(0.04f, 0.16f), new Vector2(0.31f, 0.90f), new Color(0.12f, 0.14f, 0.18f, 1f), string.Empty, out _);
            combatPlayerPortraitFrameImage = CreateCombatPortraitFrame(combatPlayerPortraitImage, "Combat Player Portrait Frame");
            combatMataiosPortraitFrameImage = CreateCombatPortraitFrame(combatMataiosPortraitImage, "Combat Mataios Portrait Frame");
            combatPlayerCardText = CreateCombatChildText(combatPlayerCard, "Combat Player Card Text", new Vector2(0.34f, 0.18f), new Vector2(0.96f, 0.92f), 24, TextAnchor.MiddleLeft);
            combatMataiosCardText = CreateCombatChildText(combatMataiosCard, "Combat Mataios Card Text", new Vector2(0.35f, 0.18f), new Vector2(0.96f, 0.92f), 24, TextAnchor.MiddleLeft);
            combatTrainingStatusIconImage = CreateCombatImage(combatPlayerCard, "Combat Training Status Icon", new Vector2(0.76f, 0.72f), new Vector2(0.84f, 0.90f));
            combatBandageStatusIconImage = CreateCombatImage(combatPlayerCard, "Combat Bandage Status Icon", new Vector2(0.84f, 0.72f), new Vector2(0.92f, 0.90f));
            combatRecallStatusIconImage = CreateCombatImage(combatPlayerCard, "Combat Recall Status Icon", new Vector2(0.68f, 0.72f), new Vector2(0.76f, 0.90f));
            playerHpFill = CreateHpBar(combatPlayerCard, "Player HP Bar", new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.15f), new Color(0.30f, 0.78f, 0.50f, 1f));

            attackButton = CreateCombatButton(combatPartyDock, "Combat Button Attack", "공격", new Vector2(0.22f, 0.045f), CombatAction.Attack, out attackActionIconImage);
            defendButton = CreateCombatButton(combatPartyDock, "Combat Button Defend", "방어", new Vector2(0.50f, 0.045f), CombatAction.Defend, out defendActionIconImage);
            skillButton = CreateCombatButton(combatPartyDock, "Combat Button Skill", "정찰", new Vector2(0.78f, 0.045f), CombatAction.Skill, out skillActionIconImage);
            skillButton.interactable = false;
            ApplyCombatStatusIconSprites();
        }

        private RectTransform CreateCombatPanelRect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
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
            image.raycastTarget = false;
            return rect;
        }

        private Text CreateCombatChildText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor alignment)
        {
            var text = CreateHudText(name, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, fontSize, alignment, PrimaryTextColor);
            text.transform.SetParent(parent, false);
            var rect = text.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            text.lineSpacing = DenseLineSpacing;
            return text;
        }

        private Image CreateCombatPortraitBox(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color, string fallbackLabel, out Text fallbackText)
        {
            fallbackText = null;
            var portraitObject = new GameObject(name);
            portraitObject.transform.SetParent(parent, false);

            var rect = portraitObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = portraitObject.AddComponent<Image>();
            image.color = color;
            image.preserveAspect = true;
            image.raycastTarget = false;

            if (!string.IsNullOrEmpty(fallbackLabel))
            {
                var label = CreateCombatChildText(portraitObject.transform, name + " Label", Vector2.zero, Vector2.one, 34, TextAnchor.MiddleCenter);
                label.text = fallbackLabel;
                label.color = new Color(0.78f, 0.86f, 0.88f, 1f);
                fallbackText = label;
            }

            return image;
        }

        private Image CreateCombatPortraitFrame(Image portraitImage, string name)
        {
            if (portraitImage == null)
            {
                return null;
            }

            var frameObject = new GameObject(name);
            frameObject.transform.SetParent(portraitImage.transform.parent, false);

            var sourceRect = portraitImage.GetComponent<RectTransform>();
            var rect = frameObject.AddComponent<RectTransform>();
            rect.anchorMin = sourceRect.anchorMin;
            rect.anchorMax = sourceRect.anchorMax;
            rect.offsetMin = sourceRect.offsetMin;
            rect.offsetMax = sourceRect.offsetMax;

            var image = frameObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.gameObject.SetActive(false);
            return image;
        }

        private Image CreateCombatImage(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);

            var rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = imageObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.96f);
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.gameObject.SetActive(false);
            return image;
        }

        private Image CreateHpBar(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color fillColor)
        {
            var frameObject = new GameObject(name + " Frame");
            frameObject.transform.SetParent(parent, false);

            var frameRect = frameObject.AddComponent<RectTransform>();
            frameRect.anchorMin = anchorMin;
            frameRect.anchorMax = anchorMax;
            frameRect.offsetMin = Vector2.zero;
            frameRect.offsetMax = Vector2.zero;

            var frame = frameObject.AddComponent<Image>();
            frame.color = new Color(0.02f, 0.025f, 0.03f, 0.92f);
            frame.raycastTarget = false;

            var fillObject = new GameObject(name + " Fill");
            fillObject.transform.SetParent(frameObject.transform, false);
            var fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0.02f, 0.18f);
            fillRect.anchorMax = new Vector2(0.98f, 0.82f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var fill = fillObject.AddComponent<Image>();
            fill.color = fillColor;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 1f;
            fill.raycastTarget = false;
            return fill;
        }

        private Button CreateCombatButton(Transform parent, string name, string labelText, Vector2 anchor, CombatAction action, out Image iconImage)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(CombatActionButtonSize, CombatActionButtonSize);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.17f, 0.20f, 0.98f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => ResolveCombatAction(action));

            var iconObject = new GameObject("Icon");
            iconObject.transform.SetParent(buttonObject.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.24f, 0.42f);
            iconRect.anchorMax = new Vector2(0.76f, 0.90f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            iconImage = iconObject.AddComponent<Image>();
            iconImage.color = new Color(0.92f, 0.86f, 0.68f, 0.96f);
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0.04f);
            labelRect.anchorMax = new Vector2(1f, 0.36f);
            labelRect.offsetMin = new Vector2(8f, 4f);
            labelRect.offsetMax = new Vector2(-8f, -4f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 28;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 22;
            label.resizeTextMaxSize = 28;
            label.raycastTarget = false;
            label.color = new Color(0.94f, 0.97f, 0.98f, 1f);
            label.text = labelText;
            return button;
        }

        private void EnsureNextFloorButton()
        {
            if (nextFloorButton != null)
            {
                return;
            }

            var buttonObject = new GameObject("Next Floor Button");
            buttonObject.transform.SetParent(HudParent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0f);
            rect.anchorMax = new Vector2(0.72f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 218f);
            rect.sizeDelta = new Vector2(0f, 104f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.17f, 0.21f, 0.98f);

            nextFloorButton = buttonObject.AddComponent<Button>();
            nextFloorButton.targetGraphic = image;
            nextFloorButton.onClick.AddListener(ResolveNextFloor);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-16f, -8f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 32;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 24;
            label.resizeTextMaxSize = 32;
            label.raycastTarget = false;
            label.color = new Color(0.95f, 0.98f, 0.99f, 1f);
            label.text = "다음 층";
            buttonObject.SetActive(false);
        }

        private void UpdateNextFloorButton(PrototypeRunSnapshot snapshot)
        {
            EnsureNextFloorButton();
            if (nextFloorButton == null)
            {
                return;
            }

            nextFloorButton.gameObject.SetActive(snapshot.StairUnlocked && !snapshot.IsInCombat && !snapshot.RunCompleted);
            nextFloorButton.interactable = nextFloorButton.gameObject.activeSelf && _roomController != null;
        }

        private void EnsureRouteActionButton()
        {
            if (routeActionButton != null)
            {
                return;
            }

            var buttonObject = new GameObject("Route Action Button");
            buttonObject.transform.SetParent(HudParent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0f);
            rect.anchorMax = new Vector2(0.72f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 218f);
            rect.sizeDelta = new Vector2(0f, 104f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.10f, 0.16f, 0.20f, 0.98f);

            routeActionButton = buttonObject.AddComponent<Button>();
            routeActionButton.targetGraphic = image;
            routeActionButton.onClick.AddListener(OpenCurrentRouteStep);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-16f, -8f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 32;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 24;
            label.resizeTextMaxSize = 32;
            label.raycastTarget = false;
            label.color = new Color(0.95f, 0.98f, 0.99f, 1f);
            label.text = "진행";
            buttonObject.SetActive(false);
        }

        private void UpdateRouteActionButton(PrototypeRunSnapshot snapshot)
        {
            EnsureRouteActionButton();
            if (routeActionButton == null)
            {
                return;
            }

            var visible = _roomController != null &&
                !snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !snapshot.StairUnlocked &&
                !string.IsNullOrEmpty(snapshot.NextDemoEncounterId) &&
                _choiceButtons.Count == 0 &&
                !RestInteractionPanelVisible;
            SetButtonLabel(routeActionButton, showRawDebugText ? "Open route step" : "다음 조우");
            routeActionButton.gameObject.SetActive(visible);
            routeActionButton.interactable = visible;
        }

        private void EnsureRestartButton()
        {
            if (restartButton != null)
            {
                return;
            }

            var buttonObject = new GameObject("Restart Run Button");
            buttonObject.transform.SetParent(HudParent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0f);
            rect.anchorMax = new Vector2(0.72f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 218f);
            rect.sizeDelta = new Vector2(0f, 104f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.17f, 0.21f, 0.98f);

            restartButton = buttonObject.AddComponent<Button>();
            restartButton.targetGraphic = image;
            restartButton.onClick.AddListener(ResolveRestart);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-16f, -8f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 32;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 24;
            label.resizeTextMaxSize = 32;
            label.raycastTarget = false;
            label.color = new Color(0.95f, 0.98f, 0.99f, 1f);
            label.text = "다시 시작";
            buttonObject.SetActive(false);
        }

        private void UpdateRestartButton(PrototypeRunSnapshot snapshot)
        {
            EnsureRestartButton();
            if (restartButton == null)
            {
                return;
            }

            restartButton.gameObject.SetActive(snapshot.RestartReady && !snapshot.IsInCombat);
            restartButton.interactable = restartButton.gameObject.activeSelf && _roomController != null;
        }

        private void EnsureEndingButtons()
        {
            if (endingRestButton == null)
            {
                endingRestButton = CreateEndingButton("Ending Button Rest", showRawDebugText ? "PLACEHOLDER_ENDING_REST" : "안식", new Vector2(-205f, 218f), ResolveEndingRest);
            }

            if (endingContinueButton == null)
            {
                endingContinueButton = CreateEndingButton("Ending Button Continue", showRawDebugText ? "PLACEHOLDER_ENDING_CONTINUE" : "동행 계속", new Vector2(205f, 218f), ResolveEndingContinue);
            }
        }

        private Button CreateEndingButton(string name, string labelText, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(HudParent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(370f, 104f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.11f, 0.15f, 0.19f, 0.98f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-16f, -8f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 30;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 22;
            label.resizeTextMaxSize = 30;
            label.raycastTarget = false;
            label.color = new Color(0.95f, 0.98f, 0.99f, 1f);
            label.text = labelText;
            buttonObject.SetActive(false);
            return button;
        }

        private void UpdateEndingButtons(PrototypeRunSnapshot snapshot)
        {
            EnsureEndingButtons();
            var visible = snapshot.EndingChoicePending && !snapshot.IsInCombat;
            if (endingRestButton != null)
            {
                SetButtonLabel(endingRestButton, showRawDebugText ? "PLACEHOLDER_ENDING_REST" : "안식");
                endingRestButton.gameObject.SetActive(visible);
                endingRestButton.interactable = visible && _roomController != null;
            }

            if (endingContinueButton != null)
            {
                SetButtonLabel(endingContinueButton, showRawDebugText ? "PLACEHOLDER_ENDING_CONTINUE" : "동행 계속");
                endingContinueButton.gameObject.SetActive(visible);
                endingContinueButton.interactable = visible && _roomController != null;
            }
        }

        private void ResolveNextFloor()
        {
            if (_roomController == null)
            {
                ShowResultMessage("next floor unavailable");
                return;
            }

            var resolution = _roomController.ResolveNextFloor();
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private void OpenCurrentRouteStep()
        {
            if (_roomController == null)
            {
                ShowResultMessage("route unavailable");
                return;
            }

            var snapshot = _roomController.GetSnapshot();
            if (snapshot.StairUnlocked && !snapshot.RunCompleted)
            {
                ResolveNextFloor();
                return;
            }

            if (snapshot.RunCompleted || snapshot.IsInCombat)
            {
                ShowRunState(snapshot);
                return;
            }

            var selectableMapNodes = _roomController.GetSelectableMapNodes();
            if (!snapshot.HasSelectedMapNode && selectableMapNodes.Length > 1)
            {
                ShowMapChoices(_roomController.GetFloorMapNodes(), mapNodeId =>
                {
                    var selected = _roomController.SelectMapNode(mapNodeId);
                    OpenSelectedRouteStep(selected);
                });
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            var selection = _roomController.SelectCurrentRouteEncounter();
            OpenSelectedRouteStep(selection);
        }

        private void OpenSelectedRouteStep(EncounterSelection selection)
        {
            if (_roomController == null)
            {
                ShowResultMessage("route unavailable");
                return;
            }

            if (!selection.HasEncounter)
            {
                ShowResultMessage(showRawDebugText ? "route unavailable" : "진행 없음");
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            if (_roomController.TryGetResolvedEncounterChoice(selection, out var resolvedChoiceStableId))
            {
                var resolution = new PrototypeNodeResolution(
                    selection.Node == null ? string.Empty : selection.Node.NodeId,
                    resolvedChoiceStableId,
                    "already resolved: " + resolvedChoiceStableId,
                    false);
                ShowResult(resolution);
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            if (selection.Encounter != null && selection.Encounter.Type == EncounterType.Rest)
            {
                ShowRestInteraction(selection);
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            if (_roomController.HasEncounterChoices(selection))
            {
                var views = _roomController.BuildEncounterChoiceViews(selection);
                ShowChoices(selection.Encounter, views, choiceStableId =>
                {
                    var resolution = _roomController.ResolveCurrentRouteChoice(selection, choiceStableId);
                    ShowResult(resolution);
                    ShowRunState(_roomController.GetSnapshot());
                });
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            var nodeResolution = _roomController.ResolveCurrentRouteNode(selection);
            ShowResult(nodeResolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private void ResolveRestart()
        {
            if (_roomController == null)
            {
                ShowResultMessage("restart unavailable");
                return;
            }

            var resolution = _roomController.RestartRun();
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private void ResolveEndingRest()
        {
            ResolveEndingChoice("PLACEHOLDER_ENDING_REST");
        }

        private void ResolveEndingContinue()
        {
            ResolveEndingChoice("PLACEHOLDER_ENDING_CONTINUE");
        }

        private void ResolveEndingChoice(string choiceStableId)
        {
            if (_roomController == null)
            {
                ShowResultMessage("ending unavailable");
                return;
            }

            var resolution = _roomController.ResolveEndingChoice(choiceStableId);
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private Text CreateHudText(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment, Color color)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(HudParent, false);

            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var text = textObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(18, fontSize - 8);
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
            text.raycastTarget = false;
            text.color = color;
            return text;
        }

        private static void SetButtonLabel(Button button, string labelText)
        {
            if (button == null)
            {
                return;
            }

            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.raycastTarget = false;
                label.text = labelText;
            }
        }

        private void UpdateRouteIndicator(PrototypeRunSnapshot snapshot)
        {
            EnsureRouteText();
            if (routeText == null)
            {
                return;
            }

            var restVisible = RestInteractionPanelVisible;
            var shopVisible = _shopPresentationActive && !snapshot.IsInCombat;
            routeText.gameObject.SetActive((!snapshot.IsInCombat && !_eventPresentationActive && !restVisible && !shopVisible) || showRawDebugText);
            if ((snapshot.IsInCombat || _eventPresentationActive || restVisible || shopVisible) && !showRawDebugText)
            {
                routeText.text = string.Empty;
                return;
            }

            if (_demoRouteLabels.Count == 0)
            {
                if (!showRawDebugText && snapshot.HasFloorMap)
                {
                    routeText.text = BuildPublicMapSummary(snapshot);
                    return;
                }

                routeText.text = showRawDebugText
                    ? (string.IsNullOrEmpty(snapshot.NextDemoEncounterId)
                    ? "route: " + snapshot.DemoStatus
                    : "route next: " + snapshot.NextDemoNodeId + "/" + snapshot.NextDemoEncounterId)
                    : "진행: " + (string.IsNullOrEmpty(snapshot.DemoStatus) ? "-" : ResolvePublicDemoStatus(snapshot));
                return;
            }

            var currentIndex = GetCurrentRouteIndex(snapshot);
            if (!showRawDebugText)
            {
                routeText.text = BuildPublicRouteSummary(snapshot, currentIndex);
                return;
            }

            var text = "route\n";
            for (var i = 0; i < _demoRouteLabels.Count; i++)
            {
                string marker;
                if ((snapshot.RunClear && !snapshot.IsInCombat) || i < currentIndex)
                {
                    marker = "[complete]";
                }
                else if (i == currentIndex)
                {
                    marker = "[current]";
                }
                else
                {
                    marker = "[locked]";
                }

                text += marker + " " + _demoRouteLabels[i];
                if (showRawDebugText && i < _demoRouteEncounterIds.Count)
                {
                    text += " | " + _demoRouteEncounterIds[i];
                }

                if (i < _demoRouteLabels.Count - 1)
                {
                    text += "\n";
                }
            }

            routeText.text = text;
        }

        private int GetCurrentRouteIndex(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                for (var i = _demoRouteEncounterIds.Count - 1; i >= 0; i--)
                {
                    if (IsCombatEncounterId(_demoRouteEncounterIds[i]))
                    {
                        return i;
                    }
                }
            }

            if (snapshot.RunClear)
            {
                return _demoRouteLabels.Count;
            }

            if (string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                return Mathf.Clamp(snapshot.DemoResolvedStepCount, 0, Mathf.Max(0, _demoRouteLabels.Count - 1));
            }

            for (var i = 0; i < _demoRouteLabels.Count; i++)
            {
                if (i < _demoRouteEncounterIds.Count && _demoRouteEncounterIds[i] == snapshot.NextDemoEncounterId)
                {
                    return i;
                }
            }

            return Mathf.Clamp(snapshot.DemoResolvedStepCount, 0, Mathf.Max(0, _demoRouteLabels.Count - 1));
        }

        private void UpdatePrimaryHeaderVisibility(PrototypeRunSnapshot snapshot)
        {
            if (interactionText == null || showRawDebugText)
            {
                return;
            }

            var hiddenByEncounterState = snapshot.IsInCombat ||
                _eventPresentationActive ||
                _shopPresentationActive ||
                RestInteractionPanelVisible ||
                IsMapSelectionVisible(snapshot);
            interactionText.gameObject.SetActive(!hiddenByEncounterState);
        }

        private string BuildPublicRouteSummary(PrototypeRunSnapshot snapshot, int currentIndex)
        {
            if (snapshot.IsInCombat)
            {
                return "목표\nFloor " + snapshot.CurrentFloor + " | 전투 중\n공격/방어로 적 HP를 줄이세요";
            }

            if (snapshot.RunClear)
            {
                if (snapshot.EndingRest)
                {
                    return "목표\n안식 선택 완료";
                }

                if (snapshot.EndingContinue)
                {
                    return "목표\n동행 계속 선택\n재시작 가능";
                }

                return "목표\n탑 정상\n안식 또는 동행 계속을 선택하세요";
            }

            if (snapshot.RunFailed)
            {
                return "목표\n실패\n재시작 가능";
            }

            if (snapshot.StairUnlocked)
            {
                return "목표\nFloor " + snapshot.CurrentFloor + " 완료\n다음 층으로 올라가세요";
            }

            if (snapshot.BossGateUnlocked)
            {
                return "목표\nFloor " + snapshot.CurrentFloor + " | 보스 관문\n승리하면 다음 단계가 열립니다";
            }

            if (snapshot.HasFloorMap)
            {
                return BuildPublicMapSummary(snapshot);
            }

            var label = currentIndex >= 0 && currentIndex < _demoRouteLabels.Count
                ? _demoRouteLabels[currentIndex]
                : ResolvePublicDemoStatus(snapshot);
            var step = Mathf.Clamp(currentIndex + 1, 1, Mathf.Max(1, _demoRouteLabels.Count));
            var remaining = Mathf.Max(0, _demoRouteLabels.Count - step);
            return "목표\nFloor " + snapshot.CurrentFloor + " | 현재: " + label + "\n진행 버튼으로 선택지를 엽니다 | 남은 단계 " + remaining;
        }

        private static string BuildPublicMapSummary(PrototypeRunSnapshot snapshot)
        {
            var selectable = 0;
            var completed = 0;
            var activeLayer = 0;
            var bossVisible = false;
            for (var i = 0; i < snapshot.FloorMapNodes.Length; i++)
            {
                var node = snapshot.FloorMapNodes[i];
                if (node.Selectable)
                {
                    selectable++;
                    activeLayer = node.Layer;
                }

                if (node.Completed)
                {
                    completed++;
                }

                if (node.Type == PrototypeFloorMapNodeType.Boss)
                {
                    bossVisible = true;
                }
            }

            var objective = snapshot.HasSelectedMapNode
                ? "선택한 노드 해결"
                : selectable > 1
                    ? "갈림길 선택"
                    : "다음 길 선택";
            var bossLayer = 0;
            for (var i = 0; i < snapshot.FloorMapNodes.Length; i++)
            {
                if (snapshot.FloorMapNodes[i].Type == PrototypeFloorMapNodeType.Boss)
                {
                    bossLayer = Mathf.Max(bossLayer, snapshot.FloorMapNodes[i].Layer);
                }
            }

            var stepsToBoss = bossLayer > 0 && activeLayer > 0 ? Mathf.Max(1, bossLayer - activeLayer + 1) : 0;
            var boss = bossVisible ? " | 보스까지 " + stepsToBoss + "번" : string.Empty;
            return "지도\nFloor " + snapshot.CurrentFloor + " | " + objective + boss + "\n밝은 노드를 선택하세요 | 완료 " + completed + "개";
        }

        private static string ResolvePublicDemoStatus(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                return "전투 중";
            }

            return snapshot.EndingRest ? "안식 선택 완료" :
                snapshot.EndingContinue ? "동행 계속 선택" :
                snapshot.RunClear ? "엔딩 선택" :
                snapshot.RunFailed ? "실패" :
                snapshot.StairUnlocked ? "다음 층 가능" :
                "준비";
        }

        private void UpdateMemoryAndCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureMemoryText();
            UpdateCombatPanel(snapshot);
            if (memoryText == null)
            {
                return;
            }

            if ((_eventPresentationActive || _shopPresentationActive || RestInteractionPanelVisible || IsMapSelectionVisible(snapshot)) && !showRawDebugText)
            {
                memoryText.gameObject.SetActive(false);
                memoryText.text = string.Empty;
                return;
            }

            string memory;
            string combat;
            if (showRawDebugText)
            {
                memory = string.IsNullOrEmpty(snapshot.LastMemoryFragmentId)
                    ? "memory: locked | count " + snapshot.MemoryFragmentCount
                    : "memory: unlocked " + snapshot.LastMemoryFragmentId + " | count " + snapshot.MemoryFragmentCount +
                      "\nkeys: " + BuildMemoryKeyLine(snapshot);
                combat = string.IsNullOrEmpty(snapshot.LastCombatId)
                    ? "combat: -"
                    : "combat: " + snapshot.LastCombatId + " | enemy " + snapshot.LastCombatEnemyId + " | " + snapshot.LastCombatResultId;
            }
            else
            {
                memory = string.IsNullOrEmpty(snapshot.LastMemoryFragmentId)
                    ? "기억: 대기"
                    : "기억: 해금";
                combat = string.IsNullOrEmpty(snapshot.LastCombatId)
                    ? "전투: -"
                    : "전투: " + BuildCombatOutcomeLabel(snapshot);
            }

            var npc = string.IsNullOrEmpty(snapshot.LastNpcReactionKey)
                ? "반응 없음"
                : ShortenPublicLine(ResolvePublicNpcReaction(snapshot.LastNpcReactionKey), 24);
            memoryText.text = BuildPlayerStatusLine(snapshot) + "\n" +
                BuildCompanionStatusLine(snapshot) + "\n" +
                memory + " | " + combat + " | " + npc;
        }

        private string BuildPlayerStatusLine(PrototypeRunSnapshot snapshot)
        {
            var buff = "버프 없음";
            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                buff = "훈련 피해 +1";
            }
            else if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0)
            {
                buff = "붕대 보유";
            }
            else if (_roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
            {
                buff = "회상 닻 보유";
            }

            return "플레이어 HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp +
                "  ATK " + snapshot.PlayerAttack +
                "  아이템 " + snapshot.ItemCount +
                "  능력 " + snapshot.AbilityCount +
                "  " + buff;
        }

        private static string BuildCompanionStatusLine(PrototypeRunSnapshot snapshot)
        {
            var affinity = snapshot.Affinity >= 4 ? "신뢰 높음" :
                snapshot.Affinity > 0 ? "신뢰 형성" :
                snapshot.Affinity < 0 ? "거리감" :
                "동행 중";
            return "마타이오스 " + affinity + "  기억 " + snapshot.MemoryFragmentCount + "  안정";
        }

        private static string ShortenPublicLine(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, Mathf.Max(1, maxLength - 1)).TrimEnd() + "…";
        }

        private void UpdateCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureCombatPanel();
            if (combatPanel == null)
            {
                return;
            }

            var hasCombat = snapshot.IsInCombat;
            if (hasCombat)
            {
                HideEventCutsceneLayout(restoreRouteText: false);
                HideMerchantPresentation();
            }

            combatPanel.gameObject.SetActive(hasCombat);
            if (memoryText != null)
            {
                memoryText.gameObject.SetActive(!hasCombat);
            }

            if (npcPortraitImage != null && hasCombat)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            if (interactionText != null && !showRawDebugText)
            {
                interactionText.gameObject.SetActive(!hasCombat &&
                    !_eventPresentationActive &&
                    !_shopPresentationActive &&
                    !RestInteractionPanelVisible &&
                    !IsMapSelectionVisible(snapshot));
            }

            if (routeText != null && hasCombat && !showRawDebugText)
            {
                routeText.gameObject.SetActive(false);
            }

            if (resultText != null && hasCombat)
            {
                resultText.gameObject.SetActive(false);
            }

            if (!hasCombat || combatText == null)
            {
                return;
            }

            combatText.text = showRawDebugText
                ? "combat: " + snapshot.LastCombatId + "\n" +
                  "enemy: " + snapshot.LastCombatEnemyId + " | HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + "\n" +
                  "player HP: " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + " | round " + snapshot.CombatRound + "\n" +
                  "last: " + (string.IsNullOrEmpty(snapshot.LastCombatRoundResult) ? "-" : snapshot.LastCombatRoundResult) + "\n" +
                  "result: " + snapshot.LastCombatResultId + " | gold " + snapshot.LastCombatGoldReward +
                  " | glitch " + FormatDelta(snapshot.LastCombatGlitchDelta) +
                  " | affinity " + FormatDelta(snapshot.LastCombatAffinityDelta) +
                  (snapshot.LastCombatComboDamage > 0 ? " | combo " + snapshot.LastCombatComboDamage : string.Empty)
                : BuildCombatPresentation(snapshot);

            UpdateCombatVisuals(snapshot);
            UpdatePartyDock(snapshot);
            UpdateCombatActionIcons();

            var canAct = snapshot.IsInCombat && _roomController != null && !IsCutscenePlaying();
            if (attackButton != null)
            {
                attackButton.interactable = canAct;
            }

            if (defendButton != null)
            {
                defendButton.interactable = canAct;
            }

            if (skillButton != null)
            {
                var scoutSkillReady = HasScoutSkill(snapshot);
                skillButton.interactable = canAct && scoutSkillReady;
                SetButtonLabel(skillButton, scoutSkillReady ? "정찰" : "정찰 잠김");
            }
        }

        private void UpdateCombatVisuals(PrototypeRunSnapshot snapshot)
        {
            var slot = ResolveCurrentPresentationSlot(snapshot);
            var enemySlot = ResolveEnemyPresentationSlot(snapshot.LastCombatEnemyId);
            if (combatEnemyImage != null)
            {
                combatEnemyImage.sprite = enemySlot != null && enemySlot.EnemySprite != null
                    ? enemySlot.EnemySprite
                    : slot == null ? null : slot.EnemySprite;
                combatEnemyImage.gameObject.SetActive(combatEnemyImage.sprite != null);
            }

            if (combatEnemyTitleText != null)
            {
                var intent = BuildCombatIntentLine(snapshot);
                combatEnemyTitleText.text = PublicEnemyName(snapshot.LastCombatEnemyId) + "  HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp +
                    (string.IsNullOrEmpty(intent) ? string.Empty : "\n" + intent);
            }

            if (enemyHpFill != null)
            {
                enemyHpFill.fillAmount = Ratio(snapshot.EnemyHp, snapshot.EnemyMaxHp);
            }

            if (playerHpFill != null)
            {
                playerHpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            }
        }

        private void UpdatePartyDock(PrototypeRunSnapshot snapshot)
        {
            if (combatPartyDock != null)
            {
                combatPartyDock.gameObject.SetActive(snapshot.IsInCombat);
            }

            if (combatPlayerPortraitImage != null)
            {
                combatPlayerPortraitImage.sprite = presentationData == null ? null : presentationData.DefaultPlayerPortrait;
                combatPlayerPortraitImage.color = combatPlayerPortraitImage.sprite == null
                    ? new Color(0.15f, 0.19f, 0.22f, 1f)
                    : Color.white;
                combatPlayerPortraitImage.gameObject.SetActive(true);
            }

            if (combatPlayerPortraitFallbackText != null)
            {
                combatPlayerPortraitFallbackText.gameObject.SetActive(combatPlayerPortraitImage == null || combatPlayerPortraitImage.sprite == null);
            }

            if (combatMataiosPortraitImage != null)
            {
                var combatMataiosPortrait = presentationData == null ? null : presentationData.CombatMataiosPortrait;
                combatMataiosPortraitImage.sprite = combatMataiosPortrait != null
                    ? combatMataiosPortrait
                    : (npcPortraitImage != null && npcPortraitImage.sprite != null ? npcPortraitImage.sprite : mataiosPortrait);
                combatMataiosPortraitImage.color = Color.white;
                combatMataiosPortraitImage.gameObject.SetActive(combatMataiosPortraitImage.sprite != null);
            }

            ApplyCombatPortraitFrame(combatPlayerPortraitFrameImage);
            ApplyCombatPortraitFrame(combatMataiosPortraitFrameImage);

            if (combatPlayerCardText != null)
            {
                combatPlayerCardText.text = BuildCombatPlayerCardText(snapshot);
            }

            UpdateCombatStatusIcons(snapshot);

            if (combatMataiosCardText != null)
            {
                combatMataiosCardText.text = BuildCombatMataiosCardText(snapshot);
            }
        }

        private void ApplyCombatPortraitFrame(Image frameImage)
        {
            if (frameImage == null)
            {
                return;
            }

            frameImage.sprite = presentationData == null ? null : presentationData.CombatPortraitFrame;
            frameImage.color = Color.white;
            frameImage.gameObject.SetActive(frameImage.sprite != null);
        }

        private void UpdateCombatActionIcons()
        {
            SetCombatActionIcon(attackActionIconImage, CombatAction.Attack, new Color(0.84f, 0.34f, 0.26f, 0.94f));
            SetCombatActionIcon(defendActionIconImage, CombatAction.Defend, new Color(0.38f, 0.58f, 0.82f, 0.94f));
            SetCombatActionIcon(skillActionIconImage, CombatAction.Skill, new Color(0.78f, 0.68f, 0.34f, 0.94f));
        }

        private void UpdateTopHudIcons(PrototypeRunSnapshot snapshot)
        {
            EnsureTopHudIcons();
            ApplyTopHudIconSprites();
            var visible = !showRawDebugText && !string.IsNullOrEmpty(snapshot.RunId);
            SetImageVisible(topGoldIconImage, visible);
            SetImageVisible(topMemoryIconImage, visible);
            SetImageVisible(topAffinityIconImage, visible);
        }

        private void ApplyTopHudIconSprites()
        {
            SetStaticIcon(topGoldIconImage, "resource.gold");
            SetStaticIcon(topMemoryIconImage, "resource.memory");
            SetStaticIcon(topAffinityIconImage, "resource.affinity");
        }

        private void ApplyCombatStatusIconSprites()
        {
            SetStaticIcon(combatTrainingStatusIconImage, "status.training");
            SetStaticIcon(combatBandageStatusIconImage, "status.bandage");
            SetStaticIcon(combatRecallStatusIconImage, "status.recall_anchor");
        }

        private void UpdateCombatStatusIcons(PrototypeRunSnapshot snapshot)
        {
            ApplyCombatStatusIconSprites();
            var hasTraining = _roomController != null && _roomController.RunState != null && _roomController.RunState.TrainingBuffActive;
            var hasBandage = _roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0;
            var hasRecall = _roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR");
            SetImageVisible(combatTrainingStatusIconImage, snapshot.IsInCombat && hasTraining);
            SetImageVisible(combatBandageStatusIconImage, snapshot.IsInCombat && hasBandage);
            SetImageVisible(combatRecallStatusIconImage, snapshot.IsInCombat && hasRecall);
        }

        private void SetStaticIcon(Image target, string key)
        {
            if (target == null)
            {
                return;
            }

            target.sprite = ResolveIcon(key);
            target.color = target.sprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
            target.preserveAspect = true;
        }

        private Sprite ResolveIcon(string key)
        {
            return presentationData != null && presentationData.TryGetIcon(key, out var icon) ? icon : null;
        }

        private static void SetImageVisible(Image target, bool visible)
        {
            if (target != null)
            {
                target.gameObject.SetActive(visible && target.sprite != null);
            }
        }

        private void SetCombatActionIcon(Image target, CombatAction action, Color fallbackColor)
        {
            if (target == null)
            {
                return;
            }

            target.sprite = presentationData != null && presentationData.TryGetCombatActionIcon(action, out var icon) ? icon : null;
            target.color = target.sprite == null ? fallbackColor : Color.white;
            target.gameObject.SetActive(true);
        }

        private void ResolveCombatAction(CombatAction action)
        {
            if (_roomController == null)
            {
                ShowResultMessage("combat unavailable");
                return;
            }

            var resolution = _roomController.ResolveCombatAction(action);
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private void UpdateDemoCompletePanel(PrototypeRunSnapshot snapshot)
        {
            EnsureDemoCompleteText();
            if (demoCompleteText == null)
            {
                return;
            }

            demoCompleteText.gameObject.SetActive((snapshot.RunClear || snapshot.RunFailed) && !snapshot.IsInCombat);
            if (snapshot.RunFailed)
            {
                demoCompleteText.text = showRawDebugText ? "run.failed\nRestart Ready" : "실패\n재시작 가능";
                return;
            }

            if (snapshot.EndingRest)
            {
                demoCompleteText.text = showRawDebugText ? "ending.rest" : "안식 선택 완료";
                return;
            }

            if (snapshot.EndingContinue)
            {
                demoCompleteText.text = showRawDebugText ? "ending.continue\nRestart Ready" : "동행 계속\n재시작 가능";
                return;
            }

            demoCompleteText.text = showRawDebugText ? "run.clear\nChoose Ending" : "클리어\n엔딩 선택";
        }

        private void UpdateResultVisibility(PrototypeRunSnapshot snapshot)
        {
            if (resultText == null)
            {
                return;
            }

            var visible = !snapshot.IsInCombat &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive &&
                !IsMapSelectionVisible(snapshot);
            SetResultVisible(visible);
        }

        private string BuildChoiceLabel(PrototypeEncounterChoiceView view, int index)
        {
            var label = showRawDebugText
                ? (string.IsNullOrEmpty(view.TextKey) ? view.ChoiceStableId : view.TextKey)
                : view.ChoiceStableId.StartsWith("floor.", StringComparison.Ordinal) && !string.IsNullOrEmpty(view.TextKey)
                    ? view.TextKey
                    : ResolvePublicChoiceLabel(view.ChoiceStableId, index);
            if (!showRawDebugText && IsPurchaseChoice(view.ChoiceStableId))
            {
                label = ResolvePurchaseChoiceTitle(view.HintText);
            }

            if (!view.Enabled)
            {
                label += showRawDebugText && !string.IsNullOrEmpty(view.ReasonTextKey)
                    ? "\n" + view.ReasonTextKey
                    : "\n" + (IsPurchaseChoice(view.ChoiceStableId)
                        ? NormalizeShopDisabledHint(view.HintText)
                        : string.IsNullOrEmpty(view.HintText) ? "선택 불가" : NormalizePublicHint(view.HintText));
            }
            else if (!showRawDebugText && !string.IsNullOrEmpty(view.HintText))
            {
                label += "\n" + NormalizePublicHint(view.HintText);
            }

            return label;
        }

        private static bool IsPurchaseChoice(string choiceStableId)
        {
            return !string.IsNullOrEmpty(choiceStableId) && choiceStableId.Contains("_BUY_", StringComparison.Ordinal);
        }

        private static string NormalizeShopDisabledHint(string hint)
        {
            var normalized = NormalizePublicHint(hint);
            return normalized.Contains("Gold 부족", StringComparison.Ordinal) ? "Gold 부족" : "구매 불가";
        }

        private Sprite ResolveShopCardSprite(bool enabled)
        {
            if (presentationData == null)
            {
                return null;
            }

            return enabled
                ? presentationData.ShopProductCard
                : presentationData.ShopLockedCard != null ? presentationData.ShopLockedCard : presentationData.ShopProductCard;
        }

        private Sprite ResolvePurchaseChoiceIcon(PrototypeEncounterChoiceView view)
        {
            var title = ResolvePurchaseChoiceTitle(view.HintText);
            var key = title switch
            {
                "붕대" => "item.field_bandage",
                "등유" => "item.lantern_oil",
                "찢어진 부적" => "item.torn_charm",
                "정찰" => "ability.scout",
                "회상 닻" => "ability.recall_anchor",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(view.ChoiceStableId) && view.ChoiceStableId.Contains("_BUY_ABILITY", StringComparison.Ordinal))
            {
                key = view.HintText.Contains("회상", StringComparison.Ordinal) ? "ability.recall_anchor" : "ability.scout";
            }

            return ResolveIcon(key);
        }

        private static string ResolvePurchaseChoiceTitle(string hint)
        {
            var normalized = NormalizePublicHint(hint);
            if (normalized.Contains("붕대", StringComparison.Ordinal))
            {
                return "붕대";
            }

            if (normalized.Contains("등유", StringComparison.Ordinal))
            {
                return "등유";
            }

            if (normalized.Contains("찢어진 부적", StringComparison.Ordinal))
            {
                return "찢어진 부적";
            }

            if (normalized.Contains("정찰", StringComparison.Ordinal))
            {
                return "정찰";
            }

            if (normalized.Contains("회상 닻", StringComparison.Ordinal))
            {
                return "회상 닻";
            }

            return "구매";
        }

        private static string ResolveMapNodeLabel(PrototypeFloorMapNodeType type)
        {
            return type switch
            {
                PrototypeFloorMapNodeType.Combat => "전투",
                PrototypeFloorMapNodeType.Event => "이벤트",
                PrototypeFloorMapNodeType.Rest => "휴식",
                PrototypeFloorMapNodeType.Shop => "상점",
                PrototypeFloorMapNodeType.Boss => "보스",
                _ => "노드"
            };
        }

        private static string ResolveMapNodeHint(PrototypeFloorMapNodeType type)
        {
            return type switch
            {
                PrototypeFloorMapNodeType.Combat => "전투 보상 또는 피해",
                PrototypeFloorMapNodeType.Event => "결과는 선택 후 공개",
                PrototypeFloorMapNodeType.Rest => "HP 회복 / 내부 불안 감소",
                PrototypeFloorMapNodeType.Shop => "보스 전 준비",
                PrototypeFloorMapNodeType.Boss => "승리하면 층 클리어",
                _ => "선택 후 공개"
            };
        }

        private string BuildRouteLabel(PrototypeDemoRunStep step, bool includeRawIds)
        {
            var encounter = step.Encounter;
            if (encounter == null)
            {
                return step.NodeId;
            }

            var displayName = ResolvePresentationDisplayName(encounter);
            return includeRawIds ? displayName + " | " + step.NodeId + "/" + encounter.Id : displayName;
        }

        private static string BuildMemoryKeyLine(PrototypeRunSnapshot snapshot)
        {
            var titleKey = string.IsNullOrEmpty(snapshot.LastMemoryFragmentTitleKey)
                ? "titleKey:-"
                : "titleKey:" + snapshot.LastMemoryFragmentTitleKey;
            var bodyKey = string.IsNullOrEmpty(snapshot.LastMemoryFragmentBodyKey)
                ? "bodyKey:-"
                : "bodyKey:" + snapshot.LastMemoryFragmentBodyKey;
            return titleKey + " | " + bodyKey;
        }

        private static string ResolveEncounterDisplayName(EncounterData encounter)
        {
            if (encounter == null)
            {
                return "Encounter";
            }

            var choices = encounter.Choices ?? new EncounterChoiceRuntimeData[0];
            for (var i = 0; i < choices.Length; i++)
            {
                var effects = choices[i] == null ? null : choices[i].effects;
                if (effects == null)
                {
                    continue;
                }

                for (var j = 0; j < effects.Length; j++)
                {
                    if (effects[j] != null && effects[j].kind == "StartCombat")
                    {
                        return "CombatGate";
                    }
                }
            }

            return encounter.Type.ToString();
        }

        private string ResolvePresentationDisplayName(EncounterData encounter)
        {
            if (encounter == null)
            {
                return "Encounter";
            }

            if (presentationData != null &&
                presentationData.TryGetSlot(encounter.Id, out var slot) &&
                !string.IsNullOrEmpty(slot.DisplayName))
            {
                return showRawDebugText ? slot.DisplayName : ResolvePublicEncounterLabel(slot.DisplayName, encounter);
            }

            return showRawDebugText ? ResolveEncounterDisplayName(encounter) : ResolvePublicEncounterLabel(string.Empty, encounter);
        }

        private string BuildResultSummary(string message)
        {
            if (showRawDebugText)
            {
                return "result: " + message;
            }

            if (message.StartsWith("선택 가능한 길", StringComparison.Ordinal) ||
                message.StartsWith("아이콘을 보고", StringComparison.Ordinal))
            {
                return "결과\n다음 선택\n밝은 노드를 선택하세요";
            }

            var summary = "결과";
            AppendResultLine(ref summary, ExtractHpChange(message));
            AppendEffectTokens(ref summary, message);

            if (message.Contains("combat started"))
            {
                AppendResultLine(ref summary, "전투 시작");
            }

            if (message.Contains("enemyDefeated True", StringComparison.Ordinal) || message.Contains("victory", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "승리");
            }
            else if (message.Contains("defeat", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "패배");
            }

            if (message.Contains("enemyDefeated True", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "적 처치", allowOverflow: false);
            }

            if (message.Contains("already resolved:", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "이미 해결됨");
            }

            if (message.Contains("stair unlocked", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "다음 층 준비");
            }

            AppendFloorProgressionLine(ref summary, message);

            if (message.Contains("run.clear", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "최종 보스 격파");
                AppendResultLine(ref summary, "엔딩 선택 가능", allowOverflow: true);
            }

            if (message.Contains("ending.rest", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "안식 선택 완료");
            }

            if (message.Contains("ending.continue", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "동행 계속 선택");
            }

            if (message.Contains("run.failed", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "실패");
            }

            if (message.Contains("run.restartReady", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "재시작 가능");
            }

            return summary == "결과" ? "결과\n-" : summary;
        }

        private static void AppendEffectTokens(ref string summary, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var tokens = message.Split(new[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                AppendResultLine(ref summary, NormalizeResultToken(tokens[i].Trim()));
            }
        }

        private static string NormalizeResultToken(string token)
        {
            if (string.IsNullOrEmpty(token) ||
                token.StartsWith("choices:", StringComparison.Ordinal) ||
                token.StartsWith("choice applied:", StringComparison.Ordinal) ||
                token.StartsWith("effects=", StringComparison.Ordinal) ||
                token.StartsWith("ignored=", StringComparison.Ordinal) ||
                token.StartsWith("combat ", StringComparison.Ordinal) ||
                token.StartsWith("round ", StringComparison.Ordinal) ||
                token.StartsWith("enemy ", StringComparison.Ordinal) ||
                token.StartsWith("result ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.StartsWith("HP ", StringComparison.Ordinal) ||
                token.StartsWith("Gold ", StringComparison.Ordinal) ||
                token.StartsWith("Mental ", StringComparison.Ordinal) ||
                token.StartsWith("Affinity ", StringComparison.Ordinal))
            {
                if (IsNoOpDelta(token.Substring(token.IndexOf(' ') + 1)))
                {
                    return string.Empty;
                }

                return token
                    .Replace("Mental ", "정신 ", StringComparison.Ordinal)
                    .Replace("Affinity ", "신뢰 ", StringComparison.Ordinal);
            }

            if (token.StartsWith("Glitch ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.Contains("HP restored", StringComparison.Ordinal))
            {
                return "HP 회복";
            }

            if (token.Contains("training buff +1 next combat", StringComparison.Ordinal))
            {
                return "다음 전투 피해 +1";
            }

            if (token.Contains("Mataios response ready", StringComparison.Ordinal))
            {
                return "마타이오스 응답";
            }

            if (token.StartsWith("rest ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.StartsWith("gold reward ", StringComparison.Ordinal))
            {
                return "Gold +" + token.Substring("gold reward ".Length).Trim().TrimStart('+');
            }

            if (token.StartsWith("glitch ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.StartsWith("affinity ", StringComparison.Ordinal))
            {
                return "신뢰 " + token.Substring("affinity ".Length).Trim();
            }

            if (token.StartsWith("playerDamage ", StringComparison.Ordinal))
            {
                return "적 피해 " + token.Substring("playerDamage ".Length).Trim();
            }

            if (token.StartsWith("enemyDamage ", StringComparison.Ordinal))
            {
                return "받은 피해 " + token.Substring("enemyDamage ".Length).Trim();
            }

            if (token.StartsWith("combo ", StringComparison.Ordinal))
            {
                return "콤보 피해 " + token.Substring("combo ".Length).Trim();
            }

            if (token.StartsWith("item ", StringComparison.Ordinal))
            {
                return NormalizeRefDelta(token.Substring("item ".Length).Trim()) + " 획득";
            }

            if (token.StartsWith("ability ", StringComparison.Ordinal))
            {
                return NormalizeRefDelta(token.Substring("ability ".Length).Trim()) + " 획득";
            }

            if (token.StartsWith("reward ", StringComparison.Ordinal))
            {
                return "보상: " + NormalizeRefDelta(token.Substring("reward ".Length).Trim());
            }

            if (token.StartsWith("memory unlocked ", StringComparison.Ordinal))
            {
                return "기억의 잔향 해금";
            }

            if (token.StartsWith("action ", StringComparison.Ordinal))
            {
                return "행동: " + PublicCombatActionName(token.Substring("action ".Length).Trim());
            }

            if (token.StartsWith("jar outcome: Gold +8", StringComparison.Ordinal))
            {
                return "골드 획득: Gold +8";
            }

            if (token.StartsWith("jar outcome: elite combat", StringComparison.Ordinal))
            {
                return "엘리트 전투 발생";
            }

            if (token.StartsWith("jar outcome: HP +5", StringComparison.Ordinal))
            {
                return "HP +5";
            }

            if (token.StartsWith("jar outcome: next 3 combat damage buff", StringComparison.Ordinal))
            {
                return "다음 3회 전투 피해 증가";
            }

            return string.Empty;
        }

        private static string NormalizeRefDelta(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var parts = value.Split(' ');
            if (parts.Length == 0)
            {
                return value;
            }

            var label = PublicRefName(parts[0]);
            return parts.Length > 1 ? label + " " + parts[1] : label;
        }

        private static string ExtractHpChange(string message)
        {
            var token = "player HP ";
            var index = string.IsNullOrEmpty(message) ? -1 : message.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
            {
                return string.Empty;
            }

            var start = index + token.Length;
            var end = message.IndexOf(" |", start, StringComparison.Ordinal);
            if (end < 0)
            {
                end = message.Length;
            }

            var value = message.Substring(start, end - start).Trim();
            var arrow = value.IndexOf("->", StringComparison.Ordinal);
            if (arrow < 0)
            {
                return "HP " + value;
            }

            if (int.TryParse(value.Substring(0, arrow).Trim(), out var before) &&
                int.TryParse(value.Substring(arrow + 2).Trim(), out var after))
            {
                var delta = after - before;
                return delta == 0 ? string.Empty : "HP " + FormatDelta(delta);
            }

            return "HP " + value;
        }

        private static void AppendFloorProgressionLine(ref string summary, string message)
        {
            var floorIndex = string.IsNullOrEmpty(message) ? -1 : message.IndexOf("floor ", StringComparison.Ordinal);
            if (floorIndex < 0 || !message.Contains(" entered", StringComparison.Ordinal))
            {
                return;
            }

            var start = floorIndex + "floor ".Length;
            var end = message.IndexOf(" entered", start, StringComparison.Ordinal);
            if (end > start)
            {
                AppendResultLine(ref summary, "Floor " + message.Substring(start, end - start).Trim() + " 진입");
            }
        }

        private static void AppendResultLine(ref string summary, string line, bool allowOverflow = false)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            var needle = "\n" + line;
            if (summary.EndsWith(needle, StringComparison.Ordinal) || summary.Contains(needle + "\n", StringComparison.Ordinal))
            {
                return;
            }

            if (!allowOverflow && CountResultLines(summary) >= ResultLineLimit)
            {
                return;
            }

            summary += "\n" + line;
        }

        private static int CountResultLines(string summary)
        {
            if (string.IsNullOrEmpty(summary) || summary == "결과")
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < summary.Length; i++)
            {
                if (summary[i] == '\n')
                {
                    count++;
                }
            }

            return count;
        }

        private static string BuildCombatOutcomeLabel(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                return "전투 중";
            }

            if (snapshot.LastCombatEnemyDefeated)
            {
                return "승리";
            }

            return string.IsNullOrEmpty(snapshot.LastCombatResultId) ? "-" : PublicCombatResultName(snapshot.LastCombatResultId);
        }

        private string BuildCombatPresentation(PrototypeRunSnapshot snapshot)
        {
            var skill = HasScoutSkill(snapshot) ? "정찰 기술: 추가 공격" : "기술 불가: 정찰 필요";
            var extra = BuildCombatExtraLine(snapshot);
            var state = string.IsNullOrEmpty(extra) ? skill :
                extra.StartsWith("콤보 피해", StringComparison.Ordinal) ? "정찰 기술 | " + extra : extra;
            return PublicEnemyName(snapshot.LastCombatEnemyId) + " | 적 HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + " | 내 HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + "\n" +
                ShortenPublicLine(BuildCombatFeedback(snapshot.LastCombatRoundResult), 34) + "\n" +
                ShortenPublicLine(state, 36);
        }

        private static string BuildCombatExtraLine(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.LastCombatComboDamage > 0)
            {
                return "콤보 피해 " + snapshot.LastCombatComboDamage;
            }

            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                return "훈련 보너스: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "training +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("bandage ", StringComparison.Ordinal))
            {
                return "붕대: HP 회복 " + ExtractRoundNumber(snapshot.LastCombatRoundResult, "bandage ").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("recall ready", StringComparison.Ordinal))
            {
                return "회상 닻: 패배 1회 방지";
            }

            return string.Empty;
        }

        private static string BuildCombatIntentLine(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.CombatRound <= 1)
            {
                return "의도: 공격 준비";
            }

            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            if (result.Contains("action Defend", StringComparison.Ordinal))
            {
                return "의도: 압박 지속";
            }

            if (result.Contains("action Skill", StringComparison.Ordinal) || result.Contains("scout +", StringComparison.Ordinal))
            {
                return "의도: 빈틈 노출";
            }

            return "의도: 반격 준비";
        }

        private string BuildCombatPlayerCardText(PrototypeRunSnapshot snapshot)
        {
            return "플레이어\n" +
                "HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + "  ATK " + snapshot.PlayerAttack + "\n" +
                BuildPlayerBuffChipLine(snapshot);
        }

        private string BuildCombatMataiosCardText(PrototypeRunSnapshot snapshot)
        {
            var reaction = string.IsNullOrEmpty(snapshot.LastNpcReactionKey)
                ? "반응 대기"
                : ShortenPublicLine(ResolvePublicNpcReaction(snapshot.LastNpcReactionKey), 16);
            return "마타이오스\n" +
                BuildMataiosChipLine(snapshot) + "\n" +
                reaction;
        }

        private string BuildPlayerBuffChipLine(PrototypeRunSnapshot snapshot)
        {
            var chips = new List<string>();
            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                chips.Add("훈련 +1");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0)
            {
                chips.Add("붕대");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
            {
                chips.Add("회상 닻");
            }

            return chips.Count == 0 ? "버프 없음" : string.Join("  ", chips);
        }

        private static string BuildMataiosChipLine(PrototypeRunSnapshot snapshot)
        {
            var affinity = snapshot.Affinity >= 4 ? "신뢰 높음" :
                snapshot.Affinity > 0 ? "신뢰 형성" :
                snapshot.Affinity < 0 ? "거리감" :
                "동행 중";
            return affinity + "  기억 " + snapshot.MemoryFragmentCount + "  안정";
        }

        private static string BuildCombatFeedback(string roundResult)
        {
            if (string.IsNullOrEmpty(roundResult))
            {
                return "전투 준비";
            }

            if (roundResult.Contains("skill unavailable", StringComparison.Ordinal))
            {
                return "기술 불가: 정찰 필요";
            }

            if (roundResult.Contains("recall anchor", StringComparison.Ordinal))
            {
                return "회상 닻: HP 회복 후 전투 지속";
            }

            if (roundResult.Contains("ready", StringComparison.OrdinalIgnoreCase))
            {
                var ready = "전투 준비";
                if (roundResult.Contains("scout ", StringComparison.Ordinal))
                {
                    var scout = ExtractRoundNumber(roundResult, "scout +").Trim();
                    ready += string.IsNullOrEmpty(scout) ? " | 정찰 준비" : " | 정찰 공격 +" + scout;
                }

                return ready;
            }

            if (roundResult.Contains("Defend", StringComparison.Ordinal))
            {
                return "방어: 받은 피해 " + ExtractRoundNumber(roundResult, "enemyDamage ").Trim() + " | 피해 절반 감소";
            }

            if (roundResult.Contains("Attack", StringComparison.Ordinal))
            {
                return "공격: 적 피해 " + ExtractRoundNumber(roundResult, "playerDamage ").Trim();
            }

            if (roundResult.Contains("Skill", StringComparison.Ordinal))
            {
                return "정찰 기술: 적 피해 " + ExtractRoundNumber(roundResult, "playerDamage ").Trim() +
                    " + 추가 " + ExtractRoundNumber(roundResult, "combo ").Trim();
            }

            return "라운드 처리";
        }

        private bool HasScoutSkill(PrototypeRunSnapshot snapshot)
        {
            if (_roomController != null && _roomController.RunState != null)
            {
                return _roomController.RunState.HasAbilityRef("ABILITY_SCOUT");
            }

            return snapshot.AbilityCount > 0;
        }

        private static string ExtractRoundNumber(string source, string token)
        {
            var index = string.IsNullOrEmpty(source) ? -1 : source.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
            {
                return string.Empty;
            }

            var start = index + token.Length;
            var end = source.IndexOf(" |", start, StringComparison.Ordinal);
            if (end < 0)
            {
                end = source.Length;
            }

            var value = source.Substring(start, end - start).Trim();
            return string.IsNullOrEmpty(value) ? string.Empty : " " + value;
        }

        private static string BuildBar(int value, int max)
        {
            if (max <= 0)
            {
                return "[-----]";
            }

            var filled = Mathf.Clamp(Mathf.CeilToInt((float)value / max * 5f), 0, 5);
            return "[" + new string('#', filled) + new string('-', 5 - filled) + "]";
        }

        private void UpdatePresentationState(PrototypeRunSnapshot snapshot)
        {
            if (RestInteractionPanelVisible)
            {
                HideLegacyEncounterVisuals(hideBackground: false);
                return;
            }

            var slot = ResolveCurrentPresentationSlot(snapshot);
            ApplyPresentationSlot(slot);
            if (IsMapSelectionVisible(snapshot))
            {
                HideLegacyEncounterVisuals(hideBackground: true);
            }
            else if (_shopPresentationActive || snapshot.IsInCombat || _eventPresentationActive)
            {
                HideLegacyEncounterVisuals(hideBackground: false);
            }

            if (!snapshot.IsInCombat && _shopPresentationActive)
            {
                ApplyMerchantPresentation(snapshot.CurrentFloor);
            }
            else if (!RestInteractionPanelVisible)
            {
                HideMerchantPresentation();
            }

            if (interactionText != null && !showRawDebugText && snapshot.RunClear && !snapshot.IsInCombat)
            {
                interactionText.text = snapshot.EndingChoicePending ? "엔딩 선택" : "클리어";
            }
        }

        private DemoPresentationSlot ResolveCurrentPresentationSlot(PrototypeRunSnapshot snapshot)
        {
            if (presentationData == null)
            {
                return null;
            }

            if (!snapshot.IsInCombat &&
                (_shopPresentationActive || _eventPresentationActive) &&
                !string.IsNullOrEmpty(_activePresentationEncounterId) &&
                presentationData.TryGetSlot(_activePresentationEncounterId, out var activeSlot))
            {
                return activeSlot;
            }

            var encounterId = snapshot.IsInCombat ? ResolveCombatEncounterId() : snapshot.NextDemoEncounterId;
            if (string.IsNullOrEmpty(encounterId) && snapshot.RunClear)
            {
                encounterId = presentationData.TryGetSlot("run.clear", out _) ? "run.clear" : ResolveDemoCompleteCutsceneEncounterId(snapshot);
            }

            return presentationData.TryGetSlot(encounterId, out var slot) ? slot : null;
        }

        private DemoPresentationSlot ResolveEnemyPresentationSlot(string enemyStableId)
        {
            if (presentationData == null || string.IsNullOrEmpty(enemyStableId))
            {
                return null;
            }

            return presentationData.TryGetSlot(enemyStableId, out var slot) ? slot : null;
        }

        private void ApplyPresentationSlot(string encounterStableId)
        {
            if (presentationData != null && presentationData.TryGetSlot(encounterStableId, out var slot))
            {
                _activePresentationEncounterId = encounterStableId;
                ApplyPresentationSlot(slot);
            }
        }

        private void ApplyPresentationSlot(DemoPresentationSlot slot)
        {
            EnsureEncounterBackgroundImage();
            if (encounterBackgroundImage != null)
            {
                encounterBackgroundImage.sprite = slot == null ? null : slot.BackgroundSprite;
                encounterBackgroundImage.gameObject.SetActive(encounterBackgroundImage.sprite != null);
            }

            if (slot != null && slot.MataiosPortrait != null)
            {
                SetNpcPortrait(slot.MataiosPortrait);
            }
        }

        private string ResolveCombatEncounterId()
        {
            for (var i = _demoRouteEncounterIds.Count - 1; i >= 0; i--)
            {
                if (IsCombatEncounterId(_demoRouteEncounterIds[i]))
                {
                    return _demoRouteEncounterIds[i];
                }
            }

            return string.Empty;
        }

        private static bool IsShopEncounterId(string encounterId)
        {
            return !string.IsNullOrEmpty(encounterId) && encounterId.Contains("SHOP", StringComparison.Ordinal);
        }

        private void UpdateCutsceneTriggers(PrototypeRunSnapshot snapshot)
        {
            if (presentationData == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(snapshot.LastMemoryFragmentId))
            {
                TryPlayCutsceneOnce(
                    snapshot.LastMemoryFragmentId,
                    "memory:" + snapshot.LastMemoryFragmentId,
                    PrototypeCutsceneTrigger.MemoryFragmentUnlock,
                    ref _lastMemoryCutsceneKey);
            }

            if (snapshot.IsInCombat && !string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                TryPlayCutsceneOnce(
                    snapshot.NextDemoEncounterId,
                    "combat:" + snapshot.LastCombatId,
                    PrototypeCutsceneTrigger.CombatGateStart,
                    ref _lastCombatCutsceneKey);
            }

            if (snapshot.RunClear && !snapshot.IsInCombat)
            {
                TryPlayCutsceneOnce(
                    ResolveDemoCompleteCutsceneEncounterId(snapshot),
                    "complete:" + snapshot.DemoResolvedStepCount,
                    PrototypeCutsceneTrigger.DemoComplete,
                    ref _lastDemoCompleteCutsceneKey);
            }
        }

        private string ResolveDemoCompleteCutsceneEncounterId(PrototypeRunSnapshot snapshot)
        {
            if (!string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                return snapshot.NextDemoEncounterId;
            }

            for (var i = _demoRouteEncounterIds.Count - 1; i >= 0; i--)
            {
                var encounterId = _demoRouteEncounterIds[i];
                if (!string.IsNullOrEmpty(encounterId) && encounterId != "demo.complete")
                {
                    return encounterId;
                }
            }

            return string.Empty;
        }

        private void TryPlayInteractionCutscene(string encounterStableId, string message)
        {
            if (string.IsNullOrEmpty(encounterStableId) || string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Contains("memory unlocked"))
            {
                TryPlayCutsceneOnce(
                    encounterStableId,
                    "memory:" + encounterStableId,
                    PrototypeCutsceneTrigger.MemoryFragmentUnlock,
                    ref _lastMemoryCutsceneKey);
            }

            if (message.Contains("combat started"))
            {
                TryPlayCutsceneOnce(
                    encounterStableId,
                    "combat:" + encounterStableId,
                    PrototypeCutsceneTrigger.CombatGateStart,
                    ref _lastCombatCutsceneKey);
            }

            if (message.Contains("demo.complete") || message.Contains("run.clear"))
            {
                TryPlayCutsceneOnce(
                    encounterStableId,
                    "complete:" + encounterStableId,
                    PrototypeCutsceneTrigger.DemoComplete,
                    ref _lastDemoCompleteCutsceneKey);
            }
        }

        private void TryPlayCutsceneOnce(string encounterStableId, string key, PrototypeCutsceneTrigger trigger, ref string lastKey)
        {
            if (lastKey == key || presentationData == null || !presentationData.TryGetCutscene(encounterStableId, trigger, out var cutscene))
            {
                return;
            }

            EnsureCutscenePlayer();
            SubscribeCutsceneFinished();
            cutscenePlayer.Play(cutscene);
            SetCombatButtonsInteractable(false);
            lastKey = key;
        }

        private void SetCombatButtonsInteractable(bool interactable)
        {
            if (attackButton != null)
            {
                attackButton.interactable = interactable;
            }

            if (defendButton != null)
            {
                defendButton.interactable = interactable;
            }

            if (skillButton != null)
            {
                skillButton.interactable = interactable && _roomController != null && _roomController.GetSnapshot().AbilityCount > 0;
            }
        }

        private void EnsureCutscenePlayer()
        {
            if (cutscenePlayer != null)
            {
                return;
            }

            var playerObject = new GameObject("Prototype Cutscene Player");
            playerObject.transform.SetParent(HudParent, false);
            var rect = playerObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            cutscenePlayer = playerObject.AddComponent<PrototypeCutscenePlayer>();
            SubscribeCutsceneFinished();
            cutscenePlayer.Hide();
        }

        private void SubscribeCutsceneFinished()
        {
            if (cutscenePlayer == null || _cutsceneFinishedSubscribed)
            {
                return;
            }

            cutscenePlayer.Finished += HandleCutsceneFinished;
            _cutsceneFinishedSubscribed = true;
        }

        private void HandleCutsceneFinished()
        {
            if (_roomController != null)
            {
                ShowRunState(_roomController.GetSnapshot());
            }
        }

        private bool IsCutscenePlaying()
        {
            return cutscenePlayer != null && cutscenePlayer.gameObject.activeInHierarchy && cutscenePlayer.IsPlaying;
        }

        private static bool IsCombatEncounterId(string encounterId)
        {
            return encounterId == "ENC_COMBAT_GATE_01" || encounterId == "ENC_COMBAT_GATE_02" || encounterId == "ENC_COMBAT_GATE_03";
        }

        private static string ResolvePublicChoiceLabel(string choiceStableId, int index)
        {
            if (!string.IsNullOrEmpty(choiceStableId))
            {
                if (choiceStableId == "CHOICE_EVT_F01_JAR_PATTERNED")
                {
                    return "신기한 문양이 각인된 항아리";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_PLAIN")
                {
                    return "평범한 항아리";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_CRACKED")
                {
                    return "금 간 항아리";
                }

                if (choiceStableId.Contains("_BUY_", StringComparison.Ordinal))
                {
                    return "구매";
                }

                if (choiceStableId.Contains("_LEAVE", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_CONTINUE", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_IGNORE", StringComparison.Ordinal))
                {
                    return "지나간다";
                }

                if (choiceStableId.Contains("_UNLOCK", StringComparison.Ordinal))
                {
                    return "기억의 잔향";
                }

                if (choiceStableId.Contains("_WITHDRAW", StringComparison.Ordinal))
                {
                    return "보류";
                }

                if (choiceStableId.Contains("_ENGAGE", StringComparison.Ordinal))
                {
                    return "전투";
                }

                if (choiceStableId.Contains("_PREPARE", StringComparison.Ordinal))
                {
                    return "준비";
                }

                if (choiceStableId.Contains("_AID", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_HELP", StringComparison.Ordinal))
                {
                    return "돕는다";
                }

                if (choiceStableId.Contains("_REFUSE", StringComparison.Ordinal))
                {
                    return "거절한다";
                }

                if (choiceStableId.Contains("_TRADE", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_BARGAIN", StringComparison.Ordinal))
                {
                    return "거래한다";
                }

                if (choiceStableId.Contains("_REST", StringComparison.Ordinal))
                {
                    return "휴식";
                }
            }

            return choiceStableId switch
            {
                "CHOICE_SHOP_01_BUY_ITEM" => "구매",
                "CHOICE_SHOP_01_BUY_ABILITY" => "구매",
                "CHOICE_SHOP_01_LEAVE" => "지나간다",
                "CHOICE_MORAL_01_AID" => "돕는다",
                "CHOICE_MORAL_01_REFUSE" => "거절한다",
                "CHOICE_MORAL_01_TRADE" => "거래한다",
                "CHOICE_MEMORY_01_UNLOCK" => "기억의 잔향",
                "CHOICE_MEMORY_01_WITHDRAW" => "보류",
                "CHOICE_COMBAT_01_ENGAGE" => "전투",
                "CHOICE_COMBAT_01_PREPARE" => "준비",
                "CHOICE_F02_SHOP_BUY_ITEM" => "구매",
                "CHOICE_F02_SHOP_BUY_ABILITY" => "구매",
                "CHOICE_F02_SHOP_LEAVE" => "지나간다",
                "CHOICE_F02_MORAL_HELP" => "돕는다",
                "CHOICE_F02_MORAL_LEAVE" => "거절한다",
                "CHOICE_F02_MORAL_BARGAIN" => "거래한다",
                "CHOICE_COMBAT_02_ENGAGE" => "전투",
                "CHOICE_COMBAT_02_PREPARE" => "준비",
                "CHOICE_COMBAT_03_ENGAGE" => "전투",
                "CHOICE_COMBAT_03_PREPARE" => "준비",
                _ => "선택 " + (index + 1)
            };
        }

        private static string ResolvePublicEncounterLabel(string configuredLabel, EncounterData encounter)
        {
            if (!string.IsNullOrEmpty(configuredLabel) && !LooksLikeInternalLabel(configuredLabel))
            {
                return configuredLabel;
            }

            if (encounter == null)
            {
                return "Encounter";
            }

            return encounter.Id switch
            {
                "ENC_SHOP_01" => "상점",
                "ENC_MORAL_CHOICE_01" => "선택",
                "ENC_MEMORY_FRAGMENT_01" => "기억의 잔향",
                "ENC_COMBAT_GATE_01" => "전투",
                "ENC_F02_SHOP_001" => "상점",
                "ENC_F02_MORAL_CHOICE_001" => "선택",
                "ENC_COMBAT_GATE_02" => "보스 관문",
                "ENC_COMBAT_GATE_03" => "최종 보스",
                _ => encounter.Type == EncounterType.MoralChoice ? "선택" :
                    encounter.Type == EncounterType.MemoryFragment ? "기억의 잔향" :
                    encounter.Type == EncounterType.Shop ? "상점" :
                    "조우"
            };
        }

        private static string ResolvePublicNpcReaction(string reactionKey)
        {
            if (string.IsNullOrEmpty(reactionKey))
            {
                return "-";
            }

            if (reactionKey.Contains("SHOP", StringComparison.OrdinalIgnoreCase))
            {
                return "상점 반응";
            }

            if (reactionKey.Contains("MORAL", StringComparison.OrdinalIgnoreCase))
            {
                return "선택 반응";
            }

            if (reactionKey.Contains("MEMORY", StringComparison.OrdinalIgnoreCase))
            {
                return "기억 반응";
            }

            if (reactionKey.Contains("COMBAT", StringComparison.OrdinalIgnoreCase) || reactionKey.Contains("BATTLE", StringComparison.OrdinalIgnoreCase))
            {
                return "전투 반응";
            }

            if (reactionKey.Contains("FLOOR", StringComparison.OrdinalIgnoreCase) || reactionKey.Contains("STAIR", StringComparison.OrdinalIgnoreCase))
            {
                return "층 이동 반응";
            }

            if (reactionKey.Contains("RECALL", StringComparison.OrdinalIgnoreCase))
            {
                return "회상 반응";
            }

            return "동행자 반응";
        }

        private static string NormalizePublicHint(string hint)
        {
            if (string.IsNullOrEmpty(hint))
            {
                return string.Empty;
            }

            var normalized = hint
                .Replace("Unavailable:", "선택 불가:", StringComparison.Ordinal)
                .Replace("선택 불가: Gold 부족", "구매 불가: Gold 부족", StringComparison.Ordinal)
                .Replace("Combat start", "전투 시작", StringComparison.Ordinal)
                .Replace("Memory unlock", "기억의 잔향 해금", StringComparison.Ordinal)
                .Replace("Ability 필요", "능력 필요", StringComparison.Ordinal);

            return ReplacePublicRefs(normalized);
        }

        private static string ReplacePublicRefs(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("ITEM_FIELD_BANDAGE", PublicRefName("ITEM_FIELD_BANDAGE"), StringComparison.Ordinal)
                .Replace("ITEM_LANTERN_OIL", PublicRefName("ITEM_LANTERN_OIL"), StringComparison.Ordinal)
                .Replace("ITEM_TORN_CHARM", PublicRefName("ITEM_TORN_CHARM"), StringComparison.Ordinal)
                .Replace("ABILITY_SCOUT", PublicRefName("ABILITY_SCOUT"), StringComparison.Ordinal)
                .Replace("ABILITY_RECALL_ANCHOR", PublicRefName("ABILITY_RECALL_ANCHOR"), StringComparison.Ordinal)
                .Replace("REWARD_CACHE_SMALL", PublicRefName("REWARD_CACHE_SMALL"), StringComparison.Ordinal)
                .Replace("REWARD_CACHE_MEMORY", PublicRefName("REWARD_CACHE_MEMORY"), StringComparison.Ordinal);
        }

        private static string PublicRefName(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return string.Empty;
            }

            return reference switch
            {
                "ITEM_FIELD_BANDAGE" => "붕대",
                "ITEM_LANTERN_OIL" => "등유",
                "ITEM_TORN_CHARM" => "찢어진 부적",
                "ABILITY_SCOUT" => "정찰",
                "ABILITY_RECALL_ANCHOR" => "회상 닻",
                "REWARD_CACHE_SMALL" => "작은 보급품",
                "REWARD_CACHE_MEMORY" => "기억 보급품",
                _ => LooksLikeInternalLabel(reference) || reference.Contains("_", StringComparison.Ordinal) ? "획득물" : reference
            };
        }

        private static string PublicEnemyName(string enemyId)
        {
            if (string.IsNullOrEmpty(enemyId))
            {
                return "적";
            }

            return enemyId switch
            {
                "ENEMY_BANDIT_MELEE_01" => "도적",
                "ENEMY_BANDIT_RANGED_01" => "원거리 도적",
                "ENEMY_SLIME_01" => "슬라임",
                "ENEMY_SKELETON_01" => "해골",
                "ENEMY_WILD_BEAST_01" => "들짐승",
                "ENEMY_EMPTY_ARMOR" => "리빙 아머",
                "ENEMY_SHADE_03" => "그림자",
                "ENEMY_WRAITH_04" => "망령",
                "ENEMY_FRACTURE_HOUND" => "변이된 들짐승",
                "ENEMY_LAMPLIGHTER_01" => "점등인",
                "BOSS_GATE_01" => "층 보스",
                "BOSS_APEX_02" => "최종 보스",
                _ => "적"
            };
        }

        private static string PublicCombatActionName(string action)
        {
            return action switch
            {
                "Attack" => "공격",
                "Defend" => "방어",
                "Skill" => "기술",
                _ => action
            };
        }

        private static string PublicCombatResultName(string resultId)
        {
            if (string.IsNullOrEmpty(resultId))
            {
                return "-";
            }

            if (resultId.Contains("victory", StringComparison.OrdinalIgnoreCase))
            {
                return "승리";
            }

            if (resultId.Contains("defeat", StringComparison.OrdinalIgnoreCase))
            {
                return "패배";
            }

            if (resultId.Contains("recall", StringComparison.OrdinalIgnoreCase))
            {
                return "회상 개입";
            }

            return resultId.Contains("_", StringComparison.Ordinal) ? "처리됨" : resultId;
        }

        private static bool LooksLikeInternalLabel(string value)
        {
            return value == "MoralChoice" ||
                   value == "MemoryFragment" ||
                   value == "CombatGate" ||
                   value == "DemoComplete" ||
                   value == "Shop" ||
                   value == "Boss Gate" ||
                   value == "Final Boss" ||
                   value == "Decision" ||
                   value == "Memory" ||
                   value == "Combat";
        }

        private static bool IsNoOpDelta(string value)
        {
            var arrow = value.IndexOf("->", StringComparison.Ordinal);
            if (arrow < 0)
            {
                return false;
            }

            var before = value.Substring(0, arrow).Trim();
            var after = value.Substring(arrow + 2).Trim();
            return before == after;
        }

        private static float Ratio(int value, int max)
        {
            return max <= 0 ? 0f : Mathf.Clamp01((float)value / max);
        }

        private static Font ResolveFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static string FormatDelta(int amount)
        {
            return amount > 0 ? "+" + amount : amount.ToString();
        }

        private static void EnsureEventSystem()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            var inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            if (Application.isPlaying)
            {
                inputModule.AssignDefaultActions();
            }
        }
    }
}
