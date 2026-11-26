using System;
using System.Configuration;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using YourApp.Data;

namespace Inventory
{
    /// <summary>
    /// Pushes items queued offline by PendingItemStore (created while the database was
    /// unreachable, via AddItemForm's offline fallback) into the real items_ table once the
    /// database is reachable again. Without this, DataCacheManager.MergePendingItemsIfApplicable
    /// kept showing those items in the in-memory cache after reconnecting - every screen looked
    /// fine - but the real database never actually received them, so the "cache" and "database"
    /// silently diverged forever (a second terminal, or this one after a restart with a fresh
    /// cache load, would never see them). Configurable (PENDING_ITEM_SYNC_ENABLED, System Config
    /// > Offline Sync) since an admin may prefer to keep reconciling these by hand instead.
    /// </summary>
    internal static class PendingItemSyncService
    {
        public static bool AutoSyncEnabled =>
            !string.Equals(ConfigurationManager.AppSettings["PENDING_ITEM_SYNC_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Inserts every still-pending item into items_ and removes it from the pending queue on
        /// success. A serial that already exists in the database (MySQL error 1062, duplicate key
        /// - some other terminal created the same barcode in the meantime) is left in the pending
        /// queue rather than overwritten, so that conflict still gets a human's attention instead
        /// of silently clobbering whatever that other terminal wrote. Returns how many synced.
        /// </summary>
        public static async Task<int> SyncPendingItemsAsync()
        {
            var pending = PendingItemStore.GetAll();
            if (pending.Count == 0)
                return 0;

            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            string connectionTimeout = ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"];
            string connectionString =
                $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};Connect Timeout={connectionTimeout};";

            int synced = 0;

            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();

                foreach (PendingItemStore.Record item in pending)
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(
                            "INSERT INTO items_(serial, description, category, price, retail_price, price_perpc, new_quantity, qty_inpc, date_of_entry) " +
                            "VALUES (@serial, @description, @category, @price, @retail_price, @price_perpc, @quantity, @quantity, @date_of_entry)", conn))
                        {
                            cmd.Parameters.AddWithValue("@serial", item.Serial);
                            cmd.Parameters.AddWithValue("@description", item.Description);
                            cmd.Parameters.AddWithValue("@category", item.Category);
                            cmd.Parameters.AddWithValue("@price", item.Price);
                            cmd.Parameters.AddWithValue("@retail_price", item.RetailPrice);
                            cmd.Parameters.AddWithValue("@price_perpc", item.PricePerPc);
                            cmd.Parameters.AddWithValue("@quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@date_of_entry", item.DateOfEntry);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        PendingItemStore.Remove(item.Serial);
                        synced++;
                        OperationLog.RecordCreate(nameof(PendingItemSyncService),
                            $"Item '{item.Serial}' ({item.Description}) synced from the offline queue to the database.");
                    }
                    catch (MySqlException ex) when (ex.Number == 1062)
                    {
                        // Duplicate serial - already exists in the database (most likely created
                        // by another terminal while this one was offline). Leave it pending so a
                        // human reconciles it rather than silently overwriting that other write.
                    }
                }
            }

            if (synced > 0)
                await ActivityLog.LogAsync($"{synced} offline-pending item(s) synced to the database.");

            return synced;
        }
    }
}
