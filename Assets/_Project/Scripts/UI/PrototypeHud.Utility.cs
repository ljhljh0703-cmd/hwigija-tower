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
        private void EnsureUtilityUi()
        {
            EnsureScreenLayers();
            utilityStatusButton = EnsureUtilityButton(utilityStatusButton, "Utility Button Status", "상태", new Vector2(0.74f, 0.50f), () => ToggleUtilityPanel("status"));
            utilityMapButton = EnsureUtilityButton(utilityMapButton, "Utility Button Map", "지도", new Vector2(0.84f, 0.50f), OpenUtilityMap);
            utilityLoadoutButton = EnsureUtilityButton(utilityLoadoutButton, "Utility Button Loadout", "장비", new Vector2(0.94f, 0.50f), () => ToggleUtilityPanel("equipment"));
            SetButtonLabel(utilityLoadoutButton, "장비");

            if (utilityPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Utility Panel");
            panelObject.transform.SetParent(HudParent, false);
            utilityPanel = panelObject.AddComponent<RectTransform>();
            utilityPanel.anchorMin = new Vector2(0.10f, 0.16f);
            utilityPanel.anchorMax = new Vector2(0.90f, 0.86f);
            utilityPanel.offsetMin = Vector2.zero;
            utilityPanel.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.018f, 0.022f, 0.026f, 0.97f);
            image.raycastTarget = false;

            utilityPlayerTabButton = CreateUtilityTabButton(utilityPanel, "Utility Player Tab", "플레이어", new Vector2(0.10f, 0.88f), () =>
            {
                _utilityCharacterMode = "player";
                RefreshUtilityPanel(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot());
            });
            utilityMataiosTabButton = CreateUtilityTabButton(utilityPanel, "Utility Mataios Tab", "마타이오스", new Vector2(0.36f, 0.88f), () =>
            {
                _utilityCharacterMode = "mataios";
                RefreshUtilityPanel(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot());
            });
            utilityPortraitImage = CreateCombatImage(utilityPanel, "Utility Portrait", new Vector2(0.08f, 0.30f), new Vector2(0.46f, 0.78f));
            utilityText = CreateCombatChildText(utilityPanel, "Utility Panel Text", new Vector2(0.50f, 0.18f), new Vector2(0.94f, 0.80f), 25, TextAnchor.MiddleLeft);
            utilityText.color = new Color(0.88f, 0.94f, 0.95f, 1f);
            utilityPanel.gameObject.SetActive(false);
        }

        private Button CreateUtilityTabButton(Transform parent, string name, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(220f, 58f);
            rect.anchoredPosition = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.10f, 0.14f, 0.17f, 0.96f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            var text = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 24, TextAnchor.MiddleCenter);
            text.text = label;
            return button;
        }

        private Button EnsureUtilityButton(Button current, string name, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
        {
            if (current != null)
            {
                return current;
            }

            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(topStatusLayer == null ? HudParent : topStatusLayer, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(86f, 54f);
            rect.anchoredPosition = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.10f, 0.14f, 0.17f, 0.96f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            var labelObject = new GameObject("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            var labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(6f, 3f);
            labelRect.offsetMax = new Vector2(-6f, -3f);

            var text = labelObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = 22;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            text.resizeTextMaxSize = 22;
            text.raycastTarget = false;
            text.color = new Color(0.94f, 0.97f, 0.98f, 1f);
            text.text = label;
            return button;
        }

        private void UpdateUtilityUi(PrototypeRunSnapshot snapshot)
        {
            EnsureUtilityUi();
            var visible = !string.IsNullOrEmpty(snapshot.RunId) && !snapshot.RunCompleted;
            var routeResultLocked = snapshot.StairUnlocked || snapshot.RunClear || snapshot.RunFailed;
            SetButtonVisible(utilityStatusButton, visible && !routeResultLocked);
            SetButtonVisible(utilityMapButton, visible && !routeResultLocked);
            SetButtonVisible(utilityLoadoutButton, visible);
            var mapInteractable = visible &&
                !routeResultLocked &&
                snapshot.HasFloorMap &&
                !snapshot.HasSelectedMapNode &&
                !snapshot.IsInCombat &&
                !snapshot.StairUnlocked &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive;
            if (utilityMapButton != null)
            {
                utilityMapButton.interactable = mapInteractable;
            }

            if (!visible || (routeResultLocked && (_utilityMode == "status" || _utilityMode == "map")))
            {
                HideUtilityPanel();
                return;
            }

            RefreshUtilityPanel(snapshot);
        }

        private void ToggleUtilityPanel(string mode)
        {
            _utilityMode = _utilityMode == mode ? string.Empty : mode;
            RefreshUtilityPanel(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot());
        }

        private void OpenUtilityMap()
        {
            if (_roomController == null)
            {
                return;
            }

            if (_roomController.PreRunPlaceholderPending)
            {
                _roomController.ConfirmPreRunPlaceholder();
                HidePreRunPlaceholder();
            }

            var snapshot = _roomController.GetSnapshot();
            if (!snapshot.IsInCombat &&
                !snapshot.RunCompleted &&
                !snapshot.StairUnlocked &&
                !snapshot.HasSelectedMapNode &&
                snapshot.HasFloorMap)
            {
                HideUtilityPanel();
                ClearChoices();
                ShowMapChoices(_roomController.GetFloorMapNodes(), mapNodeId =>
                {
                    var selected = _roomController.SelectMapNode(mapNodeId);
                    OpenSelectedRouteStep(selected);
                });
                ShowRunState(_roomController.GetSnapshot());
                return;
            }

            _utilityMode = "map";
            RefreshUtilityPanel(snapshot);
        }

        private void RefreshUtilityPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureUtilityUi();
            if (utilityPanel == null || utilityText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_utilityMode))
            {
                utilityPanel.gameObject.SetActive(false);
                return;
            }

            utilityText.text = _utilityMode switch
            {
                "status" => BuildUtilityStatus(snapshot),
                "map" => BuildUtilityMapSummary(snapshot),
                "equipment" => BuildUtilityEquipment(snapshot),
                _ => string.Empty
            };
            utilityPanel.gameObject.SetActive(!string.IsNullOrEmpty(utilityText.text));
            RefreshUtilityPortrait(snapshot);
            RefreshUtilityTabs();
        }

        private void HideUtilityPanel()
        {
            _utilityMode = string.Empty;
            if (utilityPanel != null)
            {
                utilityPanel.gameObject.SetActive(false);
            }
        }

        private string BuildUtilityStatus(PrototypeRunSnapshot snapshot)
        {
            if (_utilityCharacterMode == "mataios")
            {
                return "마타이오스\nHP " + BuildMataiosHp(snapshot) +
                    "\nATK " + BuildMataiosAttack(snapshot) +
                    "\n버프 " + BuildMataiosBuffLine(snapshot) +
                    "\n스킬 지원 / 대화";
            }

            return "플레이어\nHP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp +
                "\nATK " + snapshot.PlayerAttack +
                "\n성장 Lv " + snapshot.CombatLevel + " XP " + snapshot.CombatXp + "/" + snapshot.CombatXpToNextLevel +
                "\n정신 " + snapshot.Mental +
                "\n스킬 " + BuildOwnedSkillLine() +
                "\n버프 " + BuildPlayerBuffChipLine(snapshot) +
                "\n" + snapshot.CombatBuildSummary;
        }

        private static string BuildUtilityMapSummary(PrototypeRunSnapshot snapshot)
        {
            var selectable = 0;
            var completed = 0;
            for (var i = 0; i < snapshot.FloorMapNodes.Length; i++)
            {
                if (snapshot.FloorMapNodes[i].Selectable)
                {
                    selectable++;
                }

                if (snapshot.FloorMapNodes[i].Completed)
                {
                    completed++;
                }
            }

            return "지도\nFloor " + snapshot.CurrentFloor +
                "\n선택 가능 " + selectable + " | 완료 " + completed +
                "\n전투 중에는 요약만 표시";
        }

        private string BuildUtilityEquipment(PrototypeRunSnapshot snapshot)
        {
            return "장비\n" + BuildOwnedItemLine() +
                "\n\n능력\n" + BuildOwnedSkillLine();
        }

        private void RefreshUtilityPortrait(PrototypeRunSnapshot snapshot)
        {
            if (utilityPortraitImage == null)
            {
                return;
            }

            utilityPortraitImage.sprite = _utilityMode == "equipment"
                ? presentationData == null ? null : ResolveIcon("item.field_bandage")
                : _utilityCharacterMode == "mataios"
                    ? presentationData == null ? null : presentationData.CombatMataiosPortrait
                    : presentationData == null ? null : presentationData.DefaultPlayerPortrait;
            utilityPortraitImage.color = utilityPortraitImage.sprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
            utilityPortraitImage.gameObject.SetActive(utilityPortraitImage.sprite != null);
        }

        private void RefreshUtilityTabs()
        {
            var showCharacterTabs = _utilityMode == "status";
            SetButtonVisible(utilityPlayerTabButton, showCharacterTabs);
            SetButtonVisible(utilityMataiosTabButton, showCharacterTabs);
            TintUtilityTab(utilityPlayerTabButton, _utilityCharacterMode == "player");
            TintUtilityTab(utilityMataiosTabButton, _utilityCharacterMode == "mataios");
        }

        private static void TintUtilityTab(Button button, bool selected)
        {
            if (button != null && button.targetGraphic is Image image)
            {
                image.color = selected
                    ? new Color(0.28f, 0.20f, 0.10f, 0.98f)
                    : new Color(0.10f, 0.14f, 0.17f, 0.96f);
            }
        }
    }
}
