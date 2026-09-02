namespace HwigiTower.Lobby
{
    public readonly struct LobbySlot
    {
        public LobbySlot(
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
            string content)
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

        public bool OverlapsTopOrigin(float xMin, float xMax, float yMin, float yMax)
        {
            return X < xMax && X + Width > xMin && Y < yMax && Y + Height > yMin;
        }
    }

    public static class LobbyLayout
    {
        public const int ReferenceWidth = 1080;
        public const int ReferenceHeight = 1920;
        public const float TowerProtectedXMin = 0.35f;
        public const float TowerProtectedXMax = 0.65f;
        public const float TowerProtectedYMin = 0.10f;
        public const float TowerProtectedYMax = 0.55f;

        public static readonly string[] BandOrder = { "resource", "judgement", "subject", "party", "action" };

        public static readonly LobbySlot RunStatus = new LobbySlot(
            "runStatus", 0.06f, 0.022f, 0.88f, 0.036f, "hud", "text", string.Empty, false, false,
            "저장 유무 · 최고 도달 층 · 기억 조각");

        public static readonly LobbySlot TowerArt = new LobbySlot(
            "towerArt", 0f, 0f, 1f, 0.70f, "background", "image", string.Empty, false, false, string.Empty);

        public static readonly LobbySlot TitleMark = new LobbySlot(
            "titleMark", 0.10f, 0.46f, 0.80f, 0.12f, "title", "text", string.Empty, false, false,
            "회귀자는 탑을 오른다");

        public static readonly LobbySlot Tagline = new LobbySlot(
            "tagline", 0.15f, 0.585f, 0.70f, 0.035f, "title", "text", string.Empty, false, false,
            "되찾는 것은 잃기 위해서다");

        public static readonly LobbySlot PrimaryAction = new LobbySlot(
            "primaryAction", 0.10f, 0.690f, 0.80f, 0.078f, "ui", "button", "primary", true, true,
            "탑에 오른다");

        public static readonly LobbySlot SecondaryAction = new LobbySlot(
            "secondaryAction", 0.14f, 0.784f, 0.72f, 0.062f, "ui", "button", "secondary", true, true,
            "이어서 오른다");

        public static readonly LobbySlot UtilityRow = new LobbySlot(
            "utilityRow", 0.20f, 0.878f, 0.60f, 0.052f, "ui", "iconRow", "tertiary", true, false, string.Empty);

        public static readonly LobbySlot BuildStamp = new LobbySlot(
            "buildStamp", 0.35f, 0.955f, 0.30f, 0.022f, "hud", "text", string.Empty, false, false,
            "버전 · 커밋 해시");

        public static readonly LobbySlot[] AllSlots =
        {
            RunStatus,
            TowerArt,
            TitleMark,
            Tagline,
            PrimaryAction,
            SecondaryAction,
            UtilityRow,
            BuildStamp
        };

        public static bool ViolatesTowerProtection(LobbySlot slot)
        {
            if (slot.Layer == "title" || slot.Layer == "background")
            {
                return false;
            }

            return (slot.Layer == "ui" || slot.Layer == "actions" || slot.HasFrame) &&
                slot.OverlapsTopOrigin(TowerProtectedXMin, TowerProtectedXMax, TowerProtectedYMin, TowerProtectedYMax);
        }
    }

    public static class UiTokenContract
    {
        public const string VoidBgHex = "#0B0907";
        public const string PanelBgHex = "#17120D";
        public const string PanelBgAltHex = "#1F1811";
        public const string FrameHex = "#4A3A24";
        public const string FrameHiHex = "#8A6A38";
        public const string GoldHex = "#C9A24A";
        public const string GoldDimHex = "#8C6E2E";
        public const string InkHex = "#E8DFCF";
        public const string InkDimHex = "#A2957F";
        public const string InkMuteHex = "#6B6152";
        public const string HpHex = "#8E2F1F";
        public const string HpAllyHex = "#5C7A4A";
        public const string SanityHex = "#6E86A8";
        public const string SanityWarnHex = "#B98A3C";
        public const string SanityBrokenHex = "#A8425A";
        public const string GoldCoinHex = "#D8B45C";

        public const int TitleFontSize = 96;
        public const int ScreenFontSize = 54;
        public const int PrimaryFontSize = 42;
        public const int BodyFontSize = 34;
        public const int LabelFontSize = 26;
        public const int MicroFontSize = 22;
        public const float TitleLineHeight = 1.12f;
        public const float PrimaryLineHeight = 1.3f;
        public const float BodyLineHeight = 1.5f;
        public const float LabelLineHeight = 1.4f;
        public const float DisplayLetterSpacing = 0.04f;
        public const float LabelLetterSpacing = 0.1f;
        public const int SpacingUnit = 24;
        public const int Gutter = 48;
        public const int FrameBorderWidth = 3;
        public const int CornerSize = 36;
        public const int TapTargetMinPx = 96;
        public const float VignetteStrength = 0.35f;
    }

    public static class LobbyBuildInfo
    {
        public const string SourceRevision = "5adc2e1";
    }
}
