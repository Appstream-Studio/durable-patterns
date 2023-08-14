using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities
{
    internal class Monitor : IPatternActivity<FooItem[], MonitorResult>
    {
        private readonly ILogger<Monitor> _logger;

        public Monitor(ILogger<Monitor> logger)
        {
            _logger = logger;
        }

        public Task<PatternActivityResult<MonitorResult>> RunAsync(FooItem[] input)
        {
            var random = new Random();
            var number = random.Next(1, 11);
            var shouldStopMonitor = number % 10 == 0;

            var message = shouldStopMonitor ? "monitor ending" : "next attempt to hit 10 after 3 seconds...";

            var result = new PatternActivityResult<MonitorResult>(
                new MonitorResult { Enough = shouldStopMonitor },
                message);

            _logger.LogInformation("{number} drawn; {message}", number, message);

            return Task.FromResult(result);
        }
    }

    internal class MonitorResult
    {
        public bool Enough { get; set; }
    }
}
