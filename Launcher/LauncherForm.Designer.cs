namespace Launcher
{
    partial class LauncherForm
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

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnRemittance;
        private System.Windows.Forms.Button btnInfraMonitor;

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnRemittance = new System.Windows.Forms.Button();
            this.btnInfraMonitor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(30, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(250, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Baclig Minimart";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblSubtitle.Location = new System.Drawing.Point(32, 62);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(140, 15);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Choose an app to open";
            //
            // btnInventory
            //
            this.btnInventory.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnInventory.Location = new System.Drawing.Point(30, 100);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(340, 70);
            this.btnInventory.TabIndex = 2;
            this.btnInventory.Text = "Inventory\r\n(Point of Sale)";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            //
            // btnRemittance
            //
            this.btnRemittance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRemittance.Location = new System.Drawing.Point(30, 182);
            this.btnRemittance.Name = "btnRemittance";
            this.btnRemittance.Size = new System.Drawing.Size(340, 70);
            this.btnRemittance.TabIndex = 3;
            this.btnRemittance.Text = "Remittance\r\n(Send Money)";
            this.btnRemittance.UseVisualStyleBackColor = true;
            this.btnRemittance.Click += new System.EventHandler(this.btnRemittance_Click);
            //
            // btnInfraMonitor
            //
            this.btnInfraMonitor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnInfraMonitor.Location = new System.Drawing.Point(30, 264);
            this.btnInfraMonitor.Name = "btnInfraMonitor";
            this.btnInfraMonitor.Size = new System.Drawing.Size(340, 70);
            this.btnInfraMonitor.TabIndex = 4;
            this.btnInfraMonitor.Text = "Infra Monitor\r\n(System Health)";
            this.btnInfraMonitor.UseVisualStyleBackColor = true;
            this.btnInfraMonitor.Click += new System.EventHandler(this.btnInfraMonitor_Click);
            //
            // LauncherForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 362);
            this.Controls.Add(this.btnInfraMonitor);
            this.Controls.Add(this.btnRemittance);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LauncherForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Baclig Minimart - Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
