using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.Steps;
using AppStream.DurablePatterns.Steps.Entity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Newtonsoft.Json.Linq;

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
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity),
                typeof(List<string>),
                typeof(List<string>),
                new FanOutFanInOptions(1, 1));

            var context = Mock.Of<IDurableOrchestrationContext>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _executor.ExecuteStepAsync(step, stepsEntityId, context, null));
        }

        [Test]
        public void ExecuteStepAsync_InputIsNotCollection_ThrowsArgumentException()
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity),
                typeof(string),
                typeof(List<string>),
                new FanOutFanInOptions(1, 1));

            var context = Mock.Of<IDurableOrchestrationContext>();
            var input = "not a collection";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _executor.ExecuteStepAsync(step, stepsEntityId, context, input));
        }

        [Test]
        public void ExecuteStepAsync_ResultTypeIsNotCollection_ThrowsArgumentException()
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity),
                typeof(List<string>),
                typeof(string),
                new FanOutFanInOptions(1, 1));

            var context = Mock.Of<IDurableOrchestrationContext>();
            var input = new List<string>();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(
                () => _executor.ExecuteStepAsync(step, stepsEntityId, context, input));
        }

        [Test]
        public async Task ExecuteFanOutFanInInternalAsync_InputsAreValid_CallsActivityFunction()
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity),
                typeof(List<string>),
                typeof(List<string>),
                new FanOutFanInOptions(3, 1));
            var input = new List<string>() { "1", "2", "3" };

            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock
                .Setup(c => c.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.StepId == step.StepId)))
                .ReturnsAsync(new ActivityFunctionResult(
                    JToken.FromObject(new List<string>() { "1", "2", "3" }), 
                    TimeSpan.Zero));

            // Act
            var result = await _executor.ExecuteStepAsync(step, stepsEntityId, contextMock.Object, input);
            
            // Assert
            contextMock.Verify(m => m.CallActivityAsync<ActivityFunctionResult>(
                ActivityFunction.FunctionName, 
                It.Is<ActivityFunctionInput>(i => i.StepId == step.StepId)));
        }

        [Test]
        public async Task ExecuteFanOutFanInInternalAsync_InputsAreValid_CallsActivityFunctionOnceForEachBatch()
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity),
                typeof(List<string>),
                typeof(List<string>),
                new FanOutFanInOptions(2, 1));
            var input = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };

            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock
                .Setup(c => c.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.StepId == step.StepId)))
                .ReturnsAsync((string functionName, ActivityFunctionInput functionInput) => new ActivityFunctionResult(
                    JToken.FromObject(functionInput.Input!),
                    TimeSpan.Zero));

            // Act
            var result = await _executor.ExecuteStepAsync(step, stepsEntityId, contextMock.Object, input);

            // Assert
            contextMock.Verify(
                m => m.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.StepId == step.StepId)), 
                Times.Exactly(4));
        }

        [Test]
        public async Task CombineResults_InputsAreValid_ReturnsCombinedResults()
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var step = new Step(
                Guid.NewGuid(),
                StepType.FanOutFanIn,
                typeof(MyPatternActivity),
                typeof(List<string>),
                typeof(List<string>),
                new FanOutFanInOptions(2, 1));
            var input = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };
            var expectedResult = new List<string>() { "1", "1", "2", "2", "3", "3", "4", "4", "5", "5", "6", "6", "7", "7" };

            var contextMock = new Mock<IDurableOrchestrationContext>();
            contextMock
                .Setup(c => c.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName,
                    It.Is<ActivityFunctionInput>(i => i.StepId == step.StepId)))
                .ReturnsAsync((string functionName, ActivityFunctionInput functionInput) => new ActivityFunctionResult(
                    JToken.FromObject(((List<string>)functionInput.Input!).Concat((List<string>)functionInput.Input!)),
                    TimeSpan.Zero));

            // Act
            var result = await _executor.ExecuteStepAsync(step, stepsEntityId, contextMock.Object, input);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.TypeOf<List<string>>());
                Assert.That((List<string>)result.Result!, Is.EquivalentTo(expectedResult));
            });
        }

        public class MyPatternActivity : IPatternActivity<List<string>, List<string>>
        {
            public Task<List<string>> RunAsync(List<string> input)
            {
                return Task.FromResult(input);
            }
        }
    }
}
