namespace EndpointTypeDeterminerTests
{
    using NServiceBus;

    //referenced from in app.config
#pragma warning disable 0618
    class MyEndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration configuration)
        {

        }
    }
#pragma warning restore 0618
}