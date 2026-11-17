using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace Inventory
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            tbxStoreName.Text = ConfigurationManager.AppSettings["STORE_NAME"];
            tbxStoreCode.Text = ConfigurationManager.AppSettings["STORE_CODE"];
            tbxStoreEmail.Text = ConfigurationManager.AppSettings["STORE_EMAIL"];
            cmbSubscriptionTier.SelectedIndex = 0;
        }

        private string SelectedTierLicenseDirectory()
        {
            string tier = cmbSubscriptionTier.SelectedItem as string ?? "Basic";
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "App Inventory", "Licenses", tier);
            try { Directory.CreateDirectory(dir); } catch { }
            return dir;
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxLicenseKey.Text))
            {
                ShowStatus("Enter a license key, use Import License File, or click Continue With Trial.", isError: true);
                return;
            }

            if (!SaveStoreInfo())
                return;

            if (!TrialLicense.TryActivate(tbxLicenseKey.Text))
            {
                ShowStatus("That license key isn't valid for this store code.", isError: true);
                return;
            }

            FinishRegistration();
        }

        private void btnImportFile_Click(object sender, EventArgs e)
        {
            openFileDialogLicense.InitialDirectory = SelectedTierLicenseDirectory();
            if (openFileDialogLicense.ShowDialog(this) != DialogResult.OK)
                return;

            if (!SaveStoreInfo())
                return;

            if (!TrialLicense.TryActivateFromFile(openFileDialogLicense.FileName, out string error))
            {
                ShowStatus(error, isError: true);
                return;
            }

            FinishRegistration();
        }

        private void btnContinueTrial_Click(object sender, EventArgs e)
        {
            if (!SaveStoreInfo())
                return;

            FinishRegistration();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool SaveStoreInfo()
        {
            if (string.IsNullOrWhiteSpace(tbxStoreName.Text) || string.IsNullOrWhiteSpace(tbxStoreCode.Text))
            {
                ShowStatus("Store Name and Store Code are required.", isError: true);
                return false;
            }

            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                SetSetting(config, "STORE_NAME", tbxStoreName.Text.Trim());
                SetSetting(config, "STORE_CODE", tbxStoreCode.Text.Trim());
                SetSetting(config, "STORE_EMAIL", tbxStoreEmail.Text.Trim());
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                ShowStatus($"Could not save store info: {ex.Message}", isError: true);
                return false;
            }
        }

        private void FinishRegistration()
        {
            TrialLicense.MarkRegistered();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowStatus(string text, bool isError)
        {
            lblStatus.ForeColor = isError ? System.Drawing.Color.Tomato : System.Drawing.Color.Teal;
            lblStatus.Text = text;
        }

        private static void SetSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }
    }
}
