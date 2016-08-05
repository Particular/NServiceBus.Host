namespace NServiceBus.Hosting.Tests
{
    using System.Threading.Tasks;
    using Extensibility;
    using Transport;

    class FakeDispatcher : IDispatchMessages
    {
        public Task Dispatch(TransportOperations outgoingMessages, TransportTransaction transportTransaction, ContextBag context)
        {
            return Task.FromResult(0);
        }
    }
}