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
            Assert.Throws<ArgumentNullException>(() => _patternActivityFactory.Create<int, string>(null!));
        }

        [Test]
        public void Create_WithInvalidPatternActivityType_ThrowsUnexpectedPatternActivityTypeException()
        {
            Assert.Throws<UnexpectedPatternActivityTypeException>(() => _patternActivityFactory.Create<int, string>(typeof(string)));
        }

        [Test]
        public void Create_WithUnregisteredPatternActivityType_ThrowsActivityNotRegisteredException()
        {
            Assert.Throws<ActivityNotRegisteredException>(() => _patternActivityFactory.Create<double, int>(typeof(DoubleToIntActivity)));
        }

        [Test]
        public void Create_WithValidPatternActivityType_ReturnsPatternActivity()
        {
            var patternActivity = _patternActivityFactory.Create<int, string>(typeof(IntToStringActivity));
            Assert.That(patternActivity, Is.Not.Null);
            Assert.That(patternActivity, Is.InstanceOf<IPatternActivity<int, string>>());
        }
    }

    internal class IntToStringActivity : IPatternActivity<int, string>
    {
        public async Task<string> RunAsync(int input)
        {
            return await Task.FromResult(input.ToString());
        }
    }

    internal class StringToIntActivity : IPatternActivity<string, int>
    {
        public async Task<int> RunAsync(string input)
        {
            return await Task.FromResult(int.Parse(input));
        }
    }

    internal class DoubleToIntActivity : IPatternActivity<double, int>
    {
        public async Task<int> RunAsync(double input)
        {
            return await Task.FromResult(Convert.ToInt32(input));
        }
    }
}
