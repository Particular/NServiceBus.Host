namespace NServiceBus.Hosting.Windows.Profiles.Handlers
{
    using Hosting.Profiles;

    class LiteProfileHandler : IHandleProfile<Lite>
    {
        public void ProfileActivated(EndpointConfiguration config)
        {
            config.EnableInstallers();
            //if (!config.Configurer.HasComponent<IManageMessageFailures>()) //TODO: Not sure how to handle this yet
            //{
            // config.DiscardFailedMessagesInsteadOfSendingToErrorQueue();
            //}
        }
    }
}