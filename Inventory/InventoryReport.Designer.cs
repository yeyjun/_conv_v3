namespace _conv
{
    partial class InventoryReport
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
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.colSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWholesalePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRetailPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPricePerPiece = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.cmbStockFilter = new System.Windows.Forms.ComboBox();
            this.lblStock = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvInventory
            //
            this.dgvInventory.AllowUserToAddRows = false;
            this.dgvInventory.AllowUserToDeleteRows = false;
            this.dgvInventory.AllowUserToResizeRows = false;
            this.dgvInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSerial,
            this.colDescription,
            this.colWholesalePrice,
            this.colRetailPrice,
            this.colPricePerPiece,
            this.colQuantity});
            this.dgvInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInventory.Location = new System.Drawing.Point(0, 0);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.ReadOnly = true;
            this.dgvInventory.RowHeadersVisible = false;
            this.dgvInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInventory.Size = new System.Drawing.Size(760, 420);
            this.dgvInventory.TabIndex = 0;
            //
            // colSerial
            //
            this.colSerial.FillWeight = 150F;
            this.colSerial.HeaderText = "Serial";
            this.colSerial.Name = "colSerial";
            this.colSerial.ReadOnly = true;
            //
            // colDescription
            //
            this.colDescription.FillWeight = 220F;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            //
            // colWholesalePrice
            //
            this.colWholesalePrice.FillWeight = 110F;
            this.colWholesalePrice.HeaderText = "Wholesale Price";
            this.colWholesalePrice.Name = "colWholesalePrice";
            this.colWholesalePrice.ReadOnly = true;
            //
            // colRetailPrice
            //
            this.colRetailPrice.FillWeight = 100F;
            this.colRetailPrice.HeaderText = "Retail Price";
            this.colRetailPrice.Name = "colRetailPrice";
            this.colRetailPrice.ReadOnly = true;
            //
            // colPricePerPiece
            //
            this.colPricePerPiece.FillWeight = 110F;
            this.colPricePerPiece.HeaderText = "Price per Piece";
            this.colPricePerPiece.Name = "colPricePerPiece";
            this.colPricePerPiece.ReadOnly = true;
            //
            // colQuantity
            //
            this.colQuantity.FillWeight = 90F;
            this.colQuantity.HeaderText = "Quantity";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.ReadOnly = true;
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.lblTotal);
            this.pnlButtons.Controls.Add(this.btnExport);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 420);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(760, 44);
            this.pnlButtons.TabIndex = 1;
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(280, 15);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(70, 13);
            this.lblTotal.TabIndex = 2;
            this.lblTotal.Text = "Total items: 0";
            //
            // btnExport
            //
            this.btnExport.Location = new System.Drawing.Point(150, 8);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(120, 28);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "Export to Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(12, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 28);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh (F5)";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // pnlFilter
            //
            this.pnlFilter.Controls.Add(this.btnClearFilter);
            this.pnlFilter.Controls.Add(this.cmbStockFilter);
            this.pnlFilter.Controls.Add(this.lblStock);
            this.pnlFilter.Controls.Add(this.txtSearch);
            this.pnlFilter.Controls.Add(this.lblSearch);
            this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Location = new System.Drawing.Point(0, 0);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Size = new System.Drawing.Size(760, 36);
            this.pnlFilter.TabIndex = 2;
            //
            // btnClearFilter
            //
            this.btnClearFilter.Location = new System.Drawing.Point(505, 5);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(70, 25);
            this.btnClearFilter.TabIndex = 4;
            this.btnClearFilter.Text = "Clear";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            //
            // cmbStockFilter
            //
            this.cmbStockFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStockFilter.FormattingEnabled = true;
            this.cmbStockFilter.Items.AddRange(new object[] {
            "All Items",
            "In Stock",
            "Out of Stock"});
            this.cmbStockFilter.Location = new System.Drawing.Point(355, 6);
            this.cmbStockFilter.Name = "cmbStockFilter";
            this.cmbStockFilter.Size = new System.Drawing.Size(140, 21);
            this.cmbStockFilter.TabIndex = 3;
            this.cmbStockFilter.SelectedIndexChanged += new System.EventHandler(this.cmbStockFilter_SelectedIndexChanged);
            //
            // lblStock
            //
            this.lblStock.AutoSize = true;
            this.lblStock.Location = new System.Drawing.Point(310, 10);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(37, 13);
            this.lblStock.TabIndex = 2;
            this.lblStock.Text = "Stock:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(70, 6);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(220, 20);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 10);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            //
            // InventoryReport
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 500);
            this.Controls.Add(this.dgvInventory);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlButtons);
            this.MinimumSize = new System.Drawing.Size(500, 320);
            this.Name = "InventoryReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Inventory Report";
            this.KeyPreview = true;
            this.Load += new System.EventHandler(this.InventoryReport_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InventoryReport_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWholesalePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRetailPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPricePerPiece;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.ComboBox cmbStockFilter;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
    }
}
