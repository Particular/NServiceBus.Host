namespace NServiceBus
{
    using System.Collections.Generic;
    using Configuration.AdvanceExtensibility;

    /// <summary>
    /// Extension methods for EndpointConfiguration
    /// </summary>
    public static class EndpointStartableAndStoppableExtensions
    {
        /// <summary>
        /// Register a specific instance of an IWantToRunWhenEndpointStartsAndStops implementation
        /// </summary>
        public static void RunWhenEndpointStartsAndStops(this EndpointConfiguration configuration, IWantToRunWhenEndpointStartsAndStops startableAndStoppable)
        {
            var settings = configuration.GetSettings();

            List<IWantToRunWhenEndpointStartsAndStops> instanceList;

            if (!settings.TryGet(out instanceList))
            {
                instanceList = new List<IWantToRunWhenEndpointStartsAndStops>();
                settings.Set<List<IWantToRunWhenEndpointStartsAndStops>>(instanceList);
            }

            instanceList.Add(startableAndStoppable);
        }
    }
}