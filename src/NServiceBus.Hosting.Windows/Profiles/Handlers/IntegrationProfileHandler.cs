namespace NServiceBus.Hosting.Windows.Profiles.Handlers
{
    
    using Hosting.Profiles;

    class IntegrationProfileHandler : IHandleProfile<Integration>
    {
        public void ProfileActivated(BusConfiguration config)
        {
            config.EnableInstallers();
        }
    }
}