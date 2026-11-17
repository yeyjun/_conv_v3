namespace Inventory
{
    partial class LowStockReminderForm
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
            this.lblSummary = new System.Windows.Forms.Label();
            this.dgvLowStock = new System.Windows.Forms.DataGridView();
            this.colLowStockSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLowStockDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLowStockQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStock)).BeginInit();
            this.SuspendLayout();
            //
            // lblSummary
            //
            this.lblSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSummary.Location = new System.Drawing.Point(15, 15);
            this.lblSummary.MaximumSize = new System.Drawing.Size(450, 0);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(450, 36);
            this.lblSummary.TabIndex = 0;
            this.lblSummary.Text = "0 items at or below the low-stock threshold:";
            //
            // dgvLowStock
            //
            this.dgvLowStock.AllowUserToAddRows = false;
            this.dgvLowStock.AllowUserToDeleteRows = false;
            this.dgvLowStock.AllowUserToResizeRows = false;
            this.dgvLowStock.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLowStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLowStock.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLowStockSerial,
            this.colLowStockDescription,
            this.colLowStockQty});
            this.dgvLowStock.Location = new System.Drawing.Point(15, 58);
            this.dgvLowStock.Name = "dgvLowStock";
            this.dgvLowStock.ReadOnly = true;
            this.dgvLowStock.RowHeadersVisible = false;
            this.dgvLowStock.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLowStock.Size = new System.Drawing.Size(450, 330);
            this.dgvLowStock.TabIndex = 1;
            //
            // colLowStockSerial
            //
            this.colLowStockSerial.FillWeight = 90F;
            this.colLowStockSerial.HeaderText = "Serial";
            this.colLowStockSerial.Name = "colLowStockSerial";
            this.colLowStockSerial.ReadOnly = true;
            //
            // colLowStockDescription
            //
            this.colLowStockDescription.FillWeight = 180F;
            this.colLowStockDescription.HeaderText = "Description";
            this.colLowStockDescription.Name = "colLowStockDescription";
            this.colLowStockDescription.ReadOnly = true;
            //
            // colLowStockQty
            //
            this.colLowStockQty.FillWeight = 60F;
            this.colLowStockQty.HeaderText = "Qty";
            this.colLowStockQty.Name = "colLowStockQty";
            this.colLowStockQty.ReadOnly = true;
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(370, 400);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(95, 32);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // LowStockReminderForm
            //
            this.AcceptButton = this.btnClose;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(480, 450);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvLowStock);
            this.Controls.Add(this.lblSummary);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LowStockReminderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Low Stock Reminder";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.DataGridView dgvLowStock;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLowStockSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLowStockDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLowStockQty;
        private System.Windows.Forms.Button btnClose;
    }
}
