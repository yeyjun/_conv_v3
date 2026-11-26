using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace YourApp.Data
{
    /// <summary>
    /// Background color for the cart board grid (Purchases.dgv_items) only, configured on the
    /// Appearance tab in System Configuration (CART_BOARD_BACKGROUND_COLOR, stored as "#RRGGBB").
    /// DataGridView.BackgroundColor rejects any color with alpha &lt; 255, so real Color.Transparent
    /// isn't possible here - "transparent" instead means synced live to the grid's own container
    /// (gbx_itemList), the same visual-blend trick picCartLogo already uses for the fully-empty-cart
    /// overlay, rather than left at the DataGridView's own gray default. Unset (the default) means
    /// transparent/synced; a saved hex value pins it to a fixed color instead.
    /// Deliberately scoped to the cart board specifically, not DataGridView generally - AppTheme
    /// already excludes every grid from its own recoloring since that tends to make cell data
    /// unreadable, and the other grids (item search, manage prices, dashboard low stock) don't have
    /// the cart board's mostly-empty-canvas look that makes a mismatched background noticeable.
    /// </summary>
    public static class CartBoardTheme
    {
        public static Color? CustomColor { get; private set; }
        public static bool IsConfigured => CustomColor.HasValue;

        static CartBoardTheme()
        {
            Reload();
        }

        /// <summary>Re-reads CART_BOARD_BACKGROUND_COLOR from appSettings. Call after System
        /// Configuration saves a new value so forms opened later in the same session pick it up.</summary>
        public static void Reload()
        {
            string hex = ConfigurationManager.AppSettings["CART_BOARD_BACKGROUND_COLOR"];
            CustomColor = string.IsNullOrWhiteSpace(hex) ? (Color?)null : ParseOrNull(hex);
        }

        /// <summary>Sets <paramref name="cartGrid"/>'s BackgroundColor to the configured custom
        /// color, or to its own container's current BackColor when unconfigured (transparent).</summary>
        public static void ApplyTo(DataGridView cartGrid)
        {
            if (cartGrid == null)
                return;

            Color target = CustomColor ?? cartGrid.Parent?.BackColor ?? cartGrid.BackgroundColor;
            if (cartGrid.BackgroundColor != target)
                cartGrid.BackgroundColor = target;
        }

        private static Color? ParseOrNull(string html)
        {
            try { return ColorTranslator.FromHtml(html); }
            catch { return null; }
        }
    }
}
