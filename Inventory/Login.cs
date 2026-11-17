using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourApp.Data;

namespace Inventory
{

    public partial class Login : Form
    {
        public string id = null;
        public string name = null;
        public bool IsAuthenticated { get; private set; } = false;
        public MySqlConnection connect_db;
        public Login()
        {
            InitializeComponent();
            LogoCache.ApplyTo(this);
            AppTheme.ApplyTo(this);
            AppFontScale.ApplyTo(this);
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
        public async Task con()
        {
            try
            {
                string dbsrv = ConfigurationManager.AppSettings["DB_SERVER"];
                string dbname = ConfigurationManager.AppSettings["DB_NAME"];
                string dbuser = ConfigurationManager.AppSettings["DB_USER"];
                string dbpw = ConfigurationManager.AppSettings["DB_PASSWORD"];
                string connectionTimeout = ConfigurationManager.AppSettings["CONNECTION_TIMEOUT"];
                connect_db = new MySqlConnection($"Server={dbsrv};user id={dbuser};password={dbpw};database={dbname};Connect Timeout={connectionTimeout};");

                await connect_db.OpenAsync();

            }
            catch (Exception)
            {
                // No network right now - callers treat a closed/absent connection as "couldn't reach the DB".
            }
            return;
        }
        private void tbx_login_Validated(object sender, EventArgs e)
        {

        }

        private void tbx_login_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string enteredCode = tbx_login.Text;
                    if (string.IsNullOrWhiteSpace(enteredCode))
                        return;

                    DataRow match = FindUserRow(enteredCode);
                    if (match == null)
                    {
                        try { DataCacheManager.Instance.RefreshTable("Users", "SELECT name, fullname FROM user_"); }
                        catch { }
                        match = FindUserRow(enteredCode);
                    }
                    else
                    {
                        _ = Task.Run(() =>
                        {
                            try { DataCacheManager.Instance.RefreshTable("Users", "SELECT name, fullname FROM user_"); }
                            catch { }
                        });
                    }

                    if (match == null)
                    {
                        OperationLog.RecordLogin(nameof(Login), $"Login attempt with unrecognized code '{enteredCode}' failed.");
                        MessageBox.Show("Login code not recognized.", "Login Failed");
                        tbx_login.Clear();
                        return;
                    }

                    id = match["fullname"].ToString();
                    name = enteredCode;
                    IsAuthenticated = true;

                    tbx_login.Enabled = false;
                    tbx_login.Clear();
                    this.Close();

                    string offlineNote = DataCacheManager.Instance.IsOfflineMode ? " (offline)" : "";
                    string logText = $"User '{enteredCode}' logged in{offlineNote}.";
                    _ = ActivityLog.LogAsync(logText);
                    OperationLog.RecordLogin(nameof(Login), logText);
                    _ = TryStampLastSeenAsync(enteredCode);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Application.Exit();
                }
            }
            catch (OperationCanceledException oce)
            {
                // A background DB call (activity log / sales-token stamp) timed out or was
                // cancelled - treat it like any other offline hiccup rather than surfacing the
                // raw "task was canceled" message, and leave the field ready to retry.
                ErrorDialog.Show(this, "The login check timed out. Check your connection and try again.", "Login Timed Out", oce,
                    MessageBoxIcon.Warning);
                tbx_login.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message, "Login Error");
            }
        }

        private static DataRow FindUserRow(string enteredCode) =>
            DataCacheManager.Instance.GetTable("Users").AsEnumerable()
                .FirstOrDefault(r => string.Equals(r.Field<string>("name"), enteredCode, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Best-effort: records the last-activity timestamp (UTC) in the database. Never blocks
        /// sign-in - the user is already authenticated against the cached credentials by the
        /// time this runs.
        /// </summary>
        private async Task TryStampLastSeenAsync(string name)
        {
            try
            {
                await con();
                if (connect_db == null || connect_db.State != ConnectionState.Open)
                    return;

                using (var cmd = new MySqlCommand("UPDATE user_ SET last_seen_utc=@last_seen_utc WHERE name=@name", connect_db))
                {
                    cmd.Parameters.AddWithValue("@last_seen_utc", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@name", name);
                    await cmd.ExecuteNonQueryAsync();
                }
                connect_db.Close();
            }
            catch
            {
                // Offline or otherwise unreachable - the login itself already succeeded.
            }
        }
        public string teller_id(string id_here)
        {
            id = id_here;
            return id;
        }
        private void Login_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void tbx_login_TextChanged(object sender, EventArgs e)
        {
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
