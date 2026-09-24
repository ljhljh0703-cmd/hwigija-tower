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
        private void ShowShopChoices(EncounterData encounter, PrototypeEncounterChoiceView[] choiceViews, Action<string> onChoiceSelected)
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
            _eventPresentationActive = false;
            _shopPresentationActive = true;
            EnsureShopUi(onChoiceSelected);
            if (shopUiController == null)
            {
                return;
            }

            var offers = new List<ShopOfferPresentation>();
            var leaveChoiceStableId = string.Empty;
            var source = choiceViews ?? Array.Empty<PrototypeEncounterChoiceView>();
            for (var i = 0; i < source.Length; i++)
            {
                var view = source[i];
                if (IsPurchaseChoice(view.ChoiceStableId))
                {
                    offers.Add(BuildShopOfferPresentation(view));
                }
                else if (view.ChoiceStableId.Contains("_LEAVE", StringComparison.Ordinal))
                {
                    leaveChoiceStableId = view.ChoiceStableId;
                }
            }

            var snapshot = _roomController == null ? _lastSnapshot : _roomController.GetSnapshot();
            shopUiController.Show(snapshot, ResolvePresentationDisplayName(encounter), sceneSprite, offers.ToArray(), leaveChoiceStableId);
            for (var i = 0; i < shopUiController.OfferButtons.Count; i++)
            {
                _choiceButtons.Add(shopUiController.OfferButtons[i]);
            }
            _choiceButtons.Add(shopUiController.LeaveButton);

            if (interactionText != null)
            {
                interactionText.text = string.Empty;
                interactionText.gameObject.SetActive(false);
            }

            ShowResultMessage(string.Empty);
        }

        private void EnsureShopUi(Action<string> onChoiceSelected)
        {
            if (shopUiController == null)
            {
                var moduleObject = new GameObject("Shop UI Module", typeof(RectTransform));
                moduleObject.transform.SetParent(HudParent, false);
                shopUiController = moduleObject.AddComponent<ShopUiController>();
            }

            shopUiController.Initialize(HudParent, choiceStableId =>
            {
                ClearChoices();
                HideEventCutsceneLayout();
                onChoiceSelected?.Invoke(choiceStableId);
            });
        }

        private void HideShopUi()
        {
            if (shopUiController != null)
            {
                shopUiController.Hide();
            }
        }

        private static ShopOfferPresentation BuildShopOfferPresentation(PrototypeEncounterChoiceView view)
        {
            var title = ResolvePurchaseChoiceTitle(view.HintText);
            var detail = FirstPublicLine(NormalizePublicHint(view.HintText));
            var unavailableReason = view.Enabled ? string.Empty : NormalizeShopDisabledHint(view.HintText);
            return new ShopOfferPresentation(view.ChoiceStableId, title, detail, unavailableReason, view.Enabled);
        }

        private static string FirstPublicLine(string value)
        {
            var lineBreak = value.IndexOf('\n');
            return lineBreak < 0 ? value : value.Substring(0, lineBreak);
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

        private static bool IsShopPurchaseChoice(EncounterData encounter, string choiceStableId)
        {
            return encounter != null &&
                encounter.Type == EncounterType.Shop &&
                !string.IsNullOrEmpty(choiceStableId) &&
                choiceStableId.Contains("_BUY_", StringComparison.Ordinal);
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
    }
}
