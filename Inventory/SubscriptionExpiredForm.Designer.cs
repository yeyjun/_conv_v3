namespace _conv
{
    partial class SubscriptionExpiredForm
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
            this.btnAdminOverride = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(15, 15);
            this.lblMessage.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(340, 80);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "No active subscription token was found for this store this month. Contact suppo" +
    "rt to renew, or exit.";
            //
            // btnAdminOverride
            //
            this.btnAdminOverride.Location = new System.Drawing.Point(15, 104);
            this.btnAdminOverride.Name = "btnAdminOverride";
            this.btnAdminOverride.Size = new System.Drawing.Size(160, 32);
            this.btnAdminOverride.TabIndex = 1;
            this.btnAdminOverride.Text = "Admin Override";
            this.btnAdminOverride.UseVisualStyleBackColor = true;
            this.btnAdminOverride.Click += new System.EventHandler(this.btnAdminOverride_Click);
            //
            // btnExit
            //
            this.btnExit.Location = new System.Drawing.Point(195, 104);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(160, 32);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Tomato;
            this.lblStatus.Location = new System.Drawing.Point(15, 144);
            this.lblStatus.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 3;
            //
            // SubscriptionExpiredForm
            //
            this.AcceptButton = this.btnAdminOverride;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(370, 176);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnAdminOverride);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubscriptionExpiredForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subscription Required";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnAdminOverride;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblStatus;
    }
}
