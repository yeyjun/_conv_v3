using System;
using System.Drawing;
using System.Windows.Forms;

namespace _conv
{
    /// <summary>
    /// Generic read-only text viewer with a Print button, reused for anything under the Help
    /// menu that's just a static reference sheet (keyboard shortcuts, technical requirements) -
    /// shows it in a scrollable box that can be handed to a new cashier as a printed page
    /// instead of only ever being read on screen.
    /// </summary>
    public partial class HelpForm : Form
    {
        private readonly string bodyText;
        private string printRemainingText;

        public HelpForm(string title, string bodyText)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            Text = title;
            this.bodyText = bodyText;
            rtxtShortcuts.Text = bodyText;
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // No printer-picker dialog - every printable in the app goes straight to the
            // configured printer (System Config > Printer), same as receipts and the
            // out-of-stock report, instead of letting the cashier redirect it elsewhere.
            Purchases.ApplyReceiptPrinterSettings(printDocumentHelp);

            try
            {
                printDocumentHelp.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to print: {ex.Message}", "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocumentHelp_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e) =>
            printRemainingText = bodyText;

        // Standard .NET multi-page plain-text print recipe: measure how much of the remaining
        // text fits in this page's margin bounds, draw exactly that much, then carry the rest
        // over to the next PrintPage call via HasMorePages.
        private void printDocumentHelp_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Font font = new Font("Consolas", 10);

            e.Graphics.MeasureString(
                printRemainingText, font, e.MarginBounds.Size, StringFormat.GenericDefault,
                out int charsFitted, out int _);

            e.Graphics.DrawString(
                printRemainingText.Substring(0, charsFitted), font, Brushes.Black,
                e.MarginBounds, StringFormat.GenericDefault);

            printRemainingText = printRemainingText.Substring(charsFitted);
            e.HasMorePages = printRemainingText.Length > 0;
        }
    }
}
