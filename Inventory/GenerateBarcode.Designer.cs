namespace Inventory
{
    partial class GenerateBarcode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerateBarcode));
            this.barcoder = new System.Windows.Forms.PictureBox();
            this.lbl_printStatus = new System.Windows.Forms.Label();
            this.btn_print = new System.Windows.Forms.Button();
            this.printPreviewDialogBarcode = new System.Windows.Forms.PrintPreviewDialog();
            this.printDocumentBarcode = new System.Drawing.Printing.PrintDocument();
            this.gbx_item = new System.Windows.Forms.GroupBox();
            this.lbl_itemDescription = new System.Windows.Forms.Label();
            this.btn_findItem = new System.Windows.Forms.Button();
            this.tbx_itemLookup = new System.Windows.Forms.TextBox();
            this.lbl_lookup = new System.Windows.Forms.Label();
            this.gbx_barcodeOptions = new System.Windows.Forms.GroupBox();
            this.btn_generate = new System.Windows.Forms.Button();
            this.chk_includeText = new System.Windows.Forms.CheckBox();
            this.num_height = new System.Windows.Forms.NumericUpDown();
            this.lbl_height = new System.Windows.Forms.Label();
            this.num_width = new System.Windows.Forms.NumericUpDown();
            this.lbl_width = new System.Windows.Forms.Label();
            this.cmb_barcodeType = new System.Windows.Forms.ComboBox();
            this.lbl_type = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.barcoder)).BeginInit();
            this.gbx_item.SuspendLayout();
            this.gbx_barcodeOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_width)).BeginInit();
            this.SuspendLayout();
            //
            // barcoder
            //
            this.barcoder.Location = new System.Drawing.Point(48, 232);
            this.barcoder.Name = "barcoder";
            this.barcoder.Size = new System.Drawing.Size(316, 155);
            this.barcoder.TabIndex = 0;
            this.barcoder.TabStop = false;
            //
            // lbl_printStatus
            //
            this.lbl_printStatus.AutoSize = true;
            this.lbl_printStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_printStatus.Location = new System.Drawing.Point(48, 394);
            this.lbl_printStatus.Name = "lbl_printStatus";
            this.lbl_printStatus.Size = new System.Drawing.Size(58, 22);
            this.lbl_printStatus.TabIndex = 1;
            this.lbl_printStatus.Text = "lbl_printStatus";
            this.lbl_printStatus.Visible = false;
            //
            // btn_print
            //
            this.btn_print.Location = new System.Drawing.Point(289, 393);
            this.btn_print.Name = "btn_print";
            this.btn_print.Size = new System.Drawing.Size(75, 23);
            this.btn_print.TabIndex = 8;
            this.btn_print.Text = "Print";
            this.btn_print.UseVisualStyleBackColor = true;
            this.btn_print.Click += new System.EventHandler(this.btn_print_Click);
            //
            // printPreviewDialogBarcode
            //
            this.printPreviewDialogBarcode.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialogBarcode.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialogBarcode.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialogBarcode.Enabled = true;
            this.printPreviewDialogBarcode.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialogBarcode.Icon")));
            this.printPreviewDialogBarcode.Name = "printPreviewDialogBarcode";
            this.printPreviewDialogBarcode.Visible = false;
            //
            // printDocumentBarcode
            //
            this.printDocumentBarcode.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocumentBarcode_PrintPage);
            //
            // gbx_item
            //
            this.gbx_item.Controls.Add(this.lbl_itemDescription);
            this.gbx_item.Controls.Add(this.btn_findItem);
            this.gbx_item.Controls.Add(this.tbx_itemLookup);
            this.gbx_item.Controls.Add(this.lbl_lookup);
            this.gbx_item.Location = new System.Drawing.Point(12, 12);
            this.gbx_item.Name = "gbx_item";
            this.gbx_item.Size = new System.Drawing.Size(436, 84);
            this.gbx_item.TabIndex = 2;
            this.gbx_item.TabStop = false;
            this.gbx_item.Text = "Item";
            //
            // lbl_itemDescription
            //
            this.lbl_itemDescription.AutoSize = true;
            this.lbl_itemDescription.Location = new System.Drawing.Point(13, 58);
            this.lbl_itemDescription.Name = "lbl_itemDescription";
            this.lbl_itemDescription.Size = new System.Drawing.Size(0, 13);
            this.lbl_itemDescription.TabIndex = 3;
            //
            // btn_findItem
            //
            this.btn_findItem.Location = new System.Drawing.Point(340, 30);
            this.btn_findItem.Name = "btn_findItem";
            this.btn_findItem.Size = new System.Drawing.Size(80, 23);
            this.btn_findItem.TabIndex = 2;
            this.btn_findItem.Text = "Find";
            this.btn_findItem.UseVisualStyleBackColor = true;
            this.btn_findItem.Click += new System.EventHandler(this.btn_findItem_Click);
            //
            // tbx_itemLookup
            //
            this.tbx_itemLookup.Location = new System.Drawing.Point(16, 31);
            this.tbx_itemLookup.Name = "tbx_itemLookup";
            this.tbx_itemLookup.Size = new System.Drawing.Size(310, 20);
            this.tbx_itemLookup.TabIndex = 1;
            this.tbx_itemLookup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbx_itemLookup_KeyDown);
            //
            // lbl_lookup
            //
            this.lbl_lookup.AutoSize = true;
            this.lbl_lookup.Location = new System.Drawing.Point(13, 14);
            this.lbl_lookup.Name = "lbl_lookup";
            this.lbl_lookup.Size = new System.Drawing.Size(150, 13);
            this.lbl_lookup.TabIndex = 0;
            this.lbl_lookup.Text = "Serial code or item name:";
            //
            // gbx_barcodeOptions
            //
            this.gbx_barcodeOptions.Controls.Add(this.btn_generate);
            this.gbx_barcodeOptions.Controls.Add(this.chk_includeText);
            this.gbx_barcodeOptions.Controls.Add(this.num_height);
            this.gbx_barcodeOptions.Controls.Add(this.lbl_height);
            this.gbx_barcodeOptions.Controls.Add(this.num_width);
            this.gbx_barcodeOptions.Controls.Add(this.lbl_width);
            this.gbx_barcodeOptions.Controls.Add(this.cmb_barcodeType);
            this.gbx_barcodeOptions.Controls.Add(this.lbl_type);
            this.gbx_barcodeOptions.Location = new System.Drawing.Point(12, 102);
            this.gbx_barcodeOptions.Name = "gbx_barcodeOptions";
            this.gbx_barcodeOptions.Size = new System.Drawing.Size(436, 116);
            this.gbx_barcodeOptions.TabIndex = 3;
            this.gbx_barcodeOptions.TabStop = false;
            this.gbx_barcodeOptions.Text = "Barcode options";
            //
            // btn_generate
            //
            this.btn_generate.Location = new System.Drawing.Point(320, 78);
            this.btn_generate.Name = "btn_generate";
            this.btn_generate.Size = new System.Drawing.Size(100, 25);
            this.btn_generate.TabIndex = 7;
            this.btn_generate.Text = "Generate";
            this.btn_generate.UseVisualStyleBackColor = true;
            this.btn_generate.Click += new System.EventHandler(this.btn_generate_Click);
            //
            // chk_includeText
            //
            this.chk_includeText.AutoSize = true;
            this.chk_includeText.Checked = true;
            this.chk_includeText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_includeText.Location = new System.Drawing.Point(160, 56);
            this.chk_includeText.Name = "chk_includeText";
            this.chk_includeText.Size = new System.Drawing.Size(89, 17);
            this.chk_includeText.TabIndex = 6;
            this.chk_includeText.Text = "Include text";
            this.chk_includeText.UseVisualStyleBackColor = true;
            //
            // num_height
            //
            this.num_height.Location = new System.Drawing.Point(70, 54);
            this.num_height.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.num_height.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            this.num_height.Name = "num_height";
            this.num_height.Size = new System.Drawing.Size(70, 20);
            this.num_height.TabIndex = 5;
            this.num_height.Value = new decimal(new int[] { 50, 0, 0, 0 });
            //
            // lbl_height
            //
            this.lbl_height.AutoSize = true;
            this.lbl_height.Location = new System.Drawing.Point(13, 58);
            this.lbl_height.Name = "lbl_height";
            this.lbl_height.Size = new System.Drawing.Size(41, 13);
            this.lbl_height.TabIndex = 4;
            this.lbl_height.Text = "Height:";
            //
            // num_width
            //
            this.num_width.Location = new System.Drawing.Point(280, 22);
            this.num_width.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            this.num_width.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            this.num_width.Name = "num_width";
            this.num_width.Size = new System.Drawing.Size(70, 20);
            this.num_width.TabIndex = 3;
            this.num_width.Value = new decimal(new int[] { 150, 0, 0, 0 });
            //
            // lbl_width
            //
            this.lbl_width.AutoSize = true;
            this.lbl_width.Location = new System.Drawing.Point(223, 26);
            this.lbl_width.Name = "lbl_width";
            this.lbl_width.Size = new System.Drawing.Size(38, 13);
            this.lbl_width.TabIndex = 2;
            this.lbl_width.Text = "Width:";
            //
            // cmb_barcodeType
            //
            this.cmb_barcodeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_barcodeType.FormattingEnabled = true;
            this.cmb_barcodeType.Location = new System.Drawing.Point(70, 22);
            this.cmb_barcodeType.Name = "cmb_barcodeType";
            this.cmb_barcodeType.Size = new System.Drawing.Size(130, 21);
            this.cmb_barcodeType.TabIndex = 1;
            //
            // lbl_type
            //
            this.lbl_type.AutoSize = true;
            this.lbl_type.Location = new System.Drawing.Point(13, 26);
            this.lbl_type.Name = "lbl_type";
            this.lbl_type.Size = new System.Drawing.Size(34, 13);
            this.lbl_type.TabIndex = 0;
            this.lbl_type.Text = "Type:";
            //
            // GenerateBarcode
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 430);
            this.Controls.Add(this.gbx_barcodeOptions);
            this.Controls.Add(this.gbx_item);
            this.Controls.Add(this.btn_print);
            this.Controls.Add(this.lbl_printStatus);
            this.Controls.Add(this.barcoder);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerateBarcode";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GenerateBarcode";
            this.Load += new System.EventHandler(this.GenerateBarcode_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barcoder)).EndInit();
            this.gbx_item.ResumeLayout(false);
            this.gbx_item.PerformLayout();
            this.gbx_barcodeOptions.ResumeLayout(false);
            this.gbx_barcodeOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_width)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox barcoder;
        private System.Windows.Forms.Label lbl_printStatus;
        private System.Windows.Forms.Button btn_print;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialogBarcode;
        private System.Drawing.Printing.PrintDocument printDocumentBarcode;
        private System.Windows.Forms.GroupBox gbx_item;
        private System.Windows.Forms.Label lbl_itemDescription;
        private System.Windows.Forms.Button btn_findItem;
        private System.Windows.Forms.TextBox tbx_itemLookup;
        private System.Windows.Forms.Label lbl_lookup;
        private System.Windows.Forms.GroupBox gbx_barcodeOptions;
        private System.Windows.Forms.Button btn_generate;
        private System.Windows.Forms.CheckBox chk_includeText;
        private System.Windows.Forms.NumericUpDown num_height;
        private System.Windows.Forms.Label lbl_height;
        private System.Windows.Forms.NumericUpDown num_width;
        private System.Windows.Forms.Label lbl_width;
        private System.Windows.Forms.ComboBox cmb_barcodeType;
        private System.Windows.Forms.Label lbl_type;
    }
}
