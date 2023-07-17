using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities
{
    internal class FanIn : IPatternActivity<List<FooItem>, FooItem[]>
    {
        private readonly ILogger<FanIn> _logger;

        public FanIn(ILogger<FanIn> logger)
        {
            _logger = logger;
        }

        public Task<PatternActivityResult<FooItem[]>> RunAsync(List<FooItem> allItems)
        {
            _logger.LogInformation("this block of code is executed in a single activity function");
            foreach (var item in allItems)
            {
                _logger.LogInformation("\thello {itemName} from fan in activity", item.Name);
            }

            _logger.LogInformation("this is the last activity; orchestration finished");
            return Task.FromResult(new PatternActivityResult<FooItem[]>(
                allItems.ToArray(),
                new { itemsProcessedCount = allItems.Count }));
        }
    }
}
