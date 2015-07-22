using System;
using NServiceBus;
using NUnit.Framework;

[TestFixture]
class EndpointSlaTests
{
    [EndpointSLA("2:00:00")]
    class ClassWithHostSlaAttribute
    {

    }

    [Test]
    public void With_sla_attribute()
    {
        TimeSpan sla;
        GenericHost.TryGetSlaFromEndpointConfigType(typeof(ClassWithHostSlaAttribute), out sla);
        Assert.AreEqual(TimeSpan.FromHours(2), sla);
    }
}