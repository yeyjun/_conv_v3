using System;
using System.Globalization;
using System.Windows.Forms;

namespace Inventory
{
    public partial class CalculatorForm : Form
    {
        private double _accumulator;
        private char? _pendingOperator;
        private bool _waitingForOperand = true;

        public CalculatorForm() : this(null)
        {
        }

        public CalculatorForm(decimal? initialValue)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            foreach (Control control in Controls)
            {
                if (control is Button button)
                    button.TabStop = false;
            }

            if (initialValue != null)
            {
                txtDisplay.Text = FormatResult((double)initialValue.Value);
                _waitingForOperand = true;
                lblNotice.Visible = true;
            }
        }

        private void btnDigit_Click(object sender, EventArgs e)
        {
            string digit = ((Button)sender).Text;

            if (_waitingForOperand)
            {
                txtDisplay.Text = digit == "0" ? "0" : digit;
                _waitingForOperand = false;
            }
            else if (txtDisplay.Text != "0" || digit != "0")
            {
                txtDisplay.Text = txtDisplay.Text == "0" ? digit : txtDisplay.Text + digit;
            }
        }

        private void btnDecimal_Click(object sender, EventArgs e)
        {
            if (_waitingForOperand)
            {
                txtDisplay.Text = "0.";
                _waitingForOperand = false;
            }
            else if (!txtDisplay.Text.Contains("."))
            {
                txtDisplay.Text += ".";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "0";
            _accumulator = 0;
            _pendingOperator = null;
            _waitingForOperand = true;
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (_waitingForOperand)
                return;

            string text = txtDisplay.Text;
            text = text.Length > 1 ? text.Substring(0, text.Length - 1) : "0";

            if (text == "-")
                text = "0";

            txtDisplay.Text = text;
            _waitingForOperand = text == "0";
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            double value = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture) / 100;
            txtDisplay.Text = FormatResult(value);
            _waitingForOperand = true;
        }

        private void btnOperator_Click(object sender, EventArgs e)
        {
            char symbol = ((Button)sender).Text[0];
            char op;
            switch (symbol)
            {
                case '÷': op = '/'; break;
                case '×': op = '*'; break;
                case '−': op = '-'; break;
                default: op = symbol; break;
            }

            if (_pendingOperator != null && !_waitingForOperand)
            {
                _accumulator = Calculate(_accumulator, _pendingOperator.Value, double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture));
                txtDisplay.Text = FormatResult(_accumulator);
            }
            else
            {
                _accumulator = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
            }

            _pendingOperator = op;
            _waitingForOperand = true;
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            if (_pendingOperator == null)
                return;

            double operand = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
            _accumulator = Calculate(_accumulator, _pendingOperator.Value, operand);
            txtDisplay.Text = FormatResult(_accumulator);
            _pendingOperator = null;
            _waitingForOperand = true;
        }

        private static double Calculate(double left, char op, double right)
        {
            switch (op)
            {
                case '+': return left + right;
                case '-': return left - right;
                case '*': return left * right;
                case '/': return right == 0 ? double.NaN : left / right;
                default: return right;
            }
        }

        private static string FormatResult(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return "Error";

            return Math.Round(value, 10).ToString("G15", CultureInfo.InvariantCulture);
        }

        private void CalculatorForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && !(e.KeyCode == Keys.D8 && e.Shift))
            {
                SimulateClick(((char)('0' + (e.KeyCode - Keys.D0))).ToString());
            }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                SimulateClick(((char)('0' + (e.KeyCode - Keys.NumPad0))).ToString());
            }
            else if (e.KeyCode == Keys.Oemplus && e.Shift || e.KeyCode == Keys.Add)
            {
                btnOperator_Click(btnAdd, e);
            }
            else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
            {
                btnOperator_Click(btnSubtract, e);
            }
            else if ((e.KeyCode == Keys.D8 && e.Shift) || e.KeyCode == Keys.Multiply)
            {
                btnOperator_Click(btnMultiply, e);
            }
            else if (e.KeyCode == Keys.OemQuestion || e.KeyCode == Keys.Divide)
            {
                btnOperator_Click(btnDivide, e);
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Oemplus && !e.Shift)
            {
                btnEquals_Click(btnEquals, e);
            }
            else if (e.KeyCode == Keys.Back)
            {
                btnBackspace_Click(btnBackspace, e);
            }
            else if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                btnDecimal_Click(btnDecimal, e);
            }
            else if (e.KeyCode == Keys.Escape)
            {
            }
            else
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void CalculatorForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                this.Close();
            }
        }

        private void SimulateClick(string digit)
        {
            Button digitButton;
            switch (digit)
            {
                case "0": digitButton = btn0; break;
                case "1": digitButton = btn1; break;
                case "2": digitButton = btn2; break;
                case "3": digitButton = btn3; break;
                case "4": digitButton = btn4; break;
                case "5": digitButton = btn5; break;
                case "6": digitButton = btn6; break;
                case "7": digitButton = btn7; break;
                case "8": digitButton = btn8; break;
                case "9": digitButton = btn9; break;
                default: digitButton = null; break;
            }

            if (digitButton != null)
                btnDigit_Click(digitButton, EventArgs.Empty);
        }
    }
}
