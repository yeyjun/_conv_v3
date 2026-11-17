using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using YourApp.Data;

namespace Inventory
{
    /// <summary>
    /// Emails a sales + inventory summary to STORE_EMAIL on a configurable cadence
    /// (REPORT_SCHEDULE_FREQUENCY - System Config > Report Schedule). This app has no
    /// background service, so the send is a startup catch-up check instead: whichever
    /// machine opens the app first after the configured interval has elapsed since the
    /// last successful send does the sending. The last-sent timestamp is tracked in a
    /// local file (same OfflineCache folder ActivityLog/DataCacheManager use for their
    /// own local state), not in App.config, so it survives independently of config edits.
    /// </summary>
    internal static class ScheduledReportService
    {
        public enum Frequency
        {
            Daily,
            Weekly,
            Monthly,
            Quarterly,
            Yearly
        }

        private static string StateFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "ScheduledReportLastSent.txt");

        public static Frequency GetConfiguredFrequency() =>
            Enum.TryParse(ConfigurationManager.AppSettings["REPORT_SCHEDULE_FREQUENCY"], ignoreCase: true, out Frequency freq)
                ? freq
                : Frequency.Weekly;

        public static DateTime? GetLastSent()
        {
            if (!File.Exists(StateFilePath))
                return null;

            return DateTime.TryParse(File.ReadAllText(StateFilePath), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out DateTime lastSent) ? (DateTime?)lastSent : null;
        }

        public static DateTime? GetNextDue()
        {
            DateTime? lastSent = GetLastSent();
            return lastSent == null ? (DateTime?)null : AddInterval(lastSent.Value, GetConfiguredFrequency());
        }

        /// <summary>Startup catch-up check: sends only if due, and swallows all failures.</summary>
        public static async Task CheckAndSendAsync()
        {
            try
            {
                string recipient = ConfigurationManager.AppSettings["STORE_EMAIL"];
                if (string.IsNullOrWhiteSpace(recipient) || !IsDue())
                    return;

                await SendReportAsync(recipient);
            }
            catch
            {
                // Best-effort only - a failed scheduled report must never block startup, and
                // leaving the last-sent marker untouched means the next launch will simply try again.
            }
        }

        /// <summary>
        /// Manual trigger (System Config > Report Schedule > "Send Report Now"). Sends
        /// immediately regardless of the schedule, and lets exceptions propagate so the
        /// caller can show what went wrong.
        /// </summary>
        public static async Task SendNowAsync(string recipient)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new InvalidOperationException("No recipient email address was provided.");

            await SendReportAsync(recipient);
        }

        private static async Task SendReportAsync(string recipient)
        {
            Frequency frequency = GetConfiguredFrequency();
            DateTime periodEnd = DateTime.Now;
            DateTime periodStart = SubtractInterval(periodEnd, frequency);

            List<SaleRecord> sales = await FetchSalesAsync(periodStart, periodEnd);
            InventorySummary inventory = BuildInventorySummary();

            using (var workbookStream = new MemoryStream())
            {
                BuildWorkbook(sales, inventory).SaveAs(workbookStream);
                workbookStream.Position = 0;

                string subject = $"{frequency} Report - {ConfigurationManager.AppSettings["STORE_NAME"]} " +
                    $"({periodStart:MMM d} - {periodEnd:MMM d, yyyy})";
                string body = BuildEmailBody(frequency, periodStart, periodEnd, sales, inventory);

                await SendEmailAsync(recipient, subject, body, workbookStream, $"{frequency}Report_{periodEnd:yyyy-MM-dd}.xlsx");
            }

            MarkSent();
            string logText = $"{frequency} report emailed to {recipient}";
            await ActivityLog.LogAsync(logText);
            OperationLog.RecordAction(nameof(ScheduledReportService), logText);
        }

        private static bool IsDue()
        {
            DateTime? lastSent = GetLastSent();
            if (lastSent == null)
                return true;

            return DateTime.Now >= AddInterval(lastSent.Value, GetConfiguredFrequency());
        }

        private static DateTime AddInterval(DateTime date, Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.Daily: return date.AddDays(1);
                case Frequency.Monthly: return date.AddMonths(1);
                case Frequency.Quarterly: return date.AddMonths(3);
                case Frequency.Yearly: return date.AddYears(1);
                default: return date.AddDays(7);
            }
        }

        private static DateTime SubtractInterval(DateTime date, Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.Daily: return date.AddDays(-1);
                case Frequency.Monthly: return date.AddMonths(-1);
                case Frequency.Quarterly: return date.AddMonths(-3);
                case Frequency.Yearly: return date.AddYears(-1);
                default: return date.AddDays(-7);
            }
        }

        private static void MarkSent()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StateFilePath));
            File.WriteAllText(StateFilePath, DateTime.Now.ToString("o", CultureInfo.InvariantCulture));
        }

        private readonly struct SaleRecord
        {
            public SaleRecord(string transactionCode, string itemCode, string itemName, decimal price, DateTime date, string teller)
            {
                TransactionCode = transactionCode;
                ItemCode = itemCode;
                ItemName = itemName;
                Price = price;
                Date = date;
                Teller = teller;
            }

            public string TransactionCode { get; }
            public string ItemCode { get; }
            public string ItemName { get; }
            public decimal Price { get; }
            public DateTime Date { get; }
            public string Teller { get; }
        }

        private readonly struct InventorySummary
        {
            public InventorySummary(DataTable items, int outOfStockCount, decimal inventoryValue)
            {
                Items = items;
                OutOfStockCount = outOfStockCount;
                InventoryValue = inventoryValue;
            }

            public DataTable Items { get; }
            public int OutOfStockCount { get; }
            public decimal InventoryValue { get; }
        }

        // Matches ListSales.BuildDateRangeWhere: transaction_date is stored as text
        // ("yyyy-MMMM-dd H:m:ss"), so each day in range is matched with its own LIKE clause.
        private static async Task<List<SaleRecord>> FetchSalesAsync(DateTime from, DateTime to)
        {
            var results = new List<SaleRecord>();

            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            string connectionString = $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";

            var clauses = new List<string>();
            var parameters = new List<MySqlParameter>();
            int i = 0;
            for (DateTime d = from.Date; d <= to.Date && i < 366; d = d.AddDays(1), i++)
            {
                string pname = "@d" + i;
                clauses.Add($"s.transaction_date LIKE {pname}");
                parameters.Add(new MySqlParameter(pname, "%" + d.ToString("yyyy-MMMM-dd") + "%"));
            }
            string whereClause = clauses.Count > 0 ? "(" + string.Join(" OR ", clauses) + ")" : "1=1";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT s.transaction_code AS transaction_code, s.seller AS teller, s.itemcode AS itemcode, " +
                    "s.transaction_date AS transaction_date, s.item_price AS item_price, i.description AS itemname " +
                    "FROM sales_ s LEFT JOIN items_ i ON s.itemcode = i.serial " +
                    $"WHERE {whereClause} ORDER BY s.transaction_date DESC";
                cmd.Parameters.AddRange(parameters.ToArray());

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string raw = reader["transaction_date"]?.ToString();
                        if (!DateTime.TryParseExact(raw, "yyyy-MMMM-dd H:m:ss", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out DateTime parsed))
                            continue;

                        results.Add(new SaleRecord(
                            reader["transaction_code"]?.ToString(),
                            reader["itemcode"]?.ToString(),
                            reader["itemname"]?.ToString(),
                            reader["item_price"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["item_price"]),
                            parsed,
                            reader["teller"]?.ToString()));
                    }
                }
            }

            return results;
        }

        private static InventorySummary BuildInventorySummary()
        {
            DataTable items = DataCacheManager.Instance.GetTable("Items");
            int outOfStock = 0;
            decimal inventoryValue = 0m;

            foreach (DataRow row in items.Rows)
            {
                decimal qty = row["new_quantity"] == DBNull.Value ? 0m : Convert.ToDecimal(row["new_quantity"]);
                decimal price = row["price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["price"]);

                if (qty <= 0)
                    outOfStock++;
                else
                    inventoryValue += qty * price;
            }

            return new InventorySummary(items, outOfStock, inventoryValue);
        }

        private static XLWorkbook BuildWorkbook(List<SaleRecord> sales, InventorySummary inventory)
        {
            var salesTable = new DataTable("Sales");
            salesTable.Columns.Add("Transaction Code");
            salesTable.Columns.Add("Date");
            salesTable.Columns.Add("Item Code");
            salesTable.Columns.Add("Item Name");
            salesTable.Columns.Add("Teller");
            salesTable.Columns.Add("Price");
            foreach (var sale in sales)
                salesTable.Rows.Add(sale.TransactionCode, sale.Date.ToString("yyyy-MM-dd HH:mm"),
                    sale.ItemCode, sale.ItemName, sale.Teller, sale.Price);

            var inventoryTable = new DataTable("Inventory");
            inventoryTable.Columns.Add("Serial");
            inventoryTable.Columns.Add("Description");
            inventoryTable.Columns.Add("Price");
            inventoryTable.Columns.Add("Retail Price");
            inventoryTable.Columns.Add("Quantity");
            foreach (DataRow row in inventory.Items.Rows)
                inventoryTable.Rows.Add(
                    row["serial"].ToString(),
                    row["description"].ToString(),
                    row["price"] == DBNull.Value ? "" : Convert.ToDecimal(row["price"]).ToString("0.00"),
                    row["retail_price"] == DBNull.Value ? "" : Convert.ToDecimal(row["retail_price"]).ToString("0.00"),
                    row["new_quantity"].ToString());

            var wb = new XLWorkbook();
            wb.Worksheets.Add(salesTable);
            wb.Worksheets.Add(inventoryTable);
            return wb;
        }

        private static string BuildEmailBody(Frequency frequency, DateTime periodStart, DateTime periodEnd,
            List<SaleRecord> sales, InventorySummary inventory)
        {
            decimal totalRevenue = sales.Sum(s => s.Price);
            int transactionCount = sales.Select(s => s.TransactionCode).Distinct().Count();
            string storeName = ConfigurationManager.AppSettings["STORE_NAME"];

            return
                $"{frequency} report for {storeName} covering {periodStart:MMM d} - {periodEnd:MMM d, yyyy}\n\n" +
                "SALES\n" +
                $"  Transactions: {transactionCount}\n" +
                $"  Items sold: {sales.Count}\n" +
                $"  Total revenue: {totalRevenue:0.00}\n\n" +
                "INVENTORY\n" +
                $"  Total items: {inventory.Items.Rows.Count}\n" +
                $"  Out of stock: {inventory.OutOfStockCount}\n" +
                $"  Estimated inventory value: {inventory.InventoryValue:0.00}\n\n" +
                "Full sales and inventory detail is attached as an Excel workbook.";
        }

        private static async Task SendEmailAsync(string recipient, string subject, string body, MemoryStream attachmentStream, string attachmentName)
        {
            if (string.Equals(ConfigurationManager.AppSettings["MAIL_PROVIDER"], "Gmail", StringComparison.OrdinalIgnoreCase))
            {
                await GmailApiMailer.SendEmailAsync(recipient, subject, body, attachmentStream, attachmentName);
                return;
            }

            string host = ConfigurationManager.AppSettings["SMTP_HOST"];
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("SMTP_HOST is not configured.");

            int port = int.TryParse(ConfigurationManager.AppSettings["SMTP_PORT"], out int p) ? p : 587;
            bool useSsl = !bool.TryParse(ConfigurationManager.AppSettings["SMTP_USE_SSL"], out bool ssl) || ssl;
            string user = ConfigurationManager.AppSettings["SMTP_USER"];
            string password = ConfigurationManager.AppSettings["SMTP_PASSWORD"];
            string from = ConfigurationManager.AppSettings["SMTP_FROM"];
            if (string.IsNullOrWhiteSpace(from))
                from = user;

            using (var client = new SmtpClient(host, port) { EnableSsl = useSsl })
            using (var message = new MailMessage(from, recipient, subject, body))
            {
                if (!string.IsNullOrWhiteSpace(user))
                    client.Credentials = new NetworkCredential(user, password);

                using (var attachment = new Attachment(attachmentStream, attachmentName,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    message.Attachments.Add(attachment);
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
