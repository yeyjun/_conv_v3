using System.Configuration;

namespace Inventory
{
    /// <summary>
    /// Builds the MySQL connection string from the same 4 App.config appSettings
    /// (DB_SERVER/DB_NAME/DB_USER/DB_PASSWORD) that ListSales, LoyaltyForm, ManageUsersForm,
    /// PettyCashEntryForm, PettyCashHistoryForm, SalesChartForm, and TellerHistoryForm each used
    /// to build independently via a byte-for-byte identical private method.
    /// </summary>
    internal static class DbConnectionHelper
    {
        public static string BuildConnectionString(int? connectTimeoutSeconds = null)
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];

            string connStr = $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";
            return connectTimeoutSeconds.HasValue
                ? connStr + $"Connect Timeout={connectTimeoutSeconds.Value};"
                : connStr;
        }
    }
}
