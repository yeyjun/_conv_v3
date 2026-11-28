using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventory
{
    internal sealed class PendingSelfUpdate
    {
        public UpdateInfo Info { get; set; }
        public string ZipPath { get; set; }
    }

    /// <summary>
    /// Downloads a release's self-update zip asset and hands off to the separate
    /// InventoryUpdater.exe helper to apply it - Inventory can never overwrite its own running
    /// files, so the actual file swap + relaunch happens in that other process after this one
    /// exits. Only ever touches the Inventory install folder (no admin/UAC needed there, see
    /// installer\AppInventory.iss's icacls grant) - this intentionally does not go through the
    /// full 4-app Inno Setup bundle.
    /// </summary>
    internal static class SelfUpdateService
    {
        private const string UpdaterExeName = "InventoryUpdater.exe";

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

        private static string UpdatesRoot => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "App Inventory", "OfflineCache", "Updates");

        /// <summary>
        /// Fails open (returns null) on any error - network failure, size mismatch, disk full -
        /// matching this app's offline-resilience convention. A null result means "try again on
        /// the next poll", not "update doesn't exist".
        /// </summary>
        public static async Task<string> DownloadUpdateZipAsync(UpdateInfo info)
        {
            if (string.IsNullOrWhiteSpace(info?.AssetDownloadUrl))
                return null;

            try
            {
                string versionDir = Path.Combine(UpdatesRoot, SafeFolderName(info.TagName ?? info.Version?.ToString()));
                Directory.CreateDirectory(versionDir);
                string zipPath = Path.Combine(versionDir, "update.zip");

                // Already staged correctly from a previous attempt - no need to re-fetch.
                if (File.Exists(zipPath) && info.AssetSize > 0 && new FileInfo(zipPath).Length == info.AssetSize)
                    return zipPath;

                using (HttpResponseMessage response = await _httpClient.GetAsync(info.AssetDownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                        return null;

                    using (Stream source = await response.Content.ReadAsStreamAsync())
                    using (FileStream dest = File.Create(zipPath))
                        await source.CopyToAsync(dest);
                }

                if (info.AssetSize > 0 && new FileInfo(zipPath).Length != info.AssetSize)
                {
                    File.Delete(zipPath);
                    return null;
                }

                return zipPath;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hands off to InventoryUpdater.exe and returns whether it was launched successfully.
        /// Caller is responsible for exiting the app afterward - this never kills the process
        /// itself, since the caller may want to retry on failure instead of losing the pending
        /// update.
        /// </summary>
        public static bool LaunchInstaller(string zipPath)
        {
            try
            {
                string updaterExe = FindUpdaterExePath();
                if (updaterExe == null || !File.Exists(zipPath))
                    return false;

                string installDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                string exeName = Path.GetFileName(Application.ExecutablePath);
                int pid = Process.GetCurrentProcess().Id;

                var psi = new ProcessStartInfo(updaterExe)
                {
                    Arguments = $"\"{zipPath}\" \"{installDir}\" \"{exeName}\" {pid}",
                    UseShellExecute = false,
                };
                Process.Start(psi);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Same dual-layout resolution as Launcher\LauncherForm.cs's FindExePath: installed
        /// layout has InventoryUpdater as a sibling folder of Inventory under {app}; dev layout
        /// walks up from bin\&lt;Config&gt; to the repo root then into InventoryUpdater\bin\&lt;Config&gt;.
        /// </summary>
        private static string FindUpdaterExePath()
        {
            var ownDirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            DirectoryInfo installRoot = ownDirInfo.Parent;
            if (installRoot != null)
            {
                string installedCandidate = Path.Combine(installRoot.FullName, "InventoryUpdater", UpdaterExeName);
                if (File.Exists(installedCandidate))
                    return installedCandidate;
            }

            string ownConfig = ownDirInfo.Name;
            DirectoryInfo repoRoot = ownDirInfo.Parent?.Parent?.Parent;
            if (repoRoot != null)
            {
                foreach (string config in new[] { ownConfig, "Debug", "Release" })
                {
                    string devCandidate = Path.Combine(repoRoot.FullName, "InventoryUpdater", "bin", config, UpdaterExeName);
                    if (File.Exists(devCandidate))
                        return devCandidate;
                }
            }

            return null;
        }

        private static string SafeFolderName(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
                return "unknown";

            foreach (char c in Path.GetInvalidFileNameChars())
                tagName = tagName.Replace(c, '_');

            return tagName;
        }
    }
}
