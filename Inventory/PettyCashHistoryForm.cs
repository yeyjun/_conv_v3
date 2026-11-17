using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Inventory
{
    public partial class PettyCashHistoryForm : Form
    {
        public PettyCashHistoryForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private readonly DataTable pettyCashTable = BuildPettyCashTable();

        private static DataTable BuildPettyCashTable()
        {
            var table = new DataTable();
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("EntryDate", typeof(string));
            table.Columns.Add("SortDate", typeof(DateTime));
            return table;
        }


        private async void PettyCashHistoryForm_Load(object sender, EventArgs e)
        {
            var screen = Screen.FromControl(this.Owner ?? this);
            this.Location = new System.Drawing.Point(screen.WorkingArea.Left + 40, screen.WorkingArea.Top + 80);

            dgvPettyCash.AutoGenerateColumns = false;
            pettyCashTable.DefaultView.Sort = "SortDate DESC";
            dgvPettyCash.DataSource = pettyCashTable;

            DateTime now = DateTime.Now;
            dtpFrom.Value = new DateTime(now.Year, now.Month, 1);
            dtpTo.Value = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            await ApplyDateFilterAsync();
        }

        // Builds a parameterized WHERE clause matching each day in [from, to] against the
        // "yyyy-MMMM-dd" format date_now is stored in (see PettyCashEntryForm.btnSave_Click).
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
                clauses.Add($"date_now LIKE {pname}");
                pars.Add(new MySqlParameter(pname, "%" + d.ToString("yyyy-MMMM-dd") + "%"));
            }

            where = clauses.Count > 0 ? "(" + string.Join(" OR ", clauses) + ")" : "1=1";
            parameters = pars;
        }

        private async System.Threading.Tasks.Task ApplyDateFilterAsync()
        {
            BuildDateRangeWhere(dtpFrom.Value, dtpTo.Value, out string where, out List<MySqlParameter> pars);
            await LoadPettyCashAsync(where, pars);
        }

        private async System.Threading.Tasks.Task LoadPettyCashAsync(string whereClause, List<MySqlParameter> parameters)
        {
            pettyCashTable.Rows.Clear();

            try
            {
                using (var conn = new MySqlConnection(DbConnectionHelper.BuildConnectionString()))
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT cash, date_now FROM pettycash_ WHERE {whereClause}";

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string dateText = reader["date_now"]?.ToString();
                            DateTime.TryParseExact(dateText, "yyyy-MMMM-dd", null,
                                System.Globalization.DateTimeStyles.None, out DateTime parsed);

                            pettyCashTable.Rows.Add(
                                reader["cash"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["cash"]),
                                dateText,
                                parsed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not load the petty cash history.", "Failed to Load Petty Cash History", ex);
            }

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            int entryCount = pettyCashTable.Rows.Count;
            decimal total = pettyCashTable.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["Amount"]));
            lblSummary.Text = $"Entries: {entryCount}    Total: {total:0.00}";
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
            await LoadPettyCashAsync("1=1", null);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await ApplyDateFilterAsync();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Purchases.ExportToExcel(dgvPettyCash);
        }

        private async void btnAddEntry_Click(object sender, EventArgs e)
        {
            await ShowAddEntryDialogAsync();
        }

        private async System.Threading.Tasks.Task ShowAddEntryDialogAsync()
        {
            using (var entryForm = new PettyCashEntryForm())
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

        private async void PettyCashHistoryForm_KeyDown(object sender, KeyEventArgs e)
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
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
