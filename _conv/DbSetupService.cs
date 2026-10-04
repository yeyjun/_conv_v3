using MySql.Data.MySqlClient;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _conv
{
    internal sealed class DbSetupResult
    {
        public bool DatabaseAlreadyExisted;
        public int TablesCreated;
        public int TablesAlreadyExisted;
    }

    /// <summary>
    /// Stands up a brand-new MySQL database for a fresh store install: creates the database
    /// itself (if it doesn't exist) and every table this app reads/writes, using System Config >
    /// Database Setup. CREATE TABLE IF NOT EXISTS makes each table creation a no-op against a
    /// database that's already (partially) set up, so this is safe to run more than once and
    /// never touches an existing table's rows or definition either way.
    ///
    /// The app has never defined its own schema anywhere else - it's always been pointed at a
    /// pre-existing MySQL database. The DDL for customers_/loyalty_transactions_ below mirrors
    /// LoyaltyForm.cs verbatim, since that form already self-creates them the same way on load;
    /// everything else was reverse-engineered from every SELECT/INSERT/UPDATE the rest of the
    /// app runs against these tables, so it matches what the code actually reads and writes.
    /// </summary>
    internal static class DbSetupService
    {
        private static readonly (string Name, string Ddl)[] TableDefinitions =
        {
            ("items_",
                "CREATE TABLE IF NOT EXISTS items_ (" +
                "serial VARCHAR(191) PRIMARY KEY, " +
                "description VARCHAR(255) NOT NULL, " +
                "price DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                "retail_price DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                "price_perpc DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                "new_quantity INT NOT NULL DEFAULT 0, " +
                "qty_inpc INT NOT NULL DEFAULT 0, " +
                "date_of_entry DATETIME NOT NULL)"),

            // sales_token is a legacy free-form TEXT column that used to hold the last-login
            // timestamp as a string; last activity is now tracked in last_seen_utc (a proper
            // DATETIME, written in UTC - see Login.cs) instead. sales_token is kept around
            // un-dropped so it doesn't break older installs that still reference it.
            ("user_",
                "CREATE TABLE IF NOT EXISTS user_ (" +
                "name VARCHAR(100) PRIMARY KEY, " +
                "fullname VARCHAR(150) NOT NULL, " +
                "sales_token VARCHAR(50) NOT NULL DEFAULT '', " +
                "last_seen_utc DATETIME NULL)"),

            // transaction_date is stored as a formatted string ("yyyy-MMMM-dd H:m:ss"), not a
            // DATETIME - the rest of the app parses/LIKE-filters it as text everywhere it's used.
            ("sales_",
                "CREATE TABLE IF NOT EXISTS sales_ (" +
                "id INT AUTO_INCREMENT PRIMARY KEY, " +
                "transaction_code VARCHAR(60) NOT NULL, " +
                "itemcode VARCHAR(191) NOT NULL, " +
                "transaction_date VARCHAR(30) NOT NULL, " +
                "seller VARCHAR(100), " +
                "item_price DECIMAL(10,2) NOT NULL DEFAULT 0)"),

            ("customers_",
                "CREATE TABLE IF NOT EXISTS customers_ (" +
                "loyalty_code VARCHAR(30) PRIMARY KEY, " +
                "full_name VARCHAR(150) NOT NULL, " +
                "contact_number VARCHAR(30), " +
                "points INT NOT NULL DEFAULT 0, " +
                "date_registered DATETIME NOT NULL)"),

            ("loyalty_transactions_",
                "CREATE TABLE IF NOT EXISTS loyalty_transactions_ (" +
                "id INT AUTO_INCREMENT PRIMARY KEY, " +
                "loyalty_code VARCHAR(30) NOT NULL, " +
                "amount_spent DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                "points_earned INT NOT NULL, " +
                "teller VARCHAR(100), " +
                "date_credited DATETIME NOT NULL)"),

            // cash is whole pesos (int), not decimal - matches the existing int(11) column
            // PettyCashEntryForm.cs already assumes.
            ("pettycash_",
                "CREATE TABLE IF NOT EXISTS pettycash_ (" +
                "id INT AUTO_INCREMENT PRIMARY KEY, " +
                "cash INT NOT NULL, " +
                "date_now VARCHAR(20) NOT NULL)"),

            // log_date is never set by app code (ActivityLog.cs only ever writes event_log), so
            // it self-populates via DEFAULT rather than relying on the app to supply it.
            ("logs_",
                "CREATE TABLE IF NOT EXISTS logs_ (" +
                "id INT AUTO_INCREMENT PRIMARY KEY, " +
                "event_log TEXT NOT NULL, " +
                "log_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP)"),

            // Shared key/value store read live (not through DataCacheManager's startup-only
            // snapshot) so a flag flipped here - e.g. by AppConfigService's maintenance-mode
            // keys - takes effect on every terminal pointed at this database without a restart.
            ("setting_",
                "CREATE TABLE IF NOT EXISTS setting_ (" +
                "config_key VARCHAR(100) PRIMARY KEY, " +
                "config_value VARCHAR(255) NOT NULL DEFAULT '', " +
                "updated_utc DATETIME NULL)"),
        };

        /// <summary>
        /// Creates <paramref name="databaseName"/> on the given server if it doesn't already
        /// exist, then creates every table above inside it. Throws on any connection/permission
        /// failure - the caller is expected to surface that message directly, same as every
        /// other DB-touching action in System Config.
        /// </summary>
        public static async Task<DbSetupResult> CreateDatabaseAsync(
            string server, string user, string password, int timeoutSeconds, string databaseName,
            IProgress<string> progress = null)
        {
            if (!Regex.IsMatch(databaseName ?? "", @"^[A-Za-z0-9_]+$"))
                throw new ArgumentException("Database name can only contain letters, numbers, and underscores.");

            string serverConnStr = $"Server={server};user id={user};password={password};Connect Timeout={timeoutSeconds};";
            string dbConnStr = $"Server={server};user id={user};password={password};database={databaseName};Connect Timeout={timeoutSeconds};";

            var result = new DbSetupResult();

            await Task.Run(() =>
            {
                using (var conn = new MySqlConnection(serverConnStr))
                {
                    conn.Open();
                    result.DatabaseAlreadyExisted = DatabaseExists(conn, databaseName);

                    progress?.Report($"Creating database \"{databaseName}\"...");
                    using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4", conn))
                        cmd.ExecuteNonQuery();
                }

                using (var conn = new MySqlConnection(dbConnStr))
                {
                    conn.Open();

                    foreach ((string name, string ddl) in TableDefinitions)
                    {
                        bool existed = TableExists(conn, databaseName, name);

                        progress?.Report($"Creating table {name}...");
                        using (var cmd = new MySqlCommand(ddl, conn))
                            cmd.ExecuteNonQuery();

                        if (existed)
                            result.TablesAlreadyExisted++;
                        else
                            result.TablesCreated++;
                    }

                    // CREATE TABLE IF NOT EXISTS above is a no-op against a user_ table that
                    // already existed before last_seen_utc was introduced, so add it here too.
                    if (!ColumnExists(conn, databaseName, "user_", "last_seen_utc"))
                    {
                        progress?.Report("Adding last_seen_utc to user_...");
                        using (var cmd = new MySqlCommand("ALTER TABLE user_ ADD COLUMN last_seen_utc DATETIME NULL", conn))
                            cmd.ExecuteNonQuery();
                    }
                }
            });

            progress?.Report("Done.");
            return result;
        }

        private static bool DatabaseExists(MySqlConnection conn, string databaseName)
        {
            using (var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = @name", conn))
            {
                cmd.Parameters.AddWithValue("@name", databaseName);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static bool TableExists(MySqlConnection conn, string databaseName, string tableName)
        {
            using (var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = @db AND table_name = @table", conn))
            {
                cmd.Parameters.AddWithValue("@db", databaseName);
                cmd.Parameters.AddWithValue("@table", tableName);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static bool ColumnExists(MySqlConnection conn, string databaseName, string tableName, string columnName)
        {
            using (var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = @db AND table_name = @table AND column_name = @column", conn))
            {
                cmd.Parameters.AddWithValue("@db", databaseName);
                cmd.Parameters.AddWithValue("@table", tableName);
                cmd.Parameters.AddWithValue("@column", columnName);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
