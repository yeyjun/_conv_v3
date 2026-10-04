namespace MauiRemoteController.Services;

/// <summary>
/// Command-level wrapper around <see cref="RemoteControlClient"/> for one saved terminal -
/// bundles the host/port/token/pinned-thumbprint a Fleet entry carries and turns each of the
/// server's five commands into a typed, non-throwing call, so UI code doesn't need its own
/// try/catch around every request. This mirrors the POS app's own design goal of degrading
/// gracefully on a network hiccup instead of surfacing a raw exception.
/// </summary>
public sealed class RemoteTerminalClient
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _token;
    private readonly string? _pinnedThumbprint;
    private readonly int _timeoutMs;

    public RemoteTerminalClient(string host, int port, string token, string? pinnedThumbprint, int timeoutMs = 5000)
    {
        _host = host;
        _port = port;
        _token = token;
        _pinnedThumbprint = pinnedThumbprint;
        _timeoutMs = timeoutMs;
    }

    public async Task<PingResult> PingAsync(CancellationToken cancellationToken = default)
    {
        string response;
        try
        {
            response = await SendAsync("PING", null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return PingResult.Failure(ex.Message);
        }

        string[] parts = response.Split('|');
        if (parts.Length >= 4 && parts[0] == "OK" && parts[1] == "PONG")
            return new PingResult(true, parts[2], parts[3], null);

        return PingResult.Failure(ErrorReasonOrRaw(parts, response));
    }

    public async Task<TerminalStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        string response;
        try
        {
            response = await SendAsync("STATUS", null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return TerminalStatus.Failure(ex.Message);
        }

        string[] parts = response.Split('|');
        if (parts.Length < 2 || parts[0] != "OK" || parts[1] != "STATUS")
            return TerminalStatus.Failure(ErrorReasonOrRaw(parts, response));

        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string segment in parts.Skip(2))
        {
            int eq = segment.IndexOf('=');
            if (eq > 0)
                fields[segment[..eq]] = segment[(eq + 1)..];
        }

        return new TerminalStatus(
            true,
            ParseBool(fields, "Offline"),
            ParseBool(fields, "TrialEnabled"),
            ParseBool(fields, "Licensed"),
            ParseInt(fields, "DaysRemaining"),
            null);
    }

    public async Task<ActivateResult> ActivateAsync(string licenseKey, CancellationToken cancellationToken = default)
    {
        string response;
        try
        {
            response = await SendAsync("ACTIVATE", licenseKey, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return ActivateResult.Failure(ex.Message);
        }

        string[] parts = response.Split('|');
        return parts.Length >= 2 && parts[0] == "OK" && parts[1] == "ACTIVATED"
            ? new ActivateResult(true, null)
            : ActivateResult.Failure(ErrorReasonOrRaw(parts, response));
    }

    public async Task<MenusResult> GetMenusAsync(CancellationToken cancellationToken = default)
    {
        string response;
        try
        {
            response = await SendAsync("GET_MENUS", null, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return MenusResult.Failure(ex.Message);
        }

        string[] parts = response.Split('|');
        if (parts.Length < 3 || parts[0] != "OK" || parts[1] != "MENUS")
            return MenusResult.Failure(ErrorReasonOrRaw(parts, response));

        string allItemsCsv = parts[2];
        string hiddenCsv = parts.Length >= 4 ? parts[3] : "";

        // Split each "name:label" entry on the first colon only - a label is free text and
        // could itself contain a colon, but never a comma (the outer separator) or pipe (the
        // frame separator), matching how the server builds this string.
        List<MenuItemInfo> allItems = allItemsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(entry =>
            {
                string[] nameLabel = entry.Split(':', 2);
                return new MenuItemInfo(nameLabel[0], nameLabel.Length > 1 ? nameLabel[1] : nameLabel[0]);
            })
            .ToList();

        List<string> hidden = hiddenCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        return new MenusResult(true, allItems, hidden, null);
    }

    public async Task<SetMenusResult> SetMenusAsync(IEnumerable<string> hiddenMenuNames, CancellationToken cancellationToken = default)
    {
        string arg = string.Join(",", hiddenMenuNames);

        string response;
        try
        {
            response = await SendAsync("SET_MENUS", arg, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return SetMenusResult.Failure(ex.Message);
        }

        string[] parts = response.Split('|');
        return parts.Length >= 2 && parts[0] == "OK" && parts[1] == "MENUS_UPDATED"
            ? new SetMenusResult(true, null)
            : SetMenusResult.Failure(ErrorReasonOrRaw(parts, response));
    }

    private Task<string> SendAsync(string command, string? arg, CancellationToken cancellationToken) =>
        RemoteControlClient.SendCommandAsync(_host, _port, _token, command, arg, _pinnedThumbprint, _timeoutMs, cancellationToken);

    private static string ErrorReasonOrRaw(string[] parts, string raw) =>
        parts.Length >= 2 && parts[0] == "ERR" ? parts[1] : $"Unexpected response: {raw}";

    private static bool? ParseBool(Dictionary<string, string> fields, string key) =>
        fields.TryGetValue(key, out string? value) && bool.TryParse(value, out bool result) ? result : null;

    private static int? ParseInt(Dictionary<string, string> fields, string key) =>
        fields.TryGetValue(key, out string? value) && int.TryParse(value, out int result) ? result : null;
}
