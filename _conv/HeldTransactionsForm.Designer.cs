namespace _conv
{
    partial class HeldTransactionsForm
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
            this.dgvHeld = new System.Windows.Forms.DataGridView();
            this.colTicketCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHeldAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGrandTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.lblEmpty = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeld)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvHeld
            //
            this.dgvHeld.AllowUserToAddRows = false;
            this.dgvHeld.AllowUserToDeleteRows = false;
            this.dgvHeld.AllowUserToResizeRows = false;
            this.dgvHeld.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHeld.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTicketCode,
            this.colHeldAt,
            this.colItemCount,
            this.colGrandTotal});
            this.dgvHeld.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHeld.Location = new System.Drawing.Point(0, 0);
            this.dgvHeld.MultiSelect = false;
            this.dgvHeld.Name = "dgvHeld";
            this.dgvHeld.ReadOnly = true;
            this.dgvHeld.RowHeadersVisible = false;
            this.dgvHeld.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHeld.Size = new System.Drawing.Size(420, 253);
            this.dgvHeld.TabIndex = 0;
            this.dgvHeld.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHeld_CellDoubleClick);
            //
            // colTicketCode
            //
            this.colTicketCode.HeaderText = "Ticket";
            this.colTicketCode.Name = "colTicketCode";
            this.colTicketCode.ReadOnly = true;
            this.colTicketCode.Width = 140;
            //
            // colHeldAt
            //
            this.colHeldAt.HeaderText = "Held At";
            this.colHeldAt.Name = "colHeldAt";
            this.colHeldAt.ReadOnly = true;
            this.colHeldAt.Width = 110;
            //
            // colItemCount
            //
            this.colItemCount.HeaderText = "Items";
            this.colItemCount.Name = "colItemCount";
            this.colItemCount.ReadOnly = true;
            this.colItemCount.Width = 60;
            //
            // colGrandTotal
            //
            this.colGrandTotal.HeaderText = "Total";
            this.colGrandTotal.Name = "colGrandTotal";
            this.colGrandTotal.ReadOnly = true;
            this.colGrandTotal.Width = 90;
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnResume);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 253);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(420, 44);
            this.pnlButtons.TabIndex = 1;
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(290, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 28);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(150, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel (F9)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnResume
            //
            this.btnResume.Location = new System.Drawing.Point(12, 8);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(130, 28);
            this.btnResume.TabIndex = 0;
            this.btnResume.Text = "Resume (F8)";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            //
            // lblEmpty
            //
            this.lblEmpty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblEmpty.AutoSize = true;
            this.lblEmpty.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblEmpty.Location = new System.Drawing.Point(145, 115);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(130, 13);
            this.lblEmpty.TabIndex = 2;
            this.lblEmpty.Text = "No held transactions";
            this.lblEmpty.Visible = false;
            //
            // HeldTransactionsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 297);
            this.Controls.Add(this.lblEmpty);
            this.Controls.Add(this.dgvHeld);
            this.Controls.Add(this.pnlButtons);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 220);
            this.Name = "HeldTransactionsForm";
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Held Transactions";
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HeldTransactionsForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeld)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHeld;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTicketCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeldAt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGrandTotal;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnResume;
        private System.Windows.Forms.Label lblEmpty;
    }
}
