﻿namespace NServiceBus.Hosting.Windows.Profiles.Handlers
{
    using Hosting.Profiles;

    /// <summary>
    /// Handles the PerformanceCounters profile.
    /// </summary>
    class PerformanceCountersProfileHandler : IHandleProfile<PerformanceCounters>
    {
        public void ProfileActivated(EndpointConfiguration config)
        {
            config.EnableCriticalTimePerformanceCounter();
            config.EnableSLAPerformanceCounter();
        }
    }
}