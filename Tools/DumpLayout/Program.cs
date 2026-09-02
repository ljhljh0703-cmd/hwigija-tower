using System.Text.Json;
using HwigiTower.Lobby;
using HwigiTower.UI;

var outputPath = ReadArgument(args, "--output");
if (string.IsNullOrWhiteSpace(outputPath))
{
    Console.Error.WriteLine("Usage: dotnet run --project Tools/DumpLayout -- --output <screen>_actual_layout.json [--captured-at <source>]");
    return 2;
}

var screen = ReadArgument(args, "--screen") ?? "lobby";
var capturedAt = ReadArgument(args, "--captured-at") ??
    "origin/Proto@" + LobbyBuildInfo.SourceRevision + " static-contract (Unity 미실행)";
var document = screen switch
{
    "lobby" => BuildLobbyDocument(capturedAt),
    "bossgate" => BuildBossGateDocument(capturedAt),
    "combat" => BuildCombatDocument(capturedAt),
    "ending" => BuildEndingDocument(capturedAt),
    "event" => BuildEventDocument(capturedAt),
    "floormap" => BuildFloorMapDocument(capturedAt),
    "rest" => BuildRestDocument(capturedAt),
    "shop" => BuildShopDocument(capturedAt),
    _ => throw new ArgumentException("Unsupported screen: " + screen)
};

var fullOutputPath = Path.GetFullPath(outputPath);
var directory = Path.GetDirectoryName(fullOutputPath);
if (!string.IsNullOrEmpty(directory))
{
    Directory.CreateDirectory(directory);
}

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
File.WriteAllText(fullOutputPath, JsonSerializer.Serialize(document, options));
Console.WriteLine("actual layout written: " + fullOutputPath);
return 0;

static string? ReadArgument(string[] commandLine, string name)
{
    for (var i = 0; i < commandLine.Length - 1; i++)
    {
        if (commandLine[i] == name)
        {
            return commandLine[i + 1];
        }
    }

    return null;
}

static LayoutDocument BuildLobbyDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "lobby",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = LobbyLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualLobbySlot)
    };
}

static LayoutDocument BuildBossGateDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "bossgate",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = BossGateLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualBossGateSlot)
    };
}

static LayoutDocument BuildCombatDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "combat",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = CombatLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualCombatSlot)
    };
}

static LayoutDocument BuildEventDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "event",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = EventLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualEventSlot)
    };
}

static LayoutDocument BuildEndingDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "ending",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = EndingLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualEndingSlot)
    };
}

static LayoutDocument BuildRestDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "rest",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = RestLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualRestSlot)
    };
}

static LayoutDocument BuildFloorMapDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "floormap",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = FloorMapLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualFloorMapSlot)
    };
}

static LayoutDocument BuildShopDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "shop",
        CapturedAt = capturedAt,
        Source = "contract",
        Slots = ShopLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualShopSlot)
    };
}

static ActualSlot ToActualLobbySlot(LobbySlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = ResolveFontSize(slot),
        Visible = true,
        Layer = slot.Layer
    };
}

static ActualSlot ToActualBossGateSlot(BossGateSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static ActualSlot ToActualCombatSlot(CombatSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static ActualSlot ToActualEventSlot(EventSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static ActualSlot ToActualEndingSlot(EndingSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static ActualSlot ToActualRestSlot(RestSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static ActualSlot ToActualFloorMapSlot(FloorMapSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static ActualSlot ToActualShopSlot(ShopSlot slot)
{
    return new ActualSlot
    {
        X = slot.X,
        Y = slot.Y,
        W = slot.Width,
        H = slot.Height,
        FontSize = slot.FontSize,
        Visible = true,
        Layer = slot.Layer,
        Type = slot.Type,
        Label = slot.Label,
        Color = slot.ColorToken,
        Interactive = slot.Interactive,
        FrameStyle = slot.HasFrame
    };
}

static int ResolveFontSize(LobbySlot slot)
{
    return slot.Key switch
    {
        "titleMark" => UiTokenContract.TitleFontSize,
        "primaryAction" => UiTokenContract.PrimaryFontSize,
        "secondaryAction" => UiTokenContract.BodyFontSize,
        "utilityRow" => UiTokenContract.MicroFontSize,
        "buildStamp" => UiTokenContract.MicroFontSize,
        "towerArt" => 0,
        _ => UiTokenContract.LabelFontSize
    };
}

internal sealed class LayoutDocument
{
    public string Screen { get; init; } = string.Empty;
    public string CapturedAt { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public Dictionary<string, ActualSlot> Slots { get; init; } = new();
}

internal sealed class ActualSlot
{
    public float X { get; init; }
    public float Y { get; init; }
    public float W { get; init; }
    public float H { get; init; }
    public int FontSize { get; init; }
    public bool Visible { get; init; }
    public string Layer { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public bool Interactive { get; init; }
    public bool FrameStyle { get; init; }
}
