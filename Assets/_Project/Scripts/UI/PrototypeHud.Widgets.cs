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

        private Transform HudParent
        {
            get
            {
                EnsurePortraitRoot();
                return portraitRoot == null ? transform : portraitRoot;
            }
        }

        private void EnsurePortraitRoot()
        {
            if (portraitRoot != null)
            {
                return;
            }

            var gutterObject = new GameObject("Portrait Dark Gutter");
            gutterObject.transform.SetParent(transform, false);
            gutterObject.transform.SetAsFirstSibling();
            var gutterRect = gutterObject.AddComponent<RectTransform>();
            gutterRect.anchorMin = Vector2.zero;
            gutterRect.anchorMax = Vector2.one;
            gutterRect.offsetMin = Vector2.zero;
            gutterRect.offsetMax = Vector2.zero;
            var gutter = gutterObject.AddComponent<Image>();
            gutter.color = new Color(0.006f, 0.008f, 0.012f, 1f);
            gutter.raycastTarget = false;

            var rootObject = new GameObject("PortraitRoot");
            rootObject.transform.SetParent(transform, false);
            var rect = rootObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(1080f, 1920f);
            rect.anchoredPosition = Vector2.zero;

            var image = rootObject.AddComponent<Image>();
            image.color = new Color(0.018f, 0.023f, 0.030f, 1f);
            image.raycastTarget = false;
            portraitRoot = rect;
        }

        private void EnsureScreenLayers()
        {
            EnsurePortraitRoot();
            topStatusLayer = EnsureLayerPanel(topStatusLayer, "Screen Layer Top Status", new Vector2(0f, 0.92f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, new Color(0.025f, 0.032f, 0.04f, 0.88f), false);
            objectiveLayer = EnsureLayerPanel(objectiveLayer, "Screen Layer Objective", new Vector2(0.04f, 0.84f), new Vector2(0.96f, 0.915f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, new Color(0.05f, 0.07f, 0.09f, 0.82f), false);
            visualLayer = EnsureLayerPanel(visualLayer, "Screen Layer Visual", new Vector2(0.04f, 0.49f), new Vector2(0.96f, 0.835f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.03f, 0.04f, 0.05f, 0.52f), false);
            npcReactionLayer = EnsureLayerPanel(npcReactionLayer, "Screen Layer Companion Status", new Vector2(0.04f, 0.375f), new Vector2(0.96f, 0.485f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, PanelColor, false);
            resultLayer = EnsureLayerPanel(resultLayer, "Screen Layer Result", new Vector2(0.06f, 0.285f), new Vector2(0.94f, 0.405f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.035f, 0.045f, 0.055f, 0.90f), false);
            nodeMapLayer = EnsureLayerPanel(nodeMapLayer, "Screen Layer Node Map", new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.80f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.030f, 0.040f, 0.050f, 0.90f), false);
            actionLayer = EnsureLayerPanel(actionLayer, "Screen Layer Action", new Vector2(0.06f, 0.045f), new Vector2(0.94f, 0.265f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.02f, 0.025f, 0.03f, 0.50f), false);
            endingLayer = EnsureLayerPanel(endingLayer, "Screen Layer Ending", new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.30f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.06f, 0.055f, 0.04f, 0.90f), false);
        }

        private static Image EnsureHudIcon(Image current, string name, Transform parent, Vector2 anchor, float size)
        {
            if (current != null || parent == null)
            {
                return current;
            }

            var iconObject = new GameObject(name);
            iconObject.transform.SetParent(parent, false);
            var rect = iconObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = Vector2.zero;
            var image = iconObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private RectTransform EnsureLayerPanel(RectTransform layer, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color color, bool active)
        {
            if (layer != null)
            {
                return layer;
            }

            var layerObject = new GameObject(name);
            layerObject.transform.SetParent(HudParent, false);
            layerObject.transform.SetAsFirstSibling();

            var rect = layerObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var image = layerObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            layerObject.SetActive(active);
            return rect;
        }

        private static void SetLayerVisible(RectTransform layer, bool visible)
        {
            if (layer != null)
            {
                layer.gameObject.SetActive(visible);
            }
        }

        private void NormalizeLayout()
        {
            EnsureScreenLayers();
            MoveTextUnderPortraitRoot(focusText);
            MoveTextUnderPortraitRoot(interactionText);
            MoveTextUnderPortraitRoot(runStateText);
            MoveTextUnderPortraitRoot(resultText);
            ApplyTextRect(focusText, new Vector2(0.06f, 0.965f), new Vector2(0.94f, 0.995f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, SubtitleFontSize, TextAnchor.UpperCenter);
            ApplyTextRect(interactionText, new Vector2(0.06f, 0.89f), new Vector2(0.94f, 0.925f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, TitleFontSize, TextAnchor.MiddleCenter);
            ApplyTextRect(runStateText, new Vector2(0.06f, 0.925f), new Vector2(0.94f, 0.965f), new Vector2(0.5f, 1f), Vector2.zero, Vector2.zero, BodyFontSize, TextAnchor.MiddleCenter);
            ApplyTextRect(resultText, new Vector2(0.08f, 0.292f), new Vector2(0.92f, 0.345f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, ResultFontSize, TextAnchor.MiddleCenter);
            ApplyRouteHeaderRect();
            if (resultText != null)
            {
                resultText.lineSpacing = DenseLineSpacing;
            }

            ApplyResultIconStripRect();
        }

        private void MoveTextUnderPortraitRoot(Text text)
        {
            if (text != null && portraitRoot != null && text.transform.parent != portraitRoot)
            {
                text.transform.SetParent(portraitRoot, false);
            }
        }

        private static void ApplyTextRect(Text text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment)
        {
            if (text == null)
            {
                return;
            }

            var rect = text.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = anchorMin;
                rect.anchorMax = anchorMax;
                rect.pivot = pivot;
                rect.anchoredPosition = position;
                rect.sizeDelta = size;
            }

            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(18, fontSize - 8);
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
            text.raycastTarget = false;
        }

        private static string ResolveButtonLabel(Button button)
        {
            var label = button == null ? null : button.GetComponentInChildren<Text>();
            return label == null ? string.Empty : label.text;
        }

        private static RectTransform CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);
            var rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static void SetButtonVisible(Button button, bool visible)
        {
            if (button != null)
            {
                button.gameObject.SetActive(visible);
            }
        }

        private static void ApplyRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private Text CreateHudText(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment, Color color)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(HudParent, false);

            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var text = textObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(18, fontSize - 8);
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
            text.raycastTarget = false;
            text.color = color;
            return text;
        }

        private static void SetButtonLabel(Button button, string labelText)
        {
            if (button == null)
            {
                return;
            }

            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.raycastTarget = false;
                label.text = labelText;
            }
        }

        private void SetStaticIcon(Image target, string key)
        {
            if (target == null)
            {
                return;
            }

            target.sprite = ResolveIcon(key);
            target.color = target.sprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
            target.preserveAspect = true;
        }

        private Sprite ResolveIcon(string key)
        {
            return presentationData != null && presentationData.TryGetIcon(key, out var icon) ? icon : null;
        }

        private static void SetImageVisible(Image target, bool visible)
        {
            if (target != null)
            {
                target.gameObject.SetActive(visible && target.sprite != null);
            }
        }

        private static string BuildBar(int value, int max)
        {
            if (max <= 0)
            {
                return "[-----]";
            }

            var filled = Mathf.Clamp(Mathf.CeilToInt((float)value / max * 5f), 0, 5);
            return "[" + new string('#', filled) + new string('-', 5 - filled) + "]";
        }

        private static float Ratio(int value, int max)
        {
            return max <= 0 ? 0f : Mathf.Clamp01((float)value / max);
        }

        private static Font ResolveFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static string FormatDelta(int amount)
        {
            return amount > 0 ? "+" + amount : amount.ToString();
        }

        private static void EnsureEventSystem()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            var inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            if (Application.isPlaying)
            {
                inputModule.AssignDefaultActions();
            }
        }
    }
}
