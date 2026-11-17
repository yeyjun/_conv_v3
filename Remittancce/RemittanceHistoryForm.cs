using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Remittancce
{
    // Main window of the standalone Remittancce app - a "send money" / padala counter tool
    // the store uses as a remittance agent. Deliberately has no project reference back to
    // Inventory (same reasoning as RemoteController, see CLAUDE.md): it duplicates the DB
    // connection string building, ErrorDialog, and remittance_ table access rather than
    // sharing code with the POS app. There's no login system here, so the operator types
    // their name into the Teller field instead of reusing Purchases.log_in.
    public partial class RemittanceHistoryForm : Form
    {
        public RemittanceHistoryForm()
        {
            InitializeComponent();
            UiStyle.StyleGrid(dgvRemittance);
            lblHeaderDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private readonly DataTable remittanceTable = BuildRemittanceTable();

        private static DataTable BuildRemittanceTable()
        {
            var table = new DataTable();
            table.Columns.Add("Provider", typeof(string));
            table.Columns.Add("SenderName", typeof(string));
            table.Columns.Add("ReceiverName", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("ServiceFee", typeof(decimal));
            table.Columns.Add("Total", typeof(decimal));
            table.Columns.Add("ReferenceNo", typeof(string));
            table.Columns.Add("Teller", typeof(string));
            table.Columns.Add("EntryDate", typeof(string));
            table.Columns.Add("SortDate", typeof(DateTime));
            return table;
        }

        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"]?.Trim();
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";
        }

        private async void RemittanceHistoryForm_Load(object sender, EventArgs e)
        {
            txtTeller.Text = Environment.UserName;

            dgvRemittance.AutoGenerateColumns = false;
            remittanceTable.DefaultView.Sort = "SortDate DESC";
            dgvRemittance.DataSource = remittanceTable;

            DateTime now = DateTime.Now;
            dtpFrom.Value = new DateTime(now.Year, now.Month, 1);
            dtpTo.Value = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            await ApplyDateFilterAsync();
        }

        private async System.Threading.Tasks.Task ApplyDateFilterAsync()
        {
            DateTime start = dtpFrom.Value.Date;
            DateTime end = dtpTo.Value.Date;
            if (end < start)
            {
                DateTime tmp = start;
                start = end;
                end = tmp;
            }

            await LoadRemittanceAsync("created_at >= @from AND created_at < @to", new List<MySqlParameter>
            {
                new MySqlParameter("@from", start),
                new MySqlParameter("@to", end.AddDays(1)),
            });
        }

        private async System.Threading.Tasks.Task LoadRemittanceAsync(string whereClause, List<MySqlParameter> parameters)
        {
            remittanceTable.Rows.Clear();

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT provider, sender_name, receiver_name, amount, service_fee, " +
                        $"reference_no, teller, created_at FROM remittance_ WHERE {whereClause}";

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            decimal amount = reader["amount"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["amount"]);
                            decimal fee = reader["service_fee"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["service_fee"]);
                            DateTime createdAt = reader["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["created_at"]);

                            remittanceTable.Rows.Add(
                                reader["provider"]?.ToString(),
                                reader["sender_name"]?.ToString(),
                                reader["receiver_name"]?.ToString(),
                                amount,
                                fee,
                                amount + fee,
                                reader["reference_no"]?.ToString(),
                                reader["teller"]?.ToString(),
                                createdAt.ToString("yyyy-MMMM-dd h:mm tt"),
                                createdAt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not load the remittance history.", "Failed to Load Remittance History", ex);
            }

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            int entryCount = remittanceTable.Rows.Count;
            decimal totalSent = remittanceTable.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["Amount"]));
            decimal totalFees = remittanceTable.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["ServiceFee"]));
            lblSummary.Text = $"Entries: {entryCount}    Total Sent: {totalSent:0.00}    Fees Collected: {totalFees:0.00}";
        }

        private async void btnFilter_Click(object sender, EventArgs e)
        {
            await ApplyDateFilterAsync();
        }

        private async void btnToday_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Now;
            dtpTo.Value = DateTime.Now;
            await ApplyDateFilterAsync();
        }

        private async void btnThisMonth_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            dtpFrom.Value = new DateTime(now.Year, now.Month, 1);
            dtpTo.Value = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            await ApplyDateFilterAsync();
        }

        private async void btnAll_Click(object sender, EventArgs e)
        {
            await LoadRemittanceAsync("1=1", null);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await ApplyDateFilterAsync();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"Remittance_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Date,Provider,Sender,Receiver,Amount,Fee,Total,Reference No.,Teller");
                    foreach (DataRow row in remittanceTable.Rows)
                    {
                        sb.AppendLine(string.Join(",",
                            CsvField(row["EntryDate"]),
                            CsvField(row["Provider"]),
                            CsvField(row["SenderName"]),
                            CsvField(row["ReceiverName"]),
                            CsvField(Convert.ToDecimal(row["Amount"]).ToString("0.00", CultureInfo.InvariantCulture)),
                            CsvField(Convert.ToDecimal(row["ServiceFee"]).ToString("0.00", CultureInfo.InvariantCulture)),
                            CsvField(Convert.ToDecimal(row["Total"]).ToString("0.00", CultureInfo.InvariantCulture)),
                            CsvField(row["ReferenceNo"]),
                            CsvField(row["Teller"])));
                    }

                    File.WriteAllText(dialog.FileName, sb.ToString());
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show(this, "Could not export the remittance history.", "Export Failed", ex);
                }
            }
        }

        private static string CsvField(object value)
        {
            string text = value?.ToString() ?? "";
            return text.Contains(",") || text.Contains("\"")
                ? "\"" + text.Replace("\"", "\"\"") + "\""
                : text;
        }

        private async void btnAddEntry_Click(object sender, EventArgs e)
        {
            await ShowAddEntryDialogAsync();
        }

        private async System.Threading.Tasks.Task ShowAddEntryDialogAsync()
        {
            string tellerName = string.IsNullOrWhiteSpace(txtTeller.Text) ? Environment.UserName : txtTeller.Text.Trim();

            using (var entryForm = new RemittanceEntryForm(tellerName))
            {
                if (entryForm.ShowDialog(this) == DialogResult.OK)
                {
                    await ApplyDateFilterAsync();
                }
            }
        }

        private async void dtp_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await ApplyDateFilterAsync();
            }
        }

        private async void RemittanceHistoryForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                await ApplyDateFilterAsync();
            }
            else if (e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                await ShowAddEntryDialogAsync();
            }
        }
    }
}
