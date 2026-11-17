using System;
using System.Windows.Forms;

namespace Inventory
{
    public partial class PinPromptForm : Form
    {
        public string EnteredPin => tbx_pin.Text;

        // Defaults to the designer's "Enter manager PIN to continue:" text; callers gating
        // access behind the separate admin PIN (System Configuration) override this so the
        // prompt doesn't ask for the wrong PIN.
        public string PromptText
        {
            get => lbl_prompt.Text;
            set => lbl_prompt.Text = value;
        }

        public PinPromptForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
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
