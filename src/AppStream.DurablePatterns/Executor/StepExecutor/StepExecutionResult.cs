using AppStream.DurablePatterns.Steps;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal record StepExecutionResult(
        object? Result,
        TimeSpan Duration,
        Guid StepId,
        StepType StepType,
        bool Succeeded,
        Exception? Exception);
}
