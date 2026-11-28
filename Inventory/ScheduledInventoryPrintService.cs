using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
{
    /// <summary>
    /// Prints a categorized (Out of Stock / Low Stock / In Stock) inventory report, items sorted
    /// alphabetically by description within each category, to the configured receipt printer
    /// (System Config > Printer) on a configurable cadence (INVENTORY_PRINT_SCHEDULE_ENABLED/
    /// _FREQUENCY, System Config > Inventory Print Schedule). Same startup catch-up pattern as
    /// ScheduledReportService - this app has no background service, so whichever terminal opens
    /// first after the configured interval has elapsed does the printing - but unlike emailing a
    /// report, printing consumes real paper, so this defaults to OFF rather than opt-out, and
    /// even once due it still asks for a Yes/No confirmation (defaulting to No) before printing.
    /// "Low Stock" uses the exact same LOW_STOCK_THRESHOLD as the Low Stock Reminder feature
    /// (via AppConfigService) so the category boundary means the same thing everywhere in the app.
    /// </summary>
    internal static class ScheduledInventoryPrintService
    {
        private static string StateFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "ScheduledInventoryPrintLastPrinted.txt");

        public static bool IsScheduleEnabled =>
            string.Equals(ConfigurationManager.AppSettings["INVENTORY_PRINT_SCHEDULE_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);

        public static ScheduledReportService.Frequency GetConfiguredFrequency() =>
            Enum.TryParse(ConfigurationManager.AppSettings["INVENTORY_PRINT_SCHEDULE_FREQUENCY"], ignoreCase: true, out ScheduledReportService.Frequency freq)
                ? freq
                : ScheduledReportService.Frequency.Weekly;

        public static DateTime? GetLastPrinted()
        {
            if (!File.Exists(StateFilePath))
                return null;

            return DateTime.TryParse(File.ReadAllText(StateFilePath), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out DateTime lastPrinted) ? (DateTime?)lastPrinted : null;
        }

        public static DateTime? GetNextDue()
        {
            DateTime? lastPrinted = GetLastPrinted();
            return lastPrinted == null ? (DateTime?)null : AddInterval(lastPrinted.Value, GetConfiguredFrequency());
        }

        /// <summary>Startup catch-up check: prints only if enabled, due, and a receipt printer is
        /// configured - and only after the user confirms a Yes/No prompt defaulting to No, since
        /// unlike emailing a report this consumes real paper and shouldn't fire unattended just
        /// because a terminal happened to be the first one opened after the interval elapsed. A
        /// decline (or dismissing the prompt any other way) leaves the last-printed marker
        /// untouched, same as a failure - the very next launch that's still due asks again rather
        /// than silently skipping the report entirely. Swallows all failures - a failed scheduled
        /// print must never block startup.</summary>
        public static async Task CheckAndPrintAsync()
        {
            try
            {
                if (!IsScheduleEnabled || !Purchases.IsReceiptPrinterConfigured() || !IsDue())
                    return;

                DialogResult confirm = MessageBox.Show(
                    "A scheduled inventory report is due to print. Continue?",
                    "Scheduled Inventory Print",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (confirm != DialogResult.Yes)
                    return;

                await PrintNowAsync();
            }
            catch
            {
                // Best-effort only, same reasoning as ScheduledReportService.CheckAndSendAsync -
                // leaving the last-printed marker untouched means the next launch simply retries.
            }
        }

        /// <summary>
        /// Builds a ready-to-use PrintDocument for the categorized/alphabetized report, printer
        /// settings already applied - used both for printing directly (automated catch-up) and
        /// for showing in a PrintPreviewDialog first (System Config > Inventory Print Schedule >
        /// "Print Preview"). The last-printed marker and audit log only get written from
        /// EndPrint, so previewing without actually printing (closing the preview window) has no
        /// side effects - it only fires once a real print happens, however it was triggered
        /// (PrintPreviewDialog's own Print button, or a direct Print() call). BeginPrint resets
        /// pagination state so the same document can be paginated more than once (the preview
        /// dialog renders every page up front, then prints again if the user clicks Print inside
        /// it - without the reset, that second pass would resume from the end and print nothing).
        /// </summary>
        public static async Task<PrintDocument> CreatePrintDocumentAsync()
        {
            List<PrintLine> lines = await BuildPrintLinesAsync();
            if (lines.Count == 0)
                throw new InvalidOperationException("There are no items in inventory to print.");

            var document = new PrintDocument();
            int lineIndex = 0;
            bool isFirstPage = true;

            Purchases.ApplyReceiptPrinterSettings(document);
            document.BeginPrint += (s, e) => { lineIndex = 0; isFirstPage = true; };
            document.PrintPage += (s, e) => DrawPage(e, lines, ref lineIndex, ref isFirstPage);
            document.EndPrint += (s, e) =>
            {
                MarkPrinted();

                int itemCount = lines.Count(l => l.IsItem);
                string logText = $"Categorized inventory report printed ({itemCount} item(s)).";
                _ = ActivityLog.LogAsync(logText);
                OperationLog.RecordAction(nameof(ScheduledInventoryPrintService), logText);
            };

            return document;
        }

        /// <summary>Automated/scheduled trigger: prints immediately with no preview, since
        /// nobody's there to dismiss a dialog during an unattended catch-up check. Lets
        /// exceptions propagate so the caller (CheckAndPrintAsync, or a manual "Print Now") can
        /// decide how to handle them.</summary>
        public static async Task PrintNowAsync()
        {
            using (PrintDocument document = await CreatePrintDocumentAsync())
                document.Print();
        }

        private static bool IsDue()
        {
            DateTime? lastPrinted = GetLastPrinted();
            if (lastPrinted == null)
                return true;

            return DateTime.Now >= AddInterval(lastPrinted.Value, GetConfiguredFrequency());
        }

        private static DateTime AddInterval(DateTime date, ScheduledReportService.Frequency frequency)
        {
            switch (frequency)
            {
                case ScheduledReportService.Frequency.Daily: return date.AddDays(1);
                case ScheduledReportService.Frequency.Monthly: return date.AddMonths(1);
                case ScheduledReportService.Frequency.Quarterly: return date.AddMonths(3);
                case ScheduledReportService.Frequency.Yearly: return date.AddYears(1);
                default: return date.AddDays(7);
            }
        }

        private static void MarkPrinted()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StateFilePath));
            File.WriteAllText(StateFilePath, DateTime.Now.ToString("o", CultureInfo.InvariantCulture));
        }

        private readonly struct PrintLine
        {
            private PrintLine(bool isHeader, string headerText, string serial, string description, string quantity, string status)
            {
                IsHeader = isHeader;
                HeaderText = headerText;
                Serial = serial;
                Description = description;
                Quantity = quantity;
                Status = status;
            }

            public static PrintLine Header(string text) => new PrintLine(true, text, null, null, null, null);

            public static PrintLine Item(string serial, string description, string quantity, string status) =>
                new PrintLine(false, null, serial, description, quantity, status);

            public bool IsHeader { get; }
            public bool IsItem => !IsHeader;
            public string HeaderText { get; }
            public string Serial { get; }
            public string Description { get; }
            public string Quantity { get; }
            public string Status { get; }
        }

        private const string UncategorizedLabel = "UNCATEGORIZED";

        // Grouped by each item's own category (items_.category) rather than stock status - stock
        // status is still surfaced, just per-item (next to its quantity) instead of as the
        // grouping dimension. Categories sort alphabetically with blank/unset category bucketed
        // into one "Uncategorized" group at the end, rather than scattering those items across
        // whatever position an empty string would otherwise sort to.
        private static async Task<List<PrintLine>> BuildPrintLinesAsync()
        {
            (bool _, int threshold, int _) = await AppConfigService.GetLowStockReminderSettingsAsync();

            DataTable items = DataCacheManager.Instance.GetTable("Items");
            bool hasCategoryColumn = items.Columns.Contains("category");

            var byCategory = new Dictionary<string, List<DataRow>>(StringComparer.OrdinalIgnoreCase);

            foreach (DataRow row in items.Rows)
            {
                string category = hasCategoryColumn ? row["category"]?.ToString()?.Trim() : null;
                if (string.IsNullOrEmpty(category))
                    category = UncategorizedLabel;

                if (!byCategory.TryGetValue(category, out List<DataRow> rows))
                    byCategory[category] = rows = new List<DataRow>();

                rows.Add(row);
            }

            var lines = new List<PrintLine>();
            IEnumerable<string> orderedCategories = byCategory.Keys
                .Where(c => !string.Equals(c, UncategorizedLabel, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase);

            foreach (string category in orderedCategories)
                AppendCategory(lines, category.ToUpperInvariant(), byCategory[category], threshold);

            if (byCategory.TryGetValue(UncategorizedLabel, out List<DataRow> uncategorized))
                AppendCategory(lines, UncategorizedLabel, uncategorized, threshold);

            return lines;
        }

        private static string StockStatus(DataRow row, int lowStockThreshold)
        {
            long qty = row["new_quantity"] == DBNull.Value ? 0 : Convert.ToInt64(row["new_quantity"]);
            if (qty <= 0)
                return "OUT OF STOCK";
            if (qty <= lowStockThreshold)
                return "LOW STOCK";
            return null;
        }

        private static void AppendCategory(List<PrintLine> lines, string title, List<DataRow> rows, int lowStockThreshold)
        {
            if (rows.Count == 0)
                return;

            lines.Add(PrintLine.Header($"{title} ({rows.Count})"));

            foreach (DataRow row in rows.OrderBy(r => r["description"]?.ToString() ?? "", StringComparer.OrdinalIgnoreCase))
            {
                lines.Add(PrintLine.Item(
                    row["serial"]?.ToString() ?? "",
                    row["description"]?.ToString()?.ToUpperInvariant() ?? "",
                    row["new_quantity"]?.ToString() ?? "0",
                    StockStatus(row, lowStockThreshold)));
            }
        }

        /// <summary>
        /// Paginated draw, flattened-list version of Purchases.printDocumentOutOfStock_PrintPage -
        /// laid out page-width-driven rather than fixed column offsets since this is meant to come
        /// out of the same narrow POS80/58mm thermal roll as receipts, not a letter/A4 sheet.
        /// </summary>
        private static void DrawPage(PrintPageEventArgs e, List<PrintLine> lines, ref int lineIndex, ref bool isFirstPage)
        {
            Font fontTitle = new Font(AppFonts.MorallySerif, 13, FontStyle.Bold);
            Font fontMeta = new Font("Arial", 8.5f);
            Font fontHeader = new Font("Arial", 9.5f, FontStyle.Bold);
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

            void DrawDivider(float yPos, Pen pen) => e.Graphics.DrawLine(pen, startX, yPos, rightEdge, yPos);

            // A "— LABEL —" rule rather than a plain divider or a plain left-aligned heading, so
            // each category (Out of Stock / Low Stock / In Stock) reads as a distinct, named
            // section break on the printout instead of just a bold line blending into the item
            // list above and below it.
            void DrawLabelledSeparator(string text, Font font, float yPos)
            {
                const float sidePadding = 8f;
                float textWidth = e.Graphics.MeasureString(text, font).Width;
                float textX = Math.Max(startX, (pageWidth - textWidth) / 2f);
                float lineY = yPos + font.GetHeight() / 2f;

                if (textX > startX)
                    e.Graphics.DrawLine(thickPen, startX, lineY, textX - sidePadding, lineY);

                float afterTextX = textX + textWidth + sidePadding;
                if (afterTextX < rightEdge)
                    e.Graphics.DrawLine(thickPen, afterTextX, lineY, rightEdge, lineY);

                e.Graphics.DrawString(text, font, Brushes.Black, textX, yPos);
            }

            List<string> WrapText(string text, Font font, float maxWidth)
            {
                var wrapped = new List<string>();
                var words = (text ?? "").Split(' ');
                var current = new System.Text.StringBuilder();

                foreach (string word in words)
                {
                    string candidate = current.Length == 0 ? word : current + " " + word;
                    if (current.Length > 0 && e.Graphics.MeasureString(candidate, font).Width > maxWidth)
                    {
                        wrapped.Add(current.ToString());
                        current.Clear();
                        current.Append(word);
                    }
                    else
                    {
                        current.Clear();
                        current.Append(candidate);
                    }
                }

                wrapped.Add(current.ToString());
                return wrapped;
            }

            float y = e.MarginBounds.Top;

            if (isFirstPage)
            {
                isFirstPage = false;

                string storeName = ConfigurationManager.AppSettings["STORE_NAME"];
                DrawCentered(string.IsNullOrWhiteSpace(storeName) ? "Inventory Report" : storeName, fontTitle, y);
                y += 20;
                DrawCentered("INVENTORY REPORT - CATEGORIZED, A-Z", fontMeta, y);
                y += 13;
                DrawCentered(DateTime.Now.ToString("MMM d, yyyy h:mm tt"), fontMeta, y);
                y += 14;
                DrawDivider(y, thickPen);
                y += 10;
            }

            while (lineIndex < lines.Count)
            {
                PrintLine line = lines[lineIndex];

                if (line.IsHeader)
                {
                    const float topGap = 6f;
                    const float headerHeight = topGap + 18f;
                    if (y + headerHeight > pageBottom && y > e.MarginBounds.Top)
                        break;

                    y += topGap;
                    DrawLabelledSeparator(line.HeaderText, fontHeader, y);
                    y += headerHeight - topGap;
                    lineIndex++;
                    continue;
                }

                List<string> titleLines = WrapText(line.Description, fontItemTitle, rightEdge - startX);
                // Serial and qty/status always get their own line each (rather than one line with
                // qty/status right-aligned) - a long serial number can otherwise eat into the space
                // reserved for the right-aligned text, overlapping the two into unreadable garbled
                // text instead of wrapping.
                float itemHeight = titleLines.Count * 13 + 13 + 16 + 10;

                // Doesn't fit on this page - stop here (unless nothing's been drawn on this page
                // yet, in which case draw it anyway so a single oversized item can't stall
                // pagination in an endless empty-page loop) and pick it up on the next.
                if (y + itemHeight > pageBottom && y > e.MarginBounds.Top)
                    break;

                foreach (string titleLine in titleLines)
                {
                    e.Graphics.DrawString(titleLine, fontItemTitle, Brushes.Black, startX, y);
                    y += 13;
                }

                string qtyText = string.IsNullOrEmpty(line.Status)
                    ? "Qty: " + line.Quantity
                    : $"Qty: {line.Quantity} ({line.Status})";

                e.Graphics.DrawString("Serial: " + line.Serial, fontItemDetail, Brushes.Black, startX, y);
                y += 13;
                e.Graphics.DrawString(qtyText, fontItemDetail, Brushes.Black, startX, y);
                y += 16;

                DrawDivider(y, thinPen);
                y += 10;

                lineIndex++;
            }

            e.HasMorePages = lineIndex < lines.Count;
        }
    }
}
