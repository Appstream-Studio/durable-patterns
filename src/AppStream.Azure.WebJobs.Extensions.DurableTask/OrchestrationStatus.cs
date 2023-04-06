using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class OrchestrationStatus
    {
        public int TotalItems { get; set; }
        public int TotalBatches { get; set; }
        public int BatchesInQueue { get; set; }
        public int BatchesInProcess { get; set; }
        public int BatchesProcessed { get; set; }
    }
}
