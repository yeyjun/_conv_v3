using System;
using System.Linq;
using System.Windows.Forms;

namespace _conv
{
    public partial class HeldTransactionsForm : Form
    {
        private readonly Purchases owner;

        public HeldTransactionsForm(Purchases owner)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
            this.owner = owner;
        }

        public void RefreshList()
        {
            string selectedTicket = SelectedTicketCode();

            dgvHeld.Rows.Clear();
            foreach (var held in owner.heldTransactions.Values.OrderBy(h => h.HeldAt))
            {
                dgvHeld.Rows.Add(held.TicketCode, held.HeldAt.ToString("MM/dd hh:mm tt"), held.Items.Count, held.GrandTotal.ToString("0.00"));
            }

            foreach (DataGridViewRow row in dgvHeld.Rows)
            {
                if (string.Equals(row.Cells[0].Value?.ToString(), selectedTicket, StringComparison.Ordinal))
                {
                    row.Selected = true;
                    break;
                }
            }

            bool hasAny = dgvHeld.Rows.Count > 0;
            btnResume.Enabled = hasAny;
            btnCancel.Enabled = hasAny;
            lblEmpty.Visible = !hasAny;
        }

        private string SelectedTicketCode()
        {
            if (dgvHeld.CurrentRow == null)
                return null;
            return dgvHeld.CurrentRow.Cells[0].Value?.ToString();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            ResumeSelected();
        }

        private void dgvHeld_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                ResumeSelected();
        }

        private void ResumeSelected()
        {
            string ticketCode = SelectedTicketCode();
            if (ticketCode == null)
            {
                MessageBox.Show("Select a held transaction first.", "Resume Transaction");
                return;
            }

            if (!owner.ResumeHeldTransaction(ticketCode, out string error))
            {
                MessageBox.Show(error, "Resume Transaction");
                return;
            }

            RefreshList();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelSelected();
        }

        private void CancelSelected()
        {
            string ticketCode = SelectedTicketCode();
            if (ticketCode == null)
            {
                MessageBox.Show("Select a held transaction first.", "Cancel Transaction");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Discard held transaction \"{ticketCode}\"? This cannot be undone.",
                "Cancel Transaction", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                owner.CancelHeldTransaction(ticketCode);
                RefreshList();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void HeldTransactionsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                ResumeSelected();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F9)
            {
                CancelSelected();
                e.Handled = true;
            }
        }
    }
}
