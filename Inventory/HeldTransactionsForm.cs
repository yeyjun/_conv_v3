using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Inventory
{
    public partial class HeldTransactionsForm : Form
    {
        private readonly Purchases owner;

        private bool _suppressSelectionPreview;

        public HeldTransactionsForm(Purchases owner)
        {
            InitializeComponent();
            YourApp.Data.LogoCache.ApplyTo(this);
            YourApp.Data.AppTheme.ApplyTo(this);
            YourApp.Data.AppFontScale.ApplyTo(this);
            this.owner = owner;
        }

        public void RefreshList(bool resyncPreview = true)
        {
            string selectedTicket = SelectedTicketCode();

            _suppressSelectionPreview = true;
            try
            {
                dgvHeld.Rows.Clear();
                foreach (var held in owner.heldTransactions.Values.OrderBy(h => h.HeldAt))
                {
                    dgvHeld.Rows.Add(held.TicketCode, held.HeldAt.ToString("MM/dd hh:mm tt"), held.Items.Count,
                        held.GrandTotal.ToString("0.00"), held.Remarks ?? "");
                }

                foreach (DataGridViewRow row in dgvHeld.Rows)
                {
                    if (string.Equals(row.Cells[0].Value?.ToString(), selectedTicket, StringComparison.Ordinal))
                    {
                        row.Selected = true;
                        break;
                    }
                }
            }
            finally
            {
                _suppressSelectionPreview = false;
            }

            bool hasAny = dgvHeld.Rows.Count > 0;
            btnResume.Enabled = hasAny;
            btnCancel.Enabled = hasAny;
            lblEmpty.Visible = !hasAny;

            if (resyncPreview)
                dgvHeld_SelectionChanged(this, EventArgs.Empty);
        }

        private void dgvHeld_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelectionPreview)
                return;

            string ticketCode = SelectedTicketCode();
            if (ticketCode != null)
            {
                owner.PreviewHeldTransaction(ticketCode);

                TopMost = true;
                Rectangle workArea = Screen.FromControl(this).WorkingArea;
                Point cartBoardScreenLocation = owner.GetCartBoardScreenLocation();
                int x = Math.Max(cartBoardScreenLocation.X - Width, workArea.Left);
                int y = Math.Min(cartBoardScreenLocation.Y, workArea.Bottom - Height);
                y = Math.Max(y, workArea.Top);
                Location = new Point(x, y);
            }
            else
            {
                owner.EndHeldTransactionPreview();
                TopMost = false;
            }
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

            DialogResult confirm = ConfirmDialog.Show(this,
                $"Discard held transaction \"{ticketCode}\"? This cannot be undone. (Y/N)",
                "Cancel Transaction", MessageBoxIcon.Warning);

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
            else if (e.KeyCode == Keys.F4)
            {
                owner.ShowItemQuickSearchWindow();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                owner.suppressNextSerialKeyUp = true;
                Close();
                e.Handled = true;
            }
        }
    }
}
