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
        public void ShowMapChoices(PrototypeFloorMapNodeView[] nodes, Action<string> onNodeSelected)
        {
            ClearChoices();
            HideRestInteractionPanel();
            HideEventCutsceneLayout();
            EnsureScreenLayers();
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(objectiveLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            HideMerchantPresentation();
            HideNpcSpotlight();
            EnsureEventSystem();
            EnsureFloorMapUi(onNodeSelected);
            if (floorMapUiController == null || nodes == null)
            {
                return;
            }

            var snapshot = _roomController == null ? default(PrototypeRunSnapshot) : _roomController.GetSnapshot();
            floorMapUiController.Show(snapshot, nodes);
            for (var i = 0; i < floorMapUiController.NodeButtons.Count; i++)
            {
                _choiceButtons.Add(floorMapUiController.NodeButtons[i]);
            }

            if (interactionText != null)
            {
                interactionText.text = showRawDebugText ? "map node selection" : "지도";
                interactionText.gameObject.SetActive(showRawDebugText);
            }

            ShowResultMessage(showRawDebugText ? "select map node" : "선택 가능한 길이 밝게 표시됩니다");
            SetResultVisible(false);
            HideLegacyEncounterVisuals(hideBackground: true);
        }

        private void EnsureFloorMapUi(Action<string> onNodeSelected)
        {
            if (floorMapUiController == null)
            {
                var moduleObject = new GameObject("Floor Map UI Module", typeof(RectTransform));
                moduleObject.transform.SetParent(HudParent, false);
                floorMapUiController = moduleObject.AddComponent<FloorMapUiController>();
            }

            floorMapUiController.Initialize(
                HudParent,
                onNodeSelected,
                OpenCurrentRouteStep,
                HideFloorMapUi,
                ResolveNodeIcon);
        }

        private void HideFloorMapUi()
        {
            if (floorMapUiController != null)
            {
                floorMapUiController.Hide();
            }
        }

        private void ClearMapDecorations()
        {
            for (var i = 0; i < _mapDecorations.Count; i++)
            {
                if (_mapDecorations[i] != null)
                {
                    DestroyHudObject(_mapDecorations[i]);
                }
            }

            _mapDecorations.Clear();
        }

        private void ApplyFloorMapBackground(PrototypeFloorMapNodeView[] nodes)
        {
            EnsureFloorMapBackgroundImage();
            if (floorMapBackgroundImage == null)
            {
                return;
            }

            var floor = nodes != null && nodes.Length > 0 ? nodes[0].Floor : 1;
            floorMapBackgroundImage.sprite = presentationData != null && presentationData.TryGetFloorMapBackground(floor, out var sprite) ? sprite : null;
            floorMapBackgroundImage.color = floorMapBackgroundImage.sprite == null
                ? new Color(0.024f, 0.033f, 0.043f, 0.92f)
                : new Color(1f, 1f, 1f, 0.62f);
            floorMapBackgroundImage.gameObject.SetActive(true);
        }

        private void EnsureFloorMapBackgroundImage()
        {
            if (floorMapBackgroundImage != null || nodeMapLayer == null)
            {
                return;
            }

            var backgroundObject = new GameObject("Floor Map Background");
            backgroundObject.transform.SetParent(nodeMapLayer, false);
            backgroundObject.transform.SetAsFirstSibling();
            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            floorMapBackgroundImage = backgroundObject.AddComponent<Image>();
            floorMapBackgroundImage.preserveAspect = true;
            floorMapBackgroundImage.raycastTarget = false;
        }

        private void DrawMapRouteLines(PrototypeFloorMapNodeView[] nodes)
        {
            if (nodes == null || nodeMapLayer == null)
            {
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                var from = nodes[i];
                if (from.NextMapNodeIds == null)
                {
                    continue;
                }

                for (var n = 0; n < from.NextMapNodeIds.Length; n++)
                {
                    if (TryFindMapNode(nodes, from.NextMapNodeIds[n], out var to))
                    {
                        DrawMapRouteLine(from, to);
                    }
                }
            }
        }

        private void DrawMapStartMarker(PrototypeFloorMapNodeView[] nodes)
        {
            if (nodes == null || nodes.Length == 0 || presentationData == null || presentationData.CurrentPositionMarker == null)
            {
                return;
            }

            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Current)
                {
                    return;
                }
            }

            var markerObject = new GameObject("Map Start Marker");
            markerObject.transform.SetParent(nodeMapLayer, false);
            var rect = markerObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.04f);
            rect.anchorMax = new Vector2(0.5f, 0.04f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(MapNodeIconSize * 0.78f, MapNodeIconSize * 0.78f);
            var image = markerObject.AddComponent<Image>();
            image.sprite = presentationData.CurrentPositionMarker;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0.95f);
            _mapDecorations.Add(markerObject);
        }

        private static bool TryFindMapNode(PrototypeFloorMapNodeView[] nodes, string mapNodeId, out PrototypeFloorMapNodeView node)
        {
            for (var i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].MapNodeId == mapNodeId)
                {
                    node = nodes[i];
                    return true;
                }
            }

            node = default;
            return false;
        }

        private void DrawMapRouteLine(PrototypeFloorMapNodeView from, PrototypeFloorMapNodeView to)
        {
            var lineObject = new GameObject("Map Route Line");
            lineObject.transform.SetParent(nodeMapLayer, false);
            lineObject.transform.SetAsFirstSibling();
            var rect = lineObject.AddComponent<RectTransform>();
            var start = ResolveMapNodePosition(from);
            var end = ResolveMapNodePosition(to);

            var parentRect = nodeMapLayer.rect;
            var parentWidth = parentRect.width > 1f ? parentRect.width : 960f;
            var parentHeight = parentRect.height > 1f ? parentRect.height : 500f;
            var startPixels = new Vector2(start.x * parentWidth, start.y * parentHeight);
            var endPixels = new Vector2(end.x * parentWidth, end.y * parentHeight);
            var rawDelta = endPixels - startPixels;
            var rawLength = rawDelta.magnitude;
            if (rawLength < 8f)
            {
                Destroy(lineObject);
                return;
            }

            var direction = rawDelta / rawLength;
            var inset = Mathf.Min(MapNodeButtonHeight * 0.44f, rawLength * 0.32f);
            startPixels += direction * inset;
            endPixels -= direction * inset;
            var adjustedDelta = endPixels - startPixels;
            var adjustedCenter = (startPixels + endPixels) * 0.5f;
            var center = new Vector2(adjustedCenter.x / parentWidth, adjustedCenter.y / parentHeight);
            rect.anchorMin = center;
            rect.anchorMax = center;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(Mathf.Max(12f, adjustedDelta.magnitude), 3.5f);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(adjustedDelta.y, adjustedDelta.x) * Mathf.Rad2Deg);

            var image = lineObject.AddComponent<Image>();
            image.color = ResolveMapRouteLineColor(from, to);
            image.raycastTarget = false;
            _mapDecorations.Add(lineObject);
        }

        private static Color ResolveMapRouteLineColor(PrototypeFloorMapNodeView from, PrototypeFloorMapNodeView to)
        {
            if (from.Completed && !to.Locked)
            {
                return new Color(0.82f, 0.96f, 0.74f, 0.86f);
            }

            if (from.Current || from.Selectable || to.Selectable)
            {
                return new Color(0.76f, 0.84f, 0.92f, 0.64f);
            }

            return new Color(0.44f, 0.50f, 0.56f, 0.34f);
        }

        private bool IsMapSelectionVisible(PrototypeRunSnapshot snapshot)
        {
            if (!IsMapRouteState(snapshot))
            {
                return false;
            }

            if (floorMapUiController != null && floorMapUiController.Visible && floorMapUiController.NodeButtons.Count > 0)
            {
                return true;
            }

            return choiceContainer != null &&
                nodeMapLayer != null &&
                choiceContainer.parent == nodeMapLayer &&
                _choiceButtons.Count > 0;
        }

        private bool IsMapRouteState(PrototypeRunSnapshot snapshot)
        {
            return snapshot.HasFloorMap &&
                !snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !snapshot.StairUnlocked &&
                !snapshot.HasSelectedMapNode &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive &&
                !_bossGatePresentationActive;
        }

        private void AutoShowMapIfNeeded(PrototypeRunSnapshot snapshot)
        {
            if (_roomController == null ||
                !snapshot.HasFloorMap ||
                snapshot.HasSelectedMapNode ||
                snapshot.IsInCombat ||
                snapshot.RunCompleted ||
                snapshot.StairUnlocked ||
                RestInteractionPanelVisible ||
                _shopPresentationActive ||
                _eventPresentationActive ||
                _bossGatePresentationActive)
            {
                return;
            }

            if (IsMapSelectionVisible(snapshot))
            {
                return;
            }

            if (_choiceButtons.Count > 0)
            {
                ClearChoices();
            }

            var mapNodes = _roomController.GetFloorMapNodes();
            if (mapNodes.Length == 0)
            {
                return;
            }

            ShowMapChoices(mapNodes, mapNodeId =>
            {
                var selected = _roomController.SelectMapNode(mapNodeId);
                OpenSelectedRouteStep(selected);
            });
        }

        private Button CreateMapNodeButton(PrototypeFloorMapNodeView node, Action<string> onNodeSelected)
        {
            var index = _choiceButtons.Count;
            var view = new PrototypeEncounterChoiceView(
                node.MapNodeId,
                ResolveMapNodeLabel(node.Type),
                true,
                node.Selectable,
                string.Empty,
                BuildMapNodeHint(node));
            var button = CreateChoiceButton(view, onNodeSelected);
            button.name = "Map Node Button " + node.MapNodeId;

            var rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                var position = ResolveMapNodePosition(node);
                rect.anchorMin = position;
                rect.anchorMax = position;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(MapNodeButtonHeight, MapNodeButtonHeight);
                rect.anchoredPosition = Vector2.zero;
            }

            var image = button.targetGraphic as Image;
            if (image != null)
            {
                image.color = ResolveMapNodeTint(node);
            }

            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                var labelRect = label.GetComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0f, 0f);
                labelRect.anchorMax = new Vector2(1f, 0.28f);
                labelRect.offsetMin = new Vector2(4f, 2f);
                labelRect.offsetMax = new Vector2(-4f, -2f);
                label.text = ResolveMapNodeLabel(node.Type);
                label.fontSize = node.Selectable ? 22 : 19;
                label.resizeTextMinSize = 16;
                label.resizeTextMaxSize = node.Selectable ? 22 : 19;
                label.alignment = TextAnchor.MiddleCenter;
                label.color = node.Selectable
                    ? new Color(0.96f, 0.99f, 1f, 1f)
                    : node.Completed
                        ? new Color(0.74f, 0.84f, 0.78f, 0.92f)
                        : new Color(0.50f, 0.54f, 0.58f, 0.82f);
            }

            var iconObject = new GameObject("Node Icon");
            iconObject.transform.SetParent(button.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.58f);
            iconRect.anchorMax = new Vector2(0.5f, 0.58f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(MapNodeIconSize * 0.92f, MapNodeIconSize * 0.92f);

            var icon = iconObject.AddComponent<Image>();
            icon.sprite = ResolveNodeIcon(node.Type);
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            icon.color = node.Completed
                ? new Color(0.56f, 0.78f, 0.66f, 0.84f)
                : node.Locked
                    ? new Color(0.32f, 0.34f, 0.38f, 0.52f)
                    : Color.white;
            _mapNodeIconImages.Add(icon);

            if (node.Selectable && presentationData != null && presentationData.SelectedNodeRing != null)
            {
                AddMapNodeOverlay(button.transform, "Node Selectable Ring", presentationData.SelectedNodeRing, new Color(1f, 0.94f, 0.62f, 0.96f), 1.10f);
            }

            if (node.Locked && presentationData != null && presentationData.LockedNodeOverlay != null)
            {
                AddMapNodeOverlay(button.transform, "Node Locked Overlay", presentationData.LockedNodeOverlay, new Color(1f, 1f, 1f, 0.72f), 1.02f);
            }

            if (node.Completed && ResolveCompletedNodeBackground() != null)
            {
                AddMapNodeOverlay(button.transform, "Node Completed Mark", ResolveCompletedNodeBackground(), new Color(1f, 1f, 1f, 0.92f), 0.86f);
            }

            if (node.Current && presentationData != null && presentationData.CurrentPositionMarker != null)
            {
                AddMapNodeOverlay(button.transform, "Node Current Marker", presentationData.CurrentPositionMarker, new Color(1f, 1f, 1f, 0.98f), 1.24f);
            }

            return button;
        }

        private static Vector2 ResolveMapNodePosition(PrototypeFloorMapNodeView node)
        {
            if (node.NormalizedX > 0f || node.NormalizedY > 0f)
            {
                return new Vector2(node.NormalizedX, node.NormalizedY);
            }

            var fallbackX = node.Index <= 0 ? 0.34f : node.Index == 1 ? 0.66f : 0.50f;
            var fallbackY = node.Layer switch
            {
                1 => 0.16f,
                2 => 0.37f,
                3 => 0.58f,
                4 => 0.75f,
                5 => 0.91f,
                _ => 0.5f
            };
            return new Vector2(fallbackX, fallbackY);
        }

        private void AddMapNodeOverlay(Transform parent, string name, Sprite sprite, Color color, float scale)
        {
            if (sprite == null)
            {
                return;
            }

            var overlayObject = new GameObject(name);
            overlayObject.transform.SetParent(parent, false);
            overlayObject.transform.SetAsFirstSibling();
            var rect = overlayObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(MapNodeButtonHeight * scale, MapNodeButtonHeight * scale);

            var image = overlayObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = color;
        }

        private Sprite ResolveNodeIcon(PrototypeFloorMapNodeType type)
        {
            return presentationData != null && presentationData.TryGetNodeIcon(type, out var icon) ? icon : null;
        }

        private Sprite ResolveCompletedNodeBackground()
        {
            return presentationData != null && presentationData.TryGetCompletedNodeBackground(out var sprite) ? sprite : null;
        }

        private static Color ResolveMapNodeTint(PrototypeFloorMapNodeView node)
        {
            if (node.Completed)
            {
                return new Color(0.07f, 0.20f, 0.14f, 0.90f);
            }

            if (node.Locked || !node.Selectable)
            {
                return new Color(0.035f, 0.040f, 0.050f, 0.64f);
            }

            return node.Type == PrototypeFloorMapNodeType.Boss
                ? new Color(0.36f, 0.08f, 0.10f, 0.99f)
                : node.Type == PrototypeFloorMapNodeType.Shop
                    ? new Color(0.27f, 0.20f, 0.08f, 0.99f)
                    : new Color(0.11f, 0.22f, 0.28f, 0.99f);
        }

        private static string BuildMapNodeHint(PrototypeFloorMapNodeView node)
        {
            var state = node.Completed ? "완료" : node.Locked ? "잠김" : node.Selectable ? "선택 가능" : "대기";
            return state + " · " + ResolveMapNodeHint(node.Type);
        }

        private void ConfigureChoiceContainerForMap()
        {
            EnsureChoiceContainer();
            if (choiceContainer == null || nodeMapLayer == null)
            {
                return;
            }

            choiceContainer.SetParent(nodeMapLayer, false);
            choiceContainer.anchorMin = Vector2.zero;
            choiceContainer.anchorMax = Vector2.one;
            choiceContainer.pivot = new Vector2(0.5f, 0.5f);
            choiceContainer.sizeDelta = Vector2.zero;
            choiceContainer.anchoredPosition = Vector2.zero;
        }

        private static string ResolveMapNodeLabel(PrototypeFloorMapNodeType type)
        {
            return type switch
            {
                PrototypeFloorMapNodeType.Combat => "전투",
                PrototypeFloorMapNodeType.Event => "이벤트",
                PrototypeFloorMapNodeType.Rest => "휴식",
                PrototypeFloorMapNodeType.Shop => "상점",
                PrototypeFloorMapNodeType.Boss => "보스",
                _ => "노드"
            };
        }

        private static string ResolveMapNodeHint(PrototypeFloorMapNodeType type)
        {
            return type switch
            {
                PrototypeFloorMapNodeType.Combat => "전투 보상 또는 피해",
                PrototypeFloorMapNodeType.Event => "결과는 선택 후 공개",
                PrototypeFloorMapNodeType.Rest => "HP 회복 / 내부 불안 감소",
                PrototypeFloorMapNodeType.Shop => "보스 전 준비",
                PrototypeFloorMapNodeType.Boss => "승리하면 층 클리어",
                _ => "선택 후 공개"
            };
        }
    }
}
