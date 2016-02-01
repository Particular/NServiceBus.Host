namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Satellites;
    using NServiceBus.Unicast.Transport;
    using NUnit.Framework;
    using GenericHost = NServiceBus.GenericHost;

    [TestFixture]
    class GenericHostTests
    {
        // This test is on ignore, becaise
        // #1 It takes 30+ seconds to verify if it's working.
        // #2 It uses MSMQ as it is starting the actual bus and the host doesn't have acceptance tests.
        // Still leaving it here at the moment for reference.
        [Test, Ignore]
        public void VerifyThirtySecondTimeoutToWork()
        {
            string[] args = new string[0];
            var specifier = new MyTestEndpoint();

            GenericHost host = new GenericHost(specifier, args, new List<Type> { typeof(Production) }, "nservicebus");

            host.Start();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            host.Stop();
            sw.Stop();

            Assert.IsTrue(sw.Elapsed.Seconds < 35);
        }

        class MyTestEndpoint : IConfigureThisEndpoint, IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
        {
            public void Customize(BusConfiguration configuration)
            {
                configuration.UsePersistence<InMemoryPersistence>();
                configuration.EnableInstallers();
            }

            public MessageForwardingInCaseOfFaultConfig GetConfiguration()
            {
                return new MessageForwardingInCaseOfFaultConfig
                {
                    ErrorQueue = "error"
                };
            }
        }
    }

    public class MyCustomSatellite : IAdvancedSatellite
    {
        public bool Handle(TransportMessage message)
        {
            return true;
        }

        public void Start()
        {
        }

        public void Stop()
        {
            const int millisecondsTimeout = 35000;
            Thread.Sleep(millisecondsTimeout);
        }

        public Address InputAddress { get; }
        public bool Disabled { get; }
        public Action<TransportReceiver> GetReceiverCustomization()
        {
            throw new NotImplementedException();
        }
    }
}
