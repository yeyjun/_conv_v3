namespace Inventory
{
    partial class ReceiptPreviewForm
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

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.CheckBox chkPriceOverride;
        private System.Windows.Forms.TrackBar tbPriceOverride;
        private System.Windows.Forms.Label lblPriceOverrideValue;
        private System.Windows.Forms.Label lblPaperWidth;
        private System.Windows.Forms.ComboBox cbxPaperWidth;
        private System.Windows.Forms.CheckBox chkPaperWidthIsDefault;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlScroll;
        private DoubleBufferedPanel pnlPaper;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblSaveStatus;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkPaperWidthIsDefault = new System.Windows.Forms.CheckBox();
            this.cbxPaperWidth = new System.Windows.Forms.ComboBox();
            this.lblPaperWidth = new System.Windows.Forms.Label();
            this.lblPriceOverrideValue = new System.Windows.Forms.Label();
            this.tbPriceOverride = new System.Windows.Forms.TrackBar();
            this.chkPriceOverride = new System.Windows.Forms.CheckBox();
            this.lblHint = new System.Windows.Forms.Label();
            this.pnlScroll = new System.Windows.Forms.Panel();
            this.pnlPaper = new DoubleBufferedPanel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblSaveStatus = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPriceOverride)).BeginInit();
            this.pnlScroll.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.BackColor = System.Drawing.Color.FloralWhite;
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.chkPaperWidthIsDefault);
            this.pnlTop.Controls.Add(this.cbxPaperWidth);
            this.pnlTop.Controls.Add(this.lblPaperWidth);
            this.pnlTop.Controls.Add(this.lblPriceOverrideValue);
            this.pnlTop.Controls.Add(this.tbPriceOverride);
            this.pnlTop.Controls.Add(this.chkPriceOverride);
            this.pnlTop.Controls.Add(this.lblHint);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(640, 156);
            this.pnlTop.TabIndex = 0;
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(500, 76);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 28);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // cbxPaperWidth
            //
            this.cbxPaperWidth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPaperWidth.FormattingEnabled = true;
            this.cbxPaperWidth.Items.AddRange(new object[] {
            "Automatic (use configured printer)",
            "80mm",
            "58mm"});
            this.cbxPaperWidth.Location = new System.Drawing.Point(90, 78);
            this.cbxPaperWidth.Name = "cbxPaperWidth";
            this.cbxPaperWidth.Size = new System.Drawing.Size(390, 21);
            this.cbxPaperWidth.TabIndex = 5;
            this.cbxPaperWidth.SelectedIndexChanged += new System.EventHandler(this.cbxPaperWidth_SelectedIndexChanged);
            //
            // chkPaperWidthIsDefault
            //
            this.chkPaperWidthIsDefault.AutoSize = true;
            this.chkPaperWidthIsDefault.Location = new System.Drawing.Point(90, 105);
            this.chkPaperWidthIsDefault.Name = "chkPaperWidthIsDefault";
            this.chkPaperWidthIsDefault.Size = new System.Drawing.Size(184, 17);
            this.chkPaperWidthIsDefault.TabIndex = 7;
            this.chkPaperWidthIsDefault.Text = "Set as default paper width";
            this.chkPaperWidthIsDefault.UseVisualStyleBackColor = true;
            //
            // lblPaperWidth
            //
            this.lblPaperWidth.AutoSize = true;
            this.lblPaperWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular);
            this.lblPaperWidth.Location = new System.Drawing.Point(12, 81);
            this.lblPaperWidth.Name = "lblPaperWidth";
            this.lblPaperWidth.Size = new System.Drawing.Size(75, 13);
            this.lblPaperWidth.TabIndex = 4;
            this.lblPaperWidth.Text = "Paper Width:";
            //
            // lblPriceOverrideValue
            //
            this.lblPriceOverrideValue.AutoSize = true;
            this.lblPriceOverrideValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPriceOverrideValue.Location = new System.Drawing.Point(330, 34);
            this.lblPriceOverrideValue.Name = "lblPriceOverrideValue";
            this.lblPriceOverrideValue.Size = new System.Drawing.Size(120, 13);
            this.lblPriceOverrideValue.TabIndex = 3;
            this.lblPriceOverrideValue.Text = "Override price: 1.00";
            //
            // tbPriceOverride
            //
            this.tbPriceOverride.LargeChange = 10000;
            this.tbPriceOverride.Location = new System.Drawing.Point(12, 28);
            this.tbPriceOverride.Maximum = 999999;
            this.tbPriceOverride.Minimum = 1;
            this.tbPriceOverride.Name = "tbPriceOverride";
            this.tbPriceOverride.Size = new System.Drawing.Size(300, 45);
            this.tbPriceOverride.SmallChange = 1;
            this.tbPriceOverride.TabIndex = 2;
            this.tbPriceOverride.TickFrequency = 50000;
            this.tbPriceOverride.Value = 1;
            this.tbPriceOverride.Scroll += new System.EventHandler(this.tbPriceOverride_Scroll);
            //
            // chkPriceOverride
            //
            this.chkPriceOverride.AutoSize = true;
            this.chkPriceOverride.Location = new System.Drawing.Point(12, 6);
            this.chkPriceOverride.Name = "chkPriceOverride";
            this.chkPriceOverride.Size = new System.Drawing.Size(230, 17);
            this.chkPriceOverride.TabIndex = 1;
            this.chkPriceOverride.Text = "Override printed item price for testing";
            this.chkPriceOverride.UseVisualStyleBackColor = true;
            this.chkPriceOverride.CheckedChanged += new System.EventHandler(this.chkPriceOverride_CheckedChanged);
            //
            // lblHint
            //
            this.lblHint.AutoSize = true;
            this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblHint.ForeColor = System.Drawing.Color.Gray;
            this.lblHint.Location = new System.Drawing.Point(12, 130);
            this.lblHint.MaximumSize = new System.Drawing.Size(610, 0);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(400, 12);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "Uses the current sale\'s items, or sample items if the cart is empty. Drag the b" +
    "lue handles on the paper below to adjust its margins. Nothing here affects re" +
    "al printing until you click Save.";
            //
            // pnlScroll
            //
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlScroll.Controls.Add(this.pnlPaper);
            this.pnlScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScroll.Location = new System.Drawing.Point(0, 156);
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Padding = new System.Windows.Forms.Padding(16);
            this.pnlScroll.Size = new System.Drawing.Size(640, 404);
            this.pnlScroll.TabIndex = 1;
            //
            // pnlPaper
            //
            this.pnlPaper.BackColor = System.Drawing.Color.White;
            this.pnlPaper.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlPaper.Location = new System.Drawing.Point(16, 16);
            this.pnlPaper.Name = "pnlPaper";
            this.pnlPaper.Size = new System.Drawing.Size(200, 100);
            this.pnlPaper.TabIndex = 0;
            this.pnlPaper.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPaper_Paint);
            this.pnlPaper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlPaper_MouseDown);
            this.pnlPaper.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlPaper_MouseMove);
            this.pnlPaper.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlPaper_MouseUp);
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Controls.Add(this.lblSaveStatus);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 560);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(640, 44);
            this.pnlBottom.TabIndex = 2;
            //
            // lblSaveStatus
            //
            this.lblSaveStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSaveStatus.AutoSize = true;
            this.lblSaveStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblSaveStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblSaveStatus.Location = new System.Drawing.Point(12, 15);
            this.lblSaveStatus.MaximumSize = new System.Drawing.Size(380, 0);
            this.lblSaveStatus.Name = "lblSaveStatus";
            this.lblSaveStatus.Size = new System.Drawing.Size(0, 12);
            this.lblSaveStatus.TabIndex = 2;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(408, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 32);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(524, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // ReceiptPreviewForm
            //
            this.AcceptButton = this.btnClose;
            this.BackColor = System.Drawing.Color.FloralWhite;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(640, 604);
            this.Controls.Add(this.pnlScroll);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.MinimizeBox = false;
            this.Name = "ReceiptPreviewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Receipt Preview";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReceiptPreviewForm_FormClosed);
            this.Load += new System.EventHandler(this.ReceiptPreviewForm_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPriceOverride)).EndInit();
            this.pnlScroll.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
