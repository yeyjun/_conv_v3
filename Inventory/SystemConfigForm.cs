using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpeedTest.Net;
using SpeedTest.Net.Enums;
using SpeedTest.Net.Models;

namespace Inventory
{
    /// <summary>
    /// Lets a system admin edit the app-wide settings that currently live in App.config
    /// (store identity, database connection, manager PIN, admin PIN) without hand-editing
    /// the XML. Reached only through a PIN-gated menu item like Manage Users / Tax Config,
    /// but gated by the separate admin PIN rather than the manager PIN those use.
    /// </summary>
    public partial class SystemConfigForm : Form
    {
        // Shared across samples so repeated tests reuse one connection instead of a fresh
        // handshake every 15 seconds; the process-wide static instance is the recommended
        // HttpClient lifetime pattern.
        private static readonly HttpClient _uploadHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        private const string UploadTestUrl = "https://speed.cloudflare.com/__up";
        private const int MonitorIntervalMs = 15000;
        private const int MaxChartSamples = 30;

        private System.Windows.Forms.Timer _monitorTimer;
        private Server _cachedSpeedTestServer;
        private int _sampleIndex;
        private bool _sampleInFlight;

        public SystemConfigForm()
        {
            InitializeComponent();
            YourApp.Data.AppTheme.Exempt(pnlBgSwatch);
            YourApp.Data.AppTheme.Exempt(pnlFgSwatch);
            YourApp.Data.AppTheme.Exempt(pnlCartBgSwatch);
            YourApp.Data.AppTheme.Exempt(pnlItemSearchBgSwatch);
            YourApp.Data.AppTheme.Exempt(lblWarningBar);
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            txtDbPassword.TextChanged += (s, e) => UpdateWarningBar();
            txtManagerPin.TextChanged += (s, e) => UpdateWarningBar();
            txtAdminPin.TextChanged += (s, e) => UpdateWarningBar();
            txtStoreEmail.TextChanged += (s, e) => UpdateWarningBar();
            txtSmtpHost.TextChanged += (s, e) => UpdateWarningBar();
            txtGmailClientId.TextChanged += (s, e) => UpdateWarningBar();
            txtGmailClientSecret.TextChanged += (s, e) => UpdateWarningBar();
            txtGmailRefreshToken.TextChanged += (s, e) => UpdateWarningBar();
            txtGmailSender.TextChanged += (s, e) => UpdateWarningBar();
        }

        private static readonly System.Drawing.Color SectionSelectedColor = System.Drawing.Color.FromArgb(205, 230, 247);

        private void lstConfigSections_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (var backBrush = new System.Drawing.SolidBrush(selected ? SectionSelectedColor : lstConfigSections.BackColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            var textRect = new System.Drawing.Rectangle(e.Bounds.X + 14, e.Bounds.Y, e.Bounds.Width - 14, e.Bounds.Height);
            var format = new System.Drawing.StringFormat { LineAlignment = System.Drawing.StringAlignment.Center };

            using (var textBrush = new System.Drawing.SolidBrush(System.Drawing.SystemColors.ControlText))
                e.Graphics.DrawString(lstConfigSections.Items[e.Index].ToString(), lstConfigSections.Font, textBrush, textRect, format);
        }

        private void lstConfigSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstConfigSections.SelectedIndex >= 0)
                tabControlConfig.SelectedIndex = lstConfigSections.SelectedIndex;
        }

        private void tabControlConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlConfig.SelectedIndex >= 0)
                lstConfigSections.SelectedIndex = tabControlConfig.SelectedIndex;
        }

        private void SystemConfigForm_Load(object sender, EventArgs e)
        {
            lstConfigSections.SelectedIndex = tabControlConfig.SelectedIndex;

            txtStoreName.Text = ConfigurationManager.AppSettings["STORE_NAME"];
            txtStoreCode.Text = ConfigurationManager.AppSettings["STORE_CODE"];
            txtStoreEmail.Text = ConfigurationManager.AppSettings["STORE_EMAIL"];
            txtDbServer.Text = ConfigurationManager.AppSettings["DB_SERVER"];
            txtDbName.Text = ConfigurationManager.AppSettings["DB_NAME"];
            txtDbUser.Text = ConfigurationManager.AppSettings["DB_USER"];
            txtDbPassword.Text = ConfigurationManager.AppSettings["DB_PASSWORD"];
            txtManagerPin.Text = ConfigurationManager.AppSettings["MANAGER_PIN"];
            txtAdminPin.Text = ConfigurationManager.AppSettings["ADMIN_PIN"];

            if (int.TryParse(ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"], out int timeout))
                nud_connectionTimeout.Value = Math.Min(Math.Max(timeout, nud_connectionTimeout.Minimum), nud_connectionTimeout.Maximum);

            LoadSalesDefaults();
            LoadAlertSettings();
            LoadMenuVisibility();
            LoadItemSearchColumns();
            LoadCartColumns();
            LoadReceiptTesting();
            LoadReceiptPrinter();
            LoadAppearance();
            LoadLayoutRevision();
            LoadLayoutPresets();
            LoadReportSchedule();
            LoadInventoryPrintSchedule();
            LoadOfflineSync();
            LoadDataSync();
            LoadTrialLicense();
            LoadDbSetup();
            LoadRemoteControl();
            LoadTopSoldSettings();
            UpdateWarningBar();
            NotifyTrayOfWarnings();

            _ = LoadMaintenanceAsync();
            _ = LoadLowStockReminderAsync();
            _ = LoadUpdatesAsync();
        }

        // Cached from the last successful load/save/refresh so ComputeWarnings (called
        // synchronously, e.g. on every keystroke) can flag maintenance mode without itself
        // making a DB round-trip.
        private bool _maintenanceEnabledCache;

        private async Task LoadMaintenanceAsync()
        {
            lblMaintenanceStatus.Text = "Checking current status...";

            (bool enabled, string message) = await AppConfigService.GetMaintenanceStatusAsync();

            chkMaintenanceEnabled.Checked = enabled;
            txtMaintenanceMessage.Text = message ?? "";
            _maintenanceEnabledCache = enabled;

            lblMaintenanceStatus.ForeColor = System.Drawing.Color.Gray;
            lblMaintenanceStatus.Text = $"Status: Maintenance mode is currently {(enabled ? "ON" : "OFF")} (live, shared across all terminals).";
            UpdateWarningBar();
            NotifyTrayOfWarnings();
        }

        private async void btnSaveMaintenance_Click(object sender, EventArgs e)
        {
            btnSaveMaintenance.Enabled = false;
            lblMaintenanceStatus.Text = "Saving...";

            try
            {
                bool enabled = chkMaintenanceEnabled.Checked;
                string message = txtMaintenanceMessage.Text.Trim();

                await AppConfigService.SetMaintenanceModeAsync(enabled, message);

                _maintenanceEnabledCache = enabled;
                lblMaintenanceStatus.ForeColor = System.Drawing.Color.Gray;
                lblMaintenanceStatus.Text = $"Status: Maintenance mode is currently {(enabled ? "ON" : "OFF")} (live, shared across all terminals).";
                UpdateWarningBar();
                NotifyTrayOfWarnings();
            }
            catch (Exception ex)
            {
                lblMaintenanceStatus.ForeColor = System.Drawing.Color.Tomato;
                lblMaintenanceStatus.Text = $"Failed to save: {ex.Message}";
            }
            finally
            {
                btnSaveMaintenance.Enabled = true;
            }
        }

        private async void btnRefreshMaintenance_Click(object sender, EventArgs e) => await LoadMaintenanceAsync();

        private async Task LoadLowStockReminderAsync()
        {
            lblLowStockStatus.Text = "Checking current status...";

            (bool enabled, int threshold, int intervalMinutes) = await AppConfigService.GetLowStockReminderSettingsAsync();

            chkLowStockEnabled.Checked = enabled;
            nudLowStockThreshold.Value = Math.Min(Math.Max(threshold, nudLowStockThreshold.Minimum), nudLowStockThreshold.Maximum);
            nudLowStockInterval.Value = Math.Min(Math.Max(intervalMinutes, nudLowStockInterval.Minimum), nudLowStockInterval.Maximum);

            lblLowStockStatus.ForeColor = System.Drawing.Color.Gray;
            lblLowStockStatus.Text = $"Status: Low stock reminder is currently {(enabled ? "ON" : "OFF")} (live, shared across all terminals).";
        }

        private async void btnSaveLowStock_Click(object sender, EventArgs e)
        {
            btnSaveLowStock.Enabled = false;
            lblLowStockStatus.Text = "Saving...";

            try
            {
                bool enabled = chkLowStockEnabled.Checked;
                int threshold = (int)nudLowStockThreshold.Value;
                int intervalMinutes = (int)nudLowStockInterval.Value;

                await AppConfigService.SetLowStockReminderSettingsAsync(enabled, threshold, intervalMinutes);

                lblLowStockStatus.ForeColor = System.Drawing.Color.Gray;
                lblLowStockStatus.Text = $"Status: Low stock reminder is currently {(enabled ? "ON" : "OFF")} (live, shared across all terminals).";
            }
            catch (Exception ex)
            {
                lblLowStockStatus.ForeColor = System.Drawing.Color.Tomato;
                lblLowStockStatus.Text = $"Failed to save: {ex.Message}";
            }
            finally
            {
                btnSaveLowStock.Enabled = true;
            }
        }

        private async void btnRefreshLowStock_Click(object sender, EventArgs e) => await LoadLowStockReminderAsync();

        private async Task LoadUpdatesAsync()
        {
            lblCurrentVersion.Text = $"Installed version: {AppVersion.Current}";
            lblUpdatesStatus.Text = "Checking current status...";

            bool enabled = await AppConfigService.GetUpdateCheckEnabledAsync();
            chkUpdateCheckEnabled.Checked = enabled;

            bool autoEnabled = await AppConfigService.GetAutoUpdateEnabledAsync();
            chkAutoUpdateEnabled.Checked = autoEnabled;

            lblUpdatesStatus.ForeColor = System.Drawing.Color.Gray;
            lblUpdatesStatus.Text = $"Status: Automatic update checks are currently {(enabled ? "ON" : "OFF")}, " +
                $"automatic silent install is currently {(autoEnabled ? "ON" : "OFF")} (live, shared across all terminals).";
        }

        private async void btnSaveUpdates_Click(object sender, EventArgs e)
        {
            btnSaveUpdates.Enabled = false;
            lblUpdatesStatus.Text = "Saving...";

            try
            {
                bool enabled = chkUpdateCheckEnabled.Checked;
                await AppConfigService.SetUpdateCheckEnabledAsync(enabled);

                bool autoEnabled = chkAutoUpdateEnabled.Checked;
                await AppConfigService.SetAutoUpdateEnabledAsync(autoEnabled);

                lblUpdatesStatus.ForeColor = System.Drawing.Color.Gray;
                lblUpdatesStatus.Text = $"Status: Automatic update checks are currently {(enabled ? "ON" : "OFF")}, " +
                    $"automatic silent install is currently {(autoEnabled ? "ON" : "OFF")} (live, shared across all terminals).";
            }
            catch (Exception ex)
            {
                lblUpdatesStatus.ForeColor = System.Drawing.Color.Tomato;
                lblUpdatesStatus.Text = $"Failed to save: {ex.Message}";
            }
            finally
            {
                btnSaveUpdates.Enabled = true;
            }
        }

        private async void btnRefreshUpdates_Click(object sender, EventArgs e) => await LoadUpdatesAsync();

        private async void btnCheckUpdateNow_Click(object sender, EventArgs e)
        {
            btnCheckUpdateNow.Enabled = false;
            lblUpdatesStatus.ForeColor = System.Drawing.Color.Gray;
            lblUpdatesStatus.Text = "Checking GitHub for a newer release...";

            try
            {
                UpdateInfo info = await UpdateCheckService.CheckForUpdateAsync();
                lblUpdatesStatus.Text = info != null
                    ? $"Update available: {info.TagName} (installed: {AppVersion.Current})."
                    : $"Up to date (installed: {AppVersion.Current}).";
            }
            finally
            {
                btnCheckUpdateNow.Enabled = true;
            }
        }

        private void LoadRemoteControl()
        {
            chkRemoteControlEnabled.Checked = RemoteControlServer.IsEnabled;
            nud_remotePort.Value = Math.Min(Math.Max(RemoteControlServer.Port, nud_remotePort.Minimum), nud_remotePort.Maximum);
            txtRemoteToken.Text = ConfigurationManager.AppSettings["REMOTE_CONTROL_TOKEN"];
            UpdateRemoteIpLabel();
            UpdateRemoteStatusLabel();

            txtCertThumbprint.Text = RemoteControlServer.CertificateThumbprint;
            txtPublicIp.Text = "";

            chkBluetoothEnabled.Checked = BluetoothControlServer.IsEnabled;
            UpdateBluetoothRadioInfo();
        }

        private void UpdateBluetoothRadioInfo()
        {
            (string Name, string Address)? radio = BluetoothControlServer.GetLocalRadioInfo();
            lblBluetoothRadioInfo.Text = radio.HasValue
                ? $"Local radio: {radio.Value.Name} ({radio.Value.Address})"
                : "No Bluetooth radio detected on this machine.";
        }

        private void btnCopyThumbprint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCertThumbprint.Text))
                Clipboard.SetText(txtCertThumbprint.Text);
        }

        private async void btnCheckPublicIp_Click(object sender, EventArgs e)
        {
            btnCheckPublicIp.Enabled = false;
            txtPublicIp.Text = "Checking...";
            try
            {
                string ip = await RemoteControlServer.GetPublicIPv4Async();
                txtPublicIp.Text = ip ?? "Could not determine (no internet access?).";
            }
            finally
            {
                btnCheckPublicIp.Enabled = true;
            }
        }

        /// <summary>
        /// Shows this machine's current LAN IP address(es) - and the exact host:port to plug
        /// into the RemoteController companion app - so setting one up doesn't require separately
        /// looking up ipconfig. Re-read on demand (Refresh button) rather than cached, since a
        /// laptop can change networks while this dialog is still open.
        /// </summary>
        private void UpdateRemoteIpLabel()
        {
            string[] addresses = RemoteControlServer.GetLocalIPv4Addresses();
            int port = (int)nud_remotePort.Value;

            if (addresses.Length == 0)
            {
                lblRemoteIpAddress.Text = "This terminal's IP address: none found (no active network connection).";
                return;
            }

            lblRemoteIpAddress.Text =
                $"This terminal's IP address: {string.Join(", ", addresses)}{Environment.NewLine}" +
                $"Connect the Remote Controller to: {addresses[0]}:{port}";
        }

        private void btnRefreshIp_Click(object sender, EventArgs e) => UpdateRemoteIpLabel();

        private void nud_remotePort_ValueChanged(object sender, EventArgs e) => UpdateRemoteIpLabel();

        private void UpdateRemoteStatusLabel()
        {
            if (RemoteControlServer.IsRunning)
                lblRemoteStatus.Text = $"Status: Listening on port {RemoteControlServer.Port}.";
            else if (RemoteControlServer.LastError != null)
                lblRemoteStatus.Text = $"Status: Failed to start - {RemoteControlServer.LastError}";
            else
                lblRemoteStatus.Text = "Status: Not running.";
        }

        private void chkShowRemoteToken_CheckedChanged(object sender, EventArgs e) =>
            txtRemoteToken.UseSystemPasswordChar = !chkShowRemoteToken.Checked;

        // 24 random bytes (192 bits) as hex - long enough that guessing it isn't a realistic
        // attack, short enough to still be readable/copyable by hand if needed.
        private void btnGenerateRemoteToken_Click(object sender, EventArgs e)
        {
            var bytes = new byte[24];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            txtRemoteToken.Text = BitConverter.ToString(bytes).Replace("-", "");
            chkShowRemoteToken.Checked = true;
        }

        private void LoadDbSetup()
        {
            txtNewDbName.Text = ConfigurationManager.AppSettings["DB_NAME"];
        }

        /// <summary>
        /// Stands up a brand-new store install: creates the named MySQL database (if it doesn't
        /// already exist) and every table this app reads/writes, using the server/user/password
        /// already entered in the Settings tab. CREATE TABLE IF NOT EXISTS makes this safe to
        /// run again against a database that's already partially set up - it never touches
        /// existing tables or data.
        /// </summary>
        private async void btnCreateDatabase_Click(object sender, EventArgs e)
        {
            string dbName = txtNewDbName.Text.Trim();
            if (string.IsNullOrWhiteSpace(dbName))
            {
                lblDbSetupStatus.ForeColor = System.Drawing.Color.Tomato;
                lblDbSetupStatus.Text = "Enter a database name to create.";
                return;
            }

            if (!Regex.IsMatch(dbName, @"^[A-Za-z0-9_]+$"))
            {
                lblDbSetupStatus.ForeColor = System.Drawing.Color.Tomato;
                lblDbSetupStatus.Text = "Database name can only contain letters, numbers, and underscores.";
                return;
            }

            string server = txtDbServer.Text.Trim();
            string user = txtDbUser.Text.Trim();
            string password = txtDbPassword.Text;
            int timeout = (int)nud_connectionTimeout.Value;

            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(user))
            {
                lblDbSetupStatus.ForeColor = System.Drawing.Color.Tomato;
                lblDbSetupStatus.Text = "Enter the database server and user (Settings tab) first.";
                return;
            }

            DialogResult confirm = ConfirmDialog.Show(this,
                $"This will create the database \"{dbName}\" (if it doesn't already exist) and its " +
                $"tables on {server}, using the connection details in the Settings tab.\n\nContinue? (Y/N)",
                "Create Database", MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            btnCreateDatabase.Enabled = false;
            lblDbSetupStatus.ForeColor = System.Drawing.Color.Gray;
            lblDbSetupStatus.Text = "Connecting...";

            try
            {
                DbSetupResult result = await DbSetupService.CreateDatabaseAsync(
                    server, user, password, timeout, dbName,
                    new Progress<string>(msg => lblDbSetupStatus.Text = msg));

                lblDbSetupStatus.ForeColor = System.Drawing.Color.Gray;
                lblDbSetupStatus.Text = result.DatabaseAlreadyExisted
                    ? $"Database already existed. {result.TablesCreated} table(s) created, {result.TablesAlreadyExisted} already existed."
                    : $"Database created. {result.TablesCreated} table(s) created.";

                if (chkSwitchToNewDb.Checked)
                {
                    txtDbName.Text = dbName;
                    ShowStatus($"Database \"{dbName}\" is ready - click Save below to switch this app to it.", isError: false);
                }
            }
            catch (Exception ex)
            {
                lblDbSetupStatus.ForeColor = System.Drawing.Color.Tomato;
                lblDbSetupStatus.Text = $"Failed: {ex.Message}";
            }
            finally
            {
                btnCreateDatabase.Enabled = true;
            }
        }

        private void LoadTrialLicense()
        {
            chkTrialEnabled.Checked = TrialLicense.IsEnabled;
            nud_trialDays.Value = Math.Min(Math.Max(TrialLicense.TrialDays, nud_trialDays.Minimum), nud_trialDays.Maximum);
            chkTrialAutoClose.Checked = TrialLicense.AutoCloseOnExpiryEnabled;
            SetTrialControlsEnabled(chkTrialEnabled.Checked);

            int tierIndex = cmbSubscriptionTier.Items.IndexOf(TrialLicense.SubscriptionTier);
            cmbSubscriptionTier.SelectedIndex = tierIndex >= 0 ? tierIndex : 0;

            UpdateTrialStatusLabel();
        }

        private string SelectedTierLicenseDirectory()
        {
            string tier = cmbSubscriptionTier.SelectedItem as string ?? "Basic";
            string dir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "App Inventory", "Licenses", tier);
            try { System.IO.Directory.CreateDirectory(dir); } catch { }
            return dir;
        }

        private void SetTrialControlsEnabled(bool enabled) => nud_trialDays.Enabled = enabled;

        private void chkTrialEnabled_CheckedChanged(object sender, EventArgs e) =>
            SetTrialControlsEnabled(chkTrialEnabled.Checked);

        private void UpdateTrialStatusLabel()
        {
            if (TrialLicense.IsLicensed)
            {
                string tierSuffix = string.IsNullOrEmpty(TrialLicense.SubscriptionTier) ? "" : $" ({TrialLicense.SubscriptionTier})";
                lblTrialStatus.Text = TrialLicense.LicenseExpiryUtc.HasValue
                    ? $"Status: Licensed{tierSuffix} until {TrialLicense.LicenseExpiryUtc.Value:yyyy-MM-dd} - the trial gate does not apply."
                    : $"Status: Licensed{tierSuffix} (no expiry) - the trial gate does not apply.";
                return;
            }

            if (TrialLicense.HasExpiredLicenseKey)
            {
                lblTrialStatus.Text = $"Status: License key expired {TrialLicense.LicenseExpiryUtc:yyyy-MM-dd} - enter a new one.";
                return;
            }

            if (!TrialLicense.TrialStartDate.HasValue)
            {
                lblTrialStatus.Text = "Status: Trial has not started yet on this machine.";
                return;
            }

            lblTrialStatus.Text = TrialLicense.DaysRemaining > 0
                ? $"Status: Trial started {TrialLicense.TrialStartDate.Value.ToLocalTime():g} - {TrialLicense.DaysRemaining} day(s) remaining."
                : "Status: Trial has expired.";
        }

        private void btnActivateLicense_Click(object sender, EventArgs e)
        {
            if (TrialLicense.TryActivate(txtLicenseKey.Text))
            {
                ShowStatus("License activated.", isError: false);
                txtLicenseKey.Clear();
                UpdateTrialStatusLabel();
            }
            else
            {
                ShowStatus("That license key isn't valid for this store.", isError: true);
            }
        }

        private void btnImportLicenseFile_Click(object sender, EventArgs e)
        {
            openFileDialogLicense.InitialDirectory = SelectedTierLicenseDirectory();
            if (openFileDialogLicense.ShowDialog(this) != DialogResult.OK)
                return;

            if (TrialLicense.TryActivateFromFile(openFileDialogLicense.FileName, out string error))
            {
                ShowStatus("License activated from file.", isError: false);
                UpdateTrialStatusLabel();
            }
            else
            {
                ShowStatus(error, isError: true);
            }
        }

        private void LoadDataSync()
        {
            string configuredPath = ConfigurationManager.AppSettings["ACCESS_SYNC_PATH"];
            txtAccessDbPath.Text = !string.IsNullOrWhiteSpace(configuredPath) ? configuredPath : AccessSyncService.GetDefaultPath();
            UpdateLastSyncLabel();
        }

        private void UpdateLastSyncLabel()
        {
            DateTime? lastSync = AccessSyncService.GetLastSync();
            lblLastSync.Text = "Last sync: " + (lastSync.HasValue ? lastSync.Value.ToString("g") : "never");

            DateTime? lastPush = AccessSyncService.GetLastPush();
            lblLastPush.Text = "Last push: " + (lastPush.HasValue ? lastPush.Value.ToString("g") : "never");
        }

        private static readonly string[] CommonManagerPins =
        {
            "0000", "1111", "1212", "1234", "2222", "2580", "3333",
            "4321", "4444", "5555", "6666", "7777", "8888", "9999"
        };

        // Only re-alert the tray when the set of warnings actually changes, so editing a field
        // doesn't pop a fresh balloon on every keystroke.
        private string _lastNotifiedWarningSignature;

        private List<string> ComputeWarnings()
        {
            var warnings = new List<string>();

            if (string.IsNullOrEmpty(txtDbPassword.Text))
                warnings.Add("Database password is blank.");

            if (CommonManagerPins.Contains(txtManagerPin.Text.Trim()))
                warnings.Add("Manager PIN is a commonly guessed value.");

            if (CommonManagerPins.Contains(txtAdminPin.Text.Trim()))
                warnings.Add("System Admin PIN is a commonly guessed value.");

            if (!string.IsNullOrEmpty(txtManagerPin.Text) &&
                string.Equals(txtManagerPin.Text.Trim(), txtAdminPin.Text.Trim(), StringComparison.Ordinal))
                warnings.Add("Manager PIN and System Admin PIN are the same - a manager could reach System Configuration.");

            if (string.IsNullOrWhiteSpace(txtStoreEmail.Text))
            {
                warnings.Add("No store email is set, so scheduled reports have nowhere to send.");
            }
            else if (rdoProviderGmail.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtGmailClientId.Text) || string.IsNullOrWhiteSpace(txtGmailClientSecret.Text) ||
                    string.IsNullOrWhiteSpace(txtGmailRefreshToken.Text) || string.IsNullOrWhiteSpace(txtGmailSender.Text))
                    warnings.Add("Store email is set but the Gmail API connection is incomplete, so scheduled reports will fail to send.");
            }
            else if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                warnings.Add("Store email is set but the SMTP host is blank, so scheduled reports will fail to send.");
            }

            if (cbxReceiptPrinter.SelectedItem == null)
                warnings.Add("No receipt printer is selected; receipts cannot print.");

            if ((chkRemoteControlEnabled.Checked || chkBluetoothEnabled.Checked) && string.IsNullOrWhiteSpace(txtRemoteToken.Text))
                warnings.Add("Remote control is enabled but has no shared token set - it will not start.");

            if (_maintenanceEnabledCache)
                warnings.Add("Maintenance mode is ON - cashiers cannot log in on any terminal sharing this database.");

            return warnings;
        }

        /// <summary>
        /// Recomputes the list of configuration risks from the form's current (possibly unsaved)
        /// field values and reflects them in the hoverable warning bar - hidden when there's
        /// nothing to flag, otherwise showing a count that expands into the full list as a tooltip
        /// on hover.
        /// </summary>
        private void UpdateWarningBar()
        {
            List<string> warnings = ComputeWarnings();

            lblWarningBar.Visible = warnings.Count > 0;
            if (warnings.Count == 0)
                return;

            lblWarningBar.Text = warnings.Count == 1
                ? "⚠ 1 configuration warning - hover for details"
                : $"⚠ {warnings.Count} configuration warnings - hover for details";
            tooltipWarnings.SetToolTip(lblWarningBar, string.Join(Environment.NewLine, warnings));
        }

        /// <summary>
        /// the same warnings as the hover bar, so they're visible even after this window closes.
        /// Skipped if the warning set hasn't changed since the last balloon, to avoid spamming one
        /// per keystroke.
        /// </summary>
        private void NotifyTrayOfWarnings()
        {
            List<string> warnings = ComputeWarnings();
            string signature = string.Join("|", warnings);

            if (signature == _lastNotifiedWarningSignature)
                return;
            _lastNotifiedWarningSignature = signature;

            if (warnings.Count == 0)
                return;

            string message = string.Join(Environment.NewLine, warnings.Select(w => "• " + w));
            (Owner as Purchases)?.ShowTrayNotification("Configuration Warnings", message);
        }

        private void LoadTopSoldSettings()
        {
            string current = ConfigurationManager.AppSettings["TOP_SOLD_ITEM_NAME_CASE"] ?? "Title";
            int index = cbxTopSoldNameCase.Items.IndexOf(current);
            cbxTopSoldNameCase.SelectedIndex = index >= 0 ? index : cbxTopSoldNameCase.Items.IndexOf("Title");

            string managePricesCurrent = ConfigurationManager.AppSettings["MANAGE_PRICES_ITEM_NAME_CASE"] ?? "Title";
            int managePricesIndex = cbxManagePricesNameCase.Items.IndexOf(managePricesCurrent);
            cbxManagePricesNameCase.SelectedIndex = managePricesIndex >= 0 ? managePricesIndex : cbxManagePricesNameCase.Items.IndexOf("Title");
        }

        private void LoadReportSchedule()
        {
            cbxReportFrequency.Items.Clear();
            cbxReportFrequency.Items.AddRange(Enum.GetNames(typeof(ScheduledReportService.Frequency)));

            int index = cbxReportFrequency.Items.IndexOf(ScheduledReportService.GetConfiguredFrequency().ToString());
            cbxReportFrequency.SelectedIndex = index >= 0 ? index : cbxReportFrequency.Items.IndexOf("Weekly");

            txtSmtpHost.Text = ConfigurationManager.AppSettings["SMTP_HOST"];
            txtSmtpFrom.Text = ConfigurationManager.AppSettings["SMTP_FROM"];
            txtSmtpUser.Text = ConfigurationManager.AppSettings["SMTP_USER"];
            txtSmtpPassword.Text = ConfigurationManager.AppSettings["SMTP_PASSWORD"];

            if (int.TryParse(ConfigurationManager.AppSettings["SMTP_PORT"], out int port))
                nud_smtpPort.Value = Math.Min(Math.Max(port, nud_smtpPort.Minimum), nud_smtpPort.Maximum);

            chkSmtpUseSsl.Checked = !bool.TryParse(ConfigurationManager.AppSettings["SMTP_USE_SSL"], out bool useSsl) || useSsl;

            txtGmailClientId.Text = ConfigurationManager.AppSettings["GMAIL_CLIENT_ID"];
            txtGmailClientSecret.Text = ConfigurationManager.AppSettings["GMAIL_CLIENT_SECRET"];
            txtGmailRefreshToken.Text = ConfigurationManager.AppSettings["GMAIL_REFRESH_TOKEN"];
            txtGmailSender.Text = ConfigurationManager.AppSettings["GMAIL_SENDER"];

            bool useGmail = string.Equals(ConfigurationManager.AppSettings["MAIL_PROVIDER"], "Gmail", StringComparison.OrdinalIgnoreCase);
            rdoProviderGmail.Checked = useGmail;
            rdoProviderSmtp.Checked = !useGmail;
            UpdateMailProviderGroupsVisibility();

            UpdateScheduleStatusLabel();
        }

        private void LoadInventoryPrintSchedule()
        {
            cbxInventoryPrintFrequency.Items.Clear();
            cbxInventoryPrintFrequency.Items.AddRange(Enum.GetNames(typeof(ScheduledReportService.Frequency)));

            int index = cbxInventoryPrintFrequency.Items.IndexOf(ScheduledInventoryPrintService.GetConfiguredFrequency().ToString());
            cbxInventoryPrintFrequency.SelectedIndex = index >= 0 ? index : cbxInventoryPrintFrequency.Items.IndexOf("Weekly");

            chkInventoryPrintEnabled.Checked = ScheduledInventoryPrintService.IsScheduleEnabled;

            UpdateInventoryPrintStatusLabel();
        }

        private void UpdateInventoryPrintStatusLabel()
        {
            DateTime? lastPrinted = ScheduledInventoryPrintService.GetLastPrinted();
            DateTime? nextDue = ScheduledInventoryPrintService.GetNextDue();

            string lastPrintedText = lastPrinted.HasValue ? lastPrinted.Value.ToString("g") : "never";
            string nextDueText = nextDue.HasValue ? nextDue.Value.ToString("g") : "as soon as it's enabled and a receipt printer is configured";

            lblInventoryPrintStatus.Text = $"Last printed: {lastPrintedText}\nNext due: {nextDueText}";
        }

        private async void btnPrintInventoryNow_Click(object sender, EventArgs e)
        {
            btnPrintInventoryNow.Enabled = false;
            ShowStatus("Preparing inventory report preview...", isError: false);
            try
            {
                using (PrintDocument document = await ScheduledInventoryPrintService.CreatePrintDocumentAsync())
                using (var preview = new PrintPreviewDialog())
                {
                    preview.Document = document;
                    preview.Width = 700;
                    preview.Height = 700;
                    preview.Document.PrintController = new StandardPrintController();

                    // Always opens at a literal 100% zoom (actual print scale) rather than
                    // AutoZoom's fit-to-window, which rescaled the page to whatever size the
                    // dialog happened to be instead of showing it the size it will really print.
                    preview.PrintPreviewControl.AutoZoom = false;
                    preview.PrintPreviewControl.Zoom = 1.0;
                    ShowStatus("", isError: false);
                    preview.ShowDialog(this);
                }

                // Only reflects a real change if the admin actually clicked Print inside the
                // preview (EndPrint is what writes the last-printed marker) - closing the preview
                // without printing leaves this exactly as it was.
                UpdateInventoryPrintStatusLabel();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to preview inventory report: {ex.Message}", isError: true);
            }
            finally
            {
                btnPrintInventoryNow.Enabled = true;
            }
        }

        private void LoadOfflineSync()
        {
            chkPendingItemSyncEnabled.Checked = PendingItemSyncService.AutoSyncEnabled;
            UpdatePendingItemSyncStatusLabel();
        }

        private void UpdatePendingItemSyncStatusLabel()
        {
            int pendingCount = YourApp.Data.PendingItemStore.GetPendingCount();
            lblPendingItemSyncStatus.Text = pendingCount == 0
                ? "0 item(s) queued offline."
                : $"{pendingCount} item(s) queued offline, waiting to sync.";
        }

        private async void btnSyncPendingItemsNow_Click(object sender, EventArgs e)
        {
            btnSyncPendingItemsNow.Enabled = false;
            ShowStatus("Syncing offline-queued items...", isError: false);
            try
            {
                int synced = await PendingItemSyncService.SyncPendingItemsAsync();
                ShowStatus(synced == 0 ? "Nothing queued to sync." : $"Synced {synced} item(s) to the database.", isError: false);
                UpdatePendingItemSyncStatusLabel();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to sync offline-queued items: {ex.Message}", isError: true);
            }
            finally
            {
                btnSyncPendingItemsNow.Enabled = true;
            }
        }

        private void UpdateMailProviderGroupsVisibility()
        {
            gbxGmail.Visible = rdoProviderGmail.Checked;
            gbxSmtp.Visible = !rdoProviderGmail.Checked;
        }

        private void rdoProvider_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMailProviderGroupsVisibility();
            UpdateWarningBar();
        }

        private void chkShowGmailClientSecret_CheckedChanged(object sender, EventArgs e) =>
            txtGmailClientSecret.UseSystemPasswordChar = !chkShowGmailClientSecret.Checked;

        private void chkShowGmailRefreshToken_CheckedChanged(object sender, EventArgs e) =>
            txtGmailRefreshToken.UseSystemPasswordChar = !chkShowGmailRefreshToken.Checked;

        private void UpdateScheduleStatusLabel()
        {
            DateTime? lastSent = ScheduledReportService.GetLastSent();
            DateTime? nextDue = ScheduledReportService.GetNextDue();

            string lastSentText = lastSent.HasValue ? lastSent.Value.ToString("g") : "never";
            string nextDueText = nextDue.HasValue ? nextDue.Value.ToString("g") : "as soon as an email address is set and saved";

            lblScheduleStatus.Text = $"Last sent: {lastSentText}\nNext due: {nextDueText}";
        }

        private void LoadSalesDefaults()
        {
            bool enabled = string.Equals(ConfigurationManager.AppSettings["DEFAULT_SELLTYPE_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);
            chkDefaultSelltype.Checked = enabled;

            int index = int.TryParse(ConfigurationManager.AppSettings["DEFAULT_SELLTYPE_INDEX"], out int i) ? i : 0;
            cbxDefaultSelltype.SelectedIndex = index >= 0 && index <= 2 ? index : 0;

            cbxDefaultSelltype.Enabled = enabled;

            bool autoHoldEnabled = string.Equals(ConfigurationManager.AppSettings["AUTO_HOLD_IDLE_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);
            chkAutoHoldEnabled.Checked = autoHoldEnabled;

            int autoHoldMinutes = int.TryParse(ConfigurationManager.AppSettings["AUTO_HOLD_IDLE_MINUTES"], out int mins) ? mins : 30;
            nud_autoHoldMinutes.Value = Math.Min(Math.Max(autoHoldMinutes, nud_autoHoldMinutes.Minimum), nud_autoHoldMinutes.Maximum);

            nud_autoHoldMinutes.Enabled = autoHoldEnabled;

            chkBlockZeroPriceItems.Checked = !string.Equals(ConfigurationManager.AppSettings["BLOCK_ZERO_PRICE_ITEMS"], "false", StringComparison.OrdinalIgnoreCase);

            chkAutoStartWithWindows.Checked = AutoStartService.IsEnabled;

            chkHolidayGreetingEnabled.Checked = !string.Equals(ConfigurationManager.AppSettings["HOLIDAY_GREETING_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);

            string selltypeUiStyle = ConfigurationManager.AppSettings["SELLTYPE_UI_STYLE"] ?? "Dropdown";
            int selltypeUiStyleIndex = cbxSelltypeUiStyle.Items.IndexOf(selltypeUiStyle);
            cbxSelltypeUiStyle.SelectedIndex = selltypeUiStyleIndex >= 0 ? selltypeUiStyleIndex : cbxSelltypeUiStyle.Items.IndexOf("Dropdown");
        }

        private void chkDefaultSelltype_CheckedChanged(object sender, EventArgs e) =>
            cbxDefaultSelltype.Enabled = chkDefaultSelltype.Checked;

        private void chkAutoHoldEnabled_CheckedChanged(object sender, EventArgs e) =>
            nud_autoHoldMinutes.Enabled = chkAutoHoldEnabled.Checked;

        private void LoadAlertSettings()
        {
            bool enabled = !string.Equals(ConfigurationManager.AppSettings["ALERT_AUTO_DISMISS_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);
            chkAlertAutoDismissEnabled.Checked = enabled;

            int seconds = int.TryParse(ConfigurationManager.AppSettings["ALERT_AUTO_DISMISS_SECONDS"], out int s) && s > 0 ? s : 5;
            nud_alertAutoDismissSeconds.Value = Math.Min(Math.Max(seconds, nud_alertAutoDismissSeconds.Minimum), nud_alertAutoDismissSeconds.Maximum);

            nud_alertAutoDismissSeconds.Enabled = enabled;

            bool searchClearEnabled = !string.Equals(ConfigurationManager.AppSettings["SEARCH_AUTO_CLEAR_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);
            chkSearchAutoClearEnabled.Checked = searchClearEnabled;

            int searchClearSeconds = int.TryParse(ConfigurationManager.AppSettings["SEARCH_AUTO_CLEAR_SECONDS"], out int sc) && sc > 0 ? sc : 10;
            nud_searchAutoClearSeconds.Value = Math.Min(Math.Max(searchClearSeconds, nud_searchAutoClearSeconds.Minimum), nud_searchAutoClearSeconds.Maximum);

            nud_searchAutoClearSeconds.Enabled = searchClearEnabled;

            bool qtyResetEnabled = !string.Equals(ConfigurationManager.AppSettings["QTY_AUTO_RESET_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);
            chkQtyAutoResetEnabled.Checked = qtyResetEnabled;

            int qtyResetSeconds = int.TryParse(ConfigurationManager.AppSettings["QTY_AUTO_RESET_SECONDS"], out int qs) && qs > 0 ? qs : 5;
            nud_qtyAutoResetSeconds.Value = Math.Min(Math.Max(qtyResetSeconds, nud_qtyAutoResetSeconds.Minimum), nud_qtyAutoResetSeconds.Maximum);

            nud_qtyAutoResetSeconds.Enabled = qtyResetEnabled;
        }

        private void chkAlertAutoDismissEnabled_CheckedChanged(object sender, EventArgs e) =>
            nud_alertAutoDismissSeconds.Enabled = chkAlertAutoDismissEnabled.Checked;

        private void chkSearchAutoClearEnabled_CheckedChanged(object sender, EventArgs e) =>
            nud_searchAutoClearSeconds.Enabled = chkSearchAutoClearEnabled.Checked;

        private void chkQtyAutoResetEnabled_CheckedChanged(object sender, EventArgs e) =>
            nud_qtyAutoResetSeconds.Enabled = chkQtyAutoResetEnabled.Checked;

        private void LoadMenuVisibility()
        {
            string raw = ConfigurationManager.AppSettings["HIDDEN_MENUS"] ?? Purchases.DefaultHiddenMenus;
            var hidden = new HashSet<string>(
                raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()),
                StringComparer.OrdinalIgnoreCase);

            clbMenus.Items.Clear();
            foreach (var menu in Purchases.ConfigurableMenuItems)
                clbMenus.Items.Add(menu.Label, !hidden.Contains(menu.Name));
        }

        private void LoadItemSearchColumns()
        {
            string raw = ConfigurationManager.AppSettings["ITEM_SEARCH_HIDDEN_COLUMNS"] ?? Purchases.DefaultHiddenItemSearchColumns;
            var hidden = new HashSet<string>(
                raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()),
                StringComparer.OrdinalIgnoreCase);

            clbItemSearchColumns.Items.Clear();
            foreach (var col in Purchases.GetOrderedItemSearchColumns())
                clbItemSearchColumns.Items.Add(col.Label, !hidden.Contains(col.Name));

            chkItemSearchDateSortDescending.Checked = Purchases.ItemSearchDateSortDescending;
            chkItemSearchNameSortDescending.Checked = Purchases.ItemSearchNameSortDescending;

            chkItemSearchBgCustom.Checked = YourApp.Data.ItemSearchGridTheme.IsConfigured;
            pnlItemSearchBgSwatch.BackColor = YourApp.Data.ItemSearchGridTheme.CustomColor ?? System.Drawing.Color.White;
            SetItemSearchBgSwatchControlsEnabled(chkItemSearchBgCustom.Checked);
        }

        private void SetItemSearchBgSwatchControlsEnabled(bool enabled)
        {
            pnlItemSearchBgSwatch.Enabled = enabled;
            btnPickItemSearchBgColor.Enabled = enabled;
        }

        private void chkItemSearchBgCustom_CheckedChanged(object sender, EventArgs e) =>
            SetItemSearchBgSwatchControlsEnabled(chkItemSearchBgCustom.Checked);

        private void btnPickItemSearchBgColor_Click(object sender, EventArgs e)
        {
            colorDialogAccent.Color = pnlItemSearchBgSwatch.BackColor;
            if (colorDialogAccent.ShowDialog(this) == DialogResult.OK)
                pnlItemSearchBgSwatch.BackColor = colorDialogAccent.Color;
        }

        private void LoadCartColumns()
        {
            string raw = ConfigurationManager.AppSettings["CART_HIDDEN_COLUMNS"] ?? Purchases.DefaultHiddenCartColumns;
            var hidden = new HashSet<string>(
                raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()),
                StringComparer.OrdinalIgnoreCase);

            clbCartColumns.Items.Clear();
            foreach (var col in Purchases.GetOrderedCartColumns())
                clbCartColumns.Items.Add(col.Label, !hidden.Contains(col.Name));

            chkCartBgCustom.Checked = YourApp.Data.CartBoardTheme.IsConfigured;
            pnlCartBgSwatch.BackColor = YourApp.Data.CartBoardTheme.CustomColor ?? System.Drawing.Color.White;
            SetCartBgSwatchControlsEnabled(chkCartBgCustom.Checked);
        }

        private void SetCartBgSwatchControlsEnabled(bool enabled)
        {
            pnlCartBgSwatch.Enabled = enabled;
            btnPickCartBgColor.Enabled = enabled;
        }

        private void chkCartBgCustom_CheckedChanged(object sender, EventArgs e) =>
            SetCartBgSwatchControlsEnabled(chkCartBgCustom.Checked);

        private void btnPickCartBgColor_Click(object sender, EventArgs e)
        {
            colorDialogAccent.Color = pnlCartBgSwatch.BackColor;
            if (colorDialogAccent.ShowDialog(this) == DialogResult.OK)
                pnlCartBgSwatch.BackColor = colorDialogAccent.Color;
        }

        private static void MoveCheckedListItem(CheckedListBox list, int delta)
        {
            int index = list.SelectedIndex;
            if (index < 0)
                return;

            int newIndex = index + delta;
            if (newIndex < 0 || newIndex >= list.Items.Count)
                return;

            object item = list.Items[index];
            bool isChecked = list.GetItemChecked(index);
            list.Items.RemoveAt(index);
            list.Items.Insert(newIndex, item);
            list.SetItemChecked(newIndex, isChecked);
            list.SelectedIndex = newIndex;
        }

        private void btnItemSearchColumnUp_Click(object sender, EventArgs e) => MoveCheckedListItem(clbItemSearchColumns, -1);
        private void btnItemSearchColumnDown_Click(object sender, EventArgs e) => MoveCheckedListItem(clbItemSearchColumns, 1);
        private void btnCartColumnUp_Click(object sender, EventArgs e) => MoveCheckedListItem(clbCartColumns, -1);
        private void btnCartColumnDown_Click(object sender, EventArgs e) => MoveCheckedListItem(clbCartColumns, 1);

        private static void SaveColumnConfig(Configuration config, CheckedListBox list,
            (string Name, string Label)[] columns, string hiddenKey, string orderKey)
        {
            var labelToName = columns.ToDictionary(c => c.Label, c => c.Name, StringComparer.Ordinal);
            var order = new List<string>();
            var hidden = new List<string>();

            for (int i = 0; i < list.Items.Count; i++)
            {
                if (!labelToName.TryGetValue(list.Items[i].ToString(), out string name))
                    continue;

                order.Add(name);
                if (!list.GetItemChecked(i))
                    hidden.Add(name);
            }

            SetSetting(config, hiddenKey, string.Join(",", hidden));
            SetSetting(config, orderKey, string.Join(",", order));
        }

        /// <summary>
        /// Seeds the slider from whatever override (if any) is already live in the current
        /// session - e.g. left on from an earlier visit to this tab - rather than always
        /// resetting it to off, since it's an in-memory testing knob, not a saved setting.
        /// </summary>
        private void LoadReceiptTesting()
        {
            chkPriceOverrideEnabled.Checked = Purchases.TestPriceOverride.HasValue;
            tbPriceOverride.Value = Math.Min(Math.Max((int)(Purchases.TestPriceOverride ?? 1m), tbPriceOverride.Minimum), tbPriceOverride.Maximum);
            UpdatePriceOverrideControls();
        }

        private void UpdatePriceOverrideControls()
        {
            tbPriceOverride.Enabled = chkPriceOverrideEnabled.Checked;
            lblPriceOverrideValue.Text = $"Override price: {tbPriceOverride.Value:N0}.00";
            Purchases.TestPriceOverride = chkPriceOverrideEnabled.Checked ? (decimal?)tbPriceOverride.Value : null;
        }

        private void chkPriceOverrideEnabled_CheckedChanged(object sender, EventArgs e) => UpdatePriceOverrideControls();

        private void tbPriceOverride_Scroll(object sender, EventArgs e) => UpdatePriceOverrideControls();

        private void btnPreviewReceipt_Click(object sender, EventArgs e)
        {
            var purchases = Owner as Purchases;
            if (purchases == null)
            {
                ShowStatus("Receipt preview needs the main POS window open.", isError: true);
                return;
            }

            using (var preview = new ReceiptPreviewForm(purchases))
                preview.ShowDialog(this);

            // The preview window's own slider writes straight to Purchases.TestPriceOverride
            // (same as this tab's) - resync this tab's controls in case it was left in a
            // different state than when this dialog opened.
            LoadReceiptTesting();
        }

        /// <summary>
        /// Lists installed printers and preselects whichever one is currently configured for
        /// receipts (falling back to the Windows default printer) so the admin can see - and
        /// change - which physical thermal printer's paper size drives the receipt preview.
        /// </summary>
        // Index lines up with cbxPaperWidth's items ("Automatic", "80mm", "58mm"); 0 means "don't
        // override - use whatever ApplyReceiptPrinterSettings detected from the printer driver".
        private static readonly int[] PaperWidthMmByComboIndex = { 0, 80, 58 };

        private void LoadReceiptPrinter()
        {
            cbxReceiptPrinter.Items.Clear();
            foreach (string printerName in PrinterSettings.InstalledPrinters)
                cbxReceiptPrinter.Items.Add(printerName);

            chkSaveCounterReceiptAsJpeg.Checked = string.Equals(
                ConfigurationManager.AppSettings["COUNTER_RECEIPT_MODE"], "Jpeg", StringComparison.OrdinalIgnoreCase);

            string configuredJpegPath = ConfigurationManager.AppSettings["COUNTER_RECEIPT_JPEG_PATH"];
            txtCounterReceiptPath.Text = !string.IsNullOrWhiteSpace(configuredJpegPath)
                ? configuredJpegPath
                : Path.Combine(GetDefaultReceiptsDirectory(), "Jpeg");
            UpdateCounterReceiptPathEnabled();

            int configuredWidthMm = int.TryParse(ConfigurationManager.AppSettings["RECEIPT_PAPER_WIDTH_MM"], out int mm) ? mm : 58;
            int widthIndex = Array.IndexOf(PaperWidthMmByComboIndex, configuredWidthMm);
            cbxPaperWidth.SelectedIndex = widthIndex >= 0 ? widthIndex : Array.IndexOf(PaperWidthMmByComboIndex, 58);

            chkPaperWidthIsDefault.Checked = true;
            cbxPaperWidth.Enabled = true;

            if (cbxReceiptPrinter.Items.Count == 0)
            {
                lblPaperSizeInfo.Text = "No printers installed on this machine.";
                return;
            }

            string configured = ConfigurationManager.AppSettings["RECEIPT_PRINTER_NAME"];
            int index = !string.IsNullOrWhiteSpace(configured)
                ? cbxReceiptPrinter.Items.IndexOf(configured)
                : -1;

            cbxReceiptPrinter.SelectedIndex = index >= 0 ? index : 0;

            string systemDefault = new PrinterSettings().PrinterName;
            chkSetAsSystemDefaultPrinter.Checked = string.Equals(
                cbxReceiptPrinter.SelectedItem as string, systemDefault, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetDefaultReceiptsDirectory() => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Receipts");

        private void UpdateCounterReceiptPathEnabled()
        {
            txtCounterReceiptPath.Enabled = chkSaveCounterReceiptAsJpeg.Checked;
            btnBrowseCounterReceiptPath.Enabled = chkSaveCounterReceiptAsJpeg.Checked;
        }

        private void chkSaveCounterReceiptAsJpeg_CheckedChanged(object sender, EventArgs e) =>
            UpdateCounterReceiptPathEnabled();

        private void chkPaperWidthIsDefault_CheckedChanged(object sender, EventArgs e) =>
            cbxPaperWidth.Enabled = chkPaperWidthIsDefault.Checked;

        private void btnBrowseCounterReceiptPath_Click(object sender, EventArgs e)
        {
            string currentPath = txtCounterReceiptPath.Text.Trim();
            if (Directory.Exists(currentPath))
                folderBrowserDialogCounterReceipt.SelectedPath = currentPath;

            if (folderBrowserDialogCounterReceipt.ShowDialog(this) == DialogResult.OK)
                txtCounterReceiptPath.Text = folderBrowserDialogCounterReceipt.SelectedPath;
        }

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string name);

        private void cbxReceiptPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxReceiptPrinter.SelectedItem == null)
            {
                lblPaperSizeInfo.Text = "Detected paper size: -";
                UpdateWarningBar();
                return;
            }

            try
            {
                var printerSettings = new PrinterSettings { PrinterName = cbxReceiptPrinter.SelectedItem.ToString() };
                PaperSize paper = printerSettings.DefaultPageSettings.PaperSize;
                double widthMm = paper.Width * 25.4 / 100.0;
                double heightMm = paper.Height * 25.4 / 100.0;
                lblPaperSizeInfo.Text = $"Detected paper size: {paper.PaperName} ({widthMm:0}mm x {heightMm:0}mm)";
            }
            catch (Exception)
            {
                lblPaperSizeInfo.Text = "Could not read this printer's paper size.";
            }

            UpdateWarningBar();
        }

        /// <summary>
        /// Preselects the accent swatches from ACCENT_BACKGROUND_COLOR/ACCENT_FOREGROUND_COLOR
        /// if either is set, falling back to the system control colors otherwise.
        /// </summary>
        private void LoadAppearance()
        {
            pnlBgSwatch.BackColor = YourApp.Data.AppTheme.BackgroundColor;
            pnlFgSwatch.BackColor = YourApp.Data.AppTheme.ForegroundColor;
            chkEnableAccent.Checked = YourApp.Data.AppTheme.IsConfigured;
            SetSwatchControlsEnabled(chkEnableAccent.Checked);

            decimal deltaPt = (decimal)YourApp.Data.AppFontScale.DeltaPt;
            nud_fontScalePt.Value = Math.Min(Math.Max(deltaPt, nud_fontScalePt.Minimum), nud_fontScalePt.Maximum);

            txtLogoUrl.Text = ConfigurationManager.AppSettings["LOGO_URL"];

            chkLayoutEditEnabled.Checked = string.Equals(
                ConfigurationManager.AppSettings["LAYOUT_EDIT_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);
        }

        private void SetSwatchControlsEnabled(bool enabled)
        {
            btnPickBgColor.Enabled = enabled;
            btnPickFgColor.Enabled = enabled;
        }

        private void chkEnableAccent_CheckedChanged(object sender, EventArgs e) =>
            SetSwatchControlsEnabled(chkEnableAccent.Checked);

        private void btnPickBgColor_Click(object sender, EventArgs e)
        {
            colorDialogAccent.Color = pnlBgSwatch.BackColor;
            if (colorDialogAccent.ShowDialog(this) == DialogResult.OK)
                pnlBgSwatch.BackColor = colorDialogAccent.Color;
        }

        private void btnPickFgColor_Click(object sender, EventArgs e)
        {
            colorDialogAccent.Color = pnlFgSwatch.BackColor;
            if (colorDialogAccent.ShowDialog(this) == DialogResult.OK)
                pnlFgSwatch.BackColor = colorDialogAccent.Color;
        }

        private void LoadLayoutRevision()
        {
            txtLayoutRevision.Text = YourApp.Data.MainLayoutStore.ReadRawXml();
        }

        private void btnRefreshLayoutText_Click(object sender, EventArgs e)
        {
            LoadLayoutRevision();
            ShowStatus("Reloaded the currently saved layout.", isError: false);
        }

        private void btnApplyLayoutText_Click(object sender, EventArgs e)
        {
            try
            {
                YourApp.Data.MainLayoutStore.WriteRawXml(txtLayoutRevision.Text);
                ShowStatus("Layout updated - takes effect next time the main screen loads.", isError: false);
            }
            catch (Exception ex)
            {
                ShowStatus("Invalid layout XML: " + ex.Message, isError: true);
            }
        }

        private void LoadLayoutPresets()
        {
            cboLayoutPreset.Items.Clear();
            cboLayoutPreset.Items.AddRange(LayoutPresets.All);
            cboLayoutPreset.SelectedIndex = 0;
        }

        /// <summary>
        /// Rebuilds a column CheckedListBox to match a preset: listed columns first, in order
        /// and checked (visible); everything else from <paramref name="allColumns"/> appended
        /// unchecked (hidden) - the same shape <see cref="SaveColumnConfig"/> already expects.
        /// </summary>
        private static void ApplyColumnPreset(CheckedListBox list, (string Name, string Label)[] allColumns, string[] visibleNamesInOrder)
        {
            var labelByName = allColumns.ToDictionary(c => c.Name, c => c.Label, StringComparer.OrdinalIgnoreCase);
            var visible = new HashSet<string>(visibleNamesInOrder, StringComparer.OrdinalIgnoreCase);

            list.Items.Clear();
            foreach (string name in visibleNamesInOrder)
                if (labelByName.TryGetValue(name, out string label))
                    list.Items.Add(label, true);

            foreach (var col in allColumns)
                if (!visible.Contains(col.Name))
                    list.Items.Add(col.Label, false);
        }

        private void btnApplyLayoutPreset_Click(object sender, EventArgs e)
        {
            if (!(cboLayoutPreset.SelectedItem is LayoutPreset preset))
                return;

            nud_fontScalePt.Value = Math.Min(Math.Max(preset.FontScalePt, nud_fontScalePt.Minimum), nud_fontScalePt.Maximum);
            ApplyColumnPreset(clbCartColumns, Purchases.CartColumns, preset.VisibleCartColumns);
            ApplyColumnPreset(clbItemSearchColumns, Purchases.ItemSearchColumns, preset.VisibleItemSearchColumns);

            ShowStatus($"\"{preset.Name}\" preset loaded into Appearance/Cart Columns/Item Search - click Save to keep it.", isError: false);
        }

        private void btnLoadLayoutFile_Click(object sender, EventArgs e)
        {
            if (openFileDialogLayout.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                txtLayoutRevision.Text = File.ReadAllText(openFileDialogLayout.FileName);
                ShowStatus("Loaded layout text from file - review it, then click Apply.", isError: false);
            }
            catch (Exception ex)
            {
                ShowStatus("Could not read that file: " + ex.Message, isError: true);
            }
        }

        private void btnSaveLayoutFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialogLayout.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                File.WriteAllText(saveFileDialogLayout.FileName, txtLayoutRevision.Text);
                ShowStatus("Saved layout text to file.", isError: false);
            }
            catch (Exception ex)
            {
                ShowStatus("Could not save that file: " + ex.Message, isError: true);
            }
        }

        private void chkShowDbPassword_CheckedChanged(object sender, EventArgs e) =>
            txtDbPassword.UseSystemPasswordChar = !chkShowDbPassword.Checked;

        private void chkShowPin_CheckedChanged(object sender, EventArgs e) =>
            txtManagerPin.UseSystemPasswordChar = !chkShowPin.Checked;

        private void chkShowAdminPin_CheckedChanged(object sender, EventArgs e) =>
            txtAdminPin.UseSystemPasswordChar = !chkShowAdminPin.Checked;

        private void chkShowSmtpPassword_CheckedChanged(object sender, EventArgs e) =>
            txtSmtpPassword.UseSystemPasswordChar = !chkShowSmtpPassword.Checked;

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text) || string.IsNullOrWhiteSpace(txtStoreCode.Text) ||
                string.IsNullOrWhiteSpace(txtDbServer.Text) || string.IsNullOrWhiteSpace(txtDbName.Text) ||
                string.IsNullOrWhiteSpace(txtDbUser.Text))
            {
                ShowStatus("Store info and database fields cannot be blank.", isError: true);
                return;
            }

            if (!Regex.IsMatch(txtManagerPin.Text.Trim(), @"^\d{4,8}$"))
            {
                ShowStatus("Manager PIN must be 4-8 digits.", isError: true);
                return;
            }

            if (!Regex.IsMatch(txtAdminPin.Text.Trim(), @"^\d{4,8}$"))
            {
                ShowStatus("System Admin PIN must be 4-8 digits.", isError: true);
                return;
            }

            if (chkSaveCounterReceiptAsJpeg.Checked && string.IsNullOrWhiteSpace(txtCounterReceiptPath.Text))
            {
                ShowStatus("Choose a save folder for the counter's copy JPEG, or uncheck that option.", isError: true);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtStoreEmail.Text) &&
                !Regex.IsMatch(txtStoreEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowStatus("Store email is not a valid email address.", isError: true);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtLogoUrl.Text) &&
                (!Uri.TryCreate(txtLogoUrl.Text.Trim(), UriKind.Absolute, out Uri logoUri) ||
                 (logoUri.Scheme != Uri.UriSchemeHttp && logoUri.Scheme != Uri.UriSchemeHttps)))
            {
                ShowStatus("Logo image URL is not a valid http(s) URL.", isError: true);
                return;
            }

            if (rdoProviderGmail.Checked &&
                (string.IsNullOrWhiteSpace(txtGmailClientId.Text) || string.IsNullOrWhiteSpace(txtGmailClientSecret.Text) ||
                 string.IsNullOrWhiteSpace(txtGmailRefreshToken.Text) || string.IsNullOrWhiteSpace(txtGmailSender.Text)))
            {
                ShowStatus("Gmail API is selected - Client ID, Client Secret, Refresh Token, and Sender Address are all required.", isError: true);
                return;
            }

            if ((chkRemoteControlEnabled.Checked || chkBluetoothEnabled.Checked) && string.IsNullOrWhiteSpace(txtRemoteToken.Text))
            {
                ShowStatus("Remote control (Network or Bluetooth) is enabled but has no shared token - click Generate Token or uncheck both.", isError: true);
                return;
            }

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                SetSetting(config, "STORE_NAME", txtStoreName.Text.Trim());
                SetSetting(config, "STORE_CODE", txtStoreCode.Text.Trim());
                SetSetting(config, "STORE_EMAIL", txtStoreEmail.Text.Trim());
                SetSetting(config, "DB_SERVER", txtDbServer.Text.Trim());
                SetSetting(config, "DB_NAME", txtDbName.Text.Trim());
                SetSetting(config, "DB_USER", txtDbUser.Text.Trim());
                SetSetting(config, "DB_PASSWORD", txtDbPassword.Text);
                SetSetting(config, "CONNECTION_TIMEOUT", ((int)nud_connectionTimeout.Value).ToString());
                SetSetting(config, "MANAGER_PIN", txtManagerPin.Text.Trim());
                SetSetting(config, "ADMIN_PIN", txtAdminPin.Text.Trim());

                SetSetting(config, "DEFAULT_SELLTYPE_ENABLED", chkDefaultSelltype.Checked.ToString());
                SetSetting(config, "DEFAULT_SELLTYPE_INDEX", cbxDefaultSelltype.SelectedIndex.ToString());

                SetSetting(config, "AUTO_HOLD_IDLE_ENABLED", chkAutoHoldEnabled.Checked.ToString());
                SetSetting(config, "AUTO_HOLD_IDLE_MINUTES", ((int)nud_autoHoldMinutes.Value).ToString());

                SetSetting(config, "BLOCK_ZERO_PRICE_ITEMS", chkBlockZeroPriceItems.Checked.ToString());

                SetSetting(config, "HOLIDAY_GREETING_ENABLED", chkHolidayGreetingEnabled.Checked.ToString());

                SetSetting(config, "SELLTYPE_UI_STYLE", cbxSelltypeUiStyle.SelectedItem as string ?? "Dropdown");

                SetSetting(config, "ALERT_AUTO_DISMISS_ENABLED", chkAlertAutoDismissEnabled.Checked.ToString());
                SetSetting(config, "ALERT_AUTO_DISMISS_SECONDS", ((int)nud_alertAutoDismissSeconds.Value).ToString());

                SetSetting(config, "INVENTORY_PRINT_SCHEDULE_ENABLED", chkInventoryPrintEnabled.Checked.ToString());
                SetSetting(config, "INVENTORY_PRINT_SCHEDULE_FREQUENCY", cbxInventoryPrintFrequency.SelectedItem as string ?? "Weekly");

                SetSetting(config, "PENDING_ITEM_SYNC_ENABLED", chkPendingItemSyncEnabled.Checked.ToString());

                SetSetting(config, "SEARCH_AUTO_CLEAR_ENABLED", chkSearchAutoClearEnabled.Checked.ToString());
                SetSetting(config, "SEARCH_AUTO_CLEAR_SECONDS", ((int)nud_searchAutoClearSeconds.Value).ToString());

                SetSetting(config, "QTY_AUTO_RESET_ENABLED", chkQtyAutoResetEnabled.Checked.ToString());
                SetSetting(config, "QTY_AUTO_RESET_SECONDS", ((int)nud_qtyAutoResetSeconds.Value).ToString());

                var hiddenMenuNames = Purchases.ConfigurableMenuItems
                    .Where((menu, index) => index >= clbMenus.Items.Count || !clbMenus.GetItemChecked(index))
                    .Select(menu => menu.Name);
                SetSetting(config, "HIDDEN_MENUS", string.Join(",", hiddenMenuNames));

                SaveColumnConfig(config, clbItemSearchColumns, Purchases.ItemSearchColumns,
                    "ITEM_SEARCH_HIDDEN_COLUMNS", "ITEM_SEARCH_COLUMN_ORDER");

                SetSetting(config, "ITEM_SEARCH_BACKGROUND_COLOR",
                    chkItemSearchBgCustom.Checked ? System.Drawing.ColorTranslator.ToHtml(pnlItemSearchBgSwatch.BackColor) : "");
                SaveColumnConfig(config, clbCartColumns, Purchases.CartColumns,
                    "CART_HIDDEN_COLUMNS", "CART_COLUMN_ORDER");

                SetSetting(config, "CART_BOARD_BACKGROUND_COLOR",
                    chkCartBgCustom.Checked ? System.Drawing.ColorTranslator.ToHtml(pnlCartBgSwatch.BackColor) : "");

                SetSetting(config, "ITEM_SEARCH_DATE_SORT_DESCENDING", chkItemSearchDateSortDescending.Checked.ToString());
                SetSetting(config, "ITEM_SEARCH_NAME_SORT_DESCENDING", chkItemSearchNameSortDescending.Checked.ToString());

                string selectedPrinter = cbxReceiptPrinter.SelectedItem as string;
                if (selectedPrinter != null)
                    SetSetting(config, "RECEIPT_PRINTER_NAME", selectedPrinter);

                SetSetting(config, "COUNTER_RECEIPT_MODE", chkSaveCounterReceiptAsJpeg.Checked ? "Jpeg" : "Print");
                SetSetting(config, "COUNTER_RECEIPT_JPEG_PATH", txtCounterReceiptPath.Text.Trim());

                if (chkPaperWidthIsDefault.Checked)
                {
                    int paperWidthMm = cbxPaperWidth.SelectedIndex >= 0 ? PaperWidthMmByComboIndex[cbxPaperWidth.SelectedIndex] : 0;
                    SetSetting(config, "RECEIPT_PAPER_WIDTH_MM", paperWidthMm > 0 ? paperWidthMm.ToString() : "");
                }

                SetSetting(config, "ACCENT_BACKGROUND_COLOR",
                    chkEnableAccent.Checked ? System.Drawing.ColorTranslator.ToHtml(pnlBgSwatch.BackColor) : "");
                SetSetting(config, "ACCENT_FOREGROUND_COLOR",
                    chkEnableAccent.Checked ? System.Drawing.ColorTranslator.ToHtml(pnlFgSwatch.BackColor) : "");

                SetSetting(config, "FONT_SCALE_PT", nud_fontScalePt.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));

                SetSetting(config, "LOGO_URL", txtLogoUrl.Text.Trim());

                SetSetting(config, "LAYOUT_EDIT_ENABLED", chkLayoutEditEnabled.Checked.ToString());

                SetSetting(config, "REPORT_SCHEDULE_FREQUENCY", cbxReportFrequency.SelectedItem as string ?? "Weekly");

                SetSetting(config, "TOP_SOLD_ITEM_NAME_CASE", cbxTopSoldNameCase.SelectedItem as string ?? "Title");

                SetSetting(config, "MANAGE_PRICES_ITEM_NAME_CASE", cbxManagePricesNameCase.SelectedItem as string ?? "Title");

                SetSetting(config, "ACCESS_SYNC_PATH", txtAccessDbPath.Text.Trim());

                SetSetting(config, "TRIAL_ENABLED", chkTrialEnabled.Checked.ToString());
                SetSetting(config, "TRIAL_DAYS", ((int)nud_trialDays.Value).ToString());
                SetSetting(config, "TRIAL_AUTO_CLOSE_ENABLED", chkTrialAutoClose.Checked.ToString());

                SetSetting(config, "REMOTE_CONTROL_ENABLED", chkRemoteControlEnabled.Checked.ToString());
                SetSetting(config, "REMOTE_CONTROL_PORT", ((int)nud_remotePort.Value).ToString());
                SetSetting(config, "REMOTE_CONTROL_TOKEN", txtRemoteToken.Text.Trim());
                SetSetting(config, "REMOTE_CONTROL_BLUETOOTH_ENABLED", chkBluetoothEnabled.Checked.ToString());

                SetSetting(config, "MAIL_PROVIDER", rdoProviderGmail.Checked ? "Gmail" : "Smtp");

                SetSetting(config, "SMTP_HOST", txtSmtpHost.Text.Trim());
                SetSetting(config, "SMTP_PORT", ((int)nud_smtpPort.Value).ToString());
                SetSetting(config, "SMTP_USE_SSL", chkSmtpUseSsl.Checked.ToString());
                SetSetting(config, "SMTP_USER", txtSmtpUser.Text.Trim());
                SetSetting(config, "SMTP_PASSWORD", txtSmtpPassword.Text);
                SetSetting(config, "SMTP_FROM", txtSmtpFrom.Text.Trim());

                SetSetting(config, "GMAIL_CLIENT_ID", txtGmailClientId.Text.Trim());
                SetSetting(config, "GMAIL_CLIENT_SECRET", txtGmailClientSecret.Text.Trim());
                SetSetting(config, "GMAIL_REFRESH_TOKEN", txtGmailRefreshToken.Text.Trim());
                SetSetting(config, "GMAIL_SENDER", txtGmailSender.Text.Trim());

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                YourApp.Data.AppTheme.Reload();
                YourApp.Data.AppTheme.ReapplyToOpenForms();
                YourApp.Data.AppFontScale.Reload();
                YourApp.Data.AppFontScale.ReapplyToOpenForms();
                YourApp.Data.CartBoardTheme.Reload();
                YourApp.Data.ItemSearchGridTheme.Reload();
                foreach (var purchases in Application.OpenForms.OfType<Purchases>().ToList())
                {
                    purchases.ApplyCartBoardBackground();
                    purchases.ApplyItemSearchGridBackground();
                }
                UpdateScheduleStatusLabel();
                UpdateTrialStatusLabel();
                RemoteControlServer.Restart();
                UpdateRemoteStatusLabel();
                txtCertThumbprint.Text = RemoteControlServer.CertificateThumbprint;
                UpdateBluetoothRadioInfo();

                string statusMessage = "Saved. Restart the app for database changes to take effect.";
                if (chkSetAsSystemDefaultPrinter.Checked && selectedPrinter != null)
                {
                    if (!SetDefaultPrinter(selectedPrinter))
                        statusMessage = "Saved, but could not set the Windows default printer.";
                }

                try
                {
                    AutoStartService.SetEnabled(chkAutoStartWithWindows.Checked);
                }
                catch (Exception ex)
                {
                    statusMessage = $"Saved, but could not update the Windows startup setting: {ex.Message}";
                }

                ShowStatus(statusMessage, isError: false);
                NotifyTrayOfWarnings();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to save: {ex.Message}", isError: true);
            }
        }

        private void ShowStatus(string text, bool isError)
        {
            lblStatus.ForeColor = isError ? System.Drawing.Color.Tomato : System.Drawing.Color.Teal;
            lblStatus.Text = text;
        }

        private static void SetSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }

        private async void btnSendReportNow_Click(object sender, EventArgs e)
        {
            string recipient = txtStoreEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(recipient))
            {
                ShowStatus("Enter a store email address first.", isError: true);
                return;
            }

            btnSendReportNow.Enabled = false;
            ShowStatus("Sending report...", isError: false);
            try
            {
                await ScheduledReportService.SendNowAsync(recipient);
                ShowStatus($"Report sent to {recipient}.", isError: false);
                UpdateScheduleStatusLabel();
            }
            catch (Exception ex)
            {
                ShowStatus($"Failed to send report: {ex.Message}", isError: true);
            }
            finally
            {
                btnSendReportNow.Enabled = true;
            }
        }

        private void btnBrowseAccessDbPath_Click(object sender, EventArgs e)
        {
            string currentPath = txtAccessDbPath.Text.Trim();
            if (!string.IsNullOrEmpty(currentPath))
            {
                openFileDialogAccess.InitialDirectory = Path.GetDirectoryName(currentPath);
                openFileDialogAccess.FileName = Path.GetFileName(currentPath);
            }

            if (openFileDialogAccess.ShowDialog(this) == DialogResult.OK)
                txtAccessDbPath.Text = openFileDialogAccess.FileName;
        }

        private async void btnSyncNow_Click(object sender, EventArgs e)
        {
            string accessDbPath = txtAccessDbPath.Text.Trim();
            if (string.IsNullOrEmpty(accessDbPath))
            {
                ShowStatus("Enter or browse to an Access file location first.", isError: true);
                return;
            }

            var progress = new Progress<string>(msg => lblSyncStatus.Text = msg);

            btnSyncNow.Enabled = false;
            try
            {
                int rows = await AccessSyncService.SyncAsync(accessDbPath, progress);
                ShowStatus($"Synced {rows} row(s) to {Path.GetFileName(accessDbPath)}.", isError: false);
                UpdateLastSyncLabel();
            }
            catch (Exception ex)
            {
                ShowStatus($"Sync failed: {ex.Message}", isError: true);
                lblSyncStatus.Text = "";
            }
            finally
            {
                btnSyncNow.Enabled = true;
            }
        }

        private async void btnPushToOnline_Click(object sender, EventArgs e)
        {
            string accessDbPath = txtAccessDbPath.Text.Trim();
            if (string.IsNullOrEmpty(accessDbPath))
            {
                ShowStatus("Enter or browse to an Access file location first.", isError: true);
                return;
            }

            var progress = new Progress<string>(msg => lblPushStatus.Text = msg);

            btnPushToOnline.Enabled = false;
            try
            {
                (int updated, int inserted) = await AccessSyncService.PushItemsToOnlineAsync(accessDbPath, progress);
                ShowStatus($"Pushed items to online database: {updated} updated, {inserted} inserted.", isError: false);
                UpdateLastSyncLabel();
            }
            catch (Exception ex)
            {
                ShowStatus($"Push failed: {ex.Message}", isError: true);
                lblPushStatus.Text = "";
            }
            finally
            {
                btnPushToOnline.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void SystemConfigForm_FormClosed(object sender, FormClosedEventArgs e) => StopMonitoring();

        private void btnToggleMonitor_Click(object sender, EventArgs e)
        {
            if (_monitorTimer != null)
                StopMonitoring();
            else
                StartMonitoring();
        }

        private void StartMonitoring()
        {
            chartNetwork.Series["Download"].Points.Clear();
            chartNetwork.Series["Upload"].Points.Clear();
            _sampleIndex = 0;
            _cachedSpeedTestServer = null;

            btnToggleMonitor.Text = "Stop Monitoring";
            lblNetworkStatus.Text = "Starting...";

            _monitorTimer = new System.Windows.Forms.Timer { Interval = MonitorIntervalMs };
            _monitorTimer.Tick += async (s, e) => await RunSpeedSampleAsync();
            _monitorTimer.Start();

            _ = RunSpeedSampleAsync();
        }

        private void StopMonitoring()
        {
            _monitorTimer?.Stop();
            _monitorTimer?.Dispose();
            _monitorTimer = null;

            btnToggleMonitor.Text = "Start Monitoring";
            lblNetworkStatus.Text = "Stopped.";
        }

        /// <summary>
        /// One sample: a real download test via the SpeedTest.Net package (already a project
        /// dependency), plus a hand-rolled upload test - the package only measures download,
        /// so upload throughput is timed by POSTing a random payload to Cloudflare's public
        /// speed-test endpoint (speed.cloudflare.com/__up), which exists specifically to accept
        /// and discard arbitrary bodies for exactly this kind of client-side measurement.
        /// </summary>
        private async Task RunSpeedSampleAsync()
        {
            if (_sampleInFlight)
                return;

            _sampleInFlight = true;
            lblNetworkStatus.Text = "Testing...";

            double downloadMbps = 0;
            double uploadMbps = 0;
            bool downloadOk = true;
            bool uploadOk = true;

            try
            {
                if (_cachedSpeedTestServer == null)
                    _cachedSpeedTestServer = await SpeedTestClient.GetServer();

                DownloadSpeed result = await SpeedTestClient.GetDownloadSpeed(_cachedSpeedTestServer, SpeedTestUnit.MegaBitsPerSecond);
                downloadMbps = result.Speed;
            }
            catch
            {
                downloadOk = false;
            }

            try
            {
                uploadMbps = await MeasureUploadMbpsAsync();
            }
            catch
            {
                uploadOk = false;
            }

            _sampleIndex++;
            if (downloadOk)
                AddSamplePoint(chartNetwork.Series["Download"], downloadMbps);
            if (uploadOk)
                AddSamplePoint(chartNetwork.Series["Upload"], uploadMbps);

            lblNetworkStatus.Text = downloadOk && uploadOk
                ? $"Last: ↓{downloadMbps:0.0} Mbps  ↑{uploadMbps:0.0} Mbps"
                : "Test failed - check your connection.";

            _sampleInFlight = false;
        }

        private void AddSamplePoint(System.Windows.Forms.DataVisualization.Charting.Series series, double value)
        {
            series.Points.AddXY(_sampleIndex, value);
            while (series.Points.Count > MaxChartSamples)
                series.Points.RemoveAt(0);
        }

        private static async Task<double> MeasureUploadMbpsAsync()
        {
            var payload = new byte[4_000_000];
            new Random().NextBytes(payload);

            var stopwatch = Stopwatch.StartNew();
            using (var content = new ByteArrayContent(payload))
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                using (HttpResponseMessage response = await _uploadHttpClient.PostAsync(UploadTestUrl, content))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            stopwatch.Stop();

            double megabits = payload.Length * 8 / 1_000_000.0;
            return megabits / stopwatch.Elapsed.TotalSeconds;
        }
    }
}
