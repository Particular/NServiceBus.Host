namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class StartableAndStoppableRunnerTests
    {
        [Test]
        public async Task Should_start_all_startables()
        {
            var startable1 = new Startable1();
            var startable2 = new Startable2();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, startable2 };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);

            await runner.Start(null);

            Assert.True(startable1.Started);
            Assert.True(startable2.Started);
        }

        [Test]
        public void Should_throw_if_startable_fails_synchronously()
        {
            var startable1 = new Startable1();
            var syncThrowable = new SyncThrowingStart();
            var startable2 = new Startable2();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, syncThrowable, startable2 };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);

            Assert.That(async () => await runner.Start(null), Throws.InvalidOperationException);

            Assert.True(startable1.Started);
            Assert.False(startable2.Started);
        }

        [Test]
        public void Should_throw_if_startable_fails_asynchronously()
        {
            var startable1 = new Startable1();
            var asyncThrowable = new AsyncThrowingStart();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, asyncThrowable };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);

            Assert.That(async () => await runner.Start(null), Throws.InvalidOperationException);

            Assert.True(startable1.Started);
        }

        [Test]
        public async Task Should_stop_all_stoppables()
        {
            var startable1 = new Startable1();
            var startable2 = new Startable2();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, startable2 };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);
            await runner.Start(null);

            await runner.Stop(null);

            Assert.True(startable1.Stopped);
            Assert.True(startable2.Stopped);
        }

        [Test]
        public async Task Should_stop_only_successfully_started()
        {
            var startable1 = new Startable1();
            var startable2 = new Startable2();
            var syncThrows = new SyncThrowingStart();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, startable2, syncThrows };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);
            try
            {
                await runner.Start(null);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (InvalidOperationException)
            {
                // ignored
            }

            await runner.Stop(null);

            Assert.True(startable1.Stopped);
            Assert.True(startable2.Stopped);
            Assert.False(syncThrows.Stopped);
        }

        [Test]
        public async Task Should_not_rethrow_sync_exceptions_when_stopped()
        {
            var startable1 = new Startable1();
            var startable2 = new Startable2();
            var syncThrows = new SyncThrowingStop();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, syncThrows, startable2 };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);
            try
            {
                await runner.Start(null);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (InvalidOperationException)
            {
                // ignored
            }

            Assert.That(async () => await runner.Stop(null), Throws.Nothing);
            Assert.True(startable1.Stopped);
            Assert.True(startable2.Stopped);
        }

        [Test]
        public async Task Should_not_rethrow_async_exceptions_when_stopped()
        {
            var startable1 = new Startable1();
            var startable2 = new Startable2();
            var asyncThrows = new AsyncThrowingStop();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable1, asyncThrows, startable2 };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);
            try
            {
                await runner.Start(null);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (InvalidOperationException)
            {
                // ignored
            }

            Assert.That(async () => await runner.Stop(null), Throws.Nothing);
            Assert.True(startable1.Stopped);
            Assert.True(startable2.Stopped);
        }

        [Test]
        public void Should_throw_friendly_exception_when_IWantToRunWhenEndpointStartsAndStops_Start_returns_null()
        {
            var startable = new StartableStartReturnsNull();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[] { startable };

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);

            Assert.That(async () => await runner.Start(null), Throws.Exception.With.Message.EqualTo("Return a Task or mark the method as async."));
        }

        class Startable1 : IWantToRunWhenEndpointStartsAndStops
        {
            public bool Started { get; set; }
            public bool Stopped { get; set; }

            public Task Start(IMessageSession session)
            {
                Started = true;
                return TaskEx.CompletedTask;
            }

            public Task Stop(IMessageSession session)
            {
                Stopped = true;
                return TaskEx.CompletedTask;
            }
        }

        class Startable2 : IWantToRunWhenEndpointStartsAndStops
        {
            public bool Started { get; set; }
            public bool Stopped { get; set; }

            public Task Start(IMessageSession session)
            {
                Started = true;
                return TaskEx.CompletedTask;
            }

            public Task Stop(IMessageSession session)
            {
                Stopped = true;
                return TaskEx.CompletedTask;
            }
        }

        class StartableStartReturnsNull : IWantToRunWhenEndpointStartsAndStops
        {
            public Task Start(IMessageSession session)
            {
                return null;
            }

            public Task Stop(IMessageSession session)
            {
                throw new NotImplementedException();
            }
        }

        class StartableStopReturnsNull : IWantToRunWhenEndpointStartsAndStops
        {
            public Task Start(IMessageSession session)
            {
                throw new NotImplementedException();
            }

            public Task Stop(IMessageSession session)
            {
                return null;
            }
        }

        class SyncThrowingStart : IWantToRunWhenEndpointStartsAndStops
        {
            public bool Stopped { get; set; }

            public Task Start(IMessageSession session)
            {
                throw new InvalidOperationException("SyncThrowingStart");
            }

            public Task Stop(IMessageSession session)
            {
                Stopped = true;
                return TaskEx.CompletedTask;
            }
        }

        class AsyncThrowingStart : IWantToRunWhenEndpointStartsAndStops
        {
            public bool Stopped { get; set; }

            public async Task Start(IMessageSession session)
            {
                await Task.Yield();
                throw new InvalidOperationException("AsyncThrowingStart");
            }

            public Task Stop(IMessageSession session)
            {
                Stopped = true;
                return TaskEx.CompletedTask;
            }
        }

        class SyncThrowingStop : IWantToRunWhenEndpointStartsAndStops
        {
            public bool Started { get; set; }

            public Task Start(IMessageSession session)
            {
                Started = true;
                return TaskEx.CompletedTask;
            }

            public Task Stop(IMessageSession session)
            {
                throw new InvalidOperationException("SyncThrowingStop");
            }
        }

        class AsyncThrowingStop : IWantToRunWhenEndpointStartsAndStops
        {
            public bool Started { get; set; }

            public Task Start(IMessageSession session)
            {
                Started = true;
                return TaskEx.CompletedTask;
            }

            public async Task Stop(IMessageSession session)
            {
                await Task.Yield();
                throw new InvalidOperationException("AsyncThrowingStop");
            }
        }
    }
}