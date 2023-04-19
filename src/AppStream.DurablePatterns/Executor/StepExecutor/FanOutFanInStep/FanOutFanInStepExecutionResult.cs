using AppStream.DurablePatterns.Builder;
using AppStream.DurablePatterns.StepsConfig;

namespace AppStream.DurablePatterns.Executor.StepExecutor.FanOutFanInStep
{
    internal record FanOutFanInStepExecutionResult(
        object? Result,
        TimeSpan Duration,
        Guid StepId,
        StepType StepType,
        bool Succeeded,
        Exception? Exception,
        FanOutFanInOptions Options,
        TimeSpan? AverageBatchProcessingDuration,
        TimeSpan? AverageItemProcessingDuration,
        int? BatchesProcessed,
        int? ItemsProcessed) : StepExecutionResult(Result, Duration, StepId, StepType, Succeeded, Exception);
}
