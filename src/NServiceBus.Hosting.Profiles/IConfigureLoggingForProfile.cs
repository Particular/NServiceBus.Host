namespace NServiceBus.Hosting.Profiles
{
    using NServiceBus;

    /// <summary>
    /// Configure logging in the constructor of the class that implements IConfigureThisEndpoint.
    /// </summary>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "7.0",
        Message = "Configure logging in the constructor of the class that implements IConfigureThisEndpoint.",
        RemoveInVersion = "8.0")]
    public interface IConfigureLogging
    {
        /// <summary>
        /// Performs all logging configuration.
        /// </summary>
        // ReSharper disable once UnusedParameter.Global
        void Configure(IConfigureThisEndpoint specifier);
    }

    /// <summary>
    /// Called in order to configure logging for the given profile type.
    /// If an implementation isn't found for a given profile, then the search continues
    /// recursively up that profile's inheritance hierarchy.
    /// </summary>
    public interface IConfigureLoggingForProfile<T> : IConfigureLogging where T : IProfile {}
}