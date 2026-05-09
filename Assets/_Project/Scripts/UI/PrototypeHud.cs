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

        private readonly List<Button> _choiceButtons = new List<Button>();
        private readonly List<string> _demoRouteLabels = new List<string>();
        private readonly List<string> _demoRouteEncounterIds = new List<string>();
        private PrototypeRoomController _roomController;
        private string _lastMemoryCutsceneKey = string.Empty;
        private string _lastCombatCutsceneKey = string.Empty;
        private string _lastDemoCompleteCutsceneKey = string.Empty;
        private bool _cutsceneFinishedSubscribed;

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
        public bool RawDebugTextVisible => showRawDebugText;
        public bool HasPresentationData => presentationData != null;

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
                _demoRouteLabels.Add("Run clear");
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

        public void ShowChoices(EncounterData encounter, PrototypeEncounterChoiceView[] choiceViews, Action<string> onChoiceSelected)
        {
            ClearChoices();
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
                interactionText.text = showRawDebugText
                    ? $"node: {ResolveEncounterDisplayName(encounter)} | encounter: {encounter.Id} | choices pending"
                    : ResolvePresentationDisplayName(encounter);
            }

            ShowResultMessage(string.Empty);
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
                var status = snapshot.EndingRest ? " | Ending Rest" :
                    snapshot.EndingContinue ? " | Ending Continue" :
                    snapshot.RunClear ? " | Run Clear" :
                    snapshot.RunFailed ? " | Run Failed" :
                    string.Empty;
                runStateText.text =
                    $"Floor {snapshot.CurrentFloor}{status} | HP {snapshot.PlayerHp}/{snapshot.PlayerMaxHp} | Mental {snapshot.Mental} | Gold {snapshot.Gold}\n" +
                    $"Glitch {snapshot.GlitchLevel} | Affinity {snapshot.Affinity} | Ability {snapshot.AbilityCount} | Item {snapshot.ItemCount}";
            }

            if (snapshot.RunCompleted)
            {
                ClearChoices();
            }

            EnsureEventSystem();
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

        private Button CreateChoiceButton(PrototypeEncounterChoiceView view, Action<string> onChoiceSelected)
        {
            var buttonObject = new GameObject($"Choice Button {view.ChoiceStableId}");
            buttonObject.transform.SetParent(choiceContainer, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 96f);
            rect.anchoredPosition = new Vector2(0f, -_choiceButtons.Count * 108f);

            var image = buttonObject.AddComponent<Image>();
            image.color = view.Enabled
                ? new Color(0.16f, 0.19f, 0.23f, 0.96f)
                : new Color(0.09f, 0.10f, 0.12f, 0.78f);

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
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 16;
            label.resizeTextMaxSize = 24;
            label.supportRichText = false;
            label.lineSpacing = 0.92f;
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

        private void NormalizeLayout()
        {
            ApplyTextRect(focusText, new Vector2(0.06f, 1f), new Vector2(0.94f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -24f), new Vector2(0f, 44f), 26, TextAnchor.UpperCenter);
            ApplyTextRect(interactionText, new Vector2(0.06f, 1f), new Vector2(0.94f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -72f), new Vector2(0f, 48f), 22, TextAnchor.UpperCenter);
            ApplyTextRect(runStateText, new Vector2(0.06f, 1f), new Vector2(0.94f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -126f), new Vector2(0f, 168f), 20, TextAnchor.UpperCenter);
            ApplyTextRect(resultText, new Vector2(0.08f, 0f), new Vector2(0.92f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 438f), new Vector2(0f, 132f), 22, TextAnchor.MiddleCenter);
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
            text.resizeTextMinSize = 13;
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
        }

        private void EnsureChoiceContainer()
        {
            if (choiceContainer != null)
            {
                return;
            }

            var containerObject = new GameObject("Choice Buttons");
            containerObject.transform.SetParent(transform, false);

            choiceContainer = containerObject.AddComponent<RectTransform>();
            choiceContainer.anchorMin = new Vector2(0.08f, 0f);
            choiceContainer.anchorMax = new Vector2(0.92f, 0f);
            choiceContainer.pivot = new Vector2(0.5f, 0f);
            choiceContainer.sizeDelta = new Vector2(0f, 324f);
            choiceContainer.anchoredPosition = new Vector2(0f, 72f);
        }

        private void EnsureResultText()
        {
            if (resultText != null)
            {
                return;
            }

            var resultObject = new GameObject("Encounter Result Text");
            resultObject.transform.SetParent(transform, false);

            var rect = resultObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.08f, 0f);
            rect.anchorMax = new Vector2(0.92f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, 86f);
            rect.anchoredPosition = new Vector2(0f, 420f);

            resultText = resultObject.AddComponent<Text>();
            resultText.font = ResolveFont();
            resultText.fontSize = 22;
            resultText.alignment = TextAnchor.MiddleCenter;
            resultText.horizontalOverflow = HorizontalWrapMode.Wrap;
            resultText.verticalOverflow = VerticalWrapMode.Truncate;
            resultText.resizeTextForBestFit = true;
            resultText.resizeTextMinSize = 14;
            resultText.resizeTextMaxSize = 22;
            resultText.supportRichText = false;
            resultText.color = new Color(0.72f, 0.78f, 0.82f, 1f);
            NormalizeLayout();
        }

        private void EnsureRouteText()
        {
            if (routeText != null)
            {
                return;
            }

            routeText = CreateHudText("Demo Route Text", new Vector2(0.06f, 1f), new Vector2(0.94f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -300f), new Vector2(0f, 82f), 20, TextAnchor.UpperCenter, new Color(0.70f, 0.76f, 0.82f, 1f));
        }

        private void EnsureMemoryText()
        {
            if (memoryText != null)
            {
                return;
            }

            memoryText = CreateHudText("Memory Combat Text", new Vector2(0.08f, 0f), new Vector2(0.92f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 584f), new Vector2(0f, 112f), 20, TextAnchor.MiddleCenter, new Color(0.66f, 0.73f, 0.78f, 1f));
        }

        private void EnsureDemoCompleteText()
        {
            if (demoCompleteText != null)
            {
                return;
            }

            demoCompleteText = CreateHudText("Demo Complete Text", new Vector2(0.12f, 0.5f), new Vector2(0.88f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0f, 120f), 32, TextAnchor.MiddleCenter, new Color(0.88f, 0.92f, 0.78f, 1f));
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
            portraitObject.transform.SetParent(transform, false);

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
            backgroundObject.transform.SetParent(transform, false);
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

        private void EnsureCombatPanel()
        {
            if (combatPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Combat Panel");
            panelObject.transform.SetParent(transform, false);

            combatPanel = panelObject.AddComponent<RectTransform>();
            combatPanel.anchorMin = new Vector2(0.34f, 0f);
            combatPanel.anchorMax = new Vector2(0.94f, 0f);
            combatPanel.pivot = new Vector2(0.5f, 0f);
            combatPanel.anchoredPosition = new Vector2(0f, 728f);
            combatPanel.sizeDelta = new Vector2(0f, 310f);

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.08f, 0.10f, 0.12f, 0.88f);

            combatEnemyImage = CreateCombatImage(panelObject.transform, "Combat Enemy Image", new Vector2(0.02f, 0.36f), new Vector2(0.30f, 0.92f));
            combatText = CreateHudText("Combat Status Text", new Vector2(0.33f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(-24f, 122f), 20, TextAnchor.UpperLeft, new Color(0.78f, 0.84f, 0.86f, 1f));
            combatText.transform.SetParent(panelObject.transform, false);
            var combatTextRect = combatText.GetComponent<RectTransform>();
            combatTextRect.anchorMin = new Vector2(0.33f, 1f);
            combatTextRect.anchorMax = new Vector2(1f, 1f);
            combatTextRect.pivot = new Vector2(0.5f, 1f);
            combatTextRect.anchoredPosition = new Vector2(0f, -12f);
            combatTextRect.sizeDelta = new Vector2(-24f, 122f);

            enemyHpFill = CreateHpBar(panelObject.transform, "Enemy HP Bar", new Vector2(0.34f, 0.53f), new Vector2(0.94f, 0.60f), new Color(0.76f, 0.22f, 0.22f, 1f));
            playerHpFill = CreateHpBar(panelObject.transform, "Player HP Bar", new Vector2(0.34f, 0.40f), new Vector2(0.94f, 0.47f), new Color(0.28f, 0.68f, 0.46f, 1f));

            attackButton = CreateCombatButton(panelObject.transform, "Combat Button Attack", "공격", new Vector2(0.17f, 0f), CombatAction.Attack);
            defendButton = CreateCombatButton(panelObject.transform, "Combat Button Defend", "방어", new Vector2(0.50f, 0f), CombatAction.Defend);
            skillButton = CreateCombatButton(panelObject.transform, "Combat Button Skill", "기술 없음", new Vector2(0.83f, 0f), CombatAction.Skill);
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
            rect.anchoredPosition = new Vector2(0f, 18f);
            rect.sizeDelta = new Vector2(150f, 64f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.22f, 0.26f, 0.96f);

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
            label.fontSize = 22;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 14;
            label.resizeTextMaxSize = 22;
            label.color = new Color(0.88f, 0.92f, 0.94f, 1f);
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
            buttonObject.transform.SetParent(transform, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0f);
            rect.anchorMax = new Vector2(0.72f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 218f);
            rect.sizeDelta = new Vector2(0f, 76f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.20f, 0.24f, 0.28f, 0.96f);

            nextFloorButton = buttonObject.AddComponent<Button>();
            nextFloorButton.targetGraphic = image;
            nextFloorButton.onClick.AddListener(ResolveNextFloor);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(12f, 4f);
            labelRect.offsetMax = new Vector2(-12f, -4f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 14;
            label.resizeTextMaxSize = 24;
            label.color = new Color(0.88f, 0.92f, 0.94f, 1f);
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
            buttonObject.transform.SetParent(transform, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0f);
            rect.anchorMax = new Vector2(0.72f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 218f);
            rect.sizeDelta = new Vector2(0f, 76f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.23f, 0.27f, 0.96f);

            routeActionButton = buttonObject.AddComponent<Button>();
            routeActionButton.targetGraphic = image;
            routeActionButton.onClick.AddListener(OpenCurrentRouteStep);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(12f, 4f);
            labelRect.offsetMax = new Vector2(-12f, -4f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 14;
            label.resizeTextMaxSize = 24;
            label.raycastTarget = false;
            label.color = new Color(0.88f, 0.92f, 0.94f, 1f);
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
                _choiceButtons.Count == 0;
            SetButtonLabel(routeActionButton, showRawDebugText ? "Open route step" : "진행");
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
            buttonObject.transform.SetParent(transform, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.28f, 0f);
            rect.anchorMax = new Vector2(0.72f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 218f);
            rect.sizeDelta = new Vector2(0f, 76f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.20f, 0.24f, 0.28f, 0.96f);

            restartButton = buttonObject.AddComponent<Button>();
            restartButton.targetGraphic = image;
            restartButton.onClick.AddListener(ResolveRestart);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(12f, 4f);
            labelRect.offsetMax = new Vector2(-12f, -4f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 14;
            label.resizeTextMaxSize = 24;
            label.color = new Color(0.88f, 0.92f, 0.94f, 1f);
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
                endingRestButton = CreateEndingButton("Ending Button Rest", showRawDebugText ? "PLACEHOLDER_ENDING_REST" : "안식", new Vector2(-170f, 218f), ResolveEndingRest);
            }

            if (endingContinueButton == null)
            {
                endingContinueButton = CreateEndingButton("Ending Button Continue", showRawDebugText ? "PLACEHOLDER_ENDING_CONTINUE" : "동행 계속", new Vector2(170f, 218f), ResolveEndingContinue);
            }
        }

        private Button CreateEndingButton(string name, string labelText, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(transform, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(300f, 76f);

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.22f, 0.26f, 0.96f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10f, 4f);
            labelRect.offsetMax = new Vector2(-10f, -4f);

            var label = labelObject.AddComponent<Text>();
            label.font = ResolveFont();
            label.fontSize = 18;
            label.alignment = TextAnchor.MiddleCenter;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 10;
            label.resizeTextMaxSize = 18;
            label.color = new Color(0.88f, 0.92f, 0.94f, 1f);
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

            var selection = _roomController.SelectCurrentRouteEncounter();
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
            textObject.transform.SetParent(transform, false);

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
            text.resizeTextMinSize = 13;
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
                return "진행\nFloor " + snapshot.CurrentFloor + " | 전투 중";
            }

            if (snapshot.RunClear)
            {
                if (snapshot.EndingRest)
                {
                    return "진행\nending.rest";
                }

                if (snapshot.EndingContinue)
                {
                    return "진행\nending.continue";
                }

                return "진행\nrun.clear | 엔딩 선택";
            }

            if (snapshot.RunFailed)
            {
                return "진행\nrun.failed";
            }

            if (snapshot.StairUnlocked)
            {
                return "진행\nFloor " + snapshot.CurrentFloor + " clear | 다음 층 가능";
            }

            if (snapshot.BossGateUnlocked)
            {
                return "진행\nFloor " + snapshot.CurrentFloor + " | Boss Gate";
            }

            var label = currentIndex >= 0 && currentIndex < _demoRouteLabels.Count
                ? _demoRouteLabels[currentIndex]
                : ResolvePublicDemoStatus(snapshot);
            var step = Mathf.Clamp(currentIndex + 1, 1, Mathf.Max(1, _demoRouteLabels.Count));
            var remaining = Mathf.Max(0, _demoRouteLabels.Count - step);
            return "진행\nFloor " + snapshot.CurrentFloor + " | " + label + " | 남은 단계 " + remaining;
        }

        private static string ResolvePublicDemoStatus(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                return "전투 중";
            }

            return snapshot.EndingRest ? "ending.rest" :
                snapshot.EndingContinue ? "ending.continue" :
                snapshot.RunClear ? "run.clear" :
                snapshot.RunFailed ? "run.failed" :
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
                ? "NPC: -"
                : "NPC: " + ResolvePublicNpcReaction(snapshot.LastNpcReactionKey);
            memoryText.text = memory + "\n" + combat + "\n" + npc;
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
                skillButton.interactable = canAct && snapshot.AbilityCount > 0;
                SetButtonLabel(skillButton, snapshot.AbilityCount > 0 ? "기술" : "기술 없음");
            }
        }

        private void UpdateCombatVisuals(PrototypeRunSnapshot snapshot)
        {
            var slot = ResolveCurrentPresentationSlot(snapshot);
            if (combatEnemyImage != null)
            {
                combatEnemyImage.sprite = slot == null ? null : slot.EnemySprite;
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
                demoCompleteText.text = showRawDebugText ? "run.failed\nRestart Ready" : "Run failed\nRestart available";
                return;
            }

            if (snapshot.EndingRest)
            {
                demoCompleteText.text = showRawDebugText ? "ending.rest" : "ending.rest";
                return;
            }

            if (snapshot.EndingContinue)
            {
                demoCompleteText.text = showRawDebugText ? "ending.continue\nRestart Ready" : "ending.continue\nRestart available";
                return;
            }

            demoCompleteText.text = showRawDebugText ? "run.clear\nChoose Ending" : "run.clear\n엔딩 선택";
        }

        private void UpdateResultVisibility(PrototypeRunSnapshot snapshot)
        {
            if (resultText == null)
            {
                return;
            }

            resultText.gameObject.SetActive(!snapshot.IsInCombat);
        }

        private string BuildChoiceLabel(PrototypeEncounterChoiceView view, int index)
        {
            var label = showRawDebugText
                ? (string.IsNullOrEmpty(view.TextKey) ? view.ChoiceStableId : view.TextKey)
                : ResolvePublicChoiceLabel(view.ChoiceStableId, index);
            if (!view.Enabled)
            {
                label += showRawDebugText && !string.IsNullOrEmpty(view.ReasonTextKey)
                    ? "\n" + view.ReasonTextKey
                    : "\n" + (string.IsNullOrEmpty(view.HintText) ? "Unavailable" : view.HintText);
            }
            else if (!showRawDebugText && !string.IsNullOrEmpty(view.HintText))
            {
                label += "\n" + view.HintText;
            }

            return label;
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

            var summary = "결과";
            AppendResultLine(ref summary, ExtractHpChange(message));
            AppendEffectTokens(ref summary, message);

            if (message.Contains("combat started"))
            {
                AppendResultLine(ref summary, "Combat start");
            }

            if (message.Contains("enemyDefeated True", StringComparison.Ordinal) || message.Contains("victory", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "Victory");
            }
            else if (message.Contains("defeat", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "Defeat");
            }

            if (message.Contains("enemyDefeated True", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "Enemy defeated");
            }

            if (message.Contains("already resolved:", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "Already resolved");
            }

            if (message.Contains("stair unlocked", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "Next floor ready");
            }

            AppendFloorProgressionLine(ref summary, message);

            if (message.Contains("run.clear", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "Final boss defeated");
                AppendResultLine(ref summary, "Ending choice ready");
            }

            if (message.Contains("ending.rest", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "ending.rest");
            }

            if (message.Contains("ending.continue", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "ending.continue");
            }

            if (message.Contains("run.failed", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "Run failed");
            }

            if (message.Contains("run.restartReady", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "Restart available");
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
                token.StartsWith("Glitch ", StringComparison.Ordinal) ||
                token.StartsWith("Affinity ", StringComparison.Ordinal))
            {
                return IsNoOpDelta(token.Substring(token.IndexOf(' ') + 1)) ? string.Empty : token;
            }

            if (token.StartsWith("gold reward ", StringComparison.Ordinal))
            {
                return "Gold +" + token.Substring("gold reward ".Length).Trim().TrimStart('+');
            }

            if (token.StartsWith("glitch ", StringComparison.Ordinal))
            {
                return "Glitch " + token.Substring("glitch ".Length).Trim();
            }

            if (token.StartsWith("affinity ", StringComparison.Ordinal))
            {
                return "Affinity " + token.Substring("affinity ".Length).Trim();
            }

            if (token.StartsWith("playerDamage ", StringComparison.Ordinal))
            {
                return "Damage " + token.Substring("playerDamage ".Length).Trim();
            }

            if (token.StartsWith("enemyDamage ", StringComparison.Ordinal))
            {
                return "Enemy damage " + token.Substring("enemyDamage ".Length).Trim();
            }

            if (token.StartsWith("combo ", StringComparison.Ordinal))
            {
                return "Combo " + token.Substring("combo ".Length).Trim();
            }

            if (token.StartsWith("item ", StringComparison.Ordinal))
            {
                return NormalizeRefDelta(token.Substring("item ".Length).Trim());
            }

            if (token.StartsWith("ability ", StringComparison.Ordinal))
            {
                return token.Substring("ability ".Length).Trim() + " acquired";
            }

            if (token.StartsWith("reward ", StringComparison.Ordinal))
            {
                return "Reward " + token.Substring("reward ".Length).Trim();
            }

            if (token.StartsWith("memory unlocked ", StringComparison.Ordinal))
            {
                return token.Substring("memory unlocked ".Length).Trim() + " unlocked";
            }

            if (token.StartsWith("action ", StringComparison.Ordinal))
            {
                return "Action " + token.Substring("action ".Length).Trim();
            }

            return string.Empty;
        }

        private static string NormalizeRefDelta(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value;
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
                AppendResultLine(ref summary, "Floor " + message.Substring(start, end - start).Trim() + " entered");
            }
        }

        private static void AppendResultLine(ref string summary, string line)
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

            summary += "\n" + line;
        }

        private static string BuildCombatOutcomeLabel(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                return "전투 중";
            }

            if (snapshot.LastCombatEnemyDefeated)
            {
                return "victory";
            }

            return string.IsNullOrEmpty(snapshot.LastCombatResultId) ? "-" : snapshot.LastCombatResultId;
        }

        private static string BuildCombatPresentation(PrototypeRunSnapshot snapshot)
        {
            var text =
                "전투\n" +
                "Enemy HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + "\n" +
                "Player HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + "\n" +
                "Round " + snapshot.CombatRound + "\n" +
                BuildCombatFeedback(snapshot.LastCombatRoundResult) + "\n" +
                (snapshot.AbilityCount > 0 ? "Skill ready" : "Skill unavailable: Ability 필요");
            if (snapshot.LastCombatComboDamage > 0)
            {
                text += "\nCombo " + snapshot.LastCombatComboDamage;
            }

            return text;
        }

        private static string BuildCombatFeedback(string roundResult)
        {
            if (string.IsNullOrEmpty(roundResult))
            {
                return "Ready";
            }

            if (roundResult.Contains("ready", StringComparison.OrdinalIgnoreCase))
            {
                return "Ready";
            }

            if (roundResult.Contains("Defend", StringComparison.Ordinal))
            {
                return "방어: 피해 감소" + ExtractRoundNumber(roundResult, "enemyDamage ");
            }

            if (roundResult.Contains("Attack", StringComparison.Ordinal))
            {
                return "공격: 피해" + ExtractRoundNumber(roundResult, "playerDamage ");
            }

            if (roundResult.Contains("Skill", StringComparison.Ordinal))
            {
                return "기술: 추가 공격" + ExtractRoundNumber(roundResult, "playerDamage ");
            }

            return "Round resolved";
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
            if (interactionText != null && !showRawDebugText && snapshot.RunClear && !snapshot.IsInCombat)
            {
                interactionText.text = "Run clear";
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
                encounterId = ResolveDemoCompleteCutsceneEncounterId(snapshot);
            }

            return presentationData.TryGetSlot(encounterId, out var slot) ? slot : null;
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
            playerObject.transform.SetParent(transform, false);
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
                "ENC_COMBAT_GATE_02" => "Boss Gate",
                "ENC_COMBAT_GATE_03" => "Final Boss",
                _ => encounter.Type == EncounterType.MoralChoice ? "선택" :
                    encounter.Type == EncounterType.MemoryFragment ? "기억 파편" :
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
                return "shop.response";
            }

            if (reactionKey.Contains("MORAL", StringComparison.OrdinalIgnoreCase))
            {
                return "choice.response";
            }

            if (reactionKey.Contains("MEMORY", StringComparison.OrdinalIgnoreCase))
            {
                return "memory.response";
            }

            if (reactionKey.Contains("COMBAT", StringComparison.OrdinalIgnoreCase) || reactionKey.Contains("BATTLE", StringComparison.OrdinalIgnoreCase))
            {
                return "combat.response";
            }

            if (reactionKey.Contains("FLOOR", StringComparison.OrdinalIgnoreCase) || reactionKey.Contains("STAIR", StringComparison.OrdinalIgnoreCase))
            {
                return "floor.response";
            }

            if (reactionKey.Contains("RECALL", StringComparison.OrdinalIgnoreCase))
            {
                return "recall.response";
            }

            return "fallback.response";
        }

        private static bool LooksLikeInternalLabel(string value)
        {
            return value == "MoralChoice" ||
                   value == "MemoryFragment" ||
                   value == "CombatGate" ||
                   value == "DemoComplete" ||
                   value == "Shop" ||
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
