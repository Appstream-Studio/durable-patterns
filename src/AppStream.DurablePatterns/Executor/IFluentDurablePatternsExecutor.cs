using AppStream.DurablePatterns.StepsConfig;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.DurablePatterns.Executor
{
    internal interface IFluentDurablePatternsExecutor
    {
        Task<ExecutionResult> ExecuteAsync(IEnumerable<StepConfiguration> steps, IDurableOrchestrationContext context);
    }
}
