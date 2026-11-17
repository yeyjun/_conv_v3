using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace _conv
{
    /// <summary>
    /// Records app activity (e.g. logins) to the logs_ table when the database is reachable.
    /// When it isn't, the event is appended to a local file instead so nothing is lost -
    /// whoever manages the database can replay PendingActivityLogs.log once back online.
    /// Offline entries are grouped into OfflineCache\yyyy-MM\ subfolders by month.
    /// </summary>
    internal static class ActivityLog
    {
        public static async Task<bool> LogAsync(string eventText)
        {
            try
            {
                string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
                string dbname = ConfigurationManager.AppSettings["DB_NAME"];
                string dbuser = ConfigurationManager.AppSettings["DB_USER"];
                string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];

                using (var conn = new MySqlConnection($"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};"))
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand("INSERT INTO logs_(event_log) VALUES (@event)", conn))
                    {
                        cmd.Parameters.AddWithValue("@event", eventText);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                LogOffline(eventText);
                return false;
            }
        }

        private static void LogOffline(string eventText)
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "App Inventory", "OfflineCache", DateTime.Now.ToString("yyyy-MM"));
                Directory.CreateDirectory(dir);
                string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {eventText}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(dir, "PendingActivityLogs.log"), line);
            }
            catch
            {
                // Best-effort only - a failed log write must never block whatever triggered it.
            }
        }
    }
}
