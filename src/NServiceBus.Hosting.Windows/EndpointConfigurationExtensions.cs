namespace NServiceBus
{
    using System;
    using Configuration.AdvancedExtensibility;

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
            ValidateEndpointName(endpointName);
            configuration.GetSettings().Set("NServiceBus.Routing.EndpointName", endpointName);
        }

        private static void ValidateEndpointName(string endpointName)
        {
            if (string.IsNullOrWhiteSpace(endpointName))
                throw new ArgumentException("Endpoint name must not be empty", "endpointName");
            if (endpointName.Contains("@"))
                throw new ArgumentException("Endpoint name must not contain an '@' character.", "endpointName");
        }
    }
}