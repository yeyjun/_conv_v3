using System;
using System.Configuration;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Blocking gate shown whenever app_config_'s MAINTENANCE_MODE flag is on - both at startup
    /// (from Program.Main, before the main form ever opens) and while the app is already running
    /// (from Purchases' periodic maintenance watcher, if the flag flips on mid-session). Closing
    /// with DialogResult.OK (via a correct system admin PIN) lets the caller continue into/stay
    /// in the app; anything else (Exit, or the window closed outright) is treated as declined.
    /// </summary>
    public partial class MaintenanceForm : Form
    {
        private const string DefaultMessage =
            "This store's system is currently under maintenance. Please check back shortly.";

        public MaintenanceForm(string message)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            lblMessage.Text = string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
        }

        private void btnAdminOverride_Click(object sender, EventArgs e)
        {
            using (var pinForm = new PinPromptForm())
            {
                pinForm.PromptText = "Enter system admin PIN to continue:";
                string adminPin = ConfigurationManager.AppSettings["ADMIN_PIN"];

                if (pinForm.ShowDialog(this) != DialogResult.OK || pinForm.EnteredPin != adminPin)
                {
                    lblStatus.Text = "Incorrect PIN.";
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
