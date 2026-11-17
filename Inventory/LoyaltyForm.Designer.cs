namespace _conv
{
    partial class LoyaltyForm
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

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabRegister;
        private System.Windows.Forms.TabPage tabCredit;

        private System.Windows.Forms.Panel pnlRegisterForm;
        private System.Windows.Forms.Label lblRegisterTitle;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.Label lblPointsCaption;
        private System.Windows.Forms.Label lblPointsValue;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNewCustomer;
        private System.Windows.Forms.Button btnDeleteCustomer;
        private System.Windows.Forms.Button btnRefreshCustomers;
        private System.Windows.Forms.Label lblRegisterStatus;
        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.Panel pnlCardPreview;
        private System.Windows.Forms.Label lblCardPreviewTitle;
        private System.Windows.Forms.Label lblCardHint;
        private System.Windows.Forms.Label lblFrontCaption;
        private System.Windows.Forms.Panel pnlCardFront;
        private System.Windows.Forms.Label lblBackCaption;
        private System.Windows.Forms.Panel pnlCardBack;
        private System.Windows.Forms.PictureBox picCardBarcode;

        private System.Windows.Forms.Panel pnlCreditForm;
        private System.Windows.Forms.Label lblCreditTitle;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchCustomer;
        private System.Windows.Forms.Button btnFindCustomer;
        private System.Windows.Forms.Label lblCustomerInfoCaption;
        private System.Windows.Forms.Label lblCustomerInfo;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.TextBox txtAmountSpent;
        private System.Windows.Forms.Label lblPoints;
        private System.Windows.Forms.TextBox txtPoints;
        private System.Windows.Forms.Label lblRateInfo;
        private System.Windows.Forms.Button btnCreditPoints;
        private System.Windows.Forms.Label lblCreditStatus;
        private System.Windows.Forms.DataGridView dgvHistory;

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRegister = new System.Windows.Forms.TabPage();
            this.pnlCardPreview = new System.Windows.Forms.Panel();
            this.lblBackCaption = new System.Windows.Forms.Label();
            this.pnlCardBack = new System.Windows.Forms.Panel();
            this.picCardBarcode = new System.Windows.Forms.PictureBox();
            this.lblFrontCaption = new System.Windows.Forms.Label();
            this.pnlCardFront = new System.Windows.Forms.Panel();
            this.lblCardHint = new System.Windows.Forms.Label();
            this.lblCardPreviewTitle = new System.Windows.Forms.Label();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.pnlRegisterForm = new System.Windows.Forms.Panel();
            this.lblRegisterStatus = new System.Windows.Forms.Label();
            this.btnRefreshCustomers = new System.Windows.Forms.Button();
            this.btnDeleteCustomer = new System.Windows.Forms.Button();
            this.btnNewCustomer = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblPointsValue = new System.Windows.Forms.Label();
            this.lblPointsCaption = new System.Windows.Forms.Label();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.lblContact = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.lblRegisterTitle = new System.Windows.Forms.Label();
            this.tabCredit = new System.Windows.Forms.TabPage();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.pnlCreditForm = new System.Windows.Forms.Panel();
            this.lblCreditStatus = new System.Windows.Forms.Label();
            this.btnCreditPoints = new System.Windows.Forms.Button();
            this.lblRateInfo = new System.Windows.Forms.Label();
            this.txtPoints = new System.Windows.Forms.TextBox();
            this.lblPoints = new System.Windows.Forms.Label();
            this.txtAmountSpent = new System.Windows.Forms.TextBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.lblCustomerInfo = new System.Windows.Forms.Label();
            this.lblCustomerInfoCaption = new System.Windows.Forms.Label();
            this.btnFindCustomer = new System.Windows.Forms.Button();
            this.txtSearchCustomer = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblCreditTitle = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabRegister.SuspendLayout();
            this.pnlCardPreview.SuspendLayout();
            this.pnlCardBack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCardBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            this.pnlRegisterForm.SuspendLayout();
            this.tabCredit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.pnlCreditForm.SuspendLayout();
            this.SuspendLayout();
            //
            // tabControl1
            //
            this.tabControl1.Controls.Add(this.tabRegister);
            this.tabControl1.Controls.Add(this.tabCredit);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(860, 560);
            this.tabControl1.TabIndex = 0;
            //
            // tabRegister
            //
            this.tabRegister.Controls.Add(this.dgvCustomers);
            this.tabRegister.Controls.Add(this.pnlCardPreview);
            this.tabRegister.Controls.Add(this.pnlRegisterForm);
            this.tabRegister.Location = new System.Drawing.Point(4, 24);
            this.tabRegister.Name = "tabRegister";
            this.tabRegister.Padding = new System.Windows.Forms.Padding(3);
            this.tabRegister.Size = new System.Drawing.Size(852, 532);
            this.tabRegister.TabIndex = 0;
            this.tabRegister.Text = "Register Customer";
            this.tabRegister.UseVisualStyleBackColor = true;
            //
            // dgvCustomers
            //
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            this.dgvCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.Location = new System.Drawing.Point(3, 170);
            this.dgvCustomers.MultiSelect = false;
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.RowHeadersWidth = 25;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(846, 359);
            this.dgvCustomers.TabIndex = 1;
            this.dgvCustomers.SelectionChanged += new System.EventHandler(this.dgvCustomers_SelectionChanged);
            //
            // pnlCardPreview
            //
            this.pnlCardPreview.Controls.Add(this.lblBackCaption);
            this.pnlCardPreview.Controls.Add(this.pnlCardBack);
            this.pnlCardPreview.Controls.Add(this.lblFrontCaption);
            this.pnlCardPreview.Controls.Add(this.pnlCardFront);
            this.pnlCardPreview.Controls.Add(this.lblCardHint);
            this.pnlCardPreview.Controls.Add(this.lblCardPreviewTitle);
            this.pnlCardPreview.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlCardPreview.Location = new System.Drawing.Point(466, 170);
            this.pnlCardPreview.Name = "pnlCardPreview";
            this.pnlCardPreview.Size = new System.Drawing.Size(380, 359);
            this.pnlCardPreview.TabIndex = 2;
            //
            // lblCardPreviewTitle
            //
            this.lblCardPreviewTitle.AutoSize = true;
            this.lblCardPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCardPreviewTitle.Location = new System.Drawing.Point(20, 8);
            this.lblCardPreviewTitle.Name = "lblCardPreviewTitle";
            this.lblCardPreviewTitle.Text = "Loyalty Card Preview";
            //
            // lblCardHint
            //
            this.lblCardHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCardHint.Location = new System.Drawing.Point(20, 32);
            this.lblCardHint.Name = "lblCardHint";
            this.lblCardHint.Size = new System.Drawing.Size(340, 30);
            this.lblCardHint.TabIndex = 0;
            this.lblCardHint.Text = "The preview below updates automatically when you save a registration or select a member from the list.";
            //
            // lblFrontCaption
            //
            this.lblFrontCaption.AutoSize = true;
            this.lblFrontCaption.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblFrontCaption.Location = new System.Drawing.Point(20, 64);
            this.lblFrontCaption.Name = "lblFrontCaption";
            this.lblFrontCaption.Text = "FRONT";
            //
            // pnlCardFront
            //
            this.pnlCardFront.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardFront.Location = new System.Drawing.Point(20, 82);
            this.pnlCardFront.Name = "pnlCardFront";
            this.pnlCardFront.Size = new System.Drawing.Size(330, 190);
            this.pnlCardFront.TabIndex = 1;
            this.pnlCardFront.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCardFront_Paint);
            //
            // lblBackCaption
            //
            this.lblBackCaption.AutoSize = true;
            this.lblBackCaption.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBackCaption.Location = new System.Drawing.Point(20, 278);
            this.lblBackCaption.Name = "lblBackCaption";
            this.lblBackCaption.Text = "BACK";
            //
            // pnlCardBack
            //
            this.pnlCardBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCardBack.Controls.Add(this.picCardBarcode);
            this.pnlCardBack.Location = new System.Drawing.Point(20, 296);
            this.pnlCardBack.Name = "pnlCardBack";
            this.pnlCardBack.Size = new System.Drawing.Size(330, 190);
            this.pnlCardBack.TabIndex = 2;
            this.pnlCardBack.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlCardBack_Paint);
            //
            // picCardBarcode
            //
            this.picCardBarcode.Location = new System.Drawing.Point(90, 52);
            this.picCardBarcode.Name = "picCardBarcode";
            this.picCardBarcode.Size = new System.Drawing.Size(150, 46);
            this.picCardBarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCardBarcode.TabIndex = 0;
            this.picCardBarcode.TabStop = false;
            //
            // pnlRegisterForm
            //
            this.pnlRegisterForm.Controls.Add(this.lblRegisterStatus);
            this.pnlRegisterForm.Controls.Add(this.btnRefreshCustomers);
            this.pnlRegisterForm.Controls.Add(this.btnDeleteCustomer);
            this.pnlRegisterForm.Controls.Add(this.btnNewCustomer);
            this.pnlRegisterForm.Controls.Add(this.btnSave);
            this.pnlRegisterForm.Controls.Add(this.lblPointsValue);
            this.pnlRegisterForm.Controls.Add(this.lblPointsCaption);
            this.pnlRegisterForm.Controls.Add(this.txtContact);
            this.pnlRegisterForm.Controls.Add(this.lblContact);
            this.pnlRegisterForm.Controls.Add(this.txtFullName);
            this.pnlRegisterForm.Controls.Add(this.lblFullName);
            this.pnlRegisterForm.Controls.Add(this.txtCode);
            this.pnlRegisterForm.Controls.Add(this.lblCode);
            this.pnlRegisterForm.Controls.Add(this.lblRegisterTitle);
            this.pnlRegisterForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRegisterForm.Location = new System.Drawing.Point(3, 3);
            this.pnlRegisterForm.Name = "pnlRegisterForm";
            this.pnlRegisterForm.Size = new System.Drawing.Size(846, 167);
            this.pnlRegisterForm.TabIndex = 0;
            //
            // lblRegisterTitle
            //
            this.lblRegisterTitle.AutoSize = true;
            this.lblRegisterTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRegisterTitle.Location = new System.Drawing.Point(12, 9);
            this.lblRegisterTitle.Name = "lblRegisterTitle";
            this.lblRegisterTitle.Text = "Customer Registration";
            //
            // lblCode
            //
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(12, 45);
            this.lblCode.Name = "lblCode";
            this.lblCode.Text = "Loyalty Code";
            //
            // txtCode
            //
            this.txtCode.Location = new System.Drawing.Point(12, 63);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(160, 23);
            this.txtCode.TabIndex = 0;
            //
            // lblFullName
            //
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(190, 45);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Text = "Full Name";
            //
            // txtFullName
            //
            this.txtFullName.Location = new System.Drawing.Point(190, 63);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(220, 23);
            this.txtFullName.TabIndex = 1;
            //
            // lblContact
            //
            this.lblContact.AutoSize = true;
            this.lblContact.Location = new System.Drawing.Point(430, 45);
            this.lblContact.Name = "lblContact";
            this.lblContact.Text = "Contact Number";
            //
            // txtContact
            //
            this.txtContact.Location = new System.Drawing.Point(430, 63);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(160, 23);
            this.txtContact.TabIndex = 2;
            //
            // lblPointsCaption
            //
            this.lblPointsCaption.AutoSize = true;
            this.lblPointsCaption.Location = new System.Drawing.Point(610, 45);
            this.lblPointsCaption.Name = "lblPointsCaption";
            this.lblPointsCaption.Text = "Points Balance";
            //
            // lblPointsValue
            //
            this.lblPointsValue.AutoSize = true;
            this.lblPointsValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPointsValue.Location = new System.Drawing.Point(610, 63);
            this.lblPointsValue.Name = "lblPointsValue";
            this.lblPointsValue.Text = "0";
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(12, 95);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 27);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnNewCustomer
            //
            this.btnNewCustomer.Location = new System.Drawing.Point(132, 95);
            this.btnNewCustomer.Name = "btnNewCustomer";
            this.btnNewCustomer.Size = new System.Drawing.Size(110, 27);
            this.btnNewCustomer.TabIndex = 4;
            this.btnNewCustomer.Text = "New / Clear";
            this.btnNewCustomer.Click += new System.EventHandler(this.btnNewCustomer_Click);
            //
            // btnDeleteCustomer
            //
            this.btnDeleteCustomer.Location = new System.Drawing.Point(252, 95);
            this.btnDeleteCustomer.Name = "btnDeleteCustomer";
            this.btnDeleteCustomer.Size = new System.Drawing.Size(130, 27);
            this.btnDeleteCustomer.TabIndex = 5;
            this.btnDeleteCustomer.Text = "Delete Selected";
            this.btnDeleteCustomer.Click += new System.EventHandler(this.btnDeleteCustomer_Click);
            //
            // btnRefreshCustomers
            //
            this.btnRefreshCustomers.Location = new System.Drawing.Point(610, 95);
            this.btnRefreshCustomers.Name = "btnRefreshCustomers";
            this.btnRefreshCustomers.Size = new System.Drawing.Size(110, 27);
            this.btnRefreshCustomers.TabIndex = 6;
            this.btnRefreshCustomers.Text = "Refresh (F5)";
            this.btnRefreshCustomers.Click += new System.EventHandler(this.btnRefreshCustomers_Click);
            //
            // lblRegisterStatus
            //
            this.lblRegisterStatus.AutoSize = true;
            this.lblRegisterStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblRegisterStatus.Location = new System.Drawing.Point(12, 130);
            this.lblRegisterStatus.MaximumSize = new System.Drawing.Size(820, 0);
            this.lblRegisterStatus.Name = "lblRegisterStatus";
            this.lblRegisterStatus.Text = "";
            //
            // tabCredit
            //
            this.tabCredit.Controls.Add(this.dgvHistory);
            this.tabCredit.Controls.Add(this.pnlCreditForm);
            this.tabCredit.Location = new System.Drawing.Point(4, 24);
            this.tabCredit.Name = "tabCredit";
            this.tabCredit.Padding = new System.Windows.Forms.Padding(3);
            this.tabCredit.Size = new System.Drawing.Size(852, 532);
            this.tabCredit.TabIndex = 1;
            this.tabCredit.Text = "Credit Points";
            this.tabCredit.UseVisualStyleBackColor = true;
            //
            // dgvHistory
            //
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.Location = new System.Drawing.Point(3, 205);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersWidth = 25;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(846, 324);
            this.dgvHistory.TabIndex = 1;
            //
            // pnlCreditForm
            //
            this.pnlCreditForm.Controls.Add(this.lblCreditStatus);
            this.pnlCreditForm.Controls.Add(this.btnCreditPoints);
            this.pnlCreditForm.Controls.Add(this.lblRateInfo);
            this.pnlCreditForm.Controls.Add(this.txtPoints);
            this.pnlCreditForm.Controls.Add(this.lblPoints);
            this.pnlCreditForm.Controls.Add(this.txtAmountSpent);
            this.pnlCreditForm.Controls.Add(this.lblAmount);
            this.pnlCreditForm.Controls.Add(this.lblCustomerInfo);
            this.pnlCreditForm.Controls.Add(this.lblCustomerInfoCaption);
            this.pnlCreditForm.Controls.Add(this.btnFindCustomer);
            this.pnlCreditForm.Controls.Add(this.txtSearchCustomer);
            this.pnlCreditForm.Controls.Add(this.lblSearch);
            this.pnlCreditForm.Controls.Add(this.lblCreditTitle);
            this.pnlCreditForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCreditForm.Location = new System.Drawing.Point(3, 3);
            this.pnlCreditForm.Name = "pnlCreditForm";
            this.pnlCreditForm.Size = new System.Drawing.Size(846, 202);
            this.pnlCreditForm.TabIndex = 0;
            //
            // lblCreditTitle
            //
            this.lblCreditTitle.AutoSize = true;
            this.lblCreditTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCreditTitle.Location = new System.Drawing.Point(12, 9);
            this.lblCreditTitle.Name = "lblCreditTitle";
            this.lblCreditTitle.Text = "Credit Loyalty Points";
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(12, 45);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Text = "Search (Loyalty Code or Name)";
            //
            // txtSearchCustomer
            //
            this.txtSearchCustomer.Location = new System.Drawing.Point(12, 63);
            this.txtSearchCustomer.Name = "txtSearchCustomer";
            this.txtSearchCustomer.Size = new System.Drawing.Size(220, 23);
            this.txtSearchCustomer.TabIndex = 0;
            this.txtSearchCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchCustomer_KeyDown);
            //
            // btnFindCustomer
            //
            this.btnFindCustomer.Location = new System.Drawing.Point(240, 62);
            this.btnFindCustomer.Name = "btnFindCustomer";
            this.btnFindCustomer.Size = new System.Drawing.Size(90, 27);
            this.btnFindCustomer.TabIndex = 1;
            this.btnFindCustomer.Text = "Find";
            this.btnFindCustomer.Click += new System.EventHandler(this.btnFindCustomer_Click);
            //
            // lblCustomerInfoCaption
            //
            this.lblCustomerInfoCaption.AutoSize = true;
            this.lblCustomerInfoCaption.Location = new System.Drawing.Point(345, 45);
            this.lblCustomerInfoCaption.Name = "lblCustomerInfoCaption";
            this.lblCustomerInfoCaption.Text = "Customer";
            //
            // lblCustomerInfo
            //
            this.lblCustomerInfo.AutoSize = true;
            this.lblCustomerInfo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCustomerInfo.Location = new System.Drawing.Point(345, 63);
            this.lblCustomerInfo.MaximumSize = new System.Drawing.Size(480, 0);
            this.lblCustomerInfo.Name = "lblCustomerInfo";
            this.lblCustomerInfo.Text = "No customer selected";
            //
            // lblAmount
            //
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(12, 95);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Text = "Amount Spent";
            //
            // txtAmountSpent
            //
            this.txtAmountSpent.Location = new System.Drawing.Point(12, 113);
            this.txtAmountSpent.Name = "txtAmountSpent";
            this.txtAmountSpent.Size = new System.Drawing.Size(140, 23);
            this.txtAmountSpent.TabIndex = 2;
            this.txtAmountSpent.TextChanged += new System.EventHandler(this.txtAmountSpent_TextChanged);
            //
            // lblPoints
            //
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(170, 95);
            this.lblPoints.Name = "lblPoints";
            this.lblPoints.Text = "Points to Credit";
            //
            // txtPoints
            //
            this.txtPoints.Location = new System.Drawing.Point(170, 113);
            this.txtPoints.Name = "txtPoints";
            this.txtPoints.Size = new System.Drawing.Size(100, 23);
            this.txtPoints.TabIndex = 3;
            //
            // lblRateInfo
            //
            this.lblRateInfo.AutoSize = true;
            this.lblRateInfo.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblRateInfo.Location = new System.Drawing.Point(12, 143);
            this.lblRateInfo.Name = "lblRateInfo";
            this.lblRateInfo.Text = "Earn 1 point per PHP 200 spent (auto-computed, editable)";
            //
            // btnCreditPoints
            //
            this.btnCreditPoints.Location = new System.Drawing.Point(600, 108);
            this.btnCreditPoints.Name = "btnCreditPoints";
            this.btnCreditPoints.Size = new System.Drawing.Size(150, 30);
            this.btnCreditPoints.TabIndex = 4;
            this.btnCreditPoints.Text = "Credit Points";
            this.btnCreditPoints.Click += new System.EventHandler(this.btnCreditPoints_Click);
            //
            // lblCreditStatus
            //
            this.lblCreditStatus.AutoSize = true;
            this.lblCreditStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblCreditStatus.Location = new System.Drawing.Point(12, 170);
            this.lblCreditStatus.MaximumSize = new System.Drawing.Size(820, 0);
            this.lblCreditStatus.Name = "lblCreditStatus";
            this.lblCreditStatus.Text = "";
            //
            // LoyaltyForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 560);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "LoyaltyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customer Loyalty";
            this.Load += new System.EventHandler(this.LoyaltyForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabRegister.ResumeLayout(false);
            this.pnlCardPreview.ResumeLayout(false);
            this.pnlCardPreview.PerformLayout();
            this.pnlCardBack.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCardBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            this.pnlRegisterForm.ResumeLayout(false);
            this.pnlRegisterForm.PerformLayout();
            this.tabCredit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.pnlCreditForm.ResumeLayout(false);
            this.pnlCreditForm.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
