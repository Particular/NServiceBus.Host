
namespace rootnamespace
{
    using NServiceBus;

    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
            // NServiceBus provides multiple durable storage options
            
            // If you don't need a durable storage you can also use, configuration.UsePersistence<InMemoryPersistence>();
            
            //Also note that you can mix and match multiple storages to fit you specific needs. 
            configuration.UsePersistence<PLEASE_SELECT_ONE>();
        }
    }
}
