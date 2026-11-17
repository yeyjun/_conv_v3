using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace _conv
{
    /// <summary>
    /// Reads/writes the shared setting_ table - a small key/value store that lives in the
    /// same MySQL database as everything else, so a value written here (unlike App.config,
    /// which is per-machine) is immediately visible to every terminal pointed at that database.
    /// Used for system maintenance mode: flip MAINTENANCE_MODE on from any one terminal's
    /// System Config and every terminal - including ones already running - picks it up.
    /// </summary>
    internal static class AppConfigService
    {
        private const string MaintenanceEnabledKey = "MAINTENANCE_MODE";
        private const string MaintenanceMessageKey = "MAINTENANCE_MESSAGE";

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
                    using (var cmd = new MySqlCommand(
                        "SELECT config_key, config_value FROM setting_ WHERE config_key IN (@k1, @k2)", conn))
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
                await UpsertAsync(conn, MaintenanceEnabledKey, enabled ? "true" : "false");
                await UpsertAsync(conn, MaintenanceMessageKey, message ?? "");
            }
        }

        private static async Task UpsertAsync(MySqlConnection conn, string key, string value)
        {
            using (var cmd = new MySqlCommand(
                "INSERT INTO setting_ (config_key, config_value, updated_utc) VALUES (@k, @v, UTC_TIMESTAMP()) " +
                "ON DUPLICATE KEY UPDATE config_value = VALUES(config_value), updated_utc = VALUES(updated_utc)", conn))
            {
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@v", value);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
