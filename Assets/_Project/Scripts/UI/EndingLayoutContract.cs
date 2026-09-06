using HwigiTower.Lobby;

namespace HwigiTower.UI
{
    public readonly struct EndingSlot
    {
        public EndingSlot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
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

    public static class EndingLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;
        public static readonly string[] BandOrder = { "judgement", "subject", "action" };

        public static readonly EndingSlot EndingTitle = Slot("endingTitle", 0.08f, 0.052f, 0.84f, 0.07f, "title", "text", string.Empty, false, false, "엔딩 선택", string.Empty, string.Empty, UiTokenContract.ScreenFontSize);
        public static readonly EndingSlot EndingScene = Slot("endingScene", 0f, 0.16f, 1f, 0.26f, "background", "image", string.Empty, false, false, string.Empty, string.Empty, string.Empty, 0);
        public static readonly EndingSlot RunSummary = Slot("runSummary", 0.06f, 0.436f, 0.88f, 0.146f, "content", "card", string.Empty, false, true, string.Empty, string.Empty, string.Empty, UiTokenContract.BodyFontSize);
        public static readonly EndingSlot ChoiceRest = Slot("choiceRest", 0.06f, 0.626f, 0.88f, 0.13f, "actions", "card", "ending", true, true, "안식", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly EndingSlot ChoiceContinue = Slot("choiceContinue", 0.06f, 0.774f, 0.88f, 0.13f, "actions", "card", "ending", true, true, "동행 계속", string.Empty, string.Empty, UiTokenContract.PrimaryFontSize);
        public static readonly EndingSlot IrreversibleNote = Slot("irreversibleNote", 0.08f, 0.922f, 0.84f, 0.032f, "hud", "text", string.Empty, false, false, "선택은 되돌릴 수 없습니다", string.Empty, string.Empty, UiTokenContract.LabelFontSize);

        public static readonly EndingSlot[] AllSlots =
        {
            EndingTitle, EndingScene, RunSummary, ChoiceRest, ChoiceContinue, IrreversibleNote
        };

        private static EndingSlot Slot(string key, float x, float y, float width, float height, string layer, string type, string role, bool interactive, bool hasFrame, string content, string label, string colorToken, int fontSize)
        {
            return new EndingSlot(key, x, y, width, height, layer, type, role, interactive, hasFrame, content, label, colorToken, fontSize);
        }
    }
}
