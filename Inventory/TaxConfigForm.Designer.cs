namespace Inventory
{
    partial class TaxConfigForm
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
            this.lbl_prompt = new System.Windows.Forms.Label();
            this.nud_taxRate = new System.Windows.Forms.NumericUpDown();
            this.lbl_percent = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nud_taxRate)).BeginInit();
            this.SuspendLayout();
            //
            // lbl_prompt
            //
            this.lbl_prompt.AutoSize = true;
            this.lbl_prompt.Location = new System.Drawing.Point(12, 15);
            this.lbl_prompt.Name = "lbl_prompt";
            this.lbl_prompt.Size = new System.Drawing.Size(120, 13);
            this.lbl_prompt.TabIndex = 0;
            this.lbl_prompt.Text = "Tax rate applied to sales:";
            //
            // nud_taxRate
            //
            this.nud_taxRate.DecimalPlaces = 2;
            this.nud_taxRate.Location = new System.Drawing.Point(15, 35);
            this.nud_taxRate.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.nud_taxRate.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.nud_taxRate.Name = "nud_taxRate";
            this.nud_taxRate.Size = new System.Drawing.Size(100, 22);
            this.nud_taxRate.TabIndex = 1;
            this.nud_taxRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lbl_percent
            //
            this.lbl_percent.AutoSize = true;
            this.lbl_percent.Location = new System.Drawing.Point(121, 38);
            this.lbl_percent.Name = "lbl_percent";
            this.lbl_percent.Size = new System.Drawing.Size(15, 13);
            this.lbl_percent.TabIndex = 2;
            this.lbl_percent.Text = "%";
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(15, 68);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(135, 68);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // TaxConfigForm
            //
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(250, 112);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lbl_percent);
            this.Controls.Add(this.nud_taxRate);
            this.Controls.Add(this.lbl_prompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaxConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tax Configuration";
            this.Load += new System.EventHandler(this.TaxConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nud_taxRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_prompt;
        private System.Windows.Forms.NumericUpDown nud_taxRate;
        private System.Windows.Forms.Label lbl_percent;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
