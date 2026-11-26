using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace YourApp.Data
{
    /// <summary>
    /// Background color for the item search grid (Purchases.dgv_itemSearch) only, configured on
    /// the Item Search tab in System Configuration (ITEM_SEARCH_BACKGROUND_COLOR, stored as
    /// "#RRGGBB"). Same approach as <see cref="CartBoardTheme"/> for the cart board grid - see
    /// that type for why "transparent" means synced to the grid's own container (gbx_itemsearch)
    /// rather than a real Color.Transparent, which DataGridView.BackgroundColor rejects. Kept as
    /// its own small type rather than a shared one since each grid has its own container to sync
    /// to and its own appSetting key; the two are independent and the duplication is cheap.
    /// </summary>
    public static class ItemSearchGridTheme
    {
        public static Color? CustomColor { get; private set; }
        public static bool IsConfigured => CustomColor.HasValue;

        static ItemSearchGridTheme()
        {
            Reload();
        }

        /// <summary>Re-reads ITEM_SEARCH_BACKGROUND_COLOR from appSettings. Call after System
        /// Configuration saves a new value so forms opened later in the same session pick it up.</summary>
        public static void Reload()
        {
            string hex = ConfigurationManager.AppSettings["ITEM_SEARCH_BACKGROUND_COLOR"];
            CustomColor = string.IsNullOrWhiteSpace(hex) ? (Color?)null : ParseOrNull(hex);
        }

        /// <summary>Sets <paramref name="searchGrid"/>'s BackgroundColor to the configured custom
        /// color, or to its own container's current BackColor when unconfigured (transparent).</summary>
        public static void ApplyTo(DataGridView searchGrid)
        {
            if (searchGrid == null)
                return;

            Color target = CustomColor ?? searchGrid.Parent?.BackColor ?? searchGrid.BackgroundColor;
            if (searchGrid.BackgroundColor != target)
                searchGrid.BackgroundColor = target;
        }

        private static Color? ParseOrNull(string html)
        {
            try { return ColorTranslator.FromHtml(html); }
            catch { return null; }
        }
    }
}
