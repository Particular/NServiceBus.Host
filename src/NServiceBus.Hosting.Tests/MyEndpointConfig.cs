namespace EndpointTypeDeterminerTests
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    //referenced from in app.config
    class MyEndpointConfig : IStartThisEndpoint
    {
        public Task<IEndpointInstance> Start(string proposedEndpointName, Action<EndpointConfiguration> applyHostConventions)
        {
            var configuration = new EndpointConfiguration(proposedEndpointName);
            applyHostConventions(configuration);
            return Task.FromResult(default(IEndpointInstance));
        }
    }
}