namespace NServiceBus
{
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>
    /// Implementers will be invoked when the endpoint starts up.
    /// Dependency injection is provided for these types.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IWantToRunWhenEndpointStartsAndStops
    {
        /// <summary>
        /// Method called at startup.
        /// </summary>
        /// <param name="session">Session to access basic message operations at startup. </param>
        /// <exception cref="System.Exception">This exception will be thrown if <code>null</code> is returned. Return a Task or mark the method as <code>async</code>.</exception>
        Task Start(IMessageSession session);

        /// <summary>
        /// Method called at shutdown.
        /// </summary>
        /// <param name="session">Session to access basic message operations at shutdown. </param>
        /// <exception cref="System.Exception">This exception will be thrown if <code>null</code> is returned. Return a Task or mark the method as <code>async</code>.</exception>
        Task Stop(IMessageSession session);
    }
}