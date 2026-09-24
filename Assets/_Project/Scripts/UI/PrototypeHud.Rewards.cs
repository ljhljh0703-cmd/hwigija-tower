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
        private void EnsureBossRewardPanel()
        {
            if (bossRewardPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Boss Reward Popup");
            panelObject.transform.SetParent(HudParent, false);
            bossRewardPanel = panelObject.AddComponent<RectTransform>();
            bossRewardPanel.anchorMin = new Vector2(0.10f, 0.30f);
            bossRewardPanel.anchorMax = new Vector2(0.90f, 0.72f);
            bossRewardPanel.offsetMin = Vector2.zero;
            bossRewardPanel.offsetMax = Vector2.zero;

            var background = panelObject.AddComponent<Image>();
            background.color = new Color(0.025f, 0.032f, 0.040f, 0.98f);
            background.raycastTarget = false;

            bossRewardTitleText = CreateCombatChildText(panelObject.transform, "Boss Reward Title", new Vector2(0.08f, 0.76f), new Vector2(0.92f, 0.94f), 34, TextAnchor.MiddleCenter);
            bossRewardTitleText.text = string.Empty;

            bossRewardGoldIconImage = CreateCombatImage(panelObject.transform, "Boss Reward Gold Icon", new Vector2(0.12f, 0.48f), new Vector2(0.25f, 0.66f));
            bossRewardGoldText = CreateCombatChildText(panelObject.transform, "Boss Reward Gold Text", new Vector2(0.28f, 0.48f), new Vector2(0.86f, 0.66f), 30, TextAnchor.MiddleLeft);

            bossRewardAffinityIconImage = CreateCombatImage(panelObject.transform, "Boss Reward Affinity Icon", new Vector2(0.12f, 0.28f), new Vector2(0.25f, 0.46f));
            bossRewardAffinityText = CreateCombatChildText(panelObject.transform, "Boss Reward Affinity Text", new Vector2(0.28f, 0.28f), new Vector2(0.86f, 0.46f), 30, TextAnchor.MiddleLeft);

            var buttonObject = new GameObject("Boss Reward Next Floor Button");
            buttonObject.transform.SetParent(panelObject.transform, false);
            var buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.20f, 0.07f);
            buttonRect.anchorMax = new Vector2(0.80f, 0.22f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            var buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.15f, 0.22f, 0.26f, 0.98f);
            bossRewardNextFloorButton = buttonObject.AddComponent<Button>();
            bossRewardNextFloorButton.targetGraphic = buttonImage;
            bossRewardNextFloorButton.onClick.AddListener(ResolveNextFloor);
            var label = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 29, TextAnchor.MiddleCenter);
            label.text = "다음 층";

            bossRewardPanel.gameObject.SetActive(false);
        }

        private void UpdateBossRewardPopup(PrototypeRunSnapshot snapshot)
        {
            var visible = ShouldShowBossRewardPopup(snapshot);
            if (!visible)
            {
                HideBossRewardPopup();
                return;
            }

            EnsureBossRewardPanel();
            if (bossRewardPanel == null)
            {
                return;
            }

            bossRewardPanel.gameObject.SetActive(true);
            bossRewardPanel.transform.SetAsLastSibling();
            if (bossRewardTitleText != null)
            {
                bossRewardTitleText.text = snapshot.RunClear ? "최종 보스 격파" : "보스 격파";
            }

            SetBossRewardIcon(bossRewardGoldIconImage, "resource.gold", "G");
            SetBossRewardIcon(bossRewardAffinityIconImage, "resource.affinity", "신");
            if (bossRewardGoldText != null)
            {
                bossRewardGoldText.text = "Gold " + ExtractBossRewardValue(_lastResultMessage, "gold reward ", snapshot.LastCombatGoldReward);
            }

            if (bossRewardAffinityText != null)
            {
                bossRewardAffinityText.text = "신뢰 " + ExtractBossRewardValue(_lastResultMessage, "affinity ", snapshot.LastCombatAffinityDelta);
            }

            var nextFloorVisible = snapshot.StairUnlocked && !snapshot.RunCompleted;
            if (bossRewardNextFloorButton != null)
            {
                bossRewardNextFloorButton.gameObject.SetActive(nextFloorVisible);
                bossRewardNextFloorButton.interactable = nextFloorVisible && _roomController != null;
            }
        }

        private void HideBossRewardPopup()
        {
            if (bossRewardPanel != null)
            {
                bossRewardPanel.gameObject.SetActive(false);
            }
        }

        private void EnsureLevelRewardPanel()
        {
            if (levelRewardPanel != null)
            {
                return;
            }

            var panelObject = new GameObject("Level Reward Popup");
            panelObject.transform.SetParent(HudParent, false);
            levelRewardPanel = panelObject.AddComponent<RectTransform>();
            levelRewardPanel.anchorMin = new Vector2(0.09f, 0.24f);
            levelRewardPanel.anchorMax = new Vector2(0.91f, 0.76f);
            levelRewardPanel.offsetMin = Vector2.zero;
            levelRewardPanel.offsetMax = Vector2.zero;

            var background = panelObject.AddComponent<Image>();
            background.color = new Color(0.026f, 0.034f, 0.040f, 0.98f);
            background.raycastTarget = true;

            levelRewardTitleText = CreateCombatChildText(panelObject.transform, "Level Reward Title", new Vector2(0.08f, 0.80f), new Vector2(0.92f, 0.95f), 34, TextAnchor.MiddleCenter);
            levelRewardBodyText = CreateCombatChildText(panelObject.transform, "Level Reward Body", new Vector2(0.10f, 0.62f), new Vector2(0.90f, 0.78f), 25, TextAnchor.MiddleCenter);
            levelRewardAttackButton = CreateLevelRewardButton(panelObject.transform, "Level Reward Attack Button", new Vector2(0.12f, 0.43f), new Vector2(0.88f, 0.56f), "공격 단련\n공격력 +1 / 마타이오스 지원 +1", PrototypeRunState.LevelRewardAttackId);
            levelRewardMaxHpButton = CreateLevelRewardButton(panelObject.transform, "Level Reward Max HP Button", new Vector2(0.12f, 0.27f), new Vector2(0.88f, 0.40f), "생존 단련\n최대 HP +4 / 마타이오스 HP +3", PrototypeRunState.LevelRewardMaxHpId);
            levelRewardSkillButton = CreateLevelRewardButton(panelObject.transform, "Level Reward Skill Button", new Vector2(0.12f, 0.11f), new Vector2(0.88f, 0.24f), "Skill CD -1", PrototypeRunState.LevelRewardSkillCooldownId);
            levelRewardPanel.gameObject.SetActive(false);
        }

        private Button CreateLevelRewardButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, string labelText, string rewardId)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.13f, 0.18f, 0.21f, 0.98f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => ResolveLevelReward(rewardId));
            var label = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 27, TextAnchor.MiddleCenter);
            label.text = labelText;
            return button;
        }

        private void UpdateLevelRewardPopup(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.LevelUpRewardPending || ShouldShowCombatDefeatFeedback(snapshot))
            {
                HideLevelRewardPopup();
                return;
            }

            EnsureLevelRewardPanel();
            if (levelRewardPanel == null)
            {
                return;
            }

            levelRewardPanel.gameObject.SetActive(true);
            levelRewardPanel.transform.SetAsLastSibling();
            if (levelRewardTitleText != null)
            {
                levelRewardTitleText.text = "레벨 상승";
            }

            if (levelRewardBodyText != null)
            {
                levelRewardBodyText.text = "Lv " + snapshot.CombatLevel + "  XP " + snapshot.CombatXp + "/" + snapshot.CombatXpToNextLevel;
            }
        }

        private void HideLevelRewardPopup()
        {
            if (levelRewardPanel != null)
            {
                levelRewardPanel.gameObject.SetActive(false);
            }
        }

        private void ResolveLevelReward(string rewardId)
        {
            if (_roomController == null)
            {
                return;
            }

            _roomController.ResolveLevelReward(rewardId);
            ShowRunState(_roomController.GetSnapshot());
        }

        private bool ShouldShowBossRewardPopup(PrototypeRunSnapshot snapshot)
        {
            return !showRawDebugText &&
                !snapshot.IsInCombat &&
                snapshot.LastCombatEnemyDefeated &&
                !ShouldShowCombatDefeatFeedback(snapshot) &&
                snapshot.StairUnlocked &&
                !snapshot.RunClear &&
                IsBossClearResult(_lastResultMessage);
        }

        private void SetBossRewardIcon(Image image, string iconKey, string fallback)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = ResolveIcon(iconKey);
            image.color = image.sprite == null ? new Color(0.20f, 0.26f, 0.30f, 0.95f) : Color.white;
            image.preserveAspect = true;
            image.gameObject.SetActive(true);
            var fallbackText = image.GetComponentInChildren<Text>();
            if (fallbackText == null)
            {
                fallbackText = CreateCombatChildText(image.transform, "Fallback", Vector2.zero, Vector2.one, 20, TextAnchor.MiddleCenter);
            }

            fallbackText.text = image.sprite == null ? fallback : string.Empty;
        }

        private static string ExtractBossRewardValue(string message, string tokenPrefix, int fallback)
        {
            var token = ExtractTokenResult(message, tokenPrefix);
            if (!string.IsNullOrEmpty(token))
            {
                var space = token.LastIndexOf(' ');
                return space >= 0 ? token.Substring(space + 1).Trim() : token;
            }

            return fallback == 0 ? "+0" : FormatDelta(fallback);
        }
    }
}
