using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InfraMonitor
{
    internal sealed class IoEventsPanel : UserControl
    {
        private static readonly TimeSpan Lookback = TimeSpan.FromDays(7);
        private const int AutoRefreshMs = 60000;

        private readonly DataGridView grid;
        private readonly Label lblStatus;
        private readonly Button btnRefresh;
        private readonly Timer timer;
        private bool scanning;

        public HealthStatus Status { get; private set; } = HealthStatus.Unknown;
        public event EventHandler StatusChanged;

        public IoEventsPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = UiStyle.Background;
            Padding = new Padding(16);

            var topBar = new Panel { Dock = DockStyle.Top, Height = 36 };

            btnRefresh = new Button { Text = "Refresh", Width = 90, Height = 28, Location = new Point(0, 0) };
            UiStyle.StyleSecondary(btnRefresh);
            btnRefresh.Click += (s, e) => _ = ScanAsync();

            lblStatus = new Label
            {
                AutoSize = false,
                Location = new Point(104, 0),
                Size = new Size(700, 28),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = UiStyle.TextMuted,
                Font = UiStyle.BodyFont,
                Text = $"Disk/IO errors and warnings from the Windows System log, last {Lookback.Days} days.",
            };

            topBar.Controls.Add(btnRefresh);
            topBar.Controls.Add(lblStatus);

            grid = new DataGridView { Dock = DockStyle.Fill };
            UiStyle.StyleGrid(grid);
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Time", HeaderText = "Time", Width = 140 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Level", HeaderText = "Level", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Source", HeaderText = "Source", Width = 150 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "EventId", HeaderText = "Event ID", Width = 80 });
            var messageCol = new DataGridViewTextBoxColumn
            {
                Name = "Message",
                HeaderText = "Message",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            };
            grid.Columns.Add(messageCol);

            Controls.Add(grid);
            Controls.Add(topBar);

            timer = new Timer { Interval = AutoRefreshMs };
            timer.Tick += (s, e) => _ = ScanAsync();
            Disposed += (s, e) => timer.Dispose();

            // Started here rather than on Load: WinForms only creates a TabPage's children
            // (and fires Load) once that tab is actually selected, which would otherwise
            // leave this panel frozen until the user clicks its tab.
            _ = ScanAsync();
            timer.Start();
        }

        private async Task ScanAsync()
        {
            if (scanning)
                return;
            scanning = true;
            btnRefresh.Enabled = false;
            try
            {
                string error = null;
                var records = await Task.Run(() => IoEventScanner.ScanRecent(Lookback, out error));

                if (error != null)
                {
                    lblStatus.Text = $"Could not read the System event log: {error}";
                    lblStatus.ForeColor = UiStyle.TextMuted;
                    SetStatus(HealthStatus.Unknown);
                    return;
                }

                grid.Rows.Clear();
                HealthStatus worst = HealthStatus.Normal;

                foreach (var record in records)
                {
                    HealthStatus rowStatus = record.Level == "Error" ? HealthStatus.Critical : HealthStatus.Warning;
                    if (rowStatus > worst)
                        worst = rowStatus;

                    int rowIndex = grid.Rows.Add(
                        record.TimeGenerated.ToString("yyyy-MM-dd HH:mm"),
                        record.Level,
                        record.Source,
                        record.EventId,
                        record.Message);
                    grid.Rows[rowIndex].DefaultCellStyle.ForeColor = UiStyle.ColorFor(rowStatus);
                }

                lblStatus.Text = records.Count == 0
                    ? $"No disk/IO errors or warnings in the last {Lookback.Days} days."
                    : $"{records.Count} disk/IO event(s) in the last {Lookback.Days} days.";
                lblStatus.ForeColor = UiStyle.TextMuted;

                SetStatus(worst);
            }
            finally
            {
                scanning = false;
                btnRefresh.Enabled = true;
            }
        }

        private void SetStatus(HealthStatus status)
        {
            if (status == Status)
                return;
            Status = status;
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
