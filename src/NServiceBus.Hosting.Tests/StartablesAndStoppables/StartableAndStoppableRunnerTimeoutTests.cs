namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Moq;
    using NUnit.Framework;

    public class StartableAndStoppableRunnerTimeoutTests
    {
        static TimeSpan TimeToWarn = TimeSpan.FromSeconds(3);
        static TimeSpan LongRunningTimespan = TimeToWarn.Add(TimeSpan.FromSeconds(2));
        static TimeSpan ShortRunningTimespan = TimeToWarn.Subtract(TimeSpan.FromSeconds(2));
        const string startupWarning = ".+The endpoint will not start until this operation has completed.+";
        const string shutdownWarning = ".+The endpoint will not shut down.+";

        [Test]
        public async Task Should_log_warning_on_long_start()
        {
            var mockLogger = await RunStartableWarningTest(new LongRunningStartable());

            mockLogger.Verify(m => m.Warn(It.IsRegex(startupWarning)));
        }

        [Test]
        public async Task Should_log_warning_on_long_stop()
        {
            var mockLogger = await RunStartableWarningTest(new LongRunningStartable());

            mockLogger.Verify(m => m.Warn(It.IsRegex(shutdownWarning)));
        }
        [Test]
        public async Task Should_not_log_warning_on_quick_start()
        {
            var mockLogger = await RunStartableWarningTest(new QuickRunningStartable());

            mockLogger.Verify(m => m.Warn(It.IsRegex(startupWarning)), Times.Never);
        }

        [Test]
        public async Task Should_not_log_warning_on_quick_stop()
        {
            var mockLogger = await RunStartableWarningTest(new QuickRunningStartable());

            mockLogger.Verify(m => m.Warn(It.IsRegex(shutdownWarning)), Times.Never);
        }

        static async Task<Mock<ILog>> RunStartableWarningTest(IWantToRunWhenEndpointStartsAndStops instanceToTest)
        {
            var thingsToBeStarted = new[] { instanceToTest };

            var mockLogger = new Mock<ILog>();

            var runner = new StartableAndStoppableRunner(thingsToBeStarted);

            runner.LongRunningWarningTimeSpan = TimeToWarn;
            StartableAndStoppableRunner.Log = mockLogger.Object;

            await runner.Start(null);
            await runner.Stop(null);

            return mockLogger;
        }

        class LongRunningStartable : IWantToRunWhenEndpointStartsAndStops
        {
            public async Task Start(IMessageSession session)
            {
                await Task.Delay(LongRunningTimespan);
            }

            public async Task Stop(IMessageSession session)
            {
                await Task.Delay(LongRunningTimespan);
            }
        }

        class QuickRunningStartable : IWantToRunWhenEndpointStartsAndStops
        {
            public async Task Start(IMessageSession session)
            {
                await Task.Delay(ShortRunningTimespan);
            }

            public async Task Stop(IMessageSession session)
            {
                await Task.Delay(ShortRunningTimespan);
            }
        }
    }
}