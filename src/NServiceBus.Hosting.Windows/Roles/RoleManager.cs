namespace NServiceBus
{
    using System;
    using System.Linq;
    using Configuration.AdvancedExtensibility;
    using Features;

    class RoleManager
    {
        public static void TweakConfigurationBuilder(IConfigureThisEndpoint specifier, EndpointConfiguration config)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (specifier is AsA_Client)
            {
                config.PurgeOnStartup(true);
                config.GetSettings().Set<TransportTransactionMode>(TransportTransactionMode.None);

                config.Recoverability().Delayed(delayed => delayed.NumberOfRetries(0));
                config.DisableFeature<TimeoutManager>();
            }

            Type transportDefinitionType;
            if (TryGetTransportDefinitionType(specifier, out transportDefinitionType))
            {
                config.UseTransport(transportDefinitionType);
            }
        }

        static bool TryGetTransportDefinitionType(IConfigureThisEndpoint specifier, out Type transportDefinitionType)
        {
            var transportType= specifier.GetType()
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .SingleOrDefault(x => x.GetGenericTypeDefinition() == typeof(UsingTransport<>));
            if (transportType != null)
            {
                transportDefinitionType = transportType.GetGenericArguments().First();
                return true;
            }
            transportDefinitionType = null;
            return false;
        }
    }


}