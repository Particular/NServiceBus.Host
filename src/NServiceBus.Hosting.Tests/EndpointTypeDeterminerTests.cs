namespace EndpointTypeDeterminerTests
{
    using System.Configuration;
    using NServiceBus.Hosting.Helpers;
    using NServiceBus.Hosting.Tests.EndpointTypeTests;
    using NServiceBus.Hosting.Windows;
    using NServiceBus.Hosting.Windows.Arguments;
    using NUnit.Framework;

    [TestFixture]
    class GetEndpointConfigurationTypeForHostedEndpointTests
    {

        [Test]
        public void when_endpoint_type_is_not_provided_via_hostArgs_it_should_fall_through_to_other_modes_of_determining_endpoint_type()
        {
            var endpointTypeDefinedInConfigurationFile = typeof(MyEndpointConfig);
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
}