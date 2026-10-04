namespace MauiRemoteController.Services;

public sealed record PingResult(bool Success, string? StoreName, string? StoreCode, string? Error)
{
    public static PingResult Failure(string error) => new(false, null, null, error);
}

public sealed record TerminalStatus(
    bool Success, bool? Offline, bool? TrialEnabled, bool? Licensed, int? DaysRemaining, string? Error)
{
    public static TerminalStatus Failure(string error) => new(false, null, null, null, null, error);
}

public sealed record ActivateResult(bool Success, string? Error)
{
    public static ActivateResult Failure(string error) => new(false, error);
}

public sealed record MenuItemInfo(string Name, string Label);

public sealed record MenusResult(
    bool Success, IReadOnlyList<MenuItemInfo> AllItems, IReadOnlyList<string> HiddenNames, string? Error)
{
    public static MenusResult Failure(string error) => new(false, Array.Empty<MenuItemInfo>(), Array.Empty<string>(), error);
}

public sealed record SetMenusResult(bool Success, string? Error)
{
    public static SetMenusResult Failure(string error) => new(false, error);
}
