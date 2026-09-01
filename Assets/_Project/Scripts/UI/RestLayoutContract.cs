using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct RestSlot
    {
        public RestSlot(
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

    public static class RestLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;

        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "party", "action" };

        public static readonly RestSlot FloorChip = Slot("floorChip", 0.04f, 0.03f, 0.18f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "층", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly RestSlot SanityChip = Slot("sanityChip", 0.24f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "이성", "sanity", UiTokenContract.LabelFontSize);
        public static readonly RestSlot HpChip = Slot("hpChip", 0.50f, 0.03f, 0.24f, 0.044f, "hud", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly RestSlot GoldChip = Slot("goldChip", 0.76f, 0.03f, 0.20f, 0.044f, "hud", "chip", string.Empty, false, false, string.Empty, "골드", "gold_coin", UiTokenContract.LabelFontSize);
        public static readonly RestSlot ScreenTitle = Slot("screenTitle", 0.10f, 0.106f, 0.80f, 0.048f, "title", "text", string.Empty, false, false, "모닥불", string.Empty, string.Empty, UiTokenContract.ScreenFontSize);
        public static readonly RestSlot RestScene = Slot("restScene", 0f, 0.16f, 1f, 0.44f, "background", "image", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly RestSlot MataiosState = Slot("mataiosState", 0.52f, 0.61f, 0.44f, 0.118f, "party", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0);
        public static readonly RestSlot MataiosStateLabel = Slot("mataiosStateLabel", 0.54f, 0.698f, 0.40f, 0.024f, "party", "text", string.Empty, false, false, string.Empty, "상태", string.Empty, UiTokenContract.LabelFontSize);
        public static readonly RestSlot PlayerState = Slot("playerState", 0.04f, 0.61f, 0.44f, 0.118f, "party", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, 0);
        public static readonly RestSlot PlayerHpBar = Slot("playerHpBar", 0.06f, 0.70f, 0.40f, 0.016f, "party", "gauge", string.Empty, false, false, string.Empty, "체력", "hp", UiTokenContract.LabelFontSize);
        public static readonly RestSlot ChoiceTalk = Slot("choiceTalk", 0.04f, 0.756f, 0.28f, 0.17f, "actions", "card", "choice", true, true, "마타이오스와 대화", string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly RestSlot ChoiceTrain = Slot("choiceTrain", 0.36f, 0.756f, 0.28f, 0.17f, "actions", "card", "choice", true, true, "단련 · 다음 전투 보너스", string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly RestSlot ChoiceSleep = Slot("choiceSleep", 0.68f, 0.756f, 0.28f, 0.17f, "actions", "card", "choice", true, true, "휴식 · 체력 회복", string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly RestSlot DepartButton = Slot("departButton", 0.30f, 0.938f, 0.40f, 0.052f, "actions", "button", string.Empty, true, false, "다시 오른다", string.Empty, string.Empty, UiTokenContract.LabelFontSize);

        public static readonly RestSlot[] AllSlots =
        {
            FloorChip,
            SanityChip,
            HpChip,
            GoldChip,
            ScreenTitle,
            RestScene,
            MataiosState,
            MataiosStateLabel,
            PlayerState,
            PlayerHpBar,
            ChoiceTalk,
            ChoiceTrain,
            ChoiceSleep,
            DepartButton
        };

        private static RestSlot Slot(
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
            return new RestSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
