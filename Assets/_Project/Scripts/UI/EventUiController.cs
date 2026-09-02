using System;
using System.Collections.Generic;
using HwigiTower.Lobby;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public readonly struct EventChoicePresentation
    {
        public EventChoicePresentation(string choiceStableId, string label, bool enabled)
        {
            ChoiceStableId = choiceStableId ?? string.Empty;
            Label = label ?? string.Empty;
            Enabled = enabled;
        }

        public string ChoiceStableId { get; }
        public string Label { get; }
        public bool Enabled { get; }
    }

    public sealed class EventUiController : MonoBehaviour
    {
        private const float ChoiceRowHeight = UiTokenContract.TapTargetMinPx + 14f;
        private const float ChoiceRowGap = 8f;

        private readonly List<Button> _choiceButtons = new List<Button>();
        private readonly List<Text> _choiceLabels = new List<Text>();
        private RectTransform _root;
        private Text _floorText;
        private Text _sanityText;
        private Text _hpText;
        private Text _goldText;
        private Image _sanityFill;
        private Image _hpFill;
        private Text _titleText;
        private Text _bodyText;
        private RectTransform _choiceViewport;
        private RectTransform _choiceContent;
        private ScrollRect _choiceScroll;
        private Text _scrollHintText;
        private Button _leaveButton;
        private Action<string> _onChoiceSelected;
        private Action _onLeave;
        private int _choiceCount;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public int ChoiceCount => _choiceCount;
        public Button LeaveButton => _leaveButton;
        public string TitleText => _titleText == null ? string.Empty : _titleText.text;
        public string BodyText => _bodyText == null ? string.Empty : _bodyText.text;
        public string ScrollHint => _scrollHintText == null ? string.Empty : _scrollHintText.text;
        public Button GetChoiceButton(int index) => index < 0 || index >= _choiceCount ? null : _choiceButtons[index];

        public void Initialize(Transform parent, Action<string> onChoiceSelected, Action onLeave)
        {
            _onChoiceSelected = onChoiceSelected;
            _onLeave = onLeave;
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
            backdrop.color = UiColorTokens.WithAlpha(EventUiTokens.VoidBg, UiTokenContract.ScrimAlpha);
            backdrop.raycastTarget = false;

            BuildResourceStrip();
            _titleText = CreateTextSlot("Event Title", EventLayout.EventTitle, EventUiTokens.Ink);
            _titleText.fontStyle = FontStyle.Bold;
            _bodyText = CreateTextSlot("Event Body", EventLayout.EventBody, EventUiTokens.InkDim);
            _bodyText.alignment = TextAnchor.UpperLeft;
            _choiceViewport = CreateChoiceViewport();
            _scrollHintText = CreateTextSlot("Event Scroll Hint", EventLayout.ScrollHint, EventUiTokens.InkDim);
            _scrollHintText.gameObject.SetActive(false);
            _leaveButton = CreateSlotButton("Event Leave Button", EventLayout.LeaveButton, EventUiTokens.FrameHi);
            _leaveButton.onClick.AddListener(() => _onLeave?.Invoke());
            Hide();
        }

        public void Show(PrototypeRunSnapshot snapshot, string title, string body, IReadOnlyList<EventChoicePresentation> choices)
        {
            _root.gameObject.SetActive(true);
            _floorText.text = "층 " + snapshot.CurrentFloor;
            _sanityText.text = "이성 " + snapshot.Mental;
            _hpText.text = "체력 " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp;
            _goldText.text = "골드 " + snapshot.Gold;
            _sanityFill.fillAmount = Mathf.Clamp01((snapshot.Mental + 100f) / 200f);
            _hpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            _titleText.text = title;
            _bodyText.text = body;
            RebuildChoices(choices);
            _leaveButton.gameObject.SetActive(true);
            _leaveButton.interactable = true;
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

            for (var i = 0; i < _choiceButtons.Count; i++)
            {
                if (button == _choiceButtons[i])
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
            _floorText = CreateChip(EventLayout.FloorChip, EventUiTokens.GoldCoin);
            (_sanityText, _sanityFill) = CreateGauge(EventLayout.SanityChip, EventUiTokens.Sanity);
            (_hpText, _hpFill) = CreateGauge(EventLayout.HpChip, EventUiTokens.Hp);
            _goldText = CreateChip(EventLayout.GoldChip, EventUiTokens.GoldCoin);
        }

        private RectTransform CreateChoiceViewport()
        {
            var viewportObject = new GameObject("Event Choice List", typeof(RectTransform));
            viewportObject.transform.SetParent(_root, false);
            var rect = viewportObject.GetComponent<RectTransform>();
            ApplySlot(rect, EventLayout.ChoiceList);
            var image = viewportObject.AddComponent<Image>();
            image.color = EventUiTokens.PanelBg;
            var outline = viewportObject.AddComponent<Outline>();
            outline.effectColor = EventUiTokens.Frame;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            viewportObject.AddComponent<RectMask2D>();

            var contentObject = new GameObject("Event Choice List Content", typeof(RectTransform));
            contentObject.transform.SetParent(rect, false);
            _choiceContent = contentObject.GetComponent<RectTransform>();
            _choiceContent.anchorMin = new Vector2(0f, 1f);
            _choiceContent.anchorMax = new Vector2(1f, 1f);
            _choiceContent.pivot = new Vector2(0.5f, 1f);
            _choiceContent.anchoredPosition = Vector2.zero;
            _choiceContent.sizeDelta = Vector2.zero;

            _choiceScroll = viewportObject.AddComponent<ScrollRect>();
            _choiceScroll.viewport = rect;
            _choiceScroll.content = _choiceContent;
            _choiceScroll.horizontal = false;
            _choiceScroll.vertical = true;
            _choiceScroll.movementType = ScrollRect.MovementType.Clamped;
            _choiceScroll.scrollSensitivity = 26f;
            return rect;
        }

        private void RebuildChoices(IReadOnlyList<EventChoicePresentation> choices)
        {
            _choiceCount = choices == null ? 0 : choices.Count;
            for (var i = 0; i < _choiceCount; i++)
            {
                EnsureChoiceRow(i);
                var presentation = choices[i];
                var button = _choiceButtons[i];
                button.gameObject.SetActive(true);
                button.name = "Choice Button " + presentation.ChoiceStableId;
                button.interactable = presentation.Enabled;
                button.onClick.RemoveAllListeners();
                var choiceStableId = presentation.ChoiceStableId;
                button.onClick.AddListener(() => _onChoiceSelected?.Invoke(choiceStableId));
                var image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.color = presentation.Enabled ? EventUiTokens.PanelBgAlt : EventUiTokens.PanelBg;
                }

                var rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.03f, 1f);
                rect.anchorMax = new Vector2(0.97f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(0f, ChoiceRowHeight);
                rect.anchoredPosition = new Vector2(0f, -8f - i * (ChoiceRowHeight + ChoiceRowGap));
                _choiceLabels[i].text = presentation.Label;
                _choiceLabels[i].color = presentation.Enabled ? EventUiTokens.Ink : EventUiTokens.InkMute;
            }

            for (var i = _choiceCount; i < _choiceButtons.Count; i++)
            {
                _choiceButtons[i].gameObject.SetActive(false);
            }

            var viewportHeight = _choiceViewport.rect.height > 1f
                ? _choiceViewport.rect.height
                : EventLayout.ReferenceHeight * EventLayout.ChoiceList.Height;
            var contentHeight = Mathf.Max(viewportHeight, 16f + _choiceCount * (ChoiceRowHeight + ChoiceRowGap));
            _choiceContent.sizeDelta = new Vector2(0f, contentHeight);
            _choiceScroll.verticalNormalizedPosition = 1f;
            var needsScroll = contentHeight > viewportHeight + 1f;
            _scrollHintText.text = needsScroll ? "↓" : string.Empty;
            _scrollHintText.gameObject.SetActive(needsScroll);
        }

        private void EnsureChoiceRow(int index)
        {
            if (index < _choiceButtons.Count)
            {
                return;
            }

            var rowObject = new GameObject("Choice Button", typeof(RectTransform));
            rowObject.transform.SetParent(_choiceContent, false);
            var image = rowObject.AddComponent<Image>();
            image.color = EventUiTokens.PanelBgAlt;
            var outline = rowObject.AddComponent<Outline>();
            outline.effectColor = EventUiTokens.Frame;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            var button = rowObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rowObject.GetComponent<RectTransform>(), "Label", string.Empty, UiTokenContract.LabelFontSize, EventUiTokens.Ink, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.92f), TextAnchor.MiddleLeft);
            _choiceButtons.Add(button);
            _choiceLabels.Add(label);
        }

        private Text CreateChip(EventSlot slot, Color accent)
        {
            var panel = CreatePanelSlot("Event " + slot.Key, slot, EventUiTokens.PanelBg, true);
            var accentImage = CreateImage(panel, "Accent", new Vector2(0.03f, 0.18f), new Vector2(0.06f, 0.82f), accent);
            accentImage.raycastTarget = false;
            return CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, EventUiTokens.Ink, new Vector2(0.10f, 0.10f), new Vector2(0.94f, 0.90f), TextAnchor.MiddleCenter);
        }

        private (Text text, Image fill) CreateGauge(EventSlot slot, Color fillColor)
        {
            var panel = CreatePanelSlot("Event " + slot.Key, slot, EventUiTokens.PanelBg, true);
            var text = CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, EventUiTokens.Ink, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.92f), TextAnchor.MiddleCenter);
            return (text, CreateGaugeFill(panel, "Fill", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.32f), fillColor));
        }

        private Button CreateSlotButton(string name, EventSlot slot, Color frameColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = EventUiTokens.PanelBgAlt;
            AddFrame(buttonObject, frameColor);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rect, "Label", slot.Content, slot.FontSize, EventUiTokens.Ink, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, EventSlot slot, Color color, bool framed)
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
                AddFrame(panelObject, EventUiTokens.Frame);
            }

            return rect;
        }

        private Text CreateTextSlot(string name, EventSlot slot, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(_root, false);
            var rect = textObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var text = textObject.AddComponent<Text>();
            text.font = EventUiTokens.ResolveRuntimeFont();
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
            text.font = EventUiTokens.ResolveRuntimeFont();
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
            var frame = CreateImage(parent, name + " Frame", anchorMin, anchorMax, EventUiTokens.VoidBg);
            var fill = CreateImage(frame.rectTransform, name, new Vector2(0.02f, 0.18f), new Vector2(0.98f, 0.82f), color);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private static void ApplySlot(RectTransform rect, EventSlot slot)
        {
            rect.anchorMin = EventLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = EventLayoutRuntime.AnchorMax(slot);
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
    }
}
