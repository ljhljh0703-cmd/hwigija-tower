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
    public sealed partial class PrototypeHud
    {
        private void ShowEventChoices(EncounterData encounter, PrototypeEncounterChoiceView[] choiceViews, Action<string> onChoiceSelected)
        {
            ClearChoices();
            HideRestInteractionPanel();
            EnsureScreenLayers();
            HideEventCutsceneLayout();
            HideUtilityPanel();
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            SetResultVisible(false);
            EnsureEventSystem();
            ApplyPresentationSlot(encounter.Id);
            HideMerchantPresentation();
            _shopPresentationActive = false;
            _eventPresentationActive = true;
            EnsureEventUi(onChoiceSelected);
            if (eventUiController == null)
            {
                return;
            }

            var presentations = new List<EventChoicePresentation>();
            var source = choiceViews ?? Array.Empty<PrototypeEncounterChoiceView>();
            for (var i = 0; i < source.Length; i++)
            {
                var view = source[i];
                if (!view.Visible)
                {
                    continue;
                }

                presentations.Add(new EventChoicePresentation(view.ChoiceStableId, BuildChoiceLabel(view, i), view.Enabled));
            }

            var snapshot = _roomController == null ? _lastSnapshot : _roomController.GetSnapshot();
            eventUiController.Show(
                snapshot,
                ResolveEventHeader(encounter),
                ResolveEventBody(encounter, ResolvePresentationSlot(encounter.Id)),
                presentations);
            for (var i = 0; i < eventUiController.ChoiceCount; i++)
            {
                _choiceButtons.Add(eventUiController.GetChoiceButton(i));
            }

            if (routeText != null && !showRawDebugText)
            {
                routeText.gameObject.SetActive(false);
            }

            if (interactionText != null)
            {
                interactionText.text = string.Empty;
                interactionText.gameObject.SetActive(false);
            }

            ShowResultMessage(string.Empty);
        }

        private void EnsureEventUi(Action<string> onChoiceSelected)
        {
            if (eventUiController == null)
            {
                var moduleObject = new GameObject("Event UI Module", typeof(RectTransform));
                moduleObject.transform.SetParent(HudParent, false);
                eventUiController = moduleObject.AddComponent<EventUiController>();
            }

            eventUiController.Initialize(HudParent, choiceStableId =>
            {
                ClearChoices();
                HideEventCutsceneLayout();
                onChoiceSelected?.Invoke(choiceStableId);
            }, LeaveCurrentEvent);
        }

        private void HideEventUi()
        {
            if (eventUiController != null)
            {
                eventUiController.Hide();
            }
        }

        private void LeaveCurrentEvent()
        {
            ClearChoices();
            HideEventCutsceneLayout();
            if (_roomController == null)
            {
                return;
            }

            _roomController.CancelCurrentRouteSelection();
            ShowResultMessage(string.Empty);
            ShowRunState(_roomController.GetSnapshot());
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
            HideEventUi();
            if (eventCutscenePanel != null)
            {
                eventCutscenePanel.gameObject.SetActive(false);
            }

            if (restoreRouteText && routeText != null)
            {
                routeText.gameObject.SetActive(true);
            }
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
    }
}
