using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AppStream.DurablePatterns.Samples.CombinedOrchestrator
{
    public class CombinedOrchestratorHttpTrigger
    {
        [Function("CombinedOrchestrator_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger(nameof(HttpStart));
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(CombinedOrchestrator.OrchestratorName);

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
