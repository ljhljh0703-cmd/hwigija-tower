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
        private void EnsureLowHpWarning()
        {
            if (lowHpWarningImage != null)
            {
                return;
            }

            var warningObject = new GameObject("Low HP Edge Warning");
            warningObject.transform.SetParent(HudParent, false);
            warningObject.transform.SetAsLastSibling();
            var rect = warningObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            lowHpWarningImage = warningObject.AddComponent<Image>();
            lowHpWarningImage.color = new Color(0.80f, 0.06f, 0.05f, 0.16f);
            lowHpWarningImage.raycastTarget = false;
            lowHpWarningImage.gameObject.SetActive(false);
        }

        private void UpdateLowHpWarning(PrototypeRunSnapshot snapshot)
        {
            if (lowHpWarningImage == null)
            {
                return;
            }

            var visible = !string.IsNullOrEmpty(snapshot.RunId) &&
                snapshot.PlayerMaxHp > 0 &&
                snapshot.PlayerHp > 0 &&
                snapshot.PlayerHp / (float)snapshot.PlayerMaxHp <= lowHpWarningRatio;
            lowHpWarningImage.gameObject.SetActive(visible);
            if (visible)
            {
                lowHpWarningImage.transform.SetAsLastSibling();
            }
        }

        private void EnsureTopHudIcons()
        {
            EnsureScreenLayers();
            topGoldIconImage = EnsureHudIcon(topGoldIconImage, "Top Gold Icon", topStatusLayer, new Vector2(0.62f, 0.50f), 36f);
            topMemoryIconImage = EnsureHudIcon(topMemoryIconImage, "Top Memory Icon", topStatusLayer, new Vector2(0.28f, 0.24f), 32f);
            topAffinityIconImage = EnsureHudIcon(topAffinityIconImage, "Top Affinity Icon", topStatusLayer, new Vector2(0.48f, 0.24f), 32f);
            topPlayerProfileImage = EnsureHudIcon(topPlayerProfileImage, "Top Player Profile", topStatusLayer, new Vector2(0.052f, 0.50f), 68f);
            topMataiosProfileImage = EnsureHudIcon(topMataiosProfileImage, "Top Mataios Profile", topStatusLayer, new Vector2(0.128f, 0.50f), 68f);
        }

        private void EnsureRouteText()
        {
            if (routeText != null)
            {
                ApplyRouteHeaderRect();
                return;
            }

            routeText = CreateHudText("Demo Route Text", new Vector2(0.08f, 0.845f), new Vector2(0.92f, 0.900f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, 34, TextAnchor.MiddleCenter, new Color(0.88f, 0.94f, 0.98f, 1f));
            ApplyRouteHeaderRect();
        }

        private void ApplyRouteHeaderRect()
        {
            if (routeText == null)
            {
                return;
            }

            ApplyTextRect(routeText, new Vector2(0.08f, 0.845f), new Vector2(0.92f, 0.900f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, 34, TextAnchor.MiddleCenter);
        }

        private void EnsureMemoryText()
        {
            if (memoryText != null)
            {
                return;
            }

            memoryText = CreateHudText("Memory Combat Text", new Vector2(0.31f, 0.382f), new Vector2(0.92f, 0.478f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, CaptionFontSize, TextAnchor.MiddleLeft, new Color(0.82f, 0.89f, 0.92f, 1f));
            memoryText.lineSpacing = 0.88f;
        }

        private void UpdateRouteIndicator(PrototypeRunSnapshot snapshot)
        {
            EnsureRouteText();
            if (routeText == null)
            {
                return;
            }

            var restVisible = RestInteractionPanelVisible;
            var shopVisible = _shopPresentationActive && !snapshot.IsInCombat;
            var mapVisible = IsMapRouteState(snapshot);
            var dedicatedPresentation = HasDedicatedPresentationUi();
            routeText.gameObject.SetActive((!snapshot.IsInCombat && !_eventPresentationActive && !restVisible && !shopVisible && !mapVisible && !dedicatedPresentation) || showRawDebugText);
            if ((snapshot.IsInCombat || _eventPresentationActive || restVisible || shopVisible || mapVisible || dedicatedPresentation) && !showRawDebugText)
            {
                if (mapVisible)
                {
                    routeText.text = BuildPublicMapSummary(snapshot);
                }
                else
                {
                    routeText.text = string.Empty;
                }
                return;
            }

            if (_demoRouteLabels.Count == 0)
            {
                if (!showRawDebugText && snapshot.StairUnlocked)
                {
                    routeText.text = BuildPublicRouteSummary(snapshot, 0);
                    return;
                }

                if (!showRawDebugText && snapshot.HasFloorMap)
                {
                    routeText.text = BuildPublicMapSummary(snapshot);
                    return;
                }

                routeText.text = showRawDebugText
                    ? (string.IsNullOrEmpty(snapshot.NextDemoEncounterId)
                    ? "route: " + snapshot.DemoStatus
                    : "route next: " + snapshot.NextDemoNodeId + "/" + snapshot.NextDemoEncounterId)
                    : "진행: " + (string.IsNullOrEmpty(snapshot.DemoStatus) ? "-" : ResolvePublicDemoStatus(snapshot));
                return;
            }

            var currentIndex = GetCurrentRouteIndex(snapshot);
            if (!showRawDebugText)
            {
                routeText.text = BuildPublicRouteSummary(snapshot, currentIndex);
                return;
            }

            var text = "route\n";
            for (var i = 0; i < _demoRouteLabels.Count; i++)
            {
                string marker;
                if ((snapshot.RunClear && !snapshot.IsInCombat) || i < currentIndex)
                {
                    marker = "[complete]";
                }
                else if (i == currentIndex)
                {
                    marker = "[current]";
                }
                else
                {
                    marker = "[locked]";
                }

                text += marker + " " + _demoRouteLabels[i];
                if (showRawDebugText && i < _demoRouteEncounterIds.Count)
                {
                    text += " | " + _demoRouteEncounterIds[i];
                }

                if (i < _demoRouteLabels.Count - 1)
                {
                    text += "\n";
                }
            }

            routeText.text = text;
        }

        private int GetCurrentRouteIndex(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                for (var i = _demoRouteEncounterIds.Count - 1; i >= 0; i--)
                {
                    if (IsCombatEncounterId(_demoRouteEncounterIds[i]))
                    {
                        return i;
                    }
                }
            }

            if (snapshot.RunClear)
            {
                return _demoRouteLabels.Count;
            }

            if (string.IsNullOrEmpty(snapshot.NextDemoEncounterId))
            {
                return Mathf.Clamp(snapshot.DemoResolvedStepCount, 0, Mathf.Max(0, _demoRouteLabels.Count - 1));
            }

            for (var i = 0; i < _demoRouteLabels.Count; i++)
            {
                if (i < _demoRouteEncounterIds.Count && _demoRouteEncounterIds[i] == snapshot.NextDemoEncounterId)
                {
                    return i;
                }
            }

            return Mathf.Clamp(snapshot.DemoResolvedStepCount, 0, Mathf.Max(0, _demoRouteLabels.Count - 1));
        }

        private void UpdatePrimaryHeaderVisibility(PrototypeRunSnapshot snapshot)
        {
            if (interactionText == null || showRawDebugText)
            {
                return;
            }

            var hiddenByEncounterState = snapshot.IsInCombat ||
                _eventPresentationActive ||
                _shopPresentationActive ||
                RestInteractionPanelVisible ||
                IsMapRouteState(snapshot) ||
                HasDedicatedPresentationUi();
            interactionText.gameObject.SetActive(!hiddenByEncounterState);
        }

        private string BuildPublicRouteSummary(PrototypeRunSnapshot snapshot, int currentIndex)
        {
            if (snapshot.IsInCombat)
            {
                return "목표\nFloor " + snapshot.CurrentFloor + " | 전투 중\n공격/방어로 적 HP를 줄이세요";
            }

            if (snapshot.RunClear)
            {
                if (snapshot.EndingRest)
                {
                    return "목표\n안식 선택 완료";
                }

                if (snapshot.EndingContinue)
                {
                    return "목표\n동행 계속 선택\n재시작 가능";
                }

                return "목표\n탑 정상\n안식 또는 동행 계속을 선택하세요";
            }

            if (snapshot.RunFailed)
            {
                return "목표\n실패\n재시작 가능";
            }

            if (snapshot.StairUnlocked)
            {
                return "목표\nFloor " + snapshot.CurrentFloor + " 완료\n다음 층으로 올라가세요";
            }

            if (snapshot.BossGateUnlocked)
            {
                return "목표\nFloor " + snapshot.CurrentFloor + " | 보스 관문\n승리하면 다음 단계가 열립니다";
            }

            if (snapshot.HasFloorMap)
            {
                return BuildPublicMapSummary(snapshot);
            }

            var label = currentIndex >= 0 && currentIndex < _demoRouteLabels.Count
                ? _demoRouteLabels[currentIndex]
                : ResolvePublicDemoStatus(snapshot);
            var step = Mathf.Clamp(currentIndex + 1, 1, Mathf.Max(1, _demoRouteLabels.Count));
            var remaining = Mathf.Max(0, _demoRouteLabels.Count - step);
            return "목표\nFloor " + snapshot.CurrentFloor + " | 현재: " + label + "\n진행 버튼으로 선택지를 엽니다 | 남은 단계 " + remaining;
        }

        private static string BuildPublicMapSummary(PrototypeRunSnapshot snapshot)
        {
            return "갈림길 선택";
        }

        private static string ResolvePublicDemoStatus(PrototypeRunSnapshot snapshot)
        {
            if (snapshot.IsInCombat)
            {
                return "전투 중";
            }

            return snapshot.EndingRest ? "안식 선택 완료" :
                snapshot.EndingContinue ? "동행 계속 선택" :
                snapshot.RunClear ? "엔딩 선택" :
                snapshot.RunFailed ? "실패" :
                snapshot.StairUnlocked ? "다음 층 가능" :
                "준비";
        }

        private string BuildPlayerStatusLine(PrototypeRunSnapshot snapshot)
        {
            var buff = "버프 없음";
            if (snapshot.LastCombatRoundResult.Contains("training +", StringComparison.Ordinal))
            {
                buff = "단련 피해 +1";
            }
            else if (_roomController != null && _roomController.RunState != null && _roomController.RunState.GetItemCount("ITEM_FIELD_BANDAGE") > 0)
            {
                buff = "붕대 보유";
            }
            else if (_roomController != null && _roomController.RunState != null && _roomController.RunState.HasAbilityRef("ABILITY_RECALL_ANCHOR"))
            {
                buff = "회상 닻 보유";
            }

            return "플레이어 HP " + snapshot.PlayerHp + "/" + snapshot.PlayerMaxHp +
                "  ATK " + snapshot.PlayerAttack +
                "  아이템 " + snapshot.ItemCount +
                "  능력 " + snapshot.AbilityCount +
                "  " + buff;
        }

        private static string BuildCompanionStatusLine(PrototypeRunSnapshot snapshot)
        {
            var affinity = snapshot.Affinity >= 4 ? "신뢰 높음" :
                snapshot.Affinity > 0 ? "신뢰 형성" :
                snapshot.Affinity < 0 ? "거리감" :
                "동행 중";
            return "마타이오스 " + affinity + "  기억 " + snapshot.MemoryFragmentCount + "  안정";
        }

        private static string ShortenPublicLine(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, Mathf.Max(1, maxLength - 1)).TrimEnd() + "…";
        }

        private static string JoinCompactChips(List<string> chips, int visibleLimit)
        {
            if (chips == null || chips.Count == 0)
            {
                return string.Empty;
            }

            var limit = Mathf.Clamp(visibleLimit, 1, chips.Count);
            if (chips.Count <= limit)
            {
                return string.Join("  ", chips);
            }

            var visible = chips.GetRange(0, limit);
            visible.Add("+" + (chips.Count - limit));
            return string.Join("  ", visible);
        }

        private void UpdateTopHudIcons(PrototypeRunSnapshot snapshot)
        {
            EnsureTopHudIcons();
            ApplyTopHudIconSprites();
            var visible = !showRawDebugText && !HasDedicatedPresentationUi() && !string.IsNullOrEmpty(snapshot.RunId);
            SetImageVisible(topGoldIconImage, visible);
            SetImageVisible(topMemoryIconImage, false);
            SetImageVisible(topAffinityIconImage, false);
            SetImageVisible(topPlayerProfileImage, visible);
            SetImageVisible(topMataiosProfileImage, false);
        }

        private void ApplyTopHudIconSprites()
        {
            SetStaticIcon(topGoldIconImage, "resource.gold");
            SetStaticIcon(topMemoryIconImage, "resource.memory");
            SetStaticIcon(topAffinityIconImage, "resource.affinity");
            if (topPlayerProfileImage != null)
            {
                topPlayerProfileImage.sprite = presentationData == null ? null : presentationData.DefaultPlayerPortrait;
                topPlayerProfileImage.color = Color.white;
                topPlayerProfileImage.preserveAspect = true;
            }

            if (topMataiosProfileImage != null)
            {
                topMataiosProfileImage.sprite = presentationData == null ? null : presentationData.CombatMataiosPortrait;
                topMataiosProfileImage.color = Color.white;
                topMataiosProfileImage.preserveAspect = true;
            }
        }

        private static string BuildMemoryKeyLine(PrototypeRunSnapshot snapshot)
        {
            var titleKey = string.IsNullOrEmpty(snapshot.LastMemoryFragmentTitleKey)
                ? "titleKey:-"
                : "titleKey:" + snapshot.LastMemoryFragmentTitleKey;
            var bodyKey = string.IsNullOrEmpty(snapshot.LastMemoryFragmentBodyKey)
                ? "bodyKey:-"
                : "bodyKey:" + snapshot.LastMemoryFragmentBodyKey;
            return titleKey + " | " + bodyKey;
        }
    }
}
