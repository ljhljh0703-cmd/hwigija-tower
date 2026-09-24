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
    }
}
