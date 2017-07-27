// ReSharper disable PartialTypeWithSinglePart
namespace NServiceBus.Hosting.Profiles
{
    using NServiceBus;

    /// <summary>
    /// Configure logging in the constructor of the class that implements IConfigureThisEndpoint.
    /// </summary>
    public partial interface IConfigureLogging
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
    public partial interface IConfigureLoggingForProfile<T> : IConfigureLogging where T : IProfile {}
}