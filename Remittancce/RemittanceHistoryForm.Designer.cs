namespace Remittancce
{
    partial class RemittanceHistoryForm
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

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblHeaderDate;
        private System.Windows.Forms.DataGridView dgvRemittance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProvider;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSender;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReceiver;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFee;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReference;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeller;
        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnToday;
        private System.Windows.Forms.Button btnThisMonth;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Label lblTeller;
        private System.Windows.Forms.TextBox txtTeller;
        private System.Windows.Forms.Panel pnlFooter;
        private Remittancce.PerforationDivider dividerSummary;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.FlowLayoutPanel pnlActions;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnAddEntry;

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderDate = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvRemittance = new System.Windows.Forms.DataGridView();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProvider = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReceiver = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeller = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.txtTeller = new System.Windows.Forms.TextBox();
            this.lblTeller = new System.Windows.Forms.Label();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnThisMonth = new System.Windows.Forms.Button();
            this.btnToday = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddEntry = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblSummary = new System.Windows.Forms.Label();
            this.dividerSummary = new Remittancce.PerforationDivider();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRemittance)).BeginInit();
            this.pnlFilters.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = Remittancce.UiStyle.Ink;
            this.pnlHeader.Controls.Add(this.lblHeaderDate);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1040, 54);
            this.pnlHeader.TabIndex = 0;
            //
            // lblHeaderDate
            //
            this.lblHeaderDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeaderDate.AutoSize = true;
            this.lblHeaderDate.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.lblHeaderDate.ForeColor = Remittancce.UiStyle.HeaderSubtext;
            this.lblHeaderDate.Location = new System.Drawing.Point(850, 20);
            this.lblHeaderDate.Name = "lblHeaderDate";
            this.lblHeaderDate.Size = new System.Drawing.Size(80, 16);
            this.lblHeaderDate.TabIndex = 1;
            this.lblHeaderDate.Text = "----";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "SEND MONEY";
            //
            // dgvRemittance
            //
            this.dgvRemittance.AllowUserToAddRows = false;
            this.dgvRemittance.AllowUserToDeleteRows = false;
            this.dgvRemittance.AllowUserToResizeRows = false;
            this.dgvRemittance.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRemittance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDate,
            this.colProvider,
            this.colSender,
            this.colReceiver,
            this.colAmount,
            this.colFee,
            this.colTotal,
            this.colReference,
            this.colTeller});
            this.dgvRemittance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRemittance.Location = new System.Drawing.Point(0, 104);
            this.dgvRemittance.Name = "dgvRemittance";
            this.dgvRemittance.ReadOnly = true;
            this.dgvRemittance.RowHeadersVisible = false;
            this.dgvRemittance.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRemittance.Size = new System.Drawing.Size(1040, 274);
            this.dgvRemittance.TabIndex = 0;
            //
            // colDate
            //
            this.colDate.DataPropertyName = "EntryDate";
            this.colDate.HeaderText = "Date";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            //
            // colProvider
            //
            this.colProvider.DataPropertyName = "Provider";
            this.colProvider.HeaderText = "Provider";
            this.colProvider.Name = "colProvider";
            this.colProvider.ReadOnly = true;
            //
            // colSender
            //
            this.colSender.DataPropertyName = "SenderName";
            this.colSender.HeaderText = "Sender";
            this.colSender.Name = "colSender";
            this.colSender.ReadOnly = true;
            //
            // colReceiver
            //
            this.colReceiver.DataPropertyName = "ReceiverName";
            this.colReceiver.HeaderText = "Receiver";
            this.colReceiver.Name = "colReceiver";
            this.colReceiver.ReadOnly = true;
            //
            // colAmount
            //
            this.colAmount.DataPropertyName = "Amount";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Font = Remittancce.UiStyle.LedgerFont;
            dataGridViewCellStyle1.Format = "N2";
            this.colAmount.DefaultCellStyle = dataGridViewCellStyle1;
            this.colAmount.HeaderText = "Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.ReadOnly = true;
            //
            // colFee
            //
            this.colFee.DataPropertyName = "ServiceFee";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Font = Remittancce.UiStyle.LedgerFont;
            dataGridViewCellStyle2.Format = "N2";
            this.colFee.DefaultCellStyle = dataGridViewCellStyle2;
            this.colFee.HeaderText = "Fee";
            this.colFee.Name = "colFee";
            this.colFee.ReadOnly = true;
            //
            // colTotal
            //
            this.colTotal.DataPropertyName = "Total";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Font = Remittancce.UiStyle.LedgerFontBold;
            dataGridViewCellStyle3.ForeColor = Remittancce.UiStyle.Stamp;
            dataGridViewCellStyle3.Format = "N2";
            this.colTotal.DefaultCellStyle = dataGridViewCellStyle3;
            this.colTotal.HeaderText = "Total";
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            //
            // colReference
            //
            this.colReference.DataPropertyName = "ReferenceNo";
            this.colReference.HeaderText = "Reference No.";
            this.colReference.Name = "colReference";
            this.colReference.ReadOnly = true;
            //
            // colTeller
            //
            this.colTeller.DataPropertyName = "Teller";
            this.colTeller.HeaderText = "Teller";
            this.colTeller.Name = "colTeller";
            this.colTeller.ReadOnly = true;
            //
            // pnlFilters
            //
            this.pnlFilters.BackColor = Remittancce.UiStyle.Paper;
            this.pnlFilters.Controls.Add(this.txtTeller);
            this.pnlFilters.Controls.Add(this.lblTeller);
            this.pnlFilters.Controls.Add(this.btnAll);
            this.pnlFilters.Controls.Add(this.btnThisMonth);
            this.pnlFilters.Controls.Add(this.btnToday);
            this.pnlFilters.Controls.Add(this.btnFilter);
            this.pnlFilters.Controls.Add(this.dtpTo);
            this.pnlFilters.Controls.Add(this.lblTo);
            this.pnlFilters.Controls.Add(this.dtpFrom);
            this.pnlFilters.Controls.Add(this.lblFrom);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilters.Location = new System.Drawing.Point(0, 54);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(1040, 50);
            this.pnlFilters.TabIndex = 1;
            //
            // txtTeller
            //
            this.txtTeller.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTeller.Font = Remittancce.UiStyle.UiFont;
            this.txtTeller.Location = new System.Drawing.Point(906, 12);
            this.txtTeller.Name = "txtTeller";
            this.txtTeller.Size = new System.Drawing.Size(122, 23);
            this.txtTeller.TabIndex = 15;
            //
            // lblTeller
            //
            this.lblTeller.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTeller.AutoSize = true;
            this.lblTeller.Font = Remittancce.UiStyle.UiFont;
            this.lblTeller.ForeColor = Remittancce.UiStyle.Ink;
            this.lblTeller.Location = new System.Drawing.Point(856, 16);
            this.lblTeller.Name = "lblTeller";
            this.lblTeller.Size = new System.Drawing.Size(44, 16);
            this.lblTeller.TabIndex = 14;
            this.lblTeller.Text = "Teller:";
            //
            // btnAll
            //
            Remittancce.UiStyle.StyleSecondary(this.btnAll);
            this.btnAll.Location = new System.Drawing.Point(546, 11);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(46, 27);
            this.btnAll.TabIndex = 7;
            this.btnAll.Text = "All";
            this.btnAll.UseVisualStyleBackColor = false;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            //
            // btnThisMonth
            //
            Remittancce.UiStyle.StyleSecondary(this.btnThisMonth);
            this.btnThisMonth.Location = new System.Drawing.Point(450, 11);
            this.btnThisMonth.Name = "btnThisMonth";
            this.btnThisMonth.Size = new System.Drawing.Size(90, 27);
            this.btnThisMonth.TabIndex = 6;
            this.btnThisMonth.Text = "This Month";
            this.btnThisMonth.UseVisualStyleBackColor = false;
            this.btnThisMonth.Click += new System.EventHandler(this.btnThisMonth_Click);
            //
            // btnToday
            //
            Remittancce.UiStyle.StyleSecondary(this.btnToday);
            this.btnToday.Location = new System.Drawing.Point(384, 11);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(60, 27);
            this.btnToday.TabIndex = 5;
            this.btnToday.Text = "Today";
            this.btnToday.UseVisualStyleBackColor = false;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            //
            // btnFilter
            //
            Remittancce.UiStyle.StyleSecondary(this.btnFilter);
            this.btnFilter.Location = new System.Drawing.Point(320, 11);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(58, 27);
            this.btnFilter.TabIndex = 4;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            //
            // dtpTo
            //
            this.dtpTo.Font = Remittancce.UiStyle.UiFont;
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(200, 12);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(110, 23);
            this.dtpTo.TabIndex = 3;
            this.dtpTo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtp_KeyUp);
            //
            // lblTo
            //
            this.lblTo.AutoSize = true;
            this.lblTo.Font = Remittancce.UiStyle.UiFont;
            this.lblTo.ForeColor = Remittancce.UiStyle.Ink;
            this.lblTo.Location = new System.Drawing.Point(172, 16);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(26, 16);
            this.lblTo.TabIndex = 8;
            this.lblTo.Text = "To:";
            //
            // dtpFrom
            //
            this.dtpFrom.Font = Remittancce.UiStyle.UiFont;
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(50, 12);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(110, 23);
            this.dtpFrom.TabIndex = 2;
            this.dtpFrom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtp_KeyUp);
            //
            // lblFrom
            //
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = Remittancce.UiStyle.UiFont;
            this.lblFrom.ForeColor = Remittancce.UiStyle.Ink;
            this.lblFrom.Location = new System.Drawing.Point(12, 16);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(40, 16);
            this.lblFrom.TabIndex = 9;
            this.lblFrom.Text = "From:";
            //
            // pnlFooter
            //
            this.pnlFooter.BackColor = Remittancce.UiStyle.Paper;
            this.pnlFooter.Controls.Add(this.pnlActions);
            this.pnlFooter.Controls.Add(this.lblSummary);
            this.pnlFooter.Controls.Add(this.dividerSummary);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 378);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1040, 64);
            this.pnlFooter.TabIndex = 2;
            //
            // pnlActions
            //
            this.pnlActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlActions.Controls.Add(this.btnAddEntry);
            this.pnlActions.Controls.Add(this.btnExport);
            this.pnlActions.Controls.Add(this.btnRefresh);
            this.pnlActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlActions.Location = new System.Drawing.Point(620, 17);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(408, 38);
            this.pnlActions.TabIndex = 2;
            this.pnlActions.WrapContents = false;
            //
            // btnAddEntry
            //
            Remittancce.UiStyle.StylePrimary(this.btnAddEntry);
            this.btnAddEntry.Location = new System.Drawing.Point(255, 3);
            this.btnAddEntry.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.btnAddEntry.Name = "btnAddEntry";
            this.btnAddEntry.Size = new System.Drawing.Size(150, 32);
            this.btnAddEntry.TabIndex = 13;
            this.btnAddEntry.Text = "Send Money (F2)";
            this.btnAddEntry.UseVisualStyleBackColor = false;
            this.btnAddEntry.Click += new System.EventHandler(this.btnAddEntry_Click);
            //
            // btnExport
            //
            Remittancce.UiStyle.StyleSecondary(this.btnExport);
            this.btnExport.Location = new System.Drawing.Point(139, 3);
            this.btnExport.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 32);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "Export to CSV";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // btnRefresh
            //
            Remittancce.UiStyle.StyleSecondary(this.btnRefresh);
            this.btnRefresh.Location = new System.Drawing.Point(33, 3);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 32);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Refresh (F5)";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblSummary
            //
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = Remittancce.UiStyle.LedgerFontBold;
            this.lblSummary.ForeColor = Remittancce.UiStyle.Ink;
            this.lblSummary.Location = new System.Drawing.Point(20, 26);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(300, 19);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "Entries: 0    Total Sent: 0.00    Fees Collected: 0.00";
            //
            // dividerSummary
            //
            this.dividerSummary.BackColor = Remittancce.UiStyle.Paper;
            this.dividerSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.dividerSummary.Location = new System.Drawing.Point(0, 0);
            this.dividerSummary.Name = "dividerSummary";
            this.dividerSummary.Size = new System.Drawing.Size(1040, 11);
            this.dividerSummary.TabIndex = 1;
            //
            // RemittanceHistoryForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Remittancce.UiStyle.Paper;
            this.ClientSize = new System.Drawing.Size(1040, 442);
            this.Controls.Add(this.dgvRemittance);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlFilters);
            this.Controls.Add(this.pnlHeader);
            this.Font = Remittancce.UiStyle.UiFont;
            this.MinimumSize = new System.Drawing.Size(720, 360);
            this.Name = "RemittanceHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remittancce - Send Money";
            this.KeyPreview = true;
            this.Load += new System.EventHandler(this.RemittanceHistoryForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RemittanceHistoryForm_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRemittance)).EndInit();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
