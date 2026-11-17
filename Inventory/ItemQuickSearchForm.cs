using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using YourApp.Data;
using static Inventory.DataRowConvert;

namespace Inventory
{
    public partial class ItemQuickSearchForm : Form
    {
        public ItemQuickSearchForm()
        {
            InitializeComponent();
            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);

            TopMost = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x00020000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public void FocusSearchBox() => txtSearch.Focus();

        private void txtSearch_TextChanged(object sender, EventArgs e) => RefreshResults();

        private const string DefaultHintText = "Price check only - items are not added to the cart from here.";

        private void RefreshResults()
        {
            dgvResults.Rows.Clear();

            string query = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                lblHint.ForeColor = System.Drawing.Color.Gray;
                lblHint.Text = DefaultHintText;
                return;
            }

            try
            {
                var matches = DataCacheManager.Instance.GetTable("Items").AsEnumerable()
                    .Where(r =>
                    {
                        string serial = r.Field<string>("serial") ?? "";
                        string description = r.Field<string>("description") ?? "";
                        return !serial.Contains("https:") &&
                            (description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                             serial.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0);
                    })
                    .OrderBy(r => r.Field<string>("description"))
                    .Take(100);

                foreach (var r in matches)
                {
                    dgvResults.Rows.Add(
                        r["serial"].ToString(),
                        r["description"].ToString().ToUpperInvariant(),
                        ToDecimal(r["price"]),
                        ToDecimal(r["retail_price"]),
                        ToDecimal(r["price_perpc"]));
                }

                lblHint.ForeColor = System.Drawing.Color.Gray;
                lblHint.Text = DefaultHintText;
            }
            catch (Exception ex)
            {
                dgvResults.Rows.Clear();
                lblHint.ForeColor = System.Drawing.Color.Firebrick;
                lblHint.Text = "Search failed: " + ex.Message;
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void ItemQuickSearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
