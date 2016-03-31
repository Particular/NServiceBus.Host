namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Moq;
    using NUnit.Framework;

    public class StartableAndStoppableRunnerTimeoutTests
    {
        static TimeSpan TimeToWarn = TimeSpan.FromSeconds(1);
        static TimeSpan TimeToWaitForTimeout = TimeToWarn.Add(TimeSpan.FromSeconds(2));

        [Test]
        public async Task Should_log_warning_on_long_start()
        {
            var mockLogger = await RunSlowStartableTest();

            mockLogger.Verify(m => m.Warn(It.IsRegex(".+The endpoint will not start until this operation has completed.+")));
        }

        [Test]
        public async Task Should_log_warning_on_long_stop()
        {
            var mockLogger = await RunSlowStartableTest();

            mockLogger.Verify(m => m.Warn(It.IsRegex(".+The endpoint will not shut down.+")));
        }

        static async Task<Mock<ILog>> RunSlowStartableTest()
        {
            var startable = new BlockingLongRunningStartable();
            var thingsToBeStarted = new IWantToRunWhenEndpointStartsAndStops[]
            {
                startable
            };

            var mockLogger = new Mock<ILog>();

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);

            runner.LongRunningWarningTimeSpan = TimeToWarn;
            StartableAndStoppableRunner.Log = mockLogger.Object;

            await runner.Start(null);
            await runner.Stop(null);

            return mockLogger;
        }

        class BlockingLongRunningStartable : IWantToRunWhenEndpointStartsAndStops
        {
            public async Task Start(IMessageSession session)
            {
                await Task.Delay(TimeToWaitForTimeout);
            }

            public async Task Stop(IMessageSession session)
            {
                await Task.Delay(TimeToWaitForTimeout);
            }
        }
    }
}