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
        private void ShowBossGateChoices(EncounterData encounter, PrototypeEncounterChoiceView[] choiceViews, Action<string> onChoiceSelected)
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
            var sceneSprite = encounterBackgroundImage == null ? null : encounterBackgroundImage.sprite;
            HideMerchantPresentation();
            _shopPresentationActive = false;
            _eventPresentationActive = false;
            _bossGatePresentationActive = true;
            EnsureBossGateUi(onChoiceSelected);
            if (bossGateUiController == null)
            {
                return;
            }

            var engage = default(PrototypeEncounterChoiceView);
            var source = choiceViews ?? Array.Empty<PrototypeEncounterChoiceView>();
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i].Visible && ChoiceStartsCombat(encounter, source[i].ChoiceStableId))
                {
                    engage = source[i];
                    break;
                }
            }

            var snapshot = _roomController == null ? _lastSnapshot : _roomController.GetSnapshot();
            bossGateUiController.Show(snapshot, sceneSprite, engage.ChoiceStableId, engage.Enabled);
            _choiceButtons.Add(bossGateUiController.EngageButton);
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

        private void EnsureBossGateUi(Action<string> onChoiceSelected)
        {
            if (bossGateUiController == null)
            {
                var moduleObject = new GameObject("Boss Gate UI Module", typeof(RectTransform));
                moduleObject.transform.SetParent(HudParent, false);
                bossGateUiController = moduleObject.AddComponent<BossGateUiController>();
            }

            bossGateUiController.Initialize(HudParent, choiceStableId =>
            {
                ClearChoices();
                onChoiceSelected?.Invoke(choiceStableId);
            }, LeaveBossGate);
        }

        private void HideBossGateUi()
        {
            if (bossGateUiController != null)
            {
                bossGateUiController.Hide();
            }
        }

        private void LeaveBossGate()
        {
            ClearChoices();
            if (_roomController == null)
            {
                return;
            }

            _roomController.CancelCurrentRouteSelection();
            ShowResultMessage(string.Empty);
            ShowRunState(_roomController.GetSnapshot());
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

        private static bool IsBossGateEncounter(EncounterData encounter)
        {
            return encounter != null &&
                encounter.Id == "ENC_COMBAT_GATE_02";
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
    }
}
