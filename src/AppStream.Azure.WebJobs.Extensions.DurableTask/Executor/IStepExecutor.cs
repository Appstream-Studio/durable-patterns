using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal interface IStepExecutor
    {
        Task<StepResult> ExecuteStepAsync(IDurableOrchestrationContext context, Guid stepId, object? input);
    }
}
