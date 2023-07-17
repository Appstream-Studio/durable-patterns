using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities
{
    internal class FanOut : IPatternActivity<List<FooItem>, List<FooItem>>
    {
        private readonly ILogger<FanOut> _logger;

        public FanOut(ILogger<FanOut> logger)
        {
            _logger = logger;
        }

        public Task<PatternActivityResult<List<FooItem>>> RunAsync(List<FooItem> batch)
        {
            _logger.LogInformation("this block of code is executed in parallel batches");
            foreach (var item in batch)
            {
                _logger.LogInformation("\thello {itemName} from fan out activity", item.Name);
            }

            return Task.FromResult(new PatternActivityResult<List<FooItem>>(
                batch,
                new { itemsProcessedCount = batch.Count }));
        }
    }
}
