namespace EndpointTypeDeterminerTests
{
    using NServiceBus;

    //referenced from in app.config
    class MyEndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration configuration)
        {

        }
    }
}