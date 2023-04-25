using AppStream.DurablePatterns.Builder.ContractResolver;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class PatternActivityContractResolverTests
    {
        [Test]
        public void Resolve_WithValidType_ReturnsCorrectContract()
        {
            // Arrange
            var resolver = new PatternActivityContractResolver();

            // Act
            var contract = resolver.Resolve(typeof(SampleActivity));

            // Assert
            Assert.That(contract.InputType, Is.EqualTo(typeof(string)));
            Assert.That(contract.ResultType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Resolve_WithInvalidType_ThrowsException()
        {
            // Arrange
            var resolver = new PatternActivityContractResolver();

            // Act & Assert
            Assert.Throws<PatternActivityContractException>(() => resolver.Resolve(typeof(object)));
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
