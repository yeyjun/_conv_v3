using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
{
    public partial class GenerateBarcode : Form
    {
        public GenerateBarcode()
        {
            InitializeComponent();
            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);
        }
        public string code = "";

        private static readonly string[] BarcodeTypes = { "code128", "code39", "ean13", "upc", "qrcode" };

        private void GenerateBarcode_Load(object sender, EventArgs e)
        {
            printDocumentBarcode.PrintPage += printDocumentBarcode_PrintPage;

            barcoder.SizeMode = PictureBoxSizeMode.StretchImage;

            cmb_barcodeType.Items.AddRange(BarcodeTypes);
            cmb_barcodeType.SelectedItem = "code128";

            tbx_itemLookup.Text = code;
            ShowDescriptionFor(code);

            RefreshBarcode();
        }

        private void tbx_itemLookup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                FindItem();
            }
        }

        private void btn_findItem_Click(object sender, EventArgs e)
        {
            FindItem();
        }

        private void btn_generate_Click(object sender, EventArgs e)
        {
            RefreshBarcode();
        }

        /// <summary>
        /// Resolves whatever was typed or scanned into tbx_itemLookup against the same
        /// offline-capable item cache the rest of the app searches (DataCacheManager),
        /// preferring an exact serial match before falling back to a description search.
        /// </summary>
        private void FindItem()
        {
            string query = tbx_itemLookup.Text.Trim();
            if (string.IsNullOrEmpty(query))
                return;

            var items = DataCacheManager.Instance.GetTable("Items");
            var row = items.AsEnumerable().FirstOrDefault(r => r.Field<string>("serial") == query)
                ?? items.AsEnumerable().FirstOrDefault(r =>
                    r["description"].ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);

            if (row == null)
            {
                lbl_itemDescription.Text = "Item not found.";
                return;
            }

            tbx_itemLookup.Text = row.Field<string>("serial");
            lbl_itemDescription.Text = row["description"].ToString();
            RefreshBarcode();
        }

        private void ShowDescriptionFor(string serial)
        {
            var row = DataCacheManager.Instance.GetTable("Items").AsEnumerable()
                .FirstOrDefault(r => r.Field<string>("serial") == serial);

            lbl_itemDescription.Text = row != null ? row["description"].ToString() : "";
        }

        private void RefreshBarcode()
        {
            string value = tbx_itemLookup.Text.Trim();
            if (string.IsNullOrEmpty(value))
                return;

            string type = cmb_barcodeType.SelectedItem?.ToString() ?? "code128";
            int width = (int)num_width.Value;
            int height = (int)num_height.Value;
            bool includeText = chk_includeText.Checked;

            barcoder.ImageLocation = "https://quickchart.io/barcode?" +
                $"type={Uri.EscapeDataString(type)}" +
                $"&width={width}" +
                $"&height={height}" +
                $"&includeText={includeText.ToString().ToLowerInvariant()}" +
                $"&text={Uri.EscapeDataString(value)}";
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            // Anchors this label print on the same centrally configured printer (System Config >
            // Printer) as every other printable in the app, rather than falling back to whatever
            // Windows considers the default printer.
            Purchases.ApplyReceiptPrinterSettings(printDocumentBarcode);

            printPreviewDialogBarcode.Document = printDocumentBarcode;
            printPreviewDialogBarcode.Width = 500;
            printPreviewDialogBarcode.Height = 500;
            printPreviewDialogBarcode.Document.PrintController = new System.Drawing.Printing.StandardPrintController();

            // AutoZoom fits the rendered page to the preview window instead of a fixed 1.5x
            // that had no relation to the actual label size (and, combined with the old 300x90
            // window, made the preview effectively unreadable) - so whatever's on the label
            // shows at a size that actually matches what will print.
            printPreviewDialogBarcode.PrintPreviewControl.AutoZoom = true;
            printPreviewDialogBarcode.ShowDialog();
        }

        private void printDocumentBarcode_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(barcoder.Image, new Point(0, 0));
        }
    }
}
