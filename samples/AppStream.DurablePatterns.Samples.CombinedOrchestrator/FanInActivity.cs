using AppStream.DurablePatterns;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    internal class FanInActivity : IPatternActivity<List<FooItem>, FooItem[]>
    {
        private readonly ILogger<FanInActivity> _logger;

        public FanInActivity(ILogger<FanInActivity> logger)
        {
            _logger = logger;
        }

        public Task<FooItem[]> RunAsync(List<FooItem> input)
        {
            _logger.LogInformation("this block of code is executed in a single activity function");
            foreach (var item in input)
            {
                _logger.LogInformation($"hello {item.Name} from fan in activity");
            }

            _logger.LogInformation("this is the last activity; orchestration finished");
            return Task.FromResult(input.ToArray());
        }
    }
}
