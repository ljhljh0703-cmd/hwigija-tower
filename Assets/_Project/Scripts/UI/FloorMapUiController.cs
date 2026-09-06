using System;
using System.Collections.Generic;
using HwigiTower.Lobby;
using HwigiTower.Run;
using UnityEngine;
using UnityEngine.UI;

namespace HwigiTower.UI
{
    public sealed class FloorMapUiController : MonoBehaviour
    {
        private readonly List<Button> _nodeButtons = new List<Button>();
        private readonly List<GameObject> _graphDecorations = new List<GameObject>();
        private readonly List<Image> _nodeIconImages = new List<Image>();
        private RectTransform _root;
        private RectTransform _nodeGraph;
        private Text _floorText;
        private Text _sanityText;
        private Text _hpText;
        private Text _goldText;
        private Image _sanityFill;
        private Image _hpFill;
        private Text _titleText;
        private Text _legendText;
        private Text _selectedNodeText;
        private Button _departButton;
        private Button _backButton;
        private string _selectedNodeId = string.Empty;
        private PrototypeFloorMapNodeView[] _nodes = Array.Empty<PrototypeFloorMapNodeView>();
        private Func<PrototypeFloorMapNodeType, Sprite> _resolveNodeIcon;
        private Action<string> _onNodeSelected;
        private Action _onDepart;
        private Action _onBack;

        public RectTransform Root => _root;
        public bool Visible => _root != null && _root.gameObject.activeSelf;
        public RectTransform NodeGraph => _nodeGraph;
        public Button DepartButton => _departButton;
        public Button BackButton => _backButton;
        public IReadOnlyList<Button> NodeButtons => _nodeButtons;
        public string SelectedNodeId => _selectedNodeId;
        public string NodeIconSpriteNames
        {
            get
            {
                var names = new List<string>();
                for (var i = 0; i < _nodeIconImages.Count; i++)
                {
                    var icon = _nodeIconImages[i];
                    if (icon != null && icon.sprite != null)
                    {
                        names.Add(icon.sprite.name);
                    }
                }

                return string.Join("|", names);
            }
        }

        public void Initialize(
            Transform parent,
            Action<string> onNodeSelected,
            Action onDepart,
            Action onBack,
            Func<PrototypeFloorMapNodeType, Sprite> resolveNodeIcon)
        {
            ConfigureCallbacks(onNodeSelected, onDepart, onBack, resolveNodeIcon);
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
            backdrop.color = FloorMapUiTokens.VoidBg;
            backdrop.raycastTarget = false;

            BuildResourceStrip();
            _titleText = CreateTextSlot("FloorMap Screen Title", FloorMapLayout.ScreenTitle, FloorMapUiTokens.Ink);
            _titleText.fontStyle = FontStyle.Bold;
            _nodeGraph = CreatePanelSlot("FloorMap Node Graph", FloorMapLayout.NodeGraph, FloorMapUiTokens.PanelBg, true);
            _legendText = CreateTextSlot("FloorMap Node Legend", FloorMapLayout.NodeLegend, FloorMapUiTokens.InkDim);
            _legendText.text = "● 전투  ◌ 사건  ◉ 휴식  ◐ 상점  ◆ 보스";
            var selectedCard = CreatePanelSlot("FloorMap Selected Card", FloorMapLayout.SelectedNodeCard, FloorMapUiTokens.PanelBgAlt, true);
            _selectedNodeText = CreateChildText(selectedCard, "FloorMap Selected Node Text", string.Empty, UiTokenContract.BodyFontSize, FloorMapUiTokens.Ink, new Vector2(0.06f, 0.10f), new Vector2(0.94f, 0.90f));
            _selectedNodeText.alignment = TextAnchor.MiddleLeft;
            _departButton = CreateSlotButton("FloorMap Depart Button", FloorMapLayout.DepartButton, FloorMapUiTokens.FrameHi);
            _departButton.onClick.AddListener(InvokeDepart);
            _backButton = CreateSlotButton("FloorMap Back Button", FloorMapLayout.BackButton, FloorMapUiTokens.Frame);
            _backButton.onClick.AddListener(InvokeBack);
            Hide();
        }

        public void ConfigureCallbacks(
            Action<string> onNodeSelected,
            Action onDepart,
            Action onBack,
            Func<PrototypeFloorMapNodeType, Sprite> resolveNodeIcon)
        {
            _onNodeSelected = onNodeSelected;
            _onDepart = onDepart;
            _onBack = onBack;
            _resolveNodeIcon = resolveNodeIcon;
        }

        public void Show(PrototypeRunSnapshot snapshot, PrototypeFloorMapNodeView[] nodes)
        {
            _root.gameObject.SetActive(true);
            _nodes = nodes ?? Array.Empty<PrototypeFloorMapNodeView>();
            _selectedNodeId = snapshot.HasSelectedMapNode ? snapshot.SelectedMapNodeId : string.Empty;
            _floorText.text = "층 " + snapshot.CurrentFloor;
            _sanityText.text = "이성 " + snapshot.Mental;
            _hpText.text = "체력 " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp;
            _goldText.text = "골드 " + snapshot.Gold;
            _sanityFill.fillAmount = Mathf.Clamp01((snapshot.Mental + 100f) / 200f);
            _hpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            _titleText.text = snapshot.CurrentFloor + "층";
            RebuildGraph();
            RefreshSelectedCard();
        }

        public void SelectNode(string mapNodeId)
        {
            if (!TryFindNode(mapNodeId, out var node) || !node.Selectable)
            {
                return;
            }

            _selectedNodeId = mapNodeId;
            _onNodeSelected?.Invoke(mapNodeId);
            RebuildGraph();
            RefreshSelectedCard();
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
            _floorText = CreateChip(FloorMapLayout.FloorChip, FloorMapUiTokens.GoldCoin);
            (_sanityText, _sanityFill) = CreateGauge(FloorMapLayout.SanityChip, FloorMapUiTokens.Sanity);
            (_hpText, _hpFill) = CreateGauge(FloorMapLayout.HpChip, FloorMapUiTokens.Hp);
            _goldText = CreateChip(FloorMapLayout.GoldChip, FloorMapUiTokens.GoldCoin);
        }

        private void RebuildGraph()
        {
            for (var i = 0; i < _nodeButtons.Count; i++)
            {
                DestroyHudObject(_nodeButtons[i] == null ? null : _nodeButtons[i].gameObject);
            }
            _nodeButtons.Clear();
            _nodeIconImages.Clear();

            for (var i = 0; i < _graphDecorations.Count; i++)
            {
                DestroyHudObject(_graphDecorations[i]);
            }
            _graphDecorations.Clear();

            for (var i = 0; i < _nodes.Length; i++)
            {
                var node = _nodes[i];
                for (var next = 0; next < node.NextMapNodeIds.Length; next++)
                {
                    if (TryFindNode(node.NextMapNodeIds[next], out var target))
                    {
                        DrawRouteLine(node, target);
                    }
                }
            }

            for (var i = 0; i < _nodes.Length; i++)
            {
                _nodeButtons.Add(CreateNodeButton(_nodes[i]));
            }
        }

        private Button CreateNodeButton(PrototypeFloorMapNodeView node)
        {
            var nodeObject = new GameObject("Map Node Button " + node.MapNodeId, typeof(RectTransform));
            nodeObject.transform.SetParent(_nodeGraph, false);
            var rect = nodeObject.GetComponent<RectTransform>();
            var position = new Vector2(node.NormalizedX, node.NormalizedY);
            rect.anchorMin = position;
            rect.anchorMax = position;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(UiTokenContract.TapTargetMinPx, UiTokenContract.TapTargetMinPx);
            rect.anchoredPosition = Vector2.zero;

            var image = nodeObject.AddComponent<Image>();
            image.color = ResolveNodeFill(node);
            var outline = nodeObject.AddComponent<Outline>();
            outline.effectColor = ResolveNodeFrame(node);
            outline.effectDistance = node.Locked ? new Vector2(1f, -1f) : new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            var button = nodeObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.interactable = node.Selectable;
            button.onClick.AddListener(() => SelectNode(node.MapNodeId));

            var icon = CreateImage(rect, "Node Icon", new Vector2(0.24f, 0.32f), new Vector2(0.76f, 0.84f), FloorMapUiTokens.Gold);
            icon.sprite = _resolveNodeIcon == null ? null : _resolveNodeIcon(node.Type);
            icon.preserveAspect = true;
            icon.color = node.Locked ? FloorMapUiTokens.InkMute : node.Completed ? FloorMapUiTokens.InkDim : icon.sprite == null ? FloorMapUiTokens.Gold : UiColorTokens.NoTint;
            _nodeIconImages.Add(icon);

            var label = CreateChildText(rect, "Label", ResolveNodeTypeLabel(node.Type), UiTokenContract.MicroFontSize, node.Locked ? FloorMapUiTokens.InkMute : FloorMapUiTokens.Ink, new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.28f));
            var state = CreateChildText(rect, "State", ResolveNodeStateLabel(node), UiTokenContract.MicroFontSize, FloorMapUiTokens.InkDim, new Vector2(0.06f, 0.84f), new Vector2(0.94f, 0.98f));
            if (node.Current)
            {
                var marker = CreateImage(rect, "Current Marker", new Vector2(0.04f, 0.04f), new Vector2(0.16f, 0.16f), FloorMapUiTokens.Gold);
                marker.raycastTarget = false;
            }

            label.raycastTarget = false;
            state.raycastTarget = false;
            return button;
        }

        private void DrawRouteLine(PrototypeFloorMapNodeView from, PrototypeFloorMapNodeView to)
        {
            var lineObject = new GameObject("FloorMap Route Line", typeof(RectTransform));
            lineObject.transform.SetParent(_nodeGraph, false);
            lineObject.transform.SetAsFirstSibling();
            var rect = lineObject.GetComponent<RectTransform>();
            var width = _nodeGraph.rect.width > 1f ? _nodeGraph.rect.width : FloorMapLayout.ReferenceWidth * FloorMapLayout.NodeGraph.Width;
            var height = _nodeGraph.rect.height > 1f ? _nodeGraph.rect.height : FloorMapLayout.ReferenceHeight * FloorMapLayout.NodeGraph.Height;
            var start = new Vector2(from.NormalizedX * width, from.NormalizedY * height);
            var end = new Vector2(to.NormalizedX * width, to.NormalizedY * height);
            var delta = end - start;
            if (delta.sqrMagnitude < 1f)
            {
                DestroyHudObject(lineObject);
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = (start + end) * 0.5f - new Vector2(width, height) * 0.5f;
            rect.sizeDelta = new Vector2(delta.magnitude, 3f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            var image = lineObject.AddComponent<Image>();
            image.color = from.Completed || to.Selectable ? FloorMapUiTokens.FrameHi : FloorMapUiTokens.Frame;
            image.raycastTarget = false;
            _graphDecorations.Add(lineObject);
        }

        private void RefreshSelectedCard()
        {
            var hasSelectedNode = TryFindNode(_selectedNodeId, out var node);
            _selectedNodeText.text = hasSelectedNode
                ? ResolveNodeTypeLabel(node.Type) + " · " + ResolveNodeStateLabel(node)
                : "길을 고르십시오";
            _departButton.interactable = hasSelectedNode && node.Selectable;
            _departButton.gameObject.SetActive(true);
            _backButton.gameObject.SetActive(true);
        }

        private void InvokeDepart()
        {
            _onDepart?.Invoke();
        }

        private void InvokeBack()
        {
            _onBack?.Invoke();
        }

        private bool TryFindNode(string mapNodeId, out PrototypeFloorMapNodeView node)
        {
            for (var i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i].MapNodeId == mapNodeId)
                {
                    node = _nodes[i];
                    return true;
                }
            }

            node = default;
            return false;
        }

        private Text CreateChip(FloorMapSlot slot, Color accent)
        {
            var panel = CreatePanelSlot("FloorMap " + slot.Key, slot, FloorMapUiTokens.PanelBg, true);
            var accentImage = CreateImage(panel, "Accent", new Vector2(0.03f, 0.18f), new Vector2(0.06f, 0.82f), accent);
            accentImage.raycastTarget = false;
            return CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, FloorMapUiTokens.Ink, new Vector2(0.10f, 0.10f), new Vector2(0.94f, 0.90f));
        }

        private (Text text, Image fill) CreateGauge(FloorMapSlot slot, Color fillColor)
        {
            var panel = CreatePanelSlot("FloorMap " + slot.Key, slot, FloorMapUiTokens.PanelBg, true);
            var text = CreateChildText(panel, "Label", string.Empty, UiTokenContract.LabelFontSize, FloorMapUiTokens.Ink, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.92f));
            var fill = CreateGaugeFill(panel, "Fill", new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.32f), fillColor);
            return (text, fill);
        }

        private Button CreateSlotButton(string name, FloorMapSlot slot, Color frameColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(_root, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var image = buttonObject.AddComponent<Image>();
            image.color = FloorMapUiTokens.PanelBgAlt;
            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = frameColor;
            outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var label = CreateChildText(rect, "Label", slot.Content, slot.FontSize, FloorMapUiTokens.Ink, Vector2.zero, Vector2.one);
            label.raycastTarget = false;
            return button;
        }

        private RectTransform CreatePanelSlot(string name, FloorMapSlot slot, Color color, bool framed)
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
                var outline = panelObject.AddComponent<Outline>();
                outline.effectColor = FloorMapUiTokens.Frame;
                outline.effectDistance = new Vector2(UiTokenContract.FrameBorderWidth, -UiTokenContract.FrameBorderWidth);
            }

            return rect;
        }

        private Text CreateTextSlot(string name, FloorMapSlot slot, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(_root, false);
            var rect = textObject.GetComponent<RectTransform>();
            ApplySlot(rect, slot);
            var text = textObject.AddComponent<Text>();
            text.font = FloorMapUiTokens.ResolveRuntimeFont();
            text.fontSize = slot.FontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = slot.Content;
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

        private static Text CreateChildText(RectTransform parent, string name, string content, int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = textObject.AddComponent<Text>();
            text.font = FloorMapUiTokens.ResolveRuntimeFont();
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.color = color;
            text.text = content;
            text.raycastTarget = false;
            return text;
        }

        private static Image CreateGaugeFill(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var frame = CreateImage(parent, name + " Frame", anchorMin, anchorMax, FloorMapUiTokens.VoidBg);
            var fill = CreateImage(frame.rectTransform, name, new Vector2(0.02f, 0.18f), new Vector2(0.98f, 0.82f), color);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 1f;
            return fill;
        }

        private static void ApplySlot(RectTransform rect, FloorMapSlot slot)
        {
            rect.anchorMin = FloorMapLayoutRuntime.AnchorMin(slot);
            rect.anchorMax = FloorMapLayoutRuntime.AnchorMax(slot);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Color ResolveNodeFill(PrototypeFloorMapNodeView node)
        {
            return node.Completed ? FloorMapUiTokens.PanelBgAlt : node.Locked ? FloorMapUiTokens.VoidBg : node.Selectable ? FloorMapUiTokens.PanelBgAlt : FloorMapUiTokens.PanelBg;
        }

        private static Color ResolveNodeFrame(PrototypeFloorMapNodeView node)
        {
            return node.Current || node.Selectable ? FloorMapUiTokens.FrameHi : node.Completed ? FloorMapUiTokens.Gold : FloorMapUiTokens.Frame;
        }

        private static string ResolveNodeStateLabel(PrototypeFloorMapNodeView node)
        {
            return node.Current ? "현재" : node.Completed ? "완료" : node.Locked ? "잠김" : node.Selectable ? "선택" : "대기";
        }

        private static string ResolveNodeTypeLabel(PrototypeFloorMapNodeType type)
        {
            return type == PrototypeFloorMapNodeType.Combat ? "전투" :
                type == PrototypeFloorMapNodeType.Event ? "사건" :
                type == PrototypeFloorMapNodeType.Rest ? "휴식" :
                type == PrototypeFloorMapNodeType.Shop ? "상점" : "보스";
        }

        private static float Ratio(int current, int maximum)
        {
            return maximum <= 0 ? 0f : Mathf.Clamp01((float)current / maximum);
        }

        private static void DestroyHudObject(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }
    }
}
