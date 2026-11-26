namespace Inventory
{
    partial class ListSales
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvSales = new System.Windows.Forms.DataGridView();
            this.colTransactionCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTransactionDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeller = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.btnAll = new System.Windows.Forms.Button();
            this.btnThisMonth = new System.Windows.Forms.Button();
            this.btnToday = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnVoidRefund = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).BeginInit();
            this.pnlFilters.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvSales
            //
            this.dgvSales.AllowUserToAddRows = false;
            this.dgvSales.AllowUserToDeleteRows = false;
            this.dgvSales.AllowUserToResizeRows = false;
            this.dgvSales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSales.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.dgvSales.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSales.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTransactionCode,
            this.colItemCode,
            this.colItemName,
            this.colPrice,
            this.colTransactionDate,
            this.colTeller});
            this.dgvSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSales.Location = new System.Drawing.Point(0, 78);
            this.dgvSales.Name = "dgvSales";
            this.dgvSales.ReadOnly = true;
            this.dgvSales.RowHeadersVisible = false;
            this.dgvSales.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSales.Size = new System.Drawing.Size(900, 434);
            this.dgvSales.TabIndex = 0;
            this.dgvSales.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSales_CellClick);
            //
            // colTransactionCode
            //
            this.colTransactionCode.DataPropertyName = "TransactionCode";
            this.colTransactionCode.HeaderText = "Transaction Code";
            this.colTransactionCode.Name = "colTransactionCode";
            this.colTransactionCode.ReadOnly = true;
            //
            // colItemCode
            //
            this.colItemCode.DataPropertyName = "ItemCode";
            this.colItemCode.HeaderText = "Item Code";
            this.colItemCode.Name = "colItemCode";
            this.colItemCode.ReadOnly = true;
            //
            // colItemName
            //
            this.colItemName.DataPropertyName = "ItemName";
            this.colItemName.HeaderText = "Item Name";
            this.colItemName.Name = "colItemName";
            this.colItemName.ReadOnly = true;
            //
            // colPrice
            //
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "N2";
            this.colPrice.DefaultCellStyle = dataGridViewCellStyle1;
            this.colPrice.DataPropertyName = "Price";
            this.colPrice.HeaderText = "Price";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            //
            // colTransactionDate
            //
            this.colTransactionDate.DataPropertyName = "TransactionDate";
            this.colTransactionDate.HeaderText = "Date";
            this.colTransactionDate.Name = "colTransactionDate";
            this.colTransactionDate.ReadOnly = true;
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
            this.pnlFilters.Controls.Add(this.btnAll);
            this.pnlFilters.Controls.Add(this.btnThisMonth);
            this.pnlFilters.Controls.Add(this.btnToday);
            this.pnlFilters.Controls.Add(this.btnFilter);
            this.pnlFilters.Controls.Add(this.dtpTo);
            this.pnlFilters.Controls.Add(this.lblTo);
            this.pnlFilters.Controls.Add(this.dtpFrom);
            this.pnlFilters.Controls.Add(this.lblFrom);
            this.pnlFilters.Controls.Add(this.btnExport);
            this.pnlFilters.Controls.Add(this.btnRefresh);
            this.pnlFilters.Controls.Add(this.txtSearch);
            this.pnlFilters.Controls.Add(this.lblSearch);
            this.pnlFilters.Controls.Add(this.btnVoidRefund);
            this.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilters.Location = new System.Drawing.Point(0, 0);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(900, 78);
            this.pnlFilters.TabIndex = 1;
            //
            // btnAll
            //
            this.btnAll.Location = new System.Drawing.Point(700, 10);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(60, 25);
            this.btnAll.TabIndex = 7;
            this.btnAll.Text = "All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            //
            // btnThisMonth
            //
            this.btnThisMonth.Location = new System.Drawing.Point(600, 10);
            this.btnThisMonth.Name = "btnThisMonth";
            this.btnThisMonth.Size = new System.Drawing.Size(90, 25);
            this.btnThisMonth.TabIndex = 6;
            this.btnThisMonth.Text = "This Month";
            this.btnThisMonth.UseVisualStyleBackColor = true;
            this.btnThisMonth.Click += new System.EventHandler(this.btnThisMonth_Click);
            //
            // btnToday
            //
            this.btnToday.Location = new System.Drawing.Point(524, 10);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(70, 25);
            this.btnToday.TabIndex = 5;
            this.btnToday.Text = "Today";
            this.btnToday.UseVisualStyleBackColor = true;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            //
            // btnFilter
            //
            this.btnFilter.Location = new System.Drawing.Point(444, 10);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(74, 25);
            this.btnFilter.TabIndex = 4;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            //
            // dtpTo
            //
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(320, 11);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(115, 20);
            this.dtpTo.TabIndex = 3;
            this.dtpTo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtp_KeyUp);
            //
            // btnVoidRefund
            //
            this.btnVoidRefund.BackColor = System.Drawing.Color.FromArgb(198, 40, 40);
            this.btnVoidRefund.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVoidRefund.FlatAppearance.BorderSize = 0;
            this.btnVoidRefund.ForeColor = System.Drawing.Color.White;
            this.btnVoidRefund.Location = new System.Drawing.Point(15, 10);
            this.btnVoidRefund.Name = "btnVoidRefund";
            this.btnVoidRefund.Size = new System.Drawing.Size(95, 25);
            this.btnVoidRefund.TabIndex = 13;
            this.btnVoidRefund.Text = "Refund/Void";
            this.btnVoidRefund.UseVisualStyleBackColor = false;
            this.btnVoidRefund.Click += new System.EventHandler(this.btnVoidRefund_Click);
            //
            // lblTo
            //
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(292, 15);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(22, 13);
            this.lblTo.TabIndex = 8;
            this.lblTo.Text = "To:";
            //
            // dtpFrom
            //
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(160, 11);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(115, 20);
            this.dtpFrom.TabIndex = 2;
            this.dtpFrom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dtp_KeyUp);
            //
            // lblFrom
            //
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(120, 15);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(35, 13);
            this.lblFrom.TabIndex = 9;
            this.lblFrom.Text = "From:";
            //
            // btnExport
            //
            this.btnExport.Location = new System.Drawing.Point(780, 42);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(108, 25);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "Export to Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(672, 42);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(96, 25);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Refresh (F5)";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(160, 44);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(275, 20);
            this.txtSearch.TabIndex = 10;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(15, 47);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            //
            // pnlSummary
            //
            this.pnlSummary.Controls.Add(this.lblSummary);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSummary.Location = new System.Drawing.Point(0, 512);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(900, 30);
            this.pnlSummary.TabIndex = 2;
            //
            // lblSummary
            //
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblSummary.Location = new System.Drawing.Point(12, 8);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(200, 17);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "Transactions: 0    Items: 0    Total Sales: 0.00";
            //
            // ListSales
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 542);
            this.Controls.Add(this.dgvSales);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.pnlFilters);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "ListSales";
            this.Text = "Sales Report";
            this.KeyPreview = true;
            this.Load += new System.EventHandler(this.ListSales_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListSales_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSales)).EndInit();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSales;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTransactionDate;
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
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnVoidRefund;
    }
}
