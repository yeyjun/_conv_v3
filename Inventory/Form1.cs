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
using static Inventory.DataRowConvert;

namespace Inventory
{
    public partial class Purchases : Form
    {
        public long count = 0;
        public decimal grandTotal = 0;
        int total_row = 0;
        public string text_in_serial;
        public string transactioncode;

        private const string QtyStateColumnName = "qtyState";
        private const int QtyStateColumnIndex = 5;
        private const int QtyStatePerPiece = 1;
        private const int QtyStateBulk = 2;

        public static int countcells = 0;
        public static string log_in = "";
        private static string _loggedInCode;

        /// <summary>Shown on the menu bar immediately left of the Logout button - log_in holds
        /// the fullname (see Login.cs: id = match["fullname"]), which reads naturally in a
        /// greeting, unlike _loggedInCode (the entered login code shown in the title bar).
        /// Re-run from timerClock_Tick so the greeting flips to the next period automatically
        /// if a session happens to span a threshold.</summary>
        private void UpdateGreeting()
        {
            if (string.IsNullOrEmpty(log_in))
            {
                lblGreeting.Text = "";
                return;
            }

            int hour = DateTime.Now.Hour;
            string period = hour < 12 ? "morning" : hour < 18 ? "afternoon" : "evening";
            lblGreeting.Text = $"Good {period}, {log_in}!";
        }
        public DataTable dataTable = new DataTable();
        public string TransactionCode { get; private set; }
        public string L_DEFAULTNAME = ConfigurationManager.AppSettings["STORE_NAME"] ?? "Baclig Minimart";
        public string S_DEFAULTNAME = ConfigurationManager.AppSettings["STORE_CODE"] ?? "BCLGMNM";
        string dbname, dbpw, connection_timeout, dbuser, dbsrv;

        /// <summary>
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
            ("editLayoutToolStripMenuItem", "Edit Layout"),
            ("helpToolStripMenuItem", "Help"),
            ("gettingStartedToolStripMenuItem", "Getting Started"),
        };

        private LayoutEditMessageFilter _layoutEditFilter;

        private readonly TransactionShortcutFilter _transactionShortcutFilter = new TransactionShortcutFilter();

        /// <summary>Default HIDDEN_MENUS value when the setting is missing from App.config - hides
        public const string DefaultHiddenMenus = "systemConfigToolStripMenuItem,loyaltyToolStripMenuItem,taxConfigToolStripMenuItem,receiptsToolStripMenuItem,editLayoutToolStripMenuItem,gettingStartedToolStripMenuItem,usersToolStripMenuItem,reportsToolStripMenuItem,helpToolStripMenuItem";

        /// <summary>
        /// Shows/hides each top-level menu per the HIDDEN_MENUS appSetting (comma-separated
        /// ToolStripMenuItem names, edited from System Config > Menus). Called on load and again
        /// after System Config closes so a change takes effect immediately, no restart needed.
        /// </summary>
        /// surface its configuration warnings even after the window that raised them closes.</summary>
        public void ShowTrayNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Warning)
        {
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = message;
            trayIcon.BalloonTipIcon = icon;
            trayIcon.ShowBalloonTip(8000);
        }

        public void ApplyMenuVisibility()
        {
            var configurableNames = new HashSet<string>(ConfigurableMenuItems.Select(m => m.Name));

            foreach (ToolStripItem item in menuStripMain.Items)
            {
                if (configurableNames.Contains(item.Name))
                    item.Visible = !IsMenuHidden(item.Name);
            }
        }

        /// <summary>Re-syncs the cart board grid's background per <see cref="CartBoardTheme"/> -
        /// called from the constructor, and again by System Configuration (same pattern as
        /// <see cref="ApplyMenuVisibility"/> for SET_MENUS) when CART_BOARD_BACKGROUND_COLOR
        /// changes or when the accent background it blends with (gbx_itemList's BackColor) does.</summary>
        public void ApplyCartBoardBackground() => CartBoardTheme.ApplyTo(dgv_items);

        /// <summary>Re-syncs the item search grid's background per <see cref="ItemSearchGridTheme"/> -
        /// same pattern as <see cref="ApplyCartBoardBackground"/> for the cart board grid.</summary>
        public void ApplyItemSearchGridBackground() => ItemSearchGridTheme.ApplyTo(dgv_itemSearch);

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

        public static readonly (string Name, string Label)[] ItemSearchColumns = new[]
        {
            ("serial", "Serial"),
            ("description", "Particular"),
            ("price", "Price"),
            ("qty", "Stock/Qty"),
        };

        public const string DefaultHiddenItemSearchColumns = "qty";

        public static bool IsItemSearchColumnHidden(string columnName)
        {
            string raw = ConfigurationManager.AppSettings["ITEM_SEARCH_HIDDEN_COLUMNS"] ?? DefaultHiddenItemSearchColumns;
            return raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        private static (string Name, string Label)[] OrderColumns((string Name, string Label)[] columns, string rawOrder)
        {
            if (string.IsNullOrWhiteSpace(rawOrder))
                return columns;

            var byName = columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<(string Name, string Label)>(columns.Length);

            foreach (var name in rawOrder.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()))
            {
                if (byName.TryGetValue(name, out var col) && seen.Add(name))
                    result.Add(col);
            }
            foreach (var col in columns)
            {
                if (seen.Add(col.Name))
                    result.Add(col);
            }
            return result.ToArray();
        }

        public static (string Name, string Label)[] GetOrderedItemSearchColumns() =>
            OrderColumns(ItemSearchColumns, ConfigurationManager.AppSettings["ITEM_SEARCH_COLUMN_ORDER"]);

        public static (string Name, string Label)[] GetOrderedCartColumns() =>
            OrderColumns(CartColumns, ConfigurationManager.AppSettings["CART_COLUMN_ORDER"]);

        public void ApplyItemSearchColumnVisibility()
        {
            var ordered = GetOrderedItemSearchColumns();
            for (int i = 0; i < ordered.Length; i++)
            {
                var col = ordered[i];
                if (dgv_itemSearch.Columns.Contains(col.Name))
                {
                    dgv_itemSearch.Columns[col.Name].Visible = !IsItemSearchColumnHidden(col.Name);
                    dgv_itemSearch.Columns[col.Name].DisplayIndex = i;
                }
            }
        }

        public static bool ItemSearchDateSortDescending =>
            !string.Equals(ConfigurationManager.AppSettings["ITEM_SEARCH_DATE_SORT_DESCENDING"], "false", StringComparison.OrdinalIgnoreCase);

        public static bool ItemSearchNameSortDescending =>
            string.Equals(ConfigurationManager.AppSettings["ITEM_SEARCH_NAME_SORT_DESCENDING"], "true", StringComparison.OrdinalIgnoreCase);

        private static IOrderedEnumerable<DataRow> OrderRows<TKey>(IEnumerable<DataRow> rows, Func<DataRow, TKey> keySelector, bool descending) =>
            descending ? rows.OrderByDescending(keySelector) : rows.OrderBy(keySelector);

        public static readonly (string Name, string Label)[] CartColumns = new[]
        {
            ("serial", "Serial"),
            ("description", "Particular"),
            ("quantity", "Qty"),
            ("uprice", "Unit Price"),
            ("price", "Price"),
        };

        public const string DefaultHiddenCartColumns = "";

        public static bool IsCartColumnHidden(string columnName)
        {
            string raw = ConfigurationManager.AppSettings["CART_HIDDEN_COLUMNS"] ?? DefaultHiddenCartColumns;
            return raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        public void ApplyCartColumnVisibility()
        {
            var ordered = GetOrderedCartColumns();
            for (int i = 0; i < ordered.Length; i++)
            {
                var col = ordered[i];
                if (dgv_items.Columns.Contains(col.Name))
                {
                    dgv_items.Columns[col.Name].Visible = !IsCartColumnHidden(col.Name);
                    dgv_items.Columns[col.Name].DisplayIndex = i;
                }
            }
        }
        internal class HeldTransaction
        {
            public string TicketCode;
            public string TransactionCode;
            public int SelltypeIndex;
            public decimal GrandTotal;
            public string CounterText;
            public DateTime HeldAt;
            public List<object[]> Items;

            public string Remarks;
        }
        internal readonly Dictionary<string, HeldTransaction> heldTransactions = new Dictionary<string, HeldTransaction>();
        private HeldTransactionsForm heldTransactionsWindow;
        private ItemQuickSearchForm itemQuickSearchWindow;

        internal Point GetCartBoardScreenLocation() => gbx_itemList.PointToScreen(Point.Empty);

        internal void ShowItemQuickSearchWindow()
        {
            if (itemQuickSearchWindow == null || itemQuickSearchWindow.IsDisposed)
            {
                itemQuickSearchWindow = new ItemQuickSearchForm();
                itemQuickSearchWindow.FormClosed += (s, e) => itemQuickSearchWindow = null;
            }

            itemQuickSearchWindow.StartPosition = FormStartPosition.Manual;

            Rectangle workArea = Screen.FromControl(this).WorkingArea;
            Point anchor = heldTransactionsWindow != null && !heldTransactionsWindow.IsDisposed
                ? heldTransactionsWindow.Location
                : GetCartBoardScreenLocation();
            int x = Math.Max(anchor.X - itemQuickSearchWindow.Width, workArea.Left);
            int y = Math.Min(anchor.Y, workArea.Bottom - itemQuickSearchWindow.Height);
            y = Math.Max(y, workArea.Top);
            itemQuickSearchWindow.Location = new Point(x, y);

            itemQuickSearchWindow.Show();
            itemQuickSearchWindow.Activate();
            itemQuickSearchWindow.FocusSearchBox();
        }

        private sealed class CartSnapshot
        {
            public List<object[]> Items;
            public decimal GrandTotal;
            public string CounterText;
            public string TransactionCode;
        }
        private CartSnapshot _preHeldPreviewSnapshot;
        private string _previewedTicketCode;

        // When a MessageBox opened from a shortcut key (e.g. F6) is dismissed with Enter,
        // Windows delivers that Enter's key-up to whatever control regains focus afterward.
        // This suppresses the one resulting KeyUp on tbx_serialcode so it isn't misread as
        internal bool suppressNextSerialKeyUp;

        private bool pendingCartRowSelection;
        private string pendingCartRowSelectionText;

        private const int SelltypeSelectIndex = 3;

        private const int ScannerBurstThresholdMs = 100;
        private readonly System.Windows.Forms.Timer _serialManualKeyTimer = new System.Windows.Forms.Timer { Interval = ScannerBurstThresholdMs };
        private char _pendingManualChar;
        private bool _serialBurstConfirmed;

        // Auto-clears tbx_search after it sits idle (no typing, no mouse movement) for the
        // configured number of seconds (SEARCH_AUTO_CLEAR_ENABLED/_SECONDS, System Config >
        // Alerts). The visible countdown on lbl_searchAutoClearCountdown is what makes it
        // "dismissable": real mouse movement while it's showing counts as activity and cancels
        // the pending clear, via an app-wide WM_MOUSEMOVE message filter rather than a
        // per-control MouseMove (which wouldn't fire while the pointer sits over a child
        // control like the item search grid). WM_MOUSEMOVE fires on sub-pixel sensor jitter too
        // (a mouse just resting on the desk can report movement) - without a minimum-distance
        // threshold the countdown would get cancelled almost immediately every time, which
        // looks indistinguishable from the auto-clear "not working" at all.
        private const int MouseJitterThresholdPixels = 6;
        private readonly System.Windows.Forms.Timer _searchAutoClearTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        private int _searchAutoClearRemainingSeconds;
        private Point _searchAutoClearLastMousePosition;
        private IMessageFilter _searchAutoClearMouseFilter;

        // Resets an abandoned, half-typed tbx_quantity edit back to "1" after it sits idle for
        // the configured number of seconds (QTY_AUTO_RESET_ENABLED/_SECONDS, System Config >
        // Alerts), the same way pressing Escape would. Single-shot per idle period - restarted
        // on every keystroke via RestartQtyAutoResetTimer, stopped by ResetQuantityEditUi so it
        // can't fire again (or re-arm itself) once the edit session has already ended.
        private readonly System.Windows.Forms.Timer _qtyAutoResetTimer = new System.Windows.Forms.Timer();

        public Purchases()
        {
            InitializeComponent();

            _serialManualKeyTimer.Tick += SerialManualKeyTimer_Tick;
            _searchAutoClearTimer.Tick += SearchAutoClearTimer_Tick;
            _qtyAutoResetTimer.Tick += (s, e) => ResetQuantityEditUi();

            tabControlMain.Visible = false;
            statusStripMain.Visible = false;
            menuStripMain.Visible = false;

            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);
            ApplyCartBoardBackground();
            ApplyItemSearchGridBackground();
            FormClosed += (s, e) =>
            {
                if (_layoutEditFilter != null)
                    Application.RemoveMessageFilter(_layoutEditFilter);
                Application.RemoveMessageFilter(_transactionShortcutFilter);
            };

            _transactionShortcutFilter.Triggered += (s, e) => ShowHeldTransactionsWindow();
            Application.AddMessageFilter(_transactionShortcutFilter);

            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            trayIcon.Text = "BrewCode Innovations";

            statusStripMain.BringToFront();
            void ResizeTabControlAboveStatusBar() =>
                tabControlMain.Height = Math.Max(0, this.ClientSize.Height - tabControlMain.Top - statusStripMain.Height);
            ResizeTabControlAboveStatusBar();
            this.Resize += (s, e) => ResizeTabControlAboveStatusBar();

            // Controls are anchored to grow with the window; below this size they'd start
            // overlapping, so don't let the window shrink past its original designed size.
            this.MinimumSize = this.Size;

            // Use the embedded copy of Morally Serif so the store name renders correctly
            // on machines where the font isn't installed. GDI text rendering can't see
            // memory fonts, so this label must use GDI+ (compatible text rendering).
            lbl_storeTitle.UseCompatibleTextRendering = true;
            lbl_storeTitle.Font = new Font(AppFonts.MorallySerif, 30F, FontStyle.Bold);

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
            lbl_storeTitle.Text = L_DEFAULTNAME;
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

        private const int NetworkTitleBarRefreshMs = 15 * 60 * 1000;
        private System.Windows.Forms.Timer networkTitleBarTimer;

        private async Task RefreshNetworkTitleBarAsync()
        {
            string pingResult = await netTesting();
            UpdateTitleBar();
            UpdateNetworkSpeedStatus(pingResult);
        }

        /// <summary>Mirrors the title bar's ping readout onto the status bar - same
        /// lastPingRoundtripMs/SlowPingThresholdMs classification used by RunNetworkDiagnosticsAsync,
        /// so the color here always agrees with whether that method would warn.</summary>
        private void UpdateNetworkSpeedStatus(string pingResult)
        {
            if (lastPingRoundtripMs == null)
            {
                lblNetworkSpeedStatus.Text = "📶 No connection";
                lblNetworkSpeedStatus.ForeColor = Color.Firebrick;
                lblNetworkSpeedStatus.ToolTipText = "Cannot reach the database server.";
            }
            else if (lastPingRoundtripMs > SlowPingThresholdMs)
            {
                lblNetworkSpeedStatus.Text = $"📶 {pingResult}";
                lblNetworkSpeedStatus.ForeColor = Color.Firebrick;
                lblNetworkSpeedStatus.ToolTipText = $"Network connection to the database is slow (threshold: {SlowPingThresholdMs}ms).";
            }
            else
            {
                lblNetworkSpeedStatus.Text = $"📶 {pingResult}";
                lblNetworkSpeedStatus.ForeColor = Color.DarkGreen;
                lblNetworkSpeedStatus.ToolTipText = "Network connection to the database server is healthy.";
            }
        }

        private void StartNetworkTitleBarWatcher()
        {
            networkTitleBarTimer = new System.Windows.Forms.Timer { Interval = NetworkTitleBarRefreshMs };
            networkTitleBarTimer.Tick += async (s, e) => await RefreshNetworkTitleBarAsync();
            networkTitleBarTimer.Start();

            this.FormClosed += (s, e) => networkTitleBarTimer.Stop();
        }

        private async Task RunNetworkDiagnosticsAsync()
        {
            await RefreshNetworkTitleBarAsync();

            string dbServerSetting = ConfigurationManager.AppSettings["DB_SERVER"]?.Trim();
            if (lastPingRoundtripMs == null)
            {
                ErrorDialog.Show(this,
                    "Cannot reach the database server. Check your internet connection before proceeding - login, scanning, and payments will likely fail.",
                    "Network Warning", $"DB_SERVER={dbServerSetting}\nPing: no reply within timeout.", MessageBoxIcon.Warning);
            }
            else if (lastPingRoundtripMs > SlowPingThresholdMs)
            {
                ErrorDialog.Show(this,
                    $"Network connection to the database is slow ({lastPingRoundtripMs}ms). Item lookups and checkout may lag.",
                    "Network Warning", $"DB_SERVER={dbServerSetting}\nRound-trip time: {lastPingRoundtripMs}ms (threshold: {SlowPingThresholdMs}ms).", MessageBoxIcon.Warning);
            }
        }

        private void UpdateTitleBar()
        {
            string title = string.IsNullOrEmpty(_loggedInCode)
                ? $"POS - {S_DEFAULTNAME}"
                : $"POS - {S_DEFAULTNAME} - {_loggedInCode}";

            if (DataCacheManager.Instance.IsOfflineMode)
            {
                DateTime? lastUpdated = DataCacheManager.Instance.GetLastUpdated("Items");
                title += lastUpdated.HasValue
                    ? $" [OFFLINE - items as of {lastUpdated:MMMM dd, yyyy hh:mm tt}]"
                    : " [OFFLINE]";
            }

            if (TrialLicense.IsEnabled && !TrialLicense.IsLicensed)
                title += $" [TRIAL - {TrialLicense.DaysRemaining} day(s) left]";

            this.Text = title;
        }

        private const int StaleCacheHours = 24;

        /// <summary>Compact, icon-led status bar readout (click opens the Inventory Report) -
        /// full detail (exact refresh time, which specific thing is wrong) lives in the tooltip
        /// rather than in the visible text, which only ever shows the item count plus one glyph.</summary>
        private void UpdateCachedItemsStatusMenu()
        {
            int count;
            try
            {
                count = DataCacheManager.Instance.GetTable("Items").Rows.Count;
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            DateTime? asOf = DataCacheManager.Instance.GetLastUpdated("Items");

            bool isStale = asOf.HasValue && DateTime.Now - asOf.Value > TimeSpan.FromHours(StaleCacheHours);
            int issueCount = DataCacheManager.Instance.CountItemIntegrityIssues();
            int pendingSyncCount = PendingItemStore.GetPendingCount();
            bool hasWarning = isStale || issueCount > 0 || pendingSyncCount > 0;

            lblCachedItemsStatus.Text = (hasWarning ? "⚠ " : "📦 ") + count.ToString("N0");
            lblCachedItemsStatus.ForeColor = hasWarning ? Color.Firebrick : SystemColors.ControlText;

            string itemWord = count == 1 ? "item" : "items";
            string tooltip = asOf.HasValue
                ? $"{count:N0} {itemWord} available for offline use - last refreshed {asOf:MMMM dd, yyyy hh:mm tt}."
                : $"{count:N0} {itemWord} available for offline use.";
            if (isStale)
                tooltip += $" This data is more than {StaleCacheHours} hour(s) old - prices/stock may no longer be accurate.";
            if (issueCount > 0)
                tooltip += $" {issueCount} item(s) are missing a serial or a usable price and may not scan or sell correctly - click to review them.";
            tooltip += pendingSyncCount > 0
                ? $" {pendingSyncCount} item(s) are queued offline and not yet synced to the database."
                : " Cache and database are in sync.";
            lblCachedItemsStatus.ToolTipText = tooltip;
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
            if (!DataCacheManager.Instance.IsOfflineMode)
                return;

            Purchases purchases = Application.OpenForms.OfType<Purchases>().FirstOrDefault();
            if (purchases != null)
                _ = purchases.SyncInventoryWithDatabaseAsync(isAutomatic: true);
            else
                RefreshItemsCache();
        }

        /// <summary>
        /// Pushes any offline-queued items (added via AddItemForm while unreachable) to the
        /// database, then refreshes the cache from it - shared by the automatic reconnect watcher
        /// (TryRefreshCacheIfReconnected) and a manual click on lblInventorySyncStatus. Without
        /// the push, a plain cache refresh would keep showing queued items only in the local
        /// cache, never actually syncing them to the database itself. Surfaces progress as a tray
        /// balloon, since the automatic path runs from a poll/network-change callback with nobody
        /// necessarily watching the status bar at the moment it starts.
        /// </summary>
        private async Task SyncInventoryWithDatabaseAsync(bool isAutomatic)
        {
            if (IsSyncingInventory)
                return;

            if (PendingItemSyncService.AutoSyncEnabled)
            {
                int pendingBefore = PendingItemStore.GetPendingCount();
                string reconnectedPrefix = isAutomatic ? "Reconnected - s" : "S";
                if (pendingBefore > 0)
                    ShowTrayNotification("Inventory Sync",
                        $"{reconnectedPrefix}yncing {pendingBefore} offline-queued item(s) to the database...",
                        ToolTipIcon.Info);

                IsSyncingInventory = true;
                UpdateInventorySyncStatus();

                try
                {
                    int synced = await PendingItemSyncService.SyncPendingItemsAsync();

                    if (synced > 0)
                    {
                        int remaining = PendingItemStore.GetPendingCount();
                        ShowTrayNotification("Inventory Sync Complete",
                            remaining == 0
                                ? $"Synced {synced} item(s). Cache and database are now in sync."
                                : $"Synced {synced} item(s). {remaining} item(s) need manual review (duplicate serial already in the database).",
                            ToolTipIcon.Info);
                    }
                    else if (pendingBefore == 0)
                    {
                        ShowTrayNotification("Inventory Sync",
                            (isAutomatic ? "Reconnected - c" : "C") + "ache and database are already in sync.",
                            ToolTipIcon.Info);
                    }
                }
                catch
                {
                    // Best-effort - the refresh below still runs either way, and the next
                    // reconnect (or another click here, or System Config > Offline Sync) retries.
                    ShowTrayNotification("Inventory Sync",
                        "Syncing offline-queued items failed - it will retry automatically.",
                        ToolTipIcon.Warning);
                }
                finally
                {
                    IsSyncingInventory = false;
                }
            }

            RefreshItemsCache();
            UpdateInventorySyncStatus();
        }

        /// <summary>Lets a cashier/admin force a sync on demand instead of waiting for the next
        /// automatic reconnect poll - re-entrant clicks while one's already running are ignored
        /// (SyncInventoryWithDatabaseAsync's own IsSyncingInventory guard).</summary>
        private async void lblInventorySyncStatus_Click(object sender, EventArgs e) =>
            await SyncInventoryWithDatabaseAsync(isAutomatic: false);

        /// <summary>Hand cursor over lblInventorySyncStatus specifically - ToolStripItem has no
        /// Cursor property of its own (unlike a real Control), so this is done by hand at the
        /// owning StatusStrip level instead.</summary>
        private void statusStripMain_MouseMove(object sender, MouseEventArgs e)
        {
            ToolStripItem hovered = statusStripMain.GetItemAt(e.Location);
            statusStripMain.Cursor = hovered == lblInventorySyncStatus ? Cursors.Hand : Cursors.Default;
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
        /// Polls app_config_'s MAINTENANCE_MODE flag (via AppConfigService, which is a live DB
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

            ErrorDialog.Show(this,
                "Maintenance mode is now active and no admin override was entered. The application will now close.",
                "Maintenance Mode", $"Maintenance message from server: \"{message}\"", MessageBoxIcon.Warning);
            this.Close();
        }

        private System.Windows.Forms.Timer trialExpiryCheckTimer;
        private const int TrialExpiryCheckPollMs = 2 * 60 * 1000;

        /// <summary>
        /// Polls TrialLicense's local, config/state-file-backed expiry checks (no DB round trip,
        /// unlike StartMaintenanceWatcher) so a terminal left running across its trial or license
        /// expiry date closes itself instead of only being gated at the next restart - opt-in via
        /// TRIAL_AUTO_CLOSE_ENABLED (System Config > Trial / Licensing) since existing installs
        /// expect today's startup-only gate by default.
        /// </summary>
        private void StartTrialExpiryWatcher()
        {
            trialExpiryCheckTimer = new System.Windows.Forms.Timer { Interval = TrialExpiryCheckPollMs };
            trialExpiryCheckTimer.Tick += (s, e) => CheckTrialExpiry();
            trialExpiryCheckTimer.Start();

            this.FormClosed += (s, e) => trialExpiryCheckTimer.Stop();
        }

        private void CheckTrialExpiry()
        {
            if (!TrialLicense.AutoCloseOnExpiryEnabled)
                return;

            if (!TrialLicense.IsExpired && !TrialLicense.HasExpiredLicenseKey)
                return;

            trialExpiryCheckTimer.Stop();

            ErrorDialog.Show(this,
                "Your trial or license has expired. The application will now close.",
                "Trial/License Expired",
                $"IsExpired={TrialLicense.IsExpired}, HasExpiredLicenseKey={TrialLicense.HasExpiredLicenseKey}, LicenseExpiryUtc={TrialLicense.LicenseExpiryUtc}",
                MessageBoxIcon.Warning);
            this.Close();
        }

        private System.Windows.Forms.Timer lowStockCheckTimer;
        private const int LowStockCheckPollMs = 2 * 60 * 1000;
        private DateTime _lastLowStockReminderShownUtc = DateTime.MinValue;

        /// <summary>
        /// Polls app_config_'s low-stock reminder settings (via AppConfigService, same reasoning
        /// as StartMaintenanceWatcher - a live DB read so a threshold/interval change from
        /// another terminal's System Config takes effect here without a restart). Checked every
        /// LowStockCheckPollMs, but the popup itself only actually shows once per the admin's
        /// configured reminder interval (_lastLowStockReminderShownUtc), not on every poll.
        /// </summary>
        private void StartLowStockReminderWatcher()
        {
            lowStockCheckTimer = new System.Windows.Forms.Timer { Interval = LowStockCheckPollMs };
            lowStockCheckTimer.Tick += async (s, e) => await CheckLowStockReminderAsync();
            lowStockCheckTimer.Start();

            this.FormClosed += (s, e) => lowStockCheckTimer.Stop();

            // Also check shortly after login, so staff starting a shift see it then rather than
            // waiting out the first poll interval.
            _ = CheckLowStockReminderAsync();
        }

        private async Task CheckLowStockReminderAsync()
        {
            (bool enabled, int threshold, int intervalMinutes) = await AppConfigService.GetLowStockReminderSettingsAsync();
            if (!enabled)
                return;

            if (DateTime.UtcNow - _lastLowStockReminderShownUtc < TimeSpan.FromMinutes(intervalMinutes))
                return;

            var lowStockItems = GetLowStockItems(threshold);
            if (lowStockItems.Count == 0)
                return;

            _lastLowStockReminderShownUtc = DateTime.UtcNow;

            using (var reminderForm = new LowStockReminderForm(lowStockItems, threshold))
            {
                reminderForm.ShowDialog(this);
            }
        }

        private System.Windows.Forms.Timer updateCheckTimer;
        private const int UpdateCheckPollMs = 24 * 60 * 60 * 1000;
        private Form _updateAvailableForm;
        private string _dismissedUpdateVersion;
        private PendingSelfUpdate _pendingSelfUpdate;
        private bool _isDownloadingSelfUpdate;

        /// <summary>
        /// Polls GitHub's Releases API (via UpdateCheckService) for a newer published version -
        /// 24h, not the 2-minute pattern used by the other watchers, since a release doesn't need
        /// near-real-time reaction and a short interval risks GitHub's unauthenticated 60-req/hour
        /// rate limit across multiple terminals sharing one store's public IP. Opt-in via
        /// UPDATE_CHECK_ENABLED (System Config > Updates), same reasoning as Maintenance/Low Stock
        /// - a store-wide policy, not a per-machine preference.
        /// </summary>
        private void StartUpdateCheckWatcher()
        {
            updateCheckTimer = new System.Windows.Forms.Timer { Interval = UpdateCheckPollMs };
            updateCheckTimer.Tick += async (s, e) => await CheckForUpdateAsync();
            updateCheckTimer.Start();

            this.FormClosed += (s, e) => updateCheckTimer.Stop();

            // Also check shortly after login, so a terminal doesn't wait out the first 24h poll.
            _ = CheckForUpdateAsync();
        }

        private async Task CheckForUpdateAsync()
        {
            bool enabled = await AppConfigService.GetUpdateCheckEnabledAsync();
            if (!enabled)
                return;

            UpdateInfo info = await UpdateCheckService.CheckForUpdateAsync();
            if (info == null)
                return;

            if (info.TagName == _dismissedUpdateVersion || info.TagName == _pendingSelfUpdate?.Info.TagName)
                return;

            bool autoUpdateEnabled = await AppConfigService.GetAutoUpdateEnabledAsync();
            if (autoUpdateEnabled && !string.IsNullOrWhiteSpace(info.AssetDownloadUrl))
            {
                _ = DownloadAndStageSelfUpdateAsync(info);
                return;
            }

            if (_updateAvailableForm != null && !_updateAvailableForm.IsDisposed)
                return;

            _updateAvailableForm = UpdateAvailableForm.Show(this, info);
            _updateAvailableForm.FormClosed += (s, e) => _dismissedUpdateVersion = info.TagName;
        }

        /// <summary>
        /// Runs in the background, independent of the 24h poll loop that kicked it off - a slow
        /// download should never block CheckForUpdateAsync's caller. Fails open (logs and retries
        /// on the next poll) on any download failure, same convention as UpdateCheckService.
        /// </summary>
        private async Task DownloadAndStageSelfUpdateAsync(UpdateInfo info)
        {
            if (_isDownloadingSelfUpdate)
                return;

            _isDownloadingSelfUpdate = true;
            try
            {
                string zipPath = await SelfUpdateService.DownloadUpdateZipAsync(info);
                if (zipPath == null)
                    return;

                _pendingSelfUpdate = new PendingSelfUpdate { Info = info, ZipPath = zipPath };
                OperationLog.RecordAction(nameof(Purchases), $"Downloaded self-update {info.TagName}; staged for install when idle.");

                if (_updateAvailableForm != null && !_updateAvailableForm.IsDisposed)
                    _updateAvailableForm.Close();

                _updateAvailableForm = UpdateAvailableForm.ShowReadyToInstall(this, info);
            }
            finally
            {
                _isDownloadingSelfUpdate = false;
            }
        }

        /// <summary>
        /// Shares the idle-cart timer's 60s tick rather than its own Timer - no need for a second
        /// poll loop just for this. "Idle" here means: empty cart, no held-transaction preview
        /// being viewed, and no other window open (Application.OpenForms.Count == 1 is an
        /// approximation - there's no generalized modal tracker in this app to check against).
        /// </summary>
        private void CheckPendingSelfUpdateInstall()
        {
            if (_pendingSelfUpdate == null)
                return;

            if (dgv_items.Rows.Count > 0)
                return;

            if (_preHeldPreviewSnapshot != null)
                return;

            if (Application.OpenForms.Count > 1)
                return;

            TriggerSelfUpdateInstall();
        }

        private void TriggerSelfUpdateInstall()
        {
            PendingSelfUpdate update = _pendingSelfUpdate;

            if (!SelfUpdateService.LaunchInstaller(update.ZipPath))
            {
                OperationLog.RecordAction(nameof(Purchases), $"Failed to launch self-update installer for {update.Info.TagName}; will retry.");
                return;
            }

            _pendingSelfUpdate = null;
            OperationLog.RecordAction(nameof(Purchases), $"Closing to apply self-update {update.Info.TagName} in the background.");
            Application.Exit();
        }

        /// <summary>
        /// Reads from the already-cached Items table (DataCacheManager), same as the existing
        /// "Out of Stock Items" dashboard panel's query at LoadDashboardSalesAsync - no extra DB
        /// round trip, and it's kept fresh by the same post-sale RefreshItemsCache() calls that
        /// panel relies on.
        /// </summary>
        private List<LowStockItemInfo> GetLowStockItems(int threshold)
        {
            var items = DataCacheManager.Instance.GetTable("Items");
            if (items == null)
                return new List<LowStockItemInfo>();

            return items.AsEnumerable()
                .Select(row => new LowStockItemInfo
                {
                    Serial = row["serial"]?.ToString() ?? "",
                    Description = row["description"]?.ToString() ?? "",
                    Quantity = row["new_quantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["new_quantity"]),
                })
                .Where(i => i.Quantity <= threshold)
                .OrderBy(i => i.Quantity)
                .ThenBy(i => i.Description)
                .ToList();
        }

        private System.Windows.Forms.Timer idleCartCheckTimer;
        private const int IdleCartPollMs = 60 * 1000;
        private DateTime _lastCartActivity = DateTime.Now;

        private void StartIdleCartWatcher()
        {
            idleCartCheckTimer = new System.Windows.Forms.Timer { Interval = IdleCartPollMs };
            idleCartCheckTimer.Tick += (s, e) => CheckIdleCart();
            idleCartCheckTimer.Tick += (s, e) => CheckPendingSelfUpdateInstall();
            idleCartCheckTimer.Start();

            this.FormClosed += (s, e) => idleCartCheckTimer.Stop();
        }

        private void CheckIdleCart()
        {
            if (dgv_items.Rows.Count == 0)
                return;

            if (_preHeldPreviewSnapshot != null)
                return;

            bool enabled = string.Equals(ConfigurationManager.AppSettings["AUTO_HOLD_IDLE_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);
            if (!enabled)
                return;

            int minutes = int.TryParse(ConfigurationManager.AppSettings["AUTO_HOLD_IDLE_MINUTES"], out int m) && m > 0 ? m : 30;
            if (DateTime.Now - _lastCartActivity < TimeSpan.FromMinutes(minutes))
                return;

            HoldCurrentTransaction($"Auto-held after {minutes} minute(s) of inactivity.", silent: true);
        }

        private void StartSearchAutoClearWatcher()
        {
            _searchAutoClearMouseFilter = new MouseMoveMessageFilter(CheckMouseActivityForSearchAutoClear);
            Application.AddMessageFilter(_searchAutoClearMouseFilter);

            this.FormClosed += (s, e) =>
            {
                _searchAutoClearTimer.Stop();
                Application.RemoveMessageFilter(_searchAutoClearMouseFilter);
            };
        }

        private static bool SearchAutoClearEnabled =>
            !string.Equals(ConfigurationManager.AppSettings["SEARCH_AUTO_CLEAR_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);

        private static int SearchAutoClearSeconds =>
            int.TryParse(ConfigurationManager.AppSettings["SEARCH_AUTO_CLEAR_SECONDS"], out int s) && s > 0 ? s : 10;

        /// <summary>Called from tbx_search_TextChanged: (re)starts the idle countdown while there's
        /// text to clear, or cancels it once the box is already empty.</summary>
        private void RestartSearchAutoClearCountdown()
        {
            if (!SearchAutoClearEnabled || string.IsNullOrEmpty(tbx_search.Text))
            {
                DismissSearchAutoClearCountdown();
                return;
            }

            _searchAutoClearRemainingSeconds = SearchAutoClearSeconds;
            _searchAutoClearLastMousePosition = Cursor.Position;
            lbl_searchAutoClearCountdown.Text =
                $"Clearing search in {_searchAutoClearRemainingSeconds}s (move the mouse to cancel)...";
            lbl_searchAutoClearCountdown.Visible = true;
            _searchAutoClearTimer.Start();
        }

        private void DismissSearchAutoClearCountdown()
        {
            if (!_searchAutoClearTimer.Enabled && !lbl_searchAutoClearCountdown.Visible)
                return;

            _searchAutoClearTimer.Stop();
            lbl_searchAutoClearCountdown.Visible = false;
        }

        /// <summary>Only counts as "the pointer moved" once the cursor has strayed more than
        /// <see cref="MouseJitterThresholdPixels"/> from where it was when the countdown started -
        /// a stationary mouse can still emit WM_MOUSEMOVE from sensor noise, and reacting to that
        /// would cancel the countdown within the first second almost every time.</summary>
        private void CheckMouseActivityForSearchAutoClear()
        {
            if (!lbl_searchAutoClearCountdown.Visible)
                return;

            Point current = Cursor.Position;
            int dx = current.X - _searchAutoClearLastMousePosition.X;
            int dy = current.Y - _searchAutoClearLastMousePosition.Y;

            if ((dx * dx) + (dy * dy) >= MouseJitterThresholdPixels * MouseJitterThresholdPixels)
                DismissSearchAutoClearCountdown();
        }

        private void SearchAutoClearTimer_Tick(object sender, EventArgs e)
        {
            _searchAutoClearRemainingSeconds--;

            if (_searchAutoClearRemainingSeconds <= 0)
            {
                _searchAutoClearTimer.Stop();
                lbl_searchAutoClearCountdown.Visible = false;
                tbx_search.Clear();
                return;
            }

            lbl_searchAutoClearCountdown.Text =
                $"Clearing search in {_searchAutoClearRemainingSeconds}s (move the mouse to cancel)...";
        }

        /// <summary>Catches WM_MOUSEMOVE for the whole app, not just this form, since WinForms
        /// mouse events don't bubble from a child control (the search grid, buttons, etc.) up to
        /// the form - a plain Form.MouseMove handler would almost never fire.</summary>
        private sealed class MouseMoveMessageFilter : IMessageFilter
        {
            private const int WM_MOUSEMOVE = 0x0200;
            private readonly Action _onMouseMove;

            public MouseMoveMessageFilter(Action onMouseMove) => _onMouseMove = onMouseMove;

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_MOUSEMOVE)
                    _onMouseMove();
                return false;
            }
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

        private void RecomputeCartTotals()
        {
            grandTotal = 0;
            count = 0;

            for (int i = 0; i < dgv_items.Rows.Count; i++)
            {
                grandTotal += ToDecimal(dgv_items.Rows[i].Cells[4].Value);
                count += ToInt(dgv_items.Rows[i].Cells[2].Value);
            }

            counter.Text = BuildCounterText(grandTotal, $"Total Items: {count}");
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

            PendingItemStore.AdjustQuantityIfPending(serial, delta);
        }

        /// <summary>Same as <see cref="AdjustCachedQuantity"/> but sets an absolute value instead of a delta.</summary>
        private static void SetCachedQuantity(string serial, long value)
        {
            var row = FindItemRow(serial);
            if (row == null)
                return;

            row["new_quantity"] = value;
            PendingItemStore.SetQuantityIfPending(serial, value);
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

            OperationLog.RecordSale(nameof(Purchases), transactionCode, log_in, grandTotal, lineItems);

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
        private static bool BlockZeroPriceItemsEnabled =>
            !string.Equals(ConfigurationManager.AppSettings["BLOCK_ZERO_PRICE_ITEMS"], "false", StringComparison.OrdinalIgnoreCase);
        public decimal GetItemPrice(String itemcode)
        {
            decimal price_value = 0;

            var row = FindItemRow(itemcode);
            if (row == null)
                return price_value;

            if (selltype.SelectedIndex == 0)
            {
                price_value = ToDecimal(row["price"]);
            }
            else if (selltype.SelectedIndex == 1)
            {
                price_value = ToDecimal(row["retail_price"]);
            }
            else if (selltype.SelectedIndex == 2)
            {
                price_value = ToDecimal(row["price_perpc"]);
            }
            return price_value;
        }
        private void UpdateTime()
        {
            lbl_warning.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");

            lbl_warning.Font = new System.Drawing.Font("Arial", 15, System.Drawing.FontStyle.Italic);
        }

        private void UpdateLastTransactionCodeMenu(string transactionCode)
        {
            transactionListToolStripMenuItem.Text = string.IsNullOrEmpty(transactionCode)
                ? "No transactions yet"
                : transactionCode;
        }

        private async Task LoadLastTransactionCodeAsync()
        {
            try
            {
                await ConnectDb();
                if (connect_db == null || connect_db.State != ConnectionState.Open)
                    return;

                try
                {
                    using (var cmd = new MySqlCommand(
                        "SELECT transaction_code FROM sales_ ORDER BY id DESC LIMIT 1", connect_db))
                    {
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                            UpdateLastTransactionCodeMenu(StripLineItemSuffix(result.ToString()));
                    }
                }
                finally
                {
                    connect_db?.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        private static string StripLineItemSuffix(string salesRowTransactionCode)
        {
            int dash = salesRowTransactionCode.LastIndexOf('-');
            if (dash > 0 && int.TryParse(salesRowTransactionCode.Substring(dash + 1), out _))
                return salesRowTransactionCode.Substring(0, dash);
            return salesRowTransactionCode;
        }

        private void UpdateOfflineItemsStatus()
        {
            DateTime? lastUpdated = YourApp.Data.PendingItemStore.GetLastUpdated();
            if (lastUpdated == null)
            {
                lblOfflineItemsStatus.Text = "";
                return;
            }

            int pendingCount = YourApp.Data.PendingItemStore.GetPendingCount();
            string itemWord = pendingCount == 1 ? "item" : "items";
            lblOfflineItemsStatus.Text =
                $"Offline {itemWord} pending sync: {pendingCount} - last updated {lastUpdated:MMMM dd, yyyy hh:mm tt}";
        }

        /// <summary>Always-visible status bar readout of when the cached Items table (prices and
        /// stock) last actually pulled from the live database - distinct from
        /// lblOfflineItemsStatus, which only reports the offline-queued-items count/age and goes
        /// blank once nothing's pending.</summary>
        // Set/cleared around PendingItemSyncService's call in TryRefreshCacheIfReconnected so the
        // status bar can show "Syncing..." for that window instead of just jumping straight from
        // one last-synced timestamp to the next with nothing in between.
        internal bool IsSyncingInventory;

        /// <summary>Compact, icon-led sync-state readout - the visible text is just a glyph plus
        /// one word; the exact timestamp and pending-item count live in the tooltip instead of
        /// being spelled out in the status bar itself.</summary>
        private void UpdateInventorySyncStatus()
        {
            DateTime? lastSynced = DataCacheManager.Instance.GetLastUpdated("Items");
            int pendingCount = YourApp.Data.PendingItemStore.GetPendingCount();
            string lastSyncedText = lastSynced.HasValue
                ? $"last synced {lastSynced:MMMM dd, yyyy hh:mm tt}"
                : "not yet synced with the database";

            if (IsSyncingInventory)
            {
                lblInventorySyncStatus.Text = "🔄 Syncing";
                lblInventorySyncStatus.ForeColor = Color.DarkOrange;
                lblInventorySyncStatus.ToolTipText = $"Syncing offline-queued items with the database now ({lastSyncedText}).";
            }
            else if (DataCacheManager.Instance.IsOfflineMode)
            {
                lblInventorySyncStatus.Text = "📴 Offline";
                lblInventorySyncStatus.ForeColor = Color.Firebrick;
                lblInventorySyncStatus.ToolTipText = $"Database unreachable - running on the local cache ({lastSyncedText}). Click to retry.";
            }
            else if (pendingCount > 0)
            {
                lblInventorySyncStatus.Text = $"⚠ Out of sync ({pendingCount})";
                lblInventorySyncStatus.ForeColor = Color.Firebrick;
                string itemWord = pendingCount == 1 ? "item" : "items";
                lblInventorySyncStatus.ToolTipText = $"{pendingCount} {itemWord} queued offline, not yet in the database ({lastSyncedText}). Click to sync now.";
            }
            else
            {
                // Both directions are actually in sync here: DB -> cache (DataCacheManager's last
                // live pull) and cache -> DB (pendingCount == 0, nothing left queued to push).
                lblInventorySyncStatus.Text = lastSynced.HasValue
                    ? $"✅ Synced DB/C ({lastSynced:MMM d, h:mm tt})"
                    : "✅ Synced DB/C";
                lblInventorySyncStatus.ForeColor = Color.DarkGreen;
                lblInventorySyncStatus.ToolTipText = $"Cache and database are in sync ({lastSyncedText}). Click to sync again.";
            }
        }
        private void Purchases_Load(object sender, EventArgs e)
        {

            try
            {
                ApplyMenuVisibility();
                UpdateCachedItemsStatusMenu();
                UpdateLastTransactionCodeMenu(null);

                _ = RunNetworkDiagnosticsAsync();

                _ = LoadLastTransactionCodeAsync();

                Login login = new Login();

                this.WindowState = FormWindowState.Maximized;

                MainLayoutStore.ApplyTo(this);

                this.Enabled = false;

                login.ShowDialog(this);

                if (!login.IsAuthenticated)
                {
                    ErrorDialog.Show(this, "Login failed or cancelled. The application will now close.", "Access Denied",
                        $"Login dialog closed with IsAuthenticated=false at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                this.Enabled = true;

                tabControlMain.Visible = true;
                statusStripMain.Visible = true;
                menuStripMain.Visible = true;

                if (TrialLicense.IsEnabled && !TrialLicense.IsLicensed)
                {
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

                UpdateGreeting();

                dgv_items.ReadOnly = true;

                timerClock = new System.Windows.Forms.Timer();

                timerClock.Interval = 1000;

                timerClock.Tick += timerClock_Tick;

                gbx_itemsearch.Enabled = true;

                UpdateTime();
                UpdateOfflineItemsStatus();
                UpdateInventorySyncStatus();

                timerClock.Start();

                StartCacheReconnectWatcher();
                StartLastSeenHeartbeat();
                StartMaintenanceWatcher();
                StartTrialExpiryWatcher();
                StartLowStockReminderWatcher();
                StartUpdateCheckWatcher();
                StartNetworkTitleBarWatcher();
                StartIdleCartWatcher();
                StartSearchAutoClearWatcher();
                RestoreHeldTransactions();
                RestoreManagePricesHistory();

                this.Width = 1000;

                this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                dgv_itemSearch.SuspendLayout();

                dgv_itemSearch.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

                dgv_itemSearch.AllowUserToAddRows = false;

                dgv_itemSearch.Columns.Add("serial", "Serial");
                dgv_itemSearch.Columns.Add("description", "Particular");
                dgv_itemSearch.Columns.Add("price", "Price");
                dgv_itemSearch.Columns.Add("qty", "Stock/Qty");

                dataTable.Columns.Add("serial");
                dataTable.Columns.Add("description");
                dataTable.Columns.Add("price");

                dgv_itemSearch.Columns[2].DefaultCellStyle.Format = "0.00";
                dgv_itemSearch.Columns[0].Width = 100;
                dgv_itemSearch.Columns[1].Width = 200;
                dgv_itemSearch.Columns[3].Width = 70;

                ApplyItemSearchColumnVisibility();

                dgv_itemSearch.ResumeLayout();

                dgv_items.SuspendLayout();

                dgv_items.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

                dgv_items.AllowUserToAddRows = false;

                dgv_items.Columns.Add("serial", "Serial");
                dgv_items.Columns.Add("description", "Particular");
                dgv_items.Columns.Add("quantity", "Qty");
                dgv_items.Columns.Add("uprice", "Unit Price");
                dgv_items.Columns.Add("price", "Price");
                dgv_items.Columns.Add(QtyStateColumnName, "QtyState");
                dgv_items.Columns[QtyStateColumnIndex].Visible = false;
                dgv_items.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv_items.Columns[0].FillWeight = 100; // Serial
                dgv_items.Columns[1].FillWeight = 100; // Particular
                dgv_items.Columns[2].FillWeight = 50;  // Qty
                dgv_items.Columns[3].FillWeight = 50;  // Unit Price
                dgv_items.Columns[4].FillWeight = 100; // Price
                dgv_items.Columns[3].DefaultCellStyle.Format = "0.00"; // Unit Price
                dgv_items.Columns[4].DefaultCellStyle.Format = "0.00"; // Price

                ApplyCartColumnVisibility();

                dgv_items.ResumeLayout();

                UpdateCartEmptyLogo();

                ContextMenuStrip strip = new ContextMenuStrip();
                ToolStripMenuItem SaveToPNG = new ToolStripMenuItem("Save As PNG");
                ToolStripMenuItem Print = new ToolStripMenuItem("Print");
                SaveToPNG.Click += SaveToPNG_Click;
                Print.Click += Print_Click;
                strip.Items.Add(SaveToPNG);
                strip.Items.Add(Print);
                dgv_itemSearch.ContextMenuStrip = strip;

                this.Show();

                _ = LoadReceiptLogoAsync();

                tbx_search.Enabled = false;

                tbx_serialcode.Enabled = true;

                tbx_serialcode.Focus();

                if (string.IsNullOrEmpty(selltype.Text) &&
                    string.Equals(ConfigurationManager.AppSettings["DEFAULT_SELLTYPE_ENABLED"], "true", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(ConfigurationManager.AppSettings["DEFAULT_SELLTYPE_INDEX"], out int defaultSelltypeIndex) &&
                    defaultSelltypeIndex >= 0 && defaultSelltypeIndex <= 2)
                {
                    selltype.SelectedIndex = defaultSelltypeIndex;
                }

                if (!TutorialState.HasBeenShown)
                {
                    using (var tutorial = new TutorialForm())
                        tutorial.ShowDialog(this);

                    TutorialState.MarkShown();
                }

            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not connect to the database. The application will now close.", "Database Connection Failed", ex);
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
                ErrorDialog.Show(this, "Could not save the barcode image. Check your internet connection and try again.", "Save Barcode", ex);
            }
        }
        private void Print_Click(object sender, EventArgs e)
        {
            GenerateBarcode generateBarcodeForm = new GenerateBarcode();
            generateBarcodeForm.code = dgv_itemSearch.CurrentRow.Cells[0].Value.ToString();
            generateBarcodeForm.Show();
        }
        private void textchanged_tbx_serialcode(object sender, EventArgs e)
        {
            pendingCartRowSelection = false;

            if (string.IsNullOrEmpty(tbx_serialcode.Text))
                _serialBurstConfirmed = false;
        }

        private void tbx_serialcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            e.Handled = true;

            if (char.IsLetter(e.KeyChar))
            {
                bool hadPending = _serialManualKeyTimer.Enabled;
                char pendingChar = _pendingManualChar;
                _serialManualKeyTimer.Stop();

                tbx_search.Focus();
                tbx_search.Text += hadPending ? $"{pendingChar}{e.KeyChar}" : e.KeyChar.ToString();
                tbx_search.SelectionStart = tbx_search.Text.Length;
                return;
            }

            if (_serialBurstConfirmed || !string.IsNullOrEmpty(tbx_serialcode.Text))
            {
                _serialManualKeyTimer.Stop();
                _serialBurstConfirmed = true;
                InsertIntoSerialCode(e.KeyChar);
                return;
            }

            if (_serialManualKeyTimer.Enabled)
            {
                _serialManualKeyTimer.Stop();
                _serialBurstConfirmed = true;
                InsertIntoSerialCode(_pendingManualChar);
                InsertIntoSerialCode(e.KeyChar);
                return;
            }

            _pendingManualChar = e.KeyChar;
            _serialManualKeyTimer.Start();
        }

        private void InsertIntoSerialCode(char c)
        {
            int start = tbx_serialcode.SelectionStart;
            int len = tbx_serialcode.SelectionLength;
            tbx_serialcode.Text = tbx_serialcode.Text.Remove(start, len).Insert(start, c.ToString());
            tbx_serialcode.SelectionStart = start + 1;
        }

        private void SerialManualKeyTimer_Tick(object sender, EventArgs e)
        {
            _serialManualKeyTimer.Stop();

            if (!tbx_serialcode.Focused)
                return;

            if (selltype.SelectedIndex < 0 || selltype.SelectedIndex == SelltypeSelectIndex)
            {
                msgbox("Please select a type of purchase first.");
                return;
            }

            tbx_search.Focus();
            tbx_search.Text = _pendingManualChar.ToString();
            tbx_search.SelectionStart = tbx_search.Text.Length;
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
                else if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && dgv_items.Rows.Count > 0)
                {
                    int lastIndex = dgv_items.Rows.Count - 1;
                    int currentIndex = dgv_items.CurrentCell?.RowIndex ?? -1;

                    int targetIndex = currentIndex < 0
                        ? lastIndex
                        : e.KeyCode == Keys.Down
                            ? Math.Min(currentIndex + 1, lastIndex)
                            : Math.Max(currentIndex - 1, 0);

                    dgv_items.CurrentCell = dgv_items.Rows[targetIndex].Cells[0];

                    pendingCartRowSelection = true;
                    pendingCartRowSelectionText = tbx_serialcode.Text;

                    tbx_serialcode.Focus();
                }
                else if (e.KeyCode == Keys.Back && pendingCartRowSelection && dgv_items.CurrentRow != null)
                {
                    dgv_items.Rows.RemoveAt(dgv_items.CurrentRow.Index);

                    pendingCartRowSelection = false;

                    tbx_serialcode.Focus();
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
                            row["description"].ToString().ToUpper(),
                            ToDecimal(row["price"]),
                            ToLong(row["new_quantity"]));
                    }

                    Application.Restart();

                    MessageBox.Show("Application successfully restarted", "Successful restart!");

                    this.CenterToScreen();

                }
                else if (e.KeyValue == (char)Keys.F2)
                {
                    //dgv_itemSearch.Rows.Clear();

                    selltype.SelectedIndex = 0;

                    gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList(); //Wholesale

                }
                else if (e.KeyValue == (char)Keys.F1)
                {
                    selltype.SelectedIndex = 2;

                    //dgv_itemSearch.Rows.Clear();

                    gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList(); // By-piece
                }
                else if (e.KeyValue == (char)Keys.F3)
                {
                    selltype.SelectedIndex = 1;

                    //dgv_itemSearch.Rows.Clear();

                    gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList(); //Retail
                }
                else if (e.KeyValue == (char)Keys.F10)
                {
                    if (dgv_items.Rows.Count > 0)
                    {
                        using (var calculator = new CalculatorForm(grandTotal))
                        {
                            calculator.ShowDialog(this);
                        }
                    }
                    else
                    {
                        using (var pettyCashEntry = new PettyCashEntryForm())
                        {
                            pettyCashEntry.ShowDialog(this);
                        }
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
                        decimal itemPrice = GetItemPrice(tbx_serialcode.Text);
                        string f7ItemName = getItem(tbx_serialcode.Text)[0].ToUpperInvariant();

                        if (BlockZeroPriceItemsEnabled && itemPrice <= 0)
                        {
                            suppressNextSerialKeyUp = true;
                            msgbox($"'{f7ItemName}' has no price set for the selected selling type and cannot be added to the cart.");
                        }
                        else
                        {
                            dgv_items.Rows.Add(
                                tbx_serialcode.Text,
                                f7ItemName,
                                tbx_quantity.Text,
                                itemPrice,
                                itemPrice * Convert.ToInt32(tbx_quantity.Text),
                                QtyStateBulk);
                            OperationLog.RecordRead(nameof(Purchases), $"Item '{tbx_serialcode.Text}' ({f7ItemName}) scanned into cart.");
                            text_in_serial = tbx_serialcode.Text;
                            tbx_serialcode.Clear();
                        }
                    }
                    else
                    {
                        suppressNextSerialKeyUp = true;
                        msgbox("Please enter a valid serial code.");
                    }

                }
                else if (e.KeyCode == Keys.Enter)
                {
                    DataGridViewRow selectedCartRow = dgv_items.CurrentRow;

                    bool untouchedSinceNavigation = pendingCartRowSelection && selectedCartRow != null
                        && tbx_serialcode.Text == pendingCartRowSelectionText;
                    bool matchesSelectedRowsOwnCode = selectedCartRow != null && !string.IsNullOrEmpty(tbx_serialcode.Text)
                        && string.Equals(selectedCartRow.Cells[0].Value?.ToString(), tbx_serialcode.Text, StringComparison.Ordinal);

                    if (untouchedSinceNavigation || matchesSelectedRowsOwnCode)
                    {
                        pendingCartRowSelection = false;
                        PrepareQuantityEditForRow(selectedCartRow);
                    }
                    else if (!string.IsNullOrEmpty(tbx_serialcode.Text))
                    {
                        pendingCartRowSelection = false;

                        // Scanner sends the barcode digits then Enter - this is the actual
                        // per-scan hot path. Look the item up once and reuse it, instead of the
                        // 2-3 separate FindItemRow lookups the two branches used to do.
                        string scannedCode = tbx_serialcode.Text;
                        string itemName = getItem(scannedCode)[0].ToUpperInvariant();
                        decimal itemPrice = GetItemPrice(scannedCode);
                        decimal rowPrice = scannedCode == "01282026-085944" ? Isolated(scannedCode) : itemPrice;

                        if (BlockZeroPriceItemsEnabled && rowPrice <= 0)
                        {
                            suppressNextSerialKeyUp = true;
                            msgbox($"'{itemName}' has no price set for the selected selling type and cannot be added to the cart.");
                        }
                        else
                        {
                            dgv_items.Rows.Add(
                                    scannedCode,
                                    itemName,
                                    tbx_quantity.Text,
                                    rowPrice,
                                    rowPrice * Convert.ToInt32(tbx_quantity.Text),
                                    QtyStateBulk
                                );
                            OperationLog.RecordRead(nameof(Purchases), $"Item '{scannedCode}' ({itemName}) scanned into cart.");
                            text_in_serial = scannedCode;
                            tbx_serialcode.Clear();
                            tbx_serialcode.Focus();
                        }
                    }
                    else if (selectedCartRow != null)
                    {
                        pendingCartRowSelection = false;
                        PrepareQuantityEditForRow(selectedCartRow);
                    }
                    else
                    {
                        suppressNextSerialKeyUp = true;
                        msgbox("Please enter a valid serial code.");
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    if (heldTransactionsWindow != null && !heldTransactionsWindow.IsDisposed && heldTransactionsWindow.Visible)
                    {
                        heldTransactionsWindow.Activate();
                        return;
                    }

                    if (dgv_items.Rows.Count > 0)
                    {
                        var confirm = ConfirmDialog.Show(this, "Clear the entire cart? This cannot be undone. (Y/N)",
                            "Clear Cart", MessageBoxIcon.Question);
                        if (confirm != DialogResult.Yes)
                            return;
                    }

                    selltype.SelectedIndex = 3;

                    gbx_itemList.Text = "Item List";
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

        private async void tbx_search_KeyUp(object sender, KeyEventArgs e)
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

                    // Logged once per submitted search (Enter), not per keystroke like the old
                    // AddToLogs call in tbx_search_TextChanged used to - that opened a fresh DB
                    // connection on every character typed.
                    if (!string.IsNullOrWhiteSpace(search))
                        await ActivityLog.LogAsync($"User searched for: {search}");

                    if (selltype.SelectedIndex == 0)
                    {
                        var rows = OrderRows(ItemsTable.AsEnumerable()
                            .Where(r => (ToInt(r["price"]) != 0 && Contains(r["description"], search)) || Contains(r["serial"], search)),
                            r => ToDate(r["date_of_entry"]), ItemSearchDateSortDescending)
                            .Take(10);

                        dgv_itemSearch.Rows.Clear();

                        foreach (var r in rows)
                        {
                            if (!string.IsNullOrEmpty(r["serial"].ToString()))
                            {
                                if (!string.IsNullOrEmpty(r["price"].ToString()))
                                    dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price"]), ToLong(r["new_quantity"]));
                            }
                        }
                    }
                    else if (selltype.SelectedIndex == 1)
                    {
                        var rows = OrderRows(ItemsTable.AsEnumerable()
                            .Where(r => (ToInt(r["retail_price"]) != 0 && Contains(r["description"], search)) || Contains(r["serial"], search)),
                            r => ToDate(r["date_of_entry"]), ItemSearchDateSortDescending)
                            .Take(10);

                        dgv_itemSearch.Rows.Clear();

                        foreach (var r in rows)
                        {
                            if (!string.IsNullOrEmpty(r["serial"].ToString()))
                            {
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]), ToLong(r["new_quantity"]));
                            }
                        }
                    }
                    else if (selltype.SelectedIndex == 2)
                    {
                        var rows = OrderRows(ItemsTable.AsEnumerable()
                            .Where(r => Contains(r["description"], search) || (Contains(r["serial"], search) && ToInt(r["price_perpc"]) != 0)),
                            r => ToDate(r["date_of_entry"]), ItemSearchDateSortDescending)
                            .Take(10);

                        dgv_itemSearch.Rows.Clear();

                        foreach (var r in rows)
                        {
                            if (!string.IsNullOrEmpty(r["serial"].ToString()))
                            {
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]), ToLong(r["new_quantity"]));
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.F4)
                {
                    dgv_itemSearch.Focus();
                }
                else if (e.KeyCode == Keys.Down && !string.IsNullOrEmpty(tbx_search.Text) && dgv_itemSearch.Rows.Count > 0)
                {
                    if (dgv_itemSearch.CurrentCell == null)
                        dgv_itemSearch.CurrentCell = dgv_itemSearch.Rows[0].Cells[0];

                    dgv_itemSearch.Focus();
                }
            }
            catch (Exception ex) { ErrorDialog.Show(this, "Something went wrong while searching for items.", "Item Search", ex); }
        }
        //qtyState = 1, if selling per piece, 2, if selling bulk
        /// <summary>
        /// Applies one line item's quantity change on <paramref name="conn"/> (shared/reused by
        /// the caller across a whole checkout, instead of this method opening its own connection
        /// per line item). Returns true if the DB write succeeded, so the caller knows whether a
        /// cache refresh is worthwhile afterward. Falls back to a cache-only adjustment on failure,
        /// exactly as before - this is the deliberate per-row offline-degradation path, so each
        /// line item still succeeds or degrades independently of the others.
        /// </summary>
        public async Task<bool> inventory_update(MySqlConnection conn, string serial, int quantity, int qtyState = QtyStateBulk)
        {
            try
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    if (qtyState == QtyStateBulk)
                    {
                        cmd.CommandText = $"UPDATE items_ SET new_quantity = new_quantity - @quantity WHERE serial = @serial";

                        cmd.Parameters.AddWithValue("@quantity", quantity);

                        cmd.Parameters.AddWithValue("@serial", serial);
                    }
                    else if (qtyState == QtyStatePerPiece)
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
                if (qtyState == QtyStateBulk)
                {
                    AdjustCachedQuantity(serial, -quantity);
                    LogPendingStockAdjustment(serial, -quantity, "sale (bulk) while offline");
                }
                else if (qtyState == QtyStatePerPiece)
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

                    string searchItemName = getItem(tbx_serialcode.Text)[0].ToUpperInvariant();
                    decimal rowPrice = tbx_serialcode.Text == "01282026-085944"
                        ? Isolated(tbx_serialcode.Text, Convert.ToInt32(tbx_quantity.Text))
                        : GetItemPrice(tbx_serialcode.Text);

                    if (BlockZeroPriceItemsEnabled && rowPrice <= 0)
                    {
                        msgbox($"'{searchItemName}' has no price set for the selected selling type and cannot be added to the cart.");
                    }
                    else
                    {
                        dgv_items.Rows.Add(
                            tbx_serialcode.Text,
                            searchItemName,
                            tbx_quantity.Text,
                            rowPrice,
                            rowPrice * Convert.ToInt32(tbx_quantity.Text),
                            QtyStateBulk
                            );

                        tbx_serialcode.Clear();

                        tbx_search.Clear();

                        tbx_serialcode.Focus();
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    tbx_serialcode.Focus();
                }
                else if (e.KeyCode == Keys.Back)
                {
                    tbx_search.Focus();
                }
                else if (e.KeyCode == Keys.F1)
                {
                    selltype.SelectedIndex = 2; // By-piece

                    gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList();
                }
                else if (e.KeyCode == Keys.F2)
                {
                    selltype.SelectedIndex = 0; // Wholesale

                    gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList();
                }
                else if (e.KeyCode == Keys.F3)
                {
                    selltype.SelectedIndex = 1; // Retail

                    gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode}";

                    RefreshItemSearchList();
                }
            }
            catch (Exception xe)
            {
                MessageBox.Show(xe.Message, "Error occurred while processing the data.");
            }

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

        private void Purchases_Activated(object sender, EventArgs e)
        {

        }

        private void dgv_itemSearch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void PrepareQuantityEditForRow(DataGridViewRow row)
        {
            dgv_items.CurrentCell = row.Cells[0];

            lbl_quantityStatus.Text = "***";

            tbx_serialcode.Text = row.Cells[0].Value.ToString();

            tbx_serialcode.ReadOnly = true;

            tbx_serialcode.Enabled = false;

            tbx_quantity.Enabled = true;

            tbx_quantity.Text = row.Cells[2].Value?.ToString() ?? "1";

            tbx_quantity.Focus();

            tbx_quantity.SelectAll();

            tbx_quantity.TextAlign = HorizontalAlignment.Center;
        }

        private void dgv_items_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void dgv_items_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            tbx_serialcode.Focus();
            tbx_serialcode.Text = e.KeyChar.ToString();
            tbx_serialcode.SelectionStart = tbx_serialcode.Text.Length;

            e.Handled = true;
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
                    if (dgv_items.Rows.Count == 0)
                    {
                        tbx_serialcode.Focus();
                        return;
                    }

                    var confirm = ConfirmDialog.Show(this, "Clear the entire cart? This cannot be undone. (Y/N)",
                        "Clear Cart", MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes)
                        return;

                    dgv_items.Rows.Clear();
                    grandTotal = 0;
                    count = 0;

                    gbx_itemList.Text = "Item List";

                    tbx_serialcode.Focus();
                }
                else if (e.KeyValue == (char)Keys.Back)
                {
                    dgv_items.Rows.RemoveAt(dgv_items.CurrentRow.Index);

                    tbx_serialcode.Focus();
                }
                else if (e.KeyValue == (char)Keys.F12)
                {
                    if (dgv_items.Rows.Count > 0)
                        PrepareQuantityEditForRow(dgv_items.Rows[dgv_items.Rows.Count - 1]);
                }
                else if (e.KeyValue == (char)Keys.Enter)
                {
                    if (dgv_items.CurrentRow != null)
                        PrepareQuantityEditForRow(dgv_items.CurrentRow);
                }
                else if (e.KeyValue == (char)Keys.F11)
                {
                    tbx_serialcode.Text = dgv_items.CurrentRow.Cells[0].Value.ToString();

                    dgv_items.CurrentRow.Cells[3].Value = Itemprice_InPcs(tbx_serialcode.Text);

                    dgv_items.CurrentRow.Cells[4].Value = Convert.ToDecimal(dgv_items.CurrentRow.Cells[3].Value) * Convert.ToInt32(dgv_items.CurrentRow.Cells[2].Value);

                    dgv_items.CurrentRow.Cells[QtyStateColumnIndex].Value = QtyStatePerPiece;

                    tbx_serialcode.ReadOnly = true;

                    tbx_serialcode.Enabled = false;

                    lbl_quantityStatus.Text = "Quantity (in pieces activated)";

                    lbl_quantityStatus.Font = new Font("Arial", 7, FontStyle.Bold);

                    lbl_quantityStatus.ForeColor = Color.Red;

                    tbx_quantity.Enabled = true;

                    tbx_quantity.Text = dgv_items.CurrentRow.Cells[2].Value?.ToString() ?? "1";

                    tbx_quantity.Focus();

                    tbx_quantity.SelectAll();

                    tbx_quantity.TextAlign = HorizontalAlignment.Center;
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
            _lastCartActivity = DateTime.Now;

            try
            {
                // Fires on every single scan, so this has to stay O(1) per add. Only fold the
                // newly-inserted row(s) into the running totals instead of re-summing the whole
                // cart from scratch every time (that was O(n) per add, i.e. O(n^2) building a
                // full cart) - relies on grandTotal/count being correctly zeroed everywhere the
                int lastDataRowIndex = dgv_items.Rows.Count - 1;
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount && i <= lastDataRowIndex; i++)
                {
                    grandTotal += ToDecimal(dgv_items.Rows[i].Cells[4].Value);
                    count += ToInt(dgv_items.Rows[i].Cells[2].Value);
                }

                total_row = dgv_items.Rows.Count - 1;

                if (dgv_items.Rows.Count > 0)
                    counter.Text = BuildCounterText(grandTotal, $"Total Items: {count}");

                UpdateCartEmptyLogo();
            }
            catch (Exception)
            {
                RecomputeCartTotals();
            }
        }

        private void dgv_items_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            _lastCartActivity = DateTime.Now;

            // Removal is rare/manual (not the per-scan hot path), so a full recompute here is
            // fine - the removed row's own values aren't available from the event args, so an
            // incremental subtract isn't possible anyway. This is the single place that keeps
            // grandTotal/count correct for every way a row can disappear (the native Delete key
            // via AllowUserToDeleteRows, in particular, previously updated neither field at all -
            // this bug used to be masked by RowsAdded always rebuilding from scratch on the next
            // scan; it no longer does, so this handler has to actually run instead of quietly
            // never firing). Note this previously wrote into a local that shadowed the class
            RecomputeCartTotals();
            UpdateCartEmptyLogo();
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            UpdateTime();
            UpdateOfflineItemsStatus();
            UpdateInventorySyncStatus();
            UpdateTitleBar();
            UpdateGreeting();
            UpdateCachedItemsStatusMenu();
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
                        printDocumentReceipt.PrintPage -= PrintDocumentReceipt_PrintPage;
                        printDocumentReceipt.PrintPage += PrintDocumentReceipt_PrintPage;

                        ApplyReceiptPrinterSettings(printDocumentReceipt);

                        // No preview gate here - once the receipt printer is configured, the
                        // receipt must go straight to it instead of waiting on a dialog first.
                        if (IsCounterReceiptJpegMode())
                        {
                            // Customer's copy still goes to the physical printer as normal;
                            // only the counter's copy is diverted to a JPEG file below.
                            printSingleReceiptCopyOnly = true;
                            try
                            {
                                printDocumentReceipt.Print();
                            }
                            finally
                            {
                                printSingleReceiptCopyOnly = false;
                            }

                            SaveCounterReceiptAsJpeg();
                        }
                        else
                        {
                            printDocumentReceipt.Print();
                        }

                        // Capture before it's cleared below - this is what ties every sales_ row
                        // from this checkout back to the receipt that was just printed.
                        string completedTransactionCode = TransactionCode;
                        UpdateLastTransactionCodeMenu(completedTransactionCode);

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
                                for (int i = 0; i < dgv_items.RowCount; i++)
                                {
                                    rows = dgv_items.Rows[i].Cells[0].Value?.ToString() ?? "";

                                    qty = ToInt(dgv_items.Rows[i].Cells[2].Value);

                                    int rowQtyState = ToInt(dgv_items.Rows[i].Cells[QtyStateColumnIndex].Value);
                                    if (rowQtyState != QtyStatePerPiece && rowQtyState != QtyStateBulk)
                                        rowQtyState = QtyStateBulk;

                                    if (await inventory_update(connect_db, rows, qty, rowQtyState))
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

                        gbx_itemList.Text = "Item List";
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
                ErrorDialog.Show(this, "Something went wrong while processing the payment.", "Payment Error", ex);
            }

        }
        public decimal price_per_piece;
        private decimal Itemprice_InPcs(string serial)
        {
            try
            {
                var row = FindItemRow(serial);

                price_per_piece = row != null ? ToDecimal(row["price_perpc"]) : 0;
            }
            catch (Exception ex)
            {
                lbl_warning.Text = "An error occurred: " + ex.Message;
            }
            return price_per_piece;
        }
        private void ResetQuantityEditUi()
        {
            _qtyAutoResetTimer.Stop();

            tbx_quantity.Enabled = false;
            tbx_quantity.Text = "1";

            tbx_serialcode.ReadOnly = false;
            tbx_serialcode.Enabled = true;
            tbx_serialcode.Clear();
            tbx_serialcode.Focus();

            lbl_quantityStatus.Text = "**";
            lbl_quantityStatus.ResetForeColor();
            lbl_quantityStatus.ResetFont();
        }

        private void tbx_quantity_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var total_qty = 0;

                if (e.KeyCode == Keys.Enter)
                {
                    if (!int.TryParse(tbx_quantity.Text, out int newQuantity) || newQuantity <= 0)
                    {
                        lbl_warning.Text = "Enter a valid quantity (a positive whole number).";
                        return;
                    }

                    dgv_items.CurrentRow.Cells[2].Value = newQuantity;

                    if (dgv_items.CurrentRow.Cells[0].Value.ToString() == "01282026-085944") //atwork
                    {
                        if (newQuantity >= 4)
                        {
                            dgv_items.CurrentRow.Cells[4].Value = (newQuantity * Convert.ToDecimal(dgv_items.CurrentRow.Cells[3].Value.ToString())) - 2;
                        }
                        else
                        {
                            dgv_items.CurrentRow.Cells[4].Value = newQuantity * Convert.ToDecimal(dgv_items.CurrentRow.Cells[3].Value.ToString());
                        }
                    }
                    else
                    {
                        dgv_items.CurrentRow.Cells[4].Value = newQuantity * Convert.ToDecimal(dgv_items.CurrentRow.Cells[3].Value.ToString());
                    }

                    ResetQuantityEditUi();

                    grandTotal = 0;

                    for (int i = 0; i < dgv_items.Rows.Count; i++)
                    {
                        grandTotal += ToDecimal(dgv_items.Rows[i].Cells[4].Value);
                        total_qty += ToInt(dgv_items.Rows[i].Cells[2].Value);
                    }

                    // Keep the class-level count field (which dgv_items_RowsAdded now updates
                    // incrementally rather than always recomputing) in sync with this row edit.
                    count = total_qty;

                    counter.Text = BuildCounterText(grandTotal, $"Total Quantity: {total_qty}");
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    ResetQuantityEditUi();
                }
            }
            catch (Exception ex)
            {
                lbl_warning.Text = "An error occurred: " + ex.Message;
            }
            //counter.Text = "Total distinct items: " + count + " \nGrand Total: " + grandTotal.ToString("0.00");
        }

        private void tbx_quantity_TextChanged(object sender, EventArgs e)
        {
            if (tbx_quantity.Text.Length > 3)
            {
                string overflow = tbx_quantity.Text.Substring(3);
                tbx_quantity.Text = tbx_quantity.Text.Substring(0, 3);
                tbx_quantity.SelectionStart = tbx_quantity.Text.Length;

                msgbox("Quantity can't exceed 3 digits. Moving focus to the serial code box.");

                tbx_serialcode.Focus();
                tbx_serialcode.Text += overflow;
                tbx_serialcode.SelectionStart = tbx_serialcode.Text.Length;
                Cursor.Position = tbx_serialcode.PointToScreen(
                    new Point(tbx_serialcode.Width / 2, tbx_serialcode.Height / 2));
            }

            RestartQtyAutoResetTimer();
        }

        private static bool QtyAutoResetEnabled =>
            !string.Equals(ConfigurationManager.AppSettings["QTY_AUTO_RESET_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);

        private static int QtyAutoResetSeconds =>
            int.TryParse(ConfigurationManager.AppSettings["QTY_AUTO_RESET_SECONDS"], out int s) && s > 0 ? s : 5;

        /// <summary>Called from tbx_quantity_TextChanged: restarts the idle-reset countdown while
        /// a quantity edit is in progress, so an abandoned half-typed value doesn't sit there
        /// indefinitely. No-ops once the row's edit session has already ended (tbx_quantity
        /// disabled by <see cref="ResetQuantityEditUi"/>), so it can't keep re-arming itself.</summary>
        private void RestartQtyAutoResetTimer()
        {
            _qtyAutoResetTimer.Stop();

            if (!QtyAutoResetEnabled || !tbx_quantity.Enabled)
                return;

            _qtyAutoResetTimer.Interval = QtyAutoResetSeconds * 1000;
            _qtyAutoResetTimer.Start();
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

        public void msgbox(string msg)
        {
            OkDialog.Show(this, msg, "Notice");
        }

        private async void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageDashboard)
            {
                await LoadDashboardAsync();
                return;
            }

            if (tabControlMain.SelectedTab == tabPageTopSold)
            {
                await LoadTopSoldAsync();
                return;
            }

            if (tabControlMain.SelectedTab != tabPageManagePrices)
                return;

            using (var pinForm = new PinPromptForm())
            {
                string managerPin = ConfigurationManager.AppSettings["MANAGER_PIN"];
                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != managerPin)
                {
                    ErrorDialog.Show(this, "Incorrect PIN. Access to Manage Prices is restricted.", "Access Denied",
                        $"Manager PIN rejected for 'Manage Prices' at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
                    tabControlMain.SelectedTab = tabPageSales;
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

        private sealed class TopSoldAgg
        {
            public string Description;
            public int TimesSold;
            public decimal TotalSales;
            public DateTime? LastSold;
        }

        // Trims/collapses whitespace and applies a configurable case style (TOP_SOLD_ITEM_NAME_CASE:
        // None|Upper|Title, defaulting to Title) to items_.description before it's shown in the
        // Top Sold grid - real rows have inconsistent spacing/casing from manual data entry
        // (e.g. "coke  1.5L " vs "COKE 1.5l").
        private static string NormalizeItemName(string raw)
        {
            string cleaned = Regex.Replace((raw ?? "").Trim(), @"\s+", " ");
            if (cleaned.Length == 0)
                return cleaned;

            string caseMode = ConfigurationManager.AppSettings["TOP_SOLD_ITEM_NAME_CASE"] ?? "Title";
            switch (caseMode)
            {
                case "Upper":
                    return cleaned.ToUpperInvariant();
                case "None":
                    return cleaned;
                case "Title":
                default:
                    return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cleaned.ToLowerInvariant());
            }
        }

        // Calendar-aligned start of the selected Top Sold period, matching the dashboard's
        // "calendar month to date" convention (see LoadDashboardInventoryStats/PopulateDashboardSales).
        private static DateTime GetTopSoldPeriodStart(string period)
        {
            DateTime today = DateTime.Today;
            switch (period)
            {
                case "Weekly":
                    int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
                    return today.AddDays(-daysSinceMonday);
                case "Quarterly":
                    int quarterStartMonth = ((today.Month - 1) / 3) * 3 + 1;
                    return new DateTime(today.Year, quarterStartMonth, 1);
                case "Annually":
                    return new DateTime(today.Year, 1, 1);
                case "Monthly":
                default:
                    return new DateTime(today.Year, today.Month, 1);
            }
        }

        /// <summary>
        /// Populates the Top Sold tab for the period selected in cmbTopSoldPeriod (Weekly/
        /// Monthly/Quarterly/Annually, calendar-aligned to date): ranks items by how many
        /// sales_ rows (checkout lines) reference them in that window and the revenue those
        /// lines total. sales_ only persists itemcode + line total (see SaveSaleLineItemsAsync)
        /// - not the line's quantity - so "Times Sold" counts checkout lines, not units, same
        /// caveat as the rest of the sales reporting here. Aggregation (including "last sold")
        /// happens client-side rather than via SQL GROUP BY/MAX/WHERE because transaction_date
        /// is stored as "yyyy-MMMM-dd H:m:ss" (month name, unpadded hour/minute/second - see
        /// SaveSaleLineItemsAsync) which doesn't sort or range-compare chronologically as a string.
        /// </summary>
        private async Task LoadTopSoldAsync()
        {
            DateTime periodStart = GetTopSoldPeriodStart(cmbTopSoldPeriod.SelectedItem.ToString());

            btnTopSoldRefresh.Enabled = false;
            dgvTopSold.Rows.Clear();
            lblTopSoldStatus.Text = "";

            try
            {
                var byItem = new Dictionary<string, TopSoldAgg>();

                using (var conn = new MySqlConnection(DbConnectionHelper.BuildConnectionString()))
                using (var cmd = new MySqlCommand(
                    "SELECT s.itemcode AS itemcode, i.description AS itemname, " +
                    "s.item_price AS item_price, s.transaction_date AS transaction_date " +
                    "FROM sales_ s LEFT JOIN items_ i ON s.itemcode = i.serial", conn))
                {
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string rawDate = reader["transaction_date"]?.ToString();
                            if (!DateTime.TryParseExact(rawDate, "yyyy-MMMM-dd H:m:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed)
                                || parsed < periodStart)
                                continue;

                            string itemcode = reader["itemcode"]?.ToString() ?? "";
                            if (!byItem.TryGetValue(itemcode, out TopSoldAgg agg))
                            {
                                agg = new TopSoldAgg();
                                byItem[itemcode] = agg;
                            }

                            agg.Description = NormalizeItemName(reader["itemname"] == DBNull.Value ? "" : reader["itemname"].ToString());
                            agg.TimesSold++;
                            agg.TotalSales += reader["item_price"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["item_price"]);

                            if (agg.LastSold == null || parsed > agg.LastSold)
                                agg.LastSold = parsed;
                        }
                    }
                }

                int rank = 1;
                foreach (var kvp in byItem.OrderByDescending(p => p.Value.TimesSold).ThenByDescending(p => p.Value.TotalSales).Take(50))
                {
                    dgvTopSold.Rows.Add(
                        rank++,
                        kvp.Key,
                        kvp.Value.Description,
                        kvp.Value.TimesSold,
                        kvp.Value.TotalSales.ToString("N2"),
                        kvp.Value.LastSold?.ToString("yyyy-MM-dd") ?? "—");
                }

                if (dgvTopSold.Rows.Count == 0)
                    lblTopSoldStatus.Text = "No sales recorded yet.";
            }
            catch (Exception)
            {
                lblTopSoldStatus.Text = "Top sold data unavailable offline.";
            }
            finally
            {
                btnTopSoldRefresh.Enabled = true;
            }
        }

        private async void btnTopSoldRefresh_Click(object sender, EventArgs e) => await LoadTopSoldAsync();

        private async void cmbTopSoldPeriod_SelectedIndexChanged(object sender, EventArgs e) => await LoadTopSoldAsync();

        private void btnDashInventoryReport_Click(object sender, EventArgs e) => inventoryReportToolStripMenuItem_Click(sender, e);

        private void btnDashSalesChart_Click(object sender, EventArgs e) => monthlySalesChartToolStripMenuItem_Click(sender, e);

        private void btnDashPettyCash_Click(object sender, EventArgs e) => pettyCashReportToolStripMenuItem_Click(sender, e);

        private void btnDashLoyalty_Click(object sender, EventArgs e) => loyaltyToolStripMenuItem_Click(sender, e);

        private void btnDashSystemConfig_Click(object sender, EventArgs e) => systemConfigToolStripMenuItem_Click(sender, e);

        // Cells the manager has just edited get flagged red/bold for ManagePricesEditHighlightMs,
        // then revert - a quick "this one's unsaved and was just touched" visual cue on top of
        // (not a replacement for) btn_savePrices_Click's actual DataRowState.Modified tracking.
        // Keyed by the cell object itself rather than row/column index, since ApplyManagePricesFilter
        // re-sorts/hides rows via the bound DataTable's DefaultView without replacing DataSource -
        // index pairs would go stale, but the underlying DataGridViewCell objects for rows that
        // stay in the view do not.
        private readonly Dictionary<DataGridViewCell, System.Windows.Forms.Timer> _managePricesEditHighlights =
            new Dictionary<DataGridViewCell, System.Windows.Forms.Timer>();

        private const int ManagePricesEditHighlightMs = 15000;

        // Value a cell held right before the manager started editing it, captured at
        // CellBeginEdit (the bound DataTable isn't updated until the edit commits, so this is
        // still the pre-edit value) and consumed at CellEndEdit to log an actual before/after
        // line to the history panel - without this, every CellEndEdit (even clicking into a
        // cell and away again with no change) would read as an "edit".
        private readonly Dictionary<DataGridViewCell, object> _managePricesCellOldValues =
            new Dictionary<DataGridViewCell, object>();

        private const int ManagePricesHistoryMaxEntries = 200;

        private void dgv_managePrices_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewCell cell = dgv_managePrices.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _managePricesCellOldValues[cell] = cell.Value;
        }

        private void dgv_managePrices_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = dgv_managePrices.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            HighlightEditedManagePricesCell(cell);

            if (_managePricesCellOldValues.TryGetValue(cell, out object oldValue))
            {
                _managePricesCellOldValues.Remove(cell);

                object newValue = cell.Value;
                if (!Equals(oldValue?.ToString(), newValue?.ToString()))
                    LogManagePricesHistory(row, dgv_managePrices.Columns[e.ColumnIndex], oldValue, newValue);
            }
        }

        private static string ManagePricesColumnShortLabel(string columnName)
        {
            switch (columnName)
            {
                case "price": return "Price";
                case "retail_price": return "Retail Price";
                case "price_perpc": return "Price/Pc";
                case "category": return "Category";
                default: return columnName;
            }
        }

        private void LogManagePricesHistory(DataGridViewRow row, DataGridViewColumn column, object oldValue, object newValue)
        {
            string serial = row.Cells["serial"].Value?.ToString() ?? "";
            string label = ManagePricesColumnShortLabel(column.Name);

            lst_managePricesHistory.Items.Insert(0,
                $"{DateTime.Now:h:mm:ss tt}  {serial} — {label}: {oldValue} → {newValue}");

            // A long-running session (prices tweaked over and over through the day) shouldn't
            // let this grow without bound - oldest entries fall off the bottom.
            while (lst_managePricesHistory.Items.Count > ManagePricesHistoryMaxEntries)
                lst_managePricesHistory.Items.RemoveAt(lst_managePricesHistory.Items.Count - 1);

            SaveManagePricesHistory();
        }

        /// <summary>Loads the encrypted history file (if any) into the panel at startup, so
        /// price-edit history survives an app restart instead of only lasting the session.</summary>
        private void RestoreManagePricesHistory()
        {
            lst_managePricesHistory.Items.Clear();
            foreach (string entry in ManagePricesHistoryStore.Load())
                lst_managePricesHistory.Items.Add(entry);
        }

        private void SaveManagePricesHistory() =>
            ManagePricesHistoryStore.Save(lst_managePricesHistory.Items.Cast<string>());

        private void HighlightEditedManagePricesCell(DataGridViewCell cell)
        {
            if (_managePricesEditHighlights.TryGetValue(cell, out System.Windows.Forms.Timer runningTimer))
            {
                runningTimer.Stop();
                runningTimer.Dispose();
                _managePricesEditHighlights.Remove(cell);
            }

            cell.Style.ForeColor = Color.Red;
            cell.Style.Font = new Font(dgv_managePrices.Font, FontStyle.Bold);

            var timer = new System.Windows.Forms.Timer { Interval = ManagePricesEditHighlightMs };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                timer.Dispose();
                _managePricesEditHighlights.Remove(cell);

                // Empty/null falls back to the column's DefaultCellStyle instead of forcing a
                // specific color/font, so this plays nicely with whatever theme/scale is active.
                cell.Style.ForeColor = Color.Empty;
                cell.Style.Font = null;
                dgv_managePrices.InvalidateCell(cell);
            };
            timer.Start();
            this.FormClosed += (s, args) => timer.Stop();

            _managePricesEditHighlights[cell] = timer;
        }

        private async Task LoadManagePricesGrid()
        {
            try
            {
                await ConnectDb();
                try
                {
                    using (var adapter = new MySqlDataAdapter("SELECT serial, description, category, price, retail_price, price_perpc FROM items_", connect_db))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);

                        // A reload swaps in a brand new DataTable/grid rows, so any highlight
                        // timers still pending from the previous load would otherwise touch
                        // disposed DataGridViewCell objects.
                        foreach (System.Windows.Forms.Timer pending in _managePricesEditHighlights.Values)
                        {
                            pending.Stop();
                            pending.Dispose();
                        }
                        _managePricesEditHighlights.Clear();
                        _managePricesCellOldValues.Clear();

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
                ReplaceCategoryColumnWithComboBox((DataTable)dgv_managePrices.DataSource);
                dgv_managePrices.Columns["price"].HeaderText = "Price (whole number)";
                dgv_managePrices.Columns["price"].DefaultCellStyle.Format = "0.00";
                dgv_managePrices.Columns["retail_price"].HeaderText = "Retail Price (whole number)";
                dgv_managePrices.Columns["retail_price"].DefaultCellStyle.Format = "0.00";
                dgv_managePrices.Columns["price_perpc"].HeaderText = "Price Per Piece (whole number)";
                dgv_managePrices.Columns["price_perpc"].DefaultCellStyle.Format = "0.00";

                // Reloading swaps in a brand new DataTable, so re-apply whatever the manager
                // had already typed instead of silently dropping their search.
                ApplyManagePricesFilter();

                lbl_managePricesStatus.Text = "";
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not load the item list for Manage Prices.", "Failed to Load Items", ex);
            }
        }

        /// <summary>
        /// Swaps the auto-generated plain-text "category" column for a DataGridViewComboBoxColumn
        /// populated with every distinct category already in use, so picking one avoids near-
        /// duplicates from retyping (e.g. "Snacks" vs "snack"). Still lets a brand new category be
        /// typed directly - dgv_managePrices_EditingControlShowing sets the cell's editing combo
        /// box to DropDownStyle.DropDown (not DropDownList) rather than restricting to the list.
        /// </summary>
        private void ReplaceCategoryColumnWithComboBox(DataTable table)
        {
            DataGridViewColumn oldColumn = dgv_managePrices.Columns["category"];
            int index = oldColumn.Index;

            var categories = table.AsEnumerable()
                .Select(r => r["category"]?.ToString()?.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            dgv_managePrices.Columns.Remove(oldColumn);

            var categoryColumn = new DataGridViewComboBoxColumn
            {
                Name = "category",
                DataPropertyName = "category",
                HeaderText = "Category",
            };
            categoryColumn.Items.AddRange(categories);

            dgv_managePrices.Columns.Insert(index, categoryColumn);
        }

        private void dgv_managePrices_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgv_managePrices.CurrentCell?.OwningColumn?.Name == "category" && e.Control is ComboBox comboBox)
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;
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
                    UpdateOfflineItemsStatus();
                    UpdateInventorySyncStatus();
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
                if (!int.TryParse(row["price"].ToString(), out int price) ||
                    !int.TryParse(row["retail_price"].ToString(), out int retailPrice) ||
                    !int.TryParse(row["price_perpc"].ToString(), out int pricePerPc) ||
                    price < 0 || retailPrice < 0 || pricePerPc < 0)
                {
                    MessageBox.Show($"Invalid price value for item '{row["serial"]}'. Prices must be non-negative whole numbers.", "Invalid Input");
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
                        using (var cmd = new MySqlCommand("UPDATE items_ SET price=@price, retail_price=@retail_price, price_perpc=@price_perpc, category=@category WHERE serial=@serial", connect_db))
                        {
                            cmd.Parameters.AddWithValue("@price", row["price"]);
                            cmd.Parameters.AddWithValue("@retail_price", row["retail_price"]);
                            cmd.Parameters.AddWithValue("@price_perpc", row["price_perpc"]);
                            cmd.Parameters.AddWithValue("@category", row["category"]);
                            cmd.Parameters.AddWithValue("@serial", row["serial"]);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                finally
                {
                    connect_db?.Close();
                }

                foreach (DataRow row in changes.Rows)
                    OperationLog.RecordUpdate(nameof(Purchases), $"Item '{row["serial"]}' price updated (price={row["price"]}, retail={row["retail_price"]}, perpc={row["price_perpc"]}, category={row["category"]}).");

                table.AcceptChanges();
                RefreshItemsCache();
                lbl_managePricesStatus.Text = $"Saved {changes.Rows.Count} item(s).";
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not save the price changes to the database.", "Failed to Save Prices", ex);
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

            var confirm = ConfirmDialog.Show(this, $"Delete item '{description}' ({serial})? This cannot be undone. (Y/N)",
                "Confirm delete", MessageBoxIcon.Question);
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

                OperationLog.RecordDelete(nameof(Purchases), $"Item '{serial}' ({description}) deleted.");
                RefreshItemsCache();
                await LoadManagePricesGrid();
                lbl_managePricesStatus.Text = $"Deleted '{serial}'.";
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not delete this item from the database.", "Failed to Delete Item", ex);
            }
        }

        #region UnusedEvents
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
        private void picLogo_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Loads the store logo through LogoCache instead of PictureBox's own ImageLocation
        /// loader, so offline runs fall back to the last cached copy instead of PictureBox
        /// silently swapping in a broken-image icon that would then get printed on receipts.
        /// </summary>
        private async Task LoadReceiptLogoAsync()
        {
            Image logo = await LogoCache.LoadAsync();
            picLogo.Image = logo;
            picCartLogo.Image = logo;
        }

        private void UpdateCartEmptyLogo()
        {
            picCartLogo.Visible = dgv_items.Rows.Count == 0;
        }

        private decimal Isolated(string sugar_barcode, int num_of_items = 2)
        {
            decimal price = 0;

            var row = FindItemRow(sugar_barcode);
            if (row != null)
            {
                if (num_of_items == 1)
                {
                    price = ToDecimal(row["price_perpc"]);
                }
                else if (num_of_items >= 4)
                {
                    price = ToDecimal(row["retail_price"]) - 2;
                }
            }
            return price;
        }
        private void tbx_search_TextChanged(object sender, EventArgs e)
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
                            dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price"]), ToLong(r["new_quantity"]));
                        else if (selltype.SelectedIndex == 1 && ToInt(r["retail_price"]) != 0)
                            dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]), ToLong(r["new_quantity"]));
                        else if (selltype.SelectedIndex == 2 && ToInt(r["price_perpc"]) != 0)
                            dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price_perpc"]), ToLong(r["new_quantity"]));
                    }
                }
            }
            listing.Text = "Total items: " + retriever.ToString();

            RestartSearchAutoClearCountdown();
        }
        #endregion
        private const int SlowPingThresholdMs = 300;
        private long? lastPingRoundtripMs;

        public async Task<string> netTesting()
        {
            string pingResult = "No connection";
            lastPingRoundtripMs = null;
            try
            {
                string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"]?.Trim();
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
            catch (Exception ex) { ErrorDialog.Show(this, "Something went wrong while switching the selling type.", "Selling Type", ex); }
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

                    var rows = OrderRows(ItemsTable.AsEnumerable()
                        .Where(r => ToInt(r["price"]) != 0 && ToInt(r["new_quantity"]) != 0),
                        r => r["description"].ToString(), ItemSearchNameSortDescending);

                    foreach (var r in rows)
                    {
                        retriever += 1;

                        if (!string.IsNullOrEmpty(r["serial"].ToString()) && !r["serial"].ToString().Contains("https:"))

                            if (retriever > 10)
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price"]), ToLong(r["new_quantity"]));
                    }

                }
                else if (selltype.SelectedIndex == 1)
                {

                    dgv_itemSearch.Rows.Clear();

                    var rows = OrderRows(ItemsTable.AsEnumerable()
                        .Where(r => ToInt(r["retail_price"]) != 0 && ToInt(r["new_quantity"]) != 0),
                        r => r["description"].ToString(), ItemSearchNameSortDescending);

                    foreach (var r in rows)
                    {
                        retriever += 1;

                        if (!string.IsNullOrEmpty(r["serial"].ToString()) && !r["serial"].ToString().Contains("https:"))
                            if (retriever > 10)
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["retail_price"]), ToLong(r["new_quantity"]));
                    }
                }
                else if (selltype.SelectedIndex == 2)
                {

                    dgv_itemSearch.Rows.Clear();

                    var rows = OrderRows(ItemsTable.AsEnumerable()
                        .Where(r => ToInt(r["price_perpc"]) != 0 && ToInt(r["new_quantity"]) != 0),
                        r => r["description"].ToString(), ItemSearchNameSortDescending);

                    foreach (var r in rows)
                    {
                        retriever += 1;

                        if (!string.IsNullOrEmpty(r["serial"].ToString()) && !r["serial"].ToString().Contains("https:"))
                            if (retriever > 10)
                                dgv_itemSearch.Rows.Add(r["serial"].ToString(), r["description"].ToString().ToUpper(), ToDecimal(r["price_perpc"]), ToLong(r["new_quantity"]));
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
            catch (Exception ex) { ErrorDialog.Show(this, "Could not refresh the item search list.", "Item Search", ex); }
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

        private void printPreviewDialogReceipt_Load(object sender, EventArgs e)
        {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string shortcuts =
                "GLOBAL\n" +
                "  F6           Hold the current transaction\n" +
                "  F8           Resume a held transaction\n" +
                "  Numpad /     View held transactions\n" +
                "  Ctrl+H       Show this shortcuts list\n" +
                "  (status bar) Click the inventory sync indicator (bottom of the window)\n" +
                "               to push offline-queued items to the database on demand\n" +
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
                "  F10          Open Petty Cash (cart empty) / Calculator (cart has items)\n" +
                "  F12          Jump to the cart grid\n" +
                "  Escape       Deselect the selling type\n" +
                "\n" +
                "ITEM SEARCH BOX\n" +
                "  Enter        Search items matching the typed text\n" +
                "  F4           Jump to the search results grid\n" +
                "  Escape       Clear the search box\n" +
                "  (auto)       Left idle, clears itself after a few seconds - move the\n" +
                "               mouse to cancel (System Config > Alerts)\n" +
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
                "  (auto)       Typing a 4th digit moves it to the serial code box instead\n" +
                "  (auto)       Left idle, resets to 1 after a few seconds (System Config >\n" +
                "               Alerts)\n" +
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

        /// <summary>Hands the register to the next teller without restarting the app. Reuses
        /// HoldCurrentTransaction (same as the idle-cart auto-hold) so an in-progress sale isn't
        /// lost just because the current teller logged out mid-transaction.</summary>
        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, $"Log out {_loggedInCode}?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            HoldCurrentTransaction("Auto-held on logout.", silent: true);

            string loggedOutCode = _loggedInCode;
            string logText = $"User '{loggedOutCode}' logged out.";
            _ = ActivityLog.LogAsync(logText);
            OperationLog.RecordAction(nameof(Purchases), logText);

            log_in = "";
            _loggedInCode = null;
            UpdateTitleBar();
            UpdateGreeting();

            this.Enabled = false;

            Login login = new Login();
            login.ShowDialog(this);

            if (!login.IsAuthenticated)
            {
                ErrorDialog.Show(this, "Login failed or cancelled. The application will now close.", "Access Denied",
                    $"Login dialog closed with IsAuthenticated=false at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            this.Enabled = true;

            log_in = login.id;
            _loggedInCode = login.name;

            UpdateTitleBar();
            UpdateGreeting();
            tbx_serialcode.Focus();
        }

        private void SaveHeldTransactions() => HeldTransactionStore.Save(heldTransactions.Values);

        private void RestoreHeldTransactions()
        {
            List<HeldTransaction> restored = HeldTransactionStore.Load();
            if (restored.Count == 0)
                return;

            foreach (HeldTransaction held in restored)
                heldTransactions[held.TicketCode] = held;

            ShowTrayNotification("Held Transactions Restored",
                restored.Count == 1
                    ? "1 held transaction from a previous session was restored - open View Held Transactions (numpad /) to review it."
                    : $"{restored.Count} held transactions from a previous session were restored - open View Held Transactions (numpad /) to review them.",
                ToolTipIcon.Info);
        }

        private List<object[]> CaptureCartRows()
        {
            var items = new List<object[]>(dgv_items.Rows.Count);
            foreach (DataGridViewRow row in dgv_items.Rows)
            {
                var vals = new object[row.Cells.Count];
                for (int i = 0; i < row.Cells.Count; i++)
                    vals[i] = row.Cells[i].Value;
                items.Add(vals);
            }
            return items;
        }

        private void LoadCartRows(List<object[]> items)
        {
            dgv_items.SuspendLayout();
            try
            {
                dgv_items.Rows.Clear();
                count = 0;
                foreach (var vals in items)
                    dgv_items.Rows.Add(vals);
            }
            finally
            {
                dgv_items.ResumeLayout();
            }
        }

        private void holdTransactionToolStripMenuItem_Click(object sender, EventArgs e) =>
            HoldCurrentTransaction(remarks: null, silent: false);

        private bool HoldCurrentTransaction(string remarks, bool silent)
        {
            if (_preHeldPreviewSnapshot != null)
            {
                if (!silent)
                    msgbox($"You're previewing held ticket \"{_previewedTicketCode}\" - this isn't a live transaction. Resume it first if you want to hold it again.");
                return false;
            }

            var items = CaptureCartRows();

            if (items.Count == 0)
            {
                if (!silent)
                {
                    suppressNextSerialKeyUp = true;
                    MessageBox.Show("There is nothing to hold - the current cart is empty.", "Hold Transaction");
                }
                return false;
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
                Items = items,
                Remarks = remarks,
            };
            SaveHeldTransactions();

            dgv_items.Rows.Clear();
            grandTotal = 0;
            count = 0;
            counter.Text = "Total items: 0 \nGrand Total: 0.00";
            tbx_serialcode.Clear();
            tbx_serialcode.Enabled = true;
            tbx_denomination.Clear();
            tbx_denomination.Enabled = false;
            TransactionCode = "";

            gbx_itemList.Text = "Item List";

            heldTransactionsWindow?.RefreshList();

            if (silent)
            {
                ShowTrayNotification("Transaction Auto-Held",
                    $"An idle cart (ticket \"{ticketCode}\") was automatically held. {remarks}",
                    ToolTipIcon.Info);
            }
            else
            {
                suppressNextSerialKeyUp = true;
                MessageBox.Show($"Transaction held as ticket \"{ticketCode}\".\nUse Resume Transaction (F8) or View Held Transactions (numpad /) to bring it back.", "Hold Transaction", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            tbx_serialcode.Focus();
            return true;
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

        private void transactionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListSales listSales = new ListSales();
            listSales.ShowDialog();
        }

        private void ShowHeldTransactionsWindow()
        {
            if (heldTransactionsWindow == null || heldTransactionsWindow.IsDisposed)
            {
                heldTransactionsWindow = new HeldTransactionsForm(this);

                heldTransactionsWindow.FormClosed += (s, e) => heldTransactionsWindow = null;

                heldTransactionsWindow.FormClosed += (s, e) => EndHeldTransactionPreview();
            }

            heldTransactionsWindow.RefreshList();
            if (!heldTransactionsWindow.TopMost)
            {
                heldTransactionsWindow.StartPosition = FormStartPosition.Manual;

                Rectangle workArea = Screen.FromControl(this).WorkingArea;
                Point cartBoardScreenLocation = GetCartBoardScreenLocation();
                int x = Math.Max(cartBoardScreenLocation.X - heldTransactionsWindow.Width, workArea.Left);
                int y = Math.Min(cartBoardScreenLocation.Y, workArea.Bottom - heldTransactionsWindow.Height);
                y = Math.Max(y, workArea.Top);
                heldTransactionsWindow.Location = new Point(x, y);
            }

            heldTransactionsWindow.Show();
            heldTransactionsWindow.Activate();
        }

        internal bool ResumeHeldTransaction(string ticketCode, out string errorMessage)
        {
            errorMessage = null;

            int realCartRowCount = _preHeldPreviewSnapshot?.Items.Count ?? dgv_items.Rows.Count;
            if (realCartRowCount > 0)
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
            SaveHeldTransactions();

            _preHeldPreviewSnapshot = null;
            _previewedTicketCode = null;

            selltype.SelectedIndex = held.SelltypeIndex;
            TransactionCode = held.TransactionCode;

            gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{TransactionCode} ({ticketCode})";

            LoadCartRows(held.Items);

            grandTotal = held.GrandTotal;
            counter.Text = held.CounterText;

            tbx_serialcode.Enabled = true;

            heldTransactionsWindow?.RefreshList(false);

            if (heldTransactionsWindow != null && !heldTransactionsWindow.IsDisposed)
            {
                heldTransactionsWindow.TopMost = false;
                heldTransactionsWindow.SendToBack();
            }

            this.BringToFront();
            this.Activate();
            tbx_serialcode.Focus();

            return true;
        }

        internal void CancelHeldTransaction(string ticketCode)
        {
            heldTransactions.Remove(ticketCode);
            SaveHeldTransactions();
            heldTransactionsWindow?.RefreshList();
        }

        internal void PreviewHeldTransaction(string ticketCode)
        {
            if (ticketCode == _previewedTicketCode)
                return;

            if (!heldTransactions.TryGetValue(ticketCode, out HeldTransaction held))
                return;

            bool isFirstPreview = _preHeldPreviewSnapshot == null;
            if (isFirstPreview)
            {
                _preHeldPreviewSnapshot = new CartSnapshot
                {
                    Items = CaptureCartRows(),
                    GrandTotal = grandTotal,
                    CounterText = counter.Text,
                    TransactionCode = TransactionCode,
                };
            }

            _previewedTicketCode = ticketCode;

            LoadCartRows(held.Items);

            grandTotal = held.GrandTotal;
            counter.Text = held.CounterText;
            gbx_itemList.Text = $"Item List for {S_DEFAULTNAME}-{held.TransactionCode} ({ticketCode}) - PREVIEW";

            if (!string.IsNullOrEmpty(tbx_serialcode.Text))
            {
                tbx_search.Text = tbx_serialcode.Text;
                tbx_search.SelectionStart = tbx_search.Text.Length;
                tbx_search.Focus();
            }

            tbx_serialcode.Clear();
            tbx_serialcode.Enabled = false;

            if (isFirstPreview)
            {
                ShowTrayNotification("Held Transaction Preview",
                    $"Previewing held ticket \"{ticketCode}\" - the serial code box is disabled. Resume it (F8) in the Held Transactions window to make changes.",
                    ToolTipIcon.Info);
            }
        }

        internal void EndHeldTransactionPreview()
        {
            if (_preHeldPreviewSnapshot == null)
                return;

            CartSnapshot snapshot = _preHeldPreviewSnapshot;
            _preHeldPreviewSnapshot = null;
            _previewedTicketCode = null;

            LoadCartRows(snapshot.Items);

            grandTotal = snapshot.GrandTotal;
            counter.Text = snapshot.CounterText;
            TransactionCode = snapshot.TransactionCode;
            gbx_itemList.Text = snapshot.Items.Count > 0
                ? $"Item List for {S_DEFAULTNAME}-{TransactionCode}"
                : "Item List";

            tbx_serialcode.Enabled = true;
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

        private void lblCachedItemsStatus_Click(object sender, EventArgs e) =>
            inventoryReportToolStripMenuItem_Click(sender, e);

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
                ErrorDialog.Show(this, "Could not open the Receipts folder.", "Failed to Open Receipts Folder", ex);
            }
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var pinForm = new PinPromptForm())
            {
                string managerPin = ConfigurationManager.AppSettings["MANAGER_PIN"];
                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != managerPin)
                {
                    ErrorDialog.Show(this, "Incorrect PIN. Access to Manage Users is restricted.", "Access Denied",
                        $"Manager PIN rejected for 'Manage Users' at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
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
                    ErrorDialog.Show(this, "Incorrect PIN. Access to Tax Configuration is restricted.", "Access Denied",
                        $"Manager PIN rejected for 'Tax Configuration' at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
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

        private const int DoubleHomeWindowMs = 500;
        private DateTime _lastHomePress = DateTime.MinValue;

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

            if (keyData == Keys.Home)
            {
                DateTime now = DateTime.Now;
                if ((now - _lastHomePress).TotalMilliseconds <= DoubleHomeWindowMs)
                {
                    _lastHomePress = DateTime.MinValue;
                    this.WindowState = FormWindowState.Minimized;
                }
                else
                {
                    _lastHomePress = now;
                }
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
                    ErrorDialog.Show(this, "Incorrect PIN. Access to System Configuration is restricted.", "Access Denied",
                        $"Admin PIN rejected for 'System Configuration' at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
                    return;
                }
            }

            using (var systemConfigForm = new SystemConfigForm())
            {
                systemConfigForm.ShowDialog(this);
            }

            ApplyMenuVisibility();
            ApplyItemSearchColumnVisibility();
            ApplyCartColumnVisibility();
            ApplyStoreIdentity();
        }

        private void editLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_layoutEditFilter == null)
            {
                bool layoutEditEnabled = string.Equals(
                    ConfigurationManager.AppSettings["LAYOUT_EDIT_ENABLED"], "true", StringComparison.OrdinalIgnoreCase);
                if (!layoutEditEnabled)
                {
                    MessageBox.Show(
                        "Layout editing is turned off. Enable \"Allow Edit Layout from the main menu\" on System Config's Appearance tab first.",
                        "Edit Layout Disabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var pinForm = new PinPromptForm())
                {
                    pinForm.PromptText = "Enter system admin PIN to continue:";
                    string adminPin = ConfigurationManager.AppSettings["ADMIN_PIN"];
                    if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != adminPin)
                    {
                        ErrorDialog.Show(this, "Incorrect PIN. Editing the layout is restricted.", "Access Denied",
                            $"Admin PIN rejected for 'Edit Layout' at {DateTime.Now:G} on {Environment.MachineName}.", MessageBoxIcon.Warning);
                        return;
                    }
                }

                _layoutEditFilter = new LayoutEditMessageFilter(this);
                Application.AddMessageFilter(_layoutEditFilter);
                LayoutEditOverlay.Attach(this);
                editLayoutToolStripMenuItem.Text = "Exit Layout Edit";
                MessageBox.Show(
                    "Layout editing is on - each movable item is outlined with a small handle at its " +
                    "bottom-right corner. Drag the item itself to reposition it, or drag its handle to " +
                    "resize it. Nothing else on screen will respond to clicks until you choose " +
                    "\"Exit Layout Edit\" from this same menu.",
                    "Edit Layout", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Application.RemoveMessageFilter(_layoutEditFilter);
                _layoutEditFilter = null;
                LayoutEditOverlay.Detach(this);
                editLayoutToolStripMenuItem.Text = "Edit Layout";
                MainLayoutStore.SaveFrom(this);
                ShowTrayNotification("Layout Saved", "The main screen layout has been saved on this computer.", ToolTipIcon.Info);
            }
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
