using AppStream.DurablePatterns.ActivityFunctions.PatternActivityFactory;
using Microsoft.Extensions.DependencyInjection;

namespace AppStream.DurablePatterns.Tests
{
    [TestFixture]
    public class PatternActivityFactoryTests
    {
        private IServiceProvider _serviceProvider;
        private PatternActivityFactory _patternActivityFactory;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddTransient<IntToStringActivity>();
            services.AddTransient<StringToIntActivity>();

            _serviceProvider = services.BuildServiceProvider();
            _patternActivityFactory = new PatternActivityFactory(_serviceProvider);
        }

        [Test]
        public void Create_WithNullPatternActivityType_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _patternActivityFactory.Create<int, string>(null!));
        }

        [Test]
        public void Create_WithInvalidPatternActivityType_ThrowsUnexpectedPatternActivityTypeException()
        {
            // Act & Assert
            Assert.Throws<UnexpectedPatternActivityTypeException>(() => _patternActivityFactory.Create<int, string>(typeof(string)));
        }

        [Test]
        public void Create_WithUnregisteredPatternActivityType_ThrowsActivityNotRegisteredException()
        {
            // Act & Assert
            Assert.Throws<ActivityNotRegisteredException>(() => _patternActivityFactory.Create<double, int>(typeof(DoubleToIntActivity)));
        }

        [Test]
        public void Create_WithValidPatternActivityType_ReturnsPatternActivity()
        {
            // Act
            var patternActivity = _patternActivityFactory.Create<int, string>(typeof(IntToStringActivity));

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(patternActivity, Is.Not.Null);
                Assert.That(patternActivity, Is.InstanceOf<IPatternActivity<int, string>>());
            });
        }
    }

    internal class IntToStringActivity : IPatternActivity<int, string>
    {
        public Task<PatternActivityResult<string>> RunAsync(int input)
        {
            return Task.FromResult(new PatternActivityResult<string>(input.ToString(), null));
        }
    }

    internal class StringToIntActivity : IPatternActivity<string, int>
    {
        public Task<PatternActivityResult<int>> RunAsync(string input)
        {
            return Task.FromResult(new PatternActivityResult<int>(int.Parse(input), null));
        }
    }

    internal class DoubleToIntActivity : IPatternActivity<double, int>
    {
        public Task<PatternActivityResult<int>> RunAsync(double input)
        {
            return Task.FromResult(new PatternActivityResult<int>(Convert.ToInt32(input), null));
        }
    }
}
