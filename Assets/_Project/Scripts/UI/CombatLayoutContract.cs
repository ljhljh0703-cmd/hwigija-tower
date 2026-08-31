using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct CombatSlot
    {
        public CombatSlot(
            string key,
            float x,
            float y,
            float width,
            float height,
            string layer,
            string type,
            string role,
            bool interactive,
            bool hasFrame,
            string content,
            string label,
            string colorToken,
            int fontSize)
        {
            Key = key;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Layer = layer;
            Type = type;
            Role = role;
            Interactive = interactive;
            HasFrame = hasFrame;
            Content = content;
            Label = label;
            ColorToken = colorToken;
            FontSize = fontSize;
        }

        public string Key { get; }
        public float X { get; }
        public float Y { get; }
        public float Width { get; }
        public float Height { get; }
        public string Layer { get; }
        public string Type { get; }
        public string Role { get; }
        public bool Interactive { get; }
        public bool HasFrame { get; }
        public string Content { get; }
        public string Label { get; }
        public string ColorToken { get; }
        public int FontSize { get; }
    }

    public static class CombatLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;

        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "party", "action" };

        public static readonly CombatSlot FloorChip = Slot("floorChip", 0.04f, 0.03f, 0.18f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "층", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly CombatSlot SanityChip = Slot("sanityChip", 0.24f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot HpChip = Slot("hpChip", 0.50f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot GoldChip = Slot("goldChip", 0.76f, 0.03f, 0.20f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "골드", "gold_coin", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot ThreatReadout = Slot("threatReadout", 0.06f, 0.108f, 0.88f, 0.062f, "judgement", "glyphValue", string.Empty, false, true, "적의 다음 행동 · 예상 피해 · 내가 버틸 수 있는 턴", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly CombatSlot EnemyPanel = Slot("enemyPanel", 0.063f, 0.186f, 0.874f, 0.368f, "background", string.Empty, string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0);
        public static readonly CombatSlot EnemyTitle = Slot("enemyTitle", 0.10f, 0.196f, 0.80f, 0.038f, "enemy", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly CombatSlot EnemyHpBar = Slot("enemyHpBar", 0.148f, 0.238f, 0.704f, 0.022f, "enemy", "gauge", string.Empty, false, false, string.Empty, "적 체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot EnemyStatusChips = Slot("enemyStatusChips", 0.22f, 0.266f, 0.56f, 0.028f, "enemy", "chipRow", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.LabelFontSize);
        public static readonly CombatSlot EnemyImage = Slot("enemyImage", 0.20f, 0.30f, 0.60f, 0.246f, "enemy", "image", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly CombatSlot DamageEffectOverlay = Slot("damageEffectOverlay", 0.40f, 0.32f, 0.20f, 0.10f, "overlay", "effect", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly CombatSlot PlayerCard = Slot("playerCard", 0.055f, 0.572f, 0.42f, 0.152f, "party", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0);
        public static readonly CombatSlot PlayerHpBar = Slot("playerHpBar", 0.075f, 0.678f, 0.38f, 0.016f, "party", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot PlayerSanityBar = Slot("playerSanityBar", 0.075f, 0.70f, 0.38f, 0.016f, "party", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot MataiosCard = Slot("mataiosCard", 0.525f, 0.572f, 0.42f, 0.152f, "party", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0);
        public static readonly CombatSlot MataiosHpBar = Slot("mataiosHpBar", 0.545f, 0.678f, 0.38f, 0.016f, "party", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot MataiosSanityBar = Slot("mataiosSanityBar", 0.545f, 0.70f, 0.38f, 0.016f, "party", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly CombatSlot AttackButton = Slot("attackButton", 0.06f, 0.76f, 0.27f, 0.11f, "actions", "button", string.Empty, true, true, "공격", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly CombatSlot DefendButton = Slot("defendButton", 0.365f, 0.76f, 0.27f, 0.11f, "actions", "button", string.Empty, true, true, "방어", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly CombatSlot SkillButton = Slot("skillButton", 0.67f, 0.76f, 0.27f, 0.11f, "actions", "button", string.Empty, true, true, "정찰", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly CombatSlot CombatLog = Slot("combatLog", 0.06f, 0.888f, 0.70f, 0.056f, "log", "collapsible", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.MicroFontSize);
        public static readonly CombatSlot ItemInspectButton = Slot("itemInspectButton", 0.79f, 0.888f, 0.15f, 0.056f, "actions", "button", string.Empty, true, false, "소지품", string.Empty, string.Empty, UiTokenContract.MicroFontSize);

        public static readonly CombatSlot[] AllSlots =
        {
            FloorChip,
            SanityChip,
            HpChip,
            GoldChip,
            ThreatReadout,
            EnemyPanel,
            EnemyTitle,
            EnemyHpBar,
            EnemyStatusChips,
            EnemyImage,
            DamageEffectOverlay,
            PlayerCard,
            PlayerHpBar,
            PlayerSanityBar,
            MataiosCard,
            MataiosHpBar,
            MataiosSanityBar,
            AttackButton,
            DefendButton,
            SkillButton,
            CombatLog,
            ItemInspectButton
        };

        private static CombatSlot Slot(
            string key,
            float x,
            float y,
            float width,
            float height,
            string layer,
            string type,
            string role,
            bool interactive,
            bool hasFrame,
            string content,
            string label,
            string colorToken,
            int fontSize)
        {
            return new CombatSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
