using System.Collections.Generic;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>Lightweight row shape for the low-stock reminder - just what the popup shows.</summary>
    public sealed class LowStockItemInfo
    {
        public string Serial { get; set; } = "";
        public string Description { get; set; } = "";
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Centered, informational popup listing items at or below the configured low-stock
    /// threshold (System Config > Low Stock Reminder). Shown periodically by Purchases'
    /// low-stock watcher (see StartLowStockReminderWatcher in Form1.cs) - purely a notice, so
    /// unlike MaintenanceForm there's no PIN-gated override and no consequence to just closing
    /// it; Close is the only way out.
    /// </summary>
    public partial class LowStockReminderForm : Form
    {
        public LowStockReminderForm(List<LowStockItemInfo> items, int threshold)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            lblSummary.Text = $"{items.Count} item{(items.Count == 1 ? "" : "s")} at or below the low-stock threshold ({threshold} unit{(threshold == 1 ? "" : "s")}):";

            dgvLowStock.Rows.Clear();
            foreach (var item in items)
            {
                dgvLowStock.Rows.Add(item.Serial, item.Description, item.Quantity);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
