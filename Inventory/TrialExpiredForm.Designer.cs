namespace Inventory
{
    partial class TrialExpiredForm
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
            this.lblLicenseKey = new System.Windows.Forms.Label();
            this.tbxLicenseKey = new System.Windows.Forms.TextBox();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(15, 15);
            this.lblMessage.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(340, 60);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Your trial has expired.";
            //
            // lblLicenseKey
            //
            this.lblLicenseKey.AutoSize = true;
            this.lblLicenseKey.Location = new System.Drawing.Point(15, 85);
            this.lblLicenseKey.Name = "lblLicenseKey";
            this.lblLicenseKey.Size = new System.Drawing.Size(69, 13);
            this.lblLicenseKey.TabIndex = 1;
            this.lblLicenseKey.Text = "License key:";
            //
            // tbxLicenseKey
            //
            this.tbxLicenseKey.Location = new System.Drawing.Point(15, 102);
            this.tbxLicenseKey.Name = "tbxLicenseKey";
            this.tbxLicenseKey.Size = new System.Drawing.Size(340, 22);
            this.tbxLicenseKey.TabIndex = 2;
            //
            // btnActivate
            //
            this.btnActivate.Location = new System.Drawing.Point(15, 134);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(160, 32);
            this.btnActivate.TabIndex = 3;
            this.btnActivate.Text = "Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            //
            // btnExit
            //
            this.btnExit.Location = new System.Drawing.Point(195, 134);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(160, 32);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Tomato;
            this.lblStatus.Location = new System.Drawing.Point(15, 174);
            this.lblStatus.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 5;
            //
            // TrialExpiredForm
            //
            this.AcceptButton = this.btnActivate;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(370, 202);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.tbxLicenseKey);
            this.Controls.Add(this.lblLicenseKey);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrialExpiredForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trial Expired";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblLicenseKey;
        private System.Windows.Forms.TextBox tbxLicenseKey;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblStatus;
    }
}
