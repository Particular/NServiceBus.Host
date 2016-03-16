namespace EndpointTypeDeterminerTests
{
    using NServiceBus;

    class MyEndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration configuration)
        {

        }
    }
}