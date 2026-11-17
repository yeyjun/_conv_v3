# App Process Documentation — Baclig Minimart BrewCode

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
3. **Receipt printer check**: if `RECEIPT_PRINTER_NAME` isn't set or doesn't
   match an installed printer (`Purchases.IsReceiptPrinterConfigured()`), a
   warning box is shown and `SelectPrinterForm` opens so one can be picked
   immediately (it can also be set later from System Config > Printer).
4. **Trial / license gate**: `TrialLicense.EnsureTrialStarted()` stamps the
   per-machine trial clock on first run; if `TRIAL_ENABLED` is on, no valid
   `LICENSE_KEY` is configured, and the trial has run out
   (`TrialLicense.IsExpired`), `TrialExpiredForm` blocks startup until an admin
   enters a key (typed in or imported from a `.tmrgo` file) or cancels, which
   exits the app. See §3.
5. **Maintenance gate**: `AppConfigService.GetMaintenanceStatusAsync()` reads
   the shared `app_config_` table; if `MAINTENANCE_MODE` is on, `MaintenanceForm`
   blocks startup until the `ADMIN_PIN` is entered or the user exits. See §8.
6. **Subscription gate**: `SubscriptionTokenService.IsActiveAsync()` checks a
   separate centralized token database for a row issued to this store's
   `STORE_NAME` within the current calendar month; if none is found,
   `SubscriptionExpiredForm` blocks startup. Unlike the maintenance check, this
   one **fails closed** on a connection error — it's a billing gate, not an
   offline-resilience path, so a blocked connection to the token server must
   not silently bypass it.
7. `ScheduledReportService.CheckAndSendAsync()` is fired in the background
   (not awaited) — see §10.
8. `RemoteControlServer.Restart()` starts the TCP (and, if separately enabled,
   Bluetooth) listener described in §9, then `Application.Run(new Purchases())`
   opens the main form. `RemoteControlServer.Stop()` runs in a `finally` so the
   listener always shuts down when the app exits.
9. The main form `Purchases` (`Form1.cs`) opens. `Purchases_Load` in turn opens
   `Login` on top of it, blocking interaction with the POS screen until sign-in.
   Once signed in, `Purchases_Load` also starts three background watchers that
   run for the rest of the session: the cache reconnect watcher (§11), the
   last-seen heartbeat, and the maintenance watcher (§8).

Any exception that slips past a form's own handling (e.g. a cancelled/timed-out
network call) is caught at the top level (`Program.HandleUnhandledException`)
and shown as a plain-language message instead of a crash dialog — consistent
with the offline-resilience goal of never tearing the app down mid-sale.

## 2. Login

- `Login` is a **login-code** prompt, not a username/password form: the
  cashier types a code and presses Enter (`tbx_login_KeyUp`).
- The cached copy of `Users` is checked first (instant, no network wait). Only
  if the code isn't found there does it try a live refresh of `Users`
  (`DataCacheManager.RefreshTable`) and check again; if a code *is* found
  locally, a refresh still runs in the background (not awaited) so the next
  login attempt has fresh data. Either way, login still works offline from
  whatever is cached.
- The code is matched case-insensitively against `user_.name`; a match sets
  `id` to the user's `fullname` and marks the session authenticated.
- A successful login is recorded via `ActivityLog.LogAsync` (tagged
  `(offline)` if applicable) and best-effort stamps `user_.last_seen_utc` with
  the current time — this never blocks sign-in.
- `Escape` on the login screen exits the whole application.
- There is no logout button in the reviewed flow — the login form is only
  shown once, at startup, in front of `Purchases`.

## 3. Licensing, trial & subscription gates

Three independent gates stack at startup (§1, steps 4–6); all three must pass
for the app to open:

- **Trial / license** (`TrialLicense.cs`): a per-machine trial window
  (`TRIAL_ENABLED`/`TRIAL_DAYS`), tracked from a local timestamp file
  (`OfflineCache\TrialStart.txt`) rather than `App.config`, so it survives
  config edits. Lifted by a `LICENSE_KEY` — a deterministic
  `XXXX-XXXX-XXXX-XXXX-EXPIRY` key (SHA256 of the store code + a salt + the
  expiry segment, so a tampered expiry no longer matches its own hash and
  can't silently extend itself; `EXPIRY` of `PERM` means perpetual) — entered
  by hand or imported from an AES-encrypted `.tmrgo` file
  (`TrialLicense.TryActivateFromFile`, produced by the standalone
  `RemoteController` companion app's License Key Generator; opening a
  `.tmrgo` file in a text editor shows ciphertext, not the key).
- **Maintenance mode**: see §8 — a separate, DB-driven (not per-machine) gate.
- **Subscription token**: `SubscriptionTokenService` checks a centralized
  token server (its own `TOKEN_DB_*` connection, separate from the store's own
  database) for a row matching `STORE_NAME` dated in the current calendar
  month. Fails **closed**: any connection/query error is treated as "not
  active", the opposite of every other DB check in this app, since a
  fail-open billing gate could be bypassed just by blocking outbound access to
  the token server.

All three can also be checked/driven remotely — `RemoteControlServer`'s
`STATUS` command reports offline/trial/licensed/days-remaining, and `ACTIVATE`
lets the `RemoteController` companion app push a license key to a running
instance without anyone typing it in on-site (§9).

## 4. Sales / checkout flow (`Purchases`, the main POS tab)

1. **Add an item**: the cashier scans/types a serial code into
   `tbx_serialcode`. Item lookup, pricing, and description come from the
   in-memory `ItemsTable` (backed by `DataCacheManager`'s `Items` cache, O(1)
   via a serial→row index rebuilt whenever the underlying table instance
   changes), not a live query.
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
8. **Adding a new catalog item mid-shift**: `AddItemForm` inserts straight
   into `items_` when the DB is reachable. When it isn't, the item is queued
   locally instead of lost — see §11 (offline item queueing).

There is no separate "void completed sale" flow in the reviewed code — voiding
happens by removing the line from the cart before the sale is finalized.

## 5. Edit Layout mode

Lets an admin reposition/resize controls on the main `Purchases` screen
directly on-screen, per machine, without touching the Designer:

- Gated by both a config flag (`LAYOUT_EDIT_ENABLED`, System Config >
  Appearance — "Allow Edit Layout from the main menu") and the `ADMIN_PIN`,
  checked in `editLayoutToolStripMenuItem_Click` (`Form1.cs`). It's also one
  of the menu items controllable via `HIDDEN_MENUS` (hidden by default).
- Turning it on installs an application-wide mouse message filter
  (`LayoutEditMessageFilter`, via `Application.AddMessageFilter`) that
  intercepts every left-button mouse message on the form and uses it to drag
  or resize whichever control the mouse went down on — the control's own
  `Click`/etc. never fire while this is active, so dragging the "Process
  Sale" button around can't also ring up a sale. A dashed box with a
  bottom-right resize handle (`LayoutEditOverlay`, painted via each parent's
  `Paint` event) outlines every movable control while this is on.
- "Movable" excludes forms, `TabControl`/`TabPage`, `MenuStrip`/`StatusStrip`,
  and any control that's `Docked` or non-default `Anchor`ed — those already
  reposition themselves on resize, so forcing an absolute position on them
  would fight WinForms' own layout and could end up off-screen at a different
  window size.
- Turning it back off (menu item toggles to "Exit Layout Edit") removes the
  message filter and overlay and calls `MainLayoutStore.SaveFrom(this)`,
  persisting every moved/resized control's name, location, and size to
  `Documents\App Inventory\OfflineCache\MainLayout.xml` (per-machine, not
  synced). `MainLayoutStore.ApplyTo` restores it on every subsequent launch
  (`Purchases`' constructor, alongside `LogoCache.ApplyTo`/`AppTheme.ApplyTo`),
  clamping any saved size/position back on-screen and above a minimum size so
  a bad save can never make a control permanently unreachable.
- System Config's Layout tab can also show/edit/apply this XML directly as
  text (`MainLayoutStore.ReadRawXml`/`WriteRawXml`), independent of actually
  entering Edit Layout mode.

## 6. Hold / resume transactions

- **Hold** (`holdTransactionToolStripMenuItem_Click`, both now thin wrappers
  around `Purchases.HoldCurrentTransaction(remarks, silent)`): snapshots the
  current cart (all `dgv_items` rows, sell-type, running total, transaction
  code) into an in-memory `heldTransactions` dictionary keyed by a ticket code
  (the current `TransactionCode`, or a generated `HOLD<timestamp>` if none
  yet), then clears the POS screen for a new sale. Held transactions are not
  persisted to the database - only to a local file (see below).
  `remarks` (null for a manual hold) shows up in `HeldTransactionsForm`'s
  Remarks column; `silent` skips the confirmation `MessageBox` in favor of a
  tray balloon, used by the auto-hold watcher below (nothing may be at the
  register to dismiss a modal dialog).
- **Preview** (`Purchases.PreviewHeldTransaction`/`EndHeldTransactionPreview`):
  selecting a ticket in `HeldTransactionsForm` (not Resume) shows its items on
  the cart board as a read-only look, without actually resuming it — the real
  cart (often empty) is snapshotted first and restored once the selection
  clears or the window closes. `tbx_serialcode` is disabled and a one-time
  tray notification explains why while a preview is showing; the window also
  goes `TopMost` so it can't get buried behind the main form mid-preview, then
  drops `TopMost` again once a ticket is actually resumed
  (`ResumeHeldTransaction` explicitly un-tops and `SendToBack`s it) so it
  doesn't keep floating over the cart while the cashier scans into it. It's
  positioned just to the left of the cart board (`gbx_itemList`) both when it
  first opens (`ShowHeldTransactionsWindow`) and while re-anchoring itself
  during a preview, via `Purchases.GetCartBoardScreenLocation()` - the one way
  in for `HeldTransactionsForm` to find gbx_itemList's screen position, since
  the control itself is private to Purchases.
- **Item Search while Held Transactions is open** (`ItemQuickSearchForm.cs`,
  `Purchases.ShowItemQuickSearchWindow`): `HeldTransactionsForm` sitting just
  to the cart board's left also covers the main form's own embedded search
  box/grid (`gbx_itemsearch`, which lives at that exact spot), so the main
  form's usual F4 ("jump to the item search box") can't reach it there.
  Pressing F4 while `HeldTransactionsForm` has focus instead opens this
  standalone, price-check-only window (serial/description plus all three
  price tiers, live-filtered as you type, capped at 100 rows) - read-only, no
  way to add an item to the cart from it, anchored just to the left of
  `HeldTransactionsForm`'s own current position.
- **Auto-hold idle cart** (`Purchases.StartIdleCartWatcher`/`CheckIdleCart`,
  polled every `IdleCartPollMs` via `idleCartCheckTimer`): if `dgv_items` has
  had no rows added/removed (`_lastCartActivity`, bumped by
  `dgv_items_RowsAdded`/`RowsRemoved`) for `AUTO_HOLD_IDLE_MINUTES` minutes
  (default 30) and `AUTO_HOLD_IDLE_ENABLED` is on, the cart is held
  automatically via `HoldCurrentTransaction(remarks: "Auto-held after N
  minute(s) of inactivity.", silent: true)`. Skipped while the cart board is
  showing a preview rather than a real transaction. Both settings are
  reconfigurable from System Config > Settings > Sales Defaults (checkbox +
  minutes, same group box as the default-sell-type setting).
- **Surviving an unexpected shutdown** (`HeldTransactionStore.cs`,
  `Purchases.SaveHeldTransactions`/`RestoreHeldTransactions`): held
  transactions used to live only in memory, so a crash, power loss, or forced
  close lost every cart a cashier had set aside. `SaveHeldTransactions`
  mirrors the whole `heldTransactions` dictionary to
  `OfflineCache\HeldTransactions.xml` after every hold/resume/cancel (plain
  text cells, not typed values - tolerated the same way `ToDecimal`/`ToInt`
  already tolerate `dgv_items`' own string-backed cells day to day), so the
  file and the dictionary always agree regardless of how the previous session
  ended. `RestoreHeldTransactions` reads it back once at startup
  (`Purchases_Load`) and - if anything was left over - merges it into
  `heldTransactions` and shows a tray notification with the count, rather
  than silently discarding it.
- **Resume** (`resumeTransactionToolStripMenuItem_Click` / F8): if exactly one
  ticket is held, it's resumed directly; with more than one, it opens
  `HeldTransactionsForm` to pick one. Resuming refuses if the current cart
  isn't empty.
- **View Held Transactions**: opened from the menu or the numpad `/` key.
  That shortcut is handled by `TransactionShortcutFilter`, an
  `Application.AddMessageFilter` installed for the app's whole lifetime
  (rather than a `ToolStripMenuItem.ShortcutKeys`) so it fires no matter which
  control currently has focus — a plain menu shortcut never fires while a
  text box is focused, since the key is treated as literal input instead.
  `/` was picked because it has no legitimate use in any of this app's
  numeric entry boxes, so it's swallowed outright with nothing to undo.
  `HeldTransactionsForm` itself floats non-modally (`Show()`, not
  `ShowDialog`) beside the main window; pressing Escape there closes it
  (`HeldTransactionsForm_KeyDown`), and while it's still open, Escape reaching
  `tbx_serialcode` on the main form instead just activates it rather than
  deselecting the selling type. Closing it via Escape also sets
  `Purchases.suppressNextSerialKeyUp` first — Windows delivers that same
  Escape press's key-up to whatever control regains focus afterward
  (`tbx_serialcode`), and without the flag that trailing key-up would be
  misread as a fresh Escape there and reset the selling type/cart.
- **Cancel**: `CancelHeldTransaction` drops a ticket from the dictionary.

## 7. Manager / admin overrides (PIN gates)

`PinPromptForm` gates access, checked against two different `App.config`
PINs:

| Menu item | PIN checked | Setting |
|---|---|---|
| Manage Prices tab | Manager PIN | `MANAGER_PIN` |
| Manage Users | Manager PIN | `MANAGER_PIN` |
| Tax Configuration | Manager PIN | `MANAGER_PIN` |
| System Configuration | Admin PIN | `ADMIN_PIN` |
| Edit Layout (§5) | Admin PIN | `ADMIN_PIN` |
| Maintenance-mode admin override (§8) | Admin PIN | `ADMIN_PIN` |

System Configuration (and, for the same reason, Edit Layout and the
maintenance override) is deliberately behind a *separate* PIN from the other
three, since System Config exposes the DB connection string, receipt printer
selection, and both PIN values themselves — a store manager isn't meant to
reach it. A wrong or cancelled PIN shows an "Access Denied" message and aborts
the action; there's no lockout/retry-limit on repeated attempts (contrast with
Remote Control's lockout, §9, which exists because that surface can be
internet-reachable).

## 8. Maintenance mode

A store-wide "stop selling" switch, deliberately stored in the shared
`app_config_` table (`AppConfigService`) rather than per-machine `App.config`, so
flipping it from any one terminal's System Config takes effect on every
terminal sharing that database — including ones already running, not just new
launches:

- **Startup**: `Program.Main` checks `AppConfigService.GetMaintenanceStatusAsync()`
  before opening `Purchases`; if on, `MaintenanceForm` blocks until the
  `ADMIN_PIN` is entered (`DialogResult.OK`) or the user exits (app closes).
- **Already running**: `Purchases.StartMaintenanceWatcher` polls the same
  check every 2 minutes (`MaintenanceCheckPollMs`) for the rest of the
  session. If it comes back on, the same `MaintenanceForm` gate is shown; a
  successful override (`_maintenanceOverridden`) is remembered for the rest
  of that session so the same terminal isn't re-prompted on the next poll. No
  override and no exit from the dialog closes the app outright.
- **Fails open**: `GetMaintenanceStatusAsync` treats any DB error as "off" —
  a maintenance check should never itself be the reason a store can't sell.
- Toggled from System Config's Maintenance tab (`AppConfigService.SetMaintenanceModeAsync`,
  which does *not* fail silently — its own caller shows any save error).

## 9. Remote Control & Fleet

`RemoteControlServer` (`RemoteControlServer.cs`) is an off-by-default,
token-authenticated listener that lets the standalone `RemoteController`
companion app (a separate project, no reference to this one — see
`CLAUDE.md`) talk to a running store instance for health checks, remote menu
control, and pushing a license key, without needing someone on-site:

- **Enablement**: starts only if `REMOTE_CONTROL_ENABLED` is true *and*
  `REMOTE_CONTROL_TOKEN` is set (System Config > Remote Control); started/
  restarted from `Program.Main` (`RemoteControlServer.Restart()`) and stopped
  in a `finally` on exit.
- **Transports**: a TCP listener (`REMOTE_CONTROL_PORT`, default 51820;
  reachable over LAN or, with router port-forwarding, the public internet),
  TLS-wrapped with a self-signed certificate (`RemoteControlCertificate.cs`,
  generated via BouncyCastle and cached under `OfflineCache\RemoteControlCert.pfx`
  — its thumbprint is shown in System Config for trust-on-first-use pinning in
  `RemoteController`); and a second, separately-toggled
  (`REMOTE_CONTROL_BLUETOOTH_ENABLED`) RFCOMM listener
  (`BluetoothControlServer.cs`) reusing the exact same command handling with
  no TLS (Bluetooth pairing is its own trust step).
- **Protocol**: one line per request, one connection per request —
  `{token}|{COMMAND}[|{arg}]` → `OK|...` or `ERR|{reason}`. Commands:
  - `PING` — store name/code, a liveness check.
  - `STATUS` — offline mode, trial enabled/licensed/days-remaining.
  - `ACTIVATE|{licenseKey}` — remotely pushes/validates a license key (§3).
  - `GET_MENUS` — every configurable menu item and which are currently hidden.
  - `SET_MENUS|{comma-separated hidden names}` — writes `HIDDEN_MENUS` (same
    as System Config > Menus) and immediately calls `ApplyMenuVisibility()`
    on the live `Purchases` window (found via `Application.OpenForms`,
    marshaled onto the UI thread), so a remote menu change takes effect
    without restarting the terminal.
- **Security**: every request must present the exact configured token or it's
  rejected; a remote address (IP or Bluetooth address) that fails 5 times
  within a 5-minute window is locked out for that window — in-memory only,
  resets on app restart. The command set is deliberately tiny and
  non-destructive: nothing here deletes data, changes prices, or touches a
  sale.
- **Fleet** (`RemoteController\FleetStore.cs`/`FleetEntryForm.cs`): a purely
  local, per-machine list of known terminals (Store Name/Host/Port/Token/
  Latitude/Longitude), stored at `Documents\RemoteController\Fleet.xml` — no
  server, doesn't sync between copies of `RemoteController`. "Refresh All"
  `PING`s every entry with its saved token, so status reflects "Online"
  (correct token) vs. "Reachable (bad token)" vs. "Offline" (no connection at
  all) — not just a port scan. An optional "Detect (approximate)" button
  geolocates a host via `ip-api.com` (city-level at best, meaningless for a
  LAN address); coordinates are always manually editable and never treated as
  authoritative.

## 10. Scheduled email reports

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

## 11. Offline resilience & reconnection (cross-cutting)

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
- **New items** (`AddItemForm`): if `items_` can't be reached, the item is
  queued locally instead of lost — `PendingItemStore.Add` writes it to
  `OfflineCache\PendingItems.xml` and layers it onto the live `Items` cache
  immediately (`MergeInto`, swapping in a new `DataTable` instance so any
  cached lookup, e.g. `Purchases.FindItemRow`'s serial index, rebuilds and
  picks it up), so it's ready to ring up for a sale this session. Any sale
  (or cart-line removal) against a still-pending item adjusts its *queued*
  quantity too (`PendingItemStore.AdjustQuantityIfPending`/
  `SetQuantityIfPending`), so a later re-merge never stamps it back to its
  original quantity and silently undoes those sales. The queue is re-merged
  on every future cache load — including a later successful reconnect — until
  someone actually inserts the item for real into `items_` and removes its
  file entry by hand (same manual-reconciliation convention as the pending
  `.log` files above).
- **Activity log**: `ActivityLog.LogAsync` falls back to
  `OfflineCache\yyyy-MM\PendingActivityLogs.log`.
- **Operation log** (`OperationLog.cs`) is not an offline-only fallback like
  the entries above — it writes unconditionally, every time, regardless of
  whether the corresponding DB write succeeded. Every create, read/scan,
  update, delete, and login app-wide, plus every completed sale, gets
  appended as its own `<Operation id="OP#" module="...">` element — all
  sharing the same numbering — in `OfflineCache\Operations\yyyy-MM-dd.xml`,
  one XML file per calendar day. `module` names the form/class that
  performed the operation (passed explicitly, e.g. `nameof(Purchases)`, not
  inferred from the call stack). The `OP#` number is derived by scanning the
  highest id already in that day's file (not an in-memory counter), so it
  stays correct across app restarts within the same day and simply restarts
  at `OP1` the next day.
  - **Login** (`Login.cs`): every attempt, success or failure
    (`OperationLog.RecordLogin`).
  - **Create**: `AddItemForm` (new item), `ManageUsersForm`/`LoyaltyForm`
    (new user/customer), `PettyCashEntryForm` (new entry).
  - **Read/scanned**: `Purchases` logs every item scanned into the cart
    during checkout (both the F7 and Enter/scanner paths in
    `tbx_serialcode_KeyUp`).
  - **Update**: `ManageUsersForm`/`LoyaltyForm` (edits), `TaxConfigForm`
    (tax rate), `LoyaltyForm`'s credit-points action, and `Purchases`'
    Manage Prices tab saving price changes.
  - **Delete**: `ManageUsersForm`/`LoyaltyForm` (user/customer removal) and
    `Purchases`' Manage Prices tab deleting an item.
  - **Sale**: `Purchases.SaveSaleLineItemsAsync` (`OperationLog.RecordSale`).
  - Everything else `ActivityLog.LogAsync` records that isn't one of the
    above (unhandled exceptions in `Program.cs`, scheduled-report emails in
    `ScheduledReportService.cs`) is logged generically via
    `OperationLog.RecordAction` alongside the existing `ActivityLog` call.
- **Scheduled reports**: a failed send is silently skipped; retried on the
  next qualifying startup.
- **Maintenance check**: fails open (treated as "off") on any DB error (§8).
- **Subscription check**: the one deliberate exception — fails *closed* on
  any DB error (§3).

**Automatic reconnection — Items/Users do self-heal once the DB comes back:**
- `Purchases.StartCacheReconnectWatcher` (started after login) listens for
  `NetworkChange.NetworkAvailabilityChanged` and also polls every 60 seconds
  (`CacheReconnectPollMs`); whenever `DataCacheManager.IsOfflineMode` is
  still true, it re-pulls `Items` from the database
  (`RefreshItemsCache`/`DataCacheManager.RefreshTable`). A successful pull
  clears `IsOfflineMode` and overwrites the on-disk snapshot
  (`Items.xml`), so both the in-memory cache and the offline snapshot are
  current again with no restart needed.
- `Users` gets the equivalent treatment through the login flow instead of a
  timer (§2) — every login attempt re-pulls it live, refreshing the cache and
  its snapshot the same way.
- What is **not** auto-replayed: the `.log`/`.xml` files that record offline
  activity that already *happened* while disconnected — `PendingSalesLogs`,
  `PendingStockAdjustments`, `PendingActivityLogs`, and `PendingItems.xml`
  (until manually inserted for real) — are meant to be reconciled by hand
  against the database once the connection is back. Only the read-side
  caches (`Items`, `Users`) refresh themselves automatically; nothing here
  re-plays a write automatically.
- **Manual full-dataset mirror** (separate from the above): System Config's
  Data Sync tab (`AccessSyncService.cs`) one-way-copies every mirrored MySQL
  table (`items_`, `user_`, `sales_`, `customers_`, `loyalty_transactions_`,
  `pettycash_`, `logs_`) into a local Access file
  (`Documents\App Inventory\BacligMinimart.accdb`) for offline viewing/
  reporting — this is a manual "Sync Now" action, not automatic, and the app
  itself never reads from the `.accdb`. Its one write-back path, "Push to
  Online" (`PushItemsToOnlineAsync`), reconciles item rows edited in the
  offline Access copy back up into `items_` (update if the serial exists,
  insert if not) — also manual, triggered from the same tab.

## 12. Other feature forms (opened from the `Purchases` menu)

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
- `AddItemForm` — new inventory item entry (feeds `items_`; queues locally if
  offline — §11).
- `ManageUsersForm`, `TaxConfigForm`, `SystemConfigForm` — see §7 for their
  PIN gates. `SystemConfigForm` is a multi-tab settings screen: Settings,
  Network Monitor, Menus, Printer, Appearance, Layout (§5), Report Schedule,
  Data Sync (§11), Trial / Licensing (§3), Database Setup, Remote Control
  (§9), Maintenance (§8), and Receipt Testing.
- `GenerateBarcode` — barcode image generation/printing for items.
- `HeldTransactionsForm` — UI over the in-memory `heldTransactions` dictionary
  described in §6.
- `SelectPrinterForm`, `TrialExpiredForm`, `MaintenanceForm`,
  `SubscriptionExpiredForm` — blocking startup gates, see §1/§3/§8.
