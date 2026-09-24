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
        private static string ResolvePublicChoiceLabel(string choiceStableId, int index)
        {
            if (!string.IsNullOrEmpty(choiceStableId))
            {
                if (choiceStableId.Contains("SHOP", StringComparison.Ordinal) &&
                    choiceStableId.Contains("_LEAVE", StringComparison.Ordinal))
                {
                    return "상점 나가기";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_PATTERNED" ||
                    choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_PATTERNED_ELITE_COMBAT")
                {
                    return "신기한 문양이 각인된 항아리";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_PLAIN")
                {
                    return "평범한 항아리";
                }

                if (choiceStableId == "CHOICE_EVT_F01_JAR_ROOM_CRACKED")
                {
                    return "금 간 항아리";
                }

                if (choiceStableId.Contains("_BUY_", StringComparison.Ordinal))
                {
                    return "구매";
                }

                if (choiceStableId.Contains("_LEAVE", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_CONTINUE", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_IGNORE", StringComparison.Ordinal))
                {
                    return "지나간다";
                }

                if (choiceStableId.Contains("_UNLOCK", StringComparison.Ordinal))
                {
                    return "기억의 잔향";
                }

                if (choiceStableId.Contains("_WITHDRAW", StringComparison.Ordinal))
                {
                    return "보류";
                }

                if (choiceStableId.Contains("_ENGAGE", StringComparison.Ordinal))
                {
                    return "전투 시작";
                }

                if (choiceStableId.Contains("_PREPARE", StringComparison.Ordinal))
                {
                    return "준비";
                }

                if (choiceStableId.Contains("_AID", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_HELP", StringComparison.Ordinal))
                {
                    return "돕는다";
                }

                if (choiceStableId.Contains("_REFUSE", StringComparison.Ordinal))
                {
                    return "거절한다";
                }

                if (choiceStableId.Contains("_TRADE", StringComparison.Ordinal) ||
                    choiceStableId.Contains("_BARGAIN", StringComparison.Ordinal))
                {
                    return "거래한다";
                }

                if (choiceStableId.Contains("_REST", StringComparison.Ordinal))
                {
                    return "휴식";
                }
            }

            return choiceStableId switch
            {
                "CHOICE_SHOP_01_BUY_ITEM" => "구매",
                "CHOICE_SHOP_01_BUY_ABILITY" => "구매",
                "CHOICE_SHOP_01_LEAVE" => "지나간다",
                "CHOICE_MORAL_01_AID" => "돕는다",
                "CHOICE_MORAL_01_REFUSE" => "거절한다",
                "CHOICE_MORAL_01_TRADE" => "거래한다",
                "CHOICE_MEMORY_01_UNLOCK" => "기억의 잔향",
                "CHOICE_MEMORY_01_WITHDRAW" => "보류",
                "CHOICE_COMBAT_01_ENGAGE" => "전투",
                "CHOICE_F02_SHOP_BUY_ITEM" => "구매",
                "CHOICE_F02_SHOP_BUY_ABILITY" => "구매",
                "CHOICE_F02_SHOP_LEAVE" => "지나간다",
                "CHOICE_F02_MORAL_HELP" => "돕는다",
                "CHOICE_F02_MORAL_LEAVE" => "거절한다",
                "CHOICE_F02_MORAL_BARGAIN" => "거래한다",
                "CHOICE_COMBAT_02_ENGAGE" => "전투",
                "CHOICE_COMBAT_03_ENGAGE" => "전투",
                _ => "선택 " + (index + 1)
            };
        }

        private static string ResolvePublicEncounterLabel(string configuredLabel, EncounterData encounter)
        {
            if (!string.IsNullOrEmpty(configuredLabel) && !LooksLikeInternalLabel(configuredLabel))
            {
                return SanitizePublicText(configuredLabel);
            }

            if (encounter == null)
            {
                return "Encounter";
            }

            return encounter.Id switch
            {
                "ENC_SHOP_01" => "상점",
                "ENC_MORAL_CHOICE_01" => "선택",
                "ENC_MEMORY_FRAGMENT_01" => "기억의 잔향",
                "ENC_COMBAT_GATE_01" => "전투",
                "ENC_F02_SHOP_001" => "상점",
                "ENC_F02_MORAL_CHOICE_001" => "선택",
                "ENC_COMBAT_GATE_02" => "보스 관문",
                "ENC_COMBAT_GATE_03" => "최종 보스",
                _ => encounter.Type == EncounterType.MoralChoice ? "선택" :
                    encounter.Type == EncounterType.MemoryFragment ? "기억의 잔향" :
                    encounter.Type == EncounterType.Shop ? "상점" :
                    "조우"
            };
        }

        private static string ResolvePublicNpcReaction(string reactionKey)
        {
            if (string.IsNullOrEmpty(reactionKey))
            {
                return "-";
            }

            if (reactionKey.Contains("SHOP", StringComparison.OrdinalIgnoreCase))
            {
                return "상점 반응";
            }

            if (reactionKey.Contains("MORAL", StringComparison.OrdinalIgnoreCase))
            {
                return "선택 반응";
            }

            if (reactionKey.Contains("MEMORY", StringComparison.OrdinalIgnoreCase))
            {
                return "기억 반응";
            }

            if (reactionKey.Contains("COMBAT", StringComparison.OrdinalIgnoreCase) || reactionKey.Contains("BATTLE", StringComparison.OrdinalIgnoreCase))
            {
                return "전투 반응";
            }

            if (reactionKey.Contains("FLOOR", StringComparison.OrdinalIgnoreCase) || reactionKey.Contains("STAIR", StringComparison.OrdinalIgnoreCase))
            {
                return "층 이동 반응";
            }

            if (reactionKey.Contains("RECALL", StringComparison.OrdinalIgnoreCase))
            {
                return "회상 반응";
            }

            return "동행자 반응";
        }

        private static string NormalizePublicHint(string hint)
        {
            if (string.IsNullOrEmpty(hint))
            {
                return string.Empty;
            }

            var normalized = hint
                .Replace("Unavailable:", "선택 불가:", StringComparison.Ordinal)
                .Replace("선택 불가: Gold 부족", "구매 불가: Gold 부족", StringComparison.Ordinal)
                .Replace("Combat start", "전투 시작", StringComparison.Ordinal)
                .Replace("Memory unlock", "기억의 잔향 해금", StringComparison.Ordinal)
                .Replace("Ability 필요", "능력 필요", StringComparison.Ordinal)
                .Replace("Glitch -1", "불안 감소", StringComparison.Ordinal)
                .Replace("Glitch +1", "불안 증가", StringComparison.Ordinal);

            return SanitizePublicText(ReplacePublicRefs(normalized));
        }

        private static string ReplacePublicRefs(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var replaced = value
                .Replace("TriggerGameOver", "실패", StringComparison.Ordinal)
                .Replace("MoralChoice", "선택", StringComparison.Ordinal)
                .Replace("도덕 선택", "선택", StringComparison.Ordinal)
                .Replace("MemoryFragment", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_01", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_02", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_03", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_04", "기억의 잔향", StringComparison.Ordinal)
                .Replace("MEM_FRAGMENT_05", "기억의 잔향", StringComparison.Ordinal)
                .Replace("ITEM_FIELD_BANDAGE", PublicRefName("ITEM_FIELD_BANDAGE"), StringComparison.Ordinal)
                .Replace("ITEM_LANTERN_OIL", PublicRefName("ITEM_LANTERN_OIL"), StringComparison.Ordinal)
                .Replace("ITEM_TORN_CHARM", PublicRefName("ITEM_TORN_CHARM"), StringComparison.Ordinal)
                .Replace("ITEM_10", PublicRefName("ITEM_10"), StringComparison.Ordinal)
                .Replace("ITEM_01", PublicRefName("ITEM_01"), StringComparison.Ordinal)
                .Replace("ITEM_02", PublicRefName("ITEM_02"), StringComparison.Ordinal)
                .Replace("ITEM_03", PublicRefName("ITEM_03"), StringComparison.Ordinal)
                .Replace("ITEM_04", PublicRefName("ITEM_04"), StringComparison.Ordinal)
                .Replace("ITEM_05", PublicRefName("ITEM_05"), StringComparison.Ordinal)
                .Replace("ITEM_09", PublicRefName("ITEM_09"), StringComparison.Ordinal)
                .Replace("RELIC_GENERIC_01", PublicRefName("RELIC_GENERIC_01"), StringComparison.Ordinal)
                .Replace("RELIC_SWORD_01", PublicRefName("RELIC_SWORD_01"), StringComparison.Ordinal)
                .Replace("RELIC_LINE_01", PublicRefName("RELIC_LINE_01"), StringComparison.Ordinal)
                .Replace("RELIC_ARTS_01", PublicRefName("RELIC_ARTS_01"), StringComparison.Ordinal)
                .Replace("RELIC_GUARD_01", PublicRefName("RELIC_GUARD_01"), StringComparison.Ordinal)
                .Replace("ABILITY_SCOUT", PublicRefName("ABILITY_SCOUT"), StringComparison.Ordinal)
                .Replace("ABILITY_RECALL_ANCHOR", PublicRefName("ABILITY_RECALL_ANCHOR"), StringComparison.Ordinal)
                .Replace("ABILITY_SWORD_01", PublicRefName("ABILITY_SWORD_01"), StringComparison.Ordinal)
                .Replace("ABILITY_SWORD_02", PublicRefName("ABILITY_SWORD_02"), StringComparison.Ordinal)
                .Replace("ABILITY_SWORD_03", PublicRefName("ABILITY_SWORD_03"), StringComparison.Ordinal)
                .Replace("ABILITY_ARTS_03", PublicRefName("ABILITY_ARTS_03"), StringComparison.Ordinal)
                .Replace("ABILITY_GUARD_01", PublicRefName("ABILITY_GUARD_01"), StringComparison.Ordinal)
                .Replace("REWARD_CACHE_SMALL", PublicRefName("REWARD_CACHE_SMALL"), StringComparison.Ordinal)
                .Replace("REWARD_CACHE_MEMORY", PublicRefName("REWARD_CACHE_MEMORY"), StringComparison.Ordinal);
            return StripInternalReferenceTokens(replaced);
        }

        private static string SanitizePublicText(string value)
        {
            return StripInternalReferenceTokens(value);
        }

        private static string StripInternalReferenceTokens(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var text = value
                .Replace("[current]", "현재", StringComparison.Ordinal)
                .Replace("[locked]", "잠김", StringComparison.Ordinal)
                .Replace("[complete]", "완료", StringComparison.Ordinal)
                .Replace("[cleared]", "완료", StringComparison.Ordinal)
                .Replace("MoralChoice", "선택", StringComparison.Ordinal)
                .Replace("MemoryFragment", "기억의 잔향", StringComparison.Ordinal)
                .Replace("TriggerGameOver", "실패", StringComparison.Ordinal)
                .Replace("도덕 선택", "선택", StringComparison.Ordinal)
                .Replace("Unavailable", "선택 불가", StringComparison.Ordinal)
                .Replace("PLACEHOLDER_", string.Empty, StringComparison.Ordinal);

            text = ReplaceInternalTokenPrefix(text, "CHOICE_", "선택");
            text = ReplaceInternalTokenPrefix(text, "ENC_COMBAT_GATE_", "전투");
            text = ReplaceInternalTokenPrefix(text, "ENC_SHOP_", "상점");
            text = ReplaceInternalTokenPrefix(text, "ENC_REST_", "휴식");
            text = ReplaceInternalTokenPrefix(text, "ENC_MORAL_", "선택");
            text = ReplaceInternalTokenPrefix(text, "ENC_MEMORY_", "기억의 잔향");
            text = ReplaceInternalTokenPrefix(text, "ENC_", "조우");
            text = ReplaceInternalTokenPrefix(text, "EVT_", "이벤트");
            text = ReplaceInternalTokenPrefix(text, "MEM_FRAGMENT_", "기억의 잔향");
            while (text.Contains("  ", StringComparison.Ordinal))
            {
                text = text.Replace("  ", " ", StringComparison.Ordinal);
            }

            return text.Trim();
        }

        private static string ReplaceInternalTokenPrefix(string value, string prefix, string replacement)
        {
            var result = value;
            var searchStart = 0;
            while (searchStart < result.Length)
            {
                var start = result.IndexOf(prefix, searchStart, StringComparison.Ordinal);
                if (start < 0)
                {
                    break;
                }

                var end = start + prefix.Length;
                while (end < result.Length && IsInternalTokenCharacter(result[end]))
                {
                    end++;
                }

                result = result.Substring(0, start) + replacement + result.Substring(end);
                searchStart = start + replacement.Length;
            }

            return result;
        }

        private static bool IsInternalTokenCharacter(char value)
        {
            return char.IsLetterOrDigit(value) || value == '_' || value == '-' || value == '.';
        }

        private static string PublicRefName(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return string.Empty;
            }

            return reference switch
            {
                "ITEM_FIELD_BANDAGE" => "붕대",
                "ITEM_LANTERN_OIL" => "등유",
                "ITEM_TORN_CHARM" => "찢어진 부적",
                "ITEM_01" => "붕대 뭉치",
                "ITEM_02" => "작은 룬석",
                "ITEM_03" => "날카로운 숫돌",
                "ITEM_04" => "낡은 방패 조각",
                "ITEM_05" => "독침",
                "ITEM_09" => "마모된 부적",
                "ITEM_10" => "피의 계약서",
                "RELIC_GENERIC_01" => "회귀자의 낡은 코트",
                "RELIC_SWORD_01" => "피묻은 칼날",
                "RELIC_LINE_01" => "저격수의 망원경",
                "RELIC_ARTS_01" => "원소 수정",
                "RELIC_GUARD_01" => "강화 방패",
                "ABILITY_SCOUT" => "정찰",
                "ABILITY_RECALL_ANCHOR" => "회상 닻",
                "ABILITY_SWORD_01" => "예리한 감각",
                "ABILITY_SWORD_02" => "연속베기",
                "ABILITY_SWORD_03" => "피의 서약",
                "ABILITY_ARTS_03" => "번개 방출",
                "ABILITY_GUARD_01" => "철벽의 태세",
                "REWARD_CACHE_SMALL" => "작은 보급품",
                "REWARD_CACHE_MEMORY" => PrototypeRunState.MemoryConsequenceFeedback,
                "MEM_FRAGMENT_01" => "기억의 잔향",
                "MEM_FRAGMENT_02" => "기억의 잔향",
                "MEM_FRAGMENT_03" => "기억의 잔향",
                "MEM_FRAGMENT_04" => "기억의 잔향",
                "MEM_FRAGMENT_05" => "기억의 잔향",
                _ => LooksLikeInternalLabel(reference) || reference.Contains("_", StringComparison.Ordinal) ? "획득물" : reference
            };
        }

        private static string PublicCommandName(string commandId)
        {
            return commandId switch
            {
                PrototypeRunState.CommandAttackId => "공격",
                PrototypeRunState.CommandDefendId => "방어",
                PrototypeRunState.CommandScoutId => "정찰",
                PrototypeRunState.CommandArts03Id => "번개 방출",
                _ => string.IsNullOrEmpty(commandId) ? "command" : "스킬"
            };
        }

        private static string PublicEnemyName(string enemyId)
        {
            if (string.IsNullOrEmpty(enemyId))
            {
                return "적";
            }

            return enemyId switch
            {
                "ENEMY_BANDIT_MELEE_01" => "도적",
                "ENEMY_BANDIT_RANGED_01" => "원거리 도적",
                "ENEMY_SLIME_01" => "슬라임",
                "ENEMY_SKELETON_01" => "해골",
                "ENEMY_WILD_BEAST_01" => "들짐승",
                "ENEMY_EMPTY_ARMOR" => "리빙 아머",
                "ENEMY_SHADE_03" => "그림자",
                "ENEMY_WRAITH_04" => "망령",
                "ENEMY_FRACTURE_HOUND" => "변이된 들짐승",
                "ENEMY_LAMPLIGHTER_01" => "점등인",
                "BOSS_GATE_01" => "층 보스",
                "BOSS_APEX_02" => "최종 보스",
                _ => "적"
            };
        }

        private static bool IsBossEnemyId(string enemyId)
        {
            return enemyId == "BOSS_GATE_01" || enemyId == "BOSS_APEX_02";
        }

        private static string PublicCombatActionName(string action)
        {
            return action switch
            {
                "Attack" => "공격",
                "Defend" => "방어",
                "Skill" => "기술",
                _ => action
            };
        }

        private static string PublicCombatResultName(string resultId)
        {
            if (string.IsNullOrEmpty(resultId))
            {
                return "-";
            }

            if (resultId.Contains("victory", StringComparison.OrdinalIgnoreCase))
            {
                return "승리";
            }

            if (resultId.Contains("defeat", StringComparison.OrdinalIgnoreCase))
            {
                return "패배";
            }

            if (resultId.Contains("recall", StringComparison.OrdinalIgnoreCase))
            {
                return "회상 개입";
            }

            return resultId.Contains("_", StringComparison.Ordinal) ? "처리됨" : resultId;
        }

        private static bool LooksLikeInternalLabel(string value)
        {
            return value == "MoralChoice" ||
                   value == "MemoryFragment" ||
                   value == "CombatGate" ||
                   value == "DemoComplete" ||
                   value == "Shop" ||
                   value == "Boss Gate" ||
                   value == "Final Boss" ||
                   value == "Decision" ||
                   value == "Memory" ||
                   value == "Combat";
        }

        private static bool IsNoOpDelta(string value)
        {
            var arrow = value.IndexOf("->", StringComparison.Ordinal);
            if (arrow < 0)
            {
                return false;
            }

            var before = value.Substring(0, arrow).Trim();
            var after = value.Substring(arrow + 2).Trim();
            return before == after;
        }
    }
}
