namespace EndpointTypeDeterminerTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Hosting.Helpers;
    using NServiceBus.Hosting.Windows;
    using NUnit.Framework;

    [TestFixture]
    class GetEndpointConfigurationTypeTests
    {
        [Test]
        public void when_multiple_endpoint_types_found_via_assembly_scanning_it_should_blow_up()
        {
            var endpointTypeDeterminer = new EndpointTypeDeterminer(new AssemblyScanner(), () => null);
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var type = endpointTypeDeterminer.GetEndpointConfigurationType().Type;
            });
            Assert.That(exception.Message, Is.StringStarting("Host doesn't support hosting of multiple endpoints"));
        }

        [Test]
        public void when_endpoint_type_is_provided_via_configuration_it_should_have_first_priority()
        {
            var EndpointTypeDefinedInConfigurationFile = typeof(MyEndpointConfig);
            var EndpointTypeDeterminer = new EndpointTypeDeterminer(new AssemblyScanner(),
                () => ConfigurationManager.AppSettings["EndpointConfigurationType"]);

            var RetrievedEndpointType = EndpointTypeDeterminer.GetEndpointConfigurationType().Type;

            Assert.AreEqual(EndpointTypeDefinedInConfigurationFile, RetrievedEndpointType);
        }

        [Test]
        public void when_invalid_endpoint_type_is_provided_via_configuration_it_should_blow_up()
        {
            var AssemblyScanner = new AssemblyScanner();
            var endpointTypeDeterminer = new EndpointTypeDeterminer(AssemblyScanner, () => "I am an invalid type name");

            var exception = Assert.Throws<ConfigurationErrorsException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var type = endpointTypeDeterminer.GetEndpointConfigurationType().Type;
            });
            Assert.That(exception.Message, Is.StringStarting("The 'EndpointConfigurationType' entry in the NServiceBus.Host.exe.config"));
        }

        [Test]
        public void when_no_endpoint_type_found_via_configuration_or_assembly_scanning_it_should_blow_up()
        {
            var AssemblyScanner = new AssemblyScanner
            {
                IncludeExesInScan = false,
                AssembliesToSkip = new List<string>
                {
                    Assembly.GetExecutingAssembly().GetName().Name
                }
            };

            var endpointTypeDeterminer = new EndpointTypeDeterminer(AssemblyScanner, () => null);

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var type = endpointTypeDeterminer.GetEndpointConfigurationType().Type;
            });
            Assert.That(exception.Message, Is.StringStarting("No endpoint configuration found in scanned assemblies"));
        }

        //this will cause more than one config to be found when scanning and make the when_multiple_endpoint_types_found_via_assembly_scanning_it_should_blow_up test pass
        class MyEndpointConfig2 : IStartThisEndpoint
        {
            public Task<IEndpointInstance> Start(string proposedEndpointName, Action<EndpointConfiguration> applyHostConventions)
            {
                var configuration = new EndpointConfiguration(proposedEndpointName);
                applyHostConventions(configuration);
                return Task.FromResult(default(IEndpointInstance));
            }
        }
    }
}