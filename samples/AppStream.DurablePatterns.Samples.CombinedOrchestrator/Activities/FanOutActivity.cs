using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities
{
    internal class FanOutActivity : IPatternActivity<List<FooItem>, List<FooItem>>
    {
        private readonly ILogger<FanOutActivity> _logger;

        public FanOutActivity(ILogger<FanOutActivity> logger)
        {
            _logger = logger;
        }

        public Task<List<FooItem>> RunAsync(List<FooItem> input)
        {
            _logger.LogInformation("this block of code is executed in parallel batches");
            foreach (var item in input)
            {
                _logger.LogInformation($"hello {item.Name} from fan out activity");
            }

            return Task.FromResult(input);
        }
    }
}
