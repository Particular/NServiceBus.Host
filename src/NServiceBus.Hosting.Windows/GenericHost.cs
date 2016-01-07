namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Hosting.Helpers;
    using Hosting.Profiles;
    using Logging;
    using NServiceBus.Configuration.AdvanceExtensibility;


    class GenericHost
    {
        public GenericHost(IConfigureThisEndpoint specifier, string[] args, List<Type> defaultProfiles, string endpointName, IEnumerable<string> scannableAssembliesFullName = null)
        {
            this.specifier = specifier;
         
            if (string.IsNullOrEmpty(endpointName))
            {
                endpointName = specifier.GetType().Namespace ?? specifier.GetType().Assembly.GetName().Name;
            }

            endpointNameToUse = endpointName;
            List<Assembly> assembliesToScan;

            if (scannableAssembliesFullName == null || !scannableAssembliesFullName.Any())
            {
                var assemblyScanner = new AssemblyScanner();
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
        }

        /// <summary>
        ///     Creates and starts the bus as per the configuration
        /// </summary>
        public void Start()
        {
            try
            {
                var startableEndpoint = PerformConfiguration().Result;
                bus =  startableEndpoint.Start().Result;
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
            bus?.Stop().GetAwaiter().GetResult();
            bus = null;
        }

        /// <summary>
        ///     When installing as windows service (/install), run infrastructure installers
        /// </summary>
        public void Install(string username)
        {
            PerformConfiguration(builder => builder.EnableInstallers(username))
                .Dispose();
        }

        Task<IStartableEndpoint> PerformConfiguration(Action<BusConfiguration> moreConfiguration = null)
        {
            var loggingConfigurers = profileManager.GetLoggingConfigurer();
            foreach (var loggingConfigurer in loggingConfigurers)
            {
                loggingConfigurer.Configure(specifier);
            }

            var configuration = new BusConfiguration();
            SetSlaFromAttribute(configuration, specifier);
            configuration.EndpointName(endpointNameToUse);
            configuration.DefineCriticalErrorAction(OnCriticalError);

            moreConfiguration?.Invoke(configuration);

            specifier.Customize(configuration);
            RoleManager.TweakConfigurationBuilder(specifier, configuration);
            profileManager.ActivateProfileHandlers(configuration);

            return Endpoint.Create(configuration);
        }

        void SetSlaFromAttribute(BusConfiguration configuration, IConfigureThisEndpoint configureThisEndpoint)
        {
            var endpointConfigurationType = configureThisEndpoint
                .GetType();
            TimeSpan sla;
            if (TryGetSlaFromEndpointConfigType(endpointConfigurationType, out sla))
            {
                configuration.GetSettings().Set("EndpointSLA", sla);
            }
        }

        internal static bool TryGetSlaFromEndpointConfigType(Type endpointConfigurationType, out TimeSpan sla)
        {
            var hostSLAAttribute = (EndpointSLAAttribute) endpointConfigurationType
                .GetCustomAttributes(typeof(EndpointSLAAttribute), false)
                .FirstOrDefault();
            if (hostSLAAttribute != null)
            {
                sla = hostSLAAttribute.SLA;
                return true;
            }
            sla = TimeSpan.Zero;
            return false;
        }

        // Windows hosting behavior when critical error occurs is suicide.
        async Task OnCriticalError(ICriticalErrorContext endpoint)
        {
            if (Environment.UserInteractive)
            {
                await Task.Delay(10000).ConfigureAwait(false); // so that user can see on their screen the problem
            }
            
            Environment.FailFast($"The following critical error was encountered by NServiceBus:NServiceBus is shutting down.");
        }

        ProfileManager profileManager;
        IConfigureThisEndpoint specifier;
        IEndpointInstance bus;
        string endpointNameToUse;
    }
}