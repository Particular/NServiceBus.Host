namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Hosting.Helpers;
    using Hosting.Profiles;
    using Hosting.Wcf;
    using Logging;
    using NServiceBus.Unicast;

    class GenericHost
    {
        public GenericHost(IConfigureThisEndpoint specifier, string[] args, List<Type> defaultProfiles, string endpointName, IEnumerable<string> scannableAssembliesFullName = null)
        {
            this.specifier = specifier;

            if (String.IsNullOrEmpty(endpointName))
            {
                endpointName = specifier.GetType().Namespace ?? specifier.GetType().Assembly.GetName().Name;
            }


            endpointNameToUse = endpointName;
            endpointVersionToUse = FileVersionRetriever.GetFileVersion(specifier.GetType());

            if (scannableAssembliesFullName == null || !scannableAssembliesFullName.Any())
            {
                var assemblyScanner = new AssemblyScanner();
                assemblyScanner.MustReferenceAtLeastOneAssembly.Add(typeof(IHandleMessages<>).Assembly);
                assembliesToScan = assemblyScanner
                    .GetScannableAssemblies()
                    .Assemblies;
            }
            else
            {
                assembliesToScan = scannableAssembliesFullName
                    .Select(Assembly.Load)
                    .ToList();
            }

            profileManager = new ProfileManager(assembliesToScan, args, defaultProfiles);

            wcfManager = new WcfManager();
        }

        /// <summary>
        ///     Creates and starts the bus as per the configuration
        /// </summary>
        public void Start()
        {
            try
            {
                PerformConfiguration();

                if (bus != null && !bus.Settings.Get<bool>("Endpoint.SendOnly"))
                {
                    bus.Start();
                }

                wcfManager.Startup(bus);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger<GenericHost>().Fatal("Exception when starting endpoint.", ex);
                throw;
            }
        }

        /// <summary>
        ///     Finalize
        /// </summary>
        public void Stop()
        {
            var disposeTask = Task.Factory.StartNew(() =>
            {
                wcfManager.Shutdown();

                if (bus != null)
                {
                    bus.Dispose();

                    bus = null;
                }
            });

            StartShutdownReporter();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(AllowedTimeToShutdown));

            var finishedResult = Task.WhenAny(disposeTask, timeoutTask).GetAwaiter().GetResult();

            if (finishedResult.Equals(timeoutTask))
            {
                LogManager.GetLogger<GenericHost>().Error("The host failed to stop with-in the time allowed (30s).");
            }
        }

        private void StartShutdownReporter()
        {
            const int interval = 10000;
            var timeSpent = 0;

            var timer = new System.Timers.Timer();
            timer.Elapsed += (sender, args) =>
            {
                timeSpent += interval / 1000;
                LogManager.GetLogger<GenericHost>().InfoFormat("Trying to shut down host for {0} seconds.", timeSpent);
            };
            timer.Interval = interval;
            timer.Enabled = true;
        }

        /// <summary>
        ///     When installing as windows service (/install), run infrastructure installers
        /// </summary>
        public void Install(string username)
        {
            PerformConfiguration(builder => builder.EnableInstallers(username));

            bus.Dispose();
        }

        void PerformConfiguration(Action<BusConfiguration> moreConfiguration = null)
        {
            var loggingConfigurers = profileManager.GetLoggingConfigurer();
            foreach (var loggingConfigurer in loggingConfigurers)
            {
                loggingConfigurer.Configure(specifier);
            }

            var configuration = new BusConfiguration();

            configuration.EndpointName(endpointNameToUse);
            configuration.EndpointVersion(endpointVersionToUse);
            configuration.AssembliesToScan(assembliesToScan);
            configuration.DefineCriticalErrorAction(OnCriticalError);

            if (moreConfiguration != null)
            {
                moreConfiguration(configuration);
            }

            specifier.Customize(configuration);
            RoleManager.TweakConfigurationBuilder(specifier, configuration);
            profileManager.ActivateProfileHandlers(configuration);

            bus = (UnicastBus)Bus.Create(configuration);
        }

        // Windows hosting behavior when critical error occurs is suicide.
        void OnCriticalError(string errorMessage, Exception exception)
        {
            if (Environment.UserInteractive)
            {
                Thread.Sleep(10000); // so that user can see on their screen the problem
            }

            Environment.FailFast(String.Format("The following critical error was encountered by NServiceBus:\n{0}\nNServiceBus is shutting down.", errorMessage), exception);
        }

        protected virtual int AllowedTimeToShutdown { get; set; }

        List<Assembly> assembliesToScan;
        ProfileManager profileManager;
        IConfigureThisEndpoint specifier;
        WcfManager wcfManager;
        UnicastBus bus;
        string endpointNameToUse;
        string endpointVersionToUse;
    }
}