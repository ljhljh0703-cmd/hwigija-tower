using System;
using HwigiTower.Lobby;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class EndingUiController : MonoBehaviour
    {
        private RectTransform _root;
        private Text _titleText;
        private Image _sceneImage;
        private Text _summaryText;
        private Button _restButton;
        private Button _continueButton;
        private Text _irreversibleNote;
        private Action _onRest;
        private Action _onContinue;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public Button RestButton => _restButton;
        public Button ContinueButton => _continueButton;
        public string RunSummaryText => _summaryText == null ? string.Empty : _summaryText.text;

        public void Initialize(Transform parent, Action onRest, Action onContinue)
        {
            _onRest = onRest;
            _onContinue = onContinue;
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
            backdrop.color = new Color(EndingUiTokens.VoidBg.r, EndingUiTokens.VoidBg.g, EndingUiTokens.VoidBg.b, 0.88f);
            backdrop.raycastTarget = false;

            _titleText = CreateTextSlot("Ending Title", EndingLayout.EndingTitle, EndingUiTokens.Ink);
            _titleText.fontStyle = FontStyle.Bold;
            _sceneImage = CreateImageSlot("Ending Scene", EndingLayout.EndingScene, EndingUiTokens.PanelBg);
            _sceneImage.preserveAspect = false;
            var summaryPanel = CreatePanelSlot("Ending Run Summary", EndingLayout.RunSummary, EndingUiTokens.PanelBgAlt, true);
            _summaryText = CreateChildText(summaryPanel, "Summary Text", string.Empty, UiTokenContract.BodyFontSize, EndingUiTokens.Ink, new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.88f), TextAnchor.MiddleLeft);
            _restButton = CreateSlotButton("Ending Button Rest", EndingLayout.ChoiceRest);
            _restButton.onClick.AddListener(() => _onRest?.Invoke());
            _continueButton = CreateSlotButton("Ending Button Continue", EndingLayout.ChoiceContinue);
            _continueButton.onClick.AddListener(() => _onContinue?.Invoke());
            _irreversibleNote = CreateTextSlot("Ending Irreversible Note", EndingLayout.IrreversibleNote, EndingUiTokens.InkDim);
            Hide();
        }

        public void Show(PrototypeRunSnapshot snapshot, Sprite sceneSprite, bool interactable)
        {
            _root.gameObject.SetActive(true);
            _titleText.text = EndingLayout.EndingTitle.Content;
            _sceneImage.sprite = sceneSprite;
            _sceneImage.color = sceneSprite == null ? EndingUiTokens.PanelBg : new Color(1f, 1f, 1f, 0.62f);
            _sceneImage.gameObject.SetActive(true);
            _summaryText.text = "도달 층 " + snapshot.CurrentFloor + "\n해결 노드 " + snapshot.NodesResolved + " | 전투 승 " + snapshot.BattlesWon + " | 기억 조각 " + snapshot.MemoryFragmentCount;
            ConfigureChoice(_restButton, EndingLayout.ChoiceRest, interactable);
            ConfigureChoice(_continueButton, EndingLayout.ChoiceContinue, interactable);
            _irreversibleNote.text = EndingLayout.IrreversibleNote.Content;
            _irreversibleNote.gameObject.SetActive(true);
        }

        public bool Owns(Button button) => button != null && (button == _restButton || button == _continueButton);

        public void Hide()
        {
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
        }

        private static void ConfigureChoice(Button button, EndingSlot slot, bool interactable)
        {
            button.gameObject.SetActive(true);
            button.interactable = interactable;
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.text = slot.Content;
            }
        }

        private Button CreateSlotButton(string name, EndingSlot slot)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = EndingUiTokens.PanelBgAlt;
            AddFrame(buttonObject, EndingUiTokens.Frame);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rect, "Label", slot.Content, slot.FontSize, EndingUiTokens.Ink, Vector2.zero, Vector2.one, TextAnchor.MiddleCenter);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, EndingSlot slot, Color color, bool framed)
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
                AddFrame(panelObject, EndingUiTokens.Frame);
            }

            return rect;
        }

        private Image CreateImageSlot(string name, EndingSlot slot, Color color)
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

        private Text CreateTextSlot(string name, EndingSlot slot, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(_root, false);
            var rect = textObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var text = textObject.AddComponent<Text>();
            text.font = EndingUiTokens.ResolveRuntimeFont();
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
            text.font = EndingUiTokens.ResolveRuntimeFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = content;
            text.raycastTarget = false;
            return text;
        }

        private static void ApplySlot(RectTransform rect, EndingSlot slot)
        {
            rect.anchorMin = EndingLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = EndingLayoutRuntime.AnchorMax(slot);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void AddFrame(GameObject target, Color color)
        {
            var outline = target.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
        }
    }
}
