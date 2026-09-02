using System;
using HwigiTower.Lobby;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class BossGateUiController : MonoBehaviour
    {
        private RectTransform _root;
        private Text _floorText;
        private Text _sanityText;
        private Text _hpText;
        private Text _goldText;
        private Image _sanityFill;
        private Image _hpFill;
        private Text _titleText;
        private Image _sceneImage;
        private Text _readinessText;
        private Button _engageButton;
        private Text _warningText;
        private Button _retreatButton;
        private Action<string> _onEngage;
        private Action _onRetreat;
        private string _engageChoiceStableId = string.Empty;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public Button EngageButton => _engageButton;
        public Button RetreatButton => _retreatButton;
        public string SceneSpriteName => _sceneImage == null || _sceneImage.sprite == null ? string.Empty : _sceneImage.sprite.name;
        public string ReadinessText => _readinessText == null ? string.Empty : _readinessText.text;

        public void Initialize(Transform parent, Action<string> onEngage, Action onRetreat)
        {
            _onEngage = onEngage;
            _onRetreat = onRetreat;
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
            backdrop.color = new Color(BossGateUiTokens.VoidBg.r, BossGateUiTokens.VoidBg.g, BossGateUiTokens.VoidBg.b, 0.88f);
            backdrop.raycastTarget = false;

            BuildResourceStrip();
            _titleText = CreateTextSlot("Boss Gate Title", BossGateLayout.GateTitle, BossGateUiTokens.Ink);
            _titleText.fontStyle = FontStyle.Bold;
            _sceneImage = CreateImageSlot("Boss Gate Scene", BossGateLayout.GateScene, BossGateUiTokens.PanelBg);
            _sceneImage.preserveAspect = false;
            var readiness = CreatePanelSlot("Boss Gate Readiness Panel", BossGateLayout.ReadinessPanel, BossGateUiTokens.PanelBgAlt, true);
            _readinessText = CreateChildText(readiness, "Readiness Text", string.Empty, UiTokenContract.BodyFontSize, BossGateUiTokens.Ink, new Vector2(0.08f, 0.10f), new Vector2(0.92f, 0.90f), TextAnchor.MiddleLeft);
            _engageButton = CreateSlotButton("Boss Gate Engage Button", BossGateLayout.EngageButton, BossGateUiTokens.FrameHi);
            _engageButton.onClick.AddListener(Engage);
            _warningText = CreateTextSlot("Boss Gate Warning Line", BossGateLayout.WarningLine, BossGateUiTokens.Gold);
            _retreatButton = CreateSlotButton("Boss Gate Retreat Button", BossGateLayout.RetreatButton, BossGateUiTokens.Frame);
            _retreatButton.onClick.AddListener(() => _onRetreat?.Invoke());
            Hide();
        }

        public void Show(PrototypeRunSnapshot snapshot, Sprite sceneSprite, string engageChoiceStableId, bool engageChoiceEnabled)
        {
            _root.gameObject.SetActive(true);
            _floorText.text = "층 " + snapshot.CurrentFloor;
            _sanityText.text = "이성 " + snapshot.Mental;
            _hpText.text = "체력 " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp;
            _goldText.text = "골드 " + snapshot.Gold;
            _sanityFill.fillAmount = Mathf.Clamp01((snapshot.Mental + 100f) / 200f);
            _hpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            _titleText.text = "문 앞";
            _sceneImage.sprite = sceneSprite;
            _sceneImage.color = sceneSprite == null ? BossGateUiTokens.PanelBg : new Color(1f, 1f, 1f, 0.62f);
            _sceneImage.gameObject.SetActive(true);
            _readinessText.text = "체력 " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + "\n이성 " + snapshot.Mental + "\n보유품 " + snapshot.ItemCount;
            _engageChoiceStableId = engageChoiceStableId ?? string.Empty;
            _engageButton.gameObject.SetActive(true);
            _engageButton.interactable = snapshot.BossGateUnlocked && engageChoiceEnabled && !string.IsNullOrEmpty(_engageChoiceStableId);
            _warningText.text = _engageButton.interactable ? "되돌릴 수 없습니다" : "아직 열리지 않았다";
            _warningText.gameObject.SetActive(true);
            _retreatButton.gameObject.SetActive(true);
            _retreatButton.interactable = true;
        }

        public bool Owns(Button button) => button != null && (button == _engageButton || button == _retreatButton);

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        private void Engage()
        {
            if (_engageButton.interactable && !string.IsNullOrEmpty(_engageChoiceStableId))
            {
                _onEngage?.Invoke(_engageChoiceStableId);
            }
        }

        private void BuildResourceStrip()
        {
            _floorText = CreateChip(BossGateLayout.FloorChip, BossGateUiTokens.GoldCoin);
            (_sanityText, _sanityFill) = CreateGauge(BossGateLayout.SanityChip, BossGateUiTokens.Sanity);
            (_hpText, _hpFill) = CreateGauge(BossGateLayout.HpChip, BossGateUiTokens.Hp);
            _goldText = CreateChip(BossGateLayout.GoldChip, BossGateUiTokens.GoldCoin);
        }

        private Text CreateChip(BossGateSlot slot, Color accent)
        {
            var panel = CreatePanelSlot("BossGate " + slot.Key, slot, BossGateUiTokens.PanelBg, true);
            var accentImage = CreateImage(panel, "Accent", new Vector2(0.03f, 0.18f), new Vector2(0.06f, 0.82f), accent);
            accentImage.raycastTarget = false;
            return CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, BossGateUiTokens.Ink, new Vector2(0.10f, 0.10f), new Vector2(0.94f, 0.90f), TextAnchor.MiddleCenter);
        }

        private (Text text, Image fill) CreateGauge(BossGateSlot slot, Color fillColor)
        {
            var panel = CreatePanelSlot("BossGate " + slot.Key, slot, BossGateUiTokens.PanelBg, true);
            var text = CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, BossGateUiTokens.Ink, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.92f), TextAnchor.MiddleCenter);
            return (text, CreateGaugeFill(panel, "Fill", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.32f), fillColor));
        }

        private Button CreateSlotButton(string name, BossGateSlot slot, Color frameColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = BossGateUiTokens.PanelBgAlt;
            AddFrame(buttonObject, frameColor);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rect, "Label", slot.Content, slot.FontSize, BossGateUiTokens.Ink, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, BossGateSlot slot, Color color, bool framed)
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
                AddFrame(panelObject, BossGateUiTokens.Frame);
            }

            return rect;
        }

        private Image CreateImageSlot(string name, BossGateSlot slot, Color color)
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

        private Text CreateTextSlot(string name, BossGateSlot slot, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(_root, false);
            var rect = textObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var text = textObject.AddComponent<Text>();
            text.font = BossGateUiTokens.ResolveRuntimeFont();
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
            text.font = BossGateUiTokens.ResolveRuntimeFont();
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
            var frame = CreateImage(parent, name + " Frame", anchorMin, anchorMax, BossGateUiTokens.VoidBg);
            var fill = CreateImage(frame.rectTransform, name, new Vector2(0.02f, 0.18f), new Vector2(0.98f, 0.82f), color);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private static void ApplySlot(RectTransform rect, BossGateSlot slot)
        {
            rect.anchorMin = BossGateLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = BossGateLayoutRuntime.AnchorMax(slot);
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
