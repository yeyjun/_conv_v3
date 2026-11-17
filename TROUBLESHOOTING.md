# Troubleshooting Manual — Baclig Minimart BrewCode

This is a symptom-first guide for cashiers, store managers, and whoever holds
the admin/system PINs. For how the app is supposed to behave, see
`PROCESS.md`; for code-level detail, see `CLAUDE.md`.

Three PINs gate different things throughout this guide:

| PIN | Setting | Unlocks |
|---|---|---|
| Manager PIN | `MANAGER_PIN` | Manage Prices, Manage Users, Tax Configuration |
| System Admin PIN | `ADMIN_PIN` | System Configuration, Edit Layout, maintenance-mode override |

---

## 1. App won't start

### "Failed to load startup data" error box, then the app closes
- **Cause**: the database is unreachable **and** this machine has never
  successfully loaded data before (no offline snapshot exists yet at
  `Documents\App Inventory\OfflineCache\Items.xml` / `Users.xml`).
- **Fix**: check the network connection and that `DB_SERVER` / `DB_NAME` /
  `DB_USER` / `DB_PASSWORD` in `App.config` are correct. A brand-new
  installation needs at least one successful connection before it can ever
  run offline.

### "Offline Mode" warning box at startup
- **Cause**: the database was unreachable, but a previous snapshot exists, so
  the app is continuing with cached data (as of the date/time shown in the
  box).
- **This is not an error** — the app is working as designed. Sales and
  inventory changes will not be saved to the database until the connection
  returns (see §9). Prices/stock may be stale as of the snapshot date shown.
- **Fix**: restore the network/database connection. No restart is required —
  `Items` and `Users` refresh themselves automatically once the database
  becomes reachable again (§9). If the snapshot date is suspiciously old,
  check whether this machine has been offline for multiple sessions in a row.

### A different unexpected error appears instead of the app crashing outright
- The app catches unhandled exceptions globally and shows a plain-language
  message box ("An unexpected error occurred and was recovered from...") or,
  for a timed-out network/DB call, "A background operation... timed out or
  was cancelled." In both cases **the app keeps running** — this is the
  intended safety net, not a bug. The event is also written to the activity
  log (or its offline fallback, §9) for later review.
- If this happens repeatedly, the message text itself is the actual
  exception's message — note it down and check the network/database health.

---

## 2. Trial, licensing, and subscription

### "Trial Expired" screen blocks startup
- **Cause**: `TRIAL_ENABLED` is on, no valid `LICENSE_KEY` is set, and the
  trial period (`TRIAL_DAYS`, default 30) has run out since this machine's
  first run.
- **Fix**: enter a license key on the screen, or import a `.tmrgo` file
  (issued by whoever manages licensing, via the `RemoteController` companion
  app's License Key Generator). A `.tmrgo` file is encrypted — opening it in
  Notepad will show gibberish, that's expected; use the "Import license
  file..." option on the Trial Expired screen instead.
- A license key can also be pushed remotely via `RemoteController`'s
  `ACTIVATE` command without anyone on-site typing it in (§6).

### Entered a license key but it's rejected
- Double-check it was typed/pasted exactly, including all dashes — a key is
  tied to this store's `STORE_CODE` in `App.config`; a key issued for a
  different store code will never validate here.
- If the key's expiry segment (the last group, `yyMMdd`, or `PERM` for
  perpetual) has already passed, it will show as expired even though it's
  otherwise a real key for this store — a renewed key is needed.

### Imported a `.tmrgo` file and it's rejected
- "That file isn't a valid .tmrgo license file" — the file is corrupted, or
  isn't actually a `.tmrgo` export (don't rename an unrelated file to this
  extension).
- "This license file was issued for store code X, not Y" — the file was
  generated for a different store; get one issued for this store's code
  instead.

### "Subscription Expired" screen blocks startup
- **Cause**: no active subscription token was found for this store's
  `STORE_NAME` in the current calendar month, on the centralized token
  server (separate database from the store's own, configured via
  `TOKEN_DB_*` settings).
- **Unlike every other offline check in this app, this one fails closed**:
  if the token server itself is unreachable, the gate blocks startup rather
  than letting it through. If you believe the subscription is actually
  current, first rule out a network problem reaching the token server before
  assuming it's a billing issue — but don't assume it's "just offline mode",
  since this gate is intentionally stricter than that.

---

## 3. Maintenance mode

### Cashiers can't log in anywhere, with a maintenance message shown
- **Cause**: `MAINTENANCE_MODE` is turned on in the shared `app_config_` table
  (System Config > Maintenance) — this affects **every terminal** on the same
  database, not just one machine, by design.
- **Fix**: an admin enters the System Admin PIN on the maintenance screen to
  override it for that session, or someone with System Config access turns
  `MAINTENANCE_MODE` back off from the Maintenance tab.

### A terminal that was already open suddenly shows the maintenance screen
- **Expected behavior**: a running `Purchases` window re-checks maintenance
  status every 2 minutes, so a toggle flipped on from another terminal
  reaches it without needing a restart.
- If an admin already overrode it once this session, that same terminal
  won't be re-prompted again until restart — but a terminal that hasn't
  overridden it yet will still be blocked on its next 2-minute check.

### Maintenance mode seems "stuck on" even after disabling it
- The check fails open (treated as "off") on a database error — so if it
  still shows as on, the setting genuinely hasn't been saved yet, or the
  terminal's next 2-minute poll hasn't happened yet. It is not silently
  cached beyond that poll interval.

---

## 4. Login

### "Login code not recognized"
- The code was checked against the locally cached `Users` list first; if not
  found there, the app tries one live refresh from the database before
  giving up. If the code is genuinely new (e.g. a user added seconds ago on
  another terminal) and the database is unreachable, the refresh will
  silently fail and the code won't be found until connectivity returns.
- Double check the code was typed correctly — this is a login **code**, not
  a username/password pair.

### "The login check timed out"
- The one-time refresh of `Users` (only triggered when the code wasn't found
  locally) couldn't reach the database within the configured timeout. Check
  the connection and try again — the field is left ready to retry.

---

## 5. Items, pricing, and stock

### A scanned barcode isn't found / shows the wrong price or stock
- Item data comes from the in-memory cache, not a live query every scan.
  If this machine is in offline mode, the cache reflects whatever was last
  successfully pulled (or the snapshot's as-of date shown at startup) — it
  will self-correct once reconnected (§9), with no restart needed.
- If online and a brand-new item was just added **on a different terminal**,
  this terminal's cache won't see it until its own next refresh of `Items`
  (on reconnect-watcher tick, or restart).

### An item added via "Add Item" while offline — where did it go?
- If the database couldn't be reached, the item is **not lost**: a "Saved
  Offline" message confirms it was queued locally
  (`OfflineCache\PendingItems.xml`) and merged into this machine's live
  cache, so it can be sold immediately in this session.
- It stays queued — and keeps reappearing after every cache reload,
  including after the database reconnects — **until someone with database
  access actually inserts it into `items_` for real** and removes its entry
  from the queue file by hand. This is intentionally a manual
  reconciliation step, same as the other "pending" logs below — it does not
  insert itself automatically just because the connection came back.
- Quantity sold against a still-queued item while offline is tracked
  correctly in the queue too, so re-syncing later won't undo those sales.

### Removed a cart line but the stock count looks off
- If online, removing a line restores the item's quantity directly in
  `items_`. If offline, the restoration is applied to the cached table and
  logged to a `PendingStockAdjustments...` file under
  `OfflineCache\yyyy-MM\` for manual reconciliation later — it is not
  written back to the database automatically when the connection returns.

---

## 6. Printing and receipts

### "No receipt printer is set up yet" at startup
- No printer is configured (`RECEIPT_PRINTER_NAME`), or the configured one
  isn't installed on this machine. The printer picker shown at that moment
  can be used immediately, or set later from System Config > Printer.

### Receipt is cut off, misaligned, or wrong paper size
- Margins and paper size are derived from the selected printer's own driver
  settings. Re-select the correct printer on System Config > Printer (even
  re-selecting the same one can refresh these settings), and confirm the
  roll size (80mm/58mm) matches what the driver expects.

### Need a copy of a receipt without reprinting on paper
- Receipts can be saved as PNG/JPEG from the sale screen instead of (or in
  addition to) printing.

---

## 7. Edit Layout mode

### "Edit Layout" option is missing from the menu
- It's hidden by default like several admin-only menu items
  (`HIDDEN_MENUS`). Enable it from System Config > Menus.

### Clicking "Edit Layout" does nothing / shows "Layout editing is turned off"
- It also requires `LAYOUT_EDIT_ENABLED` to be turned on separately, from
  System Config > Appearance ("Allow Edit Layout from the main menu") —
  menu visibility and the feature flag are two different switches. It then
  also asks for the System Admin PIN.

### While in Edit Layout mode, nothing on the main screen responds (buttons, grid, etc.)
- **This is expected** — while layout editing is on, every click on the main
  screen is captured to drag/resize whichever control you clicked, so a
  misplaced click can never accidentally ring up a sale or trigger another
  button. Choose "Exit Layout Edit" from the same menu to restore normal
  behavior; your repositioning is saved automatically at that point.

### A control got dragged off-screen or resized down to nothing
- Saved positions are automatically clamped to stay at least partially
  on-screen and above a minimum size, so it should always be reachable again
  from Edit Layout mode. If it's still unworkable, System Config's Layout tab
  can show and hand-edit the raw saved layout, or you can delete
  `Documents\App Inventory\OfflineCache\MainLayout.xml` on that machine to
  reset every control back to its designed position (this is per-machine —
  other terminals are unaffected).

---

## 8. Remote Control (RemoteController companion app)

### RemoteController shows a terminal as "Offline" in Fleet
- No TCP connection could be made at all — check the host/port are correct,
  the store's network allows the connection (firewall, and router port
  forwarding if connecting over the public internet), and that
  `REMOTE_CONTROL_ENABLED` is on with `REMOTE_CONTROL_TOKEN` set on that
  terminal (with no token configured, the listener doesn't start at all).

### RemoteController shows "Reachable (bad token)"
- The connection itself works, but the token saved in that Fleet entry
  doesn't match `REMOTE_CONTROL_TOKEN` on the target terminal. Re-enter the
  correct shared token in the Fleet entry.

### Repeated connection attempts suddenly all fail, even with the right token
- A remote address that failed authentication 5 times within 5 minutes is
  temporarily locked out for the rest of that window — this is an anti-abuse
  measure, not a bug. Wait out the window, or restart the store's app to
  clear it immediately (the lockout is in-memory only).

### RemoteController's certificate warning / thumbprint mismatch
- The TCP connection is TLS-encrypted with a self-signed certificate
  generated per-store. On first connecting to a given terminal, confirm its
  thumbprint (shown on that terminal's System Config > Remote Control) matches
  what RemoteController shows before trusting it going forward.

### Bluetooth control doesn't connect
- Confirm `REMOTE_CONTROL_BLUETOOTH_ENABLED` is on (it's a separate switch
  from the TCP listener) and that the two devices are already paired at the
  OS level — Bluetooth here relies entirely on normal OS pairing for trust,
  there is no additional token-independent step.

---

## 9. Offline resilience — what recovers automatically vs. manually

| Situation | Recovers automatically on reconnect? |
|---|---|
| `Items` / `Users` cache being stale | **Yes** — refreshed within ~60s (Items) or on next login (Users), snapshot file updated too |
| Maintenance mode flag | **Yes** — re-checked every 2 minutes while running |
| Items added offline (`PendingItems.xml`) | **No** — stays queued and keeps reappearing until manually inserted into `items_` and removed from the queue |
| Sales that couldn't be saved (`PendingSalesLogs`) | **No** — manual reconciliation against `sales_` |
| Stock adjustments from offline line removal (`PendingStockAdjustments...`) | **No** — manual reconciliation |
| Activity log entries (`PendingActivityLogs.log`) | **No** — manual replay |
| Scheduled report that failed to send | **Yes, implicitly** — retried automatically next time a report is due at startup |
| Access (`.accdb`) mirror / push-back | **No** — always a manual "Sync Now" / "Push to Online" action from System Config > Data Sync |

If something seems "stuck" offline, first confirm which category it falls
into above — most read-side caching recovers itself, but every queued
write needs a human to reconcile it against the real database.

---

## 10. Where to look for more detail

- **Activity log**: normally in the `logs_` table; while offline, falls back
  to `Documents\App Inventory\OfflineCache\PendingActivityLogs.log`.
- **Offline snapshots / queues**: all under
  `Documents\App Inventory\OfflineCache\` on each machine — `Items.xml`,
  `Users.xml`, `PendingItems.xml`, `MainLayout.xml`, `TrialStart.txt`,
  `ScheduledReportLastSent.txt`, `AccessSyncLastRun.txt`, and the dated
  `PendingSalesLogs`/`PendingStockAdjustments` files.
- **`PROCESS.md`** — full runtime behavior reference, section-by-section.
- **`CLAUDE.md`** — code layout, architecture, and build instructions, for
  anyone who needs to go spelunking in the source itself.
