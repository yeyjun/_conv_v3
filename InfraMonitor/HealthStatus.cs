namespace InfraMonitor
{
    // Ordered so HealthStatus.Max() across several panels picks the worst one.
    internal enum HealthStatus
    {
        Unknown = 0,
        Normal = 1,
        Warning = 2,
        Critical = 3,
    }
}
