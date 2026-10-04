using System.Collections.ObjectModel;
using MauiRemoteController.Services;

namespace MauiRemoteController.Pages;

public sealed class MenuCheckItem
{
    public required string Name { get; init; }
    public required string Label { get; init; }
    public bool IsHidden { get; set; }
}

[QueryProperty(nameof(TerminalId), "id")]
public partial class TerminalDetailPage : ContentPage
{
    private readonly TerminalStore _store;
    private readonly ObservableCollection<MenuCheckItem> _menuItems = new();
    private SavedTerminal? _terminal;

    public TerminalDetailPage(TerminalStore store)
    {
        InitializeComponent();
        _store = store;
        MenuItemsView.ItemsSource = _menuItems;
    }

    public string? TerminalId
    {
        set
        {
            if (Guid.TryParse(value, out Guid id))
                _ = LoadAsync(id);
        }
    }

    private async Task LoadAsync(Guid id)
    {
        _terminal = await _store.GetByIdAsync(id);
        if (_terminal is null)
        {
            await DisplayAlertAsync("Not found", "This terminal no longer exists.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        Title = _terminal.Name;
        HeaderNameLabel.Text = _terminal.Name;
        HeaderHostLabel.Text = $"{_terminal.Host}:{_terminal.Port}";

        await RefreshStatusAsync();
        await LoadMenusAsync();
    }

    private async Task<RemoteTerminalClient?> GetClientAsync()
    {
        if (_terminal is null)
            return null;

        string? token = await _store.GetTokenAsync(_terminal.Id);
        return new RemoteTerminalClient(_terminal.Host, _terminal.Port, token ?? "", _terminal.PinnedThumbprint);
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_terminal is null)
            return;

        await Shell.Current.GoToAsync($"{Routes.AddEditTerminal}?id={_terminal.Id}");
    }

    private async void OnRefreshStatusClicked(object? sender, EventArgs e) => await RefreshStatusAsync();

    private async Task RefreshStatusAsync()
    {
        RemoteTerminalClient? client = await GetClientAsync();
        if (client is null)
            return;

        StatusSummaryLabel.Text = "Checking...";
        TerminalStatus status = await client.GetStatusAsync();

        StatusSummaryLabel.Text = status.Success
            ? $"Offline mode: {status.Offline}\n" +
              $"Trial enabled: {status.TrialEnabled}\n" +
              $"Licensed: {status.Licensed}\n" +
              $"Trial days remaining: {status.DaysRemaining}"
            : $"Could not get status: {status.Error}";
    }

    private async void OnActivateClicked(object? sender, EventArgs e)
    {
        string key = LicenseKeyEntry.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(key))
        {
            await DisplayAlertAsync("Missing key", "Enter a license key first.", "OK");
            return;
        }

        RemoteTerminalClient? client = await GetClientAsync();
        if (client is null)
            return;

        ActivateResult result = await client.ActivateAsync(key);
        await DisplayAlertAsync(result.Success ? "Activated" : "Activation failed",
            result.Success ? "The terminal accepted the license key." : result.Error, "OK");

        if (result.Success)
        {
            LicenseKeyEntry.Text = "";
            await RefreshStatusAsync();
        }
    }

    private async void OnLoadMenusClicked(object? sender, EventArgs e) => await LoadMenusAsync();

    private async Task LoadMenusAsync()
    {
        RemoteTerminalClient? client = await GetClientAsync();
        if (client is null)
            return;

        MenusResult result = await client.GetMenusAsync();
        if (!result.Success)
        {
            await DisplayAlertAsync("Could not load menus", result.Error, "OK");
            return;
        }

        var hiddenNames = new HashSet<string>(result.HiddenNames, StringComparer.OrdinalIgnoreCase);

        _menuItems.Clear();
        foreach (MenuItemInfo item in result.AllItems)
        {
            _menuItems.Add(new MenuCheckItem
            {
                Name = item.Name,
                Label = item.Label,
                IsHidden = hiddenNames.Contains(item.Name),
            });
        }
    }

    private async void OnSaveMenusClicked(object? sender, EventArgs e)
    {
        RemoteTerminalClient? client = await GetClientAsync();
        if (client is null)
            return;

        string[] hiddenNames = _menuItems.Where(m => m.IsHidden).Select(m => m.Name).ToArray();
        SetMenusResult result = await client.SetMenusAsync(hiddenNames);

        await DisplayAlertAsync(result.Success ? "Saved" : "Save failed",
            result.Success ? "Menu visibility updated on the terminal." : result.Error, "OK");
    }
}
