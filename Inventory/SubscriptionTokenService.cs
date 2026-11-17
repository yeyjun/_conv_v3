using MySql.Data.MySqlClient;
using System.Configuration;
using System.Threading.Tasks;

namespace Inventory
{
    /// <summary>
    /// Checks the centralized subscription "tokens" table (a separate MySQL server/database from
    /// this store's own DB_* settings - see TOKEN_DB_* appSettings) for a token row issued to this
    /// store (matched by STORE_NAME against the table's "business" column) within the current
    /// calendar month. Unlike AppConfigService's maintenance check, this deliberately fails
    /// CLOSED - a subscription gate that fails open on a DB error could be bypassed just by
    /// blocking outbound access to the token server, defeating its purpose.
    /// </summary>
    internal static class SubscriptionTokenService
    {
        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["TOKEN_DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["TOKEN_DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["TOKEN_DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["TOKEN_DB_PASSWORD"];
            int timeout = int.TryParse(ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"], out int t) ? t : 15;

            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};Connect Timeout={timeout};";
        }

        /// <summary>
        /// True only if a token row exists for this store's business name with a created_at
        /// falling in the current calendar month (compared using the token server's own clock via
        /// CURDATE(), to avoid a client/server clock mismatch at a month boundary). Any connection
        /// or query failure is treated as "not active" - see class remarks.
        /// </summary>
        public static async Task<bool> IsActiveAsync()
        {
            string business = ConfigurationManager.AppSettings["STORE_NAME"];

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM tokens WHERE business = @business " +
                        "AND YEAR(created_at) = YEAR(CURDATE()) AND MONTH(created_at) = MONTH(CURDATE())", conn))
                    {
                        cmd.Parameters.AddWithValue("@business", business);
                        long count = (long)await cmd.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
