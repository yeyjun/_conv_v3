using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace InventoryUpdater
{
    /// <summary>
    /// Standalone helper, launched by Inventory.exe (A10D-up.exe) right before it exits, with no
    /// project reference back to Inventory - same "handed to someone who doesn't build the POS
    /// app" reasoning RemoteController/Remittancce/InfraMonitor already follow for their own
    /// standalone-ness. Inventory can never overwrite its own running files, so this process
    /// waits for it to exit, then does the actual file swap and relaunch.
    ///
    /// Args: [0] staged update zip path, [1] Inventory install directory, [2] Inventory exe file
    /// name, [3] PID of the Inventory process to wait for.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Log("Usage: InventoryUpdater.exe <zipPath> <installDir> <exeName> <pid>");
                return 1;
            }

            string zipPath = args[0];
            string installDir = args[1];
            string exeName = args[2];
            int pid = int.TryParse(args[3], out int p) ? p : -1;
            string exePath = Path.Combine(installDir, exeName);

            Log($"Starting update. zip={zipPath} installDir={installDir} exe={exeName} pid={pid}");

            try
            {
                WaitForProcessExit(pid);

                string extractDir = Path.Combine(Path.GetDirectoryName(zipPath) ?? installDir, "extracted");
                if (Directory.Exists(extractDir))
                    Directory.Delete(extractDir, recursive: true);

                Log($"Extracting {zipPath} to {extractDir}");
                ZipFile.ExtractToDirectory(zipPath, extractDir);

                try
                {
                    if (File.Exists(exePath))
                        File.Copy(exePath, exePath + ".bak", overwrite: true);
                }
                catch (Exception ex)
                {
                    // Best-effort rollback safety net only - not worth failing the update over.
                    Log("Could not back up existing exe (continuing): " + ex.Message);
                }

                Log($"Copying {extractDir} over {installDir}");
                CopyDirectoryOverwrite(extractDir, installDir);

                try
                {
                    string stagingRoot = Path.GetDirectoryName(zipPath);
                    if (stagingRoot != null && Directory.Exists(stagingRoot))
                        Directory.Delete(stagingRoot, recursive: true);
                }
                catch (Exception ex)
                {
                    Log("Could not clean up staging directory (non-fatal): " + ex.Message);
                }

                Log($"Relaunching {exePath}");
                Process.Start(new ProcessStartInfo(exePath) { WorkingDirectory = installDir, UseShellExecute = true });

                Log("Update applied successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                Log("Update FAILED: " + ex);
                return 1;
            }
        }

        private static void WaitForProcessExit(int pid)
        {
            if (pid <= 0)
                return;

            try
            {
                using (Process proc = Process.GetProcessById(pid))
                {
                    Log($"Waiting for PID {pid} to exit...");
                    proc.WaitForExit(60 * 1000);
                }
            }
            catch (ArgumentException)
            {
                // Already exited before we got here - nothing to wait for.
            }

            // Give the OS a moment to fully release file handles after the process exits.
            Thread.Sleep(500);
        }

        private static void CopyDirectoryOverwrite(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relative = dir.Substring(sourceDir.Length).TrimStart(Path.DirectorySeparatorChar);
                Directory.CreateDirectory(Path.Combine(destDir, relative));
            }

            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relative = file.Substring(sourceDir.Length).TrimStart(Path.DirectorySeparatorChar);
                string destFile = Path.Combine(destDir, relative);
                CopyFileWithRetry(file, destFile);
            }
        }

        private static void CopyFileWithRetry(string sourceFile, string destFile)
        {
            const int maxAttempts = 5;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    File.Copy(sourceFile, destFile, overwrite: true);
                    return;
                }
                catch (IOException) when (attempt < maxAttempts)
                {
                    // File may still be momentarily locked right after the Inventory process exits.
                    Thread.Sleep(300);
                }
            }
        }

        private static readonly object LogLock = new object();

        private static void Log(string message)
        {
            try
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "App Inventory", "OfflineCache", "Updates");
                Directory.CreateDirectory(dir);

                lock (LogLock)
                {
                    File.AppendAllText(
                        Path.Combine(dir, "UpdateLog.txt"),
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
                }
            }
            catch
            {
                // Logging is best-effort only - never let it take down the update itself.
            }
        }
    }
}
