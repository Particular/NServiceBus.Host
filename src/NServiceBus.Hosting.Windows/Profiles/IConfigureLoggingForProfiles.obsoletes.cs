namespace NServiceBus.Hosting.Profiles
{
    /// <summary>
    /// Configure logging in the constructor of the class that implements IConfigureThisEndpoint.
    /// </summary>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "Configure logging in the constructor of the class that implements IConfigureThisEndpoint.",
        RemoveInVersion = "8.0")]
    public partial interface IConfigureLogging
    {
    }

    /// <summary>
    /// Called in order to configure logging for the given profile type.
    /// If an implementation isn't found for a given profile, then the search continues
    /// recursively up that profile's inheritance hierarchy.
    /// </summary>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "Configure logging in the constructor of the class that implements IConfigureThisEndpoint.",
        RemoveInVersion = "8.0")]
    public partial interface IConfigureLoggingForProfile<T>
    {
    }
}