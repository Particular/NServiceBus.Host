namespace NServiceBus
{
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
        /// <param name="configuration">Endpoint configuration builder.</param>
        Task<IEndpointInstance> Start(EndpointConfiguration configuration);
    }
}
