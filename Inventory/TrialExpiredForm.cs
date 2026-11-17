using System;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Blocking gate shown from Program.Main before the main form ever opens, once
    /// TrialLicense.IsExpired is true. Closing with DialogResult.OK (via Activate) lets startup
    /// continue into the app; anything else (Exit, or the window closed outright) ends the process.
    /// </summary>
    public partial class TrialExpiredForm : Form
    {
        public TrialExpiredForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            lblMessage.Text =
                $"Your trial of App Inventory ended {TrialLicense.TrialDays} day(s) after first use. " +
                "Enter the license key issued for this store to keep using the app, or exit.";
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (TrialLicense.TryActivate(tbxLicenseKey.Text))
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            lblStatus.Text = "That license key isn't valid for this store.";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
