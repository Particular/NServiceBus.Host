namespace NServiceBus
{
    using Transport;

    /// <summary>
    /// Role used to specify the desired transport to use
    /// </summary>
    /// <typeparam name="T">The <see cref="TransportDefinition"/> to use.</typeparam>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "Roles are obsoleted please manually configure the EndpointConfiguration object via IConfigureThisEndpoint.Customize(EndpointConfiguration endpointConfiguration). See upgrade guide for details",
        RemoveInVersion = "9.0")]
    public interface UsingTransport<T> where T : TransportDefinition
    {
    }
}