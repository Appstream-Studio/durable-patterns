﻿using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.Builder.ContractResolver;
using AppStream.DurablePatterns.Executor;
using AppStream.DurablePatterns.StepsConfig.ConfigurationValidator;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class DurablePatternsTests
    {
        private Builder.DurablePatterns _patterns;
        private Mock<IDurablePatternsExecutor> _executorMock;
        private Mock<IPatternActivityContractResolver> _contractResolverMock;
        private Mock<IStepConfigurationValidator> _stepValidatorMock;
        private Mock<IDurableOrchestrationContext> _contextMock;

        [SetUp]
        public void Setup()
        {
            _executorMock = new Mock<IDurablePatternsExecutor>();
            _contractResolverMock = new Mock<IPatternActivityContractResolver>();
            _contractResolverMock
                .Setup(r => r.Resolve(It.Is<Type>(t => t == typeof(SampleActivity))))
                .Returns(new PatternActivityContract(typeof(string), typeof(int)));

            _stepValidatorMock = new Mock<IStepConfigurationValidator>();
            _contextMock = new Mock<IDurableOrchestrationContext>();

            _patterns = new Builder.DurablePatterns(
                _executorMock.Object,
                _contractResolverMock.Object,
                _stepValidatorMock.Object);
        }

        [Test]
        public void ExecuteAsync_WithNullContext_ThrowsContextNotSetException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ContextNotSetException>(() => _patterns.ExecuteAsync());
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
