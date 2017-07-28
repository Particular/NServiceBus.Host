namespace NServiceBus
{
    /// <summary>
    /// Indicates this endpoint is a server.
    /// As such will be set up as a transactional endpoint using impersonation, not purging messages on startup.
    /// </summary>

    [ObsoleteEx(
        TreatAsErrorFromVersion = "8.0",
        Message = "The AsA_Server, and AsA_Publisher roles are obsoleted. Manually configure the EndpointConfiguration object via IConfigureThisEndpoint.Customize(EndpointConfiguration endpointConfiguration)",
        RemoveInVersion = "9.0")]
    public interface AsA_Server { }
}
