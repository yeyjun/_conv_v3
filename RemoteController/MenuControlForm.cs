using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RemoteController
{
    /// <summary>
    /// Lets the operator see and change which top-level menus are visible on a remote terminal -
    /// mirrors the "System Config > Menus" checklist the POS app itself has, but driven over the
    /// GET_MENUS/SET_MENUS commands instead of sitting in front of the terminal.
    /// </summary>
    public partial class MenuControlForm : Form
    {
        // (command, arg) -> response - supplied by MainForm so this dialog never needs to know
        // whether it's actually talking over Network or Bluetooth.
        private readonly Func<string, string, string> sendCommand;

        private sealed class MenuEntry
        {
            public string Name;
            public string Label;
            public override string ToString() => Label;
        }

        public MenuControlForm(Func<string, string, string> sendCommand)
        {
            InitializeComponent();
            this.sendCommand = sendCommand;
        }

        private void MenuControlForm_Load(object sender, EventArgs e) => LoadMenus();

        private void btnRefresh_Click(object sender, EventArgs e) => LoadMenus();

        private void LoadMenus()
        {
            clbMenus.Items.Clear();
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Text = "Loading...";

            string response;
            try
            {
                response = sendCommand("GET_MENUS", null);
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Tomato;
                lblStatus.Text = $"Failed to connect: {ex.Message}";
                return;
            }

            string[] parts = response.Split('|');
            if (parts.Length < 3 || parts[0] != "OK" || parts[1] != "MENUS")
            {
                lblStatus.ForeColor = Color.Tomato;
                lblStatus.Text = $"Terminal returned: {response}";
                return;
            }

            string[] hiddenNameArray = (parts.Length >= 4 ? parts[3] : "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var hiddenNames = new HashSet<string>(hiddenNameArray, StringComparer.OrdinalIgnoreCase);

            foreach (string entry in parts[2].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] nameLabel = entry.Split(new[] { ':' }, 2);
                if (nameLabel.Length != 2)
                    continue;

                var item = new MenuEntry { Name = nameLabel[0], Label = nameLabel[1] };
                clbMenus.Items.Add(item, !hiddenNames.Contains(item.Name));
            }

            lblStatus.ForeColor = Color.Gray;
            lblStatus.Text = $"Loaded {clbMenus.Items.Count} menu(s).";
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            IEnumerable<string> hidden = clbMenus.Items.Cast<MenuEntry>()
                .Where((item, index) => !clbMenus.GetItemChecked(index))
                .Select(item => item.Name);

            string arg = string.Join(",", hidden);

            lblStatus.ForeColor = Color.Gray;
            lblStatus.Text = "Applying...";

            try
            {
                string response = sendCommand("SET_MENUS", arg);
                if (response.StartsWith("OK", StringComparison.Ordinal))
                {
                    lblStatus.ForeColor = Color.SeaGreen;
                    lblStatus.Text = "Applied - the terminal's menu bar updates immediately.";
                }
                else
                {
                    lblStatus.ForeColor = Color.Tomato;
                    lblStatus.Text = $"Terminal returned: {response}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Tomato;
                lblStatus.Text = $"Failed to connect: {ex.Message}";
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
