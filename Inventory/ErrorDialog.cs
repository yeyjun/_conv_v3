using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    // Companion to ConfirmDialog/OkDialog for warnings and errors that come from a caught
    // exception: shows a short, human-readable sentence up front, with the full technical
    // text (ex.ToString() - type, message, and stack trace) tucked behind a "Show details"
    // toggle rather than always on screen. Unlike OkDialog, this never auto-dismisses -
    // someone reading or copying a stack trace shouldn't have it vanish under them.
    public static class ErrorDialog
    {
        public static DialogResult Show(IWin32Window owner, string friendlyMessage, string caption,
            Exception exception, MessageBoxIcon icon = MessageBoxIcon.Error) =>
            Show(owner, friendlyMessage, caption, exception?.ToString() ?? "", icon);

        public static DialogResult Show(IWin32Window owner, string friendlyMessage, string caption,
            string technicalDetails, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            using (var form = new Form())
            {
                form.Text = caption;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ShowIcon = false;
                form.ShowInTaskbar = false;
                form.StartPosition = FormStartPosition.CenterParent;
                form.KeyPreview = true;
                form.AutoSize = true;
                form.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                form.Padding = new Padding(15);

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 4,
                    AutoSize = true,
                };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                for (int i = 0; i < 4; i++)
                    layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                Icon dialogIcon = GetIcon(icon);
                if (dialogIcon != null)
                {
                    var pictureBox = new PictureBox
                    {
                        Image = dialogIcon.ToBitmap(),
                        SizeMode = PictureBoxSizeMode.AutoSize,
                        Margin = new Padding(0, 0, 15, 15),
                    };
                    layout.Controls.Add(pictureBox, 0, 0);
                    layout.SetRowSpan(pictureBox, 1);
                }

                var messageLabel = new Label
                {
                    Text = friendlyMessage,
                    AutoSize = true,
                    MaximumSize = new Size(380, 0),
                    Margin = new Padding(0, 0, 0, 10),
                };
                layout.Controls.Add(messageLabel, 1, 0);

                var toggleLink = new LinkLabel
                {
                    Text = "Show technical details ▼",
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, 6),
                    Visible = !string.IsNullOrWhiteSpace(technicalDetails),
                };
                layout.Controls.Add(toggleLink, 1, 1);

                var detailsBox = new TextBox
                {
                    Text = technicalDetails,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    WordWrap = false,
                    Font = new Font(FontFamily.GenericMonospace, 8f),
                    Size = new Size(480, 220),
                    Margin = new Padding(0, 0, 0, 10),
                    Visible = false,
                };
                layout.Controls.Add(detailsBox, 1, 2);

                bool detailsShown = false;
                toggleLink.LinkClicked += (s, e) =>
                {
                    detailsShown = !detailsShown;
                    detailsBox.Visible = detailsShown;
                    toggleLink.Text = detailsShown ? "Hide technical details ▲" : "Show technical details ▼";
                };

                var buttonPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    AutoSize = true,
                    Margin = new Padding(0),
                };
                layout.Controls.Add(buttonPanel, 1, 3);

                var okButton = MakeButton("&OK", DialogResult.OK, OkColor);
                buttonPanel.Controls.Add(okButton);

                if (!string.IsNullOrWhiteSpace(technicalDetails))
                {
                    var copyButton = MakeButton("&Copy Details", DialogResult.None, CopyColor);
                    copyButton.Click += (s, e) => Clipboard.SetText(technicalDetails);
                    buttonPanel.Controls.Add(copyButton);
                }

                form.Controls.Add(layout);
                form.AcceptButton = okButton;
                form.CancelButton = okButton;

                YourApp.Data.AppTheme.Exempt(okButton);
                YourApp.Data.LogoCache.ApplyTo(form);
                YourApp.Data.AppTheme.ApplyTo(form);
                YourApp.Data.AppFontScale.ApplyTo(form);

                return form.ShowDialog(owner);
            }
        }

        private static readonly Color OkColor = Color.FromArgb(25, 118, 210);
        private static readonly Color CopyColor = Color.FromArgb(97, 97, 97);

        private static Button MakeButton(string text, DialogResult result, Color color)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = result,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new Size(110, 32),
                Margin = new Padding(8, 0, 0, 0),
                UseVisualStyleBackColor = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color);
            return button;
        }

        private static Icon GetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Warning: return SystemIcons.Warning;
                case MessageBoxIcon.Question: return SystemIcons.Question;
                case MessageBoxIcon.Error: return SystemIcons.Error;
                case MessageBoxIcon.Information: return SystemIcons.Information;
                default: return null;
            }
        }
    }
}
