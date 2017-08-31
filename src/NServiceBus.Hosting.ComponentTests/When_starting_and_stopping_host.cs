namespace NServiceBus.Hosting.ComponentTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class When_starting_and_stopping_host
    {
        [Test]
        public async Task Start_and_stop_are_called_for_scanned_type()
        {
            var context = await RunHost();

            Assert.True(context.ScannedStartCalled);
            Assert.True(context.ScannedStopCalled);
        }

        [Test]
        public async Task Start_and_stop_are_called_for_instance()
        {
            var context = await RunHost();

            Assert.True(context.InstanceStartCalled);
            Assert.True(context.InstanceStopCalled);
        }

        [Test]
        public async Task Instance_is_registered_once()
        {
            var context = await RunHost();

            Assert.AreEqual(1, context.InstanceTimesRegistered);
        }

        static async Task<IWantToRunContext> RunHost()
        {
            var defaultProfiles = new List<Type>
            {
                typeof(Production)
            };
            string[] args = {};

            var context = new IWantToRunContext();

            IConfigureThisEndpoint configurer = new GenericEndpointConfig(context);

            var host = new GenericHost(configurer, args, defaultProfiles, GenericEndpointConfig.EndpointName);

            await host.Start();
            await host.Stop();

            return context;
        }

        class IWantToRunContext
        {
            public bool ScannedStartCalled { get; set; }
            public bool ScannedStopCalled { get; set; }
            public bool InstanceStartCalled { get; set; }
            public bool InstanceStopCalled { get; set; }
            public int InstanceTimesRegistered { get; set; }
        }

        class GenericEndpointConfig : IConfigureThisEndpoint
        {
            public GenericEndpointConfig(IWantToRunContext context)
            {
                this.context = context;
            }

            public static string EndpointName = "When_starting_and_stopping_host";

            public void Customize(EndpointConfiguration configuration)
            {
                configuration.EnableInstallers();
                configuration.UseTransport<LearningTransport>();
                configuration.UsePersistence<LearningPersistence>();
                configuration.SendFailedMessagesTo("error");
                configuration.UsePersistence<InMemoryPersistence>();
                configuration.RegisterComponents(c => c.ConfigureComponent(() => context, DependencyLifecycle.SingleInstance));

                configuration.RunWhenEndpointStartsAndStops(new RunStuffInstance(context));
            }

            class RunStuffInstance : IWantToRunWhenEndpointStartsAndStops
            {
                public RunStuffInstance(IWantToRunContext context)
                {
                    this.context = context;
                    this.context.InstanceTimesRegistered++;
                }

                public Task Start(IMessageSession session)
                {
                    context.InstanceStartCalled = true;
                    return Task.FromResult(0);
                }

                public Task Stop(IMessageSession session)
                {
                    context.InstanceStopCalled = true;
                    return Task.FromResult(0);
                }

                IWantToRunContext context;
            }

            IWantToRunContext context;
        }

        class RunStuffScanned : IWantToRunWhenEndpointStartsAndStops
        {
            public RunStuffScanned(IWantToRunContext context)
            {
                this.context = context;
            }

            public Task Start(IMessageSession session)
            {
                context.ScannedStartCalled = true;
                return Task.FromResult(0);
            }

            public Task Stop(IMessageSession session)
            {
                context.ScannedStopCalled = true;
                return Task.FromResult(0);
            }

            IWantToRunContext context;
        }
    }
}