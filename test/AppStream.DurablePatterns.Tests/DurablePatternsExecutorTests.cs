using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.Executor.StepExecutor;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
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
            _executor = new DurablePatternsExecutor(new Mock<ILogger<DurablePatternsExecutor>>().Object, _stepExecutorFactoryMock.Object);
        }

        [Test]
        public void ExecuteAsync_WithEmptySteps_ThrowsArgumentException()
        {
            // Arrange
            var context = Mock.Of<TaskOrchestrationContext>();
            var steps = new List<Step>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _executor.ExecuteAsync(steps, context));
        }

        [Test]
        public void ExecuteAsync_WithNullSteps_ThrowsArgumentNullException()
        {
            // Arrange
            var context = Mock.Of<TaskOrchestrationContext>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _executor.ExecuteAsync(null!, context));
        }

        [Test]
        public void ExecuteAsync_WithFailedActivityStep_ThrowsPatternActivityFailedException()
        {
            // Arrange
            var context = Mock.Of<TaskOrchestrationContext>();
            var stepExecutorMock = new Mock<IStepExecutor>();
            var steps = new List<Step> 
            {
                new Step(
                    Guid.NewGuid(),
                    StepType.FanOutFanIn,
                    typeof(SampleActivity).AssemblyQualifiedName!,
                    typeof(string).AssemblyQualifiedName!,
                    typeof(int).AssemblyQualifiedName!,
                    new FanOutFanInOptions(10, 5),
                    null)
            };

            var failedStepResult = new StepExecutionResult(
                typeof(SampleActivity).AssemblyQualifiedName!, null, null, TimeSpan.Zero, steps[0].StepId, steps[0].StepType, false, new Exception());
            stepExecutorMock
                .Setup(m => m.ExecuteStepAsync(
                    steps[0], context, null))
                .ReturnsAsync(failedStepResult);

            _stepExecutorFactoryMock
                .Setup(m => m.Get(steps[0].StepType))
                .Returns(stepExecutorMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<PatternActivityFailedException>(
                () => _executor.ExecuteAsync(steps, context));
        }

        private class SampleActivity : IPatternActivity<string, int>
        {
            public Task<PatternActivityResult<int>> RunAsync(string input)
            {
                throw new NotImplementedException();
            }
        }
    }
}
