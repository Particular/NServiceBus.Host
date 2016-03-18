
namespace rootnamespace
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration endpointConfiguration)
        {
            // NServiceBus provides multiple durable storage options, including SQL Server, RavenDB, and Azure Storage Persistence. 
            // Refer to the documentation for each option for more details.
            endpointConfiguration.UsePersistence<PLEASE_SELECT_ONE>();
        }
    }
}
