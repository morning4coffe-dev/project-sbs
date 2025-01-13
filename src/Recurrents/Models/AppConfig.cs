namespace Recurrents.Models;

public record AppConfig
{
    public string? Environment { get; init; }

    public const string TelemetryId = "";

    public const string ApplicationId = "";
    public const string TenantId = "common";
    public static string[] Scopes =
    [
        "user.read",
        "Files.ReadWrite.AppFolder"
    ];
}
