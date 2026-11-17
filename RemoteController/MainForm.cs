using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RemoteController
{
    public partial class MainForm : Form
    {
        private List<FleetEntry> fleet = new List<FleetEntry>();

        // Expiry actually baked into the most recently generated key (as opposed to whatever the
        // duration controls happen to show right now) - Save File uses this so a key generated
        // right before the user tweaks the duration controls still gets saved with the expiry it
        // was actually generated with.
        private DateTime? generatedExpiryUtc;

        private int? generatedMonthsOfUse;
        private bool hasGeneratedKey;

        public MainForm()
        {
            InitializeComponent();
            tabControlMain.SelectedIndex = 0;
            txtHost.Text = "localhost";
            cmbDurationUnit.SelectedIndex = 2; // Years
            cmbSubscriptionTier.SelectedIndex = 0;
            LoadFleet();
        }

        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            string storeCode = txtStoreCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(storeCode))
            {
                MessageBox.Show(this, "Enter the store code the key is for.", "Store Code Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            generatedExpiryUtc = ComputeExpiryFromDurationControls();
            generatedMonthsOfUse = ComputeMonthsFromDurationControls();
            hasGeneratedKey = true;
            txtGeneratedKey.Text = LicenseKeyGenerator.ComputeKey(storeCode, generatedExpiryUtc);
            lblExpiryPreview.Text = generatedExpiryUtc.HasValue
                ? $"Expires: {generatedExpiryUtc.Value:yyyy-MM-dd} (UTC)"
                : "Expires: never (permanent license)";
        }

        /// <summary>Turns the Validity row (amount + Days/Months/Years, or the "no expiry"
        /// checkbox) into an absolute UTC expiry date - null means permanent.</summary>
        private DateTime? ComputeExpiryFromDurationControls()
        {
            if (chkPermanent.Checked)
                return null;

            int amount = (int)nudDuration.Value;
            DateTime start = DateTime.UtcNow.Date;
            switch (cmbDurationUnit.SelectedItem as string)
            {
                case "Days": return start.AddDays(amount);
                case "Months": return start.AddMonths(amount);
                default: return start.AddYears(amount);
            }
        }

        private int? ComputeMonthsFromDurationControls()
        {
            if (chkPermanent.Checked)
                return null;

            int amount = (int)nudDuration.Value;
            switch (cmbDurationUnit.SelectedItem as string)
            {
                case "Days": return Math.Max(1, (int)Math.Round(amount / 30.0));
                case "Years": return amount * 12;
                default: return amount;
            }
        }

        private void chkPermanent_CheckedChanged(object sender, EventArgs e)
        {
            nudDuration.Enabled = !chkPermanent.Checked;
            cmbDurationUnit.Enabled = !chkPermanent.Checked;
        }

        private void btnCopyKey_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGeneratedKey.Text))
                return;

            Clipboard.SetText(txtGeneratedKey.Text);
        }

        private void btnSaveLicenseFile_Click(object sender, EventArgs e)
        {
            string storeCode = txtStoreCode.Text.Trim();
            if (string.IsNullOrEmpty(txtGeneratedKey.Text) || !hasGeneratedKey)
            {
                MessageBox.Show(this, "Generate a key first.", "Nothing to Save",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tier = cmbSubscriptionTier.SelectedItem as string ?? "Basic";

            string tierDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "RemoteController", "Licenses", tier);
            try { Directory.CreateDirectory(tierDir); } catch { }

            saveFileDialogLicense.InitialDirectory = tierDir;
            saveFileDialogLicense.FileName = storeCode + ".tmrgo";
            if (saveFileDialogLicense.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                LicenseKeyGenerator.SaveLicenseFile(saveFileDialogLicense.FileName, storeCode, txtGeneratedKey.Text,
                    generatedExpiryUtc, tier, generatedMonthsOfUse);
                Log($"Saved license file: {saveFileDialogLicense.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to save: {ex.Message}", "Save Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkShowToken_CheckedChanged(object sender, EventArgs e) =>
            txtToken.UseSystemPasswordChar = !chkShowToken.Checked;

        private void radTransport_CheckedChanged(object sender, EventArgs e)
        {
            bool bluetooth = radBluetooth.Checked;

            lblHost.Visible = !bluetooth;
            txtHost.Visible = !bluetooth;
            lblPort.Visible = !bluetooth;
            nud_port.Visible = !bluetooth;
            lblCertThumbprint.Visible = !bluetooth;
            txtCertThumbprint.Visible = !bluetooth;

            lblBtDevice.Visible = bluetooth;
            cmbBtDevice.Visible = bluetooth;
            btnRefreshDevices.Visible = bluetooth;

            if (bluetooth && cmbBtDevice.Items.Count == 0)
                RefreshPairedDevices();
        }

        private void btnRefreshDevices_Click(object sender, EventArgs e) => RefreshPairedDevices();

        private void RefreshPairedDevices()
        {
            cmbBtDevice.Items.Clear();
            try
            {
                foreach ((string name, string address) in BluetoothControlClient.GetPairedDevices())
                    cmbBtDevice.Items.Add(new PairedDeviceItem(name, address));

                if (cmbBtDevice.Items.Count > 0)
                    cmbBtDevice.SelectedIndex = 0;

                Log(cmbBtDevice.Items.Count == 0
                    ? "No paired Bluetooth devices found - pair the terminal first in Windows Settings > Bluetooth."
                    : $"Found {cmbBtDevice.Items.Count} paired device(s).");
            }
            catch (Exception ex)
            {
                Log($"Could not list paired devices: {ex.Message}");
            }
        }

        private sealed class PairedDeviceItem
        {
            public string Address { get; }
            private readonly string _name;

            public PairedDeviceItem(string name, string address)
            {
                _name = name;
                Address = address;
            }

            public override string ToString() => $"{_name} ({Address})";
        }

        private void btnPing_Click(object sender, EventArgs e) => RunCommand("PING", null);

        private void btnGetStatus_Click(object sender, EventArgs e) => RunCommand("STATUS", null);

        private void btnActivateRemote_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGeneratedKey.Text))
            {
                MessageBox.Show(this, "Generate (or enter) a license key first.", "Nothing to Send",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RunCommand("ACTIVATE", txtGeneratedKey.Text);
        }

        private void btnManageMenus_Click(object sender, EventArgs e)
        {
            if (!TryPrepareSend(out string errorMessage))
            {
                MessageBox.Show(this, errorMessage, "Cannot Connect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var menuForm = new MenuControlForm(SendCommand))
                menuForm.ShowDialog(this);
        }

        private void RunCommand(string command, string arg)
        {
            if (!TryPrepareSend(out string errorMessage))
            {
                MessageBox.Show(this, errorMessage, "Cannot Connect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log($"-> {command}{(arg == null ? "" : " " + arg)} via {(radBluetooth.Checked ? "Bluetooth" : "Network")}");
            try
            {
                string response = SendCommand(command, arg);
                Log($"<- {response}");
            }
            catch (Exception ex)
            {
                Log($"<- ERROR: {ex.Message}");
            }
        }

        /// <summary>Validates the fields the currently selected transport needs, without sending
        /// anything yet - shared by the toolbar buttons and the Manage Menus dialog.</summary>
        private bool TryPrepareSend(out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(txtToken.Text))
            {
                errorMessage = "Enter the shared token configured on the terminal (System Config > Remote Control).";
                return false;
            }

            if (radBluetooth.Checked)
            {
                if (!(cmbBtDevice.SelectedItem is PairedDeviceItem))
                {
                    errorMessage = "Choose a paired Bluetooth device first.";
                    return false;
                }
            }
            else if (string.IsNullOrWhiteSpace(txtHost.Text))
            {
                errorMessage = "Enter the terminal's host name or IP address.";
                return false;
            }

            return true;
        }

        /// <summary>Sends one command over whichever transport is currently selected. Passed as a
        /// delegate into MenuControlForm so it doesn't need to know Network from Bluetooth.</summary>
        private string SendCommand(string command, string arg)
        {
            string token = txtToken.Text;

            if (radBluetooth.Checked)
            {
                string address = ((PairedDeviceItem)cmbBtDevice.SelectedItem).Address;
                return BluetoothControlClient.SendCommand(address, token, command, arg);
            }

            string host = txtHost.Text.Trim();
            int port = (int)nud_port.Value;
            string thumbprint = txtCertThumbprint.Text.Trim();
            return RemoteControlClient.SendCommand(host, port, token, command, arg, thumbprint);
        }

        private void Log(string line) =>
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}");

        // ----------------------------------------------------------------------------------
        // Fleet tab
        // ----------------------------------------------------------------------------------

        private void LoadFleet()
        {
            fleet = FleetStore.Load();
            RepopulateFleetGrid();
        }

        private void RepopulateFleetGrid()
        {
            dgvFleet.Rows.Clear();
            foreach (FleetEntry entry in fleet)
            {
                int rowIndex = dgvFleet.Rows.Add(
                    entry.StoreName,
                    entry.Host,
                    entry.Port,
                    entry.Latitude.HasValue ? entry.Latitude.Value.ToString("0.0000") : "",
                    entry.Longitude.HasValue ? entry.Longitude.Value.ToString("0.0000") : "",
                    entry.Status);
                dgvFleet.Rows[rowIndex].Tag = entry;
            }
        }

        private FleetEntry GetSelectedFleetEntry() =>
            dgvFleet.SelectedRows.Count > 0 ? (FleetEntry)dgvFleet.SelectedRows[0].Tag : null;

        private void btnAddFleet_Click(object sender, EventArgs e)
        {
            using (var form = new FleetEntryForm())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                fleet.Add(form.Entry);
                FleetStore.Save(fleet);
                RepopulateFleetGrid();
            }
        }

        private void btnEditFleet_Click(object sender, EventArgs e)
        {
            FleetEntry selected = GetSelectedFleetEntry();
            if (selected == null)
            {
                MessageBox.Show(this, "Select an entry to edit first.", "Nothing Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new FleetEntryForm(selected))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                int index = fleet.IndexOf(selected);
                fleet[index] = form.Entry;
                FleetStore.Save(fleet);
                RepopulateFleetGrid();
            }
        }

        private void dgvFleet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnEditFleet_Click(sender, e);
        }

        private void btnRemoveFleet_Click(object sender, EventArgs e)
        {
            FleetEntry selected = GetSelectedFleetEntry();
            if (selected == null)
            {
                MessageBox.Show(this, "Select an entry to remove first.", "Nothing Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ConfirmDialog.Show(this, $"Remove \"{selected.StoreName}\" from the fleet list? (Y/N)", "Confirm Remove",
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            fleet.Remove(selected);
            FleetStore.Save(fleet);
            RepopulateFleetGrid();
        }

        private async void btnRefreshAllFleet_Click(object sender, EventArgs e)
        {
            if (fleet.Count == 0)
            {
                lblFleetStatus.Text = "Nothing to check - add an entry first.";
                return;
            }

            btnRefreshAllFleet.Enabled = false;
            lblFleetStatus.ForeColor = System.Drawing.Color.Gray;
            lblFleetStatus.Text = $"Checking {fleet.Count} entr{(fleet.Count == 1 ? "y" : "ies")}...";

            foreach (FleetEntry entry in fleet)
                entry.Status = await FleetStore.CheckStatusAsync(entry);

            RepopulateFleetGrid();
            lblFleetStatus.Text = $"Last checked: {DateTime.Now:g}";
            btnRefreshAllFleet.Enabled = true;
        }

        private void btnUseSelected_Click(object sender, EventArgs e)
        {
            FleetEntry selected = GetSelectedFleetEntry();
            if (selected == null)
            {
                MessageBox.Show(this, "Select an entry first.", "Nothing Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            radNetwork.Checked = true;
            txtHost.Text = selected.Host;
            nud_port.Value = Math.Min(Math.Max(selected.Port, nud_port.Minimum), nud_port.Maximum);
            txtToken.Text = selected.Token;
            txtCertThumbprint.Text = selected.CertThumbprint;

            tabControlMain.SelectedTab = tabPageRemote;
        }
    }
}
