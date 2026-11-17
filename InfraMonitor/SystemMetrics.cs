using System;
using Microsoft.VisualBasic.Devices;

namespace InfraMonitor
{
    internal static class SystemMetrics
    {
        public static string MachineName => Environment.MachineName;
        public static string OsDescription => Environment.OSVersion.VersionString;
        public static int LogicalProcessorCount => Environment.ProcessorCount;

        // Environment.TickCount wraps roughly every 24.9 days; casting through uint keeps the
        // value non-negative up to that point, which is more than enough for a POS terminal
        // that gets rebooted far more often than that.
        public static TimeSpan Uptime => TimeSpan.FromMilliseconds((uint)Environment.TickCount);

        public static (double totalGb, double availableGb, double usedPercent) GetMemoryInfo()
        {
            var info = new ComputerInfo();
            double totalGb = info.TotalPhysicalMemory / 1024.0 / 1024.0 / 1024.0;
            double availableGb = info.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0;
            double usedPercent = totalGb <= 0 ? 0 : (totalGb - availableGb) / totalGb * 100.0;
            return (totalGb, availableGb, usedPercent);
        }
    }
}
