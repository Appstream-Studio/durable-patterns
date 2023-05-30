using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.StepsConfig;
using AppStream.DurablePatterns.StepsConfig.Entity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Newtonsoft.Json.Linq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class ActivityFunctionStepExecutorTests
    {
        private Mock<IDurableOrchestrationContext> _mockContext;
        private ActivityFunctionStepExecutor _stepExecutor;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<IDurableOrchestrationContext>();
            _stepExecutor = new ActivityFunctionStepExecutor();
        }

        [Test]
        public async Task ExecuteStepInternalAsync_ReturnsStepExecutionResult()
        {
            // Arrange
            var stepsConfigEntityId = new EntityId(nameof(StepsConfigEntity), Guid.NewGuid().ToString());
            var step = new StepConfiguration(
                Guid.NewGuid(),
                StepType.ActivityFunction,
                typeof(MyPatternActivity),
                typeof(string),
                typeof(string),
                null);
            var input = "test input";
            var expectedResult = "test result";
            var activityResult = new ActivityFunctionResult(JToken.FromObject(expectedResult), TimeSpan.FromSeconds(1));
            _mockContext.Setup(x => x.CallActivityAsync<ActivityFunctionResult>(ActivityFunction.FunctionName, It.IsAny<ActivityFunctionInput>()))
                .ReturnsAsync(activityResult);

            // Act
            var result = await _stepExecutor.ExecuteStepAsync(step, stepsConfigEntityId, _mockContext.Object, input);

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
            public Task<string> RunAsync(string input)
            {
                return Task.FromResult(input);
            }
        }
    }
}
