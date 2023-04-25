using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.StepsConfig;
using AppStream.DurablePatterns.StepsConfig.ConfigurationBag;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Newtonsoft.Json.Linq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    internal class ActivityFunctionTests
    {
        private Mock<IPatternActivityFactory> _mockActivityFactory;
        private Mock<IStepConfigurationBag> _mockStepConfigurationBag;
        private ActivityFunction _activityFunction;

        [SetUp]
        public void SetUp()
        {
            _mockActivityFactory = new Mock<IPatternActivityFactory>();
            _mockActivityFactory
                .Setup(f => f.Create<MyPatternActivityInput, MyPatternActivityResult>(It.Is<Type>(t => t == typeof(MyPatternActivity))))
                .Returns(new MyPatternActivity());

            _mockStepConfigurationBag = new Mock<IStepConfigurationBag>();
            _activityFunction = new ActivityFunction(_mockActivityFactory.Object, _mockStepConfigurationBag.Object);
        }

        [Test]
        public async Task RunWorker_WithValidSetup_CallsPatternActivityFactoryCreateWithCorrectGenericArguments()
        {
            // Arrange
            var contextMock = new Mock<IDurableActivityContext>();
            var stepId = Guid.NewGuid();
            var patternActivityType = typeof(MyPatternActivity);
            var inputType = typeof(MyPatternActivityInput);
            var resultType = typeof(MyPatternActivityResult);
            var functionInput = new ActivityFunctionInput(stepId, null);
            var stepConfiguration = new StepConfiguration(stepId, StepType.ActivityFunction, patternActivityType, inputType, resultType, null);

            contextMock
                .Setup(c => c.GetInput<ActivityFunctionInput>())
                .Returns(functionInput);
            _mockStepConfigurationBag
                .Setup(s => s.Get(stepId))
                .Returns(stepConfiguration);

            // Act
            await _activityFunction.RunWorker(contextMock.Object);

            // Assert
            _mockActivityFactory.Verify(p => p.Create<MyPatternActivityInput, MyPatternActivityResult>(patternActivityType), Times.Once);
        }

        [Test]
        public async Task RunWorker_WithValidSetup_ReturnsCorrectActivityResult() 
        {
            // Arrange
            var contextMock = new Mock<IDurableActivityContext>();
            var stepId = Guid.NewGuid();
            var patternActivityType = typeof(MyPatternActivity);
            var inputType = typeof(MyPatternActivityInput);
            var resultType = typeof(MyPatternActivityResult);
            var input = new MyPatternActivityInput { Property1 = "value1", Property2 = 42 };
            var serializedInput = JToken.FromObject(input);
            var functionInput = new ActivityFunctionInput(stepId, serializedInput);
            var stepConfiguration = new StepConfiguration(stepId, StepType.ActivityFunction, patternActivityType, inputType, resultType, null);
            var expectedActivityResult = new MyPatternActivityResult { Property3 = "result3", Property4 = true };
            var patternActivity = new MyPatternActivity(expectedActivityResult);

            contextMock
                .Setup(c => c.GetInput<ActivityFunctionInput>())
                .Returns(functionInput);
            _mockActivityFactory
                .Setup(factory => factory.Create<MyPatternActivityInput, MyPatternActivityResult>(typeof(MyPatternActivity)))
                .Returns(patternActivity);
            _mockStepConfigurationBag
                .Setup(s => s.Get(stepId))
                .Returns(stepConfiguration);

            // Act
            var result = await _activityFunction.RunWorker(contextMock.Object);

            // Assert
            Assert.That(result.ActivityResult, Is.EqualTo(expectedActivityResult));
        }

        public class MyPatternActivityInput
        {
            public string? Property1 { get; internal set; }
            public int Property2 { get; internal set; }
        }

        public class MyPatternActivityResult
        {
            public string? Property3 { get; internal set; }
            public bool Property4 { get; internal set; }
        }

        public class MyPatternActivity : IPatternActivity<MyPatternActivityInput, MyPatternActivityResult>
        {
            private readonly MyPatternActivityResult? _result;

            public MyPatternActivity()
            {
            }

            public MyPatternActivity(MyPatternActivityResult? result)
            {
                _result = result;
            }

            public Task<MyPatternActivityResult> RunAsync(MyPatternActivityInput input)
            {
                return Task.FromResult(_result ?? new MyPatternActivityResult());
            }
        }
    }
}
