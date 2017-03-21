namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    /// A windows implementation of the NServiceBus hosting solution
    /// </summary>
    public class WindowsHost : MarshalByRefObject
    {
        ILog Log = LogManager.GetLogger<WindowsHost>();
        NServiceBus.GenericHost genericHost;

        /// <summary>
        /// Accepts the type which will specify the users custom configuration.
        /// This type should implement <see cref="IConfigureThisEndpoint"/>.
        /// </summary>
        public WindowsHost(Type endpointType, string[] args, string endpointName, IEnumerable<string> scannableAssembliesFullName)
        {
            var specifier = (IConfigureThisEndpoint)Activator.CreateInstance(endpointType);

            genericHost = new NServiceBus.GenericHost(specifier, args, new List<Type> { typeof(Production) }, endpointName, scannableAssembliesFullName);
        }

        /// <summary>
        /// Does startup work.
        /// </summary>
        public void Start()
        {
            try
            {
                genericHost.Start();
            }
            catch (Exception ex)
            {
                Log.Fatal("Start failure", ex);
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Does shutdown work.
        /// </summary>
        public void Stop()
        {
            try
            {
                genericHost.Stop();
            }
            catch (Exception ex)
            {
                Log.Fatal("Stop failure", ex);
                Environment.Exit(-2);
            }
        }

    }
}