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
    "combat" => BuildCombatDocument(capturedAt),
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
        Source = "LobbyLayoutContract.cs",
        Slots = LobbyLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualLobbySlot)
    };
}

static LayoutDocument BuildCombatDocument(string capturedAt)
{
    return new LayoutDocument
    {
        Screen = "combat",
        CapturedAt = capturedAt,
        Source = "CombatLayoutContract.cs",
        Slots = CombatLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualCombatSlot)
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
