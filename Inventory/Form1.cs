using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace _conv
{
    public partial class Purchases : Form
    {
        public long count = 0;
        public decimal grandTotal = 0;
        int total_row = 0;
        public string text_in_serial;
        public string transactioncode;
        public int purchaseState = 0;
        public static int countcells = 0;
        public static string log_in = "";
        private static string _loggedInCode;
        public DataTable dataTable = new DataTable();
        public string TransactionCode { get; private set; }
        public string L_DEFAULTNAME = ConfigurationManager.AppSettings["STORE_NAME"] ?? "Baclig Minimart";
        public string S_DEFAULTNAME = ConfigurationManager.AppSettings["STORE_CODE"] ?? "BCLGMNM";
        string dbname, dbpw, connection_timeout, dbuser, dbsrv;

        /// <summary>
        /// Every top-level menuStrip1 item that can be hidden/shown from System Config > Menus,
        /// paired with the label shown in that screen. Name must match the ToolStripMenuItem's
        /// designer Name so <see cref="ApplyMenuVisibility"/> and SystemConfigForm can look it up.
        /// </summary>
        public static readonly (string Name, string Label)[] ConfigurableMenuItems = new[]
        {
            ("salesToolStripMenuItem", "Sales"),
            ("generateToolStripMenuItem", "Generate"),
            ("transactionToolStripMenuItem", "Transaction"),
            ("reportsToolStripMenuItem", "Report"),
            ("receiptsToolStripMenuItem", "Receipts"),
            ("usersToolStripMenuItem", "Users"),
            ("loyaltyToolStripMenuItem", "Loyalty"),
            ("taxConfigToolStripMenuItem", "Tax Config"),
            ("systemConfigToolStripMenuItem", "System Config"),
            ("helpToolStripMenuItem", "Help"),
        };

        /// <summary>Default HIDDEN_MENUS value when the setting is missing from App.config - hides
        /// System Config, Loyalty, Tax Config, and Receipts from the menu bar until enabled explicitly.</summary>
        public const string DefaultHiddenMenus = "systemConfigToolStripMenuItem,loyaltyToolStripMenuItem,taxConfigToolStripMenuItem,receiptsToolStripMenuItem";

        /// <summary>
        /// Shows/hides each top-level menu per the HIDDEN_MENUS appSetting (comma-separated
        /// ToolStripMenuItem names, edited from System Config > Menus). Called on load and again
        /// after System Config closes so a change takes effect immediately, no restart needed.
        /// </summary>
        /// <summary>Pops a Windows tray balloon off notifyIcon1 - used by SystemConfigForm to
        /// surface its configuration warnings even after the window that raised them closes.</summary>
        public void ShowTrayNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Warning)
        {
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = message;
            notifyIcon1.BalloonTipIcon = icon;
            notifyIcon1.ShowBalloonTip(8000);
        }

        public void ApplyMenuVisibility()
        {
            var configurableNames = new HashSet<string>(ConfigurableMenuItems.Select(m => m.Name));

            foreach (ToolStripItem item in menuStrip1.Items)
            {
                if (configurableNames.Contains(item.Name))
                    item.Visible = !IsMenuHidden(item.Name);
            }
        }

        /// <summary>
        /// True if the given top-level menu (by ToolStripMenuItem name, e.g. "taxConfigToolStripMenuItem")
        /// is hidden per the HIDDEN_MENUS appSetting. Exposed statically so non-UI code - like
        /// <see cref="TaxConfig"/> - can key behavior off the same setting without a form reference.
        /// </summary>
        public static bool IsMenuHidden(string menuItemName)
        {
            string raw = ConfigurationManager.AppSettings["HIDDEN_MENUS"] ?? DefaultHiddenMenus;
            return raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Contains(menuItemName, StringComparer.OrdinalIgnoreCase);
        }
        private string pendingReceiptText;
        private string pendingReceiptCode;

        // Every receipt prints one of these, in order, as separate pages of the same print job.
        private static readonly string[] ReceiptCopyLabels = { "CUSTOMER'S COPY", "COUNTER'S COPY" };
        private int receiptCopyIndex;

        // True while printing just one receipt copy instead of looping through every
        // ReceiptCopyLabels entry - used when the counter's copy is being rendered to a JPEG
        // instead of the printer, so only the customer's copy goes to physical paper.
        private bool printSingleReceiptCopyOnly;

        // How many rows of dgvDashboardLowStock printDocument2_PrintPage has drawn so far -
        // reset to 0 before each print job, advanced across pages via HasMorePages.
        private int outOfStockPrintRowIndex;

        // Set from System Config > Receipt Testing's slider, live (not persisted to App.config -
        // this is a per-session testing knob, not a store setting). When set, every item on the
        // printed/previewed receipt shows this unit price instead of its real one (line total and
        // the subtotal/tax/grand total below it are recomputed from it too, so the whole receipt
        // stays internally consistent) - lets a tester see how the layout holds up with very small
        // or very large price values without editing real item data.
        internal static decimal? TestPriceOverride;

        // Y-position PrintDocument1_PrintPage's last page reached, i.e. the receipt's actual
        // content height - captured so SaveCounterReceiptAsJpeg can crop its (generously tall,
        // since receipt length varies with item count) scratch bitmap down to just the receipt.
        private float lastReceiptContentHeight;

        internal class HeldTransaction
        {
            public string TicketCode;
            public string TransactionCode;
            public int SelltypeIndex;
            public decimal GrandTotal;
            public string CounterText;
            public DateTime HeldAt;
            public List<object[]> Items;
        }
        internal readonly Dictionary<string, HeldTransaction> heldTransactions = new Dictionary<string, HeldTransaction>();
        private HeldTransactionsForm heldTransactionsWindow;

        // When a MessageBox opened from a shortcut key (e.g. F6) is dismissed with Enter,
        // Windows delivers that Enter's key-up to whatever control regains focus afterward.
        // This suppresses the one resulting KeyUp on tbx_serialcode so it isn't misread as
        // the user pressing Enter with an empty serial code.
        private bool suppressNextSerialKeyUp;

        public Purchases()
        {
            InitializeComponent();
            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);

            notifyIcon1.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            notifyIcon1.Text = "BrewCode Innovations";

            // Controls are anchored to grow with the window; below this size they'd start
            // overlapping, so don't let the window shrink past its original designed size.
            this.MinimumSize = this.Size;

            // Use the embedded copy of Morally Serif so the store name renders correctly
            // on machines where the font isn't installed. GDI text rendering can't see
            // memory fonts, so this label must use GDI+ (compatible text rendering).
            label6.UseCompatibleTextRendering = true;
            label6.Font = new Font(AppFonts.MorallySerif, 30F, FontStyle.Bold);

            ApplyStoreIdentity();

            SetCueBanner(tbx_search, ItemSearchPlaceholder);
        }

        // Shown as a native cue banner (grayed hint text that the Windows edit control itself
        // hides once real text is typed or the box is focused) rather than being written into
        // tbx_search.Text - a real value there used to get treated as an actual search query.
        private const string ItemSearchPlaceholder = "Scan or type the item or code here...";

        private static class NativeMethods
        {
            private const int EM_SETCUEBANNER = 0x1501;

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

            public static void SetCueBanner(TextBox textBox, string text) =>
                SendMessage(textBox.Handle, EM_SETCUEBANNER, IntPtr.Zero, text);
        }

        private static void SetCueBanner(TextBox textBox, string text) => NativeMethods.SetCueBanner(textBox, text);

        /// <summary>
        /// Re-reads STORE_NAME/STORE_CODE from App.config and pushes them into every place this
        /// running instance already rendered them, so a change made in System Config takes effect
        /// immediately - no app restart needed. Call again after System Config closes.
        /// </summary>
        public void ApplyStoreIdentity()
        {
            L_DEFAULTNAME = ConfigurationManager.AppSettings["STORE_NAME"] ?? "Baclig Minimart";
            S_DEFAULTNAME = ConfigurationManager.AppSettings["STORE_CODE"] ?? "BCLGMNM";
            label6.Text = L_DEFAULTNAME;
        }
        public MySqlConnection connect_db;

        private static DataTable ItemsTable => DataCacheManager.Instance.GetTable("Items");

        private static void RefreshItemsCache() =>
            DataCacheManager.Instance.RefreshTable("Items", "SELECT * FROM items_");

        // Serial -> row index for ItemsTable, rebuilt lazily whenever the underlying DataTable
        // instance changes (e.g. after RefreshItemsCache swaps in a freshly loaded table). Every
        // barcode scan looks an item up by serial at least once (often 2-3 times across
        // getItem/GetItemPrice/Isolated/etc.), so this turns that from an O(n) table scan per
        // lookup into an O(1) dictionary hit - the difference is most noticeable at the register
        // on a catalog with a few thousand items.
        private static DataTable _itemsIndexSource;
        private static Dictionary<string, DataRow> _itemsBySerial;

        private static DataRow FindItemRow(string serial)
        {
            var table = ItemsTable;
            if (!ReferenceEquals(_itemsIndexSource, table))
            {
                var index = new Dictionary<string, DataRow>(table.Rows.Count, StringComparer.Ordinal);
                foreach (DataRow row in table.Rows)
                {
                    string key = row.Field<string>("serial");
                    // Same semantics as AsEnumerable().FirstOrDefault(): first match wins.
                    if (key != null && !index.ContainsKey(key))
                        index[key] = row;
                }
                _itemsBySerial = index;
                _itemsIndexSource = table;
            }

            return serial != null && _itemsBySerial.TryGetValue(serial, out var found) ? found : null;
        }

        private System.Windows.Forms.Timer cacheReconnectTimer;
        private const int CacheReconnectPollMs = 60000;

        /// <summary>
        /// Watches for the DB connection coming back after an outage and re-pulls Items
        /// (prices, stock, descriptions) so the cache stops serving the stale offline
        /// snapshot. NetworkChange fires fast when the adapter comes up, but adapter-up
        /// doesn't guarantee the DB is actually reachable, so a periodic poll backs it up.
        /// </summary>
        private void StartCacheReconnectWatcher()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

            cacheReconnectTimer = new System.Windows.Forms.Timer { Interval = CacheReconnectPollMs };
            cacheReconnectTimer.Tick += (s, e) => TryRefreshCacheIfReconnected();
            cacheReconnectTimer.Start();

            this.FormClosed += (s, e) =>
            {
                NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
                cacheReconnectTimer.Stop();
            };
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable)
                return;

            // Fires on a thread-pool thread - hop back to the UI thread before touching the
            // shared Items DataTable, since every other reader/writer assumes single-threaded access.
            if (this.IsHandleCreated)
                this.BeginInvoke((Action)TryRefreshCacheIfReconnected);
        }

        /// <summary>
        /// Only re-pulls Items from the database when the cache is currently marked offline -
        /// once a live pull succeeds, DataCacheManager clears that flag, so this is a no-op
        /// until the connection actually drops again.
        /// </summary>
        private static void TryRefreshCacheIfReconnected()
        {
            if (DataCacheManager.Instance.IsOfflineMode)
                RefreshItemsCache();
        }

        private System.Windows.Forms.Timer lastSeenHeartbeatTimer;
        private const int LastSeenHeartbeatMs = 5 * 60 * 1000;

        /// <summary>
        /// Keeps user_.last_seen_utc reflecting that this teller is actively online, not just
        /// the moment they logged in - re-stamps it on a timer for as long as the register stays
        /// open. Skipped while offline, matching the app's degrade-gracefully-never-crash pattern;
        /// the next tick after reconnecting catches it back up.
        /// </summary>
        private void StartLastSeenHeartbeat()
        {
            lastSeenHeartbeatTimer = new System.Windows.Forms.Timer { Interval = LastSeenHeartbeatMs };
            lastSeenHeartbeatTimer.Tick += async (s, e) => await StampLastSeenIfOnlineAsync();
            lastSeenHeartbeatTimer.Start();

            this.FormClosed += (s, e) => lastSeenHeartbeatTimer.Stop();
        }

        private System.Windows.Forms.Timer maintenanceCheckTimer;
        private const int MaintenanceCheckPollMs = 2 * 60 * 1000;
        private bool _maintenanceOverridden;

        /// <summary>
        /// Polls setting_'s MAINTENANCE_MODE flag (via AppConfigService, which is a live DB
        /// read, not the startup-only DataCacheManager snapshot) so a store-wide maintenance
        /// toggle flipped on from another terminal's System Config reaches this one without a
        /// restart - not just the startup gate in Program.cs. Skipped for the rest of this
        /// session once an admin has already overridden it once, so the same terminal isn't
        /// re-prompted every poll.
        /// </summary>
        private void StartMaintenanceWatcher()
        {
            maintenanceCheckTimer = new System.Windows.Forms.Timer { Interval = MaintenanceCheckPollMs };
            maintenanceCheckTimer.Tick += async (s, e) => await CheckMaintenanceAsync();
            maintenanceCheckTimer.Start();

            this.FormClosed += (s, e) => maintenanceCheckTimer.Stop();
        }

        private async Task CheckMaintenanceAsync()
        {
            if (_maintenanceOverridden)
                return;

            (bool enabled, string message) = await AppConfigService.GetMaintenanceStatusAsync();
            if (!enabled)
                return;

            maintenanceCheckTimer.Stop();

            using (var maintenanceForm = new MaintenanceForm(message))
            {
                if (maintenanceForm.ShowDialog(this) == DialogResult.OK)
                {
                    _maintenanceOverridden = true;
                    maintenanceCheckTimer.Start();
                    return;
                }
            }

            MessageBox.Show(
                "Maintenance mode is now active and no admin override was entered. The application will now close.",
                "Maintenance Mode", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.Close();
        }

        private async Task StampLastSeenIfOnlineAsync()
        {
            if (string.IsNullOrEmpty(_loggedInCode) || DataCacheManager.Instance.IsOfflineMode)
                return;

            try
            {
                string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
                string dbname = ConfigurationManager.AppSettings["DB_NAME"];
                string dbuser = ConfigurationManager.AppSettings["DB_USER"];
                string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
                string connectionTimeout = ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"];

                using (var conn = new MySqlConnection($"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};Connect Timeout={connectionTimeout};"))
                using (var cmd = new MySqlCommand("UPDATE user_ SET last_seen_utc=@last_seen_utc WHERE name=@name", conn))
                {
                    cmd.Parameters.AddWithValue("@last_seen_utc", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@name", _loggedInCode);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch
            {
                // Offline or otherwise unreachable right now - the next heartbeat tick retries.
            }
        }

        /// <summary>
        /// Builds the running-total text for the cart display. Only shows the Subtotal/Tax
        /// breakdown when a tax rate is actually configured, so the register looks unchanged
        /// for stores that haven't set one up via Tax Config.
        /// </summary>
        private static string BuildCounterText(decimal subtotal, string itemsLine)
        {
            decimal tax = TaxConfig.ComputeTax(subtotal);
            decimal total = subtotal + tax;

            return TaxConfig.EffectiveTaxRatePercent > 0
                ? $"Subtotal: {subtotal:0.00}\nTax ({TaxConfig.EffectiveTaxRatePercent:0.##}%): {tax:0.00}\nGrand Total: {total:0.00}\n{itemsLine}"
                : $"Grand Total: {total:0.00}\n{itemsLine}";
        }

        /// <summary>
        /// Applies a stock change directly to the in-memory items reference (the same
        /// DataTable every lookup/search/price check reads from). Used when the database
        /// can't be reached so the app's "latest reference" for a serial's quantity still
        /// reflects what just happened at the register, instead of going stale.
        /// </summary>
        private static void AdjustCachedQuantity(string serial, long delta)
        {
            var row = FindItemRow(serial);
            if (row == null)
                return;

            row["new_quantity"] = ToLong(row["new_quantity"]) + delta;
        }

        /// <summary>Same as <see cref="AdjustCachedQuantity"/> but sets an absolute value instead of a delta.</summary>
        private static void SetCachedQuantity(string serial, long value)
        {
            var row = FindItemRow(serial);
            if (row == null)
                return;

            row["new_quantity"] = value;
        }

        /// <summary>
        /// Durable, human-readable record of stock changes applied only to the offline
        /// reference (i.e. the database write failed). Lets whoever manages the DB
        /// reconcile it manually once the connection is back. Entries are grouped into
        /// OfflineCache\yyyy-MM\ subfolders by month.
        /// </summary>
        private static void LogPendingStockAdjustment(string serial, long delta, string reason)
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "App Inventory", "OfflineCache", DateTime.Now.ToString("yyyy-MM"));
                Directory.CreateDirectory(dir);
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | serial={serial} | delta={delta:+#;-#;0} | reason={reason}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(dir, "PendingStockAdjustments.log"), line);
            }
            catch
            {
                // Best-effort audit trail only - never let this block the sale itself.
            }
        }

        /// <summary>
        /// Records every line item from a completed checkout into sales_ (one row per line,
        /// itemcode + line total, tagged with a per-row transaction code derived from the
        /// receipt's transaction code) so sales history/reporting reflects what was actually
        /// punched at the register. Falls back to a durable local log - never blocks checkout -
        /// if the DB can't be reached, matching this app's offline-resilience pattern.
        /// </summary>
        private async Task SaveSaleLineItemsAsync(string transactionCode, List<(string Serial, int Quantity, decimal ItemPrice)> lineItems)
        {
            if (string.IsNullOrEmpty(transactionCode) || lineItems == null || lineItems.Count == 0)
                return;

            try
            {
                await ConnectDb();
                if (connect_db == null || connect_db.State != ConnectionState.Open)
                    throw new InvalidOperationException("No database connection.");

                int rowNumber = 1;
                foreach (var item in lineItems)
                {
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO sales_(transaction_code, itemcode, transaction_date, seller, item_price) " +
                        "VALUES (@transaction_code, @itemcode, @transaction_date, @seller, @item_price)", connect_db))
                    {
                        cmd.Parameters.AddWithValue("@transaction_code", $"{transactionCode}-{rowNumber}");
                        cmd.Parameters.AddWithValue("@itemcode", item.Serial);
                        cmd.Parameters.AddWithValue("@transaction_date", DateTime.Now.ToString("yyyy-MMMM-dd H:m:ss"));
                        cmd.Parameters.AddWithValue("@seller", log_in);
                        cmd.Parameters.AddWithValue("@item_price", item.ItemPrice);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    rowNumber++;
                }

                connect_db.Close();
            }
            catch (Exception)
            {
                LogPendingSale(transactionCode, lineItems);
                lbl_warning.Text = "OFFLINE - sale completed but not recorded to sales history; logged for manual reconciliation.";
            }
        }

        /// <summary>
        /// Durable, human-readable record of a checkout that couldn't be written to sales_
        /// (DB unreachable). Mirrors <see cref="LogPendingStockAdjustment"/> so whoever manages
        /// the DB can reconcile both offline-sale artifacts from the same OfflineCache folder.
        /// </summary>
        private static void LogPendingSale(string transactionCode, List<(string Serial, int Quantity, decimal ItemPrice)> lineItems)
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "App Inventory", "OfflineCache", DateTime.Now.ToString("yyyy-MM"));
                Directory.CreateDirectory(dir);
                var sb = new StringBuilder();
                foreach (var item in lineItems)
                {
                    sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | transaction_code={transactionCode} | itemcode={item.Serial} | quantity={item.Quantity} | item_price={item.ItemPrice} | seller={log_in}");
                }
                File.AppendAllText(Path.Combine(dir, "PendingSalesLogs.log"), sb.ToString());
            }
            catch
            {
                // Best-effort audit trail only - never let this block the sale itself.
            }
        }

        private static bool Contains(object field, string search) =>
            field.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

        private static int ToInt(object value) =>
            value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);

        private static long ToLong(object value) =>
            value == null || value == DBNull.Value ? 0L : Convert.ToInt64(value);

        private static decimal ToDecimal(object value) =>
            value == null || value == DBNull.Value ? 0m : Convert.ToDecimal(value);

        private static DateTime ToDate(object value) =>
            value == null || value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value);

        public async Task ConnectDb()
        {
            dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            dbname = ConfigurationManager.AppSettings["DB_NAME"];
            dbuser = ConfigurationManager.AppSettings["DB_USER"];
            dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            connection_timeout = ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"];


            try
            {
                connect_db = new MySqlConnection($"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};Connect Timeout={connection_timeout};");

                // OpenAsync (not Open) so a dead connection - e.g. while the cashier is just
                // typing a search - fails on a background thread instead of freezing the UI
                // thread for the whole connect timeout on every keystroke.
                await connect_db.OpenAsync();
            }
            catch (Exception)
            {
                // No network/DB right now - leave connect_db unusable and return quietly.
                // Callers already fall back to the cached/offline item reference instead of
                // blocking on a dialog, so a single failed connection attempt (e.g. while the
                // cashier is just typing a search) shouldn't interrupt what they're doing.
            }
            return;
        }
        public String[] getItem(String itemcode)
        {
            var row = FindItemRow(itemcode);



            String item_description = row != null ? row["description"].ToString() : "";

            return item_description.Split(',');
        }
        public int GetItemPrice(String itemcode)
        {
            int price_value = 0;

            var row = FindItemRow(itemcode);
            if (row == null)
                return price_value;

            if (selltype.SelectedIndex == 0)
            {
                price_value = ToInt(row["price"]);
            }
            else if (selltype.SelectedIndex == 1)
            {
                price_value = ToInt(row["retail_price"]);
            }
            else if (selltype.SelectedIndex == 2)
            {
                price_value = ToInt(row["price_perpc"]);
            }
            return price_value;
        }
        private void UpdateTime()
        {
            lbl_warning.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");

            lbl_warning.Font = new System.Drawing.Font("Arial", 15, System.Drawing.FontStyle.Italic);
        }
        private async void Purchases_Load(object sender, EventArgs e)
        {

            try
            {
                ApplyMenuVisibility();

                this.Text = $"POSXX1 - {S_DEFAULTNAME}::{await netTesting()}";

                if (lastPingRoundtripMs == null)
                {
                    MessageBox.Show(
                        "Cannot reach the database server. Check your internet connection before proceeding - login, scanning, and payments will likely fail.",
                        "Network Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (lastPingRoundtripMs > SlowPingThresholdMs)
                {
                    MessageBox.Show(
                        $"Network connection to the database is slow ({lastPingRoundtripMs}ms). Item lookups and checkout may lag.",
                        "Network Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                Login login = new Login();

                this.WindowState = FormWindowState.Maximized;
                this.Enabled = false;

                login.ShowDialog(this);

                if (!login.IsAuthenticated)
                {
                    MessageBox.Show("Login failed or cancelled. The application will now close.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                this.Enabled = true;

                if (TrialLicense.IsEnabled && !TrialLicense.IsLicensed)
                {
                    this.Text += $" [TRIAL - {TrialLicense.DaysRemaining} day(s) left]";
                    ShowTrayNotification("Trial Mode",
                        $"{TrialLicense.DaysRemaining} day(s) remaining in your trial. Enter a license key in System Config > Trial / Licensing to activate.");
                }

                if (!string.IsNullOrEmpty(selltype.Text))
                {
                    tbx_serialcode.Enabled = true;

                    tbx_serialcode.Focus();
                }

                log_in = login.id;
                _loggedInCode = login.name;

                teller.Text = $"Teller ID: {login.teller_id(log_in).Replace(login.teller_id(log_in), "******")}";

                dgv_items.ReadOnly = true;

                timer1 = new System.Windows.Forms.Timer();

                timer1.Interval = 1000;

                timer1.Tick += timer1_Tick;


                gbx_itemsearch.Enabled = true;

                UpdateTime();

                timer1.Start();

                StartCacheReconnectWatcher();
                StartLastSeenHeartbeat();
                StartMaintenanceWatcher();

                this.Width = 1000;

                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                dgv_itemSearch.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

                dgv_itemSearch.Columns.Add("serial", "Serial");
                dgv_itemSearch.Columns.Add("description", "Particular");
                dgv_itemSearch.Columns.Add("price", "Price");

                dataTable.Columns.Add("serial");
                dataTable.Columns.Add("description");
                dataTable.Columns.Add("price");

                dgv_itemSearch.Columns[2].DefaultCellStyle.Format = "0.00";
                dgv_itemSearch.Columns[0].Width = 100;
                dgv_itemSearch.Columns[1].Width = 200;

                this.Show();

                _ = LoadReceiptLogoAsync();

                dgv_items.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
                dgv_items.Columns.Add("serial", "Serial");
                dgv_items.Columns.Add("description", "Particular");
                dgv_items.Columns.Add("quantity", "Qty");
                dgv_items.Columns.Add("uprice", "Unit Price");
                dgv_items.Columns.Add("price", "Price");
                dgv_items.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv_items.Columns[0].FillWeight = 100; // Serial
                dgv_items.Columns[1].FillWeight = 100; // Particular
                dgv_items.Columns[2].FillWeight = 50;  // Qty
                dgv_items.Columns[3].FillWeight = 50;  // Unit Price
                dgv_items.Columns[4].FillWeight = 100; // Price
                dgv_items.Columns[3].DefaultCellStyle.Format = "0.00"; // Unit Price
                dgv_items.Columns[4].DefaultCellStyle.Format = "0.00"; // Price

                ContextMenuStrip strip = new ContextMenuStrip();
                ToolStripMenuItem SaveToPNG = new ToolStripMenuItem("Save As PNG");
                ToolStripMenuItem Print = new ToolStripMenuItem("Print");
                SaveToPNG.Click += SaveToPNG_Click;
                Print.Click += Print_Click;
                strip.Items.Add(SaveToPNG);
                strip.Items.Add(Print);
                dgv_itemSearch.ContextMenuStrip = strip;

                tbx_search.Enabled = false;

                tbx_serialcode.Enabled = true;

                tbx_serialcode.Focus();

                if (!TutorialState.HasBeenShown)
                {
                    using (var tutorial = new TutorialForm())
                        tutorial.ShowDialog(this);

                    TutorialState.MarkShown();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database refused!");
                this.Close();
            }

        }
        private void SaveToPNG_Click(object sender, EventArgs e)
        {
            var row = dgv_itemSearch.CurrentRow;
            if (row == null || row.Cells[0].Value == null)
            {
                MessageBox.Show("Select an item first.", "Save Barcode");
                return;
            }

            string serial = row.Cells[0].Value.ToString();

            string downloadsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            Directory.CreateDirectory(downloadsFolder);
            string filePath = Path.Combine(downloadsFolder, serial + ".png");

            string url = "https://quickchart.io/barcode?" +
                "type=code128" +
                "&width=150" +
                "&height=50" +
                "&includeText=true" +
                $"&text={Uri.EscapeDataString(serial)}";

            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] imageBytes = client.DownloadData(url);
                    File.WriteAllBytes(filePath, imageBytes);
                }

                MessageBox.Show("Barcode saved to " + filePath, "Save Barcode");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save barcode: " + ex.Message, "Save Barcode");
            }
        }
        public static Task<List<string>> StoredData()
        {
            List<string> data = new List<string>();

            return Task.FromResult(data);
        }
        private void Print_Click(object sender, EventArgs e)
        {
            GenerateBarcode generateBarcodeForm = new GenerateBarcode();
            generateBarcodeForm.code = dgv_itemSearch.CurrentRow.Cells[0].Value.ToString();
            generateBarcodeForm.Show();
        }
        private void textchanged_tbx_serialcode(object sender, EventArgs e)
        {

        }

        private void tbx_serialcode_KeyUp(object sender, KeyEventArgs e)
        {
            if (suppressNextSerialKeyUp)
            {
                suppressNextSerialKeyUp = false;
                return;
            }

            try
            {

                if (e.KeyCode == Keys.F4)
                {
                    tbx_search.Clear();
                    tbx_search.Focus();
                }
                else if (e.KeyCode == Keys.F12)
                {
                    dgv_items.Focus();
                }
                else if (e.KeyCode == Keys.F5)
                {
                    MessageBox.Show("Application will restart", "Restarting...");

                    RefreshItemsCache();

                    dgv_itemSearch.Rows.Clear();

                    int count = 0;

                    foreach (DataRow row in ItemsTable.Rows)
                    {
                        count += 1;
                        if (!string.IsNullOrEmpty(row["serial"].ToString()))
                            dgv_itemSearch.Rows.Add(row["serial"].ToString(),
                            row["description"].ToString(),
                            ToDecimal(row["price"]));
                    }

                    Application.Restart();

                    MessageBox.Show("Application successfully restarted", "Successful restart!");

                    this.CenterToScreen();

                }
                else if (e.KeyValue == (char)Keys.F2)
                {
                    //dgv_itemSearch.Rows.Clear();

                    selltype.SelectedIndex = 0;

                    groupBox1.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList(); //Wholesale

                }
                else if (e.KeyValue == (char)Keys.F1)
                {
                    selltype.SelectedIndex = 2;

                    //dgv_itemSearch.Rows.Clear();

                    groupBox1.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList(); // By-piece
                }
                else if (e.KeyValue == (char)Keys.F3)
                {
                    selltype.SelectedIndex = 1;

                    //dgv_itemSearch.Rows.Clear();

                    groupBox1.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList(); //Retail
                }
                else if (e.KeyValue == (char)Keys.F10)
                {
                    using (var pettyCashEntry = new PettyCashEntryForm())
                    {
                        pettyCashEntry.ShowDialog(this);
                    }
                }
                else if (e.KeyValue == (char)Keys.F9)
                {
                    tbx_denomination.Clear();
                    tbx_denomination.Enabled = true;
                    tbx_denomination.Focus();

                }
                else if (e.KeyCode == Keys.F7)
                {
                    if (!string.IsNullOrEmpty(tbx_serialcode.Text))
                    {
                        dgv_items.Rows.Add(tbx_serialcode.Text, getItem(tbx_serialcode.Text), textBox1.Text, GetItemPrice(tbx_serialcode.Text));
                        text_in_serial = tbx_serialcode.Text;
                        tbx_serialcode.Clear();

                    }
                    else
                    {
                        suppressNextSerialKeyUp = true;
                        msgbox("Please enter a valid serial code.");
                    }


                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(tbx_serialcode.Text))
                    {
                        // Scanner sends the barcode digits then Enter - this is the actual
                        // per-scan hot path. Look the item up once and reuse it, instead of the
                        // 2-3 separate FindItemRow lookups the two branches used to do.
                        string scannedCode = tbx_serialcode.Text;
                        string itemName = getItem(scannedCode)[0].ToUpperInvariant();
                        int itemPrice = GetItemPrice(scannedCode);

                        if (scannedCode == "01282026-085944")
                        {
                            dgv_items.Rows.Add(
                                    scannedCode,
                                    itemName,
                                    textBox1.Text,
                                    Isolated(scannedCode),
                                    Convert.ToInt64(itemPrice) * Convert.ToInt64(textBox1.Text)
                                );
                        }
                        else
                        {
                            dgv_items.Rows.Add(
                                    scannedCode,
                                    itemName,
                                    textBox1.Text,
                                    itemPrice,
                                    Convert.ToInt64(itemPrice) * Convert.ToInt64(textBox1.Text)
                                );
                        }
                        text_in_serial = scannedCode;
                        tbx_serialcode.Clear();
                        tbx_serialcode.Focus();
                    }
                    else
                    {
                        suppressNextSerialKeyUp = true;
                        msgbox("Please enter a valid serial code.");
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    selltype.SelectedIndex = 3;
                    //if (MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    //{
                    //    return;
                    //}
                    //MessageBox.Show("Application will now close.", "Exit Application");
                    //this.Close();
                    //Application.Exit();
                }
            }
            catch (Exception exs) { MessageBox.Show($"Press OK to continue \n {exs.Message}"); }
        }


        private void Purchases_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void tbx_search_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                int GET_CURRENT_SELLING_TYPE = selltype.SelectedIndex;
                if (e.KeyCode == Keys.Escape)
                {
                    tbx_search.Clear();
                    tbx_serialcode.Enabled = true;
                    tbx_serialcode.Focus();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    string search = tbx_search.Text ?? "";

                    if (selltype.SelectedIndex == 0)
                    {
                        var rows = ItemsTable.AsEnumerable()
                            .Where(r => (ToInt(r["price"]) != 0 && Contains(r["description"], search)) || Contains(r["serial"], search))
                            .OrderByDescending(r => ToDate(r["date_of_entry"]))
                            .Take(10);

                        dgv_itemSearch.Rows.Clear();

                        foreach (var r in rows)
                        {
                            if (!string.IsNullOrEmpty(r["serial"].ToString()))
                            {
                                if (!string.IsNullOrEmpty(r["price"].ToString()))
                                    dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price"]));
                            }
                        }
                    }
                    else if (selltype.SelectedIndex == 1)
                    {
                        var rows = ItemsTable.AsEnumerable()
                            .Where(r => (ToInt(r["retail_price"]) != 0 && Contains(r["description"], search)) || Contains(r["serial"], search))
                            .OrderByDescending(r => ToDate(r["date_of_entry"]))
                            .Take(10);

                        dgv_itemSearch.Rows.Clear();

                        foreach (var r in rows)
                        {
                            if (!string.IsNullOrEmpty(r["serial"].ToString()))
                            {
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]));
                            }
                        }
                    }
                    else if (selltype.SelectedIndex == 2)
                    {
                        var rows = ItemsTable.AsEnumerable()
                            .Where(r => Contains(r["description"], search) || (Contains(r["serial"], search) && ToInt(r["price_perpc"]) != 0))
                            .OrderByDescending(r => ToDate(r["date_of_entry"]))
                            .Take(10);

                        dgv_itemSearch.Rows.Clear();

                        foreach (var r in rows)
                        {
                            if (!string.IsNullOrEmpty(r["serial"].ToString()))
                            {
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]));
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    dgv_itemSearch.Focus();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        /*
         * Add a loading station for mobile phones
         * Add a log file that will be contained by a folder which is named by month
         * Reduce the message alerts when a single purchasing transaction is done.
         * When the internet has restored, make sure to update the prices and item details stored in the cache memory
         * 
         
         */
        //qtyState = 1, if selling per piece, 2, if selling bulk
        /// <summary>
        /// Applies one line item's quantity change on <paramref name="conn"/> (shared/reused by
        /// the caller across a whole checkout, instead of this method opening its own connection
        /// per line item). Returns true if the DB write succeeded, so the caller knows whether a
        /// cache refresh is worthwhile afterward. Falls back to a cache-only adjustment on failure,
        /// exactly as before - this is the deliberate per-row offline-degradation path, so each
        /// line item still succeeds or degrades independently of the others.
        /// </summary>
        public async Task<bool> inventory_update(MySqlConnection conn, string serial, int quantity, int qtyState = 1)
        {
            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    if (purchaseState == 2)
                    {
                        cmd.CommandText = $"UPDATE items_ SET new_quantity = new_quantity - @quantity WHERE serial = @serial";

                        cmd.Parameters.AddWithValue("@quantity", quantity);

                        cmd.Parameters.AddWithValue("@serial", serial);
                    }
                    else if (purchaseState == 1)
                    {
                        cmd.CommandText = $"UPDATE items_ SET new_quantity = qty_inpc - @quantity WHERE serial = @serial";

                        cmd.Parameters.AddWithValue("@quantity", quantity);

                        cmd.Parameters.AddWithValue("@serial", serial);
                    }
                    await cmd.ExecuteNonQueryAsync();
                }
                return true;
            }
            catch (Exception)
            {
                // Offline (or the DB write otherwise failed): the sale still has to go through
                // at the register, so apply the same quantity change to the cached reference
                // directly and record it for manual reconciliation once back online.
                if (purchaseState == 2)
                {
                    AdjustCachedQuantity(serial, -quantity);
                    LogPendingStockAdjustment(serial, -quantity, "sale (bulk) while offline");
                }
                else if (purchaseState == 1)
                {
                    var row = FindItemRow(serial);
                    long qtyInPc = row != null ? ToLong(row["qty_inpc"]) : 0;
                    long newQuantity = qtyInPc - quantity;
                    SetCachedQuantity(serial, newQuantity);
                    LogPendingStockAdjustment(serial, newQuantity, "sale (per piece) while offline - new_quantity set to qty_inpc - quantity");
                }

                lbl_warning.Text = "OFFLINE - sale recorded against the last saved stock reference only.";
                return false;
            }
        }
        private void dgv_itemSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {


                if (e.KeyCode == Keys.F7)
                {

                    tbx_serialcode.Text = dgv_itemSearch.CurrentRow.Cells[0].Value.ToString();

                    //CreateTransactionCode(tbx_serialcode.Text);

                    if (tbx_serialcode.Text == "01282026-085944")
                    {
                        int quantity_exception = Convert.ToInt32(textBox1.Text);

                        dgv_items.Rows.Add(
                            tbx_serialcode.Text,
                            getItem(tbx_serialcode.Text)[0].ToUpperInvariant(),
                            textBox1.Text,
                            Isolated(tbx_serialcode.Text, quantity_exception),
                            Convert.ToInt64(Isolated(tbx_serialcode.Text, quantity_exception)) * Convert.ToInt64(textBox1.Text)
                            );
                    }
                    else
                    {
                        dgv_items.Rows.Add(
                            tbx_serialcode.Text,
                            getItem(tbx_serialcode.Text)[0].ToUpperInvariant(),
                            textBox1.Text,
                            GetItemPrice(tbx_serialcode.Text),
                            Convert.ToInt64(GetItemPrice(tbx_serialcode.Text)) * Convert.ToInt32(textBox1.Text)
                            );
                    }
                    tbx_serialcode.Clear();

                    tbx_serialcode.Focus();

                }
                else if (e.KeyCode == Keys.Escape)
                {
                    tbx_serialcode.Focus();
                }
                else if (e.KeyCode == Keys.F1)
                {
                    selltype.SelectedIndex = 2; // By-piece

                    groupBox1.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList();
                }
                else if (e.KeyCode == Keys.F2)
                {
                    selltype.SelectedIndex = 0; // Wholesale

                    groupBox1.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList();
                }
                else if (e.KeyCode == Keys.F3)
                {
                    selltype.SelectedIndex = 1; // Retail

                    groupBox1.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList();
                }
            }
            catch (Exception xe)
            {
                MessageBox.Show(xe.Message, "Error occurred while processing the data.");
            }

        }

        private void btn_searchitem_Click(object sender, EventArgs e)
        {

        }

        private void dgv_itemSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                lbl_warning.Text = "Item " + dgv_itemSearch.CurrentCell + "added to the list.";
            }
        }

        private void tbx_search_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void chkbx_list_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Purchases_Activated(object sender, EventArgs e)
        {

        }

        private void dgv_itemSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public int PurchaseState(int qtyState)
        {
            return purchaseState = qtyState;
        }
        private void dgv_items_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                // grandTotal/count are only zeroed here where the grid is actually being
                // cleared or fully recomputed (Escape/Backspace) - NOT unconditionally for
                // every keystroke. dgv_items_RowsAdded now updates those fields incrementally
                // rather than always rebuilding from scratch, so zeroing them for keys that
                // don't recompute afterward (F12/F11/F4/F9) would silently corrupt the running
                // total the next time an item is scanned.
                if (e.KeyValue == (char)Keys.Escape)
                {
                    dgv_items.Rows.Clear();
                    grandTotal = 0;
                    count = 0;

                    tbx_serialcode.Focus();
                }
                else if (e.KeyValue == (char)Keys.Back)
                {

                    dgv_items.Rows.RemoveAt(dgv_items.CurrentRow.Index);

                    count = 0;
                    grandTotal = 0;
                    for (int i = 0; i < (dgv_items.Rows.Count - 1); i++)
                    {
                        count += Convert.ToInt32(dgv_items.Rows[i].Cells[2].Value);

                        grandTotal += Convert.ToInt32(dgv_items.Rows[i].Cells[4].Value);
                    }

                    counter.Text = BuildCounterText(grandTotal, $"Total Items: {count}");

                    tbx_serialcode.Focus();
                }
                else if (e.KeyValue == (char)Keys.F12)
                {
                    label9.Text = "***";

                    PurchaseState(2);

                    tbx_serialcode.Text = dgv_items.CurrentRow.Cells[0].Value.ToString();

                    tbx_serialcode.ReadOnly = true;

                    tbx_serialcode.Enabled = false;

                    textBox1.Enabled = true;

                    textBox1.Focus();

                    textBox1.TextAlign = HorizontalAlignment.Center;
                }
                else if (e.KeyValue == (char)Keys.F11)
                {
                    tbx_serialcode.Text = dgv_items.CurrentRow.Cells[0].Value.ToString();

                    dgv_items.CurrentRow.Cells[3].Value = Convert.ToInt64(Itemprice_InPcs(tbx_serialcode.Text));

                    dgv_items.CurrentRow.Cells[4].Value = Convert.ToInt32(dgv_items.CurrentRow.Cells[3].Value) * Convert.ToInt64(dgv_items.CurrentRow.Cells[2].Value);

                    PurchaseState(1);

                    tbx_serialcode.ReadOnly = true;

                    tbx_serialcode.Enabled = false;

                    label9.Text = "Quantity (in pieces activated)";

                    label9.Font = new Font("Arial", 7, FontStyle.Bold);

                    label9.ForeColor = Color.Red;

                    textBox1.Enabled = true;

                    textBox1.Focus();

                    textBox1.TextAlign = HorizontalAlignment.Center;
                }
                else if (e.KeyCode == Keys.F4)
                {
                    tbx_serialcode.Focus();
                }
                else if (e.KeyCode == Keys.F9)
                {
                    tbx_denomination.Enabled = true;

                    tbx_denomination.Focus();
                }
            }
            catch (Exception)
            {
                MessageBox.Show(text: "Press Ok to continue!");
            }
        }

        private void dgv_items_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_items_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                // Fires on every single scan, so this has to stay O(1) per add. Only fold the
                // newly-inserted row(s) into the running totals instead of re-summing the whole
                // cart from scratch every time (that was O(n) per add, i.e. O(n^2) building a
                // full cart) - relies on grandTotal/count being correctly zeroed everywhere the
                // grid is cleared (see the Clear() call sites) rather than self-healing here.
                // AllowUserToAddRows keeps one blank template row at the end; never sum that one.
                int lastDataRowIndex = dgv_items.Rows.Count - 2;
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount && i <= lastDataRowIndex; i++)
                {
                    grandTotal += Convert.ToDecimal(dgv_items.Rows[i].Cells[4].Value);
                    count += Convert.ToInt32(dgv_items.Rows[i].Cells[2].Value);
                }

                total_row = dgv_items.Rows.Count - 1;

                if (dgv_items.Rows.Count > 1)
                    counter.Text = BuildCounterText(grandTotal, $"Total Items: {count}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string Logs(string eventname)
        {
            DataTable dt = new DataTable();
            dt.TableName = eventname;
            dt.Columns.Add("date");
            dt.Columns.Add("event");
            dt.Rows.Add(DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"), eventname);
            return eventname;
        }

        private void dgv_items_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            tbx_serialcode.SelectAll();
        }

        private void dgv_items_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            // Removal is rare/manual (not the per-scan hot path), so a full recompute here is
            // fine - the removed row's own values aren't available from the event args, so an
            // incremental subtract isn't possible anyway. This is the single place that keeps
            // grandTotal/count correct for every way a row can disappear (the native Delete key
            // via AllowUserToDeleteRows, in particular, previously updated neither field at all -
            // this bug used to be masked by RowsAdded always rebuilding from scratch on the next
            // scan; it no longer does, so this handler has to actually run instead of quietly
            // never firing). Note this previously wrote into a local that shadowed the class
            // field, so it never actually updated grandTotal despite looking like it did.
            grandTotal = 0;
            count = 0;

            for (int i = 0; i < dgv_items.Rows.Count - 1; i++)
            {
                grandTotal += Convert.ToDecimal(dgv_items.Rows[i].Cells[4].Value);
                count += Convert.ToInt32(dgv_items.Rows[i].Cells[2].Value);
            }

            counter.Text = BuildCounterText(grandTotal, $"Total Items: {count}");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }
        private async void tbx_denomination_KeyUp(object sender, KeyEventArgs e)
        {
            string rows = "";

            int qty = 0;

            try
            {
                //(Convert.ToInt32(tbx_denomination.Text) - grandTotal).ToString();
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(tbx_denomination.Text))
                    {
                        printDocument1.PrintPage -= PrintDocument1_PrintPage;
                        printDocument1.PrintPage += PrintDocument1_PrintPage;

                        ApplyReceiptPrinterSettings(printDocument1);

                        // No preview gate here - once the receipt printer is configured, the
                        // receipt must go straight to it instead of waiting on a dialog first.
                        if (IsCounterReceiptJpegMode())
                        {
                            // Customer's copy still goes to the physical printer as normal;
                            // only the counter's copy is diverted to a JPEG file below.
                            printSingleReceiptCopyOnly = true;
                            try
                            {
                                printDocument1.Print();
                            }
                            finally
                            {
                                printSingleReceiptCopyOnly = false;
                            }

                            SaveCounterReceiptAsJpeg();
                        }
                        else
                        {
                            printDocument1.Print();
                        }

                        // Capture before it's cleared below - this is what ties every sales_ row
                        // from this checkout back to the receipt that was just printed.
                        string completedTransactionCode = TransactionCode;

                        // Both copies are done rendering (preview and/or the real print) -
                        // safe to clear now that nothing will draw "Transcode: " again.
                        TransactionCode = "";

                        string changeSummary = change.Text;
                        string transactionSummary = counter.Text;

                        var soldLineItems = new List<(string Serial, int Quantity, decimal ItemPrice)>();

                        bool anyLiveInventoryUpdate = false;
                        try
                        {
                            // One connection for the whole cart instead of one per line item -
                            // each row still succeeds or falls back to the offline cache
                            // independently (see inventory_update), this just stops reconnecting
                            // and reloading the full item catalog on every single row.
                            await ConnectDb();
                            try
                            {
                                for (int i = 0; i < dgv_items.RowCount - 1; i++)
                                {
                                    rows = dgv_items.Rows[i].Cells[0].Value?.ToString() ?? "";

                                    qty = ToInt(dgv_items.Rows[i].Cells[2].Value);

                                    if (await inventory_update(connect_db, rows, qty)) // change 1
                                        anyLiveInventoryUpdate = true;

                                    soldLineItems.Add((rows, qty, ToDecimal(dgv_items.Rows[i].Cells[4].Value)));
                                }
                            }
                            finally
                            {
                                connect_db?.Close();
                            }
                        }
                        finally
                        {
                            // The receipt is already printed and payment already taken by this
                            // point, so whatever line items were successfully gathered above must
                            // still be recorded (or queued via SaveSaleLineItemsAsync's own
                            // offline fallback) even if a later row in the loop failed to parse -
                            // never let that silently drop the whole sale from sales_.
                            await SaveSaleLineItemsAsync(completedTransactionCode, soldLineItems);

                            // Reload the catalog once for the whole sale (not per line item), and
                            // only if something actually changed in the DB - skip it when the
                            // whole cart fell back to offline, since a reload would just fail again.
                            if (anyLiveInventoryUpdate)
                                RefreshItemsCache();
                        }

                        tbx_serialcode.Enabled = true;

                        tbx_serialcode.Focus();

                        tbx_denomination.Clear();

                        tbx_denomination.Text = "*,***.**";

                        change.Text = "CHANGE: 0";

                        dgv_items.Rows.Clear();
                        grandTotal = 0;
                        count = 0;

                        MessageBox.Show($"{changeSummary}\n\n{transactionSummary}", "Transaction Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        counter.Text = "Total items: 0 \nGrand Total: 0.00";

                        tbx_denomination.Enabled = false;

                        tbx_denomination.ReadOnly = false;

                        selltype.Text = "Select...";
                    }

                }
                else if (e.KeyCode == Keys.Escape)
                {
                    tbx_denomination.Enabled = false;

                    tbx_serialcode.Focus();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Press OK to continue! \n {ex.Message}", "POSXX1");
            }

        }
        public int price_per_piece;
        private int Itemprice_InPcs(string serial)
        {
            try
            {
                var row = FindItemRow(serial);

                price_per_piece = row != null ? ToInt(row["price_perpc"]) : 0;
            }
            catch (Exception ex)
            {
                lbl_warning.Text = "An error occurred: " + ex.Message;
            }
            return price_per_piece;
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                int totalRow = 0;

                var total_qty = 0;

                if (e.KeyCode == Keys.Enter)
                {

                    dgv_items.CurrentRow.Cells[2].Value = textBox1.Text;

                    if (dgv_items.CurrentRow.Cells[0].Value.ToString() == "01282026-085944") //atwork
                    {
                        if (Convert.ToInt32(dgv_items.CurrentRow.Cells[2].Value) >= 4)
                        {
                            dgv_items.CurrentRow.Cells[4].Value = (Convert.ToInt64(textBox1.Text) * Convert.ToInt64(dgv_items.CurrentRow.Cells[3].Value.ToString())) - 2;
                        }
                        else
                        {
                            dgv_items.CurrentRow.Cells[4].Value = (Convert.ToInt64(textBox1.Text) * Convert.ToInt64(dgv_items.CurrentRow.Cells[3].Value.ToString()));
                        }
                    }
                    else
                    {
                        dgv_items.CurrentRow.Cells[4].Value = Convert.ToInt64(textBox1.Text) * Convert.ToInt64(dgv_items.CurrentRow.Cells[3].Value.ToString());
                    }
                    textBox1.Enabled = false;

                    tbx_serialcode.Enabled = true;

                    tbx_serialcode.ReadOnly = false;

                    tbx_serialcode.Clear();

                    tbx_serialcode.Focus();

                    textBox1.Text = "1";

                    grandTotal = 0;


                    for (int i = 0; i < dgv_items.Rows.Count - 1; i++)
                    {
                        grandTotal += Convert.ToDecimal(dgv_items.Rows[i].Cells[4].Value);

                        totalRow = Convert.ToInt32(dgv_items.Rows[i].Cells[2].Value) + Convert.ToInt32(dgv_items.CurrentRow.Cells[2].Value);

                        total_qty += Convert.ToInt32(dgv_items.Rows[i].Cells[2].Value);
                    }

                    // Keep the class-level count field (which dgv_items_RowsAdded now updates
                    // incrementally rather than always recomputing) in sync with this row edit.
                    count = total_qty;

                    counter.Text = BuildCounterText(grandTotal, $"Total Quantity: {total_qty}");
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    textBox1.Clear();

                    tbx_serialcode.Focus();
                }
            }
            catch (Exception ex)
            {
                lbl_warning.Text = "An error occurred: " + ex.Message;
            }
            //counter.Text = "Total distinct items: " + count + " \nGrand Total: " + grandTotal.ToString("0.00");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Purchases_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void tbx_denomination_TextChanged(object sender, EventArgs e)
        {
            try
            {
                change.Text = "Change: " + (Convert.ToInt32(tbx_denomination.Text) - TaxConfig.ComputeTotalWithTax(grandTotal));
            }
            catch (Exception ex)
            { ex.Message.ToString(); }
        }

        private void tbx_serialcode_Enter(object sender, EventArgs e)
        {

        }
        public string ReturnSerial(string serial)
        {
            return serial;
        }
        public void msgbox(string msg)
        {
            MessageBox.Show(msg);
        }
        MySqlCommand update_inventory = new MySqlCommand();
        private void tbx_serialcode_Validated(object sender, EventArgs e)
        {

        }
        public async Task Update_Qty_In_Inventory(string serial, int quantity)
        {
            try
            {
                await ConnectDb();

                CreateTransactionCode(serial);

                long current_quantity = ItemsTable.AsEnumerable()
                    .Where(r => r.Field<string>("serial") == serial)
                    .Sum(r => ToLong(r["new_quantity"]));

                long diff = current_quantity - (Convert.ToInt32(quantity));

                update_inventory.Connection = connect_db;

                update_inventory.CommandText = "UPDATE items_ SET new_quantity = @new_quantity WHERE serial = @serial AND new_quantity != '' ";

                update_inventory.Parameters.AddWithValue("@new_quantity", diff);

                update_inventory.Parameters.AddWithValue("@serial", serial);

                await update_inventory.ExecuteNonQueryAsync();

                update_inventory.Connection.Close();

                RefreshItemsCache();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Qty Inventory: Error occurred while processing the data.");

            }
            connect_db?.Close();
        }

        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageDashboard)
            {
                await LoadDashboardAsync();
                return;
            }

            if (tabControl1.SelectedTab != tabPageManagePrices)
                return;

            using (var pinForm = new PinPromptForm())
            {
                string managerPin = ConfigurationManager.AppSettings["MANAGER_PIN"];
                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != managerPin)
                {
                    MessageBox.Show("Incorrect PIN. Access to Manage Prices is restricted.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl1.SelectedTab = tabPageSales;
                    return;
                }
            }

            await LoadManagePricesGrid();
        }

        /// <summary>
        /// Populates the Dashboard tab: inventory stats come straight from the in-memory
        /// Items cache (instant, works offline); sales figures need a live query, so they
        /// degrade to "unavailable offline" instead of throwing when the DB can't be reached.
        /// </summary>
        private async Task LoadDashboardAsync()
        {
            btnDashboardRefresh.Enabled = false;
            try
            {
                LoadDashboardInventoryStats();
                await LoadDashboardSalesAsync();
            }
            finally
            {
                btnDashboardRefresh.Enabled = true;
            }
        }

        private async void btnDashboardRefresh_Click(object sender, EventArgs e) => await LoadDashboardAsync();

        private void LoadDashboardInventoryStats()
        {
            DataTable items = ItemsTable;
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

            lblDashStockValue.Text = (items.Rows.Count - outOfStock).ToString();
            lblDashStockSub.Text = $"Inventory value: ₱{inventoryValue:N2}";
            lblDashOutOfStockValue.Text = outOfStock.ToString();

            var lowStock = items.AsEnumerable()
                .Where(row => (row["new_quantity"] == DBNull.Value ? 0m : Convert.ToDecimal(row["new_quantity"])) <= 0)
                .OrderBy(row => row["description"].ToString())
                .ToList();

            dgvDashboardLowStock.Rows.Clear();
            foreach (DataRow row in lowStock)
            {
                dgvDashboardLowStock.Rows.Add(
                    row["serial"].ToString(),
                    row["description"].ToString().ToUpperInvariant(),
                    row["new_quantity"].ToString());
            }
        }

        private async Task LoadDashboardSalesAsync()
        {
            int year = DateTime.Now.Year;

            try
            {
                await ConnectDb();
                if (connect_db == null || connect_db.State != ConnectionState.Open)
                    throw new InvalidOperationException("No database connection.");

                var sales = new List<(DateTime Date, decimal Price)>();
                using (var cmd = new MySqlCommand("SELECT transaction_date, item_price FROM sales_ WHERE transaction_date LIKE @yearPattern", connect_db))
                {
                    cmd.Parameters.AddWithValue("@yearPattern", year + "-%");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string raw = reader["transaction_date"]?.ToString();
                            decimal price = reader["item_price"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["item_price"]);

                            if (DateTime.TryParseExact(raw, "yyyy-MMMM-dd H:m:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                                sales.Add((parsed, price));
                        }
                    }
                }

                PopulateDashboardSales(sales, year);
                lblDashboardStatus.Text = "";
            }
            catch (Exception)
            {
                lblDashboardStatus.Text = "Sales data unavailable offline.";
                lblDashTodayValue.Text = "—";
                lblDashTodaySub.Text = "";
                lblDashMonthValue.Text = "—";
                chartDashboardSales.Series["Sales"].Points.Clear();
            }
            finally
            {
                connect_db?.Close();
            }
        }

        private void PopulateDashboardSales(List<(DateTime Date, decimal Price)> sales, int year)
        {
            DateTime today = DateTime.Today;
            decimal todayTotal = 0m;
            int todayCount = 0;
            decimal monthTotal = 0m;
            decimal[] monthlyTotals = new decimal[12];

            foreach (var s in sales)
            {
                if (s.Date.Year == year)
                    monthlyTotals[s.Date.Month - 1] += s.Price;

                if (s.Date.Date == today)
                {
                    todayTotal += s.Price;
                    todayCount++;
                }

                if (s.Date.Year == today.Year && s.Date.Month == today.Month)
                    monthTotal += s.Price;
            }

            lblDashTodayValue.Text = $"₱{todayTotal:N2}";
            lblDashTodaySub.Text = $"{todayCount} transaction{(todayCount == 1 ? "" : "s")}";
            lblDashMonthValue.Text = $"₱{monthTotal:N2}";

            string[] monthLabels = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames.Take(12).ToArray();
            var series = chartDashboardSales.Series["Sales"];
            series.Points.Clear();
            for (int i = 0; i < 12; i++)
                series.Points.AddXY(monthLabels[i], monthlyTotals[i]);
        }

        private void btnDashInventoryReport_Click(object sender, EventArgs e) => inventoryReportToolStripMenuItem_Click(sender, e);

        private void btnDashSalesChart_Click(object sender, EventArgs e) => monthlySalesChartToolStripMenuItem_Click(sender, e);

        private void btnDashPettyCash_Click(object sender, EventArgs e) => pettyCashReportToolStripMenuItem_Click(sender, e);

        private void btnDashLoyalty_Click(object sender, EventArgs e) => loyaltyToolStripMenuItem_Click(sender, e);

        private void btnDashSystemConfig_Click(object sender, EventArgs e) => systemConfigToolStripMenuItem_Click(sender, e);

        private async Task LoadManagePricesGrid()
        {
            try
            {
                await ConnectDb();
                try
                {
                    using (var adapter = new MySqlDataAdapter("SELECT serial, description, price, retail_price, price_perpc FROM items_", connect_db))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        dgv_managePrices.DataSource = table;
                    }
                }
                finally
                {
                    connect_db?.Close();
                }

                dgv_managePrices.Columns["serial"].HeaderText = "Serial";
                dgv_managePrices.Columns["serial"].ReadOnly = true;
                dgv_managePrices.Columns["description"].HeaderText = "Description";
                dgv_managePrices.Columns["description"].ReadOnly = true;
                dgv_managePrices.Columns["price"].HeaderText = "Price";
                dgv_managePrices.Columns["price"].DefaultCellStyle.Format = "0.00";
                dgv_managePrices.Columns["retail_price"].HeaderText = "Retail Price";
                dgv_managePrices.Columns["retail_price"].DefaultCellStyle.Format = "0.00";
                dgv_managePrices.Columns["price_perpc"].HeaderText = "Price Per Piece";
                dgv_managePrices.Columns["price_perpc"].DefaultCellStyle.Format = "0.00";

                // Reloading swaps in a brand new DataTable, so re-apply whatever the manager
                // had already typed instead of silently dropping their search.
                ApplyManagePricesFilter();

                lbl_managePricesStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to load items.");
            }
        }

        private async void btn_refreshPrices_Click(object sender, EventArgs e) => await LoadManagePricesGrid();

        private void tbx_managePricesSearch_TextChanged(object sender, EventArgs e) => ApplyManagePricesFilter();

        /// <summary>
        /// Filters dgv_managePrices to rows whose serial or description contains the search
        /// text, via the bound DataTable's DefaultView - the same view instance the grid
        /// already renders through, so this doesn't disturb row edit-tracking for Save Changes.
        /// </summary>
        private void ApplyManagePricesFilter()
        {
            if (!(dgv_managePrices.DataSource is DataTable table))
                return;

            try
            {
                string search = tbx_managePricesSearch.Text.Trim().Replace("'", "''");

                table.DefaultView.RowFilter = string.IsNullOrEmpty(search)
                    ? string.Empty
                    : $"serial LIKE '%{search}%' OR description LIKE '%{search}%'";
            }
            catch
            {
                // Malformed filter expression (e.g. an unmatched bracket) - leave the grid
                // showing the last valid filter instead of throwing on every keystroke.
            }
        }

        private async void btn_addItem_Click(object sender, EventArgs e)
        {
            using (var addItemForm = new AddItemForm())
            {
                addItemForm.ShowDialog(this);

                if (addItemForm.ItemSaved)
                {
                    RefreshItemsCache();
                    await LoadManagePricesGrid();
                }
            }
        }

        private async void btn_savePrices_Click(object sender, EventArgs e)
        {
            if (!(dgv_managePrices.DataSource is DataTable table))
                return;

            DataTable changes = table.GetChanges(DataRowState.Modified);
            if (changes == null)
            {
                lbl_managePricesStatus.Text = "No changes to save.";
                return;
            }

            foreach (DataRow row in changes.Rows)
            {
                if (!decimal.TryParse(row["price"].ToString(), out decimal price) ||
                    !decimal.TryParse(row["retail_price"].ToString(), out decimal retailPrice) ||
                    !decimal.TryParse(row["price_perpc"].ToString(), out decimal pricePerPc) ||
                    price < 0 || retailPrice < 0 || pricePerPc < 0)
                {
                    MessageBox.Show($"Invalid price value for item '{row["serial"]}'. Prices must be non-negative numbers.", "Invalid Input");
                    return;
                }
            }

            try
            {
                await ConnectDb();
                try
                {
                    foreach (DataRow row in changes.Rows)
                    {
                        using (var cmd = new MySqlCommand("UPDATE items_ SET price=@price, retail_price=@retail_price, price_perpc=@price_perpc WHERE serial=@serial", connect_db))
                        {
                            cmd.Parameters.AddWithValue("@price", row["price"]);
                            cmd.Parameters.AddWithValue("@retail_price", row["retail_price"]);
                            cmd.Parameters.AddWithValue("@price_perpc", row["price_perpc"]);
                            cmd.Parameters.AddWithValue("@serial", row["serial"]);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                finally
                {
                    connect_db?.Close();
                }

                table.AcceptChanges();
                RefreshItemsCache();
                lbl_managePricesStatus.Text = $"Saved {changes.Rows.Count} item(s).";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to save prices.");
            }
        }

        private async void btn_deletePrice_Click(object sender, EventArgs e)
        {
            if (dgv_managePrices.CurrentRow == null || dgv_managePrices.CurrentRow.IsNewRow)
            {
                lbl_managePricesStatus.Text = "Select an item to delete.";
                return;
            }

            string serial = dgv_managePrices.CurrentRow.Cells["serial"].Value?.ToString();
            string description = dgv_managePrices.CurrentRow.Cells["description"].Value?.ToString();

            if (string.IsNullOrEmpty(serial))
                return;

            var confirm = MessageBox.Show(this, $"Delete item '{description}' ({serial})? This cannot be undone.",
                "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                await ConnectDb();
                try
                {
                    using (var cmd = new MySqlCommand("DELETE FROM items_ WHERE serial=@serial", connect_db))
                    {
                        cmd.Parameters.AddWithValue("@serial", serial);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                finally
                {
                    connect_db?.Close();
                }

                RefreshItemsCache();
                await LoadManagePricesGrid();
                lbl_managePricesStatus.Text = $"Deleted '{serial}'.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to delete item.");
            }
        }

        #region UnusedEvents
        private void process1_Exited(object sender, EventArgs e)
        {

        }

        private void Purchases_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                selltype.SelectedIndex = 1;
            }
            else if (e.KeyCode == Keys.F8)
            {
                selltype.SelectedIndex = 0;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Loads the store logo through LogoCache instead of PictureBox's own ImageLocation
        /// loader, so offline runs fall back to the last cached copy instead of PictureBox
        /// silently swapping in a broken-image icon that would then get printed on receipts.
        /// </summary>
        private async Task LoadReceiptLogoAsync()
        {
            pictureBox1.Image = await LogoCache.LoadAsync();
        }

        private async void CreateTransactionCode(string itemcode)
        {
            try
            {
                await ConnectDb();
                if (connect_db.State != ConnectionState.Open)
                    connect_db.Open();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = connect_db;

                    cmd.CommandText = "SELECT COUNT(transaction_code) FROM sales_ ";

                    var result = await cmd.ExecuteScalarAsync();

                    cmd.CommandText = $"INSERT INTO sales_(transaction_code,itemcode, transaction_date, seller, item_price) VALUES ( @transaction_code, @itemcode, @transaction_date, @seller, @item_price)";
                    cmd.Parameters.AddWithValue("@transaction_code", TransactionCode + "-" + (result != DBNull.Value ? Convert.ToInt64(result) + 1 : 1));

                    cmd.Parameters.AddWithValue("@itemcode", itemcode);

                    cmd.Parameters.AddWithValue("@transaction_date", DateTime.Now.ToString("yyyy-MMMM-dd H:m:ss"));

                    cmd.Parameters.AddWithValue("@seller", log_in);

                    cmd.Parameters.AddWithValue("@item_price", GetItemPrice(itemcode));

                    await cmd.ExecuteNonQueryAsync();

                }
                connect_db.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Transaction Code: Error occurred while processing the data.");
            }
            return;
        }
        private int Isolated(string sugar_barcode, int num_of_items = 2)
        {
            int price = 0;

            var row = FindItemRow(sugar_barcode);
            if (row != null)
            {
                if (num_of_items == 1)
                {
                    price = ToInt(row["price_perpc"]);
                }
                else if (num_of_items >= 4)
                {
                    price = ToInt(row["retail_price"]) - 2;
                }
            }
            return price;
        }
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
        /// Shared by every printable in the app - printDocument1 (receipts), printDocument2
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
                }
                catch (InvalidPrinterException)
                {
                    // Listed in InstalledPrinters (Windows still has a queue for it) but not
                    // actually reachable right now - e.g. a disconnected/offline network or USB
                    // thermal printer, or a redirected printer from a remote session that isn't
                    // live. Accessing PrinterSettings/DefaultPageSettings throws in that case
                    // instead of degrading gracefully, which used to crash both real checkout
                    // printing and the receipt preview. Falls back to the un-configured defaults
                    // set above instead, same as if the printer weren't configured at all.
                    document.PrinterSettings.PrinterName = "";
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

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
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
                int total = 0;

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
                if (pictureBox1.Image != null)
                {
                    const int maxLogoHeight = 60;
                    Image logo = pictureBox1.Image;
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

                for (int i = 0; i < dgv_items.Rows.Count - 1; i++)
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

                    total += (int)Math.Round(lineTotal);

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
                MessageBox.Show(ex.Message);
            }
        }
        private async void tbx_search_TextChanged(object sender, EventArgs e)
        {
            string search = tbx_search.Text ?? "";

            var rows = ItemsTable.AsEnumerable()
                .Where(r => (Contains(r["description"], search) || Contains(r["serial"], search)) && ToInt(r["price"]) > 0);

            dgv_itemSearch.Rows.Clear();

            int retriever = 0;

            foreach (var r in rows)
            {
                if (!string.IsNullOrEmpty(r["serial"].ToString()))
                {

                    retriever += 1;

                    if (!r["serial"].ToString().Contains("https:"))
                    {
                        if (selltype.SelectedIndex == 0 && ToInt(r["price"]) != 0)
                            dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString(), ToDecimal(r["price"]));
                        else if (selltype.SelectedIndex == 1 && ToInt(r["retail_price"]) != 0)
                            dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString(), ToDecimal(r["retail_price"]));
                        else if (selltype.SelectedIndex == 2 && ToInt(r["price_perpc"]) != 0)
                            dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString(), ToDecimal(r["price_perpc"]));
                    }
                }
            }
            listing.Text = "Total items: " + retriever.ToString();
            await AddToLogs($"User searched for: {tbx_search.Text}");
        }
        public async Task<bool> AddToLogs(string event_name)
        {
            var is_logged = false;
            try
            {
                await ConnectDb();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = connect_db;
                    cmd.CommandText = "INSERT INTO logs_(event_log) VALUES (@event)";
                    cmd.Parameters.AddWithValue("@event", event_name);
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Connection.Close();
                    is_logged = true;
                }
            }
            catch (Exception)
            {
                // Activity logging is best-effort. Failing to log (e.g. offline) must never
                // interrupt the cashier - this fires on every keystroke in the search box.
            }
            return is_logged;
        }
        #endregion
        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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
            ApplyReceiptPrinterSettings(printDocument1);

            bool injectedSampleRows = dgv_items.Rows.Count <= 1; // just the AllowUserToAddRows placeholder
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
            // just checking for empty) and falling back to "0" keeps the Cash/Change lines on the
            // preview instead of crashing it.
            if (!int.TryParse(tbx_denomination.Text, out _))
                tbx_denomination.Text = "0";

            try
            {
                PaperSize paperSize = printDocument1.DefaultPageSettings.PaperSize;
                Margins margins = printDocument1.DefaultPageSettings.Margins;

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
                        // PrintDocument1_PrintPage - e.PageSettings itself isn't read there - so
                        // the real printDocument1.DefaultPageSettings is fine to pass through as
                        // metadata even though its PaperSize/Margins don't reflect this preview's
                        // paper-width or margin overrides.
                        receiptCopyIndex = 0; // CUSTOMER'S COPY
                        // PrintDocument1_PrintPage catches its own exceptions (shows a MessageBox
                        // and returns) instead of throwing, so it wouldn't reach the line that
                        // sets lastReceiptContentHeight if something in this render goes wrong.
                        // Reset it first so a failed render can't silently reuse a stale height
                        // from a previous, differently-sized receipt.
                        lastReceiptContentHeight = 0f;
                        var args = new PrintPageEventArgs(g, marginBounds, pageBounds, printDocument1.DefaultPageSettings);
                        PrintDocument1_PrintPage(this, args);
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
                    for (int i = dgv_items.Rows.Count - 1; i >= 0; i--)
                    {
                        if (!dgv_items.Rows[i].IsNewRow)
                            dgv_items.Rows.RemoveAt(i);
                    }

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
            ApplyReceiptPrinterSettings(printDocument1);

            PaperSize paperSize = printDocument1.DefaultPageSettings.PaperSize;
            Margins margins = printDocument1.DefaultPageSettings.Margins;

            // Printer paper sizes are reported in hundredths of an inch; 203 DPI is a typical
            // thermal receipt printer's native resolution, so the saved JPEG comes out roughly
            // the size/proportions the paper copy would have been.
            const float dpi = 203f;

            // The image copy is meant to look like a standard 80mm thermal receipt regardless of
            // what the configured/fallback printer's driver reports - without this cap, no
            // installed/configured RECEIPT_PRINTER_NAME (or one whose driver defaults to a wider
            // page like Letter) would stretch the receipt's text/dividers out to that width
            // instead of staying inside an 80mm roll. Only clamps the width used for this JPEG
            // render - printDocument1's real page settings (used for the physical print) are
            // untouched, so an intentionally configured 80mm/58mm/etc. printer is never widened.
            const float maxJpegPaperWidthMm = 80f;
            int maxJpegPaperWidthHundredths = (int)Math.Round(maxJpegPaperWidthMm / 25.4 * 100);
            int effectiveWidthHundredths = Math.Min(paperSize.Width, maxJpegPaperWidthHundredths);

            int pageWidthPx = Math.Max(1, (int)(effectiveWidthHundredths / 100f * dpi));
            int marginLeftPx = (int)(margins.Left / 100f * dpi);
            int marginTopPx = (int)(margins.Top / 100f * dpi);
            int marginWidthPx = Math.Max(1, pageWidthPx - marginLeftPx - (int)(margins.Right / 100f * dpi));

            // Receipt length varies with item count and isn't known until PrintDocument1_PrintPage
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
                    var args = new PrintPageEventArgs(g, marginBounds, pageBounds, printDocument1.DefaultPageSettings);
                    PrintDocument1_PrintPage(this, args);
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

                    string printerLabel = string.IsNullOrWhiteSpace(printDocument1.PrinterSettings.PrinterName)
                        ? "Default"
                        : printDocument1.PrinterSettings.PrinterName;
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
                    MessageBox.Show(ex.Message, "Failed to save counter's receipt as JPEG");
                }
            }

            // pendingReceiptText/pendingReceiptCode were set by the manual PrintPage call above
            // but printDocument1_EndPrint never ran for it (it wasn't a real print job) - save the
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
                    MessageBox.Show(ex.Message, "Failed to save receipt copy");
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

        private void printDocument1_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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
                MessageBox.Show(ex.Message, "Failed to save receipt copy");
            }
            finally
            {
                pendingReceiptText = null;
                pendingReceiptCode = null;
            }
        }

        private string BuildReceiptText(int total)
        {
            var sb = new StringBuilder();
            sb.AppendLine(L_DEFAULTNAME);
            sb.AppendLine("Calayan, Gonzaga, Cagayan");
            sb.AppendLine("Transcode: " + TransactionCode);
            sb.AppendLine(new string('-', 40));
            sb.AppendLine("ITEMS");

            for (int i = 0; i < dgv_items.Rows.Count - 1; i++)
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

        private const int SlowPingThresholdMs = 300;
        private long? lastPingRoundtripMs;

        public async Task<string> netTesting()
        {
            string pingResult = "No connection";
            lastPingRoundtripMs = null;
            try
            {
                string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
                using (var ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(dbsrv, 2000);
                    if (reply.Status == IPStatus.Success)
                    {
                        lastPingRoundtripMs = reply.RoundtripTime;
                        pingResult = $"{reply.RoundtripTime}ms";
                    }
                    else
                    {
                        pingResult = "No connection";
                    }
                }
            }
            catch (Exception)
            {
                pingResult = "No connection";
            }
            return pingResult;
        }
        private void selltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                tbx_search.Enabled = true;

                tbx_serialcode.Focus();

                TransactionCode = "";

                TransactionCode = DateTime.Now.ToString("yyyyMMddHHmm");

                RefreshItemSearchList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Populates dgv_itemSearch for the current selltype selection. Shared by
        // selltype_SelectedIndexChanged and the F1/F2/F3 shortcuts so pressing a
        // key produces the same list as clicking the combo box.
        private void RefreshItemSearchList()
        {
            try
            {
                int retriever = 0;

                if (selltype.SelectedIndex == 0)
                {

                    dgv_itemSearch.Rows.Clear();

                    var rows = ItemsTable.AsEnumerable()
                        .Where(r => ToInt(r["price"]) != 0 && ToInt(r["new_quantity"]) != 0)
                        .OrderBy(r => r["description"].ToString());

                    foreach (var r in rows)
                    {
                        retriever += 1;

                        if (!string.IsNullOrEmpty(r["serial"].ToString()) && !r["serial"].ToString().Contains("https:"))

                            if (retriever > 10)
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price"]));
                    }

                }
                else if (selltype.SelectedIndex == 1)
                {

                    dgv_itemSearch.Rows.Clear();

                    var rows = ItemsTable.AsEnumerable()
                        .Where(r => ToInt(r["retail_price"]) != 0 && ToInt(r["new_quantity"]) != 0)
                        .OrderBy(r => r["description"].ToString());

                    foreach (var r in rows)
                    {
                        retriever += 1;

                        if (!string.IsNullOrEmpty(r["serial"].ToString()) && !r["serial"].ToString().Contains("https:"))
                            if (retriever > 10)
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]));
                    }
                }
                else if (selltype.SelectedIndex == 2)
                {


                    dgv_itemSearch.Rows.Clear();

                    var rows = ItemsTable.AsEnumerable()
                        .Where(r => ToInt(r["price_perpc"]) != 0 && ToInt(r["new_quantity"]) != 0)
                        .OrderBy(r => r["description"].ToString());

                    foreach (var r in rows)
                    {
                        retriever += 1;

                        if (!string.IsNullOrEmpty(r["serial"].ToString()) && !r["serial"].ToString().Contains("https:"))
                            if (retriever > 10)
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price_perpc"]));
                    }
                }
                else
                {
                    TransactionCode = "";

                    dgv_itemSearch.Rows.Clear();

                    dgv_items.Rows.Clear();

                    tbx_denomination.Enabled = false;

                    tbx_serialcode.Clear();

                    tbx_denomination.Clear();

                    grandTotal = 0;
                    count = 0;

                    counter.Text = "Total items: 0 \nGrand Total: 0.00";
                }

                listing.Text = "Total items: " + retriever.ToString();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void selltype_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                selltype.SelectedIndex = 0; // Wholesale

                TransactionCode = "BMnmart" + DateTime.Now.ToString("yyyyMMddHHmm") + "-W";

                RefreshItemSearchList();
            }
            else if (e.KeyCode == Keys.F3)
            {
                selltype.SelectedIndex = 1; // Retail

                TransactionCode = "BMnmart" + DateTime.Now.ToString("yyyyMMddHHmm") + "-R";

                RefreshItemSearchList();
            }
        }

        private async void dgv_items_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            string serial = e.Row.Cells[0].Value?.ToString();
            if (string.IsNullOrEmpty(serial) || !int.TryParse(e.Row.Cells[2].Value?.ToString(), out int quantity))
                return;

            try
            {
                await ConnectDb();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = connect_db;

                    cmd.CommandText = $"DELETE FROM sales_ WHERE transcode = @transcode";

                    cmd.CommandText = $"UPDATE items_ SET new_quantity = new_quantity + @new_quantity WHERE serial = @serial";

                    cmd.Parameters.AddWithValue("@transcode", TransactionCode);

                    cmd.Parameters.AddWithValue("@serial", serial);

                    cmd.Parameters.AddWithValue("@new_quantity", quantity);


                    cmd.ExecuteNonQuery();

                    cmd.Connection.Close();

                    RefreshItemsCache();
                }
            }
            catch (Exception)
            {
                // Offline (or the DB write otherwise failed): the item still left the cart, so
                // restore the quantity against the cached reference and record it for manual
                // reconciliation once back online, instead of blocking cart editing on a dialog.
                AdjustCachedQuantity(serial, quantity);
                LogPendingStockAdjustment(serial, quantity, "cart line removed while offline");
                lbl_warning.Text = "OFFLINE - stock restored against the last saved reference only.";
            }
        }

        private void tbx_serialcode_Enter2(object sender, EventArgs e)
        {

        }

        private void tbx_serialcode_Validated_1(object sender, EventArgs e)
        {
            /*
            try
            {
                if (tbx_serialcode.Text != "")
                    tbx_serialcode.Text = dgv_itemSearch.CurrentRow.Cells[0].Value.ToString();

                if (tbx_serialcode.Text == "01282026-085944")
                {
                    int quantity_exception = Convert.ToInt32(textBox1.Text);

                    dgv_items.Rows.Add(
                        tbx_serialcode.Text,
                        getItem(tbx_serialcode.Text)[0].ToUpperInvariant(),
                        textBox1.Text,
                        Isolated(tbx_serialcode.Text, quantity_exception),
                        Convert.ToInt64(Isolated(tbx_serialcode.Text, quantity_exception)) * Convert.ToInt64(textBox1.Text)
                        );
                }
                else
                {
                    dgv_items.Rows.Add(
                        tbx_serialcode.Text,
                        getItem(tbx_serialcode.Text)[0].ToUpperInvariant(),
                        textBox1.Text,
                        GetItemPrice(tbx_serialcode.Text),
                        Convert.ToInt64(GetItemPrice(tbx_serialcode.Text)) * Convert.ToInt32(textBox1.Text)
                        );
                }
                tbx_serialcode.Clear();

                tbx_serialcode.Focus();
            }
            catch (Exception xe)
            {
                MessageBox.Show(xe.Message, "Error occurred while processing the data.");
                throw;
            }
            */
            try
            {

                if (tbx_serialcode.Text != "")
                {

                    if (tbx_serialcode.Text == "01282026-085944")
                    {
                        dgv_items.Rows.Add(
                                tbx_serialcode.Text,
                                getItem(tbx_serialcode.Text)[0].ToUpperInvariant(),
                                //getItem(tbx_serialcode.Text),
                                textBox1.Text,
                                Isolated(tbx_serialcode.Text),
                                Convert.ToInt64(GetItemPrice(tbx_serialcode.Text)) * Convert.ToInt64(textBox1.Text)
                            );
                    }
                    else
                    {
                        dgv_items.Rows.Add(
                                tbx_serialcode.Text,
                                getItem(tbx_serialcode.Text)[0].ToUpperInvariant(),
                                //getItem(tbx_serialcode.Text),
                                textBox1.Text,
                                GetItemPrice(tbx_serialcode.Text),
                                Convert.ToInt64(GetItemPrice(tbx_serialcode.Text)) * Convert.ToInt64(textBox1.Text)
                            );
                    }

                    tbx_serialcode.Clear();
                    tbx_serialcode.Focus();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void vIewSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListSales listSales = new ListSales();
            listSales.ShowDialog();
        }

        private TellerHistoryForm _tellerHistoryForm;

        private void myTransactionHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_tellerHistoryForm != null && !_tellerHistoryForm.IsDisposed)
            {
                _tellerHistoryForm.Activate();
                return;
            }

            _tellerHistoryForm = new TellerHistoryForm();
            _tellerHistoryForm.Show(this);
        }

        private PettyCashHistoryForm _pettyCashHistoryForm;

        private void pettyCashReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pettyCashHistoryForm != null && !_pettyCashHistoryForm.IsDisposed)
            {
                _pettyCashHistoryForm.Activate();
                return;
            }

            _pettyCashHistoryForm = new PettyCashHistoryForm();
            _pettyCashHistoryForm.Show(this);
        }
        GenerateBarcode generateBarcode = new GenerateBarcode();
        private void dgv_itemSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            generateBarcode.code = dgv_itemSearch.CurrentRow.Cells[0].Value.ToString();
            generateBarcode.ShowDialog();
        }

        private void dgv_itemSearch_CellContextMenuStripChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_itemSearch_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {

        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string shortcuts =
                "GLOBAL\n" +
                "  F6           Hold the current transaction\n" +
                "  F8           Resume a held transaction\n" +
                "  Ctrl+H       Show this shortcuts list\n" +
                "\n" +
                "SERIAL CODE BOX\n" +
                "  Enter        Look up the scanned/typed code and add it to the cart\n" +
                "  F7           Add the typed code to the cart (manual entry)\n" +
                "  F1           Switch to Per-Piece pricing\n" +
                "  F2           Switch to Wholesale pricing\n" +
                "  F3           Switch to Retail pricing\n" +
                "  F4           Jump to the item search box\n" +
                "  F5           Reload items and restart the app\n" +
                "  F9           Jump to the cash/denomination box\n" +
                "  F10          Open Petty Cash\n" +
                "  F12          Jump to the cart grid\n" +
                "  Escape       Deselect the selling type\n" +
                "\n" +
                "ITEM SEARCH BOX\n" +
                "  Enter        Search items matching the typed text\n" +
                "  F4           Jump to the search results grid\n" +
                "  Escape       Clear the search box\n" +
                "\n" +
                "ITEM SEARCH RESULTS GRID\n" +
                "  F7           Add the highlighted item to the cart\n" +
                "  Escape       Return to the serial code box\n" +
                "  Double-click Show/print the item's barcode\n" +
                "\n" +
                "CART GRID\n" +
                "  Backspace    Remove the selected row and recalculate totals\n" +
                "  Escape       Clear the entire cart\n" +
                "  F4           Jump to the serial code box\n" +
                "  F9           Jump to the cash/denomination box\n" +
                "  F11          Recalculate using the per-piece price\n" +
                "  F12          Mark the row for quantity override\n" +
                "\n" +
                "QUANTITY BOX\n" +
                "  Enter        Apply the typed quantity and recalculate totals\n" +
                "  Escape       Clear and return to the serial code box\n" +
                "\n" +
                "CASH / DENOMINATION BOX\n" +
                "  Enter        Finalize the sale: print receipt, update inventory, reset cart\n" +
                "  Escape       Cancel cash entry";

            using (var helpForm = new HelpForm("Keyboard Shortcuts", shortcuts))
                helpForm.ShowDialog(this);
        }

        private void gettingStartedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var tutorial = new TutorialForm())
                tutorial.ShowDialog(this);
        }

        private void techRequirementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string configuredPrinter = ConfigurationManager.AppSettings["RECEIPT_PRINTER_NAME"];
            string configuredDbServer = ConfigurationManager.AppSettings["DB_SERVER"];

            string requirements =
                "TECHNICAL REQUIREMENTS\n" +
                "\n" +
                "Operating System:  Windows 10 or later\n" +
                "Runtime:           .NET Framework 4.7.2 or later\n" +
                "Display:           1024x768 resolution or higher\n" +
                "\n" +
                "DATABASE\n" +
                "  Network access to the MySQL server configured in\n" +
                "  System Config > Database.\n" +
                $"  Currently configured: {(string.IsNullOrWhiteSpace(configuredDbServer) ? "not set" : configuredDbServer)}\n" +
                "  If the server can't be reached, the app falls back to a\n" +
                "  read-only offline snapshot instead of failing to start.\n" +
                "\n" +
                "PRINTER\n" +
                "  A receipt printer supporting the paper width configured in\n" +
                "  System Config > Printer (POS80/58mm thermal printers are\n" +
                "  the primary supported hardware).\n" +
                $"  Currently configured: {(string.IsNullOrWhiteSpace(configuredPrinter) ? "not set" : configuredPrinter)}\n" +
                "\n" +
                "LOCAL STORAGE\n" +
                "  Write access to Documents\\App Inventory\\ - used for the\n" +
                "  offline item/user cache, activity log fallback, scheduled\n" +
                "  report tracking, and saved counter's-copy receipt JPEGs.\n" +
                "\n" +
                "NETWORK (OPTIONAL)\n" +
                "  Internet access is used for emailing scheduled reports\n" +
                "  (SMTP or Gmail) and downloading the store logo; the app\n" +
                "  runs without it, just without those features.";

            using (var helpForm = new HelpForm("Technical Requirements", requirements))
                helpForm.ShowDialog(this);
        }

        private void holdTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = new List<object[]>();
            foreach (DataGridViewRow row in dgv_items.Rows)
            {
                if (row.IsNewRow) continue;

                var vals = new object[row.Cells.Count];
                for (int i = 0; i < row.Cells.Count; i++)
                    vals[i] = row.Cells[i].Value;
                items.Add(vals);
            }

            if (items.Count == 0)
            {
                suppressNextSerialKeyUp = true;
                MessageBox.Show("There is nothing to hold - the current cart is empty.", "Hold Transaction");
                return;
            }

            string ticketCode = !string.IsNullOrEmpty(TransactionCode) ? TransactionCode : "HOLD" + DateTime.Now.ToString("yyyyMMddHHmmss");

            heldTransactions[ticketCode] = new HeldTransaction
            {
                TicketCode = ticketCode,
                TransactionCode = TransactionCode,
                SelltypeIndex = selltype.SelectedIndex,
                GrandTotal = grandTotal,
                CounterText = counter.Text,
                HeldAt = DateTime.Now,
                Items = items
            };

            dgv_items.Rows.Clear();
            grandTotal = 0;
            count = 0;
            counter.Text = "Total items: 0 \nGrand Total: 0.00";
            tbx_serialcode.Clear();
            tbx_serialcode.Enabled = true;
            tbx_denomination.Clear();
            tbx_denomination.Enabled = false;
            TransactionCode = "";

            heldTransactionsWindow?.RefreshList();

            suppressNextSerialKeyUp = true;
            MessageBox.Show($"Transaction held as ticket \"{ticketCode}\".\nUse Resume Transaction (F8) or View Held Transactions (Ctrl+T) to bring it back.", "Hold Transaction", MessageBoxButtons.OK, MessageBoxIcon.Information);

            tbx_serialcode.Focus();
        }

        private void resumeTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (heldTransactions.Count == 0)
            {
                MessageBox.Show("There are no held transactions.", "Resume Transaction");
                return;
            }

            if (heldTransactions.Count == 1)
            {
                string ticketCode = heldTransactions.Values.First().TicketCode;
                if (!ResumeHeldTransaction(ticketCode, out string error))
                    MessageBox.Show(error, "Resume Transaction");
                return;
            }

            ShowHeldTransactionsWindow();
        }

        private void viewHeldTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHeldTransactionsWindow();
        }

        private void ShowHeldTransactionsWindow()
        {
            if (heldTransactionsWindow == null || heldTransactionsWindow.IsDisposed)
                heldTransactionsWindow = new HeldTransactionsForm(this);

            heldTransactionsWindow.RefreshList();
            heldTransactionsWindow.StartPosition = FormStartPosition.Manual;
            heldTransactionsWindow.Location = new Point(this.Left + this.Width, this.Top);
            heldTransactionsWindow.Show();
            heldTransactionsWindow.Activate();
        }

        internal bool ResumeHeldTransaction(string ticketCode, out string errorMessage)
        {
            errorMessage = null;

            if (dgv_items.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow))
            {
                errorMessage = "Please hold or finish the current transaction before resuming another one.";
                return false;
            }

            if (!heldTransactions.TryGetValue(ticketCode, out HeldTransaction held))
            {
                errorMessage = "That held transaction no longer exists.";
                return false;
            }

            heldTransactions.Remove(ticketCode);

            selltype.SelectedIndex = held.SelltypeIndex;
            TransactionCode = held.TransactionCode;

            dgv_items.Rows.Clear();
            count = 0;
            foreach (var vals in held.Items)
                dgv_items.Rows.Add(vals);

            grandTotal = held.GrandTotal;
            counter.Text = held.CounterText;

            tbx_serialcode.Enabled = true;
            tbx_serialcode.Focus();

            heldTransactionsWindow?.RefreshList();
            this.BringToFront();

            return true;
        }

        internal void CancelHeldTransaction(string ticketCode)
        {
            heldTransactions.Remove(ticketCode);
            heldTransactionsWindow?.RefreshList();
        }

        private void printDocument2_BeginPrint(object sender, PrintEventArgs e)
        {
            outOfStockPrintRowIndex = 0;
        }

        /// <summary>
        /// Prints the Dashboard's "Out of Stock Items" list (dgvDashboardLowStock). Laid out the
        /// same way PrintDocument1_PrintPage lays out a receipt - word-wrapped item line then a
        /// Serial/Qty detail line, page-width-driven rather than fixed column offsets - because
        /// this is meant to come out of the same narrow POS80/58mm thermal roll as receipts, not
        /// a letter/A4 sheet, and a rigid table would clip on paper that narrow.
        /// </summary>
        private void printDocument2_PrintPage(object sender, PrintPageEventArgs e)
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
                MessageBox.Show(ex.Message);
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
            ApplyReceiptPrinterSettings(printDocument2);

            printDocument2.Print();
        }

        private void barcodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generateBarcode.code = DateTime.Now.ToString("yyyyMMddHHmmss");
            generateBarcode.ShowDialog();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToExcel(dgv_itemSearch);
        }

        private void inventoryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new InventoryReport().ShowDialog();
        }

        private void salesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ListSales().ShowDialog();
        }

        private void dailySalesChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SalesChartForm(SalesChartPeriod.Daily).ShowDialog();
        }

        private void monthlySalesChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SalesChartForm(SalesChartPeriod.Monthly).ShowDialog();
        }

        private void quarterlySalesChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SalesChartForm(SalesChartPeriod.Quarterly).ShowDialog();
        }

        private void receiptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string receiptsDir = GetReceiptsDirectory();
                Directory.CreateDirectory(receiptsDir);
                Process.Start("explorer.exe", $"\"{receiptsDir}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to open Receipts folder");
            }
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var pinForm = new PinPromptForm())
            {
                string managerPin = ConfigurationManager.AppSettings["MANAGER_PIN"];
                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != managerPin)
                {
                    MessageBox.Show("Incorrect PIN. Access to Manage Users is restricted.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var usersForm = new ManageUsersForm())
            {
                usersForm.ShowDialog(this);
            }
        }

        private void taxConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var pinForm = new PinPromptForm())
            {
                string managerPin = ConfigurationManager.AppSettings["MANAGER_PIN"];
                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != managerPin)
                {
                    MessageBox.Show("Incorrect PIN. Access to Tax Configuration is restricted.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var taxConfigForm = new TaxConfigForm())
            {
                taxConfigForm.ShowDialog(this);
            }
        }

        private void loyaltyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var loyaltyForm = new LoyaltyForm())
            {
                loyaltyForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// systemConfigToolStripMenuItem is hidden from the menu bar, so its own ShortcutKeys
        /// never fires (WinForms only processes a menu shortcut while the item is Visible).
        /// Handling Ctrl+Shift+S here instead catches it regardless of which control has focus.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.S))
            {
                systemConfigToolStripMenuItem_Click(this, EventArgs.Empty);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void systemConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // System Configuration is gated behind the separate admin PIN (not the manager
            // PIN used by Manage Prices/Users/Tax Config) since it exposes the DB connection,
            // receipt printer, and both PINs themselves - a manager shouldn't need to reach it.
            using (var pinForm = new PinPromptForm())
            {
                pinForm.PromptText = "Enter system admin PIN to continue:";
                string adminPin = ConfigurationManager.AppSettings["ADMIN_PIN"];
                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != adminPin)
                {
                    MessageBox.Show("Incorrect PIN. Access to System Configuration is restricted.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var systemConfigForm = new SystemConfigForm())
            {
                systemConfigForm.ShowDialog(this);
            }

            ApplyMenuVisibility();
            ApplyStoreIdentity();
        }

        internal static void ExportToExcel(DataGridView dgv)
        {
            DataTable dt = new DataTable();

            // Add columns
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                dt.Columns.Add(col.HeaderText);
            }

            // Add rows
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        dr[i] = row.Cells[i].Value?.ToString();
                    }

                    dt.Rows.Add(dr);
                }
            }

            // Create Excel workbook
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Report");

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Report as of {DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    wb.SaveAs(sfd.FileName);

                    MessageBox.Show("Export Successful!");
                }
            }
        }
    }
}
