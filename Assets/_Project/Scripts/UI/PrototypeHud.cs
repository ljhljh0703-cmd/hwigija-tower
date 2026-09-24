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
    public sealed partial class PrototypeHud : MonoBehaviour
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
        [SerializeField] private ShopUiController shopUiController;
        [SerializeField] private EventUiController eventUiController;
        [SerializeField] private BossGateUiController bossGateUiController;
        [SerializeField] private EndingUiController endingUiController;
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
        private bool _bossGatePresentationActive;
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
        public bool EndingRestButtonVisible => endingUiController != null && endingUiController.Visible
            ? endingUiController.RestButton != null && endingUiController.RestButton.gameObject.activeSelf
            : endingRestButton != null && endingRestButton.gameObject.activeSelf;
        public bool EndingContinueButtonVisible => endingUiController != null && endingUiController.Visible
            ? endingUiController.ContinueButton != null && endingUiController.ContinueButton.gameObject.activeSelf
            : endingContinueButton != null && endingContinueButton.gameObject.activeSelf;
        public Button EndingRestButton => endingUiController != null && endingUiController.Visible ? endingUiController.RestButton : endingRestButton;
        public Button EndingContinueButton => endingUiController != null && endingUiController.Visible ? endingUiController.ContinueButton : endingContinueButton;
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
        public bool ShopUiVisible => shopUiController != null && shopUiController.Visible;
        public string ShopSceneSpriteName => shopUiController == null ? string.Empty : shopUiController.SceneSpriteName;
        public string ShopGoldText => shopUiController == null ? string.Empty : shopUiController.GoldText;
        public string ShopOfferText => shopUiController == null ? string.Empty : shopUiController.OfferText;
        public string ShopOfferDetailText => shopUiController == null ? string.Empty : shopUiController.OfferDetailText;
        public string ShopUnavailableReasonText => shopUiController == null ? string.Empty : shopUiController.UnavailableReasonText;
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
        public bool EventCutsceneVisible => (eventUiController != null && eventUiController.Visible) ||
            (eventCutscenePanel != null && eventCutscenePanel.gameObject.activeInHierarchy);
        public string EventCutsceneMessage => eventUiController != null && eventUiController.Visible
            ? (eventUiController.TitleText + "\n" + eventUiController.BodyText).Trim()
            : ((eventHeaderText == null ? string.Empty : eventHeaderText.text) + "\n" + (eventBodyText == null ? string.Empty : eventBodyText.text)).Trim();
        public bool EventUiVisible => eventUiController != null && eventUiController.Visible;
        public int EventChoiceCount => eventUiController == null ? 0 : eventUiController.ChoiceCount;
        public string EventScrollHint => eventUiController == null ? string.Empty : eventUiController.ScrollHint;
        public bool BossGateUiVisible => bossGateUiController != null && bossGateUiController.Visible;
        public string BossGateReadinessText => bossGateUiController == null ? string.Empty : bossGateUiController.ReadinessText;
        public bool EndingUiVisible => endingUiController != null && endingUiController.Visible;
        public string EndingRunSummaryText => endingUiController == null ? string.Empty : endingUiController.RunSummaryText;
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

            if (EndingRestButton != null && EndingRestButton.gameObject.activeSelf && EndingRestButton.interactable &&
                keyboard.digit1Key.wasPressedThisFrame)
            {
                ResolveEndingRest();
                return;
            }

            if (EndingContinueButton != null && EndingContinueButton.gameObject.activeSelf && EndingContinueButton.interactable &&
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
            if (!showRawDebugText && IsBossGateEncounter(encounter))
            {
                ShowBossGateChoices(encounter, choiceViews, onChoiceSelected);
                return;
            }

            if (!showRawDebugText && IsEventCutsceneEncounter(encounter))
            {
                ShowEventChoices(encounter, choiceViews, onChoiceSelected);
                return;
            }

            if (!showRawDebugText && encounter != null && encounter.Type == EncounterType.Shop)
            {
                ShowShopChoices(encounter, choiceViews, onChoiceSelected);
                return;
            }

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
            HideShopUi();
            HideEventUi();
            HideBossGateUi();
            HideEndingUi();
            for (var i = 0; i < _choiceButtons.Count; i++)
            {
                if (_choiceButtons[i] != null &&
                    (shopUiController == null || !shopUiController.Owns(_choiceButtons[i])) &&
                    (eventUiController == null || !eventUiController.Owns(_choiceButtons[i])) &&
                    (bossGateUiController == null || !bossGateUiController.Owns(_choiceButtons[i])))
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
            _bossGatePresentationActive = false;
            HideMerchantPresentation();
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
            UpdateEndingButtons(snapshot);
            UpdateScreenLayers(snapshot);
            UpdateBossRewardPopup(snapshot);
            UpdateLevelRewardPopup(snapshot);
            UpdateNextFloorButton(snapshot);
            UpdateRouteActionButton(snapshot);
            UpdateRestartButton(snapshot);
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
            UpdateRunStateTextVisibility();
        }

        private void UpdateScreenLayers(PrototypeRunSnapshot snapshot)
        {
            EnsureScreenLayers();
            var restVisible = RestInteractionPanelVisible;
            var mapVisible = IsMapRouteState(snapshot);
            var shopVisible = !snapshot.IsInCombat && !restVisible && _shopPresentationActive;
            var eventVisible = _eventPresentationActive;
            var bossGateVisible = _bossGatePresentationActive && bossGateUiController != null && bossGateUiController.Visible;
            SetLayerVisible(topStatusLayer, !HasDedicatedPresentationUi());
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, !snapshot.IsInCombat && !eventVisible && !mapVisible && !bossGateVisible);
            SetLayerVisible(nodeMapLayer, mapVisible && floorMapUiController == null);
            if (floorMapUiController != null && floorMapUiController.Root != null)
            {
                SetLayerVisible(floorMapUiController.Root, mapVisible);
            }
            SetLayerVisible(npcReactionLayer, showRawDebugText && !snapshot.IsInCombat && !snapshot.EndingChoicePending && !eventVisible && !mapVisible && !shopVisible && !restVisible && !bossGateVisible);
            SetLayerVisible(actionLayer, !snapshot.IsInCombat && !snapshot.RunCompleted && !mapVisible && !bossGateVisible);
            SetLayerVisible(resultLayer, !snapshot.IsInCombat && !eventVisible && !mapVisible && !shopVisible && !restVisible && !bossGateVisible && resultText != null && resultText.gameObject.activeSelf);
            SetLayerVisible(endingLayer, snapshot.EndingChoicePending && !snapshot.IsInCombat && (showRawDebugText || !EndingUiVisible));
        }

        private void UpdateMemoryAndCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureMemoryText();
            UpdateCombatPanel(snapshot);
            if (memoryText == null)
            {
                return;
            }

            if ((_eventPresentationActive || _shopPresentationActive || RestInteractionPanelVisible || IsMapRouteState(snapshot) || HasDedicatedPresentationUi()) && !showRawDebugText)
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

        private void UpdateRunStateTextVisibility()
        {
            var visible = !HasDedicatedPresentationUi();
            if (runStateText != null)
            {
                runStateText.gameObject.SetActive(visible);
            }

            if (focusText != null)
            {
                focusText.gameObject.SetActive(visible);
            }
        }
    }
}
