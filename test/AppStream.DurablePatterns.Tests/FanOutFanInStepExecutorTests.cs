using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;
using Moq;
using System.Text.Json;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class FanOutFanInStepExecutorTests
    {
        private FanOutFanInStepExecutor _executor;
        private Mock<IFanOutFanInOptionsValidator> _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new Mock<IFanOutFanInOptionsValidator>();
            _executor = new FanOutFanInStepExecutor(_validator.Object);
        }

        [Test]
        public void ExecuteStepAsync_InputIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                new FanOutFanInOptions(1, 1));

            var context = Mock.Of<TaskOrchestrationContext>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _executor.ExecuteStepAsync(step, context, null));
        }

        [Test]
        public void ExecuteStepAsync_InputIsNotCollection_ThrowsArgumentException()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(string).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                new FanOutFanInOptions(1, 1));

            var context = Mock.Of<TaskOrchestrationContext>();
            var input = "not a collection";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _executor.ExecuteStepAsync(step, context, input));
        }

        [Test]
        public void ExecuteStepAsync_ResultTypeIsNotCollection_ThrowsArgumentException()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                typeof(string).AssemblyQualifiedName!,
                new FanOutFanInOptions(1, 1));

            var context = Mock.Of<TaskOrchestrationContext>();
            var input = new List<string>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _executor.ExecuteStepAsync(step, context, input));
        }

        [Test]
        public async Task ExecuteFanOutFanInInternalAsync_InputsAreValid_CallsActivityFunction()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                new FanOutFanInOptions(3, 1));
            var input = new List<string>() { "1", "2", "3" };

            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock
                .Setup(c => c.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.Step.StepId == step.StepId),
                    It.IsAny<TaskOptions?>()))
                .ReturnsAsync(new ActivityFunctionResult(
                    JsonSerializer.SerializeToElement(new List<string>() { "1", "2", "3" }),
                    null,
                    TimeSpan.Zero));

            // Act
            var result = await _executor.ExecuteStepAsync(step, contextMock.Object, input);
            
            // Assert
            contextMock.Verify(m => m.CallActivityAsync<ActivityFunctionResult>(
                ActivityFunction.FunctionName, 
                It.Is<ActivityFunctionInput>(i => i.Step.StepId == step.StepId),
                It.IsAny<TaskOptions?>()));
        }

        [Test]
        public async Task ExecuteFanOutFanInInternalAsync_InputsAreValid_CallsActivityFunctionOnceForEachBatch()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName!,
                new FanOutFanInOptions(2, 1));
            var input = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };

            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock
                .Setup(c => c.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.Step.StepId == step.StepId), 
                    It.IsAny<TaskOptions?>()))
                .ReturnsAsync((TaskName functionName, ActivityFunctionInput functionInput, TaskOptions options) => new ActivityFunctionResult(
                    JsonSerializer.SerializeToElement(functionInput.ActivityInput!),
                    null,
                    TimeSpan.Zero));

            // Act
            var result = await _executor.ExecuteStepAsync(step, contextMock.Object, input);

            // Assert
            contextMock.Verify(
                m => m.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.Step.StepId == step.StepId), 
                    It.IsAny<TaskOptions?>()), 
                Times.Exactly(4));
        }

        [Test]
        public async Task CombineResults_InputsAreValid_ReturnsCombinedResults()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(List<string>).AssemblyQualifiedName! ,
                typeof(List<string>).AssemblyQualifiedName!,
                new FanOutFanInOptions(2, 1));
            var input = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };
            var expectedResult = new List<string>() { "1", "1", "2", "2", "3", "3", "4", "4", "5", "5", "6", "6", "7", "7" };

            var contextMock = new Mock<TaskOrchestrationContext>();
            contextMock
                .Setup(c => c.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.Step.StepId == step.StepId), 
                    It.IsAny<TaskOptions?>()))
                .ReturnsAsync((TaskName functionName, ActivityFunctionInput functionInput, TaskOptions options) => new ActivityFunctionResult(
                    JsonSerializer.SerializeToElement(((List<string>)functionInput.ActivityInput!).Concat((List<string>)functionInput.ActivityInput!)),
                    null,
                    TimeSpan.Zero));

            // Act
            var result = await _executor.ExecuteStepAsync(step, contextMock.Object, input);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<List<string>>());
                Assert.That((List<string>)result.Result!, Is.EquivalentTo(expectedResult));
            });
        }

        public class MyPatternActivity : IPatternActivity<List<string>, List<string>>
        {
            public Task<PatternActivityResult<List<string>>> RunAsync(List<string> input)
            {
                return Task.FromResult(new PatternActivityResult<List<string>>(input, null));
            }
        }
    }
}
