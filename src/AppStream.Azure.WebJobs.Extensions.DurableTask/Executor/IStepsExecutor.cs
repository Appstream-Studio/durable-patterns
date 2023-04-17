using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal interface IStepsExecutor
    {
        Task<PatternsExecutionResult> ExecuteAsync(IEnumerable<Step> steps, IDurableOrchestrationContext context);
    }
}
