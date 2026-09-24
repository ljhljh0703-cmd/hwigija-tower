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

        private static string ExtractChoiceId(string buttonName)
        {
            return string.IsNullOrEmpty(buttonName) ? string.Empty : buttonName.Replace("Choice Button ", string.Empty, StringComparison.Ordinal);
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
    }
}
