namespace RemoteController
{
    partial class MenuControlForm
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

        private void InitializeComponent()
        {
            this.lblHint = new System.Windows.Forms.Label();
            this.clbMenus = new System.Windows.Forms.CheckedListBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblHint
            //
            this.lblHint.AutoSize = true;
            this.lblHint.Location = new System.Drawing.Point(12, 12);
            this.lblHint.MaximumSize = new System.Drawing.Size(290, 0);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(290, 39);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "Check the menus that should be visible on this terminal, then click Apply. Fetc" +
    "hing current state...";
            //
            // clbMenus
            //
            this.clbMenus.CheckOnClick = true;
            this.clbMenus.FormattingEnabled = true;
            this.clbMenus.Location = new System.Drawing.Point(12, 60);
            this.clbMenus.Name = "clbMenus";
            this.clbMenus.Size = new System.Drawing.Size(290, 214);
            this.clbMenus.TabIndex = 1;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(12, 282);
            this.lblStatus.MaximumSize = new System.Drawing.Size(290, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 2;
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(12, 315);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnApply
            //
            this.btnApply.Location = new System.Drawing.Point(112, 315);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 28);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(212, 315);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 28);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // MenuControlForm
            //
            this.AcceptButton = this.btnApply;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(314, 355);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.clbMenus);
            this.Controls.Add(this.lblHint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Menus";
            this.Load += new System.EventHandler(this.MenuControlForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.CheckedListBox clbMenus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
    }
}
