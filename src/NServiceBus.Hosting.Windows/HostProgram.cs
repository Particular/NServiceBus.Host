namespace NServiceBus.Hosting.Windows
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using Arguments;
    using Installers;
    using Magnum.StateMachine;
    using Helpers;
    using Topshelf;
    using Topshelf.Configuration;

    /// <summary>
    /// Entry point to the process.
    /// </summary>
    public class Program
    {
        static AssemblyScannerResults assemblyScannerResults;

        static void Main(string[] args)
        {
            var arguments = new HostArguments(args);

            if (arguments.Help)
            {
                arguments.PrintUsage();
                return;
            }

            var assemblyScanner = new AssemblyScanner
            {
                ThrowExceptions = false
            };

            var endpointTypeDeterminer = new EndpointTypeDeterminer(assemblyScanner, () => ConfigurationManager.AppSettings["EndpointConfigurationType"]);
            var endpointConfigurationType = endpointTypeDeterminer.GetEndpointConfigurationTypeForHostedEndpoint(arguments);
            assemblyScannerResults = endpointTypeDeterminer.AssemblyScannerResults;

            var endpointConfigurationFile = endpointConfigurationType.EndpointConfigurationFile;
            var endpointName = endpointConfigurationType.EndpointName;
            var serviceName = endpointConfigurationType.ServiceName;
            var endpointVersion = endpointConfigurationType.EndpointVersion;
            var displayName = serviceName + "-" + endpointVersion;

            if (arguments.SideBySide)
            {
                serviceName += "-" + endpointVersion;
            }

            //Add the endpoint name so that the new appDomain can get it
            if (arguments.EndpointName == null && !string.IsNullOrEmpty(endpointName))
            {
                args = args.Concat(new[] {$"/endpointName={endpointName}"}).ToArray();
            }

            //Add the ScannedAssemblies name so that the new appDomain can get it
            if (arguments.ScannedAssemblies.Count == 0)
            {
                args = assemblyScannerResults.Assemblies.Select(s => s.ToString()).Aggregate(args, (current, result) => current.Concat(new[] {$"/scannedAssemblies={result}"}).ToArray());
            }

            //Add the endpointConfigurationType name so that the new appDomain can get it
            if (arguments.EndpointConfigurationType == null)
            {
                args = args.Concat(new[] {$"/endpointConfigurationType={endpointConfigurationType.AssemblyQualifiedName}"}).ToArray();
            }

            if (arguments.Install)
            {
                WindowsInstaller.Install(args, endpointConfigurationFile);
            }

            var cfg = RunnerConfigurator.New(x =>
            {
                x.ConfigureServiceInIsolation<WindowsHost>(endpointConfigurationType.AssemblyQualifiedName, c =>
                {
                    c.ConfigurationFile(endpointConfigurationFile);
                    c.WhenStarted(service => service.Start());
                    c.WhenStopped(service => service.Stop());
                    c.CommandLineArguments(args, () => SetHostServiceLocatorArgs);
                    c.CreateServiceLocator(() => new HostServiceLocator());
                });

                if (arguments.Username != null)
                {
                    x.RunAs(arguments.Username, arguments.Password);
                }
                else
                {
                    x.RunAsLocalSystem();
                }

                if (arguments.StartManually)
                {
                    x.DoNotStartAutomatically();
                }

                x.SetDisplayName(arguments.DisplayName ?? displayName);
                x.SetServiceName(serviceName);
                x.SetDescription(arguments.Description ?? $"NServiceBus Endpoint Host Service for {displayName}");

                var serviceCommandLine = new List<string>();

                if (!string.IsNullOrEmpty(arguments.EndpointConfigurationType))
                {
                    serviceCommandLine.Add($@"/endpointConfigurationType:""{arguments.EndpointConfigurationType}""");
                }

                if (!string.IsNullOrEmpty(endpointName))
                {
                    serviceCommandLine.Add($@"/endpointName:""{endpointName}""");
                }

                if (!string.IsNullOrEmpty(serviceName))
                {
                    serviceCommandLine.Add($@"/serviceName:""{serviceName}""");
                }

                if (!assemblyScannerResults.ErrorsThrownDuringScanning && arguments.ScannedAssemblies.Count > 0)
                {
                    serviceCommandLine.AddRange(arguments.ScannedAssemblies.Select(assembly => $@"/scannedAssemblies:""{assembly}"""));
                }

                if (arguments.OtherArgs.Any())
                {
                    serviceCommandLine.AddRange(arguments.OtherArgs);
                }

                var commandLine = string.Join(" ", serviceCommandLine);
                x.SetServiceCommandLine(commandLine);

                if (arguments.DependsOn != null)
                {
                    foreach (var dependency in arguments.DependsOn)
                    {
                        x.DependsOn(dependency);
                    }
                }
            });

            try
            {

                Runner.Host(cfg, args);
            }
            catch (StateMachineException exception)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException)
                    .Throw();
            }
        }

        static void SetHostServiceLocatorArgs(string[] args)
        {
            HostServiceLocator.Args = args;
        }
    }
}
