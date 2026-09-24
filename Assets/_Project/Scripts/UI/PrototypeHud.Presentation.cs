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
        private DemoPresentationSlot ResolvePresentationSlot(string encounterStableId)
        {
            return presentationData != null && presentationData.TryGetSlot(encounterStableId, out var slot) ? slot : null;
        }

        private void ApplyPortrait()
        {
            EnsurePortraitImage();
            if (npcPortraitImage == null)
            {
                return;
            }

            npcPortraitImage.sprite = mataiosPortrait;
            npcPortraitImage.preserveAspect = true;
            npcPortraitImage.gameObject.SetActive(mataiosPortrait != null);
        }

        private void EnsurePortraitImage()
        {
            if (npcPortraitImage != null)
            {
                return;
            }

            var portraitObject = new GameObject("Mataios Portrait");
            portraitObject.transform.SetParent(HudParent, false);

            var rect = portraitObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.06f, 0f);
            rect.anchorMax = new Vector2(0.30f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 720f);
            rect.sizeDelta = new Vector2(0f, 360f);

            npcPortraitImage = portraitObject.AddComponent<Image>();
            npcPortraitImage.color = Color.white;
            npcPortraitImage.raycastTarget = false;
        }

        private void EnsureEncounterBackgroundImage()
        {
            if (encounterBackgroundImage != null)
            {
                return;
            }

            var backgroundObject = new GameObject("Encounter Background");
            backgroundObject.transform.SetParent(HudParent, false);
            backgroundObject.transform.SetAsFirstSibling();

            var rect = backgroundObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            encounterBackgroundImage = backgroundObject.AddComponent<Image>();
            encounterBackgroundImage.color = new Color(1f, 1f, 1f, 0.72f);
            encounterBackgroundImage.preserveAspect = false;
            encounterBackgroundImage.raycastTarget = false;
            encounterBackgroundImage.gameObject.SetActive(false);
        }

        private void EnsureNpcSpotlightLayer()
        {
            if (npcSpotlightLayer != null)
            {
                return;
            }

            EnsureScreenLayers();
            var layerObject = new GameObject("NPC Spotlight Layer");
            layerObject.transform.SetParent(visualLayer == null ? HudParent : visualLayer, false);
            npcSpotlightLayer = layerObject.AddComponent<RectTransform>();
            npcSpotlightLayer.anchorMin = Vector2.zero;
            npcSpotlightLayer.anchorMax = Vector2.one;
            npcSpotlightLayer.offsetMin = Vector2.zero;
            npcSpotlightLayer.offsetMax = Vector2.zero;

            npcSpotlightBackdropImage = CreateCombatImage(npcSpotlightLayer, "NPC Spotlight Backdrop", Vector2.zero, Vector2.one);
            npcSpotlightBackdropImage.sprite = null;
            npcSpotlightBackdropImage.color = new Color(0f, 0f, 0f, 0.34f);
            npcSpotlightBackdropImage.gameObject.SetActive(true);

            npcSpotlightGlowImage = CreateCombatImage(npcSpotlightLayer, "NPC Spotlight Glow", new Vector2(0.01f, 0.08f), new Vector2(0.52f, 0.98f));
            npcSpotlightGlowImage.sprite = presentationData == null ? null : presentationData.SpotlightGradient;
            npcSpotlightGlowImage.preserveAspect = true;
            npcSpotlightGlowImage.color = npcSpotlightGlowImage.sprite == null ? new Color(0.52f, 0.72f, 0.62f, 0.22f) : new Color(0.86f, 0.96f, 0.86f, 0.72f);
            npcSpotlightGlowImage.gameObject.SetActive(true);

            npcSpotlightShadowImage = CreateCombatImage(npcSpotlightLayer, "NPC Spotlight Shadow", new Vector2(0.04f, 0.05f), new Vector2(0.48f, 0.88f));
            npcSpotlightShadowImage.sprite = presentationData == null ? null : presentationData.SpotlightGradient;
            npcSpotlightShadowImage.preserveAspect = true;
            npcSpotlightShadowImage.color = npcSpotlightShadowImage.sprite == null ? new Color(0f, 0f, 0f, 0.42f) : new Color(0f, 0f, 0f, 0.36f);
            npcSpotlightShadowImage.gameObject.SetActive(true);

            var merchantObject = new GameObject("Merchant Visual");
            merchantObject.transform.SetParent(npcSpotlightLayer, false);

            var rect = merchantObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, -0.05f);
            rect.anchorMax = new Vector2(0.48f, 0.96f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            merchantVisualImage = merchantObject.AddComponent<Image>();
            merchantVisualImage.color = new Color(1f, 1f, 1f, 0.98f);
            merchantVisualImage.preserveAspect = true;
            merchantVisualImage.raycastTarget = false;
            merchantVisualImage.gameObject.SetActive(false);

            var plate = CreatePanel("NPC Dialogue Plate", npcSpotlightLayer, new Vector2(0.04f, 0.05f), new Vector2(0.49f, 0.28f), new Color(0.03f, 0.045f, 0.050f, 0.78f));
            npcDialoguePlateImage = plate.GetComponent<Image>();
            if (npcDialoguePlateImage != null && presentationData != null && presentationData.NpcDialoguePlate != null)
            {
                npcDialoguePlateImage.sprite = presentationData.NpcDialoguePlate;
                npcDialoguePlateImage.preserveAspect = true;
                npcDialoguePlateImage.color = Color.white;
            }

            npcSpotlightNameText = CreateCombatChildText(plate, "NPC Spotlight Name", new Vector2(0.06f, 0.60f), new Vector2(0.94f, 0.92f), 25, TextAnchor.MiddleLeft);
            npcSpotlightNameText.color = new Color(0.91f, 0.98f, 0.91f, 1f);
            npcSpotlightDialogueText = CreateCombatChildText(plate, "NPC Spotlight Dialogue", new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.58f), 23, TextAnchor.MiddleLeft);
            npcSpotlightDialogueText.color = new Color(0.84f, 0.91f, 0.89f, 1f);
            npcSpotlightLayer.gameObject.SetActive(false);
        }

        private void EnsureMerchantVisualImage()
        {
            EnsureNpcSpotlightLayer();
        }

        private void ApplyMerchantPresentation(int floor)
        {
            EnsureMerchantVisualImage();
            if (merchantVisualImage == null)
            {
                return;
            }

            var sprite = presentationData != null && presentationData.TryGetMerchantSprite(floor, out var merchantSprite) ? merchantSprite : mataiosPortrait;
            ShowNpcSpotlight(sprite, "상인", ResolveMerchantSpotlightLine(floor), NpcSpotlightMode.Shop);
        }

        private void HideMerchantPresentation()
        {
            HideNpcSpotlight();
        }

        private void ShowNpcSpotlight(Sprite sprite, string displayName, string dialogue, NpcSpotlightMode mode)
        {
            EnsureNpcSpotlightLayer();
            SetLayerVisible(visualLayer, true);
            _npcSpotlightModeLabel = ResolveNpcSpotlightModeLabel(mode);
            if (npcSpotlightLayer != null)
            {
                npcSpotlightLayer.gameObject.SetActive(sprite != null);
            }

            if (merchantVisualImage != null)
            {
                merchantVisualImage.sprite = sprite;
                merchantVisualImage.gameObject.SetActive(sprite != null);
            }

            ApplyNpcSpotlightModeLayout(mode);
            if (npcSpotlightNameText != null)
            {
                npcSpotlightNameText.text = displayName;
            }

            if (npcSpotlightDialogueText != null)
            {
                npcSpotlightDialogueText.text = dialogue;
            }
        }

        private void HideNpcSpotlight()
        {
            _npcSpotlightModeLabel = string.Empty;
            if (npcSpotlightLayer != null)
            {
                npcSpotlightLayer.gameObject.SetActive(false);
            }
        }

        private void HideLegacyEncounterVisuals(bool hideBackground)
        {
            if (npcPortraitImage != null)
            {
                npcPortraitImage.gameObject.SetActive(false);
            }

            if (hideBackground && encounterBackgroundImage != null)
            {
                encounterBackgroundImage.gameObject.SetActive(false);
            }
        }

        private void ApplyNpcSpotlightModeLayout(NpcSpotlightMode mode)
        {
            if (npcSpotlightLayer == null || merchantVisualImage == null)
            {
                return;
            }

            var characterRect = merchantVisualImage.GetComponent<RectTransform>();
            var plateRect = npcSpotlightNameText == null ? null : npcSpotlightNameText.transform.parent.GetComponent<RectTransform>();
            if (mode == NpcSpotlightMode.Shop)
            {
                ApplyRect(characterRect, new Vector2(0.23f, -0.08f), new Vector2(0.77f, 0.99f));
                ApplyRect(plateRect, new Vector2(0.12f, 0.04f), new Vector2(0.88f, 0.22f));
                ApplyRect(npcSpotlightGlowImage == null ? null : npcSpotlightGlowImage.GetComponent<RectTransform>(), new Vector2(0.18f, 0.03f), new Vector2(0.82f, 0.98f));
                ApplyRect(npcSpotlightShadowImage == null ? null : npcSpotlightShadowImage.GetComponent<RectTransform>(), new Vector2(0.22f, 0.00f), new Vector2(0.78f, 0.88f));
                return;
            }

            if (mode == NpcSpotlightMode.Rest)
            {
                ApplyRect(characterRect, new Vector2(0.04f, -0.12f), new Vector2(0.42f, 0.96f));
                ApplyRect(plateRect, new Vector2(0.08f, 0.06f), new Vector2(0.52f, 0.28f));
                ApplyRect(npcSpotlightGlowImage == null ? null : npcSpotlightGlowImage.GetComponent<RectTransform>(), new Vector2(0.02f, 0.04f), new Vector2(0.48f, 0.98f));
                ApplyRect(npcSpotlightShadowImage == null ? null : npcSpotlightShadowImage.GetComponent<RectTransform>(), new Vector2(0.05f, 0.00f), new Vector2(0.44f, 0.88f));
                return;
            }

            ApplyRect(characterRect, new Vector2(0.04f, -0.06f), new Vector2(0.42f, 0.94f));
            ApplyRect(plateRect, new Vector2(0.07f, 0.06f), new Vector2(0.50f, 0.27f));
        }

        private static string ResolveNpcSpotlightModeLabel(NpcSpotlightMode mode)
        {
            return mode switch
            {
                NpcSpotlightMode.Shop => "상점",
                NpcSpotlightMode.Rest => "휴식",
                NpcSpotlightMode.Event => "이벤트",
                _ => string.Empty
            };
        }

        private static string ResolveMerchantSpotlightLine(int floor)
        {
            return floor >= 4
                ? "필요한 걸 골라. 오래 머무를 곳은 아니니까."
                : "보스 전에 필요한 준비를 끝내세요.";
        }

        private static string ResolveEncounterDisplayName(EncounterData encounter)
        {
            if (encounter == null)
            {
                return "Encounter";
            }

            var choices = encounter.Choices ?? new EncounterChoiceRuntimeData[0];
            for (var i = 0; i < choices.Length; i++)
            {
                var effects = choices[i] == null ? null : choices[i].effects;
                if (effects == null)
                {
                    continue;
                }

                for (var j = 0; j < effects.Length; j++)
                {
                    if (effects[j] != null && effects[j].kind == "StartCombat")
                    {
                        return "CombatGate";
                    }
                }
            }

            return encounter.Type.ToString();
        }

        private string ResolvePresentationDisplayName(EncounterData encounter)
        {
            if (encounter == null)
            {
                return "Encounter";
            }

            if (presentationData != null &&
                presentationData.TryGetSlot(encounter.Id, out var slot) &&
                !string.IsNullOrEmpty(slot.DisplayName))
            {
                return showRawDebugText ? slot.DisplayName : ResolvePublicEncounterLabel(slot.DisplayName, encounter);
            }

            return showRawDebugText ? ResolveEncounterDisplayName(encounter) : ResolvePublicEncounterLabel(string.Empty, encounter);
        }

        private void UpdatePresentationState(PrototypeRunSnapshot snapshot)
        {
            if (RestInteractionPanelVisible)
            {
                HideLegacyEncounterVisuals(hideBackground: false);
                return;
            }

            if (IsMapRouteState(snapshot))
            {
                HideLegacyEncounterVisuals(hideBackground: true);
                return;
            }

            var slot = ResolveCurrentPresentationSlot(snapshot);
            ApplyPresentationSlot(slot);
            if (_shopPresentationActive || snapshot.IsInCombat || _eventPresentationActive || _bossGatePresentationActive)
            {
                HideLegacyEncounterVisuals(hideBackground: false);
            }

            if (BossGateUiVisible || EndingUiVisible)
            {
                HideLegacyEncounterVisuals(hideBackground: true);
            }

            if (!snapshot.IsInCombat && _shopPresentationActive && (shopUiController == null || !shopUiController.Visible))
            {
                ApplyMerchantPresentation(snapshot.CurrentFloor);
            }
            else if (!RestInteractionPanelVisible)
            {
                HideMerchantPresentation();
            }

            if (interactionText != null && !showRawDebugText && snapshot.RunClear && !snapshot.IsInCombat)
            {
                interactionText.text = snapshot.EndingChoicePending ? "엔딩 선택" : "클리어";
            }
        }

        private bool HasDedicatedPresentationUi()
        {
            return !showRawDebugText && (EventUiVisible || BossGateUiVisible || EndingUiVisible);
        }

        private DemoPresentationSlot ResolveCurrentPresentationSlot(PrototypeRunSnapshot snapshot)
        {
            if (presentationData == null)
            {
                return null;
            }

            if (!snapshot.IsInCombat &&
                (_shopPresentationActive || _eventPresentationActive) &&
                !string.IsNullOrEmpty(_activePresentationEncounterId) &&
                presentationData.TryGetSlot(_activePresentationEncounterId, out var activeSlot))
            {
                return activeSlot;
            }

            var encounterId = snapshot.IsInCombat ? ResolveCombatEncounterId() : snapshot.NextDemoEncounterId;
            if (string.IsNullOrEmpty(encounterId) && snapshot.RunClear)
            {
                encounterId = presentationData.TryGetSlot("run.clear", out _) ? "run.clear" : ResolveDemoCompleteCutsceneEncounterId(snapshot);
            }

            return presentationData.TryGetSlot(encounterId, out var slot) ? slot : null;
        }

        private DemoPresentationSlot ResolveEnemyPresentationSlot(string enemyStableId)
        {
            if (presentationData == null || string.IsNullOrEmpty(enemyStableId))
            {
                return null;
            }

            return presentationData.TryGetSlot(enemyStableId, out var slot) ? slot : null;
        }

        private void ApplyPresentationSlot(string encounterStableId)
        {
            if (presentationData != null && presentationData.TryGetSlot(encounterStableId, out var slot))
            {
                _activePresentationEncounterId = encounterStableId;
                ApplyPresentationSlot(slot);
            }
        }

        private void ApplyRestPresentationSlot(string encounterStableId)
        {
            if (presentationData == null)
            {
                return;
            }

            if (presentationData.TryGetSlot(encounterStableId, out var slot))
            {
                _activePresentationEncounterId = encounterStableId;
                ApplyPresentationSlot(slot);
                return;
            }

            if (presentationData.TryGetSlot("ENC_REST_01", out var fallbackSlot))
            {
                _activePresentationEncounterId = "ENC_REST_01";
                ApplyPresentationSlot(fallbackSlot);
            }
        }

        private void ApplyPresentationSlot(DemoPresentationSlot slot)
        {
            EnsureEncounterBackgroundImage();
            if (encounterBackgroundImage != null)
            {
                encounterBackgroundImage.sprite = slot == null ? null : slot.BackgroundSprite;
                encounterBackgroundImage.gameObject.SetActive(encounterBackgroundImage.sprite != null);
            }

            if (slot != null && slot.MataiosPortrait != null)
            {
                SetNpcPortrait(slot.MataiosPortrait);
            }
        }

        private static bool IsShopEncounterId(string encounterId)
        {
            return !string.IsNullOrEmpty(encounterId) && encounterId.Contains("SHOP", StringComparison.Ordinal);
        }

        private void UpdateCutsceneTriggers(PrototypeRunSnapshot snapshot)
        {
            if (presentationData == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(snapshot.LastMemoryFragmentId))
            {
                TryPlayCutsceneOnce(
                    snapshot.LastMemoryFragmentId,
                    "memory:" + snapshot.LastMemoryFragmentId,
                    PrototypeCutsceneTrigger.MemoryFragmentUnlock,
                    ref _lastMemoryCutsceneKey);
            }

            if (snapshot.IsInCombat && !string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                TryPlayCutsceneOnce(
                    snapshot.NextDemoEncounterId,
                    "combat:" + snapshot.LastCombatId,
                    PrototypeCutsceneTrigger.CombatGateStart,
                    ref _lastCombatCutsceneKey);
            }

            if (snapshot.RunClear && !snapshot.IsInCombat)
            {
                TryPlayCutsceneOnce(
                    ResolveDemoCompleteCutsceneEncounterId(snapshot),
                    "complete:" + snapshot.DemoResolvedStepCount,
                    PrototypeCutsceneTrigger.DemoComplete,
                    ref _lastDemoCompleteCutsceneKey);
            }
        }

        private string ResolveDemoCompleteCutsceneEncounterId(PrototypeRunSnapshot snapshot)
        {
            if (!string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                return snapshot.NextDemoEncounterId;
            }

            for (var i = _demoRouteEncounterIds.Count - 1; i >= 0; i--)
            {
                var encounterId = _demoRouteEncounterIds[i];
                if (!string.IsNullOrEmpty(encounterId) && encounterId != "demo.complete")
                {
                    return encounterId;
                }
            }

            return string.Empty;
        }

        private void TryPlayInteractionCutscene(string encounterStableId, string message)
        {
            if (string.IsNullOrEmpty(encounterStableId) || string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Contains("memory unlocked", StringComparison.Ordinal) ||
                message.Contains(PrototypeRunState.MemoryFragmentPublicFeedback, StringComparison.Ordinal))
            {
                TryPlayCutsceneOnce(
                    encounterStableId,
                    "memory:" + encounterStableId,
                    PrototypeCutsceneTrigger.MemoryFragmentUnlock,
                    ref _lastMemoryCutsceneKey);
            }

            if (message.Contains("combat started"))
            {
                TryPlayCutsceneOnce(
                    encounterStableId,
                    "combat:" + encounterStableId,
                    PrototypeCutsceneTrigger.CombatGateStart,
                    ref _lastCombatCutsceneKey);
            }

            if (message.Contains("demo.complete") || message.Contains("run.clear"))
            {
                TryPlayCutsceneOnce(
                    encounterStableId,
                    "complete:" + encounterStableId,
                    PrototypeCutsceneTrigger.DemoComplete,
                    ref _lastDemoCompleteCutsceneKey);
            }
        }

        private void TryPlayCutsceneOnce(string encounterStableId, string key, PrototypeCutsceneTrigger trigger, ref string lastKey)
        {
            if (lastKey == key || presentationData == null || !presentationData.TryGetCutscene(encounterStableId, trigger, out var cutscene))
            {
                return;
            }

            EnsureCutscenePlayer();
            SubscribeCutsceneFinished();
            cutscenePlayer.Play(cutscene);
            SetCombatButtonsInteractable(false);
            lastKey = key;
        }

        private void EnsureCutscenePlayer()
        {
            if (cutscenePlayer != null)
            {
                return;
            }

            var playerObject = new GameObject("Prototype Cutscene Player");
            playerObject.transform.SetParent(HudParent, false);
            var rect = playerObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            cutscenePlayer = playerObject.AddComponent<PrototypeCutscenePlayer>();
            SubscribeCutsceneFinished();
            cutscenePlayer.Hide();
        }

        private void SubscribeCutsceneFinished()
        {
            if (cutscenePlayer == null || _cutsceneFinishedSubscribed)
            {
                return;
            }

            cutscenePlayer.Finished += HandleCutsceneFinished;
            _cutsceneFinishedSubscribed = true;
        }

        private void HandleCutsceneFinished()
        {
            if (_roomController != null)
            {
                ShowRunState(_roomController.GetSnapshot());
            }
        }

        private bool IsCutscenePlaying()
        {
            return cutscenePlayer != null && cutscenePlayer.gameObject.activeInHierarchy && cutscenePlayer.IsPlaying;
        }
    }
}
