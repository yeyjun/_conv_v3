using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace YourApp.Data
{
    /// <summary>
    /// Singleton cache that holds DataTables loaded from MySQL.
    /// Call LoadAll() once at application startup (before the main form opens).
    /// After that, every form/control pulls from memory via GetTable() -
    /// no repeated database round trips.
    ///
    /// Every table successfully pulled from the database is also written to a local
    /// snapshot file. If a load can't reach the database, the app keeps running on
    /// whatever is already in memory (falling back to the on-disk snapshot only if
    /// nothing has been loaded yet, e.g. a cold start with no connection) so stock
    /// data stays available as a reference while offline.
    /// </summary>
    public sealed class DataCacheManager
    {
        private static readonly Lazy<DataCacheManager> _instance =
            new Lazy<DataCacheManager>(() => new DataCacheManager());

        public static DataCacheManager Instance => _instance.Value;

        private readonly Dictionary<string, DataTable> _cache = new Dictionary<string, DataTable>();

        private readonly Dictionary<string, DateTime> _lastUpdated = new Dictionary<string, DateTime>();

        private readonly string _connectionString =
            $"Server={ConfigurationManager.AppSettings["DB_SERVER"]};" +
            $"Port=3306;Database={ConfigurationManager.AppSettings["DB_NAME"]};" +
            $"User ID={ConfigurationManager.AppSettings["DB_USER"]};" +
            $"Password={ConfigurationManager.AppSettings["DB_PASSWORD"]};" +
            $"SslMode=Required;";

        /// <summary>True if the most recent load came from the offline snapshot file, not the database.</summary>
        public bool IsOfflineMode { get; private set; }

        /// <summary>When the offline snapshot currently in use was last saved from a live database load.</summary>
        public DateTime? OfflineSnapshotAsOf { get; private set; }

        private DataCacheManager() { }

        /// <summary>
        /// Loads every table you need into memory. Call this ONCE at startup.
        /// </summary>
        /// <param name="onStatus">Optional callback invoked with a human-readable status before each table loads (e.g. for a splash screen).</param>
        public void LoadAll(Action<string> onStatus = null)
        {
            onStatus?.Invoke("Connecting to database...");

            onStatus?.Invoke("Loading inventory items...");
            LoadTable("Items", "SELECT * FROM items_");

            onStatus?.Invoke("Loading user accounts...");
            LoadTable("Users", "SELECT name, fullname FROM user_");

            // Add more tables here as your app needs them.
        }

        private void LoadTable(string key, string query)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                using (var adapter = new MySqlDataAdapter(query, conn))
                {
                    var table = new DataTable(key);
                    adapter.Fill(table);
                    _cache[key] = table;
                }

                IsOfflineMode = false;
                OfflineSnapshotAsOf = null;
                _lastUpdated[key] = DateTime.Now;
                TrySaveOfflineSnapshot(key);
            }
            catch (Exception ex)
            {
                // Any failed live load - startup or a later refresh - means we're offline right now.
                IsOfflineMode = true;

                // If we already have a live/previously-loaded copy of this table in memory, keep it
                // as the active reference rather than downgrading it with a possibly-older snapshot
                // file, and don't blow up the caller - offline just means "keep working with what we have".
                if (_cache.ContainsKey(key))
                {
                    MergePendingItemsIfApplicable(key);
                    return;
                }

                if (!TryLoadOfflineSnapshot(key))
                    throw new Exception($"Could not reach the database, and no offline stock snapshot was found on this machine.\n{ex.Message}", ex);
            }

            MergePendingItemsIfApplicable(key);
        }

        private void MergePendingItemsIfApplicable(string key)
        {
            if (key != "Items" || !_cache.TryGetValue(key, out DataTable table))
                return;

            DataTable merged = table.Copy();
            PendingItemStore.MergeInto(merged);
            _cache[key] = merged;
        }

        private static string GetOfflineCacheDirectory()
        {
            string documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsDir, "App Inventory", "OfflineCache");
        }

        private static string GetOfflineCachePath(string key) =>
            Path.Combine(GetOfflineCacheDirectory(), $"{key}.xml");

        private void TrySaveOfflineSnapshot(string key)
        {
            try
            {
                string dir = GetOfflineCacheDirectory();
                Directory.CreateDirectory(dir);
                _cache[key].WriteXml(GetOfflineCachePath(key), XmlWriteMode.WriteSchema);
            }
            catch
            {
                // Best-effort only - failing to save the snapshot should never block a successful DB load.
            }
        }

        private bool TryLoadOfflineSnapshot(string key)
        {
            string path = GetOfflineCachePath(key);
            if (!File.Exists(path))
                return false;

            try
            {
                var table = new DataTable(key);
                table.ReadXml(path);
                _cache[key] = table;
                OfflineSnapshotAsOf = File.GetLastWriteTime(path);
                _lastUpdated[key] = OfflineSnapshotAsOf.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DateTime? GetLastUpdated(string key) =>
            _lastUpdated.TryGetValue(key, out DateTime when) ? when : (DateTime?)null;

        public static bool ItemRowHasIntegrityIssue(DataRow row)
        {
            DataTable table = row.Table;
            bool hasSerial = table.Columns.Contains("serial") && !string.IsNullOrWhiteSpace(row["serial"] as string);
            bool hasPrice = HasPositivePrice(row, table, "price") ||
                            HasPositivePrice(row, table, "retail_price") ||
                            HasPositivePrice(row, table, "price_perpc");
            return !hasSerial || !hasPrice;
        }

        private static bool HasPositivePrice(DataRow row, DataTable table, string column)
        {
            if (!table.Columns.Contains(column) || row[column] == DBNull.Value)
                return false;

            try
            {
                return Convert.ToDecimal(row[column]) > 0;
            }
            catch
            {
                return false;
            }
        }

        public int CountItemIntegrityIssues()
        {
            if (!_cache.TryGetValue("Items", out DataTable items))
                return 0;

            int count = 0;
            foreach (DataRow row in items.Rows)
            {
                if (ItemRowHasIntegrityIssue(row))
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Retrieve a cached table by key. Returns instantly - no DB hit.
        /// </summary>
        public DataTable GetTable(string key)
        {
            if (_cache.TryGetValue(key, out var table))
                return table;

            throw new KeyNotFoundException(
                $"Table '{key}' was not loaded into cache. Did you call LoadAll() at startup?");
        }

        public void ReplaceTable(string key, DataTable table)
        {
            _cache[key] = table;
        }

        /// <summary>
        /// Re-pulls a single table from the database, e.g. after an INSERT/UPDATE
        /// elsewhere in the app. Use sparingly - the whole point is to avoid this.
        /// </summary>
        public void RefreshTable(string key, string query)
        {
            LoadTable(key, query);
        }

        /// <summary>
        /// Clears everything from memory, e.g. on logout.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
