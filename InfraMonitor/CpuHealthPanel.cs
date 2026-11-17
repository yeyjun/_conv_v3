using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace InfraMonitor
{
    internal sealed class CpuHealthPanel : UserControl
    {
        private const float WarningPercent = 70f;
        private const float CriticalPercent = 90f;

        private readonly Label lblReading;
        private readonly Label lblSubtext;
        private readonly SparklinePanel sparkline;
        private readonly Timer timer;

        public HealthStatus Status { get; private set; } = HealthStatus.Unknown;
        public event EventHandler StatusChanged;

        public CpuHealthPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = UiStyle.Background;
            Padding = new Padding(16);

            lblReading = new Label
            {
                Dock = DockStyle.Top,
                Height = 90,
                Font = new Font("Consolas", 42f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "-- %",
                ForeColor = UiStyle.TextPrimary,
            };

            lblSubtext = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                Font = UiStyle.BodyFont,
                ForeColor = UiStyle.TextMuted,
                Text = $"{Environment.ProcessorCount} logical processors · last 60 seconds",
            };

            sparkline = new SparklinePanel { Dock = DockStyle.Fill };

            Controls.Add(sparkline);
            Controls.Add(lblSubtext);
            Controls.Add(lblReading);

            timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) => Sample();
            Disposed += (s, e) => timer.Dispose();

            // Started here rather than on Load: WinForms only creates a TabPage's children
            // (and fires Load) once that tab is actually selected, which would otherwise
            // leave this panel frozen until the user clicks its tab.
            Sample();
            timer.Start();
        }

        private void Sample()
        {
            float value = CpuMonitor.SampleOnce();
            sparkline.AddSample(value);

            HealthStatus status =
                value >= CriticalPercent ? HealthStatus.Critical :
                value >= WarningPercent ? HealthStatus.Warning :
                HealthStatus.Normal;

            lblReading.Text = $"{value:0.0} %";
            lblReading.ForeColor = UiStyle.ColorFor(status);

            if (status != Status)
            {
                Status = status;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    // Minimal rolling line chart - just enough to show the last minute of CPU load without
    // pulling in a charting library for one sparkline.
    internal sealed class SparklinePanel : Panel
    {
        private const int MaxSamples = 60;
        private readonly Queue<float> samples = new Queue<float>();

        public SparklinePanel()
        {
            DoubleBuffered = true;
            BackColor = UiStyle.Panel;
        }

        public void AddSample(float value)
        {
            samples.Enqueue(value);
            while (samples.Count > MaxSamples)
                samples.Dequeue();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (samples.Count < 2 || Width <= 0 || Height <= 0)
                return;

            var pts = samples.ToArray();
            float stepX = (float)Width / (MaxSamples - 1);
            int startIndex = MaxSamples - pts.Length;

            using (var pen = new Pen(UiStyle.Accent, 2f))
            {
                PointF? previous = null;
                for (int i = 0; i < pts.Length; i++)
                {
                    float x = (startIndex + i) * stepX;
                    float y = Height - (pts[i] / 100f * Height);
                    var current = new PointF(x, y);
                    if (previous.HasValue)
                        e.Graphics.DrawLine(pen, previous.Value, current);
                    previous = current;
                }
            }
        }
    }
}
