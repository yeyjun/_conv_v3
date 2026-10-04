using System;
using System.Windows.Forms;

namespace _conv
{
    /// <summary>
    /// First-run "Getting Started" walkthrough: a short Next/Back/Skip slideshow covering the
    /// basics of ringing up a sale. Shown automatically once per machine from Purchases_Load (see
    /// <see cref="TutorialState"/>) and replayable anytime from Help > Getting Started.
    /// </summary>
    public partial class TutorialForm : Form
    {
        private readonly (string Title, string Body)[] steps =
        {
            ("Welcome to BrewCode Innovations",
                "This is Baclig Minimart's point-of-sale and inventory system. This quick " +
                "walkthrough covers the basics of ringing up a sale - it only takes a minute, " +
                "and you can revisit it anytime from Help > Getting Started."),

            ("Choose a Pricing Type",
                "Before scanning, press F1 for Per-Piece, F2 for Wholesale, or F3 for Retail " +
                "pricing. The type you pick applies to every item you add until you change it."),

            ("Scan or Search for Items",
                "Type or scan a barcode into the Serial Code box and press Enter to add it " +
                "straight to the cart. Prefer typing a name instead? Use the Search box, then " +
                "press F7 to add the highlighted result."),

            ("Review the Cart",
                "Items you've added appear in the Item List. Select a row and press F12 to " +
                "change its quantity, Backspace to remove it, or Escape to clear the whole cart."),

            ("Take Payment",
                "Press F9 to jump to the Denomination box and enter the cash received - the " +
                "Change amount updates automatically. Press Enter there to finalize the sale, " +
                "print the receipt, and update inventory."),

            ("Hold a Sale for Later",
                "Stepping away mid-sale? Press F6 to hold the current transaction and F8 to " +
                "resume it later, or use the Transaction menu at the top."),

            ("Explore More",
                "The Manage Prices tab lets managers edit items and prices; the Dashboard tab " +
                "shows today's sales, stock levels, and quick links to Reports, Petty Cash, and " +
                "Loyalty. Press Ctrl+H anytime for the full keyboard shortcut list."),
        };

        private int stepIndex;

        public TutorialForm()
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);

            RenderStep();
        }

        private void RenderStep()
        {
            lblStep.Text = $"Step {stepIndex + 1} of {steps.Length}";
            lblTitle.Text = steps[stepIndex].Title;
            lblBody.Text = steps[stepIndex].Body;
            btnBack.Enabled = stepIndex > 0;
            btnNext.Text = stepIndex == steps.Length - 1 ? "Finish" : "Next";
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (stepIndex == steps.Length - 1)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            stepIndex++;
            RenderStep();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            stepIndex--;
            RenderStep();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
