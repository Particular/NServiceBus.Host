using NServiceBus.Hosting.Windows;
using NUnit.Framework;
using Particular.Approvals;
using PublicApiGenerator;

[TestFixture]
public class APIApprovals
{
    [Test]
    public void Approve()
    {
        var publicApi = ApiGenerator.GeneratePublicApi(typeof(Program).Assembly);
        Approver.Verify(publicApi);
    }
}