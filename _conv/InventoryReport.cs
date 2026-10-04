using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using YourApp.Data;

namespace _conv
{
    public partial class InventoryReport : Form
    {
        public InventoryReport()
        {
            InitializeComponent();
            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);
        }

        private void InventoryReport_Load(object sender, EventArgs e)
        {
            cmbStockFilter.SelectedIndex = 0;
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            dgvInventory.Rows.Clear();

            DataTable items = DataCacheManager.Instance.GetTable("Items");

            string search = (txtSearch.Text ?? "").Trim();
            string stockFilter = cmbStockFilter.SelectedItem as string ?? "All Items";

            var rows = items.AsEnumerable().Where(row =>
                (string.IsNullOrEmpty(search)
                    || row["serial"].ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                    || row["description"].ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                && (stockFilter == "All Items"
                    || (stockFilter == "In Stock" && ToLong(row["new_quantity"]) > 0)
                    || (stockFilter == "Out of Stock" && ToLong(row["new_quantity"]) <= 0)));

            foreach (DataRow row in rows)
            {
                dgvInventory.Rows.Add(
                    row["serial"].ToString(),
                    row["description"].ToString().ToUpperInvariant(),
                    FormatPrice(row["price"]),
                    FormatPrice(row["retail_price"]),
                    FormatPrice(row["price_perpc"]),
                    row["new_quantity"].ToString());
            }

            lblTotal.Text = $"Total items: {dgvInventory.Rows.Count}";
        }

        private static long ToLong(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            return long.TryParse(value.ToString(), out long result) ? result : 0;
        }

        private static string FormatPrice(object value) =>
            value == null || value == DBNull.Value ? "" : Convert.ToDecimal(value).ToString("0.00");

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DataCacheManager.Instance.RefreshTable("Items", "SELECT * FROM items_");
            ApplyFilters();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Purchases.ExportToExcel(dgvInventory);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cmbStockFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            cmbStockFilter.SelectedIndex = 0;
        }

        private void InventoryReport_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnRefresh_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}
