using AppStream.DurablePatterns.Steps;

namespace AppStream.DurablePatterns.Executor.StepExecutor.MonitorStep
{
    internal record MonitorStepExecutionResult(
        string PatternActivityType,
        object? Result,
        object? Output,
        TimeSpan Duration,
        Guid StepId,
        StepType StepType,
        bool Succeeded,
        Exception? Exception,
        TimeSpan Expiry,
        int PollsMade,
        int PollingInterval) : StepExecutionResult(PatternActivityType, Result, Output, Duration, StepId, StepType, Succeeded, Exception);
}
