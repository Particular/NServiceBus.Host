namespace NServiceBus
{
    /// <summary>
    /// Indicates this endpoint is a server.
    /// As such will be set up as a transactional endpoint using impersonation, not purging messages on startup.
    /// </summary>

    [ObsoleteEx(
        TreatAsErrorFromVersion = "7.1",
        Message = "Roles are obsoleted and should not be used. The functionality of AsA_Server, and AsA_Publisher has been made defaults in the core, see: https://docs.particular.net/nservicebus/hosting/nservicebus-host/?version=host_6#roles-built-in-configurations",
        RemoveInVersion = "8.0")]
    public interface AsA_Server { }
}