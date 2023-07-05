using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal interface IStepExecutor
    {
        Task<StepExecutionResult> ExecuteStepAsync(
            Step step,
            TaskOrchestrationContext context,
            object? input);
    }
}
