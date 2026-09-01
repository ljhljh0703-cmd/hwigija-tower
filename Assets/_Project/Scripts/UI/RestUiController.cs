using System;
using HwigiTower.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class RestUiController : MonoBehaviour
    {
        private RectTransform _root;
        private Text _floorText;
        private Text _sanityText;
        private Text _hpText;
        private Text _goldText;
        private Image _sanityFill;
        private Image _hpFill;
        private Image _restSceneImage;
        private Image _mataiosPortrait;
        private Text _mataiosStateLabel;
        private Text _playerStateText;
        private Image _playerHpFill;
        private RectTransform _inputOverlay;
        private RectTransform _responsePanel;
        private Text _responseText;
        private InputField _inputField;
        private Button _submitButton;
        private Button _departButton;
        private Button _askMoodButton;
        private Button _trainButton;
        private Button _recoverButton;
        private Image _askMoodIcon;
        private Image _trainIcon;
        private Image _recoverIcon;
        private int _lastGlitchLevel;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public Button AskMoodButton => _askMoodButton;
        public Button TrainButton => _trainButton;
        public Button RecoverButton => _recoverButton;
        public Image AskMoodIcon => _askMoodIcon;
        public Image TrainIcon => _trainIcon;
        public Image RecoverIcon => _recoverIcon;
        public InputField InputField => _inputField;
        public Button SubmitButton => _submitButton;
        public Button DepartButton => _departButton;
        public Text ResponseText => _responseText;
        public RectTransform ResponsePanel => _responsePanel;

        public void Initialize(Transform parent, Action<string> onActionSelected, Action onSubmit, Action onDepart)
        {
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

            BuildResourceStrip();
            BuildTitle();
            BuildScene();
            BuildPartyCards();
            BuildChoiceCards(onActionSelected);
            BuildInputOverlay(onSubmit);
            BuildResponsePanel();
            BuildDepartButton(onDepart);
            Hide();
        }

        public void ShowBase(int floor, int mental, int playerHp, int playerMaxHp, int gold, int glitchLevel, Sprite restScene, Sprite mataiosPortrait)
        {
            _root.gameObject.SetActive(true);
            _lastGlitchLevel = glitchLevel;
            _floorText.text = "층 " + floor;
            _sanityText.text = "이성 " + mental;
            _hpText.text = "체력 " + playerHp + "/" + playerMaxHp;
            _goldText.text = "골드 " + gold;
            _sanityFill.fillAmount = Mathf.Clamp01((mental + 100f) / 200f);
            _hpFill.fillAmount = playerMaxHp <= 0 ? 0f : Mathf.Clamp01((float)playerHp / playerMaxHp);
            _playerHpFill.fillAmount = _hpFill.fillAmount;
            _restSceneImage.sprite = restScene;
            _restSceneImage.gameObject.SetActive(restScene != null);
            _mataiosPortrait.sprite = mataiosPortrait;
            _mataiosPortrait.gameObject.SetActive(mataiosPortrait != null);
            _mataiosStateLabel.text = BuildMataiosStateLabel(_lastGlitchLevel);
            _playerStateText.text = "플레이어";
            SetChoiceCardsVisible(true);
            SetActionButtonsInteractable(true);
            SetInputPhaseVisible(false, false);
            SetResponseVisible(false);
            _departButton.gameObject.SetActive(false);
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        public void SetActionSprite(string actionId, Sprite sprite)
        {
            var icon = actionId == "rest.ask_mood" ? _askMoodIcon :
                actionId == "rest.train" ? _trainIcon :
                actionId == "rest.recover" ? _recoverIcon : null;
            if (icon == null)
            {
                return;
            }

            icon.sprite = sprite;
            icon.gameObject.SetActive(sprite != null);
        }

        public void SetActionSelection(string selectedActionId)
        {
            ApplyChoiceVisual(_askMoodButton, selectedActionId, "rest.ask_mood");
            ApplyChoiceVisual(_trainButton, selectedActionId, "rest.train");
            ApplyChoiceVisual(_recoverButton, selectedActionId, "rest.recover");
        }

        public void SetChoiceCardsVisible(bool visible)
        {
            SetActive(_askMoodButton, visible);
            SetActive(_trainButton, visible);
            SetActive(_recoverButton, visible);
        }

        public void SetActionButtonsInteractable(bool interactable)
        {
            SetInteractable(_askMoodButton, interactable);
            SetInteractable(_trainButton, interactable);
            SetInteractable(_recoverButton, interactable);
        }

        public void SetInputPhaseVisible(bool inputVisible, bool committed)
        {
            _inputOverlay.gameObject.SetActive(inputVisible && !committed);
            _inputField.interactable = inputVisible && !committed;
            _submitButton.gameObject.SetActive(inputVisible && !committed);
            _submitButton.interactable = inputVisible && !committed;
            _departButton.gameObject.SetActive(committed);
            _departButton.interactable = committed;
        }

        public void SetResponseVisible(bool visible)
        {
            _responsePanel.gameObject.SetActive(visible);
        }

        public void SetResponseText(string value)
        {
            _responseText.text = value ?? string.Empty;
        }

        public void ResetInput()
        {
            _inputField.text = string.Empty;
            _inputField.interactable = true;
            SetResponseText(string.Empty);
        }

        private void BuildResourceStrip()
        {
            _floorText = CreateChip(RestLayout.FloorChip, RestUiTokens.GoldCoin);
            (_sanityText, _sanityFill) = CreateGauge(RestLayout.SanityChip, RestUiTokens.Sanity);
            (_hpText, _hpFill) = CreateGauge(RestLayout.HpChip, RestUiTokens.Hp);
            _goldText = CreateChip(RestLayout.GoldChip, RestUiTokens.GoldCoin);
        }

        private void BuildTitle()
        {
            var title = CreateTextSlot("Rest Screen Title", RestLayout.ScreenTitle, RestUiTokens.Ink);
            title.fontStyle = FontStyle.Bold;
        }

        private void BuildScene()
        {
            _restSceneImage = CreateImageSlot("Rest Scene", RestLayout.RestScene, RestUiTokens.PanelBg);
            _restSceneImage.preserveAspect = false;
        }

        private void BuildPartyCards()
        {
            var playerCard = CreatePanelSlot("Rest Player State", RestLayout.PlayerState, RestUiTokens.PanelBgAlt, true);
            _playerStateText = CreateChildText(playerCard.transform, "Rest Player State Text", "플레이어", UiTokenContract.BodyFontSize, RestUiTokens.Ink, new Vector2(0.08f, 0.48f), new Vector2(0.92f, 0.88f));
            _playerHpFill = CreateGaugeSlot("Rest Player HP Bar", RestLayout.PlayerHpBar, RestUiTokens.Hp);

            var mataiosCard = CreatePanelSlot("Rest Mataios State", RestLayout.MataiosState, RestUiTokens.PanelBgAlt, true);
            _mataiosPortrait = CreateImage(mataiosCard.transform, "Rest Mataios Portrait", new Vector2(0.08f, 0.18f), new Vector2(0.38f, 0.88f), RestUiTokens.InkDim);
            _mataiosPortrait.preserveAspect = true;
            _mataiosStateLabel = CreateTextSlot("Rest Mataios State Label", RestLayout.MataiosStateLabel, RestUiTokens.InkDim);
            CreateGaugeFill(mataiosCard.transform, "Rest Mataios State Accent", new Vector2(0.44f, 0.22f), new Vector2(0.92f, 0.30f), RestUiTokens.FrameHi);
        }

        private void BuildChoiceCards(Action<string> onActionSelected)
        {
            _askMoodButton = CreateChoiceCard("Rest Choice Talk", RestLayout.ChoiceTalk, "rest.ask_mood", onActionSelected, out _askMoodIcon);
            _trainButton = CreateChoiceCard("Rest Choice Train", RestLayout.ChoiceTrain, "rest.train", onActionSelected, out _trainIcon);
            _recoverButton = CreateChoiceCard("Rest Choice Sleep", RestLayout.ChoiceSleep, "rest.recover", onActionSelected, out _recoverIcon);
        }

        private void BuildInputOverlay(Action onSubmit)
        {
            _inputOverlay = CreatePanelSlot("Rest Input Overlay", new RestSlot("input", 0.04f, 0.756f, 0.92f, 0.17f, "actions", "overlay", string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0), RestUiTokens.PanelBg, true);
            _inputField = CreateInputField(_inputOverlay.transform);
            _submitButton = CreateButton(_inputOverlay.transform, "Rest Submit Button", "전달", new Vector2(0.34f, 0.05f), new Vector2(0.66f, 0.25f), RestUiTokens.FrameHi);
            _submitButton.onClick.AddListener(() => onSubmit());
        }

        private void BuildResponsePanel()
        {
            _responsePanel = CreatePanelSlot("Rest Response Panel", new RestSlot("response", 0.04f, 0.61f, 0.92f, 0.12f, "party", "panel", string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0), RestUiTokens.PanelBg, true);
            _responseText = CreateChildText(_responsePanel.transform, "Rest Response Text", string.Empty, UiTokenContract.LabelFontSize, RestUiTokens.Ink, new Vector2(0.06f, 0.10f), new Vector2(0.94f, 0.90f));
            _responseText.alignment = TextAnchor.MiddleLeft;
        }

        private void BuildDepartButton(Action onDepart)
        {
            _departButton = CreateSlotButton("Rest Depart Button", RestLayout.DepartButton, RestUiTokens.FrameHi);
            _departButton.onClick.AddListener(() => onDepart());
        }

        private Text CreateChip(RestSlot slot, Color accent)
        {
            var panel = CreatePanelSlot("Rest " + slot.Key, slot, RestUiTokens.PanelBg, true);
            var accentImage = CreateImage(panel.transform, "Accent", new Vector2(0.03f, 0.18f), new Vector2(0.06f, 0.82f), accent);
            accentImage.raycastTarget = false;
            return CreateChildText(panel.transform, "Label", string.Empty, UiTokenContract.LabelFontSize, RestUiTokens.Ink, new Vector2(0.10f, 0.10f), new Vector2(0.94f, 0.90f));
        }

        private (Text text, Image fill) CreateGauge(RestSlot slot, Color fillColor)
        {
            var panel = CreatePanelSlot("Rest " + slot.Key, slot, RestUiTokens.PanelBg, true);
            var text = CreateChildText(panel.transform, "Label", string.Empty, UiTokenContract.LabelFontSize, RestUiTokens.Ink, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.92f));
            var fill = CreateGaugeFill(panel.transform, "Fill", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.32f), fillColor);
            return (text, fill);
        }

        private Button CreateChoiceCard(string name, RestSlot slot, string actionId, Action<string> onActionSelected, out Image icon)
        {
            var button = CreateSlotButton(name, slot, RestUiTokens.FrameHi);
            var label = button.GetComponentInChildren<Text>();
            label.text = slot.Content;
            label.fontSize = UiTokenContract.BodyFontSize;
            label.alignment = TextAnchor.MiddleCenter;
            icon = CreateImage(button.transform, "Icon", new Vector2(0.34f, 0.54f), new Vector2(0.66f, 0.88f), RestUiTokens.Gold);
            icon.preserveAspect = true;
            icon.gameObject.SetActive(false);
            button.onClick.AddListener(() => onActionSelected(actionId));
            return button;
        }

        private Button CreateSlotButton(string name, RestSlot slot, Color frameColor)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = RestUiTokens.PanelBgAlt;
            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = frameColor;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(buttonObject.transform, "Label", slot.Content, slot.FontSize, RestUiTokens.Ink, Vector2.zero, Vector2.one);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, RestSlot slot, Color color, bool framed)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(_root, false);
            var rect = panelObject.AddComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            if (framed)
            {
                var outline = panelObject.AddComponent<Outline>();
                outline.effectColor = RestUiTokens.Frame;
                outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            }

            return rect;
        }

        private Image CreateImageSlot(string name, RestSlot slot, Color color)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(_root, false);
            var rect = imageObject.AddComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Image CreateGaugeSlot(string name, RestSlot slot, Color fillColor)
        {
            var frame = CreatePanelSlot(name, slot, RestUiTokens.VoidBg, false);
            return CreateGaugeFill(frame.transform, "Fill", Vector2.zero, Vector2.one, fillColor);
        }

        private static Image CreateImage(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);
            var rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Image CreateGaugeFill(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var frame = CreateImage(parent, name + " Frame", anchorMin, anchorMax, RestUiTokens.VoidBg);
            var fill = CreateImage(frame.transform, name, new Vector2(0.02f, 0.18f), new Vector2(0.98f, 0.82f), color);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private InputField CreateInputField(Transform parent)
        {
            var inputObject = new GameObject("Rest Utterance Input");
            inputObject.transform.SetParent(parent, false);
            var rect = inputObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.06f, 0.36f);
            rect.anchorMax = new Vector2(0.94f, 0.94f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = inputObject.AddComponent<Image>();
            image.color = RestUiTokens.PanelBgAlt;
            var input = inputObject.AddComponent<InputField>();
            input.targetGraphic = image;

            var text = CreateChildText(inputObject.transform, "Text", string.Empty, UiTokenContract.BodyFontSize, RestUiTokens.Ink, new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.92f));
            text.alignment = TextAnchor.MiddleLeft;
            input.textComponent = text;
            var placeholder = CreateChildText(inputObject.transform, "Placeholder", "마타이오스에게 전할 말", UiTokenContract.LabelFontSize, RestUiTokens.InkMute, new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.92f));
            placeholder.alignment = TextAnchor.MiddleLeft;
            input.placeholder = placeholder;
            return input;
        }

        private static Button CreateButton(Transform parent, string name, string content, Vector2 anchorMin, Vector2 anchorMax, Color frameColor)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = buttonObject.AddComponent<Image>();
            image.color = RestUiTokens.PanelBgAlt;
            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = frameColor;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var text = CreateChildText(buttonObject.transform, "Label", content, UiTokenContract.LabelFontSize, RestUiTokens.Ink, Vector2.zero, Vector2.one);
            text.raycastTarget = false;
            return button;
        }

        private Text CreateTextSlot(string name, RestSlot slot, Color color)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(_root, false);
            var rect = textObject.AddComponent<RectTransform>();
            ApplySlot(rect, slot);
            return ConfigureText(textObject.AddComponent<Text>(), slot.Content, slot.FontSize, color);
        }

        private static Text CreateChildText(Transform parent, string name, string content, int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return ConfigureText(textObject.AddComponent<Text>(), content, fontSize, color);
        }

        private static Text ConfigureText(Text text, string content, int fontSize, Color color)
        {
            text.font = RestUiTokens.ResolveRuntimeFont();
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = content;
            return text;
        }

        private static void ApplySlot(RectTransform rect, RestSlot slot)
        {
            rect.anchorMin = RestLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = RestLayoutRuntime.AnchorMax(slot);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetActive(Button button, bool visible)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
            }
        }

        private static void SetInteractable(Button button, bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }

        private static void ApplyChoiceVisual(Button button, string selectedActionId, string actionId)
        {
            if (button == null)
            {
                return;
            }

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = string.IsNullOrEmpty(selectedActionId) || selectedActionId == actionId
                    ? RestUiTokens.PanelBgAlt
                    : RestUiTokens.PanelBg;
            }
        }

        private static string BuildMataiosStateLabel(int glitchLevel)
        {
            _ = glitchLevel;
            return "마타이오스의 상태를 살핀다";
        }
    }
}
