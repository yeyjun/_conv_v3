using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Launcher
{
    // Picks between the standalone apps that live side by side in this solution - Inventory
    // (the POS app, built as A10D-up.exe), Remittancce (the "send money" / padala counter
    // tool), and InfraMonitor (local terminal health dashboard). None of these apps
    // reference each other, so this just shells out to whichever .exe was picked via
    // Process.Start and closes itself immediately after.
    public partial class LauncherForm : Form
    {
        private const string InventoryProjectFolder = "Inventory";
        private const string InventoryExeName = "A10D-up.exe";
        private const string RemittanceProjectFolder = "Remittancce";
        private const string RemittanceExeName = "Remittancce.exe";
        private const string InfraMonitorProjectFolder = "InfraMonitor";
        private const string InfraMonitorExeName = "InfraMonitor.exe";

        public LauncherForm()
        {
            InitializeComponent();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            Launch(InventoryProjectFolder, InventoryExeName, "Inventory");
        }

        private void btnRemittance_Click(object sender, EventArgs e)
        {
            Launch(RemittanceProjectFolder, RemittanceExeName, "Remittance");
        }

        private void btnInfraMonitor_Click(object sender, EventArgs e)
        {
            Launch(InfraMonitorProjectFolder, InfraMonitorExeName, "Infrastructure Monitor");
        }

        private void Launch(string projectFolder, string exeName, string displayName)
        {
            string exePath = FindExePath(projectFolder, exeName);
            if (exePath == null)
            {
                MessageBox.Show(this,
                    $"Could not find {exeName}. Build the {projectFolder} project (Debug or Release) first.",
                    $"{displayName} Not Built", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(exePath) { WorkingDirectory = Path.GetDirectoryName(exePath) });
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not start {displayName}:\n\n{ex.Message}", "Launch Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Two layouts to support:
        //  - Installed (AppInventory.iss): {app}\Launcher\Launcher.exe sits next to
        //    {app}\Inventory\<exe> and {app}\Remittancce\<exe> - one level up from Launcher's
        //    own folder, then into the sibling project folder.
        //  - Dev build: repoRoot\Launcher\bin\<Configuration>\Launcher.exe, with the sibling
        //    app at repoRoot\<ProjectFolder>\bin\<Configuration>\<exe> - repoRoot is three
        //    levels above Launcher's own bin\<Configuration> folder. Tries Launcher's own
        //    configuration first, then falls back to the other in case only one was built.
        private static string FindExePath(string projectFolder, string exeName)
        {
            string launcherDir = AppDomain.CurrentDomain.BaseDirectory;
            var launcherDirInfo = new DirectoryInfo(launcherDir);

            DirectoryInfo installRoot = launcherDirInfo.Parent;
            if (installRoot != null)
            {
                string installedCandidate = Path.Combine(installRoot.FullName, projectFolder, exeName);
                if (File.Exists(installedCandidate))
                    return installedCandidate;
            }

            string ownConfig = launcherDirInfo.Name;
            DirectoryInfo repoRoot = launcherDirInfo.Parent?.Parent?.Parent;
            if (repoRoot != null)
            {
                foreach (string config in new[] { ownConfig, "Debug", "Release" })
                {
                    string devCandidate = Path.Combine(repoRoot.FullName, projectFolder, "bin", config, exeName);
                    if (File.Exists(devCandidate))
                        return devCandidate;
                }
            }

            return null;
        }
    }
}
