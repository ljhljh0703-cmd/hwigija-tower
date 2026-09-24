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
                IsBossGateEncounter(selection.Encounter) ||
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

        private static bool IsCombatEncounterId(string encounterId)
        {
            return encounterId == "ENC_COMBAT_GATE_01" || encounterId == "ENC_COMBAT_GATE_02" || encounterId == "ENC_COMBAT_GATE_03";
        }
    }
}
