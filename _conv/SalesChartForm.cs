using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _conv
{
    public enum SalesChartPeriod
    {
        Daily,
        Monthly,
        Quarterly
    }

    public partial class SalesChartForm : Form
    {
        private readonly SalesChartPeriod period;

        public SalesChartForm(SalesChartPeriod period)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
            this.period = period;
        }

        private struct SaleRecord
        {
            public DateTime Date;
            public decimal Price;
        }

        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";
        }

        private async void SalesChartForm_Load(object sender, EventArgs e)
        {
            switch (period)
            {
                case SalesChartPeriod.Daily:
                    Text = "Daily Sales Chart";
                    break;
                case SalesChartPeriod.Monthly:
                    Text = "Monthly Sales Chart";
                    break;
                case SalesChartPeriod.Quarterly:
                    Text = "Quarterly Sales Chart";
                    break;
            }

            lblMonth.Visible = period == SalesChartPeriod.Daily;
            cmbMonth.Visible = period == SalesChartPeriod.Daily;

            cmbMonth.Items.AddRange(CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.Take(12).Cast<object>().ToArray());
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
            cmbMonth.SelectedIndexChanged += async (s, args) => await RefreshChartAsync();

            numYear.Value = DateTime.Now.Year;
            numYear.ValueChanged += async (s, args) => await RefreshChartAsync();

            chart1.Series["Sales"].IsValueShownAsLabel = true;
            chart1.Series["Sales"].LabelFormat = "N0";
            chart1.ChartAreas["ChartArea1"].AxisY.Title = "Total Sales";
            chart1.Legends["Legend1"].Enabled = false;

            await RefreshChartAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshChartAsync();
        }

        private async Task RefreshChartAsync()
        {
            int year = (int)numYear.Value;
            List<SaleRecord> sales = await LoadYearSalesAsync(year);

            switch (period)
            {
                case SalesChartPeriod.Daily:
                    PopulateDailyChart(sales, year, cmbMonth.SelectedIndex + 1);
                    break;
                case SalesChartPeriod.Monthly:
                    PopulateMonthlyChart(sales, year);
                    break;
                case SalesChartPeriod.Quarterly:
                    PopulateQuarterlyChart(sales, year);
                    break;
            }
        }

        // transaction_date is stored as "yyyy-MMMM-dd H:m:ss" (see Form1.CreateTransactionCode),
        // so a whole year's rows can be selected with a single "yyyy-%" LIKE pattern.
        private async Task<List<SaleRecord>> LoadYearSalesAsync(int year)
        {
            var result = new List<SaleRecord>();

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT s.transaction_date AS transaction_date, s.item_price AS item_price FROM sales_ s WHERE s.transaction_date LIKE @yearPattern";
                    cmd.Parameters.AddWithValue("@yearPattern", year + "-%");

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string raw = reader["transaction_date"]?.ToString();
                            decimal price = reader["item_price"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["item_price"]);

                            if (DateTime.TryParseExact(raw, "yyyy-MMMM-dd H:m:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                            {
                                result.Add(new SaleRecord { Date = parsed, Price = price });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to load sales");
            }

            return result;
        }

        private void PopulateDailyChart(List<SaleRecord> sales, int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            decimal[] totals = new decimal[daysInMonth];

            foreach (SaleRecord s in sales)
            {
                if (s.Date.Year == year && s.Date.Month == month)
                    totals[s.Date.Day - 1] += s.Price;
            }

            var series = chart1.Series["Sales"];
            series.Points.Clear();
            for (int day = 1; day <= daysInMonth; day++)
            {
                series.Points.AddXY(day, totals[day - 1]);
            }

            chart1.ChartAreas["ChartArea1"].AxisX.Title = "Day";
            chart1.ChartAreas["ChartArea1"].AxisX.Interval = 1;

            lblSummary.Text = $"Total Sales: {totals.Sum():N2}";
        }

        private void PopulateMonthlyChart(List<SaleRecord> sales, int year)
        {
            decimal[] totals = new decimal[12];

            foreach (SaleRecord s in sales)
            {
                if (s.Date.Year == year)
                    totals[s.Date.Month - 1] += s.Price;
            }

            string[] monthLabels = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames.Take(12).ToArray();

            var series = chart1.Series["Sales"];
            series.Points.Clear();
            for (int i = 0; i < 12; i++)
            {
                series.Points.AddXY(monthLabels[i], totals[i]);
            }

            chart1.ChartAreas["ChartArea1"].AxisX.Title = "Month";

            lblSummary.Text = $"Total Sales: {totals.Sum():N2}";
        }

        private void PopulateQuarterlyChart(List<SaleRecord> sales, int year)
        {
            decimal[] totals = new decimal[4];

            foreach (SaleRecord s in sales)
            {
                if (s.Date.Year == year)
                    totals[(s.Date.Month - 1) / 3] += s.Price;
            }

            string[] quarterLabels = { "Q1", "Q2", "Q3", "Q4" };

            var series = chart1.Series["Sales"];
            series.Points.Clear();
            for (int i = 0; i < 4; i++)
            {
                series.Points.AddXY(quarterLabels[i], totals[i]);
            }

            chart1.ChartAreas["ChartArea1"].AxisX.Title = "Quarter";

            lblSummary.Text = $"Total Sales: {totals.Sum():N2}";
        }
    }
}
