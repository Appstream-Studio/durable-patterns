using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;
using Moq;
using System.Text.Json;
using static AppStream.DurablePatterns.Tests.ActivityFunctionTests;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class ActivityFunctionStepExecutorTests
    {
        private Mock<TaskOrchestrationContext> _mockContext;
        private ActivityFunctionStepExecutor _stepExecutor;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<TaskOrchestrationContext>();
            _stepExecutor = new ActivityFunctionStepExecutor();
        }

        [Test]
        public async Task ExecuteStepInternalAsync_ReturnsStepExecutionResult()
        {
            // Arrange
            var step = new Step(
                Guid.NewGuid(),
                StepType.ActivityFunction,
                typeof(MyPatternActivity).AssemblyQualifiedName!,
                typeof(string).AssemblyQualifiedName!,
                typeof(string).AssemblyQualifiedName!,
                null);
            var input = "test input";
            var expectedResult = "test result";
            var activityResult = new ActivityFunctionResult(JsonSerializer.SerializeToElement(expectedResult), null, TimeSpan.FromSeconds(1));
            _mockContext
                .Setup(x => x.CallActivityAsync<ActivityFunctionResult>(
                    ActivityFunction.FunctionName, 
                    It.IsAny<ActivityFunctionInput>(), 
                    It.IsAny<TaskOptions?>()))
                .ReturnsAsync(activityResult);

            // Act
            var result = await _stepExecutor.ExecuteStepAsync(step, _mockContext.Object, input);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.EqualTo(expectedResult));
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Exception, Is.Null);
                Assert.That(result.StepId, Is.EqualTo(step.StepId));
                Assert.That(result.StepType, Is.EqualTo(StepType.ActivityFunction));
            });
        }

        public class MyPatternActivity : IPatternActivity<string, string>
        {
            public Task<PatternActivityResult<string>> RunAsync(string input)
            {
                return Task.FromResult(new PatternActivityResult<string>(input, null));
            }
        }
    }
}
