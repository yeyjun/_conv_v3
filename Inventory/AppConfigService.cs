using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Inventory
{
    /// <summary>
    /// Reads/writes app_config_ - a small key/value store that lives in the same MySQL
    /// database as everything else, so a value written here (unlike App.config, which is
    /// per-machine) is immediately visible to every terminal pointed at that database.
    /// Used for system maintenance mode: flip MAINTENANCE_MODE on from any one terminal's
    /// System Config and every terminal - including ones already running - picks it up.
    /// Also used for the low-stock reminder popup (System Config > Low Stock Reminder) for the
    /// same reason: the threshold/interval are store-wide policy, not a per-machine preference.
    ///
    /// NOTE: this used to point at a table named setting_. On real production databases that
    /// name was already taken by an unrelated, differently-shaped table (id/description/
    /// switch/switch2/switch3 - not a key/value store at all), so every read/write here was
    /// silently failing: writes threw "Unknown column 'config_key'" (caught by the Save
    /// button's own try/catch and shown to the admin), and reads failed the same way but hit
    /// the fail-open catch block below, so they silently always reported "off" instead of
    /// erroring. Renamed to app_config_ - a name that doesn't collide with anything - and
    /// EnsureTableAsync below creates it on first use instead of relying on an admin re-running
    /// System Config > Database Setup on a store that's already up and running.
    /// </summary>
    internal static class AppConfigService
    {
        private const string TableName = "app_config_";

        private const string MaintenanceEnabledKey = "MAINTENANCE_MODE";
        private const string MaintenanceMessageKey = "MAINTENANCE_MESSAGE";
        private const string LowStockEnabledKey = "LOW_STOCK_REMINDER_ENABLED";
        private const string LowStockThresholdKey = "LOW_STOCK_THRESHOLD";
        private const string LowStockIntervalKey = "LOW_STOCK_REMINDER_INTERVAL_MINUTES";
        private const string UpdateCheckEnabledKey = "UPDATE_CHECK_ENABLED";
        private const string AutoUpdateEnabledKey = "AUTO_UPDATE_ENABLED";

        private const int DefaultLowStockThreshold = 10;
        private const int DefaultLowStockIntervalMinutes = 30;

        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            int timeout = int.TryParse(ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"], out int t) ? t : 15;

            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};Connect Timeout={timeout};";
        }

        /// <summary>
        /// Fails open (maintenance off, no message) on any connection error - a DB hiccup should
        /// never lock the store out of selling, matching this app's offline-resilience convention
        /// everywhere else. Callers cannot distinguish "off" from "couldn't check", which is the
        /// intended trade-off here.
        /// </summary>
        public static async Task<(bool Enabled, string Message)> GetMaintenanceStatusAsync()
        {
            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    await EnsureTableAsync(conn);
                    using (var cmd = new MySqlCommand(
                        $"SELECT config_key, config_value FROM {TableName} WHERE config_key IN (@k1, @k2)", conn))
                    {
                        cmd.Parameters.AddWithValue("@k1", MaintenanceEnabledKey);
                        cmd.Parameters.AddWithValue("@k2", MaintenanceMessageKey);

                        bool enabled = false;
                        string message = null;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string key = reader.GetString(0);
                                string value = reader.IsDBNull(1) ? "" : reader.GetString(1);

                                if (string.Equals(key, MaintenanceEnabledKey, StringComparison.OrdinalIgnoreCase))
                                    enabled = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";
                                else if (string.Equals(key, MaintenanceMessageKey, StringComparison.OrdinalIgnoreCase))
                                    message = value;
                            }
                        }

                        return (enabled, message);
                    }
                }
            }
            catch
            {
                return (false, null);
            }
        }

        /// <summary>
        /// Unlike GetMaintenanceStatusAsync, this does not swallow failures - it is only ever
        /// called from System Config's Save button, which already has its own try/catch to show
        /// the error to the admin who's setting the flag.
        /// </summary>
        public static async Task SetMaintenanceModeAsync(bool enabled, string message)
        {
            using (var conn = new MySqlConnection(BuildConnectionString()))
            {
                await conn.OpenAsync();
                await EnsureTableAsync(conn);
                await UpsertAsync(conn, MaintenanceEnabledKey, enabled ? "true" : "false");
                await UpsertAsync(conn, MaintenanceMessageKey, message ?? "");
            }
        }

        /// <summary>
        /// Fails open (reminder off) on any connection error, same reasoning as
        /// GetMaintenanceStatusAsync - a DB hiccup should never pop up a dialog nobody asked for.
        /// </summary>
        public static async Task<(bool Enabled, int Threshold, int IntervalMinutes)> GetLowStockReminderSettingsAsync()
        {
            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    await EnsureTableAsync(conn);
                    using (var cmd = new MySqlCommand(
                        $"SELECT config_key, config_value FROM {TableName} WHERE config_key IN (@k1, @k2, @k3)", conn))
                    {
                        cmd.Parameters.AddWithValue("@k1", LowStockEnabledKey);
                        cmd.Parameters.AddWithValue("@k2", LowStockThresholdKey);
                        cmd.Parameters.AddWithValue("@k3", LowStockIntervalKey);

                        bool enabled = false;
                        int threshold = DefaultLowStockThreshold;
                        int intervalMinutes = DefaultLowStockIntervalMinutes;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string key = reader.GetString(0);
                                string value = reader.IsDBNull(1) ? "" : reader.GetString(1);

                                if (string.Equals(key, LowStockEnabledKey, StringComparison.OrdinalIgnoreCase))
                                    enabled = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";
                                else if (string.Equals(key, LowStockThresholdKey, StringComparison.OrdinalIgnoreCase))
                                    threshold = int.TryParse(value, out int t) ? t : DefaultLowStockThreshold;
                                else if (string.Equals(key, LowStockIntervalKey, StringComparison.OrdinalIgnoreCase))
                                    intervalMinutes = int.TryParse(value, out int m) && m > 0 ? m : DefaultLowStockIntervalMinutes;
                            }
                        }

                        return (enabled, threshold, intervalMinutes);
                    }
                }
            }
            catch
            {
                return (false, DefaultLowStockThreshold, DefaultLowStockIntervalMinutes);
            }
        }

        /// <summary>
        /// Unlike GetLowStockReminderSettingsAsync, this does not swallow failures - it is only
        /// ever called from System Config's Save button, which already has its own try/catch to
        /// show the error to the admin who's setting it.
        /// </summary>
        public static async Task SetLowStockReminderSettingsAsync(bool enabled, int threshold, int intervalMinutes)
        {
            using (var conn = new MySqlConnection(BuildConnectionString()))
            {
                await conn.OpenAsync();
                await EnsureTableAsync(conn);
                await UpsertAsync(conn, LowStockEnabledKey, enabled ? "true" : "false");
                await UpsertAsync(conn, LowStockThresholdKey, threshold.ToString());
                await UpsertAsync(conn, LowStockIntervalKey, intervalMinutes.ToString());
            }
        }

        /// <summary>
        /// Fails open (checks off) on any connection error, same reasoning as
        /// GetMaintenanceStatusAsync - a DB hiccup should never be what decides whether the
        /// update checker runs. Also defaults to off (opt-in) the first time this is read on a
        /// store that's never visited System Config > Updates, matching Maintenance/Low Stock's
        /// existing "new feature starts disabled" convention.
        /// </summary>
        public static async Task<bool> GetUpdateCheckEnabledAsync()
        {
            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    await EnsureTableAsync(conn);
                    using (var cmd = new MySqlCommand(
                        $"SELECT config_value FROM {TableName} WHERE config_key = @k", conn))
                    {
                        cmd.Parameters.AddWithValue("@k", UpdateCheckEnabledKey);
                        object result = await cmd.ExecuteScalarAsync();
                        string value = result as string;
                        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Unlike GetUpdateCheckEnabledAsync, this does not swallow failures - it is only ever
        /// called from System Config's Save button, which already has its own try/catch to show
        /// the error to the admin who's setting it.
        /// </summary>
        public static async Task SetUpdateCheckEnabledAsync(bool enabled)
        {
            using (var conn = new MySqlConnection(BuildConnectionString()))
            {
                await conn.OpenAsync();
                await EnsureTableAsync(conn);
                await UpsertAsync(conn, UpdateCheckEnabledKey, enabled ? "true" : "false");
            }
        }

        /// <summary>
        /// Fails open (off) on any connection error, same reasoning as GetUpdateCheckEnabledAsync.
        /// Deliberately a separate key from UPDATE_CHECK_ENABLED, not a sub-option of it: a store
        /// that already opted into notify-only update checks should not be silently upgraded to
        /// unattended downloads/installs just because this code shipped - that's a materially
        /// bigger risk level and needs its own explicit opt-in, defaulting to off like every other
        /// new feature flag here (System Config > Updates).
        /// </summary>
        public static async Task<bool> GetAutoUpdateEnabledAsync()
        {
            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    await EnsureTableAsync(conn);
                    using (var cmd = new MySqlCommand(
                        $"SELECT config_value FROM {TableName} WHERE config_key = @k", conn))
                    {
                        cmd.Parameters.AddWithValue("@k", AutoUpdateEnabledKey);
                        object result = await cmd.ExecuteScalarAsync();
                        string value = result as string;
                        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Unlike GetAutoUpdateEnabledAsync, this does not swallow failures - it is only ever
        /// called from System Config's Save button, which already has its own try/catch to show
        /// the error to the admin who's setting it.
        /// </summary>
        public static async Task SetAutoUpdateEnabledAsync(bool enabled)
        {
            using (var conn = new MySqlConnection(BuildConnectionString()))
            {
                await conn.OpenAsync();
                await EnsureTableAsync(conn);
                await UpsertAsync(conn, AutoUpdateEnabledKey, enabled ? "true" : "false");
            }
        }

        /// <summary>
        /// Creates app_config_ if it doesn't exist yet - cheap/no-op once it does (a single
        /// round trip), so this runs unconditionally at the top of every public method here
        /// rather than depending on an admin having re-run System Config > Database Setup on
        /// a store that's already live.
        /// </summary>
        private static async Task EnsureTableAsync(MySqlConnection conn)
        {
            using (var cmd = new MySqlCommand(
                $"CREATE TABLE IF NOT EXISTS {TableName} (" +
                "config_key VARCHAR(100) PRIMARY KEY, " +
                "config_value VARCHAR(255) NOT NULL DEFAULT '', " +
                "updated_utc DATETIME NULL)", conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static async Task UpsertAsync(MySqlConnection conn, string key, string value)
        {
            using (var cmd = new MySqlCommand(
                $"INSERT INTO {TableName} (config_key, config_value, updated_utc) VALUES (@k, @v, UTC_TIMESTAMP()) " +
                "ON DUPLICATE KEY UPDATE config_value = VALUES(config_value), updated_utc = VALUES(updated_utc)", conn))
            {
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@v", value);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
