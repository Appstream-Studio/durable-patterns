using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.Executor;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.CombinedPatterns
{
    public class CombinedOrchestrator
    {
        private readonly IFluentDurablePatterns _patterns;

        public CombinedOrchestrator(IFluentDurablePatterns patterns)
        {
            _patterns = patterns;
        }

        [FunctionName("CombinedOrchestrator")]
        public async Task<ExecutionResult> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var executionResult = await _patterns
                .WithContext(context)
                .RunActivity<GetFooItemsActivity>()
                .FanOutFanIn<FanOutActivity>(new FanOutFanInOptions(
                    BatchSize: 2, 
                    ParallelActivityFunctionsCap: 2))
                .RunActivity<FanInActivity>()
                .ExecuteAsync();

            return executionResult;
        }

        [FunctionName("CombinedOrchestrator_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("CombinedOrchestrator", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
