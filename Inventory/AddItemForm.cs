using System;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace _conv
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

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Barcode and description are required.", "Missing Information");
                return;
            }

            if (!decimal.TryParse(tbx_price.Text, out decimal price) || price < 0 ||
                !decimal.TryParse(tbx_retailPrice.Text, out decimal retailPrice) || retailPrice < 0 ||
                !decimal.TryParse(tbx_pricePerPc.Text, out decimal pricePerPc) || pricePerPc < 0 ||
                !int.TryParse(tbx_quantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Price, retail price, price per piece and starting quantity must be non-negative numbers.", "Invalid Input");
                return;
            }

            try
            {
                string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
                string dbname = ConfigurationManager.AppSettings["DB_NAME"];
                string dbuser = ConfigurationManager.AppSettings["DB_USER"];
                string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];

                using (var conn = new MySqlConnection($"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};"))
                {
                    await conn.OpenAsync();

                    using (var cmd = new MySqlCommand(
                        "INSERT INTO items_(serial, description, price, retail_price, price_perpc, new_quantity, qty_inpc, date_of_entry) " +
                        "VALUES (@serial, @description, @price, @retail_price, @price_perpc, @quantity, @quantity, @date_of_entry)", conn))
                    {
                        cmd.Parameters.AddWithValue("@serial", code);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@retail_price", retailPrice);
                        cmd.Parameters.AddWithValue("@price_perpc", pricePerPc);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@date_of_entry", DateTime.Now);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                ItemSaved = true;
                MessageBox.Show("Item saved. You can now print its barcode.", "Item Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to save item.");
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
