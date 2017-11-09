using System.Runtime.CompilerServices;
using NServiceBus.Hosting.Tests;
using NServiceBus.Hosting.Windows;
using NUnit.Framework;
using PublicApiGenerator;

[TestFixture]
public class APIApprovals
{
    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Approve()
    {
        var publicApi = ApiGenerator.GeneratePublicApi(typeof(Program).Assembly);
        TestApprover.Verify(publicApi);
    }
}