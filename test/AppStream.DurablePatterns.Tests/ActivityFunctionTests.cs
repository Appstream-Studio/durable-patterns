using AppStream.DurablePatterns.ActivityFunctions;
using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using AppStream.DurablePatterns.Steps;
using AppStream.DurablePatterns.Steps.Entity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Newtonsoft.Json.Linq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    internal class ActivityFunctionTests
    {
        private Mock<IDurableEntityClient> _mockDurableEntityClient;
        private Mock<IPatternActivityFactory> _mockActivityFactory;
        private ActivityFunction _activityFunction;

        [SetUp]
        public void SetUp()
        {
            _mockDurableEntityClient = new Mock<IDurableEntityClient>();
            _mockActivityFactory = new Mock<IPatternActivityFactory>();
            _mockActivityFactory
                .Setup(f => f.Create<MyPatternActivityInput, MyPatternActivityResult>(It.Is<Type>(t => t == typeof(MyPatternActivity))))
                .Returns(new MyPatternActivity());

            _activityFunction = new ActivityFunction(_mockActivityFactory.Object);
        }

        [Test]
        public async Task RunWorker_WithValidSetup_CallsPatternActivityFactoryCreateWithCorrectGenericArguments()
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var contextMock = new Mock<IDurableActivityContext>();
            var stepId = Guid.NewGuid();
            var patternActivityType = typeof(MyPatternActivity);
            var inputType = typeof(MyPatternActivityInput);
            var resultType = typeof(MyPatternActivityResult);
            var functionInput = new ActivityFunctionInput(stepId, stepsEntityId, Input: null);
            var stepConfiguration = new Step(stepId, StepType.ActivityFunction, patternActivityType, inputType, resultType, null);

            contextMock
                .Setup(c => c.GetInput<ActivityFunctionInput>())
                .Returns(functionInput);
            _mockDurableEntityClient
                .Setup(c => c.ReadEntityStateAsync<StepsEntity>(stepsEntityId, null, null))
                .ReturnsAsync(new EntityStateResponse<StepsEntity>
                {
                    EntityExists = true,
                    EntityState = new StepsEntity
                    {
                        Steps = new Dictionary<Guid, Step>
                        {
                            { stepId, stepConfiguration }
                        }
                    }
                });

            // Act
            await _activityFunction.RunWorker(contextMock.Object, _mockDurableEntityClient.Object);

            // Assert
            _mockActivityFactory.Verify(p => p.Create<MyPatternActivityInput, MyPatternActivityResult>(patternActivityType), Times.Once);
        }

        [Test]
        public async Task RunWorker_WithValidSetup_ReturnsCorrectActivityResult() 
        {
            // Arrange
            var stepsEntityId = new EntityId(nameof(StepsEntity), Guid.NewGuid().ToString());
            var contextMock = new Mock<IDurableActivityContext>();
            var stepId = Guid.NewGuid();
            var patternActivityType = typeof(MyPatternActivity);
            var inputType = typeof(MyPatternActivityInput);
            var resultType = typeof(MyPatternActivityResult);
            var input = new MyPatternActivityInput { Property1 = "value1", Property2 = 42 };
            var serializedInput = JToken.FromObject(input);
            var functionInput = new ActivityFunctionInput(stepId, stepsEntityId, serializedInput);
            var stepConfiguration = new Step(stepId, StepType.ActivityFunction, patternActivityType, inputType, resultType, null);
            var expectedActivityResult = new MyPatternActivityResult { Property3 = "result3", Property4 = true };
            var patternActivity = new MyPatternActivity(expectedActivityResult);

            contextMock
                .Setup(c => c.GetInput<ActivityFunctionInput>())
                .Returns(functionInput);
            _mockActivityFactory
                .Setup(factory => factory.Create<MyPatternActivityInput, MyPatternActivityResult>(typeof(MyPatternActivity)))
                .Returns(patternActivity);
            _mockDurableEntityClient
                .Setup(c => c.ReadEntityStateAsync<StepsEntity>(stepsEntityId, null, null))
                .ReturnsAsync(new EntityStateResponse<StepsEntity>
                {
                    EntityExists = true,
                    EntityState = new StepsEntity
                    {
                        Steps = new Dictionary<Guid, Step>
                        {
                            { stepId, stepConfiguration }
                        }
                    }
                });

            // Act
            var result = await _activityFunction.RunWorker(contextMock.Object, _mockDurableEntityClient.Object);

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
