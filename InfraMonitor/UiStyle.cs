using System.Drawing;
using System.Windows.Forms;

namespace InfraMonitor
{
    // Dark "ops console" palette, deliberately distinct from Inventory's look and
    // Remittancce's ink-ledger look: near-black chrome, monospace figures for the numbers
    // people actually come here to check, and three status colors (green/amber/red) reused
    // everywhere a health state is shown (banner, grid rows, CPU readout).
    internal static class UiStyle
    {
        public static readonly Color Background = Color.FromArgb(18, 20, 24);
        public static readonly Color Panel = Color.FromArgb(26, 29, 35);
        public static readonly Color Line = Color.FromArgb(45, 49, 58);
        public static readonly Color RowTint = Color.FromArgb(32, 35, 42);
        public static readonly Color TextPrimary = Color.FromArgb(226, 230, 237);
        public static readonly Color TextMuted = Color.FromArgb(138, 146, 160);
        public static readonly Color Accent = Color.FromArgb(79, 156, 249);

        public static readonly Color Normal = Color.FromArgb(63, 185, 131);
        public static readonly Color Warning = Color.FromArgb(230, 168, 54);
        public static readonly Color Critical = Color.FromArgb(224, 86, 86);

        public static Font BodyFont => new Font("Segoe UI", 9.5f);
        public static Font BodyFontBold => new Font("Segoe UI", 9.5f, FontStyle.Bold);
        public static Font LedgerFont => new Font("Consolas", 10f);

        public static Color ColorFor(HealthStatus status)
        {
            switch (status)
            {
                case HealthStatus.Normal: return Normal;
                case HealthStatus.Warning: return Warning;
                case HealthStatus.Critical: return Critical;
                default: return TextMuted;
            }
        }

        public static string LabelFor(HealthStatus status)
        {
            switch (status)
            {
                case HealthStatus.Normal: return "Normal";
                case HealthStatus.Warning: return "Warning";
                case HealthStatus.Critical: return "Critical";
                default: return "Unknown";
            }
        }

        public static void StyleSecondary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
            button.BackColor = Panel;
            button.ForeColor = TextPrimary;
            button.Font = BodyFont;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Line;
            button.FlatAppearance.MouseOverBackColor = Line;
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(Line, 0.1f);
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Panel;
            grid.GridColor = Line;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.EnableHeadersVisualStyles = false;
            grid.RowTemplate.Height = 30;
            grid.ColumnHeadersHeight = 34;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Background;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
            grid.ColumnHeadersDefaultCellStyle.Font = BodyFontBold;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Background;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextMuted;

            grid.DefaultCellStyle.BackColor = Panel;
            grid.DefaultCellStyle.ForeColor = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Line;
            grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.DefaultCellStyle.Font = BodyFont;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = RowTint;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Line;
        }
    }
}
