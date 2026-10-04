using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace _conv
{
    public partial class PettyCashEntryForm : Form
    {
        public PettyCashEntryForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";
        }

        private void PettyCashEntryForm_Load(object sender, EventArgs e)
        {
            lblDate.Text = $"Date: {DateTime.Now:yyyy-MMMM-dd}";
            txtAmount.Focus();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            // The pettycash_.cash column is int(11), so only whole amounts are accepted.
            if (!int.TryParse(txtAmount.Text.Trim(), out int cash) || cash <= 0)
            {
                MessageBox.Show("Please enter a whole amount greater than zero.", "Invalid amount");
                txtAmount.SelectAll();
                txtAmount.Focus();
                return;
            }

            btnSave.Enabled = false;
            try
            {
                string today = DateTime.Now.ToString("yyyy-MMMM-dd");

                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand("INSERT INTO pettycash_(cash,date_now) VALUES(@cash,@date_now)", conn))
                {
                    cmd.Parameters.AddWithValue("@cash", cash);
                    cmd.Parameters.AddWithValue("@date_now", today);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                await ActivityLog.LogAsync($"Petty cash entry of {cash} recorded by {Purchases.log_in}");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save petty cash entry.{Environment.NewLine}{ex.Message}", "Save failed");
                btnSave.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
