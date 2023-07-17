using AppStream.DurablePatterns.Steps;

namespace AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep
{
    internal record FanOutFanInStepExecutionResult(
        string PatternActivityType,
        object? Result,
        object?[] Outputs,
        TimeSpan Duration,
        Guid StepId,
        StepType StepType,
        bool Succeeded,
        Exception? Exception,
        FanOutFanInOptions Options,
        TimeSpan? AverageBatchProcessingDuration,
        TimeSpan? AverageItemProcessingDuration,
        int? BatchesProcessed,
        int? ItemsProcessed) : StepExecutionResult(PatternActivityType, Result, Outputs, Duration, StepId, StepType, Succeeded, Exception);
}
