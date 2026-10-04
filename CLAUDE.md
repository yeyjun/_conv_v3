# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A Windows Forms point-of-sale / inventory app ("A10D-up", store name "Baclig Minimart") targeting .NET Framework 4.7.2, backed by a remote MySQL database. `_conv.sln` has two projects: `_conv` (the POS app itself) and `RemoteController` (a standalone companion tool - license key generation and network status/activation, see below). There are no tests.

## Build

This is a classic .NET Framework project (packages.config), so use `msbuild`/`nuget`, not `dotnet`:

```powershell
nuget restore _conv.sln          # or let Visual Studio restore packages/
msbuild _conv.sln /p:Configuration=Debug
```

Output: `_conv\bin\Debug\A10D-up.exe`. If `msbuild` isn't on PATH, use the VS 2022 Developer PowerShell or the full path under `C:\Program Files\Microsoft Visual Studio\2022\`.

`FreshData.cs` at the repo root is a solution item only — it is not compiled into the project (and doesn't compile as-is; its `using MySql.Data.Client` is wrong).

## Architecture

- **`Form1.cs` contains the main form, class `Purchases`** (namespace `_conv`) — the name/file mismatch is easy to trip on. `Program.Main` runs `Purchases` directly; `Login`, `PinPromptForm`, and the other forms are opened from it. Most business logic (sales, receipts, held transactions, printing) lives in this ~2,200-line file.
- **`DataCacheManager.cs`** (namespace `YourApp.Data`) is a singleton cache of `DataTable`s loaded from MySQL once at startup (`LoadAll()` in `Program.Main`). Forms read via `GetTable("Items")` / `GetTable("Users")` instead of querying the DB. Successful loads are snapshotted to `Documents\App Inventory\OfflineCache\*.xml`; if the DB is unreachable the app runs in **offline mode** from the snapshot (`IsOfflineMode`, `OfflineSnapshotAsOf`). When adding a table the app needs at startup, register it in `LoadAll()`.
- **`ActivityLog.cs`** writes audit events to the `logs_` table, falling back to `Documents\App Inventory\OfflineCache\PendingActivityLogs.log` when offline (meant to be replayed manually later).
- **Direct DB access pattern:** forms build MySQL connection strings from `App.config` appSettings (`DB_SERVER`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`, `CONNECTION_TIMEOUT`) and run parameterized `MySqlCommand`s inline — there is no ORM or repository layer. Tables in use: `items_`, `user_`, `sales_`, `customers_`, `loyalty_transactions_`, `pettycash_`, `logs_`, `setting_` (note the trailing underscores).
- **`AppConfigService.cs`** reads/writes `setting_`, a shared key/value table — unlike everything else, which is per-machine `App.config`, a row written here is visible to every terminal on the same database. Currently used for **system maintenance mode** (`MAINTENANCE_MODE`/`MAINTENANCE_MESSAGE` keys, toggled from System Config > Maintenance): `Program.cs` checks it at startup and blocks with `MaintenanceForm` (same pattern as `TrialExpiredForm`) if on, and `Purchases` also polls it every 2 minutes (`StartMaintenanceWatcher` in `Form1.cs`) so a terminal already running gets blocked too. Either gate can be bypassed with the `ADMIN_PIN`. Reads fail open (treated as "off") on any DB error, per the offline-resilience convention below.
- **Manager overrides** (voids, sensitive actions) are gated by `PinPromptForm` against the `MANAGER_PIN` appSetting.
- **`AppFonts.cs`** loads embedded fonts (e.g. Morally Serif) into a `PrivateFontCollection`; labels using them need `UseCompatibleTextRendering = true` because GDI can't see memory fonts.
- Each feature is a separate WinForms form (`AddItemForm`, `ListSales`, `InventoryReport`, `LoyaltyForm`, `PettyCash`, `TellerHistoryForm`, `HeldTransactionsForm`, `ManageUsersForm`, `SalesChartForm`, `GenerateBarcode`, `printable`), each with a `.Designer.cs` — keep control layout changes in the Designer file as Visual Studio expects.
- **`TrialLicense.cs`** gates new installs behind a per-machine trial (`TRIAL_ENABLED`/`TRIAL_DAYS`, local start-date file), lifted by a per-store license key (`LICENSE_KEY` appSetting, or an AES-encrypted `.tmrgo` file via `TryActivateFromFile` - opening one in Notepad shows ciphertext, not the key) validated against a salted hash of `STORE_CODE`. Enforced at startup in `Program.cs` via `TrialExpiredForm`.
- **`RemoteControlServer.cs`** is an off-by-default (`REMOTE_CONTROL_ENABLED`), token-authenticated (`REMOTE_CONTROL_TOKEN`) TCP listener (`REMOTE_CONTROL_PORT`, reachable over LAN or, with router port-forwarding, the public internet) with a deliberately tiny, non-destructive command set (`PING`/`STATUS`/`ACTIVATE`/`GET_MENUS`/`SET_MENUS`) — see System Config > Remote Control. `SET_MENUS` writes `HIDDEN_MENUS` the same as System Config > Menus and immediately calls `ApplyMenuVisibility()` on the live `Purchases` window (found via `Application.OpenForms`, marshaled onto the UI thread) so a remote menu change takes effect without a restart. TCP connections are TLS-wrapped using a self-signed cert (`RemoteControlCertificate.cs`, generated via BouncyCastle - already a transitive dependency, so no new package was needed there - and cached at `Documents\App Inventory\OfflineCache\RemoteControlCert.pfx`); its thumbprint is shown in System Config for trust-on-first-use pinning in RemoteController. Repeated bad tokens from the same remote address get a temporary lockout (in-memory, per `RemoteControlServer`'s own tracking - resets on app restart). `BluetoothControlServer.cs` is a second, off-by-default (`REMOTE_CONTROL_BLUETOOTH_ENABLED`) listener reusing the exact same command handling over RFCOMM instead of TCP (no TLS there - OS-level Bluetooth pairing is its own trust step); it needs `InTheHand.Net.Bluetooth` (32feet.NET), vendored manually into `packages\` since this sandbox has no `nuget.exe` (see `packages.config` in both projects) - never assume Bluetooth actually works end-to-end without testing on real hardware, since no radio was available to verify it here. The `RemoteController` project is the client/counterpart for all of this: it has **no project reference** to `_conv` on purpose (meant to be handed to someone who only issues licenses, not build the POS app), so it duplicates the license-key algorithm and wire protocol - keep both copies in sync by hand if either changes.
- **`RemoteController\MainForm`** is a `TabControl` with three pages: License, Remote Control, and Fleet. **Fleet** (`FleetStore.cs`, `FleetEntryForm.cs`) is a purely local, per-machine list of known terminals (`Documents\RemoteController\Fleet.xml` - no server, doesn't sync between copies of RemoteController) - Store Name/Host/Port/Token/Latitude/Longitude, shown in a `DataGridView`. "Refresh All" PINGs every entry with its saved token (`FleetStore.CheckStatusAsync`), so status is really "Online" (correct token) vs "Reachable (bad token)" vs "Offline" (no connection at all), not just a port scan. `FleetEntryForm`'s "Detect (approximate)" button is an optional convenience that geolocates the entered host via `ip-api.com` (city-level accuracy at best, meaningless for a private/LAN address) - coordinates are always manually editable and never treated as authoritative.

## Conventions and gotchas

- Offline resilience is a deliberate design goal: DB failures should degrade to cached/snapshot data or local log files, never crash the app. Follow that pattern for new DB-touching code.
- Real production DB credentials and the manager PIN currently live in `App.config` — be careful not to spread them further (there's a standing TODO to move them out).
- ClickOnce publish settings live in the csproj (`PublishUrl` etc.); don't disturb them when editing the project file.
- Receipts/exports use ClosedXML (Excel), Aspose.PDF, and raw `System.Drawing.Printing`.
