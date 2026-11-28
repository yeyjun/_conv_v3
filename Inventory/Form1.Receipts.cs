using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventory
{
    // Receipt printing/rendering: the customer/counter receipt PrintDocument, the on-screen
    // preview renderer shared with ReceiptPreviewForm, the counter-copy JPEG export (with
    // print-shop bleed/crop marks), and the Dashboard's out-of-stock report printout. Split out
    // of Form1.cs (still the same Purchases class - just this file's slice of it) because this
    // was the single largest concern tangled into that file.
    public partial class Purchases
    {
        private string pendingReceiptText;
        private string pendingReceiptCode;

        // Every receipt prints one of these, in order, as separate pages of the same print job.
        private static readonly string[] ReceiptCopyLabels = { "CUSTOMER'S COPY", "COUNTER'S COPY" };
        private int receiptCopyIndex;

        // True while printing just one receipt copy instead of looping through every
        // ReceiptCopyLabels entry - used when the counter's copy is being rendered to a JPEG
        // instead of the printer, so only the customer's copy goes to physical paper.
        private bool printSingleReceiptCopyOnly;

        // reset to 0 before each print job, advanced across pages via HasMorePages.
        private int outOfStockPrintRowIndex;

        // Set from System Config > Receipt Testing's slider, live (not persisted to App.config -
        // this is a per-session testing knob, not a store setting). When set, every item on the
        // printed/previewed receipt shows this unit price instead of its real one (line total and
        // the subtotal/tax/grand total below it are recomputed from it too, so the whole receipt
        // stays internally consistent) - lets a tester see how the layout holds up with very small
        // or very large price values without editing real item data.
        internal static decimal? TestPriceOverride;

        // content height - captured so SaveCounterReceiptAsJpeg can crop its (generously tall,
        // since receipt length varies with item count) scratch bitmap down to just the receipt.
        private float lastReceiptContentHeight;

        /// <summary>
        /// Points the given PrintDocument at the thermal printer configured in System Config >
        /// Printer (RECEIPT_PRINTER_NAME) and pulls its paper size straight from that printer's
        /// own Windows driver settings, so the preview - and the eventual print - are sized for
        /// the actual POS80/58mm roll instead of whatever page size a different default printer
        /// left behind. Falls back to leaving the paper size alone (system default printer/paper)
        /// if nothing is configured yet or the configured printer isn't installed right now.
        /// If System Config > Printer has a paper width override (RECEIPT_PAPER_WIDTH_MM) set,
        /// that replaces whatever width was just detected - lets the cashier correct a driver
        /// that reports the wrong roll width instead of being stuck with it.
        /// (the out-of-stock report), and other forms' print jobs (e.g. HelpForm, GenerateBarcode)
        /// - so there is exactly one place a printer/paper choice is made instead of each print
        /// path drifting to whatever the Windows default printer happens to be. Internal (not
        /// private) so other forms in the assembly can anchor their PrintDocuments on it too.
        /// </summary>
        /// <summary>
        /// True when RECEIPT_PRINTER_NAME is set and currently installed on this machine. Shared
        /// by ApplyReceiptPrinterSettings (decides whether to pull paper settings from that
        /// printer's driver) and Program.cs's startup check (warns the admin to set one up in
        /// System Config > Printer if it's missing).
        /// </summary>
        internal static bool IsReceiptPrinterConfigured()
        {
            string configuredPrinter = ConfigurationManager.AppSettings["RECEIPT_PRINTER_NAME"];
            return !string.IsNullOrWhiteSpace(configuredPrinter) &&
                System.Drawing.Printing.PrinterSettings.InstalledPrinters
                    .Cast<string>()
                    .Any(p => string.Equals(p, configuredPrinter, StringComparison.OrdinalIgnoreCase));
        }

        internal static void ApplyReceiptPrinterSettings(PrintDocument document)
        {
            // A small fixed floor - keeps the page-width-driven layouts that use this actually
            // using the paper's real printable area, even before a real printer is selected
            // below (or if the configured one isn't installed right now).
            int marginX = 2;
            int marginY = 2;

            string configuredPrinter = ConfigurationManager.AppSettings["RECEIPT_PRINTER_NAME"];
            bool isInstalled = IsReceiptPrinterConfigured();
            bool appliedConfiguredPrinter = false;

            if (isInstalled)
            {
                try
                {
                    document.PrinterSettings.PrinterName = configuredPrinter;
                    document.DefaultPageSettings.PaperSize = document.PrinterSettings.DefaultPageSettings.PaperSize;

                    // POS80/58mm thermal drivers report a hardware-enforced unprintable border via
                    // HardMarginX/Y - e.g. the print head physically can't reach the outer ~2-4mm of
                    // an 80mm roll even though the roll (and PaperSize.Width) is nominally 80mm.
                    // MarginBounds (and the pageWidth/rightEdge derived from it below) has no idea
                    // about that border, so a margin smaller than it lets right-aligned text and
                    // dividers get laid out past what the printer can actually mark - GDI then just
                    // clips them, which reads as text "running off the edge" on real hardware even
                    // though it looked fine in a plain preview. Flooring the margin at the hard
                    // margin keeps the usable width honest for whichever printer is selected.
                    marginX = (int)Math.Max(marginX, Math.Ceiling(document.DefaultPageSettings.HardMarginX));
                    marginY = (int)Math.Max(marginY, Math.Ceiling(document.DefaultPageSettings.HardMarginY));
                    appliedConfiguredPrinter = true;
                }
                catch (InvalidPrinterException)
                {
                    // Listed in InstalledPrinters (Windows still has a queue for it) but not
                    // actually reachable right now - e.g. a disconnected/offline network or USB
                    // thermal printer, or a redirected printer from a remote session that isn't
                    // live. Accessing PrinterSettings/DefaultPageSettings throws in that case
                    // instead of degrading gracefully, which used to crash both real checkout
                    // printing and the receipt preview. Falls through to the system-default
                    // printer fallback below instead, same as if the printer weren't configured.
                }
            }

            if (!appliedConfiguredPrinter)
            {
                // No receipt printer configured, or configured but unreachable right now - name
                // the actual Windows default printer explicitly instead of leaving PrinterSettings
                // untouched/blank. A PrintDocument with no printer resolved at all falls back to a
                // hardcoded Letter-size page with no real driver behind it, which doesn't reflect
                // what would actually print on this machine - this keeps the preview/print always
                // tracking whatever paper size the system default printer is actually set to.
                string defaultPrinterName = new PrinterSettings().PrinterName;
                if (!string.IsNullOrWhiteSpace(defaultPrinterName))
                {
                    try
                    {
                        document.PrinterSettings.PrinterName = defaultPrinterName;
                        document.DefaultPageSettings.PaperSize = document.PrinterSettings.DefaultPageSettings.PaperSize;
                        marginX = (int)Math.Max(marginX, Math.Ceiling(document.DefaultPageSettings.HardMarginX));
                        marginY = (int)Math.Max(marginY, Math.Ceiling(document.DefaultPageSettings.HardMarginY));
                    }
                    catch (InvalidPrinterException)
                    {
                        // No usable printer at all right now (e.g. the "default" printer is also
                        // offline) - keep the small fixed-floor margins/paper size set above.
                    }
                }
            }

            if (double.TryParse(ConfigurationManager.AppSettings["RECEIPT_PAPER_WIDTH_MM"], out double widthMm) && widthMm > 0)
            {
                PaperSize current = document.DefaultPageSettings.PaperSize;
                int widthHundredthsOfInch = (int)Math.Round(widthMm / 25.4 * 100);
                document.DefaultPageSettings.PaperSize = new PaperSize(current.PaperName, widthHundredthsOfInch, current.Height);
            }

            // RECEIPT_MARGIN_LEFT/RIGHT/TOP_MM are saved from System Config > Receipt Testing's
            // preview window (drag a boundary handle, then Save) - each is independently optional,
            // so an admin can pin just the side(s) they actually adjusted and leave the rest on
            // the printer-driver/hard-margin default computed above. Still floored at that default
            // rather than letting a saved value go below it, for the same "don't clip on real
            // hardware" reason the hard-margin flooring above exists.
            int marginLeft = ReadMarginOverrideHundredths("RECEIPT_MARGIN_LEFT_MM") is int left ? Math.Max(marginX, left) : marginX;
            int marginRight = ReadMarginOverrideHundredths("RECEIPT_MARGIN_RIGHT_MM") is int right ? Math.Max(marginX, right) : marginX;
            int marginTop = ReadMarginOverrideHundredths("RECEIPT_MARGIN_TOP_MM") is int top ? Math.Max(marginY, top) : marginY;

            document.DefaultPageSettings.Margins = new Margins(marginLeft, marginRight, marginTop, marginY);
        }

        private static int? ReadMarginOverrideHundredths(string appSettingKey) =>
            double.TryParse(ConfigurationManager.AppSettings[appSettingKey], out double mm) && mm > 0
                ? (int)Math.Round(mm / 25.4 * 100)
                : (int?)null;

        // Inverse of ReadMarginOverrideHundredths's mm->hundredths conversion - used by
        // ReceiptPreviewForm's Save button to write a dragged margin (in preview pixels, already
        // converted to hundredths of an inch) back out as the human-readable mm App.config expects.
        internal static double HundredthsToMm(int hundredths) => hundredths / 100.0 * 25.4;

        private void PrintDocumentReceipt_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                // A handful of reused weights/sizes instead of one-off Fonts scattered through the
                // layout - keeps the receipt's type hierarchy (store name > section labels > body >
                // fine print) consistent, and every size here is bumped up from the old 5-8pt range
                // so the receipt is legible for customers who aren't reading it up close.
                Font fontStoreName = new Font(AppFonts.MorallySerif, 16, FontStyle.Bold);
                Font fontAddress = new Font("Arial", 9);
                Font fontCopyLabel = new Font("Arial", 9, FontStyle.Bold);
                Font fontTranscode = new Font("Arial", 8.5f);
                Font fontSectionHeader = new Font("Arial", 9.5f, FontStyle.Bold);
                Font fontItemTitle = new Font("Arial", 8);
                Font fontItemDetail = new Font("Arial", 7.5f);
                Font fontTotalsLabel = new Font("Arial", 9);
                Font fontGrandTotal = new Font("Arial", 10, FontStyle.Bold);
                Font fontFooterThanks = new Font("Arial", 9, FontStyle.Italic);
                Font fontFooterMeta = new Font("Arial", 7.5f);

                Pen thinPen = Pens.Black;
                Pen thickPen = new Pen(Color.Black, 1.5f);

                int startX = 5;
                decimal total = 0m;

                // Header block (logo through Transcode) is centered on the page; everything
                // below it (items, totals, footer) stays left-aligned/right-aligned to the same
                // margins so amounts line up into a clean column regardless of the configured
                // receipt printer's paper width.
                int pageWidth = e.MarginBounds.Width > 0 ? e.MarginBounds.Width : e.PageBounds.Width;
                int rightEdge = pageWidth - startX;

                void DrawCentered(string text, Font font, float yPos)
                {
                    float textWidth = e.Graphics.MeasureString(text, font).Width;
                    float x = Math.Max(0, (pageWidth - textWidth) / 2f);
                    e.Graphics.DrawString(text, font, Brushes.Black, x, yPos);
                }

                void DrawRightAligned(string text, Font font, float yPos)
                {
                    float textWidth = e.Graphics.MeasureString(text, font).Width;
                    float x = Math.Max(startX, rightEdge - textWidth);
                    e.Graphics.DrawString(text, font, Brushes.Black, x, yPos);
                }

                void DrawDivider(float yPos, Pen pen)
                {
                    e.Graphics.DrawLine(pen, startX, yPos, rightEdge, yPos);
                }

                // STORE_NAME is admin-configurable (System Config > Settings) and unlike the
                // rest of the receipt's fixed text, its length isn't known up front - shrink it
                // to fit rather than let a long store name run off the edge of a narrow roll.
                Font FitFont(string text, Font baseFont, float maxWidth)
                {
                    Font font = baseFont;
                    while (font.Size > 8f && e.Graphics.MeasureString(text, font).Width > maxWidth)
                        font = new Font(font.FontFamily, font.Size - 1f, font.Style);
                    return font;
                }

                // Word-wraps long item descriptions instead of letting them run off the edge of
                // the paper or overlap the next line. Capped at maxLines so one long description
                // can't push an unbounded number of lines down a narrow 58mm/80mm roll - once the
                // cap is hit, whatever's left over is truncated with "..." on the last line.
                List<string> WrapText(string text, Font font, float maxWidth, int maxLines = int.MaxValue)
                {
                    var lines = new List<string>();
                    var words = (text ?? "").Split(' ');
                    var current = new StringBuilder();

                    foreach (string word in words)
                    {
                        string candidate = current.Length == 0 ? word : current + " " + word;
                        if (current.Length > 0 && e.Graphics.MeasureString(candidate, font).Width > maxWidth)
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

                    if (lines.Count > maxLines)
                    {
                        string overflow = string.Join(" ", lines.Skip(maxLines - 1));
                        lines = lines.Take(maxLines - 1).ToList();

                        const string ellipsis = "...";
                        while (overflow.Length > 0 && e.Graphics.MeasureString(overflow + ellipsis, font).Width > maxWidth)
                            overflow = overflow.Substring(0, overflow.Length - 1);

                        lines.Add(overflow.TrimEnd() + ellipsis);
                    }

                    return lines;
                }

                // Customer's and counter's copy print as two pages of the same job (see
                // HasMorePages below); on a continuous thermal roll there's no page break between
                // them otherwise, so the second copy would butt right up against the first with
                // nothing to cut/tear along. Only applies when both are actually printing
                // together - not when printSingleReceiptCopyOnly renders just one copy (either
                // the customer's copy alone, or the counter's copy alone for SaveCounterReceiptAsJpeg).
                const float interCopyGapHeight = 60f;
                bool isSecondCopyInSameJob = receiptCopyIndex > 0 && !printSingleReceiptCopyOnly;
                float topOffset = isSecondCopyInSameJob ? interCopyGapHeight : 0f;

                if (isSecondCopyInSameJob)
                {
                    // A cut trace in the gap - a dashed line with a "CUT HERE" label - so the
                    // cashier has a visual guide for separating the two copies once printed.
                    float cutLineY = interCopyGapHeight / 2f;
                    using (var dashedPen = new Pen(Color.Black, 1f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                        e.Graphics.DrawLine(dashedPen, startX, cutLineY, rightEdge, cutLineY);

                    DrawCentered("- - - CUT HERE - - -", fontFooterMeta, cutLineY - 12);
                }

                int logoHeight = 0;
                if (picLogo.Image != null)
                {
                    const int maxLogoHeight = 60;
                    Image logo = picLogo.Image;
                    int logoWidth = (int)(logo.Width * ((double)maxLogoHeight / logo.Height));
                    int logoX = Math.Max(0, (pageWidth - logoWidth) / 2);
                    e.Graphics.DrawImage(logo, logoX, topOffset, logoWidth, maxLogoHeight);
                    logoHeight = maxLogoHeight;
                }

                float y = topOffset + 6 + logoHeight;

                DrawCentered(L_DEFAULTNAME, FitFont(L_DEFAULTNAME, fontStoreName, rightEdge - startX), y);
                y += 22;

                DrawCentered("Calayan, Gonzaga, Cagayan", fontAddress, y);
                y += 15;

                DrawCentered(ReceiptCopyLabels[receiptCopyIndex], fontCopyLabel, y);
                y += 15;

                DrawCentered("Transcode: " + TransactionCode, fontTranscode, y);
                y += 14;

                DrawDivider(y, thickPen);
                y += 12;

                e.Graphics.DrawString("ITEMS", fontSectionHeader, Brushes.Black, startX, y);
                y += 18;

                for (int i = 0; i < dgv_items.Rows.Count; i++)
                {
                    string title = dgv_items.Rows[i].Cells[1].Value?.ToString() ?? "";
                    string qty = dgv_items.Rows[i].Cells[2].Value?.ToString() ?? "";
                    decimal.TryParse(dgv_items.Rows[i].Cells[3].Value?.ToString(), out decimal unitPrice);
                    decimal.TryParse(dgv_items.Rows[i].Cells[4].Value?.ToString(), out decimal lineTotal);

                    if (TestPriceOverride.HasValue)
                    {
                        int.TryParse(qty, out int qtyCount);
                        unitPrice = TestPriceOverride.Value;
                        lineTotal = unitPrice * Math.Max(1, qtyCount);
                    }

                    total += lineTotal;

                    foreach (string titleLine in WrapText(title, fontItemTitle, rightEdge - startX, maxLines: 3))
                    {
                        e.Graphics.DrawString(titleLine, fontItemTitle, Brushes.Black, startX, y);
                        y += 13;
                    }

                    // "qty x unit price" on the left spells out how the right-aligned line
                    // total was arrived at, instead of an unlabeled "Price" that used to just
                    // repeat the total with no unit price shown anywhere on the receipt.
                    e.Graphics.DrawString($"{qty} x {unitPrice:0.00}", fontItemDetail, Brushes.Black, startX + 14, y);
                    DrawRightAligned(lineTotal.ToString("0.00"), fontItemDetail, y);
                    y += 16;

                    DrawDivider(y, thinPen);
                    y += 10;
                }

                y += 2;
                DrawDivider(y, thickPen);
                y += 14;

                decimal subtotal = total;
                decimal taxAmount = TaxConfig.ComputeTax(subtotal);
                decimal totalWithTax = subtotal + taxAmount;
                bool showTaxLines = TaxConfig.EffectiveTaxRatePercent > 0;

                if (showTaxLines)
                {
                    e.Graphics.DrawString("Subtotal:", fontTotalsLabel, Brushes.Black, startX, y);
                    DrawRightAligned(subtotal.ToString("0.00"), fontTotalsLabel, y);
                    y += 15;

                    e.Graphics.DrawString($"Tax ({TaxConfig.EffectiveTaxRatePercent:0.##}%):", fontTotalsLabel, Brushes.Black, startX, y);
                    DrawRightAligned(taxAmount.ToString("0.00"), fontTotalsLabel, y);
                    y += 15;
                }

                e.Graphics.DrawString("Grand Total:", fontGrandTotal, Brushes.Black, startX, y);
                DrawRightAligned(totalWithTax.ToString("0.00"), fontGrandTotal, y);
                y += 22;

                // A thin rule separates what's owed (subtotal/tax/grand total, above) from
                // how it was paid (cash/change, below) - previously these two conceptually
                // different blocks ran together with no visual break between them.
                DrawDivider(y, thinPen);
                y += 12;

                decimal cash = Convert.ToInt32(tbx_denomination.Text);
                e.Graphics.DrawString("Cash:", fontTotalsLabel, Brushes.Black, startX, y);
                DrawRightAligned(cash.ToString("0.00"), fontTotalsLabel, y);
                y += 15;

                e.Graphics.DrawString("Change:", fontTotalsLabel, Brushes.Black, startX, y);
                DrawRightAligned((cash - totalWithTax).ToString("0.00"), fontTotalsLabel, y);
                y += 20;

                DrawDivider(y, thickPen);
                y += 16;

                DrawCentered("Thank you for shopping! Come again!", fontFooterThanks, y);
                y += 16;

                DrawCentered("Date of purchase: " + DateTime.Now, fontFooterMeta, y);
                y += 12;

                DrawCentered("Cashier's ID: " + log_in, fontFooterMeta, y);

                pendingReceiptCode = !string.IsNullOrEmpty(TransactionCode) ? TransactionCode : "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss");
                pendingReceiptText = BuildReceiptText(total);
                lastReceiptContentHeight = y + 8;

                // One more copy to go? Ask the print system for another PrintPage call
                // instead of finishing the job - TransactionCode isn't cleared until
                // EndPrint, so the next copy still sees the same transcode. Skipped when only a
                // single copy was requested (e.g. the counter's copy is being rendered to a JPEG
                // separately rather than looped in as another page of this same job).
                if (!printSingleReceiptCopyOnly && receiptCopyIndex < ReceiptCopyLabels.Length - 1)
                {
                    receiptCopyIndex++;
                    e.HasMorePages = true;
                }
                else
                {
                    e.HasMorePages = false;
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Something went wrong while printing the receipt.", "Print Receipt", ex);
            }
        }

        private void printDocumentReceipt_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            receiptCopyIndex = 0;
        }
        private static string GetReceiptsDirectory()
        {
            string documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsDir, "Receipts");
        }

        /// <summary>
        /// True when System Config > Printer has COUNTER_RECEIPT_MODE set to save the counter's
        /// copy as a JPEG file instead of sending it to the physical printer. Default ("Print")
        /// keeps the original behavior of printing both copies.
        /// </summary>
        private static bool IsCounterReceiptJpegMode()
        {
            return string.Equals(
                ConfigurationManager.AppSettings["COUNTER_RECEIPT_MODE"],
                "Jpeg",
                StringComparison.OrdinalIgnoreCase);
        }

        // Pixels-per-inch used to size every on-screen receipt preview bitmap - shared with
        // ReceiptPreviewForm so its draggable margin handles convert screen pixels back to the
        // same hundredths-of-an-inch units System.Drawing.Printing.Margins uses.
        internal const float ReceiptPreviewDpi = 203f;

        internal readonly struct ReceiptPreviewResult
        {
            public Bitmap Image { get; }
            public int PageWidthPx { get; }
            public int MarginLeftPx { get; }
            public int MarginRightPx { get; }
            public int MarginTopPx { get; }

            public ReceiptPreviewResult(Bitmap image, int pageWidthPx, int marginLeftPx, int marginRightPx, int marginTopPx)
            {
                Image = image;
                PageWidthPx = pageWidthPx;
                MarginLeftPx = marginLeftPx;
                MarginRightPx = marginRightPx;
                MarginTopPx = marginTopPx;
            }
        }

        /// <summary>
        /// Renders the customer's copy onto an in-memory bitmap for on-screen preview - reuses
        /// the same drawing code PrintPage uses for the physical printer, without touching a print
        /// job. Backs System Config > Receipt Testing's preview window, so it needs something to
        /// draw even when there's no live sale in progress: if dgv_items has no real item rows,
        /// a couple of placeholder rows are added just long enough to render, then removed again.
        /// paperWidthMmOverride, if given, overrides whatever ApplyReceiptPrinterSettings detected
        /// from the configured printer, so 58mm/80mm can be previewed side by side without
        /// actually reconfiguring the receipt printer. marginLeft/Right/TopHundredthsOverride
        /// (hundredths of an inch, matching Margins) likewise override the detected margins - fed
        /// from the preview window's draggable boundary handles once the tester has dragged one.
        /// </summary>
        internal ReceiptPreviewResult RenderReceiptPreview(
            int? paperWidthMmOverride,
            int? marginLeftHundredthsOverride = null,
            int? marginRightHundredthsOverride = null,
            int? marginTopHundredthsOverride = null)
        {
            ApplyReceiptPrinterSettings(printDocumentReceipt);

            bool injectedSampleRows = dgv_items.Rows.Count == 0;
            if (injectedSampleRows)
            {
                dgv_items.Rows.Add("SAMPLE001", "Sample Item", 2, 45.00m, 90.00m);
                dgv_items.Rows.Add("SAMPLE002", "Sample Item With A Longer Description To Test Wrapping", 1, 125.50m, 125.50m);
            }

            string previousTransactionCode = TransactionCode;
            string previousPendingReceiptText = pendingReceiptText;
            string previousPendingReceiptCode = pendingReceiptCode;
            int previousReceiptCopyIndex = receiptCopyIndex;
            string previousDenominationText = tbx_denomination.Text;
            if (string.IsNullOrEmpty(TransactionCode))
                TransactionCode = "PREVIEW";
            // PrintPage does Convert.ToInt32(tbx_denomination.Text) for the Cash/Change lines -
            // fine at a real checkout (cash is always typed as a whole number before Enter is
            // pressed), but this preview can render with nothing typed there yet, or with
            // leftover text that isn't a clean integer (e.g. a decimal amount, or anything else),
            // either of which throws a FormatException. Validating with TryParse (rather than
            // just checking for empty) and falling back to the (rounded) total - rather than 0 -
            // keeps the Cash/Change lines on the preview without a misleading negative Change
            // for the all-too-common case of previewing before any cash amount has been typed.
            if (!int.TryParse(tbx_denomination.Text, out _))
                tbx_denomination.Text = Math.Round(TaxConfig.ComputeTotalWithTax(grandTotal)).ToString("0");

            try
            {
                PaperSize paperSize = printDocumentReceipt.DefaultPageSettings.PaperSize;
                Margins margins = printDocumentReceipt.DefaultPageSettings.Margins;

                int widthHundredths = paperWidthMmOverride.HasValue
                    ? (int)Math.Round(paperWidthMmOverride.Value / 25.4 * 100)
                    : paperSize.Width;
                int marginLeftHundredths = marginLeftHundredthsOverride ?? margins.Left;
                int marginRightHundredths = marginRightHundredthsOverride ?? margins.Right;
                int marginTopHundredths = marginTopHundredthsOverride ?? margins.Top;

                int pageWidthPx = Math.Max(1, (int)(widthHundredths / 100f * ReceiptPreviewDpi));
                int marginLeftPx = (int)(marginLeftHundredths / 100f * ReceiptPreviewDpi);
                int marginRightPx = (int)(marginRightHundredths / 100f * ReceiptPreviewDpi);
                int marginTopPx = (int)(marginTopHundredths / 100f * ReceiptPreviewDpi);
                int marginWidthPx = Math.Max(1, pageWidthPx - marginLeftPx - marginRightPx);

                const int scratchHeightPx = 3000;

                using (var scratch = new Bitmap(pageWidthPx, scratchHeightPx))
                {
                    using (Graphics g = Graphics.FromImage(scratch))
                    {
                        g.Clear(Color.White);

                        var marginBounds = new Rectangle(marginLeftPx, marginTopPx, marginWidthPx, scratchHeightPx - marginTopPx);
                        var pageBounds = new Rectangle(0, 0, pageWidthPx, scratchHeightPx);

                        // Only marginBounds/pageBounds (above) actually drive the layout inside
                        // metadata even though its PaperSize/Margins don't reflect this preview's
                        // paper-width or margin overrides.
                        receiptCopyIndex = 0; // CUSTOMER'S COPY
                        // and returns) instead of throwing, so it wouldn't reach the line that
                        // sets lastReceiptContentHeight if something in this render goes wrong.
                        // Reset it first so a failed render can't silently reuse a stale height
                        // from a previous, differently-sized receipt.
                        lastReceiptContentHeight = 0f;
                        var args = new PrintPageEventArgs(g, marginBounds, pageBounds, printDocumentReceipt.DefaultPageSettings);
                        PrintDocumentReceipt_PrintPage(this, args);
                    }

                    // Clamped to a sane minimum so a render that failed partway (see reset above)
                    // still shows a visible sheet of paper instead of collapsing to a near-invisible
                    // sliver a couple pixels tall.
                    const int minContentHeightPx = 200;
                    int cropHeight = (int)Math.Min(scratchHeightPx, Math.Max(lastReceiptContentHeight, minContentHeightPx));
                    var cropped = new Bitmap(pageWidthPx, cropHeight);
                    using (Graphics g = Graphics.FromImage(cropped))
                        g.DrawImage(scratch, new Rectangle(0, 0, pageWidthPx, cropHeight), new Rectangle(0, 0, pageWidthPx, cropHeight), GraphicsUnit.Pixel);

                    return new ReceiptPreviewResult(cropped, pageWidthPx, marginLeftPx, marginRightPx, marginTopPx);
                }
            }
            finally
            {
                TransactionCode = previousTransactionCode;
                pendingReceiptText = previousPendingReceiptText;
                pendingReceiptCode = previousPendingReceiptCode;
                receiptCopyIndex = previousReceiptCopyIndex;
                tbx_denomination.Text = previousDenominationText;

                if (injectedSampleRows)
                {
                    dgv_items.Rows.Clear();

                    // The cart was empty before these sample rows went in (that's what
                    // injectedSampleRows means), so restore that exactly - dgv_items_RowsAdded
                    // incrementally folded the sample rows' price/qty into grandTotal/count,
                    // and removing rows doesn't undo that on its own.
                    grandTotal = 0;
                    count = 0;
                }
            }
        }

        /// <summary>
        /// Renders the counter's copy onto an in-memory bitmap - the same drawing code PrintPage
        /// uses for the physical printer - and saves it as a JPEG under the folder configured in
        /// System Config > Printer (COUNTER_RECEIPT_JPEG_PATH), or Documents\Receipts\Jpeg if
        /// nothing's been configured, instead of consuming a print. Called right after the
        /// customer's copy has been printed, while TransactionCode/dgv_items/tbx_denomination
        /// still reflect this transaction.
        /// </summary>
        private void SaveCounterReceiptAsJpeg()
        {
            // Re-applied here (not just relying on the caller having done it before printing the
            // customer's copy) so the JPEG's paper width always tracks the configured receipt
            // printer even if a future call site saves a JPEG without printing first.
            ApplyReceiptPrinterSettings(printDocumentReceipt);

            PaperSize paperSize = printDocumentReceipt.DefaultPageSettings.PaperSize;
            Margins margins = printDocumentReceipt.DefaultPageSettings.Margins;

            // Printer paper sizes are reported in hundredths of an inch; 203 DPI is a typical
            // thermal receipt printer's native resolution, so the saved JPEG comes out roughly
            // the size/proportions the paper copy would have been.
            const float dpi = 203f;

            // The image copy is meant to look like a standard 80mm thermal receipt regardless of
            // what the configured/fallback printer's driver reports - without this cap, no
            // installed/configured RECEIPT_PRINTER_NAME (or one whose driver defaults to a wider
            // page like Letter) would stretch the receipt's text/dividers out to that width
            // instead of staying inside an 80mm roll. Only clamps the width used for this JPEG
            // untouched, so an intentionally configured 80mm/58mm/etc. printer is never widened.
            const float maxJpegPaperWidthMm = 80f;
            int maxJpegPaperWidthHundredths = (int)Math.Round(maxJpegPaperWidthMm / 25.4 * 100);
            int effectiveWidthHundredths = Math.Min(paperSize.Width, maxJpegPaperWidthHundredths);

            int pageWidthPx = Math.Max(1, (int)(effectiveWidthHundredths / 100f * dpi));
            int marginLeftPx = (int)(margins.Left / 100f * dpi);
            int marginTopPx = (int)(margins.Top / 100f * dpi);
            int marginWidthPx = Math.Max(1, pageWidthPx - marginLeftPx - (int)(margins.Right / 100f * dpi));

            // finishes drawing - render onto a generously tall scratch bitmap, then crop to
            // lastReceiptContentHeight afterward.
            const int scratchHeightPx = 3000;

            string savedFilePath;
            using (var bitmap = new Bitmap(pageWidthPx, scratchHeightPx))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);

                    var marginBounds = new Rectangle(marginLeftPx, marginTopPx, marginWidthPx, scratchHeightPx - marginTopPx);
                    var pageBounds = new Rectangle(0, 0, pageWidthPx, scratchHeightPx);

                    receiptCopyIndex = ReceiptCopyLabels.Length - 1; // COUNTER'S COPY
                    var args = new PrintPageEventArgs(g, marginBounds, pageBounds, printDocumentReceipt.DefaultPageSettings);
                    PrintDocumentReceipt_PrintPage(this, args);
                }

                int cropHeight = (int)Math.Min(scratchHeightPx, Math.Max(lastReceiptContentHeight, 1));

                try
                {
                    string configuredJpegPath = ConfigurationManager.AppSettings["COUNTER_RECEIPT_JPEG_PATH"];
                    string receiptsDir = !string.IsNullOrWhiteSpace(configuredJpegPath)
                        ? configuredJpegPath
                        : Path.Combine(GetReceiptsDirectory(), "Jpeg");
                    Directory.CreateDirectory(receiptsDir);

                    string code = !string.IsNullOrEmpty(TransactionCode) ? TransactionCode : "TXN" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"Receipt_{code}_{DateTime.Now:yyyyMMdd_HHmmss}_counter.jpg";
                    savedFilePath = Path.Combine(receiptsDir, fileName);

                    string printerLabel = string.IsNullOrWhiteSpace(printDocumentReceipt.PrinterSettings.PrinterName)
                        ? "Default"
                        : printDocumentReceipt.PrinterSettings.PrinterName;
                    double paperWidthMm = effectiveWidthHundredths / 100.0 * 25.4;
                    string slugText = $"TRIM {pageWidthPx}×{cropHeight}px  •  BLEED {BleedMm:0}mm  •  " +
                        $"{printerLabel}  •  {paperWidthMm:0}mm roll  •  {code}  •  {DateTime.Now:yyyy-MM-dd HH:mm}";

                    using (var withMarks = RenderWithBleedAndSlug(bitmap, pageWidthPx, cropHeight, dpi, slugText))
                    {
                        withMarks.Save(savedFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show(this, "Could not save the counter's receipt as a JPEG image.", "Failed to Save Counter's Receipt", ex);
                }
            }

            // pendingReceiptText/pendingReceiptCode were set by the manual PrintPage call above
            // usual .txt backup here so the counter's copy still leaves a plain-text record too.
            if (!string.IsNullOrEmpty(pendingReceiptText))
            {
                try
                {
                    string receiptsDir = GetReceiptsDirectory();
                    Directory.CreateDirectory(receiptsDir);

                    string fileName = $"Receipt_{pendingReceiptCode}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    File.WriteAllText(Path.Combine(receiptsDir, fileName), pendingReceiptText);
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show(this, "Could not save a text copy of the receipt.", "Failed to Save Receipt Copy", ex);
                }
                finally
                {
                    pendingReceiptText = null;
                    pendingReceiptCode = null;
                }
            }
        }

        // How far the crop/bleed marks sit from the receipt's trim edge, in millimeters - a
        // conventional small print-shop bleed allowance (the marks live in this margin, not on
        // top of the receipt content).
        private const float BleedMm = 3f;

        /// <summary>
        /// Wraps the tightly-cropped receipt render in a print-production bleed border: corner
        /// crop marks just outside the trim edge on all four sides, plus a slug line along the
        /// bottom identifying the trim size, bleed, configured printer/paper, transaction, and
        /// timestamp - the same marks a print shop expects around camera-ready art, so the saved
        /// JPEG can be sent out for physical reproduction without redoing the layout. Returns a
        /// new bitmap; the caller owns disposing it.
        /// </summary>
        private static Bitmap RenderWithBleedAndSlug(Bitmap source, int trimWidthPx, int trimHeightPx, float dpi, string slugText)
        {
            int bleedPx = Math.Max(1, (int)Math.Round(dpi * BleedMm / 25.4));
            Font slugFont = new Font("Arial", 6.5f);
            int slugHeightPx;
            using (var measureBmp = new Bitmap(1, 1))
            using (Graphics measureG = Graphics.FromImage(measureBmp))
                slugHeightPx = (int)Math.Ceiling(measureG.MeasureString(slugText, slugFont).Height) + 6;

            int width = trimWidthPx + 2 * bleedPx;
            int height = bleedPx + trimHeightPx + bleedPx + slugHeightPx;
            var trimBounds = new Rectangle(bleedPx, bleedPx, trimWidthPx, trimHeightPx);

            var result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.White);
                g.DrawImage(source, trimBounds, new Rectangle(0, 0, trimWidthPx, trimHeightPx), GraphicsUnit.Pixel);

                using (var markPen = new Pen(Color.Black, 1f))
                {
                    const int gap = 4; // clearance between the trim edge and where each mark starts
                    int markLen = Math.Max(1, bleedPx - gap);

                    void HMark(int y, int fromX, int toX) => g.DrawLine(markPen, fromX, y, toX, y);
                    void VMark(int x, int fromY, int toY) => g.DrawLine(markPen, x, fromY, x, toY);

                    // Top-left
                    VMark(trimBounds.Left, trimBounds.Top - gap - markLen, trimBounds.Top - gap);
                    HMark(trimBounds.Top, trimBounds.Left - gap - markLen, trimBounds.Left - gap);
                    // Top-right
                    VMark(trimBounds.Right, trimBounds.Top - gap - markLen, trimBounds.Top - gap);
                    HMark(trimBounds.Top, trimBounds.Right + gap, trimBounds.Right + gap + markLen);
                    // Bottom-left
                    VMark(trimBounds.Left, trimBounds.Bottom + gap, trimBounds.Bottom + gap + markLen);
                    HMark(trimBounds.Bottom, trimBounds.Left - gap - markLen, trimBounds.Left - gap);
                    // Bottom-right
                    VMark(trimBounds.Right, trimBounds.Bottom + gap, trimBounds.Bottom + gap + markLen);
                    HMark(trimBounds.Bottom, trimBounds.Right + gap, trimBounds.Right + gap + markLen);
                }

                using (var slugBrush = new SolidBrush(Color.Gray))
                {
                    float textWidth = g.MeasureString(slugText, slugFont).Width;
                    float x = Math.Max(0, (width - textWidth) / 2f);
                    float y = bleedPx + trimHeightPx + bleedPx + (slugHeightPx - slugFont.Height) / 2f;
                    g.DrawString(slugText, slugFont, slugBrush, x, y);
                }
            }

            return result;
        }

        private void printDocumentReceipt_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (string.IsNullOrEmpty(pendingReceiptText))
                return;

            try
            {
                string receiptsDir = GetReceiptsDirectory();
                Directory.CreateDirectory(receiptsDir);

                string fileName = $"Receipt_{pendingReceiptCode}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(receiptsDir, fileName);

                File.WriteAllText(filePath, pendingReceiptText);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not save a text copy of the receipt.", "Failed to Save Receipt Copy", ex);
            }
            finally
            {
                pendingReceiptText = null;
                pendingReceiptCode = null;
            }
        }

        private string BuildReceiptText(decimal total)
        {
            var sb = new StringBuilder();
            sb.AppendLine(L_DEFAULTNAME);
            sb.AppendLine("Calayan, Gonzaga, Cagayan");
            sb.AppendLine("Transcode: " + TransactionCode);
            sb.AppendLine(new string('-', 40));
            sb.AppendLine("ITEMS");

            for (int i = 0; i < dgv_items.Rows.Count; i++)
            {
                string title = dgv_items.Rows[i].Cells[1].Value?.ToString() ?? "";
                string qty = dgv_items.Rows[i].Cells[2].Value?.ToString() ?? "";
                string price = dgv_items.Rows[i].Cells[4].Value?.ToString() ?? "";

                sb.AppendLine(title);
                sb.AppendLine($"  Qty: {qty}    Price: {price}");
            }

            sb.AppendLine(new string('-', 40));

            decimal subtotal = total;
            decimal taxAmount = TaxConfig.ComputeTax(subtotal);
            decimal totalWithTax = subtotal + taxAmount;

            if (TaxConfig.EffectiveTaxRatePercent > 0)
            {
                sb.AppendLine("Subtotal: " + subtotal.ToString("0.00"));
                sb.AppendLine($"Tax ({TaxConfig.EffectiveTaxRatePercent:0.##}%): " + taxAmount.ToString("0.00"));
            }
            sb.AppendLine("Grand Total: " + totalWithTax.ToString("0.00"));
            sb.AppendLine($"Cash: {tbx_denomination.Text}.00");
            sb.AppendLine("Change: " + (Convert.ToInt32(tbx_denomination.Text) - totalWithTax).ToString("0.00"));
            sb.AppendLine();
            sb.AppendLine("Thank you for shopping! Come again!");
            sb.AppendLine("Date of purchase: " + DateTime.Now);
            sb.AppendLine("Cashier's ID: " + log_in);

            return sb.ToString();
        }

        private void printDocumentOutOfStock_BeginPrint(object sender, PrintEventArgs e)
        {
            outOfStockPrintRowIndex = 0;
        }

        /// <summary>
        /// Prints the Dashboard's "Out of Stock Items" list (dgvDashboardLowStock). Laid out the
        /// Serial/Qty detail line, page-width-driven rather than fixed column offsets - because
        /// this is meant to come out of the same narrow POS80/58mm thermal roll as receipts, not
        /// a letter/A4 sheet, and a rigid table would clip on paper that narrow.
        /// </summary>
        private void printDocumentOutOfStock_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Font fontTitle = new Font(AppFonts.MorallySerif, 13, FontStyle.Bold);
                Font fontMeta = new Font("Arial", 8.5f);
                Font fontItemTitle = new Font("Arial", 9);
                Font fontItemDetail = new Font("Arial", 8.5f);

                Pen thinPen = Pens.Black;
                Pen thickPen = new Pen(Color.Black, 1.5f);

                int startX = 5;
                int pageWidth = e.MarginBounds.Width > 0 ? e.MarginBounds.Width : e.PageBounds.Width;
                int rightEdge = pageWidth - startX;
                float pageBottom = e.MarginBounds.Top + e.MarginBounds.Height;

                void DrawCentered(string text, Font font, float yPos)
                {
                    float textWidth = e.Graphics.MeasureString(text, font).Width;
                    float x = Math.Max(0, (pageWidth - textWidth) / 2f);
                    e.Graphics.DrawString(text, font, Brushes.Black, x, yPos);
                }

                void DrawRightAligned(string text, Font font, float yPos)
                {
                    float textWidth = e.Graphics.MeasureString(text, font).Width;
                    float x = Math.Max(startX, rightEdge - textWidth);
                    e.Graphics.DrawString(text, font, Brushes.Black, x, yPos);
                }

                void DrawDivider(float yPos, Pen pen) => e.Graphics.DrawLine(pen, startX, yPos, rightEdge, yPos);

                List<string> WrapText(string text, Font font, float maxWidth)
                {
                    var lines = new List<string>();
                    var words = (text ?? "").Split(' ');
                    var current = new StringBuilder();

                    foreach (string word in words)
                    {
                        string candidate = current.Length == 0 ? word : current + " " + word;
                        if (current.Length > 0 && e.Graphics.MeasureString(candidate, font).Width > maxWidth)
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

                float y = e.MarginBounds.Top;

                if (outOfStockPrintRowIndex == 0)
                {
                    DrawCentered(L_DEFAULTNAME, fontTitle, y);
                    y += 20;
                    DrawCentered("OUT OF STOCK ITEMS", fontMeta, y);
                    y += 13;
                    DrawCentered(DateTime.Now.ToString("MMM d, yyyy h:mm tt"), fontMeta, y);
                    y += 14;
                    DrawDivider(y, thickPen);
                    y += 10;
                }

                while (outOfStockPrintRowIndex < dgvDashboardLowStock.Rows.Count)
                {
                    DataGridViewRow row = dgvDashboardLowStock.Rows[outOfStockPrintRowIndex];
                    string serial = row.Cells[0].Value?.ToString() ?? "";
                    string description = row.Cells[1].Value?.ToString() ?? "";
                    string qty = row.Cells[2].Value?.ToString() ?? "";

                    List<string> titleLines = WrapText(description, fontItemTitle, rightEdge - startX);
                    float itemHeight = titleLines.Count * 13 + 16 + 10;

                    // Doesn't fit on this page - stop here (unless nothing's been drawn on this
                    // page yet, in which case draw it anyway so a single oversized item can't
                    // stall pagination in an endless empty-page loop) and pick it up on the next.
                    if (y + itemHeight > pageBottom && y > e.MarginBounds.Top)
                        break;

                    foreach (string line in titleLines)
                    {
                        e.Graphics.DrawString(line, fontItemTitle, Brushes.Black, startX, y);
                        y += 13;
                    }

                    e.Graphics.DrawString("Serial: " + serial, fontItemDetail, Brushes.Black, startX, y);
                    DrawRightAligned("Qty: " + qty, fontItemDetail, y);
                    y += 16;

                    DrawDivider(y, thinPen);
                    y += 10;

                    outOfStockPrintRowIndex++;
                }

                e.HasMorePages = outOfStockPrintRowIndex < dgvDashboardLowStock.Rows.Count;
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Something went wrong while printing the out-of-stock report.", "Print Out-of-Stock Report", ex);
            }
        }

        private void btnPrintOutOfStock_Click(object sender, EventArgs e)
        {
            if (dgvDashboardLowStock.Rows.Count == 0)
            {
                MessageBox.Show("No out-of-stock items to print.", "Nothing to Print");
                return;
            }

            // Targets the same configured POS80 printer as receipts (paper size/margins pulled
            // straight from its driver) so the print actually matches the roll it'll come out
            // on. No preview gate here either - same as the sale receipt, this must go straight
            // to the printer instead of waiting on the cashier to click Print in a dialog first.
            ApplyReceiptPrinterSettings(printDocumentOutOfStock);

            printDocumentOutOfStock.Print();
        }
    }
}
