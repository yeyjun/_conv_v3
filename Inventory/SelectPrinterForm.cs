using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
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
                ErrorDialog.Show(this, "Could not save the receipt printer setting.", "Save Failed", ex);
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

        /// <summary>
        /// Offered by Program.cs right after a printer is picked above, so the admin can confirm
        /// it actually prints before relying on it at checkout. Prints the single longest item
        /// description in the catalog - deliberately the worst case for line wrapping/edge
        /// clipping on a narrow thermal roll - rather than a generic test string, using the same
        /// paper size/margins Purchases.ApplyReceiptPrinterSettings gives real receipts.
        /// </summary>
        internal static void PrintTestPage()
        {
            DataTable items = DataCacheManager.Instance.GetTable("Items");
            DataRow longestItem = items?.AsEnumerable()
                .OrderByDescending(r => r["description"]?.ToString()?.Length ?? 0)
                .FirstOrDefault();

            string itemDescription = longestItem != null ? longestItem["description"].ToString() : "(no items in catalog)";
            string itemSerial = longestItem != null ? longestItem["serial"].ToString() : "";

            var document = new PrintDocument();
            Purchases.ApplyReceiptPrinterSettings(document);

            document.PrintPage += (s, e) =>
            {
                int startX = 5;
                int pageWidth = e.MarginBounds.Width > 0 ? e.MarginBounds.Width : e.PageBounds.Width;
                float maxWidth = pageWidth - 2 * startX;
                float y = e.MarginBounds.Top;

                Font fontHeader = new Font("Arial", 9.5f, FontStyle.Bold);
                Font fontBody = new Font("Arial", 8f);

                void DrawCentered(string text, Font font, float yPos)
                {
                    float textWidth = e.Graphics.MeasureString(text, font).Width;
                    float x = Math.Max(startX, (pageWidth - textWidth) / 2f);
                    e.Graphics.DrawString(text, font, Brushes.Black, x, yPos);
                }

                string storeName = ConfigurationManager.AppSettings["STORE_NAME"] ?? "Baclig Minimart";
                DrawCentered(storeName, fontHeader, y);
                y += fontHeader.GetHeight() + 4;
                DrawCentered("PRINTER TEST PAGE", fontHeader, y);
                y += fontHeader.GetHeight() + 10;

                e.Graphics.DrawLine(Pens.Black, startX, y, startX + maxWidth, y);
                y += 6;

                e.Graphics.DrawString("Longest catalog item (worst case for wrapping):", fontBody, Brushes.Black, startX, y);
                y += fontBody.GetHeight() + 2;

                foreach (string line in WrapText(itemDescription, fontBody, maxWidth, e.Graphics))
                {
                    e.Graphics.DrawString(line, fontBody, Brushes.Black, startX, y);
                    y += fontBody.GetHeight() + 1;
                }

                if (!string.IsNullOrEmpty(itemSerial))
                {
                    y += 2;
                    e.Graphics.DrawString("Serial: " + itemSerial, fontBody, Brushes.Black, startX, y);
                    y += fontBody.GetHeight() + 2;
                }

                y += 8;
                e.Graphics.DrawLine(Pens.Black, startX, y, startX + maxWidth, y);
                y += 6;

                DrawCentered($"Printer: {document.PrinterSettings.PrinterName}", fontBody, y);
                y += fontBody.GetHeight() + 2;
                DrawCentered(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontBody, y);
            };

            document.Print();
        }

        private static List<string> WrapText(string text, Font font, float maxWidth, Graphics g)
        {
            var lines = new List<string>();
            var words = (text ?? "").Split(' ');
            var current = new StringBuilder();

            foreach (string word in words)
            {
                string candidate = current.Length == 0 ? word : current + " " + word;
                if (current.Length > 0 && g.MeasureString(candidate, font).Width > maxWidth)
                {
                    lines.Add(current.ToString());
                    current.Clear();
                    current.Append(word);
                }
                else
                {
                    current.Clear();
                    current.Append(candidate);
                }
            }
            lines.Add(current.ToString());
            return lines;
        }
    }
}
