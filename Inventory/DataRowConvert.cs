using System;

namespace Inventory
{
    /// <summary>
    /// Null/DBNull-safe numeric conversions for DataRow values pulled from the cached Items
    /// table - consolidates what used to be near-identical copies in Form1.cs,
    /// InventoryReport.cs, and ItemQuickSearchForm.cs. InventoryReport.cs's old ToLong used
    /// long.TryParse(value.ToString()), which returns 0 for a decimal-formatted string like
    /// "12.0"; this version (matching what Form1.cs already did) uses Convert.ToInt64 in a
    /// try/catch, so it also handles that case.
    /// </summary>
    internal static class DataRowConvert
    {
        public static int ToInt(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            try { return Convert.ToInt32(value); } catch { return 0; }
        }

        public static long ToLong(object value)
        {
            if (value == null || value == DBNull.Value) return 0L;
            try { return Convert.ToInt64(value); } catch { return 0L; }
        }

        public static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;
            try { return Convert.ToDecimal(value); } catch { return 0m; }
        }
    }
}
