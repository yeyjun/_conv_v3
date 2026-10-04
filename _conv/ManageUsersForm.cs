using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace _conv
{
    public partial class ManageUsersForm : Form
    {
        private readonly DataTable usersTable = BuildUsersTable();
        private string _originalCode = null;

        public ManageUsersForm()
        {
            InitializeComponent();
            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);
        }

        private static DataTable BuildUsersTable()
        {
            var table = new DataTable();
            table.Columns.Add("Login Code", typeof(string));
            table.Columns.Add("Full Name", typeof(string));
            table.Columns.Add("Last Activity", typeof(string));
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

        private async void ManageUsersForm_Load(object sender, EventArgs e)
        {
            dgvUsers.AutoGenerateColumns = true;
            dgvUsers.DataSource = usersTable;
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            usersTable.Rows.Clear();
            lblStatus.Text = "";

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand("SELECT name, fullname, last_seen_utc FROM user_ ORDER BY fullname", conn))
                {
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // last_seen_utc is NULL for accounts that have never logged in.
                            // It's stored in UTC (see Login.cs) so convert to local time for display.
                            string lastActivity = "";
                            if (reader["last_seen_utc"] != DBNull.Value)
                            {
                                DateTime lastSeenUtc = DateTime.SpecifyKind(Convert.ToDateTime(reader["last_seen_utc"]), DateTimeKind.Utc);
                                lastActivity = lastSeenUtc.ToLocalTime().ToString("g");
                            }

                            usersTable.Rows.Add(
                                reader["name"]?.ToString(),
                                reader["fullname"]?.ToString(),
                                lastActivity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to load users");
            }
        }

        private static async Task RefreshUsersCacheAsync()
        {
            try
            {
                await Task.Run(() => DataCacheManager.Instance.RefreshTable("Users", "SELECT name, fullname FROM user_"));
            }
            catch
            {
                // Best-effort - the live grid here already reflects the change; the shared
                // cache will pick it up next time it's able to reach the database.
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow?.DataBoundItem is DataRowView row)
            {
                _originalCode = row["Login Code"].ToString();
                txtCode.Text = _originalCode;
                txtFullName.Text = row["Full Name"].ToString();
                lblStatus.Text = "";
            }
        }

        private void btnNewUser_Click(object sender, EventArgs e)
        {
            dgvUsers.ClearSelection();
            _originalCode = null;
            txtCode.Clear();
            txtFullName.Clear();
            lblStatus.Text = "";
            txtCode.Focus();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(fullName))
            {
                lblStatus.Text = "Login code and full name are both required.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    if (_originalCode == null)
                    {
                        using (var cmd = new MySqlCommand("INSERT INTO user_(name, fullname) VALUES (@name, @fullname)", conn))
                        {
                            cmd.Parameters.AddWithValue("@name", code);
                            cmd.Parameters.AddWithValue("@fullname", fullName);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    else
                    {
                        using (var cmd = new MySqlCommand("UPDATE user_ SET name=@name, fullname=@fullname WHERE name=@oldName", conn))
                        {
                            cmd.Parameters.AddWithValue("@name", code);
                            cmd.Parameters.AddWithValue("@fullname", fullName);
                            cmd.Parameters.AddWithValue("@oldName", _originalCode);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await RefreshUsersCacheAsync();
                await LoadUsersAsync();
                btnNewUser_Click(sender, e);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                lblStatus.Text = $"'{code}' is already in use by another user.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to save user");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_originalCode == null)
            {
                lblStatus.Text = "Select a user to delete.";
                return;
            }

            if (usersTable.Rows.Count <= 1)
            {
                MessageBox.Show("Cannot delete the last remaining login - this would lock everyone out.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(this, $"Delete login '{_originalCode}'? This cannot be undone.",
                "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(BuildConnectionString()))
                using (var cmd = new MySqlCommand("DELETE FROM user_ WHERE name=@name", conn))
                {
                    cmd.Parameters.AddWithValue("@name", _originalCode);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                await RefreshUsersCacheAsync();
                await LoadUsersAsync();
                btnNewUser_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to delete user");
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void ManageUsersForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                await LoadUsersAsync();
            }
        }
    }
}
