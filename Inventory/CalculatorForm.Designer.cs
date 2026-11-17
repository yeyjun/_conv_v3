namespace Inventory
{
    partial class CalculatorForm
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

        private System.Windows.Forms.Label lblNotice;
        private System.Windows.Forms.TextBox txtDisplay;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnBackspace;
        private System.Windows.Forms.Button btnPercent;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btn7;
        private System.Windows.Forms.Button btn8;
        private System.Windows.Forms.Button btn9;
        private System.Windows.Forms.Button btnMultiply;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Button btn6;
        private System.Windows.Forms.Button btnSubtract;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btn0;
        private System.Windows.Forms.Button btnDecimal;
        private System.Windows.Forms.Button btnEquals;

        private void InitializeComponent()
        {
            this.lblNotice = new System.Windows.Forms.Label();
            this.txtDisplay = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnBackspace = new System.Windows.Forms.Button();
            this.btnPercent = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn9 = new System.Windows.Forms.Button();
            this.btnMultiply = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btnSubtract = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btn0 = new System.Windows.Forms.Button();
            this.btnDecimal = new System.Windows.Forms.Button();
            this.btnEquals = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblNotice
            //
            this.lblNotice.AutoSize = false;
            this.lblNotice.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblNotice.ForeColor = System.Drawing.Color.DarkRed;
            this.lblNotice.Location = new System.Drawing.Point(12, 8);
            this.lblNotice.Name = "lblNotice";
            this.lblNotice.Size = new System.Drawing.Size(248, 16);
            this.lblNotice.TabIndex = 0;
            this.lblNotice.Text = "Cart's sum of prices is loaded below";
            this.lblNotice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblNotice.Visible = false;
            //
            // txtDisplay
            //
            this.txtDisplay.Font = new System.Drawing.Font("Consolas", 16F);
            this.txtDisplay.Location = new System.Drawing.Point(12, 32);
            this.txtDisplay.Name = "txtDisplay";
            this.txtDisplay.ReadOnly = true;
            this.txtDisplay.Size = new System.Drawing.Size(248, 32);
            this.txtDisplay.TabIndex = 1;
            this.txtDisplay.TabStop = false;
            this.txtDisplay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDisplay.Text = "0";
            //
            // btnClear
            //
            this.btnClear.Location = new System.Drawing.Point(12, 80);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(56, 45);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "C";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // btnBackspace
            //
            this.btnBackspace.Location = new System.Drawing.Point(76, 80);
            this.btnBackspace.Name = "btnBackspace";
            this.btnBackspace.Size = new System.Drawing.Size(56, 45);
            this.btnBackspace.TabIndex = 2;
            this.btnBackspace.Text = "⌫";
            this.btnBackspace.UseVisualStyleBackColor = true;
            this.btnBackspace.Click += new System.EventHandler(this.btnBackspace_Click);
            //
            // btnPercent
            //
            this.btnPercent.Location = new System.Drawing.Point(140, 80);
            this.btnPercent.Name = "btnPercent";
            this.btnPercent.Size = new System.Drawing.Size(56, 45);
            this.btnPercent.TabIndex = 3;
            this.btnPercent.Text = "%";
            this.btnPercent.UseVisualStyleBackColor = true;
            this.btnPercent.Click += new System.EventHandler(this.btnPercent_Click);
            //
            // btnDivide
            //
            this.btnDivide.Location = new System.Drawing.Point(204, 80);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(56, 45);
            this.btnDivide.TabIndex = 4;
            this.btnDivide.Text = "÷";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btn7
            //
            this.btn7.Location = new System.Drawing.Point(12, 133);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(56, 45);
            this.btn7.TabIndex = 5;
            this.btn7.Text = "7";
            this.btn7.UseVisualStyleBackColor = true;
            this.btn7.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btn8
            //
            this.btn8.Location = new System.Drawing.Point(76, 133);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(56, 45);
            this.btn8.TabIndex = 6;
            this.btn8.Text = "8";
            this.btn8.UseVisualStyleBackColor = true;
            this.btn8.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btn9
            //
            this.btn9.Location = new System.Drawing.Point(140, 133);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(56, 45);
            this.btn9.TabIndex = 7;
            this.btn9.Text = "9";
            this.btn9.UseVisualStyleBackColor = true;
            this.btn9.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btnMultiply
            //
            this.btnMultiply.Location = new System.Drawing.Point(204, 133);
            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.Size = new System.Drawing.Size(56, 45);
            this.btnMultiply.TabIndex = 8;
            this.btnMultiply.Text = "×";
            this.btnMultiply.UseVisualStyleBackColor = true;
            this.btnMultiply.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btn4
            //
            this.btn4.Location = new System.Drawing.Point(12, 186);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(56, 45);
            this.btn4.TabIndex = 9;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btn5
            //
            this.btn5.Location = new System.Drawing.Point(76, 186);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(56, 45);
            this.btn5.TabIndex = 10;
            this.btn5.Text = "5";
            this.btn5.UseVisualStyleBackColor = true;
            this.btn5.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btn6
            //
            this.btn6.Location = new System.Drawing.Point(140, 186);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(56, 45);
            this.btn6.TabIndex = 11;
            this.btn6.Text = "6";
            this.btn6.UseVisualStyleBackColor = true;
            this.btn6.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btnSubtract
            //
            this.btnSubtract.Location = new System.Drawing.Point(204, 186);
            this.btnSubtract.Name = "btnSubtract";
            this.btnSubtract.Size = new System.Drawing.Size(56, 45);
            this.btnSubtract.TabIndex = 12;
            this.btnSubtract.Text = "−";
            this.btnSubtract.UseVisualStyleBackColor = true;
            this.btnSubtract.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btn1
            //
            this.btn1.Location = new System.Drawing.Point(12, 239);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(56, 45);
            this.btn1.TabIndex = 13;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btn2
            //
            this.btn2.Location = new System.Drawing.Point(76, 239);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(56, 45);
            this.btn2.TabIndex = 14;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btn3
            //
            this.btn3.Location = new System.Drawing.Point(140, 239);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(56, 45);
            this.btn3.TabIndex = 15;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(204, 239);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(56, 45);
            this.btnAdd.TabIndex = 16;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btn0
            //
            this.btn0.Location = new System.Drawing.Point(12, 292);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(120, 45);
            this.btn0.TabIndex = 17;
            this.btn0.Text = "0";
            this.btn0.UseVisualStyleBackColor = true;
            this.btn0.Click += new System.EventHandler(this.btnDigit_Click);
            //
            // btnDecimal
            //
            this.btnDecimal.Location = new System.Drawing.Point(140, 292);
            this.btnDecimal.Name = "btnDecimal";
            this.btnDecimal.Size = new System.Drawing.Size(56, 45);
            this.btnDecimal.TabIndex = 18;
            this.btnDecimal.Text = ".";
            this.btnDecimal.UseVisualStyleBackColor = true;
            this.btnDecimal.Click += new System.EventHandler(this.btnDecimal_Click);
            //
            // btnEquals
            //
            this.btnEquals.Location = new System.Drawing.Point(204, 292);
            this.btnEquals.Name = "btnEquals";
            this.btnEquals.Size = new System.Drawing.Size(56, 45);
            this.btnEquals.TabIndex = 19;
            this.btnEquals.Text = "=";
            this.btnEquals.UseVisualStyleBackColor = true;
            this.btnEquals.Click += new System.EventHandler(this.btnEquals_Click);
            //
            // CalculatorForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 349);
            this.Controls.Add(this.lblNotice);
            this.Controls.Add(this.txtDisplay);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnBackspace);
            this.Controls.Add(this.btnPercent);
            this.Controls.Add(this.btnDivide);
            this.Controls.Add(this.btn7);
            this.Controls.Add(this.btn8);
            this.Controls.Add(this.btn9);
            this.Controls.Add(this.btnMultiply);
            this.Controls.Add(this.btn4);
            this.Controls.Add(this.btn5);
            this.Controls.Add(this.btn6);
            this.Controls.Add(this.btnSubtract);
            this.Controls.Add(this.btn1);
            this.Controls.Add(this.btn2);
            this.Controls.Add(this.btn3);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btn0);
            this.Controls.Add(this.btnDecimal);
            this.Controls.Add(this.btnEquals);
            this.AcceptButton = this.btnEquals;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalculatorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Calculator";
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CalculatorForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CalculatorForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
