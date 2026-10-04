# App Process Documentation — Baclig Minimart A10D-up

This describes how the app actually behaves at runtime: startup sequence, login,
the sales/checkout flow, held transactions, manager/admin overrides, and the
offline-resilience behavior that runs underneath all of it. For code layout and
build instructions, see `CLAUDE.md`.

## 1. Startup sequence

1. `Program.Main` (`Program.cs`) shows `SplashForm`, which calls
   `DataCacheManager.Instance.LoadAll()` to pull the `Items` and `Users` tables
   from MySQL into memory and closes itself when done.
   - Each table load is also snapshotted to
     `Documents\App Inventory\OfflineCache\<Table>.xml`.
   - If a table load fails (no DB reachable) and no snapshot exists at all,
     startup aborts with a "Failed to load startup data" error box.
2. If the DB was unreachable but a snapshot existed, the app enters
   **offline mode**: a warning box tells the user data may be stale
   (`DataCacheManager.OfflineSnapshotAsOf`), and the run continues against the
   cached snapshot.
3. `ScheduledReportService.CheckAndSendAsync()` is fired in the background
   (not awaited) — see §6.
4. The main form `Purchases` (`Form1.cs`) opens. `Purchases_Load` in turn opens
   `Login` on top of it, blocking interaction with the POS screen until sign-in.

## 2. Login

- `Login` is a **login-code** prompt, not a username/password form: the
  cashier types a code and presses Enter (`tbx_login_KeyUp`).
- On Enter, the app tries to refresh the `Users` table live
  (`DataCacheManager.RefreshTable`); if that fails it silently falls back to
  whatever is already cached, so login still works offline.
- The code is matched case-insensitively against `user_.name`; a match sets
  `id` to the user's `fullname` and marks the session authenticated.
- A successful login is recorded via `ActivityLog.LogAsync` (tagged
  `(offline)` if applicable) and best-effort stamps `user_.sales_token` with
  the current time (`TryStampSalesTokenAsync`) — this never blocks sign-in.
- `Escape` on the login screen exits the whole application.
- There is no logout button in the reviewed flow — the login form is only
  shown once, at startup, in front of `Purchases`.

## 3. Sales / checkout flow (`Purchases`, the main POS tab)

1. **Add an item**: the cashier scans/types a serial code into
   `tbx_serialcode`. Item lookup, pricing, and description come from the
   in-memory `ItemsTable` (backed by `DataCacheManager`'s `Items` cache), not
   a live query.
2. **Pricing tiers**: the `selltype` dropdown selects which price column
   applies (`price`, `retail_price`, or `price_perpc`) — `GetItemPrice`
   switches on `selltype.SelectedIndex`.
3. **Cart**: items accumulate in the `dgv_items` grid with a running
   `grandTotal` and item counter shown in `counter.Text`.
4. **Transaction code**: `CreateTransactionCode` mints a code
   (`TransactionCode-N`, N = current row count of `sales_` + 1) and inserts a
   `sales_` row per line item as the sale is built up
   (`SaveSaleLineItemsAsync` inserts one row per cart line, numbered
   `transactionCode-1`, `-2`, ...).
5. **Cash tendered / change**: `tbx_denomination` accepts the amount tendered;
   change is computed against `grandTotal`.
6. **Printing**: `printDocument1` renders the receipt to the thermal printer
   configured via `RECEIPT_PRINTER_NAME` (System Config), with paper size and
   margins derived from that printer's actual driver settings
   (`ApplyReceiptPrinterSettings`) so output fits an 80mm/58mm roll. Receipts
   can also be saved as PNG/JPEG (`SaveToPNG_Click`, `SaveCounterReceiptAsJpeg`).
7. **Removing a cart line**: deleting a `dgv_items` row
   (`dgv_items_UserDeletedRow`) restores the item's quantity in `items_` (or,
   offline, against the cached table via `AdjustCachedQuantity` +
   `LogPendingStockAdjustment`) and refreshes the items cache.

There is no separate "void completed sale" flow in the reviewed code — voiding
happens by removing the line from the cart before the sale is finalized.

## 4. Hold / resume transactions

- **Hold** (`holdTransactionToolStripMenuItem_Click`): snapshots the current
  cart (all `dgv_items` rows, sell-type, running total, transaction code) into
  an in-memory `heldTransactions` dictionary keyed by a ticket code (the
  current `TransactionCode`, or a generated `HOLD<timestamp>` if none yet),
  then clears the POS screen for a new sale. Held transactions live only in
  memory for the running session — they are not persisted to the database.
- **Resume** (`resumeTransactionToolStripMenuItem_Click` / F8): if exactly one
  ticket is held, it's resumed directly; with more than one, it opens
  `HeldTransactionsForm` (Ctrl+T) to pick one. Resuming refuses if the current
  cart isn't empty.
- **Cancel**: `CancelHeldTransaction` drops a ticket from the dictionary.

## 5. Manager / admin overrides (PIN gates)

`PinPromptForm` gates three tiers of access, checked against two different
`App.config` PINs:

| Menu item | PIN checked | Setting |
|---|---|---|
| Manage Prices tab | Manager PIN | `MANAGER_PIN` |
| Manage Users | Manager PIN | `MANAGER_PIN` |
| Tax Configuration | Manager PIN | `MANAGER_PIN` |
| System Configuration | Admin PIN | `ADMIN_PIN` |

System Configuration is deliberately behind a *separate* PIN from the other
three, since it exposes the DB connection string, receipt printer selection,
and both PIN values themselves — a store manager isn't meant to reach it. A
wrong or cancelled PIN shows an "Access Denied" message and aborts the action;
there's no lockout/retry-limit on repeated attempts.

## 6. Scheduled email reports

`ScheduledReportService` has no background service/timer of its own — the app
checks once at startup (`CheckAndSendAsync`, fired-and-forgotten from
`Program.Main`) whether a report is due, based on:
- `REPORT_SCHEDULE_FREQUENCY` (Daily/Weekly/Monthly/Quarterly/Yearly, from
  System Config > Report Schedule), and
- a last-sent timestamp in
  `Documents\App Inventory\OfflineCache\ScheduledReportLastSent.txt`
  (independent of `App.config`, so it survives config edits).

Whichever machine/instance is opened first after the interval elapses does the
send (a sales + inventory summary emailed to `STORE_EMAIL` via ClosedXML +
SMTP/Gmail API). A manual "Send Report Now" trigger also exists in System
Config. All failures here are swallowed — a broken mail send never blocks
startup, and leaving the last-sent marker untouched just means the next
launch retries.

## 7. Offline resilience (cross-cutting)

This is a deliberate design pattern reused everywhere the app touches the
database — see `CLAUDE.md` for the standing rule. Concretely, in this code:

- **Startup**: DB unreachable + existing snapshot → run from snapshot,
  `IsOfflineMode = true`. DB unreachable + no snapshot → hard failure dialog.
- **Login**: refresh-then-fallback — tries a live pull of `Users`, silently
  uses the cached copy if that fails.
- **Sales**: a completed sale that can't be written to `sales_` is instead
  appended to `OfflineCache\yyyy-MM\PendingSalesLogs.log` for manual
  reconciliation, and the cashier sees a `lbl_warning` banner
  ("OFFLINE - sale completed but not recorded...").
- **Cart-line removal**: stock restoration falls back to adjusting the cached
  `ItemsTable` in memory and logging to
  `OfflineCache\yyyy-MM\PendingStockAdjustments...` equivalent
  (`LogPendingStockAdjustment`).
- **Activity log**: `ActivityLog.LogAsync` falls back to
  `OfflineCache\yyyy-MM\PendingActivityLogs.log`.
- **Scheduled reports**: a failed send is silently skipped; retried on the
  next qualifying startup.

None of these offline fallbacks retry automatically or auto-replay once the
connection returns — the `.log` files under `OfflineCache` are meant to be
reconciled manually against the database later.

## 8. Other feature forms (opened from the `Purchases` menu)

Each of these is a standalone form reading from `DataCacheManager` and/or
running its own direct MySQL queries; none maintain state shared with the POS
screen beyond what's cached:

- `ListSales` / `TellerHistoryForm` — sales history views (all tellers vs. the
  current user's own transactions).
- `InventoryReport`, `SalesChartForm` (daily/monthly/quarterly) — reporting.
- `LoyaltyForm` — customer loyalty points against `customers_` /
  `loyalty_transactions_`.
- `PettyCashEntryForm` / `PettyCashHistoryForm` — cash movements against
  `pettycash_`.
- `AddItemForm` — new inventory item entry (feeds `items_`).
- `ManageUsersForm`, `TaxConfigForm`, `SystemConfigForm` — see §5 for their
  PIN gates.
- `GenerateBarcode` — barcode image generation/printing for items.
- `HeldTransactionsForm` — UI over the in-memory `heldTransactions` dictionary
  described in §4.
