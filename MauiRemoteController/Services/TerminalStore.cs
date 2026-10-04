using System.Text.Json;
using Microsoft.Maui.Storage;

namespace MauiRemoteController.Services;

public sealed record SavedTerminal(Guid Id, string Name, string Host, int Port, string? PinnedThumbprint);

/// <summary>
/// Per-device list of known terminals - the MAUI equivalent of the WinForms RemoteController's
/// Fleet (FleetStore.cs / Documents\RemoteController\Fleet.xml in the POS repo). No server,
/// nothing syncs between devices, same as there.
///
/// Metadata (name/host/port/pinned thumbprint) lives in Preferences as a JSON blob; each
/// terminal's auth token is kept separately in SecureStorage, since it's materially more
/// sensitive on a phone that can be lost or stolen than it was living in a desktop app's
/// Documents folder.
/// </summary>
public sealed class TerminalStore
{
    private const string TerminalsKey = "Terminals";
    private static string TokenKey(Guid id) => $"terminal_token_{id}";

    public Task<List<SavedTerminal>> GetAllAsync()
    {
        string json = Preferences.Default.Get(TerminalsKey, "[]");
        List<SavedTerminal>? terminals;
        try
        {
            terminals = JsonSerializer.Deserialize<List<SavedTerminal>>(json);
        }
        catch (JsonException)
        {
            terminals = null;
        }

        return Task.FromResult(terminals ?? new List<SavedTerminal>());
    }

    public async Task<SavedTerminal?> GetByIdAsync(Guid id)
    {
        List<SavedTerminal> all = await GetAllAsync().ConfigureAwait(false);
        return all.FirstOrDefault(t => t.Id == id);
    }

    public Task<string?> GetTokenAsync(Guid id) => SecureStorage.Default.GetAsync(TokenKey(id));

    /// <summary>
    /// Adds a new terminal (Id == default/empty) or updates an existing one in place. Pass
    /// token as null (or empty) when editing without changing the previously saved token.
    /// </summary>
    public async Task<SavedTerminal> SaveAsync(SavedTerminal terminal, string? token)
    {
        List<SavedTerminal> all = await GetAllAsync().ConfigureAwait(false);

        SavedTerminal saved = terminal.Id == default ? terminal with { Id = Guid.NewGuid() } : terminal;

        int existingIndex = all.FindIndex(t => t.Id == saved.Id);
        if (existingIndex >= 0)
            all[existingIndex] = saved;
        else
            all.Add(saved);

        Persist(all);

        if (!string.IsNullOrEmpty(token))
            await SecureStorage.Default.SetAsync(TokenKey(saved.Id), token).ConfigureAwait(false);

        return saved;
    }

    public async Task DeleteAsync(Guid id)
    {
        List<SavedTerminal> all = await GetAllAsync().ConfigureAwait(false);
        all.RemoveAll(t => t.Id == id);
        Persist(all);
        SecureStorage.Default.Remove(TokenKey(id));
    }

    private static void Persist(List<SavedTerminal> terminals) =>
        Preferences.Default.Set(TerminalsKey, JsonSerializer.Serialize(terminals));
}
