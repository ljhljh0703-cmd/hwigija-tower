using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct BossGateSlot
    {
        public BossGateSlot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
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

    public static class BossGateLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;
        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "action" };

        public static readonly BossGateSlot FloorChip = Slot("floorChip", 0.04f, 0.03f, 0.18f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "층", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly BossGateSlot SanityChip = Slot("sanityChip", 0.24f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly BossGateSlot HpChip = Slot("hpChip", 0.50f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly BossGateSlot GoldChip = Slot("goldChip", 0.76f, 0.03f, 0.20f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "골드", "gold_coin", UiTokenContract.LabelFontSize);
        public static readonly BossGateSlot GateTitle = Slot("gateTitle", 0.08f, 0.108f, 0.84f, 0.058f, "title", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.ScreenFontSize);
        public static readonly BossGateSlot GateScene = Slot("gateScene", 0.00f, 0.18f, 1.00f, 0.24f, "background", "image", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly BossGateSlot ReadinessPanel = Slot("readinessPanel", 0.02f, 0.432f, 0.96f, 0.284f, "content", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly BossGateSlot EngageButton = Slot("engageButton", 0.14f, 0.76f, 0.72f, 0.072f, "actions", "button", "engage", true, true, "문을 연다", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly BossGateSlot WarningLine = Slot("warningLine", 0.08f, 0.848f, 0.84f, 0.034f, "actions", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.LabelFontSize);
        public static readonly BossGateSlot RetreatButton = Slot("retreatButton", 0.30f, 0.90f, 0.40f, 0.056f, "actions", "button", string.Empty, true, false, "지도로 돌아간다", string.Empty, string.Empty, UiTokenContract.LabelFontSize);

        public static readonly BossGateSlot[] AllSlots =
        {
            FloorChip, SanityChip, HpChip, GoldChip, GateTitle, GateScene, ReadinessPanel, EngageButton, WarningLine, RetreatButton
        };

        private static BossGateSlot Slot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
        {
            return new BossGateSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
