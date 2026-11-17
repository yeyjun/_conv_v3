using System.Diagnostics;

namespace InfraMonitor
{
    // Single shared counter so the System Health overview and the CPU Health tab read the
    // same consistent samples instead of each opening its own "% Processor Time" counter.
    internal static class CpuMonitor
    {
        private static readonly PerformanceCounter counter =
            new PerformanceCounter("Processor", "% Processor Time", "_Total");

        static CpuMonitor()
        {
            counter.NextValue(); // first call after construction always returns 0 - discard it
        }

        public static float SampleOnce() => counter.NextValue();
    }
}
