namespace EndpointTypeDeterminerTests
{
    using System.Threading.Tasks;
    using NServiceBus;

    //referenced from in app.config
    class MyEndpointConfig : IStartThisEndpoint
    {
        public Task<IEndpointInstance> Start(EndpointConfiguration configuration)
        {
            return Task.FromResult(default(IEndpointInstance));
        }
    }
}