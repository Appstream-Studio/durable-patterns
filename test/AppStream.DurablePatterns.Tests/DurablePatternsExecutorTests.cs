using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.StepsConfig;
using AppStream.DurablePatterns.StepsConfig.Entity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class DurablePatternsExecutorTests
    {
        private DurablePatternsExecutor _executor;
        private Mock<IStepExecutorFactory> _stepExecutorFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _stepExecutorFactoryMock = new Mock<IStepExecutorFactory>();
            _executor = new DurablePatternsExecutor(_stepExecutorFactoryMock.Object);
        }

        [Test]
        public void ExecuteAsync_WithEmptySteps_ThrowsArgumentException()
        {
            // Arrange
            var stepsConfigEntityId = new EntityId(nameof(StepsConfigEntity), Guid.NewGuid().ToString());
            var context = Mock.Of<IDurableOrchestrationContext>();
            var steps = new List<StepConfiguration>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _executor.ExecuteAsync(steps, stepsConfigEntityId, context));
        }

        [Test]
        public void ExecuteAsync_WithNullSteps_ThrowsArgumentNullException()
        {
            // Arrange
            var stepsConfigEntityId = new EntityId(nameof(StepsConfigEntity), Guid.NewGuid().ToString());
            var context = Mock.Of<IDurableOrchestrationContext>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _executor.ExecuteAsync(null!, stepsConfigEntityId, context));
        }

        [Test]
        public void ExecuteAsync_WithFailedActivityStep_ThrowsPatternActivityFailedException()
        {
            // Arrange
            var stepsConfigEntityId = new EntityId(nameof(StepsConfigEntity), Guid.NewGuid().ToString());
            var context = Mock.Of<IDurableOrchestrationContext>();
            var stepExecutorMock = new Mock<IStepExecutor>();
            var steps = new List<StepConfiguration> 
            {
                new StepConfiguration(
                    Guid.NewGuid(),
                    StepType.FanOutFanIn,
                    typeof(SampleActivity),
                    typeof(string),
                    typeof(int),
                    new FanOutFanInOptions(10, 5))
            };

            var failedStepResult = new StepExecutionResult(
                null, TimeSpan.Zero, steps[0].StepId, steps[0].StepType, false, new Exception());
            stepExecutorMock
                .Setup(m => m.ExecuteStepAsync(
                    steps[0], stepsConfigEntityId, context, null))
                .ReturnsAsync(failedStepResult);

            _stepExecutorFactoryMock
                .Setup(m => m.Get(steps[0].StepType))
                .Returns(stepExecutorMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<PatternActivityFailedException>(
                () => _executor.ExecuteAsync(steps, stepsConfigEntityId, context));
        }

        private class SampleActivity : IPatternActivity<string, int>
        {
            public Task<int> RunAsync(string input)
            {
                throw new NotImplementedException();
            }
        }
    }
}
