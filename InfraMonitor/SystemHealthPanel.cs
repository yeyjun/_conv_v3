using System;
using System.Drawing;
using System.Windows.Forms;

namespace InfraMonitor
{
    internal sealed class SystemHealthPanel : UserControl
    {
        private readonly Panel banner;
        private readonly Label lblBannerStatus;
        private readonly Label lblBannerDetail;
        private readonly Label lblFacts;
        private readonly Timer timer;

        public SystemHealthPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = UiStyle.Background;
            Padding = new Padding(16);

            banner = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = UiStyle.Panel };
            lblBannerStatus = new Label
            {
                Dock = DockStyle.Top,
                Height = 44,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = UiStyle.TextPrimary,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Text = "Checking system health...",
            };
            lblBannerDetail = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Font = UiStyle.BodyFont,
                ForeColor = UiStyle.TextMuted,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Text = "",
            };
            banner.Controls.Add(lblBannerDetail);
            banner.Controls.Add(lblBannerStatus);

            lblFacts = new Label
            {
                Dock = DockStyle.Fill,
                Font = UiStyle.LedgerFont,
                ForeColor = UiStyle.TextPrimary,
                Padding = new Padding(0, 20, 0, 0),
                TextAlign = ContentAlignment.TopLeft,
            };

            Controls.Add(lblFacts);
            Controls.Add(banner);

            timer = new Timer { Interval = 5000 };
            timer.Tick += (s, e) => RefreshFacts();
            Disposed += (s, e) => timer.Dispose();

            RefreshFacts();
            timer.Start();
        }

        private void RefreshFacts()
        {
            var (totalGb, availableGb, usedPercent) = SystemMetrics.GetMemoryInfo();
            float cpu = CpuMonitor.SampleOnce();

            lblFacts.Text =
                $"Machine:      {SystemMetrics.MachineName}\r\n" +
                $"OS:           {SystemMetrics.OsDescription}\r\n" +
                $"Uptime:       {FormatUptime(SystemMetrics.Uptime)}\r\n" +
                $"Logical CPUs: {SystemMetrics.LogicalProcessorCount}\r\n" +
                $"CPU load:     {cpu:0.0}%\r\n" +
                $"Memory:       {availableGb:0.0} GB free of {totalGb:0.0} GB ({usedPercent:0.0}% used)\r\n" +
                $"Last checked: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        private static string FormatUptime(TimeSpan uptime) =>
            $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";

        public void SetOverall(HealthStatus status, string detail)
        {
            lblBannerStatus.Text = UiStyle.LabelFor(status);
            lblBannerStatus.ForeColor = UiStyle.ColorFor(status);
            lblBannerDetail.Text = detail;
        }
    }
}
