using AppStream.DurablePatterns.Steps;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor.StepExecutor
{
    internal interface IStepExecutor
    {
        Task<StepExecutionResult> ExecuteStepAsync(
            Step step,
            EntityId stepsEntityId,
            IDurableOrchestrationContext context,
            object? input);
    }
}
