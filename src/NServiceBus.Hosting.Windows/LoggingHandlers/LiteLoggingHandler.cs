namespace NServiceBus.Hosting.Windows.LoggingHandlers
{
    using Hosting.Profiles;

    /// <summary>
    /// Handles logging configuration for the <see cref="Lite"/> profile.
    /// </summary>
    class LiteLoggingHandler : IConfigureLoggingForProfile<Lite>
    {
        public void Configure(IConfigureThisEndpoint specifier)
        {
        }
    }
}