using AppStream.DurablePatterns.Steps;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor
{
    internal interface IDurablePatternsExecutor
    {
        Task<ExecutionResult> ExecuteAsync(
            IEnumerable<Step> steps, 
            EntityId stepsEntityId,
            IDurableOrchestrationContext context);
    }
}
