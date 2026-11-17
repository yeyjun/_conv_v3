using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace Inventory
{
    public static class OkDialog
    {
        private static readonly Color OkColor = Color.FromArgb(25, 118, 210);

        private static bool AutoDismissEnabled =>
            !string.Equals(ConfigurationManager.AppSettings["ALERT_AUTO_DISMISS_ENABLED"], "false", StringComparison.OrdinalIgnoreCase);

        private static int AutoDismissSeconds =>
            int.TryParse(ConfigurationManager.AppSettings["ALERT_AUTO_DISMISS_SECONDS"], out int s) && s > 0 ? s : 5;

        public static DialogResult Show(IWin32Window owner, string text, string caption,
            MessageBoxIcon icon = MessageBoxIcon.Information)
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
                    RowCount = 2,
                    AutoSize = true,
                };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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
                }

                var label = new Label
                {
                    Text = text,
                    AutoSize = true,
                    MaximumSize = new Size(340, 0),
                    Margin = new Padding(0, 0, 0, 15),
                };
                layout.Controls.Add(label, 1, 0);

                var buttonPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.RightToLeft,
                    AutoSize = true,
                    Margin = new Padding(0),
                };
                layout.Controls.Add(buttonPanel, 1, 1);
                layout.SetColumnSpan(buttonPanel, dialogIcon != null ? 1 : 2);

                var okButton = MakeButton("&OK", DialogResult.OK, OkColor);
                buttonPanel.Controls.Add(okButton);

                form.Controls.Add(layout);
                form.AcceptButton = okButton;
                form.CancelButton = okButton;

                Timer autoDismissTimer = null;
                if (AutoDismissEnabled)
                {
                    autoDismissTimer = new Timer { Interval = AutoDismissSeconds * 1000 };
                    autoDismissTimer.Tick += (s, e) =>
                    {
                        form.DialogResult = DialogResult.OK;
                        form.Close();
                    };
                    autoDismissTimer.Start();
                    form.FormClosed += (s, e) => autoDismissTimer.Dispose();
                }

                YourApp.Data.AppTheme.Exempt(okButton);
                YourApp.Data.LogoCache.ApplyTo(form);
                YourApp.Data.AppTheme.ApplyTo(form);
                YourApp.Data.AppFontScale.ApplyTo(form);

                return form.ShowDialog(owner);
            }
        }

        private static Button MakeButton(string text, DialogResult result, Color color)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = result,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Size = new Size(90, 32),
                Margin = new Padding(8, 0, 0, 0),
                UseVisualStyleBackColor = false,
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
