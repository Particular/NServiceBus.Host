namespace NServiceBus
{
    [ObsoleteEx(Message = "The windows host will be deprecated in the next major version. See upgrade guide and doco for alternatives.",
        RemoveInVersion = "10.0")]
#pragma warning disable 1591
    public interface IConfigureThisEndpoint
    {

        void Customize(EndpointConfiguration configuration);
    }
#pragma warning restore 1591
}
