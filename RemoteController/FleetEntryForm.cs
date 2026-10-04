using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace RemoteController
{
    /// <summary>Add/edit dialog for one FleetStore entry.</summary>
    public partial class FleetEntryForm : Form
    {
        public FleetEntry Entry { get; private set; }

        public FleetEntryForm(FleetEntry existing = null)
        {
            InitializeComponent();

            if (existing == null)
                return;

            txtStoreName.Text = existing.StoreName;
            txtHost.Text = existing.Host;
            nud_port.Value = Math.Min(Math.Max(existing.Port, nud_port.Minimum), nud_port.Maximum);
            txtToken.Text = existing.Token;
            txtLatitude.Text = existing.Latitude.HasValue ? existing.Latitude.Value.ToString(CultureInfo.InvariantCulture) : "";
            txtLongitude.Text = existing.Longitude.HasValue ? existing.Longitude.Value.ToString(CultureInfo.InvariantCulture) : "";
        }

        private void chkShowToken_CheckedChanged(object sender, EventArgs e) =>
            txtToken.UseSystemPasswordChar = !chkShowToken.Checked;

        /// <summary>Best-effort IP geolocation via a free, no-key lookup service - city-level
        /// accuracy at best, useless for a private/LAN address, and not meant to be authoritative;
        /// it just saves typing when the coordinates are roughly known already.</summary>
        private async void btnDetectLocation_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            if (string.IsNullOrWhiteSpace(host))
            {
                lblDetectStatus.ForeColor = Color.Tomato;
                lblDetectStatus.Text = "Enter a host/IP first.";
                return;
            }

            btnDetectLocation.Enabled = false;
            lblDetectStatus.ForeColor = Color.Gray;
            lblDetectStatus.Text = "Looking up...";

            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) })
                {
                    string json = await client.GetStringAsync("http://ip-api.com/json/" + Uri.EscapeDataString(host));
                    var result = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);

                    bool ok = result.TryGetValue("status", out object status) && (string)status == "success" &&
                        result.TryGetValue("lat", out object lat) && result.TryGetValue("lon", out object lon);

                    if (ok)
                    {
                        txtLatitude.Text = Convert.ToDouble(result["lat"]).ToString(CultureInfo.InvariantCulture);
                        txtLongitude.Text = Convert.ToDouble(result["lon"]).ToString(CultureInfo.InvariantCulture);
                        string city = result.TryGetValue("city", out object c) ? c as string : null;

                        lblDetectStatus.ForeColor = Color.SeaGreen;
                        lblDetectStatus.Text = "Approximate location found" +
                            (string.IsNullOrEmpty(city) ? "" : $" ({city})") + " - verify/adjust before saving.";
                    }
                    else
                    {
                        lblDetectStatus.ForeColor = Color.Tomato;
                        lblDetectStatus.Text = "Could not determine a location for that address (private/LAN IP, or the lookup failed).";
                    }
                }
            }
            catch (Exception ex)
            {
                lblDetectStatus.ForeColor = Color.Tomato;
                lblDetectStatus.Text = $"Lookup failed: {ex.Message}";
            }
            finally
            {
                btnDetectLocation.Enabled = true;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStoreName.Text))
            {
                MessageBox.Show(this, "Enter a store name.", "Store Name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHost.Text))
            {
                MessageBox.Show(this, "Enter a host/IP.", "Host Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryParseCoordinate(txtLatitude.Text, out double? lat) || !TryParseCoordinate(txtLongitude.Text, out double? lon))
            {
                MessageBox.Show(this, "Latitude/Longitude must be numbers (or left blank).", "Invalid Coordinates",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Entry = new FleetEntry
            {
                StoreName = txtStoreName.Text.Trim(),
                Host = txtHost.Text.Trim(),
                Port = (int)nud_port.Value,
                Token = txtToken.Text,
                Latitude = lat,
                Longitude = lon
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool TryParseCoordinate(string text, out double? value)
        {
            text = (text ?? "").Trim();
            if (text.Length == 0)
            {
                value = null;
                return true;
            }

            bool ok = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed);
            value = ok ? (double?)parsed : null;
            return ok;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
