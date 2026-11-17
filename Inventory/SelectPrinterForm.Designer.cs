namespace _conv
{
    partial class SelectPrinterForm
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.lstPrinters = new System.Windows.Forms.ListBox();
            this.btnSetPrinter = new System.Windows.Forms.Button();
            this.btnNotNow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(12, 15);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(360, 40);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "No receipt printer is set up yet. Choose the printer to use for printing and saving receipts:";
            //
            // lstPrinters
            //
            this.lstPrinters.FormattingEnabled = true;
            this.lstPrinters.ItemHeight = 16;
            this.lstPrinters.Location = new System.Drawing.Point(12, 60);
            this.lstPrinters.Name = "lstPrinters";
            this.lstPrinters.Size = new System.Drawing.Size(360, 132);
            this.lstPrinters.TabIndex = 1;
            //
            // btnSetPrinter
            //
            this.btnSetPrinter.Location = new System.Drawing.Point(12, 202);
            this.btnSetPrinter.Name = "btnSetPrinter";
            this.btnSetPrinter.Size = new System.Drawing.Size(175, 30);
            this.btnSetPrinter.TabIndex = 2;
            this.btnSetPrinter.Text = "Set as Receipt Printer";
            this.btnSetPrinter.UseVisualStyleBackColor = true;
            this.btnSetPrinter.Click += new System.EventHandler(this.btnSetPrinter_Click);
            //
            // btnNotNow
            //
            this.btnNotNow.Location = new System.Drawing.Point(197, 202);
            this.btnNotNow.Name = "btnNotNow";
            this.btnNotNow.Size = new System.Drawing.Size(175, 30);
            this.btnNotNow.TabIndex = 3;
            this.btnNotNow.Text = "Not Now";
            this.btnNotNow.UseVisualStyleBackColor = true;
            this.btnNotNow.Click += new System.EventHandler(this.btnNotNow_Click);
            //
            // SelectPrinterForm
            //
            this.AcceptButton = this.btnSetPrinter;
            this.CancelButton = this.btnNotNow;
            this.ClientSize = new System.Drawing.Size(384, 244);
            this.Controls.Add(this.btnNotNow);
            this.Controls.Add(this.btnSetPrinter);
            this.Controls.Add(this.lstPrinters);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectPrinterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Up Receipt Printer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ListBox lstPrinters;
        private System.Windows.Forms.Button btnSetPrinter;
        private System.Windows.Forms.Button btnNotNow;
    }
}
