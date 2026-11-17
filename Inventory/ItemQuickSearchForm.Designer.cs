namespace Inventory
{
    partial class ItemQuickSearchForm
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
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.colSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWholesale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRetail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPerPiece = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblHint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            //
            // txtSearch
            //
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular);
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(520, 29);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // lblHint
            //
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Italic);
            this.lblHint.ForeColor = System.Drawing.Color.Gray;
            this.lblHint.Padding = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.lblHint.Location = new System.Drawing.Point(0, 29);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(520, 24);
            this.lblHint.TabIndex = 1;
            this.lblHint.Text = "Price check only - items are not added to the cart from here.";
            //
            // dgvResults
            //
            this.dgvResults.AllowUserToAddRows = false;
            this.dgvResults.AllowUserToDeleteRows = false;
            this.dgvResults.AllowUserToResizeRows = false;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSerial,
            this.colDescription,
            this.colWholesale,
            this.colRetail,
            this.colPerPiece});
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.Location = new System.Drawing.Point(0, 53);
            this.dgvResults.MultiSelect = false;
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.ReadOnly = true;
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResults.Size = new System.Drawing.Size(520, 347);
            this.dgvResults.TabIndex = 2;
            //
            // colSerial
            //
            this.colSerial.HeaderText = "Serial";
            this.colSerial.Name = "colSerial";
            this.colSerial.ReadOnly = true;
            this.colSerial.Width = 110;
            //
            // colDescription
            //
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 160;
            //
            // colWholesale
            //
            this.colWholesale.HeaderText = "Wholesale";
            this.colWholesale.Name = "colWholesale";
            this.colWholesale.ReadOnly = true;
            this.colWholesale.DefaultCellStyle.Format = "0.00";
            this.colWholesale.Width = 80;
            //
            // colRetail
            //
            this.colRetail.HeaderText = "Retail";
            this.colRetail.Name = "colRetail";
            this.colRetail.ReadOnly = true;
            this.colRetail.DefaultCellStyle.Format = "0.00";
            this.colRetail.Width = 80;
            //
            // colPerPiece
            //
            this.colPerPiece.HeaderText = "Per Piece";
            this.colPerPiece.Name = "colPerPiece";
            this.colPerPiece.ReadOnly = true;
            this.colPerPiece.DefaultCellStyle.Format = "0.00";
            this.colPerPiece.Width = 80;
            //
            // ItemQuickSearchForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 400);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.txtSearch);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(360, 220);
            this.Name = "ItemQuickSearchForm";
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Item Search";
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemQuickSearchForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWholesale;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRetail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPerPiece;
    }
}
