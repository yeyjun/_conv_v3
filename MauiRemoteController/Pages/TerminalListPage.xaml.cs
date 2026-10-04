using System.Collections.ObjectModel;
using MauiRemoteController.Services;
using Microsoft.Maui.Graphics;

namespace MauiRemoteController.Pages;

public sealed class TerminalListItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string HostAndPort { get; init; }
    public string StatusText { get; set; } = "Unknown";
    public Color StatusColor { get; set; } = Colors.Gray;
}

public partial class TerminalListPage : ContentPage
{
    private readonly TerminalStore _store;
    private readonly ObservableCollection<TerminalListItem> _items = new();

    public TerminalListPage(TerminalStore store)
    {
        InitializeComponent();
        _store = store;
        TerminalsView.ItemsSource = _items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReloadListAsync();
    }

    private async Task ReloadListAsync()
    {
        List<SavedTerminal> terminals = await _store.GetAllAsync();

        // Preserve any status already shown for terminals that are still in the list, rather
        // than resetting everyone back to "Unknown" every time the page appears (e.g. after
        // returning from the detail page).
        Dictionary<Guid, TerminalListItem> existingById = _items.ToDictionary(i => i.Id);

        _items.Clear();
        foreach (SavedTerminal terminal in terminals)
        {
            var item = new TerminalListItem
            {
                Id = terminal.Id,
                Name = terminal.Name,
                HostAndPort = $"{terminal.Host}:{terminal.Port}",
            };

            if (existingById.TryGetValue(terminal.Id, out TerminalListItem? previous))
            {
                item.StatusText = previous.StatusText;
                item.StatusColor = previous.StatusColor;
            }

            _items.Add(item);
        }
    }

    private async void OnAddClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync(Routes.AddEditTerminal);

    private async void OnTerminalTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not Guid id)
            return;

        await Shell.Current.GoToAsync($"{Routes.TerminalDetail}?id={id}");
    }

    private async void OnDeleteSwipeInvoked(object? sender, EventArgs e)
    {
        if (sender is not SwipeItem { CommandParameter: Guid id })
            return;

        TerminalListItem? item = _items.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return;

        bool confirmed = await DisplayAlertAsync("Delete terminal",
            $"Remove \"{item.Name}\" and its saved token from this device?", "Delete", "Cancel");
        if (!confirmed)
            return;

        await _store.DeleteAsync(id);
        await ReloadListAsync();
    }

    private async void OnRefreshAllClicked(object? sender, EventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            await Task.WhenAll(_items.Select(RefreshStatusAsync));
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async Task RefreshStatusAsync(TerminalListItem item)
    {
        SavedTerminal? terminal = await _store.GetByIdAsync(item.Id);
        if (terminal is null)
            return;

        string? token = await _store.GetTokenAsync(terminal.Id);
        var client = new RemoteTerminalClient(terminal.Host, terminal.Port, token ?? "", terminal.PinnedThumbprint);
        PingResult result = await client.PingAsync();

        if (result.Success)
        {
            item.StatusText = "Online";
            item.StatusColor = Colors.SeaGreen;
        }
        else if (string.Equals(result.Error, "UNAUTHORIZED", StringComparison.Ordinal))
        {
            item.StatusText = "Reachable (bad token)";
            item.StatusColor = Colors.Goldenrod;
        }
        else
        {
            item.StatusText = "Offline";
            item.StatusColor = Colors.IndianRed;
        }

        // TerminalListItem isn't a bindable object, so the CollectionView won't pick up the
        // in-place mutation above on its own - force it to re-pull the two status properties by
        // swapping in a shallow copy at the same index.
        int index = _items.IndexOf(item);
        if (index >= 0)
            _items[index] = item;
    }
}
