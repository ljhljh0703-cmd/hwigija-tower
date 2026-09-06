using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct EventSlot
    {
        public EventSlot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
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

    public static class EventLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;
        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "action", "fixed" };

        public static readonly EventSlot FloorChip = Slot("floorChip", 0.04f, 0.03f, 0.18f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "층", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly EventSlot SanityChip = Slot("sanityChip", 0.24f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly EventSlot HpChip = Slot("hpChip", 0.50f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly EventSlot GoldChip = Slot("goldChip", 0.76f, 0.03f, 0.20f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "골드", "gold_coin", UiTokenContract.LabelFontSize);
        public static readonly EventSlot EventTitle = Slot("eventTitle", 0.06f, 0.106f, 0.88f, 0.062f, "title", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.ScreenFontSize);
        public static readonly EventSlot EventBody = Slot("eventBody", 0.06f, 0.19f, 0.88f, 0.23f, "content", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly EventSlot ChoiceList = Slot("choiceList", 0.04f, 0.45f, 0.92f, 0.39f, "actions", "scrollList", "choice", true, true, string.Empty, string.Empty, string.Empty, 0);
        public static readonly EventSlot ScrollHint = Slot("scrollHint", 0.04f, 0.846f, 0.92f, 0.028f, "hud", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.LabelFontSize);
        public static readonly EventSlot LeaveButton = Slot("leaveButton", 0.30f, 0.888f, 0.40f, 0.056f, "fixed", "button", string.Empty, true, false, "물러난다", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);

        public static readonly EventSlot[] AllSlots =
        {
            FloorChip, SanityChip, HpChip, GoldChip, EventTitle, EventBody, ChoiceList, ScrollHint, LeaveButton
        };

        private static EventSlot Slot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
        {
            return new EventSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
