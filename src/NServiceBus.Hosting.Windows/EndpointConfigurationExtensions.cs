namespace NServiceBus
{
    using Configuration.AdvanceExtensibility;

    /// <summary>
    /// Extentions for <see cref="EndpointConfiguration"/>.
    /// </summary>
    public static class EndpointConfigurationExtensions
    {
        /// <summary>
        /// Defines the endpoint name in code thus overriding the previously assigned endpoint name.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="endpointName">The endpoint name to be used.</param>
        public static void DefineEndpointName(this EndpointConfiguration configuration, string endpointName)
        {
            configuration.GetSettings().Set("NServiceBus.Routing.EndpointName", endpointName);
        }
    }
}