using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Remittancce
{
    // Palette and shared control styling: an "ink ledger" look - cream paper, navy ink
    // text/chrome, and a single vermilion "stamp" accent reserved for the one primary
    // action per screen (Send Money / Save) rather than theming every control the same way.
    internal static class UiStyle
    {
        public static readonly Color Ink = Color.FromArgb(28, 37, 65);
        public static readonly Color Paper = Color.FromArgb(247, 244, 237);
        public static readonly Color Line = Color.FromArgb(228, 222, 207);
        public static readonly Color RowTint = Color.FromArgb(241, 237, 227);
        public static readonly Color Stamp = Color.FromArgb(178, 58, 46);
        public static readonly Color SelectionTint = Color.FromArgb(240, 223, 219);
        public static readonly Color HeaderSubtext = Color.FromArgb(197, 203, 219);
        public static readonly Color Slate = Color.FromArgb(91, 100, 114);

        public static Font UiFont => new Font("Segoe UI", 9.5f);
        public static Font UiFontBold => new Font("Segoe UI", 9.5f, FontStyle.Bold);
        public static Font LedgerFont => new Font("Consolas", 9.5f);
        public static Font LedgerFontBold => new Font("Consolas", 10f, FontStyle.Bold);

        public static void RoundButton(Button button, int radius = 6)
        {
            void ApplyRegion()
            {
                if (button.Width <= 0 || button.Height <= 0)
                    return;
                button.Region = new Region(RoundedPath(new Rectangle(0, 0, button.Width, button.Height), radius));
            }
            ApplyRegion();
            button.Resize += (s, e) => ApplyRegion();
        }

        public static void StylePrimary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.BackColor = Stamp;
            button.ForeColor = Color.White;
            button.Font = UiFontBold;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(Stamp, 0.2f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(Stamp, 0.1f);
            RoundButton(button);
        }

        public static void StyleSecondary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.BackColor = Paper;
            button.ForeColor = Ink;
            button.Font = UiFont;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Line;
            button.FlatAppearance.MouseOverBackColor = Line;
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(Line, 0.05f);
            RoundButton(button);
        }

        public static GraphicsPath RoundedPath(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Paper;
            grid.GridColor = Line;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.EnableHeadersVisualStyles = false;
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersHeight = 36;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Ink;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Paper;
            grid.ColumnHeadersDefaultCellStyle.Font = UiFontBold;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Ink;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Paper;

            grid.DefaultCellStyle.BackColor = Paper;
            grid.DefaultCellStyle.ForeColor = Ink;
            grid.DefaultCellStyle.SelectionBackColor = SelectionTint;
            grid.DefaultCellStyle.SelectionForeColor = Ink;
            grid.DefaultCellStyle.Font = UiFont;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = RowTint;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectionTint;
        }
    }

    // The one deliberate "signature" flourish: a dashed rule between the ledger grid and the
    // totals strip, like the perforation on a receipt about to be torn off at day's end.
    internal sealed class PerforationDivider : Panel
    {
        public PerforationDivider()
        {
            Height = 11;
            Dock = DockStyle.Top;
            BackColor = UiStyle.Paper;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int y = Height / 2;
            using (var pen = new Pen(UiStyle.Line, 1.5f) { DashStyle = DashStyle.Dash })
            {
                e.Graphics.DrawLine(pen, 0, y, Width, y);
            }
        }
    }
}
