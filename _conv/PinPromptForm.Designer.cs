namespace _conv
{
    partial class PinPromptForm
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
            this.tbx_pin = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lbl_prompt
            //
            this.lbl_prompt.AutoSize = true;
            this.lbl_prompt.Location = new System.Drawing.Point(12, 15);
            this.lbl_prompt.Name = "lbl_prompt";
            this.lbl_prompt.Size = new System.Drawing.Size(178, 13);
            this.lbl_prompt.TabIndex = 0;
            this.lbl_prompt.Text = "Enter manager PIN to continue:";
            //
            // tbx_pin
            //
            this.tbx_pin.Location = new System.Drawing.Point(15, 35);
            this.tbx_pin.Name = "tbx_pin";
            this.tbx_pin.PasswordChar = '*';
            this.tbx_pin.Size = new System.Drawing.Size(220, 22);
            this.tbx_pin.TabIndex = 1;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(15, 68);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 30);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(135, 68);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // PinPromptForm
            //
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(250, 112);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.tbx_pin);
            this.Controls.Add(this.lbl_prompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PinPromptForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Restricted Access";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_prompt;
        private System.Windows.Forms.TextBox tbx_pin;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
