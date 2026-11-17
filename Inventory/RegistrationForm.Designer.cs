namespace Inventory
{
    partial class RegistrationForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblStoreName = new System.Windows.Forms.Label();
            this.tbxStoreName = new System.Windows.Forms.TextBox();
            this.lblStoreCode = new System.Windows.Forms.Label();
            this.tbxStoreCode = new System.Windows.Forms.TextBox();
            this.lblStoreEmail = new System.Windows.Forms.Label();
            this.tbxStoreEmail = new System.Windows.Forms.TextBox();
            this.lblLicenseKey = new System.Windows.Forms.Label();
            this.tbxLicenseKey = new System.Windows.Forms.TextBox();
            this.lblSubscriptionTier = new System.Windows.Forms.Label();
            this.cmbSubscriptionTier = new System.Windows.Forms.ComboBox();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnImportFile = new System.Windows.Forms.Button();
            this.btnContinueTrial = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.openFileDialogLicense = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(15, 15);
            this.lblMessage.MaximumSize = new System.Drawing.Size(390, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(390, 40);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Register this installation for full usage. Enter a license key for immediate full access, or continue on the trial.";
            //
            // lblStoreName
            //
            this.lblStoreName.AutoSize = true;
            this.lblStoreName.Location = new System.Drawing.Point(15, 65);
            this.lblStoreName.Name = "lblStoreName";
            this.lblStoreName.Size = new System.Drawing.Size(66, 13);
            this.lblStoreName.TabIndex = 1;
            this.lblStoreName.Text = "Store Name:";
            //
            // tbxStoreName
            //
            this.tbxStoreName.Location = new System.Drawing.Point(15, 82);
            this.tbxStoreName.Name = "tbxStoreName";
            this.tbxStoreName.Size = new System.Drawing.Size(390, 22);
            this.tbxStoreName.TabIndex = 2;
            //
            // lblStoreCode
            //
            this.lblStoreCode.AutoSize = true;
            this.lblStoreCode.Location = new System.Drawing.Point(15, 115);
            this.lblStoreCode.Name = "lblStoreCode";
            this.lblStoreCode.Size = new System.Drawing.Size(65, 13);
            this.lblStoreCode.TabIndex = 3;
            this.lblStoreCode.Text = "Store Code:";
            //
            // tbxStoreCode
            //
            this.tbxStoreCode.Location = new System.Drawing.Point(15, 132);
            this.tbxStoreCode.Name = "tbxStoreCode";
            this.tbxStoreCode.Size = new System.Drawing.Size(390, 22);
            this.tbxStoreCode.TabIndex = 4;
            //
            // lblStoreEmail
            //
            this.lblStoreEmail.AutoSize = true;
            this.lblStoreEmail.Location = new System.Drawing.Point(15, 165);
            this.lblStoreEmail.Name = "lblStoreEmail";
            this.lblStoreEmail.Size = new System.Drawing.Size(69, 13);
            this.lblStoreEmail.TabIndex = 5;
            this.lblStoreEmail.Text = "Store Email:";
            //
            // tbxStoreEmail
            //
            this.tbxStoreEmail.Location = new System.Drawing.Point(15, 182);
            this.tbxStoreEmail.Name = "tbxStoreEmail";
            this.tbxStoreEmail.Size = new System.Drawing.Size(390, 22);
            this.tbxStoreEmail.TabIndex = 6;
            //
            // lblLicenseKey
            //
            this.lblLicenseKey.AutoSize = true;
            this.lblLicenseKey.Location = new System.Drawing.Point(15, 215);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(69, 13);
            this.lblLicenseKey.TabIndex = 7;
            this.lblLicenseKey.Text = "License key:";
            //
            // tbxLicenseKey
            //
            this.tbxLicenseKey.Location = new System.Drawing.Point(15, 232);
            this.tbxLicenseKey.Name = "tbxLicenseKey";
            this.tbxLicenseKey.Size = new System.Drawing.Size(390, 22);
            this.tbxLicenseKey.TabIndex = 8;
            //
            // lblSubscriptionTier
            //
            this.lblSubscriptionTier.AutoSize = true;
            this.lblSubscriptionTier.Location = new System.Drawing.Point(15, 268);
            this.lblSubscriptionTier.Name = "lblSubscriptionTier";
            this.lblSubscriptionTier.Size = new System.Drawing.Size(96, 13);
            this.lblSubscriptionTier.TabIndex = 9;
            this.lblSubscriptionTier.Text = "Subscription Tier:";
            //
            // cmbSubscriptionTier
            //
            // Only used to pick which Documents\App Inventory\Licenses\<Tier> subfolder
            // btnImportFile_Click opens into below - the tier itself comes from the imported
            // file's own Tier field (see TrialLicense.TryActivateFromFile), not this combo.
            this.cmbSubscriptionTier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubscriptionTier.Items.AddRange(new object[] { "Basic", "Pro", "Enterprise" });
            this.cmbSubscriptionTier.Location = new System.Drawing.Point(130, 264);
            this.cmbSubscriptionTier.Name = "cmbSubscriptionTier";
            this.cmbSubscriptionTier.Size = new System.Drawing.Size(150, 23);
            this.cmbSubscriptionTier.TabIndex = 10;
            //
            // btnActivate
            //
            this.btnActivate.Location = new System.Drawing.Point(15, 300);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(190, 32);
            this.btnActivate.TabIndex = 11;
            this.btnActivate.Text = "Activate License";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            //
            // btnImportFile
            //
            this.btnImportFile.Location = new System.Drawing.Point(215, 300);
            this.btnImportFile.Name = "btnImportFile";
            this.btnImportFile.Size = new System.Drawing.Size(190, 32);
            this.btnImportFile.TabIndex = 12;
            this.btnImportFile.Text = "Import License File...";
            this.btnImportFile.UseVisualStyleBackColor = true;
            this.btnImportFile.Click += new System.EventHandler(this.btnImportFile_Click);
            //
            // btnContinueTrial
            //
            this.btnContinueTrial.Location = new System.Drawing.Point(15, 340);
            this.btnContinueTrial.Name = "btnContinueTrial";
            this.btnContinueTrial.Size = new System.Drawing.Size(190, 32);
            this.btnContinueTrial.TabIndex = 13;
            this.btnContinueTrial.Text = "Continue With Trial";
            this.btnContinueTrial.UseVisualStyleBackColor = true;
            this.btnContinueTrial.Click += new System.EventHandler(this.btnContinueTrial_Click);
            //
            // btnExit
            //
            this.btnExit.Location = new System.Drawing.Point(215, 340);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(190, 32);
            this.btnExit.TabIndex = 14;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Tomato;
            this.lblStatus.Location = new System.Drawing.Point(15, 380);
            this.lblStatus.MaximumSize = new System.Drawing.Size(390, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 15;
            //
            // openFileDialogLicense
            //
            this.openFileDialogLicense.DefaultExt = "tmrgo";
            this.openFileDialogLicense.Filter = "License Key File (*.tmrgo)|*.tmrgo|All files (*.*)|*.*";
            this.openFileDialogLicense.Title = "Choose License Key File";
            //
            // RegistrationForm
            //
            this.AcceptButton = this.btnActivate;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(420, 412);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnContinueTrial);
            this.Controls.Add(this.btnImportFile);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.cmbSubscriptionTier);
            this.Controls.Add(this.lblSubscriptionTier);
            this.Controls.Add(this.tbxLicenseKey);
            this.Controls.Add(this.lblLicenseKey);
            this.Controls.Add(this.tbxStoreEmail);
            this.Controls.Add(this.lblStoreEmail);
            this.Controls.Add(this.tbxStoreCode);
            this.Controls.Add(this.lblStoreCode);
            this.Controls.Add(this.tbxStoreName);
            this.Controls.Add(this.lblStoreName);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register This Installation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblStoreName;
        private System.Windows.Forms.TextBox tbxStoreName;
        private System.Windows.Forms.Label lblStoreCode;
        private System.Windows.Forms.TextBox tbxStoreCode;
        private System.Windows.Forms.Label lblStoreEmail;
        private System.Windows.Forms.TextBox tbxStoreEmail;
        private System.Windows.Forms.Label lblLicenseKey;
        private System.Windows.Forms.TextBox tbxLicenseKey;
        private System.Windows.Forms.Label lblSubscriptionTier;
        private System.Windows.Forms.ComboBox cmbSubscriptionTier;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnImportFile;
        private System.Windows.Forms.Button btnContinueTrial;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.OpenFileDialog openFileDialogLicense;
    }
}
