namespace _conv
{
    partial class HelpForm
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
            this.components = new System.ComponentModel.Container();
            this.rtxtShortcuts = new System.Windows.Forms.RichTextBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.printDocumentHelp = new System.Drawing.Printing.PrintDocument();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // rtxtShortcuts
            //
            this.rtxtShortcuts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtShortcuts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtShortcuts.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.rtxtShortcuts.Location = new System.Drawing.Point(0, 0);
            this.rtxtShortcuts.Name = "rtxtShortcuts";
            this.rtxtShortcuts.ReadOnly = true;
            this.rtxtShortcuts.Size = new System.Drawing.Size(520, 440);
            this.rtxtShortcuts.TabIndex = 0;
            this.rtxtShortcuts.Text = "";
            this.rtxtShortcuts.WordWrap = false;
            //
            // pnlButtons
            //
            this.pnlButtons.Controls.Add(this.btnPrint);
            this.pnlButtons.Controls.Add(this.btnClose);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 440);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(520, 50);
            this.pnlButtons.TabIndex = 1;
            //
            // btnPrint
            //
            this.btnPrint.Location = new System.Drawing.Point(15, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(110, 30);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "Print...";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(395, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 30);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // printDocumentHelp
            //
            this.printDocumentHelp.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocumentHelp_BeginPrint);
            this.printDocumentHelp.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocumentHelp_PrintPage);
            //
            // HelpForm
            //
            this.AcceptButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(520, 490);
            this.Controls.Add(this.rtxtShortcuts);
            this.Controls.Add(this.pnlButtons);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "HelpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Keyboard Shortcuts";
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtShortcuts;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Drawing.Printing.PrintDocument printDocumentHelp;
    }
}
