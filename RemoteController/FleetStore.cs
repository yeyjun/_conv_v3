using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RemoteController
{
    /// <summary>One store's terminal, as known to this copy of RemoteController - purely local
    /// bookkeeping (there is no central directory/server anywhere in this system), so the same
    /// fleet has to be maintained separately on every machine that runs RemoteController.</summary>
    public sealed class FleetEntry
    {
        public string StoreName { get; set; } = "";
        public string Host { get; set; } = "";
        public int Port { get; set; } = 51820;
        public string Token { get; set; } = "";
        public string CertThumbprint { get; set; } = "";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Not persisted - populated by FleetStatusChecker.RefreshAllAsync each time it's run,
        // reset to Unknown on load since a saved status is stale the moment the app restarts.
        [XmlIgnore]
        public string Status { get; set; } = "Unknown";
        [XmlIgnore]
        public DateTime? LastCheckedUtc { get; set; }
    }

    /// <summary>
    /// Loads/saves the local fleet list as plain XML - no server, no sync between machines. The
    /// shared token for each terminal is stored here in the clear, same trust model as everything
    /// else in this app (LicenseKeyGenerator's salt, the .tmrgo file passphrase, etc.) - this file
    /// is exactly as sensitive as knowing the terminal's own System Config > Remote Control token,
    /// so keep it as carefully as you'd keep that.
    /// </summary>
    internal static class FleetStore
    {
        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "RemoteController", "Fleet.xml");

        public static List<FleetEntry> Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new List<FleetEntry>();

                var serializer = new XmlSerializer(typeof(List<FleetEntry>), new XmlRootAttribute("Fleet"));
                using (var stream = File.OpenRead(FilePath))
                    return (List<FleetEntry>)serializer.Deserialize(stream);
            }
            catch
            {
                // Missing/corrupt file just means an empty fleet, not a crash.
                return new List<FleetEntry>();
            }
        }

        public static void Save(List<FleetEntry> entries)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            var serializer = new XmlSerializer(typeof(List<FleetEntry>), new XmlRootAttribute("Fleet"));
            using (var stream = File.Create(FilePath))
                serializer.Serialize(stream, entries);
        }

        /// <summary>
        /// PING requires the correct token to come back OK, so this doubles as an auth check, not
        /// just a reachability check: "Online" means the terminal answered and the saved token is
        /// still correct; "Reachable" means something answered on that host:port but rejected the
        /// token (wrong/rotated token, or not actually this app); "Offline" means no connection
        /// could be made at all (network down, terminal off, wrong host/port).
        /// </summary>
        public static async Task<string> CheckStatusAsync(FleetEntry entry)
        {
            try
            {
                string response = await Task.Run(() =>
                    RemoteControlClient.SendCommand(entry.Host, entry.Port, entry.Token, "PING", null, entry.CertThumbprint));
                return response.StartsWith("OK", StringComparison.Ordinal) ? "Online" : "Reachable (bad token)";
            }
            catch
            {
                return "Offline";
            }
        }
    }
}
