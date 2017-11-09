#pragma warning disable 1591
namespace NServiceBus
{

    [ObsoleteEx(
        RemoveInVersion = "9.0",
        TreatAsErrorFromVersion = "8.0",
        Message =
        "PerformanceCounters has been moved to the external nuget package 'NServiceBus.Metrics.PerformanceCounters'. " +
        "Add an extra package reference and then call endpointConfiguration.EnableCriticalTimePerformanceCounter(); and endpointConfiguration.EnableSLAPerformanceCounter(); inside the IConfigureThisEndpoint.Customize().")]
    public interface PerformanceCounters : IProfile
    {
    }
}
