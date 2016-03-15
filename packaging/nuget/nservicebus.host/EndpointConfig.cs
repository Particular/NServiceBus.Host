
namespace rootnamespace
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            // NServiceBus provides multiple durable storage options please refer to our documentation for more details.
            // If you don't need a durable storage you can also use, configuration.UsePersistence<InMemoryPersistence>();
            configuration.UsePersistence<PLEASE_SELECT_ONE>();
        }
    }
}
