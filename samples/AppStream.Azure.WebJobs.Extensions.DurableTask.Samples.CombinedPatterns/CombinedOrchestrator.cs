using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<PatternsExecutionResult> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var executionResult = await _patterns
                .WithContext(context)
                .RunActivity((IFooItemRepository repository) => repository.GetFooItemsAsync())
                .WithEnumerableResults<FooItem>()
                .FanOutFanIn(
                    (items) =>
                    {
                        Console.WriteLine("this block of code is executed in parallel batches");
                        foreach (var item in items)
                        {
                            Console.WriteLine($"hello {item}");
                        }

                        return items;
                    },
                    new FanOutFanInOptions())
                .RunActivity(
                    (items) =>
                    {
                        Console.WriteLine("this block of code is executed in a single activity function");
                        foreach (var item in items)
                        {
                            Console.WriteLine($"hello {item}");
                        }

                        return items.ToArray();
                    })
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