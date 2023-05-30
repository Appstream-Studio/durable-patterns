using AppStream.DurablePatterns.StepsConfig;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor
{
    internal interface IDurablePatternsExecutor
    {
        Task<ExecutionResult> ExecuteAsync(
            IEnumerable<StepConfiguration> steps, 
            EntityId stepsConfigEntityId,
            IDurableOrchestrationContext context);
    }
}
