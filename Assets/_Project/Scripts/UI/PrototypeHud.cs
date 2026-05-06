using System;
using System.Collections.Generic;
using HwigiTower.Combat;
using HwigiTower.Encounters;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private Image npcPortraitImage;
        [SerializeField] private RectTransform combatPanel;
        [SerializeField] private Text combatText;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button defendButton;
        [SerializeField] private Button skillButton;

        private readonly List<Button> _choiceButtons = new List<Button>();
        private readonly List<string> _demoRouteLabels = new List<string>();
        private PrototypeRoomController _roomController;

        public int ChoiceButtonCount => _choiceButtons.Count;
        public string ResultMessage => resultText == null ? string.Empty : resultText.text;
        public string RouteMessage => routeText == null ? string.Empty : routeText.text;
        public string MemoryMessage => memoryText == null ? string.Empty : memoryText.text;
        public bool CombatPanelVisible => combatPanel != null && combatPanel.gameObject.activeSelf;
        public bool PortraitVisible => npcPortraitImage != null && npcPortraitImage.gameObject.activeSelf;

        public void BindRoomController(PrototypeRoomController controller)
        {
            _roomController = controller;
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
                interactionText.text = "room";
            }

            ShowRunState(default);
            if (resultText != null)
            {
                resultText.text = "result: -";
            }

            ApplyPortrait();
            ClearChoices();
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
                interactionText.text = "room";
            }

            ShowRunState(default);
            if (resultText != null)
            {
                resultText.text = "result: -";
            }

            ApplyPortrait();
            ClearChoices();
        }

        public void ConfigureDemoRoute(IReadOnlyList<PrototypeDemoRunStep> demoRunPath)
        {
            _demoRouteLabels.Clear();

            if (demoRunPath != null)
            {
                for (var i = 0; i < demoRunPath.Count; i++)
                {
                    var step = demoRunPath[i];
                    if (step == null || !step.IsValid)
                    {
                        continue;
                    }

                    _demoRouteLabels.Add(BuildRouteLabel(step));
                }
            }

            if (_demoRouteLabels.Count > 0)
            {
                _demoRouteLabels.Add("DemoComplete");
            }
        }

        public Button GetChoiceButton(int index)
        {
            return index >= 0 && index < _choiceButtons.Count ? _choiceButtons[index] : null;
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
                interactionText.text = $"node: {ResolveEncounterDisplayName(encounter)} | encounter: {encounter.Id} | choices pending";
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

            focusText.text = node == null ? "node: -" : $"node: {node.DisplayName}";
        }

        public void ShowInteraction(InteractableNode node, EncounterSelection selection, PrototypeNodeResolution resolution)
        {
            if (interactionText == null || node == null)
            {
                return;
            }

            if (selection.HasEncounter)
            {
                interactionText.text = string.IsNullOrEmpty(resolution.PayloadId)
                    ? $"encounter: {selection.EncounterId}"
                    : $"encounter: {selection.EncounterId} | payload: {resolution.PayloadId}";
                ShowResult(resolution);
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

            resultText.text = string.IsNullOrEmpty(message) ? "result: -" : $"result: {message}";
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

            var state = snapshot.RunCompleted ? "complete" : "active";
            var demo = string.IsNullOrEmpty(snapshot.NextDemoEncounterId)
                ? snapshot.DemoStatus
                : $"{snapshot.DemoStatus} next {snapshot.NextDemoNodeId}/{snapshot.NextDemoEncounterId}";
            runStateText.text =
                $"run {state} | {demo}\n" +
                $"HP {snapshot.PlayerHp}/{snapshot.PlayerMaxHp} | ATK {snapshot.PlayerAttack} | Mental {snapshot.Mental}\n" +
                $"Gold {snapshot.Gold} | Glitch {snapshot.GlitchLevel} | Affinity {snapshot.Affinity}\n" +
                $"Abilities {snapshot.AbilityCount} | Step {snapshot.DemoResolvedStepCount}/{snapshot.DemoStepCount}";
            UpdateRouteIndicator(snapshot);
            UpdateMemoryAndCombatPanel(snapshot);
            UpdateDemoCompletePanel(snapshot);
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
            label.color = view.Enabled
                ? new Color(0.88f, 0.92f, 0.94f, 1f)
                : new Color(0.58f, 0.62f, 0.66f, 1f);
            label.text = BuildChoiceLabel(view);

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

            routeText = CreateHudText("Demo Route Text", new Vector2(0.06f, 1f), new Vector2(0.94f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -306f), new Vector2(0f, 190f), 20, TextAnchor.UpperCenter, new Color(0.70f, 0.76f, 0.82f, 1f));
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

            combatText = CreateHudText("Combat Status Text", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -12f), new Vector2(-24f, 154f), 20, TextAnchor.UpperLeft, new Color(0.78f, 0.84f, 0.86f, 1f));
            combatText.transform.SetParent(panelObject.transform, false);
            var combatTextRect = combatText.GetComponent<RectTransform>();
            combatTextRect.anchorMin = new Vector2(0f, 1f);
            combatTextRect.anchorMax = new Vector2(1f, 1f);
            combatTextRect.pivot = new Vector2(0.5f, 1f);
            combatTextRect.anchoredPosition = new Vector2(0f, -12f);
            combatTextRect.sizeDelta = new Vector2(-24f, 154f);

            attackButton = CreateCombatButton(panelObject.transform, "Combat Button Attack", "Attack", new Vector2(0.17f, 0f), CombatAction.Attack);
            defendButton = CreateCombatButton(panelObject.transform, "Combat Button Defend", "Defend", new Vector2(0.50f, 0f), CombatAction.Defend);
            skillButton = CreateCombatButton(panelObject.transform, "Combat Button Skill", "Skill", new Vector2(0.83f, 0f), CombatAction.Skill);
            skillButton.interactable = false;
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
            text.color = color;
            return text;
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
                routeText.text = string.IsNullOrEmpty(snapshot.NextDemoEncounterId)
                    ? "route: " + snapshot.DemoStatus
                    : "route next: " + snapshot.NextDemoNodeId + "/" + snapshot.NextDemoEncounterId;
                return;
            }

            var text = "route\n";
            var currentIndex = GetCurrentRouteIndex(snapshot);
            for (var i = 0; i < _demoRouteLabels.Count; i++)
            {
                string marker;
                if (snapshot.DemoStatus == "demo.complete" || i < currentIndex)
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
                if (i < _demoRouteLabels.Count - 1)
                {
                    text += "\n";
                }
            }

            routeText.text = text;
        }

        private int GetCurrentRouteIndex(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.DemoStatus == "demo.complete")
            {
                return _demoRouteLabels.Count;
            }

            if (string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                return Mathf.Clamp(snapshot.DemoResolvedStepCount, 0, Mathf.Max(0, _demoRouteLabels.Count - 1));
            }

            for (var i = 0; i < _demoRouteLabels.Count; i++)
            {
                if (_demoRouteLabels[i].Contains(snapshot.NextDemoEncounterId))
                {
                    return i;
                }
            }

            return Mathf.Clamp(snapshot.DemoResolvedStepCount, 0, Mathf.Max(0, _demoRouteLabels.Count - 1));
        }

        private void UpdateMemoryAndCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureMemoryText();
            UpdateCombatPanel(snapshot);
            if (memoryText == null)
            {
                return;
            }

            var memory = string.IsNullOrEmpty(snapshot.LastMemoryFragmentId)
                ? "memory: locked | count " + snapshot.MemoryFragmentCount
                : "memory: unlocked " + snapshot.LastMemoryFragmentId + " | count " + snapshot.MemoryFragmentCount +
                  "\nkeys: " + BuildMemoryKeyLine(snapshot);
            var combat = string.IsNullOrEmpty(snapshot.LastCombatId)
                ? "combat: -"
                : "combat: " + snapshot.LastCombatId + " | enemy " + snapshot.LastCombatEnemyId + " | " + snapshot.LastCombatResultId;
            memoryText.text = memory + "\n" + combat;
        }

        private void UpdateCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureCombatPanel();
            if (combatPanel == null)
            {
                return;
            }

            var hasCombat = snapshot.IsInCombat ||
                (!string.IsNullOrEmpty(snapshot.LastCombatId) && snapshot.LastCombatResultId != "started");
            combatPanel.gameObject.SetActive(hasCombat);
            if (!hasCombat || combatText == null)
            {
                return;
            }

            combatText.text =
                "combat: " + snapshot.LastCombatId + "\n" +
                "enemy: " + snapshot.LastCombatEnemyId + " | HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + "\n" +
                "player HP: " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + " | round " + snapshot.CombatRound + "\n" +
                "last: " + (string.IsNullOrEmpty(snapshot.LastCombatRoundResult) ? "-" : snapshot.LastCombatRoundResult) + "\n" +
                "result: " + snapshot.LastCombatResultId + " | gold " + snapshot.LastCombatGoldReward +
                " | glitch " + FormatDelta(snapshot.LastCombatGlitchDelta) +
                " | affinity " + FormatDelta(snapshot.LastCombatAffinityDelta) +
                (snapshot.LastCombatComboDamage > 0 ? " | combo " + snapshot.LastCombatComboDamage : string.Empty);

            var canAct = snapshot.IsInCombat && _roomController != null;
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
                skillButton.interactable = false;
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

            demoCompleteText.gameObject.SetActive(snapshot.DemoStatus == "demo.complete");
            demoCompleteText.text = "demo.complete\nDemo Route Complete";
        }

        private static string BuildChoiceLabel(PrototypeEncounterChoiceView view)
        {
            var label = string.IsNullOrEmpty(view.TextKey) ? view.ChoiceStableId : view.TextKey;
            if (!view.Enabled && !string.IsNullOrEmpty(view.ReasonTextKey))
            {
                label += "\n" + view.ReasonTextKey;
            }

            return label;
        }

        private static string BuildRouteLabel(PrototypeDemoRunStep step)
        {
            var encounter = step.Encounter;
            if (encounter == null)
            {
                return step.NodeId;
            }

            return ResolveEncounterDisplayName(encounter) + " | " + step.NodeId + "/" + encounter.Id;
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
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            var inputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
            inputModule.AssignDefaultActions();
        }
    }
}
