namespace NServiceBus.Hosting.Windows.Profiles.Handlers
{
    using System;
    using Hosting.Profiles;

    /// <summary>
    /// Handles the PerformanceCounters profile.
    /// </summary>
    class PerformanceCountersProfileHandler : IHandleProfile<PerformanceCounters>
    {
        public void ProfileActivated(EndpointConfiguration config)
        {
            var performanceCounters = config.EnableWindowsPerformanceCounters();
            performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(100));
            performanceCounters.UpdateCounterEvery(TimeSpan.FromSeconds(2));
        }
    }
}