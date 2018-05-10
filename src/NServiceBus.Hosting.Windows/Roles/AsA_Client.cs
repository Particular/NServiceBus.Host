namespace NServiceBus
{
    /// <summary>
    /// Indicates this endpoint is a client.
    /// As such will be set up as a non-transactional endpoint with no impersonation and purging messages on startup.
    /// </summary>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "Roles are obsoleted please manually configure the EndpointConfiguration object via IConfigureThisEndpoint.Customize(EndpointConfiguration endpointConfiguration). See upgrade guide for details",
        RemoveInVersion = "9.0")]
    public interface AsA_Client {}
}