using AppStream.DurablePatterns.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AppStream.DurablePatterns.Executor
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ExecutionResult
    {
        public ExecutionResult(IEnumerable<StepExecutionResultSummary> steps)
        {
            Steps = steps;
        }

        public IEnumerable<StepExecutionResultSummary> Steps { get; }
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public record StepExecutionResultSummary(
        string Name,
        TimeSpan Duration);

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public record FanOutFanInStepExecutionResultSummary(
        string Name,
        TimeSpan Duration,
        FanOutFanInOptions FanOutFanInOptions,
        TimeSpan? AverageBatchProcessingDuration,
        TimeSpan? AverageItemProcessingDuration,
        int? BatchesProcessed,
        int? ItemsProcessed) : StepExecutionResultSummary(Name, Duration);
}
