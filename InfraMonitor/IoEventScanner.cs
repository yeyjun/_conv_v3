using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InfraMonitor
{
    internal sealed class IoEventRecord
    {
        public DateTime TimeGenerated;
        public string Level;
        public string Source;
        public long EventId;
        public string Message;
    }

    // Scans the Windows "System" event log for disk/storage-related Error and Warning entries
    // - the same sources Event Viewer itself groups under disk I/O problems (the disk class
    // driver, NTFS, volume manager, and common storage controller drivers). This app has no DB
    // or network dependency, so a failure here (no permission to read the log, log not present)
    // is swallowed and surfaced as an error message rather than crashing - the same
    // offline-resilience convention the rest of the suite follows.
    internal static class IoEventScanner
    {
        private static readonly string[] DiskRelatedSources =
        {
            "disk", "Disk", "Ntfs", "volmgr", "volsnap", "partmgr",
            "iaStorA", "iaStorAVC", "storahci", "stornvme", "msahci",
            "Microsoft-Windows-Ntfs", "Microsoft-Windows-Disk", "Microsoft-Windows-StorageSpaces-Driver",
        };

        private const int MaxEntriesScanned = 5000;
        private const int MaxResults = 200;

        public static List<IoEventRecord> ScanRecent(TimeSpan lookback, out string error)
        {
            error = null;
            var results = new List<IoEventRecord>();
            try
            {
                using (var log = new EventLog("System"))
                {
                    var entries = log.Entries;
                    DateTime cutoff = DateTime.Now - lookback;
                    int scanned = 0;
                    for (int i = entries.Count - 1; i >= 0 && scanned < MaxEntriesScanned && results.Count < MaxResults; i--)
                    {
                        scanned++;
                        var entry = entries[i];
                        if (entry.TimeGenerated < cutoff)
                            break; // entries are chronological, nothing older matters

                        if (entry.EntryType != EventLogEntryType.Error && entry.EntryType != EventLogEntryType.Warning)
                            continue;
                        if (!DiskRelatedSources.Any(s => string.Equals(s, entry.Source, StringComparison.OrdinalIgnoreCase)))
                            continue;

                        results.Add(new IoEventRecord
                        {
                            TimeGenerated = entry.TimeGenerated,
                            Level = entry.EntryType == EventLogEntryType.Error ? "Error" : "Warning",
                            Source = entry.Source,
                            EventId = entry.InstanceId & 0xFFFF,
                            Message = FirstLine(entry.Message),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return results;
        }

        private static string FirstLine(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";
            int idx = message.IndexOf('\n');
            string line = idx >= 0 ? message.Substring(0, idx) : message;
            return line.Trim();
        }
    }
}
