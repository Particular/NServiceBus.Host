namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus.Transports;

    class FakeReceiver : IPushMessages
    {
        CriticalError criticalError;
        Exception throwCritical;

        public Task Init(Func<PushContext, Task> pipe, CriticalError criticalError, PushSettings settings)
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

        public FakeReceiver(Exception throwCritical)
        {
            this.throwCritical = throwCritical;
        }
    }
}