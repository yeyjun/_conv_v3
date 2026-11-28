namespace Inventory
{
    partial class AddItemForm
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
            this.lbl_title = new System.Windows.Forms.Label();
            this.lbl_code = new System.Windows.Forms.Label();
            this.tbx_code = new System.Windows.Forms.TextBox();
            this.btn_generateCode = new System.Windows.Forms.Button();
            this.btn_printBarcode = new System.Windows.Forms.Button();
            this.lbl_description = new System.Windows.Forms.Label();
            this.tbx_description = new System.Windows.Forms.TextBox();
            this.lbl_category = new System.Windows.Forms.Label();
            this.cbx_category = new System.Windows.Forms.ComboBox();
            this.lbl_price = new System.Windows.Forms.Label();
            this.tbx_price = new System.Windows.Forms.TextBox();
            this.lbl_retailPrice = new System.Windows.Forms.Label();
            this.tbx_retailPrice = new System.Windows.Forms.TextBox();
            this.lbl_pricePerPc = new System.Windows.Forms.Label();
            this.tbx_pricePerPc = new System.Windows.Forms.TextBox();
            this.lbl_quantity = new System.Windows.Forms.Label();
            this.tbx_quantity = new System.Windows.Forms.TextBox();
            this.lbl_wholeNumberHint = new System.Windows.Forms.Label();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.picItemPhoto = new System.Windows.Forms.PictureBox();
            this.lblItemPhotoStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picItemPhoto)).BeginInit();
            this.SuspendLayout();
            //
            // lbl_title
            //
            this.lbl_title.AutoSize = true;
            this.lbl_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_title.Location = new System.Drawing.Point(16, 15);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(238, 29);
            this.lbl_title.TabIndex = 0;
            this.lbl_title.Text = "Add New Item";
            //
            // lbl_code
            //
            this.lbl_code.AutoSize = true;
            this.lbl_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_code.Location = new System.Drawing.Point(16, 70);
            this.lbl_code.Name = "lbl_code";
            this.lbl_code.Size = new System.Drawing.Size(76, 20);
            this.lbl_code.TabIndex = 1;
            this.lbl_code.Text = "Barcode:";
            //
            // tbx_code
            //
            this.tbx_code.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tbx_code.Location = new System.Drawing.Point(210, 66);
            this.tbx_code.Name = "tbx_code";
            this.tbx_code.Size = new System.Drawing.Size(220, 29);
            this.tbx_code.TabIndex = 2;
            this.tbx_code.Leave += new System.EventHandler(this.tbx_code_Leave);
            this.tbx_code.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbx_code_KeyDown);
            //
            // btn_generateCode
            //
            this.btn_generateCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btn_generateCode.Location = new System.Drawing.Point(210, 102);
            this.btn_generateCode.Name = "btn_generateCode";
            this.btn_generateCode.Size = new System.Drawing.Size(160, 36);
            this.btn_generateCode.TabIndex = 3;
            this.btn_generateCode.Text = "Auto-generate";
            this.btn_generateCode.UseVisualStyleBackColor = true;
            this.btn_generateCode.Click += new System.EventHandler(this.btn_generateCode_Click);
            //
            // btn_printBarcode
            //
            this.btn_printBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btn_printBarcode.Location = new System.Drawing.Point(380, 102);
            this.btn_printBarcode.Name = "btn_printBarcode";
            this.btn_printBarcode.Size = new System.Drawing.Size(160, 36);
            this.btn_printBarcode.TabIndex = 4;
            this.btn_printBarcode.Text = "Print Barcode";
            this.btn_printBarcode.UseVisualStyleBackColor = true;
            this.btn_printBarcode.Click += new System.EventHandler(this.btn_printBarcode_Click);
            //
            // lbl_description
            //
            this.lbl_description.AutoSize = true;
            this.lbl_description.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_description.Location = new System.Drawing.Point(16, 152);
            this.lbl_description.Name = "lbl_description";
            this.lbl_description.Size = new System.Drawing.Size(107, 20);
            this.lbl_description.TabIndex = 5;
            this.lbl_description.Text = "Description:";
            //
            // tbx_description
            //
            this.tbx_description.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tbx_description.Location = new System.Drawing.Point(210, 148);
            this.tbx_description.Name = "tbx_description";
            this.tbx_description.Size = new System.Drawing.Size(330, 29);
            this.tbx_description.TabIndex = 6;
            //
            // lbl_category
            //
            this.lbl_category.AutoSize = true;
            this.lbl_category.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_category.Location = new System.Drawing.Point(16, 196);
            this.lbl_category.Name = "lbl_category";
            this.lbl_category.Size = new System.Drawing.Size(84, 20);
            this.lbl_category.TabIndex = 7;
            this.lbl_category.Text = "Category:";
            //
            // cbx_category
            //
            this.cbx_category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cbx_category.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.cbx_category.FormattingEnabled = true;
            this.cbx_category.Location = new System.Drawing.Point(210, 192);
            this.cbx_category.Name = "cbx_category";
            this.cbx_category.Size = new System.Drawing.Size(330, 29);
            this.cbx_category.TabIndex = 8;
            //
            // lbl_price
            //
            this.lbl_price.AutoSize = true;
            this.lbl_price.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_price.Location = new System.Drawing.Point(16, 240);
            this.lbl_price.Name = "lbl_price";
            this.lbl_price.Size = new System.Drawing.Size(48, 20);
            this.lbl_price.TabIndex = 9;
            this.lbl_price.Text = "Price:";
            //
            // tbx_price
            //
            this.tbx_price.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tbx_price.Location = new System.Drawing.Point(210, 236);
            this.tbx_price.Name = "tbx_price";
            this.tbx_price.Size = new System.Drawing.Size(220, 29);
            this.tbx_price.TabIndex = 10;
            this.tbx_price.Text = "0";
            this.tbx_price.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WholeNumberTextBox_KeyPress);
            //
            // lbl_retailPrice
            //
            this.lbl_retailPrice.AutoSize = true;
            this.lbl_retailPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_retailPrice.Location = new System.Drawing.Point(16, 284);
            this.lbl_retailPrice.Name = "lbl_retailPrice";
            this.lbl_retailPrice.Size = new System.Drawing.Size(98, 20);
            this.lbl_retailPrice.TabIndex = 11;
            this.lbl_retailPrice.Text = "Retail Price:";
            //
            // tbx_retailPrice
            //
            this.tbx_retailPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tbx_retailPrice.Location = new System.Drawing.Point(210, 280);
            this.tbx_retailPrice.Name = "tbx_retailPrice";
            this.tbx_retailPrice.Size = new System.Drawing.Size(220, 29);
            this.tbx_retailPrice.TabIndex = 12;
            this.tbx_retailPrice.Text = "0";
            this.tbx_retailPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WholeNumberTextBox_KeyPress);
            //
            // lbl_pricePerPc
            //
            this.lbl_pricePerPc.AutoSize = true;
            this.lbl_pricePerPc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_pricePerPc.Location = new System.Drawing.Point(16, 328);
            this.lbl_pricePerPc.Name = "lbl_pricePerPc";
            this.lbl_pricePerPc.Size = new System.Drawing.Size(139, 20);
            this.lbl_pricePerPc.TabIndex = 13;
            this.lbl_pricePerPc.Text = "Price Per Piece:";
            //
            // tbx_pricePerPc
            //
            this.tbx_pricePerPc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tbx_pricePerPc.Location = new System.Drawing.Point(210, 324);
            this.tbx_pricePerPc.Name = "tbx_pricePerPc";
            this.tbx_pricePerPc.Size = new System.Drawing.Size(220, 29);
            this.tbx_pricePerPc.TabIndex = 14;
            this.tbx_pricePerPc.Text = "0";
            this.tbx_pricePerPc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WholeNumberTextBox_KeyPress);
            //
            // lbl_quantity
            //
            this.lbl_quantity.AutoSize = true;
            this.lbl_quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lbl_quantity.Location = new System.Drawing.Point(16, 372);
            this.lbl_quantity.Name = "lbl_quantity";
            this.lbl_quantity.Size = new System.Drawing.Size(155, 20);
            this.lbl_quantity.TabIndex = 15;
            this.lbl_quantity.Text = "Starting Quantity:";
            //
            // tbx_quantity
            //
            this.tbx_quantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tbx_quantity.Location = new System.Drawing.Point(210, 368);
            this.tbx_quantity.Name = "tbx_quantity";
            this.tbx_quantity.Size = new System.Drawing.Size(220, 29);
            this.tbx_quantity.TabIndex = 16;
            this.tbx_quantity.Text = "0";
            this.tbx_quantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WholeNumberTextBox_KeyPress);
            //
            // lbl_wholeNumberHint
            //
            this.lbl_wholeNumberHint.AutoSize = true;
            this.lbl_wholeNumberHint.ForeColor = System.Drawing.Color.DimGray;
            this.lbl_wholeNumberHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Italic);
            this.lbl_wholeNumberHint.Location = new System.Drawing.Point(16, 410);
            this.lbl_wholeNumberHint.Name = "lbl_wholeNumberHint";
            this.lbl_wholeNumberHint.Size = new System.Drawing.Size(420, 20);
            this.lbl_wholeNumberHint.TabIndex = 17;
            this.lbl_wholeNumberHint.Text = "Prices and quantity must be whole numbers (no decimals).";
            //
            // btn_save
            //
            this.btn_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btn_save.Location = new System.Drawing.Point(210, 442);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(160, 42);
            this.btn_save.TabIndex = 18;
            this.btn_save.Text = "Save Item";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            //
            // btn_close
            //
            this.btn_close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btn_close.Location = new System.Drawing.Point(380, 442);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(160, 42);
            this.btn_close.TabIndex = 19;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            //
            // picItemPhoto
            //
            this.picItemPhoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picItemPhoto.Location = new System.Drawing.Point(560, 66);
            this.picItemPhoto.Name = "picItemPhoto";
            this.picItemPhoto.Size = new System.Drawing.Size(160, 160);
            this.picItemPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picItemPhoto.TabIndex = 20;
            this.picItemPhoto.TabStop = false;
            //
            // lblItemPhotoStatus
            //
            this.lblItemPhotoStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblItemPhotoStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblItemPhotoStatus.Location = new System.Drawing.Point(560, 230);
            this.lblItemPhotoStatus.MaximumSize = new System.Drawing.Size(160, 0);
            this.lblItemPhotoStatus.Name = "lblItemPhotoStatus";
            this.lblItemPhotoStatus.Size = new System.Drawing.Size(160, 40);
            this.lblItemPhotoStatus.TabIndex = 21;
            this.lblItemPhotoStatus.Text = "Sample photo looked up by barcode once entered.";
            //
            // AddItemForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 504);
            this.Controls.Add(this.lblItemPhotoStatus);
            this.Controls.Add(this.picItemPhoto);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.lbl_wholeNumberHint);
            this.Controls.Add(this.tbx_quantity);
            this.Controls.Add(this.lbl_quantity);
            this.Controls.Add(this.tbx_pricePerPc);
            this.Controls.Add(this.lbl_pricePerPc);
            this.Controls.Add(this.tbx_retailPrice);
            this.Controls.Add(this.lbl_retailPrice);
            this.Controls.Add(this.tbx_price);
            this.Controls.Add(this.lbl_price);
            this.Controls.Add(this.cbx_category);
            this.Controls.Add(this.lbl_category);
            this.Controls.Add(this.tbx_description);
            this.Controls.Add(this.lbl_description);
            this.Controls.Add(this.btn_printBarcode);
            this.Controls.Add(this.btn_generateCode);
            this.Controls.Add(this.tbx_code);
            this.Controls.Add(this.lbl_code);
            this.Controls.Add(this.lbl_title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddItemForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add New Item";
            ((System.ComponentModel.ISupportInitialize)(this.picItemPhoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_title;
        private System.Windows.Forms.Label lbl_code;
        private System.Windows.Forms.TextBox tbx_code;
        private System.Windows.Forms.Button btn_generateCode;
        private System.Windows.Forms.Button btn_printBarcode;
        private System.Windows.Forms.Label lbl_description;
        private System.Windows.Forms.TextBox tbx_description;
        private System.Windows.Forms.Label lbl_category;
        private System.Windows.Forms.ComboBox cbx_category;
        private System.Windows.Forms.Label lbl_price;
        private System.Windows.Forms.TextBox tbx_price;
        private System.Windows.Forms.Label lbl_retailPrice;
        private System.Windows.Forms.TextBox tbx_retailPrice;
        private System.Windows.Forms.Label lbl_pricePerPc;
        private System.Windows.Forms.TextBox tbx_pricePerPc;
        private System.Windows.Forms.Label lbl_quantity;
        private System.Windows.Forms.TextBox tbx_quantity;
        private System.Windows.Forms.Label lbl_wholeNumberHint;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.PictureBox picItemPhoto;
        private System.Windows.Forms.Label lblItemPhotoStatus;
    }
}
