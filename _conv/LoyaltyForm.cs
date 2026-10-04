using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _conv
{
    public partial class LoyaltyForm : Form
    {
        // Points customers earn per currency unit spent, e.g. 1 point per 200 spent.
        private const decimal PointsRateCurrencyPerPoint = 200m;
        private readonly string StoreName = ConfigurationManager.AppSettings["STORE_NAME"] ?? "Baclig Minimart";

        private readonly DataTable customersTable = BuildCustomersTable();
        private readonly DataTable historyTable = BuildHistoryTable();

        private string _originalCode = null;
        private string _selectedCreditCode = null;

        private string _previewLoyaltyCode;
        private string _previewFullName;
        private int _previewPoints;
        private DateTime? _previewDateRegistered;

        public LoyaltyForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private static DataTable BuildCustomersTable()
        {
            var table = new DataTable();
            table.Columns.Add("Loyalty Code", typeof(string));
            table.Columns.Add("Full Name", typeof(string));
            table.Columns.Add("Contact Number", typeof(string));
            table.Columns.Add("Points", typeof(int));
            table.Columns.Add("Date Registered", typeof(string));
            return table;
        }

        private static DataTable BuildHistoryTable()
        {
            var table = new DataTable();
            table.Columns.Add("Date", typeof(string));
            table.Columns.Add("Amount Spent", typeof(string));
            table.Columns.Add("Points Earned", typeof(int));
            table.Columns.Add("Teller", typeof(string));
            return table;
        }

        private static string BuildConnectionString()
        {
            string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
            string dbname = ConfigurationManager.AppSettings["DB_NAME"];
            string dbuser = ConfigurationManager.AppSettings["DB_USER"];
            string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
            return $"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};";
        }

        private async void LoyaltyForm_Load(object sender, EventArgs e)
        {
            dgvCustomers.AutoGenerateColumns = true;
            dgvCustomers.DataSource = customersTable;

            dgvHistory.AutoGenerateColumns = true;
            dgvHistory.DataSource = historyTable;

            UpdateCardPreview(null, null, 0, null);

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    using (var cmd = new MySqlCommand(
                        "CREATE TABLE IF NOT EXISTS customers_ (" +
                        "loyalty_code VARCHAR(30) PRIMARY KEY, " +
                        "full_name VARCHAR(150) NOT NULL, " +
                        "contact_number VARCHAR(30), " +
                        "points INT NOT NULL DEFAULT 0, " +
                        "date_registered DATETIME NOT NULL)", conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new MySqlCommand(
                        "CREATE TABLE IF NOT EXISTS loyalty_transactions_ (" +
                        "id INT AUTO_INCREMENT PRIMARY KEY, " +
                        "loyalty_code VARCHAR(30) NOT NULL, " +
                        "amount_spent DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                        "points_earned INT NOT NULL, " +
                        "teller VARCHAR(100), " +
                        "date_credited DATETIME NOT NULL)", conn))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to prepare loyalty tables");
                return;
            }

            await LoadCustomersAsync();
        }

        private async Task LoadCustomersAsync()
        {
            customersTable.Rows.Clear();
            lblRegisterStatus.Text = "";

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand(
                    "SELECT loyalty_code, full_name, contact_number, points, date_registered FROM customers_ ORDER BY full_name", conn))
                {
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customersTable.Rows.Add(
                                reader["loyalty_code"]?.ToString(),
                                reader["full_name"]?.ToString(),
                                reader["contact_number"] == DBNull.Value ? "" : reader["contact_number"].ToString(),
                                Convert.ToInt32(reader["points"]),
                                Convert.ToDateTime(reader["date_registered"]).ToString("g"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to load customers");
            }
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is DataRowView row)
            {
                _originalCode = row["Loyalty Code"].ToString();
                txtCode.Text = _originalCode;
                txtFullName.Text = row["Full Name"].ToString();
                txtContact.Text = row["Contact Number"].ToString();
                lblPointsValue.Text = row["Points"].ToString();
                lblRegisterStatus.Text = "";

                DateTime.TryParse(row["Date Registered"].ToString(), out DateTime dateRegistered);
                UpdateCardPreview(_originalCode, txtFullName.Text, Convert.ToInt32(row["Points"]), dateRegistered);
            }
        }

        private void btnNewCustomer_Click(object sender, EventArgs e)
        {
            dgvCustomers.ClearSelection();
            _originalCode = null;
            txtCode.Text = "LC" + DateTime.Now.ToString("yyMMddHHmmss");
            txtFullName.Clear();
            txtContact.Clear();
            lblPointsValue.Text = "0";
            lblRegisterStatus.Text = "";
            txtFullName.Focus();
            UpdateCardPreview(null, null, 0, null);
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string contact = txtContact.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(fullName))
            {
                lblRegisterStatus.Text = "Loyalty code and full name are both required.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    if (_originalCode == null)
                    {
                        using (var cmd = new MySqlCommand(
                            "INSERT INTO customers_(loyalty_code, full_name, contact_number, points, date_registered) " +
                            "VALUES (@code, @fullname, @contact, 0, @dateRegistered)", conn))
                        {
                            cmd.Parameters.AddWithValue("@code", code);
                            cmd.Parameters.AddWithValue("@fullname", fullName);
                            cmd.Parameters.AddWithValue("@contact", contact);
                            cmd.Parameters.AddWithValue("@dateRegistered", DateTime.Now);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        using (var cmd = new MySqlCommand(
                            "UPDATE customers_ SET loyalty_code=@code, full_name=@fullname, contact_number=@contact WHERE loyalty_code=@oldCode", conn))
                        {
                            cmd.Parameters.AddWithValue("@code", code);
                            cmd.Parameters.AddWithValue("@fullname", fullName);
                            cmd.Parameters.AddWithValue("@contact", contact);
                            cmd.Parameters.AddWithValue("@oldCode", _originalCode);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await LoadCustomersAsync();
                btnNewCustomer_Click(sender, e);

                var savedRow = customersTable.AsEnumerable()
                    .FirstOrDefault(r => r["Loyalty Code"].ToString() == code);
                if (savedRow != null)
                {
                    DateTime.TryParse(savedRow["Date Registered"].ToString(), out DateTime dateRegistered);
                    UpdateCardPreview(code, savedRow["Full Name"].ToString(), Convert.ToInt32(savedRow["Points"]), dateRegistered);
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                lblRegisterStatus.Text = $"'{code}' is already in use by another customer.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to save customer");
            }
        }

        private async void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (_originalCode == null)
            {
                lblRegisterStatus.Text = "Select a customer to delete.";
                return;
            }

            var confirm = MessageBox.Show(this, $"Delete customer '{_originalCode}'? This also removes their points history and cannot be undone.",
                "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    using (var cmd = new MySqlCommand("DELETE FROM loyalty_transactions_ WHERE loyalty_code=@code", conn))
                    {
                        cmd.Parameters.AddWithValue("@code", _originalCode);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new MySqlCommand("DELETE FROM customers_ WHERE loyalty_code=@code", conn))
                    {
                        cmd.Parameters.AddWithValue("@code", _originalCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                await LoadCustomersAsync();
                btnNewCustomer_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to delete customer");
            }
        }

        private async void btnRefreshCustomers_Click(object sender, EventArgs e)
        {
            await LoadCustomersAsync();
        }

        private async void txtSearchCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await FindCustomerAsync();
            }
        }

        private async void btnFindCustomer_Click(object sender, EventArgs e)
        {
            await FindCustomerAsync();
        }

        private async Task FindCustomerAsync()
        {
            string search = txtSearchCustomer.Text.Trim();
            lblCreditStatus.Text = "";

            if (string.IsNullOrEmpty(search))
            {
                lblCreditStatus.Text = "Type a loyalty code or customer name to search.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand(
                    "SELECT loyalty_code, full_name, points FROM customers_ " +
                    "WHERE loyalty_code = @code OR full_name LIKE @nameLike LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@code", search);
                    cmd.Parameters.AddWithValue("@nameLike", "%" + search + "%");
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            _selectedCreditCode = reader["loyalty_code"].ToString();
                            string name = reader["full_name"].ToString();
                            int points = Convert.ToInt32(reader["points"]);
                            lblCustomerInfo.Text = $"{name} ({_selectedCreditCode}) — Current Points: {points}";
                        }
                        else
                        {
                            _selectedCreditCode = null;
                            lblCustomerInfo.Text = "No customer selected";
                            lblCreditStatus.Text = "No matching customer found. Register them first.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to search customers");
                return;
            }

            historyTable.Rows.Clear();

            if (_selectedCreditCode == null)
                return;

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand(
                    "SELECT date_credited, amount_spent, points_earned, teller FROM loyalty_transactions_ " +
                    "WHERE loyalty_code = @code ORDER BY date_credited DESC LIMIT 20", conn))
                {
                    cmd.Parameters.AddWithValue("@code", _selectedCreditCode);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            historyTable.Rows.Add(
                                Convert.ToDateTime(reader["date_credited"]).ToString("g"),
                                Convert.ToDecimal(reader["amount_spent"]).ToString("0.00"),
                                Convert.ToInt32(reader["points_earned"]),
                                reader["teller"] == DBNull.Value ? "" : reader["teller"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to load points history");
            }
        }

        private void txtAmountSpent_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtAmountSpent.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount) && amount > 0)
                txtPoints.Text = Math.Floor(amount / PointsRateCurrencyPerPoint).ToString(CultureInfo.InvariantCulture);
            else
                txtPoints.Text = "0";
        }

        private async void btnCreditPoints_Click(object sender, EventArgs e)
        {
            lblCreditStatus.Text = "";

            if (_selectedCreditCode == null)
            {
                lblCreditStatus.Text = "Find a customer before crediting points.";
                return;
            }

            decimal.TryParse(txtAmountSpent.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amountSpent);

            if (!int.TryParse(txtPoints.Text, out int points) || points <= 0)
            {
                lblCreditStatus.Text = "Enter a positive number of points to credit.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    using (var cmd = new MySqlCommand(
                        "INSERT INTO loyalty_transactions_(loyalty_code, amount_spent, points_earned, teller, date_credited) " +
                        "VALUES (@code, @amount, @points, @teller, @dateCredited)", conn))
                    {
                        cmd.Parameters.AddWithValue("@code", _selectedCreditCode);
                        cmd.Parameters.AddWithValue("@amount", amountSpent);
                        cmd.Parameters.AddWithValue("@points", points);
                        cmd.Parameters.AddWithValue("@teller", Purchases.log_in);
                        cmd.Parameters.AddWithValue("@dateCredited", DateTime.Now);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    using (var cmd = new MySqlCommand(
                        "UPDATE customers_ SET points = points + @points WHERE loyalty_code = @code", conn))
                    {
                        cmd.Parameters.AddWithValue("@points", points);
                        cmd.Parameters.AddWithValue("@code", _selectedCreditCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                lblCreditStatus.ForeColor = System.Drawing.Color.SeaGreen;
                lblCreditStatus.Text = $"Credited {points} point(s).";

                txtAmountSpent.Clear();
                txtPoints.Text = "0";

                await FindCustomerAsync();
                await LoadCustomersAsync();
            }
            catch (Exception ex)
            {
                lblCreditStatus.ForeColor = System.Drawing.Color.Firebrick;
                MessageBox.Show(ex.Message, "Failed to credit points");
            }
        }

        private void UpdateCardPreview(string loyaltyCode, string fullName, int points, DateTime? dateRegistered)
        {
            _previewLoyaltyCode = loyaltyCode;
            _previewFullName = fullName;
            _previewPoints = points;
            _previewDateRegistered = dateRegistered;

            picCardBarcode.ImageLocation = string.IsNullOrEmpty(loyaltyCode)
                ? null
                : "https://quickchart.io/barcode?type=code128&width=2&height=50&includeText=false" +
                  $"&text={Uri.EscapeDataString(loyaltyCode)}";

            pnlCardFront.Invalidate();
            pnlCardBack.Invalidate();
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void pnlCardFront_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = pnlCardFront.ClientRectangle;
            bounds.Width -= 1;
            bounds.Height -= 1;

            using (var path = RoundedRect(bounds, 14))
            using (var brush = new LinearGradientBrush(bounds, Color.FromArgb(28, 42, 82), Color.FromArgb(64, 105, 190), 45f))
            {
                g.FillPath(brush, path);
            }

            // Positions are fractions of the panel height so the layout still fits if
            // Windows Forms' font-based AutoScale shrinks or grows this control at runtime.
            float h = bounds.Height;
            const int marginX = 16;

            using (var titleFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (var labelFont = new Font("Segoe UI", 7.5F))
            using (var nameFont = new Font("Segoe UI", 13F, FontStyle.Bold))
            using (var codeFont = new Font("Consolas", 12F, FontStyle.Bold))
            using (var whiteBrush = new SolidBrush(Color.White))
            using (var lightBrush = new SolidBrush(Color.FromArgb(210, 220, 245)))
            {
                g.DrawString(StoreName.ToUpperInvariant(), titleFont, whiteBrush, marginX, h * 0.07f);
                g.DrawString("MEMBER CARD", labelFont, lightBrush, marginX, h * 0.18f);

                string name = string.IsNullOrWhiteSpace(_previewFullName) ? "MEMBER NAME" : _previewFullName.ToUpperInvariant();
                g.DrawString(name, nameFont, whiteBrush, marginX, h * 0.58f);

                string code = string.IsNullOrWhiteSpace(_previewLoyaltyCode) ? "----- -----" : _previewLoyaltyCode;
                g.DrawString(code, codeFont, whiteBrush, marginX, h * 0.74f);

                string since = _previewDateRegistered.HasValue ? _previewDateRegistered.Value.ToString("MMM yyyy") : "--";
                g.DrawString("MEMBER SINCE " + since, labelFont, lightBrush, marginX, h * 0.89f);
            }
        }

        private void pnlCardBack_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = pnlCardBack.ClientRectangle;
            bounds.Width -= 1;
            bounds.Height -= 1;

            using (var path = RoundedRect(bounds, 14))
            using (var brush = new SolidBrush(Color.FromArgb(245, 246, 250)))
            using (var borderPen = new Pen(Color.FromArgb(210, 213, 222)))
            {
                g.FillPath(brush, path);
                g.DrawPath(borderPen, path);
            }

            // Positions are fractions of the panel height so the layout still fits if
            // Windows Forms' font-based AutoScale shrinks or grows this control at runtime.
            float h = bounds.Height;

            using (var stripeBrush = new SolidBrush(Color.FromArgb(35, 35, 45)))
                g.FillRectangle(stripeBrush, 0, h * 0.084f, bounds.Width, h * 0.137f);

            using (var font = new Font("Segoe UI", 7.5F))
            using (var textBrush = new SolidBrush(Color.FromArgb(90, 95, 110)))
            {
                string code = _previewLoyaltyCode ?? "";
                var codeSize = g.MeasureString(code, font);
                g.DrawString(code, font, textBrush, (bounds.Width - codeSize.Width) / 2, h * 0.53f);

                string terms = "Present this card at checkout to earn and redeem points. " +
                    "Non-transferable. Lost cards may be reissued with proof of purchase.";
                g.DrawString(terms, font, textBrush, new RectangleF(14, h * 0.64f, bounds.Width - 28, h * 0.3f));

                g.DrawString(StoreName + " • Member Support", font, textBrush, 14, bounds.Height - 24);
            }
        }
    }
}
