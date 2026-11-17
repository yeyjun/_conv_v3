using System.Linq;
using System.Windows.Forms;

namespace InfraMonitor
{
    public partial class MainForm : Form
    {
        private readonly SystemHealthPanel systemHealthPanel = new SystemHealthPanel();
        private readonly StorageWatchPanel storagePanel = new StorageWatchPanel();
        private readonly CpuHealthPanel cpuPanel = new CpuHealthPanel();
        private readonly IoEventsPanel ioPanel = new IoEventsPanel();

        public MainForm()
        {
            InitializeComponent();
            BackColor = UiStyle.Background;

            tabPageSystemHealth.Controls.Add(systemHealthPanel);
            tabPageStorageWatch.Controls.Add(storagePanel);
            tabPageCpuHealth.Controls.Add(cpuPanel);
            tabPageIoEvents.Controls.Add(ioPanel);

            storagePanel.StatusChanged += (s, e) => UpdateOverallStatus();
            cpuPanel.StatusChanged += (s, e) => UpdateOverallStatus();
            ioPanel.StatusChanged += (s, e) => UpdateOverallStatus();

            // Each panel runs its first check in its own constructor (see their comments),
            // which happens before these event subscriptions above exist - so pick up
            // whatever they already found rather than waiting for the next status change.
            UpdateOverallStatus();
        }

        // Overall status is the worst of the three sub-systems; Unknown entries (a panel that
        // hasn't completed its first check yet) are excluded unless every panel is still Unknown.
        private void UpdateOverallStatus()
        {
            var known = new[] { cpuPanel.Status, storagePanel.Status, ioPanel.Status }
                .Where(status => status != HealthStatus.Unknown)
                .ToArray();

            HealthStatus overall = known.Length == 0 ? HealthStatus.Unknown : known.Max();
            string detail = overall == HealthStatus.Normal
                ? "CPU, storage, and disk I/O are all within normal range."
                : overall == HealthStatus.Unknown
                    ? "Waiting for the first health check to complete..."
                    : "Check the Storage Watch, CPU Health, or I/O tab showing Warning or Critical.";

            systemHealthPanel.SetOverall(overall, detail);
        }
    }
}
