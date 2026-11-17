namespace Inventory
{
    partial class Login
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
            this.tbx_login = new System.Windows.Forms.TextBox();
            this.lblEnterAccount = new System.Windows.Forms.Label();
            this.lblEnterHint = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tbx_login
            // 
            this.tbx_login.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbx_login.Location = new System.Drawing.Point(26, 90);
            this.tbx_login.Name = "tbx_login";
            this.tbx_login.PasswordChar = '*';
            this.tbx_login.Size = new System.Drawing.Size(270, 44);
            this.tbx_login.TabIndex = 0;
            this.tbx_login.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbx_login.TextChanged += new System.EventHandler(this.tbx_login_TextChanged);
            this.tbx_login.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbx_login_KeyUp);
            this.tbx_login.Validated += new System.EventHandler(this.tbx_login_Validated);
            // 
            // lblEnterAccount
            // 
            this.lblEnterAccount.AutoSize = true;
            this.lblEnterAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnterAccount.Location = new System.Drawing.Point(23, 70);
            this.lblEnterAccount.Name = "lblEnterAccount";
            this.lblEnterAccount.Size = new System.Drawing.Size(168, 17);
            this.lblEnterAccount.TabIndex = 1;
            this.lblEnterAccount.Text = "Enter account number";
            // 
            // lblEnterHint
            // 
            this.lblEnterHint.AutoSize = true;
            this.lblEnterHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnterHint.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblEnterHint.Location = new System.Drawing.Point(23, 137);
            this.lblEnterHint.Name = "lblEnterHint";
            this.lblEnterHint.Size = new System.Drawing.Size(117, 13);
            this.lblEnterHint.TabIndex = 2;
            this.lblEnterHint.Text = "Press Enter to continue";
            // 
            // picLogo
            // 
            this.picLogo.ImageLocation = "https://drive.google.com/thumbnail?id=1qVDEd-pMDSposIfi8oBN4ceF7XXqdkTu";
            this.picLogo.Location = new System.Drawing.Point(189, -1);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(93, 88);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 3;
            this.picLogo.TabStop = false;
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 253);
            this.Controls.Add(this.lblEnterHint);
            this.Controls.Add(this.lblEnterAccount);
            this.Controls.Add(this.tbx_login);
            this.Controls.Add(this.picLogo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Login_FormClosed);
            this.Load += new System.EventHandler(this.Login_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Login_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblEnterAccount;
        public System.Windows.Forms.TextBox tbx_login;
        private System.Windows.Forms.Label lblEnterHint;
        private System.Windows.Forms.PictureBox picLogo;
    }
}