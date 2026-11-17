using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace InfraMonitor
{
    internal sealed class StorageWatchPanel : UserControl
    {
        private const double WarningPercent = 80.0;
        private const double CriticalPercent = 95.0;

        private readonly DataGridView grid;
        private readonly Label lblLegend;
        private readonly Timer timer;

        public HealthStatus Status { get; private set; } = HealthStatus.Unknown;
        public event EventHandler StatusChanged;

        public StorageWatchPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = UiStyle.Background;
            Padding = new Padding(16);

            lblLegend = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = $"Warning at {WarningPercent:0}% used, critical at {CriticalPercent:0}% used",
                ForeColor = UiStyle.TextMuted,
                Font = UiStyle.BodyFont,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            grid = new DataGridView { Dock = DockStyle.Fill };
            UiStyle.StyleGrid(grid);
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Drive", HeaderText = "Drive", Width = 70 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Label", HeaderText = "Label", Width = 140 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "FileSystem", HeaderText = "File System", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", HeaderText = "Total", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Free", HeaderText = "Free", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "UsedPercent", HeaderText = "Used %", Width = 80 });
            var statusCol = new DataGridViewTextBoxColumn { Name = "StatusText", HeaderText = "Status", Width = 100 };
            grid.Columns.Add(statusCol);

            Controls.Add(grid);
            Controls.Add(lblLegend);

            timer = new Timer { Interval = 5000 };
            timer.Tick += (s, e) => RefreshDrives();
            Disposed += (s, e) => timer.Dispose();

            // Started here rather than on Load: WinForms only creates a TabPage's children
            // (and fires Load) once that tab is actually selected, which would otherwise
            // leave this panel frozen until the user clicks its tab.
            RefreshDrives();
            timer.Start();
        }

        public void RefreshDrives()
        {
            grid.Rows.Clear();
            HealthStatus worst = HealthStatus.Normal;

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady || drive.DriveType != DriveType.Fixed)
                    continue;

                double totalGb = drive.TotalSize / 1024.0 / 1024.0 / 1024.0;
                double freeGb = drive.AvailableFreeSpace / 1024.0 / 1024.0 / 1024.0;
                double usedPercent = totalGb <= 0 ? 0 : (totalGb - freeGb) / totalGb * 100.0;

                HealthStatus rowStatus =
                    usedPercent >= CriticalPercent ? HealthStatus.Critical :
                    usedPercent >= WarningPercent ? HealthStatus.Warning :
                    HealthStatus.Normal;
                if (rowStatus > worst)
                    worst = rowStatus;

                int rowIndex = grid.Rows.Add(
                    drive.Name.TrimEnd('\\'),
                    string.IsNullOrWhiteSpace(drive.VolumeLabel) ? "(no label)" : drive.VolumeLabel,
                    drive.DriveFormat,
                    $"{totalGb:0.0} GB",
                    $"{freeGb:0.0} GB",
                    $"{usedPercent:0.0}%",
                    UiStyle.LabelFor(rowStatus));
                grid.Rows[rowIndex].DefaultCellStyle.ForeColor = UiStyle.ColorFor(rowStatus);
            }

            if (grid.Rows.Count == 0)
                worst = HealthStatus.Unknown;

            SetStatus(worst);
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
