namespace NServiceBus.Hosting.Windows
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NServiceBus.Logging;
    using NServiceBus.ObjectBuilder;

    class LifecycleExtensions : IWantToRunBeforeConfigurationIsFinalized
    {
        public void Run(Configure config)
        {
            builder = config.Builder;

            var configure = builder.Build<IConfigureComponents>();
            foreach (var type in config.TypesToScan
                .Where(t => typeof(IWantToRunWhenBusStartsAndStops).IsAssignableFrom(t) && !(t.IsAbstract || t.IsInterface)))
            {
                configure.ConfigureComponent(type, DependencyLifecycle.InstancePerCall);
            }
        }

        public static void StartExtensions(Action<string, Exception> onError)
        {
            ProcessStartupItems(
                builder.BuildAll<IWantToRunWhenBusStartsAndStops>().ToList(),
                toRun =>
                {
                    toRun.Start();
                    thingsRanAtStartup.Add(toRun);
                    Log.DebugFormat("Started {0}.", toRun.GetType().AssemblyQualifiedName);
                },
                ex =>
                {
                    Log.Fatal("Startup task failed to complete.", ex);
                    onError("Startup task failed to complete.", ex);
                },
                startCompletedEvent);
        }

        public static void StopExtensions()
        {
            // Ensuring IWantToRunWhenBusStartsAndStops.Start has been called.
            startCompletedEvent.WaitOne();

            var tasksToStop = Interlocked.Exchange(ref thingsRanAtStartup, new ConcurrentBag<IWantToRunWhenBusStartsAndStops>());
            if (!tasksToStop.Any())
            {
                return;
            }

            ProcessStartupItems(
                tasksToStop,
                toRun =>
                {
                    toRun.Stop();
                    Log.DebugFormat("Stopped {0}.", toRun.GetType().AssemblyQualifiedName);
                },
                ex => Log.Fatal("Startup task failed to stop.", ex),
                stopCompletedEvent);

            stopCompletedEvent.WaitOne();
        }

        static void ProcessStartupItems<T>(IEnumerable<T> items, Action<T> iteration, Action<Exception> inCaseOfFault, EventWaitHandle eventToSet)
        {
            eventToSet.Reset();

            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(items, iteration);
                eventToSet.Set();
            }, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness)
            .ContinueWith(task =>
            {
                eventToSet.Set();
                inCaseOfFault(task.Exception.Flatten());
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.LongRunning);
        }

        static IBuilder builder;
        static ConcurrentBag<IWantToRunWhenBusStartsAndStops> thingsRanAtStartup = new ConcurrentBag<IWantToRunWhenBusStartsAndStops>();
        static ManualResetEvent startCompletedEvent = new ManualResetEvent(false);
        static ManualResetEvent stopCompletedEvent = new ManualResetEvent(true);
        static ILog Log = LogManager.GetLogger<LifecycleExtensions>();
    }
}