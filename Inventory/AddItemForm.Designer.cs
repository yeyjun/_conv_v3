namespace _conv
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
            this.lbl_price = new System.Windows.Forms.Label();
            this.tbx_price = new System.Windows.Forms.TextBox();
            this.lbl_retailPrice = new System.Windows.Forms.Label();
            this.tbx_retailPrice = new System.Windows.Forms.TextBox();
            this.lbl_pricePerPc = new System.Windows.Forms.Label();
            this.tbx_pricePerPc = new System.Windows.Forms.TextBox();
            this.lbl_quantity = new System.Windows.Forms.Label();
            this.tbx_quantity = new System.Windows.Forms.TextBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lbl_title
            //
            this.lbl_title.AutoSize = true;
            this.lbl_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_title.Location = new System.Drawing.Point(12, 12);
            this.lbl_title.Name = "lbl_title";
            this.lbl_title.Size = new System.Drawing.Size(160, 20);
            this.lbl_title.TabIndex = 0;
            this.lbl_title.Text = "Add New Item";
            //
            // lbl_code
            //
            this.lbl_code.AutoSize = true;
            this.lbl_code.Location = new System.Drawing.Point(12, 48);
            this.lbl_code.Name = "lbl_code";
            this.lbl_code.Size = new System.Drawing.Size(70, 13);
            this.lbl_code.TabIndex = 1;
            this.lbl_code.Text = "Barcode:";
            //
            // tbx_code
            //
            this.tbx_code.Location = new System.Drawing.Point(140, 45);
            this.tbx_code.Name = "tbx_code";
            this.tbx_code.Size = new System.Drawing.Size(150, 20);
            this.tbx_code.TabIndex = 2;
            //
            // btn_generateCode
            //
            this.btn_generateCode.Location = new System.Drawing.Point(140, 71);
            this.btn_generateCode.Name = "btn_generateCode";
            this.btn_generateCode.Size = new System.Drawing.Size(110, 26);
            this.btn_generateCode.TabIndex = 3;
            this.btn_generateCode.Text = "Auto-generate";
            this.btn_generateCode.UseVisualStyleBackColor = true;
            this.btn_generateCode.Click += new System.EventHandler(this.btn_generateCode_Click);
            //
            // btn_printBarcode
            //
            this.btn_printBarcode.Location = new System.Drawing.Point(256, 71);
            this.btn_printBarcode.Name = "btn_printBarcode";
            this.btn_printBarcode.Size = new System.Drawing.Size(110, 26);
            this.btn_printBarcode.TabIndex = 4;
            this.btn_printBarcode.Text = "Print Barcode";
            this.btn_printBarcode.UseVisualStyleBackColor = true;
            this.btn_printBarcode.Click += new System.EventHandler(this.btn_printBarcode_Click);
            //
            // lbl_description
            //
            this.lbl_description.AutoSize = true;
            this.lbl_description.Location = new System.Drawing.Point(12, 108);
            this.lbl_description.Name = "lbl_description";
            this.lbl_description.Size = new System.Drawing.Size(70, 13);
            this.lbl_description.TabIndex = 5;
            this.lbl_description.Text = "Description:";
            //
            // tbx_description
            //
            this.tbx_description.Location = new System.Drawing.Point(140, 105);
            this.tbx_description.Name = "tbx_description";
            this.tbx_description.Size = new System.Drawing.Size(226, 20);
            this.tbx_description.TabIndex = 6;
            //
            // lbl_price
            //
            this.lbl_price.AutoSize = true;
            this.lbl_price.Location = new System.Drawing.Point(12, 138);
            this.lbl_price.Name = "lbl_price";
            this.lbl_price.Size = new System.Drawing.Size(70, 13);
            this.lbl_price.TabIndex = 7;
            this.lbl_price.Text = "Price:";
            //
            // tbx_price
            //
            this.tbx_price.Location = new System.Drawing.Point(140, 135);
            this.tbx_price.Name = "tbx_price";
            this.tbx_price.Size = new System.Drawing.Size(150, 20);
            this.tbx_price.TabIndex = 8;
            this.tbx_price.Text = "0";
            //
            // lbl_retailPrice
            //
            this.lbl_retailPrice.AutoSize = true;
            this.lbl_retailPrice.Location = new System.Drawing.Point(12, 168);
            this.lbl_retailPrice.Name = "lbl_retailPrice";
            this.lbl_retailPrice.Size = new System.Drawing.Size(70, 13);
            this.lbl_retailPrice.TabIndex = 9;
            this.lbl_retailPrice.Text = "Retail Price:";
            //
            // tbx_retailPrice
            //
            this.tbx_retailPrice.Location = new System.Drawing.Point(140, 165);
            this.tbx_retailPrice.Name = "tbx_retailPrice";
            this.tbx_retailPrice.Size = new System.Drawing.Size(150, 20);
            this.tbx_retailPrice.TabIndex = 10;
            this.tbx_retailPrice.Text = "0";
            //
            // lbl_pricePerPc
            //
            this.lbl_pricePerPc.AutoSize = true;
            this.lbl_pricePerPc.Location = new System.Drawing.Point(12, 198);
            this.lbl_pricePerPc.Name = "lbl_pricePerPc";
            this.lbl_pricePerPc.Size = new System.Drawing.Size(90, 13);
            this.lbl_pricePerPc.TabIndex = 11;
            this.lbl_pricePerPc.Text = "Price Per Piece:";
            //
            // tbx_pricePerPc
            //
            this.tbx_pricePerPc.Location = new System.Drawing.Point(140, 195);
            this.tbx_pricePerPc.Name = "tbx_pricePerPc";
            this.tbx_pricePerPc.Size = new System.Drawing.Size(150, 20);
            this.tbx_pricePerPc.TabIndex = 12;
            this.tbx_pricePerPc.Text = "0";
            //
            // lbl_quantity
            //
            this.lbl_quantity.AutoSize = true;
            this.lbl_quantity.Location = new System.Drawing.Point(12, 228);
            this.lbl_quantity.Name = "lbl_quantity";
            this.lbl_quantity.Size = new System.Drawing.Size(100, 13);
            this.lbl_quantity.TabIndex = 13;
            this.lbl_quantity.Text = "Starting Quantity:";
            //
            // tbx_quantity
            //
            this.tbx_quantity.Location = new System.Drawing.Point(140, 225);
            this.tbx_quantity.Name = "tbx_quantity";
            this.tbx_quantity.Size = new System.Drawing.Size(150, 20);
            this.tbx_quantity.TabIndex = 14;
            this.tbx_quantity.Text = "0";
            //
            // btn_save
            //
            this.btn_save.Location = new System.Drawing.Point(140, 265);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(110, 30);
            this.btn_save.TabIndex = 15;
            this.btn_save.Text = "Save Item";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            //
            // btn_close
            //
            this.btn_close.Location = new System.Drawing.Point(256, 265);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(110, 30);
            this.btn_close.TabIndex = 16;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            //
            // AddItemForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 312);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.tbx_quantity);
            this.Controls.Add(this.lbl_quantity);
            this.Controls.Add(this.tbx_pricePerPc);
            this.Controls.Add(this.lbl_pricePerPc);
            this.Controls.Add(this.tbx_retailPrice);
            this.Controls.Add(this.lbl_retailPrice);
            this.Controls.Add(this.tbx_price);
            this.Controls.Add(this.lbl_price);
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
        private System.Windows.Forms.Label lbl_price;
        private System.Windows.Forms.TextBox tbx_price;
        private System.Windows.Forms.Label lbl_retailPrice;
        private System.Windows.Forms.TextBox tbx_retailPrice;
        private System.Windows.Forms.Label lbl_pricePerPc;
        private System.Windows.Forms.TextBox tbx_pricePerPc;
        private System.Windows.Forms.Label lbl_quantity;
        private System.Windows.Forms.TextBox tbx_quantity;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_close;
    }
}
