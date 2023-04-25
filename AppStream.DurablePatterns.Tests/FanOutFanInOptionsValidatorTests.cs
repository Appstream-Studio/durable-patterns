using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep.OptionsValidator;
using Moq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class FanOutFanInOptionsValidatorTests
    {
        private FanOutFanInOptionsValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new FanOutFanInOptionsValidator();
        }

        [Test]
        public void Validate_WithValidOptions_DoesNotThrowException()
        {
            // Arrange
            var options = new FanOutFanInOptions(10, 2);

            // Act & Assert
            Assert.DoesNotThrow(() => _validator.Validate(options));
        }

        [Test]
        public void Validate_WithNullOptions_ThrowsFanOutFanInOptionsValidationException()
        {
            // Arrange
            FanOutFanInOptions options = null!;

            // Act & Assert
            var ex = Assert.Throws<FanOutFanInOptionsValidationException>(() => _validator.Validate(options));
            Assert.That(ex.Message, Is.EqualTo("Options cannot be null."));
        }

        [Test]
        public void Validate_WithParallelActivityFunctionsCapLessThanOne_ThrowsFanOutFanInOptionsValidationException()
        {
            // Arrange
            var options = new FanOutFanInOptions(10, 0);

            // Act & Assert
            var ex = Assert.Throws<FanOutFanInOptionsValidationException>(() => _validator.Validate(options));
            Assert.That(ex.Message, Is.EqualTo("ParallelActivityFunctionsCap must be greater than 0."));
        }

        [Test]
        public void Validate_WithBatchSizeLessThanOne_ThrowsFanOutFanInOptionsValidationException()
        {
            // Arrange
            var options = new FanOutFanInOptions(0, 2);

            // Act & Assert
            var ex = Assert.Throws<FanOutFanInOptionsValidationException>(() => _validator.Validate(options));
            Assert.That(ex.Message, Is.EqualTo("BatchSize must be greater than 0."));
        }
    }
}
