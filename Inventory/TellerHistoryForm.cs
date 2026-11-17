using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Inventory
{
    public partial class TellerHistoryForm : Form
    {
        public TellerHistoryForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private readonly DataTable historyTable = BuildHistoryTable();

        private static DataTable BuildHistoryTable()
        {
            var table = new DataTable();
            table.Columns.Add("TransactionCode", typeof(string));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("TransactionDate", typeof(string));
            return table;
        }


        private async void TellerHistoryForm_Load(object sender, EventArgs e)
        {
            this.Text = $"My Transaction History — {Purchases.log_in}";

            var screen = Screen.FromControl(this.Owner ?? this);
            this.Location = new System.Drawing.Point(screen.WorkingArea.Left + 20, screen.WorkingArea.Top + 60);

            dgvHistory.AutoGenerateColumns = false;
            dgvHistory.DataSource = historyTable;

            DateTime now = DateTime.Now;
            dtpFrom.Value = new DateTime(now.Year, now.Month, 1);
            dtpTo.Value = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            await ApplyDateFilterAsync();
        }

        // Builds a parameterized WHERE clause matching each day in [from, to] against the
        // "yyyy-MMMM-dd ..." format transaction_date is stored in (see Purchases.SaveSaleLineItemsAsync).
        private static void BuildDateRangeWhere(DateTime from, DateTime to, out string where, out List<MySqlParameter> parameters)
        {
            var clauses = new List<string>();
            var pars = new List<MySqlParameter>();

            DateTime start = from.Date;
            DateTime end = to.Date;
            if (end < start)
            {
                DateTime tmp = start;
                start = end;
                end = tmp;
            }

            int i = 0;
            for (DateTime d = start; d <= end && i < 366; d = d.AddDays(1), i++)
            {
                string pname = "@d" + i;
                clauses.Add($"s.transaction_date LIKE {pname}");
                pars.Add(new MySqlParameter(pname, "%" + d.ToString("yyyy-MMMM-dd") + "%"));
            }

            where = clauses.Count > 0 ? "(" + string.Join(" OR ", clauses) + ")" : "1=1";
            parameters = pars;
        }

        private async System.Threading.Tasks.Task ApplyDateFilterAsync()
        {
            BuildDateRangeWhere(dtpFrom.Value, dtpTo.Value, out string where, out List<MySqlParameter> pars);
            await LoadHistoryAsync(where, pars);
        }

        private async System.Threading.Tasks.Task LoadHistoryAsync(string whereClause, List<MySqlParameter> parameters)
        {
            historyTable.Rows.Clear();

            try
            {
                using (var conn = new MySqlConnection(DbConnectionHelper.BuildConnectionString()))
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "SELECT s.transaction_code AS transaction_code, s.itemcode AS itemcode, " +
                        "s.transaction_date AS transaction_date, s.item_price AS item_price, i.description AS itemname " +
                        "FROM sales_ s LEFT JOIN items_ i ON s.itemcode = i.serial " +
                        $"WHERE s.seller = @teller AND {whereClause} ORDER BY s.transaction_date DESC";

                    cmd.Parameters.AddWithValue("@teller", Purchases.log_in);

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            historyTable.Rows.Add(
                                reader["transaction_code"]?.ToString(),
                                reader["itemcode"]?.ToString(),
                                reader["itemname"]?.ToString(),
                                reader["item_price"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["item_price"]),
                                reader["transaction_date"]?.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not load the teller's transaction history.", "Failed to Load Transaction History", ex);
            }

            ApplySearchFilter();
            UpdateSummary();
        }

        private void ApplySearchFilter()
        {
            string search = (txtSearch.Text ?? "").Trim().Replace("'", "''");
            historyTable.DefaultView.RowFilter = string.IsNullOrEmpty(search)
                ? string.Empty
                : $"ItemName LIKE '%{search}%' OR ItemCode LIKE '%{search}%' OR TransactionCode LIKE '%{search}%'";
        }

        private void UpdateSummary()
        {
            DataRow[] rows = historyTable.Select(historyTable.DefaultView.RowFilter);
            int itemCount = rows.Length;
            int transactionCount = rows.Select(r => r["TransactionCode"].ToString()).Distinct().Count();
            decimal total = rows.Sum(r => Convert.ToDecimal(r["Price"]));
            lblSummary.Text = $"Transactions: {transactionCount}    Items: {itemCount}    Total Sales: {total:0.00}";
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
            await LoadHistoryAsync("1=1", null);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await ApplyDateFilterAsync();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Purchases.ExportToExcel(dgvHistory);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplySearchFilter();
            UpdateSummary();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
            }
        }

        private async void dtp_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await ApplyDateFilterAsync();
            }
        }

        private async void TellerHistoryForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                await ApplyDateFilterAsync();
            }
        }
    }
}
