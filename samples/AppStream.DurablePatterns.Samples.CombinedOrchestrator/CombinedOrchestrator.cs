using AppStream.DurablePatterns.Samples.CombinedOrchestrator.Activities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator
{
    public class CombinedOrchestrator
    {
        public const string OrchestratorName = nameof(CombinedOrchestrator);

        private readonly IDurablePatterns _patterns;

        public CombinedOrchestrator(IDurablePatterns patterns)
        {
            _patterns = patterns;
        }

        [Function(OrchestratorName)]
        public Task<ExecutionResult> RunAsync([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            return _patterns
                .WithContext(context)
                .RunActivity<GetFooItems>()
                .FanOutFanIn<FanOut>(new FanOutFanInOptions(
                    BatchSize: 2,
                    ParallelActivityFunctionsCap: 2))
                .RunActivity<FanIn>()
                .ExecuteAsync();
        }
    }
}
