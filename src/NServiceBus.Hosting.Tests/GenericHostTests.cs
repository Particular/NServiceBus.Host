using System;
using System.Threading;
using NServiceBus;
using NServiceBus.Satellites;

namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using NServiceBus.Config;
    using NServiceBus.Config.ConfigurationSource;
    using NServiceBus.Hosting.Tests.EndpointTypeTests;
    using NServiceBus.Hosting.Windows;
    using NServiceBus.Satellites;
    using NServiceBus.Unicast.Transport;
    using NUnit.Framework;
    using GenericHost = NServiceBus.GenericHost;

    [TestFixture]
    class GenericHostTests
    {
        [Test]
        public void SomeTest()
        {
            string[] args = new string[0];
            var specifier = new MyTestEndpoint();

            GenericHost host = new GenericHost(specifier, args, new List<Type> { typeof(Production) }, "nservicebus");

            host.Start();
            host.Stop();
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
