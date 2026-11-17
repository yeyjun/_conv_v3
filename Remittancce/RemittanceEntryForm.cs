using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;

namespace Remittancce
{
    // Entry dialog for the "send money" / padala counter service - the store acts as a
    // remittance agent, taking cash from a sender and a service fee, and records what needs
    // to go out via the chosen provider (GCash, Palawan Express, Cebuana Lhuillier, etc.).
    public partial class RemittanceEntryForm : Form
    {
        private readonly string tellerName;

        public RemittanceEntryForm(string tellerName)
        {
            InitializeComponent();
            this.tellerName = tellerName;
            UiStyle.StylePrimary(btnSave);
            UiStyle.StyleSecondary(btnCancel);
        }

        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"]?.Trim();
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";
        }

        private void RemittanceEntryForm_Load(object sender, EventArgs e)
        {
            lblDate.Text = $"Date: {DateTime.Now:yyyy-MMMM-dd}";
            cboProvider.SelectedIndex = 0;
            txtSenderName.Focus();
        }

        private void AmountOrFee_TextChanged(object sender, EventArgs e)
        {
            decimal amount = ParseDecimalOrZero(txtAmount.Text);
            decimal fee = ParseDecimalOrZero(txtFee.Text);
            lblTotalValue.Text = (amount + fee).ToString("N2", CultureInfo.InvariantCulture);
        }

        private static decimal ParseDecimalOrZero(string text) =>
            decimal.TryParse(text.Trim(), out decimal value) && value > 0 ? value : 0m;

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string senderName = txtSenderName.Text.Trim();
            string receiverName = txtReceiverName.Text.Trim();

            if (string.IsNullOrWhiteSpace(senderName))
            {
                MessageBox.Show("Enter the sender's name.", "Invalid Input");
                txtSenderName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(receiverName))
            {
                MessageBox.Show("Enter the receiver's name.", "Invalid Input");
                txtReceiverName.Focus();
                return;
            }

            if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Enter an amount to send greater than zero.", "Invalid Input");
                txtAmount.SelectAll();
                txtAmount.Focus();
                return;
            }

            if (!decimal.TryParse(txtFee.Text.Trim(), out decimal fee) || fee < 0)
            {
                MessageBox.Show("Service fee must be zero or a positive amount.", "Invalid Input");
                txtFee.SelectAll();
                txtFee.Focus();
                return;
            }

            btnSave.Enabled = false;
            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand(
                    "INSERT INTO remittance_(provider, sender_name, sender_contact, receiver_name, receiver_contact, " +
                    "amount, service_fee, reference_no, teller, notes, created_at) VALUES " +
                    "(@provider, @sender_name, @sender_contact, @receiver_name, @receiver_contact, " +
                    "@amount, @service_fee, @reference_no, @teller, @notes, @created_at)", conn))
                {
                    cmd.Parameters.AddWithValue("@provider", cboProvider.Text.Trim());
                    cmd.Parameters.AddWithValue("@sender_name", senderName);
                    cmd.Parameters.AddWithValue("@sender_contact", txtSenderContact.Text.Trim());
                    cmd.Parameters.AddWithValue("@receiver_name", receiverName);
                    cmd.Parameters.AddWithValue("@receiver_contact", txtReceiverContact.Text.Trim());
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@service_fee", fee);
                    cmd.Parameters.AddWithValue("@reference_no", txtReference.Text.Trim());
                    cmd.Parameters.AddWithValue("@teller", tellerName);
                    cmd.Parameters.AddWithValue("@notes", txtNotes.Text.Trim());
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Now);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not save this remittance entry.", "Save Failed", ex);
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
