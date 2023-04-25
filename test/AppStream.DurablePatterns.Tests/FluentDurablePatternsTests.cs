using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.StepsConfig;
using AppStream.DurablePatterns.StepsConfig.ConfigurationBag;
using AppStream.DurablePatterns.StepsConfig.ConfigurationValidator;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class FluentDurablePatternsTests
    {
        private FluentDurablePatterns _patterns;
        private Mock<IFluentDurablePatternsExecutor> _executorMock;
        private Mock<IPatternActivityContractResolver> _contractResolverMock;
        private Mock<IStepConfigurationBag> _stepConfigurationBagMock;
        private Mock<IStepConfigurationValidator> _stepValidatorMock;
        private Mock<IDurableOrchestrationContext> _contextMock;

        [SetUp]
        public void Setup()
        {
            _executorMock = new Mock<IFluentDurablePatternsExecutor>();
            _contractResolverMock = new Mock<IPatternActivityContractResolver>();
            _contractResolverMock
                .Setup(r => r.Resolve(It.Is<Type>(t => t == typeof(SampleActivity))))
                .Returns(new PatternActivityContract(typeof(string), typeof(int)));

            _stepConfigurationBagMock = new Mock<IStepConfigurationBag>();
            _stepValidatorMock = new Mock<IStepConfigurationValidator>();
            _contextMock = new Mock<IDurableOrchestrationContext>();

            _patterns = new FluentDurablePatterns(
                _executorMock.Object,
                _contractResolverMock.Object,
                _stepConfigurationBagMock.Object,
                _stepValidatorMock.Object);
        }

        [Test]
        public void ExecuteAsync_WithNullContext_ThrowsContextNotSetException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ContextNotSetException>(() => _patterns.ExecuteAsync());
        }

        [Test]
        public void RunActivity_WhenContextIsNotReplaying_AddsStepConfigurationToStepsConfigurationBag()
        {
            // Arrange
            _contextMock
                .Setup(c => c.IsReplaying)
                .Returns(false);

            // Act
            _patterns
                .WithContext(_contextMock.Object)
                .RunActivity<SampleActivity>();

            // Assert
            _stepConfigurationBagMock.Verify(b => b.Add(It.Is<StepConfiguration>(sc => sc.PatternActivityType == typeof(SampleActivity))));
        }

        [Test]
        public void RunActivity_WhenContextIsReplaying_AddsStepConfigurationToStepsConfigurationBag()
        {
            // Arrange
            _contextMock
                .Setup(c => c.IsReplaying)
                .Returns(true);

            // Act
            _patterns
                .WithContext(_contextMock.Object)
                .RunActivity<SampleActivity>();

            // Assert
            _stepConfigurationBagMock.Verify(b => b.Add(It.IsAny<StepConfiguration>()), Times.Never);
        }

        [Test]
        public void FanOutFanIn_WhenContextIsNotReplaying_AddsStepConfigurationToStepsConfigurationBag()
        {
            // Arrange
            _contextMock
                .Setup(c => c.IsReplaying)
                .Returns(false);

            // Act
            _patterns
                .WithContext(_contextMock.Object)
                .FanOutFanIn<SampleActivity>(new FanOutFanInOptions(1, 1));

            // Assert
            _stepConfigurationBagMock.Verify(b => b.Add(It.Is<StepConfiguration>(sc => sc.PatternActivityType == typeof(SampleActivity))));
        }

        [Test]
        public void FanOutFanIn_WhenContextIsReplaying_AddsStepConfigurationToStepsConfigurationBag()
        {
            // Arrange
            _contextMock
                .Setup(c => c.IsReplaying)
                .Returns(true);

            // Act
            _patterns
                .WithContext(_contextMock.Object)
                .FanOutFanIn<SampleActivity>(new FanOutFanInOptions(1, 1));

            // Assert
            _stepConfigurationBagMock.Verify(b => b.Add(It.IsAny<StepConfiguration>()), Times.Never);
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
