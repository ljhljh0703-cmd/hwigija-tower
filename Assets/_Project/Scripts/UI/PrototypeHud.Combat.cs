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
        private void EnsureCombatPanel()
        {
            if (combatUiController != null)
            {
                return;
            }

            EnsureEventSystem();
            var moduleObject = new GameObject("Combat UI Module", typeof(RectTransform));
            combatUiController = moduleObject.AddComponent<CombatUiController>();
            combatUiController.Initialize(HudParent, ResolveCombatAction, ToggleSkillPicker, ToggleCombatItemInspect);
            combatPanel = combatUiController.Root;
            combatEnemyStage = combatUiController.EnemyPanel;
            combatEnemyImage = combatUiController.EnemyImage;
            combatEnemyTitleText = combatUiController.EnemyTitleText;
            combatEnemyStatusText = combatUiController.EnemyStatusText;
            enemyHpFill = combatUiController.EnemyHpFill;
            combatEnemyDamageNumberText = combatUiController.EnemyDamageText;
            combatMataiosDamageNumberText = combatUiController.MataiosDamageText;
            combatDefeatFeedbackText = combatUiController.DefeatFeedbackText;
            combatLogPanel = combatUiController.CombatLogPanel;
            combatText = combatUiController.CombatLogText;
            combatPartyDock = combatUiController.PartyDock;
            combatPlayerCard = combatUiController.PlayerCard;
            combatMataiosCard = combatUiController.MataiosCard;
            combatPlayerPortraitImage = combatUiController.PlayerPortrait;
            combatMataiosPortraitImage = combatUiController.MataiosPortrait;
            combatPlayerPortraitFrameImage = combatUiController.PlayerPortraitFrame;
            combatMataiosPortraitFrameImage = combatUiController.MataiosPortraitFrame;
            combatPlayerPortraitFallbackText = combatUiController.PlayerPortraitFallbackText;
            combatPlayerCardText = combatUiController.PlayerCardText;
            combatMataiosCardText = combatUiController.MataiosCardText;
            combatPlayerDamageNumberText = combatUiController.PlayerDamageText;
            playerHpFill = combatUiController.PlayerHpFill;
            mataiosHpFill = combatUiController.MataiosHpFill;
            combatTrainingStatusIconImage = combatUiController.TrainingStatusIcon;
            combatBandageStatusIconImage = combatUiController.BandageStatusIcon;
            combatRecallStatusIconImage = combatUiController.RecallStatusIcon;
            attackButton = combatUiController.AttackButton;
            defendButton = combatUiController.DefendButton;
            skillButton = combatUiController.SkillButton;
            attackActionIconImage = combatUiController.AttackIcon;
            defendActionIconImage = combatUiController.DefendIcon;
            skillActionIconImage = combatUiController.SkillIcon;
            combatItemInspectButton = combatUiController.ItemInspectButton;
            ApplyCombatStatusIconSprites();
        }

        private void EnsureSkillPickerPanel()
        {
            if (skillPickerPanel != null || combatPartyDock == null)
            {
                return;
            }

            var panelObject = new GameObject("Combat Skill Picker");
            panelObject.transform.SetParent(combatPartyDock, false);
            skillPickerPanel = panelObject.AddComponent<RectTransform>();
            skillPickerPanel.anchorMin = new Vector2(0.56f, 0.22f);
            skillPickerPanel.anchorMax = new Vector2(0.96f, 0.42f);
            skillPickerPanel.offsetMin = Vector2.zero;
            skillPickerPanel.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.02f, 0.025f, 0.03f, 0.97f);
            image.raycastTarget = false;
            skillPickerPanel.gameObject.SetActive(false);
        }

        private RectTransform CreateCombatPanelRect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
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

        private Text CreateCombatChildText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor alignment)
        {
            var text = CreateHudText(name, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, fontSize, alignment, PrimaryTextColor);
            text.transform.SetParent(parent, false);
            var rect = text.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            text.lineSpacing = DenseLineSpacing;
            return text;
        }

        private Image CreateCombatImage(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);

            var rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = imageObject.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.96f);
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.gameObject.SetActive(false);
            return image;
        }

        private void EnsureCombatItemInspectPanel()
        {
            if (combatItemInspectPanel != null || combatPartyDock == null)
            {
                return;
            }

            var panelObject = new GameObject("Combat Item Inspect Panel");
            panelObject.transform.SetParent(combatPartyDock, false);
            combatItemInspectPanel = panelObject.AddComponent<RectTransform>();
            combatItemInspectPanel.anchorMin = new Vector2(0.50f, 0.35f);
            combatItemInspectPanel.anchorMax = new Vector2(0.96f, 0.96f);
            combatItemInspectPanel.offsetMin = Vector2.zero;
            combatItemInspectPanel.offsetMax = Vector2.zero;
            var image = panelObject.AddComponent<Image>();
            image.color = new Color(0.018f, 0.023f, 0.030f, 0.98f);
            image.raycastTarget = false;
            combatItemInspectText = CreateCombatChildText(panelObject.transform, "Combat Item Inspect Text", new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.76f), 20, TextAnchor.UpperLeft);
            combatItemInspectText.text = string.Empty;
            combatItemInspectCloseButton = CreateCombatItemInspectCloseButton(panelObject.transform);
            combatItemInspectPanel.gameObject.SetActive(false);
        }

        private Button CreateCombatItemInspectCloseButton(Transform parent)
        {
            var buttonObject = new GameObject("Combat Item Inspect Close Button");
            buttonObject.transform.SetParent(parent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.68f, 0.80f);
            rect.anchorMax = new Vector2(0.94f, 0.96f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.14f, 0.18f, 0.22f, 0.96f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(HideCombatItemInspect);

            var label = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 18, TextAnchor.MiddleCenter);
            label.text = showRawDebugText ? "close" : "닫기";
            label.raycastTarget = false;
            return button;
        }

        private void ToggleCombatItemInspect()
        {
            EnsureCombatItemInspectPanel();
            if (combatItemInspectPanel == null)
            {
                return;
            }

            var nextVisible = !combatItemInspectPanel.gameObject.activeSelf;
            if (nextVisible)
            {
                HideSkillPicker();
            }

            combatItemInspectPanel.gameObject.SetActive(nextVisible);
        }

        private void HideCombatItemInspect()
        {
            if (combatItemInspectPanel != null)
            {
                combatItemInspectPanel.gameObject.SetActive(false);
            }
        }

        private void UpdateCombatPanel(PrototypeRunSnapshot snapshot)
        {
            EnsureCombatPanel();
            if (combatPanel == null)
            {
                return;
            }

            StartCombatDefeatFeedbackIfNeeded(snapshot);
            var defeatFeedback = ShouldShowCombatDefeatFeedback(snapshot);
            var hasCombat = snapshot.IsInCombat || defeatFeedback;
            if (hasCombat && combatUiController != null)
            {
                combatUiController.Show(snapshot);
            }
            else if (combatUiController != null)
            {
                combatUiController.Hide();
            }
            if (hasCombat)
            {
                HideEventCutsceneLayout(restoreRouteText: false);
                HideMerchantPresentation();
            }

            combatPanel.gameObject.SetActive(hasCombat);
            if (memoryText != null)
            {
                memoryText.gameObject.SetActive(!hasCombat);
            }

            if (npcPortraitImage != null && hasCombat)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            if (interactionText != null && !showRawDebugText)
            {
                interactionText.gameObject.SetActive(!hasCombat &&
                    !_eventPresentationActive &&
                    !_shopPresentationActive &&
                    !RestInteractionPanelVisible &&
                    !IsMapRouteState(snapshot));
            }

            if (routeText != null && hasCombat && !showRawDebugText)
            {
                routeText.gameObject.SetActive(false);
            }

            if (resultText != null && hasCombat)
            {
                resultText.gameObject.SetActive(false);
            }

            if (!hasCombat || combatText == null)
            {
                HideSkillPicker();
                ResetCombatEnemyFeedbackState();
                ResetCombatPlayerFeedbackState();
                return;
            }

            combatText.text = showRawDebugText
                ? "combat: " + snapshot.LastCombatId + "\n" +
                  "enemy: " + snapshot.LastCombatEnemyId + " | HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp + "\n" +
                  "player HP: " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + " | round " + snapshot.CombatRound + "\n" +
                  "last: " + (string.IsNullOrEmpty(snapshot.LastCombatRoundResult) ? "-" : snapshot.LastCombatRoundResult) + "\n" +
                  "result: " + snapshot.LastCombatResultId + " | gold " + snapshot.LastCombatGoldReward +
                  " | glitch " + FormatDelta(snapshot.LastCombatGlitchDelta) +
                  " | affinity " + FormatDelta(snapshot.LastCombatAffinityDelta) +
                  (snapshot.LastCombatComboDamage > 0 ? " | combo " + snapshot.LastCombatComboDamage : string.Empty)
                : BuildCombatLogRecord(snapshot);

            UpdateCombatVisuals(snapshot);
            UpdatePartyDock(snapshot);
            UpdateCombatActionIcons();
            UpdateCombatItemInspect(snapshot);
            UpdateCommandReplacementFlow(snapshot);

            var canAct = snapshot.IsInCombat &&
                !defeatFeedback &&
                !snapshot.LevelUpRewardPending &&
                _roomController != null &&
                !IsCutscenePlaying() &&
                _combatIntroTimer <= 0f;
            var recommendedAction = ResolveRecommendedCombatAction(snapshot);
            if (attackButton != null)
            {
                var preview = _roomController == null
                    ? new CombatActionPreview(CombatAction.Attack, "공격", string.Empty, false)
                    : _roomController.BuildCombatActionPreview(CombatAction.Attack);
                attackButton.gameObject.SetActive(snapshot.IsInCombat && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId));
                attackButton.interactable = canAct && attackButton.gameObject.activeSelf;
                SetCombatActionButtonPreview(attackButton, preview);
                ApplyCombatActionButtonVisualState(attackButton, recommendedAction == CombatAction.Attack && preview.Usable, attackButton.interactable);
            }

            if (defendButton != null)
            {
                var preview = _roomController == null
                    ? new CombatActionPreview(CombatAction.Defend, "방어", string.Empty, false)
                    : _roomController.BuildCombatActionPreview(CombatAction.Defend);
                defendButton.gameObject.SetActive(snapshot.IsInCombat && snapshot.IsCommandEquipped(PrototypeRunState.CommandDefendId));
                defendButton.interactable = canAct && defendButton.gameObject.activeSelf;
                SetCombatActionButtonPreview(defendButton, preview);
                ApplyCombatActionButtonVisualState(defendButton, recommendedAction == CombatAction.Defend && preview.Usable, defendButton.interactable);
            }

            if (skillButton != null)
            {
                var preview = _roomController == null
                    ? new CombatActionPreview(CombatAction.Skill, "스킬", "조건 부족", false)
                    : _roomController.BuildCombatActionPreview(CombatAction.Skill);
                skillButton.gameObject.SetActive(snapshot.IsInCombat);
                skillButton.interactable = canAct && skillButton.gameObject.activeSelf && preview.Usable;
                SetCombatActionButtonPreview(skillButton, preview);
                ApplyCombatActionButtonVisualState(skillButton, recommendedAction == CombatAction.Skill && preview.Usable, skillButton.interactable);
            }
        }

        private void EnsureCombatIntroOverlay()
        {
            if (combatIntroOverlay != null || combatPanel == null)
            {
                return;
            }

            var overlayObject = new GameObject("Combat Intro Overlay");
            overlayObject.transform.SetParent(combatPanel, false);
            combatIntroOverlay = overlayObject.AddComponent<RectTransform>();
            combatIntroOverlay.anchorMin = Vector2.zero;
            combatIntroOverlay.anchorMax = Vector2.one;
            combatIntroOverlay.offsetMin = Vector2.zero;
            combatIntroOverlay.offsetMax = Vector2.zero;

            var image = overlayObject.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.72f);
            image.raycastTarget = true;

            var label = CreateCombatChildText(overlayObject.transform, "Combat Intro Label", new Vector2(0.18f, 0.44f), new Vector2(0.82f, 0.56f), 34, TextAnchor.MiddleCenter);
            label.text = string.Empty;
            overlayObject.SetActive(false);
        }

        private static void SetCombatActionButtonPreview(Button button, CombatActionPreview preview)
        {
            if (button == null)
            {
                return;
            }

            var label = button.GetComponentInChildren<Text>();
            if (label == null)
            {
                return;
            }

            var actionLabel = string.IsNullOrEmpty(preview.Label)
                ? PublicCombatActionName(preview.Action.ToString())
                : preview.Label;
            var previewText = preview.PreviewText;
            if (preview.Action == CombatAction.Skill && !preview.Usable && string.IsNullOrEmpty(previewText))
            {
                previewText = "조건 부족";
            }

            label.text = string.IsNullOrEmpty(actionLabel)
                ? string.Empty
                : string.IsNullOrEmpty(previewText) ? actionLabel : actionLabel + "\n" + previewText;
            var hasPreviewText = !string.IsNullOrEmpty(previewText);
            label.fontSize = hasPreviewText ? 19 : 28;
            label.resizeTextMinSize = hasPreviewText ? 13 : 22;
            label.resizeTextMaxSize = hasPreviewText ? 19 : 28;
            label.lineSpacing = 0.92f;
        }

        private static void ApplyCombatActionButtonVisualState(Button button, bool recommended, bool usable)
        {
            if (button == null)
            {
                return;
            }

            var image = button.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            if (!button.gameObject.activeSelf)
            {
                return;
            }

            if (!usable)
            {
                image.color = new Color(0.07f, 0.08f, 0.10f, 0.90f);
                return;
            }

            image.color = recommended
                ? new Color(0.18f, 0.34f, 0.30f, 0.99f)
                : new Color(0.12f, 0.17f, 0.20f, 0.98f);
        }

        private void UpdateCombatVisuals(PrototypeRunSnapshot snapshot)
        {
            var slot = ResolveCurrentPresentationSlot(snapshot);
            var enemySlot = ResolveEnemyPresentationSlot(snapshot.LastCombatEnemyId);
            if (combatEnemyImage != null)
            {
                combatEnemyImage.sprite = enemySlot != null && enemySlot.EnemySprite != null
                    ? enemySlot.EnemySprite
                    : slot == null ? null : slot.EnemySprite;
                combatEnemyImage.gameObject.SetActive(combatEnemyImage.sprite != null);
                combatEnemyImage.color = ShouldShowCombatDefeatFeedback(snapshot)
                    ? new Color(1f, 1f, 1f, 0.48f)
                    : Color.white;
            }

            if (combatEnemyTitleText != null)
            {
                combatEnemyTitleText.text = PublicEnemyName(snapshot.LastCombatEnemyId) + "  HP " + snapshot.EnemyHp + "/" + snapshot.EnemyMaxHp;
            }

            if (combatEnemyStatusText != null)
            {
                combatEnemyStatusText.text = BuildCombatEnemyStatusChips(snapshot);
                combatEnemyStatusText.gameObject.SetActive(!string.IsNullOrEmpty(combatEnemyStatusText.text));
            }

            if (enemyHpFill != null)
            {
                enemyHpFill.fillAmount = Ratio(snapshot.EnemyHp, snapshot.EnemyMaxHp);
            }

            TrackCombatEnemyFeedback(snapshot);

            if (playerHpFill != null)
            {
                playerHpFill.fillAmount = Ratio(snapshot.PlayerHp, snapshot.PlayerMaxHp);
            }

            if (mataiosHpFill != null)
            {
                mataiosHpFill.fillAmount = Ratio(snapshot.MataiosHp, snapshot.MataiosMaxHp);
            }
        }

        private void TrackCombatEnemyFeedback(PrototypeRunSnapshot snapshot)
        {
            var defeatFeedback = ShouldShowCombatDefeatFeedback(snapshot);
            if ((!snapshot.IsInCombat && !defeatFeedback) || combatEnemyImage == null)
            {
                ResetCombatEnemyFeedbackState();
                return;
            }

            CaptureCombatEnemyFeedbackBase();
            var visualKey = (snapshot.LastCombatId ?? string.Empty) + "|" + (snapshot.LastCombatEnemyId ?? string.Empty);
            if (_lastCombatVisualKey != visualKey)
            {
                _lastCombatVisualKey = visualKey;
                _lastCombatVisualRound = snapshot.CombatRound;
                _lastCombatVisualEnemyHp = snapshot.EnemyHp;
                _lastCombatVisualPlayerHp = snapshot.PlayerHp;
                ResetCombatEnemyFeedbackTransform();
                StartCombatIntro(visualKey, snapshot);
                return;
            }

            if (snapshot.CombatRound == _lastCombatVisualRound)
            {
                return;
            }

            if (_lastCombatVisualEnemyHp >= 0 && snapshot.EnemyHp < _lastCombatVisualEnemyHp)
            {
                var totalDamage = _lastCombatVisualEnemyHp - snapshot.EnemyHp;
                var mataiosDamage = Mathf.Clamp(snapshot.LastMataiosCombatDamage, 0, totalDamage);
                var playerDamage = Mathf.Max(0, totalDamage - mataiosDamage);
                if (playerDamage > 0)
                {
                    ShowCombatDamageNumber(combatEnemyDamageNumberText, "-" + playerDamage, CombatDamageNumberRole.Enemy);
                }

                if (mataiosDamage > 0)
                {
                    ShowCombatDamageNumber(combatMataiosDamageNumberText, "마타이오스 -" + mataiosDamage, CombatDamageNumberRole.Mataios);
                }

                TriggerCombatEnemyHitShake();
            }

            if (_lastCombatVisualPlayerHp >= 0 && snapshot.PlayerHp < _lastCombatVisualPlayerHp)
            {
                ShowCombatDamageNumber(combatPlayerDamageNumberText, "-" + (_lastCombatVisualPlayerHp - snapshot.PlayerHp), CombatDamageNumberRole.Player);
                TriggerCombatPlayerHitFeedback();
            }

            if ((snapshot.LastCombatRoundResult ?? string.Empty).Contains("enemyDamage ", StringComparison.Ordinal))
            {
                TriggerCombatEnemyAttackPulse();
            }

            _lastCombatVisualRound = snapshot.CombatRound;
            _lastCombatVisualEnemyHp = snapshot.EnemyHp;
            _lastCombatVisualPlayerHp = snapshot.PlayerHp;
        }

        private enum CombatDamageNumberRole
        {
            Enemy,
            Mataios,
            Player
        }

        private void ShowCombatDamageNumber(Text text, string value, CombatDamageNumberRole role)
        {
            if (text == null || string.IsNullOrEmpty(value))
            {
                return;
            }

            text.text = value;
            text.gameObject.SetActive(true);
            if (role == CombatDamageNumberRole.Enemy)
            {
                _combatEnemyDamageNumberTimer = CombatDamageNumberDuration;
            }
            else if (role == CombatDamageNumberRole.Mataios)
            {
                _combatMataiosDamageNumberTimer = CombatDamageNumberDuration;
            }
            else
            {
                _combatPlayerDamageNumberTimer = CombatDamageNumberDuration;
            }
        }

        private void TriggerCombatEnemyHitShake()
        {
            CaptureCombatEnemyFeedbackBase();
            _combatEnemyHitShakeTimer = CombatEnemyHitShakeDuration;
        }

        private void TriggerCombatEnemyAttackPulse()
        {
            CaptureCombatEnemyFeedbackBase();
            _combatEnemyAttackPulseTimer = CombatEnemyAttackPulseDuration;
        }

        private void TriggerCombatPlayerHitFeedback()
        {
            CaptureCombatPlayerFeedbackBase();
            _combatPlayerHitShakeTimer = CombatPlayerHitShakeDuration;
            _combatPlayerHpPulseTimer = CombatPlayerHpPulseDuration;
        }

        private void StartCombatIntro(string visualKey, PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.IsInCombat || _combatIntroKey == visualKey)
            {
                return;
            }

            EnsureCombatIntroOverlay();
            _combatIntroKey = visualKey;
            _combatIntroTimer = CombatIntroDuration;
            if (combatIntroOverlay != null)
            {
                combatIntroOverlay.gameObject.SetActive(true);
                combatIntroOverlay.transform.SetAsLastSibling();
            }
        }

        private bool ShouldShowCombatDefeatFeedback(PrototypeRunSnapshot snapshot)
        {
            return !snapshot.IsInCombat &&
                snapshot.LastCombatEnemyDefeated &&
                !string.IsNullOrEmpty(snapshot.LastCombatId) &&
                _combatDefeatFeedbackTimer > 0f &&
                _combatDefeatFeedbackKey == BuildCombatDefeatFeedbackKey(snapshot);
        }

        private string BuildCombatDefeatFeedbackKey(PrototypeRunSnapshot snapshot)
        {
            return (snapshot.LastCombatId ?? string.Empty) + "|" + snapshot.CombatRound + "|" + (snapshot.LastCombatEnemyId ?? string.Empty);
        }

        private void StartCombatDefeatFeedbackIfNeeded(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat || !snapshot.LastCombatEnemyDefeated || string.IsNullOrEmpty(snapshot.LastCombatId))
            {
                return;
            }

            var key = BuildCombatDefeatFeedbackKey(snapshot);
            if (_combatDefeatFeedbackKey == key)
            {
                return;
            }

            _combatDefeatFeedbackKey = key;
            _combatDefeatFeedbackTimer = CombatDefeatFeedbackDuration;
            if (combatDefeatFeedbackText != null)
            {
                combatDefeatFeedbackText.text = IsBossEnemyId(snapshot.LastCombatEnemyId) ? "보스 격파" : "격파";
                combatDefeatFeedbackText.gameObject.SetActive(true);
            }

            TriggerCombatEnemyHitShake();
        }

        private void UpdateCombatEnemyFeedbackAnimation(float deltaTime)
        {
            if (combatEnemyImage == null || (!_combatEnemyFeedbackBaseCaptured && _combatEnemyHitShakeTimer <= 0f && _combatEnemyAttackPulseTimer <= 0f))
            {
                return;
            }

            CaptureCombatEnemyFeedbackBase();
            _combatEnemyHitShakeTimer = Mathf.Max(0f, _combatEnemyHitShakeTimer - Mathf.Max(0f, deltaTime));
            _combatEnemyAttackPulseTimer = Mathf.Max(0f, _combatEnemyAttackPulseTimer - Mathf.Max(0f, deltaTime));

            var rect = combatEnemyImage.rectTransform;
            var shakeOffset = 0f;
            if (_combatEnemyHitShakeTimer > 0f)
            {
                var progress = 1f - _combatEnemyHitShakeTimer / CombatEnemyHitShakeDuration;
                shakeOffset = Mathf.Sin(progress * Mathf.PI * 6f) * CombatEnemyHitShakePixels * (1f - progress);
            }

            var pulseScale = 1f;
            if (_combatEnemyAttackPulseTimer > 0f)
            {
                var progress = 1f - _combatEnemyAttackPulseTimer / CombatEnemyAttackPulseDuration;
                pulseScale = Mathf.Lerp(CombatEnemyAttackPulseScale, 1f, progress);
            }

            rect.anchoredPosition = _combatEnemyImageBasePosition + new Vector2(shakeOffset, 0f);
            rect.localScale = _combatEnemyImageBaseScale * pulseScale;

            if (_combatEnemyHitShakeTimer <= 0f && _combatEnemyAttackPulseTimer <= 0f)
            {
                ResetCombatEnemyFeedbackTransform();
            }
        }

        private void UpdateCombatPlayerFeedbackAnimation(float deltaTime)
        {
            if (combatPlayerCard == null || (!_combatPlayerFeedbackBaseCaptured && _combatPlayerHitShakeTimer <= 0f && _combatPlayerHpPulseTimer <= 0f))
            {
                return;
            }

            CaptureCombatPlayerFeedbackBase();
            _combatPlayerHitShakeTimer = Mathf.Max(0f, _combatPlayerHitShakeTimer - Mathf.Max(0f, deltaTime));
            _combatPlayerHpPulseTimer = Mathf.Max(0f, _combatPlayerHpPulseTimer - Mathf.Max(0f, deltaTime));

            var shakeOffset = 0f;
            if (_combatPlayerHitShakeTimer > 0f)
            {
                var progress = 1f - _combatPlayerHitShakeTimer / CombatPlayerHitShakeDuration;
                shakeOffset = Mathf.Sin(progress * Mathf.PI * 7f) * CombatPlayerHitShakePixels * (1f - progress);
            }

            var pulseScale = 1f;
            if (_combatPlayerHpPulseTimer > 0f)
            {
                var progress = 1f - _combatPlayerHpPulseTimer / CombatPlayerHpPulseDuration;
                pulseScale = Mathf.Lerp(CombatPlayerHpPulseScale, 1f, progress);
            }

            combatPlayerCard.anchoredPosition = _combatPlayerCardBasePosition + new Vector2(shakeOffset, 0f);
            combatPlayerCard.localScale = _combatPlayerCardBaseScale * pulseScale;
            var cardImage = combatPlayerCard.GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.color = _combatPlayerHitShakeTimer > 0f
                    ? new Color(0.24f, 0.08f, 0.08f, 0.98f)
                    : _combatPlayerCardBaseColor;
            }

            if (_combatPlayerHitShakeTimer <= 0f && _combatPlayerHpPulseTimer <= 0f)
            {
                ResetCombatPlayerFeedbackTransform();
            }
        }

        private void CaptureCombatEnemyFeedbackBase()
        {
            if (_combatEnemyFeedbackBaseCaptured || combatEnemyImage == null)
            {
                return;
            }

            var rect = combatEnemyImage.rectTransform;
            _combatEnemyImageBasePosition = rect.anchoredPosition;
            _combatEnemyImageBaseScale = rect.localScale;
            _combatEnemyFeedbackBaseCaptured = true;
        }

        private void ResetCombatEnemyFeedbackTransform()
        {
            if (!_combatEnemyFeedbackBaseCaptured || combatEnemyImage == null)
            {
                return;
            }

            var rect = combatEnemyImage.rectTransform;
            rect.anchoredPosition = _combatEnemyImageBasePosition;
            rect.localScale = _combatEnemyImageBaseScale;
        }

        private void CaptureCombatPlayerFeedbackBase()
        {
            if (_combatPlayerFeedbackBaseCaptured || combatPlayerCard == null)
            {
                return;
            }

            _combatPlayerCardBasePosition = combatPlayerCard.anchoredPosition;
            _combatPlayerCardBaseScale = combatPlayerCard.localScale;
            var image = combatPlayerCard.GetComponent<Image>();
            _combatPlayerCardBaseColor = image == null ? Color.white : image.color;
            _combatPlayerFeedbackBaseCaptured = true;
        }

        private void ResetCombatPlayerFeedbackTransform()
        {
            if (!_combatPlayerFeedbackBaseCaptured || combatPlayerCard == null)
            {
                return;
            }

            combatPlayerCard.anchoredPosition = _combatPlayerCardBasePosition;
            combatPlayerCard.localScale = _combatPlayerCardBaseScale;
            var image = combatPlayerCard.GetComponent<Image>();
            if (image != null)
            {
                image.color = _combatPlayerCardBaseColor;
            }
        }

        private void ResetCombatPlayerFeedbackState()
        {
            ResetCombatPlayerFeedbackTransform();
            _combatPlayerHitShakeTimer = 0f;
            _combatPlayerHpPulseTimer = 0f;
        }

        private void ResetCombatEnemyFeedbackState()
        {
            ResetCombatEnemyFeedbackTransform();
            _lastCombatVisualKey = string.Empty;
            _lastCombatVisualRound = -1;
            _lastCombatVisualEnemyHp = -1;
            _lastCombatVisualPlayerHp = -1;
            _combatEnemyHitShakeTimer = 0f;
            _combatEnemyAttackPulseTimer = 0f;
            _combatEnemyDamageNumberTimer = 0f;
            _combatMataiosDamageNumberTimer = 0f;
            _combatPlayerDamageNumberTimer = 0f;
            if (combatEnemyDamageNumberText != null)
            {
                combatEnemyDamageNumberText.gameObject.SetActive(false);
            }

            if (combatMataiosDamageNumberText != null)
            {
                combatMataiosDamageNumberText.gameObject.SetActive(false);
            }

            if (combatPlayerDamageNumberText != null)
            {
                combatPlayerDamageNumberText.gameObject.SetActive(false);
            }

            if (combatDefeatFeedbackText != null)
            {
                combatDefeatFeedbackText.gameObject.SetActive(false);
            }
        }

        private void UpdateCombatDamageNumberAnimation(float deltaTime)
        {
            var step = Mathf.Max(0f, deltaTime);
            if (_combatEnemyDamageNumberTimer > 0f)
            {
                _combatEnemyDamageNumberTimer = Mathf.Max(0f, _combatEnemyDamageNumberTimer - step);
                if (_combatEnemyDamageNumberTimer <= 0f && combatEnemyDamageNumberText != null)
                {
                    combatEnemyDamageNumberText.gameObject.SetActive(false);
                }
            }

            if (_combatMataiosDamageNumberTimer > 0f)
            {
                _combatMataiosDamageNumberTimer = Mathf.Max(0f, _combatMataiosDamageNumberTimer - step);
                if (_combatMataiosDamageNumberTimer <= 0f && combatMataiosDamageNumberText != null)
                {
                    combatMataiosDamageNumberText.gameObject.SetActive(false);
                }
            }

            if (_combatPlayerDamageNumberTimer > 0f)
            {
                _combatPlayerDamageNumberTimer = Mathf.Max(0f, _combatPlayerDamageNumberTimer - step);
                if (_combatPlayerDamageNumberTimer <= 0f && combatPlayerDamageNumberText != null)
                {
                    combatPlayerDamageNumberText.gameObject.SetActive(false);
                }
            }
        }

        private void UpdateCombatIntro(float deltaTime)
        {
            if (_combatIntroTimer <= 0f)
            {
                return;
            }

            _combatIntroTimer = Mathf.Max(0f, _combatIntroTimer - Mathf.Max(0f, deltaTime));
            if (combatIntroOverlay != null)
            {
                var image = combatIntroOverlay.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.72f, _combatIntroTimer / CombatIntroDuration));
                }

                combatIntroOverlay.gameObject.SetActive(_combatIntroTimer > 0f);
            }

            if (_combatIntroTimer <= 0f)
            {
                ShowRunState(_lastSnapshot);
            }
        }

        private void UpdateCombatDefeatFeedback(float deltaTime)
        {
            if (_combatDefeatFeedbackTimer <= 0f)
            {
                return;
            }

            _combatDefeatFeedbackTimer = Mathf.Max(0f, _combatDefeatFeedbackTimer - Mathf.Max(0f, deltaTime));
            if (combatDefeatFeedbackText != null)
            {
                combatDefeatFeedbackText.gameObject.SetActive(_combatDefeatFeedbackTimer > 0f);
            }

            if (_combatDefeatFeedbackTimer <= 0f)
            {
                ResetCombatEnemyFeedbackTransform();
                ShowRunState(_lastSnapshot);
            }
        }

        private void UpdatePartyDock(PrototypeRunSnapshot snapshot)
        {
            if (combatPartyDock != null)
            {
                combatPartyDock.gameObject.SetActive(snapshot.IsInCombat);
            }

            if (combatPlayerPortraitImage != null)
            {
                combatPlayerPortraitImage.sprite = presentationData == null ? null : presentationData.DefaultPlayerPortrait;
                combatPlayerPortraitImage.color = combatPlayerPortraitImage.sprite == null
                    ? new Color(0.15f, 0.19f, 0.22f, 1f)
                    : Color.white;
                combatPlayerPortraitImage.gameObject.SetActive(true);
            }

            if (combatPlayerPortraitFallbackText != null)
            {
                combatPlayerPortraitFallbackText.gameObject.SetActive(combatPlayerPortraitImage == null || combatPlayerPortraitImage.sprite == null);
            }

            if (combatMataiosPortraitImage != null)
            {
                var combatMataiosPortrait = presentationData == null ? null : presentationData.CombatMataiosPortrait;
                combatMataiosPortraitImage.sprite = combatMataiosPortrait != null
                    ? combatMataiosPortrait
                    : (npcPortraitImage != null && npcPortraitImage.sprite != null ? npcPortraitImage.sprite : mataiosPortrait);
                combatMataiosPortraitImage.color = Color.white;
                combatMataiosPortraitImage.gameObject.SetActive(combatMataiosPortraitImage.sprite != null);
            }

            ApplyCombatPortraitFrame(combatPlayerPortraitFrameImage);
            ApplyCombatPortraitFrame(combatMataiosPortraitFrameImage);

            if (combatPlayerCardText != null)
            {
                combatPlayerCardText.text = BuildCombatPlayerCardText(snapshot);
            }

            UpdateCombatStatusIcons(snapshot);

            if (combatMataiosCardText != null)
            {
                combatMataiosCardText.text = BuildCombatMataiosCardText(snapshot);
            }
        }

        private void ApplyCombatPortraitFrame(Image frameImage)
        {
            if (frameImage == null)
            {
                return;
            }

            frameImage.sprite = presentationData == null ? null : presentationData.CombatPortraitFrame;
            frameImage.color = Color.white;
            frameImage.gameObject.SetActive(frameImage.sprite != null);
        }

        private void UpdateCombatActionIcons()
        {
            SetCombatActionIcon(attackActionIconImage, CombatAction.Attack, new Color(0.84f, 0.34f, 0.26f, 0.94f));
            SetCombatActionIcon(defendActionIconImage, CombatAction.Defend, new Color(0.38f, 0.58f, 0.82f, 0.94f));
            SetCombatActionIcon(skillActionIconImage, CombatAction.Skill, new Color(0.78f, 0.68f, 0.34f, 0.94f));
        }

        private void ApplyCombatStatusIconSprites()
        {
            SetStaticIcon(combatTrainingStatusIconImage, "status.training");
            SetStaticIcon(combatBandageStatusIconImage, "status.bandage");
            SetStaticIcon(combatRecallStatusIconImage, "status.recall_anchor");
        }

        private void UpdateCombatStatusIcons(PrototypeRunSnapshot snapshot)
        {
            ApplyCombatStatusIconSprites();
            var hasTraining = _roomController != null && _roomController.RunState != null && _roomController.RunState.TrainingBuffActive;
            var hasBandage = _roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0;
            var hasRecall = _roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR");
            SetImageVisible(combatTrainingStatusIconImage, snapshot.IsInCombat && hasTraining);
            SetImageVisible(combatBandageStatusIconImage, snapshot.IsInCombat && hasBandage);
            SetImageVisible(combatRecallStatusIconImage, snapshot.IsInCombat && hasRecall);
        }

        private void UpdateCombatItemInspect(PrototypeRunSnapshot snapshot)
        {
            EnsureCombatItemInspectPanel();
            var visible = snapshot.IsInCombat;
            if (combatItemInspectButton != null)
            {
                combatItemInspectButton.gameObject.SetActive(visible);
                combatItemInspectButton.interactable = visible && snapshot.ItemCount > 0;
            }

            if (combatItemInspectText != null)
            {
                combatItemInspectText.text = BuildCombatItemInspectText();
            }

            if (!visible || snapshot.ItemCount <= 0)
            {
                HideCombatItemInspect();
            }
        }

        private void SetCombatActionIcon(Image target, CombatAction action, Color fallbackColor)
        {
            if (target == null)
            {
                return;
            }

            target.sprite = presentationData != null && presentationData.TryGetCombatActionIcon(action, out var icon) ? icon : null;
            target.color = target.sprite == null ? fallbackColor : Color.white;
            target.gameObject.SetActive(true);
        }

        private void ResolveCombatAction(CombatAction action)
        {
            if (_roomController == null)
            {
                ShowResultMessage("combat unavailable");
                return;
            }

            HideSkillPicker();
            HideCombatItemInspect();
            var resolution = _roomController.ResolveCombatAction(action);
            ShowResult(resolution);
            ShowRunState(_roomController.GetSnapshot());
        }

        private void ToggleSkillPicker()
        {
            EnsureSkillPickerPanel();
            if (skillPickerPanel == null)
            {
                return;
            }

            var visible = !skillPickerPanel.gameObject.activeSelf;
            if (!visible)
            {
                HideSkillPicker();
                return;
            }

            PopulateSkillPicker();
            skillPickerPanel.gameObject.SetActive(true);
        }

        private void HideSkillPicker()
        {
            if (skillPickerPanel != null)
            {
                skillPickerPanel.gameObject.SetActive(false);
            }
        }

        private void PopulateSkillPicker()
        {
            EnsureSkillPickerPanel();
            if (skillPickerPanel == null)
            {
                return;
            }

            for (var i = skillPickerPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(skillPickerPanel.GetChild(i).gameObject);
            }

            var skills = BuildOwnedCombatSkillNames();
            if (skills.Count == 0)
            {
                skills.Add("사용 가능한 스킬 없음");
            }

            for (var i = 0; i < skills.Count; i++)
            {
                var buttonObject = new GameObject("Skill Option " + i);
                buttonObject.transform.SetParent(skillPickerPanel, false);
                var rect = buttonObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(0f, 64f);
                rect.anchoredPosition = new Vector2(0f, -i * 70f);

                var image = buttonObject.AddComponent<Image>();
                image.color = new Color(0.12f, 0.17f, 0.20f, 0.98f);
                var button = buttonObject.AddComponent<Button>();
                button.targetGraphic = image;
                button.interactable = HasAnyCombatSkill();
                button.onClick.AddListener(() => ResolveCombatAction(CombatAction.Skill));

                var text = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 22, TextAnchor.MiddleCenter);
                text.text = skills[i];
            }
        }

        private void UpdateCommandReplacementFlow(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.CommandReplacementPending)
            {
                return;
            }

            EnsureSkillPickerPanel();
            if (skillPickerPanel == null)
            {
                return;
            }

            for (var i = skillPickerPanel.childCount - 1; i >= 0; i--)
            {
                Destroy(skillPickerPanel.GetChild(i).gameObject);
            }

            CreateCommandReplacementLabel(0, "새 command 장착: " + PublicCommandName(snapshot.PendingCommandEquipId));
            for (var i = 0; i < snapshot.EquippedCommandIds.Length; i++)
            {
                var commandId = snapshot.EquippedCommandIds[i];
                CreateCommandReplacementButton(i + 1, PublicCommandName(commandId), () =>
                {
                    var resolution = _roomController == null ? default : _roomController.ReplacePendingCommand(commandId);
                    HideSkillPicker();
                    ShowResult(resolution);
                    if (_roomController != null)
                    {
                        ShowRunState(_roomController.GetSnapshot());
                    }
                });
            }

            CreateCommandReplacementButton(snapshot.EquippedCommandIds.Length + 1, "취소", () =>
            {
                var resolution = _roomController == null ? default : _roomController.CancelPendingCommandReplacement();
                HideSkillPicker();
                ShowResult(resolution);
                if (_roomController != null)
                {
                    ShowRunState(_roomController.GetSnapshot());
                }
            });
            skillPickerPanel.gameObject.SetActive(true);
        }

        private void CreateCommandReplacementLabel(int row, string labelText)
        {
            var labelObject = new GameObject("Command Replacement Label");
            labelObject.transform.SetParent(skillPickerPanel, false);
            var rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 52f);
            rect.anchoredPosition = new Vector2(0f, -row * 56f);
            var text = labelObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = 20;
            text.raycastTarget = false;
            text.color = PrimaryTextColor;
            text.text = labelText;
        }

        private void CreateCommandReplacementButton(int row, string labelText, UnityEngine.Events.UnityAction action)
        {
            var buttonObject = new GameObject("Command Replacement Option");
            buttonObject.transform.SetParent(skillPickerPanel, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 48f);
            rect.anchoredPosition = new Vector2(0f, -row * 52f);
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.17f, 0.20f, 0.98f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var text = CreateCombatChildText(buttonObject.transform, "Label", Vector2.zero, Vector2.one, 20, TextAnchor.MiddleCenter);
            text.text = labelText;
        }

        private static string BuildCombatOutcomeLabel(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                return "전투 중";
            }

            if (snapshot.LastCombatEnemyDefeated)
            {
                return "승리";
            }

            return string.IsNullOrEmpty(snapshot.LastCombatResultId) ? "-" : PublicCombatResultName(snapshot.LastCombatResultId);
        }

        private string BuildCombatLogRecord(PrototypeRunSnapshot snapshot)
        {
            var feedback = BuildCombatFeedback(snapshot.LastCombatRoundResult);
            var detail = BuildCombatLogDetailLine(snapshot);
            var record = string.IsNullOrEmpty(detail)
                ? feedback
                : feedback + " | " + detail;
            return ShortenPublicLine(record, 78);
        }

        private string BuildCombatLogDetailLine(PrototypeRunSnapshot snapshot)
        {
            var highlights = new List<string>();
            var extra = BuildCombatExtraLine(snapshot);
            if (!string.IsNullOrEmpty(extra))
            {
                highlights.Add(extra);
            }

            var playerDamage = ExtractRoundNumber(snapshot.LastCombatRoundResult, "playerDamage ").Trim();
            if (!string.IsNullOrEmpty(playerDamage) && playerDamage != "0")
            {
                highlights.Add("플레이어: " + playerDamage + " 피해");
            }

            var hpChange = ExtractRoundNumber(snapshot.LastCombatRoundResult, "playerHp ").Trim();
            if (!string.IsNullOrEmpty(hpChange) && !IsNoOpDelta(hpChange))
            {
                highlights.Add("HP " + hpChange);
            }

            if (snapshot.LastCombatRoundResult.Contains("scout attack +", StringComparison.Ordinal))
            {
                highlights.Add("정찰: +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "scout attack +").Trim());
            }

            if (snapshot.LastCombatRoundResult.Contains("scout guard ", StringComparison.Ordinal))
            {
                highlights.Add("정찰: 다음 피해 감소 " + ExtractRoundNumber(snapshot.LastCombatRoundResult, "scout guard ").Trim());
            }

            if (snapshot.LastMataiosProtectReduction > 0)
            {
                highlights.Add("마타이오스 보호: 피해 " + snapshot.LastMataiosProtectReduction + " 감소");
            }

            if (snapshot.LastMataiosCombatDamage > 0)
            {
                highlights.Add("마타이오스: " + snapshot.LastMataiosCombatDamage + " 지원 피해");
            }

            if (highlights.Count > 0)
            {
                return JoinCompactChips(highlights, 4);
            }

            if (HasArtsSkill())
            {
                return "번개 방출: 직접 피해";
            }

            return HasScoutSkill(snapshot) ? "정찰: 다음 공격 강화" : "기술 불가: 보유 스킬 필요";
        }

        private static string BuildCombatBuildSummaryLine(PrototypeRunSnapshot snapshot)
        {
            if (!string.IsNullOrEmpty(snapshot.LastGrowthMessage))
            {
                return "보상: " + snapshot.LastGrowthMessage;
            }

            var chips = new List<string>();
            if (snapshot.ScoutAttackReady)
            {
                chips.Add("정찰 공격 준비");
            }

            if (snapshot.ScoutDamageReductionReady)
            {
                chips.Add("정찰 피해감소 준비");
            }

            if (snapshot.LevelAttackBonus > 0)
            {
                chips.Add("ATK +" + snapshot.LevelAttackBonus);
            }

            if (snapshot.LevelMaxHpBonus > 0)
            {
                chips.Add("HP +" + snapshot.LevelMaxHpBonus);
            }

            if (snapshot.SkillCooldownReduction > 0)
            {
                chips.Add("CD -" + snapshot.SkillCooldownReduction);
            }

            return chips.Count == 0 ? string.Empty : "상태: " + JoinCompactChips(chips, 3);
        }

        private static string BuildCombatExtraLine(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.LastCombatComboDamage > 0)
            {
                return "연속 행동: 추가 공격 " + snapshot.LastCombatComboDamage;
            }

            if (snapshot.LastCombatRoundResult.Contains("scout attack ready", StringComparison.Ordinal))
            {
                return "정찰: 다음 공격 강화";
            }

            if (snapshot.LastCombatRoundResult.Contains("scout guard ready", StringComparison.Ordinal))
            {
                return "정찰: 다음 피해 감소 1회";
            }

            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                return "단련 보너스: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "training +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("frenzy break", StringComparison.Ordinal))
            {
                return "광폭 끊김";
            }

            if (snapshot.LastCombatRoundResult.Contains("frenzy ", StringComparison.Ordinal))
            {
                return "광폭 발동: 추가타 " + ExtractRoundNumber(snapshot.LastCombatRoundResult, "frenzy ").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("oil skill +", StringComparison.Ordinal))
            {
                return "등유: 스킬 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "oil skill +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("skill opening +", StringComparison.Ordinal))
            {
                return "빈틈 공략: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "skill opening +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("heavy pressure +", StringComparison.Ordinal))
            {
                return "중압: 피해 +" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "heavy pressure +").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("heavy pressure blocked", StringComparison.Ordinal))
            {
                return "방어: 중압 차단";
            }

            if (snapshot.LastCombatRoundResult.Contains("first hit guard ", StringComparison.Ordinal))
            {
                return "찢어진 부적: 피해 -" + ExtractRoundNumber(snapshot.LastCombatRoundResult, "first hit guard ").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("bandage ", StringComparison.Ordinal))
            {
                return "붕대: HP 회복 " + ExtractRoundNumber(snapshot.LastCombatRoundResult, "bandage ").Trim();
            }

            if (snapshot.LastCombatRoundResult.Contains("recall ready", StringComparison.Ordinal))
            {
                return "회상 닻: 패배 1회 방지";
            }

            return string.Empty;
        }

        private static string BuildCombatEnemyStatusChips(PrototypeRunSnapshot snapshot)
        {
            var chips = new List<string>();
            if (snapshot.EnemyAttack > 0)
            {
                chips.Add("ATK " + snapshot.EnemyAttack);
            }

            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            if (result.Contains("poison ", StringComparison.Ordinal))
            {
                chips.Add("중독");
            }

            if (result.Contains("heavy pressure", StringComparison.Ordinal))
            {
                chips.Add("중압");
            }

            if (result.Contains("skill opening", StringComparison.Ordinal))
            {
                chips.Add("빈틈");
            }

            if (chips.Count > 4)
            {
                var overflow = chips.Count - 3;
                chips.RemoveRange(3, overflow);
                chips.Add("+" + overflow);
            }

            return string.Join("  ", chips);
        }

        private string BuildCombatItemInspectText()
        {
            if (_roomController == null || _roomController.RunState == null)
            {
                return "아이템 없음";
            }

            var lines = new List<string>();
            AddCombatInspectItemLine(lines, "ITEM_FIELD_BANDAGE", "붕대", "전투 시작 HP 회복 +4 / 최대 HP +2");
            AddCombatInspectItemLine(lines, "ITEM_LANTERN_OIL", "등유", "스킬 피해 +2");
            AddCombatInspectItemLine(lines, "ITEM_TORN_CHARM", "찢어진 부적", "첫 피격 피해 -2");
            return lines.Count == 0 ? "아이템 없음" : string.Join("\n", lines);
        }

        private void AddCombatInspectItemLine(List<string> lines, string itemRef, string label, string summary)
        {
            var count = _roomController == null || _roomController.RunState == null ? 0 : _roomController.RunState.GetItemCount(itemRef);
            if (count <= 0)
            {
                return;
            }

            lines.Add(label + " x" + count + " - " + summary);
        }

        private static string BuildCombatIntentLine(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.CombatRound <= 1)
            {
                return "의도: 공격 준비";
            }

            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            if (result.Contains("action Defend", StringComparison.Ordinal))
            {
                return "의도: 압박 지속";
            }

            if (result.Contains("action Skill", StringComparison.Ordinal) || result.Contains("scout attack +", StringComparison.Ordinal))
            {
                return "의도: 빈틈 노출";
            }

            return "의도: 반격 준비";
        }

        private static string BuildCombatDecisionLine(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.IsInCombat)
            {
                return "판단: 전투 결과 확인";
            }

            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            if (snapshot.ScoutAttackReady && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return "판단: 공격으로 정찰 보너스 발동";
            }

            if (HasSkillOpeningCue(snapshot) && HasCombatSkillEquipped(snapshot))
            {
                return result.Contains("skill opening missed", StringComparison.Ordinal)
                    ? "판단: 빈틈을 놓침 - 다음엔 스킬"
                    : "판단: 빈틈 - 스킬로 추가 피해";
            }

            if (HasHeavyPressureCue(snapshot))
            {
                return result.Contains("heavy pressure blocked", StringComparison.Ordinal)
                    ? "판단: 중압 차단 성공"
                    : "판단: 중압 - 방어로 추가 피해 차단";
            }

            if (result.Contains("frenzy ready", StringComparison.Ordinal) && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return "판단: 공격 유지 시 광폭 연결";
            }

            if (snapshot.EnemyHp > 0 && snapshot.EnemyHp <= snapshot.PlayerAttack + 2 && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return "판단: 공격으로 마무리 가능";
            }

            if (snapshot.EnemyAttack > 0)
            {
                return "판단: 적 ATK " + snapshot.EnemyAttack + " - 피해 관리 필요";
            }

            return "판단: 공격으로 압박";
        }

        private static CombatAction? ResolveRecommendedCombatAction(PrototypeRunSnapshot snapshot)
        {
            if (!snapshot.IsInCombat)
            {
                return null;
            }

            if (snapshot.ScoutAttackReady && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return CombatAction.Attack;
            }

            if (HasSkillOpeningCue(snapshot) && HasCombatSkillEquipped(snapshot))
            {
                return CombatAction.Skill;
            }

            if (HasHeavyPressureCue(snapshot) && snapshot.IsCommandEquipped(PrototypeRunState.CommandDefendId))
            {
                return CombatAction.Defend;
            }

            if (snapshot.EnemyHp > 0 && snapshot.EnemyHp <= snapshot.PlayerAttack + 2 && snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return CombatAction.Attack;
            }

            if (snapshot.IsCommandEquipped(PrototypeRunState.CommandAttackId))
            {
                return CombatAction.Attack;
            }

            if (snapshot.IsCommandEquipped(PrototypeRunState.CommandDefendId))
            {
                return CombatAction.Defend;
            }

            return HasCombatSkillEquipped(snapshot) ? CombatAction.Skill : (CombatAction?)null;
        }

        private static bool HasCombatSkillEquipped(PrototypeRunSnapshot snapshot)
        {
            return snapshot.IsCommandEquipped(PrototypeRunState.CommandScoutId) ||
                snapshot.IsCommandEquipped(PrototypeRunState.CommandArts03Id);
        }

        private static bool HasHeavyPressureCue(PrototypeRunSnapshot snapshot)
        {
            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            return snapshot.EnemyAttack >= 4 ||
                result.Contains("heavy pressure", StringComparison.Ordinal);
        }

        private static bool HasSkillOpeningCue(PrototypeRunSnapshot snapshot)
        {
            var result = snapshot.LastCombatRoundResult ?? string.Empty;
            return snapshot.EnemyMaxHp > 0 && snapshot.EnemyHp * 2 <= snapshot.EnemyMaxHp ||
                result.Contains("skill opening", StringComparison.Ordinal);
        }

        private string BuildCombatPlayerCardText(PrototypeRunSnapshot snapshot)
        {
            return "플레이어\n" +
                "HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp + "  ATK " + snapshot.PlayerAttack + "\n" +
                BuildPlayerBuffChipLine(snapshot);
        }

        private string BuildCombatMataiosCardText(PrototypeRunSnapshot snapshot)
        {
            return "마타이오스\n" +
                "HP " + BuildMataiosHp(snapshot) + "  ATK " + BuildMataiosAttack(snapshot) + "\n" +
                BuildMataiosBuffLine(snapshot);
        }

        private string BuildPlayerBuffChipLine(PrototypeRunSnapshot snapshot)
        {
            var chips = new List<string>();
            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                chips.Add("단련 +1");
            }

            if (snapshot.LevelAttackBonus > 0)
            {
                chips.Add("성장 ATK +" + snapshot.LevelAttackBonus);
            }

            if (snapshot.LevelMaxHpBonus > 0)
            {
                chips.Add("성장 HP +" + snapshot.LevelMaxHpBonus);
            }

            if (snapshot.SkillCooldownReduction > 0)
            {
                chips.Add("CD -" + snapshot.SkillCooldownReduction);
            }

            if (snapshot.LastCombatRoundResult.Contains("frenzy break", StringComparison.Ordinal))
            {
                chips.Add("광폭 끊김");
            }
            else if (snapshot.LastCombatRoundResult.Contains("frenzy ", StringComparison.Ordinal))
            {
                chips.Add("광폭 발동");
            }
            else if (snapshot.CombatBuildSummary.Contains("광폭 준비", StringComparison.Ordinal))
            {
                chips.Add("광폭 준비");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0)
            {
                chips.Add("붕대");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_LANTERN_OIL") > 0)
            {
                chips.Add("등유");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_TORN_CHARM") > 0)
            {
                chips.Add("부적");
            }

            if (_roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
            {
                chips.Add("회상 닻");
            }

            return chips.Count == 0 ? "상태 없음" : JoinCompactChips(chips, 2);
        }

        private static string BuildMataiosChipLine(PrototypeRunSnapshot snapshot)
        {
            var affinity = snapshot.Affinity >= 4 ? "신뢰 높음" :
                snapshot.Affinity > 0 ? "신뢰 형성" :
                snapshot.Affinity < 0 ? "거리감" :
                "동행 중";
            return affinity + "  기억 " + snapshot.MemoryFragmentCount + "  안정";
        }

        private static string BuildMataiosHp(PrototypeRunSnapshot snapshot)
        {
            var max = Mathf.Max(1, snapshot.MataiosMaxHp);
            var current = Mathf.Clamp(snapshot.MataiosHp, 0, max);
            return current + "/" + max;
        }

        private static int BuildMataiosAttack(PrototypeRunSnapshot snapshot)
        {
            return Mathf.Max(0, snapshot.MataiosAttack);
        }

        private static string BuildMataiosBuffLine(PrototypeRunSnapshot snapshot)
        {
            var buffs = new List<string>();
            if (snapshot.MataiosDown)
            {
                buffs.Add("전투 불능");
            }

            if (snapshot.LastMataiosProtectReduction > 0)
            {
                buffs.Add("보호 -" + snapshot.LastMataiosProtectReduction);
            }

            if (snapshot.LastMataiosCombatDamage > 0)
            {
                buffs.Add("지원 피해 " + snapshot.LastMataiosCombatDamage);
            }

            if (snapshot.LastMataiosDownEvent)
            {
                buffs.Add("붕괴도 +5");
            }

            return buffs.Count == 0 ? "상태 없음" : JoinCompactChips(buffs, 2);
        }

        private static string BuildCombatFeedback(string roundResult)
        {
            if (string.IsNullOrEmpty(roundResult))
            {
                return "전투 준비";
            }

            if (roundResult.Contains("skill unavailable", StringComparison.Ordinal))
            {
                return "기술 불가: 보유 스킬 필요";
            }

            if (roundResult.Contains("recall anchor", StringComparison.Ordinal))
            {
                return "회상 닻: HP 회복 후 전투 지속";
            }

            if (roundResult.Contains("ready", StringComparison.OrdinalIgnoreCase))
            {
                var ready = "전투 준비";
                if (roundResult.Contains("scout setup", StringComparison.Ordinal))
                {
                    ready += " | 정찰 준비";
                }

                return ready;
            }

            var enemyDamage = ExtractRoundNumber(roundResult, "enemyDamage ").Trim();
            if (roundResult.Contains("Defend", StringComparison.Ordinal))
            {
                return "선택 방어 | 받은 피해 " + enemyDamage + " | 절반 감소";
            }

            if (roundResult.Contains("Attack", StringComparison.Ordinal))
            {
                var prefix = roundResult.Contains("blood overload", StringComparison.Ordinal)
                    ? "피의 서약 과부하 | "
                    : string.Empty;
                return prefix + "선택 공격 | 적 피해 " + ExtractRoundNumber(roundResult, "playerDamage ").Trim() +
                    " | 받은 피해 " + enemyDamage;
            }

            if (roundResult.Contains("Skill", StringComparison.Ordinal))
            {
                if (roundResult.Contains("scout skill", StringComparison.Ordinal))
                {
                    return "선택 정찰 | 다음 공격 강화 | 다음 피해 감소 1회";
                }

                return "선택 스킬 | 적 피해 " + ExtractRoundNumber(roundResult, "playerDamage ").Trim() +
                    " | 받은 피해 " + enemyDamage;
            }

            return "라운드 처리";
        }

        private bool HasScoutSkill(PrototypeRunSnapshot snapshot)
        {
            if (_roomController != null && _roomController.RunState != null)
            {
                return _roomController.RunState.HasAbilityRef("ABILITY_SCOUT");
            }

            return snapshot.AbilityCount > 0;
        }

        private bool HasAnyCombatSkill()
        {
            return HasScoutSkill(_roomController == null ? _lastSnapshot : _roomController.GetSnapshot()) || HasArtsSkill();
        }

        private List<string> BuildOwnedCombatSkillNames()
        {
            var skills = new List<string>();
            if (_roomController != null && _roomController.RunState != null)
            {
                if (_roomController.RunState.HasAbilityRef("ABILITY_SCOUT"))
                {
                    skills.Add("정찰");
                }

                if (_roomController.RunState.HasAbilityRef("ABILITY_ARTS_03"))
                {
                    skills.Add("번개 방출");
                }
            }
            else if (_lastSnapshot.AbilityCount > 0)
            {
                skills.Add("스킬");
            }

            return skills;
        }

        private string BuildOwnedSkillLine()
        {
            var skills = new List<string>();
            if (_roomController != null && _roomController.RunState != null)
            {
                if (_roomController.RunState.HasAbilityRef("ABILITY_SCOUT"))
                {
                    skills.Add("정찰");
                }

                if (_roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
                {
                    skills.Add("회상 닻");
                }

                if (_roomController.RunState.HasAbilityRef("ABILITY_ARTS_03"))
                {
                    skills.Add("번개 방출");
                }
            }
            else if (_lastSnapshot.AbilityCount > 0)
            {
                skills.Add("스킬");
            }

            return skills.Count == 0 ? "없음" : string.Join("  ", skills);
        }

        private string BuildOwnedItemLine()
        {
            if (_roomController == null || _roomController.RunState == null)
            {
                return "아이템 " + _lastSnapshot.ItemCount;
            }

            var items = new List<string>();
            AddOwnedItemLine(items, "ITEM_FIELD_BANDAGE", "붕대");
            AddOwnedItemLine(items, "ITEM_01", "붕대 뭉치");
            AddOwnedItemLine(items, "ITEM_02", "작은 룬석");
            AddOwnedItemLine(items, "ITEM_03", "날카로운 숫돌");
            AddOwnedItemLine(items, "ITEM_04", "낡은 방패 조각");
            AddOwnedItemLine(items, "ITEM_05", "독침");
            AddOwnedItemLine(items, "ITEM_09", "마모된 부적");
            AddOwnedItemLine(items, "ITEM_10", "피의 계약서");
            AddOwnedItemLine(items, "RELIC_GENERIC_01", "회귀자의 낡은 코트");
            AddOwnedItemLine(items, "RELIC_SWORD_01", "피묻은 칼날");
            AddOwnedItemLine(items, "RELIC_LINE_01", "저격수의 망원경");
            AddOwnedItemLine(items, "RELIC_ARTS_01", "원소 수정");
            AddOwnedItemLine(items, "RELIC_GUARD_01", "강화 방패");
            return items.Count == 0 ? "보유 아이템 없음" : string.Join("\n", items);
        }

        private bool HasArtsSkill()
        {
            return _roomController != null &&
                _roomController.RunState != null &&
                _roomController.RunState.HasAbilityRef("ABILITY_ARTS_03");
        }

        private void AddOwnedItemLine(List<string> items, string itemRef, string label)
        {
            var count = _roomController == null || _roomController.RunState == null ? 0 : _roomController.RunState.GetItemCount(itemRef);
            if (count > 0)
            {
                items.Add(label + " x" + count);
            }
        }

        private static string ExtractRoundNumber(string source, string token)
        {
            var index = string.IsNullOrEmpty(source) ? -1 : source.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
            {
                return string.Empty;
            }

            var start = index + token.Length;
            var end = source.IndexOf(" |", start, StringComparison.Ordinal);
            if (end < 0)
            {
                end = source.Length;
            }

            var value = source.Substring(start, end - start).Trim();
            return string.IsNullOrEmpty(value) ? string.Empty : " " + value;
        }

        private string ResolveCombatEncounterId()
        {
            for (var i = _demoRouteEncounterIds.Count - 1; i >= 0; i--)
            {
                if (IsCombatEncounterId(_demoRouteEncounterIds[i]))
                {
                    return _demoRouteEncounterIds[i];
                }
            }

            return string.Empty;
        }

        private void SetCombatButtonsInteractable(bool interactable)
        {
            if (attackButton != null)
            {
                attackButton.interactable = interactable;
            }

            if (defendButton != null)
            {
                defendButton.interactable = interactable;
            }

            if (skillButton != null)
            {
                skillButton.interactable = interactable && _roomController != null && _roomController.GetSnapshot().AbilityCount > 0;
            }
        }
    }
}
