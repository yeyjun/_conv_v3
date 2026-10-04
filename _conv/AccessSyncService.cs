using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace _conv
{
    /// <summary>
    /// One-way copy of the online MySQL tables into a local Microsoft Access (.accdb) file,
    /// triggered manually from System Configuration > Data Sync. Each table is fully replaced
    /// (DELETE then re-INSERT) rather than merged, so the .accdb always ends up an exact mirror
    /// of MySQL at sync time - simpler and safer than reconciling row-by-row diffs for a file
    /// that's meant purely as an offline-viewable reference/reporting copy, not something the
    /// app itself reads from.
    /// </summary>
    internal static class AccessSyncService
    {
        private const string ProviderName = "Microsoft.ACE.OLEDB.12.0";

        // RootNamespace (_conv) + folder + filename, dots instead of slashes - how csc names
        // embedded resources by default.
        private const string TemplateResourceName = "_conv.Resources.AccessSyncTemplate.accdb";

        private sealed class ColumnSpec
        {
            public string AccessColumn;
            public string MySqlColumn; // null => no confirmed MySQL source; always synced as DBNull
            public OleDbType Type;

            public static ColumnSpec Mapped(string name, OleDbType type) =>
                new ColumnSpec { AccessColumn = name, MySqlColumn = name, Type = type };

            public static ColumnSpec MappedAs(string accessColumn, string mySqlColumn, OleDbType type) =>
                new ColumnSpec { AccessColumn = accessColumn, MySqlColumn = mySqlColumn, Type = type };

            public static ColumnSpec Unmapped(string name, OleDbType type) =>
                new ColumnSpec { AccessColumn = name, MySqlColumn = null, Type = type };
        }

        private sealed class TableSpec
        {
            public string MySqlSelect;
            public ColumnSpec[] Columns;
        }

        // Column lists mirror exactly what the app itself selects/inserts against MySQL - see
        // Form1.cs (items_/sales_), Login.cs and ManageUsersForm.cs (user_), LoyaltyForm.cs
        // (customers_/loyalty_transactions_), PettyCashEntryForm.cs (pettycash_), and
        // ActivityLog.cs (logs_) - kept in lockstep with Resources\AccessSyncTemplate.accdb.
        private static readonly Dictionary<string, TableSpec> Tables = new Dictionary<string, TableSpec>
        {
            ["items_"] = new TableSpec
            {
                MySqlSelect = "SELECT serial, description, price, retail_price, price_perpc, new_quantity, qty_inpc, date_of_entry FROM items_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("serial", OleDbType.VarWChar),
                    ColumnSpec.Mapped("description", OleDbType.VarWChar),
                    ColumnSpec.Mapped("price", OleDbType.Currency),
                    ColumnSpec.Mapped("retail_price", OleDbType.Currency),
                    ColumnSpec.Mapped("price_perpc", OleDbType.Currency),
                    ColumnSpec.Mapped("new_quantity", OleDbType.Integer),
                    ColumnSpec.Mapped("qty_inpc", OleDbType.Integer),
                    ColumnSpec.Mapped("date_of_entry", OleDbType.Date),
                }
            },
            // The Access template's column is still named sales_token (unchanged, to avoid
            // touching the binary .accdb template), but its source is now last_seen_utc - see
            // Login.cs/ManageUsersForm.cs, which moved off sales_token for last-activity tracking.
            ["user_"] = new TableSpec
            {
                MySqlSelect = "SELECT name, fullname, last_seen_utc FROM user_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("name", OleDbType.VarWChar),
                    ColumnSpec.Mapped("fullname", OleDbType.VarWChar),
                    ColumnSpec.MappedAs("sales_token", "last_seen_utc", OleDbType.Date),
                }
            },
            ["sales_"] = new TableSpec
            {
                MySqlSelect = "SELECT transaction_code, itemcode, transaction_date, seller, item_price FROM sales_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("transaction_code", OleDbType.VarWChar),
                    ColumnSpec.Mapped("itemcode", OleDbType.VarWChar),
                    ColumnSpec.Mapped("transaction_date", OleDbType.Date),
                    ColumnSpec.Mapped("seller", OleDbType.VarWChar),
                    ColumnSpec.Mapped("item_price", OleDbType.Currency),
                }
            },
            ["customers_"] = new TableSpec
            {
                MySqlSelect = "SELECT loyalty_code, full_name, contact_number, points, date_registered FROM customers_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("loyalty_code", OleDbType.VarWChar),
                    ColumnSpec.Mapped("full_name", OleDbType.VarWChar),
                    ColumnSpec.Mapped("contact_number", OleDbType.VarWChar),
                    ColumnSpec.Mapped("points", OleDbType.Integer),
                    ColumnSpec.Mapped("date_registered", OleDbType.Date),
                }
            },
            ["loyalty_transactions_"] = new TableSpec
            {
                MySqlSelect = "SELECT loyalty_code, amount_spent, points_earned, teller, date_credited FROM loyalty_transactions_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("loyalty_code", OleDbType.VarWChar),
                    ColumnSpec.Mapped("amount_spent", OleDbType.Currency),
                    ColumnSpec.Mapped("points_earned", OleDbType.Integer),
                    ColumnSpec.Mapped("teller", OleDbType.VarWChar),
                    ColumnSpec.Mapped("date_credited", OleDbType.Date),
                }
            },
            ["pettycash_"] = new TableSpec
            {
                MySqlSelect = "SELECT cash, date_now FROM pettycash_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("cash", OleDbType.Currency),
                    ColumnSpec.Mapped("date_now", OleDbType.Date),
                }
            },
            // Only event_log is ever written/read by ActivityLog.cs - log_date has no confirmed
            // MySQL source, so it's always synced as null rather than guessing a value.
            ["logs_"] = new TableSpec
            {
                MySqlSelect = "SELECT event_log FROM logs_",
                Columns = new[]
                {
                    ColumnSpec.Mapped("event_log", OleDbType.LongVarWChar),
                    ColumnSpec.Unmapped("log_date", OleDbType.Date),
                }
            },
        };

        private static string StateFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "AccessSyncLastRun.txt");

        public static string GetDefaultPath() => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "BacligMinimart.accdb");

        public static DateTime? GetLastSync()
        {
            if (!File.Exists(StateFilePath))
                return null;

            return DateTime.TryParse(File.ReadAllText(StateFilePath), out DateTime last) ? (DateTime?)last : null;
        }

        private static void SetLastSync(DateTime when)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(StateFilePath));
                File.WriteAllText(StateFilePath, when.ToString("o"));
            }
            catch
            {
                // Best-effort only - never let this block reporting the sync as successful.
            }
        }

        private static string PushStateFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "AccessPushLastRun.txt");

        public static DateTime? GetLastPush()
        {
            if (!File.Exists(PushStateFilePath))
                return null;

            return DateTime.TryParse(File.ReadAllText(PushStateFilePath), out DateTime last) ? (DateTime?)last : null;
        }

        private static void SetLastPush(DateTime when)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PushStateFilePath));
                File.WriteAllText(PushStateFilePath, when.ToString("o"));
            }
            catch
            {
                // Best-effort only - never let this block reporting the push as successful.
            }
        }

        /// <summary>
        /// Copies every mirrored table from MySQL into the given .accdb, creating the file from
        /// the bundled empty-schema template first if it doesn't exist yet. Returns the total
        /// row count copied. Reports per-table progress via <paramref name="progress"/>.
        /// </summary>
        public static async Task<int> SyncAsync(string accessDbPath, IProgress<string> progress = null)
        {
            if (string.IsNullOrWhiteSpace(accessDbPath))
                throw new ArgumentException("No Access file path is configured.");

            Directory.CreateDirectory(Path.GetDirectoryName(accessDbPath));

            if (!File.Exists(accessDbPath))
            {
                progress?.Report("Creating new Access file from template...");
                ExtractTemplate(accessDbPath);
            }

            string mySqlConnStr =
                $"Server={ConfigurationManager.AppSettings["DB_SERVER"]};" +
                $"user id={ConfigurationManager.AppSettings["DB_USER"]};" +
                $"password={ConfigurationManager.AppSettings["DB_PASSWORD"]};" +
                $"database={ConfigurationManager.AppSettings["DB_NAME"]};" +
                $"Connect Timeout={ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"]};";

            string accessConnStr = $"Provider={ProviderName};Data Source={accessDbPath};";

            int totalRows = await Task.Run(() =>
            {
                int rows = 0;
                foreach (KeyValuePair<string, TableSpec> kvp in Tables)
                {
                    string table = kvp.Key;
                    TableSpec spec = kvp.Value;

                    progress?.Report($"Reading {table} from the online database...");
                    var data = new DataTable();
                    using (var mysqlConn = new MySqlConnection(mySqlConnStr))
                    using (var adapter = new MySqlDataAdapter(spec.MySqlSelect, mysqlConn))
                        adapter.Fill(data);

                    progress?.Report($"Writing {data.Rows.Count} row(s) to {table}...");
                    using (var accessConn = new OleDbConnection(accessConnStr))
                    {
                        accessConn.Open();
                        using (OleDbTransaction tx = accessConn.BeginTransaction())
                        {
                            using (var delete = new OleDbCommand($"DELETE FROM {table}", accessConn, tx))
                                delete.ExecuteNonQuery();

                            string columnList = string.Join(", ", spec.Columns.Select(c => c.AccessColumn));
                            string placeholders = string.Join(", ", spec.Columns.Select(_ => "?"));
                            using (var insert = new OleDbCommand($"INSERT INTO {table} ({columnList}) VALUES ({placeholders})", accessConn, tx))
                            {
                                foreach (ColumnSpec col in spec.Columns)
                                    insert.Parameters.Add(new OleDbParameter { OleDbType = col.Type });

                                foreach (DataRow row in data.Rows)
                                {
                                    for (int i = 0; i < spec.Columns.Length; i++)
                                    {
                                        ColumnSpec col = spec.Columns[i];
                                        insert.Parameters[i].Value = col.MySqlColumn != null ? row[col.MySqlColumn] : DBNull.Value;
                                    }
                                    insert.ExecuteNonQuery();
                                }
                            }

                            tx.Commit();
                        }
                    }

                    rows += data.Rows.Count;
                }
                return rows;
            });

            SetLastSync(DateTime.Now);
            progress?.Report($"Sync complete - {totalRows} row(s) copied.");
            return totalRows;
        }

        /// <summary>
        /// Pushes item rows from the local offline Access file back up into MySQL, matching by
        /// serial: a serial that already exists online gets its row updated, a serial that
        /// doesn't gets inserted. This is the reverse of <see cref="SyncAsync"/> - it's the one
        /// path that writes offline-entered/edited item data back to the online database, for
        /// reconciling changes made while the store was offline.
        /// </summary>
        public static async Task<(int updated, int inserted)> PushItemsToOnlineAsync(string accessDbPath, IProgress<string> progress = null)
        {
            if (string.IsNullOrWhiteSpace(accessDbPath))
                throw new ArgumentException("No Access file path is configured.");
            if (!File.Exists(accessDbPath))
                throw new FileNotFoundException("Access file does not exist yet - run a sync first.", accessDbPath);

            ColumnSpec[] columns = Tables["items_"].Columns;
            string accessConnStr = $"Provider={ProviderName};Data Source={accessDbPath};";
            string mySqlConnStr =
                $"Server={ConfigurationManager.AppSettings["DB_SERVER"]};" +
                $"user id={ConfigurationManager.AppSettings["DB_USER"]};" +
                $"password={ConfigurationManager.AppSettings["DB_PASSWORD"]};" +
                $"database={ConfigurationManager.AppSettings["DB_NAME"]};" +
                $"Connect Timeout={ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"]};";

            (int updated, int inserted) result = await Task.Run(() =>
            {
                progress?.Report("Reading items from the offline Access file...");
                var data = new DataTable();
                using (var accessConn = new OleDbConnection(accessConnStr))
                using (var adapter = new OleDbDataAdapter(
                    $"SELECT {string.Join(", ", columns.Select(c => c.AccessColumn))} FROM items_", accessConn))
                    adapter.Fill(data);

                progress?.Report($"Pushing {data.Rows.Count} item(s) to the online database...");
                int updatedCount = 0, insertedCount = 0;

                using (var mysqlConn = new MySqlConnection(mySqlConnStr))
                {
                    mysqlConn.Open();

                    foreach (DataRow row in data.Rows)
                    {
                        object serial = row["serial"];
                        if (serial == DBNull.Value)
                            continue;

                        bool exists;
                        using (var check = new MySqlCommand("SELECT COUNT(*) FROM items_ WHERE serial = @serial", mysqlConn))
                        {
                            check.Parameters.AddWithValue("@serial", serial);
                            exists = Convert.ToInt32(check.ExecuteScalar()) > 0;
                        }

                        string commandText = exists
                            ? "UPDATE items_ SET description=@description, price=@price, retail_price=@retail_price, " +
                              "price_perpc=@price_perpc, new_quantity=@new_quantity, qty_inpc=@qty_inpc, date_of_entry=@date_of_entry " +
                              "WHERE serial=@serial"
                            : "INSERT INTO items_ (serial, description, price, retail_price, price_perpc, new_quantity, qty_inpc, date_of_entry) " +
                              "VALUES (@serial, @description, @price, @retail_price, @price_perpc, @new_quantity, @qty_inpc, @date_of_entry)";

                        using (var cmd = new MySqlCommand(commandText, mysqlConn))
                        {
                            cmd.Parameters.AddWithValue("@serial", serial);
                            cmd.Parameters.AddWithValue("@description", row["description"]);
                            cmd.Parameters.AddWithValue("@price", row["price"]);
                            cmd.Parameters.AddWithValue("@retail_price", row["retail_price"]);
                            cmd.Parameters.AddWithValue("@price_perpc", row["price_perpc"]);
                            cmd.Parameters.AddWithValue("@new_quantity", row["new_quantity"]);
                            cmd.Parameters.AddWithValue("@qty_inpc", row["qty_inpc"]);
                            cmd.Parameters.AddWithValue("@date_of_entry", row["date_of_entry"]);
                            cmd.ExecuteNonQuery();
                        }

                        if (exists) updatedCount++; else insertedCount++;
                    }
                }

                return (updatedCount, insertedCount);
            });

            SetLastPush(DateTime.Now);
            progress?.Report($"Push complete - {result.updated} updated, {result.inserted} inserted.");
            return result;
        }

        private static void ExtractTemplate(string destinationPath)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = asm.GetManifestResourceStream(TemplateResourceName))
            {
                if (resourceStream == null)
                    throw new InvalidOperationException($"Embedded Access template '{TemplateResourceName}' was not found.");

                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    resourceStream.CopyTo(fileStream);
            }
        }
    }
}
