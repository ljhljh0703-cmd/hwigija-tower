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
        private void SetResultVisible(bool visible)
        {
            if (resultText != null)
            {
                resultText.gameObject.SetActive(visible);
            }

            if (resultIconStrip != null)
            {
                resultIconStrip.gameObject.SetActive(visible && _activeResultSummaryValues.Count > 0);
            }

            SetLayerVisible(resultLayer, visible);
        }

        private void EnsureResultText()
        {
            if (resultText != null)
            {
                return;
            }

            var resultObject = new GameObject("Encounter Result Text");
            resultObject.transform.SetParent(HudParent, false);

            var rect = resultObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.34f, 0f);
            rect.anchorMax = new Vector2(0.94f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, 170f);
            rect.anchoredPosition = new Vector2(0f, 650f);

            resultText = resultObject.AddComponent<Text>();
            resultText.font = ResolveFont();
            resultText.fontSize = ResultFontSize;
            resultText.alignment = TextAnchor.MiddleCenter;
            resultText.horizontalOverflow = HorizontalWrapMode.Wrap;
            resultText.verticalOverflow = VerticalWrapMode.Truncate;
            resultText.resizeTextForBestFit = true;
            resultText.resizeTextMinSize = CaptionFontSize;
            resultText.resizeTextMaxSize = ResultFontSize;
            resultText.supportRichText = false;
            resultText.color = ResultTextColor;
            resultText.lineSpacing = DenseLineSpacing;
            NormalizeLayout();
        }

        private void EnsureResultIconStrip()
        {
            if (resultIconStrip != null)
            {
                return;
            }

            EnsurePortraitRoot();
            if (portraitRoot == null)
            {
                return;
            }

            var stripObject = new GameObject("Encounter Result Icon Strip");
            stripObject.transform.SetParent(portraitRoot, false);
            resultIconStrip = stripObject.AddComponent<RectTransform>();
            ApplyResultIconStripRect();

            for (var i = 0; i < ResultIconChipCount; i++)
            {
                CreateResultSummaryChip(i);
            }

            resultIconStrip.gameObject.SetActive(false);
        }

        private void ApplyResultIconStripRect()
        {
            if (resultIconStrip == null)
            {
                return;
            }

            resultIconStrip.anchorMin = new Vector2(0.10f, 0.350f);
            resultIconStrip.anchorMax = new Vector2(0.90f, 0.397f);
            resultIconStrip.pivot = new Vector2(0.5f, 0.5f);
            resultIconStrip.anchoredPosition = Vector2.zero;
            resultIconStrip.sizeDelta = Vector2.zero;
        }

        private void CreateResultSummaryChip(int index)
        {
            var chipObject = new GameObject("Result Summary Chip " + index);
            chipObject.transform.SetParent(resultIconStrip, false);
            var rect = chipObject.AddComponent<RectTransform>();
            var min = index / (float)ResultIconChipCount;
            var max = (index + 1) / (float)ResultIconChipCount;
            rect.anchorMin = new Vector2(min, 0f);
            rect.anchorMax = new Vector2(max, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(-8f, 0f);

            var background = chipObject.AddComponent<Image>();
            background.color = new Color(0.06f, 0.075f, 0.085f, 0.88f);
            background.raycastTarget = false;

            var iconObject = new GameObject("Icon");
            iconObject.transform.SetParent(chipObject.transform, false);
            var iconRect = iconObject.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.06f, 0.18f);
            iconRect.anchorMax = new Vector2(0.38f, 0.82f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = Vector2.zero;
            var icon = iconObject.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var fallbackText = CreateResultChipText(chipObject.transform, "Icon Fallback", new Vector2(0.06f, 0.18f), new Vector2(0.38f, 0.82f), 17, TextAnchor.MiddleCenter);
            fallbackText.color = Color.white;

            var valueText = CreateResultChipText(chipObject.transform, "Value", new Vector2(0.39f, 0f), new Vector2(0.98f, 1f), 24, TextAnchor.MiddleLeft);

            _resultSummaryChips.Add(chipObject);
            _resultSummaryIconImages.Add(icon);
            _resultSummaryFallbackTexts.Add(fallbackText);
            _resultSummaryValueTexts.Add(valueText);
        }

        private Text CreateResultChipText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            var text = textObject.AddComponent<Text>();
            text.font = ResolveFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 13;
            text.resizeTextMaxSize = fontSize;
            text.supportRichText = false;
            text.raycastTarget = false;
            text.color = ResultTextColor;
            return text;
        }

        private void UpdateResultVisibility(PrototypeRunSnapshot snapshot)
        {
            if (resultText == null)
            {
                return;
            }

            var visible = !snapshot.IsInCombat &&
                !ShouldShowCombatDefeatFeedback(snapshot) &&
                !RestInteractionPanelVisible &&
                !_shopPresentationActive &&
                !_eventPresentationActive &&
                !BossGateUiVisible &&
                !EndingUiVisible &&
                !IsMapRouteState(snapshot) &&
                !BossRewardPopupVisible;
            SetResultVisible(visible);
        }

        private void RefreshResultSummaryIcons(string message)
        {
            _activeResultSummaryLabels.Clear();
            _activeResultSummaryValues.Clear();

            if (showRawDebugText || string.IsNullOrEmpty(message))
            {
                if (resultIconStrip != null)
                {
                    resultIconStrip.gameObject.SetActive(false);
                }

                return;
            }

            EnsureResultIconStrip();
            if (resultIconStrip == null)
            {
                return;
            }

            var entries = BuildResultSummaryEntries(message);
            for (var i = 0; i < _resultSummaryChips.Count; i++)
            {
                var visible = i < entries.Count;
                _resultSummaryChips[i].SetActive(visible);
                if (!visible)
                {
                    ClearResultSummaryEntry(i);
                    continue;
                }

                ApplyResultSummaryEntry(i, entries[i]);
                _activeResultSummaryLabels.Add(entries[i].Label);
                _activeResultSummaryValues.Add(entries[i].Value);
            }

            resultIconStrip.gameObject.SetActive(entries.Count > 0 && (resultText == null || resultText.gameObject.activeSelf));
        }

        private void ClearResultSummaryEntry(int index)
        {
            if (index < _resultSummaryIconImages.Count && _resultSummaryIconImages[index] != null)
            {
                _resultSummaryIconImages[index].sprite = null;
            }

            if (index < _resultSummaryFallbackTexts.Count && _resultSummaryFallbackTexts[index] != null)
            {
                _resultSummaryFallbackTexts[index].text = string.Empty;
            }

            if (index < _resultSummaryValueTexts.Count && _resultSummaryValueTexts[index] != null)
            {
                _resultSummaryValueTexts[index].text = string.Empty;
            }
        }

        private void ApplyResultSummaryEntry(int index, ResultSummaryEntry entry)
        {
            var icon = index < _resultSummaryIconImages.Count ? _resultSummaryIconImages[index] : null;
            var fallback = index < _resultSummaryFallbackTexts.Count ? _resultSummaryFallbackTexts[index] : null;
            var value = index < _resultSummaryValueTexts.Count ? _resultSummaryValueTexts[index] : null;

            var sprite = ResolveIcon(entry.IconKey);
            if (icon != null)
            {
                icon.sprite = sprite;
                icon.color = sprite == null ? entry.FallbackColor : Color.white;
                icon.gameObject.SetActive(true);
            }

            if (fallback != null)
            {
                fallback.text = sprite == null ? ShortResultLabel(entry.Label) : string.Empty;
                fallback.gameObject.SetActive(sprite == null);
            }

            if (value != null)
            {
                value.text = entry.Value;
            }
        }

        private static List<ResultSummaryEntry> BuildResultSummaryEntries(string message)
        {
            var entries = new List<ResultSummaryEntry>();
            var hpChange = ExtractHpChange(message);
            if (hpChange.StartsWith("HP ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "HP", hpChange.Substring("HP ".Length).Trim(), string.Empty, new Color(0.62f, 0.22f, 0.22f, 0.95f));
            }

            if (string.IsNullOrEmpty(message))
            {
                return entries;
            }

            var tokens = message.Split(new[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                AppendResultSummaryEntryFromToken(entries, tokens[i].Trim());
                if (entries.Count >= ResultIconChipCount)
                {
                    break;
                }
            }

            return entries;
        }

        private static void AppendResultSummaryEntryFromToken(List<ResultSummaryEntry> entries, string token)
        {
            if (string.IsNullOrEmpty(token) || token.StartsWith("Glitch ", StringComparison.Ordinal) || token.StartsWith("glitch ", StringComparison.Ordinal))
            {
                return;
            }

            if (token.StartsWith("HP ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "HP", token.Substring("HP ".Length).Trim(), string.Empty, new Color(0.62f, 0.22f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("Gold ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Gold", token.Substring("Gold ".Length).Trim(), "resource.gold", new Color(0.82f, 0.62f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("gold reward ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Gold", "+" + token.Substring("gold reward ".Length).Trim().TrimStart('+'), "resource.gold", new Color(0.82f, 0.62f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("xp +", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "XP", "+" + token.Substring("xp +".Length).Trim().TrimStart('+'), string.Empty, new Color(0.26f, 0.48f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("xp ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "XP", token.Substring("xp ".Length).Trim(), string.Empty, new Color(0.26f, 0.48f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("level reward ready", StringComparison.Ordinal) || token.StartsWith("level ready", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Level", "보상", string.Empty, new Color(0.38f, 0.64f, 0.62f, 0.95f));
                return;
            }

            if (token.StartsWith("Affinity ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Affinity", token.Substring("Affinity ".Length).Trim(), "resource.affinity", new Color(0.55f, 0.34f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("affinity ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Affinity", token.Substring("affinity ".Length).Trim(), "resource.affinity", new Color(0.55f, 0.34f, 0.72f, 0.95f));
                return;
            }

            if (token.StartsWith("item ", StringComparison.Ordinal))
            {
                var payload = token.Substring("item ".Length).Trim();
                AppendResultSummaryEntry(entries, "Item", ExtractRefDeltaValue(payload), ResultIconKeyForRef(ExtractRefId(payload)), new Color(0.30f, 0.48f, 0.42f, 0.95f));
                return;
            }

            if (token.StartsWith("ability ", StringComparison.Ordinal))
            {
                var payload = token.Substring("ability ".Length).Trim();
                AppendResultSummaryEntry(entries, "Ability", ExtractRefDeltaValue(payload), ResultIconKeyForRef(ExtractRefId(payload)), new Color(0.55f, 0.50f, 0.25f, 0.95f));
                return;
            }

            if (token.StartsWith("memory unlocked ", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "기억의 잔향", "획득", "resource.memory", new Color(0.30f, 0.50f, 0.68f, 0.95f));
                return;
            }

            if (token == PrototypeRunState.MemoryFragmentPublicFeedback)
            {
                AppendResultSummaryEntry(entries, "기억의 잔향", "획득", "resource.memory", new Color(0.30f, 0.50f, 0.68f, 0.95f));
                return;
            }

            if (token == PrototypeRunState.MemoryConsequenceFeedback)
            {
                AppendResultSummaryEntry(entries, "기억", "기록", "resource.memory", new Color(0.30f, 0.50f, 0.68f, 0.95f));
                return;
            }

            if (token.StartsWith("jar outcome: Gold +8", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "Gold", "+8", "resource.gold", new Color(0.82f, 0.62f, 0.22f, 0.95f));
                return;
            }

            if (token.StartsWith("jar outcome: HP +5", StringComparison.Ordinal))
            {
                AppendResultSummaryEntry(entries, "HP", "+5", string.Empty, new Color(0.62f, 0.22f, 0.22f, 0.95f));
            }
        }

        private static void AppendResultSummaryEntry(List<ResultSummaryEntry> entries, string label, string value, string iconKey, Color fallbackColor)
        {
            if (entries.Count >= ResultIconChipCount || string.IsNullOrEmpty(value) || IsNoOpDelta(value))
            {
                return;
            }

            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].Label == label && entries[i].Value == value)
                {
                    return;
                }
            }

            entries.Add(new ResultSummaryEntry(label, value, iconKey, fallbackColor));
        }

        private static string ExtractRefId(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var parts = value.Split(' ');
            return parts.Length == 0 ? string.Empty : parts[0];
        }

        private static string ExtractRefDeltaValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "+1";
            }

            var parts = value.Split(' ');
            return parts.Length > 1 ? parts[parts.Length - 1] : "+1";
        }

        private static string ResultIconKeyForRef(string reference)
        {
            return reference switch
            {
                "ITEM_FIELD_BANDAGE" => "item.field_bandage",
                "ITEM_LANTERN_OIL" => "item.lantern_oil",
                "ITEM_TORN_CHARM" => "item.torn_charm",
                "ABILITY_SCOUT" => "ability.scout",
                "ABILITY_RECALL_ANCHOR" => "ability.recall_anchor",
                _ => string.Empty
            };
        }

        private static string ShortResultLabel(string label)
        {
            return label switch
            {
                "Gold" => "G",
                "Affinity" => "신",
                "Item" => "I",
                "Ability" => "A",
                "Memory" => "기",
                _ => label
            };
        }

        private string BuildResultSummary(string message)
        {
            if (showRawDebugText)
            {
                return "result: " + message;
            }

            if (IsBossClearResult(message))
            {
                return BuildBossClearResultSummary(message);
            }

            if (message.StartsWith("선택 가능한 길", StringComparison.Ordinal) ||
                message.StartsWith("아이콘을 보고", StringComparison.Ordinal))
            {
                return "결과\n다음 선택\n밝은 노드를 선택하세요";
            }

            var summary = "결과";
            AppendResultLine(ref summary, ExtractHpChange(message));
            AppendEffectTokens(ref summary, message);

            if (message.Contains("combat started"))
            {
                AppendResultLine(ref summary, "전투 시작");
            }

            if (message.Contains("enemyDefeated True", StringComparison.Ordinal) || message.Contains("victory", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "승리");
            }
            else if (message.Contains("defeat", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "패배");
            }

            if (message.Contains("enemyDefeated True", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "적 처치", allowOverflow: false);
            }

            if (message.Contains("already resolved:", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "이미 해결됨");
            }

            if (message.Contains("stair unlocked", StringComparison.OrdinalIgnoreCase))
            {
                AppendResultLine(ref summary, "다음 층 준비");
            }

            AppendFloorProgressionLine(ref summary, message);

            if (message.Contains("run.clear", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "최종 보스 격파");
                AppendResultLine(ref summary, "엔딩 선택 가능", allowOverflow: true);
            }

            if (message.Contains("ending.rest", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "안식 선택 완료");
            }

            if (message.Contains("ending.continue", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "동행 계속 선택");
            }

            if (message.Contains("run.failed", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "실패");
            }

            if (message.Contains("run.restartReady", StringComparison.Ordinal))
            {
                AppendResultLine(ref summary, "재시작 가능");
            }

            return summary == "결과" ? "결과\n-" : summary;
        }

        private static bool IsBossClearResult(string message)
        {
            return !string.IsNullOrEmpty(message) &&
                message.Contains("enemyDefeated True", StringComparison.Ordinal) &&
                (message.Contains("stair unlocked", StringComparison.OrdinalIgnoreCase) ||
                 message.Contains("run.clear", StringComparison.Ordinal));
        }

        private static string BuildBossClearResultSummary(string message)
        {
            var summary = "결과";
            AppendResultLine(ref summary, message.Contains("run.clear", StringComparison.Ordinal) ? "최종 보스 격파" : "보스 격파", allowOverflow: true);
            AppendResultLine(ref summary, ExtractTokenResult(message, "gold reward "), allowOverflow: true);
            AppendResultLine(ref summary, ExtractTokenResult(message, "affinity "), allowOverflow: true);
            AppendResultLine(ref summary, message.Contains("run.clear", StringComparison.Ordinal) ? "엔딩 선택 가능" : "다음 층 준비", allowOverflow: true);
            return summary;
        }

        private static string ExtractTokenResult(string message, string tokenPrefix)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(tokenPrefix))
            {
                return string.Empty;
            }

            var tokens = message.Split(new[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var normalized = NormalizeResultToken(tokens[i].Trim());
                if (!string.IsNullOrEmpty(normalized) && tokens[i].Trim().StartsWith(tokenPrefix, StringComparison.Ordinal))
                {
                    return normalized;
                }
            }

            return string.Empty;
        }

        private static void AppendEffectTokens(ref string summary, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var tokens = message.Split(new[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                AppendResultLine(ref summary, NormalizeResultToken(tokens[i].Trim()));
            }
        }

        private static string NormalizeResultToken(string token)
        {
            if (string.IsNullOrEmpty(token) ||
                token.StartsWith("choices:", StringComparison.Ordinal) ||
                token.StartsWith("choice applied:", StringComparison.Ordinal) ||
                token.StartsWith("effects=", StringComparison.Ordinal) ||
                token.StartsWith("ignored=", StringComparison.Ordinal) ||
                token.StartsWith("combat ", StringComparison.Ordinal) ||
                token.StartsWith("round ", StringComparison.Ordinal) ||
                token.StartsWith("enemy ", StringComparison.Ordinal) ||
                token.StartsWith("result ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.StartsWith("HP ", StringComparison.Ordinal) ||
                token.StartsWith("Gold ", StringComparison.Ordinal) ||
                token.StartsWith("Mental ", StringComparison.Ordinal) ||
                token.StartsWith("Affinity ", StringComparison.Ordinal))
            {
                if (IsNoOpDelta(token.Substring(token.IndexOf(' ') + 1)))
                {
                    return string.Empty;
                }

                return token
                    .Replace("Mental ", "정신 ", StringComparison.Ordinal)
                    .Replace("Affinity ", "신뢰 ", StringComparison.Ordinal);
            }

            if (token.StartsWith("Glitch ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.Contains("HP restored", StringComparison.Ordinal))
            {
                return "HP 회복";
            }

            if (token.Contains("training buff +1 next combat", StringComparison.Ordinal))
            {
                return "다음 전투 피해 +1";
            }

            if (token.Contains("Mataios response ready", StringComparison.Ordinal))
            {
                return "마타이오스 응답";
            }

            if (token.StartsWith("rest ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.StartsWith("gold reward ", StringComparison.Ordinal))
            {
                return "Gold +" + token.Substring("gold reward ".Length).Trim().TrimStart('+');
            }

            if (token.StartsWith("glitch ", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (token.StartsWith("affinity ", StringComparison.Ordinal))
            {
                return "신뢰 " + token.Substring("affinity ".Length).Trim();
            }

            if (token.StartsWith("playerDamage ", StringComparison.Ordinal))
            {
                return "적 피해 " + token.Substring("playerDamage ".Length).Trim();
            }

            if (token.StartsWith("enemyDamage ", StringComparison.Ordinal))
            {
                return "받은 피해 " + token.Substring("enemyDamage ".Length).Trim();
            }

            if (token.StartsWith("combo ", StringComparison.Ordinal))
            {
                return "콤보 피해 " + token.Substring("combo ".Length).Trim();
            }

            if (token.StartsWith("item ", StringComparison.Ordinal))
            {
                return "아이템 " + ExtractRefDeltaValue(token.Substring("item ".Length).Trim());
            }

            if (token.StartsWith("ability ", StringComparison.Ordinal))
            {
                return "능력 " + ExtractRefDeltaValue(token.Substring("ability ".Length).Trim());
            }

            if (token.StartsWith("reward ", StringComparison.Ordinal))
            {
                if (token.Contains(PrototypeRunState.MemoryConsequenceRewardBundleRef, StringComparison.Ordinal))
                {
                    return PrototypeRunState.MemoryConsequenceFeedback;
                }

                return "보상: " + NormalizeRefDelta(token.Substring("reward ".Length).Trim());
            }

            if (token.StartsWith("memory unlocked ", StringComparison.Ordinal))
            {
                return PrototypeRunState.MemoryFragmentPublicFeedback;
            }

            if (token == PrototypeRunState.MemoryFragmentPublicFeedback ||
                token == PrototypeRunState.MemoryConsequenceFeedback)
            {
                return token;
            }

            if (token.StartsWith("action ", StringComparison.Ordinal))
            {
                return "행동: " + PublicCombatActionName(token.Substring("action ".Length).Trim());
            }

            if (token.StartsWith("jar outcome: Gold +8", StringComparison.Ordinal))
            {
                return "Gold +8";
            }

            if (token.StartsWith("jar outcome: elite combat", StringComparison.Ordinal))
            {
                return "엘리트 전투 발생";
            }

            if (token.StartsWith("jar outcome: HP +5", StringComparison.Ordinal))
            {
                return "HP +5";
            }

            if (token.StartsWith("jar outcome: next 3 combat damage buff", StringComparison.Ordinal))
            {
                return "다음 3회 전투 피해 증가";
            }

            return string.Empty;
        }

        private static string NormalizeRefDelta(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var parts = value.Split(' ');
            if (parts.Length == 0)
            {
                return value;
            }

            var label = PublicRefName(parts[0]);
            return parts.Length > 1 ? label + " " + parts[1] : label;
        }

        private static string ExtractHpChange(string message)
        {
            var token = "player HP ";
            var index = string.IsNullOrEmpty(message) ? -1 : message.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
            {
                return string.Empty;
            }

            var start = index + token.Length;
            var end = message.IndexOf(" |", start, StringComparison.Ordinal);
            if (end < 0)
            {
                end = message.Length;
            }

            var value = message.Substring(start, end - start).Trim();
            var arrow = value.IndexOf("->", StringComparison.Ordinal);
            if (arrow < 0)
            {
                return "HP " + value;
            }

            if (int.TryParse(value.Substring(0, arrow).Trim(), out var before) &&
                int.TryParse(value.Substring(arrow + 2).Trim(), out var after))
            {
                var delta = after - before;
                return delta == 0 ? string.Empty : "HP " + FormatDelta(delta);
            }

            return "HP " + value;
        }

        private static void AppendFloorProgressionLine(ref string summary, string message)
        {
            var floorIndex = string.IsNullOrEmpty(message) ? -1 : message.IndexOf("floor ", StringComparison.Ordinal);
            if (floorIndex < 0 || !message.Contains(" entered", StringComparison.Ordinal))
            {
                return;
            }

            var start = floorIndex + "floor ".Length;
            var end = message.IndexOf(" entered", start, StringComparison.Ordinal);
            if (end > start)
            {
                AppendResultLine(ref summary, "Floor " + message.Substring(start, end - start).Trim() + " 진입");
            }
        }

        private static void AppendResultLine(ref string summary, string line, bool allowOverflow = false)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            var needle = "\n" + line;
            if (summary.EndsWith(needle, StringComparison.Ordinal) || summary.Contains(needle + "\n", StringComparison.Ordinal))
            {
                return;
            }

            if (!allowOverflow && CountResultLines(summary) >= ResultLineLimit)
            {
                return;
            }

            summary += "\n" + line;
        }

        private static int CountResultLines(string summary)
        {
            if (string.IsNullOrEmpty(summary) || summary == "결과")
            {
                return 0;
            }

            var count = 0;
            for (var i = 0; i < summary.Length; i++)
            {
                if (summary[i] == '\n')
                {
                    count++;
                }
            }

            return count;
        }
    }
}
