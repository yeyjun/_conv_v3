using System.Data.Common;
using MySql.Data.MySqlClient;

namespace ItemMonitor.Services
{
    public enum StockStatus
    {
        Normal,
        Low,
        OutOfStock,
    }

    public sealed class ItemStockRow
    {
        public string Serial { get; set; } = "";
        public string Description { get; set; } = "";
        public int Quantity { get; set; }
        public decimal RetailPrice { get; set; }
        public DateTime? DateOfEntry { get; set; }
        public StockStatus Status { get; set; }
    }

    // What the table asks for: free-text search (serial or description), an optional status
    // filter, a sort column/direction, and a page. Shared between the initial server-rendered
    // page and the JSON polling endpoint so both go through the same query-building code.
    public sealed class ItemsQuery
    {
        public string? Search { get; set; }
        public StockStatus? Status { get; set; }
        public string SortBy { get; set; } = "quantity";
        public bool SortDescending { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public sealed class ItemsPage
    {
        public List<ItemStockRow> Items { get; } = new();
        public int MatchingCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int SkippedRowCount { get; set; }
        public string? Error { get; set; }

        public int PageCount => PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling(MatchingCount / (double)PageSize));
    }

    public sealed class StockCounts
    {
        public int Total { get; set; }
        public int Low { get; set; }
        public int OutOfStock { get; set; }
        public int Threshold { get; set; }
        public DateTime CheckedAtUtc { get; set; }
        public string? Error { get; set; }

        public int Normal => Total - Low - OutOfStock;
    }

    // Read-only against the same items_ table Inventory's DataCacheManager caches at startup
    // (see CLAUDE.md) - this app never writes to it. There's no reorder-level column on
    // items_, so "low stock" is a single configurable threshold (LowStockThreshold in
    // appsettings.json) applied to every item rather than a per-item value. Both status and
    // its severity ordering are computed in SQL via a derived table (see BuildFilteredSql) so
    // search/filter/sort/paging all happen server-side in one query instead of pulling the
    // whole items_ table into memory - this store's catalog runs into the thousands of rows.
    // Connection string is built from config the same way the WinForms apps build theirs from
    // App.config appSettings - yet another place the production DB credentials now live (see
    // CLAUDE.md's standing TODO about that).
    public sealed class ItemStockService
    {
        // Maps a client-facing sort key to a whitelisted SQL expression - never interpolate
        // the client's sortBy/sortDir directly into the query (ORDER BY can't be parameterized).
        private static readonly Dictionary<string, string> SortColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            ["serial"] = "serial",
            ["description"] = "description",
            ["quantity"] = "quantity",
            ["retailprice"] = "retail_price",
            ["status"] = "status_priority",
        };

        private readonly IConfiguration config;

        public ItemStockService(IConfiguration config)
        {
            this.config = config;
        }

        public int GetThreshold() => config.GetValue<int?>("LowStockThreshold") ?? 10;

        public async Task<StockCounts> GetCountsAsync()
        {
            int threshold = GetThreshold();
            var counts = new StockCounts { Threshold = threshold, CheckedAtUtc = DateTime.UtcNow };

            try
            {
                using var conn = new MySqlConnection(BuildConnectionString());
                await conn.OpenAsync();

                using var cmd = new MySqlCommand(
                    "SELECT COUNT(*) AS total, " +
                    "SUM(CASE WHEN new_quantity <= 0 THEN 1 ELSE 0 END) AS out_of_stock, " +
                    "SUM(CASE WHEN new_quantity > 0 AND new_quantity <= @threshold THEN 1 ELSE 0 END) AS low " +
                    "FROM items_", conn);
                cmd.Parameters.AddWithValue("@threshold", threshold);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    counts.Total = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("total")));
                    counts.OutOfStock = ReadNullableCount(reader, "out_of_stock");
                    counts.Low = ReadNullableCount(reader, "low");
                }
            }
            catch (Exception ex)
            {
                counts.Error = ex.Message;
            }

            return counts;
        }

        public async Task<ItemsPage> GetPageAsync(ItemsQuery query)
        {
            int threshold = GetThreshold();
            int page = Math.Max(1, query.Page);
            int pageSize = Math.Clamp(query.PageSize <= 0 ? 50 : query.PageSize, 1, 200);
            string sortColumn = SortColumns.TryGetValue(query.SortBy, out var col) ? col : "quantity";
            string sortDir = query.SortDescending ? "DESC" : "ASC";
            string search = query.Search?.Trim() ?? "";
            string status = query.Status?.ToString() ?? "";

            var result = new ItemsPage { Page = page, PageSize = pageSize };

            const string derived =
                "(SELECT serial, description, new_quantity AS quantity, retail_price, date_of_entry, " +
                "CASE WHEN new_quantity <= 0 THEN 'OutOfStock' " +
                "     WHEN new_quantity <= @threshold THEN 'Low' " +
                "     ELSE 'Normal' END AS status, " +
                "CASE WHEN new_quantity <= 0 THEN 0 " +
                "     WHEN new_quantity <= @threshold THEN 1 " +
                "     ELSE 2 END AS status_priority " +
                "FROM items_) AS t";
            const string whereClause =
                "WHERE (@search = '' OR serial LIKE @searchLike OR description LIKE @searchLike) " +
                "AND (@status = '' OR status = @status)";

            try
            {
                using var conn = new MySqlConnection(BuildConnectionString());
                await conn.OpenAsync();

                using (var countCmd = new MySqlCommand($"SELECT COUNT(*) FROM {derived} {whereClause}", conn))
                {
                    AddFilterParams(countCmd, threshold, search, status);
                    result.MatchingCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                }

                string dataSql =
                    $"SELECT serial, description, quantity, retail_price, date_of_entry, status " +
                    $"FROM {derived} {whereClause} " +
                    $"ORDER BY {sortColumn} {sortDir}, serial ASC " +
                    "LIMIT @pageSize OFFSET @offset";

                using var dataCmd = new MySqlCommand(dataSql, conn);
                AddFilterParams(dataCmd, threshold, search, status);
                dataCmd.Parameters.AddWithValue("@pageSize", pageSize);
                dataCmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

                using var reader = await dataCmd.ExecuteReaderAsync();
                int ordSerial = reader.GetOrdinal("serial");
                int ordDescription = reader.GetOrdinal("description");
                int ordQuantity = reader.GetOrdinal("quantity");
                int ordRetailPrice = reader.GetOrdinal("retail_price");
                int ordDateOfEntry = reader.GetOrdinal("date_of_entry");
                int ordStatus = reader.GetOrdinal("status");

                // Real production rows don't always match the reverse-engineered DDL cleanly
                // (see TryReadDate below) - a malformed value in any column of one row
                // shouldn't blank out the whole page, so bad rows are skipped and counted
                // rather than aborting the read.
                while (await reader.ReadAsync())
                {
                    try
                    {
                        result.Items.Add(new ItemStockRow
                        {
                            Serial = reader.IsDBNull(ordSerial) ? "" : reader.GetString(ordSerial),
                            Description = reader.IsDBNull(ordDescription) ? "" : reader.GetString(ordDescription),
                            Quantity = reader.IsDBNull(ordQuantity) ? 0 : reader.GetInt32(ordQuantity),
                            RetailPrice = reader.IsDBNull(ordRetailPrice) ? 0 : reader.GetDecimal(ordRetailPrice),
                            DateOfEntry = TryReadDate(reader, ordDateOfEntry),
                            Status = Enum.Parse<StockStatus>(reader.GetString(ordStatus)),
                        });
                    }
                    catch (Exception)
                    {
                        result.SkippedRowCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        private static void AddFilterParams(MySqlCommand cmd, int threshold, string search, string status)
        {
            cmd.Parameters.AddWithValue("@threshold", threshold);
            cmd.Parameters.AddWithValue("@search", search);
            cmd.Parameters.AddWithValue("@searchLike", $"%{search}%");
            cmd.Parameters.AddWithValue("@status", status);
        }

        private static int ReadNullableCount(DbDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        // date_of_entry is DATETIME per DbSetupService's DDL, but that DDL was reverse-engineered
        // from app code rather than the live schema (see its own comment) - some rows on the
        // real production table don't parse as a clean date (observed: "Feb ..." rejected by
        // MySql.Data's GetDateTime). It's display-only here, so one bad row shouldn't blank out
        // the whole page - fall back to null and keep going.
        private static DateTime? TryReadDate(DbDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
                return null;
            try
            {
                return reader.GetDateTime(ordinal);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string BuildConnectionString()
        {
            string server = config["DB_SERVER"] ?? "";
            string database = config["DB_NAME"] ?? "";
            string user = config["DB_USER"] ?? "";
            string password = config["DB_PASSWORD"] ?? "";
            string timeout = config["CONNECTION_TIMEOUT"] ?? "30";
            return $"Server={server};Database={database};User ID={user};Password={password};Connection Timeout={timeout};";
        }
    }
}
