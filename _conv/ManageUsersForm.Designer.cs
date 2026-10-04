namespace _conv
{
    partial class ManageUsersForm
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

        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.Panel pnlForm;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNewUser;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;

        private void InitializeComponent()
        {
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.pnlForm = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNewUser = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.pnlForm.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvUsers
            //
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToDeleteRows = false;
            this.dgvUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsers.MultiSelect = false;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.Location = new System.Drawing.Point(0, 130);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.RowHeadersWidth = 25;
            this.dgvUsers.Size = new System.Drawing.Size(760, 320);
            this.dgvUsers.TabIndex = 1;
            this.dgvUsers.SelectionChanged += new System.EventHandler(this.dgvUsers_SelectionChanged);
            //
            // pnlForm
            //
            this.pnlForm.Controls.Add(this.lblStatus);
            this.pnlForm.Controls.Add(this.btnClose);
            this.pnlForm.Controls.Add(this.btnRefresh);
            this.pnlForm.Controls.Add(this.btnDelete);
            this.pnlForm.Controls.Add(this.btnNewUser);
            this.pnlForm.Controls.Add(this.btnSave);
            this.pnlForm.Controls.Add(this.txtFullName);
            this.pnlForm.Controls.Add(this.lblFullName);
            this.pnlForm.Controls.Add(this.txtCode);
            this.pnlForm.Controls.Add(this.lblCode);
            this.pnlForm.Controls.Add(this.lblTitle);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlForm.Location = new System.Drawing.Point(0, 0);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new System.Drawing.Size(760, 130);
            this.pnlForm.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Manage Users";
            //
            // lblCode
            //
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(12, 45);
            this.lblCode.Name = "lblCode";
            this.lblCode.Text = "Login Code";
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
            this.txtFullName.Size = new System.Drawing.Size(260, 23);
            this.txtFullName.TabIndex = 1;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(465, 62);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnNewUser
            //
            this.btnNewUser.Location = new System.Drawing.Point(585, 62);
            this.btnNewUser.Name = "btnNewUser";
            this.btnNewUser.Size = new System.Drawing.Size(110, 27);
            this.btnNewUser.TabIndex = 3;
            this.btnNewUser.Text = "New / Clear";
            this.btnNewUser.Click += new System.EventHandler(this.btnNewUser_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(12, 95);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(140, 27);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete Selected";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(585, 95);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 27);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh (F5)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(465, 95);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 27);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblStatus.Location = new System.Drawing.Point(160, 100);
            this.lblStatus.MaximumSize = new System.Drawing.Size(300, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "";
            //
            // ManageUsersForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 450);
            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.pnlForm);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "ManageUsersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Users";
            this.KeyPreview = true;
            this.Load += new System.EventHandler(this.ManageUsersForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ManageUsersForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.pnlForm.ResumeLayout(false);
            this.pnlForm.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
