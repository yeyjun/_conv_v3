namespace InfraMonitor
{
    partial class MainForm
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

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageSystemHealth;
        private System.Windows.Forms.TabPage tabPageStorageWatch;
        private System.Windows.Forms.TabPage tabPageCpuHealth;
        private System.Windows.Forms.TabPage tabPageIoEvents;

        private void InitializeComponent()
        {
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageSystemHealth = new System.Windows.Forms.TabPage();
            this.tabPageStorageWatch = new System.Windows.Forms.TabPage();
            this.tabPageCpuHealth = new System.Windows.Forms.TabPage();
            this.tabPageIoEvents = new System.Windows.Forms.TabPage();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            //
            // tabControlMain
            //
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Controls.Add(this.tabPageSystemHealth);
            this.tabControlMain.Controls.Add(this.tabPageStorageWatch);
            this.tabControlMain.Controls.Add(this.tabPageCpuHealth);
            this.tabControlMain.Controls.Add(this.tabPageIoEvents);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            //
            // tabPageSystemHealth
            //
            this.tabPageSystemHealth.Name = "tabPageSystemHealth";
            this.tabPageSystemHealth.Text = "System Health";
            this.tabPageSystemHealth.UseVisualStyleBackColor = true;
            //
            // tabPageStorageWatch
            //
            this.tabPageStorageWatch.Name = "tabPageStorageWatch";
            this.tabPageStorageWatch.Text = "Storage Watch";
            this.tabPageStorageWatch.UseVisualStyleBackColor = true;
            //
            // tabPageCpuHealth
            //
            this.tabPageCpuHealth.Name = "tabPageCpuHealth";
            this.tabPageCpuHealth.Text = "CPU Health";
            this.tabPageCpuHealth.UseVisualStyleBackColor = true;
            //
            // tabPageIoEvents
            //
            this.tabPageIoEvents.Name = "tabPageIoEvents";
            this.tabPageIoEvents.Text = "I/O Errors && Warnings";
            this.tabPageIoEvents.UseVisualStyleBackColor = true;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.tabControlMain);
            this.MinimumSize = new System.Drawing.Size(700, 480);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Baclig Minimart - Infrastructure Monitor";
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
