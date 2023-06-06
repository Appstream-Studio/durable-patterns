using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AppStream.DurablePatterns.Steps.Entity
{
    public class StepsEntity : IStepsEntity
    {
        [JsonProperty("steps")]
        public Dictionary<Guid, Step>? Steps { get; set; }

        public Task Set(Dictionary<Guid, Step> steps)
        {
            Steps ??= steps;
            return Task.CompletedTask;
        }

        [FunctionName(nameof(StepsEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext context)
        {
            return context.DispatchAsync<StepsEntity>();
        }
    }
}
