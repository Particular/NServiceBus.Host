namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Threading.Tasks;
    using Transport;

    class FakeReceiver : IPushMessages
    {
        public FakeReceiver(Exception throwCritical)
        {
            this.throwCritical = throwCritical;
        }

        public Task Init(Func<MessageContext, Task> onMessage, Func<ErrorContext, Task<ErrorHandleResult>> onError, CriticalError criticalError, PushSettings settings)
        {
            this.criticalError = criticalError;
            return Task.FromResult(0);
        }

        public void Start(PushRuntimeSettings limitations)
        {
            if (throwCritical != null)
            {
                criticalError.Raise(throwCritical.Message, throwCritical);
            }
        }

        public Task Stop()
        {
            return Task.FromResult(0);
        }

        CriticalError criticalError;
        Exception throwCritical;
    }
}