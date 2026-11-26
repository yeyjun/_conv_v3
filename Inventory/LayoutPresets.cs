namespace Inventory
{
    /// <summary>
    /// A named bundle of settings that already exist individually on System Config's
    /// Appearance/Cart Columns/Item Search tabs, so a store can switch its look in one step
    /// instead of tuning each knob by hand. <see cref="VisibleCartColumns"/>/<see cref="VisibleItemSearchColumns"/>
    /// are display order; any column from <see cref="Purchases.CartColumns"/>/<see cref="Purchases.ItemSearchColumns"/>
    /// not listed is hidden.
    /// </summary>
    internal sealed class LayoutPreset
    {
        public string Name;
        public decimal FontScalePt;
        public string[] VisibleCartColumns;
        public string[] VisibleItemSearchColumns;

        public override string ToString() => Name;
    }

    internal static class LayoutPresets
    {
        public static readonly LayoutPreset[] All =
        {
            new LayoutPreset
            {
                Name = "Default",
                FontScalePt = 0m,
                VisibleCartColumns = new[] { "serial", "description", "quantity", "uprice", "price" },
                VisibleItemSearchColumns = new[] { "serial", "description", "price" },
            },
            new LayoutPreset
            {
                Name = "Compact",
                FontScalePt = -1m,
                VisibleCartColumns = new[] { "description", "quantity", "price" },
                VisibleItemSearchColumns = new[] { "description", "price" },
            },
            new LayoutPreset
            {
                Name = "Cashier Simplified",
                FontScalePt = 2m,
                VisibleCartColumns = new[] { "description", "quantity", "price" },
                VisibleItemSearchColumns = new[] { "description", "price" },
            },
        };
    }
}
