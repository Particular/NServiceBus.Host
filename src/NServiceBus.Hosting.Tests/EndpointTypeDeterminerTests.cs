namespace EndpointTypeDeterminerTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using NServiceBus.Hosting.Helpers;
    using NServiceBus.Hosting.Tests;
    using NServiceBus.Hosting.Tests.EndpointTypeTests;
    using NServiceBus.Hosting.Windows;
    using NServiceBus.Hosting.Windows.Arguments;
    using NUnit.Framework;

    [TestFixture]
    class GetEndpointConfigurationTypeForHostedEndpoint_Tests 
    {
        
        [Test]
        public void when_endpoint_type_is_not_provided_via_hostArgs_it_should_fall_through_to_other_modes_of_determining_endpoint_type()
        {
            var endpointTypeDefinedInConfigurationFile = typeof(ConfigWithCustomTransport);
            var endpointTypeDeterminer = new EndpointTypeDeterminer(new AssemblyScanner(),
                () => ConfigurationManager.AppSettings["EndpointConfigurationType"]);
            var hostArguments = new HostArguments(new string[0]);

            // will match with config-based type
            var RetrievedEndpointType = endpointTypeDeterminer.GetEndpointConfigurationTypeForHostedEndpoint(hostArguments).Type;

            Assert.AreEqual(endpointTypeDefinedInConfigurationFile, RetrievedEndpointType);
        }

        [Test]
        public void when_endpoint_type_is_provided_via_hostArgs_it_should_have_first_priority()
        {
            var endpointTypeDeterminer = new EndpointTypeDeterminer(new AssemblyScanner(),
                () => ConfigurationManager.AppSettings["EndpointConfigurationType"]);
            var hostArguments = new HostArguments(new string[0])
                                          {
                                              EndpointConfigurationType = typeof(TestEndpointType).AssemblyQualifiedName
                                          };

            var RetrievedEndpointType = endpointTypeDeterminer.GetEndpointConfigurationTypeForHostedEndpoint(hostArguments).Type;

            Assert.AreEqual(typeof(TestEndpointType), RetrievedEndpointType);
        }

        [Test]
        public void when_invalid_endpoint_type_is_provided_via_hostArgs_it_should_blow_up()
        {
            var endpointTypeDeterminer = new EndpointTypeDeterminer(new AssemblyScanner(),
                () => ConfigurationManager.AppSettings["EndpointConfigurationType"]);
            var hostArguments = new HostArguments(new string[0])
                                          {
                                              EndpointConfigurationType = "I am an invalid type name"
                                          };

            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                // ReSharper disable once UnusedVariable
                var type = endpointTypeDeterminer.GetEndpointConfigurationTypeForHostedEndpoint(hostArguments).Type;
            });
        }
    }

    [TestFixture]
    class GetEndpointConfigurationType_Tests
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
            var EndpointTypeDefinedInConfigurationFile = typeof(ConfigWithCustomTransport);
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
    }
}