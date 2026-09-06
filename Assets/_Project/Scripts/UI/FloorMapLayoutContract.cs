using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct FloorMapSlot
    {
        public FloorMapSlot(
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

    public static class FloorMapLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;

        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "action" };

        public static readonly FloorMapSlot FloorChip = Slot("floorChip", 0.04f, 0.03f, 0.18f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "층", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly FloorMapSlot SanityChip = Slot("sanityChip", 0.24f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly FloorMapSlot HpChip = Slot("hpChip", 0.50f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly FloorMapSlot GoldChip = Slot("goldChip", 0.76f, 0.03f, 0.20f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "골드", "gold_coin", UiTokenContract.LabelFontSize);
        public static readonly FloorMapSlot ScreenTitle = Slot("screenTitle", 0.10f, 0.106f, 0.80f, 0.048f, "title", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.ScreenFontSize);
        public static readonly FloorMapSlot NodeGraph = Slot("nodeGraph", 0.02f, 0.16f, 0.96f, 0.46f, "map", "graph", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly FloorMapSlot NodeLegend = Slot("nodeLegend", 0.04f, 0.632f, 0.92f, 0.04f, "hud", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.LabelFontSize);
        public static readonly FloorMapSlot SelectedNodeCard = Slot("selectedNodeCard", 0.04f, 0.688f, 0.92f, 0.15f, "actions", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly FloorMapSlot DepartButton = Slot("departButton", 0.30f, 0.868f, 0.40f, 0.056f, "actions", "button", string.Empty, true, false, "들어간다", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly FloorMapSlot BackButton = Slot("backButton", 0.04f, 0.868f, 0.22f, 0.056f, "actions", "button", string.Empty, true, false, "지도 접기", string.Empty, string.Empty, UiTokenContract.LabelFontSize);

        public static readonly FloorMapSlot[] AllSlots =
        {
            FloorChip,
            SanityChip,
            HpChip,
            GoldChip,
            ScreenTitle,
            NodeGraph,
            NodeLegend,
            SelectedNodeCard,
            DepartButton,
            BackButton
        };

        private static FloorMapSlot Slot(
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
            return new FloorMapSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
