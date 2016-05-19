namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A windows implementation of the NServiceBus hosting solution
    /// </summary>
    public class WindowsHost : MarshalByRefObject
    {
        GenericHost genericHost;

        /// <summary>
        /// Accepts the type which will specify the users custom configuration.
        /// This type should implement <see cref="IStartThisEndpoint"/>.
        /// </summary>
        public WindowsHost(Type endpointType, string[] args, string endpointName, IEnumerable<string> scannableAssembliesFullName)
        {
            var specifier = (IStartThisEndpoint)Activator.CreateInstance(endpointType);

            genericHost = new GenericHost(specifier, args, new List<Type> { typeof(Production) }, endpointName, scannableAssembliesFullName);
        }

        /// <summary>
        /// Does startup work.
        /// </summary>
        public void Start()
        {
            genericHost.Start().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Does shutdown work.
        /// </summary>
        public void Stop()
        {
            genericHost.Stop().GetAwaiter().GetResult();
        }

    }
}