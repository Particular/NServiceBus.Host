namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A windows implementation of the NServiceBus hosting solution
    /// </summary>
    public class InstallWindowsHost : MarshalByRefObject
    {
        GenericHost genericHost;

        /// <summary>
        /// Accepts the type which will specify the users custom configuration.
        /// This type should implement <see cref="IStartThisEndpoint"/>.
        /// </summary>
        public InstallWindowsHost(Type endpointType, string[] args, string endpointName, IEnumerable<string> scannableAssembliesFullName)
        {
            var specifier = (IStartThisEndpoint)Activator.CreateInstance(endpointType);

            genericHost = new GenericHost(specifier, args, new List<Type> { typeof(Production) }, endpointName, scannableAssembliesFullName);

        }

        /// <summary>
        /// Performs installations
        /// </summary>
        /// <param name="username">Username passed in to host.</param>
        public void Install(string username)
        {
            genericHost.Install(username).GetAwaiter().GetResult();
        }
    }
}