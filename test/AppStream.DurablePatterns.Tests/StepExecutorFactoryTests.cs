using AppStream.DurablePatterns.Executor.StepExecutor.ActivityFunctionStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using AppStream.DurablePatterns.Executor.StepExecutorFactory;
using AppStream.DurablePatterns.Steps;
using Moq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class StepExecutorFactoryTests
    {
        private StepExecutorFactory _stepExecutorFactory;
        private Mock<IServiceProvider> _serviceProviderMock;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _stepExecutorFactory = new StepExecutorFactory(_serviceProviderMock.Object);
        }

        [Test]
        public void Get_StepTypeActivityFunction_ReturnsActivityFunctionStepExecutor()
        {
            // Arrange
            var expectedExecutor = new ActivityFunctionStepExecutor();
            _serviceProviderMock.Setup(x => x.GetService(typeof(ActivityFunctionStepExecutor)))
                .Returns(expectedExecutor);

            // Act
            var executor = _stepExecutorFactory.Get(StepType.ActivityFunction);

            // Assert
            Assert.That(executor, Is.EqualTo(expectedExecutor));
        }

        [Test]
        public void Get_StepTypeFanOutFanIn_ReturnsFanOutFanInStepExecutor()
        {
            // Arrange
            var expectedExecutor = new FanOutFanInStepExecutor(new Mock<IFanOutFanInOptionsValidator>().Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(FanOutFanInStepExecutor)))
                .Returns(expectedExecutor);

            // Act
            var executor = _stepExecutorFactory.Get(StepType.FanOutFanIn);

            // Assert
            Assert.That(executor, Is.EqualTo(expectedExecutor));
        }

        [Test]
        public void Get_StepTypeNotSupported_ThrowsStepTypeNotSupportedException()
        {
            // Arrange
            var stepType = (StepType)int.MaxValue;

            // Act & Assert
            Assert.Throws<StepTypeNotSupportedException>(() => _stepExecutorFactory.Get(stepType));
        }

        [Test]
        public void Get_ServiceNotRegistered_ThrowsStepExecutorNotRegisteredException()
        {
            // Arrange
            _serviceProviderMock.Setup(x => x.GetService(typeof(ActivityFunctionStepExecutor)))
                .Returns((ActivityFunctionStepExecutor)null!);

            // Act & Assert
            Assert.Throws<StepExecutorNotRegisteredException>(() => _stepExecutorFactory.Get(StepType.ActivityFunction));
        }
    }
}
