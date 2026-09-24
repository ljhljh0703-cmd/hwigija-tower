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
        private void ShowPreRunPlaceholder()
        {
            EnsurePreRunPlaceholderLayer();
            if (preRunPlaceholderLayer == null)
            {
                return;
            }

            ClearChoices();
            HideRestInteractionPanel();
            HideEventCutsceneLayout();
            HideMerchantPresentation();
            HideNpcSpotlight();
            HideSkillPicker();
            HideCombatItemInspect();
            EnsureScreenLayers();
            SetLayerVisible(nodeMapLayer, false);
            SetLayerVisible(actionLayer, false);
            SetLayerVisible(visualLayer, false);
            SetLayerVisible(npcReactionLayer, false);
            SetLayerVisible(resultLayer, false);
            SetLayerVisible(endingLayer, false);
            if (combatPanel != null)
            {
                combatPanel.gameObject.SetActive(false);
            }

            preRunPlaceholderLayer.gameObject.SetActive(true);
        }

        private void HidePreRunPlaceholder()
        {
            if (preRunPlaceholderLayer != null)
            {
                preRunPlaceholderLayer.gameObject.SetActive(false);
            }
        }

        private void EnsurePreRunPlaceholderLayer()
        {
            if (preRunPlaceholderLayer != null)
            {
                return;
            }

            var layerObject = new GameObject("Pre Run Placeholder Stepper");
            layerObject.transform.SetParent(HudParent, false);
            preRunPlaceholderLayer = layerObject.AddComponent<RectTransform>();
            preRunPlaceholderLayer.anchorMin = Vector2.zero;
            preRunPlaceholderLayer.anchorMax = Vector2.one;
            preRunPlaceholderLayer.offsetMin = Vector2.zero;
            preRunPlaceholderLayer.offsetMax = Vector2.zero;

            var background = layerObject.AddComponent<Image>();
            background.color = new Color(0.014f, 0.019f, 0.026f, 0.98f);
            background.raycastTarget = true;

            var title = CreateHudText("Pre Run Title", new Vector2(0.08f, 0.83f), new Vector2(0.92f, 0.90f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, TitleFontSize, TextAnchor.MiddleCenter, PrimaryTextColor);
            title.transform.SetParent(layerObject.transform, false);
            title.text = "출발 준비";

            CreatePlaceholderFrame(layerObject.transform, "Pre Run Art Slot", new Vector2(0.10f, 0.41f), new Vector2(0.90f, 0.79f), "이미지 슬롯");
            CreatePlaceholderFrame(layerObject.transform, "Pre Run Memory Slot", new Vector2(0.10f, 0.22f), new Vector2(0.48f, 0.36f), "기억 계승\n비활성");
            CreatePlaceholderFrame(layerObject.transform, "Pre Run Equipment Slot", new Vector2(0.52f, 0.22f), new Vector2(0.90f, 0.36f), "장비 선택\n비활성");

            var ctaObject = new GameObject("Pre Run Confirm Button");
            ctaObject.transform.SetParent(layerObject.transform, false);
            var rect = ctaObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.18f, 0.08f);
            rect.anchorMax = new Vector2(0.82f, 0.16f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = ctaObject.AddComponent<Image>();
            image.color = new Color(0.18f, 0.25f, 0.29f, 0.98f);
            var button = ctaObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(ConfirmPreRunPlaceholder);
            var label = CreateHudText("Pre Run Confirm Label", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, ButtonFontSize, TextAnchor.MiddleCenter, PrimaryTextColor);
            label.transform.SetParent(ctaObject.transform, false);
            label.text = "시작";
        }

        private void CreatePlaceholderFrame(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string labelText)
        {
            var frameObject = new GameObject(name);
            frameObject.transform.SetParent(parent, false);
            var rect = frameObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = frameObject.AddComponent<Image>();
            image.color = new Color(0.045f, 0.055f, 0.064f, 0.90f);
            image.raycastTarget = false;
            var label = CreateHudText(name + " Label", new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.92f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, CaptionFontSize, TextAnchor.MiddleCenter, new Color(0.63f, 0.68f, 0.70f, 1f));
            label.transform.SetParent(frameObject.transform, false);
            label.text = labelText;
        }

        private void ConfirmPreRunPlaceholder()
        {
            _roomController?.ConfirmPreRunPlaceholder();
            HidePreRunPlaceholder();
            if (_roomController == null)
            {
                return;
            }

            var snapshot = _roomController.GetSnapshot();
            if (snapshot.HasFloorMap && !snapshot.RunCompleted)
            {
                ShowMapChoices(_roomController.GetFloorMapNodes(), mapNodeId =>
                {
                    var selected = _roomController.SelectMapNode(mapNodeId);
                    OpenSelectedRouteStep(selected);
                });
            }

            ShowRunState(_roomController.GetSnapshot());
        }
    }
}
