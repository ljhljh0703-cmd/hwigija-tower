using System;
using System.Collections.Generic;
using HwigiTower.Lobby;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public readonly struct ShopOfferPresentation
    {
        public ShopOfferPresentation(string choiceStableId, string title, string detail, string unavailableReason, bool enabled)
        {
            ChoiceStableId = choiceStableId ?? string.Empty;
            Title = title ?? string.Empty;
            Detail = detail ?? string.Empty;
            UnavailableReason = unavailableReason ?? string.Empty;
            Enabled = enabled;
        }

        public string ChoiceStableId { get; }
        public string Title { get; }
        public string Detail { get; }
        public string UnavailableReason { get; }
        public bool Enabled { get; }
        public bool HasChoice => !string.IsNullOrEmpty(ChoiceStableId);
    }

    public sealed class ShopUiController : MonoBehaviour
    {
        private const int OfferCount = 5;
        private readonly Button[] _offerButtons = new Button[OfferCount];
        private readonly Text[] _offerTitles = new Text[OfferCount];
        private readonly Text[] _offerDetails = new Text[OfferCount];
        private readonly Text[] _offerReasons = new Text[OfferCount];
        private readonly ShopOfferPresentation[] _offers = new ShopOfferPresentation[OfferCount];

        private RectTransform _root;
        private Text _floorText;
        private Text _sanityText;
        private Text _hpText;
        private Text _goldText;
        private Image _sanityFill;
        private Image _hpFill;
        private Text _titleText;
        private Image _sceneImage;
        private Text _ownedStripText;
        private Button _leaveButton;
        private Action<string> _onChoiceSelected;
        private string _leaveChoiceStableId = string.Empty;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public IReadOnlyList<Button> OfferButtons => _offerButtons;
        public Button LeaveButton => _leaveButton;
        public string GoldText => _goldText == null ? string.Empty : _goldText.text;
        public string SceneSpriteName => _sceneImage == null || _sceneImage.sprite == null ? string.Empty : _sceneImage.sprite.name;
        public string OfferText => JoinText(_offerTitles);
        public string OfferDetailText => JoinText(_offerDetails);
        public string UnavailableReasonText => JoinText(_offerReasons);

        public void Initialize(Transform parent, Action<string> onChoiceSelected)
        {
            _onChoiceSelected = onChoiceSelected;
            if (_root != null)
            {
                return;
            }

            transform.SetParent(parent, false);
            _root = transform as RectTransform;
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
            var backdrop = gameObject.AddComponent<Image>();
            backdrop.color = ShopUiTokens.VoidBg;
            backdrop.raycastTarget = false;

            BuildResourceStrip();
            _titleText = CreateTextSlot("Shop Screen Title", ShopLayout.ScreenTitle, ShopUiTokens.Ink);
            _titleText.fontStyle = FontStyle.Bold;
            _sceneImage = CreateImageSlot("Shop Scene", ShopLayout.ShopScene, ShopUiTokens.PanelBg);
            _sceneImage.preserveAspect = false;
            _ownedStripText = CreateTextSlot("Shop Owned Strip", ShopLayout.OwnedStrip, ShopUiTokens.InkDim);
            _ownedStripText.alignment = TextAnchor.MiddleLeft;

            CreateOfferRow(0, "Shop Offer Row 1", ShopLayout.OfferRow1);
            CreateOfferRow(1, "Shop Offer Row 2", ShopLayout.OfferRow2);
            CreateOfferRow(2, "Shop Offer Row 3", ShopLayout.OfferRow3);
            CreateOfferRow(3, "Shop Offer Row 4", ShopLayout.OfferRow4);
            CreateOfferRow(4, "Shop Offer Row 5", ShopLayout.OfferRow5);
            _leaveButton = CreateSlotButton("Shop Leave Button", ShopLayout.LeaveButton, ShopUiTokens.FrameHi);
            _leaveButton.onClick.AddListener(SelectLeave);
            Hide();
        }

        public void Show(PrototypeRunSnapshot snapshot, string title, Sprite sceneSprite, ShopOfferPresentation[] offers, string leaveChoiceStableId)
        {
            _root.gameObject.SetActive(true);
            _floorText.text = "층 " + snapshot.CurrentFloor;
            _sanityText.text = "이성 " + snapshot.Mental;
            _hpText.text = "체력 " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp;
            _goldText.text = "골드 " + snapshot.Gold;
            _sanityFill.fillAmount = Mathf.Clamp01((snapshot.Mental + 100f) / 200f);
            _hpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            _titleText.text = title;
            _sceneImage.sprite = sceneSprite;
            _sceneImage.color = sceneSprite == null ? ShopUiTokens.PanelBg : UiColorTokens.WithAlpha(UiColorTokens.NoTint, UiTokenContract.SceneTintAlpha);
            _sceneImage.gameObject.SetActive(true);
            _ownedStripText.text = "보유 수 " + snapshot.ItemCount;

            for (var i = 0; i < OfferCount; i++)
            {
                var offer = offers != null && i < offers.Length ? offers[i] : default;
                _offers[i] = offer;
                ApplyOffer(i, offer);
            }

            _leaveChoiceStableId = leaveChoiceStableId ?? string.Empty;
            _leaveButton.gameObject.SetActive(true);
            _leaveButton.interactable = !string.IsNullOrEmpty(_leaveChoiceStableId);
        }

        public bool Owns(Button button)
        {
            if (button == null)
            {
                return false;
            }

            if (button == _leaveButton)
            {
                return true;
            }

            for (var i = 0; i < _offerButtons.Length; i++)
            {
                if (button == _offerButtons[i])
                {
                    return true;
                }
            }

            return false;
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        private void BuildResourceStrip()
        {
            _floorText = CreateChip(ShopLayout.FloorChip, ShopUiTokens.GoldCoin);
            (_sanityText, _sanityFill) = CreateGauge(ShopLayout.SanityChip, ShopUiTokens.Sanity);
            (_hpText, _hpFill) = CreateGauge(ShopLayout.HpChip, ShopUiTokens.Hp);
            _goldText = CreateGauge(ShopLayout.GoldChip, ShopUiTokens.GoldCoin).text;
        }

        private void CreateOfferRow(int index, string name, ShopSlot slot)
        {
            var rowObject = new GameObject(name, typeof(RectTransform));
            rowObject.transform.SetParent(_root, false);
            var rect = rowObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = rowObject.AddComponent<Image>();
            image.color = ShopUiTokens.PanelBgAlt;
            var outline = rowObject.AddComponent<Outline>();
            outline.effectColor = ShopUiTokens.Frame;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            var button = rowObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() => SelectOffer(index));
            _offerButtons[index] = button;
            _offerTitles[index] = CreateChildText(rect, "Title", string.Empty, UiTokenContract.LabelFontSize, ShopUiTokens.Ink, new Vector2(0.04f, 0.48f), new Vector2(0.96f, 0.94f), TextAnchor.MiddleLeft);
            _offerDetails[index] = CreateChildText(rect, "Detail", string.Empty, UiTokenContract.MicroFontSize, ShopUiTokens.InkDim, new Vector2(0.04f, 0.10f), new Vector2(0.96f, 0.50f), TextAnchor.MiddleLeft);
            _offerReasons[index] = CreateChildText(rect, "Unavailable Reason", string.Empty, UiTokenContract.MicroFontSize, ShopUiTokens.Gold, new Vector2(0.55f, 0.10f), new Vector2(0.96f, 0.40f), TextAnchor.MiddleRight);
            _offerReasons[index].gameObject.SetActive(false);
        }

        private void ApplyOffer(int index, ShopOfferPresentation offer)
        {
            var button = _offerButtons[index];
            button.gameObject.SetActive(offer.HasChoice);
            button.interactable = offer.HasChoice && offer.Enabled;
            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = offer.Enabled ? ShopUiTokens.PanelBgAlt : ShopUiTokens.PanelBg;
            }

            _offerTitles[index].text = offer.Title;
            _offerTitles[index].color = offer.Enabled ? ShopUiTokens.Ink : ShopUiTokens.InkDim;
            _offerDetails[index].text = offer.Detail;
            _offerDetails[index].color = offer.Enabled ? ShopUiTokens.InkDim : ShopUiTokens.InkMute;
            _offerReasons[index].text = offer.UnavailableReason;
            _offerReasons[index].gameObject.SetActive(!offer.Enabled && !string.IsNullOrEmpty(offer.UnavailableReason));
        }

        private void SelectOffer(int index)
        {
            if (index >= 0 && index < _offers.Length && _offers[index].Enabled && !string.IsNullOrEmpty(_offers[index].ChoiceStableId))
            {
                _onChoiceSelected?.Invoke(_offers[index].ChoiceStableId);
            }
        }

        private void SelectLeave()
        {
            if (!string.IsNullOrEmpty(_leaveChoiceStableId))
            {
                _onChoiceSelected?.Invoke(_leaveChoiceStableId);
            }
        }

        private Text CreateChip(ShopSlot slot, Color accent)
        {
            var panel = CreatePanelSlot("Shop " + slot.Key, slot, ShopUiTokens.PanelBg, true);
            var accentImage = CreateImage(panel, "Accent", new Vector2(0.03f, 0.18f), new Vector2(0.06f, 0.82f), accent);
            accentImage.raycastTarget = false;
            return CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, ShopUiTokens.Ink, new Vector2(0.10f, 0.10f), new Vector2(0.94f, 0.90f), TextAnchor.MiddleCenter);
        }

        private (Text text, Image fill) CreateGauge(ShopSlot slot, Color fillColor)
        {
            var panel = CreatePanelSlot("Shop " + slot.Key, slot, ShopUiTokens.PanelBg, true);
            var text = CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, ShopUiTokens.Ink, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.92f), TextAnchor.MiddleCenter);
            return (text, CreateGaugeFill(panel, "Fill", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.32f), fillColor));
        }

        private Button CreateSlotButton(string name, ShopSlot slot, Color frameColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = ShopUiTokens.PanelBgAlt;
            AddFrame(buttonObject, frameColor);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rect, "Label", slot.Content, slot.FontSize, ShopUiTokens.Ink, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, ShopSlot slot, Color color, bool framed)
        {
            var panelObject = new GameObject(name, typeof(RectTransform));
            panelObject.transform.SetParent(_root, false);
            var rect = panelObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            if (framed)
            {
                AddFrame(panelObject, ShopUiTokens.Frame);
            }

            return rect;
        }

        private Image CreateImageSlot(string name, ShopSlot slot, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(_root, false);
            var rect = imageObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateTextSlot(string name, ShopSlot slot, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(_root, false);
            var rect = textObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var text = textObject.AddComponent<Text>();
            text.font = ShopUiTokens.ResolveRuntimeFont();
            text.fontSize = slot.FontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = slot.Content;
            text.raycastTarget = false;
            return text;
        }

        private static Text CreateChildText(RectTransform parent, string name, string content, int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax, TextAnchor alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = textObject.AddComponent<Text>();
            text.font = ShopUiTokens.ResolveRuntimeFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = content;
            text.raycastTarget = false;
            return text;
        }

        private static Image CreateImage(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(parent, false);
            var rect = imageObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Image CreateGaugeFill(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var frame = CreateImage(parent, name + " Frame", anchorMin, anchorMax, ShopUiTokens.VoidBg);
            var fill = CreateImage(frame.rectTransform, name, new Vector2(0.02f, 0.18f), new Vector2(0.98f, 0.82f), color);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private static void ApplySlot(RectTransform rect, ShopSlot slot)
        {
            rect.anchorMin = ShopLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = ShopLayoutRuntime.AnchorMax(slot);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void AddFrame(GameObject target, Color color)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
        }

        private static float Ratio(int current, int maximum) => maximum <= 0 ? 0f : Mathf.Clamp01((float)current / maximum);

        private static string JoinText(Text[] values)
        {
            var result = new string[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                result[i] = values[i] == null ? string.Empty : values[i].text;
            }

            return string.Join("|", result);
        }
    }
}
