using System;
using System.Configuration;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Blocking gate shown from Program.Main before the login screen ever opens, when
    /// SubscriptionTokenService.IsActiveAsync() comes back false - either no token was issued for
    /// this store this month, or the token server couldn't be reached (this check fails closed;
    /// see SubscriptionTokenService remarks). Closing with DialogResult.OK (via a correct system
    /// admin PIN) lets startup continue into the app; anything else ends the process.
    /// </summary>
    public partial class SubscriptionExpiredForm : Form
    {
        public SubscriptionExpiredForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
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
