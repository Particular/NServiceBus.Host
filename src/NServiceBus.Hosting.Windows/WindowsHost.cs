namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Win32;

    /// <summary>
    /// A windows implementation of the NServiceBus hosting solution
    /// </summary>
    public class WindowsHost : MarshalByRefObject
    {
        NServiceBus.GenericHost genericHost;

        /// <summary>
        /// Accepts the type which will specify the users custom configuration.
        /// This type should implement <see cref="IConfigureThisEndpoint"/>.
        /// </summary>
        public WindowsHost(Type endpointType, string[] args, string endpointName, IEnumerable<string> scannableAssembliesFullName)
        {
            var specifier = (IConfigureThisEndpoint)Activator.CreateInstance(endpointType);

            genericHost = new NServiceBus.GenericHost(specifier, args, new List<Type> { typeof(Production) }, endpointName, scannableAssembliesFullName);
            genericHost.TimeAllowedToStopInSeconds = GetWaitToKillServiceTimeout() / 1000;
        }

        /// <summary>
        /// Tries to read registry key where time allowed to shut down Windows Service is stored.
        /// https://technet.microsoft.com/en-us/library/cc976045.aspx
        /// </summary>
        /// <returns></returns>
        private int GetWaitToKillServiceTimeout()
        {
            const int defaultStatus = 30000;

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\"))
                {
                    int valueInt;
                    var value = (key != null ? key.GetValue("WaitToKillServiceTimeout", defaultStatus) : defaultStatus);

                    return (value is int ? (int)value : int.TryParse(value.ToString(), out valueInt) ? valueInt : defaultStatus);
                }
            }
            catch
            {
                return defaultStatus;
            }
        }

        /// <summary>
        /// Does startup work.
        /// </summary>
        public void Start()
        {
            genericHost.Start();
        }

        /// <summary>
        /// Does shutdown work.
        /// </summary>
        public void Stop()
        {
            genericHost.Stop();
        }

    }
}