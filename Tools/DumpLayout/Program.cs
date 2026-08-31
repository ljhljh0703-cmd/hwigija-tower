using System.Text.Json;
using HwigiTower.Lobby;

var outputPath = ReadArgument(args, "--output");
if (string.IsNullOrWhiteSpace(outputPath))
{
    Console.Error.WriteLine("Usage: dotnet run --project Tools/DumpLayout -- --output <screen>_actual_layout.json [--captured-at <source>]");
    return 2;
}

var capturedAt = ReadArgument(args, "--captured-at") ??
    "origin/Proto@" + LobbyBuildInfo.SourceRevision + " static-contract (Unity 미실행)";
var document = new LayoutDocument
{
    Screen = "lobby",
    CapturedAt = capturedAt,
    Source = "LobbyLayoutContract.cs",
    Slots = LobbyLayout.AllSlots.ToDictionary(slot => slot.Key, ToActualSlot)
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

static ActualSlot ToActualSlot(LobbySlot slot)
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

static int ResolveFontSize(LobbySlot slot)
{
    return slot.Key switch
    {
        "titleMark" => LobbyUiTokenContract.TitleFontSize,
        "primaryAction" => LobbyUiTokenContract.PrimaryFontSize,
        "secondaryAction" => LobbyUiTokenContract.BodyFontSize,
        "utilityRow" => LobbyUiTokenContract.MicroFontSize,
        "buildStamp" => LobbyUiTokenContract.MicroFontSize,
        "towerArt" => 0,
        _ => LobbyUiTokenContract.LabelFontSize
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
}
