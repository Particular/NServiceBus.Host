namespace NServiceBus
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Indicate that the implementing class will specify configuration.
    /// </summary>
    [ObsoleteEx(Message = "")]
    public interface IConfigureThisEndpoint
    {
        /// <summary>
        /// Allows to override default settings.
        /// </summary>
        /// <param name="configuration">Endpoint configuration builder.</param>
        void Customize(EndpointConfiguration configuration);
    }

    /// <summary>
    /// Indicate that the implementing class will specify configuration.
    /// </summary>
    public interface IStartThisEndpoint
    {
        /// <summary>
        /// Allows to override default settings.
        /// </summary>
        /// <param name="proposedEndpointName">The proposed endpoint name.</param>
        /// <param name="applyHostConventions">Apply the host conventions.</param>
        Task<IEndpointInstance> Start(string proposedEndpointName, Action<EndpointConfiguration> applyHostConventions);
    }
}
