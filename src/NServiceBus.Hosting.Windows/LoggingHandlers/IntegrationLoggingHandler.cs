namespace NServiceBus.Hosting.Windows.LoggingHandlers
{
    using Hosting.Profiles;

    /// <summary>
    /// Handles logging configuration for the <see cref="Integration"/> profile.
    /// </summary>
    class IntegrationLoggingHandler : IConfigureLoggingForProfile<Integration>
    {
        public void Configure(IConfigureThisEndpoint specifier)
        {
        }
    }
}