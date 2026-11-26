using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using YourApp.Data;

namespace Inventory
{
    public partial class AddItemForm : Form
    {
        public bool ItemSaved { get; private set; } = false;

        public AddItemForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            PopulateCategoryChoices();
        }

        /// <summary>Lets the admin pick an existing category instead of retyping it (and risking
        /// a near-duplicate like "Snacks" vs "snack") while still allowing a brand new one to be
        /// typed directly - cbx_category's DropDownStyle is DropDown, not DropDownList.</summary>
        private void PopulateCategoryChoices()
        {
            try
            {
                DataTable items = DataCacheManager.Instance.GetTable("Items");
                if (!items.Columns.Contains("category"))
                    return;

                var categories = items.AsEnumerable()
                    .Select(r => r["category"]?.ToString()?.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(c => c, StringComparer.OrdinalIgnoreCase);

                cbx_category.Items.AddRange(categories.ToArray());
            }
            catch
            {
                // Cache not loaded (e.g. opened before LoadAll completes) - the combo just starts
                // empty and still accepts freely typed text either way.
            }
        }

        private void WholeNumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void btn_generateCode_Click(object sender, EventArgs e)
        {
            tbx_code.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void btn_printBarcode_Click(object sender, EventArgs e)
        {
            string code = tbx_code.Text.Trim();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Enter or auto-generate a barcode value first.", "Barcode");
                return;
            }

            using (var barcodeForm = new GenerateBarcode { code = code })
            {
                barcodeForm.ShowDialog(this);
            }
        }

        private async void btn_save_Click(object sender, EventArgs e)
        {
            string code = tbx_code.Text.Trim();
            string description = tbx_description.Text.Trim();
            string category = cbx_category.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Barcode and description are required.", "Missing Information");
                return;
            }

            if (!int.TryParse(tbx_price.Text, out int price) || price < 0 ||
                !int.TryParse(tbx_retailPrice.Text, out int retailPrice) || retailPrice < 0 ||
                !int.TryParse(tbx_pricePerPc.Text, out int pricePerPc) || pricePerPc < 0 ||
                !int.TryParse(tbx_quantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Price, retail price, price per piece and starting quantity must be non-negative whole numbers.", "Invalid Input");
                return;
            }

            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            DateTime dateOfEntry = DateTime.Now;

            var conn = new MySqlConnection($"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};");
            try
            {
                await conn.OpenAsync();
            }
            catch (Exception ex)
            {
                conn.Dispose();
                YourApp.Data.PendingItemStore.Add(code, description, category, price, retailPrice, pricePerPc, quantity, dateOfEntry);
                ItemSaved = true;
                OperationLog.RecordCreate(nameof(AddItemForm), $"Item '{code}' ({description}) created offline - queued for later sync.");
                ErrorDialog.Show(this,
                    "Could not reach the database, so this item was saved on this computer instead. " +
                    "It's already available for purchases here, and will need to be synced to the database once the connection is back.",
                    "Saved Offline", ex, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (conn)
                using (var cmd = new MySqlCommand(
                    "INSERT INTO items_(serial, description, category, price, retail_price, price_perpc, new_quantity, qty_inpc, date_of_entry) " +
                    "VALUES (@serial, @description, @category, @price, @retail_price, @price_perpc, @quantity, @quantity, @date_of_entry)", conn))
                {
                    cmd.Parameters.AddWithValue("@serial", code);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@retail_price", retailPrice);
                    cmd.Parameters.AddWithValue("@price_perpc", pricePerPc);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@date_of_entry", dateOfEntry);
                    await cmd.ExecuteNonQueryAsync();
                }

                ItemSaved = true;
                OperationLog.RecordCreate(nameof(AddItemForm), $"Item '{code}' ({description}) created.");

                // Without this, the newly saved item (and its category) wouldn't show up anywhere
                // that reads from the cache - Manage Prices, the item search grid, or the
                // categorized inventory report/preview - until some other action happened to
                // refresh it first.
                try { DataCacheManager.Instance.RefreshTable("Items", "SELECT * FROM items_"); } catch { }

                MessageBox.Show("Item saved. You can now print its barcode.", "Item Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not save this item to the database.", "Failed to Save Item", ex);
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
