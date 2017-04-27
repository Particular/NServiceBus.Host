namespace NServiceBus
{
    /// <summary>
    /// Indicates this endpoint is a client.
    /// As such will be set up as a non-transactional endpoint with no impersonation and purging messages on startup.
    /// </summary>

    [ObsoleteEx(
    TreatAsErrorFromVersion = "7.1",
    Message = "The functionality of AsA_Server, and AsA_Publisher has been made defaults in the core.",
    RemoveInVersion = "8.0")]
    public interface AsA_Client {}
}
