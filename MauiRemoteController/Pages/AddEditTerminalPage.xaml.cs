using MauiRemoteController.Services;

namespace MauiRemoteController.Pages;

[QueryProperty(nameof(TerminalId), "id")]
public partial class AddEditTerminalPage : ContentPage
{
    private readonly TerminalStore _store;
    private SavedTerminal? _editing;
    private string? _pendingPinnedThumbprint;

    public AddEditTerminalPage(TerminalStore store)
    {
        InitializeComponent();
        _store = store;
    }

    public string? TerminalId
    {
        set
        {
            if (Guid.TryParse(value, out Guid id))
                _ = LoadForEditAsync(id);
        }
    }

    private async Task LoadForEditAsync(Guid id)
    {
        _editing = await _store.GetByIdAsync(id);
        if (_editing is null)
            return;

        Title = "Edit Terminal";
        NameEntry.Text = _editing.Name;
        HostEntry.Text = _editing.Host;
        PortEntry.Text = _editing.Port.ToString();
        TokenEntry.Placeholder = "Leave blank to keep the current token";
        _pendingPinnedThumbprint = _editing.PinnedThumbprint;
        UpdateThumbprintLabel();
        DeleteButton.IsVisible = true;
    }

    private void UpdateThumbprintLabel()
    {
        ThumbprintLabel.Text = string.IsNullOrEmpty(_pendingPinnedThumbprint)
            ? "Not trusted yet - test the connection before saving."
            : $"Pinned: {_pendingPinnedThumbprint}";
    }

    private (bool Ok, string Host, int Port) ValidateHostPort()
    {
        string host = HostEntry.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(host))
            return (false, host, 0);

        if (!int.TryParse(PortEntry.Text, out int port) || port is <= 0 or > 65535)
            return (false, host, 0);

        return (true, host, port);
    }

    private async void OnTestConnectionClicked(object? sender, EventArgs e)
    {
        var (ok, host, port) = ValidateHostPort();
        if (!ok)
        {
            await DisplayAlertAsync("Invalid host/port", "Enter a valid host and a port between 1 and 65535.", "OK");
            return;
        }

        TestButton.IsEnabled = false;
        try
        {
            string thumbprint;
            try
            {
                thumbprint = await RemoteControlClient.GetServerCertificateThumbprintAsync(host, port);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Connection failed", ex.Message, "OK");
                return;
            }

            bool confirmed = await DisplayAlertAsync("Confirm terminal identity",
                $"This terminal presented certificate thumbprint:\n\n{thumbprint}\n\n" +
                "Compare this against the thumbprint shown on that terminal's own " +
                "System Config > Remote Control screen. Only continue if they match - " +
                "this is what stops someone else's server from impersonating your terminal.",
                "They match - Trust", "Cancel");

            if (confirmed)
            {
                _pendingPinnedThumbprint = thumbprint;
                UpdateThumbprintLabel();
            }
        }
        finally
        {
            TestButton.IsEnabled = true;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var (ok, host, port) = ValidateHostPort();
        if (!ok)
        {
            await DisplayAlertAsync("Invalid host/port", "Enter a valid host and a port between 1 and 65535.", "OK");
            return;
        }

        string name = NameEntry.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(name))
        {
            await DisplayAlertAsync("Missing name", "Enter a name for this terminal.", "OK");
            return;
        }

        string token = TokenEntry.Text?.Trim() ?? "";
        if (_editing is null && string.IsNullOrEmpty(token))
        {
            await DisplayAlertAsync("Missing token", "Enter the REMOTE_CONTROL_TOKEN configured on that terminal.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(_pendingPinnedThumbprint))
        {
            bool proceedUnpinned = await DisplayAlertAsync("No pinned certificate",
                "You haven't tested the connection and trusted this terminal's certificate. " +
                "Saving now still encrypts the connection but won't protect against an " +
                "impersonating server. Continue anyway?", "Save anyway", "Go back");
            if (!proceedUnpinned)
                return;
        }

        var terminal = new SavedTerminal(_editing?.Id ?? default, name, host, port, _pendingPinnedThumbprint);

        await _store.SaveAsync(terminal, string.IsNullOrEmpty(token) ? null : token);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_editing is null)
            return;

        bool confirmed = await DisplayAlertAsync("Delete terminal",
            $"Remove \"{_editing.Name}\" and its saved token from this device?", "Delete", "Cancel");
        if (!confirmed)
            return;

        await _store.DeleteAsync(_editing.Id);
        await Shell.Current.GoToAsync("..");
    }
}
