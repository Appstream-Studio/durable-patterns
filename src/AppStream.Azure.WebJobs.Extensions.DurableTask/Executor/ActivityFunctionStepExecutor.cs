using AppStream.Azure.WebJobs.Extensions.DurableTask.SingleItemWorkerFunction;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AppStream.Azure.WebJobs.Extensions.DurableTask.Executor
{
    internal class ActivityFunctionStepExecutor : IStepExecutor
    {
        public async Task<StepResult> ExecuteStepAsync(
            IDurableOrchestrationContext context,
            Guid stepId,
            object? input)
        {
            var started = context.CurrentUtcDateTime;

            // status?

            var result = await context.CallActivityAsync<SingleItemWorkerResult>(
                SingleItemWorkerFunction.SingleItemWorkerFunction.FunctionName,
                new SingleItemWorkerInput(stepId, input));

            // status?

            return new StepResult
            {
                Duration = context.CurrentUtcDateTime - started,
                Result = result.ActivityResult,
                StepId = stepId,
                StepType = StepType.ActivityFunction
            };
        }
    }
}
