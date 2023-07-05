using AppStream.DurablePatterns.Steps;
using Microsoft.DurableTask;

namespace AppStream.DurablePatterns.Executor
{
    internal interface IDurablePatternsExecutor
    {
        Task<ExecutionResult> ExecuteAsync(
            IEnumerable<Step> steps,
            TaskOrchestrationContext context);
    }
}
