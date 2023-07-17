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
        string StepType,
        string ActivityType,
        object? Output,
        TimeSpan Duration);

    public record FanOutFanInStepExecutionResultSummary(
        string StepType,
        string ActivityType,
        object? Output,
        TimeSpan Duration,
        FanOutFanInOptions FanOutFanInOptions,
        TimeSpan? AverageBatchProcessingDuration,
        TimeSpan? AverageItemProcessingDuration,
        int? BatchesProcessed,
        int? ItemsProcessed) : StepExecutionResultSummary(StepType, ActivityType, Output, Duration);
}
