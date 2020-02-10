namespace NServiceBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hosting;
    using Logging;

    class StartableAndStoppableRunner
    {
        public StartableAndStoppableRunner(IEnumerable<IWantToRunWhenEndpointStartsAndStops> wantToRunWhenBusStartsAndStops)
        {
            this.wantToRunWhenBusStartsAndStops = wantToRunWhenBusStartsAndStops;
        }

        public Task Start(IMessageSession session)
        {
            var startableTasks = new List<Task>();
            foreach (var startable in wantToRunWhenBusStartsAndStops)
            {
                var startable1 = startable;
                var startableName = startable1.GetType().AssemblyQualifiedName;

                var task = startable1.Start(session).ThrowIfNull();

                LongRunningWarning(task,
                    $"The start method for {startableName} is taking a long time to complete. The endpoint will not start until this operation has completed.");

                /* 
                    We can't use the await keyword because of the conditional logging. 
                    Since we want to start them concurrently and log per instance there is not much else we can do.
                */
                task.ContinueWith(t =>
                {
                    thingsRanAtStartup.Add(startable1);
                    Log.DebugFormat("Started {0}.", startableName);
                }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously).Ignore();
                task.ContinueWith(t =>
                {
                    Log.Error($"Startup task {startableName} failed to complete.", t.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously).Ignore();

                startableTasks.Add(task);
            }

            return Task.WhenAll(startableTasks.ToArray());
        }

        public async Task Stop(IMessageSession session)
        {
            var stoppables = Interlocked.Exchange(ref thingsRanAtStartup, new ConcurrentBag<IWantToRunWhenEndpointStartsAndStops>());
            if (!stoppables.Any())
            {
                return;
            }

            var stoppableTasks = new List<Task>();
            foreach (var stoppable in stoppables)
            {
                try
                {
                    var stoppable1 = stoppable;
                    var stoppableName = stoppable1.GetType().AssemblyQualifiedName;

                    var task = stoppable.Stop(session).ThrowIfNull();

                    LongRunningWarning(task,
                        $"The stop method for {stoppableName} is taking a long time to complete. The endpoint will not shut down until this operation has completed.");

                    /* 
                        We can't use the await keyword because of the conditional logging. 
                        Since we want to start them concurrently and log per instance there is not much else we can do.
                    */
                    task.ContinueWith(t =>
                    {
                        thingsRanAtStartup.Add(stoppable1);
                        Log.DebugFormat("Stopped {0}.", stoppableName);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously).Ignore();
                    task.ContinueWith(t =>
                    {
                        Log.Fatal($"Startup task {stoppableName} failed to stop.", t.Exception);
                        t?.Exception?.Flatten().Handle(e => true);
                    }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously).Ignore();

                    stoppableTasks.Add(task);
                }
                catch (Exception e)
                {
                    Log.Fatal("Startup task failed to stop.", e);
                }
            }

            try
            {
                await Task.WhenAll(stoppableTasks.ToArray()).ConfigureAwait(false);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // ignore because we want to shutdown no matter what.
            }
        }

        void LongRunningWarning(Task taskToWatch, string message)
        {
            var delayTokenSource = new CancellationTokenSource();

            var warningWaitTimeTask = Task.Delay(LongRunningWarningTimeSpan, delayTokenSource.Token);
            Task.WhenAny(taskToWatch, warningWaitTimeTask)
                .ContinueWith(t =>
                {
                    delayTokenSource.Cancel();
                    delayTokenSource.Dispose();

                    if (t.Result == warningWaitTimeTask)
                    {
                        Log.Warn(message);
                    }
                }).Ignore();
        }

        IEnumerable<IWantToRunWhenEndpointStartsAndStops> wantToRunWhenBusStartsAndStops;
        ConcurrentBag<IWantToRunWhenEndpointStartsAndStops> thingsRanAtStartup = new ConcurrentBag<IWantToRunWhenEndpointStartsAndStops>();
        public static ILog Log = LogManager.GetLogger<StartableAndStoppableRunner>();
        public TimeSpan LongRunningWarningTimeSpan = TimeSpan.FromMinutes(2);
    }
}
