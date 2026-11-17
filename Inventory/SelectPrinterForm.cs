using System;
using System.Configuration;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace _conv
{
    /// <summary>
    /// Shown right after Program.cs's startup "no receipt printer configured" warning so the
    /// admin/cashier can pick one immediately instead of having to find System Config > Printer
    /// (which is gated behind the admin PIN) on their own. Lists every printer installed on this
    /// machine, with whichever one looks most like a POS/thermal receipt printer - by name - moved
    /// to the top and preselected as the recommendation.
    /// </summary>
    public partial class SelectPrinterForm : Form
    {
        // Substrings (case-insensitive) commonly found in POS/thermal receipt printer driver
        // names - e.g. this app's own default "POS80" - used to guess which installed printer is
        // the receipt printer rather than a general-purpose/virtual one (PDF, Fax, OneNote, ...).
        private static readonly string[] RecommendedNameHints =
        {
            "POS", "RECEIPT", "THERM", "TM-", "TSP", "BIXOLON", "ZEBRA", "SEWOO", "RONGTA", "XPRINTER", "GOOJPRT"
        };

        public string SelectedPrinterName { get; private set; }

        public SelectPrinterForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
            LoadPrinters();
        }

        private void LoadPrinters()
        {
            var installed = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

            if (installed.Count == 0)
            {
                lblMessage.Text = "No printers are installed on this machine. Install/connect your receipt printer, then set it from System Config > Printer.";
                lstPrinters.Enabled = false;
                btnSetPrinter.Enabled = false;
                return;
            }

            string recommended = installed.FirstOrDefault(p =>
                RecommendedNameHints.Any(hint => p.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0));

            if (recommended == null)
            {
                // No obviously-thermal name found - fall back to whatever Windows already treats
                // as the default printer, still just a suggestion the admin can change.
                string systemDefault = new PrinterSettings().PrinterName;
                if (installed.Contains(systemDefault))
                    recommended = systemDefault;
            }

            lstPrinters.Items.Clear();
            if (recommended != null)
            {
                lstPrinters.Items.Add(recommended + "  (Recommended)");
                foreach (string printer in installed.Where(p => p != recommended))
                    lstPrinters.Items.Add(printer);
            }
            else
            {
                foreach (string printer in installed)
                    lstPrinters.Items.Add(printer);
            }

            lstPrinters.SelectedIndex = 0;
        }

        private void btnSetPrinter_Click(object sender, EventArgs e)
        {
            if (lstPrinters.SelectedItem == null)
                return;

            string chosen = lstPrinters.SelectedItem.ToString();
            const string recommendedSuffix = "  (Recommended)";
            if (chosen.EndsWith(recommendedSuffix, StringComparison.Ordinal))
                chosen = chosen.Substring(0, chosen.Length - recommendedSuffix.Length);

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["RECEIPT_PRINTER_NAME"] == null)
                    config.AppSettings.Settings.Add("RECEIPT_PRINTER_NAME", chosen);
                else
                    config.AppSettings.Settings["RECEIPT_PRINTER_NAME"].Value = chosen;

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save the receipt printer setting:\n{ex.Message}",
                    "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedPrinterName = chosen;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnNotNow_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
