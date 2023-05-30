using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AppStream.DurablePatterns.StepsConfig.Entity
{
    public class StepsConfigEntity : IStepsConfigEntity
    {
        [JsonProperty("steps")]
        public Dictionary<Guid, StepConfiguration>? Steps { get; set; }

        public Task Set(Dictionary<Guid, StepConfiguration> steps)
        {
            Steps ??= steps;
            return Task.CompletedTask;
        }

        [FunctionName(nameof(StepsConfigEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext context)
        {
            return context.DispatchAsync<StepsConfigEntity>();
        }
    }
}
