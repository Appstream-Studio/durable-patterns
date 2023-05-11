using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities
{
    internal class GetFooItems : IPatternActivity<List<FooItem>>
    {
        private readonly IFooItemRepository _repository;
        private readonly ILogger<GetFooItems> _logger;

        public GetFooItems(
            IFooItemRepository repository,
            ILogger<GetFooItems> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<List<FooItem>> RunAsync(object? input)
        {
            _logger.LogInformation("getting foo items from repository");
            return _repository.GetFooItemsAsync();
        }
    }
}
