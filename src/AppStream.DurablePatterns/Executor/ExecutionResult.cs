namespace AppStream.DurablePatterns
{
    public class ExecutionResult
    {
        public ExecutionResult(IEnumerable<StepExecutionResultSummary> steps)
        {
            Steps = steps;
        }

        public IEnumerable<StepExecutionResultSummary> Steps { get; }
    }

    public record StepExecutionResultSummary(
        string Name,
        TimeSpan Duration);

    public record FanOutFanInStepExecutionResultSummary(
        string Name,
        TimeSpan Duration,
        FanOutFanInOptions FanOutFanInOptions,
        TimeSpan? AverageBatchProcessingDuration,
        TimeSpan? AverageItemProcessingDuration,
        int? BatchesProcessed,
        int? ItemsProcessed) : StepExecutionResultSummary(Name, Duration);
}
