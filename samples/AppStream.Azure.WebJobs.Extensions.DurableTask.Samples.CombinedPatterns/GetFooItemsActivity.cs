﻿using AppStream.DurablePatterns;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    internal class GetFooItemsActivity : IPatternActivity<List<FooItem>>
    {
        private readonly IFooItemRepository _repository;
        private readonly ILogger<GetFooItemsActivity> _logger;

        public GetFooItemsActivity(
            IFooItemRepository repository,
            ILogger<GetFooItemsActivity> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<List<FooItem>> RunAsync(object? input)
        {
            _logger.LogInformation("Getting foo items from repository");
            return _repository.GetFooItemsAsync();
        }
    }
}
