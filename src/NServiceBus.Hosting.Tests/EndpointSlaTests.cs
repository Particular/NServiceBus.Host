using System;
using NServiceBus;
using NUnit.Framework;

[TestFixture]
class EndpointSlaTests
{
    [EndpointSLA("1:00:00")]
    class ClassWithCoreSlaAttribute
    {

    }

    [Test]
    public void With_core_sla_attribute()
    {
        TimeSpan sla;
        GenericHost.TryGetSlaFromEndpointConfigType(typeof(ClassWithCoreSlaAttribute), out sla);
        Assert.AreEqual(TimeSpan.FromHours(1), sla);
    }

    [NServiceBus.Hosting.Windows.EndpointSLA("2:00:00")]
    class ClassWithHostSlaAttribute
    {

    }

    [Test]
    public void With_host_sla_attribute()
    {
        TimeSpan sla;
        GenericHost.TryGetSlaFromEndpointConfigType(typeof(ClassWithHostSlaAttribute), out sla);
        Assert.AreEqual(TimeSpan.FromHours(2), sla);
    }

    [NServiceBus.Hosting.Windows.EndpointSLA("2:00:00")]
    [EndpointSLA("1:00:00")]
    class ClassWithBothSlaAttribute
    {

    }

    [Test]
    public void With_both_sla_attribute_should_throw()
    {
        var exception = Assert.Throws<Exception>(() =>
        {
            TimeSpan sla;
            GenericHost.TryGetSlaFromEndpointConfigType(typeof(ClassWithBothSlaAttribute), out sla);
        });
        Assert.AreEqual("Please either define a [NServiceBus.EndpointSLAAttribute] or a [NServiceBus.Hosting.Windows.EndpointSLAAttribute], but not both.", exception.Message);
    }
}