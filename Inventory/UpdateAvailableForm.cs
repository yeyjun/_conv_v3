using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    /// <summary>
    /// Non-modal "a new version is available" notification, built the same way as
    /// OkDialog/ConfirmDialog (manual Form, no .Designer.cs) but shown via Show(owner) instead
    /// of ShowDialog - this must never block the till. "View Release" just opens the release
    /// page in the default browser; downloading and running the installer stays a deliberate,
    /// manual action by an admin UNLESS AUTO_UPDATE_ENABLED is on and the release has a matching
    /// zip asset, in which case CheckForUpdateAsync in Form1.cs takes the SelfUpdateService path
    /// instead and shows ShowReadyToInstall below rather than this notify-only Show.
    /// </summary>
    internal static class UpdateAvailableForm
    {
        private static readonly Color ViewColor = Color.FromArgb(25, 118, 210);
        private static readonly Color DismissColor = Color.FromArgb(117, 117, 117);

        public static Form Show(IWin32Window owner, UpdateInfo info) =>
            Build(owner, $"A new version ({info.TagName ?? info.Version?.ToString()}) is available.", info);

        /// <summary>
        /// Purely informational - unlike Show, there is no "install now" action because it's
        /// already automatic, and nothing to cancel since it just waits for the till to be idle.
        /// </summary>
        public static Form ShowReadyToInstall(IWin32Window owner, UpdateInfo info) =>
            Build(owner,
                $"Update {info.TagName ?? info.Version?.ToString()} has been downloaded and will be installed " +
                "automatically the next time the till is idle.",
                info);

        private static Form Build(IWin32Window owner, string titleText, UpdateInfo info)
        {
            var form = new Form
            {
                Text = "Update Available",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterScreen,
                KeyPreview = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(15),
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                AutoSize = true,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var titleLabel = new Label
            {
                Text = titleText,
                AutoSize = true,
                MaximumSize = new Size(360, 0),
                Font = new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 10),
            };
            layout.Controls.Add(titleLabel);

            if (!string.IsNullOrWhiteSpace(info.Notes))
            {
                var notesBox = new TextBox
                {
                    Text = info.Notes,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Size = new Size(360, 90),
                    Margin = new Padding(0, 0, 0, 10),
                };
                layout.Controls.Add(notesBox);
            }

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Margin = new Padding(0),
            };

            var dismissButton = MakeButton("Dismiss", DismissColor);
            var viewButton = MakeButton("View Release", ViewColor);
            dismissButton.Click += (s, e) => form.Close();
            viewButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(info.HtmlUrl))
                    return;
                try { Process.Start(new ProcessStartInfo(info.HtmlUrl) { UseShellExecute = true }); }
                catch { /* no browser / failed launch - nothing more to do here */ }
            };

            buttonPanel.Controls.Add(dismissButton);
            buttonPanel.Controls.Add(viewButton);
            layout.Controls.Add(buttonPanel);

            form.Controls.Add(layout);
            form.CancelButton = null;
            form.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    form.Close();
            };

            YourApp.Data.AppTheme.Exempt(viewButton);
            YourApp.Data.AppTheme.Exempt(dismissButton);
            YourApp.Data.LogoCache.ApplyTo(form);
            YourApp.Data.AppTheme.ApplyTo(form);
            YourApp.Data.AppFontScale.ApplyTo(form);

            form.FormClosed += (s, e) => form.Dispose();
            form.Show(owner);
            return form;
        }

        private static Button MakeButton(string text, Color color)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new Size(110, 32),
                Margin = new Padding(8, 0, 0, 0),
                UseVisualStyleBackColor = false,
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color);
            return button;
        }
    }
}
