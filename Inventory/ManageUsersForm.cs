using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
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
                using (var conn = new MySqlConnection(DbConnectionHelper.BuildConnectionString()))
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
                ErrorDialog.Show(this, "Could not load the user list.", "Failed to Load Users", ex);
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
                using (var conn = new MySqlConnection(DbConnectionHelper.BuildConnectionString()))
                {
                    await conn.OpenAsync();

                    bool isNew = _originalCode == null;
                    if (isNew)
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

                    if (isNew)
                        OperationLog.RecordCreate(nameof(ManageUsersForm), $"User '{code}' ({fullName}) created.");
                    else
                        OperationLog.RecordUpdate(nameof(ManageUsersForm), $"User '{_originalCode}' updated to '{code}' ({fullName}).");
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
                ErrorDialog.Show(this, "Could not save this user.", "Failed to Save User", ex);
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
                ErrorDialog.Show(this, "Cannot delete the last remaining login - this would lock everyone out.", "Not allowed",
                    $"usersTable.Rows.Count={usersTable.Rows.Count}", MessageBoxIcon.Warning);
                return;
            }

            var confirm = ConfirmDialog.Show(this, $"Delete login '{_originalCode}'? This cannot be undone. (Y/N)",
                "Confirm delete", MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (var conn = new MySqlConnection(DbConnectionHelper.BuildConnectionString()))
                using (var cmd = new MySqlCommand("DELETE FROM user_ WHERE name=@name", conn))
                {
                    cmd.Parameters.AddWithValue("@name", _originalCode);
                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }

                OperationLog.RecordDelete(nameof(ManageUsersForm), $"User '{_originalCode}' deleted.");
                await RefreshUsersCacheAsync();
                await LoadUsersAsync();
                btnNewUser_Click(sender, e);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(this, "Could not delete this user.", "Failed to Delete User", ex);
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
