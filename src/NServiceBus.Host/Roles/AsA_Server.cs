namespace NServiceBus
{
    /// <summary>
    /// Indicates this endpoint is a server.
    /// As such will be set up as a transactional endpoint using impersonation, not purging messages on startup.
    /// </summary>
    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "Roles are obsoleted please manually configure the EndpointConfiguration object via IConfigureThisEndpoint.Customize(EndpointConfiguration endpointConfiguration). See upgrade guide for details",
        RemoveInVersion = "9.0")]
    public interface AsA_Server { }
}