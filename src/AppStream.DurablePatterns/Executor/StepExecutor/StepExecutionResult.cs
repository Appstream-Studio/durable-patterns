using AppStream.DurablePatterns.Steps;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal record StepExecutionResult(
        string PatternActivityType,
        object? Result,
        object? Output,
        TimeSpan Duration,
        Guid StepId,
        StepType StepType,
        bool Succeeded,
        Exception? Exception);
}
