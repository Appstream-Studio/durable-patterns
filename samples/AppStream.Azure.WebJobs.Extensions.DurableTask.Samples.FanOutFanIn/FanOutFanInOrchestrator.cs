using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Samples.FanInFanOut
{
    internal class FanOutFanInOrchestrator
    {
        private readonly IFanOutFanIn _fanInFanOut;

        public FanOutFanInOrchestrator(
            IFanOutFanIn fanInFanOut)
        {
            _fanInFanOut = fanInFanOut;
        }

        [FunctionName("Orchestrator")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var result = await _fanInFanOut.FanInFanOutAsync(
                context: context,
                items: new[] { new FooItem("1"), new FooItem("2"), new FooItem("3"), new FooItem("4") },
                activity: (IEnumerable<FooItem> items, IDependency dependency) =>
                {
                    foreach (var i in items)
                    {
                        dependency.DoStuff(i);
                    }

                    return Task.FromResult(items.Select(i => i.Name).ToList());
                },
                options: new FanOutFanInOptions
                {
                    MaxBatchSize = 2,
                    MaxParallelFunctions = 2,
                    UpdateOrchestrationStatus = true
                });

            Console.WriteLine("results:");
            foreach (var resultItem in result.Results)
            {
                Console.WriteLine($"duration: {resultItem.Duration} results:");
                foreach (var item in resultItem.Result)
                {
                    Console.WriteLine($"\t{item}");
                }
            }
            Console.WriteLine($"total duration: {result.Duration}");
        }

        [FunctionName("Orchestrator_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("Orchestrator", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
