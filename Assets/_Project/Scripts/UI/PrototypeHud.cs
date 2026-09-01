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
        [SerializeField] private Text combatEnemyStatusText;
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
        [SerializeField] private RectTransform skillPickerPanel;
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
        [SerializeField] private Image topPlayerProfileImage;
        [SerializeField] private Image topMataiosProfileImage;
        [SerializeField] private Image enemyHpFill;
        [SerializeField] private Image playerHpFill;
        [SerializeField] private Image mataiosHpFill;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button skillButton;
        [SerializeField] private Button combatItemInspectButton;
        [SerializeField] private Button combatItemInspectCloseButton;
        [SerializeField] private Button routeActionButton;
        [SerializeField] private Button nextFloorButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button endingRestButton;
        [SerializeField] private Button endingContinueButton;
        [SerializeField] private CombatUiController combatUiController;
        [SerializeField] private RestUiController restUiController;
        [SerializeField] private FloorMapUiController floorMapUiController;
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
        [SerializeField] private RectTransform resultIconStrip;
        [SerializeField] private RectTransform endingLayer;
        [SerializeField] private RectTransform portraitRoot;
        [SerializeField] private Image floorMapBackgroundImage;
        [SerializeField] private RectTransform utilityPanel;
        [SerializeField] private RectTransform preRunPlaceholderLayer;
        [SerializeField] private RectTransform combatItemInspectPanel;
        [SerializeField] private Text combatItemInspectText;
        [SerializeField] private Image lowHpWarningImage;
        [SerializeField] private RectTransform levelRewardPanel;
        [SerializeField] private Text levelRewardTitleText;
        [SerializeField] private Text levelRewardBodyText;
        [SerializeField] private Button levelRewardAttackButton;
        [SerializeField] private Button levelRewardMaxHpButton;
        [SerializeField] private Button levelRewardSkillButton;
        [SerializeField] private Text combatEnemyDamageNumberText;
        [SerializeField] private Text combatMataiosDamageNumberText;
        [SerializeField] private Text combatPlayerDamageNumberText;
        [SerializeField] private Text combatDefeatFeedbackText;
        [SerializeField] private RectTransform combatIntroOverlay;
        [SerializeField] private RectTransform bossRewardPanel;
        [SerializeField] private Text bossRewardTitleText;
        [SerializeField] private Text bossRewardGoldText;
        [SerializeField] private Text bossRewardAffinityText;
        [SerializeField] private Image bossRewardGoldIconImage;
        [SerializeField] private Image bossRewardAffinityIconImage;
        [SerializeField] private Button bossRewardNextFloorButton;
        [SerializeField] private Image utilityPortraitImage;
        [SerializeField] private Text utilityText;
        [SerializeField] private Button utilityStatusButton;
        [SerializeField] private Button utilityMapButton;
        [SerializeField] private Button utilityLoadoutButton;
        [SerializeField] private Button utilityPlayerTabButton;
        [SerializeField] private Button utilityMataiosTabButton;
        [SerializeField, Range(0.05f, 0.75f)] private float lowHpWarningRatio = 0.30f;

        private readonly List<Button> _choiceButtons = new List<Button>();
        private readonly List<Image> _mapNodeIconImages = new List<Image>();
        private readonly List<Image> _shopChoiceCardImages = new List<Image>();
        private readonly List<Image> _shopChoiceIconImages = new List<Image>();
        private readonly List<GameObject> _resultSummaryChips = new List<GameObject>();
        private readonly List<Image> _resultSummaryIconImages = new List<Image>();
        private readonly List<Text> _resultSummaryValueTexts = new List<Text>();
        private readonly List<Text> _resultSummaryFallbackTexts = new List<Text>();
        private readonly List<string> _activeResultSummaryLabels = new List<string>();
        private readonly List<string> _activeResultSummaryValues = new List<string>();
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
        private string _utilityMode = string.Empty;
        private string _utilityCharacterMode = "player";
        private string _lastResultMessage = string.Empty;
        private PrototypeRunSnapshot _lastSnapshot;
        private string _lastCombatVisualKey = string.Empty;
        private int _lastCombatVisualRound = -1;
        private int _lastCombatVisualEnemyHp = -1;
        private int _lastCombatVisualPlayerHp = -1;
        private bool _combatEnemyFeedbackBaseCaptured;
        private Vector2 _combatEnemyImageBasePosition;
        private Vector3 _combatEnemyImageBaseScale = Vector3.one;
        private bool _combatPlayerFeedbackBaseCaptured;
        private Vector2 _combatPlayerCardBasePosition;
        private Vector3 _combatPlayerCardBaseScale = Vector3.one;
        private Color _combatPlayerCardBaseColor = Color.white;
        private float _combatEnemyHitShakeTimer;
        private float _combatEnemyAttackPulseTimer;
        private float _combatEnemyDamageNumberTimer;
        private float _combatMataiosDamageNumberTimer;
        private float _combatPlayerDamageNumberTimer;
        private float _combatPlayerHitShakeTimer;
        private float _combatPlayerHpPulseTimer;
        private float _combatIntroTimer;
        private float _combatDefeatFeedbackTimer;
        private string _combatIntroKey = string.Empty;
        private string _combatDefeatFeedbackKey = string.Empty;

        private enum NpcSpotlightMode
        {
            Shop,
            Rest,
            Event
        }

        private readonly struct ResultSummaryEntry
        {
            public ResultSummaryEntry(string label, string value, string iconKey, Color fallbackColor)
            {
                Label = label;
                Value = value;
                IconKey = iconKey;
                FallbackColor = fallbackColor;
            }

            public string Label { get; }
            public string Value { get; }
            public string IconKey { get; }
            public Color FallbackColor { get; }
        }

        private const float ChoiceButtonHeight = 118f;
        private const float ChoiceButtonSpacing = 130f;
        private const float EventChoiceButtonHeight = 104f;
        private const float EventChoiceButtonSpacing = 116f;
        private const float MapNodeButtonHeight = 124f;
        private const float MapNodeButtonSpacing = 132f;
        private const float MapNodeIconSize = 84f;
        private const int TitleFontSize = 34;
        private const int SubtitleFontSize = 30;
        private const int BodyFontSize = 28;
        private const int ButtonFontSize = 30;
        private const int ResultFontSize = 28;
        private const int StatFontSize = 26;
        private const int CaptionFontSize = 23;
        private const int CombatBodyFontSize = 24;
        private const float CombatActionButtonSize = 118f;
        private const int ChoiceFontSize = ButtonFontSize;
        private const int MapNodeFontSize = 29;
        private const int ResultLineLimit = 3;
        private const int ResultIconChipCount = 6;
        private const float DenseLineSpacing = 0.92f;
        private const float CombatEnemyHitShakeDuration = 0.18f;
        private const float CombatEnemyAttackPulseDuration = 0.16f;
        private const float CombatPlayerHitShakeDuration = 0.20f;
        private const float CombatPlayerHpPulseDuration = 0.22f;
        private const float CombatDamageNumberDuration = 0.70f;
        private const float CombatIntroDuration = 0.45f;
        private const float CombatDefeatFeedbackDuration = 2.0f;
        private const float CombatEnemyHitShakePixels = 11f;
        private const float CombatPlayerHitShakePixels = 9f;
        private const float CombatEnemyAttackPulseScale = 1.045f;
        private const float CombatPlayerHpPulseScale = 1.06f;
        private static readonly Color PrimaryTextColor = new Color(0.90f, 0.95f, 0.96f, 1f);
        private static readonly Color ResultTextColor = new Color(0.88f, 0.93f, 0.95f, 1f);
        private static readonly Color PanelColor = new Color(0.035f, 0.045f, 0.055f, 0.88f);

        public int ChoiceButtonCount => _choiceButtons.Count;
        public string ResultMessage => resultText == null ? string.Empty : resultText.text;
        public string RunStateMessage => runStateText == null ? string.Empty : runStateText.text;
        public string RouteMessage => routeText == null ? string.Empty : routeText.text;
        public string MemoryMessage => memoryText == null ? string.Empty : memoryText.text;
        public string CombatMessage => combatText == null ? string.Empty : combatText.text;
        public string CombatThreatReadoutMessage => combatUiController == null || combatUiController.ThreatReadoutText == null
            ? string.Empty
            : combatUiController.ThreatReadoutText.text;
        public string CombatEnemyTitleMessage => combatEnemyTitleText == null ? string.Empty : combatEnemyTitleText.text;
        public bool CombatPanelVisible => combatPanel != null && combatPanel.gameObject.activeSelf;
        public bool CombatEnemyVisible => combatEnemyImage != null && combatEnemyImage.gameObject.activeInHierarchy;
        public bool PortraitVisible => npcPortraitImage != null && npcPortraitImage.gameObject.activeSelf;
        public bool NodeMapVisible => (floorMapUiController != null && floorMapUiController.Visible) ||
            (nodeMapLayer != null && nodeMapLayer.gameObject.activeInHierarchy);
        public bool ResultPanelVisible => resultLayer != null && resultLayer.gameObject.activeInHierarchy;
        public bool RouteHeaderVisible => routeText != null && routeText.gameObject.activeInHierarchy;
        public bool MemoryPanelVisible => memoryText != null && memoryText.gameObject.activeInHierarchy;
        public bool RouteActionButtonVisible => routeActionButton != null && routeActionButton.gameObject.activeSelf;
        public bool EndingRestButtonVisible => endingRestButton != null && endingRestButton.gameObject.activeSelf;
        public bool EndingContinueButtonVisible => endingContinueButton != null && endingContinueButton.gameObject.activeSelf;
        public bool RestInteractionPanelVisible => restInteractionPanel != null && restInteractionPanel.gameObject.activeSelf;
        public string RestResponseMessage => restResponseText == null ? string.Empty : restResponseText.text;
        public bool RawDebugTextVisible => showRawDebugText;
        public bool HasPresentationData => presentationData != null;
        public bool HasPortraitRoot => portraitRoot != null && portraitRoot.gameObject.activeInHierarchy;
        public Vector2 PortraitRootSize => portraitRoot == null ? Vector2.zero : portraitRoot.sizeDelta;
        public string CurrentBackgroundSpriteName => encounterBackgroundImage != null && encounterBackgroundImage.gameObject.activeInHierarchy && encounterBackgroundImage.sprite != null ? encounterBackgroundImage.sprite.name : string.Empty;
        public string CurrentCombatEnemySpriteName => combatEnemyImage != null && combatEnemyImage.sprite != null ? combatEnemyImage.sprite.name : string.Empty;
        public string CurrentCombatPlayerPortraitSpriteName => combatPlayerPortraitImage != null && combatPlayerPortraitImage.sprite != null ? combatPlayerPortraitImage.sprite.name : string.Empty;
        public string CurrentCombatMataiosPortraitSpriteName => combatMataiosPortraitImage != null && combatMataiosPortraitImage.sprite != null ? combatMataiosPortraitImage.sprite.name : string.Empty;
        public string CurrentCombatPortraitFrameSpriteName => combatPlayerPortraitFrameImage != null && combatPlayerPortraitFrameImage.sprite != null ? combatPlayerPortraitFrameImage.sprite.name : string.Empty;
        public bool CombatPlayerPortraitVisible => combatPlayerPortraitImage != null && combatPlayerPortraitImage.gameObject.activeInHierarchy;
        public bool CombatMataiosPortraitVisible => combatMataiosPortraitImage != null && combatMataiosPortraitImage.gameObject.activeInHierarchy;
        public bool CombatPortraitFrameVisible => combatPlayerPortraitFrameImage != null && combatPlayerPortraitFrameImage.gameObject.activeInHierarchy;
        public bool CombatPartyDockVisible => combatPartyDock != null && combatPartyDock.gameObject.activeInHierarchy;
        public string CombatPartyMessage => ((combatPlayerCardText == null ? string.Empty : combatPlayerCardText.text) + "\n" + (combatMataiosCardText == null ? string.Empty : combatMataiosCardText.text)).Trim();
        public string CombatEnemyStatusMessage => combatEnemyStatusText == null ? string.Empty : combatEnemyStatusText.text;
        public bool CombatItemInspectButtonVisible => combatItemInspectButton != null && combatItemInspectButton.gameObject.activeInHierarchy;
        public bool CombatItemInspectVisible => combatItemInspectPanel != null && combatItemInspectPanel.gameObject.activeInHierarchy;
        public string CombatItemInspectMessage => combatItemInspectText == null ? string.Empty : combatItemInspectText.text;
        public bool PreRunPlaceholderVisible => preRunPlaceholderLayer != null && preRunPlaceholderLayer.gameObject.activeInHierarchy;
        public bool LowHpWarningVisible => lowHpWarningImage != null && lowHpWarningImage.gameObject.activeInHierarchy;
        public bool LevelRewardPopupVisible => levelRewardPanel != null && levelRewardPanel.gameObject.activeInHierarchy;
        public string LevelRewardPopupMessage => ((levelRewardTitleText == null ? string.Empty : levelRewardTitleText.text) + "\n" +
            (levelRewardBodyText == null ? string.Empty : levelRewardBodyText.text) + "\n" +
            ResolveButtonLabel(levelRewardAttackButton) + "\n" +
            ResolveButtonLabel(levelRewardMaxHpButton) + "\n" +
            ResolveButtonLabel(levelRewardSkillButton)).Trim();
        public string CombatDamageNumberMessage => ((combatEnemyDamageNumberText == null || !combatEnemyDamageNumberText.gameObject.activeInHierarchy ? string.Empty : combatEnemyDamageNumberText.text) + "|" +
            (combatMataiosDamageNumberText == null || !combatMataiosDamageNumberText.gameObject.activeInHierarchy ? string.Empty : combatMataiosDamageNumberText.text) + "|" +
            (combatPlayerDamageNumberText == null || !combatPlayerDamageNumberText.gameObject.activeInHierarchy ? string.Empty : combatPlayerDamageNumberText.text)).Trim('|');
        public bool CombatIntroOverlayVisible => combatIntroOverlay != null && combatIntroOverlay.gameObject.activeInHierarchy;
        public bool CombatPlayerHitFeedbackActive => _combatPlayerHitShakeTimer > 0f || _combatPlayerHpPulseTimer > 0f;
        public bool CombatDefeatFeedbackVisible => combatDefeatFeedbackText != null && combatDefeatFeedbackText.gameObject.activeInHierarchy;
        public string CombatDefeatFeedbackMessage => combatDefeatFeedbackText == null ? string.Empty : combatDefeatFeedbackText.text;
        public bool BossRewardPopupVisible => bossRewardPanel != null && bossRewardPanel.gameObject.activeInHierarchy;
        public string BossRewardPopupMessage => ((bossRewardTitleText == null ? string.Empty : bossRewardTitleText.text) + "\n" +
            (bossRewardGoldText == null ? string.Empty : bossRewardGoldText.text) + "\n" +
            (bossRewardAffinityText == null ? string.Empty : bossRewardAffinityText.text)).Trim();
        public bool BossRewardNextFloorButtonVisible => bossRewardNextFloorButton != null && bossRewardNextFloorButton.gameObject.activeInHierarchy;
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
        public string CombatActionButtonLabels => string.Join("|", new[]
        {
            ResolveButtonLabel(attackButton),
            ResolveButtonLabel(defendButton),
            ResolveButtonLabel(skillButton)
        });
        public string CurrentTopHudIconNames => string.Join("|", new[]
        {
            topGoldIconImage != null && topGoldIconImage.gameObject.activeInHierarchy && topGoldIconImage.sprite != null ? topGoldIconImage.sprite.name : string.Empty,
            topMemoryIconImage != null && topMemoryIconImage.gameObject.activeInHierarchy && topMemoryIconImage.sprite != null ? topMemoryIconImage.sprite.name : string.Empty,
            topAffinityIconImage != null && topAffinityIconImage.gameObject.activeInHierarchy && topAffinityIconImage.sprite != null ? topAffinityIconImage.sprite.name : string.Empty
        });
        public Vector2 RouteHeaderAnchorMin => routeText == null ? Vector2.zero : routeText.GetComponent<RectTransform>().anchorMin;
        public Vector2 RouteHeaderAnchorMax => routeText == null ? Vector2.zero : routeText.GetComponent<RectTransform>().anchorMax;
        public Vector2 NodeMapLayerAnchorMax => nodeMapLayer == null ? Vector2.zero : nodeMapLayer.anchorMax;
        public Vector2 RestInteractionAnchorMin => restInteractionPanel == null ? Vector2.zero : restInteractionPanel.anchorMin;
        public string CurrentShopChoiceCardSpriteNames => JoinImageSpriteNames(_shopChoiceCardImages);
        public string CurrentShopChoiceIconNames => JoinImageSpriteNames(_shopChoiceIconImages);
        public string CurrentResultSummaryIconNames => JoinImageSpriteNames(_resultSummaryIconImages);
        public string CurrentResultSummaryLabels => string.Join("|", _activeResultSummaryLabels);
        public string CurrentResultSummaryValues => string.Join("|", _activeResultSummaryValues);
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
        public float CurrentEnemyHpFillAmount => enemyHpFill == null ? -1f : enemyHpFill.fillAmount;
        public float CurrentPlayerHpFillAmount => playerHpFill == null ? -1f : playerHpFill.fillAmount;
        public float CurrentMataiosHpFillAmount => mataiosHpFill == null ? -1f : mataiosHpFill.fillAmount;
        public bool EventCutsceneVisible => eventCutscenePanel != null && eventCutscenePanel.gameObject.activeInHierarchy;
        public string EventCutsceneMessage => ((eventHeaderText == null ? string.Empty : eventHeaderText.text) + "\n" + (eventBodyText == null ? string.Empty : eventBodyText.text)).Trim();
        public bool UtilityPanelVisible => utilityPanel != null && utilityPanel.gameObject.activeInHierarchy;
        public string UtilityPanelMessage => utilityText == null ? string.Empty : utilityText.text;
        public string UtilityButtonLabels => string.Join("|", new[]
        {
            ResolveButtonLabel(utilityStatusButton),
            ResolveButtonLabel(utilityMapButton),
            ResolveButtonLabel(utilityLoadoutButton)
        });
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
                if (floorMapUiController != null && !string.IsNullOrEmpty(floorMapUiController.NodeIconSpriteNames))
                {
                    return floorMapUiController.NodeIconSpriteNames;
                }

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
            RefreshResultSummaryIcons(_lastResultMessage);
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
            RefreshResultSummaryIcons(_lastResultMessage);
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
            UpdateCombatIntro(Time.unscaledDeltaTime);
            UpdateCombatDefeatFeedback(Time.unscaledDeltaTime);
            UpdateCombatEnemyFeedbackAnimation(Time.unscaledDeltaTime);
            UpdateCombatPlayerFeedbackAnimation(Time.unscaledDeltaTime);
            UpdateCombatDamageNumberAnimation(Time.unscaledDeltaTime);
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

        public Button GetUtilityButton(string mode)
        {
            return mode switch
            {
                "status" => utilityStatusButton,
                "map" => utilityMapButton,
                "loadout" => utilityLoadoutButton,
                _ => null
            };
        }

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public void OpenQaRouteStep(EncounterSelection selection)
        {
            if (_roomController != null && _roomController.PreRunPlaceholderPending)
            {
                _roomController.ConfirmPreRunPlaceholder();
                HidePreRunPlaceholder();
            }

            OpenSelectedRouteStep(selection);
        }
#endif

        public void ShowChoices(EncounterData encounter, PrototypeEncounterChoiceView[] choiceViews, Action<string> onChoiceSelected)
        {
            ClearChoices();
            HideRestInteractionPanel();
            EnsureScreenLayers();
            HideEventCutsceneLayout();
            HideUtilityPanel();
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

            var bossGate = IsBossGateEncounter(encounter);
            if (bossGate)
            {
                choiceViews = BuildBossGateChoiceViews(choiceViews);
                ConfigureChoiceContainerForBossGate();
            }

            for (var i = 0; i < choiceViews.Length; i++)
            {
                var view = choiceViews[i];
                if (!view.Visible)
                {
                    continue;
                }

                if (!showRawDebugText &&
                    encounter != null &&
                    encounter.Type == EncounterType.Battle &&
                    ChoiceStartsCombat(encounter, view.ChoiceStableId))
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
                    ConfigureChoiceContainerForShop();
                }
                else
                {
                    _eventPresentationActive = false;
                    _shopPresentationActive = false;
                    HideMerchantPresentation();
                    if (!bossGate)
                    {
                        ConfigureChoiceContainerDefault();
                    }
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
                    DestroyHudObject(_choiceButtons[i].gameObject);
                }
            }

            _choiceButtons.Clear();
            _mapNodeIconImages.Clear();
            _shopChoiceCardImages.Clear();
            _shopChoiceIconImages.Clear();
            HideFloorMapUi();
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
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            HideMerchantPresentation();
            HideNpcSpotlight();
            EnsureEventSystem();
            EnsureFloorMapUi(onNodeSelected);
            if (floorMapUiController == null || nodes == null)
            {
                return;
            }

            var snapshot = _roomController == null ? default(PrototypeRunSnapshot) : _roomController.GetSnapshot();
            floorMapUiController.Show(snapshot, nodes);
            for (var i = 0; i < floorMapUiController.NodeButtons.Count; i++)
            {
                _choiceButtons.Add(floorMapUiController.NodeButtons[i]);
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

        private void EnsureFloorMapUi(Action<string> onNodeSelected)
        {
            if (floorMapUiController == null)
            {
                var moduleObject = new GameObject("Floor Map UI Module", typeof(RectTransform));
                moduleObject.transform.SetParent(HudParent, false);
                floorMapUiController = moduleObject.AddComponent<FloorMapUiController>();
            }

            floorMapUiController.Initialize(
                HudParent,
                onNodeSelected,
                OpenCurrentRouteStep,
                HideFloorMapUi,
                ResolveNodeIcon);
        }

        private void HideFloorMapUi()
        {
            if (floorMapUiController != null)
            {
                floorMapUiController.Hide();
            }
        }

        private void ClearMapDecorations()
        {
            for (var i = 0; i < _mapDecorations.Count; i++)
            {
                if (_mapDecorations[i] != null)
                {
                    DestroyHudObject(_mapDecorations[i]);
                }
            }

            _mapDecorations.Clear();
        }

        private static void DestroyHudObject(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
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

            var parentRect = nodeMapLayer.rect;
            var parentWidth = parentRect.width > 1f ? parentRect.width : 960f;
            var parentHeight = parentRect.height > 1f ? parentRect.height : 500f;
            var startPixels = new Vector2(start.x * parentWidth, start.y * parentHeight);
            var endPixels = new Vector2(end.x * parentWidth, end.y * parentHeight);
            var rawDelta = endPixels - startPixels;
            var rawLength = rawDelta.magnitude;
            if (rawLength < 8f)
            {
                Destroy(lineObject);
                return;
            }

            var direction = rawDelta / rawLength;
            var inset = Mathf.Min(MapNodeButtonHeight * 0.44f, rawLength * 0.32f);
            startPixels += direction * inset;
            endPixels -= direction * inset;
            var adjustedDelta = endPixels - startPixels;
            var adjustedCenter = (startPixels + endPixels) * 0.5f;
            var center = new Vector2(adjustedCenter.x / parentWidth, adjustedCenter.y / parentHeight);
            rect.anchorMin = center;
            rect.anchorMax = center;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(Mathf.Max(12f, adjustedDelta.magnitude), 3.5f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(adjustedDelta.y, adjustedDelta.x) * Mathf.Rad2Deg);

            var image = lineObject.AddComponent<Image>();
            image.color = ResolveMapRouteLineColor(from, to);
            image.raycastTarget = false;
            _mapDecorations.Add(lineObject);
        }

        private static Color ResolveMapRouteLineColor(PrototypeFloorMapNodeView from, PrototypeFloorMapNodeView to)
        {
            if (from.Completed && !to.Locked)
            {
                return new Color(0.82f, 0.96f, 0.74f, 0.86f);
            }

            if (from.Current || from.Selectable || to.Selectable)
            {
                return new Color(0.76f, 0.84f, 0.92f, 0.64f);
            }

            return new Color(0.44f, 0.50f, 0.56f, 0.34f);
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
            EnsureResultIconStrip();
            _lastResultMessage = message ?? string.Empty;
            if (resultText == null)
            {
                return;
            }

            resultText.text = string.IsNullOrEmpty(message)
                ? "결과\n-"
                : showRawDebugText ? BuildResultSummary(message) : SanitizePublicText(BuildResultSummary(message));
            RefreshResultSummaryIcons(_lastResultMessage);
        }

        public void ShowRunState(PrototypeRunSnapshot snapshot)
        {
            _lastSnapshot = snapshot;
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
                runStateText.text = $"Floor {snapshot.CurrentFloor}{status}  HP {snapshot.PlayerHp}/{snapshot.PlayerMaxHp}  Gold {snapshot.Gold}  이성 {snapshot.Mental}";
            }

            if (snapshot.RunCompleted || snapshot.IsInCombat)
            {
                ClearChoices();
            }

            EnsureEventSystem();
            EnsureLowHpWarning();
            UpdateLowHpWarning(snapshot);
            if (_roomController != null && _roomController.PreRunPlaceholderPending)
            {
                ShowPreRunPlaceholder();
                return;
            }

            HidePreRunPlaceholder();
            if (snapshot.IsInCombat)
            {
                HideMapAndResultSurfacesForCombat();
            }

            if (!snapshot.IsInCombat && snapshot.LastCombatEnemyDefeated)
            {
                EnsureCombatPanel();
                StartCombatDefeatFeedbackIfNeeded(snapshot);
            }

            AutoShowMapIfNeeded(snapshot);
            UpdateScreenLayers(snapshot);
            UpdateBossRewardPopup(snapshot);
            UpdateLevelRewardPopup(snapshot);
            UpdateNextFloorButton(snapshot);
            UpdateRouteActionButton(snapshot);
            UpdateRestartButton(snapshot);
            UpdateEndingButtons(snapshot);
            UpdatePresentationState(snapshot);
            UpdateRouteIndicator(snapshot);
            UpdatePrimaryHeaderVisibility(snapshot);
            UpdateTopHudIcons(snapshot);
            UpdateUtilityUi(snapshot);
            UpdateMemoryAndCombatPanel(snapshot);
            UpdateResultVisibility(snapshot);
            UpdateDemoCompletePanel(snapshot);
            UpdateCutsceneTriggers(snapshot);
            AutoShowMapIfNeeded(snapshot);
        }

        private void UpdateScreenLayers(PrototypeRunSnapshot snapshot)
        {
            EnsureScreenLayers();
            var restVisible = RestInteractionPanelVisible;
            var mapVisible = IsMapRouteState(snapshot);
            var shopVisible = !snapshot.IsInCombat && !restVisible && _shopPresentationActive;
            var eventVisible = _eventPresentationActive;
            SetLayerVisible(topStatusLayer, true);
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, !snapshot.IsInCombat && !eventVisible && !mapVisible);
            SetLayerVisible(nodeMapLayer, mapVisible && floorMapUiController == null);
            if (floorMapUiController != null && floorMapUiController.Root != null)
            {
                SetLayerVisible(floorMapUiController.Root, mapVisible);
            }
            SetLayerVisible(npcReactionLayer, showRawDebugText && !snapshot.IsInCombat && !snapshot.EndingChoicePending && !eventVisible && !mapVisible && !shopVisible && !restVisible);
            SetLayerVisible(actionLayer, !snapshot.IsInCombat && !snapshot.RunCompleted && !mapVisible);
            SetLayerVisible(resultLayer, !snapshot.IsInCombat && !eventVisible && !mapVisible && !shopVisible && !restVisible && resultText != null && resultText.gameObject.activeSelf);
            SetLayerVisible(endingLayer, snapshot.EndingChoicePending && !snapshot.IsInCombat);
        }

        private bool IsMapSelectionVisible(PrototypeRunSnapshot snapshot)
        {
            if (!IsMapRouteState(snapshot))
            {
                return false;
            }

            if (floorMapUiController != null && floorMapUiController.Visible && floorMapUiController.NodeButtons.Count > 0)
            {
                return true;
            }

            return choiceContainer != null &&
                nodeMapLayer != null &&
                choiceContainer.parent == nodeMapLayer &&
                _choiceButtons.Count > 0;
        }

        private bool IsMapRouteState(PrototypeRunSnapshot snapshot)
        {
            return snapshot.HasFloorMap &&
                !snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !snapshot.StairUnlocked &&
                !snapshot.HasSelectedMapNode &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive;
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
                _shopPresentationActive ||
                _eventPresentationActive)
            {
                return;
            }

            if (IsMapSelectionVisible(snapshot))
            {
                return;
            }

            if (_choiceButtons.Count > 0)
            {
                ClearChoices();
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

        private void ShowPreRunPlaceholder()
        {
            EnsurePreRunPlaceholderLayer();
            if (preRunPlaceholderLayer == null)
            {
                return;
            }

            ClearChoices();
            HideRestInteractionPanel();
            HideEventCutsceneLayout();
            HideMerchantPresentation();
            HideNpcSpotlight();
            HideSkillPicker();
            HideCombatItemInspect();
            EnsureScreenLayers();
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            SetLayerVisible(endingLayer, false);
            if (combatPanel != null)
            {
                combatPanel.gameObject.SetActive(false);
            }

            preRunPlaceholderLayer.gameObject.SetActive(true);
        }

        private void HidePreRunPlaceholder()
        {
            if (preRunPlaceholderLayer != null)
            {
                preRunPlaceholderLayer.gameObject.SetActive(false);
            }
        }

        private void EnsurePreRunPlaceholderLayer()
        {
            if (preRunPlaceholderLayer != null)
            {
                return;
            }

            var layerObject = new GameObject("Pre Run Placeholder Stepper");
            layerObject.transform.SetParent(HudParent, false);
            preRunPlaceholderLayer = layerObject.AddComponent<RectTransform>();
            preRunPlaceholderLayer.anchorMin = Vector2.zero;
            preRunPlaceholderLayer.anchorMax = Vector2.one;
            preRunPlaceholderLayer.offsetMin = Vector2.zero;
            preRunPlaceholderLayer.offsetMax = Vector2.zero;

            var background = layerObject.AddComponent<Image>();
            background.color = new Color(0.014f, 0.019f, 0.026f, 0.98f);
            background.raycastTarget = true;

            var title = CreateHudText("Pre Run Title", new Vector2(0.08f, 0.83f), new Vector2(0.92f, 0.90f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, TitleFontSize, TextAnchor.MiddleCenter, PrimaryTextColor);
            title.transform.SetParent(layerObject.transform, false);
            title.text = "출발 준비";

            CreatePlaceholderFrame(layerObject.transform, "Pre Run Art Slot", new Vector2(0.10f, 0.41f), new Vector2(0.90f, 0.79f), "이미지 슬롯");
            CreatePlaceholderFrame(layerObject.transform, "Pre Run Memory Slot", new Vector2(0.10f, 0.22f), new Vector2(0.48f, 0.36f), "기억 계승\n비활성");
            CreatePlaceholderFrame(layerObject.transform, "Pre Run Equipment Slot", new Vector2(0.52f, 0.22f), new Vector2(0.90f, 0.36f), "장비 선택\n비활성");

            var ctaObject = new GameObject("Pre Run Confirm Button");
            ctaObject.transform.SetParent(layerObject.transform, false);
            var rect = ctaObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.18f, 0.08f);
            rect.anchorMax = new Vector2(0.82f, 0.16f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = ctaObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.25f, 0.29f, 0.98f);
            var button = ctaObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(ConfirmPreRunPlaceholder);
            var label = CreateHudText("Pre Run Confirm Label", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, ButtonFontSize, TextAnchor.MiddleCenter, PrimaryTextColor);
            label.transform.SetParent(ctaObject.transform, false);
            label.text = "시작";
        }

        private void CreatePlaceholderFrame(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string labelText)
        {
            var frameObject = new GameObject(name);
            frameObject.transform.SetParent(parent, false);
            var rect = frameObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = frameObject.AddComponent<Image>();
            image.color = new Color(0.045f, 0.055f, 0.064f, 0.90f);
            image.raycastTarget = false;
            var label = CreateHudText(name + " Label", new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.92f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, CaptionFontSize, TextAnchor.MiddleCenter, new Color(0.63f, 0.68f, 0.70f, 1f));
            label.transform.SetParent(frameObject.transform, false);
            label.text = labelText;
        }

        private void ConfirmPreRunPlaceholder()
        {
            _roomController?.ConfirmPreRunPlaceholder();
            HidePreRunPlaceholder();
            if (_roomController == null)
            {
                return;
            }

            var snapshot = _roomController.GetSnapshot();
            if (snapshot.HasFloorMap && !snapshot.RunCompleted)
            {
                ShowMapChoices(_roomController.GetFloorMapNodes(), mapNodeId =>
                {
                    var selected = _roomController.SelectMapNode(mapNodeId);
                    OpenSelectedRouteStep(selected);
                });
            }

            ShowRunState(_roomController.GetSnapshot());
        }

        private void EnsureLowHpWarning()
        {
            if (lowHpWarningImage != null)
            {
                return;
            }

            var warningObject = new GameObject("Low HP Edge Warning");
            warningObject.transform.SetParent(HudParent, false);
            warningObject.transform.SetAsLastSibling();
            var rect = warningObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            lowHpWarningImage = warningObject.AddComponent<Image>();
            lowHpWarningImage.color = new Color(0.80f, 0.06f, 0.05f, 0.16f);
            lowHpWarningImage.raycastTarget = false;
            lowHpWarningImage.gameObject.SetActive(false);
        }

        private void UpdateLowHpWarning(PrototypeRunSnapshot snapshot)
        {
            if (lowHpWarningImage == null)
            {
                return;
            }

            var visible = !string.IsNullOrEmpty(snapshot.RunId) &&
                snapshot.PlayerMaxHp > 0 &&
                snapshot.PlayerHp > 0 &&
                snapshot.PlayerHp / (float)snapshot.PlayerMaxHp <= lowHpWarningRatio;
            lowHpWarningImage.gameObject.SetActive(visible);
            if (visible)
            {
                lowHpWarningImage.transform.SetAsLastSibling();
            }
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
                HideEventCutsceneLayout();
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
            label.text = BuildShopCardLabel(view, label.text);

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

        private string BuildShopCardLabel(PrototypeEncounterChoiceView view, string currentLabel)
        {
            var gold = _roomController == null ? _lastSnapshot.Gold : _roomController.GetSnapshot().Gold;
            var label = SanitizePublicText(currentLabel);
            if (!label.Contains("보유 Gold", StringComparison.Ordinal))
            {
                label += "\n보유 Gold " + gold;
            }

            if (!view.Enabled && !label.Contains("Gold 부족", StringComparison.Ordinal))
            {
                label += "\nGold 부족";
            }

            return SanitizePublicText(label);
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
            resultLayer = EnsureLayerPanel(resultLayer, "Screen Layer Result", new Vector2(0.06f, 0.285f), new Vector2(0.94f, 0.405f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.035f, 0.045f, 0.055f, 0.90f), false);
            nodeMapLayer = EnsureLayerPanel(nodeMapLayer, "Screen Layer Node Map", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.80f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.030f, 0.040f, 0.050f, 0.90f), false);
            actionLayer = EnsureLayerPanel(actionLayer, "Screen Layer Action", new Vector2(0.06f, 0.045f), new Vector2(0.94f, 0.265f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.02f, 0.025f, 0.03f, 0.50f), false);
            endingLayer = EnsureLayerPanel(endingLayer, "Screen Layer Ending", new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.30f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.06f, 0.055f, 0.04f, 0.90f), false);
        }

        private void EnsureTopHudIcons()
        {
            EnsureScreenLayers();
            topGoldIconImage = EnsureHudIcon(topGoldIconImage, "Top Gold Icon", topStatusLayer, new Vector2(0.62f, 0.50f), 36f);
            topMemoryIconImage = EnsureHudIcon(topMemoryIconImage, "Top Memory Icon", topStatusLayer, new Vector2(0.28f, 0.24f), 32f);
            topAffinityIconImage = EnsureHudIcon(topAffinityIconImage, "Top Affinity Icon", topStatusLayer, new Vector2(0.48f, 0.24f), 32f);
            topPlayerProfileImage = EnsureHudIcon(topPlayerProfileImage, "Top Player Profile", topStatusLayer, new Vector2(0.052f, 0.50f), 68f);
            topMataiosProfileImage = EnsureHudIcon(topMataiosProfileImage, "Top Mataios Profile", topStatusLayer, new Vector2(0.128f, 0.50f), 68f);
        }

        private void EnsureUtilityUi()
        {
            EnsureScreenLayers();
            utilityStatusButton = EnsureUtilityButton(utilityStatusButton, "Utility Button Status", "상태", new Vector2(0.74f, 0.50f), () => ToggleUtilityPanel("status"));
            utilityMapButton = EnsureUtilityButton(utilityMapButton, "Utility Button Map", "지도", new Vector2(0.84f, 0.50f), OpenUtilityMap);
            utilityLoadoutButton = EnsureUtilityButton(utilityLoadoutButton, "Utility Button Loadout", "장비", new Vector2(0.94f, 0.50f), () => ToggleUtilityPanel("equipment"));
            SetButtonLabel(utilityLoadoutButton, "장비");

            if (utilityPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Utility Panel");
            panelObject.transform.SetParent(HudParent, false);
            utilityPanel = panelObject.AddComponent<RectTransform>();
            utilityPanel.anchorMin = new Vector2(0.10f, 0.16f);
            utilityPanel.anchorMax = new Vector2(0.90f, 0.86f);
            utilityPanel.offsetMin = Vector2.zero;
            utilityPanel.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.018f, 0.022f, 0.026f, 0.97f);
            image.raycastTarget = false;

            utilityPlayerTabButton = CreateUtilityTabButton(utilityPanel, "Utility Player Tab", "플레이어", new Vector2(0.10f, 0.88f), () =>
            {
                _utilityCharacterMode = "player";
                RefreshUtilityPanel(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot());
            });
            utilityMataiosTabButton = CreateUtilityTabButton(utilityPanel, "Utility Mataios Tab", "마타이오스", new Vector2(0.36f, 0.88f), () =>
            {
                _utilityCharacterMode = "mataios";
                RefreshUtilityPanel(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot());
            });
            utilityPortraitImage = CreateCombatImage(utilityPanel, "Utility Portrait", new Vector2(0.08f, 0.30f), new Vector2(0.46f, 0.78f));
            utilityText = CreateCombatChildText(utilityPanel, "Utility Panel Text", new Vector2(0.50f, 0.18f), new Vector2(0.94f, 0.80f), 25, TextAnchor.MiddleLeft);
            utilityText.color = new Color(0.88f, 0.94f, 0.95f, 1f);
            utilityPanel.gameObject.SetActive(false);
        }

        private Button CreateUtilityTabButton(Transform parent, string name, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(220f, 58f);
            rect.anchoredPosition = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.10f, 0.14f, 0.17f, 0.96f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            var text = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 24, TextAnchor.MiddleCenter);
            text.text = label;
            return button;
        }

        private Button EnsureUtilityButton(Button current, string name, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
        {
            if (current != null)
            {
                return current;
            }

            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(topStatusLayer == null ? HudParent : topStatusLayer, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(86f, 54f);
            rect.anchoredPosition = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.10f, 0.14f, 0.17f, 0.96f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(6f, 3f);
            labelRect.offsetMax = new Vector2(-6f, -3f);

            var text = labelObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = 22;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            text.resizeTextMaxSize = 22;
            text.raycastTarget = false;
            text.color = new Color(0.94f, 0.97f, 0.98f, 1f);
            text.text = label;
            return button;
        }

        private void UpdateUtilityUi(PrototypeRunSnapshot snapshot)
        {
            EnsureUtilityUi();
            var visible = !string.IsNullOrEmpty(snapshot.RunId) && !snapshot.RunCompleted;
            var routeResultLocked = snapshot.StairUnlocked || snapshot.RunClear || snapshot.RunFailed;
            SetButtonVisible(utilityStatusButton, visible && !routeResultLocked);
            SetButtonVisible(utilityMapButton, visible && !routeResultLocked);
            SetButtonVisible(utilityLoadoutButton, visible);
            var mapInteractable = visible &&
                !routeResultLocked &&
                snapshot.HasFloorMap &&
                !snapshot.HasSelectedMapNode &&
                !snapshot.IsInCombat &&
                !snapshot.StairUnlocked &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive;
            if (utilityMapButton != null)
            {
                utilityMapButton.interactable = mapInteractable;
            }

            if (!visible || (routeResultLocked && (_utilityMode == "status" || _utilityMode == "map")))
            {
                HideUtilityPanel();
                return;
            }

            RefreshUtilityPanel(snapshot);
        }

        private void ToggleUtilityPanel(string mode)
        {
            _utilityMode = _utilityMode == mode ? string.Empty : mode;
            RefreshUtilityPanel(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot());
        }

        private void OpenUtilityMap()
        {
            if (_roomController == null)
            {
                return;
            }

            if (_roomController.PreRunPlaceholderPending)
            {
                _roomController.ConfirmPreRunPlaceholder();
                HidePreRunPlaceholder();
            }

            var snapshot = _roomController.GetSnapshot();
            if (!snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !snapshot.StairUnlocked &&
                !snapshot.HasSelectedMapNode &&
                snapshot.HasFloorMap)
            {
                HideUtilityPanel();
                ClearChoices();
                ShowMapChoices(_roomController.GetFloorMapNodes(), mapNodeId =>
                {
                    var selected = _roomController.SelectMapNode(mapNodeId);
                    OpenSelectedRouteStep(selected);
                });
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            _utilityMode = "map";
            RefreshUtilityPanel(snapshot);
        }

        private void RefreshUtilityPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureUtilityUi();
            if (utilityPanel == null || utilityText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_utilityMode))
            {
                utilityPanel.gameObject.SetActive(false);
                return;
            }

            utilityText.text = _utilityMode switch
            {
                "status" => BuildUtilityStatus(snapshot),
                "map" => BuildUtilityMapSummary(snapshot),
                "equipment" => BuildUtilityEquipment(snapshot),
                _ => string.Empty
            };
            utilityPanel.gameObject.SetActive(!string.IsNullOrEmpty(utilityText.text));
            RefreshUtilityPortrait(snapshot);
            RefreshUtilityTabs();
        }

        private void HideUtilityPanel()
        {
            _utilityMode = string.Empty;
            if (utilityPanel != null)
            {
                utilityPanel.gameObject.SetActive(false);
            }
        }

        private string BuildUtilityStatus(PrototypeRunSnapshot snapshot)
        {
            if (_utilityCharacterMode == "mataios")
            {
                return "마타이오스\nHP " + BuildMataiosHp(snapshot) +
                    "\nATK " + BuildMataiosAttack(snapshot) +
                    "\n버프 " + BuildMataiosBuffLine(snapshot) +
                    "\n스킬 지원 / 대화";
            }

            return "플레이어\nHP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp +
                "\nATK " + snapshot.PlayerAttack +
                "\n성장 Lv " + snapshot.CombatLevel + " XP " + snapshot.CombatXp + "/" + snapshot.CombatXpToNextLevel +
                "\n정신 " + snapshot.Mental +
                "\n스킬 " + BuildOwnedSkillLine() +
                "\n버프 " + BuildPlayerBuffChipLine(snapshot) +
                "\n" + snapshot.CombatBuildSummary;
        }

        private static string BuildUtilityMapSummary(PrototypeRunSnapshot snapshot)
        {
            var selectable = 0;
            var completed = 0;
            for (var i = 0; i < snapshot.FloorMapNodes.Length; i++)
            {
                if (snapshot.FloorMapNodes[i].Selectable)
                {
                    selectable++;
                }

                if (snapshot.FloorMapNodes[i].Completed)
                {
                    completed++;
                }
            }

            return "지도\nFloor " + snapshot.CurrentFloor +
                "\n선택 가능 " + selectable + " | 완료 " + completed +
                "\n전투 중에는 요약만 표시";
        }

        private string BuildUtilityEquipment(PrototypeRunSnapshot snapshot)
        {
            return "장비\n" + BuildOwnedItemLine() +
                "\n\n능력\n" + BuildOwnedSkillLine();
        }

        private void RefreshUtilityPortrait(PrototypeRunSnapshot snapshot)
        {
            if (utilityPortraitImage == null)
            {
                return;
            }

            utilityPortraitImage.sprite = _utilityMode == "equipment"
                ? presentationData == null ? null : ResolveIcon("item.field_bandage")
                : _utilityCharacterMode == "mataios"
                    ? presentationData == null ? null : presentationData.CombatMataiosPortrait
                    : presentationData == null ? null : presentationData.DefaultPlayerPortrait;
            utilityPortraitImage.color = utilityPortraitImage.sprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
            utilityPortraitImage.gameObject.SetActive(utilityPortraitImage.sprite != null);
        }

        private void RefreshUtilityTabs()
        {
            var showCharacterTabs = _utilityMode == "status";
            SetButtonVisible(utilityPlayerTabButton, showCharacterTabs);
            SetButtonVisible(utilityMataiosTabButton, showCharacterTabs);
            TintUtilityTab(utilityPlayerTabButton, _utilityCharacterMode == "player");
            TintUtilityTab(utilityMataiosTabButton, _utilityCharacterMode == "mataios");
        }

        private static void TintUtilityTab(Button button, bool selected)
        {
            if (button != null && button.targetGraphic is Image image)
            {
                image.color = selected
                    ? new Color(0.28f, 0.20f, 0.10f, 0.98f)
                    : new Color(0.10f, 0.14f, 0.17f, 0.96f);
            }
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

            if (resultIconStrip != null)
            {
                resultIconStrip.gameObject.SetActive(visible && _activeResultSummaryValues.Count > 0);
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
            ApplyTextRect(resultText, new Vector2(0.08f, 0.292f), new Vector2(0.92f, 0.345f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, ResultFontSize, TextAnchor.MiddleCenter);
            ApplyRouteHeaderRect();
            if (resultText != null)
            {
                resultText.lineSpacing = DenseLineSpacing;
            }

            ApplyResultIconStripRect();
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

        private void ConfigureChoiceContainerForShop()
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
            choiceContainer.sizeDelta = new Vector2(0f, 700f);
            choiceContainer.anchoredPosition = new Vector2(0f, 78f);

            var purchaseIndex = 0;
            for (var i = 0; i < _choiceButtons.Count; i++)
            {
                var button = _choiceButtons[i];
                if (button == null)
                {
                    continue;
                }

                var rect = button.GetComponent<RectTransform>();
                var isPurchase = IsPurchaseChoice(ExtractChoiceId(button.name));
                if (rect == null)
                {
                    continue;
                }

                if (isPurchase)
                {
                    var column = purchaseIndex % 2;
                    var row = purchaseIndex / 2;
                    rect.anchorMin = new Vector2(column == 0 ? 0f : 0.52f, 1f);
                    rect.anchorMax = new Vector2(column == 0 ? 0.48f : 1f, 1f);
                    rect.pivot = new Vector2(0.5f, 1f);
                    rect.sizeDelta = new Vector2(0f, 146f);
                    rect.anchoredPosition = new Vector2(0f, -row * 160f);
                    purchaseIndex++;
                    continue;
                }

                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(0f, 102f);
                rect.anchoredPosition = new Vector2(0f, -Mathf.Max(2, (purchaseIndex + 1) / 2) * 160f - 26f);
            }
        }

        private static string ExtractChoiceId(string buttonName)
        {
            return string.IsNullOrEmpty(buttonName) ? string.Empty : buttonName.Replace("Choice Button ", string.Empty, StringComparison.Ordinal);
        }

        private void ConfigureChoiceContainerForBossGate()
        {
            EnsureChoiceContainer();
            if (choiceContainer == null)
            {
                return;
            }

            choiceContainer.SetParent(HudParent, false);
            choiceContainer.anchorMin = new Vector2(0.10f, 0f);
            choiceContainer.anchorMax = new Vector2(0.90f, 0f);
            choiceContainer.pivot = new Vector2(0.5f, 0f);
            choiceContainer.sizeDelta = new Vector2(0f, 290f);
            choiceContainer.anchoredPosition = new Vector2(0f, 118f);
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
            eventCutscenePanel.anchorMin = new Vector2(0.06f, 0.20f);
            eventCutscenePanel.anchorMax = new Vector2(0.94f, 0.84f);
            eventCutscenePanel.offsetMin = Vector2.zero;
            eventCutscenePanel.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.006f, 0.007f, 0.009f, 0.94f);
            image.raycastTarget = false;

            eventHeaderText = CreateCombatChildText(eventCutscenePanel, "Event Header Text", new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.97f), 30, TextAnchor.MiddleLeft);
            eventHeaderText.color = new Color(0.92f, 0.95f, 0.94f, 1f);

            var imageFrame = CreateCombatPanelRect(eventCutscenePanel, "Event Image Frame", new Vector2(0.24f, 0.42f), new Vector2(0.76f, 0.80f), new Color(0.020f, 0.022f, 0.025f, 0.95f));
            eventCutsceneImage = CreateCombatImage(imageFrame, "Event Cutscene Image", new Vector2(0.03f, 0.03f), new Vector2(0.97f, 0.97f));
            eventCutsceneImage.color = Color.white;

            eventBodyText = CreateCombatChildText(eventCutscenePanel, "Event Body Text", new Vector2(0.06f, 0.10f), new Vector2(0.94f, 0.34f), 29, TextAnchor.MiddleLeft);
            eventBodyText.lineSpacing = 0.94f;

            eventUtilityText = CreateCombatChildText(eventCutscenePanel, "Event Utility Text", new Vector2(0.08f, 0.01f), new Vector2(0.92f, 0.07f), 24, TextAnchor.MiddleCenter);
            eventUtilityText.color = new Color(0.76f, 0.82f, 0.84f, 1f);
            eventUtilityText.text = string.Empty;
            eventUtilityText.gameObject.SetActive(false);
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
                eventUtilityText.gameObject.SetActive(false);
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

            return "이벤트\n선택 전 결과와 위험을 확인하세요.";
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
            ApplyRestPresentationSlot(selection.EncounterId);
            HideNpcSpotlight();
            RefreshRestUi();
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

        private void RefreshRestUi()
        {
            if (restUiController == null)
            {
                return;
            }

            var snapshot = _roomController == null ? default : _roomController.GetSnapshot();
            var restScene = encounterBackgroundImage == null ? null : encounterBackgroundImage.sprite;
            var restMataios = npcPortraitImage != null && npcPortraitImage.sprite != null
                ? npcPortraitImage.sprite
                : mataiosPortrait;
            restUiController.ShowBase(
                snapshot.CurrentFloor,
                snapshot.Mental,
                snapshot.PlayerHp,
                snapshot.PlayerMaxHp,
                snapshot.Gold,
                snapshot.GlitchLevel,
                restScene,
                restMataios);
        }

        private void EnsureRestInteractionPanel()
        {
            if (restUiController != null)
            {
                return;
            }

            EnsureEventSystem();
            var panelObject = new GameObject("Rest UI Module", typeof(RectTransform));
            restUiController = panelObject.AddComponent<RestUiController>();
            restUiController.Initialize(HudParent, SelectRestAction, SubmitRestInteraction, ContinueAfterRestInteraction);
            restInteractionPanel = restUiController.Root;
            restResponseText = restUiController.ResponseText;
            restInputField = restUiController.InputField;
            restAskMoodButton = restUiController.AskMoodButton;
            restTrainButton = restUiController.TrainButton;
            restRecoverButton = restUiController.RecoverButton;
            restAskMoodIconImage = restUiController.AskMoodIcon;
            restTrainIconImage = restUiController.TrainIcon;
            restRecoverIconImage = restUiController.RecoverIcon;
            restSubmitButton = restUiController.SubmitButton;
            restContinueButton = restUiController.DepartButton;
            restResponsePanel = restUiController.ResponsePanel;
            ApplyRestActionIcons();
        }

        private void ApplyRestActionIcons()
        {
            ApplyRestActionIcon(restAskMoodIconImage, "rest.ask_mood");
            ApplyRestActionIcon(restTrainIconImage, "rest.train");
            ApplyRestActionIcon(restRecoverIconImage, "rest.recover");
        }

        private void ApplyRestActionIcon(Image image, string actionId)
        {
            if (restUiController != null)
            {
                restUiController.SetActionSprite(actionId, ResolveRestActionIcon(actionId));
                return;
            }

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
            if (restUiController != null)
            {
                restUiController.SetActionSelection(_pendingRestActionId);
                return;
            }

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
                "rest.train" => "결과: 다음 전투 단련 보너스",
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
            if (restUiController != null)
            {
                restUiController.Hide();
            }

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
            if (restUiController != null)
            {
                restUiController.SetActionButtonsInteractable(interactable);
                return;
            }

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
            if (restUiController != null)
            {
                restUiController.SetChoiceCardsVisible(visible);
                return;
            }

            SetButtonVisible(restAskMoodButton, visible);
            SetButtonVisible(restTrainButton, visible);
            SetButtonVisible(restRecoverButton, visible);
        }

        private void SetRestInputPhaseVisible(bool inputVisible, bool committed)
        {
            if (restUiController != null)
            {
                restUiController.SetInputPhaseVisible(inputVisible, committed);
                return;
            }

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
            if (restUiController != null)
            {
                restUiController.SetResponseVisible(visible);
                return;
            }

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

        private void EnsureResultIconStrip()
        {
            if (resultIconStrip != null)
            {
                return;
            }

            EnsurePortraitRoot();
            if (portraitRoot == null)
            {
                return;
            }

            var stripObject = new GameObject("Encounter Result Icon Strip");
            stripObject.transform.SetParent(portraitRoot, false);
            resultIconStrip = stripObject.AddComponent<RectTransform>();
            ApplyResultIconStripRect();

            for (var i = 0; i < ResultIconChipCount; i++)
            {
                CreateResultSummaryChip(i);
            }

            resultIconStrip.gameObject.SetActive(false);
        }

        private void ApplyResultIconStripRect()
        {
            if (resultIconStrip == null)
            {
                return;
            }

            resultIconStrip.anchorMin = new Vector2(0.10f, 0.350f);
            resultIconStrip.anchorMax = new Vector2(0.90f, 0.397f);
            resultIconStrip.pivot = new Vector2(0.5f, 0.5f);
            resultIconStrip.anchoredPosition = Vector2.zero;
            resultIconStrip.sizeDelta = Vector2.zero;
        }

        private void CreateResultSummaryChip(int index)
        {
            var chipObject = new GameObject("Result Summary Chip " + index);
            chipObject.transform.SetParent(resultIconStrip, false);
            var rect = chipObject.AddComponent<RectTransform>();
            var min = index / (float)ResultIconChipCount;
            var max = (index + 1) / (float)ResultIconChipCount;
            rect.anchorMin = new Vector2(min, 0f);
            rect.anchorMax = new Vector2(max, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(-8f, 0f);

            var background = chipObject.AddComponent<Image>();
            background.color = new Color(0.06f, 0.075f, 0.085f, 0.88f);
            background.raycastTarget = false;

            var iconObject = new GameObject("Icon");
            iconObject.transform.SetParent(chipObject.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.06f, 0.18f);
            iconRect.anchorMax = new Vector2(0.38f, 0.82f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = Vector2.zero;
            var icon = iconObject.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var fallbackText = CreateResultChipText(chipObject.transform, "Icon Fallback", new Vector2(0.06f, 0.18f), new Vector2(0.38f, 0.82f), 17, TextAnchor.MiddleCenter);
            fallbackText.color = Color.white;

            var valueText = CreateResultChipText(chipObject.transform, "Value", new Vector2(0.39f, 0f), new Vector2(0.98f, 1f), 24, TextAnchor.MiddleLeft);

            _resultSummaryChips.Add(chipObject);
            _resultSummaryIconImages.Add(icon);
            _resultSummaryFallbackTexts.Add(fallbackText);
            _resultSummaryValueTexts.Add(valueText);
        }

        private Text CreateResultChipText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            var text = textObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 13;
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
            text.raycastTarget = false;
            text.color = ResultTextColor;
            return text;
        }

        private void EnsureRouteText()
        {
            if (routeText != null)
            {
                ApplyRouteHeaderRect();
                return;
            }

            routeText = CreateHudText("Demo Route Text", new Vector2(0.08f, 0.845f), new Vector2(0.92f, 0.900f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, 34, TextAnchor.MiddleCenter, new Color(0.88f, 0.94f, 0.98f, 1f));
            ApplyRouteHeaderRect();
        }

        private void ApplyRouteHeaderRect()
        {
            if (routeText == null)
            {
                return;
            }

            ApplyTextRect(routeText, new Vector2(0.08f, 0.845f), new Vector2(0.92f, 0.900f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, 34, TextAnchor.MiddleCenter);
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
                ApplyRect(characterRect, new Vector2(0.23f, -0.08f), new Vector2(0.77f, 0.99f));
                ApplyRect(plateRect, new Vector2(0.12f, 0.04f), new Vector2(0.88f, 0.22f));
                ApplyRect(npcSpotlightGlowImage == null ? null : npcSpotlightGlowImage.GetComponent<RectTransform>(), new Vector2(0.18f, 0.03f), new Vector2(0.82f, 0.98f));
                ApplyRect(npcSpotlightShadowImage == null ? null : npcSpotlightShadowImage.GetComponent<RectTransform>(), new Vector2(0.22f, 0.00f), new Vector2(0.78f, 0.88f));
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
            if (combatUiController != null)
            {
                return;
            }

            EnsureEventSystem();
            var moduleObject = new GameObject("Combat UI Module", typeof(RectTransform));
            combatUiController = moduleObject.AddComponent<CombatUiController>();
            combatUiController.Initialize(HudParent, ResolveCombatAction, ToggleSkillPicker, ToggleCombatItemInspect);
            combatPanel = combatUiController.Root;
            combatEnemyStage = combatUiController.EnemyPanel;
            combatEnemyImage = combatUiController.EnemyImage;
            combatEnemyTitleText = combatUiController.EnemyTitleText;
            combatEnemyStatusText = combatUiController.EnemyStatusText;
            enemyHpFill = combatUiController.EnemyHpFill;
            combatEnemyDamageNumberText = combatUiController.EnemyDamageText;
            combatMataiosDamageNumberText = combatUiController.MataiosDamageText;
            combatDefeatFeedbackText = combatUiController.DefeatFeedbackText;
            combatLogPanel = combatUiController.CombatLogPanel;
            combatText = combatUiController.CombatLogText;
            combatPartyDock = combatUiController.PartyDock;
            combatPlayerCard = combatUiController.PlayerCard;
            combatMataiosCard = combatUiController.MataiosCard;
            combatPlayerPortraitImage = combatUiController.PlayerPortrait;
            combatMataiosPortraitImage = combatUiController.MataiosPortrait;
            combatPlayerPortraitFrameImage = combatUiController.PlayerPortraitFrame;
            combatMataiosPortraitFrameImage = combatUiController.MataiosPortraitFrame;
            combatPlayerPortraitFallbackText = combatUiController.PlayerPortraitFallbackText;
            combatPlayerCardText = combatUiController.PlayerCardText;
            combatMataiosCardText = combatUiController.MataiosCardText;
            combatPlayerDamageNumberText = combatUiController.PlayerDamageText;
            playerHpFill = combatUiController.PlayerHpFill;
            mataiosHpFill = combatUiController.MataiosHpFill;
            combatTrainingStatusIconImage = combatUiController.TrainingStatusIcon;
            combatBandageStatusIconImage = combatUiController.BandageStatusIcon;
            combatRecallStatusIconImage = combatUiController.RecallStatusIcon;
            attackButton = combatUiController.AttackButton;
            defendButton = combatUiController.DefendButton;
            skillButton = combatUiController.SkillButton;
            attackActionIconImage = combatUiController.AttackIcon;
            defendActionIconImage = combatUiController.DefendIcon;
            skillActionIconImage = combatUiController.SkillIcon;
            combatItemInspectButton = combatUiController.ItemInspectButton;
            ApplyCombatStatusIconSprites();
        }

        private void EnsureSkillPickerPanel()
        {
            if (skillPickerPanel != null || combatPartyDock == null)
            {
                return;
            }

            var panelObject = new GameObject("Combat Skill Picker");
            panelObject.transform.SetParent(combatPartyDock, false);
            skillPickerPanel = panelObject.AddComponent<RectTransform>();
            skillPickerPanel.anchorMin = new Vector2(0.56f, 0.22f);
            skillPickerPanel.anchorMax = new Vector2(0.96f, 0.42f);
            skillPickerPanel.offsetMin = Vector2.zero;
            skillPickerPanel.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.02f, 0.025f, 0.03f, 0.97f);
            image.raycastTarget = false;
            skillPickerPanel.gameObject.SetActive(false);
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

        private void EnsureCombatItemInspectPanel()
        {
            if (combatItemInspectPanel != null || combatPartyDock == null)
            {
                return;
            }

            var panelObject = new GameObject("Combat Item Inspect Panel");
            panelObject.transform.SetParent(combatPartyDock, false);
            combatItemInspectPanel = panelObject.AddComponent<RectTransform>();
            combatItemInspectPanel.anchorMin = new Vector2(0.50f, 0.35f);
            combatItemInspectPanel.anchorMax = new Vector2(0.96f, 0.96f);
            combatItemInspectPanel.offsetMin = Vector2.zero;
            combatItemInspectPanel.offsetMax = Vector2.zero;
            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.018f, 0.023f, 0.030f, 0.98f);
            image.raycastTarget = false;
            combatItemInspectText = CreateCombatChildText(panelObject.transform, "Combat Item Inspect Text", new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.76f), 20, TextAnchor.UpperLeft);
            combatItemInspectText.text = string.Empty;
            combatItemInspectCloseButton = CreateCombatItemInspectCloseButton(panelObject.transform);
            combatItemInspectPanel.gameObject.SetActive(false);
        }

        private Button CreateCombatItemInspectCloseButton(Transform parent)
        {
            var buttonObject = new GameObject("Combat Item Inspect Close Button");
            buttonObject.transform.SetParent(parent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.68f, 0.80f);
            rect.anchorMax = new Vector2(0.94f, 0.96f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.14f, 0.18f, 0.22f, 0.96f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(HideCombatItemInspect);

            var label = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 18, TextAnchor.MiddleCenter);
            label.text = showRawDebugText ? "close" : "닫기";
            label.raycastTarget = false;
            return button;
        }

        private void ToggleCombatItemInspect()
        {
            EnsureCombatItemInspectPanel();
            if (combatItemInspectPanel == null)
            {
                return;
            }

            var nextVisible = !combatItemInspectPanel.gameObject.activeSelf;
            if (nextVisible)
            {
                HideSkillPicker();
            }

            combatItemInspectPanel.gameObject.SetActive(nextVisible);
        }

        private void HideCombatItemInspect()
        {
            if (combatItemInspectPanel != null)
            {
                combatItemInspectPanel.gameObject.SetActive(false);
            }
        }

        private void EnsureBossRewardPanel()
        {
            if (bossRewardPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Boss Reward Popup");
            panelObject.transform.SetParent(HudParent, false);
            bossRewardPanel = panelObject.AddComponent<RectTransform>();
            bossRewardPanel.anchorMin = new Vector2(0.10f, 0.30f);
            bossRewardPanel.anchorMax = new Vector2(0.90f, 0.72f);
            bossRewardPanel.offsetMin = Vector2.zero;
            bossRewardPanel.offsetMax = Vector2.zero;

            var background = panelObject.AddComponent<Image>();
            background.color = new Color(0.025f, 0.032f, 0.040f, 0.98f);
            background.raycastTarget = false;

            bossRewardTitleText = CreateCombatChildText(panelObject.transform, "Boss Reward Title", new Vector2(0.08f, 0.76f), new Vector2(0.92f, 0.94f), 34, TextAnchor.MiddleCenter);
            bossRewardTitleText.text = string.Empty;

            bossRewardGoldIconImage = CreateCombatImage(panelObject.transform, "Boss Reward Gold Icon", new Vector2(0.12f, 0.48f), new Vector2(0.25f, 0.66f));
            bossRewardGoldText = CreateCombatChildText(panelObject.transform, "Boss Reward Gold Text", new Vector2(0.28f, 0.48f), new Vector2(0.86f, 0.66f), 30, TextAnchor.MiddleLeft);

            bossRewardAffinityIconImage = CreateCombatImage(panelObject.transform, "Boss Reward Affinity Icon", new Vector2(0.12f, 0.28f), new Vector2(0.25f, 0.46f));
            bossRewardAffinityText = CreateCombatChildText(panelObject.transform, "Boss Reward Affinity Text", new Vector2(0.28f, 0.28f), new Vector2(0.86f, 0.46f), 30, TextAnchor.MiddleLeft);

            var buttonObject = new GameObject("Boss Reward Next Floor Button");
            buttonObject.transform.SetParent(panelObject.transform, false);
            var buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.20f, 0.07f);
            buttonRect.anchorMax = new Vector2(0.80f, 0.22f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            var buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.15f, 0.22f, 0.26f, 0.98f);
            bossRewardNextFloorButton = buttonObject.AddComponent<Button>();
            bossRewardNextFloorButton.targetGraphic = buttonImage;
            bossRewardNextFloorButton.onClick.AddListener(ResolveNextFloor);
            var label = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 29, TextAnchor.MiddleCenter);
            label.text = "다음 층";

            bossRewardPanel.gameObject.SetActive(false);
        }

        private void UpdateBossRewardPopup(PrototypeRunSnapshot snapshot)
        {
            var visible = ShouldShowBossRewardPopup(snapshot);
            if (!visible)
            {
                HideBossRewardPopup();
                return;
            }

            EnsureBossRewardPanel();
            if (bossRewardPanel == null)
            {
                return;
            }

            bossRewardPanel.gameObject.SetActive(true);
            bossRewardPanel.transform.SetAsLastSibling();
            if (bossRewardTitleText != null)
            {
                bossRewardTitleText.text = snapshot.RunClear ? "최종 보스 격파" : "보스 격파";
            }

            SetBossRewardIcon(bossRewardGoldIconImage, "resource.gold", "G");
            SetBossRewardIcon(bossRewardAffinityIconImage, "resource.affinity", "신");
            if (bossRewardGoldText != null)
            {
                bossRewardGoldText.text = "Gold " + ExtractBossRewardValue(_lastResultMessage, "gold reward ", snapshot.LastCombatGoldReward);
            }

            if (bossRewardAffinityText != null)
            {
                bossRewardAffinityText.text = "신뢰 " + ExtractBossRewardValue(_lastResultMessage, "affinity ", snapshot.LastCombatAffinityDelta);
            }

            var nextFloorVisible = snapshot.StairUnlocked && !snapshot.RunCompleted;
            if (bossRewardNextFloorButton != null)
            {
                bossRewardNextFloorButton.gameObject.SetActive(nextFloorVisible);
                bossRewardNextFloorButton.interactable = nextFloorVisible && _roomController != null;
            }
        }

        private void HideBossRewardPopup()
        {
            if (bossRewardPanel != null)
            {
                bossRewardPanel.gameObject.SetActive(false);
            }
        }

        private void EnsureLevelRewardPanel()
        {
            if (levelRewardPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Level Reward Popup");
            panelObject.transform.SetParent(HudParent, false);
            levelRewardPanel = panelObject.AddComponent<RectTransform>();
            levelRewardPanel.anchorMin = new Vector2(0.09f, 0.24f);
            levelRewardPanel.anchorMax = new Vector2(0.91f, 0.76f);
            levelRewardPanel.offsetMin = Vector2.zero;
            levelRewardPanel.offsetMax = Vector2.zero;

            var background = panelObject.AddComponent<Image>();
            background.color = new Color(0.026f, 0.034f, 0.040f, 0.98f);
            background.raycastTarget = true;

            levelRewardTitleText = CreateCombatChildText(panelObject.transform, "Level Reward Title", new Vector2(0.08f, 0.80f), new Vector2(0.92f, 0.95f), 34, TextAnchor.MiddleCenter);
            levelRewardBodyText = CreateCombatChildText(panelObject.transform, "Level Reward Body", new Vector2(0.10f, 0.62f), new Vector2(0.90f, 0.78f), 25, TextAnchor.MiddleCenter);
            levelRewardAttackButton = CreateLevelRewardButton(panelObject.transform, "Level Reward Attack Button", new Vector2(0.12f, 0.43f), new Vector2(0.88f, 0.56f), "공격 단련\n공격력 +1 / 마타이오스 지원 +1", PrototypeRunState.LevelRewardAttackId);
            levelRewardMaxHpButton = CreateLevelRewardButton(panelObject.transform, "Level Reward Max HP Button", new Vector2(0.12f, 0.27f), new Vector2(0.88f, 0.40f), "생존 단련\n최대 HP +4 / 마타이오스 HP +3", PrototypeRunState.LevelRewardMaxHpId);
            levelRewardSkillButton = CreateLevelRewardButton(panelObject.transform, "Level Reward Skill Button", new Vector2(0.12f, 0.11f), new Vector2(0.88f, 0.24f), "Skill CD -1", PrototypeRunState.LevelRewardSkillCooldownId);
            levelRewardPanel.gameObject.SetActive(false);
        }

        private Button CreateLevelRewardButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string labelText, string rewardId)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.13f, 0.18f, 0.21f, 0.98f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => ResolveLevelReward(rewardId));
            var label = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 27, TextAnchor.MiddleCenter);
            label.text = labelText;
            return button;
        }

        private void UpdateLevelRewardPopup(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.LevelUpRewardPending || ShouldShowCombatDefeatFeedback(snapshot))
            {
                HideLevelRewardPopup();
                return;
            }

            EnsureLevelRewardPanel();
            if (levelRewardPanel == null)
            {
                return;
            }

            levelRewardPanel.gameObject.SetActive(true);
            levelRewardPanel.transform.SetAsLastSibling();
            if (levelRewardTitleText != null)
            {
                levelRewardTitleText.text = "레벨 상승";
            }

            if (levelRewardBodyText != null)
            {
                levelRewardBodyText.text = "Lv " + snapshot.CombatLevel + "  XP " + snapshot.CombatXp + "/" + snapshot.CombatXpToNextLevel;
            }
        }

        private void HideLevelRewardPopup()
        {
            if (levelRewardPanel != null)
            {
                levelRewardPanel.gameObject.SetActive(false);
            }
        }

        private void ResolveLevelReward(string rewardId)
        {
            if (_roomController == null)
            {
                return;
            }

            _roomController.ResolveLevelReward(rewardId);
            ShowRunState(_roomController.GetSnapshot());
        }

        private bool ShouldShowBossRewardPopup(PrototypeRunSnapshot snapshot)
        {
            return !showRawDebugText &&
                !snapshot.IsInCombat &&
                snapshot.LastCombatEnemyDefeated &&
                !ShouldShowCombatDefeatFeedback(snapshot) &&
                snapshot.StairUnlocked &&
                !snapshot.RunClear &&
                IsBossClearResult(_lastResultMessage);
        }

        private void SetBossRewardIcon(Image image, string iconKey, string fallback)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = ResolveIcon(iconKey);
            image.color = image.sprite == null ? new Color(0.20f, 0.26f, 0.30f, 0.95f) : Color.white;
            image.preserveAspect = true;
            image.gameObject.SetActive(true);
            var fallbackText = image.GetComponentInChildren<Text>();
            if (fallbackText == null)
            {
                fallbackText = CreateCombatChildText(image.transform, "Fallback", Vector2.zero, Vector2.one, 20, TextAnchor.MiddleCenter);
            }

            fallbackText.text = image.sprite == null ? fallback : string.Empty;
        }

        private static string ExtractBossRewardValue(string message, string tokenPrefix, int fallback)
        {
            var token = ExtractTokenResult(message, tokenPrefix);
            if (!string.IsNullOrEmpty(token))
            {
                var space = token.LastIndexOf(' ');
                return space >= 0 ? token.Substring(space + 1).Trim() : token;
            }

            return fallback == 0 ? "+0" : FormatDelta(fallback);
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

            nextFloorButton.gameObject.SetActive(snapshot.StairUnlocked &&
                !snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !ShouldShowCombatDefeatFeedback(snapshot) &&
                !ShouldShowBossRewardPopup(snapshot));
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
                !snapshot.HasSelectedMapNode &&
                !string.IsNullOrEmpty(snapshot.NextDemoEncounterId) &&
                _choiceButtons.Count == 0 &&
                !_eventPresentationActive &&
                !_shopPresentationActive &&
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

            _roomController.ResolveNextFloor();
            HideUtilityPanel();
            HideRestInteractionPanel();
            HideEventCutsceneLayout();
            HideMerchantPresentation();
            HideNpcSpotlight();
            HideBossRewardPopup();
            ClearChoices();
            ShowResultMessage(string.Empty);
            SetResultVisible(false);
            var snapshot = _roomController.GetSnapshot();
            ShowRunState(snapshot);
            if (snapshot.HasFloorMap && !snapshot.RunCompleted)
            {
                ShowMapChoices(_roomController.GetFloorMapNodes(), mapNodeId =>
                {
                    var selected = _roomController.SelectMapNode(mapNodeId);
                    OpenSelectedRouteStep(selected);
                });
                ShowRunState(_roomController.GetSnapshot());
            }
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

            if (TryAutoStartCombat(selection))
            {
                return;
            }

            if (_roomController.HasEncounterChoices(selection))
            {
                ShowEncounterChoicesForSelection(selection);
                return;
            }

            var nodeResolution = _roomController.ResolveCurrentRouteNode(selection);
            ShowResult(nodeResolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private bool TryGetAutoCombatStartChoice(EncounterSelection selection, out string choiceStableId)
        {
            choiceStableId = string.Empty;
            if (_roomController == null ||
                !selection.HasEncounter ||
                selection.Encounter == null ||
                selection.Encounter.Type != EncounterType.Battle ||
                selection.Node == null ||
                selection.Node.Kind != NodeKind.Battle)
            {
                return false;
            }

            var views = _roomController.BuildEncounterChoiceViews(selection);
            if (views == null)
            {
                return false;
            }

            for (var i = 0; i < views.Length; i++)
            {
                var view = views[i];
                if (!view.Visible || !view.Enabled || !ChoiceStartsCombat(selection.Encounter, view.ChoiceStableId))
                {
                    continue;
                }

                choiceStableId = view.ChoiceStableId;
                return true;
            }

            return false;
        }

        private bool TryAutoStartCombat(EncounterSelection selection)
        {
            if (_roomController == null || !TryGetAutoCombatStartChoice(selection, out var combatStartChoiceId))
            {
                return false;
            }

            HideMapAndResultSurfacesForCombat();
            var resolution = _roomController.ResolveCurrentRouteChoice(selection, combatStartChoiceId);
            var snapshot = _roomController.GetSnapshot();
            if (!snapshot.IsInCombat)
            {
                ShowResult(resolution);
            }

            ShowRunState(snapshot);
            return true;
        }

        private void HideMapAndResultSurfacesForCombat()
        {
            ClearChoices();
            HideEventCutsceneLayout(restoreRouteText: false);
            HideUtilityPanel();
            HideRestInteractionPanel();
            HideMerchantPresentation();
            HideNpcSpotlight();
            HideFloorMapUi();
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(resultLayer, false);
            SetResultVisible(false);
            HideLegacyEncounterVisuals(hideBackground: true);
            if (routeText != null && !showRawDebugText)
            {
                routeText.gameObject.SetActive(false);
            }

            if (interactionText != null && !showRawDebugText)
            {
                interactionText.gameObject.SetActive(false);
            }

            if (memoryText != null && !showRawDebugText)
            {
                memoryText.text = string.Empty;
                memoryText.gameObject.SetActive(false);
            }
        }

        private static bool ChoiceStartsCombat(EncounterData encounter, string choiceStableId)
        {
            if (encounter == null || encounter.Choices == null || string.IsNullOrEmpty(choiceStableId))
            {
                return false;
            }

            for (var i = 0; i < encounter.Choices.Length; i++)
            {
                var choice = encounter.Choices[i];
                if (choice == null || choice.stableId != choiceStableId || choice.effects == null)
                {
                    continue;
                }

                for (var j = 0; j < choice.effects.Length; j++)
                {
                    if (choice.effects[j] != null && choice.effects[j].kind == "StartCombat")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ShowEncounterChoicesForSelection(EncounterSelection selection)
        {
            if (_roomController == null || !selection.HasEncounter)
            {
                return;
            }

            if (TryAutoStartCombat(selection))
            {
                return;
            }

            var views = _roomController.BuildEncounterChoiceViews(selection);
            ShowChoices(selection.Encounter, views, choiceStableId =>
            {
                var shopPurchase = IsShopPurchaseChoice(selection.Encounter, choiceStableId);
                var resolution = _roomController.ResolveCurrentRouteChoice(selection, choiceStableId);
                ShowResult(resolution);
                if (shopPurchase && !resolution.RunCompleted && _roomController.GetSnapshot().HasSelectedMapNode)
                {
                    ShowEncounterChoicesForSelection(selection);
                }

                var snapshot = _roomController.GetSnapshot();
                if (!snapshot.IsInCombat && snapshot.HasFloorMap && !snapshot.HasSelectedMapNode)
                {
                    HideEventCutsceneLayout();
                    HideLegacyEncounterVisuals(hideBackground: true);
                }

                ShowRunState(snapshot);
            });
            ShowRunState(_roomController.GetSnapshot());
        }

        private static bool IsShopPurchaseChoice(EncounterData encounter, string choiceStableId)
        {
            return encounter != null &&
                encounter.Type == EncounterType.Shop &&
                !string.IsNullOrEmpty(choiceStableId) &&
                choiceStableId.Contains("_BUY_", StringComparison.Ordinal);
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
            var mapVisible = IsMapRouteState(snapshot);
            routeText.gameObject.SetActive((!snapshot.IsInCombat && !_eventPresentationActive && !restVisible && !shopVisible && !mapVisible) || showRawDebugText);
            if ((snapshot.IsInCombat || _eventPresentationActive || restVisible || shopVisible || mapVisible) && !showRawDebugText)
            {
                if (mapVisible)
                {
                    routeText.text = BuildPublicMapSummary(snapshot);
                }
                else
                {
                    routeText.text = string.Empty;
                }
                return;
            }

            if (_demoRouteLabels.Count == 0)
            {
                if (!showRawDebugText && snapshot.StairUnlocked)
                {
                    routeText.text = BuildPublicRouteSummary(snapshot, 0);
                    return;
                }

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
                IsMapRouteState(snapshot);
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
            return "갈림길 선택";
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

            if ((_eventPresentationActive || _shopPresentationActive || RestInteractionPanelVisible || IsMapRouteState(snapshot)) && !showRawDebugText)
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
                buff = "단련 피해 +1";
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

        private static string JoinCompactChips(List<string> chips, int visibleLimit)
        {
            if (chips == null || chips.Count == 0)
            {
                return string.Empty;
            }

            var limit = Mathf.Clamp(visibleLimit, 1, chips.Count);
            if (chips.Count <= limit)
            {
                return string.Join("  ", chips);
            }

            var visible = chips.GetRange(0, limit);
            visible.Add("+" + (chips.Count - limit));
            return string.Join("  ", visible);
        }

        private void UpdateCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureCombatPanel();
            if (combatPanel == null)
            {
                return;
            }

            StartCombatDefeatFeedbackIfNeeded(snapshot);
            var defeatFeedback = ShouldShowCombatDefeatFeedback(snapshot);
            var hasCombat = snapshot.IsInCombat || defeatFeedback;
            if (hasCombat && combatUiController != null)
            {
                combatUiController.Show(snapshot);
            }
            else if (combatUiController != null)
            {
                combatUiController.Hide();
            }
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
                    !IsMapRouteState(snapshot));
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
                HideSkillPicker();
                ResetCombatEnemyFeedbackState();
                ResetCombatPlayerFeedbackState();
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
                : BuildCombatLogRecord(snapshot);

            UpdateCombatVisuals(snapshot);
            UpdatePartyDock(snapshot);
            UpdateCombatActionIcons();
            UpdateCombatItemInspect(snapshot);
            UpdateCommandReplacementFlow(snapshot);

            var canAct = snapshot.IsInCombat &&
                !defeatFeedback &&
                !snapshot.LevelUpRewardPending &&
                _roomController != null &&
                !IsCutscenePlaying() &&
                _combatIntroTimer <= 0f;
            var recommendedAction = ResolveRecommendedCombatAction(snapshot);
            if (attackButton != null)
            {
                var preview = _roomController == null
                    ? new CombatActionPreview(CombatAction.Attack, "공격", string.Empty, false)
                    : _roomController.BuildCombatActionPreview(CombatAction.Attack);
                attackButton.gameObject.SetActive(snapshot.IsInCombat && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId));
                attackButton.interactable = canAct && attackButton.gameObject.activeSelf;
                SetCombatActionButtonPreview(attackButton, preview);
                ApplyCombatActionButtonVisualState(attackButton, recommendedAction == CombatAction.Attack && preview.Usable, attackButton.interactable);
            }

            if (defendButton != null)
            {
                var preview = _roomController == null
                    ? new CombatActionPreview(CombatAction.Defend, "방어", string.Empty, false)
                    : _roomController.BuildCombatActionPreview(CombatAction.Defend);
                defendButton.gameObject.SetActive(snapshot.IsInCombat && snapshot.IsCommandEquipped(PrototypeRunState.CommandDefendId));
                defendButton.interactable = canAct && defendButton.gameObject.activeSelf;
                SetCombatActionButtonPreview(defendButton, preview);
                ApplyCombatActionButtonVisualState(defendButton, recommendedAction == CombatAction.Defend && preview.Usable, defendButton.interactable);
            }

            if (skillButton != null)
            {
                var preview = _roomController == null
                    ? new CombatActionPreview(CombatAction.Skill, "스킬", "조건 부족", false)
                    : _roomController.BuildCombatActionPreview(CombatAction.Skill);
                skillButton.gameObject.SetActive(snapshot.IsInCombat);
                skillButton.interactable = canAct && skillButton.gameObject.activeSelf && preview.Usable;
                SetCombatActionButtonPreview(skillButton, preview);
                ApplyCombatActionButtonVisualState(skillButton, recommendedAction == CombatAction.Skill && preview.Usable, skillButton.interactable);
            }
        }

        private void EnsureCombatIntroOverlay()
        {
            if (combatIntroOverlay != null || combatPanel == null)
            {
                return;
            }

            var overlayObject = new GameObject("Combat Intro Overlay");
            overlayObject.transform.SetParent(combatPanel, false);
            combatIntroOverlay = overlayObject.AddComponent<RectTransform>();
            combatIntroOverlay.anchorMin = Vector2.zero;
            combatIntroOverlay.anchorMax = Vector2.one;
            combatIntroOverlay.offsetMin = Vector2.zero;
            combatIntroOverlay.offsetMax = Vector2.zero;

            var image = overlayObject.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.72f);
            image.raycastTarget = true;

            var label = CreateCombatChildText(overlayObject.transform, "Combat Intro Label", new Vector2(0.18f, 0.44f), new Vector2(0.82f, 0.56f), 34, TextAnchor.MiddleCenter);
            label.text = string.Empty;
            overlayObject.SetActive(false);
        }

        private static void SetCombatActionButtonPreview(Button button, CombatActionPreview preview)
        {
            if (button == null)
            {
                return;
            }

            var label = button.GetComponentInChildren<Text>();
            if (label == null)
            {
                return;
            }

            var actionLabel = string.IsNullOrEmpty(preview.Label)
                ? PublicCombatActionName(preview.Action.ToString())
                : preview.Label;
            var previewText = preview.PreviewText;
            if (preview.Action == CombatAction.Skill && !preview.Usable && string.IsNullOrEmpty(previewText))
            {
                previewText = "조건 부족";
            }

            label.text = string.IsNullOrEmpty(actionLabel)
                ? string.Empty
                : string.IsNullOrEmpty(previewText) ? actionLabel : actionLabel + "\n" + previewText;
            var hasPreviewText = !string.IsNullOrEmpty(previewText);
            label.fontSize = hasPreviewText ? 19 : 28;
            label.resizeTextMinSize = hasPreviewText ? 13 : 22;
            label.resizeTextMaxSize = hasPreviewText ? 19 : 28;
            label.lineSpacing = 0.92f;
        }

        private static void ApplyCombatActionButtonVisualState(Button button, bool recommended, bool usable)
        {
            if (button == null)
            {
                return;
            }

            var image = button.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            if (!button.gameObject.activeSelf)
            {
                return;
            }

            if (!usable)
            {
                image.color = new Color(0.07f, 0.08f, 0.10f, 0.90f);
                return;
            }

            image.color = recommended
                ? new Color(0.18f, 0.34f, 0.30f, 0.99f)
                : new Color(0.12f, 0.17f, 0.20f, 0.98f);
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
                combatEnemyImage.color = ShouldShowCombatDefeatFeedback(snapshot)
                    ? new Color(1f, 1f, 1f, 0.48f)
                    : Color.white;
            }

            if (combatEnemyTitleText != null)
            {
                combatEnemyTitleText.text = PublicEnemyName(snapshot.LastCombatEnemyId) + "  HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp;
            }

            if (combatEnemyStatusText != null)
            {
                combatEnemyStatusText.text = BuildCombatEnemyStatusChips(snapshot);
                combatEnemyStatusText.gameObject.SetActive(!string.IsNullOrEmpty(combatEnemyStatusText.text));
            }

            if (enemyHpFill != null)
            {
                enemyHpFill.fillAmount = Ratio(snapshot.EnemyHp, snapshot.EnemyMaxHp);
            }

            TrackCombatEnemyFeedback(snapshot);

            if (playerHpFill != null)
            {
                playerHpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            }

            if (mataiosHpFill != null)
            {
                mataiosHpFill.fillAmount = Ratio(snapshot.MataiosHp, snapshot.MataiosMaxHp);
            }
        }

        private void TrackCombatEnemyFeedback(PrototypeRunSnapshot snapshot)
        {
            var defeatFeedback = ShouldShowCombatDefeatFeedback(snapshot);
            if ((!snapshot.IsInCombat && !defeatFeedback) || combatEnemyImage == null)
            {
                ResetCombatEnemyFeedbackState();
                return;
            }

            CaptureCombatEnemyFeedbackBase();
            var visualKey = (snapshot.LastCombatId ?? string.Empty) + "|" + (snapshot.LastCombatEnemyId ?? string.Empty);
            if (_lastCombatVisualKey != visualKey)
            {
                _lastCombatVisualKey = visualKey;
                _lastCombatVisualRound = snapshot.CombatRound;
                _lastCombatVisualEnemyHp = snapshot.EnemyHp;
                _lastCombatVisualPlayerHp = snapshot.PlayerHp;
                ResetCombatEnemyFeedbackTransform();
                StartCombatIntro(visualKey, snapshot);
                return;
            }

            if (snapshot.CombatRound == _lastCombatVisualRound)
            {
                return;
            }

            if (_lastCombatVisualEnemyHp >= 0 && snapshot.EnemyHp < _lastCombatVisualEnemyHp)
            {
                var totalDamage = _lastCombatVisualEnemyHp - snapshot.EnemyHp;
                var mataiosDamage = Mathf.Clamp(snapshot.LastMataiosCombatDamage, 0, totalDamage);
                var playerDamage = Mathf.Max(0, totalDamage - mataiosDamage);
                if (playerDamage > 0)
                {
                    ShowCombatDamageNumber(combatEnemyDamageNumberText, "-" + playerDamage, CombatDamageNumberRole.Enemy);
                }

                if (mataiosDamage > 0)
                {
                    ShowCombatDamageNumber(combatMataiosDamageNumberText, "마타이오스 -" + mataiosDamage, CombatDamageNumberRole.Mataios);
                }

                TriggerCombatEnemyHitShake();
            }

            if (_lastCombatVisualPlayerHp >= 0 && snapshot.PlayerHp < _lastCombatVisualPlayerHp)
            {
                ShowCombatDamageNumber(combatPlayerDamageNumberText, "-" + (_lastCombatVisualPlayerHp - snapshot.PlayerHp), CombatDamageNumberRole.Player);
                TriggerCombatPlayerHitFeedback();
            }

            if ((snapshot.LastCombatRoundResult ?? string.Empty).Contains("enemyDamage ", StringComparison.Ordinal))
            {
                TriggerCombatEnemyAttackPulse();
            }

            _lastCombatVisualRound = snapshot.CombatRound;
            _lastCombatVisualEnemyHp = snapshot.EnemyHp;
            _lastCombatVisualPlayerHp = snapshot.PlayerHp;
        }

        private enum CombatDamageNumberRole
        {
            Enemy,
            Mataios,
            Player
        }

        private void ShowCombatDamageNumber(Text text, string value, CombatDamageNumberRole role)
        {
            if (text == null || string.IsNullOrEmpty(value))
            {
                return;
            }

            text.text = value;
            text.gameObject.SetActive(true);
            if (role == CombatDamageNumberRole.Enemy)
            {
                _combatEnemyDamageNumberTimer = CombatDamageNumberDuration;
            }
            else if (role == CombatDamageNumberRole.Mataios)
            {
                _combatMataiosDamageNumberTimer = CombatDamageNumberDuration;
            }
            else
            {
                _combatPlayerDamageNumberTimer = CombatDamageNumberDuration;
            }
        }

        private void TriggerCombatEnemyHitShake()
        {
            CaptureCombatEnemyFeedbackBase();
            _combatEnemyHitShakeTimer = CombatEnemyHitShakeDuration;
        }

        private void TriggerCombatEnemyAttackPulse()
        {
            CaptureCombatEnemyFeedbackBase();
            _combatEnemyAttackPulseTimer = CombatEnemyAttackPulseDuration;
        }

        private void TriggerCombatPlayerHitFeedback()
        {
            CaptureCombatPlayerFeedbackBase();
            _combatPlayerHitShakeTimer = CombatPlayerHitShakeDuration;
            _combatPlayerHpPulseTimer = CombatPlayerHpPulseDuration;
        }

        private void StartCombatIntro(string visualKey, PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.IsInCombat || _combatIntroKey == visualKey)
            {
                return;
            }

            EnsureCombatIntroOverlay();
            _combatIntroKey = visualKey;
            _combatIntroTimer = CombatIntroDuration;
            if (combatIntroOverlay != null)
            {
                combatIntroOverlay.gameObject.SetActive(true);
                combatIntroOverlay.transform.SetAsLastSibling();
            }
        }

        private bool ShouldShowCombatDefeatFeedback(PrototypeRunSnapshot snapshot)
        {
            return !snapshot.IsInCombat &&
                snapshot.LastCombatEnemyDefeated &&
                !string.IsNullOrEmpty(snapshot.LastCombatId) &&
                _combatDefeatFeedbackTimer > 0f &&
                _combatDefeatFeedbackKey == BuildCombatDefeatFeedbackKey(snapshot);
        }

        private string BuildCombatDefeatFeedbackKey(PrototypeRunSnapshot snapshot)
        {
            return (snapshot.LastCombatId ?? string.Empty) + "|" + snapshot.CombatRound + "|" + (snapshot.LastCombatEnemyId ?? string.Empty);
        }

        private void StartCombatDefeatFeedbackIfNeeded(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat || !snapshot.LastCombatEnemyDefeated || string.IsNullOrEmpty(snapshot.LastCombatId))
            {
                return;
            }

            var key = BuildCombatDefeatFeedbackKey(snapshot);
            if (_combatDefeatFeedbackKey == key)
            {
                return;
            }

            _combatDefeatFeedbackKey = key;
            _combatDefeatFeedbackTimer = CombatDefeatFeedbackDuration;
            if (combatDefeatFeedbackText != null)
            {
                combatDefeatFeedbackText.text = IsBossEnemyId(snapshot.LastCombatEnemyId) ? "보스 격파" : "격파";
                combatDefeatFeedbackText.gameObject.SetActive(true);
            }

            TriggerCombatEnemyHitShake();
        }

        private void UpdateCombatEnemyFeedbackAnimation(float deltaTime)
        {
            if (combatEnemyImage == null || (!_combatEnemyFeedbackBaseCaptured && _combatEnemyHitShakeTimer <= 0f && _combatEnemyAttackPulseTimer <= 0f))
            {
                return;
            }

            CaptureCombatEnemyFeedbackBase();
            _combatEnemyHitShakeTimer = Mathf.Max(0f, _combatEnemyHitShakeTimer - Mathf.Max(0f, deltaTime));
            _combatEnemyAttackPulseTimer = Mathf.Max(0f, _combatEnemyAttackPulseTimer - Mathf.Max(0f, deltaTime));

            var rect = combatEnemyImage.rectTransform;
            var shakeOffset = 0f;
            if (_combatEnemyHitShakeTimer > 0f)
            {
                var progress = 1f - _combatEnemyHitShakeTimer / CombatEnemyHitShakeDuration;
                shakeOffset = Mathf.Sin(progress * Mathf.PI * 6f) * CombatEnemyHitShakePixels * (1f - progress);
            }

            var pulseScale = 1f;
            if (_combatEnemyAttackPulseTimer > 0f)
            {
                var progress = 1f - _combatEnemyAttackPulseTimer / CombatEnemyAttackPulseDuration;
                pulseScale = Mathf.Lerp(CombatEnemyAttackPulseScale, 1f, progress);
            }

            rect.anchoredPosition = _combatEnemyImageBasePosition + new Vector2(shakeOffset, 0f);
            rect.localScale = _combatEnemyImageBaseScale * pulseScale;

            if (_combatEnemyHitShakeTimer <= 0f && _combatEnemyAttackPulseTimer <= 0f)
            {
                ResetCombatEnemyFeedbackTransform();
            }
        }

        private void UpdateCombatPlayerFeedbackAnimation(float deltaTime)
        {
            if (combatPlayerCard == null || (!_combatPlayerFeedbackBaseCaptured && _combatPlayerHitShakeTimer <= 0f && _combatPlayerHpPulseTimer <= 0f))
            {
                return;
            }

            CaptureCombatPlayerFeedbackBase();
            _combatPlayerHitShakeTimer = Mathf.Max(0f, _combatPlayerHitShakeTimer - Mathf.Max(0f, deltaTime));
            _combatPlayerHpPulseTimer = Mathf.Max(0f, _combatPlayerHpPulseTimer - Mathf.Max(0f, deltaTime));

            var shakeOffset = 0f;
            if (_combatPlayerHitShakeTimer > 0f)
            {
                var progress = 1f - _combatPlayerHitShakeTimer / CombatPlayerHitShakeDuration;
                shakeOffset = Mathf.Sin(progress * Mathf.PI * 7f) * CombatPlayerHitShakePixels * (1f - progress);
            }

            var pulseScale = 1f;
            if (_combatPlayerHpPulseTimer > 0f)
            {
                var progress = 1f - _combatPlayerHpPulseTimer / CombatPlayerHpPulseDuration;
                pulseScale = Mathf.Lerp(CombatPlayerHpPulseScale, 1f, progress);
            }

            combatPlayerCard.anchoredPosition = _combatPlayerCardBasePosition + new Vector2(shakeOffset, 0f);
            combatPlayerCard.localScale = _combatPlayerCardBaseScale * pulseScale;
            var cardImage = combatPlayerCard.GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.color = _combatPlayerHitShakeTimer > 0f
                    ? new Color(0.24f, 0.08f, 0.08f, 0.98f)
                    : _combatPlayerCardBaseColor;
            }

            if (_combatPlayerHitShakeTimer <= 0f && _combatPlayerHpPulseTimer <= 0f)
            {
                ResetCombatPlayerFeedbackTransform();
            }
        }

        private void CaptureCombatEnemyFeedbackBase()
        {
            if (_combatEnemyFeedbackBaseCaptured || combatEnemyImage == null)
            {
                return;
            }

            var rect = combatEnemyImage.rectTransform;
            _combatEnemyImageBasePosition = rect.anchoredPosition;
            _combatEnemyImageBaseScale = rect.localScale;
            _combatEnemyFeedbackBaseCaptured = true;
        }

        private void ResetCombatEnemyFeedbackTransform()
        {
            if (!_combatEnemyFeedbackBaseCaptured || combatEnemyImage == null)
            {
                return;
            }

            var rect = combatEnemyImage.rectTransform;
            rect.anchoredPosition = _combatEnemyImageBasePosition;
            rect.localScale = _combatEnemyImageBaseScale;
        }

        private void CaptureCombatPlayerFeedbackBase()
        {
            if (_combatPlayerFeedbackBaseCaptured || combatPlayerCard == null)
            {
                return;
            }

            _combatPlayerCardBasePosition = combatPlayerCard.anchoredPosition;
            _combatPlayerCardBaseScale = combatPlayerCard.localScale;
            var image = combatPlayerCard.GetComponent<Image>();
            _combatPlayerCardBaseColor = image == null ? Color.white : image.color;
            _combatPlayerFeedbackBaseCaptured = true;
        }

        private void ResetCombatPlayerFeedbackTransform()
        {
            if (!_combatPlayerFeedbackBaseCaptured || combatPlayerCard == null)
            {
                return;
            }

            combatPlayerCard.anchoredPosition = _combatPlayerCardBasePosition;
            combatPlayerCard.localScale = _combatPlayerCardBaseScale;
            var image = combatPlayerCard.GetComponent<Image>();
            if (image != null)
            {
                image.color = _combatPlayerCardBaseColor;
            }
        }

        private void ResetCombatPlayerFeedbackState()
        {
            ResetCombatPlayerFeedbackTransform();
            _combatPlayerHitShakeTimer = 0f;
            _combatPlayerHpPulseTimer = 0f;
        }

        private void ResetCombatEnemyFeedbackState()
        {
            ResetCombatEnemyFeedbackTransform();
            _lastCombatVisualKey = string.Empty;
            _lastCombatVisualRound = -1;
            _lastCombatVisualEnemyHp = -1;
            _lastCombatVisualPlayerHp = -1;
            _combatEnemyHitShakeTimer = 0f;
            _combatEnemyAttackPulseTimer = 0f;
            _combatEnemyDamageNumberTimer = 0f;
            _combatMataiosDamageNumberTimer = 0f;
            _combatPlayerDamageNumberTimer = 0f;
            if (combatEnemyDamageNumberText != null)
            {
                combatEnemyDamageNumberText.gameObject.SetActive(false);
            }

            if (combatMataiosDamageNumberText != null)
            {
                combatMataiosDamageNumberText.gameObject.SetActive(false);
            }

            if (combatPlayerDamageNumberText != null)
            {
                combatPlayerDamageNumberText.gameObject.SetActive(false);
            }

            if (combatDefeatFeedbackText != null)
            {
                combatDefeatFeedbackText.gameObject.SetActive(false);
            }
        }

        private void UpdateCombatDamageNumberAnimation(float deltaTime)
        {
            var step = Mathf.Max(0f, deltaTime);
            if (_combatEnemyDamageNumberTimer > 0f)
            {
                _combatEnemyDamageNumberTimer = Mathf.Max(0f, _combatEnemyDamageNumberTimer - step);
                if (_combatEnemyDamageNumberTimer <= 0f && combatEnemyDamageNumberText != null)
                {
                    combatEnemyDamageNumberText.gameObject.SetActive(false);
                }
            }

            if (_combatMataiosDamageNumberTimer > 0f)
            {
                _combatMataiosDamageNumberTimer = Mathf.Max(0f, _combatMataiosDamageNumberTimer - step);
                if (_combatMataiosDamageNumberTimer <= 0f && combatMataiosDamageNumberText != null)
                {
                    combatMataiosDamageNumberText.gameObject.SetActive(false);
                }
            }

            if (_combatPlayerDamageNumberTimer > 0f)
            {
                _combatPlayerDamageNumberTimer = Mathf.Max(0f, _combatPlayerDamageNumberTimer - step);
                if (_combatPlayerDamageNumberTimer <= 0f && combatPlayerDamageNumberText != null)
                {
                    combatPlayerDamageNumberText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateCombatIntro(float deltaTime)
        {
            if (_combatIntroTimer <= 0f)
            {
                return;
            }

            _combatIntroTimer = Mathf.Max(0f, _combatIntroTimer - Mathf.Max(0f, deltaTime));
            if (combatIntroOverlay != null)
            {
                var image = combatIntroOverlay.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.72f, _combatIntroTimer / CombatIntroDuration));
                }

                combatIntroOverlay.gameObject.SetActive(_combatIntroTimer > 0f);
            }

            if (_combatIntroTimer <= 0f)
            {
                ShowRunState(_lastSnapshot);
            }
        }

        private void UpdateCombatDefeatFeedback(float deltaTime)
        {
            if (_combatDefeatFeedbackTimer <= 0f)
            {
                return;
            }

            _combatDefeatFeedbackTimer = Mathf.Max(0f, _combatDefeatFeedbackTimer - Mathf.Max(0f, deltaTime));
            if (combatDefeatFeedbackText != null)
            {
                combatDefeatFeedbackText.gameObject.SetActive(_combatDefeatFeedbackTimer > 0f);
            }

            if (_combatDefeatFeedbackTimer <= 0f)
            {
                ResetCombatEnemyFeedbackTransform();
                ShowRunState(_lastSnapshot);
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
            SetImageVisible(topMemoryIconImage, false);
            SetImageVisible(topAffinityIconImage, false);
            SetImageVisible(topPlayerProfileImage, visible);
            SetImageVisible(topMataiosProfileImage, false);
        }

        private void ApplyTopHudIconSprites()
        {
            SetStaticIcon(topGoldIconImage, "resource.gold");
            SetStaticIcon(topMemoryIconImage, "resource.memory");
            SetStaticIcon(topAffinityIconImage, "resource.affinity");
            if (topPlayerProfileImage != null)
            {
                topPlayerProfileImage.sprite = presentationData == null ? null : presentationData.DefaultPlayerPortrait;
                topPlayerProfileImage.color = Color.white;
                topPlayerProfileImage.preserveAspect = true;
            }

            if (topMataiosProfileImage != null)
            {
                topMataiosProfileImage.sprite = presentationData == null ? null : presentationData.CombatMataiosPortrait;
                topMataiosProfileImage.color = Color.white;
                topMataiosProfileImage.preserveAspect = true;
            }
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

        private void UpdateCombatItemInspect(PrototypeRunSnapshot snapshot)
        {
            EnsureCombatItemInspectPanel();
            var visible = snapshot.IsInCombat;
            if (combatItemInspectButton != null)
            {
                combatItemInspectButton.gameObject.SetActive(visible);
                combatItemInspectButton.interactable = visible && snapshot.ItemCount > 0;
            }

            if (combatItemInspectText != null)
            {
                combatItemInspectText.text = BuildCombatItemInspectText();
            }

            if (!visible || snapshot.ItemCount <= 0)
            {
                HideCombatItemInspect();
            }
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

            HideSkillPicker();
            HideCombatItemInspect();
            var resolution = _roomController.ResolveCombatAction(action);
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private void ToggleSkillPicker()
        {
            EnsureSkillPickerPanel();
            if (skillPickerPanel == null)
            {
                return;
            }

            var visible = !skillPickerPanel.gameObject.activeSelf;
            if (!visible)
            {
                HideSkillPicker();
                return;
            }

            PopulateSkillPicker();
            skillPickerPanel.gameObject.SetActive(true);
        }

        private void HideSkillPicker()
        {
            if (skillPickerPanel != null)
            {
                skillPickerPanel.gameObject.SetActive(false);
            }
        }

        private void PopulateSkillPicker()
        {
            EnsureSkillPickerPanel();
            if (skillPickerPanel == null)
            {
                return;
            }

            for (var i = skillPickerPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(skillPickerPanel.GetChild(i).gameObject);
            }

            var skills = BuildOwnedCombatSkillNames();
            if (skills.Count == 0)
            {
                skills.Add("사용 가능한 스킬 없음");
            }

            for (var i = 0; i < skills.Count; i++)
            {
                var buttonObject = new GameObject("Skill Option " + i);
                buttonObject.transform.SetParent(skillPickerPanel, false);
                var rect = buttonObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(0f, 64f);
                rect.anchoredPosition = new Vector2(0f, -i * 70f);

                var image = buttonObject.AddComponent<Image>();
                image.color = new Color(0.12f, 0.17f, 0.20f, 0.98f);
                var button = buttonObject.AddComponent<Button>();
                button.targetGraphic = image;
                button.interactable = HasAnyCombatSkill();
                button.onClick.AddListener(() => ResolveCombatAction(CombatAction.Skill));

                var text = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 22, TextAnchor.MiddleCenter);
                text.text = skills[i];
            }
        }

        private void UpdateCommandReplacementFlow(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.CommandReplacementPending)
            {
                return;
            }

            EnsureSkillPickerPanel();
            if (skillPickerPanel == null)
            {
                return;
            }

            for (var i = skillPickerPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(skillPickerPanel.GetChild(i).gameObject);
            }

            CreateCommandReplacementLabel(0, "새 command 장착: " + PublicCommandName(snapshot.PendingCommandEquipId));
            for (var i = 0; i < snapshot.EquippedCommandIds.Length; i++)
            {
                var commandId = snapshot.EquippedCommandIds[i];
                CreateCommandReplacementButton(i + 1, PublicCommandName(commandId), () =>
                {
                    var resolution = _roomController == null ? default : _roomController.ReplacePendingCommand(commandId);
                    HideSkillPicker();
                    ShowResult(resolution);
                    if (_roomController != null)
                    {
                        ShowRunState(_roomController.GetSnapshot());
                    }
                });
            }

            CreateCommandReplacementButton(snapshot.EquippedCommandIds.Length + 1, "취소", () =>
            {
                var resolution = _roomController == null ? default : _roomController.CancelPendingCommandReplacement();
                HideSkillPicker();
                ShowResult(resolution);
                if (_roomController != null)
                {
                    ShowRunState(_roomController.GetSnapshot());
                }
            });
            skillPickerPanel.gameObject.SetActive(true);
        }

        private void CreateCommandReplacementLabel(int row, string labelText)
        {
            var labelObject = new GameObject("Command Replacement Label");
            labelObject.transform.SetParent(skillPickerPanel, false);
            var rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 52f);
            rect.anchoredPosition = new Vector2(0f, -row * 56f);
            var text = labelObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = 20;
            text.raycastTarget = false;
            text.color = PrimaryTextColor;
            text.text = labelText;
        }

        private void CreateCommandReplacementButton(int row, string labelText, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject("Command Replacement Option");
            buttonObject.transform.SetParent(skillPickerPanel, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 48f);
            rect.anchoredPosition = new Vector2(0f, -row * 52f);
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.17f, 0.20f, 0.98f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var text = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 20, TextAnchor.MiddleCenter);
            text.text = labelText;
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
                !ShouldShowCombatDefeatFeedback(snapshot) &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive &&
                !IsMapRouteState(snapshot) &&
                !BossRewardPopupVisible;
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

            return showRawDebugText ? label : SanitizePublicText(label);
        }

        private static bool IsPurchaseChoice(string choiceStableId)
        {
            return !string.IsNullOrEmpty(choiceStableId) && choiceStableId.Contains("_BUY_", StringComparison.Ordinal);
        }

        private static string NormalizeShopDisabledHint(string hint)
        {
            var normalized = NormalizePublicHint(hint);
            if (normalized.Contains("Gold 부족", StringComparison.Ordinal))
            {
                return normalized.Replace("구매 불가: Gold 부족", "Gold 부족", StringComparison.Ordinal);
            }

            return string.IsNullOrEmpty(normalized) ? "구매 불가" : normalized.Replace("선택 불가:", "구매 불가:", StringComparison.Ordinal);
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

        private void RefreshResultSummaryIcons(string message)
        {
            _activeResultSummaryLabels.Clear();
            _activeResultSummaryValues.Clear();

            if (showRawDebugText || string.IsNullOrEmpty(message))
            {
                if (resultIconStrip != null)
                {
                    resultIconStrip.gameObject.SetActive(false);
                }

                return;
            }

            EnsureResultIconStrip();
            if (resultIconStrip == null)
            {
                return;
            }

            var entries = BuildResultSummaryEntries(message);
            for (var i = 0; i < _resultSummaryChips.Count; i++)
            {
                var visible = i < entries.Count;
                _resultSummaryChips[i].SetActive(visible);
                if (!visible)
                {
                    ClearResultSummaryEntry(i);
                    continue;
                }

                ApplyResultSummaryEntry(i, entries[i]);
                _activeResultSummaryLabels.Add(entries[i].Label);
                _activeResultSummaryValues.Add(entries[i].Value);
            }

            resultIconStrip.gameObject.SetActive(entries.Count > 0 && (resultText == null || resultText.gameObject.activeSelf));
        }

        private void ClearResultSummaryEntry(int index)
        {
            if (index < _resultSummaryIconImages.Count && _resultSummaryIconImages[index] != null)
            {
                _resultSummaryIconImages[index].sprite = null;
            }

            if (index < _resultSummaryFallbackTexts.Count && _resultSummaryFallbackTexts[index] != null)
            {
                _resultSummaryFallbackTexts[index].text = string.Empty;
            }

            if (index < _resultSummaryValueTexts.Count && _resultSummaryValueTexts[index] != null)
            {
                _resultSummaryValueTexts[index].text = string.Empty;
            }
        }

        private void ApplyResultSummaryEntry(int index, ResultSummaryEntry entry)
        {
            var icon = index < _resultSummaryIconImages.Count ? _resultSummaryIconImages[index] : null;
            var fallback = index < _resultSummaryFallbackTexts.Count ? _resultSummaryFallbackTexts[index] : null;
            var value = index < _resultSummaryValueTexts.Count ? _resultSummaryValueTexts[index] : null;

            var sprite = ResolveIcon(entry.IconKey);
            if (icon != null)
            {
                icon.sprite = sprite;
                icon.color = sprite == null ? entry.FallbackColor : Color.white;
                icon.gameObject.SetActive(true);
            }

            if (fallback != null)
            {
                fallback.text = sprite == null ? ShortResultLabel(entry.Label) : string.Empty;
                fallback.gameObject.SetActive(sprite == null);
            }

            if (value != null)
            {
                value.text = entry.Value;
            }
        }

        private static List<ResultSummaryEntry> BuildResultSummaryEntries(string message)
        {
            var entries = new List<ResultSummaryEntry>();
            var hpChange = ExtractHpChange(message);
            if (hpChange.StartsWith("HP ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "HP", hpChange.Substring("HP ".Length).Trim(), string.Empty, new Color(0.62f, 0.22f, 0.22f, 0.95f));
            }

            if (string.IsNullOrEmpty(message))
            {
                return entries;
            }

            var tokens = message.Split(new[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                AppendResultSummaryEntryFromToken(entries, tokens[i].Trim());
                if (entries.Count >= ResultIconChipCount)
                {
                    break;
                }
            }

            return entries;
        }

        private static void AppendResultSummaryEntryFromToken(List<ResultSummaryEntry> entries, string token)
        {
            if (string.IsNullOrEmpty(token) || token.StartsWith("Glitch ", StringComparison.Ordinal) || token.StartsWith("glitch ", StringComparison.Ordinal))
            {
                return;
            }

            if (token.StartsWith("HP ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "HP", token.Substring("HP ".Length).Trim(), string.Empty, new Color(0.62f, 0.22f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("Gold ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Gold", token.Substring("Gold ".Length).Trim(), "resource.gold", new Color(0.82f, 0.62f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("gold reward ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Gold", "+" + token.Substring("gold reward ".Length).Trim().TrimStart('+'), "resource.gold", new Color(0.82f, 0.62f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("xp +", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "XP", "+" + token.Substring("xp +".Length).Trim().TrimStart('+'), string.Empty, new Color(0.26f, 0.48f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("xp ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "XP", token.Substring("xp ".Length).Trim(), string.Empty, new Color(0.26f, 0.48f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("level reward ready", StringComparison.Ordinal) || token.StartsWith("level ready", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Level", "보상", string.Empty, new Color(0.38f, 0.64f, 0.62f, 0.95f));
                return;
            }

            if (token.StartsWith("Affinity ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Affinity", token.Substring("Affinity ".Length).Trim(), "resource.affinity", new Color(0.55f, 0.34f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("affinity ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Affinity", token.Substring("affinity ".Length).Trim(), "resource.affinity", new Color(0.55f, 0.34f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("item ", StringComparison.Ordinal))
            {
                var payload = token.Substring("item ".Length).Trim();
                AppendResultSummaryEntry(entries, "Item", ExtractRefDeltaValue(payload), ResultIconKeyForRef(ExtractRefId(payload)), new Color(0.30f, 0.48f, 0.42f, 0.95f));
                return;
            }

            if (token.StartsWith("ability ", StringComparison.Ordinal))
            {
                var payload = token.Substring("ability ".Length).Trim();
                AppendResultSummaryEntry(entries, "Ability", ExtractRefDeltaValue(payload), ResultIconKeyForRef(ExtractRefId(payload)), new Color(0.55f, 0.50f, 0.25f, 0.95f));
                return;
            }

            if (token.StartsWith("memory unlocked ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "기억의 잔향", "획득", "resource.memory", new Color(0.30f, 0.50f, 0.68f, 0.95f));
                return;
            }

            if (token == PrototypeRunState.MemoryFragmentPublicFeedback)
            {
                AppendResultSummaryEntry(entries, "기억의 잔향", "획득", "resource.memory", new Color(0.30f, 0.50f, 0.68f, 0.95f));
                return;
            }

            if (token == PrototypeRunState.MemoryConsequenceFeedback)
            {
                AppendResultSummaryEntry(entries, "기억", "기록", "resource.memory", new Color(0.30f, 0.50f, 0.68f, 0.95f));
                return;
            }

            if (token.StartsWith("jar outcome: Gold +8", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Gold", "+8", "resource.gold", new Color(0.82f, 0.62f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("jar outcome: HP +5", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "HP", "+5", string.Empty, new Color(0.62f, 0.22f, 0.22f, 0.95f));
            }
        }

        private static void AppendResultSummaryEntry(List<ResultSummaryEntry> entries, string label, string value, string iconKey, Color fallbackColor)
        {
            if (entries.Count >= ResultIconChipCount || string.IsNullOrEmpty(value) || IsNoOpDelta(value))
            {
                return;
            }

            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].Label == label && entries[i].Value == value)
                {
                    return;
                }
            }

            entries.Add(new ResultSummaryEntry(label, value, iconKey, fallbackColor));
        }

        private static string ExtractRefId(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var parts = value.Split(' ');
            return parts.Length == 0 ? string.Empty : parts[0];
        }

        private static string ExtractRefDeltaValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "+1";
            }

            var parts = value.Split(' ');
            return parts.Length > 1 ? parts[parts.Length - 1] : "+1";
        }

        private static string ResultIconKeyForRef(string reference)
        {
            return reference switch
            {
                "ITEM_FIELD_BANDAGE" => "item.field_bandage",
                "ITEM_LANTERN_OIL" => "item.lantern_oil",
                "ITEM_TORN_CHARM" => "item.torn_charm",
                "ABILITY_SCOUT" => "ability.scout",
                "ABILITY_RECALL_ANCHOR" => "ability.recall_anchor",
                _ => string.Empty
            };
        }

        private static string ShortResultLabel(string label)
        {
            return label switch
            {
                "Gold" => "G",
                "Affinity" => "신",
                "Item" => "I",
                "Ability" => "A",
                "Memory" => "기",
                _ => label
            };
        }

        private string BuildResultSummary(string message)
        {
            if (showRawDebugText)
            {
                return "result: " + message;
            }

            if (IsBossClearResult(message))
            {
                return BuildBossClearResultSummary(message);
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

        private static bool IsBossClearResult(string message)
        {
            return !string.IsNullOrEmpty(message) &&
                message.Contains("enemyDefeated True", StringComparison.Ordinal) &&
                (message.Contains("stair unlocked", StringComparison.OrdinalIgnoreCase) ||
                 message.Contains("run.clear", StringComparison.Ordinal));
        }

        private static string BuildBossClearResultSummary(string message)
        {
            var summary = "결과";
            AppendResultLine(ref summary, message.Contains("run.clear", StringComparison.Ordinal) ? "최종 보스 격파" : "보스 격파", allowOverflow: true);
            AppendResultLine(ref summary, ExtractTokenResult(message, "gold reward "), allowOverflow: true);
            AppendResultLine(ref summary, ExtractTokenResult(message, "affinity "), allowOverflow: true);
            AppendResultLine(ref summary, message.Contains("run.clear", StringComparison.Ordinal) ? "엔딩 선택 가능" : "다음 층 준비", allowOverflow: true);
            return summary;
        }

        private static string ExtractTokenResult(string message, string tokenPrefix)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(tokenPrefix))
            {
                return string.Empty;
            }

            var tokens = message.Split(new[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var normalized = NormalizeResultToken(tokens[i].Trim());
                if (!string.IsNullOrEmpty(normalized) && tokens[i].Trim().StartsWith(tokenPrefix, StringComparison.Ordinal))
                {
                    return normalized;
                }
            }

            return string.Empty;
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
                return "아이템 " + ExtractRefDeltaValue(token.Substring("item ".Length).Trim());
            }

            if (token.StartsWith("ability ", StringComparison.Ordinal))
            {
                return "능력 " + ExtractRefDeltaValue(token.Substring("ability ".Length).Trim());
            }

            if (token.StartsWith("reward ", StringComparison.Ordinal))
            {
                if (token.Contains(PrototypeRunState.MemoryConsequenceRewardBundleRef, StringComparison.Ordinal))
                {
                    return PrototypeRunState.MemoryConsequenceFeedback;
                }

                return "보상: " + NormalizeRefDelta(token.Substring("reward ".Length).Trim());
            }

            if (token.StartsWith("memory unlocked ", StringComparison.Ordinal))
            {
                return PrototypeRunState.MemoryFragmentPublicFeedback;
            }

            if (token == PrototypeRunState.MemoryFragmentPublicFeedback ||
                token == PrototypeRunState.MemoryConsequenceFeedback)
            {
                return token;
            }

            if (token.StartsWith("action ", StringComparison.Ordinal))
            {
                return "행동: " + PublicCombatActionName(token.Substring("action ".Length).Trim());
            }

            if (token.StartsWith("jar outcome: Gold +8", StringComparison.Ordinal))
            {
                return "Gold +8";
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

        private string BuildCombatLogRecord(PrototypeRunSnapshot snapshot)
        {
            var feedback = BuildCombatFeedback(snapshot.LastCombatRoundResult);
            var detail = BuildCombatLogDetailLine(snapshot);
            var record = string.IsNullOrEmpty(detail)
                ? feedback
                : feedback + " | " + detail;
            return ShortenPublicLine(record, 78);
        }

        private string BuildCombatLogDetailLine(PrototypeRunSnapshot snapshot)
        {
            var highlights = new List<string>();
            var extra = BuildCombatExtraLine(snapshot);
            if (!string.IsNullOrEmpty(extra))
            {
                highlights.Add(extra);
            }

            var playerDamage = ExtractRoundNumber(snapshot.LastCombatRoundResult, "playerDamage ").Trim();
            if (!string.IsNullOrEmpty(playerDamage) && playerDamage != "0")
            {
                highlights.Add("플레이어: " + playerDamage + " 피해");
            }

            var hpChange = ExtractRoundNumber(snapshot.LastCombatRoundResult, "playerHp ").Trim();
            if (!string.IsNullOrEmpty(hpChange) && !IsNoOpDelta(hpChange))
            {
                highlights.Add("HP " + hpChange);
            }

            if (snapshot.LastCombatRoundResult.Contains("scout attack +", StringComparison.Ordinal))
            {
                highlights.Add("정찰: +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "scout attack +").Trim());
            }

            if (snapshot.LastCombatRoundResult.Contains("scout guard ", StringComparison.Ordinal))
            {
                highlights.Add("정찰: 다음 피해 감소 " + ExtractRoundNumber(snapshot.LastCombatRoundResult, "scout guard ").Trim());
            }

            if (snapshot.LastMataiosProtectReduction > 0)
            {
                highlights.Add("마타이오스 보호: 피해 " + snapshot.LastMataiosProtectReduction + " 감소");
            }

            if (snapshot.LastMataiosCombatDamage > 0)
            {
                highlights.Add("마타이오스: " + snapshot.LastMataiosCombatDamage + " 지원 피해");
            }

            if (highlights.Count > 0)
            {
                return JoinCompactChips(highlights, 4);
            }

            if (HasArtsSkill())
            {
                return "번개 방출: 직접 피해";
            }

            return HasScoutSkill(snapshot) ? "정찰: 다음 공격 강화" : "기술 불가: 보유 스킬 필요";
        }

        private static string BuildCombatBuildSummaryLine(PrototypeRunSnapshot snapshot)
        {
            if (!string.IsNullOrEmpty(snapshot.LastGrowthMessage))
            {
                return "보상: " + snapshot.LastGrowthMessage;
            }

            var chips = new List<string>();
            if (snapshot.ScoutAttackReady)
            {
                chips.Add("정찰 공격 준비");
            }

            if (snapshot.ScoutDamageReductionReady)
            {
                chips.Add("정찰 피해감소 준비");
            }

            if (snapshot.LevelAttackBonus > 0)
            {
                chips.Add("ATK +" + snapshot.LevelAttackBonus);
            }

            if (snapshot.LevelMaxHpBonus > 0)
            {
                chips.Add("HP +" + snapshot.LevelMaxHpBonus);
            }

            if (snapshot.SkillCooldownReduction > 0)
            {
                chips.Add("CD -" + snapshot.SkillCooldownReduction);
            }

            return chips.Count == 0 ? string.Empty : "상태: " + JoinCompactChips(chips, 3);
        }

        private static string BuildCombatExtraLine(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.LastCombatComboDamage > 0)
            {
                return "연속 행동: 추가 공격 " + snapshot.LastCombatComboDamage;
            }

            if (snapshot.LastCombatRoundResult.Contains("scout attack ready", StringComparison.Ordinal))
            {
                return "정찰: 다음 공격 강화";
            }

            if (snapshot.LastCombatRoundResult.Contains("scout guard ready", StringComparison.Ordinal))
            {
                return "정찰: 다음 피해 감소 1회";
            }

            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                return "단련 보너스: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "training +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("frenzy break", StringComparison.Ordinal))
            {
                return "광폭 끊김";
            }

            if (snapshot.LastCombatRoundResult.Contains("frenzy ", StringComparison.Ordinal))
            {
                return "광폭 발동: 추가타 " + ExtractRoundNumber(snapshot.LastCombatRoundResult, "frenzy ").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("oil skill +", StringComparison.Ordinal))
            {
                return "등유: 스킬 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "oil skill +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("skill opening +", StringComparison.Ordinal))
            {
                return "빈틈 공략: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "skill opening +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("heavy pressure +", StringComparison.Ordinal))
            {
                return "중압: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "heavy pressure +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("heavy pressure blocked", StringComparison.Ordinal))
            {
                return "방어: 중압 차단";
            }

            if (snapshot.LastCombatRoundResult.Contains("first hit guard ", StringComparison.Ordinal))
            {
                return "찢어진 부적: 피해 -" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "first hit guard ").Trim();
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

        private static string BuildCombatEnemyStatusChips(PrototypeRunSnapshot snapshot)
        {
            var chips = new List<string>();
            if (snapshot.EnemyAttack > 0)
            {
                chips.Add("ATK " + snapshot.EnemyAttack);
            }

            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            if (result.Contains("poison ", StringComparison.Ordinal))
            {
                chips.Add("중독");
            }

            if (result.Contains("heavy pressure", StringComparison.Ordinal))
            {
                chips.Add("중압");
            }

            if (result.Contains("skill opening", StringComparison.Ordinal))
            {
                chips.Add("빈틈");
            }

            if (chips.Count > 4)
            {
                var overflow = chips.Count - 3;
                chips.RemoveRange(3, overflow);
                chips.Add("+" + overflow);
            }

            return string.Join("  ", chips);
        }

        private string BuildCombatItemInspectText()
        {
            if (_roomController == null || _roomController.RunState == null)
            {
                return "아이템 없음";
            }

            var lines = new List<string>();
            AddCombatInspectItemLine(lines, "ITEM_FIELD_BANDAGE", "붕대", "전투 시작 HP 회복 +4 / 최대 HP +2");
            AddCombatInspectItemLine(lines, "ITEM_LANTERN_OIL", "등유", "스킬 피해 +2");
            AddCombatInspectItemLine(lines, "ITEM_TORN_CHARM", "찢어진 부적", "첫 피격 피해 -2");
            return lines.Count == 0 ? "아이템 없음" : string.Join("\n", lines);
        }

        private void AddCombatInspectItemLine(List<string> lines, string itemRef, string label, string summary)
        {
            var count = _roomController == null || _roomController.RunState == null ? 0 : _roomController.RunState.GetItemCount(itemRef);
            if (count <= 0)
            {
                return;
            }

            lines.Add(label + " x" + count + " - " + summary);
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

            if (result.Contains("action Skill", StringComparison.Ordinal) || result.Contains("scout attack +", StringComparison.Ordinal))
            {
                return "의도: 빈틈 노출";
            }

            return "의도: 반격 준비";
        }

        private static string BuildCombatDecisionLine(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.IsInCombat)
            {
                return "판단: 전투 결과 확인";
            }

            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            if (snapshot.ScoutAttackReady && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return "판단: 공격으로 정찰 보너스 발동";
            }

            if (HasSkillOpeningCue(snapshot) && HasCombatSkillEquipped(snapshot))
            {
                return result.Contains("skill opening missed", StringComparison.Ordinal)
                    ? "판단: 빈틈을 놓침 - 다음엔 스킬"
                    : "판단: 빈틈 - 스킬로 추가 피해";
            }

            if (HasHeavyPressureCue(snapshot))
            {
                return result.Contains("heavy pressure blocked", StringComparison.Ordinal)
                    ? "판단: 중압 차단 성공"
                    : "판단: 중압 - 방어로 추가 피해 차단";
            }

            if (result.Contains("frenzy ready", StringComparison.Ordinal) && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return "판단: 공격 유지 시 광폭 연결";
            }

            if (snapshot.EnemyHp > 0 && snapshot.EnemyHp <= snapshot.PlayerAttack + 2 && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return "판단: 공격으로 마무리 가능";
            }

            if (snapshot.EnemyAttack > 0)
            {
                return "판단: 적 ATK " + snapshot.EnemyAttack + " - 피해 관리 필요";
            }

            return "판단: 공격으로 압박";
        }

        private static CombatAction? ResolveRecommendedCombatAction(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.IsInCombat)
            {
                return null;
            }

            if (snapshot.ScoutAttackReady && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return CombatAction.Attack;
            }

            if (HasSkillOpeningCue(snapshot) && HasCombatSkillEquipped(snapshot))
            {
                return CombatAction.Skill;
            }

            if (HasHeavyPressureCue(snapshot) && snapshot.IsCommandEquipped(PrototypeRunState.CommandDefendId))
            {
                return CombatAction.Defend;
            }

            if (snapshot.EnemyHp > 0 && snapshot.EnemyHp <= snapshot.PlayerAttack + 2 && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return CombatAction.Attack;
            }

            if (snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return CombatAction.Attack;
            }

            if (snapshot.IsCommandEquipped(PrototypeRunState.CommandDefendId))
            {
                return CombatAction.Defend;
            }

            return HasCombatSkillEquipped(snapshot) ? CombatAction.Skill : (CombatAction?)null;
        }

        private static bool HasCombatSkillEquipped(PrototypeRunSnapshot snapshot)
        {
            return snapshot.IsCommandEquipped(PrototypeRunState.CommandScoutId) ||
                snapshot.IsCommandEquipped(PrototypeRunState.CommandArts03Id);
        }

        private static bool HasHeavyPressureCue(PrototypeRunSnapshot snapshot)
        {
            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            return snapshot.EnemyAttack >= 4 ||
                result.Contains("heavy pressure", StringComparison.Ordinal);
        }

        private static bool HasSkillOpeningCue(PrototypeRunSnapshot snapshot)
        {
            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            return snapshot.EnemyMaxHp > 0 && snapshot.EnemyHp * 2 <= snapshot.EnemyMaxHp ||
                result.Contains("skill opening", StringComparison.Ordinal);
        }

        private string BuildCombatPlayerCardText(PrototypeRunSnapshot snapshot)
        {
            return "플레이어\n" +
                "HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + "  ATK " + snapshot.PlayerAttack + "\n" +
                BuildPlayerBuffChipLine(snapshot);
        }

        private string BuildCombatMataiosCardText(PrototypeRunSnapshot snapshot)
        {
            return "마타이오스\n" +
                "HP " + BuildMataiosHp(snapshot) + "  ATK " + BuildMataiosAttack(snapshot) + "\n" +
                BuildMataiosBuffLine(snapshot);
        }

        private string BuildPlayerBuffChipLine(PrototypeRunSnapshot snapshot)
        {
            var chips = new List<string>();
            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                chips.Add("단련 +1");
            }

            if (snapshot.LevelAttackBonus > 0)
            {
                chips.Add("성장 ATK +" + snapshot.LevelAttackBonus);
            }

            if (snapshot.LevelMaxHpBonus > 0)
            {
                chips.Add("성장 HP +" + snapshot.LevelMaxHpBonus);
            }

            if (snapshot.SkillCooldownReduction > 0)
            {
                chips.Add("CD -" + snapshot.SkillCooldownReduction);
            }

            if (snapshot.LastCombatRoundResult.Contains("frenzy break", StringComparison.Ordinal))
            {
                chips.Add("광폭 끊김");
            }
            else if (snapshot.LastCombatRoundResult.Contains("frenzy ", StringComparison.Ordinal))
            {
                chips.Add("광폭 발동");
            }
            else if (snapshot.CombatBuildSummary.Contains("광폭 준비", StringComparison.Ordinal))
            {
                chips.Add("광폭 준비");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0)
            {
                chips.Add("붕대");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_LANTERN_OIL") > 0)
            {
                chips.Add("등유");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_TORN_CHARM") > 0)
            {
                chips.Add("부적");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
            {
                chips.Add("회상 닻");
            }

            return chips.Count == 0 ? "상태 없음" : JoinCompactChips(chips, 2);
        }

        private static string BuildMataiosChipLine(PrototypeRunSnapshot snapshot)
        {
            var affinity = snapshot.Affinity >= 4 ? "신뢰 높음" :
                snapshot.Affinity > 0 ? "신뢰 형성" :
                snapshot.Affinity < 0 ? "거리감" :
                "동행 중";
            return affinity + "  기억 " + snapshot.MemoryFragmentCount + "  안정";
        }

        private static string BuildMataiosHp(PrototypeRunSnapshot snapshot)
        {
            var max = Mathf.Max(1, snapshot.MataiosMaxHp);
            var current = Mathf.Clamp(snapshot.MataiosHp, 0, max);
            return current + "/" + max;
        }

        private static int BuildMataiosAttack(PrototypeRunSnapshot snapshot)
        {
            return Mathf.Max(0, snapshot.MataiosAttack);
        }

        private static string BuildMataiosBuffLine(PrototypeRunSnapshot snapshot)
        {
            var buffs = new List<string>();
            if (snapshot.MataiosDown)
            {
                buffs.Add("전투 불능");
            }

            if (snapshot.LastMataiosProtectReduction > 0)
            {
                buffs.Add("보호 -" + snapshot.LastMataiosProtectReduction);
            }

            if (snapshot.LastMataiosCombatDamage > 0)
            {
                buffs.Add("지원 피해 " + snapshot.LastMataiosCombatDamage);
            }

            if (snapshot.LastMataiosDownEvent)
            {
                buffs.Add("붕괴도 +5");
            }

            return buffs.Count == 0 ? "상태 없음" : JoinCompactChips(buffs, 2);
        }

        private static string BuildCombatFeedback(string roundResult)
        {
            if (string.IsNullOrEmpty(roundResult))
            {
                return "전투 준비";
            }

            if (roundResult.Contains("skill unavailable", StringComparison.Ordinal))
            {
                return "기술 불가: 보유 스킬 필요";
            }

            if (roundResult.Contains("recall anchor", StringComparison.Ordinal))
            {
                return "회상 닻: HP 회복 후 전투 지속";
            }

            if (roundResult.Contains("ready", StringComparison.OrdinalIgnoreCase))
            {
                var ready = "전투 준비";
                if (roundResult.Contains("scout setup", StringComparison.Ordinal))
                {
                    ready += " | 정찰 준비";
                }

                return ready;
            }

            var enemyDamage = ExtractRoundNumber(roundResult, "enemyDamage ").Trim();
            if (roundResult.Contains("Defend", StringComparison.Ordinal))
            {
                return "선택 방어 | 받은 피해 " + enemyDamage + " | 절반 감소";
            }

            if (roundResult.Contains("Attack", StringComparison.Ordinal))
            {
                var prefix = roundResult.Contains("blood overload", StringComparison.Ordinal)
                    ? "피의 서약 과부하 | "
                    : string.Empty;
                return prefix + "선택 공격 | 적 피해 " + ExtractRoundNumber(roundResult, "playerDamage ").Trim() +
                    " | 받은 피해 " + enemyDamage;
            }

            if (roundResult.Contains("Skill", StringComparison.Ordinal))
            {
                if (roundResult.Contains("scout skill", StringComparison.Ordinal))
                {
                    return "선택 정찰 | 다음 공격 강화 | 다음 피해 감소 1회";
                }

                return "선택 스킬 | 적 피해 " + ExtractRoundNumber(roundResult, "playerDamage ").Trim() +
                    " | 받은 피해 " + enemyDamage;
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

        private bool HasAnyCombatSkill()
        {
            return HasScoutSkill(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot()) || HasArtsSkill();
        }

        private List<string> BuildOwnedCombatSkillNames()
        {
            var skills = new List<string>();
            if (_roomController != null && _roomController.RunState != null)
            {
                if (_roomController.RunState.HasAbilityRef("ABILITY_SCOUT"))
                {
                    skills.Add("정찰");
                }

                if (_roomController.RunState.HasAbilityRef("ABILITY_ARTS_03"))
                {
                    skills.Add("번개 방출");
                }
            }
            else if (_lastSnapshot.AbilityCount > 0)
            {
                skills.Add("스킬");
            }

            return skills;
        }

        private string BuildOwnedSkillLine()
        {
            var skills = new List<string>();
            if (_roomController != null && _roomController.RunState != null)
            {
                if (_roomController.RunState.HasAbilityRef("ABILITY_SCOUT"))
                {
                    skills.Add("정찰");
                }

                if (_roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
                {
                    skills.Add("회상 닻");
                }

                if (_roomController.RunState.HasAbilityRef("ABILITY_ARTS_03"))
                {
                    skills.Add("번개 방출");
                }
            }
            else if (_lastSnapshot.AbilityCount > 0)
            {
                skills.Add("스킬");
            }

            return skills.Count == 0 ? "없음" : string.Join("  ", skills);
        }

        private string BuildOwnedItemLine()
        {
            if (_roomController == null || _roomController.RunState == null)
            {
                return "아이템 " + _lastSnapshot.ItemCount;
            }

            var items = new List<string>();
            AddOwnedItemLine(items, "ITEM_FIELD_BANDAGE", "붕대");
            AddOwnedItemLine(items, "ITEM_01", "붕대 뭉치");
            AddOwnedItemLine(items, "ITEM_02", "작은 룬석");
            AddOwnedItemLine(items, "ITEM_03", "날카로운 숫돌");
            AddOwnedItemLine(items, "ITEM_04", "낡은 방패 조각");
            AddOwnedItemLine(items, "ITEM_05", "독침");
            AddOwnedItemLine(items, "ITEM_09", "마모된 부적");
            AddOwnedItemLine(items, "ITEM_10", "피의 계약서");
            AddOwnedItemLine(items, "RELIC_GENERIC_01", "회귀자의 낡은 코트");
            AddOwnedItemLine(items, "RELIC_SWORD_01", "피묻은 칼날");
            AddOwnedItemLine(items, "RELIC_LINE_01", "저격수의 망원경");
            AddOwnedItemLine(items, "RELIC_ARTS_01", "원소 수정");
            AddOwnedItemLine(items, "RELIC_GUARD_01", "강화 방패");
            return items.Count == 0 ? "보유 아이템 없음" : string.Join("\n", items);
        }

        private bool HasArtsSkill()
        {
            return _roomController != null &&
                _roomController.RunState != null &&
                _roomController.RunState.HasAbilityRef("ABILITY_ARTS_03");
        }

        private void AddOwnedItemLine(List<string> items, string itemRef, string label)
        {
            var count = _roomController == null || _roomController.RunState == null ? 0 : _roomController.RunState.GetItemCount(itemRef);
            if (count > 0)
            {
                items.Add(label + " x" + count);
            }
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

            if (IsMapRouteState(snapshot))
            {
                HideLegacyEncounterVisuals(hideBackground: true);
                return;
            }

            var slot = ResolveCurrentPresentationSlot(snapshot);
            ApplyPresentationSlot(slot);
            if (_shopPresentationActive || snapshot.IsInCombat || _eventPresentationActive)
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

        private void ApplyRestPresentationSlot(string encounterStableId)
        {
            if (presentationData == null)
            {
                return;
            }

            if (presentationData.TryGetSlot(encounterStableId, out var slot))
            {
                _activePresentationEncounterId = encounterStableId;
                ApplyPresentationSlot(slot);
                return;
            }

            if (presentationData.TryGetSlot("ENC_REST_01", out var fallbackSlot))
            {
                _activePresentationEncounterId = "ENC_REST_01";
                ApplyPresentationSlot(fallbackSlot);
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

            if (message.Contains("memory unlocked", StringComparison.Ordinal) ||
                message.Contains(PrototypeRunState.MemoryFragmentPublicFeedback, StringComparison.Ordinal))
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

        private static bool IsBossGateEncounter(EncounterData encounter)
        {
            return encounter != null &&
                (encounter.Id == "ENC_COMBAT_GATE_01" ||
                 encounter.Id == "ENC_COMBAT_GATE_02" ||
                 encounter.Id == "ENC_COMBAT_GATE_03");
        }

        private static PrototypeEncounterChoiceView[] BuildBossGateChoiceViews(PrototypeEncounterChoiceView[] sourceViews)
        {
            PrototypeEncounterChoiceView engage = default;
            var hasEngage = false;
            if (sourceViews != null)
            {
                for (var i = 0; i < sourceViews.Length; i++)
                {
                    var view = sourceViews[i];
                    if (view.Visible && view.ChoiceStableId.Contains("_ENGAGE", StringComparison.Ordinal))
                    {
                        engage = new PrototypeEncounterChoiceView(
                            view.ChoiceStableId,
                            view.TextKey,
                            true,
                            view.Enabled,
                            view.ReasonTextKey,
                            string.Empty);
                        hasEngage = true;
                        break;
                    }
                }
            }

            if (!hasEngage)
            {
                engage = new PrototypeEncounterChoiceView("CHOICE_COMBAT_02_ENGAGE", string.Empty, true, true, string.Empty, string.Empty);
            }

            return new[] { engage };
        }

        private static bool IsCombatEncounterId(string encounterId)
        {
            return encounterId == "ENC_COMBAT_GATE_01" || encounterId == "ENC_COMBAT_GATE_02" || encounterId == "ENC_COMBAT_GATE_03";
        }

        private static string ResolvePublicChoiceLabel(string choiceStableId, int index)
        {
            if (!string.IsNullOrEmpty(choiceStableId))
            {
                if (choiceStableId.Contains("SHOP", StringComparison.Ordinal) &&
                    choiceStableId.Contains("_LEAVE", StringComparison.Ordinal))
                {
                    return "상점 나가기";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_PATTERNED" ||
                    choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_PATTERNED_ELITE_COMBAT")
                {
                    return "신기한 문양이 각인된 항아리";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_PLAIN")
                {
                    return "평범한 항아리";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_CRACKED")
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
                    return "전투 시작";
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
                "CHOICE_F02_SHOP_BUY_ITEM" => "구매",
                "CHOICE_F02_SHOP_BUY_ABILITY" => "구매",
                "CHOICE_F02_SHOP_LEAVE" => "지나간다",
                "CHOICE_F02_MORAL_HELP" => "돕는다",
                "CHOICE_F02_MORAL_LEAVE" => "거절한다",
                "CHOICE_F02_MORAL_BARGAIN" => "거래한다",
                "CHOICE_COMBAT_02_ENGAGE" => "전투",
                "CHOICE_COMBAT_03_ENGAGE" => "전투",
                _ => "선택 " + (index + 1)
            };
        }

        private static string ResolvePublicEncounterLabel(string configuredLabel, EncounterData encounter)
        {
            if (!string.IsNullOrEmpty(configuredLabel) && !LooksLikeInternalLabel(configuredLabel))
            {
                return SanitizePublicText(configuredLabel);
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
                .Replace("Ability 필요", "능력 필요", StringComparison.Ordinal)
                .Replace("Glitch -1", "불안 감소", StringComparison.Ordinal)
                .Replace("Glitch +1", "불안 증가", StringComparison.Ordinal);

            return SanitizePublicText(ReplacePublicRefs(normalized));
        }

        private static string ReplacePublicRefs(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var replaced = value
                .Replace("TriggerGameOver", "실패", StringComparison.Ordinal)
                .Replace("MoralChoice", "선택", StringComparison.Ordinal)
                .Replace("도덕 선택", "선택", StringComparison.Ordinal)
                .Replace("MemoryFragment", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_01", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_02", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_03", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_04", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_05", "기억의 잔향", StringComparison.Ordinal)
                .Replace("ITEM_FIELD_BANDAGE", PublicRefName("ITEM_FIELD_BANDAGE"), StringComparison.Ordinal)
                .Replace("ITEM_LANTERN_OIL", PublicRefName("ITEM_LANTERN_OIL"), StringComparison.Ordinal)
                .Replace("ITEM_TORN_CHARM", PublicRefName("ITEM_TORN_CHARM"), StringComparison.Ordinal)
                .Replace("ITEM_10", PublicRefName("ITEM_10"), StringComparison.Ordinal)
                .Replace("ITEM_01", PublicRefName("ITEM_01"), StringComparison.Ordinal)
                .Replace("ITEM_02", PublicRefName("ITEM_02"), StringComparison.Ordinal)
                .Replace("ITEM_03", PublicRefName("ITEM_03"), StringComparison.Ordinal)
                .Replace("ITEM_04", PublicRefName("ITEM_04"), StringComparison.Ordinal)
                .Replace("ITEM_05", PublicRefName("ITEM_05"), StringComparison.Ordinal)
                .Replace("ITEM_09", PublicRefName("ITEM_09"), StringComparison.Ordinal)
                .Replace("RELIC_GENERIC_01", PublicRefName("RELIC_GENERIC_01"), StringComparison.Ordinal)
                .Replace("RELIC_SWORD_01", PublicRefName("RELIC_SWORD_01"), StringComparison.Ordinal)
                .Replace("RELIC_LINE_01", PublicRefName("RELIC_LINE_01"), StringComparison.Ordinal)
                .Replace("RELIC_ARTS_01", PublicRefName("RELIC_ARTS_01"), StringComparison.Ordinal)
                .Replace("RELIC_GUARD_01", PublicRefName("RELIC_GUARD_01"), StringComparison.Ordinal)
                .Replace("ABILITY_SCOUT", PublicRefName("ABILITY_SCOUT"), StringComparison.Ordinal)
                .Replace("ABILITY_RECALL_ANCHOR", PublicRefName("ABILITY_RECALL_ANCHOR"), StringComparison.Ordinal)
                .Replace("ABILITY_SWORD_01", PublicRefName("ABILITY_SWORD_01"), StringComparison.Ordinal)
                .Replace("ABILITY_SWORD_02", PublicRefName("ABILITY_SWORD_02"), StringComparison.Ordinal)
                .Replace("ABILITY_SWORD_03", PublicRefName("ABILITY_SWORD_03"), StringComparison.Ordinal)
                .Replace("ABILITY_ARTS_03", PublicRefName("ABILITY_ARTS_03"), StringComparison.Ordinal)
                .Replace("ABILITY_GUARD_01", PublicRefName("ABILITY_GUARD_01"), StringComparison.Ordinal)
                .Replace("REWARD_CACHE_SMALL", PublicRefName("REWARD_CACHE_SMALL"), StringComparison.Ordinal)
                .Replace("REWARD_CACHE_MEMORY", PublicRefName("REWARD_CACHE_MEMORY"), StringComparison.Ordinal);
            return StripInternalReferenceTokens(replaced);
        }

        private static string SanitizePublicText(string value)
        {
            return StripInternalReferenceTokens(value);
        }

        private static string StripInternalReferenceTokens(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var text = value
                .Replace("[current]", "현재", StringComparison.Ordinal)
                .Replace("[locked]", "잠김", StringComparison.Ordinal)
                .Replace("[complete]", "완료", StringComparison.Ordinal)
                .Replace("[cleared]", "완료", StringComparison.Ordinal)
                .Replace("MoralChoice", "선택", StringComparison.Ordinal)
                .Replace("MemoryFragment", "기억의 잔향", StringComparison.Ordinal)
                .Replace("TriggerGameOver", "실패", StringComparison.Ordinal)
                .Replace("도덕 선택", "선택", StringComparison.Ordinal)
                .Replace("Unavailable", "선택 불가", StringComparison.Ordinal)
                .Replace("PLACEHOLDER_", string.Empty, StringComparison.Ordinal);

            text = ReplaceInternalTokenPrefix(text, "CHOICE_", "선택");
            text = ReplaceInternalTokenPrefix(text, "ENC_COMBAT_GATE_", "전투");
            text = ReplaceInternalTokenPrefix(text, "ENC_SHOP_", "상점");
            text = ReplaceInternalTokenPrefix(text, "ENC_REST_", "휴식");
            text = ReplaceInternalTokenPrefix(text, "ENC_MORAL_", "선택");
            text = ReplaceInternalTokenPrefix(text, "ENC_MEMORY_", "기억의 잔향");
            text = ReplaceInternalTokenPrefix(text, "ENC_", "조우");
            text = ReplaceInternalTokenPrefix(text, "EVT_", "이벤트");
            text = ReplaceInternalTokenPrefix(text, "MEM_FRAGMENT_", "기억의 잔향");
            while (text.Contains("  ", StringComparison.Ordinal))
            {
                text = text.Replace("  ", " ", StringComparison.Ordinal);
            }

            return text.Trim();
        }

        private static string ReplaceInternalTokenPrefix(string value, string prefix, string replacement)
        {
            var result = value;
            var searchStart = 0;
            while (searchStart < result.Length)
            {
                var start = result.IndexOf(prefix, searchStart, StringComparison.Ordinal);
                if (start < 0)
                {
                    break;
                }

                var end = start + prefix.Length;
                while (end < result.Length && IsInternalTokenCharacter(result[end]))
                {
                    end++;
                }

                result = result.Substring(0, start) + replacement + result.Substring(end);
                searchStart = start + replacement.Length;
            }

            return result;
        }

        private static bool IsInternalTokenCharacter(char value)
        {
            return char.IsLetterOrDigit(value) || value == '_' || value == '-' || value == '.';
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
                "ITEM_01" => "붕대 뭉치",
                "ITEM_02" => "작은 룬석",
                "ITEM_03" => "날카로운 숫돌",
                "ITEM_04" => "낡은 방패 조각",
                "ITEM_05" => "독침",
                "ITEM_09" => "마모된 부적",
                "ITEM_10" => "피의 계약서",
                "RELIC_GENERIC_01" => "회귀자의 낡은 코트",
                "RELIC_SWORD_01" => "피묻은 칼날",
                "RELIC_LINE_01" => "저격수의 망원경",
                "RELIC_ARTS_01" => "원소 수정",
                "RELIC_GUARD_01" => "강화 방패",
                "ABILITY_SCOUT" => "정찰",
                "ABILITY_RECALL_ANCHOR" => "회상 닻",
                "ABILITY_SWORD_01" => "예리한 감각",
                "ABILITY_SWORD_02" => "연속베기",
                "ABILITY_SWORD_03" => "피의 서약",
                "ABILITY_ARTS_03" => "번개 방출",
                "ABILITY_GUARD_01" => "철벽의 태세",
                "REWARD_CACHE_SMALL" => "작은 보급품",
                "REWARD_CACHE_MEMORY" => PrototypeRunState.MemoryConsequenceFeedback,
                "MEM_FRAGMENT_01" => "기억의 잔향",
                "MEM_FRAGMENT_02" => "기억의 잔향",
                "MEM_FRAGMENT_03" => "기억의 잔향",
                "MEM_FRAGMENT_04" => "기억의 잔향",
                "MEM_FRAGMENT_05" => "기억의 잔향",
                _ => LooksLikeInternalLabel(reference) || reference.Contains("_", StringComparison.Ordinal) ? "획득물" : reference
            };
        }

        private static string PublicCommandName(string commandId)
        {
            return commandId switch
            {
                PrototypeRunState.CommandAttackId => "공격",
                PrototypeRunState.CommandDefendId => "방어",
                PrototypeRunState.CommandScoutId => "정찰",
                PrototypeRunState.CommandArts03Id => "번개 방출",
                _ => string.IsNullOrEmpty(commandId) ? "command" : "스킬"
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

        private static bool IsBossEnemyId(string enemyId)
        {
            return enemyId == "BOSS_GATE_01" || enemyId == "BOSS_APEX_02";
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
