namespace NServiceBus
{
    /// <summary>
    /// Indicates this endpoint is a client.
    /// As such will be set up as a non-transactional endpoint with no impersonation and purging messages on startup.
    /// </summary>

    [ObsoleteEx(
    TreatAsErrorFromVersion = "8.0",
    Message = "The AsA_Server, and AsA_Publisher roles are obsoleted. Manually configure the EndpointConfiguration object via IConfigureThisEndpoint.Customize(EndpointConfiguration endpointConfiguration)",
    RemoveInVersion = "9.0")]
    public interface AsA_Client {}
}
