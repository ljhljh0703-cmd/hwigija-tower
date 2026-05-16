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
        [SerializeField] private Image merchantVisualImage;
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
        [SerializeField] private Button restSubmitButton;
        [SerializeField] private Button restContinueButton;
        [SerializeField] private RectTransform topStatusLayer;
        [SerializeField] private RectTransform objectiveLayer;
        [SerializeField] private RectTransform visualLayer;
        [SerializeField] private RectTransform nodeMapLayer;
        [SerializeField] private RectTransform npcReactionLayer;
        [SerializeField] private RectTransform actionLayer;
        [SerializeField] private RectTransform resultLayer;
        [SerializeField] private RectTransform endingLayer;
        [SerializeField] private RectTransform portraitRoot;

        private readonly List<Button> _choiceButtons = new List<Button>();
        private readonly List<Image> _mapNodeIconImages = new List<Image>();
        private readonly List<string> _demoRouteLabels = new List<string>();
        private readonly List<string> _demoRouteEncounterIds = new List<string>();
        private PrototypeRoomController _roomController;
        private EncounterSelection _pendingRestSelection;
        private string _pendingRestActionId = string.Empty;
        private string _lastMemoryCutsceneKey = string.Empty;
        private string _lastCombatCutsceneKey = string.Empty;
        private string _lastDemoCompleteCutsceneKey = string.Empty;
        private bool _cutsceneFinishedSubscribed;
        private bool _shopPresentationActive;

        private const float ChoiceButtonHeight = 118f;
        private const float ChoiceButtonSpacing = 130f;
        private const float MapNodeButtonHeight = 118f;
        private const float MapNodeButtonSpacing = 132f;
        private const float MapNodeIconSize = 72f;
        private const int TitleFontSize = 34;
        private const int SubtitleFontSize = 30;
        private const int BodyFontSize = 28;
        private const int ButtonFontSize = 30;
        private const int ResultFontSize = 28;
        private const int StatFontSize = 26;
        private const int CaptionFontSize = 23;
        private const int CombatBodyFontSize = 31;
        private const int ChoiceFontSize = ButtonFontSize;
        private const int MapNodeFontSize = 29;
        private const int RestBodyFontSize = 30;
        private const int RestInputFontSize = 34;
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
        public string CurrentPortraitSpriteName => npcPortraitImage != null && npcPortraitImage.sprite != null ? npcPortraitImage.sprite.name : string.Empty;
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
        }

        public void SetRawDebugTextVisible(bool visible)
        {
            showRawDebugText = visible;
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
            SetLayerVisible(actionLayer, true);
            SetLayerVisible(nodeMapLayer, false);
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
                if (encounter.Type == EncounterType.Shop)
                {
                    _shopPresentationActive = true;
                    ApplyMerchantPresentation(_roomController == null ? 1 : _roomController.GetSnapshot().CurrentFloor);
                }
                else
                {
                    _shopPresentationActive = false;
                    HideMerchantPresentation();
                }

                interactionText.text = showRawDebugText
                    ? $"node: {ResolveEncounterDisplayName(encounter)} | encounter: {encounter.Id} | choices pending"
                    : ResolvePresentationDisplayName(encounter);
            }

            ShowResultMessage(string.Empty);
            if (!showRawDebugText && encounter != null && encounter.Type == EncounterType.Shop && resultText != null)
            {
                var gold = _roomController == null ? 0 : _roomController.GetSnapshot().Gold;
                resultText.text = "상점\n보스 전 준비 | 현재 Gold " + gold + "\n가격과 효과를 보고 구매하세요";
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
            _shopPresentationActive = false;
            HideMerchantPresentation();
        }

        public void ShowMapChoices(PrototypeFloorMapNodeView[] nodes, Action<string> onNodeSelected)
        {
            ClearChoices();
            HideRestInteractionPanel();
            EnsureScreenLayers();
            SetLayerVisible(nodeMapLayer, true);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(resultLayer, true);
            EnsureEventSystem();
            EnsureChoiceContainer();
            if (choiceContainer == null || nodes == null)
            {
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var button = CreateMapNodeButton(node, onNodeSelected);
                _choiceButtons.Add(button);
            }

            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "map node selection" : "지도";
            }

            ShowResultMessage(showRawDebugText ? "select map node" : "선택 가능한 길이 밝게 표시됩니다");
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
            UpdateMemoryAndCombatPanel(snapshot);
            UpdateResultVisibility(snapshot);
            UpdateDemoCompletePanel(snapshot);
            UpdateCutsceneTriggers(snapshot);
        }

        private void UpdateScreenLayers(PrototypeRunSnapshot snapshot)
        {
            EnsureScreenLayers();
            SetLayerVisible(topStatusLayer, true);
            SetLayerVisible(objectiveLayer, true);
            SetLayerVisible(visualLayer, !snapshot.IsInCombat);
            SetLayerVisible(nodeMapLayer, snapshot.HasFloorMap && !snapshot.IsInCombat && !snapshot.RunCompleted && _choiceButtons.Count > 0);
            SetLayerVisible(npcReactionLayer, !snapshot.IsInCombat && !snapshot.EndingChoicePending);
            SetLayerVisible(actionLayer, !snapshot.IsInCombat && !snapshot.RunCompleted);
            SetLayerVisible(resultLayer, !snapshot.IsInCombat && !RestInteractionPanelVisible && resultText != null && resultText.gameObject.activeSelf);
            SetLayerVisible(endingLayer, snapshot.EndingChoicePending && !snapshot.IsInCombat);
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

            var selectableMapNodes = _roomController.GetSelectableMapNodes();
            if (selectableMapNodes.Length == 0)
            {
                return;
            }

            ShowMapChoices(selectableMapNodes, mapNodeId =>
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

            var stableId = view.ChoiceStableId;
            button.onClick.AddListener(() =>
            {
                ClearChoices();
                onChoiceSelected?.Invoke(stableId);
            });

            return button;
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
                var column = index % 2;
                var row = index / 2;
                rect.anchorMin = new Vector2(column == 0 ? 0f : 0.52f, 1f);
                rect.anchorMax = new Vector2(column == 0 ? 0.48f : 1f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(0f, MapNodeButtonHeight);
                rect.anchoredPosition = new Vector2(0f, -row * MapNodeButtonSpacing);
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
                labelRect.offsetMin = new Vector2(100f, 8f);
                labelRect.offsetMax = new Vector2(-14f, -8f);
                label.fontSize = node.Selectable ? MapNodeFontSize : 26;
                label.resizeTextMinSize = 21;
                label.resizeTextMaxSize = node.Selectable ? MapNodeFontSize : 26;
                label.alignment = TextAnchor.MiddleLeft;
                label.color = node.Selectable
                    ? new Color(0.96f, 0.99f, 1f, 1f)
                    : node.Completed
                        ? new Color(0.74f, 0.84f, 0.78f, 0.92f)
                        : new Color(0.50f, 0.54f, 0.58f, 0.82f);
            }

            var iconObject = new GameObject("Node Icon");
            iconObject.transform.SetParent(button.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(16f, 0f);
            iconRect.sizeDelta = new Vector2(MapNodeIconSize, MapNodeIconSize);

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

            return button;
        }

        private Sprite ResolveNodeIcon(PrototypeFloorMapNodeType type)
        {
            return presentationData != null && presentationData.TryGetNodeIcon(type, out var icon) ? icon : null;
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
            nodeMapLayer = EnsureLayerPanel(nodeMapLayer, "Screen Layer Node Map", new Vector2(0.06f, 0.045f), new Vector2(0.94f, 0.305f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.030f, 0.040f, 0.050f, 0.90f), false);
            actionLayer = EnsureLayerPanel(actionLayer, "Screen Layer Action", new Vector2(0.06f, 0.045f), new Vector2(0.94f, 0.265f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.02f, 0.025f, 0.03f, 0.50f), false);
            endingLayer = EnsureLayerPanel(endingLayer, "Screen Layer Ending", new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.30f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.06f, 0.055f, 0.04f, 0.90f), false);
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

        private void ShowRestInteraction(EncounterSelection selection)
        {
            ClearChoices();
            EnsureRestInteractionPanel();
            EnsureScreenLayers();
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(npcReactionLayer, true);
            SetLayerVisible(resultLayer, true);
            ApplyPresentationSlot(selection.EncounterId);
            _pendingRestSelection = selection;
            _pendingRestActionId = string.Empty;
            if (restInputField != null)
            {
                restInputField.text = string.Empty;
                restInputField.interactable = true;
            }

            if (restResponseText != null)
            {
                restResponseText.text = "행동을 선택하세요";
            }

            SetRestActionButtonsInteractable(true);
            if (restSubmitButton != null)
            {
                restSubmitButton.gameObject.SetActive(true);
                restSubmitButton.interactable = false;
            }

            if (restContinueButton != null)
            {
                restContinueButton.gameObject.SetActive(false);
            }

            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "rest interaction: " + selection.EncounterId : "휴식";
            }

            if (restInteractionPanel != null)
            {
                restInteractionPanel.gameObject.SetActive(true);
            }

            ShowResultMessage(string.Empty);
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

            restAskMoodButton = CreateRestActionButton(panelObject.transform, "Rest Button Ask Mood", "기분\n신뢰 +2", new Vector2(0.17f, 0.81f), "rest.ask_mood");
            restTrainButton = CreateRestActionButton(panelObject.transform, "Rest Button Train", "훈련\n피해 +1", new Vector2(0.50f, 0.81f), "rest.train");
            restRecoverButton = CreateRestActionButton(panelObject.transform, "Rest Button Recover", "휴식\nHP 회복", new Vector2(0.83f, 0.81f), "rest.recover");

            restInputField = CreateRestInputField(panelObject.transform);
            restSubmitButton = CreateRestButton(panelObject.transform, "Rest Submit Button", "전달", new Vector2(0.16f, 0.13f), new Vector2(0.46f, 0.26f));
            restSubmitButton.onClick.AddListener(SubmitRestInteraction);
            restContinueButton = CreateRestButton(panelObject.transform, "Rest Continue Button", "계속", new Vector2(0.54f, 0.13f), new Vector2(0.84f, 0.26f));
            restContinueButton.onClick.AddListener(ContinueAfterRestInteraction);
            restContinueButton.gameObject.SetActive(false);

            var responsePanel = CreatePanel("Rest Response Bubble", panelObject.transform, new Vector2(0.06f, 0.30f), new Vector2(0.94f, 0.50f), new Color(0.10f, 0.13f, 0.15f, 0.96f));
            restResponseText = CreateHudText("Rest Response Text", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(20f, 10f), new Vector2(-20f, -10f), RestBodyFontSize, TextAnchor.MiddleLeft, new Color(0.88f, 0.93f, 0.92f, 1f));
            restResponseText.horizontalOverflow = HorizontalWrapMode.Wrap;
            restResponseText.transform.SetParent(panelObject.transform, false);
            restResponseText.transform.SetParent(responsePanel, false);
            restResponseText.text = "행동을 선택하세요";
            restInteractionPanel.gameObject.SetActive(false);
        }

        private Button CreateRestActionButton(Transform parent, string name, string label, Vector2 center, string actionId)
        {
            var button = CreateRestButton(parent, name, label, new Vector2(center.x - 0.15f, center.y - 0.09f), new Vector2(center.x + 0.15f, center.y + 0.09f));
            button.onClick.AddListener(() => SelectRestAction(actionId));
            return button;
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
            if (restSubmitButton != null)
            {
                restSubmitButton.interactable = true;
            }

            if (restResponseText != null)
            {
                restResponseText.text = actionId switch
                {
                    "rest.ask_mood" => "기분을 묻는다\n말을 입력하면 신뢰 +2",
                    "rest.train" => "훈련을 진행한다\n다음 전투 피해 +1",
                    "rest.recover" => "휴식을 취한다\n입력 없이 HP 회복",
                    _ => "말을 입력한 뒤 전달하세요"
                };
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
                ShowResult(resolution);
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            SetRestActionButtonsInteractable(false);
            if (restInputField != null)
            {
                restInputField.interactable = false;
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

        private void EnsureMerchantVisualImage()
        {
            if (merchantVisualImage != null)
            {
                return;
            }

            EnsureScreenLayers();
            var merchantObject = new GameObject("Merchant Visual");
            merchantObject.transform.SetParent(HudParent, false);

            var rect = merchantObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.53f, 0.50f);
            rect.anchorMax = new Vector2(0.92f, 0.82f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            merchantVisualImage = merchantObject.AddComponent<Image>();
            merchantVisualImage.color = new Color(1f, 1f, 1f, 0.98f);
            merchantVisualImage.preserveAspect = true;
            merchantVisualImage.raycastTarget = false;
            merchantVisualImage.gameObject.SetActive(false);
        }

        private void ApplyMerchantPresentation(int floor)
        {
            EnsureMerchantVisualImage();
            if (merchantVisualImage == null)
            {
                return;
            }

            merchantVisualImage.sprite = presentationData != null && presentationData.TryGetMerchantSprite(floor, out var sprite) ? sprite : mataiosPortrait;
            merchantVisualImage.gameObject.SetActive(merchantVisualImage.sprite != null);
        }

        private void HideMerchantPresentation()
        {
            if (merchantVisualImage != null)
            {
                merchantVisualImage.gameObject.SetActive(false);
            }
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
            combatPanel.anchorMin = new Vector2(0.06f, 0.045f);
            combatPanel.anchorMax = new Vector2(0.94f, 0.835f);
            combatPanel.pivot = new Vector2(0.5f, 0.5f);
            combatPanel.anchoredPosition = Vector2.zero;
            combatPanel.sizeDelta = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.05f, 0.065f, 0.08f, 0.94f);

            combatEnemyImage = CreateCombatImage(panelObject.transform, "Combat Enemy Image", new Vector2(0.08f, 0.50f), new Vector2(0.92f, 0.93f));
            combatText = CreateHudText("Combat Status Text", new Vector2(0.08f, 0.26f), new Vector2(0.92f, 0.47f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, CombatBodyFontSize, TextAnchor.UpperLeft, PrimaryTextColor);
            combatText.lineSpacing = DenseLineSpacing;
            combatText.transform.SetParent(panelObject.transform, false);
            var combatTextRect = combatText.GetComponent<RectTransform>();
            combatTextRect.anchorMin = new Vector2(0.08f, 0.26f);
            combatTextRect.anchorMax = new Vector2(0.92f, 0.47f);
            combatTextRect.pivot = new Vector2(0.5f, 0.5f);
            combatTextRect.anchoredPosition = Vector2.zero;
            combatTextRect.sizeDelta = Vector2.zero;

            enemyHpFill = CreateHpBar(panelObject.transform, "Enemy HP Bar", new Vector2(0.08f, 0.955f), new Vector2(0.92f, 0.985f), new Color(0.84f, 0.24f, 0.24f, 1f));
            playerHpFill = CreateHpBar(panelObject.transform, "Player HP Bar", new Vector2(0.08f, 0.195f), new Vector2(0.92f, 0.225f), new Color(0.30f, 0.78f, 0.50f, 1f));

            attackButton = CreateCombatButton(panelObject.transform, "Combat Button Attack", "공격", new Vector2(0.17f, 0.08f), CombatAction.Attack);
            defendButton = CreateCombatButton(panelObject.transform, "Combat Button Defend", "방어", new Vector2(0.50f, 0.08f), CombatAction.Defend);
            skillButton = CreateCombatButton(panelObject.transform, "Combat Button Skill", "기술 없음", new Vector2(0.83f, 0.08f), CombatAction.Skill);
            skillButton.interactable = false;
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

        private Button CreateCombatButton(Transform parent, string name, string labelText, Vector2 anchor, CombatAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 20f);
            rect.sizeDelta = new Vector2(250f, 96f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.13f, 0.18f, 0.22f, 0.98f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => ResolveCombatAction(action));

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(8f, 4f);
            labelRect.offsetMax = new Vector2(-8f, -4f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = ButtonFontSize;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 22;
            label.resizeTextMaxSize = ButtonFontSize;
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
                ShowMapChoices(selectableMapNodes, mapNodeId =>
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
                    ? "기억 파편: 보류"
                    : "기억 파편: 해금";
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

            return "[Player] HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp +
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
            return "[Mataios] " + affinity + "  기억 " + snapshot.MemoryFragmentCount + "  안정";
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
            combatPanel.gameObject.SetActive(hasCombat);
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
                SetButtonLabel(skillButton, scoutSkillReady ? "정찰 기술" : "기술 없음");
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

            if (enemyHpFill != null)
            {
                enemyHpFill.fillAmount = Ratio(snapshot.EnemyHp, snapshot.EnemyMaxHp);
            }

            if (playerHpFill != null)
            {
                playerHpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            }
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

            resultText.gameObject.SetActive(!snapshot.IsInCombat && !RestInteractionPanelVisible);
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
                    : "\n" + (string.IsNullOrEmpty(view.HintText) ? "선택 불가" : NormalizePublicHint(view.HintText));
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
                return "기억 파편 해금";
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
            var round = ShortenPublicLine(BuildCombatFeedback(snapshot.LastCombatRoundResult) + " | " + state, 44);
            var text =
                "상대: " + PublicEnemyName(snapshot.LastCombatEnemyId) + " | 적 HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + "\n" +
                "내 HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + " | R" + snapshot.CombatRound + "\n" +
                round;

            return text;
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
            var slot = ResolveCurrentPresentationSlot(snapshot);
            ApplyPresentationSlot(slot);
            if (!snapshot.IsInCombat && (_shopPresentationActive || IsShopEncounterId(snapshot.NextDemoEncounterId)))
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
                    return "기억 파편";
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
                "CHOICE_MEMORY_01_UNLOCK" => "기억 파편",
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
                "ENC_MEMORY_FRAGMENT_01" => "기억 파편",
                "ENC_COMBAT_GATE_01" => "전투",
                "ENC_F02_SHOP_001" => "상점",
                "ENC_F02_MORAL_CHOICE_001" => "선택",
                "ENC_COMBAT_GATE_02" => "보스 관문",
                "ENC_COMBAT_GATE_03" => "최종 보스",
                _ => encounter.Type == EncounterType.MoralChoice ? "선택" :
                    encounter.Type == EncounterType.MemoryFragment ? "기억 파편" :
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
                .Replace("Memory unlock", "기억 파편 해금", StringComparison.Ordinal)
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

            inputModule.AssignDefaultActions();
        }
    }
}
