using System;
using System.Windows.Forms;

namespace Inventory
{
    public partial class TaxConfigForm : Form
    {
        public TaxConfigForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private void TaxConfigForm_Load(object sender, EventArgs e)
        {
            nud_taxRate.Value = Math.Min(Math.Max(TaxConfig.TaxRatePercent, nud_taxRate.Minimum), nud_taxRate.Maximum);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TaxConfig.Save(nud_taxRate.Value);
            OperationLog.RecordUpdate(nameof(TaxConfigForm), $"Tax rate set to {nud_taxRate.Value}%.");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
