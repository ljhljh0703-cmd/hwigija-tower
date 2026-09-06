using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct ShopSlot
    {
        public ShopSlot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
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

    public static class ShopLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;
        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "action" };

        public static readonly ShopSlot FloorChip = Slot("floorChip", 0.04f, 0.03f, 0.18f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "층", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly ShopSlot SanityChip = Slot("sanityChip", 0.24f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly ShopSlot HpChip = Slot("hpChip", 0.50f, 0.03f, 0.20f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly ShopSlot GoldChip = Slot("goldChip", 0.72f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "골드", "gold_coin", UiTokenContract.LabelFontSize);
        public static readonly ShopSlot ScreenTitle = Slot("screenTitle", 0.10f, 0.106f, 0.80f, 0.048f, "title", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.ScreenFontSize);
        public static readonly ShopSlot ShopScene = Slot("shopScene", 0.00f, 0.16f, 1.00f, 0.26f, "background", "image", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly ShopSlot OwnedStrip = Slot("ownedStrip", 0.04f, 0.442f, 0.92f, 0.038f, "hud", "text", string.Empty, false, false, string.Empty, string.Empty, string.Empty, UiTokenContract.LabelFontSize);
        public static readonly ShopSlot OfferRow1 = Slot("offerRow1", 0.04f, 0.53f, 0.92f, 0.062f, "actions", "row", "offer", true, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly ShopSlot OfferRow2 = Slot("offerRow2", 0.04f, 0.598f, 0.92f, 0.062f, "actions", "row", "offer", true, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly ShopSlot OfferRow3 = Slot("offerRow3", 0.04f, 0.666f, 0.92f, 0.062f, "actions", "row", "offer", true, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly ShopSlot OfferRow4 = Slot("offerRow4", 0.04f, 0.734f, 0.92f, 0.062f, "actions", "row", "offer", true, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly ShopSlot OfferRow5 = Slot("offerRow5", 0.04f, 0.802f, 0.92f, 0.062f, "actions", "row", "offer", true, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly ShopSlot LeaveButton = Slot("leaveButton", 0.30f, 0.90f, 0.40f, 0.056f, "actions", "button", string.Empty, true, false, "나간다", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);

        public static readonly ShopSlot[] AllSlots =
        {
            FloorChip, SanityChip, HpChip, GoldChip, ScreenTitle, ShopScene, OwnedStrip,
            OfferRow1, OfferRow2, OfferRow3, OfferRow4, OfferRow5, LeaveButton
        };

        private static ShopSlot Slot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
        {
            return new ShopSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
