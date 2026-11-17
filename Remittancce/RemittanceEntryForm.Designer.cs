namespace Remittancce
{
    partial class RemittanceEntryForm
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

        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.ComboBox cboProvider;
        private System.Windows.Forms.Label lblSenderName;
        private System.Windows.Forms.TextBox txtSenderName;
        private System.Windows.Forms.Label lblSenderContact;
        private System.Windows.Forms.TextBox txtSenderContact;
        private System.Windows.Forms.Label lblReceiverName;
        private System.Windows.Forms.TextBox txtReceiverName;
        private System.Windows.Forms.Label lblReceiverContact;
        private System.Windows.Forms.TextBox txtReceiverContact;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label lblFee;
        private System.Windows.Forms.TextBox txtFee;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Label lblReference;
        private System.Windows.Forms.TextBox txtReference;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        private void InitializeComponent()
        {
            this.lblProvider = new System.Windows.Forms.Label();
            this.cboProvider = new System.Windows.Forms.ComboBox();
            this.lblSenderName = new System.Windows.Forms.Label();
            this.txtSenderName = new System.Windows.Forms.TextBox();
            this.lblSenderContact = new System.Windows.Forms.Label();
            this.txtSenderContact = new System.Windows.Forms.TextBox();
            this.lblReceiverName = new System.Windows.Forms.Label();
            this.txtReceiverName = new System.Windows.Forms.TextBox();
            this.lblReceiverContact = new System.Windows.Forms.Label();
            this.txtReceiverContact = new System.Windows.Forms.TextBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.lblFee = new System.Windows.Forms.Label();
            this.txtFee = new System.Windows.Forms.TextBox();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.lblReference = new System.Windows.Forms.Label();
            this.txtReference = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblProvider
            //
            this.lblProvider.AutoSize = true;
            this.lblProvider.Font = Remittancce.UiStyle.UiFont;
            this.lblProvider.ForeColor = Remittancce.UiStyle.Ink;
            this.lblProvider.Location = new System.Drawing.Point(16, 18);
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Size = new System.Drawing.Size(55, 16);
            this.lblProvider.TabIndex = 0;
            this.lblProvider.Text = "Provider:";
            //
            // cboProvider
            //
            this.cboProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvider.Font = Remittancce.UiStyle.UiFont;
            this.cboProvider.Items.AddRange(new object[] {
            "GCash",
            "PayMaya",
            "Palawan Express",
            "Cebuana Lhuillier",
            "Western Union",
            "Other"});
            this.cboProvider.Location = new System.Drawing.Point(146, 14);
            this.cboProvider.Name = "cboProvider";
            this.cboProvider.Size = new System.Drawing.Size(186, 24);
            this.cboProvider.TabIndex = 1;
            //
            // lblSenderName
            //
            this.lblSenderName.AutoSize = true;
            this.lblSenderName.Font = Remittancce.UiStyle.UiFont;
            this.lblSenderName.ForeColor = Remittancce.UiStyle.Ink;
            this.lblSenderName.Location = new System.Drawing.Point(16, 52);
            this.lblSenderName.Name = "lblSenderName";
            this.lblSenderName.Size = new System.Drawing.Size(85, 16);
            this.lblSenderName.TabIndex = 2;
            this.lblSenderName.Text = "Sender Name:";
            //
            // txtSenderName
            //
            this.txtSenderName.Font = Remittancce.UiStyle.UiFont;
            this.txtSenderName.Location = new System.Drawing.Point(146, 48);
            this.txtSenderName.Name = "txtSenderName";
            this.txtSenderName.Size = new System.Drawing.Size(186, 23);
            this.txtSenderName.TabIndex = 3;
            //
            // lblSenderContact
            //
            this.lblSenderContact.AutoSize = true;
            this.lblSenderContact.Font = Remittancce.UiStyle.UiFont;
            this.lblSenderContact.ForeColor = Remittancce.UiStyle.Ink;
            this.lblSenderContact.Location = new System.Drawing.Point(16, 84);
            this.lblSenderContact.Name = "lblSenderContact";
            this.lblSenderContact.Size = new System.Drawing.Size(97, 16);
            this.lblSenderContact.TabIndex = 4;
            this.lblSenderContact.Text = "Sender Contact:";
            //
            // txtSenderContact
            //
            this.txtSenderContact.Font = Remittancce.UiStyle.UiFont;
            this.txtSenderContact.Location = new System.Drawing.Point(146, 80);
            this.txtSenderContact.Name = "txtSenderContact";
            this.txtSenderContact.Size = new System.Drawing.Size(186, 23);
            this.txtSenderContact.TabIndex = 5;
            //
            // lblReceiverName
            //
            this.lblReceiverName.AutoSize = true;
            this.lblReceiverName.Font = Remittancce.UiStyle.UiFont;
            this.lblReceiverName.ForeColor = Remittancce.UiStyle.Ink;
            this.lblReceiverName.Location = new System.Drawing.Point(16, 116);
            this.lblReceiverName.Name = "lblReceiverName";
            this.lblReceiverName.Size = new System.Drawing.Size(93, 16);
            this.lblReceiverName.TabIndex = 6;
            this.lblReceiverName.Text = "Receiver Name:";
            //
            // txtReceiverName
            //
            this.txtReceiverName.Font = Remittancce.UiStyle.UiFont;
            this.txtReceiverName.Location = new System.Drawing.Point(146, 112);
            this.txtReceiverName.Name = "txtReceiverName";
            this.txtReceiverName.Size = new System.Drawing.Size(186, 23);
            this.txtReceiverName.TabIndex = 7;
            //
            // lblReceiverContact
            //
            this.lblReceiverContact.AutoSize = true;
            this.lblReceiverContact.Font = Remittancce.UiStyle.UiFont;
            this.lblReceiverContact.ForeColor = Remittancce.UiStyle.Ink;
            this.lblReceiverContact.Location = new System.Drawing.Point(16, 148);
            this.lblReceiverContact.Name = "lblReceiverContact";
            this.lblReceiverContact.Size = new System.Drawing.Size(105, 16);
            this.lblReceiverContact.TabIndex = 8;
            this.lblReceiverContact.Text = "Receiver Contact:";
            //
            // txtReceiverContact
            //
            this.txtReceiverContact.Font = Remittancce.UiStyle.UiFont;
            this.txtReceiverContact.Location = new System.Drawing.Point(146, 144);
            this.txtReceiverContact.Name = "txtReceiverContact";
            this.txtReceiverContact.Size = new System.Drawing.Size(186, 23);
            this.txtReceiverContact.TabIndex = 9;
            //
            // lblAmount
            //
            this.lblAmount.AutoSize = true;
            this.lblAmount.Font = Remittancce.UiStyle.UiFont;
            this.lblAmount.ForeColor = Remittancce.UiStyle.Ink;
            this.lblAmount.Location = new System.Drawing.Point(16, 180);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(99, 16);
            this.lblAmount.TabIndex = 10;
            this.lblAmount.Text = "Amount to Send:";
            //
            // txtAmount
            //
            this.txtAmount.Font = Remittancce.UiStyle.LedgerFont;
            this.txtAmount.Location = new System.Drawing.Point(146, 176);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(120, 23);
            this.txtAmount.TabIndex = 11;
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAmount.TextChanged += new System.EventHandler(this.AmountOrFee_TextChanged);
            //
            // lblFee
            //
            this.lblFee.AutoSize = true;
            this.lblFee.Font = Remittancce.UiStyle.UiFont;
            this.lblFee.ForeColor = Remittancce.UiStyle.Ink;
            this.lblFee.Location = new System.Drawing.Point(16, 212);
            this.lblFee.Name = "lblFee";
            this.lblFee.Size = new System.Drawing.Size(76, 16);
            this.lblFee.TabIndex = 12;
            this.lblFee.Text = "Service Fee:";
            //
            // txtFee
            //
            this.txtFee.Font = Remittancce.UiStyle.LedgerFont;
            this.txtFee.Location = new System.Drawing.Point(146, 208);
            this.txtFee.Name = "txtFee";
            this.txtFee.Size = new System.Drawing.Size(120, 23);
            this.txtFee.TabIndex = 13;
            this.txtFee.Text = "0";
            this.txtFee.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtFee.TextChanged += new System.EventHandler(this.AmountOrFee_TextChanged);
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = Remittancce.UiStyle.UiFontBold;
            this.lblTotal.ForeColor = Remittancce.UiStyle.Ink;
            this.lblTotal.Location = new System.Drawing.Point(16, 248);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(114, 16);
            this.lblTotal.TabIndex = 14;
            this.lblTotal.Text = "Total to Collect:";
            //
            // lblTotalValue
            //
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = Remittancce.UiStyle.LedgerFontBold;
            this.lblTotalValue.ForeColor = Remittancce.UiStyle.Stamp;
            this.lblTotalValue.Location = new System.Drawing.Point(146, 246);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(44, 19);
            this.lblTotalValue.TabIndex = 15;
            this.lblTotalValue.Text = "0.00";
            //
            // lblReference
            //
            this.lblReference.AutoSize = true;
            this.lblReference.Font = Remittancce.UiStyle.UiFont;
            this.lblReference.ForeColor = Remittancce.UiStyle.Ink;
            this.lblReference.Location = new System.Drawing.Point(16, 280);
            this.lblReference.Name = "lblReference";
            this.lblReference.Size = new System.Drawing.Size(92, 16);
            this.lblReference.TabIndex = 16;
            this.lblReference.Text = "Reference No.:";
            //
            // txtReference
            //
            this.txtReference.Font = Remittancce.UiStyle.UiFont;
            this.txtReference.Location = new System.Drawing.Point(146, 276);
            this.txtReference.Name = "txtReference";
            this.txtReference.Size = new System.Drawing.Size(186, 23);
            this.txtReference.TabIndex = 17;
            //
            // lblNotes
            //
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = Remittancce.UiStyle.UiFont;
            this.lblNotes.ForeColor = Remittancce.UiStyle.Ink;
            this.lblNotes.Location = new System.Drawing.Point(16, 312);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(45, 16);
            this.lblNotes.TabIndex = 18;
            this.lblNotes.Text = "Notes:";
            //
            // txtNotes
            //
            this.txtNotes.Font = Remittancce.UiStyle.UiFont;
            this.txtNotes.Location = new System.Drawing.Point(146, 308);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(186, 23);
            this.txtNotes.TabIndex = 19;
            //
            // lblDate
            //
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.lblDate.ForeColor = Remittancce.UiStyle.Slate;
            this.lblDate.Location = new System.Drawing.Point(16, 344);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(36, 15);
            this.lblDate.TabIndex = 20;
            this.lblDate.Text = "Date:";
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(146, 372);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.TabIndex = 21;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(242, 372);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // RemittanceEntryForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Remittancce.UiStyle.Paper;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(350, 416);
            this.Controls.Add(this.lblProvider);
            this.Controls.Add(this.cboProvider);
            this.Controls.Add(this.lblSenderName);
            this.Controls.Add(this.txtSenderName);
            this.Controls.Add(this.lblSenderContact);
            this.Controls.Add(this.txtSenderContact);
            this.Controls.Add(this.lblReceiverName);
            this.Controls.Add(this.txtReceiverName);
            this.Controls.Add(this.lblReceiverContact);
            this.Controls.Add(this.txtReceiverContact);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.lblFee);
            this.Controls.Add(this.txtFee);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblTotalValue);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemittanceEntryForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Send Money";
            this.Load += new System.EventHandler(this.RemittanceEntryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
